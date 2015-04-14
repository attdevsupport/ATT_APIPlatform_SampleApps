/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.att.api.immn.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.commons.codec.binary.Base64;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.service.IMMNService;
import com.att.api.immn.service.MessageContent;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class GetMsgContentController extends APIController {

    private static final long serialVersionUID = -4440218374177112836L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token");
        IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
        final String msgId = request.getParameter("contentMsgId");
        final String partNumb = request.getParameter("contentPartNumber");

        JSONObject jresponse = new JSONObject();
        try {
            final MessageContent msgContent 
                = srvc.getMessageContent(msgId, partNumb);
            jresponse.put("success", true);
            final String type = msgContent.getContentType().toLowerCase();
            /* TODO: handle unknown type */
            if (type.contains("text")) {
                jresponse.put("text",
                    "Message Content: " + new String(msgContent.getContent())
                );
            } else if (type.contains("image")) {
                final byte[] binary = msgContent.getContent();
                final String base64 = new String(Base64.encodeBase64(binary));
                final JSONObject image = new JSONObject()
                    .put("type", msgContent.getContentType())
                    .put("base64", base64);
                jresponse.put("image", image);
            } else if (type.contains("video")) {
                final byte[] binary = msgContent.getContent();
                final String base64 = new String(Base64.encodeBase64(binary));
                final JSONObject image = new JSONObject()
                    .put("type", msgContent.getContentType())
                    .put("base64", base64);
                jresponse.put("video", image);
            } else if (type.contains("audio")) {
                final byte[] binary = msgContent.getContent();
                final String base64 = new String(Base64.encodeBase64(binary));
                final JSONObject image = new JSONObject()
                    .put("type", msgContent.getContentType())
                    .put("base64", base64);
                jresponse.put("audio", image);
            }
        } catch (RESTException re) {
            jresponse.put("success", false).put("text", re.getMessage());
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
