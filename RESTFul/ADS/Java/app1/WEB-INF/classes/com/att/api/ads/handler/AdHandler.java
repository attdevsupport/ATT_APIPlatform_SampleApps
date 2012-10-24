package com.att.api.ads.handler;

import java.util.ArrayList;
import java.util.Map;
import java.util.Set;

import javax.servlet.http.HttpServletRequest;

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.methods.GetMethod;
import org.json.JSONObject;

import com.att.api.ads.model.AdsResponse;
import com.att.api.validation.CommonValidator;

/**
 * 
 * @author Sendhil Chokkalingam Ad API Handler which encapsulates Ad API call
 *         for the sample application and returns the model for JSP for
 *         rendering
 * 
 * */
public class AdHandler {

	private String UDID;
	private String endpoint;
	private HttpServletRequest request;
	private String adType;

	/**
	 * 
	 * @param endpoint
	 *            API endpoint
	 * @param UDID
	 *            UDID
	 * @param adType
	 * 			  Ad Type - configuration value, if none exists API will default to text and image
	 * @param request
	 *            Servlet Request
	 */
	public AdHandler(String endpoint, String UDID, String adType,
			HttpServletRequest request) {
		this.request = request;
		this.UDID = UDID;
		this.adType = adType;
		this.endpoint = endpoint;
	}

	/**
	 * Parses the request parameters and invokes the Ad API for processing
	 */
	public AdsResponse processRequest() throws Exception {

		String accessToken = (String) request.getSession().getAttribute(
				"accessToken");
		AdsResponse model = new AdsResponse();
		if (accessToken == null || accessToken.length() == 0) {
			model.setResultStatus(false);
			model.setStatusCode((Integer) request.getSession().getAttribute(
					"statusCode"));
			model.setErrorResponse((String) request.getSession().getAttribute(
					"errorResponse"));
			return model;
		}

		String params = parseRequestParams();
		if (params.length() != 0) {
			model.setFormStatus(true);
			ArrayList<String> errors = CommonValidator
					.validateInputParams(request);
			if (errors.size() > 0) {
				model.setErrors(errors);
				return model;
			}

			String category = request.getAttribute("category").toString();
			// Initialize the client
			HttpClient client = new HttpClient();
			GetMethod method = new GetMethod(endpoint);
			try {
				// setup request parameters
				method.setQueryString("Category=" + category + "&" + params);
				method.addRequestHeader("Accept", "application/json");
				method.addRequestHeader("Authorization", "Bearer "
						+ accessToken);
				method.addRequestHeader("User-Agent",
						request.getHeader("user-agent"));
				method.addRequestHeader("UDID", UDID);
				// Send the request, parse based on HTTP status code
				int statusCode = client.executeMethod(method);
				if (statusCode == 200) {
					model.setResultStatus(true);
					model.setStatusCode(200);
					model.setErrorResponse(null);
				} else {
					model.setResultStatus(false);
					String errorResponse = method.getResponseBodyAsString();
					if (errorResponse == null) {
						errorResponse = method.getStatusText();
					}
					model.setErrorResponse(errorResponse);
					model.setStatusCode(statusCode);
				}
				if (model.getResultStatus()) {
					JSONObject jsonResponse = new JSONObject(
							method.getResponseBodyAsString());
					JSONObject adsResponse = jsonResponse
							.getJSONObject("AdsResponse");
					JSONObject ads = adsResponse.getJSONObject("Ads");
					//
					model.setAdType(ads.getString("Type"));
					model.setClickUrl(ads.getString("ClickUrl"));
					//

					if (model.getAdType().equals("thirdparty")) {
						model.setAdText(ads.getString("Text"));
						model.setAdContent(ads.getString("Content"));
					} else if (model.getAdType().equals("text")) {
						model.setAdText(ads.getString("Text"));
					} else if (model.getAdType().equals("image")) {
						JSONObject imageUrl = ads.getJSONObject("ImageUrl");
						model.setImageUrl(imageUrl.getString("Image"));
					}
				}
			} finally {
				method.releaseConnection();
			}
		}
		return model;
	}

	/**
	 * parse the request parameter and its value and return the string
	 * url-encoded form
	 */
	@SuppressWarnings("unchecked")
	private String parseRequestParams() {
		// check whether submit button is pressed
		if (request.getParameter("btnGetAds") == null)
			return "";

		StringBuilder sbuilder = new StringBuilder();
		Set<Map.Entry<String, String[]>> entries = (Set<Map.Entry<String, String[]>>) request
				.getParameterMap().entrySet();

		for (Map.Entry<String, String[]> entry : entries) {
			String[] values = entry.getValue();
			if (values != null && !entry.getKey().equals("btnGetAds")) {
				if (entry.getKey().equals("MMA")) {
					sbuilder.append(buildAdSizeParams(values[0]));
				} else {
					// take the first parameter value
					sbuilder.append(entry.getKey());
					sbuilder.append("=");
					sbuilder.append(values[0]);
					sbuilder.append("&");
				}
				request.setAttribute(entry.getKey(), values[0]);
			}
		}
		//if we have ad type available then send it as part of request
		if (adType != null && adType.length() == 0) {
			sbuilder.append("Type=");
			sbuilder.append(adType);
		}
		sbuilder.trimToSize();
		return sbuilder.toString();
	}

	/**
	 * Builds Ad size parameters
	 * 
	 * @param mmaSize
	 * @return
	 */
	private String buildAdSizeParams(String mmaSize) {
		StringBuilder adSize = new StringBuilder("");
		if (mmaSize == null || mmaSize.length() == 0)
			return adSize.toString();
		String[] size = mmaSize.split("x");
		int w = Integer.parseInt(size[0].trim());
		int h = Integer.parseInt(size[1].trim());
		int min = Math.min(w, h);
		int max = Math.max(w, h);
		adSize.append("MinWidth=").append(min).append("&");
		adSize.append("MinHeight=").append(min).append("&");
		adSize.append("MaxHeight=").append(max).append("&");
		adSize.append("MaxWidth=").append(max).append("&");
		return adSize.toString();
	}

}