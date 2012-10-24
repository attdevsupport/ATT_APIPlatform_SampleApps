/*! \mainpage MS SDK Sample Applications
 * 
 * This guide provides introduction for MS SDK Sample Application which show cases the usage of AT&T MS SDK Library.
 * 
 * The AT&T API Platform SDK for Microsoft leverages the power of the Microsoft .NET platform and AT&T services 
 * so that developers can quickly bring robust C# and Visual Basic applications to market. 
 *
 * The following sample applications are provided.
 * 
 * \li SMS App1
 * \li SMS App2
 * \li MMS App1
 * \li MMS App2
 * \li MMS App3
 * \li Payment App1
 * \li Payment App2
 * \li WapPush App1
 * \li TL App1
 * \li Speech App1
 * \li MOBO App1
 * \li MIM App1 
 * 
 * \n For Sample application, refer <a href="examples.html">samples</a> section
 * 
 * For Registartion, Installation, Configuration and Execution of sample applications, refer <a href="_application.html">Application Registration & Startup</a>
 * 
 * The complete documentation for the AT&T API Platform can be found at http://developer.att.com. 
 
 * Documentation of sample applications can be referred at <a href="modules.html">modules</a> section
 */

/*! \page Application Application Registration & Startup
 * \tableofcontents
 * 
 * \section intro_sec Introduction
 * This file describes how to set up, configure and run the C# Applications of the AT&T API Platform SDK for Microsoft sample applications. 
 * It covers all steps required to register the application on DevConnect and, based on the generated API keys and secrets, create and run one's own full-fledged sample  applications.
 * 
 * \section config_sec Registration of Applications in AT&T DevConnect 
 * To utilize AT&T platform services, applications need to be registered in AT&T DevConnect.
 * 
 * \li To register an application, go to https://devconnect-api.att.com/ and login with your valid username and password.
 * \li Next, choose "My Apps" from the bar at the top of the page and click the "Setup a New Application" button. 
 * \li Fill in the form, in particular all fields marked as "required". 
 * \li Be careful while filling in the "OAuth Redirect URL" field. It should contain the URL that the oAuth provider will redirect users to when he/she successfully  authenticates and authorizes your application.
 * 
 * NOTE: You MUST select appropriate service in the list of services under field 'Services' in order to use this sample application code. 
 * 
 * \li Having your application registered, you will get back an important pair of data: an API key and Secret key. They are necessary to get your applications working with the  AT&T API Platform SDK for Microsoft. 
 * \li See 'Configuration' below to learn how to use these keys. Initially your newly registered application is restricted to the "Sandbox" environment only. 
 * \li To move it to production, you may promote it by clicking the "Promote to  production" button. Notice that you will get a different API key and secret, so these values in your application should be adjusted accordingly.
 * \li Depending on the kind of authentication used, an application may be based on either the Autonomous Client or the Web-Server Client OAuth flow 
 * (see https://devconnect-api.att.com/docs/oauth-v1/client-credentials-grant-type or https://devconnect-api.att.com/docs/oauth-v1/authorization-code-grant-type).
 * 
 * \section install_sec Installation
 * To run the examples you need an IIS Server. 
 * 
 * \li Copy the files of bin directory of MS SDK zip into bin directory of domain in IIS server.
 * \li Copy the contents of Csharp-SampleApplication folder from SDK Sample Application zip into webroot of domain.
 * \li Copy the contents of Vb-SampleApplication folder from SDK Sample Application zip into webroot of domain.
 * 
 * \section parameters_sec Configuration
 * 
 * Each sample application contains a web.config file. It holds configurable parameters described in an easy to read format. 
 * 
 * Please populate the following parameters in  config.web as specified below:
 * 
 * \li api_key			: {set the value as per your registered application 'API key' field value} 
 * \li secret_key		: {set the value as per your registered application 'Secret key' field value} 
 * \li endPoint			: https://api.att.com 
 * 
 * Note: You must update parameters 1-2 after you promote your application from 'Sandbox' environment to 'Production' environment.
 * 
 * \section run_sec Running the application
 * 
 * Suppose you copied the sample app files in your IIS server webroot/Csharp-SampleApplication/ folder, 
 * 
 * In order to run the sample application, type  in 'http://IIS_HOSTNAME:8080/Csharp-SampleApplication/<app_name>' 
 * (assuming you're using a HOSTNAME machine with IIS Server and have not changed the default port  number, otherwise adjust accordingly) on your browser.
 */