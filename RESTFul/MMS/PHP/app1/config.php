<?php

// Mandatory parameter that should be set to the registered application's 
// 'API Key' value.
$api_key = "";

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
$secret_key = "";

// Mandatory parameter. Registered application's shortcode.
$shortCode = '5555551234';

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
$FQDN = "https://api.att.com";

// Mandatory parameter. Scope to use when requesting access token.
$scope = "MMS";

// Mandatory parameter that specifies attachments directory to use for any
// attachments to send.
$attachments_dir = "attachment";

// Mandtory parameter. File to save access token after request. File/folder 
// must have write permissions.
$oauth_file = "mmsoauthtoken.php";

// URL Link for this sample app's source code.
$linkSource = "#";

// URL link for this sample app's download.
$linkDownload = "#";

// URL link for this sample app's help page.
$linkHelp = "#";

// Optional parameters. Set any proxy settings to use.
// $proxy_host = 'proxy.host';
// $proxy_port = 8080;

// Optional parameter. Sets whether to accept all certificates, such as 
// self-signed certificates. Useful for testing but should not be used on
// production.
// $accept_all_certs = false;

?>
