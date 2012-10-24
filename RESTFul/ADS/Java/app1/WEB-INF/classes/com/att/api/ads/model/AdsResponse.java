package com.att.api.ads.model;

import java.util.ArrayList;

import javax.servlet.http.HttpServletRequest;

/**
 * 
 * @author Sendhil Chokkalingam
 * Model for Ads API Sample App
 *
 */
public class AdsResponse {

	private boolean resultStatus;
	private boolean formStatus;
	private String errorResponse;
	private int statusCode;
	private ArrayList<String> errors = new ArrayList<String>();
	
	private String adType;
	private String clickUrl;
	private String imageUrl;
	private String adText;
	private String adContent;
	
	public ArrayList<String> getErrors() {
		return errors;
	}

	public void setErrors(ArrayList<String> errors) {
		this.errors = errors;
	}
	
	public boolean getResultStatus() {
		return resultStatus;
	}
	public void setResultStatus(boolean status) {
		this.resultStatus = status;
	}
	public boolean isFormStatus() {
		return formStatus;
	}
	public void setFormStatus(boolean formStatus) {
		this.formStatus = formStatus;
	}

	public String getErrorResponse() {
		return errorResponse;
	}
	public void setErrorResponse(String errorResponse) {
		this.errorResponse = errorResponse;
	}
	
	public int getStatusCode() {
		return statusCode;
	}
	public void setStatusCode(int statusCode) {
		this.statusCode = statusCode;
	}
	public String getAdType() {
		return adType;
	}
	public void setAdType(String adType) {
		this.adType = adType;
	}
	public String getClickUrl() {
		return clickUrl;
	}
	public void setClickUrl(String clickUrl) {
		this.clickUrl = clickUrl;
	}
	public String getImageUrl() {
		return imageUrl;
	}
	public void setImageUrl(String imageUrl) {
		this.imageUrl = imageUrl;
	}
	public String getAdText() {
		return adText;
	}
	public void setAdText(String adText) {
		this.adText = adText;
	}
	public String getAdContent()
	{
		return adContent;
	}
	public void setAdContent(String adContent)
	{
		this.adContent = adContent;
	}
	/**
	 * Return selected attribute when a particular options is selected for the parameter
	 * @param request
	 * @param name
	 * @param value
	 * @return
	 */
	public String isSelected(HttpServletRequest request, String name, String value)
	{
		String paramValue = (String) request.getAttribute(name);
		if (paramValue == null) return "";
		
		if (paramValue.equals(value)){
			return " selected=\"selected\"";
		}
		return "";
	}
	/**
	 * Return checkbox's checked attribute when the checkbox is checked for the parameter
	 * @param request
	 * @param name
	 * @param value
	 * @return
	 */
	public String isChecked(HttpServletRequest request, String name, String value)
	{
		String paramValue = (String) request.getAttribute(name);
		if (paramValue == null) return "";
		
		if (paramValue.equals(value)){
			return " checked";
		}
		return "";
	}
	/**
	 * Returns value of the attribute
	 * @param request
	 * @param name
	 * @return
	 */
	public String getAttribute(HttpServletRequest request, String name)
	{
		String value = (String) request.getAttribute(name);
		if (value != null) return value;
		return "";
	}

}
