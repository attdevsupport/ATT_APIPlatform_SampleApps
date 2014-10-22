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

use InvalidArgumentException;

/**
 * Immutable class used to contain an Address Book Address.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class Address
{
    /* Address type constants. */
    const TYPE_HOME = 'HOME';
    const TYPE_WORK = 'WORK';
    const TYPE_NO_VALUE = 'NO_VALUE';

    /**
     * Address type.
     *
     * @var string
     */
    private $_type;

    /**
     * Whether address is preferred.
     *
     * @var boolean
     */
    private $_preferred;

    /**
     * PO Box.
     *
     * @var string
     */
    private $_poBox;

    /**
     * Address line one.
     *
     * @var string
     */
    private $_addressLineOne;

    /**
     * Address line two.
     *
     * @var string
     */
    private $_addressLineTwo;

    /**
     * City.
     *
     * @var string
     */
    private $_city;

    /**
     * State.
     *
     * @var string
     */
    private $_state;

    /**
     * Zip code.
     *
     * @var string
     */
    private $_zipCode;

    /**
     * Country.
     *
     * @var string
     */
    private $_country;


    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
        $this->_type = null;
        $this->_preferred = null;
        $this->_poBox = null;
        $this->_addressLineOne = null;
        $this->_addressLineTwo = null;
        $this->_city = null;
        $this->_state = null;
        $this->_zipCode = null;
        $this->_country = null;
    }

    /**
     * Gets the address type.
     *
     * The following values are valid address types:
     * <ul>
     * <li>HOME</li>
     * <li>WORK</li>
     * <li>NO VALUE</li>
     * </ul>
     *
     * @return string address type
     */
    public function getAddressType()
    {
        return $this->_type;
    }

    /**
     * Gets whether this address is preferred.
     *
     * @return boolean true if preferred, false otherwise
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
     * Gets PO Box.
     *
     * @return string PO Box
     */
    public function getPoBox()
    {
        return $this->_poBox;
    }

    /**
     * Gets address line one.
     *
     * @return string address line one
     */
    public function getAddressLineOne()
    {
        return $this->_addressLineOne;
    }

    /**
     * Gets address line two.
     *
     * @return string address line two
     */
    public function getAddressLineTwo()
    {
        return $this->_addressLineTwo;
    }

    /**
     * Gets city.
     *
     * @return string city
     */
    public function getCity()
    {
        return $this->_city;
    }

    /**
     * Gets state.
     *
     * @return string city
     */
    public function getState()
    {
        return $this->_state;
    }

    /**
     * Gets zip code.
     *
     * @return string zip code
     */
    public function getZipCode()
    {
        return $this->_zipCode;
    }

    /**
     * Gets country.
     *
     * @return string country
     */
    public function getCountry()
    {
        return $this->_country;
    }

    /**
     * Gets the Address object as an array.
     *
     * @return array Address object as array
     */
    public function toArray()
    {
        $arr = array();

        $mappings = array(
            'type' => &$this->_type,
            'preferred' => &$this->_preferred,
            'poBox' => &$this->_poBox,
            'addressLine1' => &$this->_addressLineOne,
            'addressLine2' => &$this->_addressLineTwo,
            'city' => &$this->_city,
            'state' => &$this->_state,
            'zip' => &$this->_zipCode,
            'country' => &$this->_country
        );

        foreach ($mappings as $k => $v) {
            if ($v !== null) {
                $arr[$k] = $v;
            }
        }

        return $arr;
    }

    /**
     * Factory method for creating an Address object from an array.
     *
     * @param array $arr array to construct an Address object from
     * @return Contact Contact response object
     */
    public static function fromArray($arr)
    {
        $addr = new Address();

        $mappings = array(
            'type' => &$addr->_type,
            'preferred' => &$addr->_preferred,
            'poBox' => &$addr->_poBox,
            'addressLine1' => &$addr->_addressLineOne,
            'addressLine2' => &$addr->_addressLineTwo,
            'city' => &$addr->_city,
            'state' => &$addr->_state,
            'zip' => &$addr->_zipCode,
            'country' => &$addr->_country
        );

        foreach ($mappings as $k => $v) {
            if (isset($arr[$k])) {
                $mappings[$k] = $arr[$k];
            }
        }

        return $addr;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
