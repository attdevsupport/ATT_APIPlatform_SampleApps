/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker */

/*
 * Copyright 2014 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.att.api.util;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Provides a set of utility date methods.
 *
 * @version 3.0
 * @since 2.2
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
     * @since 3.0
     * @see java.text.SimpleDateFormat
     */
    public static String getTime() {
        SimpleDateFormat df = new SimpleDateFormat(D_FORMAT);
        df.setTimeZone(TimeZone.getTimeZone("UTC"));
        String serverDate = df.format(new Date());
        return serverDate;
    }
}
