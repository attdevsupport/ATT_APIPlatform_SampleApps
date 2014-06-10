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

public final class Phone {
    private final String type;
    private final String number;
    private final Boolean preferred;

    public Phone(String type, String number, Boolean preferred) {
        this.type = type;
        this.number = number;
        this.preferred = preferred;
    }

    public String getType() {
        return type;
    }

    public String getNumber() {
        return number;
    }

    public Boolean getPreferred() {
        return preferred;
    }

    public Boolean isPreferred() {
        return preferred;
    }

    public JSONObject toJson() {
        JSONObject jobj = new JSONObject();

        final String[] keys = { "type", "number", "preferred" };
        String prefString = null;
        if (getPreferred() != null) {
            prefString = isPreferred() ? "TRUE" : "FALSE";
        }
        final String[] values = { getType(), getNumber(), prefString };
        for (int i = 0; i < keys.length; ++i) {
            if (values[i] == null) continue;
            jobj.put(keys[i], values[i]);
        }

        return jobj;
    }

    public static Phone valueOf(JSONObject jobj) {
        String type = jobj.has("type") ? jobj.getString("type") : null;
        String number = jobj.has("number") ? jobj.getString("number") : null;
        Boolean pref = jobj.has("preferred") ? jobj.getBoolean("preferred") : null;
        return new Phone(type, number, pref);
    }
}
