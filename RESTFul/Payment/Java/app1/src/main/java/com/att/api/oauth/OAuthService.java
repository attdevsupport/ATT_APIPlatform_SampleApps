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

import java.io.IOException;
import java.text.ParseException;

import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import org.json.JSONObject;

/**
 * Implements the OAuth 2.0 Authorization Framework for requesting access
 * tokens. This implementation of the OAuth 2.0 Framework provides two models
 * for obtaining an access token, which can then be used for requesting access
 * to protected resources. These models are:
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
 * An example of usage can be found below:
 * <pre>
 * // Declare variables to use
 * final String tokenFile = "/tmp/tokenfile.properties";
 * final String endpoint = "http://api.att.com" + OAuthService.API_URL;
 * final String clientId = "12345";
 * final String clientSecret = "12345";
 *
 * try {
 *     // Attempt to load token from file before sending token request
 *     OAuthToken token = OAuthToken.loadToken(tokenFile);
 *     if (token == null || token.isAccessTokenExpired()) {
 *        // attempt to send request
 *        final RESTConfig cfg = new RESTConfig(endpoint);
 *        final OAuthService service = 
 *            new OAuthService(cfg, clientId, clientSecret);    
 *        final String scope = "SMS";
 *
 *        // send request
 *        token = service.getToken(scope); 
 *        
 *        // token obtained--save it
 *        token.saveToken(tokenFile);
 *     }
 * } catch (RESTException e) {
 *     // if an error occured when access token is requested 
 * }
 *
 * </pre>
 *
 * @version 2.2
 * @since 2.2
 * @see https://tools.ietf.org/html/rfc6749
 */
public class OAuthService {
    // 100 years
    private static final long ACCESS_TOKEN_EXPIRY = 3155692597470L;

    // 24 hrs
    // private static final long REFRESH_TOKEN_EXPIRY = 86400000l;

    public static final String API_URL = "/oauth/token";

    private final RESTConfig cfg;

    private final String clientId;
    private final String clientSecret;

    /**
     * Validates client credentials.
     *
     * @param scope
     *            scope to validate
     * @param clientId
     *            clientId to validate
     * @param clientSecret
     *            client secret to validate
     *
     * @throws com.att.api.rest.RESTException
     *             if any of the aforementioned variables are invalid.
     */
    private void validateClientCredentials(String scope, String clientId,
            String clientSecret) throws RESTException {

        final String[] toValidate = { scope, clientId, clientSecret };
        final String[] varNames = { "Scope", "Client Id", "ClientSecret" };
        for (int i = 0; i < toValidate.length; ++i) {
            if (toValidate[i] == null || toValidate[i].length() == 0) {
                String exception = varNames[i] + " must not be empty.";
                throw new RESTException(-1, exception);
            }
        }
    }

    /**
     * Parses the API response from the API server when an access token was
     * requested.
     *
     * @param apiResponse
     *            API Response to parse
     * @return OAuthToken if successful response
     * @throws com.att.api.rest.RESTException if there is an issue reading the API response
     */
    private OAuthToken parseResponse(APIResponse response) throws RESTException {

        final long currentTime = System.currentTimeMillis();

        try {
            JSONObject rpcObj = new JSONObject(response.getResponseBody());

            final String accessToken = rpcObj.getString("access_token");
            final String refreshToken = rpcObj.getString("refresh_token");
            long expiry = Long.parseLong(rpcObj.getString("expires_in"));
            // 0 indicates no expiry
            // therefore just make the token expire in 100 years
            if (expiry == 0) {
                expiry = currentTime + ACCESS_TOKEN_EXPIRY;
            }

            return new OAuthToken(accessToken, expiry, refreshToken);
        } catch (ParseException e) {
            String msg = e.getMessage();
            String err = "API Server returned unexpected result: " + msg; 
            throw new RESTException(err);
        }
    }

    /**
     * Sends an HTTP POST request using the specified REST client with the
     * content type set to 'application/x-ww-form/urlencoded'.
     *
     * @param client REST client to use for sending the POST request
     * @return API Response returned by the REST client
     *
     * @throws java.io.IOException if the REST client throws an exception
     */
    private APIResponse sendReceive(RESTClient client) throws RESTException {
        return client 
            .addHeader("Content-Type", "application/x-www-form-urlencoded")
            .httpPost();  
    }

    /**
     * Creates an OAuthService object.
     *
     * @param cfg RESTConfig to use
     * @param clientId client id to use
     * @param clientSecret client secret to use
     */
    public OAuthService(final RESTConfig cfg, final String clientId, 
            final String clientSecret) {

        this.cfg = cfg;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    /**
     * Gets an access token using the specified code. The parameters set during
     * object creation will be used when requesting the access token.
     * <p>
     * The token request is done using the 'authorization_code' grant type.
     *
     * @param code code to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws java.io.IOException if unable to send request
     */
    public OAuthToken getTokenUsingCode(String code) throws RESTException {
        RESTClient client = 
            new RESTClient(this.cfg)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("code", code)
            .addParameter("grant_type", "authorization_code");

        APIResponse response = sendReceive(client);
        
        return parseResponse(response);
    }

    /**
     * Gets an access token using the specified code. The parameters set during
     * object creation will be used when requesting the access token.
     * <p>
     * The token request is done using the 'client_credentials' grant type.
     *
     * @param scope scope to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws com.att.api.rest.RESTException
     *      if unable to send request or invalid arguments supplied
     */
    public OAuthToken getToken(String scope) throws RESTException {
        validateClientCredentials(scope, clientId, clientSecret);

        RESTClient client = 
            new RESTClient(this.cfg)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("scope", scope)
            .addParameter("grant_type", "client_credentials");

        APIResponse apiResponse = sendReceive(client);

        return parseResponse(apiResponse);
        
    }

    /**
     * Gets an access token. The parameters set during object creation will 
     * be used when requesting the access token.
     * <p>
     * The token request is done using the 'refresh_token' grant type.
     *
     * @param clientId client id to use when requesting access token
     * @param clientSecret client secret to use when requesting access token
     * @param refreshToken refresh token to use when requesting access token
     * @return OAuthToken object if successful
     *
     * @throws com.att.api.rest.RESTException if unable to send request
     */
    public OAuthToken refreshToken(String clientId, String clientSecret, 
        String refreshToken) throws RESTException {
        RESTClient client = 
            new RESTClient(this.cfg)
            .addParameter("client_id", clientId)
            .addParameter("client_secret", clientSecret)
            .addParameter("refresh_token", refreshToken)
            .addParameter("grant_type", "refresh_token");

        APIResponse response = sendReceive(client);

        return parseResponse(response);
    }
}
