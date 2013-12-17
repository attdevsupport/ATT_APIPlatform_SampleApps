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

/**
 * Immutable class for holding any config variables for sending RESTFul
 * requests.
 *
 * @author pk9069
 * @version 1.0
 * @since 1.0
 */
public class RESTConfig {
    /** Default proxy to use, if any. */
    private static String defaultProxyHost = null;

    /** Default proxy port to use, if any. */
    private static int defaultProxyPort = -1;

    /** Default setting for accepting ssl certificates. */
    private static boolean defaultTrustAllCerts = false;

    /** Url to use for RESTFul request. */
    private final String url;

    /** Whether to trust all ssl certs, such as self-signed certs. */
    private final boolean trustAllCerts;

    /** Proxy host to use or <tt>null</tt> if none. */
    private final String proxyHost;

    /** Proxy port to use or -1 if none. */
    private final int proxyPort;

    /**
     * Creates a RESTConfig object with the specified url.
     *
     * <p>
     * Default settings will be used for proxy and ssl certificate settings.
     * </p>
     *
     * @param url url
     * @see #setDefaultProxy(String, int)
     * @see #setDefaultTrustAllCerts(boolean)
     */
    public RESTConfig(final String url) {
        this(url, defaultProxyHost, defaultProxyPort, defaultTrustAllCerts);
    }

    /**
     * Creates a RESTConfig object with the specified url and ssl certificate
     * settings.
     *
     * <p>
     * Default settings will be used for proxy settings.
     * </p>
     *
     * @param url url
     * @param trustAllCerts whether to allow all ssl certificates
     * @see #setDefaultProxy(String, int)
     */
    public RESTConfig(final String url, final boolean trustAllCerts) {
        this(url, defaultProxyHost, defaultProxyPort, trustAllCerts);
    }

    /**
     * Creates a RESTConfig object with the specified url and proxy settings.
     *
     * <p>
     * Default values will be used for ssl certificate settings.
     * </p>
     *
     * @param url url
     * @param proxyHost proxy host
     * @param proxyPort proxy port
     * @see #setDefaultTrustAllCerts(boolean)
     */
    public RESTConfig(String url, String proxyHost, int proxyPort) {
        this(url, proxyHost, proxyPort, defaultTrustAllCerts);
    }

    /**
     * Creates a RESTConfig object with the specified url, proxy settings, and
     * ssl certificate settings.
     *
     * @param url url
     * @param proxyHost proxy host
     * @param proxyPort proxy port
     * @param trustAllCerts whether to allow all ssl certificates
     */
    public RESTConfig(String url, String proxyHost,
            int proxyPort, boolean trustAllCerts) {

        this.url = url;
        this.proxyHost = proxyHost;
        this.proxyPort = proxyPort;
        this.trustAllCerts = trustAllCerts;
    }

    /**
     * Gets the url setting.
     *
     * @return url setting
     */
    public String getURL() {
        return this.url;
    }

    /**
     * Gets whether to allow all certificates, such as self-signed certs.
     *
     * @return whether to trust all certs
     */
    public boolean trustAllCerts() {
        return this.trustAllCerts;
    }

    /**
     * Gets proxy host to use or null if none.
     *
     * @return proxy host to use
     */
    public String getProxyHost() {
        return this.proxyHost;
    }

    /**
     * Gets proxy port to use or -1 if none.
     *
     * @return proxy port to use
     */
    public int getProxyPort() {
        return this.proxyPort;
    }

    /**
     * Sets the default proxy to use if none is specified during object
     * creation.
     *
     * @param host proxy host
     * @param port proxy port
     */
    public static synchronized void setDefaultProxy(String host, int port) {
        RESTConfig.defaultProxyHost = host;
        RESTConfig.defaultProxyPort = port;
    }

    /**
     * Sets the default ssl certificate setting to use if none is specified
     * during object creation.
     *
     * @param trust whether to allow all ssl certificates
     */
    public static synchronized void setDefaultTrustAllCerts(boolean trust) {
        RESTConfig.defaultTrustAllCerts = trust;
    }
}
