/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simpletts;

import java.net.URI;
import java.net.URISyntaxException;
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
import android.widget.Button;
import android.widget.TextView;

/**
 * SimpleTTS is a basic demonstration of using the ATTSpeechKit to do speech 
 * recognition (speech-to-text) and the AT&T Speech API for text-to-speech.  
 * It is designed to introduce a developer to making 
 * a new application that uses the AT&T SpeechKit Android library.  
 * It also documents some of the more basic Android methods for those developers 
 * that are new to Android as well.
**/
public class SimpleTTSDemo extends Activity 
{
    private AudioPlayer audioPlayer = null;
    private TTSClient ttsClient = null;
    private Button speakButton = null;
    private TextView resultView = null;
    private String oauthToken = null;
    
    /** 
     * Called when the activity is first created.  This is where we'll hook up 
     * our views in XML layout files to our application.
    **/
    @Override public void 
    onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        
        // First, we specify which layout resource we'll be using.
        setContentView(R.layout.speech);
        
        // A simple UI-less player for the TTS audio.
        audioPlayer = new AudioPlayer(this);
        
        // This is the Speak button that the user presses to start a speech
        // interaction.
        speakButton = (Button)findViewById(R.id.speak_button);
        speakButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                stopTTS(); // don't let playback interfere with microphone
                startSpeechService();
            }
        });
        
        // This will show the recognized text.
        resultView = (TextView)findViewById(R.id.result);
    }
    
    /** 
     * Called when the activity is coming to the foreground.
     * This is where we will fetch a fresh OAuth token and
     * then speak the greeting.
    **/
    @Override protected void 
    onStart() 
    {
        super.onStart();
        // Fetch the OAuth credentials.  
        validateOAuth();
    }
    
    
    /**
     * Called when the activity is leaving the foreground.
     * Stops any audio playback.
    **/
    @Override protected void 
    onPause()
    {
        super.onPause();
        stopTTS();
    }

    /**
     * When the app is authenticated with the Speech API, 
     * enable the interface and speak out a greeting.
    **/
    private void 
    readyForSpeech() 
    {
        // First enable the speech buttons.
        speakButton.setText(R.string.speak_button);
        speakButton.setEnabled(true);
        // Make Text to Speech request that will speak out a greeting.
        startTTS(getString(R.string.greeting));
    }
    
    /**
     * Start a TTS request to speak the argument.
    **/
    private void
    startTTS(String textToSpeak)
    {
        TTSRequest tts = TTSRequest.forService(SpeechConfig.ttsUrl(), oauthToken);
        ttsClient = new TTSClient();
        tts.postText(textToSpeak, ttsClient);
    }
    
    /**
     * Stops any Text to Speech in progress.
    **/
    private void
    stopTTS()
    {
        if (ttsClient != null)
            ttsClient.cancel = true;
        audioPlayer.stop();
    }
    
    /**
     * This callback object will get the TTS responses.
    **/
    private class TTSClient implements TTSRequest.Client 
    {
        @Override public void 
        handleResponse(byte[] audioData, Exception error) 
        {
            if (cancel)
                return;
            if (audioData != null) {
                Log.v("SimpleTTS", "Text to Speech returned "+audioData.length+" of audio.");
                audioPlayer.play(audioData);
            }
            else {
                // The TTS service was not able to generate the audio.
                Log.v("SimpleTTS", "Unable to convert text to speech.", error);
                // Real applications probably shouldn't display an alert.
                alert(null, "Unable to convert text to speech.");
            }
        }
        /** Set to true to prevent playing. **/
        boolean cancel = false;
    }
    
    /** 
     * Called by the Speak button in the sample activity.
     * Starts the SpeechKit service that listens to the microphone and returns
     * the recognized text.
    **/
    private void 
    startSpeechService() 
    {
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
            speechService.setRecognitionURL(new URI(SpeechConfig.recognitionUrl()));
        }
        catch (URISyntaxException ex) {
            throw new IllegalArgumentException(ex);
        }
        
        // Specify the speech context for this app.
        speechService.setSpeechContext("QuestionAndAnswer");
        
        // Set the OAuth token that was fetched in the background.
        speechService.setBearerAuthToken(oauthToken);
        
        // Add extra arguments for speech recognition.
        // The parameter is the name of the current screen within this app.
        speechService.setXArgs(
                Collections.singletonMap("ClientScreen", "main"));

        // Finally we have all the information needed to start the speech service.  
        speechService.startListening();
        Log.v("SimpleTTS", "Starting speech interaction");
    }
    
    /**
     * This callback object will get all the speech success notifications.
    **/
    private class ResultListener implements ATTSpeechResultListener 
    {
        public void 
        onResult(ATTSpeechResult result) 
        {
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
                Log.v("SimpleTTS", "Recognized "+textList.size()+" hypotheses.");
                handleRecognition(resultText);
            }
            else {
                // The speech service did not recognize what was spoken.
                Log.v("SimpleTTS", "Recognized no hypotheses.");
                alert("Didn't recognize speech", "Please try again.");
            }
        }
    }
    
    /** Make use of the recognition text in this app. **/
    private void 
    handleRecognition(String resultText) 
    {
        // In this example, we set display the text in the result view.
        resultView.setText(resultText);
        // And then we speak a response.
        String response = getString(R.string.response_format, resultText);
        startTTS(response);
    }
    
    /**
     * This callback object will get all the speech error notifications.
    **/
    private class ErrorListener implements ATTSpeechErrorListener 
    {
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
                Log.v("SimpleTTS", "User canceled.");
            }
            else {
                // Any other value for the result code means an error has occurred.
                // The argument includes a message to help the programmer 
                // diagnose the issue.
                String errorMessage = error.getMessage();
                Log.v("SimpleTTS", "Recognition error #"+resultCode+": "+errorMessage, 
                        error.getException());
                alert("Speech Error", "Please try again later.");
            }
        }
    }

    /**
     * Start an asynchronous OAuth credential check. 
     * Disables the Speak button until the check is complete.
    **/
    private void 
    validateOAuth() 
    {
        SpeechAuth auth = 
            SpeechAuth.forService(SpeechConfig.oauthUrl(), SpeechConfig.oauthScope(), 
                SpeechConfig.oauthKey(), SpeechConfig.oauthSecret());
        auth.fetchTo(new OAuthResponseListener());
        speakButton.setText(R.string.speak_button_wait);
        speakButton.setEnabled(false);
    }
    
    /**
     * Handle the result of an asynchronous OAuth check.
    **/
    private class OAuthResponseListener implements SpeechAuth.Client 
    {
        public void 
        handleResponse(String token, Exception error)
        {
            if (token != null) {
                oauthToken = token;
                readyForSpeech();
            }
            else {
                Log.v("SimpleTTS", "OAuth error: "+error);
                // There was either a network error or authentication error.
                // Show alert for the latter.
                alert("Speech Unavailable", 
                    "This app was rejected by the speech service.  Contact the developer for an update.");
            }
        }
    }

    private void 
    alert(String header, String message) 
    {
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
