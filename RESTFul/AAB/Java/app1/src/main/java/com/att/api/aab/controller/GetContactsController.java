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
import com.att.api.aab.service.Address;
import com.att.api.aab.service.Contact;
import com.att.api.aab.service.Email;
import com.att.api.aab.service.Im;
import com.att.api.aab.service.Phone;
import com.att.api.aab.service.ContactResultSet;
import com.att.api.aab.service.QuickContact;
import com.att.api.aab.service.WebURL;
import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;

public class GetContactsController extends APIController {

    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();

        JSONObject jresponse = new JSONObject();
        try {
            OAuthToken token = (OAuthToken) session.getAttribute("token");
            final AABService srvc = new AABService(appConfig.getApiFQDN(), token);
            final JSONArray jtables = new JSONArray();
            final String searchVal = request.getParameter("contactsSearchValue");

            ContactResultSet rs = srvc.getContacts(searchVal);
            Contact[] contacts = rs.getContacts();
            if (contacts != null) {
                for (final Contact contact : contacts) {
                    final String contactId = contact.getContactId();
                    jtables.put(AabUtil.generateContactsTable(contact));
                    final Phone[] phones = contact.getPhones();
                    if (phones != null && phones.length > 0) {
                        jtables.put(AabUtil.generatePhonesTable(contactId, phones));
                    }
                    final Email[] emails = contact.getEmails();
                    if (emails != null && emails.length > 0) {
                        jtables.put(AabUtil.generateEmailsTable(contactId, emails));
                    }
                    final Im[] ims = contact.getIms();
                    if (ims != null && ims.length > 0) {
                        jtables.put(AabUtil.generateImsTable(contactId, ims));
                    }
                    final Address[] addresses = contact.getAddresses();
                    if (addresses != null && addresses.length > 0) {
                        jtables.put(AabUtil.generateAddressesTable(contactId, addresses));
                    }
                    final WebURL[] weburls = contact.getWeburls();
                    if (weburls != null && weburls.length > 0) {
                        jtables.put(AabUtil.generateWeburlsTable(contactId, weburls));
                    }
                }
            }
            QuickContact[] quickContacts = rs.getQuickContacts();
            if (quickContacts != null) {
                for (final QuickContact quickContact : quickContacts) {
                    final String contactId = quickContact.getContactId();
                    jtables.put(AabUtil.generateQuickContactsTable(quickContact));
                    if (quickContact.getPhone() != null) {
                        jtables.put(
                            AabUtil.generatePhonesTable(
                                contactId, new Phone[] {quickContact.getPhone()}
                            )
                        );
                    }
                    if (quickContact.getEmail() != null) {
                        jtables.put(
                            AabUtil.generateEmailsTable(
                                contactId, new Email[] {quickContact.getEmail()}
                            )
                        );
                    }
                    if (quickContact.getIm() != null) {
                        jtables.put(
                            AabUtil.generateImsTable(
                                contactId, new Im[] {quickContact.getIm()}
                            )
                        );
                    }
                    if (quickContact.getAddress() != null) {
                        jtables.put(
                            AabUtil.generateAddressesTable(
                                contactId, new Address[] {quickContact.getAddress()}
                            )
                        );
                    }
                }
            }

            jresponse.put("success", true)
                     .put("tables", jtables);
        } catch (Exception e) {
            e.printStackTrace();
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
