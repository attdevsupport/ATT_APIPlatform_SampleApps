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
 * Contains <i>Gender</i> constants.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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
