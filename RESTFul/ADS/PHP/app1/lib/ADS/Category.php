<?php
namespace Att\Api\ADS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * ADS Library
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
 * @package   ADS 
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Contains <i>Category</i> constants.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
final class Category
{
    /**
     * Disallow instances.
     */
    private function __construct() 
    {
    }

    const AUTO = "auto";
    const BUSINESS = "business";
    const FINANCE = "finance";
    const CHAT = "chat";
    const COMMUNITY = "community";
    const SOCIAL = "social";
    const PERSONALS = "personals";
    const COMMUNICATION = "communication";
    const TECHNOLOGY = "technology";
    const GAMES = "games";
    const HEALTH = "health";
    const MEDICAL = "medical";
    const MAPS = "maps";
    const LOCAL = "local";
    const ENTERTAINMENT = "entertainment";
    const MOVIES = "movies";
    const TV = "tv";
    const MUSIC = "music";
    const PHOTOS = "photos";
    const VIDEO = "video";
    const NEWS = "news";
    const WEATHER = "weather";
    const SHOPPING = "shopping";
    const SPORTS = "sports";
    const TOOLS = "tools";
    const TRAVEL = "travel";
    const OTHER = "other";
}
