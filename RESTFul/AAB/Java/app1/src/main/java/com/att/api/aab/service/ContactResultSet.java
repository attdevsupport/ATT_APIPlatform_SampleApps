/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/*
 * Copyright 2014 AT&T
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

package com.att.api.aab.service;

import org.json.JSONArray;
import org.json.JSONObject;

public final class ContactResultSet {
    private final int totalRecords;
    private final int totalPages;
    private final int currentPageIndex;
    private final int previousPage;
    private final int nextPage;
    private Contact[] contacts;
    private QuickContact[] quickContacts;

    private ContactResultSet(JSONObject jobj) {
        this.totalRecords = jobj.getInt("totalRecords");
        this.totalPages = jobj.getInt("totalPages");
        this.currentPageIndex = jobj.getInt("currentPageIndex");
        this.previousPage = jobj.getInt("previousPage");
        this.nextPage = jobj.getInt("nextPage");
        if (jobj.has("contacts")) {
            JSONArray jarr = jobj.getJSONObject("contacts").getJSONArray("contact");
            Contact[] contacts = new Contact[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                contacts[i] = Contact.valueOf(jarr.getJSONObject(i));
            }
            this.contacts = contacts;
        } 
        if (jobj.has("quickContacts")) {
            JSONArray jarr = jobj.getJSONObject("quickContacts").getJSONArray("quickContact");
            QuickContact[] quickContacts = new QuickContact[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                quickContacts[i] = QuickContact.valueOf(jarr.getJSONObject(i));
            }
            this.quickContacts = quickContacts;
        }
    }

    public int getTotalRecords() {
        return totalRecords;
    }

    public int getTotalPages() {
        return totalPages;
    }

    public int getCurrentPageIndex() {
        return currentPageIndex;
    }

    public int getPreviousPage() {
        return previousPage;
    }

    public int getNextPage() {
        return nextPage;
    }

    public Contact[] getContacts() {
        return contacts;
    }

    public QuickContact[] getQuickContacts() {
        return quickContacts;
    }

    public static ContactResultSet valueOf(JSONObject jobj) {
        return new ContactResultSet(jobj);
    }

}
