<?php
namespace Att\Api\AAB;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * AAB Library
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
 * @category  API
 * @package   AAB
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to contain photo information.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class Photo
{
    /**
     * Photo encoding.
     *
     * @var string
     */
    private $_encoding;

    /**
     * Photo value.
     *
     * @var string
     */
    private $_value;

    /**
     * Creates a Photo object.
     *
     * @param string $value    photo value
     * @param string $encoding photo encoding
     */
    public function __construct($value, $encoding='BASE64')
    {
        $this->_encoding = $encoding;
        $this->_value = $value;
    }

    /**
     * Gets photo encoding.
     *
     * @return string photo encoding
     */
    public function getEncoding()
    {
        return $this->_encoding;
    }

    /**
     * Gets photo value.
     *
     * @return string photo value
     */
    public function getValue()
    {
        return $this->_value;
    }
}

?>
