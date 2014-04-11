package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

public class NLUHypothesis {
    private String cast;
    private String title;
    private List<OutComposite> outComposite;
    private Station station;
    private Genre genre;

    private NLUHypothesis(Builder builder) {
        this.cast = builder.cast;
        this.title = builder.title;
        this.outComposite = builder.outComposite;
        this.station = builder.station;
        this.genre = builder.genre;
    }

    /**
     * @return the cast
     */
    public String getCast() {
        return cast;
    }

    /**
     * @return the title
     */
    public String getTitle() {
        return title;
    }

    /**
     * @return the outComposite
     */
    public List<OutComposite> getOutComposite() {
        return outComposite;
    }

    /**
     * @return the station
     */
    public Station getStation() {
        return station;
    }

    /**
     * @return the genre
     */
    public Genre getGenre() {
        return genre;
    }

    public static NLUHypothesis valueOf(JSONObject json){
        Builder builder = new Builder();

        String key = "OutComposite";
        if (json.has(key)) {
            List<OutComposite> outs = new ArrayList<OutComposite>();
            JSONArray outarr = json.getJSONArray(key);
            for (int i = 0; i < outarr.length(); ++i){
                JSONObject obj = outarr.getJSONObject(i);
                OutComposite o = new OutComposite(obj.getString("Grammar"), 
                        obj.getString("Out"));
                outs.add(o);
            }
            builder.setOutComposite(outs);
        }

        key = "cast";
        if (json.has(key)) {
            String cast = json.getString(key);
            builder.setCast(cast);
        }

        key = "title";
        if (json.has(key)) {
            String title = json.getString(key);
            builder.setTitle(title);
        }

        String number = "";
        String name = "";
        key = "station.number";
        if (json.has(key))
            number = json.getString(key);
        key = "station.name";
        if (json.has(key))
            number = json.getString(key);
        if (!number.isEmpty() || !name.isEmpty())
            builder.setStation(new Station(number, name));

        String id = "";
        String words = "";
        key = "genre.id";
        if (json.has(key))
            number = json.getString(key);
        key = "genre.words";
        if (json.has(key))
            number = json.getString(key);
        if (!id.isEmpty() || !words.isEmpty())
            builder.setGenre(new Genre(id, words));

        return builder.build();
    }

    public static class Builder {
        private String cast;
        private String title;
        private List<OutComposite> outComposite;
        private Station station;
        private Genre genre;

        public NLUHypothesis build() {
            return new NLUHypothesis(this);
        }

        /**
         * @param cast the cast to set
         */
        public Builder setCast(String cast) {
            this.cast = cast;
            return this;
        }

        /**
         * @param title the title to set
         */
        public Builder setTitle(String title) {
            this.title = title;
            return this;
        }

        /**
         * @param outComposite the outComposite to set
         */
        public Builder setOutComposite(List<OutComposite> outComposite) {
            this.outComposite = outComposite;
            return this;
        }

        /**
         * @param station the station to set
         */
        public Builder setStation(Station station) {
            this.station = station;
            return this;
        }

        /**
         * @param genre the genre to set
         */
        public Builder setGenre(Genre genre) {
            this.genre = genre;
            return this;
        }
    }
}
