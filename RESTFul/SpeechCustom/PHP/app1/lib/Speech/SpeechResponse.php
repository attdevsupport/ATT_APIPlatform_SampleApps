<?php
namespace Att\Api\Speech;

/*
 * Copyright 2014 AT&T
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

/** 
 * Immutable class that holds the speech response variables returned by AT&T's
 * speech API.
 * 
 * @category API
 * @package  Speech
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Speech
 */
final class SpeechResponse
{
    /**
     * Response id.
     * 
     * @var string
     */
    private $_responseId;

    /**
     * Response status, such as OK.
     *
     * @var string
     */
    private $_status;

    /**
     * N-Best hypothesis.
     * 
     * @var NBest
     */
    private $_NBest;

    /** 
     * Creates a speech response object using the specified parameters.
     *
     * @param string     $responseId response id
     * @param string     $status     status
     * @param NBest|null $nBest      N-Best hypothesis object or null if none
     */
    public function __construct($responseId, $status, NBest $nBest = null)
    {
        $this->_responseId = $responseId;
        $this->_status = $status;
        $this->_NBest = $nBest;
    }

    /**
     * Gets the response id.
     *
     * @return string response id
     */
    public function getResponseId()
    {
        return $this->_responseId;
    }

    /**
     * Gets response status.
     *
     * @return string status
     */
    public function getStatus()
    {
        return $this->_status;
    }

    /**
     * Gets the N-Best hypothesis object.
     * 
     * @return N-Best hypothesis object
     */
    public function getNBest()
    {
        return $this->_NBest;
    }

    public static function fromArray($arr)
    {
        $recognition = $arr['Recognition'];

        $responseId = $recognition['ResponseId'];
        $status = $recognition['Status'];
        $nBest = null;
        if (strtolower($status) == 'ok') {
            $nbests = $recognition['NBest'];
            $jsonNBest = $nbests[0];
            $nBest = NBest::fromArray($jsonNBest);
        }

        $sResponse = new SpeechResponse($responseId, $status, $nBest);
        return $sResponse;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
