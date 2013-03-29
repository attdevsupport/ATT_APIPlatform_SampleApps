<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * SMS Library
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
 * @category SMS 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../Common/RESTFulRequest.php';

/**
 * Used for sending SMS requests.
 * 
 * @package SMS 
 */
class SMSRequest extends RESTFulRequest { 
    // access token to use when sending requests
    private $_token;

    /**
    * Creates an SMSRequest object with the following url and following
    * access token.
    * 
    * @param $url URL to send requests to
    * @param $token OAuthToken token to use for authorization
    */
    public function __construct($url, OAuthToken $token) {
        parent::__construct($url);
        $this->_token = $token;
    }


    /** 
     * Sends an SMS to the following address.
     *
     * @param $addr string address to send SMS to
     * @param $msg message to send
     */
    public function sendSMS($addr, $msg, $notifyDeliveryStatus = true) {
        $vals = array();
        $vals['address'] = $addr;
        $vals['message'] = $msg;
        $vals['notifyDeliveryStatus'] = $notifyDeliveryStatus;
        $jsobj = array('outboundSMSRequest' => $vals);
        $jvals = json_encode($jsobj);

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
    
        $jvals = json_decode($response);
        
        return $jvals;
    }

    /**
     * Gets SMS Delivery status. To use this method, create an SMSRequest object
     * with the URL pointing to the endpoint/{messageId}.
     *
     * @return array an array of values returned by endpoint
     */
    public function getSMSDeliveryStatus() {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addAuthorizationHeader($this->_token);
        $this->setHeader('Content-Type', 'application/x-www-form-urlencoded');

        $responseVals = $this->sendRequest();

        $response = $responseVals['response'];
        $responseCode = $responseVals['responseCode'];

        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }

        $jvals = json_decode($response);
        return $jvals;
    }

    /**
     * Gets any SMS messages received to short code. To specified shortcode,
     * create an SMSRequest object with the URL specified to 'Registration'.
     */
    public function getMessages() {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addAuthorizationHeader($this->_token);
        $this->setHeader('Content-Type', 'application/x-www-form-urlencoded');
        //$this->setHeader('Content-Type', 'application/json');

        $responseVals = $this->sendRequest();

        $response = $responseVals['response'];
        $responseCode = $responseVals['responseCode'];

        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }

        $jvals = json_decode($response);
        return $jvals;
    } 
}
?>
