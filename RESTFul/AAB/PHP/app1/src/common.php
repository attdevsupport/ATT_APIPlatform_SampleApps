<?php
/*
 * Copyright 2015 AT&T
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

/*
 * This file holds common functions used by the sample applications.
 */

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Restful/RestfulEnvironment.php';
require_once __DIR__ . '/../lib/OAuth/OAuthCode.php';
require_once __DIR__ . '/../lib/OAuth/OAuthCodeRequest.php';
require_once __DIR__ . '/../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../lib/OAuth/OAuthTokenService.php';

use Att\Api\Restful\RestfulEnvironment;
use Att\Api\OAuth\OAuthCode;
use Att\Api\OAuth\OAuthCodeRequest;
use Att\Api\OAuth\OAuthException;
use Att\Api\OAuth\OAuthToken;
use Att\Api\OAuth\OAuthTokenService;

/**
 * Initializes any environment settings.
 *
 * @return void
 */
function envinit()
{
    if (defined('PROXY_HOST') && defined('PROXY_PORT')) {
        // set any RESTFul environmental settings
        RestfulEnvironment::setProxy($proxy_host, $proxy_port);
    }
    if (defined('ACCEPT_ALL_CERTS')) {
        RestfulEnvironment::setAcceptAllCerts($acceptAllCerts);
    }
}

/** 
 * Gets an access token that will be cached using a file. 
 *
 * This method works first trying to load the file specified in config, 
 * and, if a saved OAuth token isn't found, this method will send an API 
 * request. The OAuth token will then be saved for future use.
 *
 * @return OAuthToken OAuth token that can be used for authorization
 * @throws OAuthException if API request was not successful or if 
 *                        there was a file IO issue
 */
function getFileToken() 
{
    $token = OAuthToken::loadToken(OAUTH_FILE);
    if ($token == null || $token->isAccessTokenExpired()) {
        $tokenSrvc = new OAuthTokenService(FQDN, CLIENT_ID, CLIENT_SECRET);
        $token = $tokenSrvc->getTokenUsingScope(SCOPE);
        // save token for future use
        $token->saveToken(OAUTH_FILE);
    }

    return $token;
}

/**
 * Gets the fully qualified domain name to use for sending API requests.
 *
 * @return string fully qualified domain name
 */
function getFqdn() {
    return FQDN;
}

/** 
 * Gets the URL to redirect an application to for authorization.
 *
 * This function uses the parameters specified in the configuration file.
 *
 * @return string URL to redirect for authorization
 */
function getCodeLocation() 
{
    $codeUrl = getFqdn() . '/oauth/v4/authorize';
    $codeRequest = new OAuthCodeRequest(
        $codeUrl, CLIENT_ID, SCOPE, AUTHORIZE_REDIRECT_URI
    );
    return $codeRequest->getCodeLocation();
}


/** 
 * Gets an access token that will be cached using the user's session. 
 *
 * This method works by first trying to load the token from the user's 
 * session, and, if a saved OAuth token isn't found, this method will send 
 * an API request.
 *
 * @return OAuthToken OAuth token that can be used for authorization
 * @throws OAuthException if API request was not successful or if 
 *                        there was a session issue  
 */
function getSessionToken() 
{
    // Try loading token from session 
    $token = isset($_SESSION['token']) ?
        unserialize($_SESSION['token']) : null;

    // No token or token is expired... send token request
    if (!$token || $token->isAccessTokenExpired()) {
        // check for error/error description in request params
        if (isset($_REQUEST['error'])) {
            $error = $_REQUEST['error'];
            $errorDesc = '';
            if (isset($_REQUEST['error_description'])) {
                $errorDesc = $_REQUEST['error_description'];
            }
            throw new OAuthException($error, $errorDesc);
        }

        $code = null;
        // check for code in request params
        if (isset($_REQUEST['code'])) { 
            $code = new OAuthCode($_REQUEST['code']);
        } else {
            $error = 'Invalid state';
            $errrorDesc = 'No code found in parameter';
            throw new OAuthException($error, $errrorDesc);
        }

        $fqdn = getFqdn();
        $tokenSrvc = new OAuthTokenService($fqdn, CLIENT_ID, CLIENT_SECRET);
        $token = $tokenSrvc->getTokenUsingCode($code);
        $_SESSION['token'] = serialize($token);
    }

    return $token;
}

/** 
 * Gets whether the user is authenticated.
 *
 * This function is only applicable to applications that use authorization code
 * for authentication.
 *
 * @return boolean true if authenticated, false otherwise
 */
function isSessionAuthenticated() 
{
    $token = isset($_SESSION['token']) ? unserialize($_SESSION['token']) : null;
    return $token != null && !$token->isAccessTokenExpired();
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
