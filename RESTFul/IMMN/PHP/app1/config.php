<?php
// Mandatory parameter that should be set to the registered application's 
// 'API Key' value.
$api_key = "";

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
$secret_key = "";

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
$FQDN = "https://api.att.com";

// Mandatory parameter that specifies the authorization redirect URL. This is
// URL that the consent flow will redirect to.
$authorize_redirect_uri = "localhost/IMMN/index.php";

// Mandatory parameter. Scope to use when requesting access token.
$scope = "IMMN,MIM";

// Folder to use when sending any attachments.
$attachments_folder = 'attachments';

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
