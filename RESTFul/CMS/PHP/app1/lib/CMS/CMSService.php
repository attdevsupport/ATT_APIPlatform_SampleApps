<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * CMS Library
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
 * @package   CMS 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 1 of the CMS API.
 *
 * @category API
 * @package  CMS
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Call%20Management%20(Beta)
 */
class CMSService extends APIService
{

    /**
     * Creates a CMSRequest object with the specified URL and access token.
     * 
     * @param string     $FQDN  fully qualified domain name to send requests to
     * @param OAuthToken $token access token to use for authorization
     */
    public function __construct($FQDN, OAuthToken $token)
    {
        parent::__construct($FQDN, $token);
    }

    /**
     * Sends an API request for creating a CMS session. 
     *
     * @param array $vals an array of strings to be send in the request
     *
     * @return array API response as an array of key-value pairs
     * @throws ServiceException if API request was not successful
     */
    public function createSession($vals) 
    {
        $jvals = json_encode($vals);

        $endpoint = $this->FQDN . '/rest/1/Sessions';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/json');
        $req->addAuthorizationHeader($this->token);
        $req->setBody($jvals);
        $result = $req->sendRequest();
        return $this->parseResult($result);
    }
    
    /**
     * Sends an API request for sending a signal to a previously created
     * session.
     *
     * @param string $signal    signal to send to a CMS session
     * @param string $sessionId session to which signal will be sent 
     *
     * @return array API response as an array of key-value pairs
     * @throws ServiceException if API request was not successful
     */
    public function sendSignal($signal, $sessionId) 
    {
        $jvals = json_encode(array('signal' => $signal));
        $endpoint = $this->FQDN . '/rest/1/Sessions/' . $sessionId . '/Signals';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/json');
        $req->addAuthorizationHeader($this->token);
        $req->setBody($jvals);
        $result = $req->sendRequest();
        return $this->parseResult($result);
    }
}

?>
