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

/**
 * Configuration values to be used by the RESTClient.
 *
 */
public class RESTConfig {
    private final String URL;

    private final boolean trustAllCerts;
    private final String proxyHost;
    private final int proxyPort;

    /**
     * Creates a RESTConfig with the specified URL.
     * <p>
     * By default, no proxy will be used, and only valid certificates will
     * be used.
     *
     * @param URL url to set
     */
    public RESTConfig(final String URL) {
        this(URL, null, -1, false);
    }

    /**
     * Creates a RESTConfig with the specified URL and whether to trust all
     * certificates.
     * <p>
     * Trusting all certificates is useful for testing self-signed
     * certificates. By default, no proxy will be used.
     *
     * @param URL url to set
     * @param trustAllCerts whether to trust all certificates
     */
    public RESTConfig(final String URL, final boolean trustAllCerts) {
        this(URL, null, -1, trustAllCerts);
    }
    
    /**
     * Creates a RESTConfig with the specified URL and proxy.
     * <p>
     * By default, only valid certificates will be allowed.
     *
     * @param URL url to set
     * @param proxyHost proxy host to set 
     * @param proxyPort proxy port to set
     */
    public RESTConfig(final String URL, final String proxyHost, 
            final int proxyPort) {

        this(URL, proxyHost, proxyPort, false);
    }

    /**
     * Creates a RESTConfig object with the specified URL, proxy host, proxy
     * port, and whether to trust all certificates. Allowing all certificates
     * is useful for testing self-signed certificates.
     *
     * @param URL url to set
     * @param proxyHost proxy host to set
     * @param proxyPort porxy port to set
     * @param trustAllCerts whether to trust all certificates
     */
    public RESTConfig(final String URL, final String proxyHost, 
            final int proxyPort, boolean trustAllCerts) {
        
        this.URL = URL;
        this.proxyHost = proxyHost;
        this.proxyPort = proxyPort;
        this.trustAllCerts = trustAllCerts;
    }

    /**
     * Gets the URL to use.
     *
     * @return URL to use
     */
    public String getURL() {
        return this.URL;
    }

    /**
     * Gets whether to trust all certificates.
     * 
     * @return true if to trust all certificates, false otherwise
     */
    public boolean trustAllCerts() {
        return this.trustAllCerts;
    }
    
    /**
     * Gets proxy host or null if proxy host has not been set.
     *
     * @return proxy host
     */
    public String getProxyHost() {
        return this.proxyHost;
    }

    /**
     * Gets proxy port or -1 if no proxy port has been set.
     *
     * @return proxy port
     */
    public int getProxyPort() {
        return this.proxyPort;
    }
}
