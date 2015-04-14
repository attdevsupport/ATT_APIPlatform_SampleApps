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

package com.att.api.webhooks.service;

import org.json.JSONException;
import org.json.JSONObject;

import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTException;

public final class CreateSubscriptionResponse {

    public static class Subscription {
        private final String subscriptionId;
        private final Integer expiresIn;
        private final long creationTime;

        public Subscription(
            final String subscriptionId, final Integer expiresIn
        ) {
            this(subscriptionId, expiresIn, System.currentTimeMillis()/1000);
        }

        /**
         * Create a new subscription object 
         *
         * @param subscriptionId the subscription ID
         * @param expiresIn time subscription expires, relative to creationTime
         * @param creationTime time creation took place in seconds
         */
        public Subscription(
                final String subscriptionId, 
                final Integer expiresIn, 
                final Long creationTime) 
        {
            this.subscriptionId = subscriptionId;
            this.expiresIn = expiresIn;
            this.creationTime = creationTime;
        }

        public String getSubscriptionId() {
            return subscriptionId;
        }

        /**
         * The time in seconds the subscription will expire since creation
         *
         * @return Number of seconds till expiration or null if not set
         */
        public Integer getExpiresIn() {
            return expiresIn;
        }

        /**
         * Time this object was created as a Unix timestamp
         *
         * @return the creationTime
         */
        public long getCreationTime() {
            return creationTime;
        }
    }

    private final String contentType;
    private final String location;
    private final String systemTransactionId;
    private final CreateSubscriptionResponse.Subscription subscription ;

    public CreateSubscriptionResponse(
        final String contentType, 
        final String location,
        final String systemTransactionId, 
        final CreateSubscriptionResponse.Subscription subscription
    ) {
        this.contentType = contentType;
        this.location = location;
        this.systemTransactionId = systemTransactionId;
        this.subscription = subscription;
    }

    public String getContentType() {
        return this.contentType;
    }

    public String getLocation() {
        return this.location;
    }

    public String getSystemTransactionId() {
        return this.systemTransactionId;
    }

    public CreateSubscriptionResponse.Subscription getSubscription() {
        return this.subscription;
    }

    public static CreateSubscriptionResponse valueOf(APIResponse response)
            throws RESTException {
        final String location = response.getHeader("location");
        final String systemTransId = response
                .getHeader("x-systemTransactionId");
        final String contentType = response.getHeader("content-type");
    
        try {
            final JSONObject jobjresponse 
                = new JSONObject(response.getResponseBody());
            final JSONObject jsubscriptionresponse 
                = jobjresponse.getJSONObject("subscription");
            final String subscriptionIdResponse 
                = jsubscriptionresponse.getString("subscriptionId");

            Integer expiresInResponse = null; 
            if (jsubscriptionresponse.has("expiresIn"))
                expiresInResponse = jsubscriptionresponse.getInt("expiresIn");

            final CreateSubscriptionResponse.Subscription subscription
                = new CreateSubscriptionResponse.Subscription(
                    subscriptionIdResponse, expiresInResponse
                );
                    
            return new CreateSubscriptionResponse(
                    contentType, location, systemTransId, subscription);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
