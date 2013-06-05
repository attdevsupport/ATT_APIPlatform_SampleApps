<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * MMS Library
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
 * @package   MMS 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '../../Restful/Multipart.php';


/**
 * Used to interact with version 3 of the MMS API.
 *
 * @category API
 * @package  MMS
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/MMS
 */
class MMSService extends APIService
{

    /**
     * Creates a MMSService object that can be used to interact with
     * the MMS API.
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
     * Sends a request to the API for sending MMS to the specified address.
     *
     * @param string      $addr                 address to which MMS will be 
     *                                          sent.
     * @param string|null $subject              subject of MMS.
     * @param string|null $priority             priority of MMS
     * @param boolean     $notifyDeliveryStatus whether the API should send a
     *                                          notification after MMS has been 
     *                                          sent.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function sendMMS($addr, $fnames, $subject = null, $priority = null, 
        $notifyDeliveryStatus = false
    ) {
        $endpoint = $this->FQDN . '/mms/v3/messaging/outbox';
        $req = new RESTFULRequest($endpoint);
        $req->setHttpMethod(RESTFULRequest::HTTP_METHOD_POST);
        $req->setHeader('Content-Type', 'application/json');
        $req->setHeader('Accept', 'application/json');
        $req->addAuthorizationHeader($this->token);

        $outboundRequest = array(
                'address' => $addr,
                'notifyDeliveryStatus' => $notifyDeliveryStatus
                );

        if ($subject != null) {
            $outboundRequest['subject'] = $subject;
        }
        if ($priority != null) {
            $outboundRequest['priority'] = $priority;
        }

        $vals = array('outboundMessageRequest' => $outboundRequest);
        $jvals = json_encode($vals);

        $mpart = new MultipartBody();
        $mpart->addJSONPart($jvals);
        foreach ($fnames as $fname) {
            $mpart->addFilePart($fname);
        }
        $req->setMultipart($mpart);

        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

    /**
     * Sends a request to the API for getting delivery status of an MMS.
     *
     * @param string $mmsId MMS Id for which to get status.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function getMMSStatus($mmsId) 
    {
        $endpoint = $this->FQDN . '/mms/v3/messaging/outbox/' . $mmsId;              
        $req = new RESTFULRequest($endpoint);
        $req->setHttpMethod(RESTFULRequest::HTTP_METHOD_GET);
        $req->setHeader('Accept', 'application/json');
        $req->addAuthorizationHeader($this->token);

        $result = $req->sendRequest();
        return $this->parseResult($result);
    }
}

?>
