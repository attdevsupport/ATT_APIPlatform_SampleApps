<!--Licensed by AT&T under 'Software Development Kit Tools Agreement.'2012
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->

<?php
    $api_key = "";
    $secret_key = "";
    $FQDN = "https://api.att.com";
    $endpoint = $FQDN."/rest/2/SpeechToText";
    $scope = "SPEECH";
    $oauth_file = "speechoauthtoken.php";
    $refreshTokenExpiresIn = "";
    $default_file = "default.wav";
    $speech_context_config = "Questions and Answers,Generic,TV,BusinessSearch,Websearch,SMS,Voicemail";
    $x_arg = "ClientApp=NoteTaker, ClientVersion=1.0.1,DeviceType=iPhone4";  

?>

