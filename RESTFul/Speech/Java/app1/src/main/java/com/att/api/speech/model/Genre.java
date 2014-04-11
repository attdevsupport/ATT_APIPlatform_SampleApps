package com.att.api.speech.model;

public final class Genre {
    private String id;
    private String words;

    public Genre(String id, String words){
        this.id = id;
        this.words = words;
    }

    /**
     * @return the id
     */
    public String getId() {
        return id;
    }

    /**
     * @return the words
     */
    public String getWords() {
        return words;
    }
}
