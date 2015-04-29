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
import com.att.api.sms.model.SMSGetResponse;
import com.att.api.sms.model.SMSMessage;
import com.att.api.sms.service.SMSService;

public class GetMessages extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        JSONObject jresponse = new JSONObject();
        try {
            String shortCode = appConfig.getProperty("getMsgsShortCode");
            if (shortCode == null) return;

            OAuthToken token = getFileToken();

            SMSService service = new SMSService(appConfig.getApiFQDN(), token);
            SMSGetResponse r = service.getSMS(shortCode);

            JSONArray values = new JSONArray();
            for (SMSMessage msg : r.getMessages())
                values.put(new JSONArray()
                        .put(msg.getMessageId())
                        .put(msg.getSenderAddress())
                        .put(msg.getMessage()));

            jresponse.put("success", true);
            jresponse.put("tables", new JSONArray()
                    .put(new JSONObject()
                        .put("caption", "Message List Information:")
                        .put("headers", new JSONArray()
                            .put("Number Of Messages In This Batch")
                            .put("Resource Url")
                            .put("Total Number Of Pending Messages"))
                        .put("values", new JSONArray().put(new JSONArray()
                                .put(r.getNumberOfMessages())
                                .put(r.getResourceUrl())
                                .put(r.getPendingMessages()))))
                    .put(new JSONObject()
                        .put("caption", "Messages")
                        .put("headers", new JSONArray()
                            .put("Message ID")
                            .put("Sender Address")
                            .put("Message"))
                        .put("values", values)));
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
