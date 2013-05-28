package com.att.api.rest;

public class RESTConfig {
    private final String URL;

    private final boolean trustAllCerts;
    private final String proxyHost;
    private final int proxyPort;

    public RESTConfig(final String URL) {
        this(URL, null, -1, false);
    }

    public RESTConfig(final String URL, final boolean trustAllCerts) {
        this(URL, null, -1, trustAllCerts);
    }
    
    public RESTConfig(final String URL, final String proxyHost, 
            final int proxyPort) {

        this(URL, proxyHost, proxyPort, false);
    }

    public RESTConfig(final String URL, final String proxyHost, 
            final int proxyPort, boolean trustAllCerts) {
        
        this.URL = URL;
        this.proxyHost = proxyHost;
        this.proxyPort = proxyPort;
        this.trustAllCerts = trustAllCerts;
    }

    public String getURL() {
        return this.URL;
    }

    public boolean trustAllCerts() {
        return this.trustAllCerts;
    }
    
    public String getProxyHost() {
        return this.proxyHost;
    }

    public int getProxyPort() {
        return this.proxyPort;
    }
}
