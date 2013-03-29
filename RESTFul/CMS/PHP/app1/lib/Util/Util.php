<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Utility functions 
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
 * @category Utility
 * @package Util
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

/**
 * Utility class with static helper methods.
 * 
 * @package Util
 */
class Util {
    private static $fileInfo = NULL;

    // disallow instances
    private function __construct() {
    }

    /**
     * Gets the server's current time. NOTE: For accurate results, the 
     * 'date.timezone' configuration setting should be set to the server's
     * time zone.
     *
     * @see http://php.net/manual/en/datetime.configuration.php
     * @return string current server's time
     */
    public static function getServerTime() {
        return date('D, F j, Y G:i:s T'); 
    }

    /**
     * Gets a file's MIME type. 
     *
     * @param $fname string file name for which to get MIME type
     */
    public static function getFileMIMEType($fname) {
        if (self::$fileInfo == NULL) {
            self::$fileInfo = new finfo(FILEINFO_MIME);
        }

        return self::$fileInfo->file($fname);
    }
    
    /**
     * Given an address string, this method converts that string to an array
     * of 'acceptable' strings that can be used by ATT's API.
     *
     * @param $addrStr string address string
     * @return array an array of address strings
     * @throws InvalidArgumentException if address string contains invalid 
     * addresses.
     */
    public static function convertAddresses($addrStr) {
        $addresses = explode(',', $addrStr);
        $encodedAddr = array(); 
        foreach ($addresses as $addr) {
            $cleanAddr = str_replace('-', '', $addr);
            $cleanAddr = str_replace('tel:', '', $cleanAddr);
            $cleanAddr = str_replace('+1', '', $cleanAddr);
            if (preg_match("/\d{10}/",$cleanAddr)) {
                $encodedAddr[] = 'tel:' . $cleanAddr;
            } else if (preg_match("/^[^@]*@[^@]*\.[^@]*$/", $cleanAddr)) {
                $encodedAddr[] = $cleanAddr;
            } else if (preg_match("/\d[3-8]/", $cleanAddr)){
                $encodedAddr[] = 'short:' . $cleanAddr;
            } else {
                throw new InvalidArgumentException('Invalid address: ' . $addr);
            }
        }

        return $encodedAddr;
    }
}
?>
