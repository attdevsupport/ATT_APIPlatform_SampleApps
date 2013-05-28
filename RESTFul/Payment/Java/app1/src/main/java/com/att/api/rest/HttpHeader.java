package com.att.api.rest;

public class HttpHeader {
    private final String name;
    private final String value;

    public HttpHeader(final String name, final String value) {
        if (name == null) {
            throw new IllegalArgumentException("Name may not be null.");
        }

        this.name = name;
        this.value = value;
    }

    public String getName() {
        return this.name;
    }

    public String getValue() {
        return this.value;
    }
}
