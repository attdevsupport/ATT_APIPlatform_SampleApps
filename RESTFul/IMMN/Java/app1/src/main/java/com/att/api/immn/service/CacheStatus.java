package com.att.api.immn.service;

public enum CacheStatus {
    NOT_INITIALIZED("NOT_INITIALIZED"), 
    INITIALIZING("INITIALIZING"),
    INITIALIZED("INITIALIZED"),
    ERROR("ERROR");

    private final String str;

    private CacheStatus(final String str) {
        this.str = str;
    }

    public static CacheStatus fromString(String str) {
        final CacheStatus[] statuses = CacheStatus.values();
        for (CacheStatus status : statuses) {
            if (status.getString().equals(str)) {
                return status;
            }
        }

        return null;
    }

    public String getString() {
        return str;
    }

}
