package com.att.api.immn.service;

public enum MessageType {
    SMS("SMS"), MMS("MMS");

    private final String val;

    private MessageType(String val) {
        this.val = val;
    }

    public String getString() {
        return this.val;
    }

}
