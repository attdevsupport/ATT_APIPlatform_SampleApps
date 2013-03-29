package com.att.api.mms.file;

public class ImageEntry {
    private final String imgPath;
    private final String txt;
    private final String date;
    private final String senderAddr;

    public ImageEntry(String imgPath, String txt, String date, 
            String senderAddr) {
        this.imgPath = imgPath;
        this.txt = txt;
        this.date = date;
        this.senderAddr = senderAddr;
    }

    public String getImagePath() {
        return this.imgPath;
    }

    public String getText() {
        return this.txt;
    }

    public String getDate() {
        return this.date;
    }

    public String getSenderAddress() {
        return this.senderAddr;
    }
}
