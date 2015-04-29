package com.att.api.mms.file;

public class ImageEntry {
    public final static Object fileCountLock = new Object();
    public static long fileCount = 0;

    private final String id;
    private final String imgPath;
    private final String txt;
    private final String date;
    private final String senderAddr;

    public ImageEntry(String imgPath, String txt, String date, 
            String senderAddr, String id) {
        this.imgPath = imgPath;
        this.txt = txt;
        this.date = date;
        this.senderAddr = senderAddr;
        this.id = id;
    }

    public String getImagePath() {
        return this.imgPath;
    }

    public String getText() {
        return this.txt;
    }

    /**
     * @return the id
     */
    public String getId() {
        return id;
    }

    public String getDate() {
        return this.date;
    }

    public String getSenderAddress() {
        return this.senderAddr;
    }
}
