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
 * Immutable class that holds SMS API receive message information.
 *
 * @author pk9069
 * @author kh455g
 * @version 1.0
 * @since 1.0
 */
public final class SMSReceiveMessage {
    /** Message date time. */
    private final String dTime;

    /** Message id. */
    private final String msgId;

    /** Message body. */
    private final String msg;

    /** Sender address. */
    private final String senderAddr;

    /** Destination address. */
    private final String destinationAddr;

    /**
     * Creates an SMSReceiveMsg object.
     *
     * @param dTime time of message
     * @param msgId message id
     * @param msg message
     * @param senderAddr sender address
     * @param destinationAddr destination address
     */
    public SMSReceiveMessage(String dTime, String msgId, String msg,
            String senderAddr, String destinationAddr) {

        // TODO (pk9069): probably not the best place to set default...
        if (msgId == null || msgId.equals("")) {
            msgId = "-";
        }

        this.dTime = dTime;
        this.msgId = msgId;
        this.msg = msg;
        this.senderAddr = senderAddr;
        this.destinationAddr = destinationAddr;
    }

    /**
     * Gets date-time of message.
     *
     * @return date time
     */
    public String getDateTime() {
        return dTime;
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
     * Gets message.
     *
     * @return message
     */
    public String getMessage() {
        return msg;
    }

    /**
     * Gets address of sender.
     *
     * @return address of sender
     */
    public String getSenderAddress() {
        return senderAddr;
    }

    /**
     * Gets destination of address.
     *
     * @return destination of address.
     */
    public String getDestinationAddress() {
        return destinationAddr;
    }

    public static SMSReceiveMessage valueOf(JSONObject jobj) {
        final String msgId = jobj.getString("MessageId");
        final String msg = jobj.getString("Message");
        final String senderAddr = jobj.getString("SenderAddress");
        final String dateTime = jobj.getString("DateTime");
        final String dest = jobj.getString("DestinationAddress");

        return new SMSReceiveMessage(dateTime, msgId, msg, senderAddr, dest);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */
