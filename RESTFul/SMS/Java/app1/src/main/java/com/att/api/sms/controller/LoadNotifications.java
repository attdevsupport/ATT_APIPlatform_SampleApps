package com.att.api.sms.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.sms.model.SMSReceiveMessage;
import com.att.api.sms.model.SMSReceiveStatus;
import com.att.api.sms.service.SMSFileUtil;

public class LoadNotifications extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        JSONArray statuses = new JSONArray();
        for (final SMSReceiveStatus status : SMSFileUtil.getStatuses()) {
            statuses.put(new JSONArray()
                    .put(status.getMessageId())
                    .put(status.getAddress())
                    .put(status.getDeliveryStatus()));
        }

        JSONArray msgs = new JSONArray();
        for (final SMSReceiveMessage msg : SMSFileUtil.getMsgs()) {
            msgs.put(new JSONArray()
                    .put(msg.getMessageId())
                    .put(msg.getDateTime())
                    .put(msg.getSenderAddress())
                    .put(msg.getSenderAddress())
                    .put(msg.getDestinationAddress())
                    .put(msg.getMessage()));
        }
        JSONObject jresponse = new JSONObject()
            .put("messages", msgs)
            .put("deliveryStatus", statuses);

        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();
        writer.print(jresponse);
        writer.flush();
    }
    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
            //Ignore get requests
            return;
    }
}
