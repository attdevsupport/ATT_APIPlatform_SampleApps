<?php
namespace Att\Api\Restful;

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
 * Immutable class that contains response information received after sending a 
 * Restful request.
 * 
 * @category Network
 * @package  Restful 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
