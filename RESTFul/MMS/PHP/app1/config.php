<?php

// Mandatory parameter that should be set to the registered application's 
// 'API Key' value.
$clientId = "ENTER_VALUE";

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
$clientSecret = "ENTER_VALUE";

// Mandatory parameter. Registered application's shortcode.
$notificationShortcode = 'ENTER_VALUE';

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
$FQDN = "https://api.att.com";

// Mandatory parameter. Scope to use when requesting access token.
$scope = "MMS";

// Mandatory parameter. File to save access token after request. File/folder 
// must have write permissions.
$oauthFile = sys_get_temp_dir() . '/mmsoauthtoken.php';

// URL Link for this sample app's Github code.
$linkGithub = "#";

// URL link for this sample app's download.
$linkDownload = "#";

// File used for saving status notifications
$statusFile = sys_get_temp_dir() . '/mmsstatus.db';

// Name of the path used for saving MMS messages sent to shortcode.
$imagesPathName = 'messages';

// Directory used for saving MMS messages sent to shortcode. Directory must
// have write permissions.
$imagesPath = __DIR__ . '/' . $imagesPathName;

// File used to story meta information about images. File/folder must have
// write permissions.
$imagesDbPath = $imagesPath . '/mmslistener.db';

// Max number of entries to save for status notifications
$statusLimit = 5;

// Max number of MMS images to save
$mmsLimit = 3;

// Optional parameters. Set any proxy settings to use.
// $proxyHost = 'proxy.host';
// $proxyPort = 8080;

// Optional parameter. Sets whether to accept all certificates, such as 
// self-signed certificates. Useful for testing but should not be used on
// production.
// $acceptAllCerts = false;

?>
