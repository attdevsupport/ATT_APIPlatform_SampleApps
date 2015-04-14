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
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;
import com.att.api.webhooks.service.GetSubscriptionResponse;
import com.att.api.webhooks.service.WebhooksService;

public class GetSubscriptionController extends APIController {
    private static final long serialVersionUID = -6258405486108130230L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();
        JSONObject jresponse = new JSONObject();
        try {
            if (session.getAttribute("subscriptionId") == null) {
                throw new RESTException(
                    "You must first create a subscription."
                );
            }
            final OAuthToken token = (OAuthToken) session.getAttribute("token");
            final String channelId = appConfig.getProperty("channelId");
            final String subscriptionId 
                = (String) session.getAttribute("subscriptionId");
            final String fqdn = appConfig.getApiFQDN();
            final WebhooksService srvc = new WebhooksService(fqdn, token);
            GetSubscriptionResponse getResponse 
                = srvc
                    .getNotificationSubscriptionDetails(
                        channelId, subscriptionId
                    );
            JSONArray jheaders = new JSONArray()
                .put("Subscription Id")
                .put("Expires In")
                .put("Queues")
                .put("Callback Data");
            JSONArray jvalues = new JSONArray()
                .put(getResponse.getSubscription().getSubscriptionId())
                .put(getResponse.getSubscription().getExpiresIn())
                .put(getResponse.getSubscription().getEvents())
                .put(getResponse.getSubscription().getCallbackData());
            JSONObject jtable = new JSONObject()
                .put("caption", "Subscription Details")
                .put("headers", jheaders)
                .put("values", new JSONArray().put(jvalues));
            JSONArray jtables = new JSONArray().put(jtable);
            jresponse.put("success", true).put("tables", jtables);
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
