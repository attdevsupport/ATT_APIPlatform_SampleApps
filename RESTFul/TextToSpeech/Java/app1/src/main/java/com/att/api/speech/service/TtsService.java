package com.att.api.speech.service;

import java.io.IOException;

import org.apache.http.ParseException;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.service.APIService;
import com.att.api.speech.model.SpeechResponse;

/**
 * Class that handles communication with the speech server.
 *
 */
public class TtsService extends APIService {

    /**
     * @param fqdn
     * @param token
     */
    public TtsService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    /**
     * If the server returned a successful response, this method parses the
     * response and returns a {@link SpeechResponse} object.
     *
     * @param response
     *            the response returned by the server
     * @return the server response as a binary byte[] array
     * @throws IOException
     *             if unable to read the passed-in response
     * @throws java.text.ParseException
     */
    private byte[] parseSuccess(APIResponse wavResponse)
            throws IOException
    {
        //decodes binary properly with iso-8859-1 charset
        return wavResponse.getResponseBody().getBytes("ISO-8859-1");
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
    private void parseFailure(APIResponse response)
            throws ParseException, IOException {
        String result;
        if (response.getResponseBody() == null) {
            result = String.valueOf(response.getStatusCode());
        } else {
            result = response.getResponseBody();
        }
        throw new IOException(result);
    }

    /**
     * Sends the request to the server.
     *
     * @param file
     *            file to send.
     * @param speechContext
     *            speech context
     * @return a byte array 
     * @see SpeechResponse
     */
    public byte[] sendRequest(String contentType,
            String speechText, String xArg) throws Exception {

        final String endpoint = getFQDN() + "/speech/v3/textToSpeech";
        RESTClient restClient = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .addHeader("Content-Type", contentType)
            .addHeader("Accept", "audio/x-wav");                         

        if (xArg != null && !xArg.equals("")) {
            restClient.addHeader("X-Arg", xArg);
        }

        APIResponse apiResponse = restClient.httpPost(speechText);

        byte[] wavBytes = null;
        int statusCode = apiResponse.getStatusCode();
        if (statusCode == 200 || statusCode == 201) {
            wavBytes = parseSuccess(apiResponse);
        } else if (statusCode == 401) {
            throw new IOException("Unauthorized request.");
        } else {
            parseFailure(apiResponse);
        }
        return wavBytes;
    }
}
