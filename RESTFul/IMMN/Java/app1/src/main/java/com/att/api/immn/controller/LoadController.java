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
import com.att.api.oauth.OAuthToken;
import com.att.api.util.DateUtil;

public class LoadController extends APIController {

    /**
     * Generated serial id.
     */
    private static final long serialVersionUID = 6145106010438821277L;

    public void doPost(HttpServletRequest request,
            HttpServletResponse response) throws ServletException, IOException {
        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token"); 
        final Boolean authenticated = (token != null && !token.isAccessTokenExpired());
            
        final String FQDN = appConfig.getOauthFQDN();
        final String clientId = appConfig.getClientId();
        final String scope = appConfig.getProperty("scope");
        final String redirectUri = appConfig.getProperty("redirectUri");
        final String redirect_url = FQDN + "/oauth/v4/authorize?client_id=" +
            clientId + "&scope=" + scope + "&redirect_uri=" + redirectUri; 
        final boolean subscriptionActive 
            = session.getAttribute("subscriptionId") != null;

        JSONObject jchannel = new JSONObject()
            .put("channelId", appConfig.getProperty("channelId"))
            .put("channelType", appConfig.getProperty("channelType"))
            .put("maxEvents", appConfig.getProperty("maxEvents"))
            .put("contentType", appConfig.getProperty("contentType"));

        JSONObject jresponse = new JSONObject()
            .put("authenticated", authenticated)
            .put("redirect_url", redirect_url)
            .put("server_time", DateUtil.getTime())
            .put("download", this.appConfig.getProperty("linkDownload"))
            .put("github", this.appConfig.getProperty("linkGithub"))
            .put("notificationChannel", jchannel)
            .put("subscriptionActive", subscriptionActive);

        final String savedData = (String) session.getAttribute("savedData");
        if (savedData != null) {
            final JSONObject jsavedData = new JSONObject(savedData);
            jresponse.put("savedData", jsavedData);
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
