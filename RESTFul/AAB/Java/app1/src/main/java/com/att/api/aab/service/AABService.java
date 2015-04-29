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


import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

/**
 * Used to interact with version 1 of the Address Book API.
 *
 * <p>
 * This class is thread safe.
 * </p>
 *
 */
public class AABService extends APIService {
    
    private void addPageParams(RESTClient client, PageParams params) {
        if (params == null) return;

        final String[] keys = { "order", "orderBy", "limit", "offset" };
        final String[] values = { 
            params.getOrder(), params.getOrderBy(), params.getLimit(),
            params.getOffset() 
        };

        for (int i = 0; i < keys.length; ++i) {
            if (values[i] == null) continue;
            client.addParameter(keys[i], values[i]);
        }
    }

    /**
     * Creates a AABService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public AABService(final String fqdn, final OAuthToken token) {
        super(fqdn, token);
    }

    public String createContact(Contact contact) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/contacts";

        JSONObject payload = new JSONObject();
        payload.put("contact", contact.toJson());
        APIResponse response = new RESTClient(endpoint)
            .addHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPost(payload.toString());

        if (response.getStatusCode() != 201) {
            throw new RESTException(response.getResponseBody());
        }

        return response.getHeader("location");
    }

    public ContactWrapper getContact(String contactId, String xFields)
            throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/contacts/" + contactId;

        RESTClient client = new RESTClient(endpoint)
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken());

        if (xFields != null) {
            client.addHeader("x-fields", xFields);
        }

        APIResponse response = client.httpGet();
        if (response.getStatusCode() != 200) {
            throw new RESTException(response.getResponseBody());
        }

        final String body = response.getResponseBody();
        JSONObject jobj = new JSONObject(body);
        if (jobj.has("contact")) {
            Contact contact = Contact.valueOf(jobj.getJSONObject("contact"));
            return new ContactWrapper(contact);
        }

        JSONObject jQc = jobj.getJSONObject("quickContact");
        return new ContactWrapper(QuickContact.valueOf(jQc));
    }


    public ContactResultSet getContacts() throws RESTException {
        return getContacts(null, null, null);
    }

    public ContactResultSet getContacts(String sParams) throws RESTException {
        return getContacts(null, null, sParams);
    }

    public ContactResultSet getContacts(String xFields, String sParams) 
            throws RESTException {
        return getContacts(xFields, null, sParams);
    }

    public ContactResultSet getContacts(PageParams pParams, String sParams)
            throws RESTException {
        return getContacts(null, pParams, sParams);
    }

    public ContactResultSet getContacts(String xFields, PageParams pParams,
            String sParams) throws RESTException {

        String endpoint = getFQDN() + "/addressBook/v1/contacts";

        RESTClient client = new RESTClient(endpoint)
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken());

        if (xFields != null) {
            client.addHeader("x-fields", xFields);
        }
        if (pParams != null) {
            this.addPageParams(client, pParams);
        }

        if (sParams != null) {
            client.addParameter("search", sParams);
        }

        APIResponse response = client.httpGet();
        if (response.getStatusCode() != 200) {
            throw new RESTException(response.getResponseBody());
        }
        final String body = response.getResponseBody();
        try {
            JSONObject jrs = new JSONObject(body).getJSONObject("resultSet");
            return ContactResultSet.valueOf(jrs);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public GroupResultSet getContactGroups(String contactId)
            throws RESTException {
        return getContactGroups(contactId, null);
    }

    public GroupResultSet getContactGroups(String contactId, PageParams params)
            throws RESTException {
        String endpoint = getFQDN() 
            + "/addressBook/v1/contacts/" + contactId + "/groups";

        RESTClient client = new RESTClient(endpoint)
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken());

        this.addPageParams(client, params);
        APIResponse response = client.httpGet();
        if (response.getStatusCode() != 200) {
            throw new RESTException(response.getResponseBody());
        }
        final String body = response.getResponseBody();
        try {
            JSONObject jrs = new JSONObject(body).getJSONObject("resultSet");
            return GroupResultSet.valueOf(jrs);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public void updateContact(Contact contact, String contactId)
            throws RESTException {

        String endpoint = getFQDN() + "/addressBook/v1/contacts/" + contactId;

        JSONObject payload = new JSONObject();
        payload.put("contact", contact.toJson());
        APIResponse response = new RESTClient(endpoint)
            .addHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPatch(payload.toString());

        int statusCode = response.getStatusCode();
        if (statusCode != 200 && statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public void deleteContact(String contactId) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/contacts/" + contactId;

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .httpDelete();

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public String createGroup(Group group) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/groups";

        JSONObject payload = new JSONObject();
        payload.put("group", group.toJson());
        APIResponse response = new RESTClient(endpoint)
            .addHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPost(payload.toString());

        int statusCode = response.getStatusCode();
        if (statusCode != 201) {
            throw new RESTException(response.getResponseBody());
        }

        return response.getHeader("location");
    }

    public GroupResultSet getGroups(String groupName) 
            throws RESTException {
        return getGroups(null, groupName);
    }

    public GroupResultSet getGroups(PageParams params, String groupName)
            throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/groups";

        RESTClient client = new RESTClient(endpoint)
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken());

        this.addPageParams(client, params);
        if (groupName != null) client.addParameter("groupName", groupName);
        APIResponse response = client.httpGet();

        int statusCode = response.getStatusCode();
        if (statusCode != 200) {
            throw new RESTException(response.getResponseBody());
        }

        final String body = response.getResponseBody();
        try {
            JSONObject jrs = new JSONObject(body).getJSONObject("resultSet");
            return GroupResultSet.valueOf(jrs);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public void deleteGroup(String groupId) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/groups/" + groupId;

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .httpDelete();

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public void updateGroup(Group group, String groupId) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/groups/" + groupId;
        
        JSONObject payload = new JSONObject();
        payload.put("group", group.toJson());
        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Content-Type", "application/json")
            .httpPatch(payload.toString());

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public void addContactsToGroup(String groupId, String contactIds)
            throws RESTException {

        // TODO: encode contactIds
        String endpoint = getFQDN() + "/addressBook/v1/groups/" + groupId 
            + "/contacts?contactIds=" + contactIds;

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .httpPost();

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public void removeContactsFromGroup(String groupId, String contactIds)
            throws RESTException {

        // TODO: encode contactIds
        String endpoint = getFQDN() + "/addressBook/v1/groups/" + groupId 
            + "/contacts?contactIds=" + contactIds;

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .httpDelete();

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

    public String[] getGroupContacts(String groupId) throws RESTException {
        return getGroupContacts(groupId, null);
    }

    public String[] getGroupContacts(String groupId, PageParams params)
            throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/groups/" + groupId 
            + "/contacts";

        RESTClient client = new RESTClient(endpoint)
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken());

        this.addPageParams(client, params);
        APIResponse response = client.httpGet();

        int statusCode = response.getStatusCode();
        if (statusCode != 200) {
            throw new RESTException(response.getResponseBody());
        }

        final String body = response.getResponseBody();
        try {
            JSONObject jIds = new JSONObject(body).getJSONObject("contactIds");
            JSONArray idsArr = jIds.getJSONArray("id");
            String[] ids = new String[idsArr.length()];
            for (int i = 0; i < ids.length; ++i) {
                ids[i] = idsArr.getString(i);
            }
            return ids;
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public Contact getMyInfo() throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/myInfo";

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .httpGet();

        int statusCode = response.getStatusCode();
        if (statusCode != 200) {
            throw new RESTException(response.getResponseBody());
        }

        final String body = response.getResponseBody();
        try {
            JSONObject jMyInfo = new JSONObject(body);
            return Contact.valueOf(jMyInfo.getJSONObject("myInfo"));
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public void updateMyInfo(Contact myInfo) throws RESTException {
        String endpoint = getFQDN() + "/addressBook/v1/myInfo";

        JSONObject payload = new JSONObject();
        payload.put("myInfo", myInfo.toJson());

        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Content-Type", "application/json")
            .httpPatch(payload.toString());

        int statusCode = response.getStatusCode();
        if (statusCode != 204) {
            throw new RESTException(response.getResponseBody());
        }
    }

}
