<?php
namespace Att\Api\Util;

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

use finfo;
use InvalidArgumentException;

/**
 * Utility class with static helper methods.
 * 
 * @category Utility
 * @package  Util
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

    /* TODO: document */
    public static function convertNulls($arr, $replaceChar='-')
    {
        for ($i = 0; $i < count($arr); ++$i) {
            if ($arr[$i] === null) {
                $arr[$i] = $replaceChar;
            }
        }
        return $arr;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
