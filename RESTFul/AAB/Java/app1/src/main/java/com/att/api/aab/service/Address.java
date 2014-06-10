/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2014.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.aab.service;

import org.json.JSONObject;

public final class Address {
    public static class Builder {
        private String type;
        private Boolean preferred;
        private String poBox;
        private String addrLineOne;
        private String addrLineTwo;
        private String city;
        private String state;
        private String zipcode;
        private String country;

        public Builder() {
            this.type = null;
            this.preferred = null;
            this.poBox = null;
            this.addrLineOne = null;
            this.addrLineTwo = null;
            this.city = null;
            this.state = null;
            this.zipcode = null;
            this.country = null;
        }

        public Builder setType(String type) {
            this.type = type; return this;
        }

        public Builder setPreferred(Boolean preferred) {
            this.preferred = preferred; return this;
        }

        public Builder setPoBox(String poBox) {
            this.poBox = poBox; return this;
        }

        public Builder setAddrLineOne(String addrLineOne) {
            this.addrLineOne = addrLineOne; return this;
        }

        public Builder setAddrLineTwo(String addrLineTwo) {
            this.addrLineTwo = addrLineTwo; return this;
        }

        public Builder setCity(String city) {
            this.city = city; return this;
        }

        public Builder setState(String state) {
            this.state = state; return this;
        }

        public Builder setZipcode(String zipcode) {
            this.zipcode = zipcode; return this;
        }

        public Builder setCountry(String country) {
            this.country = country; return this;
        }

        public Address build() {
            return new Address(this);
        }

    }

    private final String type;
    private final Boolean preferred;
    private final String poBox;
    private final String addrLineOne;
    private final String addrLineTwo;
    private final String city;
    private final String state;
    private final String zipcode;
    private final String country;

    private Address(Address.Builder builder) {
        this.type = builder.type;
        this.preferred = builder.preferred;
        this.poBox = builder.poBox;
        this.addrLineOne = builder.addrLineOne;
        this.addrLineTwo = builder.addrLineTwo;
        this.city = builder.city;
        this.state = builder.state;
        this.zipcode = builder.zipcode;
        this.country = builder.country;
    }

    public Address(JSONObject jobj) {
        this.type = jobj.has("type") ? jobj.getString("type") : null;

        this.preferred = jobj.has("preferred") ? jobj.getString("preferred")
            .toLowerCase().equals("true") : null;

        this.poBox = jobj.has("poBox") ? jobj.getString("poBox") : null;

        this.addrLineOne 
            = jobj.has("addressLine1") ? jobj.getString("addressLine1") : null;

        this.addrLineTwo
            = jobj.has("addressLine2") ? jobj.getString("addressLine2") : null;

        this.city = jobj.has("city") ? jobj.getString("city") : null;
        this.state = jobj.has("state") ? jobj.getString("state") : null;
        this.zipcode = jobj.has("zipcode") ? jobj.getString("zipcode") : null;
        this.country = jobj.has("country") ? jobj.getString("country") : null;
    }

    public String getType() {
        return type;
    }

    public Boolean getPreferred() {
        return preferred;
    }

    public Boolean isPreferred() {
        return preferred;
    }

    public String getPoBox() {
        return poBox;
    }
    public String getAddressLineOne() {
        return addrLineOne;
    }

    public String getAddressLineTwo() {
        return addrLineTwo;
    }

    public String getAddrLineOne() {
        return addrLineOne;
    }

    public String getAddrLineTwo() {
        return addrLineTwo;
    }

    public String getCity() {
        return city;
    }

    public String getState() {
        return state;
    }

    public String getZipcode() {
        return zipcode;
    }

    public String getCountry() {
        return country;
    }

    public JSONObject toJson() {
        final JSONObject jobj = new JSONObject();
        final String[] keys = { 
            "type", "preferred", "poBox", "addressLine1",
            "addrLineTwo", "city", "state", "zipcode", "country"
        };
        
        String prefString = null;
        if (isPreferred() != null) {
            prefString = isPreferred() ? "TRUE" : "FALSE";
        }
        final String[] values = {
            getType(), prefString, getPoBox(), getAddrLineOne(),
            getAddrLineTwo(), getCity(), getState(), getZipcode(), getCountry()
        };

        for (int i = 0; i < keys.length; ++i) {
            // do not add any null values to the json object
            if (values[i] == null) continue;
            jobj.put(keys[i], values[i]);
        }

        return jobj;
    }

    public static Address valueOf(JSONObject jobj) {
        return new Address(jobj);
    }

}
