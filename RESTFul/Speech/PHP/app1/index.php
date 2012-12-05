<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<?php

header("Content-Type: text/html; charset=ISO-8859-1");
include("config.php");
include($oauth_file);
include("tokens.php");
error_reporting(0);
session_start();

$chkChunked = $_REQUEST["chkChunked"];
$_SESSION["chkChunked"] = $chkChunked;
$speechcontext = $_REQUEST["speechcontext"];
$_SESSION["speechcontext"] = $speechcontext;
$header_array = array();


$speech_context_array = preg_split('/,/', $speech_context_config, -1, PREG_SPLIT_NO_EMPTY);
$counter = count($speech_context_array);



?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Speech Application - Speech to Text Application
    </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
<div id="container">
<!-- open HEADER -->
<div id="header">
    <div>
        <div class="hcRight">
            <?php echo date("D M j G:i:s T Y");?>
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
            AT&amp;T Sample Speech Application - Speech to Text Application</h1>
        <h2>
            Feature 1: Speech to Text
        </h2>
    </div>
</div>
<br />
<br />
<form enctype="multipart/form-data" name="SpeechToText" action="" method="post">
    <div class="navigation">
        <table border="0" width="100%">
            <tbody>
            <tr>
                <td valign="middle" class="label" align="right">
                    Speech Context:
                </td>
               <td class="cell">
                    <select name="speechcontext">
                         <option value = ""></option>
                        <?php for($i = 0; $i <= $counter-1; $i++) {?>
                        <option value="<?php echo $speech_context_array[$i];?>"><?php echo $speech_context_array[$i];?></option><br><?php  } ?>
                    </select>
                </td>
            </tr>
            <tr>
                <td width="25%" valign="top" class="label" align="right">
                    Audio File:
                </td>
                <td class="cell">
                    <input name="f1" type="file"/>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td class="cell">
                    <input type="checkbox" name="chkChunked" value="Send Chunked" />
                    Send Chunked
                </td>
            </tr>
            <tr>
                <td valign="middle" class="label" align="right">
                    X-Arg:
                </td>
                <td class="cell">
                    <input type="text" name="x-arg" style="height: 120px; width: 234px" value="<?php echo $x_arg; ?>" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td></td>
                <td>
                    <button type="submit" name="SpeechToText">
                        Submit</button>
                </td>
            </tr>
            </tbody>
        </table>
    </div>
    <div class="extra">
        <table border="0" width="100%">
            <tbody>
            <tr>
                <td />
                <td>
                    <div id="extraleft">
                        <div class="warning">
                            <strong>Note:</strong><br />
                            If no file is chosen, a <a href="./default.wav">default.wav</a> file will be loaded
                            on submit.<br />
                            <strong>Speech file format constraints:</strong>
                            <br />
                            16 bit PCM WAV, single channel, 8 kHz sampling
                            <br />
                            16 bit PCM WAV, single channel, 16 kHz sampling
                            <br />
                            AMR (narrowband), 12.2 kbit/s, 8 kHz sampling
                            <br />
                            AMR-WB (wideband) is 12.65 kbit/s, 16khz sampling
                            <br />
                            OGG - speex encoding, 8kHz sampling
                            <br />
                            OGG - speex encoding, 16kHz sampling
                        </div>
                    </div>
                </td>
                <td />
            </tr>
            </tbody>
        </table>
    </div>
