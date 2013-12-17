/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2013.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.tl.model;

import org.json.JSONObject;

/**
 * Immutable class that holds a TL response information.
 *
 * @author <a href="mailto:pk9069@att.com">Pavel Kazakov</a>
 * @version 3.0
 * @since 2.2
 */
public final class TLResponse {
    /** Elapsed time for api request. */
    private final String elapsedTime;

    /** Latitude of device. */
    private final String latitude;

    /** Longitude of device. */
    private final String longitude;

    /** Accuracy of location in meters. */
    private final String accuracy;

    /**
     * Creates a TLResponse object.
     *
     * @param elapsedTime elapsed time for request
     * @param latitude device latitude
     * @param longitude device longitude
     * @param accuracy accuracy of location in meters
     */
    public TLResponse(final String elapsedTime, final String latitude,
            final String longitude, final String accuracy) {

        this.elapsedTime = elapsedTime;
        this.latitude = latitude;
        this.longitude = longitude;
        this.accuracy = accuracy;
    }

    /**
     * Gets how long the request took to complete, in seconds.
     *
     * @return elapsed time
     */
    public String getElapsedTime() {
        return elapsedTime;
    }

    /**
     * Gets latitude of device.
     *
     * @return latitude
     */
    public String getLatitude() {
        return latitude;
    }

    /**
     * Gets longitude of device.
     *
     * @return longitude
     */
    public String getLongitude() {
        return longitude;
    }

    /**
     * Gets accuracy in meters of location.
     *
     * @return accuracy
     */
    public String getAccuracy() {
        return accuracy;
    }

    /**
     * Factory method for creating a TLResponse using the specified elapsedTime
     * and json object.
     *
     * @param elapsedTime elapsed time
     * @param jobj json object used to build response
     * @return tl response object
     * @since 3.0
     */
    public static TLResponse valueOf(long elapsedTime, JSONObject jobj) {
        final String latitude = jobj.getString("latitude");
        final String longitude = jobj.getString("longitude");
        final String accuracy = jobj.getString("accuracy");
        return new TLResponse(elapsedTime + "", latitude, longitude, accuracy);
    }
}
