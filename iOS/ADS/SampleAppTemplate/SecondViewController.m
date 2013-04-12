//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import <CoreLocation/CoreLocation.h>

#import "SecondViewController.h"
#import "FirstViewController.h"
#import "DropDownCell.h"

    NSString *strAdSizes = @",Any,320 x 50,300 x 250,300 x 50,216 x 36,168 x 28,120 x 20";

@interface SecondViewController ()


@end

@implementation SecondViewController


@synthesize btnAutoDetectLocation;



- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.title = NSLocalizedString(@"Settings", @"Settings");
        self.tabBarItem.image = [UIImage imageNamed:@"Settings"];
    }
    return self;
}

#pragma mark - View Life Cycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    ///Setting initial selected values on the user interface elements with respect to the app delegate default values.
    arrayPreferences = [[NSMutableArray alloc]init];
    
    [scrlViewSettings setContentSize:CGSizeMake(320, 660)];
    
    tblAdSize = [[UITableView alloc]initWithFrame:CGRectMake(0,360, 320,350) style:UITableViewStyleGrouped];
    [tblAdSize setDelegate:self];
    [tblAdSize setDataSource:self];
    [tblAdSize setBackgroundColor:[UIColor clearColor]];
    [scrlViewSettings addSubview:tblAdSize];
    [tblAdSize setScrollEnabled:NO];
    [tblAdSize release];

    
    
//    adSizes = [[NSArray alloc]initWithObjects:@"",@"Any",@"320 x 50",@"300 x 250",@"300 x 50",@"216 x 36",@"168 x 28",@"120 x 20",nil];
    
    arrayAdSizes = [[NSArray alloc] initWithArray:[strAdSizes componentsSeparatedByString:@","]];
    
    [btnAutoDetectLocation setImage:[UIImage imageNamed:@"cb_dark_off.png"] forState:UIControlStateNormal];
    [txtLatitude setTag:1];
    [txtLongitude setTag:2];
    [txtZipCode setTag:3];
    [txtKeywords setTag:4];
    [txtLatitude setKeyboardType:UIKeyboardTypeNumbersAndPunctuation];
    [txtLongitude setKeyboardType:UIKeyboardTypeNumbersAndPunctuation];

}

