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

import org.json.JSONObject;

public final class QuickContact {
    private String contactId;
    private String formattedName;
    private String firstName;
    private String middleName;
    private String lastName;
    private String prefix;
    private String suffix;
    private String nickname;
    private String organization;
    private Address address;
    private Phone phone;
    private Email email;
    private Im im;

    private QuickContact() {
        this.contactId = null;
        this.formattedName = null;
        this.firstName = null;
        this.middleName = null;
        this.lastName = null;
        this.prefix = null;
        this.suffix = null;
        this.nickname = null;
        this.organization = null;
        this.address = null;
        this.phone = null;
        this.email = null;
        this.im = null;
    }

    public String getContactId() {
        return contactId;
    }

    public String getFormattedName() {
        return formattedName;
    }

    public String getFirstName() {
        return firstName;
    }

    public String getMiddleName() {
        return middleName;
    }

    public String getLastName() {
        return lastName;
    }

    public String getPrefix() {
        return prefix;
    }

    public String getSuffix() {
        return suffix;
    }

    public String getNickname() {
        return nickname;
    }

    public String getOrganization() {
        return organization;
    }

    public Address getAddress() {
        return address;
    }

    public Phone getPhone() {
        return phone;
    }

    public Email getEmail() {
        return email;
    }

    public Im getIm() {
        return im;
    }

    public static QuickContact valueOf(JSONObject jobj) {
        QuickContact contact = new QuickContact();
        if (jobj.has("contactId"))
            contact.contactId = jobj.getString("contactId");
        if (jobj.has("formattedName"))
            contact.formattedName = jobj.getString("formattedName");
        if (jobj.has("firstName"))
            contact.firstName = jobj.getString("firstName");
        if (jobj.has("middleName"))
            contact.middleName = jobj.getString("middleName");
        if (jobj.has("lastName"))
            contact.lastName = jobj.getString("lastName");
        if (jobj.has("prefix"))
            contact.prefix = jobj.getString("prefix");
        if (jobj.has("suffix"))
            contact.suffix = jobj.getString("suffix");
        if (jobj.has("nickName"))
            contact.nickname = jobj.getString("nickName");

        if (jobj.has("phone")) {
            contact.phone = Phone.valueOf(jobj.getJSONObject("phone"));
        }
        if (jobj.has("address")) {
            contact.address = Address.valueOf(jobj.getJSONObject("address"));
        }
        if (jobj.has("email")) {
            contact.email = Email.valueOf(jobj.getJSONObject("email"));
        }
        if (jobj.has("im")) {
            contact.im = Im.valueOf(jobj.getJSONObject("im"));
        }

        return contact;
    }
}
