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
import java.io.UnsupportedEncodingException;
import java.net.URLConnection;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.security.cert.X509Certificate;
import java.util.*;

import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
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
import org.apache.http.util.EntityUtils;
import org.json.JSONObject;

import com.att.api.oauth.OAuthToken;

/**
 * Client used to send RESTFul requests.
 * <p>
 * Many of the methods return a reference to the 'this,' thereby allowing 
 * method chaining. 
 * <br>
 * An example of usage can be found below:
 * <pre>
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
 * </pre>
 *
 * @version 2.2
 * @since 2.2
 */
public class RESTClient {
    private final boolean trustAllCerts;
    private final String proxyHost;
    private final int proxyPort;

    private final String URL;

    private final HashMap<String, List<String>> headers;
    private final HashMap<String, List<String>> parameters;
    private static final HashMap<String,String> extMimeTypeMap;

    static {
        extMimeTypeMap = new HashMap<String, String>();
        extMimeTypeMap.put("grxml","application/srgs+xml");
        extMimeTypeMap.put("pls","application/pls+xml");
    }

    /**
     * Internal method used to build an APIResponse using the specified 
     * HttpResponse object.
     *
     * @param response response wrapped inside an APIResponse object
     * @return api response
     *
     * @throws RESTException if api request was unsuccessful (http status code 
     * was neither 200 nor 201)
     */
    private APIResponse buildResponse(HttpResponse response) 
            throws RESTException {

        APIResponse apiResponse = new APIResponse(response);
        int statusCode = apiResponse.getStatusCode();
        String responseBody = apiResponse.getResponseBody();

        // request was not successful, throw an exception with the status 
        // code and response body
        if (statusCode != 200 && statusCode != 201) {
            throw new RESTException(statusCode, responseBody);
        }
        
        return apiResponse;
    }

