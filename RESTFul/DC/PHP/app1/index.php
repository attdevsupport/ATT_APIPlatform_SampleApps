<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/DCController.php';
require_once __DIR__ . '/lib/Util/Util.php';
use Att\Api\Util\Util;
$controller = new DCController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();
?>
<!DOCTYPE html>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample DC Application - Get Device Capabilities Application</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
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
    <div id="pageContainer">
      <div id="header">
        <div class="logo"> 
        </div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div> 
        <ul class="links" id="nav">
          <li><a href="#" target="_blank">Full Page<img src="images/max.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkSource; ?>" target="_blank">Source<img src="images/opensource.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkDownload; ?>" target="_blank">Download<img src="images/download.png"></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkHelp; ?>" target="_blank">Help</a>
          </li>
          <li id="back"><a href="#top">Back to top</a>
          </li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample DC Application - Get Device Capabilities Application</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->
        <div class="formBox" id="formBox">
          <div class="formContainer" id="formContainer">
            <h2>Feature 1: Get Device Capabilities</h2>
            <div class="lightBorder"></div>

            <div class="note">
              <strong>OnNet Flow:</strong>
              Request Device Capabilities details from the AT&amp;T network
              for the mobile device of an AT&amp;T subscriber who is using an AT&amp;T direct Mobile data
              connection to access this application.
              <br />
              <strong>OffNet Flow:</strong> Where the end-user is not on an AT&amp;T Mobile data connection
              or using a Wi-Fi or tethering connection while accessing this application. This
              will result in an HTTP 400 error.
            </div> <!-- end note -->

            <?php 
            if (isset($results[DCController::RESULT_DEVICE_INFO])) { 
              $dInfo = $results[DCController::RESULT_DEVICE_INFO];
              $dcaps = $dInfo->getCapabilities();
            ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br />
                Device parameters listed below.
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td data-value="Parameter">TypeAllocationCode</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dInfo->getTypeAllocationCode()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">Name</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getName()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">Vendor</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getVendor()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">Model</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getModel()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">FirmwareVersion</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getFirmwareVersion()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">UaProf</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getUaProf()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">MmsCapable</td>
                    <td data-value="Value"> <?php echo $dcaps->isMmsCapable() ? 'Y' : 'N'; ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">AssistedGps</td>
                    <td data-value="Value"> <?php echo $dcaps->isAssistedGps() ? 'Y' : 'N'; ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">LocationTechnology</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getLocationTechnology()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">DeviceBrowser</td>
                    <td data-value="Value"> <?php echo htmlspecialchars($dcaps->getDeviceBrowser()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">WapPushCapable</td>
                    <td data-value="Value"> <?php echo $dcaps->getDeviceBrowser() ? 'Y' : 'N'; ?></td>
                  </tr>
                </tbody>
              </table>
            <?php } ?>
              
            <?php
            if (isset($errors[DCController::ERROR_DEVICE_INFO])) {
              $err = $errors[DCController::ERROR_DEVICE_INFO];
            ?>
              <div class="errorWide">
                <b>ERROR:</b><br>
                <?php echo htmlspecialchars($err); ?>
              </div>
            <?php } ?>
          </div> <!-- end of formContainer -->
        </div> <!-- end of formBox -->

        <!-- SAMPLE APP CONTENT ENDS HERE! -->

      </div> <!-- end of content -->
      <div class="border"></div>
      <div id="footer">
        <div id="powered_by">
          Powered by AT&amp;T Cloud Architecture
        </div>
        <p>
        The Application hosted on this site are working examples
        intended to be used for reference in creating products to consume
        AT&amp;T Services and not meant to be used as part of your
        product. The data in these pages is for test purposes only and
        intended only for use as a reference in how the services perform.
        <br /><br />
        For download of tools and documentation, please go to 
        <a href="https://devconnect-api.att.com/" 
          target="_blank">https://devconnect-api.att.com</a>
        <br> For more information contact 
        <a href="mailto:developer.support@att.com">developer.support@att.com</a>
        <br /><br />
        &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
