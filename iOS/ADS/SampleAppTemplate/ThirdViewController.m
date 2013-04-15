//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import "ThirdViewController.h"


@interface ThirdViewController ()

@end

@implementation ThirdViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.title = NSLocalizedString(@"JSONResponse", @"JSONResponse");
        self.tabBarItem.image = [UIImage imageNamed:@"Response"];
    }
    return self;
}

#pragma mark - View Life Cycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    appDelegate= (AppDelegate *)[[UIApplication sharedApplication] delegate];
    ///Displaying the jsonResponse from the appDelegate on the UITextView and Logging in the console, as the View Loads
    
    responseView.text = appDelegate.jsonResponse;
    
    NSLog(@"appdelegate json response = %@",appDelegate.jsonResponse);
    
}
///Displaying the jsonResponse from the appDelegate on the UITextView and Logging in the console, as the View Appears.
- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    responseView.text = appDelegate.jsonResponse;
    
    NSLog(@"appdelegate json response = %@",appDelegate.jsonResponse);
    
}

- (void)viewDidUnload
{
    responseView = nil;
    [super viewDidUnload];
    // Release any retained subviews of the main view.
}



- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

@end
