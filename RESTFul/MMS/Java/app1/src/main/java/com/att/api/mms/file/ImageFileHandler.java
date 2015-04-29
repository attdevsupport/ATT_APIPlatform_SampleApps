package com.att.api.mms.file;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.channels.FileLock;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class ImageFileHandler {
    private String savePath;
    private int limit;

    public ImageFileHandler(String savePath, int limit) {
        this.savePath = savePath;
        this.limit = limit;
    }

    public ImageFileHandler(String savePath) {
        this(savePath, 100);
    }

    public void addImageEntry(ImageEntry entry) throws IOException {
            List<ImageEntry> entries = this.getImageEntrys();
            if (entries.size() >= this.limit) {
                ImageEntry toRemove = entries.remove(0);
                new File(toRemove.getImagePath()).delete();
            }
            entries.add(entry);

            FileOutputStream fOutputStream = null;
            FileLock fLock = null;

            try {
                fOutputStream = new FileOutputStream(this.savePath);
                fLock = fOutputStream.getChannel().lock();
                Properties props = new Properties();
                for (int i = 0; i < entries.size(); ++i) {
                    ImageEntry lentry = entries.get(i);
                    props.setProperty("imgPath" + i, lentry.getImagePath());
                    props.setProperty("text" + i, lentry.getText());
                    props.setProperty("date" + i, lentry.getDate());
                    props.setProperty("senderAddr" + i, 
                            lentry.getSenderAddress());
                    props.setProperty("id" + i, lentry.getId());
                }
                props.store(fOutputStream, "Image Information");
            } catch (IOException ioe) {
                throw ioe;
            } finally {
                if (fLock != null) fLock.release();
                if (fOutputStream != null) fOutputStream.close();
            }
        }

    public List<ImageEntry> getImageEntrys() throws IOException {
        FileInputStream fInputStream = null;
        FileLock fLock = null;

        try {
            if (!new File(this.savePath).exists()) {
                return new ArrayList<ImageEntry>();
            }
            
            fInputStream = new FileInputStream(this.savePath);
            fLock = fInputStream.getChannel().lock(0L, Long.MAX_VALUE, true);
            Properties props = new Properties();
            props.load(fInputStream);

            List<ImageEntry> entries = new ArrayList<ImageEntry>();
            for (int i = 0; i < this.limit; ++i) {
                String imgPath = props.getProperty("imgPath" + i);
                if (imgPath == null) {
                    break;
                }
                ImageEntry entry = new ImageEntry(
                        imgPath,
                        props.getProperty("text" + i),
                        props.getProperty("date" + i),
                        props.getProperty("senderAddr" + i),
                        props.getProperty("id" + i)
                        );
                entries.add(entry);
            }
            return entries;
        } catch (IOException e) {
            throw e;
        } finally {
            if (fLock != null) fLock.release();
            if (fInputStream != null) fInputStream.close();
        }
    }
}

