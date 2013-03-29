<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * DC API Library
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
 * @category DCAPI 
 * @package dc 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */
require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';

/**
 * Used for sending DC requests. Such requests include getting and sending
 * messages.
 *
 * @package dc 
 */
class DCRequest extends RESTFulRequest {
    private $_headers;
    private $_token;

    /**
     * Creates an DC object with the specified url and access token.
     *
     * @param $URL string URL to send requests to
     * @param $token OAuthToken access token to use for authorization
     */
    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token; 
    }

    public function getDeviceInformation() {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addAuthorizationHeader($this->_token);
        $result = $this->sendRequest();

        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return json_decode($response, true);
    }
}
?>
