//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import <UIKit/UIKit.h>
#import "AppDelegate.h"
#import "ATTAdViewDelegate.h"
#import "ATTAdView.h"
#import "ATTPopupWindow.h"

@interface FirstViewController : UIViewController<UIPickerViewDelegate, UIPickerViewDataSource,ATTAdViewDelegate>

{
    IBOutlet UIPickerView *adPicker;
    NSArray *adCategories;
    AppDelegate *appDelegate;
    ATTAdView *attAdView;
    UIView *backView;
    
}

/**
 *
 * Button tap capture action for GetAd button.
 * Update the ad with current settings 
 *
 */

- (IBAction)getAdButton:(id)sender;


/**
 *
 * Capture current settings and update the ad component.
 * Update the ad with current settings 
 *
 */

- (void)updateAd;


/**
 
 Obfuscates or unobfuscates the input string with
 
 the provided key.  Key must be the same for both
 
 obfuscation and unobfuscation.
 
 @param inputString String to be obfuscated/unobfuscated
 
 @param key - key to be used for obfuscation/unobfuscation
 
 @return NSString - obfuscated/unobfuscated input string
 
 */

- (NSString *)unobfuscate:(NSString *)inputString withKey:(NSString *)key;

@end
