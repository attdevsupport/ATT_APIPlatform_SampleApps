package com.att.api.speech.service;

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
public class SpeechCustomService extends APIService {

    /**
     * Setup a service for handling custom speech requests
     *
     * @param fqdn The server processing the request
     * @param token The token being used to authenticate
     */
    public SpeechCustomService(String fqdn, OAuthToken token) {
        super(fqdn, token);
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
     * @param speechContext
     *            speech context
     * @param xArg
     *            Extra custom parameters to send with the request
     * @return a response in the form of a SpeechResponse object
     * @see SpeechResponse
     */
    public SpeechResponse sendRequest(String [] attachments, 
            String speechContext, String xArg) throws Exception {
        final String endpoint = getFQDN() + "/speech/v3/speechToTextCustom";

        RESTClient restClient = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Accept", "application/json")
            .addHeader("X-SpeechContext", speechContext);

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
