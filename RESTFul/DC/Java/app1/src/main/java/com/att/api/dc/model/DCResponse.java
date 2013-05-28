package com.att.api.dc.model;

import java.util.Iterator;
import java.util.LinkedHashMap;

import org.json.JSONObject;

public class DCResponse {
    private LinkedHashMap<String, String> deviceMap; 

    public DCResponse(final JSONObject jobj) {
        deviceMap = new LinkedHashMap<String, String>();

        JSONObject deviceInfo = jobj.getJSONObject("DeviceInfo");
        JSONObject deviceId = deviceInfo.getJSONObject("DeviceId");
        JSONObject capabilities = deviceInfo.getJSONObject("Capabilities");
        String typeAllocationCode = deviceId.getString("TypeAllocationCode"); 
        deviceMap.put("TypeAllocationCode", typeAllocationCode);

        // suppress generics warning
        @SuppressWarnings("unchecked")
        Iterator<String> keys = capabilities.keys();
        while (keys.hasNext()) {
            String key = keys.next();
            String value = capabilities.getString(key);
            deviceMap.put(key, value);
        }
    }

    public LinkedHashMap<String, String> getResponseMap() {
        return deviceMap;
    }
}
