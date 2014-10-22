<?php
namespace Att\Api\OAuth;

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
 * Basic class for storing any exceptions related to OAuth.
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @link     http://developer.att.com
 */
class OAuthException extends Exception
{
    /**
     * error returned by OAuth.
     */
    private $_error;

    /** 
     * error description returned by Oauth.
     */
    private $_errorDesc;


    /**
     * Creates a new OAuth Exception object.
     *
     * @param string    $error     the OAuth error
     * @param string    $errorDesc the OAuth error description
     * @param string    $msg       optional exception message
     * @param int       $code      optional exception code
     * @param Exception $previous  optional previous exception
     */
    public function __construct(
        $error, $errorDesc, $msg = null, $code = 0, Exception $previous = null
    ) {

        if (!isset($msg)) {
            $msg = 'Error: ' . $error . ' Error Description: ' . $errorDesc; 
        }

        parent::__construct($msg, $code, $previous);
        $this->_error = $error;
        $this->_errorDesc = $errorDesc;
    }

    /**
     * Gets the OAuth error associated with this exception.
     * 
     * @return string error
     */
    public function getError()
    {
        return $this->_error;
    }

    /**
     * Gets the OAuth error description associated with this exception.
     * 
     * @return string error description
     */
    public function getErrorDescription() 
    {
        return $this->_errorDesc;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
