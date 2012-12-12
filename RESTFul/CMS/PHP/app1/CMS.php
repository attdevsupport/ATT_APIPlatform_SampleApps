<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<?php
header("Content-Type: text/html; charset=ISO-8859-1");
include ("config.php");
include ($oauth_file);
include ("tokens.php");

function getVar($varName) {
    if (isset($_POST[$varName])) {
            $value = $_POST[$varName];
            $_SESSION[$varName] = $value;
            return $value;
    } else if (isset($_SESSION[$varName])) {
        return $_SESSION[$varName];
    }

    return "";
}

session_start();
$signal = getVar('signal');
$messageToPlay = getVar('txtMessageToPlay');
$numberToDial = getVar('txtNumberToDial');
$txtNumber = getVar('txtNumber');
$template = getVar('template');
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Application for Call Management </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
    <style type="text/css">
        .style4
        {
            font-style: normal;
            font-variant: normal;
            font-weight: bold;
            font-size: 12px;
            line-height: normal;
            font-family: Arial, Sans-serif;
            width: 21%;
        }
        .style6
        {
            font-style: normal;
            font-variant: normal;
            font-weight: bold;
            font-size: 12px;
            line-height: normal;
            font-family: Arial, Sans-serif;
            width: 25%;
        }
    </style>
</head>
<body>
<div id="container">
    <!-- open HEADER -->
    <div id="header">
        <div>
            <div class="hcRight">
                <?php echo  date("D M j G:i:s T Y"); ?>
            </div>
            <div class="hcLeft">
                Server Time:</div>
        </div>
        <div>
            <div class="hcRight">

                <script language="JavaScript" type="text/javascript">
                    var myDate = new Date();
                    document.write(myDate);
                </script>
            </div>
            <div class="hcLeft">
                Client Time:</div>
        </div>
        <div>
            <div class="hcRight">
                <script language="JavaScript" type="text/javascript">
                    document.write("" + navigator.userAgent);
                </script>
            </div>
            <div class="hcLeft">
                User Agent:</div>
        </div>
        <br clear="all" />
    </div>
    <!-- close HEADER -->
    <div>
        <div class="content">
            <h1>
                &nbsp;</h1>
            <h1>
                AT&T Sample Application for Call Management</h1>
            <h2>
                Feature 1: Outbound Session from
                <label for="phonenumber">
                    <?php echo $number; ?></label>
            </h2>
        </div>
    </div>
    <br clear="all" />
    <form name="input" method="post">
        <div class="navigation">
            <table>
                <tbody>
                <tr>
                    <td class="style4">
                        Make call to:
                    </td>
                    <td class="cell" style="width: 60%">
                        <input type="text" name="txtNumberToDial" value="<?php echo htmlspecialchars($numberToDial);?>" title="telephone number or sip address"/>
                    </td>
                </tr>
                <tr>
                    <td class="style4">
                        Script Function:
                    </td>


                    <?php
                    $filename = dirname(__FILE__) . '/First.tphp';
                    $file = file_get_contents($filename, true);


                    ?>
                    <form name="input" method="post">
                        <td class="cell" style="width: 20%">
                            <select name="template" onchange="this.form.submit()">>
                                <option value=""></option>
                                <option value="ask" <?php if($template == "ask") { $txt="selected='selected'"; echo $txt; }?>>ask</option>
                                <option value="conference"  <?php if($template == "conference") { $txt="selected='selected'"; echo $txt; }?>>conference</option>
                                <option value="reject" <?php if($template == "reject") { $txt="selected='selected'"; echo $txt; }?>>reject</option>
                                <option value="transfer" <?php if($template == "transfer") { $txt="selected='selected'"; echo $txt; }?>>transfer</option>
				<option value="message" <?php if($template == "message") { $txt="selected='selected'"; echo $txt; }?>>message</option>
                                <option value="wait" <?php if($template == "wait") { $txt="selected='selected'"; echo $txt; }?>>wait</option>
                            </select>


                        </td>
                </tr>
    </form>

    <tr>
        <td class="style4">
            Number parameter for Script Function:</td>
        <td class="cell" style="width: 60%">
            <input type="text" name="txtNumber" value="<?php echo htmlspecialchars($txtNumber);?>" title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from"/>
        </td>
    </tr>
    <tr>
        <td class="style4">
            Message To Play:</td>
        <td class="cell" style="width: 60%">
            <input type="text" name="txtMessageToPlay" value="<?php echo htmlspecialchars($messageToPlay);?>" title="enter long message or mp3 audio url, this is used as music on hold"/>
        </td>
    </tr>
    <tr>
        <td class="style4">
            Script Source Code:</td>
        <td class="cell" style="width: 60%">
            <textarea name="description" rows="2" cols="20" id="txtCreateSession" disabled="disabled" value="This script answers the incoming call with the welcome message, and requests user to enter 4 or 5 digit pin with # as terminator, 90 seconds" style="height:141px;width:655px;">
                <?php echo htmlspecialchars($file); ?></textarea>
        </td>
    </tr>
    <tr>
        <td class="style4">
        </td>
        <td align="center">
            <button type="submit" name="btnupload1" >
                Create Session</button><span id="Label1" class="warning" style="display:inline-block;width:200px;">Create Session will trigger an outbound call from application  to <strong>"Make call to"</strong> number.</span>

        </td>
    </tr>
    </tbody>
    </table>
    </form>
