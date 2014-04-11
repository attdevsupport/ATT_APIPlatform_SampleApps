<?php
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/ADSController.php';
require_once __DIR__ . '/lib/Util/Util.php';
use Att\Api\Util\Util;

$controller = new ADSController();
$controller->handleRequest();
$errors = $controller->getErrors();
$results = $controller->getResults();
?>
<!DOCTYPE html>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2014 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Advertisement</title>		
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
          <h1>AT&amp;T Sample Application - Advertisement</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="lightBorder"></div>

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <h2>Feature 1: Get Advertisement</h2>
            <form method="post" name="getAdvertisement" action="index.php">
              <div id="getAds">

                <label>Category</label>
                <?php 
                  $categories = array('auto', 'business', 'chat', 'communication', 'community', 'entertainment',
                      'finance', 'games', 'health', 'local', 'maps', 'medical', 'movies', 'music', 'news', 
                      'other', 'personals', 'photos', 'shopping', 'shopping', 'social', 'sports', 'technology',
                      'tools', 'travel', 'tv', 'video', 'weather');
                ?>
                <select name="category" id="category">
                  <?php foreach($categories as $category) { ?>
                    <?php if (isset($_SESSION['category']) && $_SESSION['category'] == $category) { ?>
                      <option value="<?php echo $category; ?>" selected="selected"><?php echo $category; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $category; ?>"><?php echo $category; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <label>MMA Size:</label>
                <?php
                  $mmaSizes = array('', '120 x 20', '168 x 28', '216 x 36', '300 x 50', '300 x 250', '320 x 50'); 
                ?>
                <select name="MMA" id="MMA">
                  <?php foreach($mmaSizes as $mmaSize) { ?>
                    <?php if (isset($_SESSION['MMA']) && $_SESSION['MMA'] == $mmaSize) { ?>
                      <option value="<?php echo $mmaSize; ?>" selected="selected"><?php echo $mmaSize; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $mmaSize; ?>"><?php echo $mmaSize; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <?php
                  $ageGroups = array('', '1-13', '14-25', '26-35', '36-55', '55-100');
                ?>
                <label>Age Group:</label>
                <select name="ageGroup" id="ageGroup">
                  <?php foreach($ageGroups as $ageGroup) { ?>
                    <?php if (isset($_SESSION['ageGroup']) && $_SESSION['ageGroup'] == $ageGroup) { ?>
                      <option value="<?php echo $ageGroup; ?>" selected="selected"><?php echo $ageGroup; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $ageGroup; ?>"><?php echo $ageGroup; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <label>Premium:</label>
                <?php
                  $premiums = array('' => '', '0' => 'NonPremium', '1' => 'PremiumOnly', '2' => 'Both' );
                ?>
                <select name="Premium" id="Premium">
                  <?php foreach($premiums as $k => $v) { ?>
                    <?php if (isset($_SESSION['Premium']) && $_SESSION['Premium'] == "$k") { ?>
                      <?php echo 'session: ' . $_SESSION['Premium']; ?> 
                      <option value="<?php echo $k; ?>" selected="selected"><?php echo $v; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $k; ?>"><?php echo $v; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <label>Gender:</label>
                <?php
                  $genders = array('' => '', 'M' => 'Male', 'F' => 'Female');
                ?>
                <select name="gender" id="gender">
                  <?php foreach($genders as $k => $v) { ?>
                    <?php if (isset($_SESSION['gender']) && $_SESSION['gender'] == "$k") { ?>
                      <option value="<?php echo $k; ?>" selected="selected"><?php echo $v; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $k; ?>"><?php echo $v; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <label>Over 18 Ad Content:</label>
                <?php
                  $over18Vals = array('' => '', '0' => 'Deny Over 18', '2' => 'Only Over 18', '3' => 'Allow All Ads');
                ?>
                <select name="over18" id="over18">
                  <?php foreach($over18Vals as $k => $v) { ?>
                    <?php if (isset($_SESSION['over18']) && $_SESSION['over18'] == "$k") { ?>
                      <option value="<?php echo $k; ?>" selected="selected"><?php echo $v; ?></option>
                    <?php } else { ?>
                      <option value="<?php echo $k; ?>"><?php echo $v; ?></option>
                    <?php } ?>
                  <?php } ?>
                </select>

                <div class="inputSeperator"></div>
                <label>Zip Code:&nbsp;</label>
                <?php if (isset($_SESSION['zipCode'])) { ?>
                  <input placeholder="Zip Code" type="text" id="zipCode" 
                    value="<?php echo htmlspecialchars($_SESSION['zipCode']); ?>" name="zipCode" />
                <?php } else { ?>
                  <input placeholder="Zip Code" type="text" id="zipCode" name="zipCode" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <label>City:&nbsp;</label>
                <?php if (isset($_SESSION['city'])) { ?>
                  <input placeholder="City" type="text" id="city" 
                    value="<?php echo htmlspecialchars($_SESSION['zipCode']); ?>" name="city" />
                <?php } else { ?>
                  <input placeholder="City" type="text" id="city" name="city" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <label>Area Code:&nbsp;</label>
                <?php if (isset($_SESSION['areaCode'])) { ?>
                  <input placeholder="Area Code" type="text" id="areaCode" 
                    value="<?php echo htmlspecialchars($_SESSION['areaCode']); ?>" name="areaCode" />
                <?php } else { ?>
                  <input placeholder="Area Code" type="text" id="areaCode" name="areaCode" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <label>Country:&nbsp;</label>
                <?php if (isset($_SESSION['country'])) { ?>
                  <input placeholder="Country" type="text" id="country" 
                    value="<?php echo htmlspecialchars($_SESSION['country']); ?>" name="country" />
                <?php } else { ?>
                  <input placeholder="Country" type="text" id="country" name="country" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <label>Latitude:&nbsp;</label>
                <?php if (isset($_SESSION['latitude'])) { ?>
                  <input placeholder="Latitude" type="text" id="latitude" 
                    value="<?php echo htmlspecialchars($_SESSION['latitude']); ?>" name="latitude" />
                <?php } else { ?>
                  <input placeholder="Latitude" type="text" id="latitude" name="latitude" />
                <?php } ?>
                
                <div class="inputSeperator"></div>
                <label>Longitude:&nbsp;</label>
                <?php if (isset($_SESSION['longitude'])) { ?>
                  <input placeholder="Longitude" type="text" id="longitude" 
                    value="<?php echo htmlspecialchars($_SESSION['longitude']); ?>" name="longitude" />
                <?php } else { ?>
                  <input placeholder="Longitude" type="text" id="longitude" name="longitude" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <label>Keywords:&nbsp;</label>
                <?php if (isset($_SESSION['keywords'])) { ?>
                  <input placeholder="Keywords" type="text" id="keywords" 
                    value="<?php echo htmlspecialchars($_SESSION['keywords']); ?>" name="keywords" />
                <?php } else { ?>
                  <input placeholder="Keywords" type="text" id="keywords" name="keywords" />
                <?php } ?>

                <div class="inputSeperator"></div>
                <button type="submit" name="btnGetAds">Get Advertisement</button>
              </div> <!-- end of getAds -->
            </form>
              <?php 
              if (isset($results[ADSController::RESULT_AD])) {
                $result = $results[ADSController::RESULT_AD];
              ?>
              <div class="successWide">
                <strong>SUCCESS:</strong><br>
                <?php 
                  if (is_string($result)) { 
                    echo htmlspecialchars($result);
                ?>
              </div>
              <?php } else { ?>
              <table>
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td data-value="Parameter">Type</td>
                    <td data-value="Value"><?php echo htmlspecialchars($result->getAdsType()); ?></td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">ClickUrl</td>
                    <td data-value="Value"><?php echo htmlspecialchars($result->getClickUrl()); ?></td>
                  </tr>
                </tbody>
              </table>
              <?php echo $result->getContent(); ?>

              <?php } ?>
              <?php } ?>
            <?php if (isset($errors[ADSController::ERROR_AD])) { ?>
              <div class="errorWide">
                <strong>ERROR:</strong>
                <br />
                <?php echo htmlspecialchars($errors[ADSController::ERROR_AD]); ?>
              </div>
            <?php } ?>
          <!-- SAMPLE APP CONTENT ENDS HERE! -->

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
        <br /><br />
        For download of tools and documentation, please go to 
        <a href="https://devconnect-api.att.com/" 
          target="_blank">https://devconnect-api.att.com</a>
        <br> For more information contact 
        <a href="mailto:developer.support@att.com">developer.support@att.com</a>
        <br /><br />
        &#169; 2014 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
