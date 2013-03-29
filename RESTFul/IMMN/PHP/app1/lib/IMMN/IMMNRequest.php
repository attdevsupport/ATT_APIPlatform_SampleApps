<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * IMMN API Library
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
 * @category IMMNAPI 
 * @package IMMN 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */
require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';
require_once __DIR__ . '/IMMNResponse.php';

/**
 * Used for sending IMMN requests. Such requests include getting and sending
 * messages.
 *
 * @package IMMN
 */
class IMMNRequest extends RESTFulRequest {
    private $_headers;
    private $_token;

    /**
     * Creates an IMMN object with the specified url and access token.
     *
     * @param $URL string URL to send requests to
     * @param $token OAuthToken access token to use for authorization
     */
    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token; 
    }

    /**
     * Gets message headers sent to the resource, such as a phone number (MSIDN),  
     * associated with the previously specified access token.
     *
     * @param $headerCount int number of messages (headers) to get
     * @param $indexCursor index from which to start getting messages
     *
     * @return IMMNResponse an IMMNResponse object containing the messages
     * @throws InvalidArgumentException if headerCount is not an integer
     * @throw Exception if request was not successful
     */
    public function getMessageHeaders($headerCount, $indexCursor = NULL) {
        if ($headerCount == NULL || !is_int($headerCount)) {
            $exception = 'headerCount must be an integer.';
            throw new InvalidArgumentException($exception);
        }

        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addAuthorizationHeader($this->_token);
        $this->addParam('HeaderCount', $headerCount);
        // Index cursor needs slight modification
        // if ($indexCursor != NULL) {
        //     $this->addParam('IndexCursor', $indexCursor);
        // }
        $result = $this->sendRequest();

        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return IMMNResponse::buildFromJSON($response); 
    }

    public function getMessageBody() {
    	$this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);	
        $this->setHeader('Accept', 'application/json');
        $this->setHeader('Content-Type', 'application/json');
        $this->addAuthorizationHeader($this->_token);
        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        $headers = $result['headers'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return new IMMNBody($headers['content_type'], $response);
    }

    /**
     * Sends a message to the specified addresses. 
     *
     * @param $addresses array array of strings that holds the addresses
     * to send the messages to
     * @param $text string text body
     * @param $subject string subject
     * @param $fnames file names of attachments (optional)
     *
     * @return string message id 
     * @throws Exception if request was not successful
     */
    public function sendMessage($addresses, $text, $subject, $fnames = NULL) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->addAuthorizationHeader($this->_token);

        $vals = array('Addresses' => $addresses, 'Text' => $text,
                'Subject' => $subject);
        $jvals = json_encode($vals);

        if ($fnames == NULL) {
            // no attachments, send basic POST
            $this->setHeader('Content-Type', 'application/json');
            $this->setBody($jvals);
        } else {
            // attachments, send as multipart 
            $mpart = new Multipart();
            $mpart->addJSONPart($jvals); 
            foreach($fnames as $fname) {
                $mpart->addFilePart($fname);
            }
            $this->setMultipart($mpart);  
        }

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        $jresponse = json_decode($response);
        return $jresponse->Id;
    }
}
?>
