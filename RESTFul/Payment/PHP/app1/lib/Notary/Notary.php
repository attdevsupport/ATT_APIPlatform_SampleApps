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
 * @category  API 
 * @package   Notary 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to hold notary information. This information includes:
 * <ul>
 * <li>Signed Document</li>
 * <li>Signature</li>
 * <li>Payload</li>
 * </ul>
 *
 * @category API 
 * @package  Notary 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package version@
 * @link     http://developer.att.com
 */
class Notary
{

    /**
     * Signed document.
     */
    private $_signedDocument;

    /**
     * Signature.
     */
    private $_signature;

    /**
     * Payload used to generate signed document and signature.
     */
    private $_payload;

    /**
     * Creates a Notary object with the specified signed document, signature,
     * and payload.
     *
     * @param string $signedDoc signed document
     * @param string $sig       signature
     * @param string $payload   payload used to generate signed document and 
     *                          signature
     */
    public function __construct($signedDoc, $sig, $payload)
    {
        $this->_signedDocument = $signedDoc;
        $this->_signature = $sig;
        $this->_payload = $payload;
    }
    
    /**
     * Gets the signed document.
     *
     * @return string signed document
     */
    public function getSignedDocument()
    {
        return $this->_signedDocument;
    }

    /**
     * Gets the signed document.
     *
     * @return string signature
     */
    public function getSignature()
    {
        return $this->_signature;
    }

    /**
     * Gets the payload used to generate the signed document and signature. 
     *
     * @return string payload
     */
    public function getPayload()
    {
        return $this->_payload;
    }
}
?>
