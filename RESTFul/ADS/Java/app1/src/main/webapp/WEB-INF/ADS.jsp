<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
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
                <c:forEach var="category" items="${categories}">
                  <option value="${category}">${category}</option>
                </c:forEach>
                </select>
                <div class="inputSeperator"></div>
                  <label>MMA Size:</label>
                  <select name="MMA" id="MMA">
                  <c:forEach var="mmaSize" items="${mmaSizes}">
                    <option value="${mmaSize}">${mmaSize}</option>
                  </c:forEach>
                  </select>
                <div class="inputSeperator"></div>
                <label>Age Group:</label>
                <select name="ageGroup" id="ageGroup">
                  <c:forEach var="ageGroup" items="${ageGroups}">
                    <option value="${ageGroup}">${ageGroup}</option>
                  </c:forEach>
                </select>
                <div class="inputSeperator"></div>
                <label>Premium:</label>
                <select name="Premium" id="Premium">
                  <c:forEach var="entry" items="${premiums}">
                    <option value="${entry.key}">${entry.value}</option>
                  </c:forEach>
                </select>
                <div class="inputSeperator"></div>
                <label>Gender:</label>
                <select name="gender" id="gender">
                  <c:forEach var="entry" items="${genders}">
                    <option value="${entry.key}">${entry.value}</option>
                  </c:forEach>
                </select>
                <div class="inputSeperator"></div>
                <label>Over 18 Ad Content:</label>
                <select name="over18" id="over18">
                  <c:forEach var="entry" items="${over18s}">
                    <option value="${entry.key}">${entry.value}</option>
                  </c:forEach>
                </select>
                <div class="inputSeperator"></div>
                <label>Zip Code:&nbsp;</label>

                <c:if test="${not empty zipCode}">
                <input placeholder="Zip Code" value="<c:out value="${zipCode}" />" type="text" id="zipCode" 
                  name="zipCode" />
                </c:if>
                <c:if test="${empty zipCode}" >
                <input placeholder="Zip Code" type="text" id="zipCode" name="zipCode" />
                </c:if>

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

            <c:if test="${not empty resultAds}">
              <div class="successWide">
                <strong>SUCCESS:</strong><br>
                <c:if test="${not empty noAds}">
                  <c:out value="${noAds}" />
                </c:if>
              </div>
              <c:if test="${not empty type}">
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
                    <td data-value="Value">${type}</td>
                  </tr>
                  <tr>
                    <td data-value="Parameter">ClickUrl</td>
                    <td data-value="Value">${clickUrl}</td>
                  </tr>
                </tbody>
              </table>
              ${content}
              </c:if>
            </c:if>

            <c:if test="${not empty error}">
              <div class="errorWide">
                <strong>ERROR:</strong><br>
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
