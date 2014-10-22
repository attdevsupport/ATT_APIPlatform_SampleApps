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

require_once __DIR__ . '/Contact.php';
require_once __DIR__ . '/QuickContact.php';

/**
 * Immutable class used to contain a contact result set.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class ContactsResultSet
{

    /**
     * Total number of contacts.
     *
     * @var int
     */
    private $_totalRecords;

    /**
     * Total number of pages.
     *
     * @var int
     */
    private $_totalPages;

    /**
     * Current page index.
     *
     * @var int
     */
    private $_currentPageIndex;

    /**
     * Previous page.
     *
     * @var int
     */
    private $_previousPage;

    /**
     * Next page.
     *
     * @var int
     */
    private $_nextPage;

    /**
     * Contacts.
     *
     * @var array
     */
    private $_contacts;

    /**
     * Quick contacts.
     *
     * @var array
     */
    private $_quickContacts;

    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
         $this->_contacts = array();
         $this->_quickContacts = array();
    }

    /**
     * Gets the total number of records.
     *
     * @return int total number of records
     */
    public function getTotalRecords()
    {
        return $this->_totalRecords;
    }

    /**
     * Gets the total number of pages.
     *
     * @return int total number of pages
     */
    public function getTotalPages()
    {
        return $this->_totalPages;
    }

    /**
     * Gets the current page index.
     *
     * @return int current page index
     */
    public function getCurrentPageIndex()
    {
        return $this->_currentPageIndex;
    }

    /**
     * Gets the previous page.
     *
     * @return int previous page
     */
    public function getPreviousPage()
    {
        return $this->_previousPage;
    }

    /**
     * Gets the next page.
     *
     * @return int next page
     */
    public function getNextPage()
    {
        return $this->_nextPage;
    }

    /**
     * Gets contacts.
     *
     * @return array contacts
     */
    public function getContacts()
    {
        return $this->_contacts;
    }

    /**
     * Gets quick contacts.
     *
     * @return array quick contacts
     */
    public function getQuickContacts()
    {
        return $this->_quickContacts;
    }

    /**
     * Factory method for creating a ContactResultSet object from an array.
     *
     * @param array $arr array to construct a ContactResultSet object from
     * @return Contact Contact response object
     */
    public static function fromArray($arr)
    {
        $resultSet = new ContactsResultSet();

        $rsArr = $arr['resultSet'];

        $resultSet->_totalRecords = $rsArr['totalRecords'];
        $resultSet->_totalPages = $rsArr['totalPages'];
        $resultSet->_currentPageIndex = $rsArr['currentPageIndex'];
        $resultSet->_previousPage = $rsArr['previousPage'];
        $resultSet->_nextPage = $rsArr['nextPage'];

        if (isset($rsArr['contacts'])) {
            $contactsArr = $rsArr['contacts'];
            $contactsArr = $contactsArr['contact'];

            foreach ($contactsArr as $contactArr) {
                $resultSet->_contacts[] = Contact::fromArray($contactArr);
            }
        }

        if (isset($rsArr['quickContacts'])) {
            $quickContactsArr = $rsArr['quickContacts'];
            $quickContactsArr = $quickContactsArr['quickContact'];

            foreach ($quickContactsArr as $quickContactArr) {
                $resultSet->_quickContacts[] = QuickContact::fromArray($quickContactArr);
            }
        }

        return $resultSet;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
