<?php

// Mandatory parameter that should be set to the registered application's 
// 'API Key' value.
$clientId = "ENTER_VALUE";

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
$clientSecret = "ENTER_VALUE";

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
$FQDN = "https://api.att.com";

// Mandtory parameter. File to save access token after request. File/folder 
// must have write permissions.
$oauthFile = sys_get_temp_dir() . "/adsoauthtoken.php";

// Mandatory parameter. Scope to use when requesting access token.
$scope = "ADS";

// Mandatory parameter. Specifies the universally unique identifier. 
// See 'https://developer.att.com/docs/apis/rest/1/Advertising' for a detailed
// explanation.
$udid = "ENTER_VALUE";

// Mandatory parameter. Specifies the user agent to send (must be mobile).
// e.g. $userAgent = 'Mozilla/5.0 (Android; Mobile; rv:13.0) Gecko/13.0 Firefox/13.0';
$userAgent = 'ENTER_VALUE';

// URL Link for this sample app's Github code.
$linkGithub = "#";

// URL Link for this sample app's source code.
$linkDownload='#';

// Optional parameters. Set any proxy settings to use.
// $proxy_host = 'proxy.host';
// $proxy_port = 8080;

// Optional parameter. Sets whether to accept all certificates, such as 
// self-signed certificates. Useful for testing but should not be used on
// production.
// $accept_all_certs = false;

?>
