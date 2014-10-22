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

require_once __DIR__ . '/Address.php';
require_once __DIR__ . '/ContactCommon.php';
require_once __DIR__ . '/Email.php';
require_once __DIR__ . '/Im.php';
require_once __DIR__ . '/Phone.php';

/**
 * Immutable class used to contain a quick contact.
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
final class QuickContact
{

    /**
     * Contact identifier.
     *
     * @var string
     */
    private $_contactId;

    /**
     * Contact's formatted name.
     *
     * @var string
     */
    private $_formattedName;

    /**
     * Contact's first name, if any.
     *
     * @var string|null
     */
    private $_firstName;

    /**
     * Contact's middle name, if any.
     *
     * @var string|null
     */
    private $_middleName;

    /**
     * Contact's last name.
     *
     * @var string|null
     */
    private $_lastName;

    /**
     * Contact's prefix, if any.
     *
     * @var string|null
     */
    private $_prefix;

    /**
     * Contact's suffix, if any.
     *
     * @var string|null
     */
    private $_suffix;

    /**
     * Contact's nickname, if any.
     *
     * @var string|null
     */
    private $_nickname;

    /**
     * Contact's organization, if any.
     *
     * @var string|null
     */
    private $_organization;

    /**
     * Contact's preferred phone, if any.
     *
     * @var Phone
     */
    private $_phone;

    /**
     * Contact's preferred email, if any.
     *
     * @var Email
     */
    private $_email;

    /**
     * Contact's preferred instance messenger address, if any.
     *
     * @var Im
     */
    private $_im;

    /**
     * Contact's preferred address, if any.
     *
     * @var Address
     */
    private $_addr;

    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
        $this->_contactId = null;
        $this->_formattedName = null;
    }

    /**
     * Gets contact identifier.
     *
     * @var string contact id
     */
    public function getContactId()
    {
        return $this->_contactId;
    }

    /**
     * Gets formatted name.
     *
     * @var string formatted name
     */
    public function getFormattedName()
    {
        return $this->_formattedName;
    }

    /**
     * Gets first name.
     *
     * @var string first name
     */
    public function getFirstName()
    {
        return $this->_firstName;
    }

    /**
     * Gets middle name.
     *
     * @var string middle name
     */
    public function getMiddleName()
    {
        return $this->_middleName;
    }

    /**
     * Gets last name.
     *
     * @var string last name
     */
    public function getLastName()
    {
        return $this->_lastName;
    }

    /**
     * Gets prefix.
     *
     * @var string prefix
     */
    public function getPrefix()
    {
        return $this->_prefix;
    }

    /**
     * Gets suffix.
     *
     * @var string suffix
     */
    public function getSuffix()
    {
        return $this->_suffix;
    }

    /**
     * Gets nickname.
     *
     * @var string nickname
     */
    public function getNickname()
    {
        return $this->_nickname;
    }

    /**
     * Gets organization.
     *
     * @var string organization
     */
    public function getOrganization()
    {
        return $this->_organization;
    }

    /**
     * Gets preferred phone.
     *
     * @var Phone phone
     */
    public function getPhone()
    {
        return $this->_phone;
    }

    /**
     * Gets preferred email.
     *
     * @var Email email
     */
    public function getEmail()
    {
        return $this->_email;
    }

    /**
     * Gets preferred Im.
     *
     * @var Im im
     */
    public function getIm()
    {
        return $this->_im;
    }

    /**
     * Gets preferred address.
     *
     * @var Address address
     */
    public function getAddress()
    {
        return $this->_addr;
    }

    /**
     * Gets the QuickContact object as an array.
     *
     * @return array contact object as array
     */
    public function toArray()
    {
        $arr = array();
        if ($this->_contactId !== null)
            $arr['contactId'] = $this->_contactId;

        if ($this->_creationDate !== null)
            $arr['creationDate'] = $this->_creationDate;

        if ($this->_modificationDate !== null)
            $arr['modificationDate'] = $this->_modificationDate;

        if ($this->_formattedName !== null)
            $arr['formattedName'] = $this->_formattedName;

        $commonArr = $this->_contactCommon->toArray();

        $arr = array_merge($arr, $commonArr);

        return $arr;
    }

    /**
     * Factory method for creating a QuickContact object from an array.
     *
     * @param array $arr array to construct a QuickContact object from
     * @return QuickContact QuickContact response object
     */
    public static function fromArray($arr)
    {
        $qcontact = new QuickContact();

        $mappings = array(
            'contactId' => &$qcontact->_contactId,
            'formattedName' => &$qcontact->_formattedName,
            'firstName' => &$qcontact->_firstName,
            'middleName' => &$qcontact->_middleName,
            'lastName' => &$qcontact->_lastName,
            'prefix' => &$qcontact->_prefix,
            'suffix' => &$qcontact->_suffix,
            'nickName' => &$qcontact->_nickname,
            'organization' => &$qcontact->_organization,
        );

        foreach ($mappings as $k => $v) {
            if (isset($arr[$k])) {
                $mappings[$k] = $arr[$k];
            }
        }

        if (isset($arr['phone'])) {
            $qcontact->_phone = Phone::fromArray($arr['phone']);
        }
        if (isset($arr['email'])) {
            $qcontact->_email = Email::fromArray($arr['email']);
        }
        if (isset($arr['im'])) {
            $qcontact->_im = Im::fromArray($arr['im']);
        }
        if (isset($arr['address'])) {
            $qcontact->_addr = Address::fromArray($arr['address']);
        }

        return $qcontact;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
