<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library
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
 * @category Authentication 
 * @package OAuth
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/OAuthCode.php';
require_once __DIR__ . '/OAuthException.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting OAuth codes,
 * which can then be used to request an OAuth access token.
 *
 * @see https://tools.ietf.org/html/rfc6749
 * 
 * @package OAuth 
 */
class OAuthCodeRequest extends RESTFulRequest {
    private $_URL;
    private $_clientId;
    private $_scope;
    private $_redirectURI;
    private $_params;

    /**
     * Creates an OAuthCodeRequest object for requesting authorization codes. 
     * These codes may then be used to request access tokens.
     *
     * @param $URL string URL from which to request code
     * @param $clientId client id to use when requesting code
     * @param $scope string scope to use
     * @param $redirectURI string redirect URI to use
     */
    public function __construct($URL, $clientId, $scope = NULL, 
            $redirectURI = NULL) {

        $this->_URL = $URL;
        $this->_clientId = $clientId;
        $this->_scope = $scope;
        $this->_redirectURI = $redirectURI;
        $this->_params = array();
    }

    /**
     * Returns the location from which to redirect the browser in order to get
     * an authorization code
     *
     * @return string redirect location
     */
    public function getCodeLocation() {
        $location = $this->_URL . 
            "?client_id=" . urlencode($this->_clientId) .
            "&scope=" . urlencode($this->_scope) .
            "&redirect_uri=" . urlencode($this->_redirectURI);
        return $location;
    }

    /**
     * Used to get an authorization code. First checks request parameters for
     * code, and if not found, redirects browser to get authorization code.
     *
     * @return OAuthCode authorization code if in request parameters
     * @throws OAuthException if there was an error getting code
     */
    public function getCode() {
        // check for code in request params
        if(isset($_REQUEST['code'])) { 
            return new OAuthCode($_REQUEST['code']);
        }

        // check for error/error description in request params
        if (isset($_REQUEST['error'])) {
            $error = $_REQUEST['error'];
            $errorDesc = $_REQUEST['error_description'];
            throw new OAuthException($error, $errorDesc);
        }

        // no code and no error/error description
        // therefore, redirect to get code
        $location = $this->getCodeLocation();

        header("Location: $location");
    }
}
