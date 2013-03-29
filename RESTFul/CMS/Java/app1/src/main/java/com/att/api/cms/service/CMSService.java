package com.att.api.cms.service;

import java.text.ParseException;
import java.util.HashMap;
import java.util.Map;

import org.apache.http.HttpException;
import org.json.JSONObject;

import com.att.api.cms.model.CallControlResponse;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

public class CMSService {
    public CMSService() {
    }

    private String buildScriptVariables(final HashMap<String,String> vars) {
        final JSONObject scriptVars = new JSONObject();
        for (final Map.Entry<String,String> entry : vars.entrySet()) {
            scriptVars.put(entry.getKey(), entry.getValue());
        }
        return scriptVars.toString();
    }

    public CallControlResponse createSession(RESTConfig cfg, HashMap<String, 
            String> variables, OAuthToken token) throws RESTException{

        String scriptVars = buildScriptVariables(variables);
        
        final APIResponse apiResponse = sendReceive(cfg, scriptVars, token);

        try {
            String responseBody = apiResponse.getResponseBody();
            JSONObject jsonResponse = new JSONObject(responseBody);
            final String id = jsonResponse.getString("id");
	    final boolean success = jsonResponse.getBoolean("success");
            return CallControlResponse.sessionIdResponse(id, success);
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }
    
    /**
     * Send Signal to active session
     * 
     * @param signal
     * @throws HttpException
     */
    public CallControlResponse sendSignal(RESTConfig cfg, String sessionId, 
            String signal, OAuthToken token) throws RESTException{

        if (sessionId == null) {
            final String errorMessage 
                = "Please create a session before sending a signal";
            return CallControlResponse.signalErrorResponse(errorMessage);
        }
        
        JSONObject msgBody = new JSONObject();
        msgBody.put("signal", signal);
        final APIResponse apiResponse 
            = sendReceive(cfg, msgBody.toString(), token);
        
        final int statusCode = apiResponse.getStatusCode();
        final String responseBody = apiResponse.getResponseBody();
        if (statusCode != 200 && statusCode != 201) {
            final String error = statusCode + ":" + responseBody;
            return CallControlResponse.sessionErrorResponse(error);
        }

        try {
            final JSONObject jsonResponse = new JSONObject(responseBody);
            final String status = jsonResponse.getString("status");
            return CallControlResponse.signalStatusResponse(status);
        } catch (ParseException pe) {
            throw new RESTException(pe);
        }
    }
    
    private APIResponse sendReceive(final RESTConfig cfg, final String body, 
            final OAuthToken token) throws RESTException {

        return new RESTClient(cfg)
            .addHeader("Content-Type", "application/json")
            .addHeader("Accept", "application/json")
            .addAuthorizationHeader(token)
            .httpPost(body);
    }
    
}
