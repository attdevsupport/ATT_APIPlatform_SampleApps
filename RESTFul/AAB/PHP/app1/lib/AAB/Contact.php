<?php
namespace Att\Api\AAB;

require_once __DIR__ . '/ContactCommon.php';

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Address Book Library
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
 * Immutable class used to contain an Address Book contact.
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
final class Contact
{

    /**
     * Common contact information.
     *
     * @var ContactCommon
     */
    private $_contactCommon;

    /**
     * Contact's id.
     *
     * @var string
     */
    private $_contactId;

    /**
     * Contact's creation timestamp as a string.
     *
     * @string
     */
    private $_creationDate;

    /**
     * Contact's modification timestamp as a string.
     *
     * @string
     */
    private $_modificationDate;

    /**
     * Contact's formatted name.
     *
     * @var string
     */
    private $_formattedName;

    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
        $this->_contactId = null;
        $this->_creationDate = null;
        $this->_modificationDate = null;
        $this->_formattedName = null;
    }

    /**
     * Gets the contact id.
     *
     * @return string contact id
     */
    public function getContactId()
    {
        return $this->_contactId;
    }

    /**
     * Gets the contact creation timestamp as a string.
     *
     * @return string contact creation timestamp as a string.
     */
    public function getCreationDate()
    {
        return $this->_creationDate;
    }

    /**
     * Gets the contact modification timestamp as a string.
     *
     * @return string contact modification timestamp as a string
     */
    public function getModificationDate()
    {
        return $this->_modificationDate;
    }

    /**
     * Gets the contact formatted name.
     *
     * @return string contact formatted name
     */
    public function getFormattedName()
    {
        return $this->_formattedName;
    }

    /**
     * Gets the contact's first name, if any.
     *
     * @return string|null contact's first name
     */
    public function getFirstName()
    {
        return $this->_contactCommon->firstName;
    }

    /**
     * Gets the contact's middle name, if any.
     *
     * @return string|null contact's middle name
     */
    public function getMiddleName()
    {
        return $this->_contactCommon->middleName;
    }

    /**
     * Gets the contact's last name, if any.
     *
     * @return string|null contact's last name
     */
    public function getLastName()
    {
        return $this->_contactCommon->lastName;
    }

    /**
     * Gets the contact's prefix, if any.
     *
     * @return string|null contact's prefix
     */
    public function getPrefix()
    {
        return $this->_contactCommon->prefix;
    }

    /**
     * Gets the contact's suffix, if any.
     *
     * @return string|null contact's suffix
     */
    public function getSuffix()
    {
        return $this->_contactCommon->suffix;
    }

    /**
     * Gets the contact's nickname, if any.
     *
     * @return string|null contact's nickname
     */
    public function getNickname()
    {
        return $this->_contactCommon->nickname;
    }

    /**
     * Gets the contact's organization, if any.
     *
     * @return string|null contact's organization
     */
    public function getOrganization()
    {
        return $this->_contactCommon->organization;
    }

    /**
     * Gets the contact's job title, if any.
     *
     * @return string|null contact's job title
     */
    public function getJobTitle()
    {
        return $this->_contactCommon->jobTitle;
    }

    /**
     * Gets the contact's anniversary date (format: mm/dd/yyyy), if any.
     *
     * @return string|null contact's anniversary date
     */
    public function getAnniversary()
    {
        return $this->_contactCommon->anniversary;
    }

    /**
     * Gets the contact's gender, if any.
     *
     * @return string|null contact's gender
     */
    public function getGender()
    {
        return $this->_contactCommon->gender;
    }

    /**
     * Gets the contact's spouse, if any.
     *
     * @return string|null contact's spouse
     */
    public function getSpouse()
    {
        return $this->_contactCommon->spouse;
    }

    /**
     * Gets the contact's children as a comma seperated string, if any.
     *
     * @return string|null contact's children
     */
    public function getChildren()
    {
        return $this->_contactCommon->children;
    }

    /**
     * Gets the contact's hobbies as a comma seperated string, if any.
     *
     * @return string|null contact's hobbies
     */
    public function getHobby()
    {
        return $this->_contactCommon->hobby;
    }

    /**
     * Gets the contact's assistants as a comma seperated string, if any.
     *
     * @return string|null contact's assistants
     */
    public function getAssistant()
    {
        return $this->_contactCommon->assistant;
    }

    /**
     * Gets the contact's phones.
     *
     * @return array contact's phones
     */
    public function getPhones()
    {
        return $this->_contactCommon->phones;
    }

    /**
     * Gets the contact's addresses.
     *
     * @return array contact's addresses
     */
    public function getAddresses()
    {
        return $this->_contactCommon->addresses;
    }

    /**
     * Gets the contact's emails.
     *
     * @return array contact's emails
     */
    public function getEmails()
    {
        return $this->_contactCommon->emails;
    }

    /**
     * Gets the contact's instance message addresses (ims).
     *
     * @return array contact's ims
     */
    public function getIms()
    {
        return $this->_contactCommon->ims;
    }

    /**
     * Gets the contact's web urls.
     *
     * @return array contact's web urls
     */
    public function getWeburls()
    {
        return $this->_contactCommon->weburls;
    }

    /**
     * Gets the Contact object as an array.
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
     * Factory method for creating a Contact object from an array.
     *
     * @param array $arr array to construct a Contact object from
     * @return Contact Contact response object
     */
    public static function fromArray($arr)
    {
        $contact = new Contact();

        $contact->_contactCommon = ContactCommon::fromArray($arr);

        $contact->_contactId =
            isset($arr['contactId']) ? $arr['contactId'] : null;

        $contact->_creationDate =
            isset($arr['creationDate']) ? $arr['creationDate'] : null;

        $contact->_modificationDate =
            isset($arr['modificationDate']) ? $arr['modificationDate'] : null;

        $contact->_formattedName =
            isset($arr['formattedName']) ? $arr['formattedName'] : null;

        return $contact;
    }
}

?>
