package com.att.api.speech.service;

import java.io.IOException;
import java.security.cert.X509Certificate;

import org.apache.http.HttpResponse;
import org.apache.http.ParseException;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.conn.ssl.TrustStrategy;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.util.EntityUtils;

import com.att.api.rest.RESTConfig;
import javax.servlet.jsp.jstl.core.Config;
import org.apache.commons.io.IOUtils;
import org.apache.http.entity.StringEntity;

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
     * @see Config
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
    private byte[] parseSuccess(HttpResponse wavResponse)
            throws IOException
    {
        return IOUtils.toByteArray(wavResponse.getEntity().getContent());
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
    private void parseFailure(HttpResponse response)
            throws ParseException, IOException {
        String result;
        if (response.getEntity() == null) {
            result = response.getStatusLine().getReasonPhrase();
        } else {
            result = EntityUtils.toString(response.getEntity());
        }
        throw new IOException(result);
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
     * @see SpeechResponse
     */
    public byte[] sendRequest(String accessToken, String contentType,
            String speechText, String xArg) throws Exception {
        SchemeRegistry registry = new SchemeRegistry();

        DefaultHttpClient client;
        if (cfg.trustAllCerts()) {
            // Trust all host certs. Only enable if on testing!
            SSLSocketFactory socketFactory = new SSLSocketFactory(
                    new TrustStrategy() {
                        @Override 
                        public boolean isTrusted(final X509Certificate[] chain,
                                String authType) {
                            return true;
                        }

                    }, SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);

            registry.register(new Scheme("http", 80, PlainSocketFactory
                    .getSocketFactory()));
            registry.register(new Scheme("https", 443, socketFactory));
            ThreadSafeClientConnManager cm = new ThreadSafeClientConnManager(
                    registry);
            client = new DefaultHttpClient(cm,
                    new DefaultHttpClient().getParams());
        } else {
            client = new DefaultHttpClient();
        }

        HttpPost hPost = new HttpPost(cfg.getURL());
        hPost.addHeader("Authorization", "Bearer " + accessToken);
        hPost.addHeader("Content-Type", contentType);
        hPost.addHeader("Accept", "audio/x-wav");

        if (xArg != null && !xArg.equals("")) {
            hPost.addHeader("X-Arg", xArg);
        }

        hPost.setEntity(new StringEntity(speechText));
        
        byte[] wavBytes = null;
        HttpResponse response = client.execute(hPost);
        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode == 200 || statusCode == 201) {
            wavBytes = parseSuccess(response);
        } else if (statusCode == 401) {
            throw new IOException("Authorized request.");
        } else {
            parseFailure(response);
        }
        return wavBytes;
    }
}
