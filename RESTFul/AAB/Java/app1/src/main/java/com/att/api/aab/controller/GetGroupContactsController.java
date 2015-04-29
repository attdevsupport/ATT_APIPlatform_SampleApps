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

package com.att.api.aab.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.aab.service.AABService;
import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;

public class GetGroupContactsController extends APIController {

    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();

        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = (OAuthToken) session.getAttribute("token");
            final AABService srvc = new AABService(appConfig.getApiFQDN(), token);
            final String gid = request.getParameter("getContactsGroupId");
            final String[] ids = srvc.getGroupContacts(gid);


            final JSONArray jheaders = new JSONArray().put("contactId");
            final JSONObject jtable = new JSONObject()
                .put("caption", "Contacts:")
                .put("headers", jheaders);
            final JSONArray jvalues = new JSONArray();
            for (final String id : ids) {
                jvalues.put(new JSONArray().put(id));
            }
            jtable.put("values", jvalues);

            jresponse.put("success", true)
                     .put("tables", new JSONArray().put(jtable));
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
