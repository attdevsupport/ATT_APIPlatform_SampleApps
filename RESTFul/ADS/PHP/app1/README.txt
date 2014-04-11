******************************************************************************************
* Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014
* TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
* Copyright 2014 AT&T Intellectual Property. All rights reserved. http://developer.att.com
* For more information contact developer.support@att.com<mailto:developer.support@att.com>
******************************************************************************************

  AT&T API Platform Samples - Ad App 1
 -------------------------------------

This application demonstrates the usage of the Advertisement API of AT&T platform. 
The Advertisement API is a service that returns advertisements, thereby enabling the 
developer to insert advertisements into their application.

This file describes how to set up, configure, and run the PHP Application of the
AT&T API Platform. It covers all steps required to register the application on 
DevConnect and, based on the generated API keys and secrets, create and run one's 
own full-fledged sample applications.

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

NOTE: You MUST select Ads in the list of services under field 'Services' in order to
use this sample application code. 

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
    PHP 5.4+
    PHP CURL extension	
    Apache and PHP configured. The package manager on most GNU/Linux systems should automatically 
    configure Apache/PHP upon installation.

  Installation:
    Install Apache, PHP, and PHP CURL extension according to their respective documentation.
    Copy the sample application folder to Apache web root folder, for example /var/www/html.
 

3. Parameters

  Each application contains a config.php file. This file holds configurable parameters described in an 
  easy-to-read format. Please modify the config.php file using the comments specified within the file. 
  
  Note: If your application is promoted from Sandbox environment to Production environment and you decide to use
  production application settings, you must update parameters as per production application details.

4. Running the application

  After the sample application folder has been copied to the Apache web root folder, you need to start Apache. 

  Once Apache has been started, use a web browser and open the the web page to where you copied the sample application folder.
  For example, http://localhost/app1/ADS.php
  
