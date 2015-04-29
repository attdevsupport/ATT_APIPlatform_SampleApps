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

package com.att.api.ads.service;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONObject;
import org.json.JSONException;


/**
 * Used to interact with version 1 of the Advertising API.
 *
 * <p>
 * This class is thread safe.
 * </p>
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 * @see <a href="https://developer.att.com/docs/apis/rest/1/Advertising">Advertising Documentation</a>
 */
public class ADSService extends APIService {

    /**
     * Converts an array of keywords to a keywords string to be sent to the
     * API.
     *
     * @param keywords keywords
     * 
     * @return keywords as a string
     */
    private String getKeywordsStr(final String[] keywords) {
        if (keywords == null) {
            return null;
        }

        final StringBuilder sb = new StringBuilder();
        final int length = keywords.length;
        for (int i = 0; i < length - 1; ++i) {
            sb.append(keywords[i] + ",");
        }
        sb.append(keywords[length - 1]);

        return sb.toString();
    }

    /**
     * Appends any optional arguments to the request.
     *
     * @param args optional arguments to append
     * @param client rest client to append arguments to
     */
    private void appendArguments(ADSArguments args, RESTClient client) {
        // A lot of this could probably be moved to array...

        if (args.getGender() != null)
            client.addParameter("Gender", args.getGender().getString());

        if (args.getZipCode() != -1)
            client.addParameter("ZipCode", "" + args.getZipCode());

        if (args.getAreaCode() != -1)
            client.addParameter("AreaCode", "" + args.getAreaCode());

        if (args.getCity() != null)
            client.addParameter("City", args.getCity());

        if (args.getCountry() != null)
            client.addParameter("Country", args.getCountry());

        if (args.getLongitude() != -1)
            client.addParameter("Longitude", "" + args.getLongitude());

        if (args.getLatitude() != -1)
            client.addParameter("Latitude", "" + args.getLatitude());

        if (args.getType() != null)
            client.addParameter("Type", "" + args.getType().getValue());

        if (args.getAgeGroup() != null)
            client.addParameter("AgeGroup", args.getAgeGroup());

        if (args.getOver18() != null)
            client.addParameter("Over18", "" + args.getOver18().getValue());

        final String keywordsStr = getKeywordsStr(args.getKeywords());
        if (keywordsStr != null)
            client.addParameter("Keywords", keywordsStr);

        if (args.getPremium() != null)
            client.addParameter("Premium", "" + args.getPremium().getValue());

        if (args.getMaxHeight() != -1)
            client.addParameter("MaxHeight", "" + args.getMaxHeight());

        if (args.getMaxWidth() != -1)
            client.addParameter("MaxWidth", "" + args.getMaxWidth());

        if (args.getMinHeight() != -1)
            client.addParameter("MinHeight", "" + args.getMinHeight());

        if (args.getMinWidth() != -1)
            client.addParameter("MinWidth", "" + args.getMinWidth());

    }

    /**
     * Creates an ADService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public ADSService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * Convenience method for sending a request to the API for getting an
     * advertisement with only the required values set.
     *
     * @param category category of advertisement
     * @param userAgent user agent
     * @param udid universally unique identifier
     * @return API response
     * @throws RESTException if API request was not successful
     * @throws IllegalArgumentException if args is null
     */
    public ADSResponse getAdvertisement(Category category, String userAgent,
            String udid) throws RESTException {

        final ADSArguments args 
            = new ADSArguments.Builder(category, userAgent, udid).build();
        
        return this.getAdvertisement(args);
    }

    /**
     * Sends a request to the API for getting an advertisement.
     *
     * @param args arguments 
     * @return API response
     * @throws RESTException if API request was not successful
     * @throws IllegalArgumentException if args is null
     */
    public ADSResponse getAdvertisement(ADSArguments args) throws RESTException {

        if (args == null)
            throw new IllegalArgumentException("Arguments must not be null.");

        final String endpoint = getFQDN() + "/rest/1/ads";

        final RESTClient client = new RESTClient(endpoint)
            .addParameter("Category", args.getCategory().getString())
            .addAuthorizationHeader(getToken())
            .addHeader("User-Agent", args.getUserAgent())
            .addHeader("UDID", args.getUdid());

        this.appendArguments(args, client);

        try {
            final APIResponse response = client.httpGet();
            if (response.getStatusCode() == 204) {
                return null;
            }
            final String responseBody = response.getResponseBody();
            return ADSResponse.valueOf(new JSONObject(responseBody));
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */
