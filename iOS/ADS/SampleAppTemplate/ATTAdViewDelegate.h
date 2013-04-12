

//
//  ATTAdViewDelegate.h
//  AT&T Ad SDK
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: 
// http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//

#ifndef ATTAdLibrary_ATTAdDelegate_h
#define ATTAdLibrary_ATTAdDelegate_h

/**
 
 Delegate to handle the call backs from the ATTAdView
 
 One must confirm to this protocol to receive the notifications
 
 when the ad is received successfully or failed to receive
 
 */


@protocol ATTAdViewDelegate


/**
 
 Success delegate
 
 Delegate method receives message when the
 
 ad is received successfully and rendered.
 
 This message also receives the JSON response
 
 received from the service call, which can be
 
 used for logging or dubugging purposes.
 
 @param jsonResponse Raw response received from Ad Service
 
 */


-(void) didReciveAd:(NSString *)jsonResponse;



/**
 
 Failure Delegate
 
 Delegate method receives message when the
 
 app is failed to receive valid response.
 
 This message receives the NSError received
 
 from the service call.  The app developer
 
 may handle different way than the way SDK supports.
 
 @param nsError: NSError object containing the error details
 
 */

-(void) didFailedToReceiveAd:(NSError*)nsError; 


@end


#endif
