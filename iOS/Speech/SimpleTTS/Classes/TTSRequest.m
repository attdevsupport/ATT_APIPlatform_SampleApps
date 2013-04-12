//  TTSRequest.m
//
// Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&T Intellectual Property. All rights reserved.
// For more information contact developer.support@att.com http://developer.att.com

#import "TTSRequest.h"
#import "ATTSpeechKit.h"

typedef enum
{
    LoaderStateUnknown = 0,
    LoaderStateInitialized,
    LoaderStateConnecting,
    LoaderStateReceivedResponse,
    LoaderStateReceivedData,
    LoaderStateFinished,
    LoaderStateFailed,
    LoaderStateCanceling,
    LoaderStateCanceled
} LoaderState;

// Memory Management
// 
// This object will retain its initialiation parameters (the NSURLRequest)
// during the lifetime of this object.  
// It will retain the connection, response, data, and delegate only between
// the call to start and the callbacks to the delegate.
// It will also add a retain of itself during that interval.  That way, the 
// client can autorelease this object after starting, and this object 
// will remain in memory while active.

@interface TTSRequest () {
    @private
    LoaderState state;
}
@property (copy) TTSClientBlock clientBlock;
@property (retain) NSMutableURLRequest* request;
@property (retain) NSURLConnection* connection;
@property (retain) NSHTTPURLResponse* response;
@property (retain) NSMutableData* data;

- (NSInteger) statusCode;
- (void) clear;
@end

@implementation TTSRequest

@synthesize clientBlock = _clientBlock;
@synthesize request = _request;
@synthesize connection = _connection;
@synthesize response = _response;
@synthesize data = _data;


- (id) initWithRequest: (NSMutableURLRequest*) request
{
    self = [super init];
    if (self != nil)
    {
        // First see if the request can be handled.
        if (![NSURLConnection canHandleRequest: request]) {
            [self release];
            return nil;
        }
        self.request = request; 
        self.data = [NSMutableData data];
        _connection = nil; // Create connection when client wants to start loading.
        _response = nil;
        state = LoaderStateInitialized;
    }
    return self;
}

/** Tune the timeout values based on application behavior. **/
static const NSTimeInterval CONNECT_TIMEOUT = 30.0; // seconds

/**
 * Creates a basic TTSRequest object for the service.
 * Follow this with a call to postText:forClient to actually perform the TTS
 * conversion and fetch the data.
 * @param ttsService the URL of the Text to Speech service
 * @param oauthToken the OAuth client credentials token for the application
 **/
+ (TTSRequest*) forService: (NSURL*) ttsService
                 withOAuth: (NSString*) oauthToken
{
    NSMutableURLRequest* request = [NSMutableURLRequest requestWithURL: ttsService];
    request.HTTPMethod = @"POST";
    [request setValue: [NSString stringWithFormat: @"BEARER %@", oauthToken]
        forHTTPHeaderField: @"Authorization"];
    request.timeoutInterval = CONNECT_TIMEOUT;   
    
    // TODO: Add the other request properties needed by your application,
    //       such as response type, language, etc.
    [request setValue: @"audio/wav" forHTTPHeaderField: @"Accept"];
    
    return [[[self alloc] initWithRequest: request] autorelease];
}

- (void) dealloc
{
    // We should have already been cleared, but just in case...
    [_connection cancel];
    
    self.request = nil;
    self.response = nil;
    self.data = nil;
    self.clientBlock = nil;
    self.connection = nil;

    [super dealloc];
}

/**
 * Begin posting the text and fetching the audio response asynchronously.
 * @param text  the text to convert to audio
 * @param clientBlock  the callback to receive the response
 **/
- (void) postText: (NSString*) text forClient: (TTSClientBlock) clientBlock
{
    self.clientBlock = clientBlock;
    _request.HTTPBody = [text dataUsingEncoding: NSUTF8StringEncoding];
    [_request setValue: @"text/plain" forHTTPHeaderField: @"Content-Type"];
    [self start];
}



- (NSInteger) statusCode
{
    if (_response == nil)
        return 100; // HTTP "Continue"
    else
        return _response.statusCode;
}

- (void) start
{
    state = LoaderStateConnecting;
    // Add a retention to this object do it doesn't dispose during the connection.
    [self retain];

    // Allocate the NSURLConnection and start it in one step.
    self.connection = [NSURLConnection connectionWithRequest: _request 
                                                    delegate: self];
        
    if (_connection == nil) {
        state = LoaderStateFailed;
        // Report the error the delegate on the next time through the runloop.
        [[NSOperationQueue mainQueue] addOperationWithBlock: ^{
            // TO DO: the arguments to NSError are completely arbitrary!
            NSError* error = [NSError errorWithDomain: ATTSpeechServiceErrorDomain 
                                                 code: ATTSpeechServiceErrorCodeConnectionFailure 
                                             userInfo: nil];
            _clientBlock(nil, error);
            [self clear];
        }];
        return;
    }
    // Don't call [_connection start], since it's already started.
}

- (void) clear
{
    // Completely dispose the connection, delegate, and response data 
    // when we are done.
    self.clientBlock = nil;
    [_connection cancel];
    self.connection = nil;
    self.response = nil;
    _data.length = 0;
    // And release the retain count we added during start.
    [self release];
}

- (void) cancel
{
    // Completely dispose the connection when we cancel it.
    if (state != LoaderStateFinished) {
        state = LoaderStateCanceling;
        [self clear];
    }
}

// NSURLConnection delegate methods


- (void) connection: (NSURLConnection*) connection 
 didReceiveResponse: (NSURLResponse*) response
{
    // The connection just got a new response.  Clear out anything we've already loaded.
    self.response = (NSHTTPURLResponse*)response;
    _data.length = 0;
    state = LoaderStateReceivedResponse;
}

- (void) connection: (NSURLConnection*) connection 
     didReceiveData: (NSData*) data
{
    // The connection is sending us some data incrementally.
    [_data appendData: data];
    state = LoaderStateReceivedData;
}

- (void) connectionDidFinishLoading: (NSURLConnection*) connection
{
    // Loading is complete. 
    state = LoaderStateFinished;

    NSError* error = nil;
    BOOL succeeded = NO;
    if (self.statusCode == 200 && _data.length) {
        _clientBlock([_data copy], nil);
        succeeded = YES;
    }
    if (!succeeded) {
        // TO DO: What data should we put in the userInfo?
        if (error == nil)
            error = [NSError errorWithDomain: ATTSpeechServiceHTTPErrorDomain
                                        code: self.statusCode userInfo: nil];
        _clientBlock(nil, error);
    }
    
    // The callback is complete, so clean up.
    [self clear];
}

- (void) connection: (NSURLConnection*) connection didFailWithError: (NSError*) error
{
    // Loading failed. 
    state = LoaderStateFailed;

    _clientBlock(nil, error);
    
    // The callback is complete, so clean up.
    [self clear];
}

@end


