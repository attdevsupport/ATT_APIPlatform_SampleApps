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
}
?>
