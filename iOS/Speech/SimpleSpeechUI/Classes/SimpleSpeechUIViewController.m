//  SimpleSpeechViewController.m
//  SimpleSpeech
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleSpeechUIViewController.h"
#import "SpeechConfig.h"
#import "SpeechAuth.h"

@interface SimpleSpeechUIViewController ()
@property (retain, nonatomic) NSString* text;
- (void) speechAuthFailed: (NSError*) error;
@end

@implementation SimpleSpeechUIViewController

@synthesize textLabel;
@synthesize webView;
@synthesize talkButton;
@synthesize text;

#pragma mark -
#pragma mark Lifecyle

- (instancetype) init{
    self = [super init];
    if (self != nil) {
        self.text = @"";
    }
    return self;
}

- (void) dealloc
{
    self.textLabel = nil;
    self.webView = nil;
    self.talkButton = nil;
    self.text = nil;
    [super dealloc];
}


// Initialize SpeechKit for this app.
- (void) prepareSpeech
{
    // Access the SpeechKit singleton.
    ATTSpeechService* speechService = [ATTSpeechService sharedSpeechService];
    
    // Point to the SpeechToText API.
    speechService.recognitionURL = SpeechServiceUrl();
    
    // Hook ourselves up as a delegate so we can get called back with the response.
    speechService.delegate = self;
    
    // Use a custom speech UI.
    speechService.showUI = NO;
    
    // Choose the speech recognition package.
    speechService.speechContext = @"WebSearch";
    
    // Enable the Speex codec, which provides better speech recognition accuracy.
    speechService.audioFormat = ATTSKAudioFormatSpeex_WB;
    
    // Start the OAuth background operation, disabling the Talk button until 
    // it's done.
    talkButton.enabled = NO;
    [[SpeechAuth authenticatorForService: SpeechOAuthUrl()
                                  withId: SpeechOAuthKey()
                                  secret: SpeechOAuthSecret()
                                   scope: SpeechOAuthScope()]
        fetchTo: ^(NSString* token, NSError* error) {
            if (token) {
                speechService.bearerAuthToken = token;
                talkButton.enabled = YES;
            }
            else
                [self speechAuthFailed: error];
    }];

    // Wake the audio components so there is minimal delay on the first request.
    [speechService prepare];
}

#pragma mark -
#pragma mark UI

- (BOOL) shouldAutorotateToInterfaceOrientation: (UIInterfaceOrientation) interfaceOrientation
{
    return YES;
}

#pragma mark -
#pragma mark Actions

