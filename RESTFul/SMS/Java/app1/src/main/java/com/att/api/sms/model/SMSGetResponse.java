/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Immutable class that holds a Get SMS API response.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class SMSGetResponse {
    private final SMSMessage[] msgs;

    private final String numberOfMsgs;
    private final String resourceUrl;
    private final String pendingMsgs;

    public SMSGetResponse(SMSMessage[] msgs, String numberOfMsgs,
            String resourceUrl, String pendingMsgs) {

        this.msgs = msgs;
        this.numberOfMsgs = numberOfMsgs;
        this.resourceUrl = resourceUrl;
        this.pendingMsgs = pendingMsgs;
    }

    public SMSMessage[] getMessages() {
        // TODO: return copy
        return msgs;
    }

    public String getNumberOfMessages() {
        return numberOfMsgs;
    }

    public String getResourceUrl() {
        return resourceUrl;
    }

    public String getPendingMessages() {
        return pendingMsgs;
    }

    public static SMSGetResponse valueOf(JSONObject jobj) {
        JSONObject msgList = jobj.getJSONObject("InboundSmsMessageList");

        String numberOfMsgs = msgList.getString("NumberOfMessagesInThisBatch");
        String resourceUrl = msgList.getString("ResourceUrl");
        String pendingMsgs = msgList.getString("TotalNumberOfPendingMessages");

        JSONArray msgs = msgList.getJSONArray("InboundSmsMessage");
        int length = msgs.length();

        SMSMessage[] msgModels = new SMSMessage[length];
        for (int i = 0; i < length; ++i) {
            JSONObject jmsg = msgs.getJSONObject(i);

            msgModels[i] = SMSMessage.valueOf(jmsg);
        }

        return new SMSGetResponse(
            msgModels, numberOfMsgs, resourceUrl, pendingMsgs
        );

    }
    
}
