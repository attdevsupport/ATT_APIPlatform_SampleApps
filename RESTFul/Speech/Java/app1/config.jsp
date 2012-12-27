<%//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com%>
<%@ page import="com.att.api.speech.handler.Config"%>
<%
Config cfg = new Config();

cfg.clientIdAuth = "";
cfg.clientSecretAuth = "";

cfg.FQDN = "https://api.att.com";
cfg.endPointURL = cfg.FQDN + "/rest/2/SpeechToText";

cfg.speechContexts = new String[] { "BusinessSearch", "Websearch", "SMS", "Voicemail",
	                        "QuestionAndAnswer", "TV", "Generic" };
				

// max size permitted for upload file
cfg.maxUploadFileSize = 10 * 1024 * 1024; //10 mb

cfg.defaultFile = "bostonCeltics.wav";

/*
the optional X-Arg HTTP Header may also be set
Example:
cfg.xarg.put("ClientVersion", "1.0.0");
cfg.xarg.put("DeviceType", "SAMSUNG-SGH-I927");
*/

cfg.trustAllCerts = false;

cfg.linkSource = "#";
cfg.linkDownload = "#";
cfg.linkHelp = "#";

cfg.audioFolder = "audio/";

session.setAttribute("cfg", cfg);
%>