// Perform the action of the "Push to talk" button
- (void) startListening
{
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

// Perform the action of the "Stop" button
- (void) stopListening
{
    NSLog(@"Manually ending microphone input");
    [[ATTSpeechService sharedSpeechService] stopListening];
}

// Perform the action of the "Cancel" button
- (void) cancelProcessing
{
    NSLog(@"Manually canceling speech request");
    [[ATTSpeechService sharedSpeechService] cancel];
}

- (IBAction) buttonPressed: (id) sender
{
    // The action to perform depends on the current state of the speech interaction.
    // Make sure this code is kept in sync with -[speechService:willEnterState:].
    ATTSpeechServiceState speechState =
        [[ATTSpeechService sharedSpeechService] currentState];
    switch (speechState) {
        case ATTSpeechServiceStateRecording:
            // Speech SDK is capturing audio from the microphone.
            // The button behavior to manually endpoint audio.
            [self stopListening];
            break;
        case ATTSpeechServiceStateProcessing:
            // Speech SDK is waiting for the server.
            // The button behavior is to cancel waiting.
            [self cancelProcessing];
            break;
        default:
            // Speech SDK is not interacting with the user.
            // The button behavior to start listening.
            [self startListening];
            break;
    }
}

// Make use of the recognition text in this app.
- (void) handleRecognition: (NSString*) recognizedText
{
    // Display the recognized text.
    self.text = recognizedText;
    [self.textLabel setText: recognizedText];
    
    // Load a website using the recognized text.
    // First make the recognizedText safe for use as a search term in a URL.
    NSString* escapedTerm =
        [recognizedText stringByAddingPercentEscapesUsingEncoding: NSUTF8StringEncoding];
    NSString* urlString =
        [NSString stringWithFormat: @"http://en.m.wikipedia.org/w/index.php?search=%@", escapedTerm];
    NSURL* url = [NSURL URLWithString: urlString];
    NSURLRequest* request = [NSURLRequest requestWithURL: url];
    [self.webView loadRequest:request];
}

#pragma mark -
#pragma mark Speech Service Delegate Methods

/**
 The Speech SDK calls this method when it transitions among states in a recording
 interaction, for example, from capturing to processing.
 The method argument contains the state the service will enter, and
 speechService.currentState contains the state the service is leaving.
 Upon exiting this delegate callback, the service will be in the new state.
 @param speechService The service notifying the delegate that the state is changing.
 @param newState The state the service is changing to.
 **/
- (void) speechService: (ATTSpeechService*) speechService
        willEnterState: (ATTSpeechServiceState) newState
{
    switch (newState) {
        case ATTSpeechServiceStateRecording:
            // Speech SDK has started capturing audio from the microphone.
            textLabel.text = @"(Listening...)";
            // Change the button behavior to manually endpoint audio.
            [talkButton setTitle: @"Stop" forState: UIControlStateNormal];
            break;
        case ATTSpeechServiceStateProcessing:
            // Speech SDK is done capturing audio and is now waiting for the server.
            textLabel.text = @"(Processing...)";
            // Change the button behavior to cancel waiting.
            [talkButton setTitle: @"Cancel" forState: UIControlStateNormal];
            break;
        default:
            // Speech SDK is not interacting with the user.
            // Restore the button behavior to start listening.
            [talkButton setTitle: @"Press to Talk" forState: UIControlStateNormal];
            break;
    }
}

/**
 The Speech SDK calls this method repeatedly as it captures audio. The callback
 rate is roughly 1/10 second. An application can use the audio level data to
 update its UI.
 @param speechService The service notifying the delegate about the audio level.
 @param level The audio level.
**/
- (void) speechService: (ATTSpeechService*) speechService
            audioLevel: (float) level
{
    // Show the audio level in the text label.
    textLabel.text = [NSString stringWithFormat: @"(Listening [level %.2f])",
                      level];
}

/**
 The Speech SDK calls this method when it returns a recognition result. The
 ATTSpeechService object will contain properties that include the response data
 and recognized text.
 @param speechService The service notifying the delegate of success.
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
        UIAlertView* alert =
            [[UIAlertView alloc] initWithTitle: @"Didn't recognize speech"
                                       message: @"Please try again."
                                      delegate: self 
                             cancelButtonTitle: @"OK"
                             otherButtonTitles: nil];
        [alert show];
        [alert release];
    }
}

/**
 The Speech SDK calls this method when speech recognition fails. The reasons for
 failure can include the user canceling, network errors, or server errors.
 @param speechService The service notifying the delegate of failure.
 @param error The error that has occurred.
**/
- (void) speechService: (ATTSpeechService*) speechService
       failedWithError: (NSError*) error
{
    // First restore the UI to its state before speech processing.
    textLabel.text = text;
    
    if ([error.domain isEqualToString: ATTSpeechServiceErrorDomain]
        && (error.code == ATTSpeechServiceErrorCodeCanceledByUser)) {
        NSLog(@"Speech service canceled");
        // Nothing else to do in this case
        return;
    }
    NSLog(@"Speech service had an error: %@", error);
    
    UIAlertView* alert =
        [[UIAlertView alloc] initWithTitle: @"An error occurred"
                                   message: @"Please try again later."
                                  delegate: self 
                         cancelButtonTitle: @"OK"
                         otherButtonTitles: nil];
    [alert show];
    [alert release];
}

#pragma mark -
#pragma mark OAuth

/* The SpeechAuth authentication failed. */
- (void) speechAuthFailed: (NSError*) error
{
    NSLog(@"OAuth error: %@", error);
    UIAlertView* alert = 
        [[UIAlertView alloc] initWithTitle: @"Speech Unavailable"
                                   message: @"This app was rejected by the speech service.  Contact the developer for an update."
                                  delegate: self 
                         cancelButtonTitle: @"OK"
                         otherButtonTitles: nil];
    [alert show];
    [alert release];
}

@end
