package com.att.api.speech.model;

import org.json.JSONObject;

public class SpeechInterpretation {
    private String cast;
    private String title;
    private String control;
    private Station station;
    private Genre genre;

    private SpeechInterpretation(Builder builder) {
        this.cast = builder.cast;
        this.title = builder.title;
        this.control = builder.control;
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
     * @return the control
     */
    public String getControl() {
        return control;
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

    public static SpeechInterpretation valueOf(JSONObject json){
        Builder builder = new Builder();

        String key = "cast";
        if (json.has(key)) {
            String cast = json.getString(key);
            builder.setCast(cast);
        }

        key = "title";
        if (json.has(key)) {
            String title = json.getString(key);
            builder.setTitle(title);
        }

        key = "control";
        if (json.has(key)) {
            String control = json.getString(key);
            builder.setControl(control);
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
        private String control;
        private Station station;
        private Genre genre;

        public SpeechInterpretation build() {
            return new SpeechInterpretation(this);
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
         * @param control the control to set
         */
        public void setControl(String control) {
            this.control = control;
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
