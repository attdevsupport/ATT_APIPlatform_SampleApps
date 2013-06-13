//  SimpleSpeechViewController.m
//  SimpleSpeech
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleSpeechViewController.h"
#import "SpeechConfig.h"
#import "SpeechAuth.h"

@interface SimpleSpeechViewController ()
- (void) speechAuthFailed: (NSError*) error;
@end

@implementation SimpleSpeechViewController

@synthesize textLabel;
@synthesize webView;
@synthesize talkButton;

#pragma mark -
#pragma mark Lifecyle

- (void) dealloc
{
    self.textLabel = nil;
    self.webView = nil;
    self.talkButton = nil;
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
    
    // Use default speech UI.
    speechService.showUI = YES;
    
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
- (IBAction) listen: (id) sender
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

// Make use of the recognition text in this app.
- (void) handleRecognition: (NSString*) recognizedText
{
    // Display the recognized text.
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
