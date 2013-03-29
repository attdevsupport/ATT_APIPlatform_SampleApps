<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Util Library
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
 * @category Util 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

class FileUtil {
    /** 
     * Gets file size.
     *
     * @param $location string file path
     * @return int the size of file in bytes, or FALSE in case of an error 
     */
    private static function fsize($location) {
        clearstatcache(); // clear performance buffering for file size
        return filesize($location);
    }

    public static function saveArray($arr, $fpath) {
        $handle = fopen($fpath, 'w');

        if (!$handle) {
            throw new Exception('Unable to open file: '. $fpath);
        }
        if (!flock($handle, LOCK_EX)) {
            throw new Exception('Unable to get lock on ' . $fpath);
        }

        fwrite($handle, serialize($arr)); 

        flock($handle, LOCK_UN);
        fclose($handle);
    }

    public static function loadArray($fpath) {
        if (!file_exists($fpath)) {
            return array();
        }

        $handle = fopen($fpath, 'r');
        if (!$handle) {
            throw Exception('Unable to open file: '. $fpath);
        }
        if (!flock($handle, LOCK_SH)) { 
            throw Exception('Unable to get lock on ' . $fpath);
        }

        $content = fread($handle, FileUtil::fsize($fpath));
        $arr = unserialize($content);
        if (!is_array($arr)) {
            return array();
        }

        flock($handle, LOCK_UN);
        fclose($handle);

        return $arr;
    }
}

?>
