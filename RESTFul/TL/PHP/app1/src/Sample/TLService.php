<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * TL Sample Application 
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
 * @category TL Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCodeRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCode.php';
require_once __DIR__ . '/../../lib/TL/TLRequest.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class TLService {

    // values to load from config
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_scope;
    private $_redirect;

    private $_error;

    /**
     * Given an array of names, this method will copy the named values from 
     * request parameters to session.
     *
     * @param $vnames array list of variable names
     */
    private function copyToSession($vnames) {
        foreach ($vnames as $vname) {
            if (isset($_REQUEST[$vname])) {
                $_SESSION[$vname] = $_REQUEST[$vname];
            }
        }
    }

    /**
     * Unsets the session for the specified names.
     *
     * @param $vnames array of variable names
     */
    private function clearSession($vnames) {
        foreach ($vnames as $vname) {
            unset($_SESSION[$vname]);
        }
    }

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
        if (!$token) {
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
     * Creates a IMMNService object.
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

    /**
     * Attempts to get terminal location from ATT's API, if applicable. Will 
     * return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's getError() method can be used to get
     * the error that occured.
     *
     * @return mixed array if request was successful or NULL otherwise 
     */
    public function getTerminalLocation() {

        $vnames = array('getLocation', 'acceptableAccuracy',
                'requestedAccuracy', 'tolerance');
        $this->copyToSession($vnames);

        /* default values */
        if(!isset($_SESSION['requestedAccuracy'])) {
            $_SESSION['requestedAccuracy'] = 1000;
        }
        if (!isset($_SESSION['acceptableAccuracy'])) {
            $_SESSION['acceptableAccuracy'] = 10000;
        }
        if (!isset($_SESSION['tolerance'])) {
            $_SESSION['tolerance'] = 'LowDelay';
        }

        if (!isset($_SESSION['getLocation'])) {
            return NULL;
        }

        try {
            $acceptableAccuracy = $_SESSION['acceptableAccuracy'];
            $requestedAccuracy = $_SESSION['requestedAccuracy'];
            $tolerance = $_SESSION['tolerance'];

            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/2/devices/location/';
            $tlRequest = new TLRequest($endpoint, $token);
            $tlRequest->setAcceptAllCerts(true);

            $this->clearSession(Array('getLocation'));
            return $tlRequest->getLocation($requestedAccuracy, 
                    $acceptableAccuracy, $tolerance);
        } catch (Exception $e) {
            $this->_error = $e->getMessage();
            $this->clearSession(Array('getLocation'));
            return NULL;
        }
    }


    /**
     * Gets an error if there was one during a TL request, otherwise returns
     * NULL.
     *
     * @return string error if there was one, NULL otherwise
     */
    public function getError() {
        return $this->_error;
    }
}
?>
