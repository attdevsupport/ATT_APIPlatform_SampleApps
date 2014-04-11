package com.att.api.immn.service;

public final class MessageContent {
    private final String contentType;
    private final String contentLength;
    private final byte[] content;

    public MessageContent(String ctype, String clength, byte[] content) {
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

    public byte[] getContent() {
        return content;
    }
}
