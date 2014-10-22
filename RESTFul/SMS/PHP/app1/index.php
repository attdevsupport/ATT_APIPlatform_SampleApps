<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/lib/Util/Util.php';
require_once __DIR__ . '/src/Controller/SMSController.php';

use Att\Api\Util\Util;

$controller = new SMSController();
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
    <title>AT&amp;T Sample Application - Basic SMS Service Application</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
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
  <body onload="setup()">
    <div id="pageContainer">
      <div id="header">
        <div class="logo"></div> 
        <div id="menuButton" class="hide"><a id="jump" href="#nav">Main Navigation</a></div> 
        <ul class="links" id="nav">
          <li>
            <a href="<?php echo $linkSource; ?>" target="_blank" 
                id="SourceLink">Source<img alt="source" src="images/opensource.png" /></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkDownload; ?>" target="_blank" 
                id="DownloadLink">Download<img alt="download" src="images/download.png"></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkHelp; ?>" target="_blank" id="HelpLink">Help</a>
          </li>
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - Basic SMS Service Application</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div> 
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div class="inputFields">
              <div id="sendSMSdiv">
                <h2>Feature 1: Send SMS</h2>
                <form method="post" action="index.php#sendSMS" name="sendSMSForm" id="sendSMSForm">
                  <input placeholder="Address" name="address" id="address" type="text"
                      value="<?php echo isset($_SESSION['rawaddrs']) ? $_SESSION['rawaddrs'] : ''; ?>" />
                  <label>
                    Message
                    <select name="message" id="message">
                      <option value="ATT SMS sample Message">ATT SMS sample message</option>
                    </select>
                  </label>
                  <label>
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="True"
                        title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                    Receive Delivery Status Notification<br />
                  </label>
                  <button type="submit" class="submit" name="sendSMS" id="sendSMS">Send SMS</button>
                </form>
                <?php 
                if (isset($errors[SMSController::ERROR_SEND_SMS])) { 
                  $sendErr = $errors[SMSController::ERROR_SEND_SMS]; 
                ?>
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  <?php echo htmlspecialchars($sendErr); ?>
                </div>
                <?php } else if (isset($results[SMSController::RESULT_SEND_SMS])) {
                  $response = $results[SMSController::RESULT_SEND_SMS];
                  $msgId = $response->getMessageId();
                  $resourceUrl = $response->getResourceUrl();
                ?>
                <div class="successWide">
                  <strong>SUCCESS: </strong><br>
                  <strong>messageId: </strong><?php echo $msgId; ?><br>
                  <?php if ($resourceUrl != null) { ?>
                  <strong>resourceURL: </strong><?php echo $resourceUrl; ?><br>
                  <?php } ?>
                </div>
                <?php } ?>
              </div> <!-- end of sendSMS -->
              <div class="lightBorder"></div>
              <div id="getStatusdiv">
                <h2>Feature 2: Get Delivery Status</h2>
                <form method="post" action="index.php#getStatus" name="getStatusForm" id="getStatusForm">
                  <input placeholder="Message ID" name="messageId" id="messageId" type="text" 
                      value="<?php echo isset($_SESSION['SmsId']) ? $_SESSION['SmsId'] : '' ?>">
                  <button type="submit" class="submit" name="getStatus" id="getStatus">Get Status</button>
                </form>
                <?php 
                if (isset($errors[SMSController::ERROR_SMS_DELIVERY])) { 
                  $deliveryErr = $errors[SMSController::ERROR_SMS_DELIVERY];
                ?>
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  <?php echo htmlspecialchars($deliveryErr); ?>
                </div>
                <?php } else if(isset($results[SMSController::RESULT_SMS_DELIVERY])) { 
                $response = $results[SMSController::RESULT_SMS_DELIVERY];
                $statuses = $response->getDeliveryInfoList();
                $resourceUrl = $response->getResourceUrl();
                ?>
                <div class="successWide">
                  <strong>SUCCESS: </strong><br>
                  <strong>ResourceUrl: </strong><?php echo htmlspecialchars($resourceUrl); ?><br>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>Id</th>
                      <th>Address</th>
                      <th>DeliveryStatus</th>
                    </tr>
                  </thead>
                  <tbody>
                  <?php foreach ($statuses as $status) { ?>
                    <tr>
                      <td data-value="Id"><?php echo htmlspecialchars($status->getId()); ?></td>
                      <td data-value="Address"><?php echo htmlspecialchars($status->getAddress()); ?></td>
                      <td data-value="DeliveryStatus"><?php echo htmlspecialchars($status->getDeliveryStatus()); ?></td>
                    </tr>
                  <?php } ?>
                  </tbody>
                </table>
                <?php } ?>
              </div> <!-- end of getStatus -->
              <div class="lightBorder"></div>
              <div id="receiveStatusdiv">
                <h2>Feature 3: Receive Delivery Status</h2>
                <form method="post" action="index.php" name="refreshStatusForm" id="refreshStatusForm">
                  <button type="submit" class="submit" name="receiveStatusBtn" id="receiveStatusBtn">
                    Refresh Notifications</button>
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
                  <?php foreach ($results[SMSController::RESULT_STATUS_ARR] as $statusNotification) { 
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
              </div><!-- end of receiveStatus -->
              <div class="lightBorder"></div>
              <div id="getMessagesDiv">
                <h2>Feature 4: Get Messages (<?php echo $getMsgsShortCode; ?>)</h2>
                <form method="post" action="index.php" name="getMessagesForm" id="getMessagesForm">
                  <button type="submit" class="submit" name="getMessages" id="getMessages">
                    Get Messages</button>
                </form>
                <?php 
                if (isset($errors[SMSController::ERROR_GET_MSGS])) { 
                  $getMsgsErr = $errors[SMSController::ERROR_GET_MSGS];
                ?>
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  <?php echo htmlspecialchars($getMsgsErr); ?>
                </div>
                <?php } else if (isset($results[SMSController::RESULT_GET_MSGS])) {
                  $response = $results[SMSController::RESULT_GET_MSGS];
                  $msgList = $response->getMessages();
                  $numMessages = $response->getNumberOfMessages();
                  $numPending = $response->getNumberOfPendingMessages();
                  ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong><br>
                  <strong>Messages in this batch: <strong><?php echo $numMessages; ?><br>
                  <strong>Messages pending: <strong><?php echo $numPending; ?><br>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>Message Index</th>
                      <th>Message Text</th>
                      <th>Sender Address</th>
                    </tr>
                  </thead>
                  <tbody>
                  <?php foreach($msgList as $msg) { ?>
                    <tr>
                      <td data-value="Message Index"><?php echo htmlspecialchars($msg->getMessageId()); ?></td>
                      <td data-value="Message Text"><?php echo htmlspecialchars($msg->getMessage()); ?></td>
                      <td data-value="Sender Address"><?php echo htmlspecialchars($msg->getSenderAddress()); ?></td>
                    </tr>
                    <?php } ?>
                  </tbody>
                </table>
                <?php } ?>
              </div><!-- end of getMessages -->
              <div class="lightBorder"></div>
              <div id="receiveMsgs">
                <h2>Feature 5: Receive Messages (<?php echo $receiveMsgsShortCode; ?>)</h2>
                <form method="post" action="index.php" name="receiveMessagesForm" id="receiveMessagesForm">
                  <button type="submit" class="submit" name="receiveMessages" 
                      id="receiveMessages">Refresh Received Messages</button>
                </form>
                  <table>
                    <thead>
                      <tr>
                        <th>DateTime</th>
                        <th>Message Id</th>
                        <th>Message</th>
                        <th>Sender Address</th>
                        <th>Destination Address</th>
                      </tr>
                    </thead>
                    <tbody>
                      <?php foreach ($results[SMSController::RESULT_MSGS_ARR] as $msg) { ?>
                        <tr>
                          <td data-value="DateTime"> <?php echo $msg['DateTime']; ?></td>
                          <td data-value="Message Id"> <?php echo $msg['MessageId'] == '' ? '-' : $msg['MessageId']; ?></td>
                          <td data-value="Message"> <?php echo $msg['Message']; ?></td>
                          <td data-value="Sender Address"><?php echo $msg['SenderAddress']; ?></td>
                          <td data-value="Destination Address"><?php echo $msg['DestinationAddress']; ?></td>
                        </tr>
                      <?php } ?>
                  </tbody>
                </table>
              </div> <!-- end of receiveMsgs -->
            </div> <!-- end of inputFields -->
          </div> <!-- end of formContainer -->
        </div> <!-- end of formBox -->
      </div> <!-- end of content -->
      <div class="border"></div>
      <div id="footer">
        <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
        <p>
          The Application hosted on this site are working examples intended to be used for reference in creating 
          products to consume AT&amp;T Services and not meant to be used as part of your product. The data in 
          these pages is for test purposes only and intended only for use as a reference in how the services 
          perform. 
          <br> <br> 
          To access your apps, please go to
          <a href="https://developer.att.com/developer/mvc/auth/login"
            target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
          <br> For support refer to
          <a href="https://developer.att.com/support">https://developer.att.com/support</a>
          <br> <br>
          &copy; 2014 AT&amp;T Intellectual Property. All rights reserved.
          <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <script>setup();</script>
  </body>
</html>
