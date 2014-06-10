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

final class GroupSearchParams
{
    private $_groupType;
    private $_groupName;

    public function __construct($groupType, $groupName)
    {
        $this->_groupType = $groupType;
        $this->_groupName = $groupName;
    }

    public function getGroupType()
    {
        return $this->_groupType;
    }

    public function getGroupName()
    {
        return $this->_groupName;
    }

    public function toArray()
    {
        $arr = array();

        if ($this->_groupType != null) {
            $arr['groupType'] = $this->_groupType;
        }
        if ($this->_groupName != null) {
            $arr['groupName'] = $this->_groupName;
        } 

        return $arr;
    }

}

final class SearchParameters
{

    public $formattedName = null;
    public $firstName = null;
    public $lastName = null;
    public $nickname = null;
    public $email = null;
    public $phone = null;
    public $organization = null;
    public $addressLineOne = null;
    public $addressLineTwo = null;
    public $city = null;
    public $zipcode = null;
    public $notes = null;

    public function toArray() {
        $arr = array();

        if ($this.formattedName != null) 
            $arr['formattedName'] = $this.formattedName;

        if ($this.firstName != null) 
            $arr['firstName'] = $this.firstName;

        if ($this.lastName != null) 
            $arr['lastName'] = $this.lastName;

        if ($this.nickname != null) 
            $arr['nickname'] = $this.nickname;

        if ($this.email != null) 
            $arr['email'] = $this.email;

        if ($this.phone != null) 
            $arr['phone'] = $this.phone;

        if ($this.organization != null) 
            $arr['organization'] = $this.organization;

        if ($this.addressLineOne != null) 
            $arr['addressLineOne'] = $this.addressLineOne;

        if ($this.addressLineTwo != null) 
            $arr['addressLineTwo'] = $this.addressLineTwo;

        if ($this.city != null) 
            $arr['city'] = $this.city;

        if ($this.zipcode != null) 
            $arr['zipcode'] = $this.zipcode;

        if ($this.notes != null) 
            $arr['notes'] = $this.notes;
            
        return $arr;
    }
}

?>
