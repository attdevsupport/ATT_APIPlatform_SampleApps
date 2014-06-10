<?php
namespace Att\Api\Util;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Utility functions 
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
 * @category  Utility
 * @package   Util
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

use finfo;
use InvalidArgumentException;

/**
 * Utility class with static helper methods.
 * 
 * @category Utility
 * @package  Util
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
final class Util
{
    /**
     * Used to generate file MIME types.
     */
    private static $_fileInfo = null;

    /**
     * Disallow instances.
     */
    private function __construct() 
    {
        /* empty */
    }

    /**
     * Gets the server's current time. NOTE: For accurate results, the 
     * 'date.timezone' configuration setting should be set to the server's
     * time zone.
     *
     * @link http://php.net/manual/en/datetime.configuration.php
     * @return string current server's time
     */
    public static function getServerTime() 
    {
        if (!ini_get('date.timezone'))
            date_default_timezone_set('UTC');

        return date('D, F j, Y G:i:s T'); 
    }

    /**
     * Gets a file's MIME type. 
     *
     * @param string $fname file name for which to get MIME type
     *
     * @return void
     */
    public static function getFileMIMEType($fname) 
    {
        // TODO: Move to file util


        // lazy init
        if (self::$_fileInfo == null) {
            self::$_fileInfo = new finfo(FILEINFO_MIME);
        }

        return self::$_fileInfo->file($fname);
    }
    
    /**
     * Given an address string, this method converts that string to an array
     * of 'acceptable' strings that can be used by ATT's API.
     *
     * @param string $addrStr address string
     * 
     * @return array an array of address strings
     * @throws InvalidArgumentException if address string contains invalid 
     * addresses.
     */
    public static function convertAddresses($addrStr) 
    {
        /* TODO: Clean this up */

        $addresses = explode(',', $addrStr);
        $encodedAddr = array(); 
        foreach ($addresses as $addr) {
            $cleanAddr = str_replace('-', '', $addr);
            $cleanAddr = str_replace('tel:', '', $cleanAddr);
            $cleanAddr = str_replace('+1', '', $cleanAddr);
            if (preg_match("/\d{10}/", $cleanAddr)) {
                $encodedAddr[] = 'tel:' . $cleanAddr;
            } else if (preg_match("/^[^@]*@[^@]*\.[^@]*$/", $cleanAddr)) {
                $encodedAddr[] = $cleanAddr;
            } else if (preg_match("/\d[3-8]/", $cleanAddr)) {
                $encodedAddr[] = 'short:' . $cleanAddr;
            } else {
                throw new InvalidArgumentException('Invalid address: ' . $addr);
            }
        }

        return $encodedAddr;
    }

}
?>
