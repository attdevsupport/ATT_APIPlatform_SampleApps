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

package com.att.api.cms.service;

import java.text.ParseException;
import java.util.Map;

import org.json.JSONObject;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

/**
 * Used to interact with version 1 of the Call Management API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/1/Call%20Management%20(Beta)">CMS Documentation</a>
 */
public class CMSService extends APIService {

    /**
     * Creates a CMSService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public CMSService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Internal method used to convert a map of variables to a JSON string.
     *
     * @param vars map to convert
     * @return JSON string
     */
    private String buildScriptVariables(final Map<String, String> vars) {
        final JSONObject scriptVars = new JSONObject();
        for (final Map.Entry<String, String> entry : vars.entrySet()) {
            scriptVars.put(entry.getKey(), entry.getValue());
        }
        return scriptVars.toString();
    }

    /**
     * Sends a request to the API for creating a CMS session.
     *
     * @param variables variables to send
     * @return CMSSessionResponse API response
     * @throws RESTException if API request was not successful
     */
    public CMSSessionResponse createSession(Map<String, String> variables)
            throws RESTException {

        String endpoint = getFQDN() + "/rest/1/Sessions";
        String body = buildScriptVariables(variables);
        APIResponse response = new RESTClient(endpoint)
            .addHeader("Content-Type", "application/json")
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPost(body);

        try {
            String responseBody = response.getResponseBody();
            JSONObject jsonResponse = new JSONObject(responseBody);
            final String id = jsonResponse.getString("id");
            final boolean success = jsonResponse.getBoolean("success");
            return new CMSSessionResponse(id, success);
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }

    /**
     * Sends a request to the API for sending a signal to an existing session.
     *
     * <p>
     * A session may be created with <code>createSession()</code>.
     * </p>
     *
     * @param sessionId session id to use for sending signal
     * @param signal signal to send
     * @return status
     * @throws RESTException if API request was not successful
     * @see #createSession(java.util.Map)
     */
    public String sendSignal(String sessionId, String signal)
            throws RESTException {

        if (sessionId == null || sessionId.equals("")) {
            String err = "Please create a session before sending a signal";
            throw new RESTException(err);
        }

        String fqdn = getFQDN();
        String endpoint = fqdn + "/rest/1/Sessions/" + sessionId + "/Signals";
        JSONObject msgBody = new JSONObject();
        msgBody.put("signal", signal);

        APIResponse apiResponse = new RESTClient(endpoint)
            .setHeader("Accept", "application/json")
            .setHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPost(msgBody.toString());

        final String responseBody = apiResponse.getResponseBody();
        try {
            final JSONObject jsonResponse = new JSONObject(responseBody);
            final String status = jsonResponse.getString("status");
            return status;
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }
}
