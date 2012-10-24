package com.att.api.util;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

public class DateUtil {

	private static final String UTC_DATE_FORMAT = "EEE MMM dd HH:mm:ss zzz yyyy";
	
	/**
	 * Returns UTC formatted Server time
	 * @return
	 */
	public static String getServerTime()
	{
		SimpleDateFormat df = new SimpleDateFormat(UTC_DATE_FORMAT);
		df.setTimeZone(TimeZone.getTimeZone("UTC"));
		String serverDate = df.format(new Date());
		return serverDate;
	}

}
