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

// JSON extension is required for this class to work
if (!function_exists('json_decode')) {
    $err = 'Service class requires the JSON extension for PHP.';
    throw new Exception($err);
}

require_once __DIR__ . '/ServiceException.php';
require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../Restful/RestfulRequest.php';

/**
 * Base class used to hold common code for sending Restful requests.
 *
 * @category API
 * @package  Service
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     http://developer.att.com
 */
abstract class Service
{

    /**
     * Convenience method for parsing the result of an api request that
     * contains a json response body.
     *
     * @param RestfulResponse $result        result to parse
     * @param array           $successCodes  array of http status codes to
     *                                       treat as successful; an exception
     *                                       will be thrown if the http status
     *                                       code in the <var>$result<var>
     *                                       object does not match one of the
     *                                       success codes
     *
     * @see RestfulRequest::sendRequest
     * @return array api response as an array of key-value pairs.
     * @throws ServiceException if api request was not successful
     */
    public static function parseJson($result, $successCodes=array(200, 201))
    {
        $responseBody = $result->getResponseBody();
        $responseCode = $result->getResponseCode();

        foreach ($successCodes as $successCode) {
            if ($responseCode == $successCode) {
                return json_decode($responseBody, true);
            }
        }

        throw new ServiceException($responseCode, $responseBody);
    }

    /**
     * Empty constructor for now.
     */
    protected function __construct()
    {
        // empty
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
