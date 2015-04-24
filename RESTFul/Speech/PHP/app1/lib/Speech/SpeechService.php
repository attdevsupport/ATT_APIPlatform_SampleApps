<?php
namespace Att\Api\Speech;

/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/NBest.php';
require_once __DIR__ . '/SpeechMultipart.php';
require_once __DIR__ . '/SpeechResponse.php';

use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpPost;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;
use Att\Api\Srvc\ServiceException;

use InvalidArgumentException;

/**
 * Used to interact with version 3 of the Speech API.
 *
 * @category API
 * @package  Speech
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

        $endpoint = $this->getFqdn() . '/speech/v3/speechToText';
        $req = new RESTFulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', $this->_getFileMIMEType($fname))
            ->setHeader('X-SpeechContext', $speechContext);

        if ($chunked) {
            $req->setHeader('Transfer-Encoding', 'chunked');
        } else {
            $req->setHeader('Content-Length', filesize($fname));
        }
        if ($xArg != null) {
            $req->setHeader('xArg', $xArg);
        }

        if ($speechSubContext != null) {
            $req->setHeader('X-SpeechSubContext', $speechSubContext);
        }

        $httpPost = new HttpPost();
        $httpPost->setBody($fileBinary);

        $result = $req->sendHttpPost($httpPost);

        $jsonArr = Service::parseJson($result);

        return SpeechResponse::fromArray($jsonArr);
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
        $endpoint = $this->getFqdn() . '/speech/v3/textToSpeech';
        $req = new RESTFulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'audio/x-wav')
            ->setHeader('Content-Type', $ctype);

        if ($xArg != null) {
            $req->setHeader('X-Arg', $xArg);
        }

        $httpPost = new HttpPost();
        $httpPost->setBody($txt);

        $result = $req->sendHttpPost($httpPost);

        $code = $result->getResponseCode();
        $body = $result->getResponseBody();
        if ($code != 200 && $code != 201) {
            throw new ServiceException($body, $code);
        }

        $body = $result->getResponseBody();
        return $body;
    }

    /**
     * Sends a custom API request for converting speech to text.
     *
     * @param string $cntxt  speech context.
     * @param string $fname  path to file that contains speech to convert.
     * @param string $gfname path to file that contains grammar.
     * @param string $dfname path to file that contains dictionary.
     * @param string $xArg   optional arguments.
     * @param string $lang   language used to set the Content-Language header.
     *
     * @return array API response as an array of key-value pairs.
     * @throws ServiceException if API request was not successful.
     */
    public function speechToTextCustom($cntxt, $fname, $gfname = null, 
        $dfname = null, $xArg = null, $lang = 'en-US'
    ) {
        $endpoint = $this->getFqdn() . '/speech/v3/speechToTextCustom';
        $mpart = new SpeechMultipartBody();

        $req = new RESTFulRequest($endpoint);
        $req
            ->setHeader('X-SpeechContext', $cntxt)
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Language', $lang)
            ->setAuthorizationHeader($this->getToken());
        
        if ($xArg != null) {
            $req->setHeader('X-Arg', $xArg);
        }
        if ($dfname != null) {
            $mpart->addXDictionaryPart($dfname);
        }
        if ($gfname != null) {
            $mpart->addXGrammarPart($gfname);
        }
        $mpart->addFilePart($fname);
        $result = $req->sendHttpMultipart($mpart);

        return Service::parseJson($result);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
