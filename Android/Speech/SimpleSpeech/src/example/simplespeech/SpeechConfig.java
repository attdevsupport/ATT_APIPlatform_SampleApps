/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simplespeech;

/** Configuration parameters for this application's account on Speech API. **/
public class SpeechConfig {
    private SpeechConfig() {} // can't instantiate
    
    /** The URL of AT&T Speech API. **/
    static String serviceUrl() {
        return "https://api.att.com/speech/v3/speechToText";
    }
        
    /** The URL of AT&T Speech API OAuth service. **/
    static String oauthUrl() {
        return "https://api.att.com/oauth/token";
    }
    
    /** The OAuth scope of AT&T Speech API. **/
    static String oauthScope() {
        return "SPEECH";
    }
    
    /** Unobfuscates the OAuth client_id credential for the application. **/
    static String oauthKey() {
        // TODO: Replace this with code to unobfuscate your OAuth client_id.
        return myUnobfuscate(MY_OBFUSCATED_CLIENT_ID);
    }

    /** Unobfuscates the OAuth client_secret credential for the application. **/
    static String oauthSecret() {
        // TODO: Replace this with code to unobfuscate your OAuth client_secret.
        return myUnobfuscate(MY_OBFUSCATED_CLIENT_SECRET);
    }
}
