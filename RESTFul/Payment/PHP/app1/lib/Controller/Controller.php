<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Controller Library
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
 * @category  MVC 
 * @package   Controller 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '../../OAuth/OAuthToken.php';

/**
 * Base class used to implement an MVC controller. 
 * 
 * This implementation is a very rough version of MVC and not a full 
 * implementation. For simplicity and minimization, a full MVC framework is
 * not used. Furthemore, this controller definition is specific to API 
 * controllers and not a general one.
 *
 * @category MVC 
 * @package  Controller 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
abstract class Controller
{

    /**
     * Fully qualified domain name.
     *
     * @var string
     */
    protected $FQDN;

    /**
     * Client id used for authenticaton.
     *
     * @var string
     */
    protected $clientId;
    /**
     * Client secret used for authenticaton.
     *
     * @var string
     */
    protected $clientSecret;

    /**
     * Scope used for authenticaton.
     */
    protected $scope;

    /** 
     * Gets an access token that will be cached using a file. 
     *
     * This method works first trying to load the file specified in config, 
     * and, if a saved OAuth token isn't found, this method will send an API 
     * request. The OAuth token will then be saved for future use.
     *
     * @return OAuthToken OAuth token that can be used for authorization
     * @throws OAuthException if API request was not successful or if 
     *                        there is a file IO issue.  
     */
    protected function getFileToken() 
    {
        // load location where to save token 
        include __DIR__ . '/../../config.php';

        if (!isset($oauth_file)) {
            // set default if can't load
            $oauth_file = 'token.php';
        }

        $token = OAuthToken::loadToken($oauth_file);
        if ($token == null || $token->isAccessTokenExpired()) {
            $tokenSrvc = new OAuthTokenService(
                $this->FQDN, 
                $this->clientId,
                $this->clientSecret
            );
            $token = $tokenSrvc->getTokenUsingScope($this->scope);
            // save token for future use
            $token->saveToken($oauth_file);
        }
    }

    /**
     * Initializes common information for a Controller object.
     */
    protected function __construct() 
    {
        // Copy config values to member variables
        include __DIR__ . '/../../config.php';
        $this->FQDN = $FQDN;
        $this->clientId = $api_key;
        $this->clientSecret = $secret_key;
        $this->scope = $scope;
    }


    /**
     * Gets any API results to be displayed in the view.
     *
     * @return array results or an empty array if no results.
     */
    public abstract function getResults();

    /**
     * Gets any errors that occured as a result of API requests.
     *
     * @return array errors or an empty array if no results.
     */
    public abstract function getErrors();
    
}
?>
