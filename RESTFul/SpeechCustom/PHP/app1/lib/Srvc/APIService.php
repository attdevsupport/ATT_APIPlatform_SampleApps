<?php
namespace Att\Api\Srvc;

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

require_once __DIR__ . '/Service.php';

use Att\Api\OAuth\OAuthToken;

/**
 * Base class used to hold common code for sending API requests.
 *
 * @category API
 * @package  Service
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     http://developer.att.com
 * @see      Service
 */
abstract class APIService extends Service
{

    /**
     * OAuth token used for authorization.
     *
     * @var OAuthToken
     */
    private $_token;

    /**
     * Fully qualified domain name to which requests will be sent.
     *
     * @var string
     */
    private $_fqdn;

    /**
     * Creates an APIService object that an be used to interact with
     * APIs.
     *
     * @param string     $fqdn  fully qualified domain name to send requests to
     * @param OAuthToken $token OAuth token used for authorization 
     */
    protected function __construct($fqdn, OAuthToken $token)
    {
        $this->_token = $token; 
        $this->_fqdn = $fqdn;
    }

    /**
     * Gets the fully qualified domain name.
     *
     * @return string fully qualified domain name
     */
    protected function getFqdn()
    {
        return $this->_fqdn;
    }

    /**
     * Gets OAuthToken.
     *
     * @return OAuthToken token
     */
    protected function getToken()
    {
        return $this->_token;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
