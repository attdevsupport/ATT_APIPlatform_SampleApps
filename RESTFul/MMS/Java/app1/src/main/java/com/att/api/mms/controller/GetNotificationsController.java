package com.att.api.mms.controller;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.mms.file.ImageEntry;
import com.att.api.mms.file.ImageFileHandler;
import com.att.api.mms.model.MMSDeliveryInfo;
import com.att.api.mms.service.MMSFileUtil;

public class GetNotificationsController extends APIController {
    private static final long serialVersionUID = 1L;

    private String getPath() {
        return getServletContext().getRealPath("/") + "MMSImages";
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        JSONObject jresponse = new JSONObject();

        // Update statuses
        JSONArray statuses = new JSONArray();
        MMSDeliveryInfo[] infos = MMSFileUtil.getStatuses();
        for (MMSDeliveryInfo info : infos) {
            statuses.put(info.getMessageId())
                .put(info.getAddress())
                .put(info.getDeliveryStatus());
        }
        jresponse.put("statusNotifications", new JSONArray().put(statuses));

        // Update messages
        JSONArray messages = new JSONArray();
        String path = this.getPath() + "/mmslistener.db";
        List<ImageEntry> entries = new ImageFileHandler(path).getImageEntrys();
        for (ImageEntry image : entries) {
            messages.put(new JSONObject()
                    .put("id", image.getId())
                    .put("date", image.getDate())
                    .put("address", image.getSenderAddress())
                    .put("text", image.getText())
                    .put("image", image.getImagePath()));
        }
        jresponse.put("mmsNotifications", messages);

        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();
        writer.print(jresponse);
        writer.flush();
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
