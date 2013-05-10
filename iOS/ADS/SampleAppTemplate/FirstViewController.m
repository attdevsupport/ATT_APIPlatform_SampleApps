//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import "FirstViewController.h"
#import "SecondViewController.h"
#import "ATTAdView.h"
#import "Reachability.h"

/**
 * Category List currently supported by SDK.  Developer can
 * add more, if supported.
 * Make sure, no space added after comma (',').
 */

NSString *categoryListStr = @"auto,business,chat,communication,community,entertainment,finance,games,health,local,maps,medical,movies,music,news,personals,photos,shopping,social,sports,technology,tools,travel,tv,weather,others";


// Default height for the Ad component
// Width will be 100% by default.

NSInteger adMaxHeight = 50;

/**
 * Developer can use Obfuscated App Key and Secret in the below declaration.
 * The app will unobfuscate and send them to the API call.  The
 * Developer can use the below unobfuscate method to obfuscate
 * and provide the values for app key and secret or provide their
 * own implementation for the same.
 * - (NSString *)unobfuscate:(NSString *)string withKey:(NSString *)key
 */

#define APP_KEY @ADD_YOUR_APP_KEY_HERE
#define APP_SECRET @ADD_YOUR_APP_SECRET_HERE

/**
 * UDID to be provided by the Developer
 * Must be 30 characters
 */

#define UDID @ADD_YOUR_UDID_HERE

@interface FirstViewController ()

@end

@implementation FirstViewController

-(ATTAdView *) startAdsViewWithWidth:(CGFloat)maxWidth
                          withHeight:(CGFloat)maxHeight
{
    //Launch Ads view.
    /****************** Use this if the keys are Obfuscated. ****************************************
     
     attAdView = [attAdView initWithFrame:CGRectMake(0, 0,maxWidth,maxHeight)
     appKey:[self unobfuscate:APP_KEY withKey:@"ATT"]
     appsecret:[self unobfuscate:APP_SECRET withKey:@"ATT"]
     category:appDelegate.category udid:UDID];
     
     *************************************************************************************************/
    
    //Use this if the keys are not obfuscated.
    attAdView = [attAdView initWithFrame:CGRectMake(0, 0,maxWidth,maxHeight)
                                  appKey:APP_KEY
                               appsecret:APP_SECRET
                                category:appDelegate.category udid:UDID];
    
    return attAdView;
}

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.title = NSLocalizedString(@"GetAd", @"GetAd");
        self.tabBarItem.image = [UIImage imageNamed:@"GetAd"];
    }
    return self;
}

#pragma mark - View Life Cycle

