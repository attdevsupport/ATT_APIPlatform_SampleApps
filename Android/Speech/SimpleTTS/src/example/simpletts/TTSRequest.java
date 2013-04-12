/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simpletts;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;

import android.os.Handler;

/**
 * Posts to AT&T Text to Speech service and fetches audio response.
 * Performs the load over a separate thread so that it doesn't block the main UI.
**/
public class TTSRequest
{
    /** Supplies audio response back to client. **/
    public interface Client {
        /**
         * Called on client's thread to return audio data on success,
         * or an Exception on failure. 
         * @param audioData  the bytes of audio data on success, 
         *                   or null if TTS failed
         * @param error  null on success, 
         *               or the exception if TTS failed
        **/
        public void 
        handleResponse(byte[] audioData, Exception error);
    }
    
    /**
     * Sets up basic TTS request.
     * Follow this with a call to postText() to actually perform the TTS
     * conversion and fetch the data.
     * @param ttsService the URL of the Text to Speech
     * @param oauthToken the OAuth client credentials token for the application
     * @throws IllegalArgumentException for bad URL, etc.
    **/
    public static TTSRequest
    forService(String ttsService, String oauthToken)
        throws IllegalArgumentException
    {
        try {
            URL url = new URL(ttsService);
            HttpURLConnection request = (HttpURLConnection)url.openConnection();
            request.setRequestMethod("POST");
            request.setRequestProperty("Authorization", "BEARER "+oauthToken);
            request.setConnectTimeout(CONNECT_TIMEOUT);
            request.setReadTimeout(READ_TIMEOUT);
            // TODO: Add the other request properties needed by your application,
            //       such as response type, language, etc.
            return new TTSRequest(request);
        }
        catch (IOException e) {
            throw new IllegalArgumentException(e);
        }
        catch (ClassCastException e) {
            throw new IllegalArgumentException("URL must be HTTP: "+ttsService, e);
        }
    }

    /** Tune these timeout values based on application behavior. **/
    private static final int CONNECT_TIMEOUT = 5*1000; // milliseconds
    private static final int READ_TIMEOUT = 15*1000;

    /**
     * Create a loader for a URLConnection that returns the response data to a client. 
    **/
    private
    TTSRequest(HttpURLConnection request)
    {
        this.connection = request;
    }
    
    private Client client; // becomes null after cancel()
    private HttpURLConnection connection;
    private byte[] postData; // becomes null after getResponseStream()
    
    /**
     * Begin posting the text and fetching the audio response asynchronously.
     * This must be invoked on a thread with a Looper,
     * and the client will be called back on this thread.
     * @param text  the text to convert to audio
     * @param client  the callback to receive the response
    **/
    public void
    postText(String text, Client client)
    {
        this.client = client;
        try {
            this.postData = text.getBytes("UTF8");
        }
        catch (UnsupportedEncodingException e) {
            throw new RuntimeException(e);
        }
        connection.setRequestProperty("Content-Type", "text/plain");
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
            final byte[] data;
            try {
                data = readAllBytes(response);
            }
            finally {
                try {response.close(); } catch (IOException ex) {/* ignore */}
            }
            // Extract the token from JSON.
            // Give it back to the client.
            callingThread.post(new Runnable() {
                public void run() {
                    if (client != null)
                        client.handleResponse(data, null);
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