- (void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    appDelegate= (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
        
}



- (void)viewDidUnload
{
    txtKeywords = nil;
    txtZipCode = nil;
    segCtrlPremium = nil;
    segCtrlAgeGroup = nil;
    [btnAutoDetectLocation release];
    btnAutoDetectLocation = nil;
    [self setBtnAutoDetectLocation:nil];
    [scrlViewSettings release];
    scrlViewSettings = nil;
    [txtLatitude release];
    txtLatitude = nil;
    [txtLongitude release];
    txtLongitude = nil;
    [super viewDidUnload];
    // Release any retained subviews of the main view.
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
}

#pragma mark - TextField

-(BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [txtKeywords resignFirstResponder];
    [txtZipCode resignFirstResponder];
    [self.view endEditing:YES];
    return YES;
}


-(void)textFieldDidEndEditing:(UITextField *)textField
{
    appDelegate.zipCode = [txtZipCode.text intValue];
    appDelegate.keyWords = txtKeywords.text;
    [self.view endEditing:YES];
    
    
    // Validate the latitude and longitude values to be as below
    // Latitude between -90.0 and +90.0
    // Longitude between -180.0 and +180.0
    
    if(textField.tag ==1)  // Latitude text field
    {
        float latitude = [txtLatitude.text floatValue];
//        if(latitude < -90.0 && latitude  > 90.0)
            
       if(! (latitude >= -90.0 && latitude  <= 90.0))
        {
            UIAlertView *alert = [[UIAlertView alloc]initWithTitle:@"Alert" message:@"Latitude value is invalid" delegate:self cancelButtonTitle:nil otherButtonTitles:@"OK", nil];
            [alert show];
            [alert setTag:1];
            [alert release];
        }else {
            appDelegate.latitude = txtLatitude.text;
        }
    }
    if(textField.tag ==2) // Longitude text field
    {
        float longitude = [txtLongitude.text floatValue];
//        if(longitude < -180.0 && longitude > 180.0)
            
        if(! (longitude >= -180.0 && longitude <= 180.0))
        {
            UIAlertView *alert = [[UIAlertView alloc]initWithTitle:@"Alert" message:@"Longitude value is invalid" delegate:self cancelButtonTitle:nil otherButtonTitles:@"OK", nil];
            [alert show];
            [alert setTag:2];
            [alert release];
        }else {
             appDelegate.longitude = txtLongitude.text;
        }
    }
       
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if(alertView.tag==1)
    {
        txtLatitude.text = @"";
    }
    if(alertView.tag==2)
    {
        txtLongitude.text = @"";
    }
}

#pragma mark - UserInterface elements actions


///Segmented control to accept the type of ad as Premium ar Non Premium or Both.
///Default values sent to the API would be 0 for Premium, 1 for Non-Premium and 2 for Both.

- (IBAction)premiumSegment:(id)sender {
    
    switch (segCtrlPremium.selectedSegmentIndex) {
        case 0:
            [arrayPreferences addObject:@"Non-Premium"];
            [appDelegate setPremium:PremiumNonPremium];
            break;
        case 1:
            [arrayPreferences addObject:@"Premium"];
            [appDelegate setPremium:PremiumPremium];
            break;
        case 2:
            [arrayPreferences addObject:@"Both"];
            [appDelegate setPremium:PremiumBoth];
            break;
        default:
            break;
    }
}

///Segmented control to accept the age group for the user
///Default values sent to the API would be 1-13, 14-25, 26-35, 36-55, 55-100

- (IBAction)ageGroupSegment:(id)sender {
    switch (segCtrlAgeGroup.selectedSegmentIndex) {
        case 1:
            [arrayPreferences addObject:@"1-13"];
            appDelegate.ageGroup = @"1-13";
            break;
        case 2:
            [arrayPreferences addObject:@"14-25"];
            appDelegate.ageGroup = @"14-25";
            break;
        case 3:
            [arrayPreferences addObject:@"26-35"];
            appDelegate.ageGroup = @"26-35";
            break;
        case 4:
            [arrayPreferences addObject:@"36-55"];
            appDelegate.ageGroup = @"36-55";
            break;
        case 5:
            [arrayPreferences addObject:@"55-100"];
            appDelegate.ageGroup = @"55-100";
            break;
        default:
            appDelegate.ageGroup = @"";
            break;
    }
    
}


-(IBAction)detectLocation:(id)sender
{
    CLLocationManager *locationManager = [[CLLocationManager alloc] init];  
    locationManager.delegate = self;
    if([CLLocationManager authorizationStatus] == 2)
    {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Alert" message:@"Please enable location service for the application from Settings" delegate:self cancelButtonTitle:nil otherButtonTitles:@"OK", nil];
        [alertView show];
        [alertView release];
        [txtLatitude setUserInteractionEnabled:NO];
        [txtLongitude setUserInteractionEnabled:NO];
    }
    if(appDelegate.isDetectLocation)
    {
        [btnAutoDetectLocation setImage:[UIImage imageNamed:@"cb_dark_off.png"] forState:UIControlStateNormal];
        appDelegate.isDetectLocation = NO;
        [txtLatitude setText:@""];
        [txtLongitude setText:@""];
        [txtLatitude setBackgroundColor:[UIColor whiteColor]];
        [txtLongitude setBackgroundColor:[UIColor whiteColor]];
        
    }else{
        [btnAutoDetectLocation setImage:[UIImage imageNamed:@"cb_dark_on.png"] forState:UIControlStateNormal];
        appDelegate.isDetectLocation = YES;
        [txtLatitude setBackgroundColor:[UIColor lightGrayColor]];
        [txtLongitude setBackgroundColor:[UIColor lightGrayColor]];
        locationManager.desiredAccuracy = kCLLocationAccuracyBest;
        locationManager.distanceFilter = kCLDistanceFilterNone;
        [locationManager startUpdatingLocation];
    }
}

- (void)locationManager:(CLLocationManager *)manager didUpdateToLocation:(CLLocation *)newLocation fromLocation:(CLLocation *)oldLocation
{
    CLLocationCoordinate2D coordinate = [newLocation coordinate];
    txtLatitude.text =[NSString stringWithFormat:@"%f",coordinate.latitude];
    txtLongitude.text = [NSString stringWithFormat:@"%f",coordinate.longitude];
    appDelegate.latitude = txtLatitude.text;
    appDelegate.longitude = txtLongitude.text;
    NSLog(@"--------Latitude : %@", appDelegate.latitude);
    NSLog(@"--------Longitude : %@",appDelegate.longitude);
    [manager stopUpdatingLocation];
    
}

- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error {
    [manager stopUpdatingLocation];
    NSLog(@"locationManager error%@",error);
    [txtLatitude setText:@""];
    [txtLongitude setText:@""];
    [txtLatitude setBackgroundColor:[UIColor whiteColor]];
    [txtLongitude setBackgroundColor:[UIColor whiteColor]];
    
}

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)status __OSX_AVAILABLE_STARTING(__MAC_10_7,__IPHONE_4_2)
{
    if([CLLocationManager authorizationStatus] == 2)
    {
        [txtLatitude setUserInteractionEnabled:YES];
        [txtLongitude setUserInteractionEnabled:YES];
        [btnAutoDetectLocation setImage:[UIImage imageNamed:@"cb_dark_off.png"] forState:UIControlStateNormal];
        
    }else {
        [txtLatitude setUserInteractionEnabled:YES];
        [txtLongitude setUserInteractionEnabled:YES];
    }
}



