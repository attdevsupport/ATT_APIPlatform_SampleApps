<?php
namespace Att\Api\SMS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * SMS Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2013. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API
 * @package   SMS 
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class that holds the status for a single SMS message.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class DeliveryInfo
{
    private $_id;

    private $_addr;

    private $_status;

    public function __construct($id, $addr, $status)
    {
        $this->_id = $id;
        $this->_addr = $addr;
        $this->_status = $status;
    }

    public function getId()
    {
        return $this->_id;
    }

    public function getAddress()
    {
        return $this->_addr;
    }

    public function getDeliveryStatus()
    {
        return $this->_status;
    }

    public static function fromArray($arr)
    {
        $id = $arr['Id'];
        $addr = $arr['Address'];
        $ds = $arr['DeliveryStatus'];

        return new DeliveryInfo($id, $addr, $ds);
    }
}

?>
