//  SimpleGrammarAppDelegate.h
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import <UIKit/UIApplication.h>
@class SimpleGrammarViewController;

@interface SimpleGrammarAppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow* window;
@property (nonatomic, retain) IBOutlet SimpleGrammarViewController *viewController;

@end
