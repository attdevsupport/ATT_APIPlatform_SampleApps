/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simplespeechui;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Locale;

import org.json.JSONObject;

import android.os.Handler;

/**
 * Fetches OAuth client credentials, calling an interface when done.
 * Performs the load over a separate thread so that it doesn't block the main UI.
**/
public class SpeechAuth
{
    /** Supplies OAuth response back to client. **/
    public interface Client {
        /**
         * Called on client's thread to return OAuth token on success,
         * or an Exception on failure. 
         * @param token  the OAuth token on success, 
         *               or null if authentication failed
         * @param error  null on success, 
         *               or the exception if authentication failed
        **/
        public void 
        handleResponse(String token, Exception error);
    }
    
    /**
     * Sets up an OAuth client credentials authentication.
     * Follow this with a call to fetchTo() to actually load the data.
     * @param oauthService the URL of the OAuth client credentials service
     * @param apiKey the OAuth client ID
     * @param apiSecret the OAuth client secret
     * @throws IllegalArgumentException for bad URL, etc.
    **/
    public static SpeechAuth
    forService(String oauthService, String oauthScope,
               String apiKey, String apiSecret)
        throws IllegalArgumentException
    {
        try {
            URL url = new URL(oauthService);
            HttpURLConnection request = (HttpURLConnection)url.openConnection();
            String data = String.format(Locale.US, OAUTH_DATA, 
                    oauthScope, apiKey, apiSecret);
            byte[] bytes = data.getBytes("UTF8");
            request.setConnectTimeout(CONNECT_TIMEOUT);
            request.setReadTimeout(READ_TIMEOUT);
            return new SpeechAuth(request, bytes);
        }
        catch (IOException e) {
            throw new IllegalArgumentException(e);
        }
        catch (ClassCastException e) {
            throw new IllegalArgumentException("URL must be HTTP: "+oauthService, e);
        }
    }

    private static final String OAUTH_DATA = 
            "grant_type=client_credentials&scope=%s&client_id=%s&client_secret=%s";
    /** The OAuth server is quite quick.  The only timeouts will be for network failures. **/
    private static final int CONNECT_TIMEOUT = 5*1000; // milliseconds
    private static final int READ_TIMEOUT = 5*1000;

    /**
     * Create a loader for a URLConnection that returns the response data to a client. 
    **/
    private
    SpeechAuth(HttpURLConnection request, byte[] postData)
    {
        this.connection = request;
        this.postData = postData;
    }
    
    private Client client; // becomes null after cancel()
    private HttpURLConnection connection;
    private byte[] postData; // becomes null after getResponseStream()
    
    /**
     * Begin fetching the credentials asynchronously.
     * This must be invoked on a thread with a Looper,
     * and the client will be called back on this thread.
    **/
    public void
    fetchTo(Client client)
    {
        this.client = client;
        // TODO: prevent starting twice
        final Handler callingThread = new Handler();
        Thread reader = new Thread(connection.getURL().toString()) {
            @Override public void run() {
                performFetch(callingThread);
            }
        };
        reader.start();
    }
    
    /**
     * Stop the loading.  
     * This should only be called from the same thread that called start().
    **/
    public void
    cancel()
    {
        // TODO: figure out a way to implement cancel()
        client = null;
        //connection.disconnect();
    }
    
    /**
     * Posts request data, gets bytes of response, and calls client when done.
     * Performs blocking I/O, so it must be called from its own thread.
    **/
    private void
    performFetch(Handler callingThread)
    {
        try {
            // Post the credentials.
            connection.setDoOutput(true);
            OutputStream out = connection.getOutputStream();
            out.write(postData);
            out.close();
            // Wait for the response.  
            // Note that getInputStream will throw exception for non-200 status.
            InputStream response = connection.getInputStream();
            byte[] data = readAllBytes(response);
            // Extract the token from JSON.
            String body = new String(data, "UTF8");
            JSONObject json = new JSONObject(body);
            final String token = json.getString("access_token");
            // Give it back to the client.
            callingThread.post(new Runnable() {
                public void run() {
                    if (client != null)
                        client.handleResponse(token, null);
                }
            });
        }
        catch (final Exception ex) {
            callingThread.post(new Runnable() {
                public void run() {
                    if (client != null)
                        client.handleResponse(null, ex);
                }
            });
        }
        // XXX: Should we close the connection?
        // finally { connection.close(); }
    }
    
    private static final int BLOCK_SIZE = 16*1024;
    
    /**
     * Reads the stream until EOF.  Returns all the bytes read.
     * @param input stream to read
     * @param maxLength maximum number of bytes to read
     * @return all the bytes from the steam
     * @throws IOException 
     * @throws InputTooLargeException when length exceeds 
    **/
    private static byte[]
    readAllBytes(InputStream input)
        throws IOException
    {
        ByteArrayOutputStream buffer = new ByteArrayOutputStream(BLOCK_SIZE);
        byte[] block = new byte[BLOCK_SIZE];
        int n;
        while ((n = input.read(block)) > 0)
            buffer.write(block, 0, n);
        return buffer.toByteArray();
    }
}
