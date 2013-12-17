package com.att.api.immn.service;

import org.json.JSONObject;

public final class DeltaChange {
    private final String messageId;
    private final Boolean isFavorite;
    private final Boolean isUnread;

    public DeltaChange(String messageId, Boolean isFavorite, Boolean isUnread) {
        this.messageId = messageId;
        this.isFavorite = isFavorite;
        this.isUnread = isUnread;
    }

    public String getMessageId() {
        return messageId;
    }

    // alias for isFavorite
    public Boolean getFavorite() {
        return isFavorite;
    }

    public Boolean isFavorite() {
        return isFavorite;
    }

    // alias for isUnread
    public Boolean getUnread() {
        return isUnread;
    }

    public Boolean isUnread() {
        return isUnread;
    }

    public static DeltaChange valueOf(JSONObject jobj) {
        String msgId = jobj.getString("messageId");
        Boolean isFavorite = jobj.getBoolean("isFavorite");
        Boolean isUnread = jobj.getBoolean("isUnread");

        return new DeltaChange(msgId, isFavorite, isUnread);
    }
}
