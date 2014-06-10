<?php
namespace Att\Api\AAB;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * AAB Library
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
 * @package   AAB
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to contain a web URL.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class WebUrl
{
    /* WebUrl Type constants. */
    const TYPE_HOME = 'HOME';
    const TYPE_WORK = 'WORK';
    const TYPE_NO_VALUE = 'NO VALUE';

    /**
     * WebUrl type, if any.
     *
     * @var string|null
     */
    private $_type;

    /**
     * Web URL, if any.
     *
     * @var string|null
     */
    private $_url;

    /**
     * Whether the Web URL is preferred, if set.
     *
     * @var boolean|null
     */
    private $_preferred;

    /**
     * Creates a WebURL object.
     *
     * @param string|null  $type      WebUrl type or null
     * @param string|null  $url       Web URL
     * @param boolean|null $preferred whether email is preferred or null
     */
    public function __construct($type, $url, $preferred)
    {
        $this->_type = $type;
        $this->_url = $url;
        $this->_preferred = $preferred;
    }

    /**
     * Gets WebUrl type.
     *
     * @return string WebUrl type
     */
    public function getWebUrlType()
    {
        return $this->_type;
    }

    /**
     * Gets Web URL.
     *
     * @return string web URL
     */
    public function getUrl()
    {
        return $this->_url;
    }

    /**
     * Gets whether WebUrl is preferred.
     *
     * @return boolean whether WebUrl is preferred
     */
    public function isPreferred()
    {
        if (is_string($this->_preferred)) {
            $lpref = strtolower($this->_preferred);
            return $lpref == 'true';
        }

        return $this->_preferred;
    }

    /**
     * Converts a WebUrl object to an associative array.
     *
     * @return array WebUrl object as an array
     */
    public function toArray()
    {
        $arr = array();
        if ($this->getWebUrlType() !== null)
            $arr['type'] = $this->getWebUrlType();

        if ($this->getUrl() !== null)
            $arr['url'] = $this->getUrl();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        return $arr;
    }

    /**
     * Factory method for creating a WebUrl object from an array.
     *
     * @param array $arr an array to create an WebUrl object from
     * @return WebUrl WebUrl response object
     */
    public static function fromArray($arr)
    {
        $type = $arr['type'];
        $url = $arr['url'];
        $preferred = $arr['preferred'];

        return new WebUrl($type, $url, $preferred);
    }

}

?>
