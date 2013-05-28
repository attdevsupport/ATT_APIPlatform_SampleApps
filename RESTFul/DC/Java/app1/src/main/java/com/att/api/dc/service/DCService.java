package com.att.api.dc.service;

import com.att.api.dc.model.DCResponse;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import org.json.JSONObject;

import java.text.ParseException;

public class DCService {
    private final OAuthToken token;
    private final RESTConfig cfg;

    public DCService(final RESTConfig cfg, final OAuthToken token) { 
        this.token = token;
        this.cfg = cfg;
    }

    public DCResponse getDeviceCapabilities() throws RESTException {
        final APIResponse response = 
            new RESTClient(this.cfg)
            .addAuthorizationHeader(token)
            .httpGet();

        final String responseBody = response.getResponseBody();

        try {
            JSONObject jsonResponse = new JSONObject(responseBody);
            return new DCResponse(jsonResponse);
        } catch (ParseException pe) {
            String msg = pe.getMessage();
            String err = "Invalid response from API Server: " + msg;
            throw new RESTException(-1, err); 
        }
    }
}
