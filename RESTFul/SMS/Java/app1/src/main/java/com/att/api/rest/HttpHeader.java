/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/*
 * Copyright 2014 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
