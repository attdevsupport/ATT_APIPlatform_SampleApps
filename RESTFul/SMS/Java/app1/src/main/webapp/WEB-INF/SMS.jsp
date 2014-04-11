<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2014 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<html lang="en"> 
  <head> 
    <title>AT&amp;T Sample Application - Basic SMS Service Application</title>
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
          <li>
            <a href="${cfg.linkSource}" target="_blank" 
                id="SourceLink">Source<img alt="source" src="images/opensource.png" /></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="${cfg.linkDownload}" target="_blank"
                id="DownloadLink">Download<img alt="download" src="images/download.png"></a>
            <span class="divider"> |&nbsp;</span>
          </li>
          <li>
            <a href="${cfg.linkHelp}" target="_blank" id="HelpLink">Help</a>
          </li>
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - Basic SMS Service Application</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b>${dateutil.serverTime}</div> 
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div class="inputFields">
              <div id="sendSMSdiv">
                <h2>Feature 1: Send SMS</h2>
                <form method="post" action="sendSMS" name="sendSMSForm" id="sendSMSForm">
                  <c:if test="${not empty addr}"> 
                  <input placeholder="Address" name="address" id="address" type="text" value="${addr}"/>
                  </c:if>
                  <c:if test="${empty addr}"> 
                  <input placeholder="Address" name="address" id="address" type="text" />
                  </c:if>
                  <label>
                    Message
                    <select name="message" id="message">
                      <option value="ATT SMS sample Message">ATT SMS sample message</option>
                    </select>
                  </label>
                  <label>
                    <input type="checkbox" name="chkGetOnlineStatus" id="chkGetOnlineStatus" value="true"
                        title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                    Receive Delivery Status Notification<br>
                  </label>
                  <button type="submit" class="submit" name="sendSMS" id="sendSMS">Send SMS</button>
                </form>
                <c:if test="${not empty requestScope.sendError}">
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  ${requestScope.sendError}
                </div>
                </c:if>
                <c:if test="${not empty requestScope.sendId}">
                <div class="successWide">
                  <strong>SUCCESS: </strong><br>
                  <strong>messageId: </strong>${requestScope.sendId}<br>
                  <c:if test="${not empty requestScope.resourceURL}">
                  <strong>resourceURL: </strong>${requestScope.resourceURL}<br>
                  </c:if>
                </div>
                </c:if>
              </div> <!-- end of sendSMS -->
              <div class="lightBorder"></div>
              <div id="getStatusdiv">
                <h2>Feature 2: Get Delivery Status</h2>
                <form method="post" action="getStatus" name="getStatusForm" id="getStatusForm">
                  <c:if test="${not empty sessionScope.statusId}">
                  <input placeholder="Message ID" name="messageId" id="messageId" type="text" 
                    value="${sessionScope.statusId}">
                  </c:if>
                  <c:if test="${empty sessionScope.statusId}">
                  <input placeholder="Message ID" name="messageId" id="messageId" type="text">
                  </c:if>
                  <button type="submit" class="submit" name="getStatus" id="getStatus">Get Status</button>
                </form>
                <c:if test="${not empty requestScope.statusError}">
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  ${requestScope.statusError}
                </div>
                </c:if>
                <c:if test="${not empty requestScope.resultGetStatuses}">
                <div class="successWide">
                  <strong>SUCCESS: </strong><br>
                  <strong>ResourceUrl: </strong>${requestScope.resourceURL}<br>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>Id</th>
                      <th>Address</th>
                      <th>DeliveryStatus</th>
                    </tr>
                  </thead>
                  <tbody>
                  <c:forEach var="status" items="${resultGetStatuses}">
                    <tr>
                      <td data-value="Message Id"><c:out value="${status.messageId}" /></td>
                      <td data-value="Status"><c:out value="${status.address}" /></td>
                      <td data-value="Resouce Url"><c:out value="${status.deliveryStatus}" /></td>
                    </tr>
                  </c:forEach>
                  </tbody>
                </table>
                </c:if>
              </div> <!-- end of getStatus -->
              <div class="lightBorder"></div>
              <div id="receiveStatusdiv">
                <h2>Feature 3: Receive Delivery Status</h2>
                <form method="post" action="refresh" name="refreshStatusForm" id="refreshStatusForm">
                  <button type="submit" class="submit" name="receiveStatusBtn" id="receiveStatusBtn">
                    Refresh Notifications</button>
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
              <div id="getMessages">
                <h2>Feature 4: Get Messages (${cfg.pollMsgsShortCode})</h2>
                <form method="post" action="getMessages" name="getMessagesForm" id="getMessagesForm">
                  <button type="submit" class="submit" name="getMessages" id="getMessages">Get Messages</button>
                </form>
                <c:if test="${not empty requestScope.receiveError}">
                <div class="errorWide">
                  <strong>ERROR: </strong><br>
                  ${requestScope.receiveError}
                </div>
                </c:if>
                <c:if test="${not empty requestScope.numbMessages}">
                <div class="successWide">
                  <strong>SUCCESS:</strong><br>
                  <strong>Messages in this batch: <strong>${requestScope.numbMessages}<br>
                  <strong>Messages pending: <strong>${requestScope.numbPending}<br>
                </div>
                <table>
                  <thead>
                    <tr>
                      <th>Message Index</th>
                      <th>Message Text</th>
                      <th>Sender Address</th>
                    </tr>
                  </thead>
                  <tbody>
                    <c:forEach var="msg" items="${requestScope.SMSMessages}">
                    <tr>
                      <td data-value="Message Index">${msg.messageIndex}</td>
                      <td data-value="Message Text">${msg.messageText}</td>
                      <td data-value="Sender Address">${msg.senderAddress}</td>
                    </tr>
                    </c:forEach>
                  </tbody>
                </table>
                </c:if>
              </div> <!-- end of getMessages -->
              <div class="lightBorder"></div>
              <div id="receiveMsgs">
                <h2>Feature 5: Receive Messages (${cfg.receiveMsgsShortCode})</h2>
                <form method="post" action="refresh" name="receiveMessagesForm" id="receiveMessagesForm">
                  <button type="submit" class="submit" name="receiveMessages" 
                      id="receiveMessages">Refresh Received Messages</button>
                </form>
                  <table>
                    <thead>
                      <tr>
                        <th>DateTime</th>
                        <th>Message Id</th>
                        <th>Message</th>
                        <th>Sender Address</th>
                        <th>Destination Address</th>
                      </tr>
                    </thead>
                    <tbody>
                    <c:forEach var="msg" items="${resultReceiveMsgs}">
                        <tr>
                          <td data-value="DateTime"><c:out value="${msg.dateTime}" /></td>
                          <td data-value="Message Id"><c:out value="${msg.msgId}" /></td>
                          <td data-value="Message"><c:out value="${msg.msg}" /></td>
                          <td data-value="Sender Address"><c:out value="${msg.senderAddr}" /></td>
                          <td data-value="Destination Address"><c:out value="${msg.destinationAddr}" /></td>
                        </tr>
                    </c:forEach>
                  </tbody>
                </table>
              </div> <!-- end of receiveMsgs -->
            </div> <!-- end of inputFields -->
          </div> <!-- end of formContainer -->
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
          &copy; 2014 AT&amp;T Intellectual Property. All rights reserved.
          <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <script>setup();</script>
  </body>
</html>
