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

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Immutable class used to hold an IMMN response.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @author <a href="mailto:kh455g@att.com">Kyle Hill</a>
 * @version 3.0
 * @since 2.2
 */
public final class IMMNResponse {
    /** Index cursor. */
    private final String indexCursor;

    /** Header count. */
    private final String headerCount;

    /** Array of immn headers. */
    private final IMMNHeader[] headers;

    /**
     * Creates an IMMNResponse object using the specified parameters.
     *
     * @param ic index cursor
     * @param hc header count
     * @param headers headers
     */
    public IMMNResponse(String ic, String hc, IMMNHeader[] headers) {
        this.indexCursor = ic;
        this.headerCount = hc;
        // TODO (pk9069): avoid exposing internals
        this.headers = headers;
    }


    /**
     * Gets immn headers.
     *
     * @return immn headers.
     */
    public IMMNHeader[] getHeaders() {
        // TODO (pk9069): avoid exposing internals?
        return headers;
    }

    /**
     * Gets index cursor.
     *
     * @return index cursor
     */
    public String getIndexCursor() {
        return indexCursor;
    }

    /**
     * Gets header count.
     *
     * @return header count
     */
    public String getHeaderCount() {
        return headerCount;
    }

    /**
     * Alias for <code>valueOf()</code>.
     *
     * @param jobj json object
     * @return immn response
     * @see #valueOf(JSONObject)
     */
    public static IMMNResponse buildFromJSON(JSONObject jobj) {
        return IMMNResponse.valueOf(jobj);
    }

    /**
     * Factory method used to create an IMMN response using the specified json
     * object.
     *
     * @param jobj json object
     * @return immn response
     * @since 3.0
     */
    public static IMMNResponse valueOf(JSONObject jobj) {
        JSONObject jheaderslist = jobj.getJSONObject("MessageHeadersList");
        JSONArray jheaders = jheaderslist.getJSONArray("Headers");
        IMMNHeader[] headers = new IMMNHeader[jheaders.length()];
        for (int i = 0; i < jheaders.length(); ++i) {
            JSONObject jheader = jheaders.getJSONObject(i);
            headers[i] = IMMNHeader.buildFromJSON(jheader);
        }

        String indexCursor = jheaderslist.getString("IndexCursor");
        String headerCount = jheaderslist.getString("HeaderCount");
        return new IMMNResponse(indexCursor, headerCount, headers);
    }
}
