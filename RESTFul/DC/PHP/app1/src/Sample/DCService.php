<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * DC Sample Application 
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
 * @category DC Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCodeRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCode.php';
require_once __DIR__ . '/../../lib/DC/DCRequest.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class DCService {

    // values to load from config
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_scope;
    private $_redirect;

    private $_error;

    /**
     * Gets an access token.  
     * 
     * @return OAuthToken access token if successful.
     * @throws Exception if any underlying code throws exception
     */
    private function getToken() {
        // Try loading token from session 
        $token = isset($_SESSION['token']) ?
            unserialize($_SESSION['token']) : NULL;

        // No token... send token request
        if ($token == NULL) {
            $codeURL = $this->_FQDN . '/oauth/authorize';
            $codeRequest = new OAuthCodeRequest($codeURL, $this->_clientId, 
                    $this->_scope, $this->_redirect);
            $code = $codeRequest->getCode();

            $tokenURL = $this->_FQDN . '/oauth/token';
            $tokenRequest = new OAuthTokenRequest($tokenURL, $this->_clientId, 
                    $this->_clientSecret);
            $tokenRequest->setAcceptAllCerts(true);
            $token = $tokenRequest->getTokenUsingCode($code);
            $_SESSION['token'] = serialize($token);
        }

        return $token;
    }

    /**
     * Creates a DC object.
     */
    public function __construct() {
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_FQDN = $FQDN;
        $this->_scope = $scope;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_redirect = $authorize_redirect_uri;

        $this->_error = NULL;
    }

    public function getDeviceInformation() {
        try {
            if (isset($_REQUEST['error'])) {
                throw new Exception('error=' . $_REQUEST['error'] . '&' 
                        . 'error_description=' . $_REQUEST['error_description']);
            }

            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/rest/2/Devices/Info/';
            $dcRequest = new DCRequest($endpoint, $token);
            $dcRequest->setAcceptAllCerts(true);

            return $dcRequest->getDeviceInformation();

        } catch (Exception $e) {
            $this->_error = $e->getMessage();
            return NULL;
        }
    }


    /**
     * Gets an error if there was one during a DC request, otherwise returns
     * NULL.
     *
     * @return string error if there was one, NULL otherwise
     */
    public function getError() {
        return $this->_error;
    }
}
?>
