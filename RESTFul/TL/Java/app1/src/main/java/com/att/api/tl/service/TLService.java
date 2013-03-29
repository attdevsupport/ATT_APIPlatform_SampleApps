package com.att.api.tl.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;
import com.att.api.tl.model.TLResponse;

import org.json.JSONObject;

import java.text.ParseException;

public class TLService {
    private final OAuthToken token;
    private final RESTConfig cfg;

    private TLResponse buildResponse(long elapsedTime, 
            String jsonStr) throws RESTException  {
        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(jsonStr);
        } catch (ParseException pe) {
            String msg = pe.getMessage();
            String err = "Invalid response from API Server: " + msg;
            throw new RESTException(-1, err); 
        }
        final String latitude = jsonResponse.getString("latitude");
        final String longitude = jsonResponse.getString("longitude");
        final String accuracy = jsonResponse.getString("accuracy");
        return new TLResponse(elapsedTime + "", latitude, longitude, accuracy);
    }

    public TLService(final RESTConfig cfg, final OAuthToken token) { 
        this.token = token;
        this.cfg = cfg;
    }

    public TLResponse getLocation(String requestedAccuracy, 
            String acceptableAccuracy, String tolerance) throws RESTException {

        final RESTClient client = new RESTClient(this.cfg)
            .addParameter("requestedAccuracy", requestedAccuracy)
            .addParameter("acceptableAccuracy", acceptableAccuracy)
            .addParameter("tolerance", tolerance)
            .addAuthorizationHeader(token);

        final long timeStart = System.currentTimeMillis();

        final APIResponse response = client.httpGet();

        final long elapsedTime = (System.currentTimeMillis() - timeStart) / 1000;
        final String responseBody = response.getResponseBody();

        return buildResponse(elapsedTime, responseBody);
    }
}
