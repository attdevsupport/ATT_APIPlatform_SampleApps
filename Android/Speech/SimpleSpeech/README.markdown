Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com

# SimpleSpeech example app for AT&T Android SpeechKit

This sample app includes source code and an Eclipse project to show how to call the AT&T Android SpeechKit from an application.  The app displays a simple client-side mashup.  Pressing a button initiates a speech interaction, a text area shows the recognition result, and a web view uses the recognized speech to search a website.

## Setting up the project

The SimpleSpeech Eclipse project is already configured to link with SpeechKit, but it needs a copy of the files from the SpeechKit distribution.  Follow these steps to add the latest SpeechKit to the project.

1. If it's not present, create a `libs` subdirectory of this sample app.
2. Unzip the AT&T Android SpeechKit SDK into its own folder.
2. Copy the file `ATTSpeechKit.jar` into the `libs` subfolder.
4. Expand the `libs` group within the Eclipse project window.  You should see the ATTSpeechKit JAR there.

## Running the sample

Before building the sample app, you will need to add the configuration for your Speech API account: the URL of the service, your developer ID, and its password.  Set those values in `SpeechConfig.java`.  Before distributing your app to the public, make sure you add code to obfuscate those credentials.

## Understanding the sample

The main code of the sample app is in the `SimpleSpeechActivityDemo` and `SimpleSpeechServiceDemo` classes.  Look in the `startSpeechActivity` and `startSpeechService` methods for examples of setting up and starting a speech interaction.  Look in the `onActivityResult` method and `ResultListener`/`ErrorListener` inner classes for handling recognition responses and errors.

## Reusable OAuth code

An example of OAuth client credential validation is in the class `SpeechAuth`.  You may use that class in your own applications, or you can use any other OAuth library. 
