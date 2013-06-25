<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library.
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
 * @category  Authentication 
 * @package   OAuth
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class for storing an OAuth authorization code.
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
class OAuthCode
{

    /**
     * Authorization code.
     */
    private $_code;

    /**
     * Creates an OAuth Code object with the specified code.
     *
     * @param string $code authorization code
     * 
     * @throws InvalidArgumentException if code is null
     */
    public function __construct($code)
    {
        if ($code == null) {
            throw new InvalidArgumentException('Code must not be null.');
        } 
        
        if ($code == '') {
            throw new InvalidArgumentException('Code must not be empty.');
        }

        $this->_code = $code;
    }

    /**
     * Returns the OAuth code object as a string.
     *
     * @return string OAuth code as string
     */
    public function __toString()
    {
        return $this->_code;
    }
}

?>
