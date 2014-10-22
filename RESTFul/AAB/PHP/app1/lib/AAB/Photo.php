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
 * Immutable class used to contain photo information.
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
final class Photo
{
    /**
     * Photo encoding.
     *
     * @var string
     */
    private $_encoding;

    /**
     * Photo value.
     *
     * @var string
     */
    private $_value;

    /**
     * Creates a Photo object.
     *
     * @param string $value    photo value
     * @param string $encoding photo encoding
     */
    public function __construct($value, $encoding='BASE64')
    {
        $this->_encoding = $encoding;
        $this->_value = $value;
    }

    /**
     * Gets photo encoding.
     *
     * @return string photo encoding
     */
    public function getEncoding()
    {
        return $this->_encoding;
    }

    /**
     * Gets photo value.
     *
     * @return string photo value
     */
    public function getValue()
    {
        return $this->_value;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
