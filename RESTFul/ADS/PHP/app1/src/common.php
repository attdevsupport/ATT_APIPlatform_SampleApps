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

/**
 * This file holds common functions used by the sample applications.
 */

require_once __DIR__ . '/../lib/Restful/RestfulEnvironment.php';
require_once __DIR__ . '/../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../lib/OAuth/OAuthTokenService.php';

use Att\Api\Restful\RestfulEnvironment;
use Att\Api\OAuth\OAuthToken;
use Att\Api\OAuth\OAuthTokenService;

/**
 * Initializes any environment settings.
 *
 * @return void
 */
function envinit()
{
    require __DIR__ . '/../config.php';

    // maintain backwards compability with older configs
    if (isset($proxy_host)) {
        $proxyHost = $proxy_host;
    }
    if (isset($proxy_port)) {
        $proxyPort = $proxy_port;
    }
    if (isset($accept_all_certs)) {
        $acceptAllCerts = $accept_all_certs;
    }

    // set any RESTFul environmental settings
    if (isset($proxyHost) && isset($proxyPort))
        RestfulEnvironment::setProxy($proxy_host, $proxy_port);

    if (isset($acceptAllCerts))
        RestfulEnvironment::setAcceptAllCerts($acceptAllCerts);
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
    require __DIR__ . '/../config.php';

    // maintain backwards compability with older configs
    if (isset($api_key)) {
        $clientId = $api_key;
    }
    if (isset($secret_key)) {
        $clientSecret = $secret_key;
    }
    if (isset($oauth_file)) {
        $oauthFile = $oauth_file;
    }

    $token = OAuthToken::loadToken($oauthFile);
    if ($token == null || $token->isAccessTokenExpired()) {
        $tokenSrvc = new OAuthTokenService($FQDN, $clientId, $clientSecret);
        $token = $tokenSrvc->getTokenUsingScope($scope);
        // save token for future use
        $token->saveToken($oauthFile);
    }

    return $token;
}

/**
 * Gets the fully qualified domain name to use for sending API requests.
 *
 * @return string fully qualified domain name
 */
function getFqdn() {
    require __DIR__ . '/../config.php';

    return $FQDN;
}


/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
