******************************************************************************************
* Licensed by AT&T under 'AT&T SDK Tools Agreement' 2013
* Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
* For more information contact developer.support@att.com<mailto:developer.support@att.com>
******************************************************************************************

  AT&T API Samples - Payment app 2
 ---------------------------------
This application allows the user to 

  1. Make a new subscription to product 1 or product 2
  2. View the details of the notary signPayload request made in the background
  3. Get status of the subscription
  4. Get details of subscriptions
  4. Refund any of the latest five subscriptions
  5. View the latest five notifications.

To make a new subscription request, the app uses the Notary app in the background
to sign the payload first.

This file describes how to set up, configure and run AT&T MSSDK VB.NET sample 
applications. It covers all steps required to register the application on 
https://developer.att.com/ and, based on the generated API keys and secrets, create and run 
one's own full-fledged sample applications.

  1. Configuration
  2. Installation
  3. Parameters
  4. Running the application


1. Configuration

  Configuration consists of a few steps necessary to get an application registered
  on https://developer.att.com/ with the proper services and endpoints, depending on the type of
  client-side application (autonomous/non-autonomous). 

  To register an application, go to https://developer.att.com/ and login with
  your valid username and password. Next, choose "My Apps" from the bar at the top
  of the page and click the "Setup a New Application" button. 

  Fill in the form, in particular all fields marked as "required".

  Be careful while filling in the "OAuth Redirect URL" field. It should contain the
  URL that the oAuth provider will redirect users to when he/she successfully
  authenticates and authorizes your application.

NOTE: You MUST select Payment in the list of services under field 'Services' in order
to use this sample application code. 

  Having your application registered, you will get back an important pair of data:
  an API key and Secret key. They are necessary to get your applications working
  with the AT&T Platform APIs. 

  Initially your newly registered application is restricted to the "Sandbox"
  environment only. To move it to production, you may promote it by clicking the
  "Promote to production" button. Notice that you will get a different API key and
  secret, so these values in your application should be adjusted accordingly.

  Depending on the kind of authentication used, an application may be based on either
  the Autonomous Client or the Web-Server Client OAuth flow (see OAuth section in 
  https://developer.att.com/docs ).



2. Installation

** Requirements

   To run the examples you need an IIS Server. 

   Download the application files from the download link published in AT&T portal
   into webdomain of your IIS server.



3. Parameters
   
Each sample application contains a web.config file. It holds configurable parameters
described in an easy to read format. Please populate the following parameters in
web.config as specified below:

1) api_key				: This is mandatory parameter, Set the value as
					  per your registered application 'API key'
					  field value.

2) secret_key     	  	        : This is mandatory parameter,Set the value as
					  per your registered application 'Secret key'
					  field value.

3) endPoint		  	        : This is mandatory parameter, Set it to the
					  end point URI of AT&T Service.

4) MerchantPaymentRedirectUrl		: Set to the URL pointing to the application.
					  ATT platform uses this URL to return the
					  control back to the application after
					  subscription processing is completed.
					  Example : https://IIS_HOSTNAME:8080/payment/app2/Default.aspx.

5) notificationFilePath			: ~\\notificationDetailsFile.txt - (This is
					  optional parameter, which points to the file
					  path, where the notification details will be
					  saved by listener. If the parameter is not
					  configured, it will take the default value as
					  ~\\notificationDetailsFile.txt. Give read/write
					  access to this file.)

6) subscriptionRefundFilePath           : ~\\subsRefundFile.txt - (This is optional
					  parameter, which points to the file path, where
					  latest subscription IDs will be stored. If the
					  parameter is not configured, it will take the
					  default value as ~\\subsRefundFile.txt. Give
					  read/write access to this file.)

7) subscriptionDetailsFilePath		: ~\\subsDetailsFile.txt - (This is optional
					  parameter, which points to the file path,
					  where subscription details will be stored by
					  the application. If the parameter is not
					  configured, it will take the default value as
					  ~\\subsDetailsFile.txt. Give read/write access
					  to this file.)

8) subsDetailsCountToDisplay		: 5 (This is optional parameter, which defines
					  the number of subscription details to be
					  displayed by the application for subscription
					  feature, default is 5}

9) notificationCountToDisplay		: 5 (This is optional parameter, which defines
					  the number of notification details to be
					  displayed by the application for notification
					  feature, default is 5}

Note: You must update parameters 1-2 after you promote your application from 'Sandbox'
environment to 'Production' environment.



4. Running the application

Suppose you copied the sample app files in your IIS server webroot/payment/app2/ folder.
In order to run the sample application, type in'http://IIS_HOSTNAME:8080/payment/app2/Default.aspx'
(assuming you're using a HOSTNAME machine with IIS Server and have not changed the 
default port number, otherwise adjust accordingly) on your browser.
