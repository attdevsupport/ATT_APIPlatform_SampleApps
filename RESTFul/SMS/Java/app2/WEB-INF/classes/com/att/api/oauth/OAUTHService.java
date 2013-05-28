package com.att.api.oauth;

import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.text.ParseException;

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.HttpException;
import org.apache.commons.httpclient.methods.PostMethod;
import org.apache.commons.httpclient.methods.StringRequestEntity;
import org.json.JSONObject;

public class OAUTHService {

	private String savedAccessToken;
	private String savedRefreshToken;
	private Long savedRefreshTokenExpiry;
	private Long savedAccessTokenExpiry;

	private static final Long ACCESSTOKEN_EXPIRY = Long
			.parseLong("3155692597470");
	private static final Long REFRESHTOKEN_EXPIRY = Long.parseLong("86400000");

	private static final String APIURL = "/oauth/token";
	private String endPoint;
	private OAUTHResponse oauthResponse;

	/**
	 * 
	 * @param FQDN
	 *            API Domain name
	 * @param savedAccessToken
	 *            previously saved access token
	 * @param savedRefreshToken
	 *            previously saved refresh token
	 * @param savedRefreshTokenExpiry
	 *            previously saved refresh token expiry
	 * @param savedAccessTokenExpiry
	 *            previously saved access token expiry
	 */
	public OAUTHService(String FQDN, String savedAccessToken,
			String savedRefreshToken, Long savedRefreshTokenExpiry,
			Long savedAccessTokenExpiry) {
		this.savedAccessToken = savedAccessToken;
		this.savedRefreshToken = savedRefreshToken;
		this.savedRefreshTokenExpiry = savedRefreshTokenExpiry;
		this.savedAccessTokenExpiry = savedAccessTokenExpiry;
		this.endPoint = FQDN.concat(APIURL);
		this.oauthResponse = new OAUTHResponse();
	}

	/**
	 * Returns access token based on client credentials. Following rules are
	 * applied before getting new access token 1. Check to see whether the
	 * access token is expired 2. Check to see whether the token can be
	 * refreshed 3. If all the above fails then retrieves new token and save it
	 * to supplied JSP file for later retrieval
	 * 
	 * @param scope
	 * @param clientIdAut
	 * @param clientSecretAut
	 * @return
	 */
	public OAUTHResponse getAccessToken(String scope, String clientIdAut,
			String clientSecretAut) {
		if (!validateClientCredentials(scope,clientIdAut,clientSecretAut)) return oauthResponse;
		
		Long currentTime = System.currentTimeMillis();
		if (currentTime < savedAccessTokenExpiry) {
			oauthResponse.setStatus(true);
			oauthResponse.setAccessToken(savedAccessToken);
			oauthResponse.setRefreshToken(savedRefreshToken);
			oauthResponse.setAccessTokenExpiry(savedAccessTokenExpiry);
			oauthResponse.setRefreshTokenExpiry(savedRefreshTokenExpiry);
			return oauthResponse;
		} else if (currentTime < savedRefreshTokenExpiry) {
			// Otherwise if there is a refresh token that has not expired,
			// use that to renew and save to file
			refreshTokens(clientIdAut, clientSecretAut, currentTime);
		} else if (currentTime > savedRefreshTokenExpiry) {
			// Otherwise get a new access token and refresh token, and save
			// them to file
			getNewTokens(scope, clientIdAut, clientSecretAut,
					currentTime);
		}
		return oauthResponse;
	}
	
	/**
	 * Validates input parameters
	 * @param scope
	 * @param clientIdAut
	 * @param clientSecretAut
	 * @return
	 */
	private boolean validateClientCredentials(String scope, String clientIdAut, String clientSecretAut){
		if (scope == null
				|| clientIdAut == null
				|| clientSecretAut == null) {
			oauthResponse.setStatus(false);
			oauthResponse.setStatusCode(-1);
			oauthResponse
					.setErrorResponse("Can not get access token, empty client credentials...");
			return false;
		}
		if (scope.length() == 0
				|| clientIdAut.length() == 0
				|| clientSecretAut.length() == 0) {
			oauthResponse.setStatus(false);
			oauthResponse.setStatusCode(-1);
			oauthResponse
					.setErrorResponse("Can not get access token, empty client credentials...");
			return false;
			
		}	
		return true;
	}
	
	/**
	 * Retrieve new access token and save it for later use
	 * 
	 * @param scope
	 * @param clientIdAut
	 * @param clientSecretAut
	 * @param date
	 * @return
	 */
	private void getNewTokens(String scope, String clientIdAut,
			String clientSecretAut, Long date) {
		String b = "client_id=" + clientIdAut + "&client_secret="
				+ clientSecretAut + "&grant_type=client_credentials&scope="
				+ scope;
		PostMethod method = sendReceive(clientIdAut, clientSecretAut, b);
		parseResponse(method, date);
	}

