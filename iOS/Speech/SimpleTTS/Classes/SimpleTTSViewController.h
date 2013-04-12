//  SimpleTTSViewController.h
//  SimpleTTS
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import <UIKit/UIViewController.h>
#import "ATTSpeechKit.h"


/**
 * SimpleTTS is a basic demonstration of using the ATTSpeechKit to do speech
 * recognition (speech-to-text) and the AT&T Speech API for text-to-speech.
 * It is designed to introduce a developer to making
 * a new application that uses the AT&T SpeechKit iOS library.
 **/
@interface SimpleTTSViewController : UIViewController <ATTSpeechServiceDelegate>

@property (retain, nonatomic) IBOutlet UITextView* textView;
@property (retain, nonatomic) IBOutlet UIButton* talkButton;

// Initialize SpeechKit for this app.
- (void) prepareSpeech;

// Message sent by "Press to Talk" button in UI
- (IBAction) listen: (id) sender;

@end

