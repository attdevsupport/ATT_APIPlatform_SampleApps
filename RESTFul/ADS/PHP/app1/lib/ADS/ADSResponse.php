<?php
namespace Att\Api\ADS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * ADS Library
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
 * @category  API
 * @package   ADS 
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to contain an ADSResponse.
 *
 * For a list of response values and their definitions, refer to 
 * {@link https://developer.att.com/docs/apis/rest/1/Advertising}.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
final class ADSResponse
{

    /**
     * Click Url.
     *
     * @var string
     */
    private $_clickUrl;

    /**
     * Type of advertisement.
     *
     * @var string
     */
    private $_type;

    /**
     * Image Url, if any.
     *
     * @var string
     */
    private $_imageUrl;

    /**
     * Track Url, if any.
     *
     * @var string
     */
    private $_trackUrl;

    /**
     * Ad content, if any.
     *
     * @var string
     */
    private $_content;


    /** 
     * Creates an object that encapsulates an ADS response.
     *
     * @param string $clickUrl click url
     * @param string $type     ads type
     * @param string $imageUrl image url, if any
     * @param string $trackUrl track url, if any
     * @param string $content  content, if any
     */
    public function __construct($clickUrl, $type, $imageUrl = null,
        $trackUrl = null, $content = null
    ) {
        $this->_clickUrl = $clickUrl;
        $this->_type = $type;

        $this->_imageUrl = $imageUrl;
        $this->_trackUrl = $trackUrl;
        $this->_content = $content;
    }

    /**
     * Gets the click url.
     *
     * @return string click url
     */
    public function getClickUrl()
    {
        return $this->_clickUrl;
    }

    /**
      * Gets the type of ad.
      *
      * @return string ad type
      */
    public function getAdsType()
    {
        return $this->_type;
    }

    /**
     * Gets the image url, if any.
     *
     * @return string|null image url or null if none
     */
    public function getImageUrl()
    {
        return $this->_imageUrl;
    }

    /**
     * Gets the track url, if any.
     *
     * @return string|null track url or null if none
     */
    public function getTrackUrl()
    {
        return $this->_trackUrl;
    }

    /**
     * Gets content, if any.
     *
     * @return string|null content or null if none
     */
    public function getContent()
    {
        return $this->_content;
    }

    /**
     * Createa an ADSResponse object from the specified array.
     *
     * @param array $arr array to use for creating an ADSResponse object
     * 
     * @return ADSResponse ADSResponse object
     * @throws ServiceException if array contains unexpected values
     */
    public static function fromArray($arr)
    {
        $adsResponse = $arr['AdsResponse'];
        $ads = $adsResponse['Ads'];

        // required response values 
        $required = array('ClickUrl', 'Type');
        foreach ($required as $value) {
            if (!isset($ads[$value])) {
                $msg = "Required field '$value' not set.";
                $httpCode = $result->getResponseCode();
                throw new ServiceException($msg, $httpCode);
            }
        }

        $type = $ads['Type'];
        $clickUrl = $ads['ClickUrl'];

        // optional response values
        $imgUrl = isset($ads['ImageUrl']) ? $ads['ImageUrl'] : null;
        $trackUrl = isset($ads['TrackUrl']) ? $ads['TrackUrl'] : null;
        $content = isset($ads['Content']) ? $ads['Content'] : null;
        
        return new ADSResponse($clickUrl, $type, $imgUrl, $trackUrl, $content);
    }

}

?>
