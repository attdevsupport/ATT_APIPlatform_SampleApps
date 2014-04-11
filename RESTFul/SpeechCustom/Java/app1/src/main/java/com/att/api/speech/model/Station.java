package com.att.api.speech.model;

public final class Station {
    private String number;
    private String name;

    public Station(String number, String name){
        this.number = number;
        this.name = name;
    }

    /**
     * @return the number
     */
    public String getNumber() {
        return number;
    }

    /**
     * @return the name
     */
    public String getName() {
        return name;
    }

}
