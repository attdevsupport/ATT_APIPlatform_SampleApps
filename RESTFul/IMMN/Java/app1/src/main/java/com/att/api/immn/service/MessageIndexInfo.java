package com.att.api.immn.service;

import org.json.JSONObject;

public final class MessageIndexInfo {

    private String contentType;
    private int contentLength;
    private CacheStatus status;
    private String state;
    private int messageCount;

    private MessageIndexInfo() {
    }

    public static MessageIndexInfo valueOf(JSONObject jobj) {
        MessageIndexInfo info = new MessageIndexInfo();

        JSONObject jmsgIndexInfo = jobj.getJSONObject("messageIndexInfo");
        
        info.status = CacheStatus.fromString(jmsgIndexInfo.getString("status"));
        info.state = jmsgIndexInfo.getString("state");
        info.messageCount = jmsgIndexInfo.getInt("messageCount");

        return info;
    }

    public String getContentType() {
        return contentType;
    }

    public int getContentLength() {
        return contentLength;
    }

    public CacheStatus getStatus() {
        return status;
    }

    public String getState() {
        return state;
    }

    public int getMessageCount() {
        return messageCount;
    }

}
