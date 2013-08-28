Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com

# SimpleSpeechUI example Android app for AT&T Speech SDK

This sample app includes Android source code and an Eclipse project to show how to call the AT&T Speech SDK from an application that wants a custom speech UI.  The app displays a simple client-side mashup.  Pressing a button initiates a speech interaction, a text area shows the recognition result, and a web view uses the recognized speech to search a website.  The text area also shows the progress of the speech interaction, and the button changes to "stop" or "cancel" during the phases of the interaction.

## Setting up the project

The SimpleSpeechUI Eclipse project is already configured to link with the Speech SDK, but it needs a copy of the files from the Speech SDK distribution.  Follow these steps to add the latest SpeechKit to the project.

1. If it's not present, create a `libs` subdirectory of this sample app.
2. Unzip the AT&T Speech SDK for Android into its own folder.
2. Copy the file `ATTSpeechKit.jar` into the `libs` subfolder.
4. Expand the `libs` group within the Eclipse project window.  You should see the ATTSpeechKit JAR there.

## Running the sample

Before building the sample app, you will need to add the configuration for your Speech API account: the URL of the service, your developer ID, and its password.  Set those values in `SpeechConfig.java`.  Before distributing your app to the public, make sure you add code to obfuscate those credentials.

## Understanding the sample

The main code of the sample app is in the `SimpleSpeechUIDemo` class.  Look in the `setupSpeechService` method and `SpeechButtonListener` inner class for examples of setting up and starting a speech interaction.  Look in the `SpeechResultListener`/`SpeechErrorListener` inner classes for handling recognition responses and errors.  Look in the `SpeechStateListener` and `SpeechLevelListener` inner classes for examples of handling SpeechKit callbacks for a custom UI.

## Reusable OAuth code

An example of OAuth client credential validation is in the class `SpeechAuth`.  You may use that class in your own applications, or you can use any other OAuth library. Look in `SimpleSpeechUIDemo.validateOAuth()` for an example of calling `SpeechAuth` to obtain an access token. 
