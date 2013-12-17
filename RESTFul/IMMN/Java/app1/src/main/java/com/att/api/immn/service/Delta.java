package com.att.api.immn.service;

import org.json.JSONArray;
import org.json.JSONObject;

public final class Delta {
    private final String type;
    private final DeltaChange[] adds;
    private final DeltaChange[] deletes;
    private final DeltaChange[] updates;

    public Delta(String type, DeltaChange[] adds, DeltaChange[] deletes,
            DeltaChange[] updates) {
            
        this.type = type;
        this.adds = adds;
        this.deletes = deletes;
        this.updates = updates;
    }

    public String getType() {
        return type;
    }

    public DeltaChange[] getAdds() {
        return adds;
    }

    public DeltaChange[] getDeletes() {
        return deletes;
    }

    public DeltaChange[] getUpdates() {
        return updates;
    }

    public static Delta valueOf(JSONObject jobj) {
        String type = jobj.getString("type");

        JSONArray jadds = jobj.getJSONArray("adds");
        DeltaChange[] adds = new DeltaChange[jadds.length()];
        for (int i = 0; i < jadds.length(); ++i) {
            JSONObject jchange = jadds.getJSONObject(i);
            adds[i] = DeltaChange.valueOf(jchange);
        }

        JSONArray jdeletes = jobj.getJSONArray("deletes");
        DeltaChange[] deletes = new DeltaChange[jdeletes.length()];
        for (int i = 0; i < jdeletes.length(); ++i) {
            JSONObject jchange = jdeletes.getJSONObject(i);
            deletes[i] = DeltaChange.valueOf(jchange);
        }

        JSONArray jupdates = jobj.getJSONArray("updates");
        DeltaChange[] updates = new DeltaChange[jupdates.length()];
        for (int i = 0; i < jupdates.length(); ++i) {
            JSONObject jchange = jupdates.getJSONObject(i);
            updates[i] = DeltaChange.valueOf(jchange);
        }

        return new Delta(type, adds, deletes, updates);
    }

}
