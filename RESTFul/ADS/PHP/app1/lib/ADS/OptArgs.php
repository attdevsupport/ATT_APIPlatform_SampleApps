<?php
namespace Att\Api\ADS;

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

/**
 * Holds optional arguments that can be passed to the Advertising API.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
final class OptArgs
{

    /**
     * Age group.
     *
     * @var string
     */
    private $_ageGroup;

    /**
     * Area code.
     *
     * @var int
     */
    private $_areaCode;

    /**
     * City.
     *
     * @var string
     */
    private $_city;

    /**
     * Country.
     *
     * @var string
     */
    private $_country;

    /**
     * Gender.
     *
     * @var string
     */
    private $_gender;

    /**
     * Keywords.
     *
     * @var array
     */
    private $_keywords;

    /**
     * Latitude.
     *
     * @var float
     */
    private $_latitude;

    /**
     * Longitude.
     *
     * @var float
     */
    private $_longitude;

    /**
     * Max height.
     *
     * @var
     */
    private $_maxHeight;

    /**
     * Max width.
     *
     * @var int
     */
    private $_maxWidth;

    /**
     * Min height.
     *
     * @var int
     */
    private $_minHeight;

    /**
     * Min width.
     *
     * @var int
     */
    private $_minWidth;

    /**
     * Ad type.
     *
     * @var int
     */
    private $_type;

    /**
     * Zip code.
     *
     * @var int
     */
    private $_zipCode;

    /**
     * Creates an object to hold optional arguments.
     *
     * All of the optional arguments are set to null during object creation. 
     * Any arguments set to null will <i>not<i> be sent to the Advertising API.
     */
    public function __construct()
    {
        $this->_ageGroup = null;
        $this->_areaCode = null;
        $this->_city = null;
        $this->_country = null;
        $this->_gender = null;
        $this->_keywords = null;
        $this->_latitude = null;
        $this->_longitude = null;
        $this->_maxHeight = null;
        $this->_maxWidth = null;
        $this->_minHeight = null;
        $this->_minWidth = null;
        $this->_type = null;
        $this->_zipCode = null;
    }

    /**
     * Gets age group.
     *
     * @return string
     */
    public function getAgeGroup()
    {
        return $this->_ageGroup;
    }

    /**
     * Gets area code.
     *
     * @return int|null
     */
    public function getAreaCode()
    {
        return $this->_areaCode;
    }

    /**
     * Gets city.
     *
     * @return string|null
     */
    public function getCity()
    {
        return $this->_city;
    }

    /**
     * Gets country.
     *
     * @return string|null
     */
    public function getCountry()
    {
        return $this->_country;
    }

    /**
     * Gets gender.
     *
     * @return string|null
     */
    public function getGender()
    {
        return $this->_gender;
    }

    /**
     * Gets keywords.
     *
     * @return array|null
     */
    public function getKeywords()
    {
        return $this->_keywords;
    }

    /**
     * Gets Latitude.
     *
     * @return float|null
     */
    public function getLatitude()
    {
        return $this->_latitude;
    }

    /**
     * Gets longitude.
     *
     * @return float|null
     */
    public function getLongitude()
    {
        return $this->_longitude;
    }

    /**
     * Gets max height.
     *
     * @return int|null
     */
    public function getMaxHeight()
    {
        return $this->_maxHeight;
    }

    /**
     * Gets max width.
     *
     * @return int|null
     */
    public function getMaxWidth()
    {
        return $this->_maxWidth;
    }

    /**
     * Gets min height.
     *
     * @return int|null
     */
    public function getMinHeight()
    {
        return $this->_minHeight;
    }

    /**
     * Gets min width.
     *
     * @return int|null
     */
    public function getMinWidth()
    {
        return $this->_minWidth;
    }

    /**
     * Gets ad type.
     *
     * @return string|null
     */
    public function getAdType()
    {
        return $this->_type;
    }

    /**
     * Gets zip code.
     *
     * @return int|null
     */
    public function getZipCode()
    {
        return $this->_zipCode;
    }

    /**
     * Sets the age group to send to the advertising API.
     * 
     * The AgeGroup class contains constants that can be used to set the age
     * group.
     *
     * @param string $ageGroup age group
     *
     * @see AgeGroup
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setAgeGroup($ageGroup)
    {
        $this->_ageGroup = $ageGroup;
        return $this;
    }

    /**
     * Sets area code to send to the advertising API.
     *
     * @param int $areaCode area code
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setAreaCode($areaCode)
    {
        $this->_areaCode = $areaCode;
        return $this;
    }

    /**
     * Sets the city to send to the advertising API.
     *
     * @param string $city city
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setCity($city)
    {
        $this->_city = $city;
        return $this;
    }

    /**
     * Sets the country to send to the advertising API.
     *
     * @param string $country country
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setCountry($country)
    {
        $this->_country = $country;
        return $this;
    }

    /**
     * Sets the gender to send to the advertising API.
     *
     * The Gender class contains constants that can be used to set the gender.
     *
     * @param string $gender gender
     *
     * @see Gender
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setGender($gender)
    {
        $this->_gender = $gender;
        return $this;
    }

    /**
     * Sets keyword to send to the advertising API.
     *
     * @param array $keywords keywords
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setKeywords($keywords)
    {
        $this->_keywords = $keywords;
        return $this;
    }

    /**
     * Sets latitude to send to the advertising API.
     *
     * @param float $latitude latitude
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setLatitude($latitude)
    {
        $this->_latitude = $latitude;
        return $this;
    }

    /**
     * Sets longitude to send to the advertising API.
     *
     * @param float $longitude longitude
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setLongitude($longitude)
    {
        $this->_longitude = $longitude;
        return $this;
    }

    /**
     * Sets max height to send to the advertising API.
     *
     * @param int $maxHeight max height
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setMaxHeight($maxHeight)
    {
        $this->_maxHeight = $maxHeight;
        return $this;
    }

    /**
     * Sets max width to send to the advertising API.
     *
     * @param int $maxWidth max width
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setMaxWidth($maxWidth)
    {
        $this->_maxWidth = $maxWidth;
        return $this;
    }

    /**
     * Sets min height to send to the advertising API.
     *
     * @param int $minHeight min height
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setMinHeight($minHeight)
    {
        $this->_minHeight = $minHeight;
        return $this;
    }

    /**
     * Sets min width to send to the advertising API.
     *
     * @param int $minWidth min width
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setMinWidth($minWidth)
    {
        $this->_minWidth = $minWidth;
        return $this;
    }

    /**
     * Sets advertising type to send to the advertising API.
     *
     * The AdType class contains constants that can be used to set the ad type.
     *
     * @param string $type advertising type
     *
     * @see AdType
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setAdType($type)
    {
        $this->_type = $type;
        return $this;
    }

    /**
     * Sets zip code to send to the advertising API.
     *
     * @param int $zipCode zip code
     *
     * @return OptArgs a reference to <var>$this</var>
     */
    public function setZipCode($zipCode)
    {
        $this->_zipCode = $zipCode;
        return $this;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
