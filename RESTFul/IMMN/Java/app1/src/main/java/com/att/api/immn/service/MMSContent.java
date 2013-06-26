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

import org.json.JSONObject;

/**
 * Immutable class used to hold MMS content.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @author <a href="mailto:kh455g@att.com">Kyle Hill</a>
 * @version 3.0
 * @since 2.2
 */
public final class MMSContent {

    /** Content type. */
    private final String contentType;

    /** Content name. */
    private final String contentName;

    /** Part number. */
    private final String partNumber;

    /**
     * Creates an MMS object.
     *
     * @param jobj json object used to create MMS object.
     * @deprecated replaced by {@link #valueOf(JSONObject)}
     */
    public MMSContent(JSONObject jobj) {
        this.contentType = jobj.getString("ContentType");
        this.contentName = jobj.getString("ContentName");
        this.partNumber = jobj.getString("PartNumber");
    }

    /**
     * Creates an MMSContent object.
     *
     * @param cType content type
     * @param cName content name
     * @param pNumber part number
     * @since 3.0
     */
    public MMSContent(String cType, String cName, String pNumber) {
        this.contentType = cType;
        this.contentName = cName;
        this.partNumber = pNumber;
    }

    /**
     * Gets content type.
     *
     * @return content type
     */
    public String getContentType() {
        return contentType;
    }

    /**
     * Gets content name.
     *
     * @return content name
     */
    public String getContentName() {
        return contentName;
    }

    /**
     * Gets part number.
     *
     * @return part number
     */
    public String getPartNumber() {
        return partNumber;
    }

    /**
     * Factory method for creating an MMSContent object out of the specified
     * <code>JSONObject</code>.
     *
     * @param jobj json object used to create MMS object
     * @return MMSContent MMSContent object created
     * @since 3.0
     */
    public static MMSContent valueOf(JSONObject jobj) {
        String cType = jobj.getString("ContentType");
        String cName = jobj.getString("ContentName");
        String pNumber = jobj.getString("PartNumber");
        return new MMSContent(cType, cName, pNumber);
    }
}
