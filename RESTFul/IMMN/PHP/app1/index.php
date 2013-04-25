<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Sample/IMMNService.php';
$immnService = new IMMNService();
$msgHeaders = $immnService->getMessageHeaders();
$msgBody = $immnService->getMessageBody();
$immnSend = $immnService->sendMessage();
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
    <title>AT&amp;T Sample Application - In App Messaging from Mobile Number</title>
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
  <body>
    <div id="pageContainer">
      <div id="header">
        <div class="logo"></div> 
        <div id="menuButton" class="hide"><a id="jump" href="#nav">Main Navigation</a></div> 
        <ul class="links" id="nav">
          <li><a href="#" target="_blank">Full Page<img src="images/max.png" /></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkSource ?>" target="_blank">Source<img src="images/opensource.png" /></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkDownload ?>" target="_blank">Download<img src="images/download.png"></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="<?php echo $linkHelp ?>" target="_blank">Help</a>
          </li>
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - In App Messaging from Mobile Number</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div> 
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMessages">
              <h2>Send Messages:</h2>
              <form method="post" action="index.php" name="msgContentForm" >
                <div class="inputFields">
                  <?php if (isset($_SESSION['Address'])) { ?> 
                  <input placeholder="Address" name="Address" type="text" 
                      value="<?php echo htmlspecialchars($_SESSION['Address']); ?>" />     
                  <?php } else { ?>
                  <input placeholder="Address" name="Address" type="text" />     
                  <?php } ?>
                  <?php if (isset($_SESSION['checkbox']) && $_SESSION['checkbox'] == true) { ?>
                  <label>Group: <input name="groupCheckBox" type="checkbox" checked /></label>
                  <?php } else { ?>
                  <label>Group: <input name="groupCheckBox" type="checkbox" /></label>
                  <?php } ?>
                  <label>
                    Message:
                    <select name="message">
                      <option value="ATT IMMN sample message">ATT IMMN sample message</option>
                    </select>
                  </label>
                  <label>
                    Subject:
                    <select name="subject">
                      <option value="ATT IMMN sample subject">ATT IMMN sample subject</option>
                    </select>
                  </label>
                  <label>
                    Attachment:
                    <select name="attachment">
                      <option value="None">None</option>
                      <?php foreach ($immnService->getAttachments() as $attachment) { ?>
                        <?php if (isset($_SESSION['attachment']) && $_SESSION['attachment'] == $attachment) { ?>
                        <option value="<?php echo $attachment; ?>"
                            selected="selected"><?php echo htmlspecialchars($attachment); ?></option>
                        <?php } else { ?>
                        <option value="<?php echo $attachment; ?>"><?php echo htmlspecialchars($attachment); ?></option>
                        <?php } ?>
                      <?php } ?>
                    </select>
                  </label>
                  <button type="submit" class="submit" id="sendMessage" name="sendMessage">Send Message</button>
                </div> <!-- end of inputFields -->
              </form>

              <?php if ($immnSend != NULL) { ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <?php echo htmlspecialchars('Message ID: ' . $immnSend); ?>
                </div> <!-- end of successWide -->
              <?php } ?>

              <?php if ($immnService->errorSend() != NULL) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($immnService->errorSend()); ?>
                </div> <!-- end of errorWide -->
              <?php } ?>

            </div> <!-- end of sendMessages -->
            <div class="lightBorder"></div>
            <div id="getMessages">
              <h2>Read Messages:</h2>
              <form method="post" action="index.php" name="msgHeaderForm" id="msgHeaderForm">
                <div class="inputFields">

                  <?php if (isset($_SESSION['headerCountTextBox'])) { ?>
                    <input name="headerCountTextBox" type="text" maxlength="3" 
                        value="<?php echo htmlspecialchars($_SESSION['headerCountTextBox']); ?>" 
                        placeholder="Header Counter" />     
                  <?php } else { ?>
                    <input name="headerCountTextBox" type="text" maxlength="3" placeholder="Header Counter" />     
                  <?php } ?>

                  <?php if (isset($_SESSION['headerCountTextBox'])) { ?>
                    <input name="indexCursorTextBox" type="text" maxlength="30" 
                        value="<?php echo htmlspecialchars($_SESSION['indexCursorTextBox']); ?>" 
                        placeholder="Index Cursor" />     
                  <?php } else { ?>
                    <input name="indexCursorTextBox" type="text" maxlength="30" placeholder="Index Cursor" />     
                  <?php } ?>

                  <button type="submit" class="submit" name="getMessageHeaders" 
                      id="getMessageHeaders">Get Message Headers</button>

                </div> <!-- end of inputFields -->
              </form>
              <form method="post" action="index.php" name="msgContentForm" id="msgContentForm">
                <div class="inputFields">
                  <?php if (isset($_SESSION['MessageId'])) { ?>
                    <input name="MessageId" id="MessageId" type="text" maxlength="30" 
                        value="<?php echo htmlspecialchars($_SESSION['MessageId']); ?>" placeholder="Message ID" />     
                  <?php } else { ?>
                    <input name="MessageId" id="MessageId" type="text" maxlength="30" placeholder="Message ID" />     
                  <?php } ?>

                  <?php if (isset($_SESSION['PartNumber'])) { ?>
                    <input name="PartNumber" id="PartNumber" type="text" maxlength="30" 
                        value="<?php echo htmlspecialchars($_SESSION['PartNumber']); ?>" placeholder="Part Number" />     
                  <?php } else { ?>
                    <input name="PartNumber" id="PartNumber" type="text" maxlength="30" placeholder="Part Number" />     
                  <?php } ?>
                  <button type="submit" class="submit" name="getMessageContent" 
                      id="getMessageContent">Get Message Content</button>
                </div> <!-- end of inputFields -->
              </form>
              <label class="note">To use this feature, you must be a subscriber to My AT&amp;T Messages.</label>
            </div> <!-- end of getMessages -->
          </div> <!-- end of formContainer -->

          <!-- BEGIN HEADER CONTENT RESULTS -->
          <?php 
          if ($msgBody != NULL) { 
          $rawCType = $msgBody->getContentType();
          $tokenCType = strtok($rawCType, ';');
          $splitCType = strtok($rawCType, '/');
          ?> 
          <div class="successWide">
            <strong>SUCCESS:</strong>
          </div> <!-- end of successWide -->
          <?php 
          if (strcmp('TEXT', $splitCType) == 0) { 
          echo htmlspecialchars($msgBody->getData()); 
          } else if (strcmp($tokenCType, 'APPLICATION/SMIL') == 0) {
          $data = htmlspecialchars($msgBody->getData());
          ?>

          <textarea name="TextBox1" rows="2" cols="20" id="TextBox1" disabled="disabled"><?php echo $data; ?></textarea>
          <?php } else if (strcmp($splitCType, 'IMAGE') == 0) { ?>

          <img src="data:<?php echo $tokenCType; ?>;base64,<?php echo base64_encode($msgBody->getData()); ?>" />
          <?php
          }
          }
          ?>

          <!-- END HEADER CONTENT RESULTS -->

          <!-- BEGIN HEADER RESULTS -->
          <?php 
          if ($msgHeaders != NULL) { 
          $headers = $msgHeaders->getHeaders();
          $indexCursor = $msgHeaders->getIndexCursor();
          $headerCount = $msgHeaders->getHeaderCount();
          ?>

          <div class="successWide">
            <strong>SUCCESS:</strong>
          </div> <!-- end of successWide -->
          <p id="headerCount">Header Count: <?php echo $headerCount; ?></p>
          <p id="indexCursor">Index Cursor: <?php echo $indexCursor; ?></p>
          <table class="kvp" id="kvp">
            <thead>
              <tr>
                <th>MessageId</th>
                <th>From</th>
                <th>To</th>
                <th>Received</th>
                <th>Text</th>
                <th>Favorite</th>
                <th>Read</th>
                <th>Type</th>
                <th>Direction</th>
                <th>Contents</th>
              </tr>
            </thead>
            <tbody>
            <?php for ($i = 0; $i < count($headers); ++$i) { 
              $header = $headers[$i];
              $text = $header->getText();
              if ($text == NULL || $text == '') {
                $text = '&#45';
              }
              $toArr = $header->getTo();
              $to = (count($toArr) > 0) ? $toArr[0] : ''; 
              for ($j = 1; $j < count($toArr); ++$j) {
                $to .= ',' . $toArr[$j];
              }
              $favorite = ($header->getFavorite()) ? 'true' : 'false';
              ?>

              <tr id="<?php echo 'row' . $i; ?>">
                <td data-value="MessageId"><?php echo $header->getMessageId(); ?></td>
                <td data-value="From"><?php echo $header->getFrom(); ?></td>
                <td data-value="To"><?php echo $to; ?></td>
                <td data-value="Received"><?php echo $header->getReceived(); ?></td>
                <td data-value="Text"><?php echo $text; ?></td>
                <td data-value="Favorite"><?php echo $favorite; ?></td>
                <td data-value="Read"><?php echo $header->getRead(); ?></td>
                <td data-value="Type"><?php echo $header->getType(); ?></td>
                <td data-value="Direction"><?php echo $header->getDirection(); ?></td>
                <td data-value="Contents">
                <?php if ($header->getMmsContent() != NULL) { ?>

                  <select id="attachments" onchange='chooseSelect("<?php echo 'row' . $i; ?>", this)'>
                    <option>More..</option>
                    <?php foreach ($header->getMmsContent() as $p) { ?>

                    <option><?php echo $p['PartNumber'] . '-' . $p['ContentName'] . '-' . $p['ContentType']; ?></option>
                    <?php } ?>

                  </select>
                    <?php } else { ?>
                      &#45;
                      <?php } ?>

                </td>
              </tr>
              <?php } ?>

            </tbody>
          </table>
          <?php } ?>

          <!-- END HEADER RESULTS -->
          <?php if ($immnService->errorGet() != NULL) { ?>

          <div class="errorWide">
            <strong>ERROR:</strong>
            <?php echo htmlspecialchars($immnService->errorGet()); ?>

          </div> <!-- end of errorWide -->
          <?php } ?>

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
          For download of tools and documentation, please go to 
          <a href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
          <br> 
          For more information contact 
          <a href="mailto:developer.support@att.com">developer.support@att.com</a>
          <br> <br>
          &copy; 2013 AT&amp;T Intellectual Property. All rights reserved.
          <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <script>setup();</script>
  </body>
</html>
