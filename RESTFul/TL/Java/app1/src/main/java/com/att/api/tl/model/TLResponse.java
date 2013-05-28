package com.att.api.tl.model;

public class TLResponse {
    private final String elapsedTime;
    private final String latitude;
    private final String longitude;
    private final String accuracy;

    public TLResponse(final String elapsedTime, final String latitude, 
            final String longitude, final String accuracy) {

        this.elapsedTime = elapsedTime;
        this.latitude = latitude;
        this.longitude = longitude;
        this.accuracy = accuracy;
    }

    public String getElapsedTime() {
        return elapsedTime;
    }

    public String getLatitude() {
        return latitude;
    }

    public String getLongitude() {
        return longitude;
    }

    public String getAccuracy() {
        return accuracy;
    }
}
