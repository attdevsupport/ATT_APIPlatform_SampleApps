package com.att.api.speech.service;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;

import org.apache.http.HttpResponse;
import org.apache.http.ParseException;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.FileEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.speech.handler.Config;
import com.att.api.speech.model.SpeechResponse;

/**
 * Class that handles communication with the speech server.
 * 
 */
public class SpeechService {
	private Config cfg;

	private boolean chunked;

	/**
	 * Creates a speech service object. By default, chunked is set to false.
	 * 
	 * @param cfg
	 *            the configuration to use for setting HTTP request values
	 * @see Config
	 */
	public SpeechService(Config cfg) {
		this.cfg = cfg;

		chunked = false;
	}
	
	/**
	 * If the server returned a successful response, this method parses the
	 * response and returns a {@link SpeechResponse} object.
	 * 
	 * @param response
	 *            the response returned by the server
	 * @return the server response as a SpeechResponse object
	 * @throws IOException
	 *             if unable to read the passed-in response
	 */
	private SpeechResponse parseSuccess(HttpResponse response) throws IOException {
		String result = EntityUtils.toString(response.getEntity());
		try {
			JSONObject object = new JSONObject(result);
			JSONObject recognition = object.getJSONObject("Recognition");
			JSONArray nBest = recognition.getJSONArray("NBest");
			SpeechResponse sp = new SpeechResponse();
			sp.addAttribute("ResponseID", recognition.getString("ResponseId"));
			final String[] names = { "Hypothesis", "LanguageId", "Confidence", "Grade",
					"ResultText", "Words", "WordScores" };
			for (int i = 0; i < nBest.length(); ++i) {
				JSONObject nBestObject = (JSONObject) nBest.get(i);
				for (final String name : names) {
					String value = nBestObject.getString(name);
					if (value != null) {
						sp.addAttribute(name, value);
					}
				}
			}
			return sp;
		} catch (java.text.ParseException e) {
			return new SpeechResponse("Server responded with an unexpected reply.");
		}
	}

	/**
	 * If the server responds with a failure, this method returns a
	 * {@link SpeechResponse} object with the failure message.
	 * 
	 * @param response
	 *            response to parse
	 * @return error in a SpeechResponse object
	 * @throws ParseException
	 *             if unable to parse the passed-in response
	 * @throws IOException
	 *             if unable to read the passed-in response
	 */
	private SpeechResponse parseFailure(HttpResponse response) throws ParseException, IOException {
		String result = null;
		if (response.getEntity() == null) {
			result = response.getStatusLine().getReasonPhrase();
		} else {
			result = EntityUtils.toString(response.getEntity());
		}
		return new SpeechResponse(result);
	}

