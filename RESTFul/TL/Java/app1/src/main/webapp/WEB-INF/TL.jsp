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
    <title>AT&amp;T Sample Application - Location</title>		
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
          <h1>AT&amp;T Sample Application - Location</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="lightBorder"></div>

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <h2>Feature 1: Map of Location</h2>
            <form method="post" name="getLocation" action="getLocation">
              <div class="inputFields">
                Requested Accuracy:	
                <input type="radio" name="requestedAccuracy" value="150"
                    ${sessionScope.requestedAccuracy eq 150 ? 'checked' : ''}> 150 m
                <input type="radio" name="requestedAccuracy" value="1000" 
                    ${sessionScope.requestedAccuracy eq 1000 ? 'checked' : ''}> 1,000 m
                <input type="radio" name="requestedAccuracy" value="10000"
                    ${sessionScope.requestedAccuracy eq 10000 ? 'checked' : ''}> 10,000 m
                <br />
                Acceptable Accuracy:
                <input type="radio" name="acceptableAccuracy" value="150"
                    ${sessionScope.acceptableAccuracy eq 150 ? 'checked' : ''}> 150 m
                <input type="radio" name="acceptableAccuracy" value="1000"
                    ${sessionScope.acceptableAccuracy eq 1000 ? 'checked' : ''}> 1,000 m
                <input type="radio" name="acceptableAccuracy" value="10000" 
                    ${sessionScope.acceptableAccuracy eq 10000 ? 'checked' : ''}> 10,000 m
                <br />
                Delay Tolerance:
                <input type="radio" name="tolerance" value="NoDelay"
                    ${sessionScope.tolerance eq 'NoDelay' ? 'checked' : ''}>No Delay
                <input type="radio" name="tolerance" value="LowDelay" 
                    ${sessionScope.tolerance eq 'LowDelay' ? 'checked' : ''}>Low Delay
                <input type="radio" name="tolerance" value="DelayTolerant"
                    ${sessionScope.tolerance eq 'DelayTolerant' ? 'checked' : ''}>Delay Tolerant
                <br />
                <button type="submit" name="getLocation">Get Phone Location</button>
              </div> <!-- end of Device Location -->
            </form>

            <c:if test="${not empty tlResponse}">
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br />
                <strong>Latitude:</strong> <c:out value="${tlResponse.latitude}" /> 
                <br />
                <strong>Longitude:</strong> <c:out value="${tlResponse.longitude}" />
                <br />
                <strong>Accuracy:</strong> <c:out value="${tlResponse.accuracy}" />
                <br />
                <strong>Response Time:</strong> <c:out value="${tlResponse.elapsedTime}" /> seconds
              </div>
              <div align="center">
                <iframe width="600" height="400" frameborder="0" scrolling="no" marginheight="0" marginwidth="0"
                  src="http://maps.google.com/?q=${tlResponse.latitude}+${tlResponse.longitude}&output=embed"></iframe>
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
