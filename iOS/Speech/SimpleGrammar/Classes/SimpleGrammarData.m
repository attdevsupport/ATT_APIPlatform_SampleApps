//  SimpleGrammarData.m
//  SimpleGrammar
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "SimpleGrammarData.h"


@implementation SimpleGrammarData

@synthesize kind;
@synthesize size;
@synthesize crust;
@synthesize ingredients;

/**
 * Assign the slots of this object from the values in the NluHypothesis.Out 
 * dictionary.
**/
- (void) assignSlots: (NSDictionary*) slots
{
    NSString* kindSlot = slots[@"kind"];
    self.kind = kindSlot.length ? kindSlot : @"(not specified)";
    NSString* sizeSlot = slots[@"size"];
    self.size = sizeSlot.length ? sizeSlot : @"(not specified)";
    NSString* crustSlot = slots[@"crust"];
    self.crust = crustSlot.length ? crustSlot : @"(not specified)";
    NSArray* ingredientsSlot = slots[@"ingredients"];
    self.ingredients = ingredientsSlot.count ? ingredientsSlot : @[@"(none specified)"];
}

/**
 * Returns a SimpleGrammarData from the JSON responseDictionary.
**/
+ (SimpleGrammarData*) parseResponse: (NSDictionary*) responseDictionary
{
    SimpleGrammarData* result = nil;
    if (nil != responseDictionary) {
        NSDictionary* recognition = responseDictionary[@"Recognition"];
        if (nil != recognition) {
            NSArray* nBest = recognition[@"NBest"];
            if (nil != nBest && 0 < nBest.count) {
                NSDictionary* hypothesis = nBest[0];
                NSDictionary* NluHypothesis = hypothesis[@"NluHypothesis"];
                if (nil != NluHypothesis) {
                    id values = NluHypothesis[@"Out"];
                    if (nil != values && [values isKindOfClass: [NSDictionary class]])
                    {
                        result = [[self alloc] init];
                        [result assignSlots: values];
                    }
                }
            }
        }
    }
    return result;
}

/**
 * Returns the custom grammar data for this application.
**/
+ (NSData*) loadGrammarData
{
    // Load pizza.srgs from the application resources.
    NSBundle* bundle = [NSBundle mainBundle];
    NSURL* grammarURL = [bundle URLForResource: @"pizza" withExtension: @"srgs"];
    return [[NSData alloc] initWithContentsOfURL: grammarURL];
}

/**
 * Picks an example phrase to appear when the app launches.
**/
+ (NSString*) randomSampleText
{
    NSArray* randomSamples = @[
                               @"Large thin crust Hawaiian pizza with extra cheese",
                               @"Thick crust pizza with olives, mushrooms and pepperoni",
                               @"Small thick crust pizza with garlic, cheese, olives and mushrooms",
                               @"Personal vegetarian pizza with garlic"];
    unsigned int seed = [[NSDate date] timeIntervalSince1970];
    int index = rand_r(&seed) % randomSamples.count;
    return randomSamples[index];
}

#pragma mark - TableView DataSource Methods

/** Constant indices for the slots in the table view. **/
enum {
    INDEX_KIND, INDEX_SIZE, INDEX_CRUST, INDEX_INGREDIENTS, COUNT_SLOTS
};

- (NSInteger) numberOfSectionsInTableView: (UITableView*) tableView
{
    return COUNT_SLOTS;
}

- (NSString*) tableView: (UITableView*) tableView titleForHeaderInSection: (NSInteger) inSection;
{
    switch (inSection) {
        case INDEX_KIND:
            return @"Kind";
        case INDEX_SIZE:
            return @"Size";
        case INDEX_CRUST:
            return @"Crust";
        case INDEX_INGREDIENTS:
            return @"Ingredients";
        default:
            return nil; // should not happen
    }
}

- (NSInteger) tableView: (UITableView*) tableView numberOfRowsInSection: (NSInteger) inSection
{
    switch (inSection) {
        case INDEX_KIND:
        case INDEX_SIZE:
        case INDEX_CRUST:
            return 1;
        case INDEX_INGREDIENTS:
            return self.ingredients.count;
        default:
            return 0; // should not happen
    }

}

static NSString* const CellIdentifier = @"cellIdentifier";

- (UITableViewCell*) tableView: (UITableView*) tableView cellForRowAtIndexPath: (NSIndexPath*) indexPath;
{
    UITableViewCell* cell = [tableView dequeueReusableCellWithIdentifier: CellIdentifier];
    if (nil == cell) {
        cell = [[UITableViewCell alloc] initWithStyle: UITableViewCellStyleDefault
                                      reuseIdentifier: CellIdentifier];
    }
    
    switch (indexPath.section) {
        case INDEX_KIND:
            cell.textLabel.text = self.kind;
            break;
        case INDEX_SIZE:
            cell.textLabel.text = self.size;
            break;
        case INDEX_CRUST:
            cell.textLabel.text = self.crust;
            break;
        case INDEX_INGREDIENTS:
            cell.textLabel.text = self.ingredients[indexPath.row];
            break;
    }
    return cell;
}


@end
