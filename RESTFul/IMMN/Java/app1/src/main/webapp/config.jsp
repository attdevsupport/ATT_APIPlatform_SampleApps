<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>
<%@ page import="com.att.api.immn.controller.Config" %>
<%
Config cfg = new Config(); 

cfg.clientIdAuth = "";
cfg.clientSecretAuth = "";
cfg.scope="IMMN,MIM";

cfg.FQDN = "";
cfg.endPointURL = cfg.FQDN + "/rest/1/MyMessages";
cfg.redirectUri = "http://localhost:8080/immn/oauth";
cfg.postOAuthUri = "/postOauth";

// max size permitted for upload file
cfg.maxUploadFileSize = 10 * 1024 * 1024; //10 mb

cfg.trustAllCerts = true;

cfg.linkSource = "#";
cfg.linkDownload = "#";
cfg.linkHelp = "#";

cfg.attachmentFolder = "attachments/";
cfg.proxyHost = "";
cfg.proxyPort = 0;

session.setAttribute("cfg", cfg);
%>