    /**
     * Used to release any resources used by the connection.
     * @param response HttpResponse object used for releasing the connection
     * @throws RESTException if unable to release connection
     *
     */
    private void releaseConnection(HttpResponse response) throws RESTException {
        try {
            EntityUtils.consume(response.getEntity());
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
     * Builds the query part of a URL.
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
                for(final String value : values) {
                    sb.append(URLEncoder.encode(name, charSet));
                    sb.append("=");
                    sb.append(URLEncoder.encode(value, charSet));
                }
            }
        } catch (UnsupportedEncodingException e) {
            // should not occur
            e.printStackTrace();
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
            httpClient.getParams().setParameter(ConnRoutePNames.DEFAULT_PROXY, proxy);
        }
    }

 
    /**
     * Creates an http client that can be used for sending http requests.
     *
     * @return created http client
     *
     * @throws RESTException if unable to create http client.
     */
    private HttpClient createClient() throws RESTException {
        DefaultHttpClient client;

        if (trustAllCerts) {
            // Trust all host certs. Only enable if on testing!
            SSLSocketFactory socketFactory = null;
            try {
                socketFactory = new SSLSocketFactory(new TrustStrategy() {
                    public boolean isTrusted(final X509Certificate[] chain, String authType) {
                        return true;
                    }
                }, SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);
            } catch (Exception e) {
                // shouldn't occur, but just in case
                final String msg = e.getMessage();
                throw new RESTException("Unable to create HttpClient. " + msg); 
            }

            SchemeRegistry registry = new SchemeRegistry();

            registry.register(new Scheme("http", 80, PlainSocketFactory.getSocketFactory()));
            registry.register(new Scheme("https", 443, socketFactory));
            ThreadSafeClientConnManager cm = new ThreadSafeClientConnManager(registry);
            client = new DefaultHttpClient(cm, new DefaultHttpClient().getParams());
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
     * @param URL URL to send request to
     * @param proxyHost proxy host to use for sending request
     * @param proxyPort proxy port to use for sendin request
     *
     * @throws RESTException if unable to create a RESTClient
     */
    public RESTClient(String URL, String proxyHost, int proxyPort) 
            throws RESTException {
        this(new RESTConfig(URL, proxyHost, proxyPort));
    }

    /**
     * Creates a RESTClient with the specified URL. No proxy host nor port will
     * be used. 
     *
     * @param URL URL to send request to
     *
     * @throws RESTException if unable to create a RESTClient
     */
    public RESTClient(String URL) throws RESTException {
        this(new RESTConfig(URL));
    }
    
    /**
     * Creates a RESTClient with the RESTConfig object.
     *
     * @param  cfg to use for sending request
     *
     * @throws RESTException if unable to create a RESTClient
     */
    public RESTClient(RESTConfig cfg) throws RESTException {
        this.headers = new HashMap<String, List<String>>();
        this.parameters = new HashMap<String, List<String>>();
        this.URL = cfg.getURL();
        this.trustAllCerts = cfg.trustAllCerts();
        this.proxyHost = cfg.getProxyHost();
        this.proxyPort = cfg.getProxyPort();
    }

    /**
     * Adds parameter to be sent during http request.
     * <p>
     * Does not remove any parameters with the same name, thus allowing 
     * duplicates.
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
     * <p>
     * Removes any parameters with the same name, thus disallowing duplicates.
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
     * <p>
     * Does not remove any headers with the same name, thus allowing 
     * duplicates.
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
     * <p>
     * Does not remove any headers with the same name, thus allowing 
     * duplicates.
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
     * @return a reference to 'this', which can be used for method chaining
     */
    public RESTClient addAuthorizationHeader(OAuthToken token) {
        this.addHeader("Authorization", "Bearer " + token.getAccessToken());
        return this;
    }

    /**
     * Alias for httpGet().
     *
     * @see RESTClient#httpGet()
     */
    public APIResponse get() throws RESTException {
        return httpGet();
    }

    /**
     * Sends an http GET request using the parameters and headers previously
     * set.
     *
     * @return api response
     *
     * @throws RESTException if request was unsuccessful
     */
    public APIResponse httpGet() throws RESTException {
        HttpClient httpClient = null;
        HttpResponse response = null;

        try {
            httpClient = createClient();

            HttpGet httpGet = new HttpGet(this.URL + "?" + buildQuery());
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
     * Alias for httpPost()
     *
     * @see RESTClient#httpPost()
     */
    public APIResponse post() throws RESTException {
        return httpPost();
    }

    /**
     * Sends an http POST request.
     * <p>
     * POST body will be set to the values set using add/setParameter()
     *
     * @return api response
     *
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse httpPost() throws RESTException {
            APIResponse response = httpPost(buildQuery()); 
            return response;
    }

    /**
     * Sends an http POST request using the specified body.
     *
     * @return api response
     *
     * @throws RESTException if POST was unsuccessful
     */
    public APIResponse httpPost(String body) throws RESTException {
        HttpResponse response = null;
        try {   
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(this.URL);
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

            HttpPost httpPost = new HttpPost(this.URL);
            this.setHeader("Content-Type",
                    "multipart/form-data; type=\"application/json\"; " 
                    + "start=\"<startpart>\"; boundary=\"foo\"");
            addInternalHeaders(httpPost);

            final Charset encoding = Charset.forName("UTF-8");
            MultipartEntity entity = 
                new MultipartEntity(HttpMultipartMode.STRICT, "foo", encoding);
            StringBody sbody 
                = new StringBody(jsonObj.toString(), "application/json", encoding);
            FormBodyPart stringBodyPart = new FormBodyPart("root-fields", sbody);
            stringBodyPart.addField("Content-ID", "<startpart>");
            entity.addPart(stringBodyPart);

            for (int i = 0; i < fnames.length; ++i) {
                final String fname = fnames[i];
                String type = URLConnection
                    .guessContentTypeFromStream(new FileInputStream(fname));
                if (type == null) {
                    type = URLConnection.guessContentTypeFromName(fname);
                }
                if (type == null)
                    type = "application/octet-stream";

                FileBody fb = new FileBody(new File(fname), type, "UTF-8");
                FormBodyPart fileBodyPart = new FormBodyPart(fb.getFilename(), fb);
                fileBodyPart.addField("Content-ID", "<fileattachment" + i + ">");
                fileBodyPart.addField("Content-Location", fb.getFilename());
                entity.addPart(fileBodyPart);
            }
            httpPost.setEntity(entity);
            return buildResponse(httpClient.execute(httpPost));
        } catch (Exception e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }


    public APIResponse httpPost(String [] fnames, String subType, String [] bodyNameAttribute) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(this.URL);
            this.setHeader("Content-Type",
                    "multipart/" + subType + "; "
                            + "boundary=\"foo\"");
            addInternalHeaders(httpPost);

            final Charset encoding = Charset.forName("UTF-8");
            MultipartEntity entity =
                    new MultipartEntity(HttpMultipartMode.STRICT, "foo", encoding);

            for (int i = 0; i < fnames.length; ++i) {
                final String fname = fnames[i];
                String contentType = null;
                contentType = URLConnection
                        .guessContentTypeFromStream(new FileInputStream(fname));
                if (contentType == null) {
                    contentType = URLConnection.guessContentTypeFromName(fname);
                }
                if (contentType == null){
                    contentType = this.getMIMEType(new File(fname));
                }
                if (contentType == null && fname.lastIndexOf(".") !=-1) {
                    contentType = extMimeTypeMap.get(fname.substring(fname.lastIndexOf(".")+1));
                }
                if (contentType == null) throw new RESTException(-1,"Unable to determine content type");

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
        } catch (Exception e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }


    // TODO: Move to another class?
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
        FileInputStream fStream = null;
        String contentType = null;
        try {
            fStream = new FileInputStream(file);
            // Read the first 4 bytes
            int bytesRead = fStream.read(header, 0, 4);

            if (bytesRead >= 4 && Arrays.equals(header, RIFFHeader)) {
                // read more bytes to determine if it's a wav file
                if(fStream.skip(4) >= 4) {  // size if wav structure
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
            if (fStream != null) fStream.close();
        }
        return contentType;
    }

    public APIResponse httpPost(File file, boolean  chunked) throws RESTException {

        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(this.URL);
            addInternalHeaders(httpPost);

            String contentType = this.getMIMEType(file);
            FileEntity fileEntity = new FileEntity(file, contentType);
            fileEntity.setChunked(chunked);
            httpPost.setEntity(fileEntity);
            return buildResponse(httpClient.execute(httpPost));
        } catch (Exception e) {
            throw new RESTException(e);
        } finally {
            if (response != null) {
                this.releaseConnection(response);
            }
        }
    }

}
