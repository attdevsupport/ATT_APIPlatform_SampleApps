<?php
namespace Att\Api\OAuth;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * OAuth Library.
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
 * @category  Authentication 
 * @package   OAuth
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

 use Exception;

/**
 * Basic class for storing any exceptions related to OAuth.
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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


?>
