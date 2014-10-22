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

require_once __DIR__ . '/Group.php';

/**
 * Immutable class used to contain a group result set.
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
final class GroupsResultSet
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
     * Groups
     *
     * @var array
     */
    private $_groups;

    // disallow instances to be created using constructor--use factory method
    // instead
    private function __construct()
    {
        $this->_groups = array();
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
     * Gets groups.
     *
     * @return array groups
     */
    public function getGroups()
    {
        return $this->_groups;
    }

    /**
     * Factory method for creating a GroupResultSet object from an array.
     *
     * @param array $arr array to construct a GroupResultSet object from
     * @return GroupResultSet GroupResultSet response object
     */
    public static function fromArray($arr)
    {
        $resultSet = new GroupsResultSet();

        $rsArr = $arr['resultSet'];

        $resultSet->_totalRecords = $rsArr['totalRecords'];
        $resultSet->_totalPages = $rsArr['totalPages'];
        $resultSet->_currentPageIndex = $rsArr['currentPageIndex'];
        $resultSet->_previousPage = $rsArr['previousPage'];
        $resultSet->_nextPage = $rsArr['nextPage'];

        if (isset($rsArr['groups'])) {
            $groupsArr = $rsArr['groups'];
            $groupsArr = $groupsArr['group'];

            foreach ($groupsArr as $groupArr) {
                $resultSet->_groups[] = Group::fromArray($groupArr);
            }
        }

        return $resultSet;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
