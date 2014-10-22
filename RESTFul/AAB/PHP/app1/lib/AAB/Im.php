<?php
namespace Att\Api\AAB;

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
 * Immutable class used to contain Im information.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
final class Im
{

    /**
     * Im type, if any.
     *
     * @var string|null
     */
    private $_type;

    /**
     * Whether Im is preferred, if any.
     *
     * @var boolean|null
     */
    private $_preferred;

    /**
     * Im URI, if any.
     *
     * @var string|null
     */
    private $_imUri;

    /**
     * Creates an Im object.
     *
     * @param string|null $type    Im type or null
     * @param string|null $pref    whether email is preferred or null
     * @param string|null $address Im URI or null
     */
    public function __construct($type, $pref, $uri)
    {
        $this->_type = $type;
        $this->_preferred = $pref;
        $this->_imUri = $uri;
    }

    /**
     * Gets Im type.
     *
     * @return string Im type
     */
    public function getImType()
    {
        return $this->_type;
    }

    /**
     * Gets whether Im is preferred.
     *
     * @return boolean whether Im is preferred
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
     * Gets Im URI.
     *
     * @return string Im URI
     */
    public function getImUri()
    {
        return $this->_imUri;
    }

    /**
     * Converts Im object to an associative array.
     *
     * @return array Im object as an associative array
     */
    public function toArray()
    {
        $arr = array();
        if ($this->getImType() !== null)
            $arr['type'] = $this->getImType();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        if ($this->getImUri() !== null)
            $arr['imUri'] = $this->getImUri();

        return $arr;
    }

    /**
     * Factory method for creating an Im object from an array.
     *
     * @param array $arr an array to create an Im object from
     * @return Im Im response object
     */
    public static function fromArray($arr)
    {
        $type = isset($arr['type']) ? $arr['type'] : null;
        $pref = isset($arr['preferred']) ? $arr['preferred'] : null;
        $uri = isset($arr['imUri']) ? $arr['imUri'] : null;

        return new Im($type, $pref, $uri);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
