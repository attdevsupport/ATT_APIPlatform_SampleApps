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

/**
 * Immutable class for storing an OAuth authorization code.
 * 
 * @category Authentication 
 * @package  OAuth 
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
final class OAuthCode
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
        if ($code == null || $code == '') {
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
