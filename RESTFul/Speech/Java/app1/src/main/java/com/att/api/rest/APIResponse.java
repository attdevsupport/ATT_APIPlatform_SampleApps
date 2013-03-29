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
 * An immutable class that encapsulates an API response.
 *
 * @version 2.2
 * @since 2.2
 */
public class APIResponse {
    private final int statusCode;
    private final String responseBody;
    private final HttpHeader[] headers;

    /**
     * Internal method used to create http headers using the specified
     * HttpResponse object.
     *
     * @param httpResponse used to create headers
     * @return http headers
     */
    private HttpHeader[] buildHeaders(final HttpResponse httpResponse) {
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
     * Create an APIResponse object using the specified HttpResponse object.
     *
     * @param httpResponse used to create the APIResponse
     *
     * @throws RESTException if unable to read from the HttpResponse object
     */
    public APIResponse(final HttpResponse httpResponse) throws RESTException {
        try {
            this.statusCode = httpResponse.getStatusLine().getStatusCode();
            this.responseBody = EntityUtils.toString(httpResponse.getEntity());
            this.headers = buildHeaders(httpResponse);
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    /**
     * Gets the http status code returned by the api server.
     * <p>
     * For example, status code 200 represents 'OK.' 
     *
     * @return status code
     */
    public int getStatusCode() {
        return this.statusCode;
    }

    /**
     * Gets the http response body.
     *
     * @return http response body
     */
    public String getResponseBody() {
        return this.responseBody;
    }

    /**
     * Gets a list of all the headers returned by the API response.
     *
     * @return an array of all the HttpHeaders 
     */
    public HttpHeader[] getAllHeaders() {
        // avoid exposing internals, create copy
        HttpHeader[] copy = new HttpHeader[this.headers.length];
        for (int i = 0; i < this.headers.length; ++i) {
            copy[i] = headers[i];
        }
        return copy;
    }
}