	/**
	 * Gets MIME type for specified file. MIME type calculated by doing a very
	 * simple check based on file header.
	 * <p>
	 * Currently supports checking for the following formats:
	 * <li>AMR</li>
	 * <li>AMR-WB</li>
	 * <li>WAV</li>
	 * <li>Speex</li>
	 * 
	 * @param file
	 *            file to check for MIME type
	 * @return String MIME type
	 * @throws IOException
	 *             if there is a problem reading the specified file
	 */
	private String getMIMEType(File file) throws IOException {
		// AMR/AMR-WB check will be done according to RFC3267
		// (http://www.ietf.org/rfc/rfc3267.txt?number=3267)
		final byte[] AMRHeader = { '#', '!', 'A', 'M', 'R' };
		final byte[] AMRWBExtension = { '-', 'W', 'B' };

		final byte[] RIFFHeader = { 'R', 'I', 'F', 'F' };
		final byte[] WAVEHeader = { 'W', 'A', 'V', 'E' };

		// Check for Speex in Ogg files. Ogg will be checked according to
		// RFC3533 (http://www.ietf.org/rfc/rfc3533.txt). Speex will be checked
		// according to the format specified the speex manual
		// (www.speex.org/docs/manual/speex-manual/node8.html)
		final byte[] OggHeader = { 'O', 'g', 'g', 'S' };
		final byte[] SpeexHeader = { 'S', 'p', 'e', 'e', 'x', ' ', ' ', ' ' };

		final byte[] header = new byte[4];
		FileInputStream fStream = new FileInputStream(file);
		// Read the first 4 bytes
		fStream.read(header, 0, 4);

		String contentType = null;
		if (Arrays.equals(header, RIFFHeader)) {
			// read more bytes to determine if it's a wav file
			fStream.skip(4); // size if wav structure
			fStream.read(header, 0, 4); // wav header if wav structure
			if (Arrays.equals(header, WAVEHeader)) {
				contentType = "audio/wav";
			}
		} else if (Arrays.equals(header, OggHeader)) {
			// first 28 bytes are ogg. Afterwards should be speex header.
			fStream.skip(24);
			final byte[] headerExt = new byte[8];
			fStream.read(headerExt, 0, 8);
			if (Arrays.equals(headerExt, SpeexHeader)) {
				contentType = "audio/x-speex";
			}
		}

		// try looking for AMR
		final byte[] testHeader = new byte[5];
		for (int i = 0; i < header.length; ++i) {
			testHeader[i] = header[i];
		}
		fStream.read(testHeader, 4, 1);
		if (Arrays.equals(testHeader, AMRHeader)) {
			final byte[] headerExt = new byte[3];
			fStream.read(headerExt, 0, 3);
			if (Arrays.equals(headerExt, AMRWBExtension)) {
				contentType = "audio/amr-wb";
			} else {
				contentType = "audio/amr";
			}
		}

		fStream.close();

		return contentType;
	}

	/**
	 * Sets whether to send the request body chunked or non-chunked.
	 * 
	 * @param chunked
	 *            value to set
	 */
	public void setChunked(boolean chunked) {
		this.chunked = chunked;
	}

	/**
	 * Sends the request to the server.
	 * 
	 * @param file
	 *            file to send.
	 * @param accessToken
	 *            access token used for authorization
	 * @param speechContext
	 *            speech context
	 * @return a response in the form of a SpeechResponse object
	 * @throws ClientProtocolException
	 *             if there was an error in the HTTP protocol
	 * @throws IOException
	 *             if there was an error reading the server response
	 * @see SpeechResponse
	 */
	public SpeechResponse sendRequest(File file, String accessToken, String speechContext)
			throws ClientProtocolException, IOException {
		DefaultHttpClient client = new DefaultHttpClient();

		String contentType = getMIMEType(file);
		if (contentType == null) {
			return new SpeechResponse(
					"Invalid content type. File content does not satisfy speech constraints.");
		}
		HttpPost hPost = new HttpPost(cfg.endPointURL);
		FileEntity fEntity = new FileEntity(file, contentType);

		hPost.setEntity(fEntity);

		hPost.addHeader("Authorization", "Bearer " + accessToken);
		hPost.addHeader("Content-Type", contentType);
		hPost.addHeader("Accept", "application/json");
		hPost.addHeader("X-SpeechContext", speechContext);

		if (!chunked) {
			fEntity.setChunked(false);
		} else {
			fEntity.setChunked(true);
		}

		if (cfg.getXArgHTTPValue() != null && cfg.getXArgHTTPValue() != "") {
			hPost.addHeader("X-Arg", cfg.getXArgHTTPValue());
		}

		HttpResponse jsonResponse = client.execute(hPost);
		int statusCode = jsonResponse.getStatusLine().getStatusCode();
		if (statusCode == 200 || statusCode == 201) {
			return parseSuccess(jsonResponse);
		} else if (statusCode == 401) {
			return new SpeechResponse("Invalid token. Unauthorized request.");
		} else {
			return parseFailure(jsonResponse);
		}
	}
}
