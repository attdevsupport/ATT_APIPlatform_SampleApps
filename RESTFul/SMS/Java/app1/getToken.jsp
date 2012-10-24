<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>

<%@ page contentType="text/html; charset=iso-8859-1" language="java" %>
<%@ include file="OauthStorage.jsp" %>
<%@ include file="config.jsp" %>
<%@ page import="com.att.api.oauth.OAUTHService"%>
<%@ page import="com.att.api.oauth.OAUTHResponse"%>

<%
//Initialize some variables here, check if relevant variables were passed in, if not then check session, otherwise set default.
String accessToken = (String) session.getAttribute("acessToken");

String oauthErrorResponse = "";
int oauthStatusCode = 0;
boolean oauthStatus = true;

if (request.getParameter("sendSms") !=  null ||
	request.getParameter("getSmsDeliveryStatus") != null ||
	request.getParameter("getReceivedSms") != null)
{

if (accessToken == null)
{
	OAUTHService oauthService = new OAUTHService(FQDN, savedAccessToken,savedRefreshToken,savedAccessTokenExpiry,savedRefreshTokenExpiry);
	OAUTHResponse oauthResponse = oauthService.getAccessToken(scope, clientIdAut, clientSecretAut);
	if (oauthResponse.isStatus())
	{
		accessToken = oauthResponse.getAccessToken();
		if (savedAccessToken.length() == 0){
			String filePath = application.getRealPath("/OauthStorage.jsp");
			oauthService.saveTokenToJSP(filePath);
		}
		session.setAttribute("accessToken", accessToken);
	}
	else
	{
		oauthErrorResponse = oauthResponse.getErrorResponse();
		oauthStatusCode = oauthResponse.getStatusCode();
		oauthStatus = false;
	}
}
}

%>
