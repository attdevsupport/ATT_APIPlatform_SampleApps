<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2012. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2012 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category Authentication 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

/**
 * Class containing common information for sending OAuth access requests.
 * 
 * @package OAuth 
 */
abstract class OAuthRequest {
    // Encoding to be used when sending request 
    private $_encoding;

    // Content type to use when sending request
    private $_contentType;

    // The HTTP Method to use when sending the request. E.g. GET or POST
    private $httpMethod;
}
