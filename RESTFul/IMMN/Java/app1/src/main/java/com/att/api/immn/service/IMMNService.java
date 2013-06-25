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

import java.io.File;
import java.text.ParseException;
import org.json.JSONArray;
import org.json.JSONObject;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

/**
 * Used to interact with version 1 of the In-app Messaging from Mobile
 * Number(IMMN) API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @author <a href="mailto:kh455g@att.com">Kyle Hill</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number">Documentation</a>
 */
public class IMMNService extends APIService {
    /**
     * Creates an IMMNService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public IMMNService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends a request to the API for getting message headers.
     *
     * @param headerCount header count
     * @param indexCursor index cursor
     * @return API response
     * @throws RESTException if API request was not successful
     */
    public IMMNResponse getMessageHeaders(int headerCount,
            String indexCursor) throws RESTException {

        String endpoint = getFQDN() + "/rest/1/MyMessages";
        APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addParameter("HeaderCount", "" + headerCount)
            .httpGet();

        JSONObject jresponse = null;
        try {
            jresponse = new JSONObject(response.getResponseBody());
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
        return IMMNResponse.buildFromJSON(jresponse);
    }

    /**
     * Sends an API request for getting message body.
     *
     * @param msgId message id
     * @param partNumb part number
     * @return API response
     * @throws RESTException if API request was not successful
     */
    public IMMNBody getMessageBody(String msgId, String partNumb)
            throws RESTException {
        String endpoint
            = getFQDN() + "/rest/1/MyMessages/" + msgId + "/" + partNumb;

        // TODO (pk9069): Check if 'accept' and 'content-type' are required or
        // even proper
        APIResponse response = new RESTClient(endpoint)
            .setHeader("Accept", "application/json")
            .setHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpGet();

        String headerValue = response.getHeader("Content-Type");
        return new IMMNBody(headerValue, response.getResponseBody());
    }

    /**
     * Sends an API request for sending a message.
     *
     * @param addrs array of address to send message to
     * @param txt message text
     * @param subject message subject
     * @param files array of files to send or <tt>null</tt> if none
     * @return message id
     * @throws RESTException if API request was not successful
     */
    public String sendMessage(String[] addrs, String txt, String subject,
            File[] files) throws RESTException {

        // TODO (pk9069): Should address format conversion be done here?
        // TODO (kh455g): yes
        addrs = formatAddresses(addrs);
        String endpoint = getFQDN() + "/rest/1/MyMessages";
        RESTClient client = new RESTClient(endpoint);
        client.addAuthorizationHeader(getToken());
        JSONObject jobj = new JSONObject();
        jobj.put("Text", txt);
        jobj.put("Subject", subject);
        JSONArray jaddrs = new JSONArray();

        for (String addr : addrs) {
            jaddrs.put(addr);
        }

        jobj.put("Addresses", jaddrs);
        APIResponse response;

        if (files == null || files.length == 0) {
            client.setHeader("Content-Type", "application/json");
            response = client.httpPost(jobj.toString());
        } else {
            String[] fnames = new String[files.length];
            for (int i = 0; i < files.length; ++i) {
                File f = files[i];
                fnames[i] = f.getAbsolutePath();
            }
            response = client.httpPost(jobj, fnames);
        }

        JSONObject jresponse;
        try {
            jresponse = new JSONObject(response.getResponseBody());
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
        return jresponse.getString("Id");
    }
}
