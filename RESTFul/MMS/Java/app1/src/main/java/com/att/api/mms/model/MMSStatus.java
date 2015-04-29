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

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Immutable class for holding MMS status response information.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public final class MMSStatus {
    private final String resourceUrl;

    private final MMSDeliveryInfo[] infoList;

    public MMSStatus(String resourceUrl, MMSDeliveryInfo[] infoList) {
        this.resourceUrl = resourceUrl;
        this.infoList = infoList;
    }

    /**
     * @return the resourceUrl
     */
    public String getResourceUrl() {
        return resourceUrl;
    }

    /**
     * Gets an array that contains delivery information for each message.
     *
     * @return info list
     */
    public MMSDeliveryInfo[] getInfoList() {
        if (infoList == null)
            return null;

        // provide a copy instead of exposing internal array
        MMSDeliveryInfo[] list = new MMSDeliveryInfo[infoList.length];
        for (int i = 0; i < list.length; ++i) {
            list[i] = infoList[i];
        }

        return list;
    }

    public static MMSStatus valueOf(JSONObject jobj) {
        JSONObject deliveryInfoList = jobj.getJSONObject("DeliveryInfoList");

        final String resourceUrl = deliveryInfoList.getString("ResourceUrl");

        JSONArray infos = deliveryInfoList.getJSONArray("DeliveryInfo");
        final int length = infos.length();

        MMSDeliveryInfo[] infoList = new MMSDeliveryInfo[length];

        for (int i = 0; i < length; ++i) {
            infoList[i] = MMSDeliveryInfo.valueOf(infos.getJSONObject(i));
        }

        return new MMSStatus(resourceUrl, infoList);
    }

}
