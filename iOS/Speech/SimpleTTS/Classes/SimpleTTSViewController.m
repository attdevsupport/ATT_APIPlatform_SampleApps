//  SimpleTTSViewController.m
//  SimpleTTS
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleTTSViewController.h"
#import <AVFoundation/AVAudioPlayer.h>
#import <AVFoundation/AVAudioSession.h>
#import "SpeechConfig.h"
#import "SpeechAuth.h"
#import "TTSRequest.h"

/**
 * SimpleTTS is a basic demonstration of using the ATTSpeechKit to do speech
 * recognition (speech-to-text) and the AT&T Speech API for text-to-speech.
 * It is designed to introduce a developer to making
 * a new application that uses the AT&T SpeechKit iOS library.
**/
@interface SimpleTTSViewController ()
@property (retain,nonatomic) AVAudioPlayer* audioPlayer;
@property (retain,nonatomic) NSString* ttsInProgress;
@property (retain,nonatomic) NSString* oauthToken;
- (void) validateOAuthForService: (ATTSpeechService*) speechService;
- (void) readyForSpeech;
- (void) speechAuthFailed: (NSError*) error;
- (void) startTTS: (NSString*) textToSpeak;
@end

@implementation SimpleTTSViewController

@synthesize textView;
@synthesize talkButton;
@synthesize audioPlayer;
@synthesize oauthToken;

#pragma mark -
#pragma mark Lifecyle

- (void) dealloc
{
    self.textView = nil;
    self.talkButton = nil;
    self.audioPlayer = nil;
    self.ttsInProgress = nil;
    [super dealloc];
}


/**
 * Initialize SpeechKit for this app.
**/
- (void) prepareSpeech
{
    // Set up this application for audio output.
    NSError* error = nil;
    [[AVAudioSession sharedInstance] setCategory: AVAudioSessionCategoryPlayback error: &error];
    if (error != nil) {
        NSLog(@"Not able to initialize audio session for playback: %@", error);
    }
    
    // Access the SpeechKit singleton.
    ATTSpeechService* speechService = [ATTSpeechService sharedSpeechService];
    
    // Point to the SpeechToText API.
    speechService.recognitionURL = SpeechServiceUrl();
    
    // Hook ourselves up as a delegate so we can get called back with the response.
    speechService.delegate = self;
    
    // Use default speech UI.
    speechService.showUI = YES;
    
    // Choose the speech recognition package.
    speechService.speechContext = @"QuestionAndAnswer";
    
    // Start the OAuth background operation, disabling the Talk button until 
    // it's done.
    [self validateOAuthForService: speechService];

    // Wake the audio components so there is minimal delay on the first request.
    [speechService prepare];
}


- (BOOL) shouldAutorotateToInterfaceOrientation: (UIInterfaceOrientation) interfaceOrientation
{
    return YES;
}

#pragma mark -
#pragma mark Speech Actions


/**
 * When the app is authenticated with the Speech API,
 * enable the interface and speak out a greeting.
**/
- (void) readyForSpeech
{
    // First enable the speech buttons.
    talkButton.enabled = YES;
    // Make Text to Speech request that will speak out a greeting.
    [self startTTS: NSLocalizedString(@"Say Something", @"Greeting")];
}

/**
 * Start a TTS request to speak the argument.
**/
- (void) startTTS: (NSString*) textToSpeak
{
    TTSRequest* tts = [TTSRequest forService: TTSUrl() withOAuth: self.oauthToken];
    self.ttsInProgress = textToSpeak;
    [tts postText: textToSpeak forClient: ^(NSData* audioData, NSError* error) {
        if (![textToSpeak isEqualToString: self.ttsInProgress]) {
            // TTS was canceled, so don't play it back.
        }
        else if (audioData != nil) {
            NSLog(@"Text to Speech returned %d bytes of audio.", audioData.length);
            [self playAudioData: audioData];
        }
        else {
            NSLog(@"Unable to convert text to speech: %@", error);
            // Real applications probably shouldn't display an alert.
            [self showAlertWithTitle: @"An error occurred"
                             message: @"Unable to convert text to speech."];
        }
    }];
}

/**
 * End TTS retrieval and playback.
**/
- (void) stopTTS
{
    // Cancel any TTS request in progress.
    self.ttsInProgress = nil;
    // Stop audio playback.
    [self stopPlaying];
}

/**
 * Play the audio data.
**/
- (void) playAudioData: (NSData*) audioData
{
    [self stopPlaying];
    NSError* error = nil;
    // Set up this application for audio output.
    // We have to do this after microphone input, because otherwise the OS
    // will route audio to the phone receiver, not the speaker.
    [[AVAudioSession sharedInstance] setCategory: AVAudioSessionCategoryPlayback error: &error];
    if (error != nil) {
        NSLog(@"Not able to set audio session for playback: %@", error);
    }
    AVAudioPlayer* newPlayer = [[AVAudioPlayer alloc] initWithData: audioData error: &error];
    if (newPlayer == nil) {
        NSLog(@"Unable to play TTS audio data: %@", error);
        // Real applications probably shouldn't display an alert.
        [self showAlertWithTitle: @"An error occurred"
                         message: @"Unable to play audio."];
    }
    [newPlayer play];
    self.audioPlayer = newPlayer;
    [newPlayer release];
}

