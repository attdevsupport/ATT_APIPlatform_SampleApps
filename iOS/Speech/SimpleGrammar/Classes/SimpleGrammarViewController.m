//  ViewController.m
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleGrammarViewController.h"
#import "SpeechConfig.h"
#import "SpeechAuth.h"
#import "SimpleGrammarData.h"


@interface SimpleGrammarViewController () <ATTSpeechServiceDelegate>

@property (strong, nonatomic) SimpleGrammarData* understanding;

@end


@implementation SimpleGrammarViewController

@synthesize tableView;
@synthesize talkButton;
@synthesize sampleText;
@synthesize understanding;

#pragma mark -
#pragma mark - Lifecyle

- (void) dealloc
{
    // Make sure tableView doesn't try to access data
    // after ARC releases self.understanding.
    tableView.dataSource = nil;
}

#pragma mark -
#pragma mark UI

/**
 * Prepare the UI for display.
**/
- (void) viewDidLoad
{
    [super viewDidLoad];
    
    // Display an example query.
    sampleText.text = [NSString stringWithFormat: @"Try saying: %@",
                   [SimpleGrammarData randomSampleText]];
}

#pragma mark -
#pragma mark Speech

/**
 * Configure speech recognition.
 * Call this every time the app comes to the foreground.
**/
- (void) prepareSpeech
{
    // Access the SpeechKit singleton.
    ATTSpeechService* speechService = [ATTSpeechService sharedSpeechService];
    
    // Point to the SpeechToText API.
    speechService.recognitionURL = SpeechServiceUrl();
    
    // Hook ourselves up as a delegate so we can get called back with the response.
    speechService.delegate = self;
    //speechService.endingSilence = 1.25;
    
    // Use default speech UI.
    speechService.showUI = YES;
    
    // Use a custom grammar.
    // SpeechKit will call [speechServiceSendingPartsBeforeAudio:] when it
    // needs the grammar data.
    speechService.speechContext = @"GrammarList";
    speechService.isMultipart = YES;
    
    // Start the OAuth background operation, disabling the Talk button until
    // it's done.
    self.talkButton.enabled = NO;
    [[SpeechAuth authenticatorForService: SpeechOAuthUrl()
                                  withId: SpeechOAuthKey()
                                  secret: SpeechOAuthSecret()
                                   scope: SpeechOAuthScope()]
     fetchTo: ^(NSString* token, NSError* error) {
         if (token) {
             speechService.bearerAuthToken = token;
             self.talkButton.enabled = YES;
         }
         else
             [self speechAuthFailed: error];
     }];
    
    // Wake the audio components so there is minimal delay on the first request.
    [speechService prepare];
}

/**
 * Action called by pressing "Talk" button.
**/
- (IBAction) talk: (id) sender
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

/**
 * Update the views based on the parsed speech response.
**/
- (void) processResponse: (SimpleGrammarData*) parsedResponse
{
    self.understanding = parsedResponse;
    // The view displays the parsed slots as rows in a table.
    tableView.dataSource = parsedResponse;
    [tableView reloadData];
}


#pragma mark - Speech Service Delegate Methods

/**
 * Send the grammar data as part of a multipart request to the Speech API.
**/
- (void) speechServiceSendingPartsBeforeAudio: (ATTSpeechService*) speechService
{
    // Add the grammar part.  
    NSData* grammarData = [SimpleGrammarData loadGrammarData];
    if (nil != grammarData && 0 < grammarData.length) {
        [speechService addPart: grammarData
                   contentType: @"application/srgs+xml"
                   disposition: @"form-data; name=\"x-grammar\""];
    }
}


/**
 * Handle the result a successful speech interaction.
**/
- (void) speechServiceSucceeded: (ATTSpeechService*) speechService
{
    NSLog(@"Speech service succeeded");
    
    // Extract the needed data from the SpeechService object:
    // For raw bytes, read speechService.responseData.
    // For a JSON tree, read speechService.responseDictionary.
    // For the n-best ASR strings, use speechService.responseStrings.
    // With a custom grammar, the "NluHypothesis" field of the JSON
    // contains the parsed response.
    
    // In this example, we display use both the ASR string and the parsed data.
    // Display the recognized text in the interface is it's non-empty,
    // otherwise have the user try again.
    
    NSArray* responseStrings = speechService.responseStrings;
    NSString* recognizedText = @"";
    if (responseStrings != nil && responseStrings.count > 0)
        recognizedText = responseStrings[0];
    SimpleGrammarData* parsedResponse =
        [SimpleGrammarData parseResponse: speechService.responseDictionary];

    if (parsedResponse == nil || !recognizedText.length) {
        UIAlertView* alert =
        [[UIAlertView alloc] initWithTitle: @"Didn't recognize speech"
                                   message: @"Please try again."
                                  delegate: self
                         cancelButtonTitle: @"OK"
                         otherButtonTitles: nil];
        [alert show];
        [self processResponse: nil]; // clear the old parsed data
        return;
    }
    
    sampleText.text = [NSString stringWithFormat: @"You ordered: %@",
                       recognizedText];
    [self processResponse: parsedResponse];
    
}

/**
 * Handle the error an unsuccessful speech interaction.
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
    
    UIAlertView* alert =
    [[UIAlertView alloc] initWithTitle: @"An error occurred"
                               message: @"Please try again later."
                              delegate: self
                     cancelButtonTitle: @"OK"
                     otherButtonTitles: nil];
    [alert show];
}


#pragma mark - OAuth

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
//  [alert release];
}

@end
