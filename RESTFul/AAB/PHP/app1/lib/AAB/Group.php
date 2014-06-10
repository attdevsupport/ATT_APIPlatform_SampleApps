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
 * Immutable class used to contain group information.
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
final class Group
{

    /**
     * Group identifier, if any.
     *
     * @var string|null
     */
    private $_groupId;

    /**
     * Group name.
     *
     * @var string
     */
    private $_groupName;

    /**
     * Group type, if any.
     *
     * @var string|null
     */
    private $_groupType;

    /**
     * Creates a Group object.
     *
     * @param string      $name group name
     * @param string|null $id   group id
     * @param string|null $type group type
     */
    public function __construct($name, $id=null, $type=null)
    {
        $this->_groupName = $name;
        $this->_groupId = $id;
        $this->_groupType = $type;
    }

    /**
     * Gets group id, or null if none.
     *
     * @return string|null group id
     */
    public function getGroupId()
    {
        return $this->_groupId;
    }

    /**
     * Gets group name.
     *
     * @return string group name 
     */
    public function getGroupName()
    {
        return $this->_groupName;
    }

    /**
     * Gets group type, or null if none.
     *
     * @return string group type
     */
    public function getGroupType()
    {
        return $this->_groupType;
    }

    /**
     * Converts Group object to an associative array.
     *
     * @return array Group object as an array
     */
    public function toArray()
    {
        $arr = array();
        $groupArr = array();

        $groupArr['groupName'] = $this->_groupName;

        if ($this->_groupId != null) $groupArr['groupId'] = $this->_groupId;
        if ($this->_groupType != null) $groupArr['groupType'] = $this->_groupType;

        $arr['group'] = $groupArr;
        return $arr;
    }

    /**
     * Factory method for creating a Group object from an array.
     *
     * @param array $arr an array to create an Group object from
     * @return Group Group response object
     */
    public static function fromArray($arr)
    {
        $id = $arr['groupId'];
        $name = isset($arr['groupName']) ? $arr['groupName'] : null;
        $type = isset($arr['groupType']) ? $arr['groupType'] : null;

        return new Group($name, $id, $type);
    }

}
