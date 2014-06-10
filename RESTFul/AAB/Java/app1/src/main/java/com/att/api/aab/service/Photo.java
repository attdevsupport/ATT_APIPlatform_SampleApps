/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2014.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.aab.service;

import org.json.JSONObject;

public final class Photo {
    public final String encoding;
    public final String value;

    public Photo(String encoding, String value) {
        this.encoding = encoding;
        this.value = value;
    }

    public String getValue() {
        return this.value;
    }

    public String getEncoding() {
        return this.encoding;
    }

    public JSONObject toJson() {
        JSONObject jobj = new JSONObject();

        jobj.put("encoding", getEncoding());
        jobj.put("value", getValue());

        return jobj;
    }

    public static Photo valueOf(JSONObject jobj) {
        String encoding = jobj.has("encoding") ? jobj.getString("encoding") : null;
        String value = jobj.has("value") ? jobj.getString("value") : null;
        return new Photo(encoding, value);
    }

}
