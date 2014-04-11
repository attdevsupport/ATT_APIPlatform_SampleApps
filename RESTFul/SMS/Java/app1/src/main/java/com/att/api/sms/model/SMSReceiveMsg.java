/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2014.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.sms.model;

/**
 * Immutable class that holds SMS API receive message information.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 */
public final class SMSReceiveMsg {
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
    public SMSReceiveMsg(String dTime, String msgId, String msg,
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
    public String getMsgId() {
        return msgId;
    }

    /**
     * Gets message.
     *
     * @return message
     */
    public String getMsg() {
        return msg;
    }

    /**
     * Gets address of sender.
     *
     * @return address of sender
     */
    public String getSenderAddr() {
        return senderAddr;
    }

    /**
     * Gets destination of address.
     *
     * @return destination of address.
     */
    public String getDestinationAddr() {
        return destinationAddr;
    }
}
