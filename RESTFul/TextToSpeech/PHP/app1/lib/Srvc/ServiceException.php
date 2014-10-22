<?php
namespace Att\Api\Srvc;

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

use Exception;

/**
 * Basic class for storing any exceptions related to API requests.
 *
 * @category API
 * @package  Service
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @link     http://developer.att.com
 * @see      Exception
 */
final class ServiceException extends Exception
{
    /**
     * Http status code returned by API after unsuccessful request.
     *
     * @var int
     */
    private $_errorCode;

    /** 
     * Http response body returned by API after unsuccessful request.
     *
     * @var string
     */
    private $_errorResponse;


    /**
     * Creates a new ServiceException object.
     *
     * @param string    $errBody  http status code as a result of an 
     *                            unsuccessful API request   
     * @param string    $errCode  http message body as a result of an 
     *                            unsuccessful API request
     * @param string    $msg      optional exception message
     * @param int       $code     optional exception code
     * @param Exception $previous optional previous exception
     */
    public function __construct(
        $errBody, $errCode, $msg = null, $code = 0, Exception $previous = null
    ) {

        if (!isset($msg)) {
            $msg = $errCode . ':' . $errBody; 
        }

        parent::__construct($msg, $code, $previous);
        $this->_errorCode = $errCode;
        $this->_errorResponse = $errBody;
    }

    /**
     * Gets the API http code associated with this exception. 
     * 
     * @return int http error code
     */
    public function getErrorCode()
    {
        return $this->_errorCode;
    }

    /**
     * Gets the API http response body associated with this exception.
     * 
     * @return string http error response body 
     */
    public function getErrorResponse() 
    {
        return $this->_errorResponse;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
