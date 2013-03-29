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

/** 
 * An immutable class that holds the speech response variables returned by
 * AT&T's speech API.
 * 
 * @package Speech
 */
class SpeechResponse {
    // used to hold the response id
    private $_responseId;

    // used to hold the status, e.g. OK
    private $_status;

    // holds N-best hypothesis
    private $_NBest;

    /** 
     * Creates a speech response object using the specified parameters.
     *
     * @param string $responseId response id
     * @param string $status status
     * @param NBest $nBest N-Best hypothesis object
     */
    public function __construct($responseId, $status, NBest $nBest = NULL) {
        $this->_responseId = $responseId;
        $this->_status = $status;
        $this->_NBest = $nBest;
    }

    /**
     * Getter method for getting the response id.
     *
     * @return string response id
     */
    public function getResponseId() {
        return $this->_responseId;
    }

    /**
     * Getter method for getting response status.
     *
     * @return string status
     */
    public function getStatus() {
        return $this->_status;
    }

    /**
     * Getter method for getting the N-Best hypothesis object.
     * 
     * @return N-Best hypothesis object
     */
    public function getNBest() {
        return $this->_NBest;
    }

}

?>
