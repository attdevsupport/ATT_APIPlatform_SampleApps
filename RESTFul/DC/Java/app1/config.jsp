
<% 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>

<%

//Provisioned from App
//Client id
String client_id = ""; 
//Secret key
String secret_key = ""; 
//Application Scope
String scope = "DC";

//API Host name
String FQDN = "https://api.att.com";

//Device Capabilities
String getdc_url = "/rest/2/Devices/Info";

//Endpoint
String endpoint = FQDN + getdc_url;


//Application redirect URL, OAUTH will redirect to this URL. The value must match as provisioned by App
String redirectURL = "";


%>
