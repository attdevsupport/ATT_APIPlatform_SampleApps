package com.att.api.immn.service;

public final class MessageContent {
    private final String contentType;
    private final String contentLength;
    private final String content;

    public MessageContent(String ctype, String clength, String content) {
        this.contentType = ctype;
        this.contentLength = clength;
        this.content = content;
    }

    public String getContentType() {
        return contentType;
    }

    public String getContentLength() {
        return contentLength;
    }

    public String getContent() {
        return content;
    }
}
