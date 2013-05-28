<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Speech Library
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
 * @package   Speech 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/NBest.php';
require_once __DIR__ . '/SpeechMultipart.php';
require_once __DIR__ . '/SpeechResponse.php';

/**
 * Used to interact with version 3 of the Speech API.
 *
 * @category API
 * @package  Speech
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Speech
 */
class SpeechService extends APIService
{

    /**
     * Gets the MIME type of the specified file. 
     *
     * This implementation currently only looks at the file ending and is 
     * therefore very trivial.
     *
     * @param string $fname file name to examine.
     *
     * @return string MIME type of specified file.
     */
    private function _getFileMIMEType($fname)
    {
        $arr = explode('.', $fname);
        $ending = end($arr);
        if (strcmp('spx', $ending) == 0) {
            $ending = 'x-speex'; 
        }
        $type = 'audio/' . $ending;
        return $type;
    }

    /**
     * Builds a SpeechResponse object from the specified array.
     *
     * @param array $jsonArr API response as an array.
     * 
     * @return SpeechResponse API response as a SpeechResponse object
     */
    private function _buildSpeechResponse($jsonArr)
    {
        $recognition = $jsonArr['Recognition'];

        $responseId = $recognition['ResponseId'];
        $status = $recognition['Status'];
        $nbests = $recognition['NBest'];
        $jsonNBest = $nbests[0];

        $nBest = null;
        if ($status == 'OK') {
            $nBest = new NBest(
                $jsonNBest['Hypothesis'],
                $jsonNBest['LanguageId'],
                $jsonNBest['Confidence'],
                $jsonNBest['Grade'],
                $jsonNBest['ResultText'],
                $jsonNBest['Words'],
                $jsonNBest['WordScores']
            );
        }

        $sResponse = new SpeechResponse($responseId, $status, $nBest);
        return $sResponse;
    }

    /**
     * Creates an SpeechService object that can be used to interact with
     * the Speech API.
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
     * Sends a request to the API for converting speech to text.
     *
     * @param string      $fname            file location that contains speech 
     *                                      to convert.
     * @param string      $speechContext    speech context to use.
     * @param string|null $speechSubContext speech sub context to use, if not 
     *                                      null.
     * @param string|null $xArg             optional arguments to use, if not 
     *                                      null.
     * @param boolean     $chunked          whether to send the request chunked.
     *
     * @return SpeechResponse API response as a SpeechResponse object.
     * @throws ServiceException if API request was not successful.
     */
    public function speechToText($fname, $speechContext, 
        $speechSubContext = null, $xArg = null, $chunked = true
    ) {
        // read file
        $fileResource = fopen($fname, 'r');
        if (!$fileResource) {
            throw new InvalidArgumentException('Could not open file ' . $fname);
        }
        $fileBinary = fread($fileResource, filesize($fname));
        fclose($fileResource);

        $endpoint = $this->FQDN . '/speech/v3/speechToText';
        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFULRequest::HTTP_METHOD_POST);
        $req->addAuthorizationHeader($this->token);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', $this->_getFileMIMEType($fname));

        if ($chunked) {
            $req->setHeader('Content-Transfer-Encoding', 'chunked');
        } else {
            $req->setHeader('Content-Length', filesize($fname));
        }
        $req->setHeader('X-SpeechContext', $speechContext);
        if ($xArg != null) {
            $req->setHeader('xArg', $xArg);
        }

        if ($speechSubContext != null) {
            $req->setHeader('X-SpeechSubContext', $speechSubContext);
        }

        $req->setBody($fileBinary);
        $result = $req->sendRequest();
        $jsonArr = $this->parseResult($result);
        return $this->_buildSpeechResponse($jsonArr);
    }

    /**
     * Sends a request to the API for converting text to speech.
     *
     * @param string      $ctype content type.
     * @param string      $txt   text to convert to speech.
     * @param string|null $xArg  optional arguments to set, if not null.
     *
     * @return raw audio/x-wave data.
     * @throws ServiceException if API request was not successful.
     */
    public function textToSpeech($ctype, $txt, $xArg = null) 
    {
        $endpoint = $this->FQDN . '/speech/v3/textToSpeech';
        $req = new RESTFulRequest($endpoint);

        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $req->addAuthorizationHeader($this->token);
        $req->setHeader('Accept', 'audio/x-wav');
        $req->setHeader('Content-Type', $ctype);
        if ($xArg != null) {
            $req->setHeader('X-Arg', $xArg);
        }

        $req->setBody($txt);

        $result = $req->sendRequest();
        $code = $result->getResponseCode();
        $body = $result->getResponseBody();
        if ($code != 200 && $code != 201) {
            throw new ServiceException($body, $code);
        }

        return $body;
    }

    /**
     * Sends a custom API request for converting speech to text.
     *
     * @param string $cntxt  speech context.
     * @param string $fname  file location that contains speech to convert.
     * @param string $gfname file location that contains grammar.
     * @param string $dfname file location that contains dictionary.
     * @param string $xArg   optional arguments.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function speechToTextCustom($cntxt, $fname, $gfname = null, 
        $dfname = null, $xArg = null
    ) {
        $endpoint = $this->FQDN . '/speech/v3/speechToTextCustom';
        $req = new RESTFulRequest($endpoint);
        $req->setHeader('Accept', 'application/json');
        $req->addAuthorizationHeader($this->token);
        
        if ($xArg != null) {
            $req->setHeader('X-Arg', $xArg);
        }

        $mpart = new SpeechMultipartBody();
        $req->setHeader('Content-Type', $mpart->getContentType());

        if ($dfname != null) {
            $mpart->addXDictionaryPart($dfname);
        }
        if ($gfname != null) {
            $mpart->addXGrammarPart($gfname);
        }
        $mpart->addFilePart($fname);
        $req->setMultipart($mpart);

        $result = $req->sendRequest();
        return $this->parseResult($result);
    }

}

?>
