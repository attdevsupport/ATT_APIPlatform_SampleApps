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

/**
 * Immutable class used to hold a device capabilities.
 *
 * @category API
 * @package  DC
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/2/Device%20Capabilities
 */
final class DeviceCapabilities
{
   private $_name;
   private $_vendor;
   private $_model;
   private $_firmwareVersion;
   private $_uaProf;
   private $_mmsCapable;
   private $_assistedGps;
   private $_locationTechnology;
   private $_deviceBrowser;
   private $_wapPushCapable;

   /**
    * Allow only the factory method for object creation, at least for now.
    */
   private function __construct()
   {
       // empty 
   }

   public static function fromArray($arr)
   {
       $caps = new DeviceCapabilities();

       $caps->_name = $arr['Name'];
       $caps->_vendor = $arr['Vendor'];
       $caps->_model = $arr['Model'];
       $caps->_firmwareVersion = $arr['FirmwareVersion'];
       $caps->_uaProf = $arr['UaProf'];
       $caps->_mmsCapable = $arr['MmsCapable'] == 'Y' ? true : false;
       $caps->_assistedGps = $arr['AssistedGps'] == 'Y' ? true : false;
       $caps->_locationTechnology = $arr['LocationTechnology'];
       $caps->_deviceBrowser = $arr['DeviceBrowser'];
       $caps->_wapPushCapable = $arr['WapPushCapable'] == 'Y' ? true : false;

       return $caps;
   }

   public function getName() {
       return $this->_name;
   }
   public function getVendor() {
       return $this->_vendor;
   }
   public function getModel() {
       return $this->_model;
   }
   public function getFirmwareVersion() {
       return $this->_firmwareVersion;
   }
   public function getUaProf() {
       return $this->_uaProf;
   }
   public function isMmsCapable() {
       return $this->_mmsCapable;
   }
   public function isAssistedGps() {
       return $this->_assistedGps;
   }
   public function getLocationTechnology() {
       return $this->_locationTechnology;
   }
   public function getDeviceBrowser() {
       return $this->_deviceBrowser;
   }
   public function isWapPushCapable() {
       return $this->_wapPushCapable;
   }
}
