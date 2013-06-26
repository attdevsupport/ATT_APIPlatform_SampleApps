<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Service Library
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
 * @category  API
 * @package   Service
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
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
require_once __DIR__ . '../../Restful/RESTFulRequest.php';

/**
 * Base class used to hold common code for sending Restful requests.
 *
 * @category API
 * @package  Service
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
abstract class Service
{

    /**
     * Convenience method for parsing the result of an api request.
     *
     * @param RESTFulResponse $result response returned by sendRequest() method
     *                                of a RESTFulRequest object. 
     *
     * @see RESTFulRequest::sendRequest
     * @return array api response as an array of key-value pairs.
     * @throws ServiceException if api request was not successful
     */ 
    protected function parseResult($result)
    {
        $responseBody = $result->getResponseBody();
        $responseCode = $result->getResponseCode();

        // api request was not successful
        if ($responseCode != 200 && $responseCode != 201) {
            throw new ServiceException($responseCode, $responseBody);
        }

        // TODO: move away from JSON hardcoding (maybe do dynamic check)
        $responseArr = json_decode($responseBody, true);
        return $responseArr;
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