/**
 * Stop playing audio data.
**/
- (void) stopPlaying
{
    AVAudioPlayer* oldPlayer = self.audioPlayer;
    if (oldPlayer != nil) {
        [oldPlayer stop];
        self.audioPlayer = nil;
    }
}

/**
 * Perform the action of the "Push to talk" button.
**/
- (IBAction) listen: (id) sender
{
    // Don't let TTS playback interfere with audio capture.
    [self stopTTS];
    
    NSLog(@"Starting speech request");
    
    // Start listening via the microphone.
    ATTSpeechService* speechService = [ATTSpeechService sharedSpeechService];

    // Add extra arguments for speech recogniton.
    // The parameter is the name of the current screen within this app.
    speechService.xArgs =
        [NSDictionary dictionaryWithObjectsAndKeys:
         @"main", @"ClientScreen", nil];

    [speechService startListening];
}

/**
 * Make use of the recognition text in this app.
**/
- (void) handleRecognition: (NSString*) recognizedText
{
    // Display the recognized text.
    [self.textView setText: recognizedText];
    
    // Speak a response.
    NSString* response =
        [NSString stringWithFormat: NSLocalizedString(@"Answer %@", @"Answer"),
         recognizedText];
    [self startTTS: response];
}

#pragma mark -
#pragma mark Speech Service Delegate Methods

/**
 * The AT&T Speech to Text service returned a result.
**/
- (void) speechServiceSucceeded: (ATTSpeechService*) speechService
{
    NSLog(@"Speech service succeeded");
    
    // Extract the needed data from the SpeechService object:
    // For raw bytes, read speechService.responseData.
    // For a JSON tree, read speechService.responseDictionary.
    // For the n-best ASR strings, use speechService.responseStrings.
    
    // In this example, use the ASR strings.
    // There can be 0 strings, 1 empty string, or 1 non-empty string.
    // Display the recognized text in the interface is it's non-empty,
    // otherwise have the user try again.
    NSArray* nbest = speechService.responseStrings;
    NSString* recognizedText = @"";
    if (nbest != nil && nbest.count > 0)
        recognizedText = [nbest objectAtIndex: 0];
    if (recognizedText.length) { // non-empty?
        [self handleRecognition: recognizedText];
    }
    else {
        [self showAlertWithTitle: @"Didn't recognize speech"
                         message: @"Please try again."];
    }
}

/**
 * The AT&T Speech SDK or Speech to Text service returned an error.
 **/
- (void) speechService: (ATTSpeechService*) speechService
         failedWithError: (NSError*) error
{
    if ([error.domain isEqualToString: ATTSpeechServiceErrorDomain]
        && (error.code == ATTSpeechServiceErrorCodeCanceledByUser)) {
        NSLog(@"Speech service canceled");
        // Nothing to do in this case
        return;
    }
    NSLog(@"Speech service had an error: %@", error);
    
    [self showAlertWithTitle: @"An error occurred"
                     message: @"Please try again later."];
}

#pragma mark -
#pragma mark OAuth

/**
 * Start an asynchronous OAuth credential check.
 * Disables the Speak button until the check is complete.
**/
- (void) validateOAuthForService: (ATTSpeechService*) speechService
{
    talkButton.enabled = NO;
    [[SpeechAuth authenticatorForService: SpeechOAuthUrl()
                                  withId: SpeechOAuthKey()
                                  secret: SpeechOAuthSecret()
                                   scope: SpeechOAuthScope()]
     fetchTo: ^(NSString* token, NSError* error) {
         if (token) {
             self.oauthToken = token;
             speechService.bearerAuthToken = token;
             [self readyForSpeech];
         }
         else {
             self.oauthToken = nil;
             [self speechAuthFailed: error];
         }
     }];
}

/**
 * The SpeechAuth authentication failed.
**/
- (void) speechAuthFailed: (NSError*) error
{
    NSLog(@"OAuth error: %@", error);
    [self showAlertWithTitle: @"Speech Unavailable"
                     message: @"This app was rejected by the speech service.  Contact the developer for an update."];
}

/**
 * Display an error alert.
**/
- (void) showAlertWithTitle: (NSString*) title message: (NSString*) message
{
    UIAlertView* alert =
        [[UIAlertView alloc] initWithTitle: title
                                   message: message
                                  delegate: self
                         cancelButtonTitle: @"OK"
                         otherButtonTitles: nil];
    [alert show];
    [alert release];
}

@end
