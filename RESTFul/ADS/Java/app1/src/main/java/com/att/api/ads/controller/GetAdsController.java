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

package com.att.api.ads.controller;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.ads.service.ADSArguments;
import com.att.api.ads.service.ADSResponse;
import com.att.api.ads.service.ADSService;
import com.att.api.ads.service.Category;
import com.att.api.ads.service.Gender;
import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;

public class GetAdsController extends APIController {

    private static final long serialVersionUID = 161244360288778547L;

    private final String valueOrNull(final String value) {
        if (value == null) {
            return null;
        }

        return value.equals("") ? null : value;
    }

    private final int parseInt(final String value) {
        if (valueOrNull(value) == null) {
            return -1;
        }

        return Integer.parseInt(value);
    }

    private final double parseDouble(final String value) {
        if (valueOrNull(value) == null) {
            return -1;
        }

        return Double.parseDouble(value);
    }

    private ADSArguments parseArgs(final HttpServletRequest request) {
        final Category category 
            = Category.fromString(request.getParameter("category"));

        final String udid = appConfig.getProperty("udid");
        final String userAgent = appConfig.getProperty("userAgent");

        int width = -1;
        int height = -1;
        final String mmaSize = request.getParameter("mmaSize");
        if (mmaSize != null && !mmaSize.equals("")) {
            final String[] vals = mmaSize.split(" x ");
            width = Integer.parseInt(vals[0]);
            height = Integer.parseInt(vals[1]);
        }
        String[] keywords = null;
        if (request.getParameter("keywords") != null
                && !request.getParameter("keywords").equals("")) {

            keywords = request.getParameter("keywords").split(",");
        }

        Gender gender = null;
        final String genderStr = request.getParameter("gender");
        if (genderStr != null) {
            if (genderStr.equals("M")) {
                gender = Gender.MALE;
            } else if (genderStr.equals("F")) {
                gender = Gender.FEMALE;
            }
        }

        final ADSArguments args = new ADSArguments
            .Builder(category, userAgent, udid)
            .maxWidth(width)
            .minWidth(width)
            .maxHeight(height)
            .minHeight(height)
            .setKeywords(keywords)
            .setAgeGroup(valueOrNull(request.getParameter("ageGroup")))
            .setGender(gender)
            .setZipCode(parseInt(request.getParameter("zipCode")))
            .setCity(valueOrNull(request.getParameter("city")))
            .setAreaCode(parseInt(request.getParameter("areaCode")))
            .setCountry(valueOrNull(request.getParameter("country")))
            .setLatitude(parseDouble(request.getParameter("latitude")))
            .setLongitude(parseDouble(request.getParameter("longitude")))
            .build();

        return args;
    }

    public void doPost(HttpServletRequest request,
            HttpServletResponse response) throws ServletException, IOException {

        final JSONObject jresponse = new JSONObject();
        try {
        
            final OAuthToken token = this.getFileToken();
            final ADSService adsService 
                = new ADSService(appConfig.getApiFQDN(), token);
            ADSResponse adsResponse
                = adsService.getAdvertisement(this.parseArgs(request));

            if (adsResponse == null) {
                jresponse
                    .put("success", true)
                    .put("text", "No Ads were returned");
            } else {
                final JSONArray jheaders = new JSONArray()
                    .put("Type").put("ClickUrl");

                final JSONArray jvalues = new JSONArray();
                jvalues.put(
                    new JSONArray()
                        .put(adsResponse.getType())
                        .put(adsResponse.getClickUrl())
                );
                final JSONObject jtable = new JSONObject()
                    .put("caption", "Speech Response:")
                    .put("headers", jheaders)
                    .put("values", jvalues);
                final JSONArray jtables = new JSONArray().put(jtable);

                jresponse.put("success", true).put("tables", jtables);
            }
        } catch (Exception e) {
            jresponse.put("success", false).put("text", e.getMessage());
            e.printStackTrace();
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
