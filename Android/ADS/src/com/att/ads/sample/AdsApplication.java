package com.att.ads.sample;

import android.app.Application;

/**
 * 
 * This class is used to maintain Global application state and it is implemented
 * as a Singleton context. This class will be instantiated when the process of
 * the application is created.
 * 
 * @author ATT
 * 
 */
public class AdsApplication extends Application {

	private static AdsApplication instance = null;
	// defined default values for settings tab
	private String keywords = null;
	private int zip = 0;
	private int premium = 0;// 0 - non-premium, 1 - premium only, 2 - both
	private String ageGroup = null;
	private int maxHeight = 0;
	private int maxWidth = 0;
	private String response = "";
	private double latitude = 0.0;
	private double longitude = 0.0;
	private boolean isChangeInSettings;

	/**
	 * This method is called when the process of the application is created.
	 */
	@Override
	public void onCreate() {
		super.onCreate();
		instance = this;
	}

	/**
	 * Singleton instance to get application context.
	 * 
	 * @return instance
	 */
	public static AdsApplication getInstance() {
		return instance;
	}

	/**
	 * Return the keywords.
	 * 
	 * @return keywords
	 */
	public String getKeywords() {
		return keywords;
	}

	/**
	 * Set the keywords value.
	 * 
	 * @param keywords
	 */
	public void setKeywords(String keywords) {
		this.keywords = keywords;
	}

	/**
	 * Return the Zip.
	 * 
	 * @return Zip
	 */
	public int getZip() {
		return zip;
	}

	/**
	 * Set the zip value.
	 * 
	 * @param zip
	 */
	public void setZip(int zip) {
		this.zip = zip;
	}

	/**
	 * Return the premium.
	 * 
	 * @return premium
	 */
	public int getPremium() {
		return premium;
	}

	/**
	 * Set the Premium value( 0 - non-premium, 1 - premium only, 2 - both ).
	 * 
	 * @param premium
	 */
	public void setPremium(int premium) {
		this.premium = premium;
	}

	
	/**
	 * Return an age group.
	 * 
	 * @return ageGrop
	 */
	public String getAgeGroup() {
		return ageGroup;
	}

	/**
	 * Set an age group value( 1-13,14-25,26-35,36-55,56-100 ).
	 * 
	 * @param ageGroup
	 */
	public void setAgeGroup(String ageGroup) {
		this.ageGroup = ageGroup;
	}

	/**
	 * Return the max height.
	 * 
	 * @return maxHeight
	 */
	public int getMaxHeight() {
		return maxHeight;
	}

	/**
	 * Set ads max height value( 0-320 ).
	 */
	public void setMaxHeight(int maxHeight) {
		this.maxHeight = maxHeight;
	}

	/**
	 * Return the max width.
	 * 
	 * @return maxWidth
	 */
	public int getMaxWidth() {
		return maxWidth;
	}

	/**
	 * Set ads max width value( 0-200 ).
	 */
	public void setMaxWidth(int maxWidth) {
		this.maxWidth = maxWidth;
	}

	/**
	 * Returns response.
	 * 
	 * @return response
	 */
	public String getResponse() {
		return response;
	}

	/**
	 * Sets response into application context.
	 * 
	 * @param response
	 */
	public void setResponse(String response) {
		this.response = response;
	}
	
	public double getLatitude() {
		return latitude;
	}

	public void setLatitude(double latitude) {
		this.latitude = latitude;
	}

	public double getLongitude() {
		return longitude;
	}

	public void setLongitude(double longitude) {
		this.longitude = longitude;
	}

	public boolean isChangeInSettings() {
		return isChangeInSettings;
	}

	public void setChangeInSettings(boolean isChangeInSettings) {
		this.isChangeInSettings = isChangeInSettings;
	}

	
}
