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
 * Contains <i>Gender</i> constants.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
final class Gender
{

    /**
     * Disallow instances.
     */
    private function __construct() 
    {
    }

    const MALE = 'M';
    const FEMALE = 'F';
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
