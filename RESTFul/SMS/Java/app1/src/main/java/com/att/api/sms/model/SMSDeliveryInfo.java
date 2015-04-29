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
 * Immutable class that holds SMS delivery information.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class SMSDeliveryInfo {

    private final String msgId;

    private final String addr;

    private final String status;

    public SMSDeliveryInfo(String msgId, String addr, String status) {
        this.msgId = msgId;
        this.addr = addr;
        this.status = status;
    }

    public String getMessageId() {
        return msgId;
    }

    public String getAddress() {
        return addr;
    }

    public String getDeliveryStatus() {
        return status;
    }

    public static SMSDeliveryInfo valueOf(JSONObject jobj) {
        final String id = jobj.getString("Id");
        final String addr = jobj.getString("Address");
        final String status = jobj.getString("DeliveryStatus");

        return new SMSDeliveryInfo(id, addr, status);
    }
}
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */
