package com.att.api.rest;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLConnection;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.security.KeyManagementException;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.UnrecoverableKeyException;
import java.security.cert.X509Certificate;
import java.util.List;
import java.util.Map;

import org.apache.commons.collections.map.MultiValueMap;
import org.apache.http.Header;
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

public class RESTClient {
    private boolean trustAllCerts;
    private String proxyHost;
    private int proxyPort;

    public RESTClient(boolean trustAllCerts, String proxyHost, int proxyPort)
    {
        this.trustAllCerts = trustAllCerts;
        this.proxyHost = proxyHost;
        this.proxyPort = proxyPort;
    }

    private HttpClient createClient(APIResponse response) 
    {
        if (trustAllCerts) {
            // Trust all host certs. Only enable if on testing!
            SSLSocketFactory socketFactory = null;
            try {
                socketFactory = new SSLSocketFactory(
                        new TrustStrategy() {
                            public boolean isTrusted(final X509Certificate[] chain,
                                String authType) {
                                return true;
                            }
                        }, SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);
            } catch (KeyManagementException e) {
                response.setStatusCode(400);
                response.setReasonPhrase(e.getMessage());
            } catch (UnrecoverableKeyException e) {
                response.setStatusCode(400);
                response.setReasonPhrase(e.getMessage());
            } catch (NoSuchAlgorithmException e) {
                response.setStatusCode(400);
                response.setReasonPhrase(e.getMessage());
            } catch (KeyStoreException e) {
                response.setStatusCode(400);
                response.setReasonPhrase(e.getMessage());
            }
            SchemeRegistry registry = new SchemeRegistry();

            registry.register(new Scheme("http", 80, PlainSocketFactory
                        .getSocketFactory()));
            registry.register(new Scheme("https", 443, socketFactory));
            ThreadSafeClientConnManager cm = new ThreadSafeClientConnManager(
                    registry);
            DefaultHttpClient httpClient = new DefaultHttpClient(cm, new DefaultHttpClient().getParams());
            setProxyAttributes(httpClient);
            return httpClient;
        } else {
            HttpClient client = new DefaultHttpClient();
            setProxyAttributes(client);
            return client;
        }
    }

    private void setProxyAttributes(HttpClient httpClient)
    {
        if (this.proxyHost != null && this.proxyPort != 0) {
            HttpHost proxy = new HttpHost(this.proxyHost, this.proxyPort);
            httpClient.getParams().setParameter(ConnRoutePNames.DEFAULT_PROXY, proxy);
        }
    }

    /**
     * Send a Get request for the specific url.
     * @param url endpoint to make the HTTP request
     * @return HttpResponse from the Endpoint.
     */
    public APIResponse sendHttpGetRequest(String url, String accessToken)
    {
        return sendHttpGetRequest(url, accessToken, null);
    }

    /**
     * Send a Get request for the specific url.
     * @param url endpoint to make the HTTP request
     * @return HttpResponse from the Endpoint.
     */
    public APIResponse sendHttpGetRequest(String url, String accessToken, String cookie)
    {
        HttpResponse responseBody = null;

        APIResponse apiResponse = new APIResponse();
        HttpClient httpClient = createClient(apiResponse);
        if (httpClient == null) return apiResponse;

        try {
            httpClient.getParams().setBooleanParameter(ClientPNames.HANDLE_REDIRECTS, false);
            HttpGet httpGet = new HttpGet(url); 
            httpGet.addHeader("Authorization", "BEARER "+ accessToken);
            httpGet.addHeader("Accept", "application/json");
            httpGet.addHeader("Content_Type", "application/json");
            System.out.println(url);
            if (cookie != null) httpGet.setHeader("Cookie",cookie);
            responseBody = httpClient.execute(httpGet);
        } catch (Exception e) {
            apiResponse.setReasonPhrase(e.getMessage());
        } finally {
            apiResponse = buildResponse(httpClient,responseBody);
        }
        return apiResponse;
    }

    /**
     * Send a Post Request for a specific url.
     * @param url endpoint to make the HTTP request
     * @param nameValuePairs Name-Value pairs for the form data of the HTTP POST Request
     * @return HttpResponse from the Endpoint.
     */
    public APIResponse sendHttpPostRequest(MultiValueMap nameValuePairs,
            String url, MultiValueMap headers){
        APIResponse apiResponse = new APIResponse();
        HttpClient httpClient = createClient(apiResponse);
        if (httpClient == null) return apiResponse;

        HttpResponse responseBody = null;
        httpClient.getParams().setBooleanParameter(ClientPNames.HANDLE_REDIRECTS, false);
        HttpPost httpPost = new HttpPost(url); 
        try {
            // encode form data and set it as http POST entity.
            httpPost.setEntity(new StringEntity(buildStringBodyPart(nameValuePairs)));
            setHeaders(headers,httpPost);
            responseBody = httpClient.execute(httpPost);
        } catch(Exception e){
            apiResponse.setReasonPhrase(e.getMessage());
        } finally {
            apiResponse = buildResponse(httpClient,responseBody);
        }
        return apiResponse;
    }

    private void setHeaders(MultiValueMap headers, AbstractHttpMessage message) {
        for (Object entry : headers.entrySet()) {
            Map.Entry<String,List<String>> mapEntry = (Map.Entry<String, List<String>>) entry;
            for (String value : mapEntry.getValue()) {
                message.setHeader(mapEntry.getKey(),value);
            }
        }
    }

    private String buildStringBodyPart(MultiValueMap valuePairs) throws UnsupportedEncodingException {
        StringBuilder sbuilder = new StringBuilder();
        for (Object entry : valuePairs.entrySet()) {
            Map.Entry<String,List<String>> mapEntry = (Map.Entry<String, List<String>>) entry;
            for (String value : mapEntry.getValue()) {
                sbuilder.append(mapEntry.getKey()).append("=").append(URLEncoder.encode(value,"UTF-8"));
                sbuilder.append("&");
            }
        }
        return sbuilder.toString();
    }

    public APIResponse sendMultiPartRequest(String endpoint, List<String> files, String accessToken, MultiValueMap valuePairs) throws IOException
    {
        APIResponse apiResponse = new APIResponse();
        HttpClient httpClient = createClient(apiResponse);
        if (httpClient == null) return apiResponse;

        HttpPost post = new HttpPost(endpoint);
        post.addHeader("Authorization", "BEARER "+ accessToken);
        post.addHeader("Content-Type", "multipart/form-data; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"foo\"");
        MultipartEntity entity = new MultipartEntity(HttpMultipartMode.STRICT, "foo", Charset.forName("UTF-8"));
        StringBody sbody = new StringBody(buildStringBodyPart(valuePairs), "application/x-www-form-urlencoded",
                Charset.forName("UTF-8"));
        FormBodyPart stringBodyPart = new FormBodyPart("root-fields", sbody);
        stringBodyPart.addField("Content-ID", "<startpart>");
        entity.addPart(stringBodyPart);

        if (files != null) {
            for (int i = 0; i < files.size(); ++i) {
                final String fname = files.get(i);
                String type = URLConnection.guessContentTypeFromStream(new FileInputStream(fname));
                if (type == null){
                    type = URLConnection.guessContentTypeFromName(fname);
                }
                if (type == null) type="application/octet-stream";
                FileBody fb = new FileBody(new File(fname), type, "UTF-8");
                FormBodyPart fileBodyPart = new FormBodyPart(fb.getFilename(), fb);
                fileBodyPart.addField("Content-ID", "<fileattachment" + i + ">");
                fileBodyPart.addField("Content-Location", fb.getFilename());
                entity.addPart(fileBodyPart);
            }
        }
        post.setEntity(entity);
        HttpResponse responseBody = httpClient.execute(post);
        return buildResponse(httpClient,responseBody);
    }


    private APIResponse buildResponse(HttpClient httpClient, HttpResponse responseBody) {
        APIResponse apiResponse = new APIResponse();
        apiResponse.setHttpVersion(responseBody.getStatusLine().getProtocolVersion().toString());
        apiResponse.setReasonPhrase(responseBody.getStatusLine().getReasonPhrase());
        apiResponse.setStatusCode(responseBody.getStatusLine().getStatusCode());
        MultiValueMap headers = new MultiValueMap();
        for (Header header : responseBody.getAllHeaders()) {
            headers.put(header.getName(),header.getValue());
        }
        apiResponse.setResponseHeaders(headers);
        try {
            apiResponse.setRawBody(EntityUtils.toByteArray(responseBody.getEntity()));
        } catch (Exception e) {
            apiResponse.setReasonPhrase(e.getMessage());
            e.printStackTrace();
        } finally {
            httpClient.getConnectionManager().shutdown();
        }
        return apiResponse;
    }

    /**
     * Set attributes for HttpGet Headers.
     * @return httpGet: Initialized HttpGet object.
     */
    public void setDefaultHeaders(AbstractHttpMessage message){
        message.setHeader("Connection", "keep-alive");
        message.setHeader("Keep-Alive","timeout=5, max=100");
        message.setHeader("Cache-Control", "max-age=0");
        message.setHeader("Content-Type","application/x-www-form-urlencoded");
        message.setHeader("Accept-Charset","ISO-8859-1,utf-8;q=0.7,*/*;q=0.3");
        message.setHeader("Accept","text/html"); 
    }

    /**
     * Build the query with initial url FQDN
     * @param fqdn  fully qualified domain name.
     * @param names Names fetch from HTML form.
     * @param values Name specific values for the from the HTML form.
     * @return query string to be sent as a HTTP request.
     */
    public String buildQuery(String fqdn, String[] names, String[] values){
        StringBuilder sb = new StringBuilder();
        String charSet = "UTF-8";
        try {
            sb.append(fqdn);
            sb.append("?");
            sb.append(URLEncoder.encode(names[0], charSet));
            sb.append("=");
            sb.append(URLEncoder.encode(values[0], charSet));

            for(int i=1;i<names.length;i++){
                sb.append("&");
                sb.append(URLEncoder.encode(names[i], charSet));
                sb.append("=");
                sb.append(URLEncoder.encode(values[i], charSet));
            }
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        return sb.toString();
    }
}
