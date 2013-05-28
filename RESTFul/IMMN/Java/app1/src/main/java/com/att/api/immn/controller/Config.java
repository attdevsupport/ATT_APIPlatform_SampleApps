package com.att.api.immn.controller;

/**
 * Used to hold configuration values.
 */
public class Config {

	public String clientIdAuth;
	public String clientSecretAuth;
	public String FQDN;
	public String endPointURL;
	public String linkSource;
	public String linkDownload;
	public String linkHelp;
	public String attachmentFolder;
	public String scope;
    public String postOAuthUri;
	public boolean trustAllCerts;

	public int maxUploadFileSize;
	public String redirectUri;
    public int proxyPort;
    public String proxyHost;


    /**
	 * Gets any errors in the configuration settings. Returns null if there is
	 * no error.
	 * 
	 * @return String if error, null is no error.
	 */
	public String getError() {
		String err = null;
		final String[] values = { clientIdAuth, clientSecretAuth, endPointURL };
		final String[] names = { "clientIdAuth", "clientSecretAuth", "endPointURL"};
		for (int i = 0; i < values.length; ++i) {
			if (values[i] == null || values[i] == "") {
				err = names[i];
				break;
			}
		}

		if (err != null) {
			err = "The following required value not set in configuration: " + err;
		}

		return err;
	}
}
