package com.att.api.sms.model;

public class SMSReceiveMsg {
    private final String dTime;
    private final String msgId;
    private final String msg;
    private final String senderAddr;
    private final String destinationAddr;

    public SMSReceiveMsg(String dTime, String msgId, String msg, 
            String senderAddr, String destinationAddr) {

        if (msgId == null || msgId.equals("")) {
            msgId = "-";
        }

        this.dTime = dTime;
        this.msgId = msgId;
        this.msg = msg;
        this.senderAddr = senderAddr;
        this.destinationAddr = destinationAddr;
    }

    public String getDateTime() {
        return dTime;
    }

    public String getMsgId() {
        return msgId;
    }

    public String getMsg() {
        return msg;
    }

    public String getSenderAddr() {
        return senderAddr;
    }

    public String getDestinationAddr() {
        return destinationAddr;
    }
}
