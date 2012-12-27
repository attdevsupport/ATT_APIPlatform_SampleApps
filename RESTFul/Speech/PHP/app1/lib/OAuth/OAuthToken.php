<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * OAuth Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2012. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2012 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category Authentication 
 * @package OAuth
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

/**
 * Immutable class used to hold OAuth token information. This information 
 * includes:
 * <ul>
 * <li>Access Token
 * <li>Time Access Token Expires (in milliseconds)
 * <li>Refresh Token
 * </ul>
 *
 * @package OAuth 
 */
class OAuthToken { 
    // access token
    private $_accessToken;

    // expiration time of access token
    private $_accessTokenExpiry;

    // refresh token used access token has expired
    private $_refreshToken;

    // used to indicate an access token has no expiration
    const NO_EXPIRATION = 0;

    /**
     * Creates an OAuthToken object with the specified parameters. The 
     * expires_in parameter should be set to when the access token expires and 
     * should be set in milliseconds since the Unix Epoch 
     * (January 1 1970 00:00:00 GMT).
     * <p>
     * Note: To make the access token never expire, set the accessTokenExpiry
     * to OAuthToken::NO_EXPIRATION.
     * 
     * @param string $accessToken access token
     * @param string $accessTokenExpiry expiration of access token
     * @param string $refreshToken refresh token
     */
    public function __construct($accessToken, $accessTokenExpiry,
            $refreshToken) {

        $this->_accessToken = $accessToken;
        $this->_accessTokenExpiry = $accessTokenExpiry;
        $this->_refreshToken = $refreshToken;
    }

    /**
     * Gets the access token.
     * 
     * @return string access token
     */
    public function getAccessToken() {
        return $this->_accessToken;
    }

    /**
     * Gets whether the access token has expired. This function uses the 
     * expires_in parameter in order to calculate when the access token 
     * expires. Note: This function uses the system's current time, which may 
     * not be totally accurate. Furthermore, the server does not have to honor 
     * the time set in expires_in (the access token could potentially be valid
     * even after it's expired according to expires_in). Therefore, an 
     * alternative approach would be to wait until the server returns a 401 
     * error with the error_description set to 'token expired' before 
     * attempting to refresh token.
     * 
     * @return boolean true if access token is expired, false otherwise
     */
    public function isAcccessTokenExpired() {
        if ($this->_accessTokenExpiry == NO_EXPIRATION) {
            return false;
        }

        $tnow = time() * 1000; // Get the current time in milliseconds

        return $tnow >= $this->_accessTokenExpiry;
    }

    /**
     * Gets the refresh token.
     *
     * @return string refresh token
     */
    public function getRefreshToken() {
        return $this->_refreshToken;
    }

    /**
     * Saves this token to the specified location in a synchronized 
     * (thread-safe) manner by using file locks and will block. 
     * Example $location: /tmp/token.php 
     * <p>
     * Note: For security reasons, it is recommended that the file ending end
     * in '.php' because the saving method prepends and appends the file 
     * content with comment tags. In the event that the file permissions are 
     * too open, the server will interpret the file, thereby preventing an 
     * unauthorized user from gaining access.
     *
     * @param string $location location of file
     * @see http://php.net/manual/en/function.flock.php
     */
    public function saveToken($location) {
        $handle = fopen($location, 'w');
        if (!$handle) {
            throw new RuntimeException('Unable to open file: '. $location);
        }

        if (!flock($handle, LOCK_EX)) {
            throw new RuntimeException('Unable to get lock on ' . $location);
        }

        fwrite($handle, '<?php /*' . "\n");
        fwrite($handle, $this->_accessToken . "\n"); 
        fwrite($handle, $this->_accessTokenExpiry . "\n"); 
        fwrite($handle, $this->_refreshToken . "\n"); 
        fwrite($handle, '*/ ?>' . "\n");

        flock($handle, LOCK_UN);
        fclose($handle);
    }

    /**
     * Attempts to load an access token from the specified location. This method
     * is done in a synchronized (thread-safe) manner by using file locks and
     * will block.
     *
     * @return OAuthToken the token loaded if successful, null otherwise
     * @see http://php.net/manual/en/function.flock.php
     */
    public static function loadToken($location) {
        if (!file_exists($location)) {
            return NULL;
        }

        $handle = fopen($location, 'r');
        if (!$handle) {
            throw RuntimeException('Unable to open file: '. $location);
        }
       
        if (!flock($handle, LOCK_SH)) { 
            throw RuntimeException('Unable to get lock on ' . $location);
        }

        fgets($handle); // ignore php starting comment
        $aToken = rtrim(fgets($handle), "\n"); // access token
        $expiry = rtrim(fgets($handle), "\n"); // access token expiry
        $rToken = rtrim(fgets($handle), "\n"); // refresh token
        // fgets($handle); // ignore php ending comment

        flock($handle, LOCK_UN);
        fclose($handle);

        return new OAuthToken($aToken, $expiry, $rToken);
    }
}


?>
