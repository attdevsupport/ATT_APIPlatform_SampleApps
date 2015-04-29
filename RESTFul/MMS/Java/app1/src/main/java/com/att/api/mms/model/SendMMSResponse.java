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

package com.att.api.mms.model;

import org.json.JSONObject;

/**
 * Immutable class for holding a Send MMS response.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class SendMMSResponse {

    /** Message id. */
    private final String msgId;

    /** Resource url. */
    private final String resourceUrl;

    public SendMMSResponse(String msgId) {
        this(msgId, null);
    }

    public SendMMSResponse(String msgId, String resourceUrl) {
        this.msgId = msgId;
        this.resourceUrl = resourceUrl;
    }

    /**
     * Gets message id.
     *
     * @return message id
     */
    public String getMessageId() {
        return msgId;
    }

    /**
     * Gets resource url.
     *
     * @return resource url
     */
    public String getResourceUrl() {
        return resourceUrl;
    }

    // factory method
    public static SendMMSResponse valueOf(JSONObject jobj) {
        JSONObject response = jobj.getJSONObject("outboundMessageResponse");

        final String msgId = response.getString("messageId");

        String resourceUrl = null;
        if (response.has("resourceReference")) {
            JSONObject ref = response.getJSONObject("resourceReference");
            if (ref.has("resourceURL")) {
                resourceUrl = ref.getString("resourceURL");
            }
        }

        return new SendMMSResponse(msgId, resourceUrl);
    }

}
