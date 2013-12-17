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

package com.att.api.tl.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;
import com.att.api.tl.model.TLResponse;
import org.json.JSONObject;
import java.text.ParseException;

/**
 * Used to interact with version 2 of the Terminal Location API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/2/Location">Location Documentation</a>
 */
public class TLService extends APIService {

    /**
     * Creates a TLService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public TLService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends a request to the API for getting device location.
     *
     * @param requestedAccuracy requested accuracy
     * @param acceptableAccuracy acceptable accuracy
     * @param tolerance tolerance
     * @return device location
     * @throws RESTException if API request was not successful
     */
    public TLResponse getLocation(String requestedAccuracy,
            String acceptableAccuracy, String tolerance) throws RESTException {

        String endpoint = getFQDN() + "/2/devices/location";

        RESTClient client = new RESTClient(endpoint)
            .addParameter("requestedAccuracy", requestedAccuracy)
            .addParameter("acceptableAccuracy", acceptableAccuracy)
            .addParameter("tolerance", tolerance)
            .addAuthorizationHeader(getToken());

        long timeStart = System.currentTimeMillis();

        APIResponse response = client.httpGet();

        // elapsed time
        long eTime = (System.currentTimeMillis() - timeStart) / 1000;
        String responseBody = response.getResponseBody();

        try {
            return TLResponse.valueOf(eTime, new JSONObject(responseBody));
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }
}
