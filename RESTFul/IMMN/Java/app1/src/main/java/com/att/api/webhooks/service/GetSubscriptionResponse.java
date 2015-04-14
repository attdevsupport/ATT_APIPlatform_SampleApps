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

public final class GetSubscriptionResponse {

    public static class Subscription {
        private final String subscriptionId;
        private final Integer expiresIn;
        private final String[] events;
        private final String callbackData;

        public Subscription(
            final String subscriptionId, final Integer expiresIn,
            final String[] events, final String callbackData
        ) {
            this.subscriptionId = subscriptionId;
            this.expiresIn = expiresIn;
            this.events = new String[events.length];
            for (int i = 0; i < events.length; ++i) {
                this.events[i] = events[i];
            }
            this.callbackData = callbackData;
        }

        public String getSubscriptionId() {
            return subscriptionId;
        }

        public Integer getExpiresIn() {
            return expiresIn;
        }

        public String[] getEvents() {
            // return a copy
            final String[] toReturn = new String[this.events.length];
            for (int i = 0; i < toReturn.length; ++i) {
                toReturn[i] = this.events[i];
            }
            return toReturn;
        }

        public String getCallbackData() {
            return callbackData;
        }

    }

    private final String contentType;
    private final String systemTransactionId;
    private final GetSubscriptionResponse.Subscription subscription ;

    public GetSubscriptionResponse(
        final String contentType, final String systemTransactionId, 
        final GetSubscriptionResponse.Subscription subscription
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

    public GetSubscriptionResponse.Subscription getSubscription() {
        return this.subscription;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
