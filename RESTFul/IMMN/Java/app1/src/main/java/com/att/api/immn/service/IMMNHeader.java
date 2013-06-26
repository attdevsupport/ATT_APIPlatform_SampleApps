/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2013.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.immn.service;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Immutable class used to hold an IMMN header.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @author <a href="mailto:kh455g@att.com">Kyle Hill</a>
 * @version 3.0
 * @since 2.2
 */
public final class IMMNHeader {
    /** Message id. */
    private final String messageId;

    /** From. */
    private final String from;

    /** To. */
    private final String to;

    /** Message subject. */
    private final String subject;

    /** Message text. */
    private final String text;

    /** Array of MMS content. */
    private final MMSContent[] mmsContent;

    /** Received. */
    private final String received;

    /** Favorite. */
    private final String favorite;

    /** Read. */
    private final String read;

    /** Type. */
    private final String type;

    /** Direction. */
    private final String direction;

/*
    private final String contentName;
    private final String contentType;
    private final String id;
    private final String partNumber;
*/

    /**
     * Creates an IMMNHeader from the specified <code>JSONObject</code>.
     *
     * @param jobj json object used to create the immn header
     * @deprecated replaced by {@link #valueOf(JSONObject)}
     */
    public IMMNHeader(JSONObject jobj) {
        String[] vals = {
            "MessageId", "From", "To", "Subject", "Text", "Received",
            "Favorite", "Read", "Type", "Direction", "ContentName",
            "ContentType", "PartNumber"
        };

        for (String val : vals) {
            if (!jobj.has(val)) {
                // TODO (pk9069): null would probably be a better choice
                jobj.put(val, "");
            }
        }

        if (jobj.has("MmsContent")) {
            JSONArray mmsArray = jobj.getJSONArray("MmsContent");
            this.mmsContent = new MMSContent[mmsArray.length()];
            for (int i = 0; i < mmsArray.length(); ++i) {
                mmsContent[i] = new MMSContent(mmsArray.getJSONObject(i));
            }
        } else {
            mmsContent = null;
        }

        this.messageId = jobj.getString("MessageId");
        this.from = jobj.getString("From");
        this.to = jobj.getString("To");
        this.subject = jobj.getString("Subject");
        this.text = jobj.getString("Text");
        this.received = jobj.getString("Received");
        this.favorite = jobj.getString("Favorite");
        this.read = jobj.getString("Read");
        this.type = jobj.getString("Type");
        this.direction = jobj.getString("Direction");
    }

    /**
     * Gets message id.
     *
     * @return message id
     */
    public String getMessageId() {
        return this.messageId;
    }

    /**
     * Gets from.
     *
     * @return from
     */
    public String getFrom() {
        return this.from;
    }

    /**
     * Gets to.
     *
     * @return to
     */
    public String getTo() {
        // TODO (pk9069): convert to String[]
        return this.to;
    }
    /**
     * Gets message subject.
     *
     * @return message subject
     */
    public String getSubject() {
        return this.subject;
    }

    /**
     * Gets message text.
     *
     * @return message text
     */
    public String getText() {
        return this.text;
    }
    /**
     * Gets mms content, if any.
     *
     * @return mms content or <tt>null</tt> if none
     */
    public MMSContent[] getMmsContent() {
        return this.mmsContent;
    }
    /**
     * Gets received string.
     *
     * @return received string
     */
    public String getReceived() {
        return this.received;
    }
    /**
     * Gets favorite string.
     *
     * @return favorite string
     */
    public String getFavorite() {
        return this.favorite;
    }
    /**
     * Gets read string.
     *
     * @return read string
     */
    public String getRead() {
        // TODO (pk9069): Should this be boolean?
        return this.read;
    }
    /**
     * Gets message type.
     *
     * @return message type
     */
    public String getType() {
        return this.type;
    }
    /**
     * Gets direction.
     *
     * @return direction
     */
    public String getDirection() {
        return this.direction;
    }

    /**
     * Alias for <code>valueOf()</code>.
     *
     * @param jobj json object
     * @return immn header
     * @see #valueOf(JSONObject)
     */
    public static IMMNHeader buildFromJSON(JSONObject jobj) {
        return new IMMNHeader(jobj);
    }

    /**
     * Factory method used to create an IMMN header using the specified json
     * object.
     *
     * @param jobj object
     * @return immn header
     * @since 3.0
     */
    public static IMMNHeader valueOf(JSONObject jobj) {
        return IMMNHeader.buildFromJSON(jobj);
    }
}

