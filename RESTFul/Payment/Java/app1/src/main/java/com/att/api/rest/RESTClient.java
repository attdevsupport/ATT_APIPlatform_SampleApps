package com.att.api.rest;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
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
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.message.AbstractHttpMessage;
import org.apache.http.util.EntityUtils;

import com.att.api.oauth.OAuthToken;

public class RESTClient {
    private final boolean trustAllCerts;
    private final String proxyHost;
    private final int proxyPort;

    private final String URL;

    private final HashMap<String, List<String>> headers;
    private final HashMap<String, List<String>> parameters;

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

    private void releaseConnection(HttpResponse response) throws RESTException {
        try {
            EntityUtils.consume(response.getEntity());
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

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

    private void setProxyAttributes(HttpClient httpClient) {
        if (this.proxyHost != null && this.proxyPort != -1) {
            HttpHost proxy = new HttpHost(this.proxyHost, this.proxyPort);
            httpClient.getParams().setParameter(ConnRoutePNames.DEFAULT_PROXY, proxy);
        }
    }

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
    
    public RESTClient(RESTConfig cfg) throws RESTException {
        this.headers = new HashMap<String, List<String>>();
        this.parameters = new HashMap<String, List<String>>();
        this.URL = cfg.getURL();
        this.trustAllCerts = cfg.trustAllCerts();
        this.proxyHost = cfg.getProxyHost();
        this.proxyPort = cfg.getProxyPort();
    }

    public RESTClient addParameter(String name, String value) {
        if (!parameters.containsKey(name)) {
            parameters.put(name, new ArrayList<String>());
        }

        List<String> values = parameters.get(name);
        values.add(value);

        return this;
    }

    public RESTClient setParameter(String name, String value) {
        if (parameters.containsKey(name)) {
            parameters.get(name).clear();
        }

        addParameter(name, value);

        return this;
    }

    public RESTClient addHeader(String name, String value) {
        if (!headers.containsKey(name)) {
            headers.put(name, new ArrayList<String>());
        }

        List<String> values = headers.get(name);
        values.add(value);

        return this;
    }

    public RESTClient setHeader(String name, String value) {
        if (headers.containsKey(name)) {
            headers.get(name).clear();
        }

        addHeader(name, value);

        return this;
    }
    
    public RESTClient addAuthorizationHeader(OAuthToken token) {
        this.addHeader("Authorization", "Bearer " + token.getAccessToken());
        return this;
    }

    public APIResponse get() throws RESTException {
        return httpGet();
    }

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

    public APIResponse post() throws RESTException {
        return httpPost();
    }

    public APIResponse httpPost() throws RESTException {
            APIResponse response = httpPost(buildQuery()); 
            return response;
    }

    public APIResponse httpPost(String body) throws RESTException {
        HttpResponse response = null;
        try {   
            HttpClient httpClient = createClient();

            HttpPost httpPost = new HttpPost(this.URL);
            addInternalHeaders(httpPost);
            if (body != null && body != "") {
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

    public APIResponse httpPut(String body) throws RESTException {
        HttpResponse response = null;
        try {
            HttpClient httpClient = createClient();

            HttpPut httpPut = new HttpPut(this.URL);
            addInternalHeaders(httpPut);
            if (body != null && body != "") {
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


//    public APIResponse sendMultiPartRequest(String endpoint, List<String> files,
//            String accessToken, MultiValueMap valuePairs) throws IOException {
//        APIResponse apiResponse = new APIResponse();
//        HttpClient httpClient = createClient(apiResponse);
//        if (httpClient == null)
//            return apiResponse;
//
//        HttpPost post = new HttpPost(endpoint);
//        post.addHeader("Authorization", "BEARER " + accessToken);
//        post.addHeader(
//                "Content-Type",
//                "multipart/form-data; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"foo\"");
//        MultipartEntity entity = new MultipartEntity(HttpMultipartMode.STRICT, "foo",
//                Charset.forName("UTF-8"));
//        StringBody sbody = new StringBody(buildStringBodyPart(valuePairs),
//                "application/x-www-form-urlencoded", Charset.forName("UTF-8"));
//        FormBodyPart stringBodyPart = new FormBodyPart("root-fields", sbody);
//        stringBodyPart.addField("Content-ID", "<startpart>");
//        entity.addPart(stringBodyPart);
//
//        if (files != null) {
//            for (int i = 0; i < files.size(); ++i) {
//                final String fname = files.get(i);
//                String type = URLConnection.guessContentTypeFromStream(new FileInputStream(fname));
//                if (type == null) {
//                    type = URLConnection.guessContentTypeFromName(fname);
//                }
//                if (type == null)
//                    type = "application/octet-stream";
//                FileBody fb = new FileBody(new File(fname), type, "UTF-8");
//                FormBodyPart fileBodyPart = new FormBodyPart(fb.getFilename(), fb);
//                fileBodyPart.addField("Content-ID", "<fileattachment" + i + ">");
//                fileBodyPart.addField("Content-Location", fb.getFilename());
//                entity.addPart(fileBodyPart);
//            }
//        }
//        post.setEntity(entity);
//        HttpResponse responseBody = httpClient.execute(post);
//        return buildResponse(responseBody);
//    }

}
