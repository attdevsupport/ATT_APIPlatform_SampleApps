<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

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
    private $_error;
    private $_xArgs;

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
            $token = $tokenRequest->getTokenUsingScope($this->_scope);

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
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_xArgs = $x_arg;
        $this->_error = NULL;
    }


    public function textToSpeech() {
        if (!isset($_REQUEST['TextToSpeechButton'])) {
            return NULL;
        }
        try {
            $token = $this->getToken();

            $ctype = $_REQUEST['ContentType'];

            $txt;
            if (strcmp($ctype, 'text/plain') == 0) {
                $txt = file_get_contents(__DIR__ . '/../../text/PlainText.txt'); 
            } else {
                $txt = file_get_contents(__DIR__ . '/../../text/SSMLWithPhoneme.txt');
            }

            $speechRequest = new SpeechRequest($this->_endpoint, $token);
            return $speechRequest->speechToText($ctype, $txt, $this->_xArgs);
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
