<?php
namespace Att\Api\Notary;

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
 * Immutable class used to hold notary information. This information includes:
 * <ul>
 * <li>Signed Document</li>
 * <li>Signature</li>
 * <li>Payload</li>
 * </ul>
 *
 * @category API 
 * @package  Notary 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

?>
