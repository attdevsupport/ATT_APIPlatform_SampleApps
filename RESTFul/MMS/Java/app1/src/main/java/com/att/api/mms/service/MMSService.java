package com.att.api.mms.service;

import java.text.ParseException;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import org.json.JSONArray;
import org.json.JSONObject;

public class MMSService {
    private final OAuthToken token;
    private final RESTConfig cfg;

    private String formatAddr(String address) {
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

    public MMSService(final RESTConfig cfg, final OAuthToken token) { 
        this.token = token;
        this.cfg = cfg;
    }

    public JSONObject sendMMS(String rawAddr, String[] fnames, String subject, 
            String priority, boolean notifyDelStatus) throws RESTException {
        JSONObject jvars = new JSONObject();
        JSONObject outboundRequest = new JSONObject();
        String[] addrs = rawAddr.split(","); 
        JSONArray jaddrs = new JSONArray();
        for (String addr : addrs) {
            jaddrs.put(formatAddr(addr));
        }

        if (addrs.length == 1) {
            outboundRequest.put("address", formatAddr(addrs[0]));
        } else {
            outboundRequest.put("address", jaddrs);
        }

        if (subject != null) {
            outboundRequest.put("subject", subject);
        }
        if (priority != null) {
            outboundRequest.put("priority", priority);
        }
        outboundRequest.put("notifyDeliveryStatus", notifyDelStatus);

        jvars.put("outboundMessageRequest", outboundRequest);
        
        try {
            APIResponse response = 
                new RESTClient(cfg)
                .setHeader("Content-Type", "application/json")
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(this.token)
                .httpPost(jvars, fnames);

            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }

    public JSONObject getMMSStatus() throws RESTException {
        try {
            APIResponse response = 
                new RESTClient(cfg)
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(this.token)
                .httpGet();
            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }
}
