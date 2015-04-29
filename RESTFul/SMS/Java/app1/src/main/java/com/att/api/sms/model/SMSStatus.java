/*
 * Copyright 2015 AT&T
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
 * Immutable class that holds SMS API status information.
 *
 * @author pk9069
 * @author kh455g
 * @version 1.0
 * @since 1.0
 */
public final class SMSStatus {
    private final String resourceUrl;

    private final SMSDeliveryInfo[] infoList;

    public SMSStatus(String resourceUrl, SMSDeliveryInfo[] infoList) {
        this.resourceUrl = resourceUrl;
        this.infoList = infoList;
    }

    public String getResourceUrl() {
        return resourceUrl;
    }

    public SMSDeliveryInfo[] getInfoList() {
        if (infoList == null)
            return null;

        // provide a copy instead of exposing internal array
        SMSDeliveryInfo[] list = new SMSDeliveryInfo[infoList.length];
        for (int i = 0; i < list.length; ++i) {
            list[i] = infoList[i];
        }

        return list;
    }

    public static SMSStatus valueOf(JSONObject jobj) {
        JSONObject deliveryInfoList = jobj.getJSONObject("DeliveryInfoList");

        final String resourceUrl = deliveryInfoList.getString("ResourceUrl");

        JSONArray infos = deliveryInfoList.getJSONArray("DeliveryInfo");
        final int length = infos.length();

        SMSDeliveryInfo[] infoList = new SMSDeliveryInfo[length];

        for (int i = 0; i < length; ++i) {
            infoList[i] = SMSDeliveryInfo.valueOf(infos.getJSONObject(i));
        }

        return new SMSStatus(resourceUrl, infoList);
    }

}
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */
