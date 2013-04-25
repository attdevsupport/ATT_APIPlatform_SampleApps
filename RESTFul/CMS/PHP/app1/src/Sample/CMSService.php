<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * CMS Sample Application 
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
 * @category CMS Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/CMS/CMSRequest.php';
require_once __DIR__ . '/../../lib/Util/Util.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class CMSService {
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_oauthFile;
    private $_scope;
    private $_number;

    private $_createSessionError;
    private $_sendSignalError;

    /**
     * Gets an access token. First attempts to load access token from file, and
     * if not successful, will send API request for access token.
     *
     * @return OAuthToken access token if successful.
     * @throws Exception if any underlying code throws exception
     */
    private function getToken() {
        // Try loading token from file
        $token = OAuthToken::loadToken($this->_oauthFile);

        // No token saved or token is expired... send token request
        if (!$token || $token->isAccessTokenExpired()) {
            $URL = $this->_FQDN . '/oauth/token';
            $id = $this->_clientId;
            $secret = $this->_clientSecret;
            $tokenRequest = new OAuthTokenRequest($URL, $id, $secret);
            $tokenRequest->setAcceptAllCerts(true);
            $token = $tokenRequest->getTokenUsingScope($this->_scope);

            // Save token for future use 
            $token->saveToken($this->_oauthFile);
        }

        return $token;
    }

    /**
     * Creates a CMSService object.
     */
    public function __construct() {
        require __DIR__ . '/../../config.php';

        $this->_FQDN = $FQDN;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_oauthFile = $oauth_file;
        $this->_scope = $scope;
        $this->_number = $number;

        $this->_createSessionError= NULL;
        $this->_sendSignalError= NULL;
    }

    /**
     * Attempts to create a session using ATT's API, if applicable. Will 
     * return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's getCreateSessionError() method 
     * can be used to get the error that occured.
     *
     * @return array response parameters if request was successful or NULL otherwise. 
     */
    public function createSession() {
        if (!isset($_REQUEST['btnCreateSession'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();

            $txtNumberToDial = $_REQUEST['txtNumberToDial'];
            $scriptType = $_REQUEST['scriptType'];
            $txtNumber = $_REQUEST['txtNumber'];
            $txtMessageToPlay = $_REQUEST['txtMessageToPlay'];

            /* save to session */
            $_SESSION['txtNumberToDial'] = $txtNumberToDial;
            $_SESSION['scriptType'] = $scriptType;
            $_SESSION['txtNumber'] = $txtNumber;
            $_SESSION['txtMessageToPlay'] = $txtMessageToPlay;

            $vals = array(
                    'smsCallerId' => $this->_number,
                    'feature' => $scriptType,
                    'numberToDial' => $txtNumberToDial,
                    'featurenumber' => $txtNumber,
                    'messageToPlay' => $txtMessageToPlay
                    );
                            
            
            $url = $this->_FQDN . '/rest/1/Sessions';
            $request = new CMSRequest($url, $token);
            $request->setAcceptAllCerts(true);
            $result = $request->createSession($vals);

            $sessionId = $result["id"];
            $_SESSION['sessionId'] = $sessionId;

            return $result;
        } catch (Exception $e) {
            $this->_createSessionError= $e->getMessage();
            return NULL;
        }
    }

    /**
     * Attempts to send a signal to a previously created session, if applicable. 
     * Will return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's getSendSignalError() method 
     * can be used to get the error that occured.
     *
     * @return array an array with the API response
     */
    public function sendSignal() {
        if (!isset($_REQUEST['btnSendSignal'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();
            $sessionId = $this->getSessionId();
            if ($sessionId == NULL || $sessionId == '') {
                $error = "Session ID must not be empty.";
                throw new InvalidArgumentException($error);
            }
            $signal = $_REQUEST['signal'];
            
            /* save to session */
            $_SESSION['signal'] = $signal;

            $url = 
                $this->_FQDN . '/rest/1/Sessions/' . $sessionId . '/Signals'; 

            $request = new CMSRequest($url, $token);
            $request->setAcceptAllCerts(true);
            $result = $request->sendSignal($signal);

            return $result;
        } catch (Exception $e) {
            $this->_sendSignalError = $e->getMessage();
            return NULL;
        }
    }


    /**
     * If there was an error during creating a session, gets the error. 
     *
     * @return string error if exists, NULL otherwise
     */
    public function getCreateSessionError() {
        return $this->_createSessionError;
    }

    /**
     * If there was an error during sending a signal, gets the error. 
     *
     * @return string error if exists, NULL otherwise
     */
    public function getSendSignalError() {
        return $this->_sendSignalError;
    }

    /**
     * Gets the contents of the script that is used for powering CMS.
     *
     * @return string script contents
     */
    public function getScriptContents() {
        $fname = __DIR__ . '/../../First.tphp';
        return file_get_contents($fname);
    }

    /**
     * Gets Session Id or an empty string if no session id was found.
     *
     * @return session id or empty string
     */
    public function getSessionId() {
        return isset($_SESSION['sessionId']) ? $_SESSION['sessionId'] : '';
    }
}
?>
