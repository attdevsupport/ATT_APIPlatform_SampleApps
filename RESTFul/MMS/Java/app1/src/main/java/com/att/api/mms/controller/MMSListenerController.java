package com.att.api.mms.controller;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.commons.codec.binary.Base64;

import com.att.api.mms.file.ImageEntry;
import com.att.api.mms.file.ImageFileHandler;
import com.att.api.util.DateUtil;

public class MMSListenerController extends HttpServlet {

    private static final long serialVersionUID = 3936098408570735112L;

    private String getPath() {
        return getServletContext().getRealPath("/") + "MMSImages";
    }

    private void createDirectory() {
        String dir = this.getPath();
        File fDir = new File(dir);
        if (!fDir.exists()) {
            fDir.mkdir();
        }
    }

    private String readData(HttpServletRequest request) throws IOException {
        InputStream iStream = request.getInputStream();
        ByteArrayOutputStream bStream = new ByteArrayOutputStream();
        byte buffer[] = new byte[1024];

        int bytesRead;
        while ((bytesRead = iStream.read(buffer)) > 0) {
            bStream.write(buffer, 0, bytesRead);
        }

        return new String(bStream.toByteArray());
    }

    private void parseData(String data) throws IOException {
        String senderAddress = data.split("<SenderAddress>tel:")[1]
            .split("</SenderAddress>")[0].substring(2).replaceFirst("...$", "***");

        String date = DateUtil.getTime();

        String[] parts = data.split("--Nokia-mm-messageHandler-BoUnDaRy");
        String[] lowerParts = parts[3].split("BASE64");

        byte[] outFile = Base64.decodeBase64(lowerParts[1]);

        String imgName = null;
        synchronized (ImageEntry.fileCountLock) {
            imgName = "" + ImageEntry.fileCount;
            ImageEntry.fileCount += 1;
        }
        String savePath = this.getPath() + "/" + imgName;

        FileOutputStream fos = new FileOutputStream(savePath);
        fos.write(outFile);
        fos.close();
        String decodedText = "";
        if (parts.length > 4) {
            String textPart = parts[2].split("BASE64")[1];
            decodedText = new String(Base64.decodeBase64(textPart));
            decodedText = decodedText.trim();
        }

        String imgPath = "MMSImages/" + imgName;
        ImageEntry entry = new ImageEntry(imgPath, decodedText, date,
                senderAddress, imgName);

        new ImageFileHandler(this.getPath() + "/mmslistener.db")
            .addImageEntry(entry);
    }
    
    public void doPost(HttpServletRequest request, 
            HttpServletResponse response) throws IOException {
        this.createDirectory();
        String data = this.readData(request);
        this.parseData(data);
    }
}
