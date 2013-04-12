//
//  ATTAdView.h
//  AT&T Ad SDK
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com
//
//




#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "ATTAdViewDelegate.h"




#define ATTAdVersionString	@"1.0.0"
#define ATTAdBuildString	@"1F001"


// Enumeration to specify Premium or Non-Premium or both to be accepted
typedef enum {
	PremiumNonPremium = 0,     	// show only Non-Premium ads
	PremiumPremium = 1,     	// show only Premium ads
	PremiumBoth = 2,     		// show both Premium and Non-Premius ads
} Premium;

// Enumeration to specify Type of ads to be displayed, i.e. Text, Image, Both or No preference
typedef enum {
	TypeAll = -1,     			// No preference, show any type of ads
	TypeTextOnly = 1,     		// Show only text ads
	TypeImagesOnly = 2,    		// show only image ads
	TypeImagesAndText = 3,     	// show both text and image type of ads
} Type;

// Enumeration to specify Over18 category to be denied/accepted
typedef enum {
	Over18DenyOver18 = 1,     	// Deny Over18 ads
	Over18OnlyOver18 = 2,     	// Show only Over18 ads
	Over18All = 3,     			// Show all ads
} Over18;


/**
 Use the ATTAdView class to embed advertisement content in your application.  To do so, you simply create an instance of the ATTAdView object 
 and add it to a UIView.  An instance of ATTAdView is the means for displaying advertisements from the ad publisher's site.  
 
 The ATTAdView handles the rendering of any content in its area: text or images.  The ad view also handles user interactions with
 the advertisement content.  Ads generally have links that allow users to visit web sites.  To control all interactions implement the 
 ATTAdViewDelegate protocol.
 
 Advanced ad view customization is supported.  Ad content can be filtered using the premium property.  Use the properties minSize and maxSize
 to configure ad content size in server response.  Also you can set the search parameters using the keywords.
 
 
 Set the delegate property to an object conforming to the ATTAdViewDelegate protocol if you want to listen the processing of ad content.
 */



@interface ATTAdView : UIView<NSURLConnectionDelegate,UIWebViewDelegate>
{

    id <ATTAdViewDelegate>	attAdViewDelegate;
}

@property (assign) id <ATTAdViewDelegate>	attAdViewDelegate;


/**
 
 Udid would be provided by the developer
 It should be atleast 30 characters . If its <30 then
 an error will be returned
 
 */

@property (retain) NSString* udid;


/**
 
 Category filter for the advertisement
 
 Use this property to filter the ad content basing
 
 on one of the listed categories.  These are pre-defined
 
 and the SDK supports the below values:
 
 
 
 The value is required.
 
 */

@property (retain) NSString*            category;


/**
 
 Ad Service URL
 
 Use this property to override the default ad service URL
 
 The value is optional.
 
 */

@property (retain) NSString*            adServiceURL;


/**
 
 Gender of a user M or F.
 
 The value is optional and default is nil.
 
 */

@property (retain) NSString*            gender;


/**
 
 Zip/Postal code of user.  For US only.
 
 The value is optional and default is nil.
 
 */

@property (assign) NSInteger			zipCode;


/** 
 
 Area code of a user. For US only.
 
 The value is optional and default is nil.
 
 */

@property (assign) NSInteger           areaCode;


/** 
 
 City of the device user (with state). For US only.
 
 The value is optional and default is nil.
 
 */

@property (retain) NSString*            city;


/** 
 
 Country of visitor. 
 
 Overrides the country detected by IP.
 
 An ISO 3166 code to be used for specifying a country code.
 
 The value is optional and default is nil.
 
 */

@property (retain) NSString*            country;


/**
 
 Current Longitude of the user location
 
 User location latitude value (in degrees).
 
 The value is optional and default is nil.
 
 */

@property (retain)NSString*						longitude;


/** 
 
 Current Latitude of the user location
 
 User location longitude value (in degrees).
 
 The value is optional and default is nil.
 
 */

@property (retain)NSString*						latitude;


