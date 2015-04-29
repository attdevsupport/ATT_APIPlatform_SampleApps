/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

package com.att.api.mms.service;

import com.att.api.mms.model.MMSStatus;
import com.att.api.mms.model.SendMMSResponse;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Used to interact with version 3 of the MMS API.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
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
     * @param subject subject or null if none
     * @param priority priority or null if to use default
     * @param notifyDelStatus whether to notify of delivery status
     * @return API response
     * @throws RESTException if API request was not successful
     */
    public SendMMSResponse sendMMS(String rawAddrs, String[] fnames,
            String subject, String priority, boolean notifyDelStatus)
            throws RESTException {

        JSONObject outboundRequest = new JSONObject();
        String[] addrs = APIService.formatAddresses(rawAddrs);
        JSONArray jaddrs = new JSONArray();
        for (String addr : addrs) {
            jaddrs.put(addr);
        }

        Object addrStr = addrs.length == 1 ? addrs[0] : jaddrs;
        outboundRequest.put("address", addrStr);

        if (subject != null) { outboundRequest.put("subject", subject); }
        if (priority != null) { outboundRequest.put("priority", priority); }

        outboundRequest.put("notifyDeliveryStatus", notifyDelStatus);

        JSONObject jvars = new JSONObject();
        jvars.put("outboundMessageRequest", outboundRequest);
        try {
            final String endpoint = getFQDN() + "/mms/v3/messaging/outbox";

            final String responseBody =
                new RESTClient(endpoint)
                .setHeader("Content-Type", "application/json")
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(getToken())
                .httpPost(jvars, fnames)
                .getResponseBody();

            return SendMMSResponse.valueOf(new JSONObject(responseBody));
        } catch (JSONException e) {
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
    public MMSStatus getMMSStatus(String mmsId) throws RESTException {
        try {
            String endpoint = getFQDN() + "/mms/v3/messaging/outbox/" + mmsId;
            final String responseBody = new RESTClient(endpoint)
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(getToken())
                .httpGet()
                .getResponseBody();

            return MMSStatus.valueOf(new JSONObject(responseBody));
        } catch (JSONException e) {
            throw new RESTException(e);
        }
    }

}
