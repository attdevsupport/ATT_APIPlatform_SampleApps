/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

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
