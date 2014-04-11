package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

public class NBest {
    private String hypothesis;
    private String languageId;
    private double confidence;
    private String grade;
    private String resultText;
    private NLUHypothesis nluHypothesis;
    private List<String> words;
    private List<Double> wordScores;

    private NBest(Builder builder) {
        this.hypothesis = builder.hypothesis;
        this.languageId = builder.languageId;
        this.confidence = builder.confidence;
        this.grade = builder.grade;
        this.resultText = builder.resultText;
        this.nluHypothesis = builder.nluHypothesis;
        this.words = builder.words;
        this.wordScores = builder.wordScores;
    }

    /**
     * @return the hypothesis
     */
    public String getHypothesis() {
        return this.hypothesis;
    }

    /**
     * @return the languageId
     */
    public String getLanguageId() {
        return this.languageId;
    }

    /**
     * @return the confidence
     */
    public double getConfidence() {
        return this.confidence;
    }

    /**
     * @return the grade
     */
    public String getGrade() {
        return this.grade;
    }

    /**
     * @return the resultText
     */
    public String getResultText() {
        return this.resultText;
    }

    /**
     * @return the nluHypothesis
     */
    public NLUHypothesis getNluHypothesis() {
        return this.nluHypothesis;
    }

    /**
     * @return the words
     */
    public List<String> getWords() {
        return this.words;
    }

    /**
     * @return the wordScores
     */
    public List<Double> getWordScores() {
        return this.wordScores;
    }

    public static List<String> jsonStringArrayToList(JSONArray arr) {
        List<String> list = new ArrayList<String>();
        for (int i = 0; i < arr.length(); ++i) {
            list.add(arr.getString(i));
        }
        return list;
    }

    public static List<Double> jsonDoubleArrayToList(JSONArray arr) {
        List<Double> list = new ArrayList<Double>();
        for (int i = 0; i < arr.length(); ++i) {
            list.add(arr.getDouble(i));
        }
        return list;
    }

    public static NBest valueOf(JSONObject json) {
        // Add optional parts
        Builder builder = new Builder();

        String key = "LanguageId";
        if (json.has(key))
                builder.setLanguageId(json.getString(key));

        key = "ResultText";
        if (json.has(key))
                builder.setResultText(json.getString(key));

        key = "Words";
        if (json.has(key)){
            List<String> words = jsonStringArrayToList(json.getJSONArray(key));
            builder.setWords(words);
        }

        key = "WordScores";
        if (json.has(key)){
            List<Double> scores = jsonDoubleArrayToList(json.getJSONArray(key));
            builder.setWordScores(scores);
        }

        key = "NluHypothesis";
        if (json.has(key)){
            NLUHypothesis nlu = NLUHypothesis.valueOf(json.getJSONObject(key));
            builder.setNluHypothesis(nlu);
        }

        // Add required parts and build new object
        return builder
            .setHypothesis(json.getString("Hypothesis"))
            .setConfidence(json.getDouble("Confidence"))
            .setGrade(json.getString("Grade"))
            .build();
    }

    public static class Builder {
        private String hypothesis;
        private String languageId;
        private double confidence;
        private String grade;
        private String resultText;
        private NLUHypothesis nluHypothesis;
        private List<String> words;
        private List<Double> wordScores;

        /**
         * @param hypothesis the hypothesis to set
         */
        public Builder setHypothesis(String hypothesis) {
            this.hypothesis = hypothesis;
            return this;
        }

        /**
         * @param languageId the languageId to set
         */
        public Builder setLanguageId(String languageId) {
            this.languageId = languageId;
            return this;
        }

        /**
         * @param confidence the confidence to set
         */
        public Builder setConfidence(double confidence) {
            this.confidence = confidence;
            return this;
        }

        /**
         * @param grade the grade to set
         */
        public Builder setGrade(String grade) {
            this.grade = grade;
            return this;
        }

        /**
         * @param resultText the resultText to set
         */
        public Builder setResultText(String resultText) {
            this.resultText = resultText;
            return this;
        }

        /**
         * @param nluHypothesis the nluHypothesis to set
         */
        public Builder setNluHypothesis(NLUHypothesis nluHypothesis) {
            this.nluHypothesis = nluHypothesis;
            return this;
        }

        /**
         * @param words the words to set
         */
        public Builder setWords(List<String> words) {
            this.words = words;
            return this;
        }

        /**
         * @param wordScores the wordScores to set
         */
        public Builder setWordScores(List<Double> wordScores) {
            this.wordScores = wordScores;
            return this;
        }

        public NBest build(){
            return new NBest(this);
        }
    }
}
