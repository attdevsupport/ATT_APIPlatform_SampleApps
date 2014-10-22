<?php
namespace Att\Api\DC;

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

require_once __DIR__ . '/DeviceCapabilities.php';

/**
 * Immutable class used to hold a device information response.
 *
 * @category API
 * @package  DC
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/2/Device%20Capabilities
 */
final class DCResponse
{
    private $_typeAllocationCode;

    private $_capabilities;

    public function __construct(
        $typeAllocationCode, DeviceCapabilities $capabilities
    ) {
        $this->_typeAllocationCode = $typeAllocationCode;
        $this->_capabilities = $capabilities;
    }

    public function getTypeAllocationCode()
    {
        return $this->_typeAllocationCode;
    }

    public function getCapabilities()
    {
        return $this->_capabilities;
    }

    public static function fromArray($arr)
    {
        $deviceInfo = $arr['DeviceInfo'];
        $deviceId = $deviceInfo['DeviceId'];
        
        $typeAllocationCode = $deviceId['TypeAllocationCode'];
        $capArr = $deviceInfo['Capabilities'];

        $capabilities = DeviceCapabilities::fromArray($capArr);

        return new DCResponse($typeAllocationCode, $capabilities);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
