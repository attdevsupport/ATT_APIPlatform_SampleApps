

AT&T Ad SDK Sample Application

Licensed by AT&T under 'Software Development Kit Tools Agreement' 2012.
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved.
For more information contact developer.support@att.com http://developer.att.com


The Sample Application explains the basic usage of AT&T Ad SDK provided by http://developer.att.com
The Sample Application contains a user interface with the Ad View component included
and with other mandatory/optional parameters.  The app contains three tabs, Ad View,
Settings and JSONResponse.  

The first tab contains Ad View component and a selectable
input component for mandatory 'Category' parameter.  Button 'Get Ad'
can be tapped to refresh the ad considering the parameters selected
in the settings tab.

The second tab contains different input fields for various optional parameters. 
These include:
Keywords - to provide search criteria
Zip - zip code of the user, to provide best content for the area
Premium - To choose from premium content, non-premium content and both
Age Group - To request ad targed for the selected age group
Ad size - Max height and Max width that the component can allow the ad image
Location - The latitude and longitude of the user.  The sample app can detect
the current location and send them as parameters.
 
The SDK accepts few other parameters than displayed in settings tab.  
For the full list of parameters, please refer to the SDK 
documentation.  

The third tab displays the raw JSON response that is received from Ad 
Service call.  This response is captured from one of the call back method that the app 
receives and can be used to understand the response structure and for debugging purposes.

The sample app is distributed along with the code base and explains how to integrate
AT&T Ad SDK with any of the developer application.  Also, the sample app can be used
to understand the working of SDK basing on various inputs for its optional parameters
like age group, zip code, city etc.


