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
