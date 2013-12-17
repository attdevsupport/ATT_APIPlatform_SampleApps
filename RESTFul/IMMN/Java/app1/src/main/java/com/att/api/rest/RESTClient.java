/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2013.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.rest;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLConnection;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.http.HttpEntity;
import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.client.params.ClientPNames;
import org.apache.http.conn.params.ConnRoutePNames;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.conn.ssl.TrustStrategy;
import org.apache.http.entity.FileEntity;
import org.apache.http.entity.StringEntity;
import org.apache.http.entity.mime.FormBodyPart;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.message.AbstractHttpMessage;
import org.json.JSONObject;

import com.att.api.oauth.OAuthToken;

/**
 * Client used to send RESTFul requests.
 *
 * <p>
 * Many of the methods return a reference to the 'this,' thereby allowing
 * method chaining.
 * </p>
 *
 * An example of usage can be found below:
 * <pre>
 * <code>
 * RESTClient client;
 * try {
 *     client = new RESTClient("http://www.att.com");
 *     APIResponse response = client
 *         .setHeader("Accept", "application/json")
 *         .setHeader("Clientid", "clientid")
 *         .setHeader("header", "value")
 *         .httpPost("postbody");
 *     if (response.getStatusCode() == 200) {
 *         System.out.println("Success!");
 *     }
 *  } catch (RESTException re) {
 *      // Handle Exception
 *  }
 * </code>
 * </pre>
 *
 * @version 1.0
 * @since 1.0
 */
public class RESTClient {

    /**
     * Whether to trust all SSL certificates, which may be used for self-signed
     * or invalidly-signed certs.
     */
    private final boolean trustAllCerts;

    /** Proxy host to use, if any. */
    private final String proxyHost;

    /** Proxy port to use, if any. */
    private final int proxyPort;

    /** URL that request will be sent to. */
    private final String url;

    /** Http headers to send. */
    private final Map<String, List<String>> headers;

    /** Http parameters to send. */
    private final Map<String, List<String>> parameters;

    /**
     * Internal method used to build an APIResponse using the specified
     * HttpResponse object.
     *
     * @param response response wrapped inside an APIResponse object
     * @return api response
     * @throws RESTException if request was not successful
     */
    private APIResponse buildResponse(HttpResponse response)
            throws RESTException {

        APIResponse apir = APIResponse.fromHttpResponse(response);
        int statusCode = apir.getStatusCode();
        // TODO (pk9069): allow these codes to be configurable
        if (statusCode != 200 && statusCode != 201 && statusCode != 202 && statusCode != 204) {
            throw new RESTException(statusCode, apir.getResponseBody());
        }

        return apir;
    }

