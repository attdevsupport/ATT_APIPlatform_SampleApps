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

public final class UpdateSubscriptionResponse {

    public static class Subscription {
        private final String subscriptionId;
        private final Integer expiresIn;

        public Subscription(
            final String subscriptionId, final Integer expiresIn
        ) {
            this.subscriptionId = subscriptionId;
            this.expiresIn = expiresIn;
        }

        public String getSubscriptionId() {
            return subscriptionId;
        }

        public Integer getExpiresIn() {
            return expiresIn;
        }

    }

    private final String contentType;
    private final String systemTransactionId;
    private final UpdateSubscriptionResponse.Subscription subscription ;

    public UpdateSubscriptionResponse(
        final String contentType, final String systemTransactionId, 
        final UpdateSubscriptionResponse.Subscription subscription
    ) {
        this.contentType = contentType;
        this.systemTransactionId = systemTransactionId;
        this.subscription = subscription;
    }

    public String getContentType() {
        return this.contentType;
    }

    public String getSystemTransactionId() {
        return this.systemTransactionId;
    }

    public UpdateSubscriptionResponse.Subscription getSubscription() {
        return this.subscription;
    }

    public static UpdateSubscriptionResponse valueOf(APIResponse response)
            throws RESTException {
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
            final Integer expiresInResponse
                = jsubscriptionresponse.getInt("expiresIn");

            final UpdateSubscriptionResponse.Subscription subscription
                = new UpdateSubscriptionResponse.Subscription(
                        subscriptionIdResponse, expiresInResponse);
                    
            return new UpdateSubscriptionResponse(
                contentType, systemTransId, subscription
            );
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
