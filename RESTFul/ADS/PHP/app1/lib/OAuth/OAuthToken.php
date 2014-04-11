<?php
namespace Att\Api\OAuth;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * OAuth Library
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
 * @category  Authentication 
 * @package   OAuth
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

use Exception;

/**
 * Immutable class that holds OAuth token information. 
 *
 * This information includes:
 * <ul>
 * <li>Access Token</li>
 * <li>Time Access Token Expires (in seconds)</li>
 * <li>Refresh Token</li>
 * </ul>
 *
 * This class also provides utility methods for saving and loading OAuth
 * tokens.
 *
 * @category Authentication 
 * @package  OAuth 
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
final class OAuthToken
{
    // used to indicate an access token has no expiration
    const NO_EXPIRATION = 0;

    /**
     * Access token.
     *
     * @var string
     */
    private $_accessToken;

    /**
     * Refresh token used when access token has expired.
     *
     * @var string
     */
    private $_refreshToken;

    /**
     * UNIX timestamp to measure when access token expires.
     * 
     * @var float
     */
    private $_accessTokenExpiry;

    /**
     * Creates an OAuthToken object with the specified parameters. 
     * 
     * Note: To make the access token never expire, set the expiresIn to 
     * OAuthToken::NO_EXPIRATION.
     * 
     * @param string $accessToken  access token
     * @param float  $expiresIn    the number of seconds until token 
     *                             expires (relative to token creation 
     *                             time)
     * @param string $refreshToken refresh token
     * @param float  $creationTime creation time of token as measured by 
     *                             the number of seconds since the Unix
     *                             Epoch (January 1 1970 00:00:00 GMT).
     */
    public function __construct(
        $accessToken, $expiresIn, $refreshToken, $creationTime = null
    ) {

        if ($creationTime == null) {
            $creationTime = time();
        }

        $this->_accessToken = $accessToken;
        $this->_refreshToken = $refreshToken;

        if ($expiresIn == OAuthToken::NO_EXPIRATION) {
            $this->_accessTokenExpiry = OAuthToken::NO_EXPIRATION;
        } else {
            $this->_accessTokenExpiry = $expiresIn + $creationTime;
        }
    }

    /**
     * Gets the access token.
     * 
     * @return string access token
     */
    public function getAccessToken()
    {
        return $this->_accessToken;
    }

    /**
     * Gets whether the access token has expired. 
     * 
     * This function uses the <var>$expiresIn</var> and
     * <var>$creationTime</var> parameters to calculate access token expiry.
     * 
     * @return boolean true if access token is expired, false otherwise
     */
    public function isAccessTokenExpired()
    {
        if ($this->_accessTokenExpiry == OAuthToken::NO_EXPIRATION) {
            return false;
        }

        // Get the current time in seconds 
        $tnow = time(); 

        // make the expiration time 20 seconds sooner to account for any 
        // latency issues
        $expiry = $this->_accessTokenExpiry - 20;

        return $tnow >= $expiry;
    }

    /**
     * Gets the refresh token.
     *
     * @return string refresh token
     */
    public function getRefreshToken()
    {
        return $this->_refreshToken;
    }

    /**
     * Saves this token to the specified location in a synchronized manner by 
     * using file locks. 
     
     * This method will block waiting for file lock. 
     * 
     * Note: if the file can not be saved in a secure location, for security 
     * reasons, it is recommended that the file ending end in '.php' because 
     * the saving method prepends and appends the file content with comment 
     * tags. In the event that the file permissions are too open, the server 
     * will interpret the file, thereby preventing an unauthorized user from 
     * gaining access.
     * 
     * @param string $location location of file
     *
     * @return void
     * @link http://php.net/manual/en/function.flock.php
     * @throws Exception if unable to open file or obtain file lock.
     */
    public function saveToken($location)
    {
        // TODO: Better error handling if open or lock fails
        $handle = fopen($location, 'w');
        if (!$handle) {
            throw new Exception('Unable to open file: '. $location);
        }

        if (!flock($handle, LOCK_EX)) {
            fclose($handle);
            throw new Exception('Unable to get lock on ' . $location);
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
     * Attempts to load an access token from the specified location. 
     *
     * Token loading is done in a synchronized manner using file locks. This
     * method will block while waiting for the file lock. 
     *
     * @param string $location file path used to load token
     *
     * @return OAuthToken the token loaded if successful, null otherwise
     * @link http://php.net/manual/en/function.flock.php
     * @throws Exception if unable to open file or obtain file lock.
     */
    public static function loadToken($location)
    {
        if (!file_exists($location)) {
            return null;
        }

        // TODO: Better error handling if open or lock fails

        $handle = fopen($location, 'r');
        if (!$handle) {
            throw Exception('Unable to open file: '. $location);
        }
       
        if (!flock($handle, LOCK_SH)) { 
            fclose($handle);
            throw Exception('Unable to get lock on ' . $location);
        }

        fgets($handle); // ignore php starting comment
        $aToken = rtrim(fgets($handle), "\n"); // access token
        $expiry = rtrim(fgets($handle), "\n"); // access token expiry
        $rToken = rtrim(fgets($handle), "\n"); // refresh token
        // fgets($handle); // ignore php ending comment

        flock($handle, LOCK_UN);
        fclose($handle);

        $token = new OAuthToken($aToken, 0, $rToken);
        $token->_accessTokenExpiry = $expiry;
        return $token;
    }
}


?>
