package com.att.api.immn.service;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.text.ParseException;
import java.util.List;

import org.apache.commons.collections.map.MultiValueMap;
import org.json.JSONObject;

import com.att.api.immn.controller.Config;
import com.att.api.immn.model.IMMNResponse;
import com.att.api.mim.model.MIMResponse;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;

public class IMMNService {

    private Config config;

    public IMMNService(Config config){
        this.config = config;
    }

    //TODO: test failure conditions
    public IMMNResponse sendMessages(String accessToken, List<String> files, String address, String message, String subject, String group)
    {
        IMMNResponse response = null; 
        APIResponse apiResponse  = null;
        RESTClient restClient = new RESTClient(true,config.proxyHost, config.proxyPort);
        try
        {
            MultiValueMap valuePairs = new MultiValueMap();
            valuePairs.put("Addresses", address.toString());
            valuePairs.put("Text", message.trim());
            valuePairs.put("Subject", subject.trim());
            valuePairs.put("Group", group);
            apiResponse = restClient.sendMultiPartRequest(config.endPointURL, files, accessToken, valuePairs);
        } catch (FileNotFoundException e)
        {
            response = IMMNResponse.createErrorResponse(e.getMessage());
        } catch (IOException e)
        {
            response = IMMNResponse.createErrorResponse(e.getMessage());
        }
        if (apiResponse.getStatusCode() == 200 || apiResponse.getStatusCode() == 201) {
            try {
                JSONObject rpcObject = new JSONObject(apiResponse.getResponseBody());
                String ID = rpcObject.getString("Id");
                response = new IMMNResponse(ID);
            } catch (ParseException e) {
                response = IMMNResponse.createErrorResponse(e.getMessage());
            }
        }
        else {
            response = IMMNResponse.createErrorResponse(apiResponse.getResponseBody());
        }
        return response;
    }
}
