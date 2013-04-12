//  SpeechConfig.h
//
// Declares customization parameters for this application's use of
// AT&T Speech SDK.

@class NSString, NSURL;

/** The URL of AT&T Speech API. **/
NSURL* SpeechServiceUrl(void);

/** The URL of AT&T Text to Speech service. **/
NSURL* TTSUrl(void);

/** The URL of AT&T Speech API OAuth service. **/
NSURL* SpeechOAuthUrl(void);

/** Unobfuscates the OAuth client_id credential for the application. **/
NSString* SpeechOAuthKey(void);

/** Unobfuscates the OAuth client_secret credential for the application. **/
NSString* SpeechOAuthSecret(void);

/** The OAuth scope for the Speech API requests. **/
NSString* SpeechOAuthScope(void);
