package com.att.api.speech.model;

import java.util.ArrayList;
import java.util.List;

/**
 * Class used to represent a response to a speech-to-text request from the
 * server.
 */
public class SpeechResponse {
	private String error;
	private List<String[]> result;

	/**
	 * Creates a speech response object with an error response.
	 */
	public SpeechResponse(String error) {
		this.error = error;
	}

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
	 * Gets whether this speech response contains an error message.
	 * 
	 * @return true if contains error message, false otherwise
	 */
	public boolean hasError() {
		return error != null;
	}

	/**
	 * Gets the speech error message or null if there isn't one.
	 * 
	 * @return speech error message or null
	 */
	public String getError() {
		return error;
	}

	/**
	 * Gets the speech response as a list of key value pairs (encompassed in an
	 * array).
	 * 
	 * @return result as a list of key-value pairs
	 */
	public List<String[]> getResult() {
		if (hasError()) {
			return null;
		}

		return result;
	}
}
