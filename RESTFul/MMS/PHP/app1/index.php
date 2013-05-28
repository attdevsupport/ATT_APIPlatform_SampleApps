<?php
session_start();
require_once __DIR__ . '/lib/Util/Util.php';
require_once __DIR__ . '/src/Controller/MMSController.php';
require_once __DIR__ . '/config.php';
$controller = new MMSController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();

?>
<!DOCTYPE html>
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Multimedia Messaging Service</title>		
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <link rel="stylesheet" type="text/css" href="style/common.css">
    <script src="scripts/utils.js"></script>
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
        <div class="logo"></div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div> <!-- end of menuButton -->
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
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - Multimedia Messaging Service</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:</b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMMS">
              <h2>Feature 1: Send MMS Message</h2>
              <form method="post" name="sendMms" action="index.php">
                <div class="inputFields">
                  <?php if (isset($_SESSION['addr'])) { ?>
                  <input name="address" placeholder="Address" 
                      value="<?php echo htmlspecialchars($_SESSION['addr']); ?>" />
                  <?php } else { ?>
                  <input name="address" placeholder="Address" />
                  <?php } ?>
                  <label>
                    Message:
                    <select name="subject">
                      <option>MMS Sample Message</option>
                    </select>
                  </label>
                  <label>
                    Attachment:
                    <select name="attachment">
                      <?php foreach ($results['fnames'] as $fname) { ?>
                      <?php if (isset($_SESSION['attachment']) && $_SESSION['attachment'] == $fname) { ?>
                      <option selected="selected"><?php echo $fname; ?></option>
                      <?php } else { ?>
                      <option><?php echo $fname; ?></option>
                      <?php } ?>
                      <?php } /* end of foreach */?>
                    </select>
                  </label>
                  <label>
                  <?php if (isset($_SESSION['notifyDeliveryStatus']) && $_SESSION['notifyDeliveryStatus'] == true) { ?>
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="True" checked
                      title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                  <?php } else { ?>
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="True"
                      title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                <?php } ?>
                      Receive Delivery Status Notification<br>
                  </label>
                  <button type="submit" class="submit" name="sendMms">Send MMS Message</button>
                </div> <!-- end of inputFields -->
              </form>
            </div> <!-- end of sendMMS -->
            <?php if (isset($errors['sendMMS'])) { ?>
            <div class="errorWide">
              <strong>ERROR: </strong><br>
              <?php echo htmlspecialchars($errors['sendMMS']); ?>
            </div>
            <?php } ?>
            <?php if (isset($results['sendMMS'])) { ?>
            <div class="successWide">
              <strong>SUCCESS: </strong><br>
              <strong>messageId: </strong><?php echo $results['messageId']; ?><br>
              <?php if (isset($results['resourceURL'])) { ?>
              <strong>resourceURL: </strong><?php echo $results['resourceURL']; ?><br>
              <?php } ?>
            </div>
            <?php } ?>
            <div class="lightBorder"></div>
            <div id="getDeliveryStatus">
              <h2>Feature 2: Get Delivery Status</h2>
              <form method="post" name="getStatus" action="index.php">
                <div class="inputFields">
                  <?php if (isset($results['id'])) { ?>
                  <input maxlength="20" name="mmsId" placeholder="Message ID" value="<?php echo $results['id']; ?>" />
                  <?php } else { ?>
                  <input maxlength="20" name="mmsId" placeholder="Message ID" />
                  <?php } ?>
                  <button type="submit" class="submit" name="getStatus">Get Status</button>
                </div> <!-- end of inputFields -->	
              </form> 
            </div> <!-- end of getDeliveryStatus -->
            <?php if (isset($errors['getStatus'])) { ?>
            <div class="errorWide">
            <strong>ERROR: </strong><br>
            <?php echo htmlspecialchars($errors['getStatus']); ?>
            </div>
            <?php } else if(isset($results['getStatus'])) { 
              $sResult = $results['getStatus']; $infoList = $sResult['DeliveryInfoList']; 
              $info = $infoList['DeliveryInfo']; $firstInfo = $info[0];
              $status = $firstInfo['DeliveryStatus']; 
              $resourceURL = $infoList['ResourceUrl'];
            ?>
            <div class="successWide">
            <strong>SUCCESS: </strong><br>
            <strong>Status: </strong><?php echo htmlspecialchars($status); ?><br>
            <strong>Resource URL: </strong><?php echo htmlspecialchars($resourceURL); ?><br>
            </div>
            <?php } ?>
            <div class="lightBorder"></div>
            <div id="receiveStatusdiv">
              <form method="post" name="refresh" action="index.php">
                <h2>Feature 3: Receive Delivery Status</h2>
                <button type="submit" class="submit" name="receiveStatusBtn" 
                    id="receiveStatusBtn">Refresh Notifications</button>
              </form>
              <table>
                <thead>
                  <tr>
                    <th>Message Id</th>
                    <th>Address</th>
                    <th>Delivery Status</th>
                  </tr>
                </thead>
                <tbody>
                <?php foreach ($results['resultStatusN'] as $statusNotification) { 
                  $dInfoNotification = $statusNotification['deliveryInfoNotification']; 
                  $dInfo = $dInfoNotification['deliveryInfo'];
                ?>
                  <tr>
                    <td data-value="Message Id"><?php echo $dInfoNotification['messageId']; ?></td>
                    <td data-value="Status"><?php echo $dInfo['address']; ?></td>
                    <td data-value="Resouce Url"><?php echo $dInfo['deliveryStatus']; ?></td>
                  </tr>
                  <?php } ?>
                </tbody>
              </table>
            </div> <!-- end of receiveStatus -->
            <div class="lightBorder"></div>
            <div id="webGallery">
              <h2>Feature 4: Web gallery of MMS photos sent to short code</h2>
              <p>Photos sent to short code <?php echo $shortCode ?> : <?php echo count($results['messages']) ?></p>
              <?php foreach ($results['messages'] as $msg) { 
              $txt = file_get_contents('MMSImages/' . $msg['text']);
              ?>
              <img src="<?php echo 'MMSImages/' . $msg['image']; ?>" width="150" border="0" /><br>
              <strong>Sent from:&nbsp;</strong><?php echo $msg['address']; ?><br>
              <strong>On:&nbsp;</strong><?php echo $msg['date']; ?><br>
              <strong>Text:&nbsp;</strong><?php echo $txt; ?><br>
              <?php } ?>
            </div> <!-- end of webGallery -->	
          </div> <!-- end of formContainer -->
        </div> <!-- end of formBox -->
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
          <br><br>
          For download of tools and documentation, please go to 
          <a href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
          <br> For more information contact 
          <a href="mailto:developer.support@att.com">developer.support@att.com</a>
          <br><br>
          &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. 
          <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
