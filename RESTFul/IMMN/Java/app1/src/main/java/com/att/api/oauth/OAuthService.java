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

import java.text.ParseException;

import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;

import org.json.JSONObject;

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting access
 * tokens.
 *
 * <p>
 * This implementation of the OAuth 2.0 Framework provides two models
 * for obtaining an access token, which can then be used for requesting access
 * to protected resources.
 * </p>
 *
 * These models are:
 * <ul>
 * <li>
 * Authorization Code - Uses a subscriber context by requesting an authorization
 * code before requesting an access token.
 * </li>
 * <li>
 * Client Credentials - Sends a direct request for an access token using a
 * client id, client secret, and scope.
 * </li>
 * </ul>
 *
 * An example of usage can be found below:
 * <pre>
 * // Declare variables to use
 * final String tokenFile = "/tmp/tokenfile.properties";
 * final String fqdn = "http://api.att.com";
 * final String clientId = "12345";
 * final String clientSecret = "12345";
 *
 * try {
 *     // Attempt to load token from file before sending token request
 *     OAuthToken token = OAuthToken.loadToken(tokenFile);
 *     if (token == null || token.isAccessTokenExpired()) {
 *         // attempt to send request
 *         OAuthService service
 *             = new OAuthService(fqdn, clientId, clientSecret);
 *         final String scope = "SMS";
 *
 *         // send request
 *         token = service.getToken(scope);
 *
 *         // token obtained--save it
 *         token.saveToken(tokenFile);
 *     }
 * } catch (RESTException e) {
 *     // if an error occured during access token request
 * } catch (IOException ioe) {
 *     // if an error occured loading or saving token
 * }
 * </pre>
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 * @see <a href="https://tools.ietf.org/html/rfc6749">OAuth 2.0 Framework</a>
 */
public class OAuthService {

    /** Added to fqdn to use for sending OAuth requests. */
    public static final String API_URL = "/oauth/token";

    /** Fully qualified domain name. */
    private final String fqdn;

    /** Client id to use for requesting an OAuth token. */
    private final String clientId;

    /** Client secret to use for requestion an OAuth token. */
    private final String clientSecret;

    /**
     * Parses the API response from the API server when an access token was
     * requested.
     *
     * @param response API Response to parse
     * @return OAuthToken object if successful response
     * @throws RESTException if there is an issue reading the API response
     */
    private OAuthToken parseResponse(APIResponse response)
            throws RESTException {

        try {
            JSONObject rpcObj = new JSONObject(response.getResponseBody());

            final String accessToken = rpcObj.getString("access_token");
            final String refreshToken = rpcObj.getString("refresh_token");
            long expiresIn = rpcObj.getLong("expires_in");

            // 0 indicates no expiry
            if (expiresIn == 0) {
                expiresIn = OAuthToken.NO_EXPIRATION;
            }

            return new OAuthToken(accessToken, expiresIn, refreshToken);
        } catch (ParseException e) {
            String msg = e.getMessage();
            String err = "API Server returned unexpected result: " + msg;
            throw new RESTException(err);
        }
    }

    /**
     * Sends an HTTP POST request using the specified REST client with the
     * content type set to 'application/x-www-form/urlencoded'.
     *
     * @param client REST client to use for sending the POST request
     * @return API Response returned by the REST client
     *
     * @throws RESTException if the REST client throws an exception
     */
    private APIResponse sendRequest(RESTClient client) throws RESTException {
        return client
            .addHeader("Content-Type", "application/x-www-form-urlencoded")
            .httpPost();
    }

    /**
     * Creates an OAuthService object.
     *
     * @param fqdn fully qualified domain used for sending request
     * @param clientId client id to use
     * @param clientSecret client secret to use
     */
    public OAuthService(String fqdn, String clientId, String clientSecret) {
        this.fqdn = fqdn;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    /**
     * Gets an access token using the specified code.
     *
     * <p>
     * The parameters set during object creation will be used when requesting
     * the access token.
     * </p>
     * <p>
     * The token request is done using the 'authorization_code' grant type.
     * </p>
     *
     * @param code code to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws RESTException if unable to send request
     */
    public OAuthToken getTokenUsingCode(String code) throws RESTException {
        RESTClient client =
            new RESTClient(this.fqdn + API_URL)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("code", code)
            .addParameter("grant_type", "authorization_code");

        APIResponse response = sendRequest(client);

        return parseResponse(response);
    }

    /**
     * Gets an access token using the specified code.
     *
     * <p>
     * The parameters set during object creation will be used when requesting
     * the access token.
     * </p>
     *
     * <p>
     * The token request is done using the 'client_credentials' grant type.
     * </p>
     *
     * @param scope scope to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws RESTException if request was unsuccessful
     */
    public OAuthToken getToken(String scope) throws RESTException {
        RESTClient client =
            new RESTClient(this.fqdn + API_URL)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("scope", scope)
            .addParameter("grant_type", "client_credentials");

        APIResponse apiResponse = sendRequest(client);

        return parseResponse(apiResponse);

    }

    /**
     * Gets an access token.
     *
     * <p>
     * The parameters set during object creation will be used when requesting
     * the access token.
     * </p>
     *
     * <p>
     * The token request is done using the 'refresh_token' grant type.
     * </p>
     *
     * @param refreshToken refresh token to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws RESTException if request was unsuccessful
     * @see OAuthToken#getRefreshToken()
     */
    public OAuthToken refreshToken(String refreshToken) throws RESTException {
        RESTClient client =
            new RESTClient(this.fqdn + API_URL)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("refresh_token", refreshToken)
            .addParameter("grant_type", "refresh_token");

        APIResponse response = sendRequest(client);

        return parseResponse(response);
    }
}
