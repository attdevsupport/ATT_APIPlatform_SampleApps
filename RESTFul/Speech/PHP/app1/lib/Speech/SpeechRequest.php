<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Speech API Library
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
 * @category SpeechAPI 
 * @package Speech 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require __DIR__ . '/NBest.php';
require __DIR__ . '/SpeechResponse.php';

/**
 * Used for sending API Speech requests.
 *
 * @package Speech
 */
class SpeechRequest {
    private $_xArgs;
    private $_xSpeechSubContext;
    private $_speechContext;
    private $_chunked;
    private $_speechURL;
    private $_token;
    private $_file;
    private $_headers;

    /** 
     * Builds a speech response using the specified JSON object and returns
     * the result as a SpeechResponse object.
     *
     * @param JSONObject json object
     * @return SpeechResponse JSON Object converted to a SpeechResponse Object
     */
    private function buildSpeechResponse($jsonObj) {
        $responseId = $jsonObj->ResponseId;
        $status = $jsonObj->Status;
        $jsonNBest = $jsonObj->NBest[0];

        $nBest = NULL;
        if (strcmp($status, "OK") == 0) {
            $nBest = new NBest(
                    $jsonNBest->Hypothesis,
                    $jsonNBest->LanguageId,
                    $jsonNBest->Confidence,
                    $jsonNBest->Grade,
                    $jsonNBest->ResultText,
                    $jsonNBest->Words,
                    $jsonNBest->WordScores);
        }
    
        $sResponse = new SpeechResponse($responseId, $status, $nBest);
        return $sResponse;
    }

    /**
     * Gets a file's MIME type. This function is currently very basic and
     * just looks at file ending.
     *
     * @return string file MIME type
     */
    private function getFileMIMEType() {
        $arr = explode('.', $this->_file);
        $ending = end($arr);
        if (strcmp('spx', $ending) == 0) {
            $ending = 'x-speex'; 
        }
        $type = 'audio/' . $ending;
        return $type;
    }

    /**
     * Build the headers array to be used in API request.
     */
    private function buildHeaders() {
        $headersMap = array();
        $headersMap['Authorization'] = 
            'BEARER ' . $this->_token->getAccessToken();
        $headersMap['Accept'] = 'application/json';
        $headersMap['Content-Type'] = $this->getFileMIMEType();

        if ($this->_chunked) {
            $headersMap['Content-Transfer-Encoding'] = 'chunked';
        } else {
            $headersMap['Content-Length'] = filesize($this->_file);
        }

        $headersMap['X-SpeechContext'] = $this->_speechContext;

        if ($this->_xArgs != NULL) {
            $headersMap['X-Arg'] = $this->_xArgs;
        }

        if ($this->_xSpeechSubContext != null) {
            $headersMap['X-SpeechSubContext'] = $this->_xSpeechSubContext;
        }

        $this->_headers = array();
        foreach ($headersMap as $key => $value) { 
            array_push($this->_headers, $key . ': ' . $value); 
        }
    }

    /**
     * Creates a SpeechRequest object with the specified parameters.
     * The following defaults are used for some of the variables:
     * <ul> 
     * <li>xArgs: NULL</li>
     * <li>speech context: Generic</li>
     * <li>chunked: true</li>
     * </ul>
     *
     * @param OAuthToken $token access token to use when sending request
     * @param string $speechURL speech URL to send request to
     * @param string $file file to load when sending request
     */
    public function __construct(OAuthToken $token, $speechURL, $file) {
        $this->_token = $token;
        $this->_speechURL = $speechURL;
        $this->_file = $file;

        $this->_headers = array();

        $this->_xArgs = NULL;
        $this->_xSpeechSubContext = NULL;
        $this->_speechContext = 'Generic';
        $this->_chunked = true;
    }

    /**
    * Overwrites the current file to send with the specified one.
    *
    * @param string $file file name
    */
    public function setFile($file) {
        $this->_file = $file;
    }

    /**
    * Sets the optional X-Args to use when sending request
    *
    * @param string $xArgs
    */
    public function setXArgs($xArgs) {
        $this->_xArgs = $xArgs;
    }

    public function setXSpeechSubContext($xSpeechSubContext) {
        $this->_xSpeechSubContext = $xSpeechSubContext;
    }


    /**
    * Sets whether to send the request chunked.
    *
    * @param boolean $chunked whether to send request chunked
    */
    public function setChunked($chunked) {
        $this->_chunked = $chunked;
    }

    public function setSpeechContext($context) {
        $this->_speechContext = $context;
    }

    /**
     * Sends an API request using the previously set parameters.
     * 
     * @return SpeechResponse the speech response sent back by the API
     */
    public function sendRequest() {
        $fileResource = fopen($this->_file, 'r');
        if (!$fileResource) {
            throw new RuntimeException('Could not open file ' . $this->_file);
        }

        $fileBinary = fread($fileResource, filesize($this->_file));
        fclose($fileResource);
        $this->buildHeaders();

        $options = array(
                CURLOPT_URL => $this->_speechURL,
                CURLOPT_HTTPGET => true,
                CURLOPT_HEADER => false,
                CURLINFO_HEADER_OUT => true,
                CURLOPT_HTTPHEADER => $this->_headers,
                CURLOPT_RETURNTRANSFER => true,
                CURLOPT_POSTFIELDS => $fileBinary,
                CURLOPT_SSL_VERIFYHOST => false,
                CURLOPT_SSL_VERIFYPEER => false
                );

        $connection = curl_init();
        curl_setopt_array($connection, $options);
        $response = curl_exec($connection);
        $responseCode = curl_getinfo($connection, CURLINFO_HTTP_CODE);
    
        // request not successful
        if ($responseCode != 200 && $responseCode != 201) {
            throw new OAuthException($responseCode, $response);
        }
        
        $jsonResponse = json_decode($response);
        $jsonObj = $jsonResponse->Recognition;
        return $this->buildSpeechResponse($jsonObj);
    }
}

?>
