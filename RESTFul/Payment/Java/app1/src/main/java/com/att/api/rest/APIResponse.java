package com.att.api.rest;

import java.io.IOException;

import org.apache.http.Header;
import org.apache.http.HttpResponse;
import org.apache.http.util.EntityUtils;

public class APIResponse {
    private final int statusCode;
    private final String responseBody;
    private final HttpHeader[] headers;

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

    public APIResponse(final HttpResponse httpResponse) throws RESTException {
        try {
            this.statusCode = httpResponse.getStatusLine().getStatusCode();
            this.responseBody = EntityUtils.toString(httpResponse.getEntity());
            this.headers = buildHeaders(httpResponse);
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    public int getStatusCode() {
        return this.statusCode;
    }

    public String getResponseBody() {
        return this.responseBody;
    }

    public HttpHeader[] getAllHeaders() {
        return this.headers;
    }
}
