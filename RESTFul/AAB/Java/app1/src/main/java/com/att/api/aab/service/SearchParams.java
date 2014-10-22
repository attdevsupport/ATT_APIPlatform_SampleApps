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

public final class SearchParams {
    public static class Builder {
        private String formattedName;
        private String firstName;
        private String middleName;
        private String lastName;
        private String nickname;
        private String organization;
        private String email;
        private String phone;
        private String addressLineOne;
        private String addressLineTwo;
        private String city;
        private String zipcode;
        
        public Builder() {
        }

        public Builder setFormattedName(String formattedName) {
            this.formattedName = formattedName; return this;
        }

        public Builder setFirstName(String firstName) {
            this.firstName = firstName; return this;
        }

        public Builder setMiddleName(String middleName) {
            this.middleName = middleName; return this;
        }

        public Builder setLastName(String lastName) {
            this.lastName = lastName; return this;
        }

        public Builder setNickname(String nickname) {
            this.nickname = nickname; return this;
        }

        public Builder setOrganization(String organization) {
            this.organization = organization; return this;
        }

        public Builder setEmail(String email) {
            this.email = email; return this;
        }

        public Builder setPhone(String phone) {
            this.phone = phone; return this;
        }

        public Builder setAddressLineOne(String addressLineOne) {
            this.addressLineOne = addressLineOne; return this;
        }

        public Builder setAddressLineTwo(String addressLineTwo) {
            this.addressLineTwo = addressLineTwo; return this;
        }

        public Builder setCity(String city) {
            this.city = city; return this;
        }

        public Builder setZipcode(String zipcode) {
            this.zipcode = zipcode; return this;
        }

    }

    private final String formattedName;
    private final String firstName;
    private final String middleName;
    private final String lastName;
    private final String nickname;
    private final String organization;
    private final String email;
    private final String phone;
    private final String addressLineOne;
    private final String addressLineTwo;
    private final String city;
    private final String zipcode;

    private SearchParams(SearchParams.Builder builder) {
        this.formattedName = builder.formattedName;
        this.firstName = builder.firstName;
        this.middleName = builder.middleName;
        this.lastName = builder.lastName;
        this.nickname = builder.nickname;
        this.organization = builder.organization;
        this.email = builder.email;
        this.phone = builder.phone;
        this.addressLineOne = builder.addressLineOne;
        this.addressLineTwo = builder.addressLineTwo;
        this.city = builder.city;
        this.zipcode = builder.zipcode;
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

    public String getNickname() {
        return nickname;
    }

    public String getOrganization() {
        return organization;
    }

    public String getEmail() {
        return email;
    }

    public String getPhone() {
        return phone;
    }

    public String getAddressLineOne() {
        return addressLineOne;
    }

    public String getAddressLineTwo() {
        return addressLineTwo;
    }

    public String getCity() {
        return city;
    }

    public String getZipcode() {
        return zipcode;
    }
}
