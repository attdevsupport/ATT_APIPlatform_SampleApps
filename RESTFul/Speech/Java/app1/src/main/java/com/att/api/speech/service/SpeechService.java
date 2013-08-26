package com.att.api.speech.service;

import java.io.File;
import java.io.IOException;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.service.APIService;
import com.att.api.speech.model.SpeechResponse;

/**
 * Class that handles communication with the speech server.
 *
 */
public class SpeechService extends APIService {
    private boolean chunked;

    public SpeechService(String fqdn, OAuthToken token) {
        super(fqdn, token);
        this.chunked = false;
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
     * @throws java.text.ParseException
     */
    private SpeechResponse parseSuccess(String response)
            throws IOException, java.text.ParseException {
        String result = response;
        JSONObject object = new JSONObject(result);
        JSONObject recognition = object.getJSONObject("Recognition");
        SpeechResponse sp = new SpeechResponse();
        sp.addAttribute("ResponseID", recognition.getString("ResponseId"));
        final String jStatus = recognition.getString("Status");

        sp.addAttribute("Status", jStatus);

        if (jStatus.equals("OK")) {
            JSONArray nBest = recognition.getJSONArray("NBest");
            final String[] names = { "Hypothesis", "LanguageId", "Confidence",
                    "Grade", "ResultText", "Words", "WordScores" };
            for (int i = 0; i < nBest.length(); ++i) {
                JSONObject nBestObject = (JSONObject) nBest.get(i);
                for (final String name : names) {
                    String value = nBestObject.getString(name);
                    if (value != null) {
                        sp.addAttribute(name, value);
                    }
                }
            }
        }

        return sp;
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
     * @param xArg
     *            Extra custom parameters to send with the request
     * @param speechContext
     *            speech context
     * @param subContext
     *            speech
     * @return a response in the form of a SpeechResponse object
     * @see SpeechResponse
     */
    public SpeechResponse sendRequest(File file, String xArg, 
            String speechContext, String subContext) throws Exception {
        final String endpoint = getFQDN() + "/speech/v3/speechToText";

        RESTClient restClient = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .addHeader("X-SpeechContext", speechContext);

        if (xArg != null && !xArg.equals("")) {
            restClient.addHeader("X-Arg", xArg);
        }
        if (subContext != null && !subContext.equals("") && speechContext.equals("Gaming")){
            restClient.addHeader("X-SpeechSubContext",subContext);
        }
        APIResponse apiResponse = restClient.httpPost(file);
        return parseSuccess(apiResponse.getResponseBody());
    }
}
