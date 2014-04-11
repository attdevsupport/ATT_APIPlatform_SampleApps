<?php
namespace Att\Api\Restful;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Restful Library.
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
 * @category  Network
 * @package   Restful
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class that contains response information received after sending a 
 * Restful request.
 * 
 * @category Network
 * @package  Restful 
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
final class RestfulResponse
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
     * Creates a new RestfulResponse with the specified HTTP response body, 
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

    /**
     * Gets the value of the specified HTTP header or null if no such header.
     *
     * The <code>$name</code> field is NOT case sensitive.
     *
     * @param string $name http header name
     *
     * @return string http header value
     */
    public function getHeader($name)
    {
        foreach ($this->_headers as $k => $v) {
            if (strtolower($k) == strtolower($name)) {
                return $v;
            }
        }

        return null;
    }

}
?>
