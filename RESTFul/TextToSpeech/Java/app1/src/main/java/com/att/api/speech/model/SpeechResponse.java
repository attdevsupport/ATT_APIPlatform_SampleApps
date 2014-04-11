package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Class used to represent a response to a speech-to-text request from the
 * server.
 */
public class SpeechResponse {
    private String status;
    private String responseId;
    private SpeechInfo info;
    private List<NBest> nbests;

    /**
     * Creates a speech response object with an empty response.
     */
    private SpeechResponse(Builder builder) {
        this.status = builder.status;
        this.responseId = builder.responseId;
        this.info = builder.info;
        this.nbests = builder.nbests;
    }

    /**
     * @return the status
     */
    public String getStatus() {
        return this.status;
    }

    /**
     * @return the responseId
     */
    public String getResponseId() {
        return this.responseId;
    }

    /**
     * @return the info
     */
    public SpeechInfo getInfo() {
        return this.info;
    }

    /**
     * @return the nbests
     */
    public List<NBest> getNbests() {
        return this.nbests;
    }


    public static SpeechResponse valueOf(JSONObject json) {
        JSONObject root = json.getJSONObject("Recognition");

        Builder builder = new Builder()
            .setResponseId(root.getString("ResponseId"))
            .setStatus(root.getString("Status"));

        if (root.has("Info"))
            builder.setInfo(SpeechInfo.valueOf(root.getJSONObject("Info")));

        if (builder.status.equals("OK")) {
            JSONArray nBests = root.getJSONArray("NBest");

            List<NBest> lnbest = new ArrayList<NBest>();
            for (int i = 0; i < nBests.length(); ++i) {
                NBest nbest = NBest.valueOf(nBests.getJSONObject(i));
                lnbest.add(nbest);
            }
            builder.setNBests(lnbest);
        }

        return builder.build();
    }

    public static class Builder {
        private String status;
        private String responseId;
        private SpeechInfo info;
        private List<NBest> nbests;

        public Builder setStatus(String status){
            this.status = status;
            return this;
        }

        public Builder setResponseId(String responseId){
            this.responseId = responseId;
            return this;
        }

        public Builder setInfo(SpeechInfo info){
            this.info = info;
            return this;
        }

        public Builder addNBest(NBest nbest){
            if (this.nbests == null)
                this.nbests = new ArrayList<NBest>();
            this.nbests.add(nbest);
            return this;
        }

        public Builder setNBests(List<NBest> nbests){
            this.nbests = nbests;
            return this;
        }

        public SpeechResponse build(){
            return new SpeechResponse(this);
        }
    }
}
