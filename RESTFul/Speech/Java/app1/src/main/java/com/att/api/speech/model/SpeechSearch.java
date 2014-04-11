package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

public class SpeechSearch {
    private List<JSONObject> mMeta;
    private List<JSONObject> mPrograms;
    private List<JSONObject> mShowtimes;

    public SpeechSearch(List<JSONObject> meta,
            List<JSONObject> programs,
            List<JSONObject> showtimes){
        this.mMeta = meta;
        this.mPrograms = programs;
        this.mShowtimes = showtimes;
    }

    public static SpeechSearch valueOf(JSONObject json) {
        List<JSONObject> meta = new ArrayList<JSONObject>();
        List<JSONObject> programs = new ArrayList<JSONObject>();
        List<JSONObject> showtimes = new ArrayList<JSONObject>();

        String key = "meta";
        if (json.has(key)) {
            JSONArray arr = json.getJSONArray(key);
            for (int i = 0; i < arr.length(); ++i)
                meta.add(arr.getJSONObject(i));
        }

        key = "programs";
        if (json.has(key)) {
            JSONArray arr = json.getJSONArray(key);
            for (int i = 0; i < arr.length(); ++i)
                programs.add(arr.getJSONObject(i));
        }

        key = "showtimes";
        if (json.has(key)) {
            JSONArray arr = json.getJSONArray(key);
            for (int i = 0; i < arr.length(); ++i)
                showtimes.add(arr.getJSONObject(i));
        }

        return new SpeechSearch(meta, programs, showtimes);
    }

    /**
     * @return the meta
     */
    public List<JSONObject> getMeta() {
        return mMeta;
    }

    /**
     * @return the programs
     */
    public List<JSONObject> getPrograms() {
        return mPrograms;
    }

    /**
     * @return the showtimes
     */
    public List<JSONObject> getShowtimes() {
        return mShowtimes;
    }
}
