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

	//For the first time, we dont have access token, so we redirect to authenticate client id
	if((session.getAttribute("accessToken") == null && request.getParameter("error_description") == null && code.length() == 0)) 
   	{
   		response.sendRedirect(FQDN + "/oauth/authorize?client_id=" + clientIdAut + "&scope=" + scope + "&redirect_uri=" + redirectUri);
   	}
	//Second time, if client id is valid we should now have the code parameter on the url. Use the code get the access token and set the token in session
   	else if(!code.equalsIgnoreCase("")) 
   	{

        String url = FQDN + "/oauth/token";   
        HttpClient client = new HttpClient(); 
        PostMethod method = new PostMethod(url); 
        String b = "client_id=" + clientIdAut + "&client_secret=" + clientSecretAut + "&grant_type=authorization_code&code=" + code;
        method.addRequestHeader("Accept","application/json");
        method.addRequestHeader("Content-Type","application/x-www-form-urlencoded");
        method.setRequestEntity(new StringRequestEntity(b));
        int statusCode = client.executeMethod(method);    
        if(statusCode==200)
        { 
           	JSONObject rpcObject = new JSONObject(method.getResponseBodyAsString());
           	String accessToken = rpcObject.getString("access_token");

           	refreshToken = rpcObject.getString("refresh_token");
           	session.setAttribute("refreshToken", refreshToken);
            session.setAttribute("accessToken", accessToken);

            String expires_in = rpcObject.getString("expires_in");
            method.releaseConnection();
	
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
		response.sendRedirect("MIM.jsp?" + session.getAttribute("requestType"));
    }
    //Refresh token scenario
    else if(!getRefreshToken.equalsIgnoreCase("")) 
    {
	    String url = FQDN + "/oauth/token";   
	    HttpClient client = new HttpClient();
	    PostMethod method = new PostMethod(url); 
	    String b = "client_id=" + clientIdAut + "&client_secret=" + clientSecretAut + "&grant_type=refresh_token&refresh_token=" + refreshToken;
	    method.addRequestHeader("Content-Type","application/x-www-form-urlencoded");
        method.setRequestEntity(new StringRequestEntity(b));
	    int statusCode = client.executeMethod(method);    
	    if(statusCode==200)
	    { 
		   	String accessToken = method.getResponseBodyAsString().substring(18,50);
		   	session.setAttribute("accessToken", accessToken);
		   	String postOauth = (String) session.getAttribute("postOauth");
		   	if(postOauth!= null) 
		   	{
		   		session.setAttribute("postOauth", null);
		   		response.sendRedirect(postOauth);
		   	}
	    }
	    method.releaseConnection();
		response.sendRedirect("MIM.jsp?" + session.getAttribute("requestType"));
    }
    else if (request.getParameter("error") != null)
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
		response.sendRedirect("MIM.jsp?" + session.getAttribute("requestType"));
    }
%>
