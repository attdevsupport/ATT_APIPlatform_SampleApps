Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com

# SimpleTTS example app for AT&T Speech API and SpeechKit for Android

This sample app includes source code and an Eclipse project to show how to call the AT&T Speech API and Android SpeechKit from an application.  The app demonstrates both the Speech to Text and Text to Speech (TTS) services.  At launch time, the app speaks a greeting.  The user speaks a question, and the app speaks out an answer.

## Setting up the project

The SimpleTTS Eclipse project is already configured to link with SpeechKit, but it needs a copy of the files from the SpeechKit distribution.  Follow these steps to add the latest SpeechKit to the project.

1. If it's not present, create a `libs` subdirectory of this sample app.
2. Unzip the AT&T Android SpeechKit SDK into its own folder.
2. Copy the file `ATTSpeechKit.jar` into the `libs` subfolder.
4. Expand the `libs` group within the Eclipse project window.  You should see the ATTSpeechKit JAR there.

## Running the sample

Before building the sample app, you will need to add the configuration for your Speech API account: the URL of the service, your developer ID, and its password.  Set those values in `SpeechConfig.java`.  Before distributing your app to the public, make sure you add code to obfuscate those credentials.

## Understanding the sample

The main code of the sample app is in the SimpleTTSDemo class.  Look at the `readyForSpeech` method to see how to start TTS playback, and the `startSpeechService` method shows how to set up and start speech recognition.  The TTSClient inner class has an example of handling TTS responses.  Look in the ResultListener/ErrorListener classes for code handling recognition responses and errors.

## Reusable TTS code

The Text to Speech client code is in the class `TTSRequest`.  This class is self-contained, and you can use it in your own applications if you wish.  Note that TTS functionality is completely independent of the speech recogntion code in the ATTSpeechKit library.  If you application only has TTS and doesn't use recognition, you don't need to link ATTSpeechKit to your app.

## Reusable OAuth code

An example of OAuth client credential validation is in the class `SpeechAuth`.  You may use that class in your own applications, or you can use any other OAuth library. 

