<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/lib/Notary/Notary.php';
require_once __DIR__ . '/lib/Util/Util.php';
require_once __DIR__ . '/src/Controller/PaymentController.php';

use Att\Api\Util\Util;

$controller = new PaymentController();
$controller->handleRequest();
$results = $controller->getResults();
$errors = $controller->getErrors();

?>
<!DOCTYPE html>
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Payment</title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
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
        </div> 
        <ul class="links" id="nav">
          <li>
          <a href="<?php echo $linkSource; ?>" 
              target="_blank">Source<img src="images/opensource.png" alt="opensource.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkDownload; ?>" 
              target="_blank">Download<img src="images/download.png" alt="download.png"></a>
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
          <h1>AT&amp;T Sample Application - Payment</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:</b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="transactionToggle" 
              href="javascript:toggle('transaction','transactionToggle', 'Transaction');">Show Transaction</a>
            <div class="toggle" id="transaction">
              <h2>Feature 1: Create New Transaction</h2>
              <form method="post" name="newTransaction" action="index.php">
                <div class="inputFields">
                  <input type="radio" name="product" value="1" 
                      checked="checked">Buy product 1 for $<?php echo $minTransactionValue; ?><br>
                  <input type="radio" name="product" 
                      value="2">Buy product 2 for $<?php echo $maxTransactionValue; ?><br>
                  <button type="submit" name="newTransaction">Buy Product</button>
                </div> <!-- end of inputFields -->
              </form> <!-- end of newTransaction -->
              <?php if(isset($errors['newTrans'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['newTrans']); ?>
              </div>
              <?php } ?>
              <br>
              <h2>Feature 2: Get Transaction Status</h2>
              <div class="inputFields">
                <strong>Merchant Transaction ID</strong><br>
                <form method="post" name="fgetTransactionTID" action="index.php">
                  <select name="getTransactionMTID" onChange="this.form.submit()">
                    <?php foreach ($results['t_merchantTransIds'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
                <strong>Auth Code</strong><br>
                <form method="post" name="fgetTransactionAuthCode" action="index.php">
                  <select name="getTransactionAuthCode" onChange="this.form.submit()">
                    <?php foreach ($results['t_authCodes'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
                <strong>Transaction ID</strong><br>
                <form method="post" name="fgetTransactionTID" action="index.php">
                  <select name="getTransactionTID" onChange="this.form.submit()">
                    <?php foreach ($results['t_transIds'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['transInfo'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['transInfo']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['transInfo'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Transaction Status Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $transInfo = $results['transInfo'];
                foreach ($transInfo as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
              <br>
              <h2>Feature 3: Refund Transaction</h2>
              <div class="inputFields">
                <div id="refundTransIds">
                  <strong>Transaction ID</strong><br>
                  <form method="post" name="refundTransactionTID" action="index.php">
                    <select name="refundTransactionId" onChange="this.form.submit()">
                      <?php foreach ($results['t_transIds'] as $id) { ?>
                      <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                      <?php } ?>
                    </select>
                  </form>
                </div> <!-- end of refundTransIds -->
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['t_refund'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['t_refund']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['t_refund'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Refund Status Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $rinfo = $results['t_refund'];
                foreach ($rinfo as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
            </div> <!-- end of transaction -->
            <div class="lightBorder"></div>
            <a id="subscriptionToggle" 
                href="javascript:toggle('subscription','subscriptionToggle', 'Subscription');">Show Subscription</a>
            <div class="toggle" id="subscription">
              <div class="note">Note: You must refund/cancel any previous subscription to subscribe again using the same phone number.</div>
              <h2>Feature 1: Create New Subscription</h2>
              <form method="post" name="newSubscription" action="index.php">
                <div class="inputFields">
                  <input type="radio" name="product" value="1" 
                      checked="checked" />Subscribe for $<?php echo $minSubscriptionValue; ?> per month
                  <br>
                  <input type="radio" name="product" 
                      value="2" />Subscribe for $<?php echo $maxSubscriptionValue; ?> per month
                  <br>
                  <button type="submit" name="newSubscription">Subscribe</button>
                </div> <!-- end of inputFields -->
              </form> <!-- end of newSubscription -->
              <br>
              <h2>Feature 2: Get Subscription Status</h2>
              <div class="inputFields">
                <strong>Merchant Transaction ID</strong><br>
                <form method="post" name="fgetSubscriptionTID" action="index.php">
                  <select name="getSubscriptionMTID" onChange="this.form.submit()">
                    <?php foreach ($results['s_merchantTransIds'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
                <strong>Auth Code</strong><br>
                <form method="post" name="fgetSubscriptionAuthCode" action="index.php">
                  <select name="getSubscriptionAuthCode" onChange="this.form.submit()">
                    <?php foreach ($results['s_authCodes'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
                <strong>Subscription ID</strong><br>
                <form method="post" name="fgetSubscriptionTID" action="index.php">
                  <select name="getSubscriptionTID" onChange="this.form.submit()">
                    <?php foreach ($results['s_subIds'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['subInfo'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['subInfo']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['subInfo'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Subscription Status Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $subInfo = $results['subInfo'];
                foreach ($subInfo as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
              <br>
              <h2>Feature 3: Get Subscription Details</h2>
              <div class="inputFields">
                <strong>Merchant Subscription ID</strong><br>
                <form method="post" name="fgetSubscriptionTID" action="index.php">
                  <select name="getSDetailsMSID" onChange="this.form.submit()">
                    <?php foreach ($results['s_merchantSubIds'] as $id) { ?>
                    <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                    <?php } ?>
                  </select>
                </form>
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['subDetails'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['subDetails']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['subDetails'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Subscription Details Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $subDetails = $results['subDetails'];
                foreach ($subDetails as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
              <br>
              <h2>Feature 4: Cancel Future Subscription</h2>
              <div class="inputFields">
                <div id="cancelIds">
                  <strong>Subscription ID</strong><br>
                  <form method="post" name="cancelSubscriptionID" action="index.php">
                    <select name="cancelSubscriptionId" onChange="this.form.submit()">
                      <?php foreach ($results['s_subIds'] as $id) { ?>
                      <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                      <?php } ?>
                    </select>
                  </form>
                </div> <!-- end of cancelIds-->
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['s_cancel'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['s_cancel']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['s_cancel'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Refund Status Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $rinfo = $results['s_cancel'];
                foreach ($rinfo as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
              <h2>Feature 5: Refund Current and Cancel Future Subscription</h2>
              <div class="inputFields">
                <div id="refundSubIds">
                  <strong>Subscription ID</strong><br>
                  <form method="post" name="refundSubscriptionID" action="index.php">
                    <select name="refundSubscriptionId" onChange="this.form.submit()">
                      <?php foreach ($results['s_subIds'] as $id) { ?>
                      <option value="<?php echo $id; ?>"><?php echo $id; ?></option>
                      <?php } ?>
                    </select>
                  </form>
                </div> <!-- end of refundSubIds -->
              </div> <!-- end of inputFields -->
              <?php if(isset($errors['s_refund'])) { ?> 
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['s_refund']); ?>
              </div>
              <?php } ?>
              <?php if (isset($results['s_refund'])) { ?>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br>
                Refund Status Listed Below:
              </div>
              <table class="kvp" id="kvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                <?php
                $rinfo = $results['s_refund'];
                foreach ($rinfo as $name => $value) {
                ?>
                  <tr>
                    <td data-value="Parameter">
                      <?php echo htmlspecialchars($name); ?>
                    </td>
                    <td data-value="Value">
                      <?php if($value == '') { $value = '-'; } echo htmlspecialchars($value); ?>
                    </td>
                  </tr>
                <?php } ?>
                </tbody>
              </table>
              <?php } ?>
            </div> <!-- end of subscription -->
            <div class="lightBorder"></div>
            <a id="notaryToggle" href="javascript:toggle('notary','notaryToggle', 'Notary');">Show Notary</a>
            <div class="toggle" id="notary">
              <h2>Feature 1: Sign Payload</h2>
              <form method="post" name="signContent" action="index.php">
                <div class="inputFields">
                  <label>Request:
                    <?php if (isset($results['notary'])) { ?> 
                    <textarea id="payload" name="payload" placeholder="Payload" 
                        ><?php $notary = $results['notary']; echo $notary->getPayload(); ?></textarea>
                    <?php } else { ?>
                    <textarea id="payload" name="payload" placeholder="Payload"></textarea>
                    <?php } ?>
                  </label>
                  <div id="notaryInfo">
                    <strong>Signed Payload:</strong><br>
                    <?php if (isset($results['notary'])) { 
                      $notary = $results['notary']; echo htmlspecialchars($notary->getSignedDocument()); } 
                    ?>
                    <br>
                    <strong>Signature:</strong><br>
                    <?php if (isset($results['notary'])) { 
                      $notary = $results['notary']; echo htmlspecialchars($notary->getSignature()); } 
                    ?>
                    <br>
                    <button type="submit" name="signPayload" value="signPayload">Sign Payload</button>
                  </div> <!-- end of notaryInfo -->
                </div> <!-- end of inputFields -->
              </form> <!-- end of signContent -->
              <?php if (isset($errors['notary'])) { ?>
              <div class="errorWide">
                <strong>ERROR: </strong><br>
                <?php echo htmlspecialchars($errors['notary']); ?>
              </div>
              <?php } ?>
            </div> <!-- end of notary -->
            <div class="lightBorder"></div>
            <a id="notificationToggle" 
                href="javascript:toggle('notifications','notificationToggle', 'Notifications');">Show Notifications</a>
            <div class="toggle" id="notifications">
              <div class="inputFields">
                <div id="notificationDetails" class="columns">
                  <?php
                  $notifications = $results['notifications'];
                  for ($i = 0; $i < count($notifications); ++$i) {
                  $notification = $notifications[$i];
                  $notifInfo = $i == 0 ? ' [ Displays last 5 notifications ]' : '';
                  ?>
                    <h2>Notification : <?php echo(($i + 1) . $notifInfo); ?></h2>
                    <table>
                      <thead>
                        <tr>
                          <?php 
                          foreach ($notification as $k => $v) { 
                            $k = htmlspecialchars($k); 
                          ?>
                            <th><?php echo $k; ?></th>
                          <?php } ?>
                        </tr>
                      </thead>
                      <tbody>
                        <tr>
                          <?php 
                          foreach ($notification as $k => $v) { 
                            $v = $v == '' ? '-' : $v;
                            $v = htmlspecialchars($v);
                            $k = htmlspecialchars($k);
                          ?>
                            <td data-value="<?php echo $k; ?>"><?php echo $v; ?></td>
                          <?php } ?>
                        </tr>
                      </tbody>
                    </table>
                  <?php } ?>
                  <form method="post" name="refreshNotifications" action="index.php">
                    <button type="submit" name="refreshNotifications">Refresh</button>
                  </form> <!-- end of refreshNotifications -->
                </div> <!-- end of notificationDetails -->
              </div> <!-- end of inputFields -->
            </div> <!-- end of notifications -->
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
    <?php if ($controller->showTransaction()) { ?>
      <script>toggle('transaction', 'transactionToggle', 'Transaction');</script>
    <?php } ?>
    <?php if ($controller->showSubscription()) { ?>
      <script>toggle('subscription', 'subscriptionToggle', 'Subscription');</script>
    <?php } ?>
    <?php if ($controller->showNotary()) { ?>
      <script>toggle('notary', 'notaryToggle', 'Notary');</script>
    <?php } ?>
    <?php if ($controller->showNotifications()) { ?>
      <script>toggle('notifications', 'notificationToggle', 'Notifications');</script>
    <?php } ?>
  </body>
</html>
