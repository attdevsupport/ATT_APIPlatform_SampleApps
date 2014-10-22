<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Multimedia Messaging Service</title>		
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
        <div class="logo"></div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div> <!-- end of menuButton -->
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
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - Multimedia Messaging Service</h1>
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
            <div id="sendMMS">
              <h2>Feature 1: Send MMS Message</h2>
              <form method="post" name="sendMms" action="MMSAction">
                <div class="inputFields">
		              <c:if test="${not empty addr}">	
                  <input name="address" placeholder="Address" value="<c:out value="${addr}" />" />
                  </c:if>
		              <c:if test="${empty addr}">	
                  <input name="address" placeholder="Address" />
                  </c:if>
                  <label>
                    Message:
                    <select name="subject">
                      <option>MMS Sample Message</option>
                    </select>
                  </label>
                  <label>
                    Attachment:
                    <select name="attachment">
                      <c:forEach var="fname" items="${fnames}">
                        <c:if test="${fname eq sessionScope.attachment}">
                          <option selected="selected"><c:out value="${fname}" /></option>
                        </c:if>
                        <c:if test="${fname ne sessionScope.attachment}">
                          <option><c:out value="${fname}" /></option>
                        </c:if>
                      </c:forEach>
                    </select>
                  </label>
                  <label>
                    <c:if test="${notify eq true}">
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="True" checked
                      title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                    </c:if>
                    <c:if test="${notify ne true}">
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="True"
                      title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                    </c:if>
                      Receive Delivery Status Notification<br>
                  </label>
                  <button type="submit" class="submit" name="sendMms">Send MMS Message</button>
                </div> <!-- end of inputFields -->
              </form>
            </div> <!-- end of sendMMS -->
            <c:if test="${not empty errorSendMMS}">
            <div class="errorWide">
              <strong>ERROR: </strong><br>
              <c:out value="${errorSendMMS}" />
            </div>
            </c:if>
            <c:if test="${not empty resultSendMMS}">
            <div class="successWide">
              <strong>SUCCESS: </strong><br>
              <strong>messageId: </strong>${resultSendId}<br>
              <c:if test="${not empty resultResourceUrl}">
              <strong>resourceURL: </strong>${resultResourceUrl}<br>
              </c:if>
            </div>
            </c:if>
            <div class="lightBorder"></div>
            <div id="getDeliveryStatus">
              <h2>Feature 2: Get Delivery Status</h2>
              <form method="post" name="getStatus" action="MMSAction">
                <div class="inputFields">
                  <c:if test="${not empty resultId}">
                  <input maxlength="20" name="mmsId" placeholder="Message ID" value="${resultId}" />
                  </c:if>
                  <c:if test="${empty resultId}">
                  <input maxlength="20" name="mmsId" placeholder="Message ID" />
                  </c:if>
                  <button type="submit" class="submit" name="getStatus">Get Status</button>
                </div> <!-- end of inputFields -->	
              </form> 
            </div> <!-- end of getDeliveryStatus -->
            <c:if test="${not empty errorGetStatus}">
            <div class="errorWide">
            <strong>ERROR: </strong><br>
            <c:out value="${errorGetStatus}" />
            </div>
            </c:if>
            <c:if test="${not empty resultStatus}">
            <div class="successWide">
            <strong>SUCCESS:&nbsp;</strong><br>
            <strong>Status:&nbsp;</strong><c:out value="${resultStatus}" /><br>
            <strong>ResourceUrl:&nbsp;</strong><c:out value="${resultResourceUrl}" /><br>
            </div>
            </c:if>
            <div class="lightBorder"></div>
            <div id="receiveStatusdiv">
              <h2>Feature 3: Receive Delivery Status</h2>
              <form method="post" name="refresh" action="refresh">
                <button type="submit" class="submit" name="receiveStatusBtn" 
                    id="receiveStatusBtn">Refresh Notifications</button>
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
                  <c:forEach var="status" items="${resultStatuses}">
                    <tr>
                      <td data-value="Message Id"><c:out value="${status.messageId}" /></td>
                      <td data-value="Status"><c:out value="${status.address}" /></td>
                      <td data-value="Resouce Url"><c:out value="${status.deliveryStatus}" /></td>
                    </tr>
                  </c:forEach>
                </tbody>
              </table>
            </div><!-- end of receiveStatus -->
            <div class="lightBorder"></div>
            <div id="webGallery">
              <h2>Feature 4: Web gallery of MMS photos sent to short code</h2>
              <p>Photos sent to short code <c:out value="${cfg.shortCode}" /> : <c:out value="${imgCount}" /></p>
              <c:forEach var="entry" items="${images}">
              <img src="${entry.imagePath}" width="150" border="0" /><br>
              <strong>Sent from:&nbsp;</strong><c:out value="${entry.senderAddress}" /><br>
              <strong>On:&nbsp;</strong><c:out value="${entry.date}" /><br>
              <strong>Text:&nbsp;</strong><c:out value="${entry.text}" /><br>
              </c:forEach>
            </div> <!-- end of webGallery -->	
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
          To access your apps, please go to
          <a href="https://developer.att.com/developer/mvc/auth/login"
          target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
          <br> For support refer to
          <a href="https://developer.att.com/support">https://developer.att.com/support</a>
          <br><br>
          &#169; 2014 AT&amp;T Intellectual Property. All rights reserved. 
          <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
