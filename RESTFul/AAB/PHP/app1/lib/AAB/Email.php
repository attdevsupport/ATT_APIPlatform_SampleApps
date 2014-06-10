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
 * Immutable class used to contain email information.
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
final class Email
{
    const TYPE_INTERNET = 'INTERNET';
    const TYPE_INTERNET_WORK = 'INTERNET,WORK';
    const TYPE_INTERNET_HOME = 'INTERNET,HOME';
    const TYPE_NO_VALUE = 'NO_VALUE';

    /**
     * Email type, if any.
     *
     * @var string|null
     */
    private $_type;

    /**
     * Whether email is preferred, if set.
     *
     * @var boolean|null
     */
    private $_preferred;

    /**
     * Email address, if any.
     *
     * @var string|null
     */
    private $_emailAddress;

    /**
     * Creates an Email object.
     *
     * @param string|null $type    email type or null
     * @param boolean|null $pref   whether email is preferred or null
     * @param string|null $address email address or null
     */
    public function __construct($type, $pref, $address)
    {
        $this->_type = $type;
        $this->_preferred = $pref;
        $this->_emailAddress = $address;
    }

    /**
     * Gets email type.
     *
     * @return string email type
     */
    public function getEmailType()
    {
        return $this->_type;
    }

    /**
     * Gets whether email is preferred.
     *
     * @return boolean whether email is preferred
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
     * Gets email address.
     *
     * @return string email address
     */
    public function getEmailAddress()
    {
        return $this->_emailAddress;
    }

    /**
     * Converts Email object to an associative array.
     *
     * @return array Email object as an array
     */
    public function toArray()
    {
        $arr = array();
        if ($this->getEmailType() !== null)
            $arr['type'] = $this->getEmailType();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        if ($this->getEmailAddress() !== null)
            $arr['emailAddress'] = $this->getEmailAddress();

        return $arr;
    }

    /**
     * Factory method for creating an Email object from an array.
     *
     * @param array $arr an array to create an Email object from
     * @return Email Email response object
     */
    public static function fromArray($arr)
    {
        $type = isset($arr['type']) ? $arr['type'] : null;
        $pref = isset($arr['preferred']) ? $arr['preferred'] : null;
        $addr = isset($arr['emailAddress']) ? $arr['emailAddress'] : null;

        return new Email($type, $pref, $addr);
    }
}

?>
