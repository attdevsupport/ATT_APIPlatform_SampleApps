<!--
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 
2013 TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
Property. All rights reserved. http://developer.att.com For more information
contact developer.support@att.com -->
-->

AT&T API Platform Sample Application
-------------------------------------

This file describes how to set up, configure, and run the Java Application
using AT&T's API Platform services. It covers all steps required to register
the application, and create and run one's own full-fledged sample applications
based on the generated API keys and secrets.


1. Configuration

  Configuration consists of a few steps necessary to get an application
  registered with the proper services and endpoints, depending on the type of
  client-side application (autonomous/non-autonomous). 

  To register an application, go to https://devconnect-api.att.com/ and login
  with your valid username and password.  Next, choose "My Apps" from the bar
  at the top of the page and click the "Setup a New Application" button. 

  Fill in the form--particularly all fields marked as "required."

  NOTE: You MUST select the application used in the list of services under 
  field 'Services' in order to use this sample application code. 

  Having your application registered, you will get back an important pair of
  data: an API key and Secret key. They are necessary to get your applications
  working with the AT&T APIs. See 'Adjusting parameters' below to learn how to
  use these keys.

  Initially your newly registered application is restricted to the "Sandbox"
  environment only. To move it to production, you may promote it by clicking
  the "Promote to production" button. Notice that you will get a different API
  key and secret, so these values in your application should be adjusted
  accordingly.

  Depending on the kind of authentication used, an application may be based on
  either the Autonomous Client or the Web-Server Client OAuth flow (see
  https://devconnect-api.att.com/docs/oauth20/autonomous-client-application-oauth-flow
  or
  https://devconnect-api.att.com/docs/oauth20/web-server-client-application-oauth-flow
  respectively).


2. Installation

** Requirements

  To run the examples, you will need the Java Runtime Environment (JRE), Java
  Development Kit (JDK), and Apache Maven.

** Setting up multiple sample applications simultaneously

   In case multiple applications need to be run at the same time, make sure to
   put each app in separate folders.

3. Parameters
   
  Each sample application contains an application.properties file. This file
  is located in the 'src/main/resources/' folder. This file holds configurable
  parameters described in an easy-to-read format. Please modify the
  application.properties file using the comments specified within the file. 

  Note: If your application is promoted from Sandbox environment to Production
  environment and you decide to use production application settings, you must
  update parameters as per production application details.


4. Running the application

  The project follows Apache Maven's Standard Directory Layout and can be
  built using Apache Maven. For information about Apache Maven and for more
  detailed instructions, consult Apache Maven's documentation.
  
  Using a terminal, change the current directory to the root folder of the
  sample application (the directory should contain pom.xml). Run the following
  command in order to build and run the application:
  
  mvn clean jetty:run

  This command should run the application on port 8080. Make sure no other
  application is running on port 8080. In order to change the port, consult
  Jetty's documentation. To connect to the sample application, open a web
  browser and visit 'http://localhost:8080/<appname>' replacing <appname> with
  the application's name.
