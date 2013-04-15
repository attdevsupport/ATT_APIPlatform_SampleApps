//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import "ATTPopupWindow.h"

#define kShadeViewTag 1000

@interface ATTPopupWindow(Private)
//- (id)initWithSuperview:(UIView*)sview andFile:(NSString*)fName;
- (id)initWithSuperview:(UIView*)sview withChildView:(UIView*)cview;

@end


@implementation ATTPopupWindow
@synthesize bigPanelView,bgView;
/**
 * This is the only public method, it opens a popup window and loads the given content
 * @param UIView* insideView provide a child view to be added to the main view
 * @param UIView* view provide a UIViewController's view here (or other view)
 */

-(void)showWindowWithView:(UIView*)childView insideView:(UIView*)view{
    [self initWithSuperview:(UIView*)view withChildView:(UIView*)childView];
    
}


/**
 * Initializes the class instance, gets a view where the window will pop up in
 */

- (id)initWithSuperview:(UIView*)sview withChildView:(UIView*)cview {
//    self = [super init];
    if (self) {
        // Initialization code here.
        bgView = [[UIView alloc] initWithFrame: sview.bounds];
        [sview addSubview: bgView];
        
        
        // proceed with animation after the bgView was added
        [self performSelector:@selector(doTransitionWithChildView:) withObject:cview afterDelay:0.1];
    }
    
    return self;
}


/**
 * Afrer the window background is added to the UI the window can animate in
 * and load the UIWebView
 */

-(void)doTransitionWithChildView:(UIView*)cview{
    //faux view
    [cview setBackgroundColor:[UIColor clearColor]];
    UIView* fauxView = [[UIView alloc] initWithFrame: CGRectMake(10, 10, 200, 200)];
    [bgView addSubview: fauxView];

    //the new panel
    bigPanelView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, bgView.frame.size.width, bgView.frame.size.height)] ;
    bigPanelView.center = CGPointMake( bgView.frame.size.width/2, bgView.frame.size.height/2);
    
    //add the window background
    UIImageView* background = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"popupWindowBack.png"]];
    background.center = CGPointMake(bigPanelView.frame.size.width/2, bigPanelView.frame.size.height/2);
    cview.center = CGPointMake(bigPanelView.frame.size.width/2, bigPanelView.frame.size.height/2);
    cview.layer.cornerRadius = 16;
    cview.layer.masksToBounds = YES;
    [bigPanelView addSubview: background];
    [bigPanelView addSubview: cview];
    
    //add the close button
    int closeBtnOffset = 10;
    UIImage* closeBtnImg = [UIImage imageNamed:@"popupCloseBtn.png"];
    UIButton* closeBtn = [UIButton buttonWithType:UIButtonTypeCustom];
    [closeBtn setImage:closeBtnImg forState:UIControlStateNormal];
    [closeBtn setFrame:CGRectMake( background.frame.origin.x + background.frame.size.width - closeBtnImg.size.width - closeBtnOffset, 
                                  background.frame.origin.y ,
                                  closeBtnImg.size.width + closeBtnOffset, 
                                  closeBtnImg.size.height + closeBtnOffset)];
    [closeBtn addTarget:self action:@selector(closePopupWindow) forControlEvents:UIControlEventTouchUpInside];
    [bigPanelView addSubview: closeBtn];
    
    //animation options
    UIViewAnimationOptions options = UIViewAnimationOptionTransitionFlipFromRight |
    UIViewAnimationOptionAllowUserInteraction    |
    UIViewAnimationOptionBeginFromCurrentState;
    
    //run the animation
    [UIView transitionFromView:fauxView toView:bigPanelView duration:0.5 options:options completion: ^(BOOL finished) {
        
        //dim the contents behind the popup window
        UIView* shadeView = [[UIView alloc] initWithFrame:bigPanelView.frame];
        shadeView.backgroundColor = [UIColor blackColor];
        shadeView.alpha = 0.3;
        shadeView.tag = kShadeViewTag;
        NSLog(@"%@",shadeView);
        [bigPanelView addSubview: shadeView];
        //[shadeView release];
        [bigPanelView sendSubviewToBack: shadeView];
    }];
}



/**
 * Removes the window background and calls the animation of the window
 */
-(void)closePopupWindow
{
    //remove the shade
    [[bigPanelView viewWithTag: kShadeViewTag] removeFromSuperview]; 
    [self performSelector:@selector(closePopupWindowAnimate) withObject:nil afterDelay:0.1];
    
}


/**
 * Animates the window and when done removes all views from the view hierarchy
 * since they are all only retained by their superview this also deallocates them
 * finally deallocate the class instance
 */
-(void)closePopupWindowAnimate
{
    
    //faux view
    __block UIView* fauxView = [[UIView alloc] initWithFrame: CGRectMake(10, 10, 200, 200)];
    [bgView addSubview: fauxView];

    //run the animation
    UIViewAnimationOptions options = UIViewAnimationOptionTransitionFlipFromLeft |
    UIViewAnimationOptionAllowUserInteraction    |
    UIViewAnimationOptionBeginFromCurrentState;
    
    //hold to the bigPanelView, because it'll be removed during the animation
    [bigPanelView retain];
    
    [UIView transitionFromView:bigPanelView toView:fauxView duration:0.5 options:options completion:^(BOOL finished) {

        //when popup is closed, remove all the views
        for (UIView* child in bigPanelView.subviews) {
            if (child.tag == 100) {
                [child removeFromSuperview];
                [child dealloc];
                child = nil;
            }
            else {
                [child removeFromSuperview];
            }
        }
        for (UIView* child in bgView.subviews) {
            [child removeFromSuperview];
        }
        
        [bigPanelView release];
        [bgView removeFromSuperview];

        [self release];
    }];
}

@end