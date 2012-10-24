package com.att.api.speech.handler;

import java.util.HashMap;

/**
 * Used to hold configuration values.
 */
public class Config {
	public Config() {
		xarg = new HashMap<String, String>();
	}

	public String clientIdAuth;
	public String clientSecretAuth;
	public String FQDN;
	public String endPointURL;

	public int maxUploadFileSize;
	public String defaultFile;
	public String[] speechContexts;

	public HashMap<String, String> xarg;

	/**
	 * Gets the value to set in the X-Arg HTTP header.
	 * 
	 * @return value to set in HTTP header or the empty string "" if no
	 *         parameters are set
	 */
	public String getXArgHTTPValue() {
		if (xarg.size() == 0) {
			return "";
		}

		StringBuilder sb = new StringBuilder();
		for (String key : xarg.keySet()) {
			String value = xarg.get(key);
			sb.append(key + "=" + value + ',');
		}

		// remove the last comma
		sb.deleteCharAt(sb.length() - 1);

		return sb.toString();
	}

	/**
	 * Gets any errors in the configuration settings. Returns null if there is
	 * no error.
	 * 
	 * @return String if error, null is no error.
	 */
	public String getError() {
		String err = null;
		final String[] values = { clientIdAuth, clientSecretAuth, endPointURL,
				defaultFile };
		final String[] names = { "clientIdAuth", "clientSecretAuth",
				"endPointURL", "defaultFile" };
		for (int i = 0; i < values.length; ++i) {
			if (values[i] == null || values[i] == "") {
				err = names[i];
				break;
			}
		}
		if (speechContexts == null) {
			err = "speechContexts";
		}

		if (err != null) {
			err = "The following required value not set in configuration: " + err;
		}

		return err;
	}
}
