<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/IMMNController.php';
require_once __DIR__ . '/lib/Util/Util.php';
use Att\Api\Util\Util;
$controller = new IMMNController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();
?>
<!DOCTYPE html>
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - In App Messaging from Mobile Number</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
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
        </div> 
        <ul class="links" id="nav">
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
          <h1>AT&amp;T Sample Application - In App Messaging from mobile number</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div> 
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->
        <div class="formBox">
          <div class="formContainer">

            <div class="lightBorder"></div>

            <a id="sendMsgToggle" 
              href="javascript:toggle('sendMsg');">Send Message</a>
            <div class="toggle" id="sendMsg">
              <label><i>Max message size must be 1MB or less</i></label>
              <h2>Send Message</h2>
              <form method="post" action="index.php" id="sendMessageForm">
                <div class="inputFields">
                  <input type="text" name="address" placeholder="Address" 
                      value="<?php echo isset($_SESSION['address']) ? htmlspecialchars($_SESSION['address']) : ""; ?>" />
                  <label>Group: 
                    <input type="checkbox" name="groupCheckBox" 
                        <?php if(isset($_SESSION['groupCheckBox']) && $_SESSION['groupCheckBox']) echo "checked"; ?> />
                  </label>
                  <label>
                    Message: <i>max 200 characters are allowed</i>
                    <input type="text" name="message" placeholder="ATT IMMN sample message"
                        value="<?php echo isset($_SESSION['message']) ? htmlspecialchars($_SESSION['message']) : ""; ?>" maxlength="200">
                  </label>
                  <label>
                    Subject: <i>max 30 characters are allowed</i>
                    <input type="text" name="subject" placeholder="ATT IMMN APP" 
                        value="<?php echo isset($_SESSION['subject']) ? htmlspecialchars($_SESSION['subject']) : ""; ?>" maxlength="30">
                  </label>
                  <label>
                    Attachment:
                    <select name="attachment">
                      <?php
                        $attachments = $results[C_ATTACHMENTS];
                        foreach($attachments as $attachment) {
                      ?>
                          <?php if (isset($_SESSION['attachment']) && $_SESSION['attachment'] == $attachment) { ?>
                              <option value="<?php echo $attachment; ?>" selected><?php echo $attachment; ?></option>
                          <?php } else { ?>
                              <option value="<?php echo $attachment; ?>"><?php echo $attachment; ?></option>
                          <?php } ?>
                      <?php } ?>
                    </select>
                  </label>
                  <button name="sendMessage" type="submit" class="submit">Send Message</button>
                </div>
              </form>
              <?php if (isset($results[C_SEND_MSG])) { ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <?php echo htmlspecialchars($results[C_SEND_MSG]); ?>
                </div>
              <?php } ?>
              <?php if (isset($errors[C_SEND_MSG])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_SEND_MSG]); ?>
                </div>
              <?php } ?>
            </div> <!-- end of send message -->

            <div class="lightBorder"></div>
            <label>
              <b>Note:</b> In order to use the following features, you must be subscribed to 
              <a href="http://messages.att.net">AT&amp;T Messages</a>
            </label>
            <div class="lightBorder"></div>

            <a id="createMsgToggle" 
              href="javascript:toggle('createMsg');">Create Message Index</a>
            <div class="toggle" id="createMsg">
              <h2>Create Message Index</h2>
              <form method="post" action="index.php" id="createMessageIndexForm">
                <div class="inputFields">
                  <button name="createMessageIndex" type="submit" class="submit">Create Index</button>
                </div>
              </form>
              <?php if (isset($results[C_CREATE_MSG_INDEX])) { ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
              <?php } ?>
              <?php if (isset($errors[C_CREATE_MSG_INDEX])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_CREATE_MSG_INDEX]); ?>
                </div>
              <?php } ?>
            </div> <!-- end of create message index -->

            <div class="lightBorder"></div>

            <a id="getMsgToggle" 
              href="javascript:toggle('getMsg');">Get Message</a>
            <div class="toggle" id="getMsg">
              <h2>Get Message List <i>(Displays last 5 messages from the list)</i></h2>
              <form method="post" action="index.php" id="getMessageListForm">
                <div class="inputFields">
                  <label><input name="favorite" type="checkbox" 
                    <?php echo isset($_SESSION['favorite']) ? 'checked' : '' ?>>Filter by favorite</label>
                  <label><input name="unread" type="checkbox"
                    <?php echo isset($_SESSION['unread']) ? 'checked' : '' ?>>Filter by unread flag</label>
                  <label><input name="incoming" type="checkbox" 
                    <?php echo isset($_SESSION['incoming']) ? 'checked' : '' ?>>Filter by incoming flag</label>
                  <label>Filter by recipients:
                    <input name="keyword" type="text" placeholder="555-555-5555, etc..." 
                      value="<?php echo htmlspecialchars(isset($_SESSION['keyword']) ? $_SESSION['keyword'] : ''); ?>">
                  </label>
                  <button name="getMessageList" type="submit" class="submit">Get Message List</button>
                </div>
              </form>
              <?php 
              if (isset($results[C_GET_MSG_LIST])) { 
                $msgList = $results[C_GET_MSG_LIST];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>limit</th>
                      <th>offset</th>
                      <th>total</th>
                      <th>state</th>
                      <th>cache status</th>
                      <th>failed messages</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td data-value="limit"><?php echo htmlspecialchars($msgList->getLimit()); ?></td>
                      <td data-value="offset"><?php echo htmlspecialchars($msgList->getOffset()); ?></td>
                      <td data-value="total"><?php echo htmlspecialchars($msgList->getTotal()); ?></td>
                      <td data-value="state"><?php echo htmlspecialchars($msgList->getState()); ?></td>
                      <td data-value="cache status"><?php echo htmlspecialchars($msgList->getCacheStatus()); ?></td>
                      <?php 
                      if ($msgList->getFailedMessages() != null) { 
                        $failedMsgs = htmlspecialchars(join(', ', $msgList->getFailedMessages()));
                      ?>
                        <td data-value="failed messages"><?php echo $failedMsgs; ?></td>
                      <?php } else { ?>
                        <td data-value="failed messages">None</td>
                      <?php } ?>
                    </tr>
                  </tbody>
                </table>
                <div id="listMsg">
                <?php
                  $count = 0;
                  $msgs = $msgList->getMessages();
                  foreach($msgs as $msg) {
                ?>
                    <h3>Message <?php echo ++$count; ?></h3>
                    <table>
                      <thead>
                        <tr>
                          <th>message id</th>
                          <th>from</th>
                          <th>recipients</th>
                          <th>text</th>
                          <?php if ($msg->getTypeMetaData()->getSubject() != null) { ?>
                            <th>subject</th>
                          <?php } ?>
                          <th>timestamp</th>
                          <th>isFavorite</th>
                          <th>isUnread</th>
                          <th>isIncoming</th>
                          <th>type</th>
                          <?php if ($msg->getTypeMetaData()->isSegmented()) { ?>
                            <th>segmentation reference number</th>
                            <th>segmentation part</th>
                            <th>segmentation total parts</th>
                          <?php } ?>
                        </tr>
                      </thead>
                      <tbody>
                        <tr>
                          <td data-value="message id"><?php echo htmlspecialchars($msg->getMessageId()); ?></td>
                          <td data-value="from"><?php echo htmlspecialchars($msg->getFrom()); ?></td>
                          <td data-value="recipients"><?php echo htmlspecialchars(join(', ', $msg->getRecipients())); ?></td>
                          <td data-value="text"><?php echo htmlspecialchars($msg->getText() != '' ? $msg->getText() : '-'); ?></td>
                          <?php if ($msg->getTypeMetaData()->getSubject() != null) { ?>
                            <td data-value="subject"><?php echo htmlspecialchars($msg->getTypeMetaData()->getSubject()); ?></td>
                          <?php } ?>
                          <td data-value="timestamp"><?php echo htmlspecialchars($msg->getTimeStamp()); ?></td>
                          <td data-value="isFavorite"><?php echo htmlspecialchars($msg->isFavorite() ? 'true' : 'false'); ?></td>
                          <td data-value="isUnread"><?php echo htmlspecialchars($msg->isUnread() ? 'true' : 'false'); ?></td>
                          <td data-value="isIncoming"><?php echo htmlspecialchars($msg->isIncoming() ? 'true' : 'false'); ?></td>
                          <td data-value="type"><?php echo htmlspecialchars($msg->getMessageType()); ?></td>
                          <?php 
                            if ($msg->getTypeMetaData()->isSegmented()) { 
                              $segDetails = $msg->getTypeMetaData()->getSegmentationDetails();
                          ?>
                            <td data-value="segmentation reference number"><?php echo htmlspecialchars($segDetails->getSegmentationMsgRefNumber()); ?></td>
                            <td data-value="segmentation part"><?php echo htmlspecialchars($segDetails->getThisPartNumber()); ?></td>
                            <td data-value="segmentation total parts"><?php echo htmlspecialchars($segDetails->getTotalNumberOfParts()); ?></td>
                          <?php } ?>
                        </tr>
                      </tbody>
                    </table>
                  <?php } ?>
                </div>
              <?php } ?>
              <?php if (isset($errors[C_GET_MSG_LIST])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_GET_MSG_LIST]); ?>
                </div>
              <?php } ?>

              <h2>Get Message</h2>
              <form method="post" action="index.php" id="getMessageForm">
                <div class="inputFields">
                  <input name="messageId" type="text" maxlength="30" placeholder="Message ID"
                      value="<?php echo htmlspecialchars(isset($_SESSION['messageId']) ? $_SESSION['messageId'] : ''); ?>" />
                  <button name="getMessage" type="submit" class="submit">
                    Get Message
                  </button>
                </div>
              </form>
              <?php 
              if (isset($results[C_GET_MSG])) { 
                $msg = $results[C_GET_MSG];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <table>
                  <thead>
                    <th>message id</th>
                    <th>from</th>
                    <th>recipients</th>
                    <th>text</th>
                     <?php if ($msg->getTypeMetaData()->getSubject() != null) { ?>
                       <th>subject</th>
                     <?php } ?>
                    <th>timestamp</th>
                    <th>isFavorite</th>
                    <th>isUnread</th>
                    <th>isIncoming</th>
                    <th>type</th>
                    <?php if ($msg->getTypeMetaData()->isSegmented()) { ?>
                      <th>segmentation reference number</th>
                      <th>segmentation part</th>
                      <th>segmentation total parts</th>
                    <?php } ?>
                  </thead>
                  <tbody>
                     <td data-value="message id"><?php echo htmlspecialchars($msg->getMessageId()); ?></td>
                     <td data-value="from"><?php echo htmlspecialchars($msg->getFrom()); ?></td>
                     <td data-value="recipients"><?php echo htmlspecialchars(join(', ', $msg->getRecipients())); ?></td>
                     <td data-value="text"><?php echo htmlspecialchars($msg->getText() != '' ? $msg->getText() : '-'); ?></td>
                     <?php if ($msg->getTypeMetaData()->getSubject() != null) { ?>
                       <td data-value="subject"><?php echo htmlspecialchars($msg->getTypeMetaData()->getSubject()); ?></td>
                     <?php } ?>
                     <td data-value="timestamp"><?php echo htmlspecialchars($msg->getTimeStamp()); ?></td>
                     <td data-value="isFavorite"><?php echo htmlspecialchars($msg->isFavorite() ? 'true' : 'false'); ?></td>
                     <td data-value="isUnread"><?php echo htmlspecialchars($msg->isUnread() ? 'true' : 'false'); ?></td>
                     <td data-value="isIncoming"><?php echo htmlspecialchars($msg->isIncoming() ? 'true' : 'false'); ?></td>
                     <td data-value="type"><?php echo htmlspecialchars($msg->getMessageType()); ?></td>
                     <?php 
                       if ($msg->getTypeMetaData()->isSegmented()) { 
                         $segDetails = $msg->getTypeMetaData()->getSegmentationDetails();
                     ?>
                       <td data-value="segmentation reference number"><?php echo htmlspecialchars($segDetails->getSegmentationMsgRefNumber()); ?></td>
                       <td data-value="segmentation part"><?php echo htmlspecialchars($segDetails->getThisPartNumber()); ?></td>
                       <td data-value="segmentation total parts"><?php echo htmlspecialchars($segDetails->getTotalNumberOfParts()); ?></td>
                     <?php } ?>
                  </tbody>
                </table>
              <?php } ?>
              <?php if (isset($errors[C_GET_MSG])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_GET_MSG]); ?>
                </div>
              <?php } ?>

              <?php
                $sessionMsgId = htmlspecialchars(isset($_SESSION['messageId']) ? $_SESSION['messageId'] : '');
                $sessionPartNumber = htmlspecialchars(isset($_SESSION['partNumber']) ? $_SESSION['partNumber'] : '');
              ?>
              <h2>Get Message Content</h2>
              <form method="post" action="index.php" id="getMessageContentForm">
                <div class="inputFields">
                  <input name="messageId" type="text" maxlength="30" placeholder="Message ID" 
                    value="<?php echo $sessionMsgId; ?>" />     
                  <input name="partNumber" type="text" maxlength="30" placeholder="Part Number" 
                    value="<?php echo $sessionPartNumber; ?>" />     
                  <button  type="submit" class="submit" name="getMessageContent">
                    Get Message Content
                  </button>
                </div>
              </form>
              <?php 
                if (isset($results[C_GET_MSG_CONTENT])) { 
                  $msgContent = $results[C_GET_MSG_CONTENT];
                  $splitType = explode('/', $msgContent->getContentType());
                  $ctype = $splitType[0];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <?php if ($ctype == 'text') { ?>
                  <?php echo htmlspecialchars($msgContent->getContent()); ?>
                <?php } else if ($ctype == 'image') { ?>
                  <img src="data:<?php echo $msgContent->getContentType(); ?>;base64,<?php echo base64_encode($msgContent->getContent()); ?>" />
                <?php } else if ($ctype == 'video') { ?>
                  <video controls="controls" autobuffer="autobuffer" autoplay="autoplay">
                    <source src="data:<?php echo $msgContent->getContentType(); ?>;base64,<?php echo base64_encode($msgContent->getContent()); ?>" />
                  </video>
                <?php } else if ($ctype == 'audio') { ?>
                  <audio controls="controls" autobuffer="autobuffer" autoplay="autoplay">
                    <source src="data:<?php echo $msgContent->getContentType(); ?>;base64,<?php echo base64_encode($msgContent->getContent()); ?>" />
                  </audio>
                <?php } ?>
              <?php } ?>
              <?php if (isset($errors[C_GET_MSG_CONTENT])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_GET_MSG_CONTENT]); ?>
                </div>
              <?php } ?>

              <h2>Get Delta</h2>
              <form method="post" action="index.php" id="getDeltaForm">
                <div class="inputFields">
                  <?php
                    $sessionState = htmlspecialchars(isset($_SESSION['state']) ? $_SESSION['state'] : '');
                  ?>
                  <input name="state" type="text" maxlength="30" placeholder="Message State" 
                    value="<?php echo $sessionState; ?>" />
                  <button  type="submit" class="submit" name="getDelta">
                    Get Delta
                  </button>
                </div>
              </form>
              <?php 
                if (isset($results[C_GET_DELTA])) { 
                  $deltaResponse = $results[C_GET_DELTA];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <?php foreach($deltaResponse->getDeltas() as $delta) { ?>
                  <p><b>Delta type:</b> <?php echo $delta->getDeltaType(); ?></p>
                  <table>
                    <thead>
                      <tr>
                        <th>Delta Operation</th>
                        <th>MessageId</th>
                        <th>Favorite</th>
                        <th>Unread</th>
                      </tr>
                    </thead>
                    <tbody>
                      <?php foreach($delta->getAdds() as $add) { ?>
                        <tr>
                          <td data-value="Delta Operation">Add</td>
                          <td data-value="MessageId"><?php echo $add->getMessageId(); ?></td>
                          <td data-value="Favorite"><?php echo $add->isFavorite() ? 'true' : 'false'; ?></td>
                          <td data-value="Unread"><?php echo $add->isUnread() ? 'true' : 'false'; ?></td>
                        </tr>
                      <?php } ?>
                      <?php foreach($delta->getDeletes() as $delete) { ?>
                        <tr>
                          <td data-value="Delta Operation">Delete</td>
                          <td data-value="MessageId"><?php echo $delete->getMessageId(); ?></td>
                          <td data-value="Favorite"><?php echo $delete->isFavorite() ? 'true' : 'false'; ?></td>
                          <td data-value="Unread"><?php echo $delete->isUnread() ? 'true' : 'false'; ?></td>
                        </tr>
                      <?php } ?>
                      <?php foreach($delta->getDeletes() as $update) { ?>
                        <tr>
                          <td data-value="Delta Operation">Update</td>
                          <td data-value="MessageId"><?php echo $update->getMessageId(); ?></td>
                          <td data-value="Favorite"><?php echo $update->isFavorite() ? 'true' : 'false'; ?></td>
                          <td data-value="Unread"><?php echo $update->isUnread() ? 'true' : 'false'; ?></td>
                        </tr>
                      <?php } ?>
                    </tbody>
                  </table>
                <?php } ?>
              <?php } ?>
              <?php if (isset($errors[C_GET_DELTA])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_GET_DELTA]); ?>
                </div>
              <?php } ?>

              <h2> Get Message Index Info</h2>
              <form method="post" action="index.php" id="getMessageIndexInfoForm">
                <div class="inputFields">
                  <button name="getMessageIndexInfo" type="submit" class="submit">
                    Get Message Index Info
                  </button>
                </div>
              </form>
              <?php 
                if (isset($results[C_GET_MSG_INDEX_INFO])) { 
                  $indexInfo = $results[C_GET_MSG_INDEX_INFO];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>Status</th>
                      <th>State</th>
                      <th>Message Count</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td data-value="Status"><?php echo htmlspecialchars($indexInfo->getStatus()); ?></td>
                      <td data-value="State"><?php echo htmlspecialchars($indexInfo->getState()); ?></td>
                      <td data-value="Message Count"><?php echo htmlspecialchars($indexInfo->getMessageCount()); ?></td>
                    </tr>
                  </tbody>
                </table>
              <?php } ?>
              <?php if (isset($errors[C_GET_MSG_INDEX_INFO])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_GET_MSG_INDEX_INFO]); ?>
                </div>
              <?php } ?>

            </div> <!-- end of get message toggle -->

            <div class="lightBorder"></div>
            <a id="updateMsgToggle" 
              href="javascript:toggle('updateMsg');">Update Message</a>
            <div class="toggle" id="updateMsg">
              <h2>Update Message/Messages</h2>
              <form method="post" action="index.php" id="updateMessageForm">
                <div class="inputFields">
                  <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                  <?php
                    $sessionMsgId = htmlspecialchars(isset($_SESSION['messageId']) ? $_SESSION['messageId'] : '');
                  ?>
                  <input name="messageId" type="text" maxlength="30" placeholder="Message ID" 
                      value="<?php echo $sessionMsgId; ?>" />
                  <label>Change Status:</label>
                  <?php
                    $rflag = isset($_SESSION['readflag']) ? $_SESSION['readflag'] : 'read';
                  ?>
                  <label>
                    <input type="radio" name="readflag" value="read" <?php echo $rflag == 'read' ? 'checked' : ''; ?>>
                    Read
                  </label>
                  <label>
                    <input type="radio" name="readflag" value="unread" <?php echo $rflag == 'unread' ? 'checked' : ''?>>
                    Unread
                  </label>
                  <button name="updateMessage" type="submit" class="submit">Update Message/Messages</button>
                </div>
              </form>

              <?php if (isset($results[C_UPDATE_MSGS])) { ?> 
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
              <?php } ?>
              <?php if (isset($errors[C_UPDATE_MSGS])) { ?> 
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_UPDATE_MSGS]); ?>
                </div>
              <?php } ?>
            </div> <!-- end of update message toggle -->

            <div class="lightBorder"></div>
            <a id="delMsgToggle" href="javascript:toggle('delMsg');">Delete Message</a>
            <div class="toggle" id="delMsg">
              <h2>Delete Message/Messages</h2>
              <form method="post" action="index.php" id="deleteMessageForm">
                <div class="inputFields">
                  <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                  <?php
                    $sessionMsgId = htmlspecialchars(isset($_SESSION['messageId']) ? $_SESSION['messageId'] : '');
                  ?>
                  <input name="messageId" type="text" maxlength="30" placeholder="Message ID" 
                    value="<?php echo $sessionMsgId; ?>" />
                  <button name="deleteMessage" type="submit" class="submit">Delete Message/Messages</button>
                </div>
              </form>

              <?php if (isset($results[C_DELETE_MSGS])) { ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
              <?php } ?>
              <?php if (isset($errors[C_DELETE_MSGS])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_DELETE_MSGS]); ?>
                </div>
              <?php } ?>
            </div> <!-- end of delete toggle -->

            <div class="lightBorder"></div>

            <a id="getMsgNotToggle" 
              href="javascript:toggle('getMsgNot');">Get Notification Connection Details</a>
            <div class="toggle" id="getMsgNot">
              <form method="post" action="index.php" id="getNotifyDetailsForm">
                <div class="inputFields">
                  <label>Notification Subscription:</label>
                  <?php
                    $sessionQueues = isset($_SESSION['queues']) ? $_SESSION['queues'] : 'text'; 
                  ?>
                  <label><input type="radio" name="queues" value="text"
                    <?php echo $sessionQueues == 'text' ? 'checked' : '' ?>>Text</label>
                  <label><input type="radio" name="queues" value="mms" 
                    <?php echo $sessionQueues == 'mms' ? 'checked' : '' ?>>MMS</label>
                  <button name="getNotifyDetails" type="submit" class="submit">Get Details</button>
                </div>
              </form>
              <?php 
                if (isset($results[C_NOTIFICATION_DETAILS])) { 
                  $notifDetails = $results[C_NOTIFICATION_DETAILS];
              ?>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                </div>
                <h3>Connection Details</h3>
                <table>
                  <thead>
                    <tr>
                      <th>Username</th>
                      <th>Password</th>
                      <th>https url</th>
                      <th>wss url</th>
                      <th>queues</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td data-value="Username"><?php echo htmlspecialchars($notifDetails->getUsername()); ?></td>
                      <td data-value="Password"><?php echo htmlspecialchars($notifDetails->getPassword()); ?></td>
                      <td data-value="https url"><?php echo htmlspecialchars($notifDetails->getHttpsUrl()); ?></td>
                      <td data-value="wss url"><?php echo htmlspecialchars($notifDetails->getWssUrl()); ?></td>
                      <td data-value="queues"><?php echo htmlspecialchars(implode(',', $notifDetails->getQueues())); ?></td>
                    </tr>
                  </tbody>
                </table>
              <?php } ?>
              <?php if (isset($errors[C_NOTIFICATION_DETAILS])) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($errors[C_NOTIFICATION_DETAILS]); ?>
                </div>
              <?php } ?>
            </div> <!-- end of getMsgNot toggle -->
          </div> <!-- end of form container -->
        </div> <!-- end of form box -->
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
        <a href="https://devconnect-api.att.com/" 
          target="_blank">https://devconnect-api.att.com</a>
        <br> For more information contact 
        <a href="mailto:developer.support@att.com">developer.support@att.com</a>
        <br><br>
        &#169; 2014 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <script>
      <?php 
        /* handle toggling of divs */ 

        // map results and errors to div toggles
        $resultsArr = array(
          C_SEND_MSG => 'sendMsg',
          C_CREATE_MSG_INDEX => 'createMsg',
          C_GET_MSG_LIST => 'getMsg',
          C_GET_MSG => 'getMsg',
          C_GET_MSG_CONTENT => 'getMsg',
          C_GET_DELTA => 'getMsg',
          C_GET_MSG_INDEX_INFO => 'getMsg',
          C_UPDATE_MSGS => 'updateMsg',
          C_DELETE_MSGS => 'delMsg',
          C_NOTIFICATION_DETAILS => 'getMsgNot'
        );

        foreach ($resultsArr as $k => $v) {
          if (isset($results[$k]) || isset($errors[$k])) {
            echo "toggle('$v')";
          }
        }
      ?>
    </script>
  </body>
</html>

