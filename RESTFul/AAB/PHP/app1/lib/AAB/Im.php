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
 * Immutable class used to contain Im information.
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
final class Im
{

    /**
     * Im type, if any.
     *
     * @var string|null
     */
    private $_type;

    /**
     * Whether Im is preferred, if any.
     *
     * @var boolean|null
     */
    private $_preferred;

    /**
     * Im URI, if any.
     *
     * @var string|null
     */
    private $_imUri;

    /**
     * Creates an Im object.
     *
     * @param string|null $type    Im type or null
     * @param string|null $pref    whether email is preferred or null
     * @param string|null $address Im URI or null
     */
    public function __construct($type, $pref, $uri)
    {
        $this->_type = $type;
        $this->_preferred = $pref;
        $this->_imUri = $uri;
    }

    /**
     * Gets Im type.
     *
     * @return string Im type
     */
    public function getImType()
    {
        return $this->_type;
    }

    /**
     * Gets whether Im is preferred.
     *
     * @return boolean whether Im is preferred
     */
    public function isPreferred()
    {
        if (is_string($this->_preferred)) {
            $lpref = strtolower($this->_preferred);
            return $lpref == 'true';
        }

        return $this->_preferred;
    }

    /**
     * Gets Im URI.
     *
     * @return string Im URI
     */
    public function getImUri()
    {
        return $this->_imUri;
    }

    /**
     * Converts Im object to an associative array.
     *
     * @return array Im object as an associative array
     */
    public function toArray()
    {
        $arr = array();
        if ($this->getImType() !== null)
            $arr['type'] = $this->getImType();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        if ($this->getImUri() !== null)
            $arr['imUri'] = $this->getImUri();

        return $arr;
    }

    /**
     * Factory method for creating an Im object from an array.
     *
     * @param array $arr an array to create an Im object from
     * @return Im Im response object
     */
    public static function fromArray($arr)
    {
        $type = isset($arr['type']) ? $arr['type'] : null;
        $pref = isset($arr['preferred']) ? $arr['preferred'] : null;
        $uri = isset($arr['imUri']) ? $arr['imUri'] : null;

        return new Im($type, $pref, $uri);
    }
}

?>
