//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import <UIKit/UIKit.h>
#import "ATTAdView.h"
#import "AppDelegate.h"
#import <CoreLocation/CoreLocation.h>

@class FirstViewController;

@interface SecondViewController : UIViewController <UITextFieldDelegate,CLLocationManagerDelegate,UITableViewDataSource,UITableViewDelegate,UIAlertViewDelegate>
{
    AppDelegate *appDelegate;
    
    IBOutlet UIButton *btnAutoDetectLocation;  //btnAutoDetectLocation
    IBOutlet UISegmentedControl *segCtrlAgeGroup;//segCtrlAgeGroup
    IBOutlet UISegmentedControl *segCtrlPremium;//segCtrlPremium
    IBOutlet UITextField *txtZipCode;//txtZip
    IBOutlet UITextField *txtKeywords;//txtKeywords
    NSMutableArray *arrayPreferences;//arrayPreferences
    
    IBOutlet UIScrollView *scrlViewSettings;  //scrlViewSettings
    IBOutlet UITextField *txtLongitude;// txtLongitude
    IBOutlet UITextField *txtLatitude; //txtLatitude
    UITableView *tblAdSize; //tblAdSize
    
    NSString *strAdSizeChoosen;  //strAdSizeChoosen
    
    BOOL isAdSizeDropdownOpen;
    
    NSArray *arrayAdSizes;//arrayAdSizes
    
}


/**
 *
 * checkbox to choose for autodetect location
 *
 */

@property (retain, nonatomic) IBOutlet UIButton *btnAutoDetectLocation;


/**
 *
 * Assigns the non-premium/premium/both to app delegage when selected the segment
 *
 */

- (IBAction)premiumSegment:(id)sender;

/**
 *
 * Assigns the agegroup to app delegage when selected the segment
 *
 */

- (IBAction)ageGroupSegment:(id)sender;


/**
 *
 * Detects the current Latitude/Longitude when the user opts for
 *
 */

-(IBAction)detectLocation:(id)sender;


@end
