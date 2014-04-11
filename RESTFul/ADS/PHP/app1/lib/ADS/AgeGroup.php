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
 * Contains <i>AgeGroup</i> contants.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
final class AgeGroup
{

    /**
     * Disallow instances.
     */
    private function __construct() 
    {
    }

    const AG_14_TO_25 = "14-25";
    const AG_26_TO_35 = "26-35";
    const AG_36_TO_55 = "36-55";
    const AG_56_TO_100 = "56-100";
}

?>
