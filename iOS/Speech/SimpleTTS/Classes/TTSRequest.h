//  TTSRequest.h
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import <Foundation/NSObject.h>

@class NSString, NSError;

/**
 * Type of block called when TTSRequest gets audio response or fails.
 * If there's an problem authenticating, audioData will be nil and
 * error will contain the error.
 * @param audioData the bytes of audio data on success, or nil if TTS failed
 * @param error null on success, or the error if TTS failed
**/
typedef void (^TTSClientBlock)(NSData* audioData, NSError* error);

/**
 * Posts to AT&T Text to Speech service and fetches audio response, 
 * calling a block when done.
 * Performs the load asynchronously so that it doesn't block the main UI.
**/
@interface TTSRequest : NSObject {
}

/**
 * Creates a basic TTSRequest object for the service. 
 * Follow this with a call to postText:forClient to actually perform the TTS
 * conversion and fetch the data.
 * @param ttsService the URL of the Text to Speech service
 * @param oauthToken the OAuth client credentials token for the application
**/
+ (TTSRequest*) forService: (NSURL*) ttsService
                 withOAuth: (NSString*) oauthtoken;

/**
 * Begin posting the text and fetching the audio response asynchronously.
 * @param text  the text to convert to audio
 * @param clientBlock  the callback to receive the response
 **/
- (void) postText: (NSString*) text forClient: (TTSClientBlock) clientBlock;

/** Stop fetching. Once stopped, loading may not resume. **/
- (void) cancel;

@end
