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

public final class Group {
    private final String groupId;
    private final String groupName;
    private final String groupType;
    
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
