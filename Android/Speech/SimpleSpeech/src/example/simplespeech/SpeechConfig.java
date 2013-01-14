/*
Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com
*/
package example.simplespeech;

import java.io.UnsupportedEncodingException;
import java.util.Locale;
import java.util.SimpleTimeZone;
import java.util.TimeZone;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Build;
import android.text.format.Time;

import com.att.android.speech.ATTSpeechService;

/** Configuration parameters for this application's account on Speech API. **/
public class SpeechConfig {
    private SpeechConfig() {} // can't instantiate
    
    /** The URL of AT&T Speech API. **/
    static String serviceUrl() {
        return "https://api.att.com/rest/1/SpeechToText";
    }
    
    /** The URL of AT&T Speech API OAuth service. **/
    static String oauthUrl() {
        return "https://api.att.com/oauth/token";
    }
    
    /** Unobfuscates the OAuth client_id credential for the application. **/
    static String oauthKey() {
        // TODO: Replace this with code to unobfuscate your OAuth client_id.
        return myUnobfuscate(MY_OBFUSCATED_CLIENT_ID);
    }

    /** Unobfuscates the OAuth client_secret credential for the application. **/
    static String oauthSecret() {
        // TODO: Replace this with code to unobfuscate your OAuth client_secret.
        return myUnobfuscate(MY_OBFUSCATED_CLIENT_SECRET);
    }
    
    /** Percent-escape everything but unreserved characters in RFC 3986. **/
    private static String percentEscape(String orig)
    {
        // Unreserved URI characters are letters, numbers, and "-._~"
        // Can't use java.net.URLEncoder.encode() because
        // that converts spaces to pluses.
        try {
            StringBuilder s = new StringBuilder(orig.length());
            byte[] bytes = orig.getBytes("UTF8");
            String digits = "0123456789ABCDEF";
            for (byte b : bytes) {
                char ch = (char)b;
                if ((ch >= 'A' && ch <= 'Z')
                    || (ch >= 'a' && ch <= 'z')
                    || (ch >= '0' && ch <= '9')
                    || ch == '-' || ch == '.' || ch == '_' || ch == '~')
                    s.append(ch);
                else
                    s.append('%')
                     .append(digits.charAt((b & 0xf0) >> 4))
                     .append(digits.charAt(b & 0xf));
            }
            return s.toString();
        }
        catch (UnsupportedEncodingException e) {
            throw new RuntimeException("UTF8 not supported!", e);
        }
    }

    /** Value to use for X-Arg HTTP header. **/
    static String extraArguments(Context activity, String clientScreen)
    {
        StringBuilder s = new StringBuilder();
        s.append("ClientSdk=ATTSpeechKit-Android-").append(ATTSpeechService.VERSION);
        // Get the device name from the OS.
        String deviceType = Build.MANUFACTURER+' '+Build.MODEL;
        s.append(",DeviceType=").append(percentEscape(deviceType));
        // Get the OS version.
        String osVersion = Build.VERSION.RELEASE;
        s.append(",DeviceOs=Android-").append(percentEscape(osVersion));
        // Local time, with local time zone.
        Time time = new Time();
        time.setToNow();
        String deviceTime = time.format("%Y-%m-%d %H:%M:%S ")
                + SimpleTimeZone.getDefault()
                    .getDisplayName(time.isDst > 0, TimeZone.SHORT, Locale.US);
        s.append(",DeviceTime=").append(percentEscape(deviceTime));
        // Extract the app name and version from AndroidManifest.
        PackageInfo pinfo;
        try {
            pinfo = activity.getPackageManager().getPackageInfo(activity.getPackageName(), 0);
        }
        catch (NameNotFoundException e) {
            throw new IllegalStateException("Can't get activity metadata", e);
        }
        String clientApp = pinfo.packageName;
        s.append(",ClientApp=").append(percentEscape(clientApp));
        String clientVersion = pinfo.versionName;
        s.append(",ClientVersion=").append(percentEscape(clientVersion));
        s.append(",ClientScreen=").append(percentEscape(clientScreen));
        //Log.d("SimpleSpeech", "X-Arg: "+s);
        return s.toString();
    }

}
