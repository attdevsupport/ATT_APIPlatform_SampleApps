<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2013. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category Authentication 
 * @package OAuth
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/OAuthToken.php';
require_once __DIR__ . '/OAuthRequest.php';
require_once __DIR__ . '/OAuthException.php';

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting access 
 * tokens. This implementation of the OAuth 2.0 Framework provides two models
 * for obtaining an access token, which can then be used for requesting access
 * to protected resources. These models are:
 * <ul>
 * <li>
 * Authorization Code - Uses a subscriber context by requesting an 
 * authorization code before requesting an access token.
 * </li>
 * Client Credentials - Sends a direct request for an access token using a 
 * client id, client secret, and scope.
 * </li>
 * </ul>
 *
 * @see https://tools.ietf.org/html/rfc6749
 * 
 * @package OAuth 
 */
class OAuthTokenRequest extends OAuthRequest {
    // URL to use when requesting an access token
    private $_URL;

    // Client ID to use when requesting access token
    private $_clientId;

    // Client secret to use when requesting access token
    private $_clientSecret;


    /**
     * Creates an OAuthTokenRequest object with the specified URL, client id, 
     * and client secret. These values will then be used when requesting an
     * access token.
     * 
     * @param string $URL endpoint URL
     * @param string $clientId client id
     * @param string $clientSecret client secret
     */
    public function __construct($URL, $clientId, $clientSecret) {
        $this->_URL = $URL;
        $this->_clientId = $clientId;
        $this->_clientSecret = $clientSecret;
    }


    /**
     * Gets an access token using the specified scope. The parameters supplied
     * will be used when requesting an access token.
     *
     * @param string $scope scope to use when requesting access token
     * @return OAuthToken an access token
     * @throws OAuthException if server did not return valid access token
     */
    public function getToken($scope) {
        $postData = 'client_id=' . urlencode($this->_clientId) . 
            '&client_secret=' . urlencode($this->_clientSecret) .
            '&scope=' . urlencode($scope) .
            '&grant_type=' . urlencode('client_credentials');

        $headers = array('Content-Type: application/x-www-form-urlencoded
                ');

        $options = array(CURLOPT_URL => $this->_URL, //set the url
                CURLOPT_HEADER => false, // ignore headers from result
                CURLINFO_HEADER_OUT => false,
                CURLOPT_HTTPHEADER => $headers,
                CURLOPT_RETURNTRANSFER => true, // return result as a string
                CURLOPT_SSL_VERIFYPEER => false, // allow self-signed certs
                CURLOPT_POST => true, 
                CURLOPT_POSTFIELDS => $postData
                );

        $connection = curl_init();
        curl_setopt_array($connection, $options);
        $response = curl_exec($connection);
        $responseCode = curl_getinfo($connection, CURLINFO_HTTP_CODE);

        if ($responseCode == 200 || $responseCode == 201) { //success
            $jsonToken = json_decode($response);

            return new OAuthToken(
                    $jsonToken->access_token,
                    $jsonToken->expires_in,
                    $jsonToken->refresh_token);
        }
        
        // unable to get token
        throw new OAuthException($responseCode, $response);
    }
}

?>
