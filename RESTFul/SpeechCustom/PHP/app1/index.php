<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/SpeechController.php';
require_once __DIR__ . '/lib/Util/Util.php';

use Att\Api\Util\Util;

$controller = new SpeechController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();
?>
<!DOCTYPE html>
<!-- 
Copyright 2014 AT&T

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
-->
<html lang="en">
  <head>
    <title>AT&amp;T Sample Speech Application - Speech to Text Custom</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type">
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
    <link rel="stylesheet" type="text/css" href="style/common.css">
    <script type="text/javascript">
      var _gaq = _gaq || [];
      _gaq.push(['_setAccount', 'UA-33466541-1']);
        _gaq.push(['_trackPageview']);

        (function () {
             var ga = document.createElement('script');
             ga.type = 'text/javascript';
             ga.async = true;
             ga.src = ('https:' == document.location.protocol ? 'https://ssl'
                                         : 'http://www')
                                         + '.google-analytics.com/ga.js';
             var s = document.getElementsByTagName('script')[0];
             s.parentNode.insertBefore(ga, s);
         })();

        function enableNameParam(list,nameParam){
          var selectedValue = list.options[list.selectedIndex].value;
          if(selectedValue == "GenericHints"){
            document.getElementById("nameParam").disabled=false;
          }else{
            document.getElementById("nameParam").disabled=true;
            var choices = document.getElementById("nameParam");
            choices.options[0].selected = true;
          }
        }
    </script>
  </head>
  <body>
    <div id="pageContainer" class="pageContainer">
      <div id="header">
        <div class="logo" id="top"></div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div>
        <ul class="links" id="nav">
          <li>
            <a href="<?php echo $linkSource; ?>" 
              target="_blank">Source<img src="images/source.png" alt="Source" /></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkDownload; ?>" 
              target="_blank">Download<img src="images/download.png" alt="Link"></a>
            <span class="divider">|&nbsp;</span>
          </li>
          <li><a href="<?php echo $linkHelp; ?>" target="_blank">Help</a></li>
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div class="content">
        <div class="contentHeading">
          <h1>AT&amp;T Sample Application - Speech to Text Custom</h1>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div>
            <div>
              <b>Client Time:&nbsp;</b>
              <script>
                document.write("" + new Date());
              </script>
            </div>
            <div>
              <b>User Agent:&nbsp;</b>
              <script>
                document.write("" + navigator.userAgent);
              </script>
            </div>
          </div> <!-- end of introtext -->
        </div><!-- end of contentHeading -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <form name="SpeechToText" action="index.php" method="post">
              <div id="formData"> 
                <!-- start context select -->
                <h3>Speech Context:</h3>
                <select name="SpeechContext" onchange="enableNameParam(this,'nameParam')">
                <?php
                  $speechContexts = $controller->getSpeechContexts();
                    foreach ($speechContexts as $sname) { 
                      $selected = '';
                      if ($controller->isSpeechContextSelected($sname)) $selected = 'selected ';
                ?>
                        <option <?php echo $selected; ?>value="<?php echo $sname ?>"><?php echo $sname ?></option>
                <?php } ?>
                </select> 
                <!-- end context select --> 

                <!-- name parameter select -->
                <h3>Name Parameter:</h3>
                <select name="nameParam" id="nameParam">
                  <?php 
                    $nameParams = array('x-grammar', 'x-grammar-prefix', 'x-grammar-altgram');
                    foreach ($nameParams as $nameParam) {
                     if(isset($_SESSION['nameParam']) && $_SESSION['nameParam'] == $nameParam) {
                  ?>
                      <option value="<?php echo $nameParam; ?>"
                        selected="selected"><?php echo $nameParam; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $nameParam; ?>"><?php echo $nameParam; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>
                <!-- end name parameter select -->
                            
                <!-- start audio file select -->
                <h3>Audio File:</h3>
                <select name="audio_file">
                  <?php 
                  $audioFiles = $controller->getAudioFiles();
                    foreach ($audioFiles as $fname) { 
                      $selected = '';
                      if ($controller->isAudioFileSelected($fname)) $selected ='selected ';
                  ?>
                        <option <?php echo $selected; ?>value="<?php echo $fname ?>"><?php echo $fname ?></option>
                  <?php } ?>
                </select> 
                <!-- end audio file select -->

                <h3>X-Arg:</h3>
                <label><?php echo htmlspecialchars($x_arg); ?></label>

                <h3>MIME Data</h3>
                <textarea id="x_arg" name="x_arg" readonly="readonly" rows="4" 
                    value="<?php echo htmlspecialchars($controller->getMIMEData()); ?>"
                    ><?php echo htmlspecialchars($controller->getMIMEData()); ?></textarea>
                <br>
                <button type="submit" name="SpeechToText" id="SpeechToTextCustom">Submit</button>
              </div> <!-- end of formData -->
            </form> <!-- end of SpeechToText form -->
          </div> <!-- end of formContainer -->
          <?php 
          if (isset($errors[SpeechController::ERROR_STT])) {
            $error = $errors[SpeechController::ERROR_STT];
          ?>
            <div class="errorWide">
              <strong>ERROR:</strong><br><?php echo htmlspecialchars($error); ?>
            </div>
          <?php } else if (isset($results[SpeechController::RESULT_STT])) { 
            $response = $results[SpeechController::RESULT_STT]; 
          ?>
            <div class="successWide">
              <strong>SUCCESS:</strong> <br>Response parameters listed below.
            </div>
            <table class="kvp">
              <thead>
                <tr>
                  <th class="label">Parameter</th>
                  <th class="label">Value</th>
                </tr>
              </thead>
              <tbody>
              <?php 
              $recognization = $response['Recognition']; 
              $nbest = $recognization['NBest'];
              $nbest = $nbest[0];
              ?>
                <tr>
                  <td class="cell" align="center"><em>ResponseId</em></td>
                  <td class="cell" align="center"><em><?php echo $recognization['ResponseId']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Status</em></td>
                  <td class="cell" align="center"><em><?php echo $recognization['Status']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Hypothesis</em></td>
                  <td class="cell" align="center"><em><?php echo $nbest['Hypothesis']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>LanguageId</em></td>
                  <td class="cell" align="center"><em><?php echo $nbest['LanguageId']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Confidence</em></td>
                  <td class="cell" align="center"><em><?php echo $nbest['Confidence']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Grade</em></td>
                  <td class="cell" align="center"><em><?php echo $nbest['Grade']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>ResultText</em></td>
                  <td class="cell" align="center"><em><?php echo $nbest['ResultText']; ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Words</em></td>
                  <td class="cell" align="center"><em><?php echo json_encode($nbest['Words']); ?></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>WordScores</em></td>
                  <td class="cell" align="center"><em><?php echo json_encode($nbest['WordScores']); ?></em></td>
                </tr>
              </tbody>
            </table>
          <?php } ?>
        </div> <!-- end of formBox -->
      </div> <!-- end of content -->
      <div id="footer">
        <div id="ft">
          <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
          <p>
            The Application hosted on this site are working examples intended to be used for reference in creating 
            products to consume AT&amp;T Services and not meant to be used as part of your product. The data in 
            these pages is for test purposes only and intended only for use as a reference in how the services 
            perform. 
            <br><br> 
            To access your apps, please go to
            <a href="https://developer.att.com/developer/mvc/auth/login"
              target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
            <br> For support refer to
            <a href="https://developer.att.com/support">https://developer.att.com/support</a>
            <br> <br>
            &copy; 2014 AT&amp;T Intellectual Property. All rights reserved.
            <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
          </p>
        </div> <!-- end of ft -->
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
