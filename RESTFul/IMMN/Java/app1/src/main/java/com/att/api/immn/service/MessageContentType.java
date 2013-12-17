package com.att.api.immn.service;

public enum MessageContentType {
    TEXT("TEXT"), IMAGE("IMAGE"), AUDIO("AUDIO"), VIDEO("VIDEO");
    
    private final String str;

    private MessageContentType(String str) {
        this.str = str;
    }

    public static MessageContentType fromString(String str) {
        final MessageContentType[] types = MessageContentType.values();

        for (int i = 0; i < types.length; ++i) {
            MessageContentType type = types[i];
            if (type.str.equalsIgnoreCase(str)) {
                return types[i];
            }
        }

        return null;
    }

    public String getString() {
        return str;
    }
}
