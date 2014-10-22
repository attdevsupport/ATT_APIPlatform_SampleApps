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

package com.att.api.service;

import com.att.api.oauth.OAuthToken;

/**
 * Provides a common interface, as well as common functionality, for API
 * services.
 *
 * @author pk9069
 * @author kh455g
 * @version 1.0
 * @since 1.0
 */
public abstract class APIService {

    /** Fully qualified domain name used for request. */
    private String fqdn;

    /** OAuth token used for request. */
    private OAuthToken token;

    /**
     * Gets the fully qualified domain name.
     *
     * @return fully qualified domain name
     */
    protected final String getFQDN() {
        return fqdn;
    }

    /**
     * Gets the OAuth token used for authorization.
     *
     * @return OAuth token
     */
    protected final OAuthToken getToken() {
        return token;
    }

    /**
     * Creates an APIService object with the specified fully qualified domain
     * name and OAuth token.
     *
     * <p>
     * Default proxy and ssl settings will be used.
     * <p>
     *
     * @param fqdn fully qualified domain name
     * @param token OAuth token
     * @see com.att.api.rest.RESTConfig#setDefaultProxy(String, int)
     * @see com.att.api.rest.RESTConfig#setDefaultTrustAllCerts(boolean)
     */
    public APIService(String fqdn, OAuthToken token) {
        this.fqdn = fqdn;
        this.token = token;
    }

    /**
     * Utility method for converting a raw string to an array of formated
     * addresses that is usable by the API.
     *
     * <p>
     * Addresses are split based on the "," character.
     * </p>
     *
     * @param rawString raw string to convert
     * @return array of formatted addresses
     */
    public static String[] formatAddresses(String rawString) {
        return APIService.formatAddresses(rawString.split(","));
    }

    /**
     * Utility method for converting a an array of addresses to an array of
     * formated addresses that is usable by the API.
     *
     * @param addresses array of unformatted addresses
     * @return array of formatted addresses
     */
    public static String[] formatAddresses(String[] addresses) {
        String[] fStr = new String[addresses.length];
        for (int i = 0; i < addresses.length; ++i) {
            String address = addresses[i];
            address = address.replaceAll(" ", "");
            address = address.replaceAll("-", "");
            address = address.replaceAll("tel:", "");
            address = address.trim();
            if (!address.contains("@"))
                address = "tel:" + address;
            fStr[i] = address;
        }
        return fStr;
    }
}
