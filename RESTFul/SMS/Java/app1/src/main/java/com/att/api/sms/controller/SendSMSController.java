package com.att.api.sms.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.sms.model.SMSSendResponse;
import com.att.api.sms.service.SMSService;


public class SendSMSController extends APIController {
    private static final long serialVersionUID = 1L;
    private final String DEFAULT_MESSAGE = "AT&T Sample Message";

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = getFileToken();
            SMSService service = new SMSService(appConfig.getApiFQDN(), token);

            String addr = request.getParameter("address");
            String msg = DEFAULT_MESSAGE;
            boolean getNotification 
                = request.getParameter("deliveryNotificationStatus") != null;

            SMSSendResponse r = service.sendSMS(addr, msg, getNotification);

            JSONArray sentSms = new JSONArray().put(r.getMessageId());
            if (r.getResourceUrl() != null)
                sentSms.put(r.getResourceUrl());
            else
                sentSms.put("-");

            JSONArray jtablearr = new JSONArray()
                .put(new JSONObject()
                    .put("caption", "Message sent successfully!")
                    .put("headers", 
                        new JSONArray().put("Message ID").put("Resource URL"))
                    .put("values", new JSONArray().put(sentSms)));

            jresponse.put("success", true);
            jresponse.put("tables", jtablearr);
        } catch (Exception e) {
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
