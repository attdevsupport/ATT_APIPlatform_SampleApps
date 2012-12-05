******************************************************************************************
* Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
* TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
* Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
* For more information contact developer.support@att.com<mailto:developer.support@att.com>
******************************************************************************************

  AT&T API Platform Samples - CMS App 1
 -------------------------------------

This application demonstrates the usage of Call Management API of AT&T platform. 
The application provides operations to support the creation of outgoing call session
and signals to existing sessions. Incoming call sessison will be created automatically
in the call environment.

This file describes how to set up, configure and run the PHP Application of the
AT&T API Platform sample applications. It covers all steps required to register the
application on DevConnect and, based on the generated API keys and secrets, 
create and run one's own full-fledged sample applications.

  1. Configuration
  2. Installation
  3. Parameters
  4. Running the application

1. Configuration

  Configuration consists of a few steps necessary to get an application registered
  on DevConnect with the proper services and endpoints, depending on the type of
  client-side application (autonomous/non-autonomous). 

  To register an application, go to https://devconnect-api.att.com/ and login with
  your valid username and password. Next, choose "My Apps" from the bar at the top
  of the page and click the "Setup a New Application" button. 

  Fill in the form, in particular all fields marked as "required".

  Be careful while filling in the "OAuth Redirect URL" field. It should contain the
  URL that the oAuth provider will redirect users to when he/she successfully
  authenticates and authorizes your application.

  NOTE: You MUST select Call Control in the list of services under field 'Services'
  in order to use this sample application code. 

  Having your application registered, you will get back an important pair of data:
  an API key and Secret key. They are necessary to get your applications working with
  the AT&T Platform APIs.

  Initially your newly registered application is restricted to the "Sandbox"
  environment only. To move it to production, you may promote it by clicking the
  "Promote to production" button. Notice that you will get a different API key and
  secret, so these values in your application should be adjusted accordingly.

  Depending on the kind of authentication used, an application may be based on either
  the Autonomous Client or the Web-Server Client OAuth flow (see
  https://devconnect-api.att.com/docs/oauth20/autonomous-client-application-oauth-flow or
  https://devconnect-api.att.com/docs/oauth20/web-server-client-application-oauth-flow
  respectively).
  

2. Installation

   Requirements:
     Apache web server 
     PHP 5.2+
     PHP CURL extension	
     Apache and PHP configured. The package manager on most Linux systems should automatically 
     configure Apache/PHP upon installation.

   Installation:
     Install Apache, PHP, and PHP CURL extension according to their respective documentation.
     Copy the sample application folder to Apache web root folder, for example /var/www/html.
 

3. Parameters
   
  Each sample application contains a config file. It holds configurable parameters
  described in an easy to read format. Please populate the following parameters in
  config.php as specified below:

  1) $api_key            : This is a mandatory parameter. Set the value as per your
			   registered application 'API key' field value.

  2) $secret_key     	 : This is a mandatory parameter. Set the value as per your
			   registered application 'Secret key' field value.

  3) $FQDN		 : https://api.att.com

  4) $oauth_file         : File containing authorization and refresh tokens 

  5) $scope              : Scope used when requested access token. Set to CMS. 

  6) $callcontrol_file   : Contains path to call control file. Default: callcontrolscript.php

  7) $number             : Application's test phone number 


  Note: If your application is promoted from Sandbox environment to Production
  environment and you decide to use production application settings, you must update
  parameters 1-2 as per production application details.

4. Running the application

  After the sample application folder has been copied to the Apache web root folder, you need to start Apache. 

  Once Apache has been started, use a web browser and open the the web page to where you copied the sample application folder.
  For example, http://localhost/app1/index.php
