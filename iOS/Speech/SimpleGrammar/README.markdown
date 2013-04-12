Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. 
For more information contact developer.support@att.com http://developer.att.com

# SimpleGrammar example app for AT&T Speech SDK

This sample app includes source code and an Xcode project to show how to call the AT&T Speech to Text Custom service and Speech SDK from an application.  The app displays a bare-bones screen with button that initiates a speech interaction and a table that shows how the speech was understood by the custom grammar.

## Setting up the project

The SimpleGrammar Xcode project is already configured to link with the Speech SDK, but it needs a copy of the files from the Speech SDK distribution.  Follow these steps to add the latest SpeechKit to the project.

1. Unzip the AT&T Speech SDK into its own folder.
2. Copy the files `ATTSpeechKit.a` and `ATTSpeechKit.h` into the `ATTSpeechKit` subfolder of this sample app.
3. Open the SimpleSpeech Xcode project.
4. Expand the `ATTSpeechKit` group within the project window.  Both files should appear in black text, not red (which would indicate that Xcode can't find them).

## Running the sample

Before building the sample app, you will need to add the configuration for your Speech API account: the URL of the service, your application's API key and API secret.  Set those values in `SpeechConfig.m`.   Before distributing your app to the public, make sure you add code to obfuscate those credentials.

## Understanding the speech code

Two classes in the sample app contain most of the speech logic.

* The `SimpleGrammarViewController` class manages the user interaction for speech input.  Look in the `-[prepareSpeech]` and `-[listen:]` methods for examples of setting up `ATTSpeechService` and starting a speech interaction.  Look in the `-[speechServiceSucceeded:]` and `[speechService:failedWithError:]` methods for examples of handling recognition responses.   
* The `SimpleGrammarData` class manages the data model for the custom grammar.  It loads an SRGS file from the application resources.  Instances represent the natural language understanding of the speech, using the SISR code in the grammar file to determine the useful words of the speech.  It handles `UITableView` data source methods to show the recognized words in a table.

## Reusable OAuth code

The `SpeechAuth` class provides example code for authenticating your application with the OAuth client credentials protocol.  It performs an asynchronous network request for an OAuth access token that can be used in the Speech API.  Look in `-[SimpleGrammarViewController prepareSpeech]` for examples of calling `SpeechAuth` to obtain an access token. 

## About the example grammar

The included grammar understands simple descriptions of pizzas.  The SISR (JavaScript) code in the grammar describes four slots (size, crust, kind, and ingredients) for natural language understanding.  As the Speech API recognizes speech using the grammar, it builds a JSON object containing the 4 slots and returns it in the `NluHypothesis` field of the response data.

The included grammar accepts phrases in the structure:

    [personal|small|medium|large] [(thin|thick) crust] 
    (Hawaiian|meat lovers|vegetarian|pepperoni|barbeque chicken|supreme) pizza 
    [with (#ingredients [and #ingredients])+]

Things to try saying:

* "Personal vegetarian pizza with garlic"
* "Thick crust pizza with olives, mushrooms and pepperoni"
* "Large thin crust Hawaiian pizza with extra cheese"
