package com.att.api.speech.service;

import java.io.IOException;

import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.rest.RESTConfig;
import com.att.api.speech.model.SpeechResponse;

/**
 * Class that handles communication with the speech server.
 *
 */
public class SpeechService {
    private RESTConfig cfg;

    /**
     * Creates a speech service object. By default, chunked is set to false.
     *
     * @param cfg
     *            the configuration to use for setting HTTP request values
     * @see RESTConfig
     */
    public SpeechService(RESTConfig cfg) {
        this.cfg = cfg;
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
     * Sends the request to the server.
     *
     * @param attachments
     *            attachments to send.
     * @param accessToken
     *            access token used for authorization
     * @param speechContext
     *            speech context
     * @return a response in the form of a SpeechResponse object
     * @see SpeechResponse
     */
    public SpeechResponse sendRequest(String [] attachments, String accessToken,
            String speechContext, String xArg) throws Exception {
        RESTClient restClient = new RESTClient(this.cfg);
        restClient.addHeader("Authorization", "Bearer " + accessToken).
                addHeader("Accept", "application/json").
                addHeader("X-SpeechContext", speechContext);
        if (xArg != null && !xArg.equals("")) {
            restClient.addHeader("X-Arg", xArg);
        }
        String subType = "x-srgs-audio";
        String [] bodyNameAttribute = new String[3];
        bodyNameAttribute[0] = "x-dictionary" ;
        bodyNameAttribute[1] = "x-grammar" ;
        bodyNameAttribute[2] = "x-voice";

        APIResponse apiResponse = restClient.httpPost(attachments, subType, bodyNameAttribute);
        return parseSuccess(apiResponse.getResponseBody());
    }
}
