//
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#import <UIKit/UIKit.h>


@interface DropDownCell : UITableViewCell {
    
    IBOutlet UILabel *textLabel;
    IBOutlet UIImageView *arrow_up;
    IBOutlet UIImageView *arrow_down;
    
    BOOL isOpen;

}

- (void) setOpen;
- (void) setClosed;

@property (nonatomic) BOOL isOpen;
@property (nonatomic, retain) IBOutlet UILabel *textLabel;
@property (nonatomic, retain) IBOutlet UIImageView *arrow_up;
@property (nonatomic, retain) IBOutlet UIImageView *arrow_down;

@end
