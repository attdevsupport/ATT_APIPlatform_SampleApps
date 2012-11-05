package com.att.api.oauth;

public class OAUTHResponse {
	
	private String accessToken;
	private String refreshToken;
	private Long accessTokenExpiry;
	private Long refreshTokenExpiry;
	private boolean status;
	private String errorResponse;
	private int statusCode;
	
	public String getAccessToken() {
		return accessToken;
	}
	public void setAccessToken(String accessToken) {
		this.accessToken = accessToken;
	}
	public String getRefreshToken() {
		return refreshToken;
	}
	public void setRefreshToken(String refreshToken) {
		this.refreshToken = refreshToken;
	}
	public Long getAccessTokenExpiry() {
		return accessTokenExpiry;
	}
	public void setAccessTokenExpiry(Long accessTokenExpiry) {
		this.accessTokenExpiry = accessTokenExpiry;
	}
	public Long getRefreshTokenExpiry() {
		return refreshTokenExpiry;
	}
	public void setRefreshTokenExpiry(Long refreshTokenExpiry) {
		this.refreshTokenExpiry = refreshTokenExpiry;
	}
	public boolean isStatus() {
		return status;
	}
	public void setStatus(boolean status) {
		this.status = status;
	}
	public String getErrorResponse() {
		return errorResponse;
	}
	public void setErrorResponse(String errorResponse) {
		this.errorResponse = errorResponse;
	}
	public void setStatusCode(int code) {
		this.statusCode = code;
	}
	public int getStatusCode()
	{
		return this.statusCode;
	}
}
