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

public final class CreateChannelResponse {
    public static final class Channel {
        private final String channelId;
        private final Integer maxEventsPerNotification;

        public Channel(String channelId, Integer maxEventsPerNotification) {
            this.channelId = channelId;
            this.maxEventsPerNotification = maxEventsPerNotification;
        }

        public String getChannelId() {
            return channelId;
        }

        public Integer getMaxEventsPerNotification() {
            return maxEventsPerNotification;
        }

    }

    private final String location;
    private final String systemTransId;
    private final Channel channel;

    public CreateChannelResponse(
        String location, String systemTransId, Channel channel
    ) {
        this.location = location;
        this.systemTransId = systemTransId;
        this.channel = channel;
    }

    public String getLocation() {
        return location;
    }

    public String getSystemTransactionId() {
        return systemTransId;
    }

    public Channel getChannel() {
        return channel;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
