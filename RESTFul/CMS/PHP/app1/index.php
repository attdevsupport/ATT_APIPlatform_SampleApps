<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Sample/CMSService.php';
require_once __DIR__ . '/lib/Util/Util.php';
$service = new CMSService();
$createSession = $service->createSession();
$signalStatus = $service->sendSignal();
$sendSignalStatus = null;
?>
<!DOCTYPE html>
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Call Management</title>		
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
          <h1>AT&amp;T Sample Application - Call Management</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time: </b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:</b> <script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMessages">
              <h2>Feature 1: Outbound Session from <?php echo $number; ?></h2>
              <form method="post" action="index.php" id="msgContentForm" name="msgContentForm">
                <div class="inputFields">

                  <label>
                    Make call to:
                    <?php if (isset($_SESSION['txtNumberToDial'])) { ?>
                    <input type="text" name="txtNumberToDial" 
                        value="<?php echo htmlspecialchars($_SESSION['txtNumberToDial']); ?>" placeholder="Address" 
                        title="telephone number or sip address" />
                    <?php } else { ?>
                    <input type="text" name="txtNumberToDial" value="" placeholder="Address" 
                        title="telephone number or sip address" />
                    <?php } ?>
                  </label>
                  <label>
                    Script Function:
                    <select name="scriptType">
                      <?php foreach ($scriptFunctions as $sfunc) { ?>
                        <?php if (isset($_SESSION['scriptType']) && $_SESSION['scriptType'] == $sfunc) { ?>
                        <option value="<?php echo $sfunc; ?>" selected="selected"><?php echo $sfunc ?></option>
                        <?php } else { ?>
                        <option value="<?php echo $sfunc; ?>"><?php echo $sfunc ?></option>
                        <?php } ?>
                      <?php } ?>
                    </select>
                  </label>

                  <label>
                    Number parameter for Script Function:
                    <?php if (isset($_SESSION['txtNumber'])) { ?>
                    <input type="text" name="txtNumber" 
                        value="<?php echo htmlspecialchars($_SESSION['txtNumber']); ?>" placeholder="Number" 
                        title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from"/>
                    <?php } else { ?>
                    <input type="text" name="txtNumber" 
                        value="" placeholder="Number" 
                        title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from"/>
                    <?php } ?>
                  </label>

                  <label>
                    Message To Play:
                    <?php if (isset($_SESSION['txtNumber'])) { ?>
                    <input type="text" name="txtMessageToPlay" 
                      value="<?php echo htmlspecialchars($_SESSION['txtMessageToPlay']); ?>" 
                      placeholder="Message" title="enter long message or mp3 audio url, this is used as music on hold"/>
                    <?php } else { ?>
                    <input type="text" name="txtMessageToPlay" 
                      value="" placeholder="Message" title="enter long message or mp3 audio url, this is used as music on hold"/>
                    <?php } ?>
                  </label>

                  <div id="scriptText">
                  <label>
                    Script Source Code:
                  </label>
                    <textarea name="txtCreateSession" rows="2" cols="20" disabled="disabled" id="txtCreateSession" >
                      <?php echo htmlspecialchars($service->getScriptContents()); ?>
                    </textarea>
                </div>

                  <div>
                    <button type="submit" class="submit" name="btnCreateSession" >
                      Create Session</button>
                  </div>
                </div>
              </form>
              <?php if ($createSession != NULL) { ?>
              <div class="successWide">
                <strong>SUCCESS</strong><br>
                id:&nbsp;<?php echo $createSession['id']; ?><br>
                success:&nbsp;<?php echo ($createSession['success'] ? 'True' : 'False'); ?><br>
              </div>
              <?php } else if ($service->getCreateSessionError() != NULL) { ?>
              <div class="errorWide">
                <strong>ERROR:</strong>
                <?php echo htmlspecialchars($service->getCreateSessionError()); ?>
              </div>
              <?php } ?>
            </div> <!-- end of Create Session -->

            <div class="lightBorder"></div>

            <div id="sendSignal">
              <h2>Feature 2: Send Signal to Session</h2>
              <form method="post" name="sendSignal" action="index.php">
                <div class="inputFields">
                  <label class="label">
                    Session ID: <?php echo $service->getSessionId(); ?>
                  </label>

                  <label class="label">
                    Signal to Send:
                    <select name="signal"> 
                      <?php $signals = array('exit', 'stopHold', 'dequeue');
                        foreach ($signals as $signal) { 
                          if (isset($_SESSION['signal']) && $signal == $_SESSION['signal']) {
                            ?>
                            <option value="<?php echo $signal; ?>" selected="selected"><?php echo $signal; ?></option>
                      <?php } else { ?>
                            <option value="<?php echo $signal; ?>"><?php echo $signal; ?></option>
                      <?php } ?>
                      <?php } /* end of foreach */ ?>
                    </select>
                  </label>

                  <div>
                    <button type="submit" class="submit" name="btnSendSignal">
                      Send Signal
                    </button>
                  </div>

                </div>	
              </form>
              <?php if ($signalStatus != NULL) { ?>
                <div class="successWide">
                  <strong>SUCCESS</strong><br />
                  <strong>Status:&nbsp;</strong><?php echo htmlspecialchars(json_encode($signalStatus)); ?>
                </div>
              <?php } ?>

              <?php if ($service->getSendSignalError() != NULL) { ?>
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <?php echo htmlspecialchars($service->getSendSignalError()); ?>
                </div>
              <?php } ?>
            </div> <!-- end of Send Signal -->
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
    <script>setup();</script>
  </body>
</html>
