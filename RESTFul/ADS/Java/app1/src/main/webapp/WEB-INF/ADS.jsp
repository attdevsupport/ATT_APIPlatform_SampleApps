<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
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
          <li><a href="#" target="_blank">Full Page<img src="images/max.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkSource}" target="_blank">Source<img src="images/opensource.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkDownload}" target="_blank">Download<img src="images/download.png"></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkHelp}" target="_blank">Help</a>
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
            <div><b>Server Time:&nbsp;</b>${dateutil.serverTime}</div> 
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="lightBorder"></div>

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <h2>Feature 1: Get Advertisement</h2>
            <form method="post" name="getAdvertisement" action="getAds">
              <div id="getAds">
                <label>Category</label>
                <select name="category" id="category">
                  <option value="auto" selected="selected">auto</option>
                  <option value="business">business</option>
                  <option value="chat">chat</option>
                  <option value="communication">communication</option>
                  <option value="community">community</option>
                  <option value="entertainment">entertainment</option>
                  <option value="finance">finance</option>
                  <option value="games">games</option>
                  <option value="health">health</option>
                  <option value="local">local</option>
                  <option value="maps">maps</option>
                  <option value="medical">medical</option>
                  <option value="movies">movies</option>
                  <option value="music">music</option>
                  <option value="news">news</option>
                  <option value="other">other</option>
                  <option value="personals">personals</option>
                  <option value="photos">photos</option>
                  <option value="shopping">shopping</option>
                  <option value="social">social</option>
                  <option value="sports">sports</option>
                  <option value="technology">technology</option>
                  <option value="tools">tools</option>
                  <option value="travel">travel</option>
                  <option value="tv">tv</option>
                  <option value="video">video</option>
                  <option value="weather">weather</option>
                </select>
                <div class="inputSeperator"></div>
                  <label>MMA Size:</label>
                  <select name="MMA" id="MMA">
                    <option value=""></option>
                    <option value="120 x 20">120 x 20</option>
                    <option value="168 x 28">168 x 28</option>
                    <option value="216 x 36">216 x 36</option>
                    <option value="300 x 50">300 x 50</option>
                    <option value="300 x 250">300 x 250</option>
                    <option value="320 x 50">320 x 50</option>
                  </select>
                <div class="inputSeperator"></div>
                <label>Age Group:</label>
                <select name="ageGroup" id="ageGroup">
                  <option value=""></option>
                  <option value="1-13">1-13</option>
                  <option value="14-25" selected="selected">14-25</option>
                  <option value="26-35">26-35</option>
                  <option value="36-55">36-55</option>
                  <option value="55-100">55-100</option>
                </select>
                <div class="inputSeperator"></div>
                <label>Premium:</label>
                <select name="Premium" id="Premium">
                  <option value=""></option>
                  <option value="0" >NonPremium</option>
                  <option value="1" >Premium Only</option>
                  <option value="2" >Both</option>
                </select>
                <div class="inputSeperator"></div>
                <label>Gender:</label>
                <select name="gender" id="gender">
                  <option value=""></option>
                  <option value="M" >Male</option>
                  <option value="F"  selected="selected">Female</option>
                </select>
                <div class="inputSeperator"></div>
                <label>Over 18 Ad Content:</label>
                <select name="over18" id="over18">
                  <option value=""></option>
                  <option value="0" >Deny Over 1</option>
                  <option value="2" >Only Over 18</option>
                  <option value="3" >Allow All Ads</option>
                </select>
                <div class="inputSeperator"></div>
                <label>Zip Code:&nbsp;</label><input placeholder="Zip Code" type="text" id="zipCode" name="zipCode" />
                <div class="inputSeperator"></div>
                <label>City:&nbsp;</label><input placeholder="City" type="text" id="city" name="city" />
                <div class="inputSeperator"></div>
                <label>Area Code:&nbsp;</label><input placeholder="Area Code" type="text" id="areaCode" name="areaCode" />
                <div class="inputSeperator"></div>
                <label>Country:&nbsp;</label><input placeholder="Country" type="text" id="country" name="country" />
                <div class="inputSeperator"></div>
                <label>Latitude:&nbsp;</label><input placeholder="Latitude" type="text" id="latitude" name="latitude" />
                <div class="inputSeperator"></div>
                <label>Longitude:&nbsp;</label><input placeholder="Longitude" type="text" id="longitude" name="longitude" />
                <div class="inputSeperator"></div>
                <label>Keywords:&nbsp;</label><input placeholder="Keywords" type="text" id="keywords" name="keywords" />
                <div class="inputSeperator"></div>
                <button type="submit" name="btnGetAds">Get Advertisement</button>
              </div> <!-- end of getAds -->
            </form>

            <c:if test="${not empty type}">
              <div class="successWide">
                <strong>SUCCESS:</strong><br>
                <b>Type:&nbsp;</b><c:out value="${type}" /><br>
                <b>Click URL:&nbsp;</b><c:out value="${clickUrl}" /><br>
                <c:if test="${not empty text}">
                <b>Text:&nbsp;</b><c:out value="${text}" /><br>
                </c:if>
                <c:if test="${not empty content}">
                <b>Content:&nbsp;</b>${content}<br>
                </c:if>
                <c:if test="${not empty image}">
                <b>Image:&nbsp;</b>${image}<br>
                </c:if>
              </div>
            </c:if>
            <c:if test="${not empty error}">
              <div class="errorWide">
                <strong>ERROR:</strong>
                <br />
                <c:out value="${error}" />
              </div>
            </c:if>
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
        &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
