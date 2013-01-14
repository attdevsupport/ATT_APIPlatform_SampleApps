//  SpeechConfig.m
//
// Implements customization parameters for this application's use of
// AT&T Speech SDK.
//
// Customize the functions declared here with the parameters of your application.

#import "SpeechConfig.h"
#import <Foundation/NSString.h>
#import <Foundation/NSDictionary.h>
#import <Foundation/NSURL.h>
#import <sys/sysctl.h>
#import <UIKit/UIDevice.h>
#import <time.h>
#import <Foundation/NSBundle.h>
#import "ATTSpeechKit.h"


/** The URL of AT&T Speech API. **/
NSURL* SpeechServiceUrl(void)
{
    return [NSURL URLWithString: @"https://api.att.com/rest/1/SpeechToText"];
}

/** The URL of AT&T Speech API OAuth service. **/
NSURL* SpeechOAuthUrl(void)
{
    return [NSURL URLWithString: @"https://api.att.com/oauth/token"];
}

/** Unobfuscates the OAuth client_id credential for the application. **/
NSString* SpeechOAuthKey(void)
{
    #error Add code to unobfuscate your Speech API credentials, then delete this line.
    return MY_UNOBFUSCATE(my_obfuscated_client_id);
}

/** Unobfuscates the OAuth client_secret credential for the application. **/
NSString* SpeechOAuthSecret(void)
{
    #error Add code to unobfuscate your Speech API credentials, then delete this line.
    return MY_UNOBFUSCATE(my_obfuscated_client_secret);
}

/** Percent-escape everything but unreserved characters in RFC 3986. **/
static NSString* percentEscape(NSString* orig)
{
    // Unreserved URI characters are letters, numbers, and "-._~"
    // Can't use -[NSString stringByAddingPercentEscapesUsingEncoding:] because
    // that leaves characters like ',' unescaped.
    NSString* escapedString = (NSString*)
        CFURLCreateStringByAddingPercentEscapes(NULL,
            (CFStringRef) orig,
            NULL, // don't leave anything unescaped that should be escaped
            CFSTR("!\"#$%&\'()*+,/:;<=>?@[\\]^`{|}"), // escape these
            kCFStringEncodingUTF8);
    return [escapedString autorelease];
}

/** Value to use for X-Arg HTTP header. **/
NSString* SpeechExtraArguments(NSString* clientScreen)
{
    NSMutableString* s = [NSMutableString string];
    [s appendString: @"ClientSdk=ATTSpeechKit-iOS-" ATTSKVersionString];
    // Get the model name from the OS.
    // UIDevice only returns strings like "iPhone", which don't help distinguish
    // microphone hardware between e.g. iPhone 3GS and iPhone 5.
    NSString* deviceType;
    {
        size_t len;
        sysctlbyname("hw.machine", NULL, &len, NULL, 0);
        char* cstr = alloca(len);
        sysctlbyname("hw.machine", cstr, &len, NULL, 0);
        deviceType = [NSString stringWithCString: cstr encoding: NSUTF8StringEncoding];
    }
    [s appendString: @",DeviceType="];
    [s appendString: percentEscape(deviceType)];
    // Get the OS version.
    UIDevice* device = [UIDevice currentDevice];
    NSString* osVersion = device.systemVersion;
    [s appendString: @",DeviceOs=iOS-"];
    [s appendString: percentEscape(osVersion)];
    // Local time, with local time zone.
    NSString* deviceTime;
    {
        time_t epochtime = time(NULL);
        struct tm tstruct = {0};
        localtime_r(&epochtime, &tstruct);
        char cstr[100] = {0};
        strftime(cstr, 100, "%Y-%m-%d %H:%M:%S %Z", &tstruct);
        deviceTime = [NSString stringWithCString: cstr encoding: NSUTF8StringEncoding];
    }
    [s appendString: @",DeviceTime="];
    [s appendString: percentEscape(deviceTime)];
    // Extract the app name and version from info.plist.
    NSDictionary* info = [[NSBundle mainBundle] infoDictionary];
    NSString* clientApp = [info objectForKey: (NSString*) kCFBundleIdentifierKey];
    [s appendString: @",ClientApp="];
    [s appendString: percentEscape(clientApp)];
    NSString* clientVersion = [info objectForKey: (NSString*) kCFBundleVersionKey];
    [s appendString: @",ClientVersion="];
    [s appendString: percentEscape(clientVersion)];
    [s appendString: @",ClientScreen="];
    [s appendString: percentEscape(clientScreen)];
    NSLog(@"X-Arg: %@", s);
    return s;
}
