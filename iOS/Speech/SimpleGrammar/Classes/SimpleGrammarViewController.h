//  SimpleGrammarViewController.h
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import <UIKit/UIViewController.h>
#import "ATTSpeechKit.h"


@interface SimpleGrammarViewController : UIViewController

@property (strong, nonatomic) IBOutlet UIButton* talkButton;
@property (strong, nonatomic) IBOutlet UITableView* tableView;
@property (strong, nonatomic) IBOutlet UILabel* sampleText;

// Initialize SpeechKit for this app.
- (void) prepareSpeech;

// Message sent by "Press to Talk" button in UI
- (IBAction) talk: (id) sender;

@end
