/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

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
package com.att.api.mms.service;

import java.text.ParseException;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Used to interact with version 3 of the MMS API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/3/MMS">MMS Documentation</a>
 */
public class MMSService extends APIService {

    /**
     * Creates an MMSService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public MMSService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends request to the API for sending an MMS using the specified
     * parameters.
     *
     * @param rawAddrs addresses to use for sending mms
     * @param fnames path of attachments
     * @param subject subject
     * @param priority priority
     * @param notifyDelStatus whether to notify of delivery status
     * @return API response
     * @throws RESTException if API request was not successful
     */
    public JSONObject sendMMS(String rawAddrs, String[] fnames, String subject,
            String priority, boolean notifyDelStatus) throws RESTException {
        // TODO (pk9069): avoid returning a JSONObject and move to explicit
        // model
        JSONObject outboundRequest = new JSONObject();
        String[] addrs = APIService.formatAddresses(rawAddrs);
        JSONArray jaddrs = new JSONArray();
        for (String addr : addrs) {
            jaddrs.put(addr);
        }

        Object addrStr = addrs.length == 1 ? addrs[0] : jaddrs;
        outboundRequest.put("address", addrStr);

        if (subject != null) {
            outboundRequest.put("subject", subject);
        }
        if (priority != null) {
            outboundRequest.put("priority", priority);
        }
        outboundRequest.put("notifyDeliveryStatus", notifyDelStatus);

        JSONObject jvars = new JSONObject();
        jvars.put("outboundMessageRequest", outboundRequest);
        try {
            final String endpoint = getFQDN() + "/mms/v3/messaging/outbox";
            APIResponse response =
                new RESTClient(endpoint)
                .setHeader("Content-Type", "application/json")
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(getToken())
                .httpPost(jvars, fnames);

            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }

    /**
     * Sends a request to the API for getting MMS status.
     *
     * @param mmsId MMS id to get status for
     * @return API response
     * @throws RESTException if API request was not successful
     */
    public JSONObject getMMSStatus(String mmsId) throws RESTException {
        // TODO (pk9069): avoid returning a JSONObject and move to explicit
        // model
        try {
            String endpoint = getFQDN() + "/mms/v3/messaging/outbox/" + mmsId;
            APIResponse response =
                new RESTClient(endpoint)
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(getToken())
                .httpGet();
            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }
}
