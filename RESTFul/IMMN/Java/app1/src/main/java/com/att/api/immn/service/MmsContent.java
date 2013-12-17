package com.att.api.immn.service;

import org.json.JSONObject;

public final class MmsContent {
    private final String contentName;
    private final String contentType;
    private final String contentUrl;
    private final MessageContentType type;

    public MmsContent(String cname, String ctype, String curl,
            MessageContentType type) {

        this.contentName = cname;
        this.contentType = ctype;
        this.contentUrl = curl;
        this.type = type;
    }

    public String getContentName() {
        return contentName;
    }

    public String getContentType() {
        return contentType;
    }

    public String getContentUrl() {
        return contentUrl;
    }

    public MessageContentType getType() {
        return type;
    }

    public static MmsContent valueOf(JSONObject jobj) {
        final String cname = jobj.getString("contentName");
        final String ctype = jobj.getString("contentType");
        final String curl = jobj.getString("contentUrl");
        
        final String typestr = jobj.getString("type");
        final MessageContentType type = MessageContentType.fromString(typestr);

        return new MmsContent(cname, ctype, curl, type);
    }

}
