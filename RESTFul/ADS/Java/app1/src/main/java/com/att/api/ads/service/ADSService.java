package com.att.api.ads.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import org.json.JSONObject;

import java.text.ParseException;

public class ADSService {
    private final OAuthToken token;
    private final RESTConfig cfg;

    public ADSService(final RESTConfig cfg, final OAuthToken token) { 
        this.token = token;
        this.cfg = cfg;
    }

    public JSONObject getAdvertisement(String category, String userAgent) 
            throws RESTException {

        // TODO: Refactor
        APIResponse response = 
            new RESTClient(this.cfg)
            .addParameter("Category", category)
            .addAuthorizationHeader(this.token)
            .addHeader("User-Agent", userAgent)
            // UDID is random... trust me 
            .addHeader("UDID", "9c8bdedf56991a7efb7f02b200915ee4")
            .httpGet();

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(responseBody);
        } catch (ParseException pe) {
            throw new RESTException (
                "Invalid response from API Server. ParseError: " 
                + pe.getMessage());
        }
        return jsonResponse;
    }
}
