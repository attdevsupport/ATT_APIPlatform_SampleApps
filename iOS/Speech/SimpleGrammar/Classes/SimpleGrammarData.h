//  SimpleGrammarData.h
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import <Foundation/NSObject.h>
#import <UIKit/UITableView.h>

/**
 * Represents the items parsed using the grammar.
**/
@interface SimpleGrammarData : NSObject <UITableViewDataSource>

@property (strong, nonatomic) NSString* kind;
@property (strong, nonatomic) NSString* size;
@property (strong, nonatomic) NSString* crust;
@property (strong, nonatomic) NSArray* ingredients;

// Returns a SimpleGrammarData from the JSON responseDictionary.
+ (SimpleGrammarData*) parseResponse: (NSDictionary*) responseDictionary;

// Returns the custom grammar data for this application.
+ (NSData*) loadGrammarData;

// Picks an example phrase to appear when the app launches.
+ (NSString*) randomSampleText;

@end
