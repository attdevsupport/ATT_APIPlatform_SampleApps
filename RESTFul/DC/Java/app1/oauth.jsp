<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>
<%@ page contentType="text/html; charset=iso-8859-1" language="java"%>
<%@ page import="java.util.Date" %>
<%@ page import="java.io.PrintWriter" %>
<%@ page import="java.io.BufferedWriter" %>
<%@ page import="java.io.FileWriter" %>
<%@ page import="org.apache.commons.httpclient.*"%>
<%@ page import="org.apache.commons.httpclient.methods.*"%>
<%@ page import="org.json.JSONObject"%>
<%@ page import="java.util.Map" %>
<%@ page import="java.net.URLDecoder" %>
<%@ include file="OauthStorage.jsp" %>
<%@ include file="config.jsp"%>
<%
	Long currentTime = System.currentTimeMillis();
	String accessTokenError = "";
	String code = request.getParameter("code");
	if(code==null) code="";
	
	String refreshToken = request.getParameter("refreshToken");
	if (refreshToken==null) 
		refreshToken=(String) session.getAttribute("refreshToken");
	if (refreshToken==null) 
		refreshToken="";
	String getRefreshToken = request.getParameter("getRefreshToken");
	if (getRefreshToken==null) getRefreshToken="";

	//Second time, if client id is valid we should now have the code parameter on the url. Use the code get the access token and set the token in session
   	if(!code.equalsIgnoreCase("")) 
   	{
        String url = FQDN + "/oauth/token";   
        HttpClient client = new HttpClient(); 
        PostMethod method = new PostMethod(url); 
        String b = "client_id=" + client_id + "&client_secret=" + secret_key + "&grant_type=authorization_code&code=" + code;
        method.addRequestHeader("Accept","application/json");
        method.addRequestHeader("Content-Type","application/x-www-form-urlencoded");
        method.setRequestBody(b);
        int statusCode = client.executeMethod(method);    
        if(statusCode==200)
        { 
           	JSONObject rpcObject = new JSONObject(method.getResponseBodyAsString());
           	String accessToken = rpcObject.getString("access_token");

           	refreshToken = rpcObject.getString("refresh_token");
           	session.setAttribute("refreshToken", refreshToken);
            session.setAttribute("accessToken", accessToken);

            String expires_in = rpcObject.getString("expires_in");
			if (expires_in.equals("0"))
			{
				savedAccessTokenExpiry = currentTime + (Long.parseLong("3155692597470")); //100 years
			}
            savedRefreshTokenExpiry = currentTime + Long.parseLong("86400000");
            
            method.releaseConnection();
	
	     //Applications could reuse the access token, since the token is reserved per device
            PrintWriter outWrite = new PrintWriter(new BufferedWriter(new FileWriter(application.getRealPath("/OauthStorage.jsp"))), false);
            String toSave = "\u003C\u0025\nString savedAccessToken = \"" + accessToken + "\";\nLong savedAccessTokenExpiry = Long.parseLong(\"" + savedAccessTokenExpiry + "\");\nString savedRefreshToken = \"" + refreshToken + "\";\nLong savedRefreshTokenExpiry = Long.parseLong(\"" + savedRefreshTokenExpiry + "\");\n\u0025\u003E";
            outWrite.write(toSave);
            outWrite.close();
            String postOauth = (String) session.getAttribute("postOauth");
            if(postOauth!= null) 
            {
           	session.setAttribute("postOauth", null);
           	response.sendRedirect(postOauth);
            }
        }
	    else
		{
			accessTokenError = method.getResponseBodyAsString();
		       if (accessTokenError == null || accessTokenError.length() == 0) accessTokenError = "" + statusCode;
			session.setAttribute("errorResponse",accessTokenError);
		}
        method.releaseConnection();
    }
    //Refresh token scenario
    else if(!getRefreshToken.equalsIgnoreCase("")) 
    {
	    String url = FQDN + "/oauth/token";   
	    HttpClient client = new HttpClient();
	    PostMethod method = new PostMethod(url); 
	    String b = "client_id=" + client_id + "&client_secret=" + secret_key + "&grant_type=refresh_token&refresh_token=" + refreshToken;
	    method.addRequestHeader("Content-Type","application/x-www-form-urlencoded");
        method.setRequestBody(b);
	    int statusCode = client.executeMethod(method);    
	    if(statusCode==200)
	    { 
		   	String accessToken = method.getResponseBodyAsString().substring(18,50);
		   	session.setAttribute("accessToken", accessToken);
	    }
	    method.releaseConnection();
    }
    else if (request.getParameter("error") != null || request.getParameter("error_description") != null)
    {
		String error = (String) request.getParameter("error");
		String errorResponse = "";
		String errorDescription = request.getParameter("error_description");
		if (error != null )
		{
		   errorResponse = URLDecoder.decode(request.getQueryString(),"iso-8859-1");		
		}
		else if (errorDescription != null && errorDescription.length() != 0)
		{
			errorResponse = errorDescription;
		}
		session.setAttribute("errorResponse",errorResponse);
    }
%>
