package com.att.api.speech.model;

import org.json.JSONObject;

public class SpeechInfo {
    private String actionType;
    private String recognized;
    private String version;

    private JSONObject metrics;
    private SpeechSearch search;
    private SpeechInterpretation interpretation;

    private SpeechInfo(Builder builder) {
        this.actionType = builder.actionType;
        this.recognized = builder.recognized;
        this.version = builder.version;
        this.metrics = builder.metrics;
        this.interpretation = builder.interpretation;
        this.search = builder.search;
    }

    public static SpeechInfo valueOf(JSONObject json){
        Builder builder = new Builder();
        
        String key = "version";
        if (json.has(key))
            builder.setVersion(json.getString(key));

        key = "recognized";
        if (json.has(key))
            builder.setRecognized(json.getString(key));

        key = "actionType";
        if (json.has(key))
            builder.setActionType(json.getString(key));

        key = "metrics";
        if (json.has(key))
            builder.setMetrics(json.getJSONObject(key));

        key = "interpretation";
        if (json.has(key))
            builder.setInterpretation(
                    SpeechInterpretation.valueOf(json.getJSONObject(key)));

        key = "search";
        if (json.has(key))
            builder.setSearch(SpeechSearch.valueOf(json.getJSONObject(key)));

        return builder.build();
    }

    /**
     * @return the actionType
     */
    public String getActionType() {
        return actionType;
    }

    /**
     * @return the recognized
     */
    public String getRecognized() {
        return recognized;
    }

    /**
     * @return the version
     */
    public String getVersion() {
        return version;
    }

    /**
     * @return the metrics
     */
    public JSONObject getMetrics() {
        return metrics;
    }

    /**
     * @return the interpretation
     */
    public SpeechInterpretation getInterpretation() {
        return interpretation;
    }

    /**
     * @return the search
     */
    public SpeechSearch getSearch() {
        return search;
    }

    public static class Builder {
        private String actionType;
        private String recognized;
        private String version;

        private JSONObject metrics;
        private SpeechInterpretation interpretation;
        private SpeechSearch search;

        /**
         * @param actionType the actionType to set
         */
        public Builder setActionType(String actionType) {
            this.actionType = actionType;
            return this;
        }

        /**
         * @param recognized the recognized to set
         */
        public Builder setRecognized(String recognized) {
            this.recognized = recognized;
            return this;
        }

        /**
         * @param version the version to set
         */
        public void setVersion(String version) {
            this.version = version;
        }

        /**
         * @param metrics the metrics to set
         */
        public Builder setMetrics(JSONObject metrics) {
            this.metrics = metrics;
            return this;
        }

        /**
         * @param interpretation the interpretation to set
         */
        public Builder setInterpretation(SpeechInterpretation interpretation) {
            this.interpretation = interpretation;
            return this;
        }

        /**
         * @param search the search to set
         */
        public Builder setSearch(SpeechSearch search) {
            this.search = search;
            return this;
        }

        public SpeechInfo build() {
            return new SpeechInfo(this);
        }
        
    }
}
