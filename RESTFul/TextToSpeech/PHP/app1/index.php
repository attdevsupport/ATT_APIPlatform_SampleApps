<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/SpeechController.php';
require_once __DIR__ . '/lib/Util/Util.php';

$controller = new SpeechController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();
?>
<!DOCTYPE html>
<!-- 
tLicensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/

Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com

For more information contact developer.support@att.com
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample Speech Application - Text To Speech</title>
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
                    <a href="#" target="_blank">Full Page<img src="images/max.png" alt="Max" /></a>
                    <span class="divider"> |&nbsp;</span>
                </li>
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
                <h1>AT&amp;T Sample Application - Text To Speech</h1>
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
        </div> <!-- end of content -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="formData">
              <form name="TextToSpeech" action="index.php" method="post">
                <h3>Content Type:</h3>
                <select name="ContentType" id="ContentType">
                  <option value="text/plain" selected="selected">text/plain</option>
                  <option value="application/ssml+xml">application/ssml+xml</option>
                </select>
                <h3>Content:</h3>
                <label>text/plain</label><br>
                  <textarea id="plaintext" name="plaintext" readonly="readonly" 
                    rows="4"><?php echo htmlspecialchars(file_get_contents(__DIR__ . '/text/PlainText.txt')); ?></textarea><br>
                <label>application/ssml</label><br>
                <textarea id="ssml" name="ssml" readonly="readonly" 
                    rows="4"><?php echo htmlspecialchars(file_get_contents(__DIR__ . '/text/SSMLWithPhoneme.txt')); ?></textarea>
                <h3>X-Arg:</h3>
                <textarea id="x_arg" type="text" name="x_arg" readonly="readonly" rows="4" 
                    value="<?php echo $x_arg; ?>"><?php echo $x_arg; ?></textarea>
                <button id="btnSubmit" name="TextToSpeechButton" type="submit">
                  Submit
                </button>
              </form>
              <?php
              if (isset($results[SpeechController::RESULT_TTS])) {
                $response = $results[SpeechController::RESULT_TTS];
              ?>
                <div class="successWide" align="left">
                  <strong>SUCCESS:</strong>
                  <br>
                  <audio controls="controls" autobuffer="autobuffer" autoplay="autoplay">
                    <source src="data:audio/wav;base64,<?php echo base64_encode($response); ?>" />
                  </audio>
                  </div>
              <?php } ?>
              <?php
              if (isset($errors[SpeechController::ERROR_TTS])) {
                $error = $errors[SpeechController::ERROR_TTS];
              ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <br>
                  <?php echo htmlspecialchars($error); ?>
                  </div>
              <?php } ?>
              </div><!-- end formData -->
          </div><!-- end formContainer -->
        </div><!-- end formBox -->
        <div id="footer">
            <div id="ft">
                <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
                <p>
                The Application hosted on this site are working examples intended to be used for reference in creating 
                products to consume AT&amp;T Services and not meant to be used as part of your product. The data in 
                these pages is for test purposes only and intended only for use as a reference in how the services 
                perform. 
                <br> <br> 
                For download of tools and documentation, please go to 
                <a href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
                <br> 
                For more information contact 
                <a href="mailto:developer.support@att.com">developer.support@att.com</a>
                <br> <br>
                &copy; 2013 AT&amp;T Intellectual Property. All rights reserved.
                <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
                </p>
            </div> <!-- end of ft -->
        </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
</body>
</html>
