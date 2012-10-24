<?php
/*
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
*/

/* List global configuration constants */

define('API_KEY', '');
define('SECRET_KEY', '');

//FDQN = Fully qualified domain name
define('FQDN', 'https://api.att.com');

//DC = Device Capabilities
define('SCOPE', 'DC');

define('POSTAUTH_URL', '');
define('REDIRECT_URL', dirname(POSTAUTH_URL) . '/oauth/callback/callback.php');

define('AUTH_CODE_URL', FQDN . '/oauth/authorize?scope='. SCOPE .'&client_id=' . API_KEY . '&redirect_uri=' . REDIRECT_URL);
define('ACCESSTOK_URL', FQDN . '/oauth/token');
define('GETDCURL', FQDN . '/rest/2/Devices/Info');

//Index in session array for storing access token
define("SESSION_TOKEN_INDEX", "devicecapabilities_access_token");

?>
