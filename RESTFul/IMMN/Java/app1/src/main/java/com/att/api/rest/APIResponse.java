package com.att.api.rest;

import org.apache.commons.collections.map.MultiValueMap;
import java.io.InputStream;

/**
 * Created with IntelliJ IDEA.
 * User: sendhilc
 * Date: 1/16/13
 * Time: 9:23 AM
 * To change this template use File | Settings | File Templates.
 */
public class APIResponse {

    private String reasonPhrase;
    private byte[] rawBody = null;
    private int statusCode;
    private String httpVersion;
    private String responseBody;
    private MultiValueMap responseHeaders;

    public APIResponse(){
    }

    public String getReasonPhrase() {
        return this.reasonPhrase;
    }

    public void setReasonPhrase(String reasonPhrase) {
        this.reasonPhrase = reasonPhrase;
    }

    public int getStatusCode() {
        return this.statusCode;
    }

    public void setStatusCode(int statusCode) {
        this.statusCode = statusCode;
    }

    public String getHttpVersion() {
        return this.httpVersion;
    }

    public void setHttpVersion(String httpVersion) {
        this.httpVersion = httpVersion;
    }

    public String getResponseBody() {
        return this.responseBody;
    }

    public void setResponseBody(String responseBody) {
        this.responseBody = responseBody;
    }

    public MultiValueMap getAllHeaders() {
        return this.responseHeaders;
    }

    public void setResponseHeaders(MultiValueMap responseHeaders) {
        this.responseHeaders = responseHeaders;
    }

    public byte[] getRawBody(){
      return this.rawBody;
    }

    public void setRawBody(byte[] data) {
      this.rawBody = data;
      this.setResponseBody(new String(rawBody));
    }
}
