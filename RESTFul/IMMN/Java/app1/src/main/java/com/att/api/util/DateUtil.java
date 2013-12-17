/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

package com.att.api.util;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Provides a set of utility date methods.
 *
 * @version 1.0
 * @since 1.0
 */
public class DateUtil {

    /** Default date format. */
    private static final String D_FORMAT = "EEE, MMMM dd, yyyy HH:mm:ss zzz";

    /**
     * Creates a DateUtil object.
     *
     * <p>
     * Constructor is made public instead of private for backwards
     * compatibility.
     * </p>
     * @deprecated methods should be called from a static context
     */
    @Deprecated
    public DateUtil() {
    }

    /**
     * Alias for <code>getTime()</code>.
     *
     * @return UTC time
     * @deprecated replaced by {@link #getTime()}
     */
    @Deprecated
    public String getServerTime() {
        return DateUtil.getTime();
    }

    /**
     * Returns the current time using UTC with following format.
     * <code>EEE, MMMM dd, yyyy HH:mm:ss zzz</code>
     *
     * @return UTC time
     * @since 1.0
     * @see java.text.SimpleDateFormat
     */
    public static String getTime() {
        SimpleDateFormat df = new SimpleDateFormat(D_FORMAT);
        df.setTimeZone(TimeZone.getTimeZone("UTC"));
        String serverDate = df.format(new Date());
        return serverDate;
    }
}
