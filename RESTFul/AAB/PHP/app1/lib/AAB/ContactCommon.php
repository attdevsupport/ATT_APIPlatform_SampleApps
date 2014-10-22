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
require_once __DIR__ . '/Email.php';
require_once __DIR__ . '/Im.php';
require_once __DIR__ . '/Phone.php';
require_once __DIR__ . '/Photo.php';
require_once __DIR__ . '/WebUrl.php';

/**
 * Contains common information for Address Book Contacts.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class ContactCommon
{

    /**
     * Contact's first name, if any.
     *
     * @var string|null
     */
    public $firstName;

    /**
     * Contact's middle name, if any.
     *
     * @var string|null
     */
    public $middleName;

    /**
     * Contact's last name.
     *
     * @var string|null
     */
    public $lastName;

    /**
     * Contact's prefix, if any.
     *
     * @var string|null
     */
    public $prefix;

    /**
     * Contact's suffix, if any.
     *
     * @var string|null
     */
    public $suffix;

    /**
     * Contact's nickname, if any.
     *
     * @var string|null
     */
    public $nickname;

    /**
     * Contact's organization, if any.
     *
     * @var string|null
     */
    public $organization;

    /**
     * Contact's job title, if any.
     *
     * @var string|null
     */
    public $jobTitle;

    /**
     * Contact's anniversary, if any.
     *
     * @var string|null
     */
    public $anniversary;

    /**
     * Contact's gender, if any.
     *
     * @var string|null
     */
    public $gender;

    /**
     * Contact's spouse, if any.
     *
     * @var string|null
     */
    public $spouse;

    /**
     * Contact's children as a comma seperated string, if any.
     *
     * @var string|null
     */
    public $children;

    /**
     * Contact hobbies as a comma seperated string, if any.
     *
     * @var string|null
     */
    public $hobby;

    /**
     * Contact's assistants as a comma seperated string, if any
     *
     * @var string|null
     */
    public $assistant;

    /**
     * Contact's phones.
     *
     * @var array
     */
    public $phones;

    /**
     * Contact's addresses.
     *
     * @var array
     */
    public $addresses;

    /**
     * Contact's emails.
     *
     * @var array
     */
    public $emails;

    /**
     * Contact's instance message addresses (ims).
     *
     * @var array
     */
    public $ims;

    /**
     * Contact's web urls.
     *
     * @var array
     */
    public $weburls;

    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
        $this->firstName = null;
        $this->middleName = null;
        $this->lastName = null;
        $this->prefix = null;
        $this->suffix = null;
        $this->nickname = null;
        $this->organization = null;
        $this->jobTitle = null;
        $this->anniversary = null;
        $this->gender = null;
        $this->spouse = null;
        $this->children = null;
        $this->hobby = null;
        $this->assistant = null;
        $this->phones = array();
        $this->addresses = array();
        $this->emails = array();
        $this->ims = array();
        $this->weburls = array();
    }

    /**
     * Gets the ContactCommon object as an array.
     *
     * @return array contact object as array
     */
    public function toArray()
    {
        $arr = array();

        $mappings = array(
            'firstName' => &$this->firstName,
            'lastName' => &$this->lastName,
            'prefix' => &$this->prefix,
            'suffix' => &$this->suffix,
            'nickName' => &$this->nickname,
            'organization' => &$this->organization,
            'jobTitle' => &$this->jobTitle,
            'anniversary' => &$this->anniversary,
            'gender' => &$this->gender,
            'spouse' => &$this->spouse,
            'hobby' => &$this->hobby,
            'assistant' => &$this->assistant,
        );

        foreach ($mappings as $k => $v) {
            if ($v !== null) {
                $arr[$k] = $v;
            }
        }

        if (!empty($this->phones)) {
            $phonesArr = array();
            foreach ($this->phones as $phone) {
                $phonesArr[] = $phone->toArray();
            }
            $phoneArr = array('phone' => $phonesArr);
            $arr['phones'] = $phoneArr;
        }
        if (!empty($this->addresses)) {
            $addressesArr = array();
            foreach ($this->addresses as $address) {
                $addressesArr[] = $address->toArray();
            }
            $addressArr = array('address' => $addressesArr);
            $arr['addresses'] = $addressArr;
        }
        if (!empty($this->emails)) {
            $emailsArr = array();
            foreach ($this->emails as $email) {
                $emailsArr[] = $email->toArray();
            }
            $emailArr = array('email' => $emailsArr);
            $arr['emails'] = $emailArr;
        }
        if (!empty($this->ims)) {
            $imsArr = array();
            foreach ($this->ims as $im) {
                $imsArr[] = $im->toArray();
            }
            $imArr = array('im' => $imsArr);
            $arr['ims'] = $imArr;
        }
        if (!empty($this->weburls)) {
            $weburlsArr = array();
            foreach ($this->weburls as $weburl) {
                $weburlsArr[] = $weburl->toArray();
            }
            $weburlArr = array('webUrl' => $weburlsArr);
            $arr['weburls'] = $weburlArr;
        }

        return $arr;
    }

    /**
     * Factory method for creating a ContactCommon object from an array.
     *
     * @param array $arr array to construct a Contact object from
     * @return Contact Contact response object
     */
    public static function fromArray($contactArr)
    {
        $contact = new ContactCommon();

        $mappings = array(
            'firstName' => &$contact->firstName,
            'lastName' => &$contact->lastName,
            'prefix' => &$contact->prefix,
            'suffix' => &$contact->suffix,
            'nickName' => &$contact->nickname,
            'organization' => &$contact->organization,
            'jobTitle' => &$contact->jobTitle,
            'anniversary' => &$contact->anniversary,
            'gender' => &$contact->gender,
            'spouse' => &$contact->spouse,
            'hobby' => &$contact->hobby,
            'assistant' => &$contact->assistant,
        );

        foreach ($mappings as $k => $v) {
            if (isset($contactArr[$k])) {
                $mappings[$k] = $contactArr[$k];
            }
        }

        // TODO: look into cleaning up/refactoring code
        if (isset($contactArr['phones'])) {
            $phonesObj = $contactArr['phones'];

            foreach ($phonesObj['phone'] as $phoneArr) {
                $contact->phones[] = Phone::fromArray($phoneArr);
            }
        }
        if (isset($contactArr['addresses'])) {
            $addressesObj = $contactArr['addresses'];
            foreach ($addressesObj['address'] as $addressArr) {
                $contact->addresses[] = Address::fromArray($addressArr);
            }
        }
        if (isset($contactArr['emails'])) {
            $emailsObj = $contactArr['emails'];
            foreach ($emailsObj['email'] as $emailArr) {
                $contact->emails[] = Email::fromArray($emailArr);
            }
        }
        if (isset($contactArr['ims'])) {
            $imsObj = $contactArr['ims'];
            if (isset($imsObj['im'])) { 
                foreach ($imsObj['im'] as $imArr) {
                    $contact->ims[] = Im::fromArray($imArr);
                }
            } else {
                foreach ($imsObj['IM'] as $imArr) {
                    $contact->ims[] = Im::fromArray($imArr);
                }
            }
        }
        if (isset($contactArr['weburls'])) {
            $weburlsObj = $contactArr['weburls'];

            foreach ($weburlsObj['webUrl'] as $weburlArr) {
                $contact->weburls[] = WebUrl::fromArray($weburlArr);
            }
        }

        return $contact;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
