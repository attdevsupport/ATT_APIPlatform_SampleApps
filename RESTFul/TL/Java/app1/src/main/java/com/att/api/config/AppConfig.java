package com.att.api.config;

import java.io.IOException;
import java.util.Properties;

/**
 * An immutable, singleton class used for getting the application's
 * configuration values.
 *
 */
public final class AppConfig {
    private static final String CONFIG_NAME = "application.properties";

    private Properties properties;

    // singleton instance
    private static AppConfig config = null;

    /**
     * Loads config from file specified in CONFIG_NAME.
     *
     * @throws IOException
     *             if unable to load properties file.
     */
    private static void loadConfig() throws IOException {
        AppConfig.config = new AppConfig();

        // load properties
        config.properties = new Properties();
        Properties props = config.properties;
        props.load(AppConfig.class.getClassLoader().getResourceAsStream(
                    AppConfig.CONFIG_NAME));
    }

    /**
     * Disallow creating instances using contructor.
     *
     * @see AppConfig.getConfig
     */
    private AppConfig() {
    }

    /**
     * Gets the singleton configuration instance.
     *
     * @return AppConfig configuration instance
     *
     * @throws IOException
     *             if unable to load properties file
     */
    public static synchronized AppConfig getInstance() throws IOException {
        // lazy init
        if (AppConfig.config == null) {
            loadConfig();
        }

        return AppConfig.config;
    }

    /**
     * Convenience method for getting whether to trust all certiciates.
     *
     * @return boolean whether to trust all certs
     */
    public boolean getTrustAllCerts() {
        String val = config.properties.getProperty("trustAllCerts", "false");
        return Boolean.parseBoolean(val);
    }

    /**
     * Convenience method for getting the proxy host or null if no proxy host
     * has been set.
     *
     * @return String proxy host or null
     */
    public String getProxyHost() {
        return config.properties.getProperty("proxyHost");
    }

    /**
     * Convenience method for getting the proxy port or -1 if no proxy port has
     * been set.
     *
     * @return int proxy port or -1 if none has been set
     */
    public int getProxyPort() {
        String val = config.properties.getProperty("proxyPort", "-1");
        return Integer.parseInt(val);
    }

    /**
     * Convenience method for getting the fully qualified domain name or null if
     * no FQDN has been set.
     *
     * @return String fully qualifed domain name or null
     * @deprecated replaced by {@link #getOauthFQDN()} and {@link #getApiFQDN()}
     */
    @Deprecated
    public String getFQDN() {
        return getApiFQDN();
    }

    /**
     * Convenience method for getting the fully qualified domain name for
     * api or null if no api FQDN has been set.
     */
    public String getApiFQDN() {
        final String fqdn = config.properties.getProperty("apiFQDN");
        if (fqdn == null) // backwards compatible
            return config.properties.getProperty("FQDN");

        return fqdn;
    }

    /**
     * Convenience method for getting the fully qualified domain name for
     * oauth or null if no oauth FQDN has been set.
     *
     * @return String fully qualifed domain name or null
     */
    public String getOauthFQDN() {
        final String fqdn = config.properties.getProperty("oauthFQDN");
        if (fqdn == null) // backwards compatible
            return config.properties.getProperty("FQDN");

        return fqdn;
    }

    /**
     * Convenience method for getting the client id or null if no client id has
     * been set.
     *
     * @return String client id or null
     */
    public String getClientId() {
        return config.properties.getProperty("clientId");
    }

    /**
     * Convenience method for getting the client secret or null if no client
     * secret has been set.
     *
     * @return String client secret or null
     */
    public String getClientSecret() {
        return config.properties.getProperty("clientSecret");
    }

    /**
     * Gets the property value for the specified key or null if key was not
     * found.
     *
     * @param key
     *            key value to search
     * @return value for the specified key
     */
    public String getProperty(String key) {
        return config.properties.getProperty(key);
    }

    /**
     * Gets the property value for the specified key or uses the specified 
     * default value if key was not found..
     *
     * @param key
     *            key value to search
     * @return value for the specified key
     */
    public String getProperty(String key, String defaultValue) {
        return config.properties.getProperty(key, defaultValue);
    }
}
