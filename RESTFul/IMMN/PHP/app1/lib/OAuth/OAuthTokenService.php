<?php
namespace Att\Api\OAuth;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * OAuth Library
 *
 * PHP version 5.4+
 *
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2014.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 *
 * @category  Authentication
 * @package   OAuth
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '/OAuthException.php';
require_once __DIR__ . '../../Srvc/Service.php';

use Att\Api\Srvc\Service;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Restful\HttpPost;

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting access
 * tokens.
 *
 * This implementation of the OAuth 2.0 Framework provides two models for
 * obtaining an access token, which can then be used for requesting access to
 * protected resources.
 *
 * These models are:
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
 * @author   pk9069
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
     * Parses the result received from sending an API request for an OAuth
     * token.
     *
     * @param array $result the result returned from a restful request
     *
     * @return OAuthToken oauth token if request was successful
     * @throws OAuthException if request was not successful
     * @see Service::parseResult()
     */
    protected function parseResult($result)
    {
        $tokenResponse = Service::parseJson($result);
    
        if (!isset($tokenResponse['access_token']))
            throw new OAuthException('Parse', 'No access token in response.');

        if (!isset($tokenResponse['expires_in']))
            throw new OAuthException('Parse', 'No expires_in in response.');

        if (!isset($tokenResponse['refresh_token']))
            throw new OAuthException('Parse', 'No refresh_token in response.');

        return new OAuthToken(
            $tokenResponse['access_token'],
            $tokenResponse['expires_in'],
            $tokenResponse['refresh_token']
        );
    }

    /**
     * Creates an OAuthTokenService object with the specified FQDN, client id,
     * and client secret.
     *
     * These values will then be used when requesting an access token. The
     * request will be sent to <var>FQDN + OAuthTokenService::URL_PATH</var>
     * unless overriden using {@link #setURL()}
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
        $httpPost = new HttpPost();

        $httpPost
            ->setParam('client_id', $this->_clientId)
            ->setParam('client_secret', $this->_clientSecret)
            ->setParam('code', '' . $code)
            ->setParam('grant_type', 'authorization_code');

        $req = new RestfulRequest($this->_url);

        $result = $req->sendHttpPost($httpPost);
        return $this->parseResult($result);
    }

    /**
     * Alias for getToken().
     *
     * @param string $scope scope to use when requesting access token
     *
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     * @see getToken()
     */
    public function getTokenUsingScope($scope)
    {
        return $this->getToken($scope);
    }

    /**
     * Gets an access token using the specified scope.
     *
     * The token request is done using the <i>client_credentials</i> grant type.
     *
     * @param string $scope scope to use when requesting access token
     *
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     */
    public function getToken($scope)
    {
        $httpPost = new HttpPost();

        $httpPost
            ->setParam('scope', $scope)
            ->setParam('grant_type', 'client_credentials')
            ->setParam('client_id', $this->_clientId)
            ->setParam('client_secret', $this->_clientSecret);

        $req = new RestfulRequest($this->_url);

        $result = $req->sendHttpPost($httpPost);
        return $this->parseResult($result);
    }

    /**
     * Gets a new OAuth token using the refresh token of the specified OAuth
     * token.
     *
     * The token request is done using the <i>refresh_token</i> grant type.
     *
     * @param OAuthToken $token OAuth token to use for refreshing
     *
     * @return OAuthToken an OAuth token
     * @throws OAuthException if server did not return valid access token
     */
    public function refreshToken(OAuthToken $token)
    {
        $httpPost = new HttpPost();

        $httpPost
            ->setParam('refresh_token', $token->getRefreshToken())
            ->setParam('grant_type', 'refresh_token')
            ->setParam('client_id', $this->_clientId)
            ->setParam('client_secret', $this->_clientSecret);

        $req = new RestfulRequest($this->_url);

        $result = $req->sendHttpPost($httpPost);
        return $this->parseResult($result);
    }
}
?>
