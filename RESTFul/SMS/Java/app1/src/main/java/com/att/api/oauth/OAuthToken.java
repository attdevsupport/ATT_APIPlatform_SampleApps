/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.att.api.oauth;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.channels.FileLock;
import java.util.HashMap;
import java.util.Properties;

import org.json.JSONObject;

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

    /** Seconds at which access token will expire relative to creation.  **/
    private final long expiresIn;

    /** Unix timestamp, in seconds, to denote access token creation date. */
    private final long creationTime;

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
        this.accessToken = accessToken;
        this.expiresIn = expiresIn;
        this.refreshToken = refreshToken;
        this.creationTime = creationTime;
    }

    /**
     * Creates an OAuthToken object with the <code>creationTime</code> set to
     * the current time.
     *
     * @param accessToken access token
     * @param expiresIn time in seconds token expires from current time
     * @param refreshToken refresh token
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
        return this.getAccessTokenExpiry() != NO_EXPIRATION
            && xtimestamp() >= this.getAccessTokenExpiry();
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
     * Get the Unix timestamp, in seconds, that the token will expire
     *
     * @return the accessTokenExpiry
     */
    public long getAccessTokenExpiry() {
        return this.expiresIn == OAuthToken.NO_EXPIRATION 
            ? OAuthToken.NO_EXPIRATION : this.expiresIn + this.creationTime;
    }

    /**
     * Get the number of seconds that the token will expire relative to
     * creationTime.
     *
     * @return the expiresIn
     */
    public long getExpiresIn() {
        return expiresIn;
    }

    /**
     * Get the Unix timestamp, in seconds, that the token was created.
     *
     * @return the creationTime
     */
    public long getCreationTime() {
        return creationTime;
    }

    /**
     * Gets refresh token.
     *
     * @return refresh token
     */
    public String getRefreshToken() {
        return refreshToken;
    }


    public static OAuthToken valueOf(JSONObject jobj) {
        final String accessToken = jobj.getString("access_token");
        final String refreshToken = jobj.getString("refresh_token");
        long expiresIn = jobj.getLong("expires_in");

        // 0 indicates no expiry
        if (expiresIn == 0) {
            expiresIn = OAuthToken.NO_EXPIRATION;
        }

        return new OAuthToken(accessToken, expiresIn, refreshToken);
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

            try {
                fOutputStream = new FileOutputStream(fpath);
                fLock = fOutputStream.getChannel().lock();
                Properties props = new Properties();
                props.setProperty("accessToken", accessToken);
                props.setProperty("creationTime", String.valueOf(creationTime));
                props.setProperty("expiresIn", String.valueOf(expiresIn));
                props.setProperty("refreshToken", refreshToken);
                props.store(fOutputStream, "Token Information");
            } catch (IOException e) {
                throw e; // pass along exception
            } finally {
                if (fLock != null) { fLock.release(); }
                if (fOutputStream != null) { fOutputStream.close(); }
            }
        }
    }

    /**
     * Attempts to load an OAuthToken from a file in an asynchronous-safe
     * manner.
     *
     * <p>
     * If <code>fpath</code> does not exist or some required values are missing
     * from the saved file, null is returned.
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

        synchronized (LOCK_OBJECT) {
            // attempt to load from cached tokens, thereby saving file I/O
            if (cachedTokens != null && cachedTokens.get(fpath) != null) {
                return cachedTokens.get(fpath);
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
                if (!props.containsKey("creationTime") 
                        || !props.containsKey("expiresIn")) {
                    return null;
                }

                String accessToken = props.getProperty("accessToken");
                if (accessToken == null || accessToken.equals("")) {
                    return null;
                }

                String refreshToken = props.getProperty("refreshToken");

                String sExpiresIn = props.getProperty("expiresIn");
                long expiresIn = new Long(sExpiresIn).longValue();

                String sCreationTime = props.getProperty("creationTime");
                long creationTime = new Long(sCreationTime).longValue();

                return new OAuthToken(accessToken, expiresIn, refreshToken,
                        creationTime);
            } catch (IOException e) {
                throw e; // pass along exception
            } finally {
                if (fLock != null) { fLock.release(); }
                if (fInputStream != null) { fInputStream.close(); }
            }
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
