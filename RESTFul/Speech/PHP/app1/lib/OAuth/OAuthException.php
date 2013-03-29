<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library
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
 * @category Authentication 
 * @package OAuth
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

/**
 * Basic class for storing any exceptions related to OAuth.
 * 
 * @package OAuth 
 */
class OAuthException extends Exception {
    // error returned by OAuth 
    private $_error;

    // error description returned by Oauth
    private $_errorDesc;


    /**
     * Creates a new OAuth Exception object.
     *
     * @param string $error the OAuth error
     * @param string $errorDesc the OAuth error description
     * @param string $msg optional exception message
     * @param code int optional exception code
     * @param Exception optional previous exception
     */
    public function __construct($error, $errorDesc, $msg = NULL, $code = 0, 
            Exception $previous = NULL) {

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
    public function getError() {
        return $this->error;
    }

    /**
     * Gets the OAuth error description associated with this exception.
     * 
     * @return string error description
     */
    public function getErrorDescription() {
        return $this->errorDesc;
    }
}


?>
