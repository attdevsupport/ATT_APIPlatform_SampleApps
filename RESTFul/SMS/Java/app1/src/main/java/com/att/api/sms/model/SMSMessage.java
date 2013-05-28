package com.att.api.sms.model;

public class SMSMessage {
    private final String messageIndex;
    private final String messageText;
    private final String messageAddress;
    
    public SMSMessage(final String mIndex, final String mText, 
            final String mAddr) {
        this.messageIndex = mIndex;
        this.messageText = mText;
        this.messageAddress = mAddr;
    }

    public String getMessageIndex() {
        return this.messageIndex; 
    }
    public String getMessageText() {
        return this.messageText;
    }
    public String getSenderAddress() {
        return this.messageAddress;
    }
}
