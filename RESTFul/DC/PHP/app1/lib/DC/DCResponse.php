<?php
namespace Att\Api\DC;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * DC Library
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
 * @package   DC 
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '/DeviceCapabilities.php';

/**
 * Immutable class used to hold a device information response.
 *
 * @category API
 * @package  DC
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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

?>
