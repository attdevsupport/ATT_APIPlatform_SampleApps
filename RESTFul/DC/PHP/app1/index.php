<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->

<?php
header("Content-Type: text/html; charset=ISO-8859-1");
header('P3P: CP="IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT"');
require_once("config.php");

session_start();
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title>AT&amp;T Sample DC Application - Get Device Capabilities
    Application</title>
<meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
<link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
    <div id="container">
<?php
require('header.php')
?>
        <div>
            <div class="content">
                <h1>AT&amp;T Sample DC Application - Get Device Capabilities
                Application</h1>
                <h2>Feature 1: Get Device Capabilities</h2>
            </div>
        </div>
        <br /> <br />
        <div class="extra">
            <table>
                <tbody>
                    <div id="extraleft">
                        <div class="warning">
                            <strong>Note:</strong><br /> <strong>OnNet Flow:</strong> Request
                                Device Capabilities details from the AT&T network for the mobile
                                device of an AT&T subscriber who is using an AT&T direct Mobile
                                data connection to access this application. <br /> <strong>OffNet
                                Flow:</strong> Where the end-user is not on an AT&T Mobile data
                                connection or using a Wi-Fi or tethering connection while
                                accessing this application. This will result in an HTTP 400
                                error.
                        </div>
                    </div>
                </tbody>
            </table>
                </div>
                <br clear="all" />
                <?php
                $error = isset($_SESSION['error']) ? $_SESSION['error'] : null;

                if($error != null) {
                ?>
        
            <div class="errorWide">
                <strong>ERROR:</strong><br />
                <?php 
                echo $_SESSION['error_description']; 
            
                unset($_SESSION['error']);
                unset($_SESSION['error_description']);
                ?>
            </div>
                <?php
                } else {
                    //Device capabilities  session access token
                    $sToken = isset($_SESSION[SESSION_TOKEN_INDEX]) ? $_SESSION[SESSION_TOKEN_INDEX] : null;

                    if($sToken == null || $sToken  == '') {
                        header('Location: ' . AUTH_CODE_URL);
                    } else {
                        $authorization = 'Authorization: Bearer ' . $sToken; 
                        $accept = 'Accept: application/json';

                        $device_capabilities = curl_init();

                        curl_setopt($device_capabilities, CURLOPT_URL, GETDCURL);
                        curl_setopt($device_capabilities, CURLOPT_RETURNTRANSFER,true);
                        curl_setopt($device_capabilities, CURLOPT_HTTPGET, true);
                        curl_setopt($device_capabilities, CURLINFO_HEADER_OUT, 1);
                        curl_setopt($device_capabilities, CURLOPT_SSL_VERIFYPEER,false);
                        curl_setopt($device_capabilities, CURLOPT_SSL_VERIFYHOST,false);
                        curl_setopt($device_capabilities, CURLOPT_HTTPHEADER, array($authorization, $accept));

                        $device_capabilities_response = curl_exec($device_capabilities);

                        $responseCode=curl_getinfo($device_capabilities,CURLINFO_HTTP_CODE);

                    if($responseCode >= 200 && $responseCode <= 300) {
                        $jsonObj = json_decode($device_capabilities_response);//decode the response and display it.
                ?>
                <br clear="all" />
                <div class="successWide" runat="server" id="tb_dc_output" visible="false">
                    <strong>SUCCESS:</strong><br /> Device parameters listed below.
                </div>
                <br />
                <div align="center">
                    <table width="500" cellpadding="1" cellspacing="1" border="0"
                        runat="server" id="tbDeviceCapabilities" visible="false">
                        <thead>
                            <tr>
                                <th width="50%" class="label">Parameter</th>
                                <th width="50%" class="label">Value</th>
                            </tr>
                        </thead>
                        <tbody>
                            <?php
                            $names = array('TypeAllocationCode', 'Name', 'Vendor', 
                                'Model', 'FirmwareVersion', 'UaProf', 'MmsCapable', 
                                'AssistedGps', 'LocationTechnology', 'DeviceBrowser', 
                                'WapPushCapable');

                            $ids = array('lblTypeAllocationCode', 'lblName', 'lblVendor',
                                'lblModel', 'lblRelease', 'lblUAProf', 'lblMMSCapable', 'lblAGPS',
                                'lblLocation', 'lblBrowserName', 'lblWAPPush');

                            // Device Information
                            $dInfo = $jsonObj->DeviceInfo;

                            // Capabilities
                            $caps = $dInfo->Capabilities;


                            $values = array($dInfo->DeviceId->TypeAllocationCode, $caps->Name, 
                                $caps->Vendor, $caps->Model, $caps->FirmwareVersion, $caps->UaProf, 
                                $caps->MmsCapable, $caps->AssistedGps, $caps->LocationTechnology, 
                                $caps->DeviceBrowser, $caps->WapPushCapable);
                    
                            for ($i = 0; $i < count($names); ++$i) {

                            ?>

                            <tr>
                                <td class="cell" align="center"<em><?php echo $names[$i] ?></em>
                                </td>
                                <td class="cell" align="center">
                                    <em>
                                        <label id="<?php echo $ids[$i] ?>">
                                        <?php echo $values[$i] ?>

                                        </label>
                                    <em>
                                <td>
                            <tr>
			    <?php 
			    } //end of for loop
			    ?>

                        </tbody>
                    </table>
                </div>
                <?php   
                    } else {
                        $msghead = "Error";
                        $msgdata = curl_error($device_capabilities);
                        $errormsg = $msgdata.$device_capabilities_response; ?>

                <div class="errorWide">
                    <strong>ERROR2:</strong><br />
                    <?php  echo $errormsg ;  ?>
                </div>
                <?php }
                    curl_close ($device_capabilities);
                  }
              } ?>
              
              <br clear="all" />
        
        
<?php 
require('footer.php') 
?>

    </div>
    <p>&nbsp;</p>
</body>
</html>
