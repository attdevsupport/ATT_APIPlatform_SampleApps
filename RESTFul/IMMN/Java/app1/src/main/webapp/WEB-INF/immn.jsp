<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib uri="http://java.sun.com/jsp/jstl/functions" prefix="fn" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<html>
  <head>
    <title>AT&amp;T Sample Application - In App Messaging From Mobile Number</title>
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
          <h1>AT&amp;T Sample Application - In App Messaging From Mobile Number</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b>${dateutil.serverTime}</div> 
            <div><b>Client Time:</b> <script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMessages">
              <h2>Send Messages:</h2>
              <form method="post" action="index.jsp" name="msgContentForm" >
                  <div class="inputFields">
                      <c:choose>
                          <c:when test="${not empty address}">
                              <input placeholder="Address" value="${Address}" name="Address" type="text" />
                          </c:when>
                          <c:otherwise>
                              <input placeholder="Address" name="Address" type="text" />     
                          </c:otherwise>
                      </c:choose>
                      <label>Group: <input name="groupCheckBox" type="checkbox" /></label>
                  <label>
                    Message:
                    <select name="message">
                      <option value="ATT IMMN sample message">ATT IMMN sample message</option>
                    </select>
                  </label>
                  <label>
                    Subject:
                    <select name="subject">
                      <option value="ATT IMMN sample subject">ATT IMMN sample subject</option>
                    </select>
                  </label>
                  <label>
                    Attachment:
                    <select name="attachment">
                      <option value="None">None</option>
                      <option value="att.gif">att.gif</option>
                    </select>
                  </label>
                  <button id="sendMessage" type="submit" class="submit">Send Message</button>
                </div>
            </form>
            <c:if test="${not empty immnError}">
                <div class="errorWide">
                    <strong>ERROR:</strong>
                    <c:out value="${immnError}" />
                </div>
            </c:if>
            <c:if test="${not empty immnResponse}">
                <div class="successWide">
                    <strong>SUCCESS:</strong>
                    <c:out value="${immnResponse}" />
                </div>
            </c:if>
            </div> <!-- end of sendMessages -->

            <div class="lightBorder"></div>

            <div id="getMessages">
              <h2>Read Messages:</h2>
              <form method="post" action="index.jsp" name="msgHeaderForm" id="msgHeaderForm">
                <div class="inputFields">
                  <input name="headerCountTextBox" type="text" maxlength="3" placeholder="Header Counter" />     
                  <input name="indexCursorTextBox" type="text" maxlength="30" placeholder="Index Cursor" />     
                  <button type="submit" class="submit" name="getMessageHeaders" id="getMessageHeaders">
                      Get Message Headers
                  </button>
                </div>
              </form>

              <form method="post" action="index.jsp" name="msgContentForm" id="msgContentForm">
                <div class="inputFields">
                  <input name="MessageId" id="MessageId" type="text" maxlength="30" placeholder="Message ID" />     
                  <input name="PartNumber" id="PartNumber" type="text" maxlength="30" placeholder="Part Number" />     
                  <button  type="submit" class="submit" name="getMessageContent" id="getMessageContent">
                    Get Message Content
                  </button>
                </div>
              </form>
              <label class="note">To use this feature, you must be a subscriber to My AT&amp;T Messages.</label>
            </div> <!-- end of getMessages -->
        </div> <!-- end of formContainer -->

        <!-- BEGIN HEADER CONTENT RESULTS -->
        <c:if test="${not empty mimError}">
            <div class="errorWide">
                <strong>ERROR:</strong>
                <c:out value="${mimError}" />
            </div>
        </c:if>

        <c:if test="${not empty mimContent}">
            <div class="successWide">
                <strong>SUCCESS:</strong>
            </div>
            <c:choose>
                <c:when test="${fn:contains(mimContent.contentType, 'TEXT')}">
                    <c:out value="${mimContent.data}" />
                </c:when>
                <c:when test="${fn:contains(mimContent.contentType, 'SMIL')}">
                    <textarea name="TextBox1" rows="2" cols="20" id="TextBox1" disabled="disabled">
                        <c:out value="${mimContent.data}" />
                    </textarea>
                </c:when>
                <c:when test="${fn:contains(mimContent.contentType, 'IMAGE')}">
                    <img src="data:${mimContent.contentType} %>;base64,${mimContent.data}" />
                </c:when>
            </c:choose>
        </c:if>

        <c:if test="${not empty mimResponse}">
            <div class="successWide">
                <strong>SUCCESS:</strong>
            </div>
            <p id="headerCount">Header Count: ${mimResponse.headerCount}</p>
            <p id="indexCursor">Index Cursor: ${mimResponse.indexCursor}</p>
            <table class="kvp" id="kvp">
                <thead>
                    <tr>
                        <th>MessageId</th>
                        <th>From</th>
                        <th>To</th>
                        <th>Received</th>
                        <th>Text</th>
                        <th>Favourite</th>
                        <th>Read</th>
                        <th>Type</th>
                        <th>Direction</th>
                        <th>Contents</th>
                    </tr>
                </thead>
                <tbody>
                    <c:if test="${not empty mimHeaders}">
                        <c:forEach  var="mheader" items="${mimHeaders}" varStatus="count">
                          <c:set var="rowId" value="row${count.count}" />
                          <tr id="${rowId}">
                            <td data-value="MessageId">
                              <c:out value="${mheader.messageId}" />
                            </td>
                            <td data-value="From">
                              <c:out value="${mheader.from}" />
                            </td>
                            <td data-value="To">
                              <c:out value="${mheader.to}" />
                            </td>
                            <td data-value="Received">
                              <c:out value="${mheader.received}" />
                            </td>
                            <td data-value="Text">
                              <c:out value="${mheader.text}" default="-" />
                            </td>
                            <td data-value="Favorite">
                              <c:out value="${mheader.favorite}" />
                            </td>
                            <td data-value="Read">
                              <c:out value="${mheader.read}" />
                            </td>
                            <td data-value="Type">
                              <c:out value="${mheader.type}" />
                            </td>
                            <td data-value="Direction">
                              <c:out value="${mheader.direction}" />
                            </td>
                            <td data-value="Contents">
                              <c:choose>
                                <c:when test="${not empty mheader.mmsContent}">
                                  <select id="attachments" onchange='chooseSelect("${rowId}",this)'>
                                    <option value="More..">More..</option>
                                    <c:forEach items="${mheader.mmsContent}" var="attachment">
                                      <option value="${attachment.partNumber} - ${attachment.contentName} - ${attachment.contentType}">
                                        <c:out value="${attachment.partNumber} - ${attachment.contentName} - ${attachment.contentType}" />
                                      </option>
                                    </c:forEach>
                                  </select>
                                </c:when>
                                <c:otherwise>
                                  &#45;
                                </c:otherwise>
                              </c:choose>
                            </td>
                          </tr>
                        </c:forEach>
                      </c:if>
                    </tbody>
                  </table>
                </c:if>
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
