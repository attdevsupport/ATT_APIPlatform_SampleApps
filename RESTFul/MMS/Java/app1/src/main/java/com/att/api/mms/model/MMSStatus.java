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
