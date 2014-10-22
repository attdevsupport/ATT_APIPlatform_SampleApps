<?php
namespace Att\Api\SMS;

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
 * Immutable class that holds the status for a single SMS message.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
