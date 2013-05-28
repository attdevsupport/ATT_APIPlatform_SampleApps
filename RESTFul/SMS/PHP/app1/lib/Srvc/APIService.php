<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Service Library
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
 * @package   Service
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '/Service.php';

/**
 * Base class used to hold common code for sending API requests. 
 *
 * @category API
 * @package  Service
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 * @see      Service
 */
abstract class APIService extends Service
{

    /**
     * OAuth token used for authorization.
     *
     * @var OAuthToken
     */
    protected $token;

    /**
     * Fully qualified domain name to which requests will be sent.
     *
     * @var string
     */
    protected $FQDN;

    /**
     * Creates an APIService object that an be used to interact with
     * APIs.
     *
     * @param string     $FQDN  fully qualified domain name to send requests to
     * @param OAuthToken $token OAuth token used for authorization 
     */
    protected function __construct($FQDN, OAuthToken $token)
    {
        $this->token = $token; 
        $this->FQDN = $FQDN;
    }

}
?>
