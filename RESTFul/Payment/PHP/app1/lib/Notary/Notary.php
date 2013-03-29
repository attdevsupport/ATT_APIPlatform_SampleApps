<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Notary Library
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
 * @category Notary 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

class Notary {
    private $_signedDocument;
    private $_signature;
    private $_payload;

    public function __construct($signedDoc, $sig, $payload) {
        $this->_signedDocument = $signedDoc;
        $this->_signature = $sig;
        $this->_payload = $payload;
    }

    public function getSignedDocument() {
        return $this->_signedDocument;
    }

    public function getSignature() {
        return $this->_signature;
    }

    public function getPayload() {
        return $this->_payload;
    }
}
?>
