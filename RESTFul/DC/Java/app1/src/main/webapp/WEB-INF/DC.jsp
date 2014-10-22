<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
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
          <h1>AT&amp;T Sample DC Application - Get Device Capabilities Application</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
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

            <c:if test="${not empty dcResponse}">
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
                  <c:forEach var="entry" items="${dcResponse.responseMap}">
                    <tr>
                      <td data-value="Parameter">
                        <c:out value="${entry.key}" />
                      </td>
                      <td data-value="Value">
                        <c:out value="${entry.value}" />
                      </td>
                    </tr>
                  </c:forEach>
                </tbody>
              </table>
            </c:if>

            <c:if test="${not empty error}">
              <div class="errorWide">
                <b>ERROR:</b><br>
                <c:out value="${error}" />
              </div>
            </c:if>

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
        To access your apps, please go to
        <a href="https://developer.att.com/developer/mvc/auth/login"
        target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
        <br> For support refer to
        <a href="https://developer.att.com/support">https://developer.att.com/support</a>
        <br /><br />
        &#169; 2014 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
