<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Restful Library.
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
 * @category  Network
 * @package   Restful
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class that contains response information received after sending a 
 * RESTFul request.
 * 
 * @category Network
 * @package  Restful 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
class RESTFulResponse
{
    /**
     * HTTP response body.
     *
     * @var string
     */
    private $_responseBody;

    /**
     * HTTP status code.
     *
     * @var int
     */
    private $_responseCode;

    /**
     * HTTP headers.
     *
     * @var array
     */
    private $_headers;

    /**
     * Creates a new RESTFulResponse with the specified HTTP response body, 
     * HTTP response code, and HTTP headers.
     *
     * @param string $responseBody HTTP response body
     * @param int    $responseCode HTTP response code
     * @param array  $headers      HTTP headers
     */
    public function __construct($responseBody, $responseCode, $headers) 
    {
        $this->_responseBody = $responseBody;
        $this->_responseCode = $responseCode;
        $this->_headers = $headers;
    }

    /**
     * Gets the HTTP response body.
     * 
     * @return string response body
     */
    public function getResponseBody() 
    {
        return $this->_responseBody;
    }

    /**
     * Gets the HTTP response code.
     *
     * @return int response code
     */
    public function getResponseCode() 
    { 
        return $this->_responseCode;
    }

    /**
     * Gets HTTP headers.
     *
     * @return array HTTP headers
     */
    public function getHeaders()
    {
        return $this->_headers;
    }
}
?>
