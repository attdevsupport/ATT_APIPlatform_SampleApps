<?php
namespace Att\Api\AAB;

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
 * Immutable class used to contain email information.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
