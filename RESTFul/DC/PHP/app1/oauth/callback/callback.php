<?php
/*
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
*/
header('P3P: CP="IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT"');
require_once('../../config.php');
require_once('tokens.php');

session_start();

$error = isset($_GET['error']) ? $_GET['error'] : null;
if ($error != null) {
    
    if (!isset($_GET['error_description'])) {
        die('GET parameters contain error but not error description!');
    }

    $_SESSION['error'] = $error;
    $_SESSION['error_description'] = $_GET['error_description'];
    header('Location: ' . POSTAUTH_URL);

}

$authCode = isset($_GET['code']) ? $_GET['code'] : null;
if (!($authCode == null || $authCode == '')) {
    $accessToken = GetAccessToken(FQDN, API_KEY, SECRET_KEY, SCOPE, $authCode);
    $_SESSION[SESSION_TOKEN_INDEX] = $accessToken;
    header('Location: ' . POSTAUTH_URL);
}


?>
