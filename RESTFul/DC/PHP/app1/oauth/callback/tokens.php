<?php
/*
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
*/

require_once("../../config.php");

/**
 * Gets access token using specified parameters.
 * @param string $FQDN fully qualified domain nae
 * @param string $api_key api key
 * @param string $secret_key secret key
 * @param string $scope scope of access token
 * @param string $authCode authorization code
 * @return string the access token if successful, else null
 */
function GetAccessToken($FQDN, $api_key, $secret_key, $scope, $authCode) {
    //Form URL to get the access token
    $accessTok_Url1 = "$FQDN/oauth/token";
    
    //http header values
    $accessTok_headers = array(
            'Content-Type: application/x-www-form-urlencoded'
    );

    //Invoke the URL
    $post_data="client_id=".$api_key."&client_secret=".$secret_key."&code=".$authCode."&grant_type=authorization_code";

    $accessTok = curl_init();
    curl_setopt($accessTok, CURLOPT_URL, $accessTok_Url1);
    curl_setopt($accessTok, CURLOPT_HTTPGET, 1);
    curl_setopt($accessTok, CURLOPT_HEADER, 0);
    curl_setopt($accessTok, CURLINFO_HEADER_OUT, 0);
    curl_setopt($accessTok, CURLOPT_HTTPHEADER, $accessTok_headers);
    curl_setopt($accessTok, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($accessTok, CURLOPT_SSL_VERIFYPEER, false);
    curl_setopt($accessTok, CURLOPT_POST, 1);
    curl_setopt($accessTok, CURLOPT_POSTFIELDS,$post_data);
    $accessTok_response = curl_exec($accessTok);

    $responseCode=curl_getinfo($accessTok,CURLINFO_HTTP_CODE);
    /*If URL invocation is successful fetch the access token and return it,
        otherwise return null.
    */
    $access_token = null;
    if($responseCode == 200) {
        $jsonObj = json_decode($accessTok_response);
        $access_token = $jsonObj->{'access_token'};//fetch the access token from the response.
    } else {
        echo curl_error($accessTok);//
    }
    
    curl_close ($accessTok);

    return $access_token;
}

?>
