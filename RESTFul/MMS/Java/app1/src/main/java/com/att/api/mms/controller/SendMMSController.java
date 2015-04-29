package com.att.api.mms.controller;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.mms.model.SendMMSResponse;
import com.att.api.mms.service.MMSService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;


public class SendMMSController extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        JSONObject jresponse = new JSONObject();
        try {
            String addr = request.getParameter("address");
            String subject = request.getParameter("sendMsgInput");
            String attachment = request.getParameter("attachmentInput");
            boolean notify = request.getParameter("receiveStatus") != null;

            String dir = appConfig.getProperty("attachmentsDir");
            String[] fnames;
            if (attachment.trim().equalsIgnoreCase("none")) {
                fnames = new String[] {};
            } else {
                final String path = getServletContext().getRealPath("/") + dir
                    + File.separator + attachment;
                fnames = new String[] { path };
            }

            OAuthToken token = getFileToken();
            final MMSService srvc = new MMSService(appConfig.getApiFQDN(),
                    token);

            SendMMSResponse mmsResponse = srvc.sendMMS(addr, fnames, subject,
                    null, notify); /* priority = null... for now */

            mmsResponse.getMessageId();

            jresponse.put("success", true);
            jresponse.put("tables", new JSONArray()
                    .put(new JSONObject()
                        .put("headers", new JSONArray()
                            .put("MessageId")
                            .put("ResourceURL"))
                        .put("values", new JSONArray().put(new JSONArray()
                                .put(mmsResponse.getMessageId())
                                .put(mmsResponse.getResourceUrl())
                                ))));

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
