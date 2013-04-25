<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * IMMN Sample Application 
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
 * @category IMMN Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCodeRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCode.php';
require_once __DIR__ . '/../../lib/IMMN/IMMNRequest.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class IMMNService {

    // values to load from config
    private $_FQDN;
    private $_endpoint;
    private $_clientId;
    private $_clientSecret;
    private $_scope;
    private $_redirect;
    private $_attachmentsFolder;
    private $_errorGet;
    private $_errorSend;

    /**
     * Given an address string, this method converts that string to an array
     * of 'acceptable' strings that can be used by ATT's API.
     *
     * @param $addrStr string address string
     * @return array an array of address strings
     * @throws InvalidArgumentException if address string contains invalid 
     * addresses.
     */
    private function convertAddresses($addrStr) {
        $addresses = explode(',', $addrStr);
        $encodedAddr = array(); 
        foreach ($addresses as $addr) {
            $cleanAddr = str_replace('-', '', $addr);
            $cleanAddr = str_replace('tel:', '', $cleanAddr);
            $cleanAddr = str_replace('+1', '', $cleanAddr);
            if (preg_match("/\d{10}/",$cleanAddr)) {
                $encodedAddr[] = 'tel:' . $cleanAddr;
            } else if (preg_match("/^[^@]*@[^@]*\.[^@]*$/", $cleanAddr)) {
                $encodedAddr[] = $cleanAddr;
            } else if (preg_match("/\d[3-8]/", $cleanAddr)){
                $encodedAddr[] = 'short:' . $cleanAddr;
            } else {
                throw new InvalidArgumentException('Invalid address: ' . $addr);
            }
        }

        return $encodedAddr;
    }

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
        $this->_endpoint = $endpoint;
        $this->_scope = $scope;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_redirect = $authorize_redirect_uri;
        $this->_attachmentsFolder = $attachments_folder;

        $this->_errorGet = NULL;
        $this->_errorSend = NULL;
    }


    /**
     * Attempts to send message using ATT's API, if applicable. Will 
     * return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's errorSend() method can be used to 
     * get the error that occured.
     *
     * @return string message id if request was successful or NULL otherwise. 
     */
    public function sendMessage() {
        $vnames = array('sendMessage', 'Address', 'message', 'subject', 
                'attachment', 'groupCheckBox');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['sendMessage'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();
            $addr = $this->convertAddresses($_SESSION['Address']);
            $msg = $_SESSION['message'];
            $subject = $_SESSION['subject'];
            $attachment = $_SESSION['attachment'];
            if (strcmp($attachment, 'None') == 0) {
                $attachment = NULL;
            } else {
                $attachment = 
                    array($this->_attachmentsFolder . '/' .  $attachment);
            }

            $checkbox = isset($_REQUEST['groupCheckBox']);
            $_SESSION['checkbox'] = $checkbox;

            $immnRequest = new IMMNRequest($this->_endpoint, $token);
            $immnRequest->setAcceptAllCerts(true);
            $id = $immnRequest->sendMessage($addr, $msg, $subject, 
                    $attachment);
            $this->clearSession(array('sendMessage'));
            return $id;
        } catch (Exception $e) {
            $this->_errorSend = $e->getMessage();
            $this->clearSession(array('sendMessage'));
            return NULL;
        }
    }

    /**
     * Attempts to get message body from ATT's API, if applicable. Will 
     * return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's errorGet() method can be used to get
     * the error that occured.
     *
     * @return mixed IMMNBody object if request was successful or NULL 
     * otherwise. 
     */
    public function getMessageBody() {
        $vnames = array ('getMessageContent', 'MessageId', 'PartNumber');
        $this->copyToSession($vnames);

        if (!isset($_SESSION['getMessageContent'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();

            $msgId = $_SESSION['MessageId'];
            $partNum = $_SESSION['PartNumber'];
            $url = $this->_endpoint . '/' . $msgId . '/' . $partNum; 

            $immnRequest = new IMMNRequest($url, $token);
            $immnRequest->setAcceptAllCerts(true);
            $this->clearSession(array('getMessageContent'));
            return $immnRequest->getMessageBody();
        } catch (Exception $e) {
            $this->_errorGet = $e->getMessage();
            $this->clearSession(array('getMessageContent'));
            return NULL;
        }
    }

    /**
     * Attempts to get message headers from ATT's API, if applicable. Will 
     * return NULL if no POST request was made or if there was an error.
     * If there was an error, this class's errorGet() method can be used to get
     * the error that occured.
     *
     * @return mixed IMMNResponse object if request for getting messages 
     * headers, or NULL otherwise.
     */
    public function getMessageHeaders() {
        $vnames = array('getMessageHeaders', 'headerCountTextBox',
                'indexCursorTextBox');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessageHeaders'])) {
            return NULL;
        }

        try {
            $headerCount = intval($_SESSION['headerCountTextBox']);
            $indexCursor = $_SESSION['indexCursorTextBox'];

            $token = $this->getToken();
            $immnRequest = new IMMNRequest($this->_endpoint, $token);
            $immnRequest->setAcceptAllCerts(true);
            $this->clearSession(array('getMessageHeaders'));

            return $immnRequest->getMessageHeaders($headerCount, $indexCursor);
        } catch (Exception $e) {
            $this->_errorGet = $e->getMessage();
            $this->clearSession(array('getMessageHeaders'));
            return NULL;
        }
    }

    /**
     * Returns a list of attachments.
     */
    public function getAttachments() {
        $allFiles = scandir($this->_attachmentsFolder);
        $attachFiles = array();

        // copy all files except directories
        foreach ($allFiles as $fname) {
            if (!is_dir($fname)) {
                $attachFiles[] = $fname;
            }
        }

        return $attachFiles;
    }

    /**
     * Gets an error if there was an error getting messages
     *
     * @return string error if exists, NULL otherwise
     */
    public function errorGet() {
        return $this->_errorGet;
    }

    /**
     * Gets an error if there was one during sending a message
     */
    public function errorSend() {
        return $this->_errorSend;
    }
}
?>
