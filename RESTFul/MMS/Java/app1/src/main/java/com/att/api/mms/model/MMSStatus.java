package com.att.api.mms.model;

public class MMSStatus {
    private final String msgId;
    private final String addr;
    private final String deliveryStatus;

    public MMSStatus(String msgId, String addr, String deliveryStatus) {
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
