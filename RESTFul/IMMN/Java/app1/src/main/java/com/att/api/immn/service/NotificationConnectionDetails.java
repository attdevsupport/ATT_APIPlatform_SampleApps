package com.att.api.immn.service;

import org.json.JSONObject;

public final class NotificationConnectionDetails {
    private String contentType;
    private int contentLength;
    private String username;
    private String password;
    private String httpsUrl;
    private String wssUrl;
    private String queues;

    private NotificationConnectionDetails() {
    }

    public String getContentType() {
        return contentType;
    }

    public int getContentLength() {
        return contentLength;
    }

    public String getUsername() {
        return username;
    }

    public String getPassword() {
        return password;
    }

    public String getHttpsUrl() {
        return httpsUrl;
    }

    public String getWssUrl() {
        return wssUrl;
    }

    public String getQueues() {
        return queues;
    }

    public static NotificationConnectionDetails valueOf(JSONObject jobj) {
        final NotificationConnectionDetails details 
            = new NotificationConnectionDetails();

        JSONObject jdetails 
            = jobj.getJSONObject("notificationConnectionDetails");

        details.username = jdetails.getString("username");
        details.password = jdetails.getString("password");
        details.httpsUrl = jdetails.getString("httpsUrl");
        details.wssUrl = jdetails.getString("wssUrl");

        JSONObject queues = jdetails.getJSONObject("queues");
        if (queues.has("text"))
            details.queues = queues.getString("text");
        if (queues.has("mms"))
            details.queues = queues.getString("mms");

        return details;
    }

}