    /**
     * Used to release any resources used by the connection.
     *
     * @param response HttpResponse object used for releasing the connection
     * @throws RESTException if unable to release connection
     */
    private void releaseConnection(HttpResponse response) throws RESTException {
        final HttpEntity entity = response.getEntity();

        if (entity == null) {
            return;
        }

        try { 
            if (entity.isStreaming()) {
                InputStream instream = entity.getContent();
                if (instream != null) {
                    instream.close();
                }
            }
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    /**
     * Sets headers to the http message.
     *
     * @param httpMsg http message to set headers for
     */
    private void addInternalHeaders(AbstractHttpMessage httpMsg) {
        if (headers.isEmpty()) {
            return;
        }

        final Set<String> keySet = headers.keySet();
        for (final String key : keySet) {
            final List<String> values = headers.get(key);
            for (final String value : values) {
                httpMsg.addHeader(key, value);
            }
        }
    }

    /**
     * Builds the query part of a URL using the UTF-8 encoding.
     *
     * @return query
     */
    private String buildQuery() {
        if (this.parameters.size() == 0) {
            return "";
        }

        StringBuilder sb = new StringBuilder();
        String charSet = "UTF-8";
        try {
            Iterator<String> keyitr = this.parameters.keySet().iterator();
            for (int i = 0; keyitr.hasNext(); ++i) {
                if (i > 0) {
                    sb.append("&");
                }

                final String name = keyitr.next();
                final List<String> values = this.parameters.get(name);
                for (final String value : values) {
                    sb.append(URLEncoder.encode(name, charSet));
                    sb.append("=");
                    sb.append(URLEncoder.encode(value, charSet));
                }
            }
        } catch (UnsupportedEncodingException e) {
            // UTF-8 is a Java supported encoding.
            // This should not occur unless the Java VM is not functioning
            // properly.
            throw new IllegalStateException();
        }

        return sb.toString();
    }

    /**
     * Sets the proxy attributes for the specified http client.
     *
     * @param httpClient client to set proxy attributes for
     */
    private void setProxyAttributes(HttpClient httpClient) {
        if (this.proxyHost != null && this.proxyPort != -1) {
            HttpHost proxy = new HttpHost(this.proxyHost, this.proxyPort);
            httpClient.getParams().setParameter(
                    ConnRoutePNames.DEFAULT_PROXY, proxy);
        }
    }


    /**
     * Creates an http client that can be used for sending http requests.
     *
     * <p>
     * Sets proxy and certificate settings.
     * </p>
     *
     * @return http client
     * @throws RESTException if unable to create http client.
     */
    private HttpClient createClient() throws RESTException {
        DefaultHttpClient client;

        if (trustAllCerts) {
            // Trust all host certs. Only enable if on testing!
            SSLSocketFactory socketFactory = null;
            try {
                socketFactory = new SSLSocketFactory(new TrustStrategy() {
                    public boolean isTrusted(final X509Certificate[] chain,
                            String authType) {

                        return true;
                    }
                }, SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);
            } catch (Exception e) {
                // shouldn't occur, but just in case
                final String msg = e.getMessage();
                throw new RESTException("Unable to create HttpClient. " + msg);
            }

            SchemeRegistry registry = new SchemeRegistry();

            registry.register(
                new Scheme("http", 80, PlainSocketFactory.getSocketFactory())
            );
            registry.register(new Scheme("https", 443, socketFactory));
            ThreadSafeClientConnManager cm
                = new ThreadSafeClientConnManager(registry);
            client = new DefaultHttpClient(cm,
                    new DefaultHttpClient().getParams());
        } else {
            client = new DefaultHttpClient();
        }

        client.getParams().setBooleanParameter(
                ClientPNames.HANDLE_REDIRECTS, false);

        setProxyAttributes(client);
        return client;
    }

    /**
     * Creates a RESTClient with the specified URL, proxy host, and proxy port.
     *
     * <p>
     * The RESTClient object is created with the default ssl settings of the
     * <code>RESTConfig</code> object.
     * </p>
     *
     * @param url url to send request to
     * @param proxyHost proxy host to use for sending request
     * @param proxyPort proxy port to use for sendin request
     *
     * @throws RESTException if unable to create a RESTClient
     * @see #RESTClient(RESTConfig)
     * @see RESTConfig#setDefaultTrustAllCerts(boolean)
     */
    public RESTClient(String url, String proxyHost, int proxyPort)
            throws RESTException {
        this(new RESTConfig(url, proxyHost, proxyPort));
    }

    /**
     * Creates a RESTClient with the specified URL.
     *
     * <p>
     * The RESTClient object is created with the default proxy and ssl settings
     * of the <code>RESTConfig</code> object.
     * </p>
     *
     * @param url url to send request to
     *
     * @throws RESTException if unable to create a RESTClient
     * @see #RESTClient(RESTConfig)
     * @see RESTConfig#setDefaultProxy(String, int)
     * @see RESTConfig#setDefaultTrustAllCerts(boolean)
     */
    public RESTClient(String url) throws RESTException {
        this(new RESTConfig(url));
    }

    /**
     * Creates a RESTClient with the RESTConfig object.
     *
     * @param cfg config to use for sending request
     * @throws RESTException if unable to create a RESTClient
     * @see RESTConfig
     */
    public RESTClient(RESTConfig cfg) throws RESTException {
        this.headers = new HashMap<String, List<String>>();
        this.parameters = new HashMap<String, List<String>>();
        this.url = cfg.getURL();
        this.trustAllCerts = cfg.trustAllCerts();
        this.proxyHost = cfg.getProxyHost();
        this.proxyPort = cfg.getProxyPort();
    }

    /**
     * Adds parameter to be sent during http request.
     *
     * <p>
     * Does not remove any parameters with the same name, thus allowing
     * duplicates.
     * </p>
     *
     * @param name name of parameter
     * @param value value of parametr
     * @return a reference to 'this', which can be used for method chaining
     */
    public RESTClient addParameter(String name, String value) {
        if (!parameters.containsKey(name)) {
            parameters.put(name, new ArrayList<String>());
        }

        List<String> values = parameters.get(name);
        values.add(value);

        return this;
    }

    /**
     * Sets parameter to be sent during http request.
     *
     * <p>
     * Removes any parameters with the same name, thus disallowing duplicates.
     * </p>
     *
     * @param name name of parameter
     * @param value value of parametr
     * @return a reference to 'this', which can be used for method chaining
     */
    public RESTClient setParameter(String name, String value) {
        if (parameters.containsKey(name)) {
            parameters.get(name).clear();
        }

        addParameter(name, value);

        return this;
    }

    /**
     * Adds http header to be sent during http request.
     *
     * <p>
     * Does not remove any headers with the same name, thus allowing
     * duplicates.
     * </p>
     *
     * @param name name of header
     * @param value value of header
     * @return a reference to 'this', which can be used for method chaining
     */
    public RESTClient addHeader(String name, String value) {
        if (!headers.containsKey(name)) {
            headers.put(name, new ArrayList<String>());
        }

        List<String> values = headers.get(name);
        values.add(value);

        return this;
    }

    /**
     * Sets http header to be sent during http request.
     *
     * <p>
     * Does not remove any headers with the same name, thus allowing
     * duplicates.
     * </p>
     *
     * @param name name of header
     * @param value value of header
     * @return a reference to 'this', which can be used for method chaining
     */
    public RESTClient setHeader(String name, String value) {
        if (headers.containsKey(name)) {
            headers.get(name).clear();
        }

        addHeader(name, value);

        return this;
    }

    /**
     * Convenience method for adding the authorization header using the
     * specified OAuthToken object.
     *
     * @param token token to use for setting authorization
     * @return a reference to 'this,' which can be used for method chaining
     * @see #addAuthorizationHeader(String)
     */
    public RESTClient addAuthorizationHeader(OAuthToken token) {
        return addAuthorizationHeader(token.getAccessToken());
    }

    /**
     * Convenience method for adding the authorization header using the
     * specified access token.
     *
     * @param token token to use for setting authorization
     * @return a reference to 'this,' which can be used for method chaining
     */
    public RESTClient addAuthorizationHeader(String token) {
        this.addHeader("Authorization", "BEARER " + token);
        return this;
    }

    /**
     * Alias for httpGet().
     *
     * @return api response
     * @throws RESTException if request was unsuccessful
     * @see #httpGet()
     */
    public APIResponse get() throws RESTException {
        return httpGet();
    }

    /**
     * Sends an http GET request using the parameters and headers previously
     * set.
     *
     * @return api response
     * @throws RESTException if request was unsuccessful
     */
    public APIResponse httpGet() throws RESTException {
        HttpClient httpClient = null;
        HttpResponse response = null;

        try {
            httpClient = createClient();

            String query = "";
            if (!buildQuery().equals("")) {
                query = "?" + buildQuery();
            }
            HttpGet httpGet = new HttpGet(url + query);
            addInternalHeaders(httpGet);

            response = httpClient.execute(httpGet);

            APIResponse apiResponse = buildResponse(response);
            return apiResponse;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }

    /**
     * Alias for <code>httpPost()</code>.
     *
     * @see RESTClient#httpPost()
     * @return api response
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse post() throws RESTException {
        return httpPost();
    }

    /**
     * Sends an http POST request.
     *
     * <p>
     * POST body will be set to the values set using
     * <code>addParameter()</code> or <code>setParameter()</code>.
     * </p>
     *
     * @return api response
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse httpPost() throws RESTException {
        APIResponse response = httpPost(buildQuery());
        return response;
    }

    /**
     * Sends an http POST request using the specified body.
     *
     * <p>
     * <strong>NOTE</strong>: Any parameters set using
     * <code>addParameter()</code> or <code>setParameter()</code> will be
     * ignored.
     * </p>
     *
     * @param body string to use as POST body
     * @return api response
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse httpPost(String body) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(url);
            addInternalHeaders(httpPost);
            if (body != null && !body.equals("")) {
                httpPost.setEntity(new StringEntity(body));
            }

            response = httpClient.execute(httpPost);

            return buildResponse(response);
        } catch (IOException e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }

    /**
     * Sends an http POST request with the POST body set to the file.
     *
     * <p>
     * <strong>NOTE</strong>: Any parameters set using
     * <code>addParameter()</code> or <code>setParameter()</code> will be
     * ignored.
     * </p>
     *
     * @param file file to use as POST body
     * @return api response
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse httpPost(File file) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(url);
            addInternalHeaders(httpPost);

            String contentType = this.getMIMEType(file);

            httpPost.setEntity(new FileEntity(file, contentType));

            return buildResponse(httpClient.execute(httpPost));
        } catch (Exception e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }


    // TODO (pk9069): This should probably be moved to a util class
    /**
     * Gets MIME type for specified file.
     *
     * <p>
     * MIME type calculated by doing a very simple check based on file header.
     * </p>
     *
     * Currently supports checking for the following formats:
     * <ul>
     * <li>AMR</li>
     * <li>AMR-WB</li>
     * <li>WAV</li>
     * <li>Speex</li>
     * </ul>
     *
     * @param file file to check for MIME type
     * @return String MIME type
     * @throws IOException if there is a problem reading the specified file
     */
    private String getMIMEType(File file) throws IOException {
        // AMR/AMR-WB check will be done according to RFC3267
        // (http://www.ietf.org/rfc/rfc3267.txt?number=3267)
        final byte[] AMRHeader = {'#', '!', 'A', 'M', 'R'};
        final byte[] AMRWBExtension = {'-', 'W', 'B'};

        final byte[] RIFFHeader = {'R', 'I', 'F', 'F'};
        final byte[] WAVEHeader = {'W', 'A', 'V', 'E'};

        // Check for Speex in Ogg files. Ogg will be checked according to
        // RFC3533 (http://www.ietf.org/rfc/rfc3533.txt). Speex will be checked
        // according to the format specified the speex manual
        // (www.speex.org/docs/manual/speex-manual/node8.html)
        final byte[] OggHeader = {'O', 'g', 'g', 'S'};
        final byte[] SpeexHeader = {'S', 'p', 'e', 'e', 'x', ' ', ' ', ' '};

        final byte[] header = new byte[4];
        FileInputStream fStream = null;
        String contentType = null;
        try {
            fStream = new FileInputStream(file);
            // Read the first 4 bytes
            int bytesRead = fStream.read(header, 0, 4);

            if (bytesRead >= 4 && Arrays.equals(header, RIFFHeader)) {
                // read more bytes to determine if it's a wav file
                if (fStream.skip(4) >= 4) {  // size if wav structure
                    bytesRead = fStream.read(header, 0, 4); // wav header
                    if (bytesRead >= 4 && Arrays.equals(header, WAVEHeader)) {
                        contentType = "audio/wav";
                    }
                }
            } else if (Arrays.equals(header, OggHeader)
                    && fStream.skip(24) >= 24) {
                // first 28 bytes are ogg. Afterwards should be speex header.
                final byte[] headerExt = new byte[8];
                bytesRead = fStream.read(headerExt, 0, 8);
                if (bytesRead >= 8
                        && Arrays.equals(headerExt, SpeexHeader)) {
                    contentType = "audio/x-speex";
                }
            }

            // try looking for AMR
            final byte[] testHeader = new byte[5];
            for (int i = 0; i < header.length; ++i) {
                testHeader[i] = header[i];
            }
            bytesRead = fStream.read(testHeader, 4, 1);
            if (bytesRead >= 1 && Arrays.equals(testHeader, AMRHeader)) {
                final byte[] headerExt = new byte[3];
                bytesRead = fStream.read(headerExt, 0, 3);
                if (bytesRead >= 3
                        && Arrays.equals(headerExt, AMRWBExtension)) {
                    contentType = "audio/amr-wb";
                } else {
                    contentType = "audio/amr";
                }
            }
        } catch (IOException ioe) {
            throw ioe; // pass along exception
        } finally {
            if (fStream != null) { fStream.close(); }
        }

        return contentType;
    }

    /**
     * Sends an http POST multipart request.
     *
     * @param jsonObj JSON Object to set as the start part
     * @param fnames file names for any files to add
     * @return api response
     *
     * @throws RESTException if request was unsuccessful
     */
    public APIResponse httpPost(JSONObject jsonObj, String[] fnames)
            throws RESTException {

        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(url);
            this.setHeader("Content-Type",
                    "multipart/form-data; type=\"application/json\"; "
                    + "start=\"<startpart>\"; boundary=\"foo\"");
            addInternalHeaders(httpPost);

            final Charset encoding = Charset.forName("UTF-8");
            MultipartEntity entity =
                new MultipartEntity(HttpMultipartMode.STRICT, "foo", encoding);
            StringBody sbody = new StringBody(jsonObj.toString(),
                    "application/json", encoding);
            FormBodyPart stringBodyPart
                = new FormBodyPart("root-fields", sbody);
            stringBodyPart.addField("Content-ID", "<startpart>");
            entity.addPart(stringBodyPart);

            for (int i = 0; i < fnames.length; ++i) {
                final String fname = fnames[i];
                String type = URLConnection
                    .guessContentTypeFromStream(new FileInputStream(fname));

                if (type == null) {
                    type = URLConnection.guessContentTypeFromName(fname);
                }
                if (type == null) {
                    type = "application/octet-stream";
                }

                FileBody fb = new FileBody(new File(fname), type, "UTF-8");
                FormBodyPart fileBodyPart
                    = new FormBodyPart(fb.getFilename(), fb);

                fileBodyPart.addField(
                    "Content-ID", "<fileattachment" + i + ">"
                );

                fileBodyPart.addField("Content-Location", fb.getFilename());
                entity.addPart(fileBodyPart);
            }
            httpPost.setEntity(entity);
            return buildResponse(httpClient.execute(httpPost));
        } catch (Exception e) {
            throw new RESTException(e);
        } finally {
            if (response != null) { this.releaseConnection(response); }
        }
    }

    public APIResponse httpPost(String[] fnames, String subType,
            String[] bodyNameAttribute) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(url);
            this.setHeader("Content-Type",
                    "multipart/" + subType + "; " + "boundary=\"foo\"");
            addInternalHeaders(httpPost);

            final Charset encoding = Charset.forName("UTF-8");
            MultipartEntity entity = new MultipartEntity(
                    HttpMultipartMode.STRICT, "foo", encoding);

            for (int i = 0; i < fnames.length; ++i) {
                final String fname = fnames[i];
                String contentType = null;
                contentType = URLConnection
                        .guessContentTypeFromStream(new FileInputStream(fname));
                if (contentType == null) {
                    contentType = URLConnection.guessContentTypeFromName(fname);
                }
                if (contentType == null)
                    contentType = this.getMIMEType(new File(fname));
                if (fname.endsWith("grxml")) contentType = "application/srgs+xml";
                if (fname.endsWith("pls")) contentType="application/pls+xml";
                FileBody fb = new FileBody(new File(fname), contentType, "UTF-8");
                FormBodyPart fileBodyPart = new FormBodyPart(bodyNameAttribute[i], fb);
                fileBodyPart.addField("Content-ID", "<fileattachment" + i + ">");
                fileBodyPart.addField("Content-Location", fb.getFilename());
                if(contentType != null) {
                    fileBodyPart.addField("Content-Type", contentType);
                }
                entity.addPart(fileBodyPart);
            }
            httpPost.setEntity(entity);
            return buildResponse(httpClient.execute(httpPost));
        } catch (IOException e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }

    public APIResponse httpPut(String body) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            String query = "";
            if (!buildQuery().equals("")) {
                query = "?" + buildQuery();
            }
            HttpPut httpPut = new HttpPut(this.url + query);
            addInternalHeaders(httpPut);
            if (body != null && !body.equals("")) {
                httpPut.setEntity(new StringEntity(body));
            }

            response = httpClient.execute(httpPut);

            return buildResponse(response);
        } catch (IOException e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }

    public APIResponse httpDelete() throws RESTException {
        HttpClient httpClient = null;
        HttpResponse response = null;

        try {
            httpClient = createClient();

            HttpDelete httpDelete = new HttpDelete(this.url);

            addInternalHeaders(httpDelete);

            response = httpClient.execute(httpDelete);

            APIResponse apiResponse = buildResponse(response);
            return apiResponse;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }
}