/** 
 
 Maximum height of the ad content can be shown.
 
 Use this property to set the maximum height of the ad content
 
 when requesting an ad from the server.  
 
 The content height may be smaller than this value.
 
 The value is optional and default is nil.
 
 */

@property NSInteger						maxHeight;


/** 
 
 Maximum width of the ad content can be shown.
 
 Use this property to set the maximum width of the ad content when 
 
 requesting an ad from the server.  
 
 The content width may be smaller than this value.
 
 The value is optional and default is nil.
 
 */

@property NSInteger						maxWidth;


/** 
 
 Minimal height of the ad content can be shown.
 
 Use this property to set the minimal height of the ad content 
 
 when requesting an ad from the server.  
 
 The content height may be larger than this value.
 
 The value is optional and default is nil.
 
 */

@property NSInteger						minHeight;

/** 
 
 Minimal width of the ad content can be shown.
 
 Use this property to set the minimal width of the ad content 
 
 when requesting an ad from the server.  
 
 The content width may be larger than this value.
 
 The value is optional and default is nil.
 
 */

@property NSInteger						minWidth;


/** 
 
 
 
 Ad type filter.
 
 Use this property to specify the type of ad accepted.
 
 The value is optional and default is TypeAll (-1)
 
 */

@property Type                          type;


/** 
 
 Timeout of the ad call
 
 Use this property to set maximum time you are willing 
 
 to wait for an ad response. 
 
 Default value is 1000ms. Maximum value is 3000ms.
 
 */

@property NSInteger						timeout;


/** 
 
 Age Group filter.
 
 Use this property to specify the age group of the 
 
 demographic audience of the app
 
 The value is optional and default is nil.
 
 */

@property (assign) NSString*            ageGroup;


/** 
 
 Over18 filter.
 
 Use this property to filter ads by over 18 content:
 
 0 or 1 ó deny over 18 content, 
 
 2 ó only over 18 content, 
 
 3 - allow all ads
 
 The value is optional and default is nil.
 
 */

@property Over18                        over18;


/** Ad Content Selection */

/**
 
 Search words for the requested ad.
 
 Use this property to filter the ads basing on
 
 keywords.
 
 The value is optional and default is nil.
 
 */

@property (assign) NSString*			keywords;



/** 
 
 Image size request
 
 Use this property to request the image size (width and height) 
 
 in the response
 
 The value is optional and default is NO
 
 */

@property BOOL                          isSizeRequired;


/** 
 
 Ad premium filter
 
 Use this property to filter the content of ad by premium status.
 
 The default value is PremiumNonPremium (0)
 
 */

@property Premium                       premium;



/** 
 
 Ad Refresh period in seconds.
 
 Use this property to refresh the Ad for every interval of specified period.
 
 If the ad is not required to be refreshed, set the value to zero (0)
 
 The value is optional and default is 120 seconds
 
 */

@property (nonatomic, assign)NSInteger adRefreshPeriod;


/** 
 
 The Ad component will be collapsed, if there is any error
 
 in getting the Ad.  In addition, the ATTAdViewDelegate will also
 
 be notified of success or failure.  If the app developer
 
 just want to remove the component, the parameter can be
 
 used.
 
 
 The value is optional and default is NO.
 
 */

@property (nonatomic, assign)BOOL onErrorCollapse;



/** 
 
 Creates the ATTAdView component
 
 Initializes and returns an instance of ATTAdView having the given frame
 
 using the app key, secret and other provided properties e.g. category, city, 
 
 zipCode etc.  The SDK requests for access token, mainitains
 
 in the app and re-uses for the further requests.  Alternatively, the 
 
 app developer can keep all this logic in the app and directly pass the
 
 access token directly.
 
 @param frame The frame, in which the ad content is to rendered
 
 @param appKey The App Key provided by the AT&T Developer forum
 
 @param appSecret Secret provided by the AT&T Developer forum for the app
 
 @return ATTAdView The component containg the ad content received from Ad Service
 
 
 */

-(id)initWithFrame:(CGRect)frame appKey:(NSString*)appKey appsecret:(NSString*)appSecret category:(NSString*)adCategory udid:(NSString*) Udid;




@end
