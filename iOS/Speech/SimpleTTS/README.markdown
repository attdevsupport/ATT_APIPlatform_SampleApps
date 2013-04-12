Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com

# SimpleTTS example app for AT&T Speech SDK

This sample app includes source code and an Xcode project to show how to call the AT&T Speech API and iOS Speech SDK from an application.  The app demonstrates both the Speech to Text and Text to Speech (TTS) services.  At launch time, the app speaks a greeting.  The user speaks a question, and the app speaks out an answer.

## Setting up the project

The SimpleTTS Xcode project is already configured to link with the Speech SDK, but it needs a copy of the files from the Speech SDK distribution.  Follow these steps to add the latest SpeechKit to the project.

1. Unzip the AT&T Speech SDK into its own folder.
2. Copy the files `ATTSpeechKit.a` and `ATTSpeechKit.h` into the `ATTSpeechKit` subfolder of this sample app.
3. Open the SimpleTTS Xcode project.
4. Expand the `ATTSpeechKit` group within the project window.  Both files should appear in black text, not red (which would indicate that Xcode can't find them).

## Running the sample

Before building the sample app, you will need to add the configuration for your Speech API account: the URL of the service, your application's API key and API secret.  Set those values in `SpeechConfig.m`.   Before distributing your app to the public, make sure you add code to obfuscate those credentials.

## Understanding the speech code

The main code of the sample app is in the `SimpleTTSViewController` class.  Look at the `[startTTS:]` method to see how to perform TTS playback, and in the `-[prepareSpeech]` and `-[listen:]` methods for examples of setting up and starting speech recognition.  Look in the `-[speechServiceSucceeded:]` and `[speechService:failedWithError:]` methods for examples of handling recognition responses.  

## Reusable TTS code

The Text to Speech client code is in the class `TTSRequest`.  This class is self-contained, and you can use it in your own applications if you wish.  Note that TTS functionality is completely independent of the speech recogntion code in the ATTSpeechKit library.  If you application only has TTS and doesn't use recognition, you don't need to link ATTSpeechKit to your app.

## Reusable OAuth code

The `SpeechAuth` class provides example code for authenticating your application with the OAuth client credentials protocol.  It performs an asynchronous network request for an OAuth access token that can be used in the Speech API.  Look in `-[SimpleTTSViewController prepareSpeech]` for examples of calling `SpeechAuth` to obtain an access token. 
