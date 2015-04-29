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

package com.att.api.dc.controller;

import java.io.IOException;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class OAuthController extends APIController {

    private static final long serialVersionUID = -4945596741398732461L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        /* TODO: handle error case
         * the below code assumes that the 'code' parameter is present.
         * However, error/error_description should also be handled.
         */
        final HttpSession session = request.getSession();
        final String clientId = appConfig.getClientId();
        final String clientSecret = appConfig.getClientSecret();

        final String code = (String) request.getParameter("code");
        if (code != null) {
            final OAuthService service = new OAuthService(
                    appConfig.getOauthFQDN(), clientId, clientSecret);
            OAuthToken token;
            try {
                token = service.getTokenUsingCode(code);
                session.setAttribute("token", token);
            } catch (RESTException e) {
                // TODO Auto-generated catch block
                // TODO: handle this exception
                e.printStackTrace();
            }
        }
        final String redirect = appConfig.getProperty("indexUri");
        response.sendRedirect(redirect);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        doPost(request, response);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