	/**
	 * Parse successful JSON response
	 * 
	 * @param method
	 * @param currentTime
	 * @return
	 */
	private void parseResponse(PostMethod method, Long currentTime) {
		int statusCode = method.getStatusCode();
		String accessToken = "";
		if (statusCode == 200) {
			JSONObject rpcObject;
			try {
				rpcObject = new JSONObject(method.getResponseBodyAsString());
				accessToken = rpcObject.getString("access_token");
				String refreshToken = rpcObject.getString("refresh_token");
				String expires_in = rpcObject.getString("expires_in");
				Long accessTokenExpiry = null;
				if (expires_in.equals("0")) {
					accessTokenExpiry = currentTime + ACCESSTOKEN_EXPIRY; // 100
																			// years
				}
				Long refreshTokenExpiry = currentTime + REFRESHTOKEN_EXPIRY;
				oauthResponse.setStatus(true);
				oauthResponse.setAccessToken(accessToken);
				oauthResponse.setRefreshToken(refreshToken);
				oauthResponse.setAccessTokenExpiry(accessTokenExpiry);
				oauthResponse.setRefreshTokenExpiry(refreshTokenExpiry);

			} catch (ParseException e) {
				setErrorResponse(statusCode, e.getMessage());
			} catch (IOException e) {
				setErrorResponse(statusCode, e.getMessage());
			}
		} else {
			try {
				String errorResponse = method.getResponseBodyAsString() == null ? method
						.getStatusText() : method.getResponseBodyAsString();
				setErrorResponse(statusCode, errorResponse);
			} catch (IOException e) {
				setErrorResponse(statusCode, e.getMessage());
			}
		}
	}

	/**
	 * Refresh token and save it for future use
	 * 
	 * @param clientIdAut
	 * @param clientSecretAut
	 * @param date
	 * @return
	 */
	private void refreshTokens(String clientIdAut, String clientSecretAut,
			Long date) {
		String b = "client_id=" + clientIdAut + "&client_secret="
				+ clientSecretAut + "&grant_type=refresh_token&refresh_token="
				+ savedRefreshToken;
		PostMethod method = sendReceive(clientIdAut, clientSecretAut, b);
		parseResponse(method, date);
	}

	/**
	 * Invoke API end point to get the new set of tokens
	 * 
	 * @param clientIdAut
	 * @param clientSecretAut
	 * @param requestBody
	 * @return
	 */
	private PostMethod sendReceive(String clientIdAut, String clientSecretAut,
			String requestBody) {
		HttpClient client = new HttpClient();
		PostMethod method = new PostMethod(this.endPoint);
		method.addRequestHeader("Content-Type",
				"application/x-www-form-urlencoded");
		method.setRequestEntity(new StringRequestEntity(requestBody));

		try {
			client.executeMethod(method);
			return method;
		} catch (HttpException e) {
			setErrorResponse(method.getStatusCode(), e.getMessage());
		} catch (IOException e) {
			setErrorResponse(method.getStatusCode(), e.getMessage());
		}
		return method;
	}

	/**
	 * Set error response variables in session
	 * 
	 * @param statusCode
	 * @param ex
	 */
	private void setErrorResponse(int statusCode, String msg) {
		oauthResponse.setStatusCode(statusCode);
		oauthResponse.setStatus(false);
		oauthResponse.setErrorResponse(msg);
	}

	/**
	 * Save tokens, expiry values into JSP for later use
	 * 
	 * @throws IOException
	 */
	public void saveTokenToJSP(String filePath) throws IOException {
		PrintWriter outWrite = new PrintWriter(new BufferedWriter(
				new FileWriter(filePath)), false);
		String toSave = "\u003C\u0025\nString savedAccessToken = \""
				+ oauthResponse.getAccessToken()
				+ "\";\nLong savedAccessTokenExpiry = Long.parseLong(\""
				+ oauthResponse.getAccessTokenExpiry()
				+ "\");\nString savedRefreshToken = \""
				+ oauthResponse.getRefreshToken()
				+ "\";\nLong savedRefreshTokenExpiry = Long.parseLong(\""
				+ oauthResponse.getRefreshTokenExpiry() + "\");\n\u0025\u003E";
		outWrite.write(toSave);
		outWrite.close();
	}

	public OAUTHResponse getOauthResponse() {
		return oauthResponse;
	}

	public void setOauthResponse(OAUTHResponse oauthResponse) {
		this.oauthResponse = oauthResponse;
	}

}
