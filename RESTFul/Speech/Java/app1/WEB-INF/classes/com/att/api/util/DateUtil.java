// 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
//

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
