//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
// 

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>

@interface ATTPopupWindow : NSObject
{
    UIView* bgView;
    UIView* bigPanelView;
}
@property (nonatomic, retain)UIView* bigPanelView;
@property (nonatomic, retain)UIView* bgView;
-(void)showWindowWithView:(UIView*)childView insideView:(UIView*)view;
-(void)closePopupWindow;

@end
