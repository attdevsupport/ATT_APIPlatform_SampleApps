package com.att.api.sms.model;

public class SMSStatus {
    private final String msgId;
    private final String addr;
    private final String deliveryStatus;

    public SMSStatus(String msgId, String addr, String deliveryStatus) {
        this.msgId = msgId;
        this.addr = addr;
        this.deliveryStatus = deliveryStatus;
    }

    public String getMessageId() {
        return msgId;
    }

    public String getAddress() {
        return addr;
    }

    public String getDeliveryStatus() {
        return deliveryStatus;
    }
}
