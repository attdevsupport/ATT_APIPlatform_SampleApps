<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * TL API Library
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
 * @category TLAPI 
 * @package TL 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */
require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';

/**
 * Used for sending Terminal Location (TL) requests. Such requests allow 
 * determining the location of a device.
 *
 * @package TL 
 */
class TLRequest extends RESTFulRequest {
    private $_token;

    /**
     * Creates a TLRequest object with the specified url and access token.
     *
     * @param $URL string URL to send requests to
     * @param $token OAuthToken access token to use for authorization
     */
    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token; 
    }

    /**
     * Gets device location using the specified requested accuracy, acceptable 
     * accuracy, and tolerance.
     *
     * @param $requestedAccuracy string requested accuracy
     * @param $acceptableAccuracy string acceptable accuracy
     * @param $tolerance string tolerance
     * @return array an array of values
     */
    public function getLocation($requestedAccuracy, $acceptableAccuracy, 
            $tolerance) {

        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addParam('requestedAccuracy', $requestedAccuracy);
        $this->addParam('acceptableAccuracy', $acceptableAccuracy);
        $this->addParam('tolerance', $tolerance);
        $this->addAuthorizationHeader($this->_token);

        $tNow= time();

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($response);
        }
        $responseArr = json_decode($response, true);
        $responseArr['elapsedTime'] = time() - $tNow;
        return $responseArr;
    }
}
?>
