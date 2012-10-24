<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>

<%

String clientIdAut = "";
String clientSecretAut =  "";
String FQDN = "https://api-uat.bf.pacer.sl.attcompute.com";

//Sample call control scripts location
String scriptPath = "/scripts/";

String sessionEndPoint = FQDN + "/rest/1/Sessions";
//{sessionid} will get replaced with session id
String signalEndPoint = FQDN + "/rest/1/Sessions/{sessionid}/Signals";
//Application's test phone number
String phoneNumber = "";

%>
