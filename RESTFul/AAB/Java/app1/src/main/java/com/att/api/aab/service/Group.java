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

public final class Group {
    private final String groupId;
    private final String groupName;
    private final String groupType;

    public Group(String groupName) {
        this(null, groupName, null);
    }
    
    public Group(String groupName, String groupType) {
        this(null, groupName, groupType);
    }

    public Group(String groupId, String groupName, String groupType) {
        this.groupId = groupId;
        this.groupName = groupName;
        this.groupType = groupType;
    }

    public String getGroupId() {
        return groupId;
    }

    public String getGroupName() {
        return groupName;
    }

    public String getGroupType() {
        return groupType;
    }

    public JSONObject toJson() {
        JSONObject jobj = new JSONObject();
        if (getGroupId() != null) jobj.put("groupId", getGroupId());
        if (getGroupName() != null) jobj.put("groupName", getGroupName());
        if (getGroupType() != null) jobj.put("groupType", getGroupType());
        return jobj;
    }

    public static Group valueOf(JSONObject jobj) {
        String id = jobj.has("groupId") ? jobj.getString("groupId") : null;
        String name = jobj.has("groupName") ? jobj.getString("groupName") : null;
        String type = jobj.has("groupType") ? jobj.getString("groupType") : null;
        return new Group(id, name, type);
    }
}
