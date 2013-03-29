<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Sample/SpeechService.php';
require_once __DIR__ . '/lib/Util/Util.php';
$speechService = new SpeechService();
$response = $speechService->speechToText();
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
    <title>AT&amp;T Sample Speech Application - Speech to Text(Generic)</title>
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
                <h1>AT&amp;T Sample Application - Speech to Text</h1>
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
                            <select name="SpeechContext">
                              <?php
                              $speechContexts = $speechService->getSpeechContexts();
                              foreach ($speechContexts as $sname) { 
                              $selected = '';
                              if ($speechService->isSpeechContextSelected($sname)) $selected = 'selected ';
                              ?>
                              <option <?php echo $selected; ?>value="<?php echo $sname ?>"><?php echo $sname ?></option>
                              <?php } ?>
                            </select> 
                        <!-- end context select --> 
                            
                        <!-- start audio file select -->
                            <h3>Audio File:</h3>
                            <select name="audio_file">
                              <?php 
                              $audioFiles = $speechService->getAudioFiles();
                              foreach ($audioFiles as $fname) { 
                              $selected = '';
                              if ($speechService->isAudioFileSelected($fname)) $selected ='selected ';
                              ?>
                              <option <?php echo $selected; ?>value="<?php echo $fname ?>"><?php echo $fname ?></option>
                              <?php } ?>
                            </select> 
                        <!-- end audio file select -->
                            <div id="chunked">

                                <br>
                                <b>Send Chunked:</b>
                                <?php $chked = $speechService->isChunkedSelected() ? ' checked'  : ''; ?>

                                <input name="chkChunked" value="Send Chunked" type="checkbox"<?php echo $chked; ?>>
                            </div>
                            <h3>X-Arg:</h3>
                            <textarea id="x_arg" name="x_arg" readonly="readonly" 
                                rows="4" value="<?php echo $x_arg?>"><?php echo $x_arg ?></textarea>
                            <h3>X-SpeechSubContext</h3>
                            <textarea id="x_subContext" name="x_arg" readonly="readonly" rows="4" 
                                value="<?php echo $xSpeechSubContext?>"><?php echo $xSpeechSubContext?></textarea>
                            <br>
                            <button type="submit" id="btnSubmit" name="SpeechToText">Submit</button>
                        </div> <!-- end of formData -->
                    </form> <!-- end of SpeechToText form -->
                </div> <!-- end of formContainer -->
                <?php 
                $error = $speechService->getError();
                if ($error) {
                ?>

                <div class="errorWide">
                    <strong>ERROR:</strong><br><?php echo htmlspecialchars($error); ?>
                </div>
                <?php } else if ($response) { ?>

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
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'ResponseID'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $response->getResponseId(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'Status'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $response->getStatus(); ?></em></td>
                        </tr>
                            <?php 
                            $nbest = $response->getNBest();
                            if ($nbest != NULL) { ?>

                        <tr>
                            <td class="cell" align="center"><em><?php echo 'Hypothesis'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $nbest->getHypothesis(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'LanguageId'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $nbest->getLanguageId(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'Confidence'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $nbest->getConfidence(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'Grade'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $nbest->getGrade(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'ResultText'; ?></em></td>
                            <td class="cell" align="center"><em><?php echo $nbest->getResultText(); ?></em></td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'Words'; ?></em></td>
                            <td class="cell" align="center">
                                <em><?php echo json_encode($nbest->getWords()); ?></em>
                            </td>
                        </tr>
                        <tr>
                            <td class="cell" align="center"><em><?php echo 'WordScores'; ?></em></td>
                            <td class="cell" align="center">
                                <em><?php echo json_encode($nbest->getWordScores()); ?></em>
                            </td>
                        </tr>
                            <?php } ?>

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
