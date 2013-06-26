<?php

// Mandatory parameter. File to save access token after request. File/folder 
// must have write permissions.
$oauth_file = 'token.php';

// Mandatory parameter that should be set to the registered application's 
// 'API Key' value.
$api_key = '';

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
$secret_key = '';

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
$FQDN = 'https://api.att.com';

// Mandatory parameter that specifies the authorization redirect URL. This is
// URL that the consent flow will redirect to.
$redirect_url = 'http://localhost/payment/index.php';

// Mandatory parameter. Scope to use when requesting access token.
$scope = 'PAYMENT';

// Min transaction value to use during payment request.
$minTransactionValue = '0.00';

// Max transaction value to use during payment request.
$maxTransactionValue = '2.99';

// Min subscription value to use during payment request.
$minSubscriptionValue = '0.00';

// Max subscription value to use during payment request.
$maxSubscriptionValue = '2.99';

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
