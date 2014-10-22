<?php
namespace Att\Api\DC;

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

require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/DCResponse.php';

use Att\Api\Srvc\Service;
use Att\Api\Srvc\APIService;
use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\RestfulRequest;

/**
 * Used to interact with version 2 of the Device Capabilities API.
 *
 * @category API
 * @package  DC
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/2/Device%20Capabilities
 */
class DCService extends APIService
{
    /**
     * Creates a DCService object that can be used to interact with
     * the DC API.
     *
     * @param string     $FQDN  fully qualified domain name to which requests 
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization 
     */
    public function __construct($FQDN, OAuthToken $token) 
    {
        parent::__construct($FQDN, $token);
    }

    /**
     * Sends a request to the API for getting device capabilities. 
     *
     * @return DCResponse API response. 
     * @throws ServiceException if API request was not successful
     */
    public function getDeviceInformation() 
    {
        $endpoint = $this->getFqdn() . '/rest/2/Devices/Info';

        $req = new RESTFulRequest($endpoint);

        $result = $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->sendHttpGet();

        $arr = Service::parseJson($result);
        return DCResponse::fromArray($arr);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
