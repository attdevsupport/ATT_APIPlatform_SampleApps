/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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
 * can be used for accessing protected resources.
 *
 * <p>
 * This class also offers convenience methods for checking whether the token is
 * expired, and saving/loading token from file in an asynchronous-safe manner.
 * </p>
 *
 * An example of usage can be found below:
 * <pre>
 * <code>
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
 * </code>
 * </pre>
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 * @see <a href="https://tools.ietf.org/html/rfc6749">OAuth 2.0 Framework</a>
 */
public class OAuthToken {

    /** Static synchronization object. */
    private final static Object LOCK_OBJECT = new Object();

    /** Cache tokens loaded from file to speed up load times. */
    private static HashMap<String, OAuthToken> cachedTokens = null;

    /** Access token. */
    private final String accessToken;

    /** Unix timestamp, in seconds, to denote access token expiry. */
    private final long accessTokenExpiry;

    /** Refresh token. */
    private final String refreshToken;

    /** Whether to cache OAuth tokens, thereby saving file IO. */
    private static volatile boolean useCaching = true;

    /** Used to indicate access token does not expire. */
    public static final long NO_EXPIRATION = -1;

    /**
     * Gets the current time as a Unix timestamp.
     *
     * @return seconds since Unix epoch
     */
    private static long xtimestamp() {
        return System.currentTimeMillis() / 1000;
    }

    /**
     * Creates an OAuthToken object with the specified parameters.
     *
     * <p>
     * <strong>NOTE:</strong> To make an access token never expire, set the
     * <code>expiresIn</code> parameter to <code>OAuthToken.NO_EXPIRATION</code>
     * </p>
     *
     * @param accessToken access token
     * @param expiresIn time in seconds token expires since
     *                  <code>creationTime</code>
     * @param refreshToken refresh token
     * @param creationTime access token creation time as a Unix timestamp
     */
    public OAuthToken(String accessToken, long expiresIn, String refreshToken,
            long creationTime) {

        if (expiresIn == OAuthToken.NO_EXPIRATION) {
            this.accessTokenExpiry = OAuthToken.NO_EXPIRATION;
        } else {
            this.accessTokenExpiry = expiresIn + creationTime;
        }

        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
    }

    /**
     * Creates an OAuthToken object with the <code>creationTime</code> set to
     * the current time.
     *
     * @param accessToken access token
     * @param expiresIn time in seconds token expires from current time
     * @param refreshToken refresh token
     * @see #OAuthToken(String, long, String, long)
     */
    public OAuthToken(String accessToken, long expiresIn, String refreshToken) {
        this(accessToken, expiresIn, refreshToken, xtimestamp());
    }

    /**
     * Gets whether the access token is expired.
     *
     * @return <tt>true</tt> if access token is expired, <tt>false</tt>
     *         otherwise
     */
    public boolean isAccessTokenExpired() {
        return accessTokenExpiry != NO_EXPIRATION
            && xtimestamp() >= accessTokenExpiry;
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
     * @throws IOException if unable to save token
     */
    public void saveToken(String fpath) throws IOException {
        FileOutputStream fOutputStream = null;
        FileLock fLock = null;

        // save to cached tokens
        synchronized (LOCK_OBJECT) {
            // lazy init
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
            props.setProperty("accessTokenExpiry", String.valueOf(accessTokenExpiry));
            props.setProperty("refreshToken", refreshToken);
            props.store(fOutputStream, "Token Information");
        } catch (IOException e) {
            throw e; // pass along exception
        } finally {
            if (fLock != null) { fLock.release(); }
            if (fOutputStream != null) { fOutputStream.close(); }
        }
    }

    /**
     * Attempts to load an OAuthToken from a file in an asynchronous-safe
     * manner.
     *
     * <p>
     * If <code>fpath</code> does not exist, null is returned.
     * </p>
     *
     * <p>
     * <strong>WARNING</strong>: Because caching may be used, manually modifying
     * the saved token properties file may yield unexpected results unless
     * caching is disabled.
     * </p>
     *
     * @param fpath file path from which to load token
     * @return OAuthToken an OAuthToken object if successful, null otherwise
     * @throws IOException if there was an error loading the token
     * @see #useTokenCaching(boolean)
     */
    public static OAuthToken loadToken(String fpath) throws IOException {
        FileInputStream fInputStream = null;
        FileLock fLock = null;

        // attempt to load from cached tokens, thereby saving file I/O
        synchronized (LOCK_OBJECT) {
            if (cachedTokens != null && cachedTokens.get(fpath) != null) {
                return cachedTokens.get(fpath);
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
            if (accessToken == null || accessToken.equals("")) {
                return null;
            }
            String sExpiry = props.getProperty("accessTokenExpiry", "0");
            long expiry = new Long(sExpiry).longValue();
            String refreshToken = props.getProperty("refreshToken");
            return new OAuthToken(accessToken, expiry, refreshToken);
        } catch (IOException e) {
            throw e; // pass along exception
        } finally {
            if (fLock != null) { fLock.release(); }
            if (fInputStream != null) { fInputStream.close(); }
        }
    }

    /**
     * Not yet implemented.
     *
     * @param useCaching not yet implemented
     */
    public static void useTokenCaching(boolean useCaching) {
        // TODO (pk9069): Implement
        throw new UnsupportedOperationException();
    }
}
