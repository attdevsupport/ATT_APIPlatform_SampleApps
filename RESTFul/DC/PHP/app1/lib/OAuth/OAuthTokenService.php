<?php
namespace Att\Api\OAuth;

/*
 * Copyright 2014 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @link     http://developer.att.com
 * @link     https://tools.ietf.org/html/rfc6749
 */
class OAuthTokenService extends Service
{
    const URL_PATH = '/oauth/v4/token';
    const REVOKE_PATH = '/oauth/v4/revoke';

    /**
     * URL to which request for an OAuth token will be sent.
     *
     * @var string
     */
    private $_url;

    /**
     * Revoke token URL.
     *
     * @var string
     */
    private $_revoke_url;


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
     * @param string $FQDN         fully qualified domain name
     * @param string $clientId     client id
     * @param string $clientSecret client secret
     *
     * @return void
     */
    public function __construct($FQDN, $clientId, $clientSecret)
    {
        $this->_url = $FQDN . OAuthTokenService::URL_PATH;
        $this->_revoke_url = $FQDN . OAuthTokenService::REVOKE_PATH;
        $this->_clientId = $clientId;
        $this->_clientSecret = $clientSecret;
    }

    /**
     * Gets an access token using the specified code. The parameters previously
     * supplied will be used when requesting an access token.
     *
     * The token request is done using the <i>authorization_code</i> grant
     * type.
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

    /**
     * Revokes the specified token.
     *
     * @param string $token token to revoke
     * @param string $hint hint for token type
     *
     * @throws OAuthException if API gateway returned an error
     */
    public function revokeToken($token, $hint='access_token')
    {
        $httpPost = new HttpPost();

        $httpPost
            ->setParam('client_id', $this->_clientId)
            ->setParam('client_secret', $this->_clientSecret)
            ->setParam('token', $token)
            ->setParam('token_type_hint', $hint);

        $req = new RestfulRequest($this->_revoke_url);
        $result = $req->sendHttpPost($httpPost);

        if ($result->getResponseCode() != 200) {
            throw new OAuthException('HTTP Code', $result->getResponseBody());
        }
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
