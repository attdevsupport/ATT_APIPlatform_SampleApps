package com.att.api.dc.handler;

import java.io.IOException;
import java.text.ParseException;

import javax.servlet.http.HttpServletRequest;

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.HttpException;
import org.apache.commons.httpclient.methods.GetMethod;
import org.json.JSONObject;

import com.att.api.dc.model.DeviceInfo;

public class DCHandler {
	
	private HttpServletRequest request;
	private String endPoint;
	private String accessToken;
	
	/**
	 * 
	 * @param request
	 * @param endPoint
	 * @param accessToken
	 */
	public DCHandler(HttpServletRequest request, String endPoint, String accessToken){
		this.endPoint = endPoint;
		this.request = request;
		this.accessToken = accessToken;
	}
	
	/**
	 * 
	 * @throws HttpException
	 * @throws IOException
	 * @throws ParseException
	 */
	public DeviceInfo processRequest() throws HttpException, IOException, ParseException
	{
		GetMethod method = sendReceive(endPoint,accessToken,null);
	    int statusCode = method.getStatusCode();
		String response = method.getResponseBodyAsString();
		method.releaseConnection();

		DeviceInfo info = null;
	    if (statusCode == 200)
		{
	    	info = buildModel(response);
	    	info.setStatus(true);
		}
	    else
	    {
	    	info = new DeviceInfo(false,response);
	    	if (response == null) info.setErrorResponse("Code:" + statusCode);
	    }
	    return info;
	}
	
	/**
	 * 
	 * @param response
	 * @return
	 * @throws ParseException
	 */
	private DeviceInfo buildModel(String response) throws ParseException{
		
		DeviceInfo deviceInfo = new DeviceInfo();
		JSONObject json = new JSONObject(response);
		
		JSONObject device = json.getJSONObject("DeviceInfo").getJSONObject("DeviceId");
		deviceInfo.setTypeAllocationCode(device.getString("TypeAllocationCode"));
		
		JSONObject capabilities = json.getJSONObject("DeviceInfo").getJSONObject("Capabilities");
		deviceInfo.setName(capabilities.get("Name").toString());
		deviceInfo.setVendor(capabilities.get("Vendor").toString());
		deviceInfo.setModel(capabilities.get("Model").toString());
		deviceInfo.setFirmwareVersion(capabilities.get("FirmwareVersion").toString());
		deviceInfo.setUaProf(capabilities.get("UaProf").toString());
		deviceInfo.setMmsCapable(capabilities.get("MmsCapable").toString());
		deviceInfo.setAssitedGps(capabilities.get("AssistedGps").toString());
		deviceInfo.setLocationTechnology(capabilities.get("LocationTechnology").toString());
		deviceInfo.setBrowserType(capabilities.get("DeviceBrowser").toString());
		deviceInfo.setWapPushCapable(capabilities.get("WapPushCapable").toString());
		
		return deviceInfo;
	}
	
	
	/**
	 * 
	 * @param endpointURI
	 * @param accessToken
	 * @param body
	 * @return
	 * @throws HttpException
	 * @throws IOException
	 */
	private GetMethod sendReceive(String endpointURI, String accessToken,
			String body) throws HttpException, IOException {
			GetMethod method = new GetMethod(endpointURI);
			// Initialize the client
			HttpClient client = new HttpClient();
			// setup request parameters
			method.addRequestHeader("Accept", "application/json");
			method.addRequestHeader("Accept-Encoding","gzip, deflate");
			method.addRequestHeader("Content-Type","application/json");
			method.addRequestHeader("Authorization","Bearer " + accessToken);
			method.addRequestHeader("User-Agent", request.getHeader("user-agent"));
			client.executeMethod(method);
			return method;
	}
	
	
}