</form>
</form>
<br clear="all" />
<div align="center">
    <?php

    if (isset($_POST['SpeechToText'])) {
        

        $fullToken["accessToken"]=$accessToken;
        $fullToken["refreshToken"]=$refreshToken;
        $fullToken["refreshTime"]=$refreshTime;
        $fullToken["updateTime"]=$updateTime;

        $fullToken=check_token($FQDN,$api_key,$secret_key,$scope,$fullToken,$oauth_file);
        $accessToken=$fullToken["accessToken"];

        if($speechcontext == "") {
             $speechcontext = "Generic";
        }
        if($chkChunked == true) {
            $transfer_encoding = 'Content-Transfer-Encoding: chunked';
            array_push($header_array, $transfer_encoding);
        }
        if($x_arg != null) {
            
            $x_arg_header = "X-Arg:".urlencode($x_arg);
            array_push($header_array, $x_arg_header);
        }


        $filename = $_FILES['f1']['name'];
        


        if($filename == null) {
            $filename = dirname(__FILE__).'/bostonSeltics.wav';
            $file_binary = fread(fopen($filename, 'rb'), filesize($filename));

        } else{

            $temp_file = $_FILES['f1']["tmp_name"];
            $dir = dirname($temp_file);
            $file_binary = fread(fopen($temp_file, "r"), filesize($temp_file));
        }
        $ext = end(explode('.', $filename));
        $type = 'audio/'.$ext;


        if($type == 'audio/wav' || $type == 'audio/amr' || $type =='audio/amr-wb' || $type =='audio/x-speex') {
         

            $speech_info_url = $FQDN."/rest/2/SpeechToText";
            $authorization = "Authorization: BEARER ".$accessToken;
            $accept = "Accept: application/json";
            $context = "X-Speech-Context:".$speechcontext;
            
            $content = "Content-Type:".$type;
            array_push($header_array, $authorization, $accept, $context, $content);




            $speech_info_request = curl_init();
            curl_setopt($speech_info_request, CURLOPT_URL, $speech_info_url);
            curl_setopt($speech_info_request, CURLOPT_HTTPGET, 1);
            curl_setopt($speech_info_request, CURLOPT_HEADER, 0);
            curl_setopt($speech_info_request, CURLINFO_HEADER_OUT, 1);
            curl_setopt($speech_info_request, CURLOPT_HTTPHEADER, $header_array);
            curl_setopt($speech_info_request, CURLOPT_RETURNTRANSFER, 1);
            curl_setopt($speech_info_request, CURLOPT_POSTFIELDS, $file_binary);
            curl_setopt($speech_info_request, CURLOPT_SSL_VERIFYPEER, false);
            curl_setopt($speech_info_request, CURLOPT_SSL_VERIFYHOST, false);

            $speech_info_response = curl_exec($speech_info_request);
            $responseCode=curl_getinfo($speech_info_request,CURLINFO_HTTP_CODE);

            if($responseCode==200)
            {
                $jsonObj2 = json_decode($speech_info_response);
                $wordscounter = count($jsonObj2->Recognition->NBest[0]->Words);
                $wordscorescounter = count($jsonObj2->Recognition->NBest[0]->WordScores);
                
                
                ?><div class="successWide" align="left">
                <strong>SUCCESS:</strong>
                <br />
                Response parameters listed below.
            </div>
                <table width="500" cellpadding="1" cellspacing="1" border="0">
                    <thead>
                    <tr>
                        <th width="50%" class="label">Parameter</th>
                        <th width="50%" class="label">Value</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr>
                        <td class="cell" align="center"><em>ResponseId</em></td>
                        <td class="cell" align="center"><em><?php echo $jsonObj2->Recognition->ResponseId ?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>Status</em></td>
                        <td class="cell" align="center">
                            <em><?php $jStatus = $jsonObj2->Recognition->Status; echo $jStatus; ?></em>
                        </td>
                    </tr>
                    <?php if (strcmp($jStatus, "OK") == 0) { ?>
                    <tr>
                        <td class="cell" align="center"><em>Hypothesis</em></td>
                        <td class="cell" align="center"><em> <?php echo $jsonObj2->Recognition->NBest[0]->Hypothesis ?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>LanguageId</em></td>
                        <td class="cell" align="center"><em><?php echo $jsonObj2->Recognition->NBest[0]->LanguageId?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>Confidence</em></td>
                        <td class="cell" align="center"><em><?php echo $jsonObj2->Recognition->NBest[0]->Confidence ?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>Grade</em></td>
                        <td class="cell" align="center"><em><?php echo $jsonObj2->Recognition->NBest[0]->Grade?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>ResultText</em></td>
                        <td class="cell" align="center"><em><?php echo $jsonObj2->Recognition->NBest[0]->ResultText?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>Words</em></td>
                        <td class="cell" align="center"><em><?php for ($i=0; $i<=$wordscounter; $i++) { echo $jsonObj2->Recognition->NBest[0]->Words[$i]; echo ' '; }?></em></td>
                    </tr>
                    <tr>
                        <td class="cell" align="center"><em>WordScores</em></td>
                        <td class="cell" align="center"><em><?php for ($i=0; $i<=$wordscorescounter; $i++) { echo $jsonObj2->Recognition->NBest[0]->WordScores[$i]; echo ' '; }?></em></td>
                    </tr>
                    <?php } ?>
                    </tbody>
                </table><?php
            }else{

                $msghead="Error";
                $msgdata=curl_error($speech_info_request);
                $errormsg=$msgdata.$speech_info_response;
                ?>
                <div class="errorWide">
                    <strong>ERROR:</strong><br />
                    <?php  echo $errormsg?>
                </div>
                <?php }
            curl_close ($speech_info_request);
        }else{
            ?>
            <div class="errorWide">
                <strong>ERROR:</strong><br />
                <?php echo "Invalid file specified. Valid file formats are .wav, .amr, .amr-wb and x-speex'"?>
            </div>
            <?php }}


    ?>

    <br clear="all" />
    <div id="footer">
        <div style="float: right; width: 20%; font-size: 9px; text-align: right">
            Powered by AT&amp;T Cloud Architecture</div>
        <p>
            &#169; 2012 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
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
            For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a></p>
    </div>
</div>
<p>
    &nbsp;</p>
</body>
</html>
