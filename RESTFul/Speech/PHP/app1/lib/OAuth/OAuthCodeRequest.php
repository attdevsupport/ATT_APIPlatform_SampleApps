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

require_once __DIR__ . '/OAuthCode.php';
require_once __DIR__ . '/OAuthException.php';

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting OAuth codes,
 * which can then be used to request OAuth tokens.
 *
 * Redirection works as follows: 
 * The browser is redirected to the consent flow screen using the specified 
 * URL, client id, and scope. After the consent flow is is either successful or
 * unsuccessful, the browser is then redirected to the specified redirect URI.
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @link     http://developer.att.com
 * @link     https://tools.ietf.org/html/rfc6749
 */
class OAuthCodeRequest
{
    /**
     * URL used for determining redirect URL. 
     * 
     * @var string
     */
    private $_URL;

    /**
     * Client id.
     *
     * @var string
     */
    private $_clientId;

    /**
     * Scope.
     * 
     * @var string
     */
    private $_scope;

    /**
     * URI the the consent flow will redirect to after attempting to obtain
     * an authorization code.
     * 
     * @var string
     */
    private $_redirectURI;

    /**
     * Creates an OAuthCodeRequest object for requesting authorization codes. 
     * These codes may then be used to request access tokens.
     *
     * @param string $URL         URL from which to request code
     * @param string $clientId    client id to use when requesting code
     * @param string $scope       scope to use
     * @param string $redirectURI URI sent to API that will be used for 
     *                            redirecting browser after consent flow. 
     */
    public function __construct(
        $URL, $clientId, $scope = null, $redirectURI = null
    ) {

        $this->_URL = $URL;
        $this->_clientId = $clientId;
        $this->_scope = $scope;
        $this->_redirectURI = $redirectURI;
    }

    /**
     * Returns the location from which to redirect the browser in order to get
     * an authorization code.
     *
     * @return string redirect location
     */
    public function getCodeLocation()
    {
        $location = $this->_URL . 
            "?client_id=" . urlencode($this->_clientId) .
            "&scope=" . urlencode($this->_scope) .
            "&redirect_uri=" . urlencode($this->_redirectURI);
        return $location;
    }

    /**
     * Used to get an authorization code. First checks request parameters for
     * code, and if not found, redirects browser to get authorization code.
     *
     * @return OAuthCode authorization code if in request parameters
     * @throws OAuthException if there was an error getting code
     */
    public function getCode()
    {
        // check for code in request params
        if (isset($_REQUEST['code'])) { 
            return new OAuthCode($_REQUEST['code']);
        }

        // check for error/error description in request params
        if (isset($_REQUEST['error'])) {
            $error = $_REQUEST['error'];
            $errorDesc = $_REQUEST['error_description'];
            throw new OAuthException($error, $errorDesc);
        }

        // no code and no error/error description
        // therefore, redirect to get code
        $location = $this->getCodeLocation();

        header("Location: $location");
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
