//  SimpleGrammarAppDelegate.m
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleGrammarAppDelegate.h"
#import "SimpleGrammarViewController.h"

@implementation SimpleGrammarAppDelegate

@synthesize viewController;

- (void) applicationDidBecomeActive: (UIApplication*) application
{
    // Since the app has come to the foreground, (re-)initialize SpeechKit.
    [(SimpleGrammarViewController*)self.window.rootViewController prepareSpeech];
}

@end
