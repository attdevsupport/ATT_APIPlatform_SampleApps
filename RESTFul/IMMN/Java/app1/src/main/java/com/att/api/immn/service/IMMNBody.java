/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

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

package com.att.api.immn.service;

/**
 * Immutable class used to hold an IMMN body.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @author <a href="mailto:kh455g@att.com">Kyle Hill</a>
 * @version 3.0
 * @since 2.2
 */
public final class IMMNBody {

    /** Content type. */
    private final String contentType;

    /** Content data, as a string. */
    private final String data;

    /**
     * Creats an IMMNBody object.
     *
     * @param contentType content type
     * @param data content data
     */
    public IMMNBody(String contentType, String data) {
        this.contentType = contentType;
        this.data = data;
    }

    /**
     * Gets content type.
     *
     * @return content type
     */
    public String getContentType() {
        return this.contentType;
    }

    /**
     * Gets content data.
     *
     * @return content data
     */
    public String getData() {
        return this.data;
    }
}
