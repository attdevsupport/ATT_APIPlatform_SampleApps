<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * DC Library
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
 * @package   DC 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 2 of the Device Capabilities API.
 *
 * @category API
 * @package  DC
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
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
     * @return array API response as an associative array. 
     * @throws ServiceException if API request was not successful
     */
    public function getDeviceInformation() 
    {
        $endpoint = $this->FQDN . '/rest/2/Devices/Info';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addAuthorizationHeader($this->token);
        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

}
?>
