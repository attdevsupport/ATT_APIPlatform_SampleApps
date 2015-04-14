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
import java.util.LinkedList;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;
import com.att.api.util.DateUtil;
import com.att.api.webhooks.service.UpdateSubscriptionArgs;
import com.att.api.webhooks.service.UpdateSubscriptionResponse;
import com.att.api.webhooks.service.WebhooksService;

public class UpdateSubscriptionController extends APIController {
    private static final long serialVersionUID = -2395204041002509313L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();
        final List<String> events = new LinkedList<String>();
        JSONObject jresponse = new JSONObject();
        try {
            if (session.getAttribute("subscriptionId") == null) {
                throw new RESTException(
                    "You must first create a subscription."
                );
            }
            if (request.getParameter("updateSubscriptionText") != null) {
                events.add("TEXT");
            }
            if (request.getParameter("updateSubscriptionMms") != null) {
                events.add("MMS");
            }
            if (events.size() == 0) {
                throw new RESTException(
                    "You must select at least one of Text or MMS."
                );
            }
            final OAuthToken token = (OAuthToken) session.getAttribute("token");
            final String subscriptionId 
                = (String) session.getAttribute("subscriptionId");
            final String fqdn = appConfig.getApiFQDN();
            final WebhooksService srvc = new WebhooksService(fqdn, token);
            final String channelId = appConfig.getProperty("channelId");
            final Integer expiresIn = 
                Integer.valueOf(appConfig.getProperty("expiresIn"));
            final String callbackData = request.getParameter("callbackData");
            final UpdateSubscriptionArgs args = new UpdateSubscriptionArgs(
                channelId, subscriptionId, 
                events.toArray(new String[events.size()]), callbackData, 
                expiresIn
            );
            final UpdateSubscriptionResponse updateResponse
                = srvc.updateNotificationSubscription(args);
            session.setAttribute("subscriptionExpiry",
                DateUtil.xtimestamp() 
                + updateResponse.getSubscription().getExpiresIn()
            );
            jresponse.put("success", true).put("text", "Subscription updated.");
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
