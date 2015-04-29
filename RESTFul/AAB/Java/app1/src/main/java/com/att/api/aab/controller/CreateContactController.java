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

import org.json.JSONObject;

import com.att.api.aab.service.AABService;
import com.att.api.aab.service.Address;
import com.att.api.aab.service.Contact;
import com.att.api.aab.service.Email;
import com.att.api.aab.service.Gender;
import com.att.api.aab.service.Im;
import com.att.api.aab.service.Phone;
import com.att.api.aab.service.WebURL;
import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;

public class CreateContactController extends APIController {

    private static final long serialVersionUID = 5039680686529624210L;

    private String getParamValue(HttpServletRequest request, String paramName) {
        final String paramValue = request.getParameter(paramName);
        if (paramValue == null || paramValue.equals(""))
            return null;

        return paramValue;
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();

        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = (OAuthToken) session.getAttribute("token");
            final AABService srvc = new AABService(appConfig.getApiFQDN(), token);
            final Contact.Builder builder = new Contact.Builder()
                .setFirstName(getParamValue(request, "createFirstName"))
                .setMiddleName(getParamValue(request, "createMiddleName"))
                .setLastName(getParamValue(request, "createLastName"))
                .setPrefix(getParamValue(request, "createPrefix"))
                .setSuffix(getParamValue(request, "createSuffix"))
                .setNickname(getParamValue(request, "createNickname"))
                .setOrganization(getParamValue(request, "createOrganization"))
                .setJobTitle(getParamValue(request, "createJobTitle"))
                .setAnniversary(getParamValue(request, "createAnniversary"))
                .setSpouse(getParamValue(request, "createSpouse"))
                .setChildren(getParamValue(request, "createChildren"))
                .setHobby(getParamValue(request, "createHobby"))
                .setAssistant(getParamValue(request, "createAssistant"));

            String gender = getParamValue(request, "createGender");
            if (gender != null) {
                gender = gender.toLowerCase();
                if (gender.equals("male") || gender.equals("m")) {
                    builder.setGender(Gender.MALE);
                } else if (gender.equals("female") || gender.equals("f")) {
                    builder.setGender(Gender.FEMALE);
                } else {
                    throw new IllegalArgumentException(
                        "If specified, gender must be either 'male' or 'female'"
                    );
                }
            }
            final int phoneCount 
                = Integer.valueOf(request.getParameter("createPhoneIndex"));
            if (phoneCount > 0) {
                final Phone[] phones = new Phone[phoneCount];
                for (int i = 0; i < phoneCount; ++i) {
                    final String number 
                        = request.getParameter("createPhoneNumber" + i);
                    final boolean pref
                        = request.getParameter("createPhonePref" + i)
                          .equals("True");
                    final String type
                        = request.getParameter("createPhoneType" + i);
                    phones[i] = new Phone(type, number, pref);
                }
                builder.setPhones(phones);
            }
            final int imCount 
                = Integer.valueOf(request.getParameter("createIMIndex"));
            if (imCount > 0) {
                final Im[] ims = new Im[imCount];
                for (int i = 0; i < imCount; ++i) {
                    final String uri 
                        = request.getParameter("createIMUri" + i);
                    final boolean pref
                        = request.getParameter("createIMPref" + i)
                          .equals("True");
                    final String type
                        = request.getParameter("createIMType" + i);
                    ims[i] = new Im(type, uri, pref);
                }
                builder.setIms(ims);
            }
            final int addressCount 
                = Integer.valueOf(request.getParameter("createAddressIndex"));
            if (addressCount > 0) {
                final Address[] addresses = new Address[addressCount];
                for (int i = 0; i < addressCount; ++i) {
                    final boolean pref
                        = request.getParameter("createAddressPref" + i)
                          .equals("True");
                    final String type
                        = request.getParameter("createAddressType" + i);
                    final String poBox
                        = request.getParameter("createAddressPoBox" + i);
                    final String addressLineOne
                        = request.getParameter("createAddressLineOne" + i);
                    final String addressLineTwo
                        = request.getParameter("createAddressLineTwo" + i);
                    final String city
                        = request.getParameter("createAddressCity" + i);
                    final String state
                        = request.getParameter("createAddressState" + i);
                    final String zip
                        = request.getParameter("createAddressZip" + i);
                    final String country
                        = request.getParameter("createAddressCountry" + i);

                    addresses[i] = new Address.Builder()
                        .setPreferred(pref)
                        .setType(type)
                        .setPoBox(poBox)
                        .setAddrLineOne(addressLineOne)
                        .setAddrLineTwo(addressLineTwo)
                        .setCity(city)
                        .setState(state)
                        .setZipcode(zip)
                        .setCountry(country)
                        .build();
                }
                builder.setAddresses(addresses);
            }
            final int emailCount 
                = Integer.valueOf(request.getParameter("createEmailIndex"));
            if (emailCount > 0) {
                final Email[] emails = new Email[emailCount];
                for (int i = 0; i < emailCount; ++i) {
                    final String emailAddr 
                        = request.getParameter("createEmailAddress" + i);
                    final boolean pref
                        = request.getParameter("createEmailPref" + i)
                          .equals("True");
                    final String type
                        = request.getParameter("createEmailType" + i);
                    emails[i] = new Email(type, emailAddr, pref);
                }
                builder.setEmails(emails);
            }
            final int weburlCount 
                = Integer.valueOf(request.getParameter("createWeburlIndex"));
            if (weburlCount > 0) {
                final WebURL[] weburls = new WebURL[weburlCount];
                for (int i = 0; i < weburlCount; ++i) {
                    final String url 
                        = request.getParameter("createWeburl" + i);
                    final boolean pref
                        = request.getParameter("createWeburlPref" + i)
                          .equals("True");
                    final String type
                        = request.getParameter("createWeburlType" + i);
                    weburls[i] = new WebURL(type, url, pref);
                }
                builder.setWeburls(weburls);
            }

            final String location = srvc.createContact(builder.build());
            jresponse.put("success", true).put("text", location);
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
