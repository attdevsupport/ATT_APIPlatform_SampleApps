package com.att.api.immn.service;

import org.json.JSONObject;
import org.json.JSONArray;

public final class DeltaResponse {
    public final String state;
    public final Delta[] delta;

    public DeltaResponse(String state, Delta[] delta) {
        this.state = state;
        this.delta = delta;
    }

    public String getState() {
        return state;
    }

    public Delta[] getDeltas() {
        return delta;
    }

    public static DeltaResponse valueOf(JSONObject jobj) {
        JSONObject jdeltaResponse = jobj.getJSONObject("deltaResponse");
        String state = jdeltaResponse.getString("state");

        JSONArray jdelta = jdeltaResponse.getJSONArray("delta");
        Delta[] delta = new Delta[jdelta.length()];
        for (int i = 0; i < jdelta.length(); ++i)
          delta[i] = Delta.valueOf(jdelta.getJSONObject(i));

        return new DeltaResponse(state, delta);
    }
}