////////////////////////////////////////////////////

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    switch (section) {
        case 0: 
            if (isAdSizeDropdownOpen) {
                return 8;
            }
            else
            {
                return 1;
            }
        default:
                return 0;
            
        }
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    static NSString *DropDownCellIdentifier = @"DropDownCell";
    
    switch ([indexPath section]) {
        case 0: {
            
            switch ([indexPath row]) {
                case 0: {
                    DropDownCell *cell = (DropDownCell*) [tableView dequeueReusableCellWithIdentifier:DropDownCellIdentifier];
                    if (cell == nil){
                        //NSLog(@"New Cell Made");

                        NSArray *topLevelObjects = [[NSBundle mainBundle] loadNibNamed:@"DropDownCell" owner:nil options:nil];
                        
                        for(id currentObject in topLevelObjects)
                        {
                            if([currentObject isKindOfClass:[DropDownCell class]])
                            {
                                cell = (DropDownCell *)currentObject;
                                break;
                            }
                        }
                    }
                    [cell setSelectionStyle:UITableViewCellSelectionStyleNone];
                    [[cell textLabel] setText:@"Any"];
                    strAdSizeChoosen = @"Any";
                    
                    // Configure the cell.
                    return cell;
                    
                    break;
                }
                default: {
                    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
                    
                    if (cell == nil) {
                        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
                        
                    }
                    NSString *label = [NSString stringWithFormat:@"%@", [arrayAdSizes objectAtIndex:indexPath.row]];
                    
                    [[cell textLabel] setText:label];
                    
                    // Configure the cell.
                    return cell;
                    
                    break;
                }
            }
            
            break;
        }
        default:
            
            return nil;
            break;
    }
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    
    switch ([indexPath section]) {
        case 0: {
            
            switch ([indexPath row]) {
                case 0:
                {
                    DropDownCell *cell = (DropDownCell*) [tableView cellForRowAtIndexPath:indexPath];
                    
                    NSIndexPath *path0 = [NSIndexPath indexPathForRow:[indexPath row]+1 inSection:[indexPath section]];
                    NSIndexPath *path1 = [NSIndexPath indexPathForRow:[indexPath row]+2 inSection:[indexPath section]];
                    NSIndexPath *path2 = [NSIndexPath indexPathForRow:[indexPath row]+3 inSection:[indexPath section]];
                    NSIndexPath *path3 = [NSIndexPath indexPathForRow:[indexPath row]+4 inSection:[indexPath section]];
                    NSIndexPath *path4 = [NSIndexPath indexPathForRow:[indexPath row]+5 inSection:[indexPath section]];
                    NSIndexPath *path5 = [NSIndexPath indexPathForRow:[indexPath row]+6 inSection:[indexPath section]];
                    NSIndexPath *path6 = [NSIndexPath indexPathForRow:[indexPath row]+7 inSection:[indexPath section]];
                    
                    
                    NSArray *indexPathArray = [NSArray arrayWithObjects:path0, path1, path2,path3, path4, path5,path6,nil];
                    
                    if ([cell isOpen])
                    {
                        [cell setClosed];
                        isAdSizeDropdownOpen = [cell isOpen];
                        
                        [tableView deleteRowsAtIndexPaths:indexPathArray withRowAnimation:UITableViewRowAnimationTop];
                    }
                    else
                    {
                        [cell setOpen];
                        isAdSizeDropdownOpen = [cell isOpen];
                        
                        [tableView insertRowsAtIndexPaths:indexPathArray withRowAnimation:UITableViewRowAnimationTop];
                    }
                    
                    break;
                }   
                default:
                {
                    strAdSizeChoosen = [[[tableView cellForRowAtIndexPath:indexPath] textLabel] text];
                    
                    NSIndexPath *path = [NSIndexPath indexPathForRow:0 inSection:[indexPath section]];
                    DropDownCell *cell = (DropDownCell*) [tableView cellForRowAtIndexPath:path];
                    
                    [[cell textLabel] setText:strAdSizeChoosen];
                    if (![strAdSizeChoosen isEqualToString:@"Any"]) {
                    
                    NSString *adViewSizes = [NSString stringWithFormat:@"%@",strAdSizeChoosen];
                    NSArray *adSizesArray = [adViewSizes componentsSeparatedByString:@"x"];
                    
                    NSString *maxWidthString = [adSizesArray objectAtIndex:0];
                    int width = [maxWidthString intValue];
                    
                    NSString *maxHeightString = [adSizesArray objectAtIndex:1];
                    int height = [maxHeightString intValue];
                    
                    appDelegate.maxWidth = width;
                    appDelegate.maxHeight = height;
                    NSLog(@"MaxWidth= %d maxHeight=%d",appDelegate.maxWidth,appDelegate.maxHeight);
                        
                    }
                    else {
                        appDelegate.maxWidth = 0;
                        appDelegate.maxHeight = 0;
                        NSLog(@"MaxWidth= %d maxHeight=%d",appDelegate.maxWidth,appDelegate.maxHeight);

                    }
                    
                    // close the dropdown cell
                    
                    NSIndexPath *path0 = [NSIndexPath indexPathForRow:[path row]+1 inSection:[indexPath section]];
                    NSIndexPath *path1 = [NSIndexPath indexPathForRow:[path row]+2 inSection:[indexPath section]];
                    NSIndexPath *path2 = [NSIndexPath indexPathForRow:[path row]+3 inSection:[indexPath section]];
                    NSIndexPath *path3 = [NSIndexPath indexPathForRow:[path row]+4 inSection:[indexPath section]];
                    NSIndexPath *path4 = [NSIndexPath indexPathForRow:[path row]+5 inSection:[indexPath section]];
                    NSIndexPath *path5 = [NSIndexPath indexPathForRow:[path row]+6 inSection:[indexPath section]];
                    NSIndexPath *path6 = [NSIndexPath indexPathForRow:[path row]+7 inSection:[indexPath section]];
                    
                    
                    NSArray *indexPathArray = [NSArray arrayWithObjects:path0, path1, path2,path3, path4, path5,path6,nil];
                    
                    [cell setClosed];
                    isAdSizeDropdownOpen = [cell isOpen];
                    
                    [tableView deleteRowsAtIndexPaths:indexPathArray withRowAnimation:UITableViewRowAnimationTop];
                    
                    break;
                }
            }
            
            break;
        }
         
        default:
            break;
    }
    
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}


- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath;
{
    return 35.0f;
}



- (BOOL)textField:(UITextField *)textField shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)string {
    static NSCharacterSet *unacceptedInput = nil;
    if([textField tag]==3)
    {
    if (textField.text.length + string.length > 10) {
        return NO;
    }
    unacceptedInput = [[NSCharacterSet decimalDigitCharacterSet] invertedSet];
    return ([[string componentsSeparatedByCharactersInSet:unacceptedInput] count] <= 1);
    }
    if([textField tag]==4)
    {
        if (textField.text.length + string.length > 50) {
            return NO;
        }
        
    }
    if ([textField tag]==1 || [textField tag]==2) {
        if (textField.text.length + string.length > 12) {
            return NO;
        }

        if([string isEqualToString:@"-"] && [textField.text rangeOfString:@"-"].location == NSNotFound && textField.text.length == 0 )
        {
            return YES;
        }
        if([string isEqualToString:@"."] && [textField.text rangeOfString:@"."].location == NSNotFound)
        {
            return YES;
        }
        unacceptedInput = [[NSCharacterSet decimalDigitCharacterSet] invertedSet];
        NSMutableCharacterSet *unacceptedInput2 = [NSMutableCharacterSet characterSetWithCharactersInString:@"-"];
        [unacceptedInput2 formUnionWithCharacterSet:unacceptedInput]; 
        return ([[string componentsSeparatedByCharactersInSet:unacceptedInput2] count] <= 1);
    }
    return YES;
}


- (void)dealloc {
    [btnAutoDetectLocation release];
    [btnAutoDetectLocation release];
    [scrlViewSettings release];
    [txtLatitude release];
    [txtLongitude release];
    [super dealloc];
}
@end
