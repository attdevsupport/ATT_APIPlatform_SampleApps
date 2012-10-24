package com.att.api.validation;

import java.util.ArrayList;

import javax.servlet.http.HttpServletRequest;

public class CommonValidator 
{

	private static final String INVALID_MAXIMUM_HEIGHT = "Invalid Maximum Height";
	private static final String INVALID_MINIMUM_WIDTH = "Invalid Minimum Width";
	private static final String INVALID_MAXIMUM_WIDTH = "Invalid Maximum Width";
	private static final String INVALID_AREA_CODE = "Invalid Area Code";
	private static final String INVALID_ZIP_CODE = "Invalid Zip Code";
	private static final String INVALID_LONGITUDE = "Invalid Longitude";
	private static final String INVALID_LATITUDE = "Invalid Latitude";
	
	/**
	 * Validate Input parameters
	 * @param request
	 * @return
	 */
	public static ArrayList<String> validateInputParams(HttpServletRequest request)
	{
		String latitude = (String)request.getAttribute("latitude");
		String longitude = (String)request.getAttribute("longitude"); 
		String zipCode = (String)request.getAttribute("zipCode");
		String areaCode= (String)request.getAttribute("areaCode");
		String maxWidth= (String)request.getAttribute("maxWidth");
		String minWidth= (String)request.getAttribute("minWidth");
		String maxHeight= (String)request.getAttribute("maxHeight");
		String minHeight= (String)request.getAttribute("minHeight");
		ArrayList<String> errors = new ArrayList<String>();
		
		if ((latitude != null && latitude != "" && !CommonValidator.isValidLattitude(latitude)))
		{
			errors.add(INVALID_LATITUDE);
		}
		
		if (longitude != null && longitude != "" && !CommonValidator.isValidLongitude(longitude))
		{
			errors.add(INVALID_LONGITUDE);
		}
		
		if (zipCode != null && zipCode != "" && !CommonValidator.isValidZipCode(zipCode))
		{
			errors.add(INVALID_ZIP_CODE);
		}
		
		if (areaCode != null && areaCode != "" && !CommonValidator.isValidAreaCode(areaCode))
		{
			errors.add(INVALID_AREA_CODE);
		}

		if (maxWidth != null && maxWidth != "" && !CommonValidator.isValidMaxWidth(maxWidth))
		{
			errors.add(INVALID_MAXIMUM_WIDTH);
		}
		
		if (minWidth != null && minWidth != "" && !CommonValidator.isValidMinWidth(minWidth))
		{
			errors.add(INVALID_MINIMUM_WIDTH);
		}
		
		if (maxHeight != null && maxHeight != "" && !CommonValidator.isValidMaxHeight(maxHeight))
		{
			errors.add(INVALID_MAXIMUM_HEIGHT);
		}
		
		if (minHeight != null && minHeight != "" && !CommonValidator.isValidMinHeight(minHeight))
		{
			errors.add("Invalid Minimum Height");
		}
		return errors;
	}
	
	/**
	 * Validate whether lattitude is a number
	 * @param lattitude
	 * @return
	 */
	public static boolean isValidLattitude(String lattitude)
	{
		return isNumber(lattitude);
	}
	
	/**
	 * Validate whether longitude is a numnber
	 * @param longitude
	 * @return
	 */
	public static boolean isValidLongitude(String longitude)
	{
		return isNumber(longitude);
	}
	
	/**
	 * Validate whether the zipCode is a number
	 * @param zipCode
	 * @return
	 */	
	public static boolean isValidZipCode(String zipCode)
	{
		return isInteger(zipCode);
	}
	
	/**
	 * Validate whether the areaCode is a number
	 * @param areaCode
	 * @return
	 */
	public static boolean isValidAreaCode(String areaCode)
	{
		return isInteger(areaCode);
	}
	
	/**
	 * Validate whether the maxHeigth is a number
	 * @param maxHeight
	 * @return
	 */
	public static boolean isValidMaxHeight(String maxHeight)
	{
		return isInteger(maxHeight);
	}
	/**
	 * Validate whether the minHeigth is a number
	 * @param minHeigth
	 * @return
	 */
	public static boolean isValidMinHeight(String minHeigth)
	{
		return isInteger(minHeigth);
	}
	/**
	 * Validate whether the maxWidth is a number
	 * @param maxWidth
	 * @return
	 */
	public static boolean isValidMaxWidth(String maxWidth)
	{
		return isInteger(maxWidth);
	}
	
	/**
	 * Validate whether the minWidth is a number
	 * @param maxWidth
	 * @return
	 */
	public static boolean isValidMinWidth(String minWidth)
	{
		return isInteger(minWidth);
	}
	

	/**
	 * Validate whether the input is a number
	 * @param str
	 * @return
	 */
	public static boolean isInteger(String str)
	{
		boolean isValid = false;
		try
		{
			Integer.parseInt(str);
			isValid = true;
		}
		catch (Exception ex)
		{
			//
		}
		return isValid;
	}
	
	
	/**
	 * Validate whether the input is a number
	 * @param str
	 * @return
	 */
	public static boolean isNumber(String str)
	{
		boolean isValid = false;
		try
		{
			Float.parseFloat(str);
			isValid = true;
		}
		catch (Exception ex)
		{
			//
		}
		return isValid;
	}
	
}
