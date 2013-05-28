<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>
<%@ page contentType="text/html; charset=iso-8859-1" language="java"%>
<%@ page import="org.json.JSONObject"%>
<%@ page import="java.net.URLDecoder" %>
<%@ page import="org.apache.http.impl.client.DefaultHttpClient" %>
<%@ page import="org.apache.http.client.HttpClient" %>
<%@ page import="org.apache.http.client.methods.HttpPost" %>
<%@ page import="org.apache.http.entity.StringEntity" %>
<%@ page import="org.apache.http.HttpResponse" %>
<%@ page import="org.apache.http.util.EntityUtils" %>
<%@ page import="org.apache.http.conn.params.ConnRoutePNames" %>
<%@ page import="org.apache.http.HttpHost" %>
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
        String url = cfg.FQDN + "/oauth/token";
        HttpClient client = new DefaultHttpClient();
        HttpHost proxy = new HttpHost("proxy.entp.attws.com", 8080);
        client.getParams().setParameter(ConnRoutePNames.DEFAULT_PROXY, proxy);

        HttpPost method = new HttpPost(url);
        String b = "client_id=" + cfg.clientIdAuth + "&client_secret=" + cfg.clientSecretAuth + "&grant_type=authorization_code&code=" + code;
        method.setHeader("Accept", "application/json");
        method.setHeader("Content-Type", "application/x-www-form-urlencoded");
        method.setEntity(new StringEntity(b));
        HttpResponse httpResponse =  client.execute(method);
        int statusCode = httpResponse.getStatusLine().getStatusCode();
        if(statusCode==200)
        { 
           	JSONObject rpcObject = new JSONObject(EntityUtils.toString(httpResponse.getEntity()));
           	String accessToken = rpcObject.getString("access_token");

           	refreshToken = rpcObject.getString("refresh_token");
           	session.setAttribute("refreshToken", refreshToken);
            session.setAttribute("accessToken", accessToken);
System.out.println("accesstoken :" + accessToken);
            client.getConnectionManager().shutdown();
           	response.sendRedirect(cfg.postOAuthUri);
        }
	    else
		{
			accessTokenError = EntityUtils.toString(httpResponse.getEntity());
		       if (accessTokenError == null || accessTokenError.length() == 0) accessTokenError = "" + statusCode;
			session.setAttribute("errorResponse",accessTokenError);
		}
    }
    //Refresh token scenario
    else if(!getRefreshToken.equalsIgnoreCase("")) 
    {
	    String url = cfg.FQDN + "/oauth/token";
	    HttpClient client = new DefaultHttpClient();
        HttpHost proxy = new HttpHost("proxy.entp.attws.com", 8080);
        client.getParams().setParameter(ConnRoutePNames.DEFAULT_PROXY, proxy);

        HttpPost method = new HttpPost(url);
	    String b = "client_id=" + cfg.clientIdAuth + "&client_secret=" + cfg.clientSecretAuth + "&grant_type=refresh_token&refresh_token=" + refreshToken;
	    method.setHeader("Content-Type", "application/x-www-form-urlencoded");
        method.setEntity(new StringEntity(b));
        HttpResponse httpResponse = client.execute(method);
	    int statusCode = httpResponse.getStatusLine().getStatusCode();
	    if(statusCode==200)
	    { 
		   	String accessToken = EntityUtils.toString(httpResponse.getEntity()).substring(18,50);
		   	session.setAttribute("accessToken", accessToken);
	    }
        client.getConnectionManager().shutdown();
        response.sendRedirect(cfg.postOAuthUri);
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
        response.sendRedirect(cfg.postOAuthUri);
    }
%>
