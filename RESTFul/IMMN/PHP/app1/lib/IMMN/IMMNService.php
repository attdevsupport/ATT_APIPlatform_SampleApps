<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * IMMN Library
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
 * @category  API
 * @package   IMMN 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/IMMNResponse.php';

/**
 * Used to interact with version 1 of the In-app Messaging from Mobile Number 
 * (IMMN) API.
 *
 * @category API
 * @package  IMMN
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number
 */
class IMMNService extends APIService
{

    /**
     * Creates an IMMNService object that can be used to interact with
     * the IMMN API.
     *
     * @param string     $FQDN  fully qualified domain name to which requests 
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization 
     */
    public function __construct($FQDN, OAuthToken $token) 
    {
        parent::__construct($FQDN, $token);
    }

    /**
     * Sends a request to the API for getting message headers sent to a
     * resource, such as a phone number (MSIDN), associated with the OAuth 
     * token specified during object creation.
     *
     * @param int    $headerCount number of messages (headers) to get
     * @param string $indexCursor index from which to start getting messages
     *
     * @return IMMNResponse an IMMNResponse object containing the messages
     * @throws InvalidArgumentException if headerCount is not an integer
     * @throws ServiceException if API request was not successful
     */
    public function getMessageHeaders($headerCount, $indexCursor = null) 
    {
        if ($headerCount == null || !is_int($headerCount)) {
            $exception = 'headerCount must be an integer.';
            throw new InvalidArgumentException($exception);
        }
        // TODO: Move endpoint common to constant variable
        $endpoint = $this->FQDN . '/rest/1/MyMessages';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addAuthorizationHeader($this->token);
        $req->addParam('HeaderCount', $headerCount);
        // Index cursor needs slight modification
        // if ($indexCursor != null) {
        //     $this->addParam('IndexCursor', $indexCursor);
        // }
        $result = $req->sendRequest();
        $responseArr = $this->parseResult($result);
        return IMMNResponse::buildFromJSON(json_encode($responseArr)); 
    }

    /**
     * Sends a message to the API for getting message content.
     *
     * @param string $msgId    message id
     * @param string $partNumb part number
     *
     * @return IMMNBody API response
     * @throws ServiceException if API request was not successful
     */
    public function getMessageBody($msgId, $partNumb) 
    {
        $endpoint = $this->FQDN . '/rest/1/MyMessages/' . $msgId . '/' 
            . $partNumb; 

        $req = new RESTFulRequest($endpoint);

        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);	
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/json');
        $req->addAuthorizationHeader($this->token);
        $result = $req->sendRequest();

        // ignore return value, call method to check if exception should be
        // thrown
        $this->parseResult($result);
        
        $headers = $result->getHeaders();
        $responseBody = $result->getResponseBody();
        return new IMMNBody($headers['content_type'], $responseBody);
    }

    /**
     * Sends a message to the specified addresses. 
     *
     * @param array      $addresses strings that holds the addresses to which 
     *                              the specified messages will be sent. 
     * @param string     $text      text body of message.
     * @param string     $subject   subject of message.
     * @param array|null $fnames    file names of attachments or null if none 
     *
     * @return string message id 
     * @throws ServiceException if API request was not successful
     */
    public function sendMessage($addresses, $text, $subject, $fnames = null) 
    {
        $endpoint = $this->FQDN . '/rest/1/MyMessages';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $req->addAuthorizationHeader($this->token);

        $vals = array(
            'Addresses' => $addresses, 
            'Text' => $text,
            'Subject' => $subject
        );
        $jvals = json_encode($vals);

        if ($fnames == null) {
            // no attachments, send basic POST
            $req->setHeader('Content-Type', 'application/json');
            $req->setBody($jvals);
        } else {
            // attachments, send as multipart 
            $mpart = new MultipartBody();
            $mpart->addJSONPart($jvals); 
            foreach ($fnames as $fname) {
                $mpart->addFilePart($fname);
            }
            $req->setMultipart($mpart);  
        }

        $result = $req->sendRequest();
        $responseArr = $this->parseResult($result);
        return $responseArr['Id'];
    }
}
?>
