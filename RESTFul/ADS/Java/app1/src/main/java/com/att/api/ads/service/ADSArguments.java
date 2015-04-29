/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

package com.att.api.ads.service;

/**
 * Used to hold parameters sent to version 1 of the Advertising API.
 *
 * <p>
 * This class uses the builder pattern for object creation.
 * </p>
 *
 * An example of how to create an ADSArguments object can be found below:
 * <pre>
 * <code>
 * ADSArguments args = new ADSArguments.Builder(Category.AUTO, "ua", "udid")
 *     .setCity("Seattle")
 *     .setAreaCode(206)
 *     .build();
 * </code>
 * </pre>
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 * @see <a href="https://developer.att.com/docs/apis/rest/1/Advertising">Advertising Documentation</a>
 */
public final class ADSArguments {

    public static class Builder {
        private final Category category;
        private final String userAgent;
        private final String udid;

        private Gender gender;
        private int zipCode;
        private int areaCode;
        private String city;
        private String country;
        private double longitude;
        private double latitude;
        private Type type;
        private String ageGroup;
        private Over18 over18;
        private String[] keywords;
        private Premium premium;
        private int maxHeight;
        private int maxWidth;
        private int minHeight;
        private int minWidth;

        public Builder(Category category, String userAgent, String udid) {
            this.category = category;
            this.userAgent = userAgent;
            this.udid = udid;

            this.gender = null;
            this.zipCode = -1;
            this.areaCode = -1;
            this.city = null;
            this.country = null;
            this.longitude = -1;
            this.latitude = -1;
            this.type = null;
            this.ageGroup = null;
            this.over18 = null;
            this.keywords = null;
            this.premium = null;
            this.maxHeight = -1;
            this.maxWidth = -1;
            this.minHeight = -1;
            this.minWidth = -1;
        }

        public Builder setGender(Gender gender)
            { this.gender = gender; return this; }
        public Builder setZipCode(int zipCode)
            { this.zipCode = zipCode; return this; }
        public Builder setAreaCode(int areaCode)
            { this.areaCode = areaCode; return this; }
        public Builder setCity(String city)
            { this.city = city; return this; }
        public Builder setCountry(String country)
            { this.country = country; return this; }
        public Builder setLongitude(double longitude)
            { this.longitude = longitude; return this; }
        public Builder setLatitude(double latitude)
            { this.latitude = latitude; return this; }
        public Builder setType(Type type)
            { this.type = type; return this; }
        public Builder setAgeGroup(String ageGroup)
            { this.ageGroup = ageGroup; return this; }
        public Builder setOver18(Over18 over18)
            { this.over18 = over18; return this; }
        public Builder setKeywords(String[] keywords)
            { this.keywords = keywords; return this; }
        public Builder setPremium(Premium prem)
            { this.premium= prem; return this; }
        public Builder maxHeight(int maxHeight)
            { this.maxHeight = maxHeight; return this; }
        public Builder maxWidth(int maxWidth)
            { this.maxWidth = maxWidth; return this; }
        public Builder minHeight(int minHeight)
            { this.minHeight = minHeight; return this; }
        public Builder minWidth(int minWidth)
            { this.minWidth = minWidth; return this; }

        public ADSArguments build() {
            return new ADSArguments(this);
        }
    }

    private final Category category;
    private final String userAgent;
    private final String udid;

    private final Gender gender;
    private final int zipCode;
    private final int areaCode;
    private final String city;
    private final String country;
    private final double longitude;
    private final double latitude;
    private final Type type;
    private final String ageGroup;
    private final Over18 over18;
    private final String[] keywords;
    private final Premium premium;
    private final int maxHeight;
    private final int maxWidth;
    private final int minHeight;
    private final int minWidth;

    private ADSArguments(Builder builder) {
        this.category = builder.category;
        this.userAgent = builder.userAgent;
        this.udid = builder.udid;
        
        this.gender = builder.gender;
        this.zipCode = builder.zipCode;
        this.areaCode = builder.areaCode;
        this.city = builder.city;
        this.country = builder.country;
        this.longitude = builder.longitude;
        this.latitude = builder.latitude;
        this.type = builder.type;
        this.ageGroup = builder.ageGroup;
        this.over18 = builder.over18;
        this.keywords = builder.keywords;
        this.premium = builder.premium;
        this.maxHeight = builder.maxHeight;
        this.maxWidth = builder.maxWidth;
        this.minHeight = builder.minHeight;
        this.minWidth = builder.minWidth;
    }

    public Category getCategory() {
        return category;
    }

    public String getUserAgent() {
        return userAgent;
    }

    public String getUdid() {
        return udid;
    }

    public Gender getGender() {
        return gender;
    }

    public int getZipCode() {
        return zipCode;
    }

    public int getAreaCode() {
        return areaCode;
    }

    public String getCity() {
        return city;
    }

    public String getCountry() {
        return country;
    }

    public double getLongitude() {
        return longitude;
    }

    public double getLatitude() {
        return latitude;
    }

    public Type getType() {
        return type;
    }

    public String getAgeGroup() {
        return ageGroup;
    }

    public Over18 getOver18() {
        return over18;
    }

    public String[] getKeywords() {
        return keywords;
    }

    public Premium getPremium() {
        return premium;
    }

    public int getMaxHeight() {
        return maxHeight;
    }

    public int getMaxWidth() {
        return maxWidth;
    }

    public int getMinHeight() {
        return minHeight;
    }

    public int getMinWidth() {
        return minWidth;
    }
}
