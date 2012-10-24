package com.att.api.dc.model;

public class DeviceInfo {
	
	//status flags for display
	private boolean status;
	private String errorResponse;
	
	private String typeAllocationCode;
	private String name;
	private String vendor;
	private String model;
	private String firmwareVersion;
	private String uaProf;
	private String mmsCapable;
	private String assitedGps;
	private String locationTechnology;
	private String browserType;
	private String wapPushCapable;
	
	public DeviceInfo()
	{
		errorResponse = "";
	}

	public DeviceInfo(boolean status, String errorResponse)
	{
		this.status = status;
		this.errorResponse = errorResponse;
	}
	
	/**
	 * @return the deviceId
	 */
	public String getDeviceId() {
		return typeAllocationCode;
	}

	/**
	 * @param typeAllocationCode the deviceId to set
	 */
	public void setTypeAllocationCode(String typeAllocationCode) {
		this.typeAllocationCode = typeAllocationCode;
	}

	/**
	 * @return the name
	 */
	public String getName() {
		return name;
	}

	/**
	 * @param name the name to set
	 */
	public void setName(String name) {
		this.name = name;
	}

	/**
	 * @return the vendor
	 */
	public String getVendor() {
		return vendor;
	}

	/**
	 * @param vendor the vendor to set
	 */
	public void setVendor(String vendor) {
		this.vendor = vendor;
	}

	/**
	 * @return the model
	 */
	public String getModel() {
		return model;
	}

	/**
	 * @param model the model to set
	 */
	public void setModel(String model) {
		this.model = model;
	}

	/**
	 * @return the firmwareVersion
	 */
	public String getFirmwareVersion() {
		return firmwareVersion;
	}

	/**
	 * @param firmwareVersion the firmwareVersion to set
	 */
	public void setFirmwareVersion(String firmwareVersion) {
		this.firmwareVersion = firmwareVersion;
	}

	/**
	 * @return the uaProf
	 */
	public String getUaProf() {
		return uaProf;
	}

	/**
	 * @param uaProf the uaProf to set
	 */
	public void setUaProf(String uaProf) {
		this.uaProf = uaProf;
	}

	/**
	 * @return the mmsCapable
	 */
	public String getMmsCapable() {
		return mmsCapable;
	}

	/**
	 * @param mmsCapable the mmsCapable to set
	 */
	public void setMmsCapable(String mmsCapable) {
		this.mmsCapable = mmsCapable;
	}

	/**
	 * @return the assitedGps
	 */
	public String getAssitedGps() {
		return assitedGps;
	}

	/**
	 * @param assitedGps the assitedGps to set
	 */
	public void setAssitedGps(String assitedGps) {
		this.assitedGps = assitedGps;
	}

	/**
	 * @return the locationTechnology
	 */
	public String getLocationTechnology() {
		return locationTechnology;
	}

	/**
	 * @param locationTechnology the locationTechnology to set
	 */
	public void setLocationTechnology(String locationTechnology) {
		this.locationTechnology = locationTechnology;
	}

	/**
	 * @return the browserType
	 */
	public String getBrowserType() {
		return browserType;
	}

	/**
	 * @param browserType the browserType to set
	 */
	public void setBrowserType(String browserType) {
		this.browserType = browserType;
	}

	/**
	 * @return the wapPushCapable
	 */
	public String getWapPushCapable() {
		return wapPushCapable;
	}

	/**
	 * @param wapPushCapable the wapPushCapable to set
	 */
	public void setWapPushCapable(String wapPushCapable) {
		this.wapPushCapable = wapPushCapable;
	}

	/**
	 * @return the status
	 */
	public boolean isStatus() {
		return status;
	}

	/**
	 * @param status the status to set
	 */
	public void setStatus(boolean status) {
		this.status = status;
	}

	/**
	 * @return the errorResponse
	 */
	public String getErrorResponse() {
		return errorResponse;
	}

	/**
	 * @param errorResponse the errorResponse to set
	 */
	public void setErrorResponse(String errorResponse) {
		this.errorResponse = errorResponse;
	}
	
}
