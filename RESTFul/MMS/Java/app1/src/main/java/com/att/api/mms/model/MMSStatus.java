/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

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

/**
 * Immutable class for holding MMS status response information.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 */
public final class MMSStatus {
    /** Message id. */
    private final String msgId;

    /** Address. */
    private final String addr;

    /** Message delivery status. */
    private final String deliveryStatus;


    /**
     * Creates an MMS status object.
     *
     * @param msgId message id
     * @param addr address
     * @param deliveryStatus delivery status
     */
    public MMSStatus(String msgId, String addr, String deliveryStatus) {
        this.msgId = msgId;
        this.addr = addr;
        this.deliveryStatus = deliveryStatus;
    }

    /**
     * Gets message id.
     *
     * @return message id
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
}
