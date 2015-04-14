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

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.service.Delta;
import com.att.api.immn.service.DeltaChange;
import com.att.api.immn.service.DeltaResponse;
import com.att.api.immn.service.IMMNService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class GetDeltaController extends APIController {

    private static final long serialVersionUID = -1114451412915024437L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token");
        IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
        final String msgState = request.getParameter("msgState");

        JSONObject jresponse = new JSONObject();
        try {
            final DeltaResponse deltaResponse = srvc.getDelta(msgState);
            final JSONArray tables = new JSONArray();
            for (final Delta delta : deltaResponse.getDeltas()) {
                final JSONArray values = new JSONArray();
                for (final DeltaChange dc : delta.getAdds()) {
                    values.put(new JSONArray().put("Add").
                    put(dc.getMessageId()) .put(dc.isFavorite())
                    .put(dc.isUnread()));
                }
                for (final DeltaChange dc : delta.getDeletes()) {
                    values.put(new JSONArray().put("Delete")
                    .put(dc.getMessageId()).put(dc.isFavorite())
                    .put(dc.isUnread()));
                }
                for (final DeltaChange dc : delta.getUpdates()) {
                    values.put(new JSONArray().put("Update")
                    .put(dc.getMessageId()).put(dc.isFavorite())
                    .put(dc.isUnread()));
                }
                tables.put(
                    new JSONObject().put(
                        "caption", "Delta Type: " + delta.getType()
                    ).put(
                        "headers", new JSONArray().put("Delta Operation")
                                   .put("MessageId").put("Favorite")
                                   .put("Unread")
                    ).put("values", values)
                );
            }
            jresponse.put("success", true).put("tables", tables);
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
