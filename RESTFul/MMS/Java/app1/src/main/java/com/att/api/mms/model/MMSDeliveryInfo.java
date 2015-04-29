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

package com.att.api.mms.model;

import org.json.JSONObject;

/**
 * Immutable class for holding MMS delivery information.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class MMSDeliveryInfo {

    private final String msgId;
    private final String addr;
    private final String deliveryStatus;

    public MMSDeliveryInfo(String msgId, String addr, String deliveryStatus) {
        this.msgId = msgId;
        this.addr = addr;
        this.deliveryStatus = deliveryStatus;
    }

    /**
     * Gets message id.
     *
     * @return msg id
     */
    public String getMessageId() {
        return msgId;
    }

    /**
     * Gets address.
     *
     * @return address
     */
    public String getAddress() {
        return addr;
    }

    /**
     * Gets delivery status.
     *
     * @return delivery status
     */
    public String getDeliveryStatus() {
        return deliveryStatus;
    }

    public static MMSDeliveryInfo valueOf(JSONObject jobj) {
        final String msgId = jobj.getString("Id");
        final String addr = jobj.getString("Address");
        final String dStatus = jobj.getString("DeliveryStatus");

        return new MMSDeliveryInfo(msgId, addr, dStatus);
    }
}
