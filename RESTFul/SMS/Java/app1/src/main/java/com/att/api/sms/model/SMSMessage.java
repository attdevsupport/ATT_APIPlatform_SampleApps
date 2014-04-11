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
 * Immutable class that holds an SMS message.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 */
public final class SMSMessage {

    /** Message index. */
    private final String messageIndex;

    /** Message text. */
    private final String messageText;

    /** Message address. */
    private final String messageAddress;

    /**
     * Creates an SMSMessage object.
     *
     * @param mIndex message index
     * @param mText message text
     * @param mAddr message address
     */
    public SMSMessage(final String mIndex, final String mText,
            final String mAddr) {
        this.messageIndex = mIndex;
        this.messageText = mText;
        this.messageAddress = mAddr;
    }

    /**
     * Gets message index.
     *
     * @return message index
     */
    public String getMessageIndex() {
        return this.messageIndex;
    }

    /**
     * Gets message text.
     *
     * @return message text
     */
    public String getMessageText() {
        return this.messageText;
    }

    /**
     * Gets sender address.
     *
     * @return sender address
     */
    public String getSenderAddress() {
        return this.messageAddress;
    }
}
