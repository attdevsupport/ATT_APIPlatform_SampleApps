<?php
namespace Att\Api\Util;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Util Library
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

/**
 * Utility class for file handling. 
 * 
 * @category Utility
 * @package  Util
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
final class FileUtil
{

    /** 
     * Gets file size.
     *
     * @param string $location file path
     * 
     * @return int the size of file in bytes, or FALSE in case of an error 
     */
    private static function _fsize($location)
    {
        clearstatcache(); // clear performance buffering for file size
        return filesize($location);
    }


    /**
     * Disallow instances.
     */
    private function __construct()
    { 
    }

    /**
     * Saves an array to the given file path.
     *
     * @param array  $arr   the array to save
     * @param string $fpath file path
     * 
     * @return void
     */
    public static function saveArray($arr, $fpath)
    {
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

    /**
     * Loads an array from the given file path.
     * 
     * This method uses file locks and is therefore synchornization-safe. 
     * This method will block until a file lock is acquired. 
     * 
     * @param string $fpath file path
     *
     * @return array array loaded from file path, or an empty array if fpath 
     * does not exist
     */
    public static function loadArray($fpath)
    {
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

        $content = fread($handle, FileUtil::_fsize($fpath));
        $arr = unserialize($content);
        if (!is_array($arr)) {
            return array();
        }

        flock($handle, LOCK_UN);
        fclose($handle);

        return $arr;
    }

    /**
    * Gets a list of files contained within the specified director. 
    *
    * @param string $dir directory to scan for files.
    *
    * @return array list of files
    */
    public static function getFiles($dir) 
    {
        $allFiles = scandir($dir);
        $files = array();
        // copy all files except directories
        foreach ($allFiles as $fname) {
            if (!is_dir($fname)) {
                $files[] = $fname;
            }
        }

        return $files;
    }
}

?>
