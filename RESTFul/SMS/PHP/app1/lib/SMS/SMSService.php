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
 * @category  API
 * @package   SMS 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 3 of the SMS API.
 *
 * @category API
 * @package  SMS
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
class SMSService extends APIService
{
    /**
     * Creates a SMSService object that can be used to interact with
     * the SMS API.
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
     * Sends a request to the API for sending a SMS to the specified address.
     *
     * @param string  $addr                 address to which SMS should be sent. 
     * @param string  $msg                  SMS message body to send.
     * @param boolean $notifyDeliveryStatus whether the API should sent a
     *                                      notification after delivery.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function sendSMS($addr, $msg, $notifyDeliveryStatus = true) 
    {
        $vals = array(
            'address' => $addr, 
            'message' => $msg, 
            'notifyDeliveryStatus' => $notifyDeliveryStatus
        );
        $jsobj = array('outboundSMSRequest' => $vals);
        $jvals = json_encode($jsobj);

        $endpoint = $this->FQDN . '/sms/v3/messaging/outbox';
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
     * Sends a request to the API for getting SMS Delivery status. 
     *
     * @param string $smsId SMS id for which to get delivery status.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function getSMSDeliveryStatus($smsId)
    {

        $encodedId = urlencode($smsId);
        $endpoint = $this->FQDN . '/sms/v3/messaging/outbox/' . $encodedId;  
        $req = new RESTFulRequest($endpoint);

        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addAuthorizationHeader($this->token);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/x-www-form-urlencoded');

        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

    /**
     * Sends a request t othe API for getting any SMS messages that were sent 
     * to the short code. 
     *
     * @param int $shortCode short code for which to get messages
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function getMessages($shortCode) 
    {
        $endpoint = $this->FQDN . '/rest/sms/2/messaging/inbox?' 
            . '&RegistrationID=' . urlencode($shortCode);  

        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addAuthorizationHeader($this->token);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/x-www-form-urlencoded');
        
        $result = $req->sendRequest();
        return $this->parseResult($result);
    } 
}
?>
