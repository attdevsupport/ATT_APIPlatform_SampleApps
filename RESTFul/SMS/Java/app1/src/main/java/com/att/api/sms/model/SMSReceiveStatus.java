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

package com.att.api.sms.model;

import org.json.JSONObject;

/**
 * Immutable class that holds SMS API status notification information.
 *
 * @author kh455g
 * @version 1.0
 * @since 1.0
 */
public class SMSReceiveStatus {
    private final String messageId;
    private final String address;
    private final String deliveryStatus;

    /**
     * Construct a received sms status
     *
     * @param msgid the message id
     * @param addr the address
     * @param ds the delivery status
     */
    public SMSReceiveStatus(final String msgid, final String addr,
            final String ds) {
        this.messageId = msgid;
        this.address = addr;
        this.deliveryStatus = ds;
    }

    /**
     * @return the messageId
     */
    public String getMessageId() {
        return messageId;
    }

    /**
     * @return the address
     */
    public String getAddress() {
        return address;
    }

    /**
     * @return the deliveryStatus
     */
    public String getDeliveryStatus() {
        return deliveryStatus;
    }

    public static SMSReceiveStatus valueOf(JSONObject jobj) {
        JSONObject base = jobj.getJSONObject("deliveryInfoNotification");
        final String msgid = base.getString("messageId");
        JSONObject di = base.getJSONObject("deliveryInfo");
        final String ds = di.getString("deliveryStatus");
        final String addr = di.getString("address");
        return new SMSReceiveStatus(msgid, addr, ds);
    }
}
