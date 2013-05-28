package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

/**
 * Class used to represent a response to a speech-to-text request from the
 * server.
 */
public class SpeechResponse {
	private List<String[]> result;

	/**
	 * Creates a speech response object with an empty response.
	 */
	public SpeechResponse() {
		result = new ArrayList<String[]>();
	}

	/**
	 * Adds a response attribute.
	 * 
	 * @param name
	 *            name to add
	 * @param value
	 *            value to add
	 */
	public void addAttribute(String name, String value) {
		final String[] kvp = new String[] { name, value };
		this.result.add(kvp);
    }

	/**
	 * Gets the speech response as a list of key value pairs (encompassed in an
	 * array).
	 * 
	 * @return result as a list of key-value pairs
	 */
	public List<String[]> getResult() {
		return result;
	}
}
