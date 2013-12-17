<?php
namespace Att\Api\TL;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * TL Library
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
 * @package   TL
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to hold a terminal location response.
 *
 * @category API
 * @package  TL
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/2/Location
 */
final class TLResponse 
{
    /**
     * 
     * @var string
     */
    private $_timestamp;

    /**
     * 
     * @var string
     */
    private $_accuracy;

    /**
     * 
     * @var string
     */
    private $_latitude;

    /**
     * 
     * @var string
     */
    private $_longitude;

    /**
     * 
     * @var string
     */
    private $_elapsedTime;

    public function __construct(
        $timestamp, $accuracy, $latitude, $longitude, $elapsedTime
    ) {
        $this->_timestamp = $timestamp;
        $this->_accuracy = $accuracy;
        $this->_latitude = $latitude;
        $this->_longitude = $longitude;
        $this->_elapsedTime = $elapsedTime;
    }

    public function getTimestamp()
    {
        return $this->_timestamp;
    }

    public function getAccuracy()
    {
        return $this->_accuracy;
    }

    public function getLatitude()
    {
        return $this->_latitude;
    }

    public function getLongitude()
    {
        return $this->_longitude;
    }

    public function getElapsedTime()
    {
        return $this->_elapsedTime;
    }

    public static function fromArray($arr)
    {
        // required response values 
        $required = array('timestamp', 'accuracy', 'latitude', 'longitude');
        foreach ($required as $value) {
            if (!isset($arr[$value])) {
                $msg = "Required field '$value' not set.";
                $httpCode = $result->getResponseCode();
                throw new ServiceException($msg, $httpCode);
            }
        }

        $timestamp = $arr['timestamp'];
        $accuracy = $arr['accuracy'];
        $latitude = $arr['latitude'];
        $longitude = $arr['longitude'];
        $elapsedTime = $arr['elapsedTime'];
          
        return new TLResponse(
            $timestamp, $accuracy, $latitude, $longitude, $elapsedTime
        );
    }

}
?>
