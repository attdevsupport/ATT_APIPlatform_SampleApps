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
 * @category RESTFul 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../Common/RESTFulRequest.php';

/**
 * Used for sending CMS requests.
 *
 * @package CMS 
 */
class CMSRequest extends RESTFulRequest { 
    private $_token;

    /**
     * Creates a CMSRequest object with the specified URL and access token.
     * 
     * @param $url string URL to send request to
     * @param $token OAuthToken access token to use for authorization
     */
    public function __construct($url, OAuthToken $token) {
        parent::__construct($url);
        $this->_token = $token;
    }

    /**
     * Creates a CMS session using the specified values.
     *
     * @param vals array an array of strings to be send in the request
     */
    public function createSession($vals) {
        $jvals = json_encode($vals);

        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->setHeader('Accept', 'application/json');
        $this->setHeader('Content-Type', 'application/json');
        $this->addAuthorizationHeader($this->_token);
        $this->setBody($jvals);
        $responseVals = $this->sendRequest();

        $response = $responseVals['response'];
        $responseCode = $responseVals['responseCode'];

        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
    
        $jvals = json_decode($response, true);
        
        return $jvals;
    }
    
    /**
     * Sends the specified signal.
     */
    public function sendSignal($signal) {
        $jvals = json_encode(array('signal' => $signal));

        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->setHeader('Accept', 'application/json');
        $this->setHeader('Content-Type', 'application/json');
        $this->addAuthorizationHeader($this->_token);
        $this->setBody($jvals);
        $responseVals = $this->sendRequest();

        $response = $responseVals['response'];
        $responseCode = $responseVals['responseCode'];

        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
    
        $jvals = json_decode($response, true);
        
        return $jvals;
    }
}

?>
