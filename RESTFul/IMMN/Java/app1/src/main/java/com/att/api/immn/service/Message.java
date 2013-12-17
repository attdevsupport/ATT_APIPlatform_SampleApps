package com.att.api.immn.service;

import org.json.JSONArray;
import org.json.JSONObject;

public final class Message {
    private String messageId;
    private String from;
    private String[] recipients;
    private String text;
    private MmsContent[] mmsContents;
    private String timeStamp;

    private boolean isFavorite;
    private boolean isUnread;
    private String type;
    private TypeMetaData typeMetaData;
    private boolean isIncoming;

    private Message() {
        this.messageId = null;
        this.from = null;
        this.recipients = null;
        this.text = null;
        this.mmsContents = null;
        this.timeStamp = null;
        this.type = null;
        this.typeMetaData = null;
        this.isIncoming = false;
    }

    public static Message valueOf(JSONObject jobj) {
        Message msg = new Message();

        msg.messageId = jobj.getString("messageId");

        if (jobj.has("from"))
            msg.from = jobj.getJSONObject("from").getString("value");

        if (jobj.has("recipients")) {
            JSONArray jarr = jobj.getJSONArray("recipients");
            final String[] recipients = new String[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                recipients[i] = jarr.getJSONObject(i).getString("value");
            }
            msg.recipients = recipients;
        }

        if (jobj.has("text")) msg.text = jobj.getString("text");

        if (jobj.has("mmsContent")) {
            JSONArray jarr = jobj.getJSONArray("mmsContent");
            final MmsContent[] mmsContents = new MmsContent[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                mmsContents[i] = MmsContent.valueOf(jarr.getJSONObject(i));
            }
            msg.mmsContents = mmsContents;
        }

        if (jobj.has("timeStamp")) msg.timeStamp = jobj.getString("timeStamp");
        msg.isFavorite = jobj.getBoolean("isFavorite");
        msg.isUnread = jobj.getBoolean("isUnread");
        msg.type = jobj.getString("type");
        msg.typeMetaData = TypeMetaData.valueOf(jobj.getJSONObject("typeMetaData"));

        if (jobj.has("isIncoming")) {
            msg.isIncoming = jobj.getString("isIncoming").equals("true");
        }

        return msg;
    }

    public String getMessageId() {
        return messageId;
    }

    public String getFrom() {
        return from;
    }

    public String[] getRecipients() {
        return recipients;
    }

    public String getText() {
        return text;
    }

    public MmsContent[] getMmsContents() {
        return mmsContents;
    }

    public String getTimeStamp() {
        return timeStamp;
    }

    public boolean isFavorite() {
        return isFavorite;
    }

    public boolean isUnread() {
        return isUnread;
    }

    public String getType() {
        return type;
    }

    public TypeMetaData getTypeMetaData() {
        return typeMetaData;
    }

    public boolean isIncoming() {
        return isIncoming;
    }

}
