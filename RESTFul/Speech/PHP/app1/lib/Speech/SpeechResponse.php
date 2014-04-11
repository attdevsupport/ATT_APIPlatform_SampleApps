<?php
namespace Att\Api\Speech;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Speech API Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2014. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API 
 * @package   Speech 
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      https://developer.att.com/docs/apis/rest/3/Speech
 */

/** 
 * Immutable class that holds the speech response variables returned by AT&T's
 * speech API.
 * 
 * @category API
 * @package  Speech
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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
        $nbests = $recognition['NBest'];
        $jsonNBest = $nbests[0];

        $nBest = null;
        if (strtolower($status) == 'ok') {
            $nBest = NBest::fromArray($jsonNBest);
        }

        $sResponse = new SpeechResponse($responseId, $status, $nBest);
        return $sResponse;
    }

}

?>
