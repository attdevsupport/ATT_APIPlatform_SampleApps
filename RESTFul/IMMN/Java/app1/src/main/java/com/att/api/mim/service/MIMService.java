package com.att.api.mim.service;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.text.ParseException;
import java.util.List;

import org.json.JSONObject;

import com.att.api.immn.controller.Config;
import com.att.api.immn.model.IMMNResponse;
import com.att.api.mim.model.MIMResponse;
import com.att.api.mim.model.MIMContentResponse;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;

public class MIMService {

    private Config config;

    public MIMService(Config config){
        this.config = config;
    }

    //TODO: properly implement getContent
    public MIMContentResponse getMessageContent(String msgId, String partNumber, String accessToken)
    {
        MIMContentResponse response = null;
        APIResponse apiResponse  = null;
        RESTClient restClient = new RESTClient(true, config.proxyHost, config.proxyPort);
        String url;

        if (config.endPointURL.endsWith("/"))
            url = config.endPointURL + msgId + "/" + partNumber;
        else
            url = config.endPointURL + "/" + msgId + "/" + partNumber;

        apiResponse = restClient.sendHttpGetRequest(url, accessToken);

        if (apiResponse.getStatusCode() == 200 || apiResponse.getStatusCode() == 201) {
            try {
                response = new MIMContentResponse(apiResponse);
            } catch (ParseException e) {
                response = MIMContentResponse.createErrorResponse(e.getMessage());
            }
        }
        else
          response = MIMContentResponse.createErrorResponse(apiResponse.getResponseBody());
        return response;
    }

    public MIMResponse getMessageHeader(String headerCount, String indexCursor, String accessToken) 
    {
        MIMResponse response = null;
        APIResponse apiResponse = null;
        RESTClient restClient = new RESTClient(true, config.proxyHost, config.proxyPort);

        String url = config.endPointURL + "?HeaderCount=" + headerCount + "&IndexCursor" + indexCursor;

        apiResponse = restClient.sendHttpGetRequest(url, accessToken);

        if (apiResponse.getStatusCode() == 200 || apiResponse.getStatusCode() == 201) {
            try {
                JSONObject rpcObject = new JSONObject(apiResponse.getResponseBody());
                response = new MIMResponse(rpcObject);
            } catch (ParseException e) {
                response = MIMResponse.createErrorResponse(e.getMessage());
            } 
        }
        else 
          response = MIMResponse.createErrorResponse("" + apiResponse.getResponseBody());
        return response;
    }
}
