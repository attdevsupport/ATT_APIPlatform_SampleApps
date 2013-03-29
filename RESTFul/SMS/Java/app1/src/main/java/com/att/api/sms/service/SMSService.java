package com.att.api.sms.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import org.apache.commons.lang3.StringUtils;
import org.json.JSONArray;
import org.json.JSONObject;

import java.text.ParseException;

public class SMSService {
    private final OAuthToken token;
    private final RESTConfig cfg;

    public SMSService(final RESTConfig cfg, final OAuthToken token) { 
        this.token = token;
        this.cfg = cfg;
    }

    private String formatAddr(String address) {
        // Check for a few known formats the user could have entered the
        // address, adjust accordingly
        if ((address.indexOf("-") == 3) && (address.length() == 12))
            return "tel:" + address.substring(0, 3) + address.substring(4, 7)
                    + address.substring(8, 12);
        else if ((address.indexOf(":") == 3) && (address.length() == 14))
            return address;
        else if ((address.indexOf("-") == -1) && (address.length() == 10))
            return "tel:" + address;
        else if ((address.indexOf("-") == -1) && (address.length() == 11))
            return "tel:" + address.substring(1);
        else if ((address.indexOf("-") == -1) && (address.indexOf("+") == 0)
                && (address.length() == 12))
            return "tel:" + address.substring(2);
        else
            return "";
    }

    public JSONObject sendSMS(String rawAddr, String msg, 
            boolean notifyDeliveryStatus) throws RESTException {

        String[] addrs = rawAddr.split(",");
        JSONArray jaddrs = new JSONArray();
        for (String addr : addrs) {
            jaddrs.put(formatAddr(addr));
        }

        // Build the request body
        JSONObject rpcObject = new JSONObject();
        JSONObject body = new JSONObject();
        body.put("message", msg);

        if (addrs.length == 1) {
            body.put("address", formatAddr(addrs[0]));
        } else {
            body.put("address", jaddrs);
        }

        body.put("notifyDeliveryStatus", notifyDeliveryStatus);
        rpcObject.put("outboundSMSRequest", body);

        APIResponse response = 
            new RESTClient(this.cfg)
            .addHeader("Content-Type", "application/json")
            .addAuthorizationHeader(token)
            .addHeader("Accept", "application/json")
            .httpPost(rpcObject.toString());

        final String responseBody = response.getResponseBody();

        JSONObject jsonResponse;
        try {
            jsonResponse = new JSONObject(responseBody);
        } catch (ParseException pe) {
            throw new RESTException (
                    "Invalid response from API Server" + ". ParseError: "
                    + pe.getMessage());
        }

        return jsonResponse;
    }

    public JSONObject getSMSDeliveryStatus() throws RESTException {
        APIResponse response = 
            new RESTClient(this.cfg)
            .addAuthorizationHeader(token)
            .addHeader("Accept", "application/json")
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

    public JSONObject getSMSReceive(String registrationID) 
            throws RESTException {

        APIResponse response = 
            new RESTClient(this.cfg)
            .addAuthorizationHeader(token)
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
