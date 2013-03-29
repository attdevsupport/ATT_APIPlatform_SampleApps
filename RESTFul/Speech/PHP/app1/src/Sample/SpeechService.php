<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Speech Sample Application 
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
 * @category Speech Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/Speech/SpeechRequest.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class SpeechService {

    // values to load from config
    private $_FQDN;
    private $_oauthFile;
    private $_endpoint;
    private $_clientId;
    private $_clientSecret;
    private $_scope;
    private $_audioFolder;
    private $_speechContexts;
    private $_xSpeechSubContext;
    private $_error;

    /**
    * Gets an access token. First attempts to load access token from file, and
    * if not successful, will send API request for access token
    *
    * @return OAuthToken access token if successful.
    * @throws Exception if any underlying code throws exception
    */
    private function getToken() {
        // Try loading token from file
        $token = OAuthToken::loadToken($this->_oauthFile);

        // No token saved... send token request
        if (!$token) {
            $URL = $this->_FQDN . '/oauth/token';
            $id = $this->_clientId;
            $secret = $this->_clientSecret;
            $tokenRequest = new OAuthTokenRequest($URL, $id, $secret);
            $token = $tokenRequest->getToken($this->_scope);

            // Save token for future use 
            $token->saveToken($this->_oauthFile);
        }

        return $token;
    }

    /**
     * Creates a SpeechService object.
     */
    public function __construct() {
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_FQDN = $FQDN;
        $this->_oauthFile = $oauth_file;
        $this->_endpoint = $endpoint;
        $this->_scope = $scope;
        $this->_audioFolder = $audioFolder;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_speechContexts = $speech_context_config;
        $this->_xSpeechSubContext = $xSpeechSubContext;
        $this->_xArgs = $x_arg;
        $this->_error = NULL;
    }

    /**
    * Gets the list of audio files that are specified in config value 
    * 'audioFolder'.
    *
    * @return array list of files
    */
    public function getAudioFiles() {
        $allFiles = scandir($this->_audioFolder);
        $audioFiles = array();

        // copy all files except directories
        foreach ($allFiles as $fname) {
            if (!is_dir($fname)) {
                array_push($audioFiles, $fname);
            }
        }

        return $audioFiles;
    }

    /**
    * Gets whether the specified audio file was selected in a previous session.
    *
    * @return boolean true if previously selected, false otherwise
    */
    public function isAudioFileSelected($fname) {
    	return isset($_SESSION['audio_file']) 
                && strcmp($_SESSION['audio_file'], $fname) == 0;
    } 

    /**
    * Returns an array of speech contexts.
    *
    * @return array array of speech contexts
    */
    public function getSpeechContexts() {
        return $this->_speechContexts;
    }

    /**
    * Gets whether the specified context was selected in a previous session.
    *
    * @return boolean true if previously selected, false otherwise
    */
    public function isSpeechContextSelected($cname) {
        return isset($_SESSION['SpeechContext']) 
            && strcmp($_SESSION['SpeechContext'], $cname) == 0; 
    }

    /**
    * Gets whether chunked was selected in a previous session.
    */
    public function isChunkedSelected() {
        return isset($_SESSION['chunked']) && $_SESSION['chunked'];
    } 

    /**
    * Attempts to get a speech response from ATT's API, if applicable. Will 
    * return NULL if no POST request was made or NULL if there was an error.
    * If there was an error, this class's getError() method can be used to get
    * the error that occured.
    *
    * @return SpeechResponse SpeechResponse object if successful, NULL otherwise
    */
    public function speechToText() {
        if (!isset($_REQUEST['SpeechToText'])) {
            return NULL;
        }
        try {
            $token = $this->getToken();

            $context = $_REQUEST['SpeechContext'];
            $_SESSION['SpeechContext'] = $context;

            $filename = $_REQUEST['audio_file'];
            $_SESSION['audio_file'] = $filename;

            $chunked = isset($_REQUEST['chkChunked']) ? 
                $_REQUEST['chkChunked'] : false;
            $_SESSION['chunked'] = $chunked;

            $endpoint = $this->_endpoint;
            $flocation = $this->_audioFolder . '/' . $filename; 
            $speechRequest = new SpeechRequest($token, $endpoint, $flocation);
            $speechRequest->setSpeechContext($context);
            $speechRequest->setChunked($chunked);
            $speechRequest->setXArgs($this->_xArgs);
            if (strcmp($context, 'Gaming') == 0) {
                $speechRequest->setXSpeechSubContext($this->_xSpeechSubContext);
            }
            $speechResponse = $speechRequest->sendRequest();
           
            return $speechResponse;
        } catch (Exception $e) {
            $this->_error = $e->getMessage();
            return NULL;
        }
    }

    /**
    * Gets an error if there is one.
    *
    * @return string error if exists, NULL otherwise
    */
    public function getError() {
        return $this->_error;
    }
}
?>