</div>
<div class="extra" align="left">
<div class="warning">
    <strong>Note:</strong> <br />
<?php if($template == "") { ?>
    User is asked to press digit to activiate music on hold
    <strong>\"Message to Play\"</strong> to handle the signal (feature 2)<?php }
else if ($template == "ask") {?>
    For ask script function, user is prompted
    to enter few digits and entered digits are played back. <br />
    User is asked to press digit to activiate music on hold <strong>Message
        to Play</strong> to handle the signal (feature 2)<?php }
else if($template == "conference") {?>
    For
    conference script function, user is prompted to join the
    conference.<br /> After quitting the conference, user is asked to
    press digit to activiate music on hold <strong>Message to
        Play</strong> to handle the signal (feature 2)<?php }
else if($template == "message" ) {?>
    For 
    <strong>message()</strong> script function, user is played 
    back <strong>"Number parameter for Script Function"</strong> 
    number and an SMS Message is sent to that number.<br/> 
    User is asked to press digit to activate music on hold 
    <strong>Message to Play</strong> to handle the signal (feature 2)
<?php }
else if($template == "reject") {?>
    For
    reject script function, if <strong>Number parameter for
        Script Function</strong> matches with calling id, call will be dropped.<br />
    If calling id doesnt match, calling id and <strong>Number
        parameter for Script Function</strong> number are played to User.<br />
    User is asked to press digit to activiate music on hold <strong>Message
        to Play</strong> to handle the signal (feature 2)<?php }
else if($template == "transfer") {?>
    For
    transfer script function, user is played back with <strong>Number
        parameter for Script Function</strong> and call be transferred to that
    number.<br /> While doing transfer music on hold <strong>Message
        to Play</strong> is played. Once <strong>Number parameter for
        Script Function</strong> number disconnects the call, user is asked to
    press digit to activiate music on hold <strong>Message to
        Play</strong> to handle the signal (feature 2)<?php }
else if($template == "wait") {?>
    For
    wait script function, if <strong>Number parameter for
        Script Function</strong> matches with calling id, call will be kept on
    hold for 3 seconds.<br /> If calling id doesnt match, calling id
    and <strong>Number parameter for Script Function</strong> number
    are played to User.<br /> User is asked to press digit to
    activiate music on hold <strong>Message to Play</strong> to handle
    the signal (feature 2)<?php }
?>


<?php
if (isset ($_POST["btnupload1"])) {
    if($numberToDial == null) {

        ?></div></div>
    <br style="clear: both;" />
    <br clear="all" />
    <div class="errorWide">
        <strong>ERROR:</strong><br />
        <?php echo "Please enter in a number to make a call";?>
    </div>
        <?php

    }else{


        $fullToken["accessToken"] = $accessToken;
        $fullToken["refreshToken"] = $refreshToken;
        $fullToken["refreshTime"] = $refreshTime;
        $fullToken["updateTime"] = $updateTime;

        $fullToken = check_token($FQDN, $api_key, $secret_key, $scope, $fullToken, $oauth_file);
        $accessToken = $fullToken["accessToken"];

        // Form the URL to send SMS
        $CMS_RequestBody = json_encode(array('smsCallerID' => $number, 'feature' => $template, 'numberToDial' => $numberToDial,
	'featurenumber' => $txtNumber, 'messageToPlay' => $messageToPlay));

        $CMS_Url = $FQDN . "/rest/1/Sessions";
        $authorization = 'Authorization: Bearer ' . $accessToken;
        $content = "Content-Type: application/json";

        //Invoke the URL
        $CMS = curl_init();
        curl_setopt($CMS, CURLOPT_URL, $CMS_Url);
        curl_setopt($CMS, CURLOPT_POST, 1);
        curl_setopt($CMS, CURLOPT_HEADER, 0);
        curl_setopt($CMS, CURLINFO_HEADER_OUT, 0);
        curl_setopt($CMS, CURLOPT_HTTPHEADER, array (
            $authorization,
            $content
        ));
        curl_setopt($CMS, CURLOPT_POSTFIELDS, $CMS_RequestBody);
        curl_setopt($CMS, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($CMS, CURLOPT_SSL_VERIFYPEER, false);
        $CMS_response = curl_exec($CMS);
        $responseCode = curl_getinfo($CMS, CURLINFO_HTTP_CODE);

        if ($responseCode == 200) {
            $jsonResponse = json_decode($CMS_response, true);
            $id = $jsonResponse["id"];
            $success = $jsonResponse["success"];
            $_SESSION["id"] = $id;
            $_SESSION["success"] = $success;
            ?></div>
                        </div>
        <br style="clear: both;" />
        <br clear="all" />
	<div class="successWide">
        <strong>Success:</strong><br />
        <table border="0" width="100%" align="center">
            <tbody>
            <tr>
                <td class="label" style="width: 10%">
                    id
                </td>
                <td class="cell">
                    <?php echo $_SESSION["id"];?>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 10%">
                    success
                </td>
                <td class="cell">
                     <?php if($_SESSION["success"] == true) {
                          echo "true";
                      }else if($_SESSION["success"] == false) {
                         echo "false";
                     }
                    ?>
                </td>
            </tr>
            </tbody>
        </table>
    </div>
     <?php
        }


        else
        {
            ?></div></div>
        <br style="clear: both;" />
        <br clear="all" />
        <div class="errorWide">
            <strong>ERROR:</strong><?php echo $responseCode; ?><br />
            <?php echo $CMS_response; ?>
        </div>
            <?php
        }
    }
}
?>
</div>
</div>
<br clear="all" />
<br style="clear: both;" />
<div>
    <div class="content">
        <h2>
            &nbsp;</h2>
        <h2>
            Feature 2: Send Signal to Session</h2>
    </div>
</div>
<br />
<div class="navigation">
    <table style="width: 100%">
        <tbody>
        <tr>
            <td class="style6">
                <label class="label">
                    Session ID:
                </label>
            </td>
            <td>
                <label class="cell">
                    <?php echo isset($id) ? $id : ""; ?></label>
            </td>
        </tr>
        <tr>
            <td class="style6">
                <label class="label">
                    Signal to Send:
                </label>
            </td>
            <td>
                <form name="input" method="post">
                    <select name ="signal">
                        <option value="exit"<?php if($signal == "exit") { $txt="selected='selected'"; echo $txt; }?>>exit</option>
                        <option value="stopHold"<?php if($signal == "stopHold") { $txt="selected='selected'"; echo $txt; }?>>stopHold</option>
                        <option value="dequeue"<?php if($signal == "dequeue") { $txt="selected='selected'"; echo $txt; }?>>dequeue</option>
                    </select>
            </td>
        </tr>
        <tr>
            <td></td>
            <td align="center">
                <button type="submit" name="btnupload">
                    Send Signal</button>
            </td>
        </tr>
        </tbody>
    </table>
    </form>
</div>
<div class="extra">
</div>
<br style="clear: both;" />
<br clear="all" />
<?php
if (isset ($_POST["btnupload"])) {

    $fullToken["accessToken"] = $accessToken;
    $fullToken["refreshToken"] = $refreshToken;
    $fullToken["refreshTime"] = $refreshTime;
    $fullToken["updateTime"] = $updateTime;

    $fullToken = check_token($FQDN, $api_key, $secret_key, $scope, $fullToken, $oauth_file);
    $accessToken = $fullToken["accessToken"];

    if($_SESSION["id"] != null) {

        // Form the URL to send SMS
        $CMSsignal_RequestBody = json_encode(array('signal' => $signal)); 

        $CMSsignal_Url = $FQDN . "/rest/1/Sessions/" . $_SESSION["id"] . "/Signals";
        $authorization = 'Authorization: Bearer ' . $accessToken;
        $content = "Content-Type: application/json";

        //Invoke the URL
        $CMSsignal = curl_init();

        curl_setopt($CMSsignal, CURLOPT_URL, $CMSsignal_Url);
        curl_setopt($CMSsignal, CURLOPT_POST, 1);
        curl_setopt($CMSsignal, CURLOPT_HEADER, 0);
        curl_setopt($CMSsignal, CURLINFO_HEADER_OUT, 0);
        curl_setopt($CMSsignal, CURLOPT_HTTPHEADER, array (
            $authorization,
            $content
        ));
        curl_setopt($CMSsignal, CURLOPT_POSTFIELDS, $CMSsignal_RequestBody);
        curl_setopt($CMSsignal, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($CMSsignal, CURLOPT_SSL_VERIFYPEER, false);

        $CMSsignal_response = curl_exec($CMSsignal);

        $responseCode = curl_getinfo($CMSsignal, CURLINFO_HTTP_CODE);

        if ($responseCode == 200) {
             $jsonresponse = json_decode($CMSsignal_response, true);
             $status = $jsonresponse["status"];
            ?>
      <br style="clear: both;" />
         <br clear="all" />
        <div class="successWide">
            <strong>Success:</strong><br />
            <table border="0" width="100%" align="center">
                <tbody>
                    <tr>
                        <td class="label" style="width: 10%">
                            status
                        </td>
                        <td class="cell">
                           <?php echo $status;?>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
            <?php
            $_SESSION["id"] = null;
        }
        else
        {
            ?>
        <br style="clear: both;" />
        <br clear="all" />
        <div class="errorWide">
            <strong>ERROR:</strong><?php echo $responseCode; ?><br />
            <?php echo $CMSsignal_response; ?>
        </div>
            <?php
        }
    }else{
        ?>
    <br style="clear: both;" />
    <br clear="all" />
    <div class="errorWide">
        <strong>ERROR:</strong></br>
        <?php echo "Please create a session and then send signal";?>
    </div>
        <?php
    }
}
?>
<div id="footer" align="center">
    <div style="float: right; width: 20%; font-size: 9px; text-align: right">
        Powered by AT&amp;T Cloud Architecture</div>
    <p>
        © 2012 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
                                                                       target="_blank">http://developer.att.com</a>
        <br />
        The Application hosted on this site are working examples intended to be used for
        reference in creating products to consume AT&amp;T Services and not meant to be
        used as part of your product. The data in these pages is for test purposes only
        and intended only for use as a reference in how the services perform.
        <br />
        For download of tools and documentation, please go to <a href="https://devconnect-api.att.com/"
                                                                 target="_blank">https://devconnect-api.att.com</a>
        <br />
        For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a>
    </p>
</div>
</div>
</form>
</body>
</html>
