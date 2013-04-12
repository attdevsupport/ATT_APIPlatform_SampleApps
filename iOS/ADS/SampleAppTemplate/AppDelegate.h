//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate, UITabBarControllerDelegate>
{
    
//    NSString *jsonResponse;
//    BOOL isDetectLocation;
//    NSString *latitude;
//    NSString *longitude;
//    NSString *category;
}

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) UITabBarController *tabBarController;
@property (nonatomic, retain) NSString *keyWords;
@property (nonatomic, assign) NSInteger zipCode;
@property (nonatomic,assign) NSInteger premium;
@property (nonatomic,assign) NSInteger maxHeight;
@property (nonatomic,assign) NSInteger maxWidth;
@property (nonatomic, retain) NSString *ageGroup;
@property (nonatomic, retain)  NSString *jsonResponse;
@property (nonatomic)  BOOL isDetectLocation;
@property (nonatomic, retain) NSString *latitude;
@property (nonatomic, retain) NSString *longitude;
@property (nonatomic, retain) NSString *category;

@end
