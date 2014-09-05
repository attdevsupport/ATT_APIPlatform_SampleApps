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

package com.att.api.ads.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONObject;

import java.util.Map;
import java.util.Set;

/**
 * Used to interact with version 1 of the Advertising API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/1/Advertising">Advertising Documentation</a>
 */
public class ADSService extends APIService {

    /**
     * Creates an ADService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public ADSService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends a request to the API for getting an advertisement.
     *
     * @param category category of advertisement
     * @param userAgent user agent
     * @param optVals any optional values to send
     * @param udid universally unique identifier
     * @return JSONObject API response
     * @throws RESTException if API request was not successful
     */
    public JSONObject getAdvertisement(String category, String userAgent, String udid,
            Map<String, String> optVals) throws RESTException {

        String endpoint = getFQDN() + "/rest/1/ads";

        RESTClient client = new RESTClient(endpoint)
            .addParameter("Category", category)
            .addAuthorizationHeader(getToken())
            .addHeader("User-Agent", userAgent)
            .addHeader("UDID", udid);

        Set<String> keys = optVals.keySet();
        for (String key : keys) {
            client.addParameter(key, optVals.get(key));
        }

        APIResponse response = client.httpGet();

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        jsonResponse = new JSONObject(responseBody);
        return jsonResponse;
    }
}
