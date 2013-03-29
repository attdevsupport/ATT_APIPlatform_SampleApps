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

require_once __DIR__ . '/OAuthCode.php';
require_once __DIR__ . '/OAuthToken.php';
require_once __DIR__ . '/OAuthException.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';

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
class OAuthTokenRequest extends RESTFulRequest {
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
        parent::__construct($URL);
        $this->addParam('client_id', $clientId);
        $this->addParam('client_secret', $clientSecret);
    }

    /**
     * Gets an access token using the specified code. The parameters previously
     * supplied will be used when requesting an access token.
     * <p>
     * The token request is done using the authorization_code grant type.
     *
     * @param OAuthCode code to use when requesting access token
     * @return OAuthToken an access token
     * @throws OAuthException if server did not return valid access token
     */
    public function getTokenUsingCode(OAuthCode $code) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->addParam('code', '' . $code);
        $this->addParam('grant_type', 'authorization_code');
        $result = $this->sendRequest();
    
        $response = $result['response'];
        $responseCode = $result['responseCode'];

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

    /**
     * Gets an access token using the specified scope. The parameters 
     * previously supplied will be used when requesting an access token.
     * <p>
     * The token request is done using the client_credentials grant type.
     *
     * @param string $scope scope to use when requesting access token
     * @return OAuthToken an access token
     * @throws OAuthException if server did not return valid access token
     */
    public function getTokenUsingScope($scope) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->addParam('scope', $scope);
        $this->addParam('grant_type', 'client_credentials');

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];

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

//    // TODO: Implement
//    public function refreshToken(OAuthToken $token) {
//    }

}

?>
