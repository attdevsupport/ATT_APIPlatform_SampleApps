<?php
namespace Att\Api\Srvc;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Service Library
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
 * @category  API
 * @package   Service
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
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
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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
?>
