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

import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.service.IMMNService;
import com.att.api.immn.service.SendResponse;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class SendMsgController extends APIController {
    private static final long serialVersionUID = -6695332131611171489L;

    public void doPost(HttpServletRequest request,
            HttpServletResponse response) throws ServletException, IOException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token"); 

        final String addr = request.getParameter("address");
        final String[] addrs = addr.split(",");
        for (int i = 0; i < addrs.length; ++i)
            addrs[i] = addrs[i].trim();

        final String msg = request.getParameter("sendMsgInput");
        final String subject = request.getParameter("sendSubjectInput");
        final boolean group = request.getParameter("groupCheckbox") != null;
        // TODO: handle attachments

        IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
        JSONObject jresponse = new JSONObject();
        try {
            SendResponse sendResponse = null;
            if (!subject.isEmpty() || group) {
                sendResponse = srvc.sendMessage(
                    addrs, msg, subject, group, /* TODO: finish attachments */ null
                );
            } else { 
                sendResponse = srvc.sendMessage(addrs, msg);
            }
            final String id = sendResponse.getId();
            jresponse.put("success", true).put("text", "id: " + id);
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
