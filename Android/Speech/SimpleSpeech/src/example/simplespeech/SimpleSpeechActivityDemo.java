/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simplespeech;

import java.net.URLEncoder;
import java.util.ArrayList;

import com.att.android.speech.ATTSpeechActivity;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
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
 * As with all apps that use the ATTSpeechActivity, make sure the manifest file 
 * includes a reference to the activity:
 *         <activity android:name="com.att.android.speech.ATTSpeechActivity"
 *          android:theme="@android:style/Theme.Translucent.NoTitleBar" />
**/
public class SimpleSpeechActivityDemo extends Activity {
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
                startSpeechActivity();
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
     * The request code is required to use the startActivityForResult method. 
     * It allows you to identify multiple requests from activities when they 
     * finish their work.  In this example, we only ever have a single child 
     * activity active, which we identify by an arbitrary constant.
    **/
    private static final int SPEECH_REQUEST_CODE = 42;
    
    /** 
     * Called by the Speak button in the sample activity.
     * Starts the SpeechKit activity that listens to the microphone and returns
     * the recognized text.
     */
    private void startSpeechActivity() {
        // The ATTSpeechKit uses its own activity to do speech recognition.  
        // We're going to call that by creating an Intent.
        // The intent takes two parameters -- the activity we're calling FROM 
        // and the class of the activity we want to call TO.
        Intent recognizerIntent = new Intent(this, ATTSpeechActivity.class);
        
        // Next, we'll put in some basic parameters.
        // First is the Request URL.  This is the URL of the speech recognition 
        // service that you were given during onboarding.
        recognizerIntent.putExtra(ATTSpeechActivity.EXTRA_RECOGNITION_URL, 
                SpeechConfig.serviceUrl());
        
        // Specify the speech context for this app.
        recognizerIntent.putExtra(ATTSpeechActivity.EXTRA_SPEECH_CONTEXT, 
                "WebSearch");
        
        // Set the OAuth token that was fetched in the background.
        recognizerIntent.putExtra(ATTSpeechActivity.EXTRA_BEARER_AUTH_TOKEN, 
                oauthToken);

        // Add extra arguments for speech recognition.
        // The parameter is the name of the current screen within this app.
        recognizerIntent.putExtra(ATTSpeechActivity.EXTRA_XARGS, 
                "ClientScreen=main");

        // Finally we have all the information needed to start the speech activity.  
        startActivityForResult(recognizerIntent, SPEECH_REQUEST_CODE);
        Log.v("SimpleSpeech", "Starting speech interaction");
    }
    
    /**
     * Since we use another activity to do the speech recognition, the results 
     * are sent back to our project as an "Activity Result". 
     * This method collects those results and do further processing.
    **/
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent resultData) {
        if (requestCode == SPEECH_REQUEST_CODE) {
            // The result code indicates whether or not the child activity was 
            // successful performing its task. 
            // In SpeechKit, if the speech processing occurred without errors, 
            // it returns the android.app.Activity.RESULT_OK result.
            if (resultCode == RESULT_OK) {
                // The results are sent back as intent "extras" and can be 
                // retrieved in a number of ways.  The simplest is to get a list 
                // of strings representing different possible results.
                ArrayList<String> textList = 
                        resultData.getStringArrayListExtra(ATTSpeechActivity.EXTRA_RESULT_TEXT_STRINGS);
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
            else if (resultCode == RESULT_CANCELED) {
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
                // A message to help the programmer diagnose the issue is  
                // returned as a string extra under the ERROR_MESSAGE key.
                String errorMessage = (resultData != null)
                        ? resultData.getStringExtra(ATTSpeechActivity.EXTRA_RESULT_ERROR_MESSAGE)
                            : "(no explanation)";
                Log.v("SimpleSpeech", "Recognition error #"+resultCode+": "+errorMessage);
                alert("Speech Error", "Please try again later.");
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