/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simplespeech;

import java.net.URI;
import java.net.URISyntaxException;
import java.net.URLEncoder;
import java.util.Collections;
import java.util.List;

import com.att.android.speech.ATTSpeechError;
import com.att.android.speech.ATTSpeechError.ErrorType;
import com.att.android.speech.ATTSpeechErrorListener;
import com.att.android.speech.ATTSpeechResult;
import com.att.android.speech.ATTSpeechResultListener;
import com.att.android.speech.ATTSpeechService;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.TextView;

/**
 * SimpleSpeech is a very basic demonstration of using the ATTSpeechKit 
 * to do voice recognition.  It is designed to introduce a developer to making 
 * a new application that uses the AT&T SpeechKit Android library.  
 * It also documents some of the more basic Android methods for those developers 
 * that are new to Android as well.
 * 
 * This version of the sample code shows how to call ATTSpeechService.
**/
public class SimpleSpeechServiceDemo extends Activity {
    private Button speakButton = null;
    private TextView resultView = null;
    private WebView webView = null;
    private String oauthToken = null;
    
    /** 
     * Called when the activity is first created.  This is where we'll hook up 
     * our views in XML layout files to our application.
    **/
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        // First, we specify which layout resource we'll be using.
        setContentView(R.layout.speech);
        
        // This is the Speak button that the user presses to start a speech
        // interaction.
        speakButton = (Button)findViewById(R.id.speak_button);
        speakButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                startSpeechService();
            }
        });
        
        // This will show the recognized text.
        resultView = (TextView)findViewById(R.id.result);
        
        // This will show a website receiving the recognized text.
        webView = (WebView)findViewById(R.id.webview);
        configureWebView();
    }
    
    /** 
     * Called when the activity is coming to the foreground.
     * This is where we will fetch a fresh OAuth token.
    **/
    @Override
    protected void onStart() {
        super.onStart();
        
        // Fetch the OAuth credentials.  
        validateOAuth();
    }

    /** 
     * Called by the Speak button in the sample activity.
     * Starts the SpeechKit service that listens to the microphone and returns
     * the recognized text.
    **/
    private void startSpeechService() {
        // The ATTSpeechKit uses a singleton object to interface with the 
        // speech server.
        ATTSpeechService speechService = ATTSpeechService.getSpeechService(this);
        
        // Register for the success and error callbacks.
        speechService.setSpeechResultListener(new ResultListener());
        speechService.setSpeechErrorListener(new ErrorListener());
        // Next, we'll put in some basic parameters.
        // First is the Request URL.  This is the URL of the speech recognition 
        // service that you were given during onboarding.
        try {
            speechService.setRecognitionURL(new URI(SpeechConfig.serviceUrl()));
        }
        catch (URISyntaxException ex) {
            throw new IllegalArgumentException(ex);
        }
        
        // Specify the speech context for this app.
        speechService.setSpeechContext("WebSearch");
        
        // Set the OAuth token that was fetched in the background.
        speechService.setBearerAuthToken(oauthToken);
        
        // Add extra arguments for speech recognition.
        // The parameter is the name of the current screen within this app.
        speechService.setXArgs(
                Collections.singletonMap("ClientScreen", "main"));

        // Finally we have all the information needed to start the speech service.  
        speechService.startListening();
        Log.v("SimpleSpeech", "Starting speech interaction");
    }
    
    /**
     * This callback object will get all the speech success notifications.
    **/
    private class ResultListener implements ATTSpeechResultListener {
        public void onResult(ATTSpeechResult result) {
            // The hypothetical recognition matches are returned as a list of strings.
            List<String> textList = result.getTextStrings();
            String resultText = null;
            if (textList != null && textList.size() > 0) {
                // There may be multiple results, but this example will only use
                // the first one, which is the most likely.
                resultText = textList.get(0);
            }
            if (resultText != null && resultText.length() > 0) {
                // This is where your app will process the recognized text.
                Log.v("SimpleSpeech", "Recognized "+textList.size()+" hypotheses.");
                handleRecognition(resultText);
            }
            else {
                // The speech service did not recognize what was spoken.
                Log.v("SimpleSpeech", "Recognized no hypotheses.");
                alert("Didn't recognize speech", "Please try again.");
            }
        }
    }
    
    /** Make use of the recognition text in this app. **/
    private void handleRecognition(String resultText) {
        // In this example, we set display the text in the result view.
        resultView.setText(resultText);
        // And then perform a search on a website using the text.
        String query = URLEncoder.encode(resultText);
        String url = "http://en.m.wikipedia.org/w/index.php?search="+query;
        webView.loadUrl(url);
    }
    
    /** Configure the webview that displays websites with the recognition text. **/
    private void configureWebView() {
        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                return false; // Let the webview display the URL
            }
        });
    }

    /**
     * This callback object will get all the speech error notifications.
    **/
    private class ErrorListener implements ATTSpeechErrorListener {
        public void onError(ATTSpeechError error) {
            ErrorType resultCode = error.getType();
            if (resultCode == ErrorType.USER_CANCELED) {
                // The user canceled the speech interaction.
                // This can happen through several mechanisms:
                // pressing a cancel button in the speech UI;
                // pressing the back button; starting another activity;
                // or locking the screen.
                
                // In all these situations, the user was instrumental
                // in canceling, so there is no need to put up a UI alerting 
                // the user to the fact.
                Log.v("SimpleSpeech", "User canceled.");
            }
            else {
                // Any other value for the result code means an error has occurred.
                // The argument includes a message to help the programmer 
                // diagnose the issue.
                String errorMessage = error.getMessage();
                Log.v("SimpleSpeech", "Recognition error #"+resultCode+": "+errorMessage);
                
                alert("Speech Error", "Please try again later.");
            }
        }
    }

    /**
     * Start an asynchronous OAuth credential check. 
     * Disables the Speak button until the check is complete.
    **/
    private void validateOAuth() {
        SpeechAuth auth = 
            SpeechAuth.forService(SpeechConfig.oauthUrl(), SpeechConfig.oauthScope(), 
                SpeechConfig.oauthKey(), SpeechConfig.oauthSecret());
        auth.fetchTo(new OAuthResponseListener());
        speakButton.setText(R.string.speak_wait);
        speakButton.setEnabled(false);
    }
    
    /**
     * Handle the result of an asynchronous OAuth check.
    **/
    private class OAuthResponseListener implements SpeechAuth.Client {
        public void 
        handleResponse(String token, Exception error)
        {
            if (token != null) {
                oauthToken = token;
                speakButton.setText(R.string.speak);
                speakButton.setEnabled(true);
            }
            else {
                Log.v("SimpleSpeech", "OAuth error: "+error);
                // There was either a network error or authentication error.
                // Show alert for the latter.
                alert("Speech Unavailable", 
                    "This app was rejected by the speech service.  Contact the developer for an update.");
            }
        }
    }

    private void alert(String header, String message) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage(message)
            .setTitle(header)
            .setCancelable(true)
            .setPositiveButton(android.R.string.ok, new DialogInterface.OnClickListener() {
                @Override public void onClick(DialogInterface dialog, int which) {
                    dialog.dismiss();
                }
            });
        AlertDialog alert = builder.create();
        alert.show();
    }
}
