package com.att.api.mms.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.mms.model.MMSDeliveryInfo;
import com.att.api.mms.model.MMSStatus;
import com.att.api.mms.service.MMSService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class GetStatusController extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = getFileToken();
            String mmsId = request.getParameter("msgId");

            MMSService srvc = new MMSService(appConfig.getApiFQDN(), token);
            MMSStatus status = srvc.getMMSStatus(mmsId);

            JSONArray infos = new JSONArray();
            for (MMSDeliveryInfo info : status.getInfoList()) {
                infos.put(info.getMessageId())
                    .put(info.getAddress())
                    .put(info.getDeliveryStatus());
            }

            jresponse.put("success", true);
            jresponse.put("text", "ResourceURL: " + status.getResourceUrl());
            jresponse.put("tables", new JSONArray().put(infos));
        } catch (RESTException e) {
            jresponse.put("success", false).put("text", e.getMessage());
        }

        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();
        writer.print(jresponse);
        writer.flush();
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
