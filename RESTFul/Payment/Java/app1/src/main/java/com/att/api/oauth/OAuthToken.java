/*                                                                             
 * ==================================================================== 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools          
 * Agreement.' 2013.                                                            
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:               
 * http://developer.att.com/sdk_agreement/                                      
 *                                                                              
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.              
 * For more information contact developer.support@att.com                       
 * ==================================================================== 
 */  

package com.att.api.oauth;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.channels.FileLock;
import java.util.HashMap;
import java.util.Properties;

/**
 * An immutable OAuthToken object that encapsulates an OAuth 2.0 token, which 
 * can be used for accessing protected resources. This class also offers 
 * convenience methods for checking whether the token is expired, and 
 * saving/loading token from file in an asynchronous-safe manner.
 * <p>
 * An example of usage can be found below:
 * <pre>
 * // declare variables
 * final long expiry = OAuthToken.NO_EXPIRATION;
 * final String accessToken = "12345";
 * final String refreshToken = "12345";
 * OAuthToken token = new OAuthToken(accessToken, expiry, refreshToken);
 *
 * // check if access token is expired
 * if (token.isAccessTokenExpired()) {
 *     System.out.println("Access token is expired!");
 * }
 *
 * // save token
 * token.saveToken("/tmp/token.properties");
 *
 * // load token
 * token = OAuthToken.loadToken("/tmp/token.properties"); 
 *
 * </pre>
 *
 * @version 2.2
 * @since 2.2
 * @see https://tools.ietf.org/html/rfc6749
 */
public class OAuthToken {
    // Allow synchronization
    private final static Object lockObj = new Object();

    // instead of reading from file, cache to speed up loading time
    private static HashMap<String, OAuthToken> cachedTokens = null;

    private final String accessToken;
    private final long accessTokenExpiry;
    private final String refreshToken;

    // indicates this access token does not expire
    public static long NO_EXPIRATION = -1;

    /**
     * Creates an OAuthToken object with the specified access token, access
     * token expiry, and refresh token.
     *
     * @param accessToken access token
     * @param accessTokenExpiry access token expiry
     * @param refreshToken refresh token
     */
    public OAuthToken(final String accessToken, final long accessTokenExpiry,
            final String refreshToken) {

        this.accessToken = accessToken;
        this.accessTokenExpiry = accessTokenExpiry;
        this.refreshToken = refreshToken;
    }

    /**
     * Gets whether the access token is expired.
     *
     * @return true if access token is expired, false otherwise
     */
    public boolean isAccessTokenExpired() {
        if (this.accessTokenExpiry == NO_EXPIRATION) {
            return false;
        }

        long currentTime = System.currentTimeMillis();
        return currentTime >= accessTokenExpiry;
    }

    /**
     * Gets access token.
     *
     * @return access token
     */
    public String getAccessToken() {
        return accessToken;
    }

    /**
     * Gets refresh token.
     *
     * @return refresh token
     */
    public String getRefreshToken() {
        return refreshToken;
    }

    /**
     * Saves this token to a file in an asynchronous-safe manner. 
     *
     * @param fpath file path
     * @throws java.io.IOException if unable to save token
     */
    public void saveToken(String fpath) throws IOException {
        FileOutputStream fOutputStream = null;
        FileLock fLock = null;

        // save to cached tokens
        synchronized (lockObj) {
            if (cachedTokens == null) {
                cachedTokens = new HashMap<String, OAuthToken>();
            }

            OAuthToken.cachedTokens.put(fpath, this);
        }

        try {
            fOutputStream = new FileOutputStream(fpath);
            fLock = fOutputStream.getChannel().lock();
            Properties props = new Properties();
            props.setProperty("accessToken", accessToken);
            props.setProperty("accessTokenExpiry", "" + accessTokenExpiry);
            props.setProperty("refreshToken", refreshToken);
            props.store(fOutputStream, "Token Information");
        } catch (IOException e) {
            throw e; // pass along exception
        } finally {
            // will be called before return/throw
            // attempt to clean up
            if (fLock != null) fLock.release();
            if (fOutputStream != null) fOutputStream.close();
        }
    }

    /**
     * Attempts to loads an OAuthToken from a file in an asynchronous-safe 
     * manner. If specified file path is not found or access token can not be 
     * loaded, null is returned.
     * <p>
     * WARNING: Because caching is used, manually modifying the saved token
     * properties file may yield unexpected results. Therefore, only the
     * token's saveToken() method should be used for modifying the file.
     *
     * @return OAuthToken an OAuthToken object if successful, null otherwise 
     * @throws java.io.IOException if there was an error loading the token
     */
    public static OAuthToken loadToken(String fpath) throws IOException {
        FileInputStream fInputStream = null;
        FileLock fLock = null;

        // attempt to load from cached tokens, thereby saving file I/O
        synchronized (lockObj) {
            OAuthToken cachedToken;
            if (cachedTokens != null
                    && (cachedToken = cachedTokens.get(fpath)) != null) {

                return cachedToken;
            }
        }

        if (!new File(fpath).exists()) {
            return null;
        }

        try {
            fInputStream = new FileInputStream(fpath);
            // acquire shared lock
            fLock = fInputStream.getChannel().lock(0L, Long.MAX_VALUE, true);
            Properties props = new Properties();
            props.load(fInputStream);
            String accessToken = props.getProperty("accessToken");
            if (accessToken == null || accessToken == "") {
                return null;
            }
            String sExpiry = props.getProperty("accessTokenExpiry", "0");
            long expiry = Long.parseLong(sExpiry);
            String refreshToken = props.getProperty("refreshToken");
            return new OAuthToken(accessToken, expiry, refreshToken);
        } catch (IOException e) {
            throw e; // pass along exception
        } finally {
            // will be called before return/throw
            // attempt to clean up
            if (fLock != null) fLock.release();
            if (fInputStream != null) fInputStream.close();
        }
    }
}
