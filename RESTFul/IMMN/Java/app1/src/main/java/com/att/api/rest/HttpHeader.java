/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2013.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.rest;

/**
 * Immutable class that holds an HTTP header.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class HttpHeader {
    /** Http header name. */
    private final String name;

    /** Http header value. */
    private final String value;

    /**
     * Creates a key-value pair used to represent an http header.
     *
     * @param name http name
     * @param value http value
     */
    public HttpHeader(final String name, final String value) {
        if (name == null || name.equals("")) {
            throw new IllegalArgumentException("Name must not be empty.");
        }

        this.name = name;
        this.value = value;
    }

    /**
     * Gets http header name.
     *
     * @return http header name
     */
    public String getName() {
        return this.name;
    }


    /**
     * Gets http header value.
     *
     * @return http header value
     */
    public String getValue() {
        return this.value;
    }
}
