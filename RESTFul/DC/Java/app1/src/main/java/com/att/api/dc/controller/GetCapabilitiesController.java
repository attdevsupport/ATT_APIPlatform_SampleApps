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
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.dc.model.DCResponse;
import com.att.api.dc.service.DCService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class GetCapabilitiesController extends APIController {

    private static final long serialVersionUID = 8043092926070964289L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();

        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = (OAuthToken) session.getAttribute("token");
            final DCService srvc = new DCService(appConfig.getApiFQDN(), token);
            final DCResponse dcResponse = srvc.getDeviceCapabilities();

            final JSONArray tables = new JSONArray();
            final JSONArray headers = new JSONArray()
                .put("TypeAllocationCode")
                .put("Name")
                .put("Vendor")
                .put("Model")
                .put("FirmwareVersion")
                .put("UaProf")
                .put("MmsCapable")
                .put("AssistedGps")
                .put("LocationTechnology")
                .put("DeviceBrowser")
                .put("WapPushCapable");

            final JSONArray valuesEntry = new JSONArray()
                .put(dcResponse.getTypeAllocationCode())
                .put(dcResponse.getName())
                .put(dcResponse.getVendor())
                .put(dcResponse.getModel())
                .put(dcResponse.getFirmwareVersion())
                .put(dcResponse.getUaProf())
                .put(dcResponse.isMmsCapable())
                .put(dcResponse.isAssistedGpsCapable())
                .put(dcResponse.getLocationTechnology())
                .put(dcResponse.getDeviceBrowser())
                .put(dcResponse.isWapPushCapable());

            tables.put(
                new JSONObject()
                .put("caption", "Message Index Info:")
                .put("headers", headers)
                .put("values", new JSONArray().put(valuesEntry))
            );
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
