package com.att.api.immn.service;

import org.json.JSONArray;
import org.json.JSONObject;

public final class MessageList {
    private Message[] msgs;
    private int offset;
    private int limit;
    private int total;
    private String state;
    private CacheStatus cacheStatus;
    private String[] failedMessages;

    private MessageList() {
        msgs = null;
        state = null;
        cacheStatus = null;
        failedMessages = null;
    }

    public static MessageList valueOf(JSONObject jobj) {
        MessageList msgList = new MessageList();

        JSONObject jMsgList = jobj.getJSONObject("messageList");

        JSONArray jmsgs = jMsgList.getJSONArray("messages");
        Message[] msgs = new Message[jmsgs.length()];

        for (int i = 0; i < jmsgs.length(); ++i) {
            Message msg = Message.valueOf(jmsgs.getJSONObject(i));
            msgs[i] = msg;
        }
        msgList.msgs = msgs;
        msgList.offset = jMsgList.getInt("offset");
        msgList.limit = jMsgList.getInt("limit");
        msgList.total = jMsgList.getInt("total");
        msgList.state = jMsgList.getString("state");

        final String cacheStatusStr = jMsgList.getString("cacheStatus");
        msgList.cacheStatus = CacheStatus.fromString(cacheStatusStr);

        if (jMsgList.has("failedMessages")) {
            JSONArray jfailedMessages = jMsgList.getJSONArray("failedMessages");
            final int length = jfailedMessages.length();
            final String[] failedMessages = new String[length];
            for (int i = 0; i < length; ++i) {
                failedMessages[i] = jfailedMessages.getString(i);
            }

            msgList.failedMessages = failedMessages;
        }

        return msgList;
    }

    public Message[] getMessages() {
        return msgs;
    }

    public int getOffset() {
        return offset;
    }

    public int getLimit() {
        return limit;
    }

    public int getTotal() {
        return total;
    }

    public String getState() {
        return state;
    }

    public CacheStatus getCacheStatus() {
        return cacheStatus;
    }

    public String[] getFailedMessages() {
        return failedMessages;
    }

}
