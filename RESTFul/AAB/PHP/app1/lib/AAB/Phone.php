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
 * Immutable class used to contain phone information.
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
final class Phone
{
    /* Phone type constants. */
    const TYPE_CELL = 'CELL';
    const TYPE_HOME_CELL = 'HOME,CELL';
    const TYPE_WORK_CELL = 'WORK,CELL';
    const TYPE_VOICE = 'VOICE';
    const TYPE_HOME_VOICE = 'HOME,VOICE';
    const TYPE_WORK_VOICE = 'WORK,VOICE';
    const TYPE_FAX = 'FAX';
    const TYPE_HOME_FAX = 'HOME,FAX';
    const TYPE_WORK_FAX = 'WORK,FAX';
    const TYPE_VIDEO = 'VIDEO';
    const TYPE_HOME_VIDEO = 'HOME,VIDEO';
    const TYPE_WORK_VIDEO = 'WORK,VIDEO';
    const TYPE_PAGER = 'PAGER';
    const TYPE_CAR = 'CAR';
    const TYPE_OTHER = 'OTHER';
    const TYPE_NO_VALUE = 'NO VALUE';

    /**
     * Phone type, if any.
     *
     * @var string|null
     */
    private $_type;

    /**
     * Phone number, if any.
     *
     * @var string|null
     */
    private $_number;

    /**
     * Whether phone is preferred, if any.
     *
     * @var boolean|null
     */
    private $_preferred;

    /**
     * Creates an Phone object.
     *
     * @param string|null $type      phone type or null
     * @param boolean|null $number   phone number or null
     * @param string|null $preferred whether phone is preferred or null
     */
    public function __construct($type, $number, $preferred)
    {
        $this->_type = $type;
        $this->_number = $number;
        $this->_preferred = $preferred;
    }

    /**
     * Gets phone type.
     *
     * @return string phone type
     */
    public function getPhoneType()
    {
        return $this->_type;
    }

    /**
     * Gets whether email is preferred.
     *
     * @return boolean whether email is preferred
     */
    public function getNumber()
    {
        return $this->_number;
    }

    /**
     * Gets whether phone is preferred.
     *
     * @return boolean whether phone is preferred
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
     * Converts Phone object to an associative array.
     *
     * @return array Phone object as an array
     */
    public function toArray()
    {
        $arr = array();

        if ($this->getPhoneType() !== null)
            $arr['type'] = $this->getPhoneType();

        if ($this->getNumber() !== null)
            $arr['number'] = $this->getNumber();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        return $arr;
    }

    /**
     * Factory method for creating a Phone object from an array.
     *
     * @param array $arr an array to create an Phone object from
     * @return Phone Phone response object
     */
    public static function fromArray($arr)
    {
        $type = isset($arr['type']) ? $arr['type'] : null;
        $number = isset($arr['number']) ? $arr['number'] : null;
        $preferred = isset($arr['preferred']) ? $arr['preferred'] : null;

        $phone = new Phone($type, $number, $preferred);

        return $phone;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