- (void)viewDidLoad {
    [super viewDidLoad];
    
    appDelegate= (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    // Create the categories array with from categoryListStr
    adCategories = [[NSArray alloc] initWithArray:[categoryListStr componentsSeparatedByString:@","]];
    
    // Defaulting to NonPremium for the first use
    appDelegate.premium = PremiumNonPremium;

}

/**
 
 Implementation for obfuscation/unobfuscation
 
 */

- (NSString *)unobfuscate:(NSString *)inputString withKey:(NSString *)key 
{
    // Create data object from the string
    NSData *data = [inputString dataUsingEncoding:NSUTF8StringEncoding];
    
    // Get pointer to data to obfuscate
    char *dataPtr = (char *) [data bytes];
    
    // Get pointer to key data
    char *keyData = (char *) [[key dataUsingEncoding:NSUTF8StringEncoding] bytes];
    
    // Points to each char in sequence in the key
    char *keyPtr = keyData;
    int keyIndex = 0;
    
    // For each character in data, xor with current value in key
    for (int x = 0; x < [data length]; x++) 
    {
        // Replace current character in data with 
        // current character xor'd with current key value.
        // Bump each pointer to the next character
       // *dataPtr = *dataPtr++ ^ *keyPtr++; 
        
        *dataPtr = *dataPtr ^ *keyPtr;
        dataPtr++;
        keyPtr++;
        
        // If at end of key data, reset count and 
        // set key pointer back to start of key value
        if (++keyIndex == [key length])
            keyIndex = 0, keyPtr = keyData;
    }
    
    return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
}

- (void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    appDelegate= (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    [self updateAd];
    
}

- (void)viewDidUnload
{
    adPicker = nil;
    [super viewDidUnload];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
}

#pragma mark - Managing UIPicker View

-(NSInteger)numberOfComponentsInPickerView:(UIPickerView *)pickerView
{
    return 1;
}

-(NSInteger)pickerView:(UIPickerView *)pickerView numberOfRowsInComponent:(NSInteger)component
{
    ///Setting the number of rows in the picker view to the number of categories in the array.
    return [adCategories count];
}

-(void)pickerView:(UIPickerView *)pickerView didSelectRow:(NSInteger)row inComponent:(NSInteger)component
{
    ///Assigning the Ad category chosen from the picker view to a string
    appDelegate.category = [adCategories objectAtIndex:row];

    
}

-(NSString *)pickerView:(UIPickerView *)pickerView titleForRow:(NSInteger)row forComponent:(NSInteger)component
{
    //Assigning the row title with the Ad category title.
    return [adCategories objectAtIndex:row];
}

#pragma mark - Button Actions

/**
 
 Implementation for getAdButton
 
 */

- (IBAction)getAdButton:(id)sender {    
    
    [self updateAd];
}


/**
 
 Implementation for updateAd
 
 */

-(void)updateAd
{
    appDelegate= (AppDelegate *)[[UIApplication sharedApplication] delegate];
    //Changing the default value of pickerView from null to Auto, if the picker view row has not been changed.
    if ([adPicker selectedRowInComponent:0] == 0)
    {
        appDelegate.category = @"auto";
    }
    
    // remove the existing, if present
    if(attAdView)
    {
        [attAdView removeFromSuperview];
        attAdView  = nil;
    }
    
    //Initialize the AttAdsView.
    attAdView = [[ATTAdView alloc] init];
    
    //Set Ads view properties.
    [attAdView setKeywords:appDelegate.keyWords];
    [attAdView setZipCode:appDelegate.zipCode];
    [attAdView setPremium:appDelegate.premium ];
    [attAdView setMaxHeight:appDelegate.maxHeight];
    [attAdView setMaxWidth:appDelegate.maxWidth];
    [attAdView setAgeGroup:appDelegate.ageGroup] ;
    [attAdView setLatitude:appDelegate.latitude] ;
    [attAdView setLongitude:appDelegate.longitude];
    [attAdView setAdRefreshPeriod:30];  // in seconds
    [attAdView setAttAdViewDelegate:self];
    
    
    ///Accessing the preferences values like KeyWords,ZipCode,Premium categories, Ad Type, Age Group, Maximum Height and width, from the AppDelegate and passing to the API to set preferences for the Ad
        
    if (appDelegate.maxWidth == 300 && appDelegate.maxHeight == 250) {
        BOOL isnetWorkStatus = [self currentNetworkStatus];
        if(isnetWorkStatus)
        {
            attAdView = [self startAdsViewWithWidth:appDelegate.maxWidth withHeight:appDelegate.maxHeight];
            
        if(backView)
        {
            [backView removeFromSuperview];
            backView = nil;
        }
        backView =[[UIView alloc]initWithFrame:CGRectMake(0, 0,320, 480)];
        [self.view addSubview:backView];
        [backView setBackgroundColor:[UIColor clearColor]];
        
        UIView *addView =[[UIView alloc]initWithFrame:CGRectMake(10,105,300, 240)];
        [backView addSubview:addView];
        [addView setBackgroundColor:[UIColor whiteColor]];
        [[addView layer] setCornerRadius:10];
        [addView setClipsToBounds:YES];
        
        // Create colored border using CALayer property
        [[addView layer] setBorderColor:
         [[UIColor colorWithRed:0.2 green:0.09 blue:0.07 alpha:1] CGColor]];
        [[addView layer] setBorderWidth:2.75];
        
        [addView addSubview:attAdView];
        
        UIButton *popUpCloseBtn =[UIButton buttonWithType:UIButtonTypeCustom];
        [popUpCloseBtn setFrame:CGRectMake(275,1, 25, 25)];
        [addView addSubview:popUpCloseBtn];
        [popUpCloseBtn setImage:[UIImage imageNamed:@"popupCloseBtn.png"] forState:UIControlStateNormal];
        [popUpCloseBtn addTarget:self action:@selector(popUpBtnTapped) forControlEvents:UIControlEventTouchUpInside];
        [popUpCloseBtn bringSubviewToFront:addView];
        [addView release];
        }
        

    }
    else {   
        NSInteger xCord =(NSInteger) (self.view.frame.size.width - appDelegate.maxWidth)/4;
        
        NSLog(@"xCord = %d",xCord);

        if (appDelegate.maxWidth == 0 && appDelegate.maxHeight == 0) {
            
            if(backView)
            {
                [backView removeFromSuperview];
                backView = nil;
            }

            attAdView = [self startAdsViewWithWidth:self.view.frame.size.width withHeight:adMaxHeight];
         }
        else {
            
            if(backView)
            {
                [backView removeFromSuperview];
                backView = nil;
            }
            
            attAdView = [self startAdsViewWithWidth:appDelegate.maxWidth withHeight:appDelegate.maxHeight];
        }
        
        [self.view addSubview:attAdView];
    }
}

-(void)popUpBtnTapped
{
    [backView removeFromSuperview];
    backView = nil;
}


#pragma mark - JSONResponses


-(void)didReciveAd:(NSString *)jsonResponse
{
    NSLog(@"Advertisement successfully loaded.  JSON Response received from Ad Service %@",jsonResponse);
    
    //  Assign the response to AppDelegate variable so that it
    //  can be used to displayed in the JSONResponse tab.
    
    appDelegate.jsonResponse = jsonResponse;
}

/**
 
 Delegate method to handle the failure of ad component loading
 
 Developers can use this method to change the implementation
 
 when ad loading fails.  The error object contains the details of
 
 error occurred
 
 */

-(void)didFailedToReceiveAd:(NSError *)error
{
    NSLog(@"Failed to load the Advertisement.  Error : %@", error);
    appDelegate.jsonResponse = [NSString stringWithFormat:@"%@",error];
}



-(BOOL)currentNetworkStatus {
    
        BOOL networkPresent;
        Reachability *attAdReachability = [[Reachability reachabilityForInternetConnection] retain];
        [attAdReachability startNotifier];
        NetworkStatus netStatus = [attAdReachability currentReachabilityStatus];
        return networkPresent = (netStatus != NotReachable);
       
}



@end
