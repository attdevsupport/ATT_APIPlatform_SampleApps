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

package com.att.api.dc.service;

import com.att.api.dc.model.DCResponse;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;
import org.json.JSONObject;
import java.text.ParseException;

/**
 * Used to interact with version 2 of the Device Capabilities API.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 * @see <a href="https://developer.att.com/docs/apis/rest/2/Device%20Capabilities">DC Documentation</a>
 */
public class DCService extends APIService {

    /**
     * Creates a DCService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public DCService(String fqdn, final OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Sends a request to the API for getting device capabilities.
     *
     * @return DCResponse API Response
     * @throws RESTException if API request was not successful
     */
    public DCResponse getDeviceCapabilities() throws RESTException {
        String endpoint = getFQDN() + "/rest/2/Devices/Info";

        final APIResponse response =
            new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .httpGet();

        final String responseBody = response.getResponseBody();

        try {
            JSONObject jsonResponse = new JSONObject(responseBody);
            return DCResponse.valueOf(jsonResponse);
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }
}
