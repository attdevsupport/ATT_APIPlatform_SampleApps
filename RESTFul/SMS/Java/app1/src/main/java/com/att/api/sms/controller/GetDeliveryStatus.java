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
import com.att.api.sms.model.SMSDeliveryInfo;
import com.att.api.sms.model.SMSStatus;
import com.att.api.sms.service.SMSService;

public class GetDeliveryStatus extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        JSONObject jresponse = new JSONObject();
        try {
            final OAuthToken token = getFileToken();
            final String msgid = request.getParameter("messageId");
            request.getSession().setAttribute("statusId", msgid);

            SMSService service = new SMSService(appConfig.getApiFQDN(), token);
            SMSStatus r = service.getSMSDeliveryStatus(msgid);

            JSONArray deliveryInfo = new JSONArray();
            SMSDeliveryInfo[] infoList = r.getInfoList();
            for (SMSDeliveryInfo info : infoList) {
                deliveryInfo.put(new JSONArray()
                        .put(info.getMessageId())
                        .put(info.getAddress())
                        .put(info.getDeliveryStatus()));
            }

            jresponse.put("success", true);
            jresponse.put("tables", new JSONArray().put(new JSONObject()
                        .put("caption", "Resource URL: " + r.getResourceUrl())
                        .put("headers", new JSONArray()
                            .put("Message ID")
                            .put("Address")
                            .put("Delivery Status"))
                        .put("values", deliveryInfo)));
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
