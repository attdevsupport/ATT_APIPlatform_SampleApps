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

session_start();



 $gender = $_POST['gender'];
    $_SESSION["gender"] = $gender;
    $ageGroup = $_POST['ageGroup'];
    $_SESSION["ageGroup"] = $ageGroup;
    $over18 = $_POST['over18'];
    $_SESSION["over18"] = $over18;
    $Premium = $_POST['Premium'];
    $_SESSION["Premium"] = $Premium;
    $category = $_POST['category'];
    $_SESSION["category"] = $category;


function RefreshToken($FQDN, $api_key, $secret_key, $scope, $fullToken)
{

    $refreshToken = $fullToken["refreshToken"];
    $accessTok_Url = $FQDN . "/oauth/token";

    //http header values
    $accessTok_headers = array(
        'Content-Type: application/x-www-form-urlencoded'
    );

    //Invoke the URL
    $post_data = "client_id=" . $api_key . "&client_secret=" . $secret_key . "&refresh_token=" . $refreshToken . "&grant_type=refresh_token";

    $accessTok = curl_init();
    curl_setopt($accessTok, CURLOPT_URL, $accessTok_Url);
    curl_setopt($accessTok, CURLOPT_HTTPGET, 1);
    curl_setopt($accessTok, CURLOPT_HEADER, 0);
    curl_setopt($accessTok, CURLINFO_HEADER_OUT, 0);
    curl_setopt($accessTok, CURLOPT_HTTPHEADER, $accessTok_headers);
    curl_setopt($accessTok, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($accessTok, CURLOPT_SSL_VERIFYPEER, false);
    curl_setopt($accessTok, CURLOPT_POST, 1);
    curl_setopt($accessTok, CURLOPT_POSTFIELDS, $post_data);
    $accessTok_response = curl_exec($accessTok);
    $currentTime = time();

    $responseCode = curl_getinfo($accessTok, CURLINFO_HTTP_CODE);
    if ($responseCode == 200) {
        $jsonObj = json_decode($accessTok_response);
        $accessToken = $jsonObj->{'access_token'}; //fetch the access token from the response.
        $refreshToken = $jsonObj->{'refresh_token'};
        $expiresIn = $jsonObj->{'expires_in'};

        if ($expiresIn == 0) {
            $expiresIn = 24 * 60 * 60;

        }

        $refreshTime = $currentTime + (int)($expiresIn); // Time for token refresh
        $updateTime = $currentTime + (24 * 60 * 60); // Time to get for a new token update, current time + 24h

        $fullToken["accessToken"] = $accessToken;
        $fullToken["refreshToken"] = $refreshToken;
        $fullToken["refreshTime"] = $refreshTime;
        $fullToken["updateTime"] = $updateTime;

    } else {
        $fullToken["accessToken"] = null;
        $fullToken["errorMessage"] = curl_error($accessTok) . $accessTok_response;


    }
    curl_close($accessTok);
    return $fullToken;

}

function GetAccessToken($FQDN, $api_key, $secret_key, $scope)
{

    $accessTok_Url = $FQDN . "/oauth/token";

    //http header values
    $accessTok_headers = array(
        'Content-Type: application/x-www-form-urlencoded'
    );

    //Invoke the URL
    $post_data = "client_id=" . $api_key . "&client_secret=" . $secret_key . "&scope=" . $scope . "&grant_type=client_credentials";

    $accessTok = curl_init();
    curl_setopt($accessTok, CURLOPT_URL, $accessTok_Url);
    curl_setopt($accessTok, CURLOPT_HTTPGET, 1);
    curl_setopt($accessTok, CURLOPT_HEADER, 0);
    curl_setopt($accessTok, CURLINFO_HEADER_OUT, 0);
    curl_setopt($accessTok, CURLOPT_HTTPHEADER, $accessTok_headers);
    curl_setopt($accessTok, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($accessTok, CURLOPT_SSL_VERIFYPEER, false);
    curl_setopt($accessTok, CURLOPT_POST, 1);
    curl_setopt($accessTok, CURLOPT_POSTFIELDS, $post_data);
    $accessTok_response = curl_exec($accessTok);


    $responseCode = curl_getinfo($accessTok, CURLINFO_HTTP_CODE);
    $currentTime = time();
    /*
     If URL invocation is successful fetch the access token and store it in session,
     else display the error.
    */
    if ($responseCode == 200) {
        $jsonObj = json_decode($accessTok_response);
        $accessToken = $jsonObj->{'access_token'}; //fetch the access token from the response.
        $refreshToken = $jsonObj->{'refresh_token'};
        $expiresIn = $jsonObj->{'expires_in'};

        if ($expiresIn == 0) {
            $expiresIn = 24 * 60 * 60 * 365 * 100;

        }
        $refreshTime = $currentTime + (int)($expiresIn); // Time for token refresh
        $updateTime = $currentTime + (24 * 60 * 60); // Time to get a new token update, current time + 24h

        $fullToken["accessToken"] = $accessToken;
        $fullToken["refreshToken"] = $refreshToken;
        $fullToken["refreshTime"] = $refreshTime;
        $fullToken["updateTime"] = $updateTime;

    } else {

        $fullToken["accessToken"] = null;
        $fullToken["errorMessage"] = curl_error($accessTok) . $accessTok_response;

    }
    curl_close($accessTok);
    return $fullToken;
}

function SaveToken($fullToken, $oauth_file)
{

    $accessToken = $fullToken["accessToken"];
    $refreshToken = $fullToken["refreshToken"];
    $refreshTime = $fullToken["refreshTime"];
    $updateTime = $fullToken["updateTime"];


    $tokenfile = $oauth_file;
    $fh = fopen($tokenfile, 'w');
    $tokenfile = "<?php \$accessToken=\"" . $accessToken . "\"; \$refreshToken=\"" . $refreshToken . "\"; \$refreshTime=" . $refreshTime . "; \$updateTime=" . $updateTime . "; ?>";
    fwrite($fh, $tokenfile);
    fclose($fh);
}

function check_token($FQDN, $api_key, $secret_key, $scope, $fullToken, $oauth_file)
{

    $currentTime = time();

    if (($fullToken["updateTime"] == null) || ($fullToken["updateTime"] <= $currentTime)) {
        $fullToken = GetAccessToken($FQDN, $api_key, $secret_key, $scope);
        if ($fullToken["accessToken"] == null) {
            //      echo $fullToken["errorMessage"];
        } else {
            //      echo $fullToken["accessToken"];
            SaveToken($fullToken, $oauth_file);
        }
    } elseif ($fullToken["refreshTime"] <= $currentTime) {
        $fullToken = RefreshToken($FQDN, $api_key, $secret_key, $scope, $fullToken);
        if ($fullToken["accessToken"] == null) {
            //      echo $fullToken["errorMessage"];
        } else {
            //      echo $fullToken["accessToken"];
            SaveToken($fullToken, $oauth_file);
        }
    }

    return $fullToken;

}

?>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Advertisement Application - Get Advertisement Application
    </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
<form name="input" method="post">
   <div id="container">
<!-- open HEADER -->
<div id="header">
    <div>
        <div class="hcRight">
            <?php echo  date("D M j G:i:s T Y"); ?>
        </div>
        <div class="hcLeft">
            Server Time:
        </div>
    </div>
    <div>
        <div class="hcRight">
            <script language="JavaScript" type="text/javascript">
                var myDate = new Date();
                document.write(myDate);
            </script>
        </div>
        <div class="hcLeft">
            Client Time:
        </div>
    </div>
    <div>
        <div class="hcRight">
            <script language="JavaScript" type="text/javascript">
                document.write("" + navigator.userAgent);
            </script>
        </div>
        <div class="hcLeft">
            User Agent:
        </div>
    </div>
    <br clear="all"/>
</div>
        <!-- close HEADER -->
        <div>
            <div class="content">
                <h1>
                    AT&amp;T Sample Advertisement Application - Get Advertisement Application</h1>
                <h2>
                    Feature 1: Get Advertisement</h2>
            </div>
        </div>
        <br />
        <br />
        <div class="extra">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td width="10%" class="label" title="Zip/Postal code of a user. For US only. (Integer)">
                            Zip Code:
                        </td>
                        <td class="cell">
                            <input type="text" id="zipCode" name="zipCode" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="User location latitude value (in degrees).">
                            Country:
                        </td>
                        <td class="cell">
                            <input type="text" name="country" id="country" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label">
                            Over 18 Ad Content:
                        </td>
                                  <td class="cell">
								<select name="over18" id="over18">
                                <option value=""></option>
                                <option value="0"<?php if($over18 == "0") { $txt="selected='selected'"; echo $txt; }?>>Deny Over 18 Content</option>
                                <option value="2"<?php if($over18 == "2") { $txt="selected='selected'"; echo $txt; }?>>Only Over 18 Content</option>
                                <option value="3"<?php if($over18 == "3") { $txt="selected='selected'"; echo $txt; }?>>Allow Both</option>
                            </select>
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <table>
                <tbody>
                    <div id="extraleft">
                        <div class="warning">
                            <strong>Note:</strong><br />
                            All Parameters are optional except Category.<br />
                            If this application is accessed from desktop browser, 
                            you may see successful response without Ads (HTTP 204).
                        </div>
                    </div>
                </tbody>
            </table>
        </div>
        <div class="navigation">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td width="10%" class="label">
                            *Category:
                        </td>
                        <td class="cell">
                <select name="category" id="category">
                <option value="auto" <?php if($category == "auto") { $txt="selected='selected'"; echo $txt; }?>>auto</option>
                                <option value="business" <?php if($category == "business") { $txt="selected='selected'"; echo $txt; }?>>business</option>
                                <option value="chat" <?php if($category == "chat") { $txt="selected='selected'"; echo $txt; }?>>chat</option>
                                <option value="communication" <?php if($category == "communication") { $txt="selected='selected'"; echo $txt; }?>>communication</option>
                                <option value="community" <?php if($category == "community") { $txt="selected='selected'"; echo $txt; }?>>community</option>
                                <option value="entertainment" <?php if($category == "entertainment") { $txt="selected='selected'"; echo $txt; }?>>entertainment</option>
                                <option value="finance" <?php if($category == "finance") { $txt="selected='selected'"; echo $txt; }?>>finance</option>
                                <option value="games" <?php if($category == "games") { $txt="selected='selected'"; echo $txt; }?>>games</option>
                                <option value="health" <?php if($category == "health") { $txt="selected='selected'"; echo $txt; }?>>health</option>
                                <option value="local" <?php if($category == "local") { $txt="selected='selected'"; echo $txt; }?>>local</option>
                                <option value="maps" <?php if($category == "maps") { $txt="selected='selected'"; echo $txt; }?>>maps</option>
                                <option value="medical" <?php if($category == "medical") { $txt="selected='selected'"; echo $txt; }?>>medical</option>
                                <option value="movies" <?php if($category == "movies") { $txt="selected='selected'"; echo $txt; }?>>movies</option>
                                <option value="music" <?php if($category == "music") { $txt="selected='selected'"; echo $txt; }?>>music</option>
                                <option value="news" <?php if($category == "news") { $txt="selected='selected'"; echo $txt; }?>>news</option>
                                <option value="other" <?php if($category == "other") { $txt="selected='selected'"; echo $txt; }?>>other</option>
                                <option value="personals" <?php if($category == "personals") { $txt="selected='selected'"; echo $txt; }?>>personals</option>
                                <option value="photos" <?php if($category == "photos") { $txt="selected='selected'"; echo $txt; }?>>photos</option>
                                <option value="shopping" <?php if($category == "shopping") { $txt="selected='selected'"; echo $txt; }?>>shopping</option>
                                <option value="social" <?php if($category == "social") { $txt="selected='selected'"; echo $txt; }?>>social</option>
                                <option value="sports" <?php if($category == "sports") { $txt="selected='selected'"; echo $txt; }?>>sports</option>
                                <option value="technology" <?php if($category == "technology") { $txt="selected='selected'"; echo $txt; }?>>technology</option>
                                <option value="tools" <?php if($category == "tools") { $txt="selected='selected'"; echo $txt; }?>>tools</option>
                                <option value="travel" <?php if($category == "travel") { $txt="selected='selected'"; echo $txt; }?>>travel</option>
                                <option value="tv" <?php if($category == "tv") { $txt="selected='selected'"; echo $txt; }?>>tv</option>
                                <option value="video" <?php if($category == "video") { $txt="selected='selected'"; echo $txt; }?>>video</option>
                                <option value="weather" <?php if($category == "weather") { $txt="selected='selected'"; echo $txt; }?>>weather</option>
                            </select>

                        </td>
                        <td width="10%" class="label" title="MMA Size in pixels">
                            MMA Size:
                        </td>
                        <td class="cell">
                            <select name="MMA" id="MMA">
                                <option value=""></option>
                                <option value="120 x 20"<?php if($MMA == "120 x 20") { $txt="selected='selected'"; echo $txt; }?>>120 x 20</option>
                                <option value="168 x 28"<?php if($MMA == "168 x 28") { $txt="selected='selected'"; echo $txt; }?>>168 x 28</option>
                                <option value="216 x 36"<?php if($MMA == "216 x 36") { $txt="selected='selected'"; echo $txt; }?>>216 x36</option>
                                <option value="300 x 50"<?php if($MMA == "300 x 50") { $txt="selected='selected'"; echo $txt; }?>>300 x 50</option>
                                <option value="300 x 250"<?php if($MMA == "300 x 250") { $txt="selected='selected'"; echo $txt; }?>>300 x 250</option>
                                <option value="320 x 50"<?php if($MMA == "320 x 50") { $txt="selected='selected'"; echo $txt; }?>>320 x 50</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="The City of the user. For US only.">
                            City:
                        </td>
                        <td class="cell">
                            <input type="text" name="city" id="city" />
                        </td>
                        <td width="10%" class="label" title="Area code of a user. For US only. (Integer)">
                            Area Code:
                        </td>
                        <td class="cell">
                            <input type="text" name="areaCode" id="areaCode" />
                        </td>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="Country of user. An ISO 3166 code to be used for specifying a country code.">
                            Latitude:
                        </td>
                        <td class="cell">
                            <input type="text" name="Latitude" id="Latitiude" />
                        </td>
                        <td width="10%" class="label" title="User location longitude value (in degrees).">
                            Longitude:
                        </td>
                        <td class="cell">
                            <input type="text" name="Longitude" id="Longitude" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label">
                            Age Group:
                        </td>
                        <td class="cell">
                                 <select name="ageGroup" id="ageGroup">
                    <option value=""></option>
                    <option value="1-13"<?php if($ageGroup == "1-13") { $txt="selected='selected'"; echo $txt; }?>>1-13</option>
                    <option value="14-25"<?php if($ageGroup == "14-25") { $txt="selected='selected'"; echo $txt; }?>>14-25</option>
                    <option value="26-35"<?php if($ageGroup == "26-35") { $txt="selected='selected'"; echo $txt; }?>>26-35</option>
                    <option value="36-55"<?php if($ageGroup == "36-55") { $txt="selected='selected'"; echo $txt; }?>>36-55</option>
                    <option value="55-100"<?php if($ageGroup == "55-100") { $txt="selected='selected'"; echo $txt; }?>>55-100</option>
                </select>
                        </td>
                        <td width="10%" class="label">
                            Premium:
                        </td>
                        <td class="cell">
                <select name="Premium" id="Premium">
                    <option value=""></option>
                    <option value="0" <?php if($Premium == "0") { $txt="selected='selected'"; echo $txt; }?>>Non Premium</option>
                    <option value="1" <?php if($Premium == "1") { $txt="selected='selected'"; echo $txt; }?>>Premium Only</option>
                    <option value="2" <?php if($Premium == "2") { $txt="selected='selected'"; echo $txt; }?>>Both</option>
                </select>
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label">
                            Gender:
                        </td>
                        <td class="cell">
                <select name="gender" id="gender">
                    <option value=""></option>
                    <option value="M"<?php if($gender == "M") { $txt="selected='selected'"; echo $txt; }?>>Male</option>
                    <option value="F"<?php if($gender == "F") { $txt="selected='selected'"; echo $txt; }?>>Female</option>
                </select>
                        </td>
                        <td width="10%" class="label" title="Filter ads by keywords (delimited by commas Ex: music,singer)">
                            Keywords:
                        </td>
                        <td class="cell">
                            <input type="text" name="keywords" id="keywords" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                </tbody>
            </table>
            <br />
            <table border="0" width="100%">
                <tbody>
                    <tr valign="middle" align="right">
                        <td class="cell" width="35%">
                            <button type="submit" name="btnGetAds">
                                Get Advertisement</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
		</form>
	<?php
if (isset($_POST['btnGetAds'])) {



    $MMA = $_POST['MMA'];
    $_SESSION["MMA"] = $MMA;
    $keywords = $_POST['keywords'];
    $_SESSION["keywords"] = $keywords;
    $areaCode = $_POST['areaCode'];
    $_SESSION["areaCode"] = $areaCode;
    $zipCode = $_POST['zipCode'];
    $_SESSION["zipCode"] = $zipCode;
    $city = $_POST['city'];
    $_SESSION["city"] = $city;
    $country = $_POST['country'];
    $_SESSION["country"] = $country;
    $Longitude = $_POST['Longitude'];
    $_SESSION["Longitude"] = $Longitude;
    $Latitude = $_POST['Latitude'];
    $_SESSION["Latitude"] = $Latitude;





    $fullToken["accessToken"] = $accessToken;
    $fullToken["refreshToken"] = $refreshToken;
    $fullToken["refreshTime"] = $refreshTime;
    $fullToken["updateTime"] = $updateTime;

    $fullToken = check_token($FQDN, $api_key, $secret_key, $scope, $fullToken, $oauth_file);
    $accessToken = $fullToken["accessToken"];
    $invalid_fields = array();



    $ads_Url = "$FQDN/rest/1/ads";
    $ads_Url .= "?Category=" . $category;
    if ($gender != null) {
        $ads_Url .= "&Gender=" . $gender;
    }
    if ($ageGroup != null) {
        $ads_Url .= "&AgeGroup=" . $ageGroup;
    }
    if ($over18 != null) {
        $ads_Url .= "&Over18=" . $over18;
    }
    if ($MMA != null) {
        if($MMA == '120 x 20') {
            $ads_Url .= "&MinWidth=20&MinHeight=20&MaxHeight=120&MaxWidth=120";
        }else if($MMA == '168 x 28') {
            $ads_Url .= "&MinWidth=28&MinHeight=28&MaxHeight=168&MaxWidth=168";
		}else if($MMA == '216 x 36') {
            $ads_Url .= "&MinWidth=36&MinHeight=36&MaxHeight=216&MaxWidth=216";
		}else if($MMA == '300 x 50') {
            $ads_Url .= "&MinWidth=50&MinHeight=50&MaxHeight=300&MaxWidth=300";
		}else if($MMA == '300 x 250') {
            $ads_Url .= "&MinWidth=250&MinHeight=250&MaxHeight=300&MaxWidth=300";
		}else if($MMA == '320 x 50') {
            $ads_Url .= "&MinWidth=50&MinHeight=50&MaxHeight=320&MaxWidth=320";
		}		
    }
    if ($adType != null) {
        $ads_Url .= "&Type=".$adType;
    }
    if ($premium != null) {
        $ads_Url .= "&Premium=" . $premium;
    }
    if ($keywords != null) {
        $ads_Url .= "&Keywords=" . $keywords;
    }
    if ($areaCode != null) {
        if(is_numeric($areaCode)) {
            $ads_Url .= "&AreaCode=" . $areaCode;
        }else{

            array_push($invalid_fields, "Area Code");
        }
    }
    if ($zipCode != null) {
        if(is_numeric($zipCode)) {
            $ads_Url .= "&ZipCode=" . $zipCode;
        }else{

            array_push($invalid_fields, "Zip Code");
        }
    }
    if ($country != null) {
        $ads_Url .= "&Country=" . $country;
    }
     if ($Longitude != null) {
        if(preg_match("/^[+-]?(\d*\.\d+([eE]?[+-]?\d+)?|\d+[eE][+-]?\d+)$/", $Longitude)) { 
            $ads_Url .= "&Longitude=" . $Longitude;
        } else{

            array_push($invalid_fields, "Longitude");

        }
    }
    if ($Latitude != null) {
        if(preg_match("/^[+-]?(\d*\.\d+([eE]?[+-]?\d+)?|\d+[eE][+-]?\d+)$/", $Latitude)) {
            $ads_Url .= "&Latitude=" . $Latitude;
        } else{

            array_push($invalid_fields, "Latitude");

        }


    }

    if(!empty($invalid_fields)){

        $count = count($invalid_fields);?>



    <br style="clear: both;" />
        <div align="center">
            <div id="statusPanel" style="font-family:Calibri;font-size:XX-Small;">
                <table class="errorWide" style="font-family:Sans-serif;font-size:9pt;">
                    <tr>
                        <td style="font-weight:bold;">ERROR:</td>
                    </tr><tr>
                    <td><?php
                        echo "Please correct the following error(s):";?> </br></br>
                        <ul><?php
                            for($i = 0; $i <= $count - 1; $i++) {?><li><?php
                                echo "Invalid ".$invalid_fields[$i]; ?></li><?php }?>
            </div>
        </ul></td>
		</tr>
	</table>
</div>
        </div>

<?php
    }else{
        $authorization = 'Authorization: Bearer ' . $accessToken;
        $content = "Content-Type: application/json";
        $accept = "Accept: application/json";
        $user_agent = "User-Agent:". $_SERVER['HTTP_USER_AGENT'];


        //Invoke the URL
        $ads = curl_init();



        curl_setopt($ads, CURLOPT_URL, $ads_Url);
        curl_setopt($ads, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ads, CURLOPT_HTTPGET, true);
        curl_setopt($ads, CURLINFO_HEADER_OUT, 1);
        curl_setopt($ads, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($ads, CURLOPT_SSL_VERIFYHOST, false);
        curl_setopt($ads, CURLOPT_HTTPHEADER, array($authorization, $content, $accept, $udid, $user_agent));

        $ads_response = curl_exec($ads);


        $responseCode = curl_getinfo($ads, CURLINFO_HTTP_CODE);
       
        $info = curl_getinfo($ads);
        /*
          If URL invocation is successful print success msg along with sms ID,
          else print the error msg
        */


        if ($responseCode >= 200 && $responseCode <= 300) {
            $jsonresponse = json_decode($ads_response);
            if($jsonresponse == null) {
             
			?>

                    <table class="successWide" align="center" style="font-family:Sans-serif;font-size:9pt;">
                        <tr>
                            <td style="font-weight:bold;width:20%;">Success:</td>
                        </tr>
                        <td class="cell"><?php echo "No Ads are Returned"; ?></td>
                        </table>
                        </div><?php
                       

	    }else {
            $type = $jsonresponse->AdsResponse->Ads->Type;
            $click_url = $jsonresponse->AdsResponse->Ads->ClickUrl;
            $image_url = $jsonresponse->AdsResponse->Ads->ImageUrl->Image;
            $text = $jsonresponse->AdsResponse->Ads->Text;
            $track_url = $jsonresponse->AdsResponse->Ads->TrackUrl;
            $content = $jsonresponse->AdsResponse->Ads->Content


            
                ?>

                <div style="text-align: left">
                    <br style="clear: both;"/>

                    <table class="successWide" align="center" style="font-family:Sans-serif;font-size:9pt;">
                        <tr>
                            <td style="font-weight:bold;width:20%;">Success:</td>
                        </tr>
<?php
                       if($type != null) {?>
                        <tr>
                            <td class="label" align="right" style="font-weight:bold;">Type:</td>
                            <td class="cell"><?php echo $type; ?></td>
                        </tr><?php }
                        if($click_url != null) {?>
                        <tr>
                            <td class="label" align="right" style="font-weight:bold;">ClickUrl:</td>
                            <td class="cell"><?php echo $click_url; ?></td>
                        </tr><?php }
                        if($track_url != null) {?>
                         <tr>
                            <td class="label" align="right" style="font-weight:bold;">TrackUrl:</td>
                            <td class="cell"><?php echo $track_url; ?></td>
                        </tr><?php }
                       if($text != null) {?>
                       <tr>
                            <td class="label" align="right" style="font-weight:bold;">Text:</td>
                            <td class="cell"><?php echo $text; ?></td>
                        </tr><?php }
                        if($image_url != null) {?>
                        <tr>
                            <td class="label" align="right" style="font-weight:bold;">ImageUrl.Image:</td>
                            <td class="cell"><?php echo $image_url; ?></td>
                        </tr>
                    </table>

                    <br style="clear: both;"/>

                    <div align="center">
                        <img src="<?php echo $image_url; ?>" alt=""/>
                    </div>
                </div>
                <?php }
                   if($content != null) {?>
                     </table>
                    <br style="clear: both;"/>

                    <div align="center">
                        <td class="cell"><?php echo $content; ?></td>
                
                <?php }?>

<?php

        }} else {
            $msghead = "Error";
            $msgdata = curl_error($ads);
            $errormsg = $msgdata . $ads_response;
            ?>

<div style="text-align: left">
  <br style="clear: both;"/>


            <div class="errorWide">
                <strong>ERROR:</strong><br/>

                <?php echo $errormsg  ?>
            </div>

            <?php
        }
    }

}?>
	
        <br clear="all" />
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
