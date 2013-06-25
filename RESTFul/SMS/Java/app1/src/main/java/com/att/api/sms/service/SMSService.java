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

package com.att.api.sms.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONArray;
import org.json.JSONObject;

import java.text.ParseException;

/**
 * Used to interact with version 3 of the SMS API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/3/SMS">SMS Documentation</a>
 */
public class SMSService extends APIService {
    // TODO (pk9069): the json objects need to be moved to explicit models

    /**
     * Creates an SMSService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public SMSService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends a request to the API for sending an SMS.
     *
     * @param rawAddr addresses to send sms to
     * @param msg message to send
     * @param notifyDeliveryStatus whether to notify of delivery status
     * @return api response
     * @throws RESTException if API request was not successful
     */
    public JSONObject sendSMS(String rawAddr, String msg,
            boolean notifyDeliveryStatus) throws RESTException {

        String[] addrs = APIService.formatAddresses(rawAddr);
        JSONArray jaddrs = new JSONArray();
        for (String addr : addrs) {
            jaddrs.put(addr);
        }

        // Build the request body
        JSONObject rpcObject = new JSONObject();
        JSONObject body = new JSONObject();
        body.put("message", msg);

        Object addrStr = addrs.length == 1 ? addrs[0] : jaddrs;
        body.put("address", addrStr);

        body.put("notifyDeliveryStatus", notifyDeliveryStatus);
        rpcObject.put("outboundSMSRequest", body);

        String endpoint = getFQDN() + "/sms/v3/messaging/outbox";
        APIResponse response =
            new RESTClient(endpoint)
            .addHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .httpPost(rpcObject.toString());

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(responseBody);
        } catch (ParseException pe) {
            throw new RESTException("Invalid response from API Server"
                    + ". ParseError: " + pe.getMessage());
        }

        return jsonResponse;
    }

    /**
     * Sends a request for getting delivery status information about an SMS.
     *
     * @param msgId message id used to get status
     * @return api response
     * @throws RESTException if API request was not successful
     */
    public JSONObject getSMSDeliveryStatus(String msgId) throws RESTException {
        String endpoint = getFQDN() + "/sms/v3/messaging/outbox/" + msgId;
        APIResponse response =
            new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .httpGet();

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(responseBody);
        } catch (ParseException pe) {
            throw new RESTException(
                "Invalid response from API Server. ParseError: "
                + pe.getMessage());
        }
        return jsonResponse;
    }

    /**
     * Sends a request to the API for getting any messages sent to the
     * specified shortcode.
     *
     * @param registrationID registration id (registered shortcode)
     * @return api response
     * @throws RESTException if API request was not successful
     */
    public JSONObject getSMSReceive(String registrationID) throws RESTException {

        String endpoint = getFQDN() + "/rest/sms/2/messaging/inbox";
        APIResponse response =
            new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .addParameter("RegistrationID", registrationID)
            .httpGet();

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(responseBody);
        } catch (ParseException pe) {
            throw new RESTException("Invalid response from API Server"
                    + ". ParseError: " + pe.getMessage());
        }
            return jsonResponse;
    }
}
