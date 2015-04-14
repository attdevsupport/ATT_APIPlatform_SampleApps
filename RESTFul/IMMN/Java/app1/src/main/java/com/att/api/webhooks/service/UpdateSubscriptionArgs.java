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

public final class UpdateSubscriptionArgs {
    private final String channelId;
    private final String subscriptionId;
    private final String[] events;
    private final String callbackData;
    private final Integer expiresIn;

    public UpdateSubscriptionArgs(
        final String channelId, final String subscriptionId,
        final String[] events, final String callbackData,
        final Integer expiresIn
    ) {
        this.channelId = channelId;
        this.subscriptionId = subscriptionId;
        this.events = events;
        this.callbackData = callbackData;
        this.expiresIn = expiresIn;
    }

    public String getChannelId() {
        return channelId;
    }

    public String getSubscriptionId() {
        return subscriptionId;
    }

    public String[] getEvents() {
        // return a copy
        final String[] eventsCopy = new String[this.events.length];
        for (int i = 0; i < events.length; ++i) {
            eventsCopy[i] = events[i];
        }
        return eventsCopy;
    }

    public String getCallbackData() {
        return callbackData;
    }

    public Integer getExpiresIn() {
        return expiresIn;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
