/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

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

package com.att.api.sms.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONArray;
import org.json.JSONObject;

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
        jsonResponse = new JSONObject(responseBody);
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
        jsonResponse = new JSONObject(responseBody);
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
        jsonResponse = new JSONObject(responseBody);
        return jsonResponse;
    }
}
