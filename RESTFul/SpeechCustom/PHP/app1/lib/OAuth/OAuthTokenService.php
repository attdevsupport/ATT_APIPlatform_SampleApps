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
 * @category  Authentication 
 * @package   OAuth
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '/OAuthException.php';
require_once __DIR__ . '../../Srvc/Service.php';

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
 * <li>
 * Client Credentials - Sends a direct request for an access token using a 
 * client id, client secret, and scope.
 * </li>
 * </ul>
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 * @link     https://tools.ietf.org/html/rfc6749
 */
class OAuthTokenService extends Service
{
    const URL_PATH = '/oauth/token';

    /**
     * URL to which request for an OAuth token will be sent.
     *
     * @var string
     */
    private $_url;


    /** 
     * Client id.
     *
     * @var string
     */
    private $_clientId;

    /**
     * Client secret.
     *
     * @var string
     */
    private $_clientSecret;

    /**
     * Creates a RESTFulRequest object contains common information that all 
     * OAuth method calls share. 
     * 
     * @return RESTFulRequest created request
     */
    private function _createRESTFulRequest() 
    {
        $req = new RESTFulRequest($this->_url);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $req->addParam('client_id', $this->_clientId);
        $req->addParam('client_secret', $this->_clientSecret);
        return $req;
    }

    /**
     * Parses the result received from sending an API request for an OAuth 
     * token.
     *
     * @param array $result the result returned from a restful request
     *
     * @return OAuthToken oauth token if request was successful
     * @throws ServiceException if request was not successful
     * @see Service::parseResult()
     */
    protected function parseResult($result)
    {
        $tokenResponse = parent::parseResult($result);
        return new OAuthToken(
            $tokenResponse['access_token'],
            $tokenResponse['expires_in'],
            $tokenResponse['refresh_token']
        );
    }

    /**
     * Creates an OAuthTokenService object with the specified FQDN, client id, 
     * and client secret. These values will then be used when requesting an
     * access token. 
     * 
     * The request will be sent to the FQDN + OAuthTokenService::URL_PATH 
     * unless overriden using the setURL() method.
     * 
     * @param string $FQDN         fully qualified domain name 
     * @param string $clientId     client id
     * @param string $clientSecret client secret
     *
     * @return void
     */
    public function __construct($FQDN, $clientId, $clientSecret)
    {
        $this->_url = $FQDN . OAuthTokenService::URL_PATH;
        $this->_clientId = $clientId;
        $this->_clientSecret = $clientSecret;
    }
    
    /**
     * Sets the URL to send request to.
     *
     * @param string $url URL to send request to
     *
     * @return void
     */
    public function setURL($url)
    {
        $this->_url = $url;
    }

    /**
     * Gets an access token using the specified code. The parameters previously
     * supplied will be used when requesting an access token.
     * 
     * The token request is done using the authorization_code grant type.
     *
     * @param OAuthCode $code code to use when requesting access token
     * 
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     */
    public function getTokenUsingCode(OAuthCode $code)
    {
        $req = $this->_createRESTFulRequest();
        $req->addParam('code', '' . $code);
        $req->addParam('grant_type', 'authorization_code');
        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

    /**
     * Gets an access token using the specified scope. The parameters 
     * previously supplied will be used when requesting an access token.
     * 
     * The token request is done using the client_credentials grant type.
     *
     * @param string $scope scope to use when requesting access token
     *
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     */
    public function getTokenUsingScope($scope) 
    {
        $req = $this->_createRESTFulRequest();
        $req->addParam('scope', $scope);
        $req->addParam('grant_type', 'client_credentials');

        $result = $req->sendRequest();

        return $this->parseResult($result);
    }

    /**
     * Gets a new OAuth token using the refresh token of the specified OAuth 
     * token. 
     * 
     * The token request is done using the refresh_token grant type.
     *
     * @param OAuthToken $token OAuth token to use for refreshing 
     *
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     */
    public function refreshToken(OAuthToken $token) 
    {
        $req = $this->_createRESTFulRequest();
        $req->addParam('refresh_token', $token->getRefreshToken());
        $req->addParam('grant_type', 'refresh_token');
        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

}

?>
