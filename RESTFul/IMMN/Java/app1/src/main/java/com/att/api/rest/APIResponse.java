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

package com.att.api.rest;

import java.io.IOException;

import org.apache.http.Header;
import org.apache.http.HttpResponse;
import org.apache.http.util.EntityUtils;

/**
 * Immutable class that holds API response information.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public class APIResponse {

    /** HTTP status code. */
    private final int statusCode;

    /** HTTP response body. */
    private final String responseBody;

    /** Array of HTTP headers. */
    private final HttpHeader[] headers;

    /**
     * Given an HttpResponse object, this method generates an array of HTTP
     * headers.
     *
     * @param httpResponse used for building HTTP headers
     * @return array of http headers
     */
    private static HttpHeader[] buildHeaders(final HttpResponse httpResponse) {
        final Header[] headers = httpResponse.getAllHeaders();

        HttpHeader[] httpHeaders = new HttpHeader[headers.length];
        for (int i = 0; i < headers.length; ++i) {
            final Header header = headers[i];
            final String name = header.getName();
            final String value = header.getValue();
            final HttpHeader httpHeader = new HttpHeader(name, value);
            httpHeaders[i] = httpHeader;
        }

        return httpHeaders;
    }

    /**
     * Creates an API response with the specified status code, response body,
     * and http headers.
     *
     * @param statusCode status code
     * @param responseBody response body
     * @param headers http headers
     */
    public APIResponse(int statusCode, String responseBody,
            HttpHeader[] headers) {

        this.statusCode = statusCode;
        this.responseBody = responseBody;

        // avoid potentially exposing internals
        this.headers = APIResponse.copyHeaders(headers);
    }

    /**
     * Create an API response from an <code>HttpResponse</code> object.
     *
     * @param httpResponse used for creating API response
     * @throws RESTException if unable to parse http response
     * @see org.apache.http.HttpResponse
     * @deprecated replaced by {@link #valueOf(HttpResponse)}
     */
    @Deprecated
    public APIResponse(HttpResponse httpResponse) throws RESTException {
        try {
            statusCode = httpResponse.getStatusLine().getStatusCode();
            responseBody = EntityUtils.toString(httpResponse.getEntity());
            headers = APIResponse.buildHeaders(httpResponse);
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    /**
     * Gets HTTP status code.
     *
     * @return http status code
     */
    public int getStatusCode() {
        return this.statusCode;
    }

    /**
     * Gets HTTP response body.
     *
     * @return http response body
     */
    public String getResponseBody() {
        return this.responseBody;
    }

    /**
     * Gets an array of all http headers returned.
     *
     * <p>
     * <strong>NOTE</strong>: Use <code>getHeader()</code> to obtain a specific
     * header instead of this function, for performance reasons.
     * </p>
     *
     * @return array of http headers
     * @see #getHeader(String)
     */
    public HttpHeader[] getAllHeaders() {
        // Avoid exposing internals at the cost of a performance hit
        return APIResponse.copyHeaders(this.headers);
    }

    /**
     * Gets the the value of the specified http header name or <tt>null</tt> if
     * none is found.
     *
     * @param name header name
     * @return http header value
     */
    public String getHeader(String name) {
        // Linear search is used for finding value.
        // Although asympotically this is O(n), where n is the number of
        // headers, linear search is in practice faster for a sufficiently
        // small array than an algorithm that would require building a data
        // structure.
        HttpHeader[] headers = getAllHeaders();
        for (HttpHeader header : headers) {
            if (header.getName().equals(name)) {
                return header.getValue();
            }
        }

        return null;
    }

    /**
     * Alias for <code>valueOf()</code>.
     *
     * @param httpResponse used for creating API response
     * @return response
     * @throws RESTException if unable to parse http response
     * @see #valueOf(HttpResponse)
     * @since 1.0
     */
    public static APIResponse fromHttpResponse(HttpResponse httpResponse)
            throws RESTException {

        return APIResponse.valueOf(httpResponse);
    }

    /**
     * Factory method for creating an API response from an
     * <code>HttpResponse</code> object.
     *
     * @param httpResponse used for creating API response
     * @return response
     * @throws RESTException if unable to parse http response
     * @see org.apache.http.HttpResponse
     * @since 1.0
     */
    public static APIResponse valueOf(HttpResponse httpResponse)
            throws RESTException {

        try {
            int statusCode = httpResponse.getStatusLine().getStatusCode();
            String rb = "";
            if (httpResponse.getEntity() != null) {
                rb = EntityUtils.toString(httpResponse.getEntity());
            }
            HttpHeader[] headers = APIResponse.buildHeaders(httpResponse);
            return new APIResponse(statusCode, rb, headers);
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    /**
     * Utility method for copying an array of headers.
     *
     * <p>
     * <strong>NOTE</strong>: Since <code>HttpHeader</code> objects are 
     * immutable, the objects themselves are not copied, only the array.
     * </p>
     *
     * @param headers HTTP headers to copy
     * @return copy of http headers
     * @since 1.0
     */
    public static HttpHeader[] copyHeaders(final HttpHeader[] headers) {
        final HttpHeader[] headersCopy = new HttpHeader[headers.length];
        for (int i = 0; i < headers.length; ++i) {
            // beauty of immutability :)
            // since HttpHeader is immutable, no need to provide a copy
            headersCopy[i] = headers[i];
        }
        return headersCopy;
    }

}
