<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
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
          <h1>AT&amp;T Sample Application - Call Management</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time: </b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:</b> <script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMessages">
              <h2>Feature 1: Outbound Session from ${cfg.number}</h2>
              <form method="post" action="CMS.jsp" id="msgContentForm" name="msgContentForm">
                <div class="inputFields">

                  <label>
                    Make call to:
                    <input type="text" name="txtNumberToDial" value="<c:out value="${numbToDial}" />"
                      placeholder="Address" title="telephone number or sip address"/>
                  </label>
                  <label>
                    Script Function:
                    <select name="scriptType">
                    <c:forEach var="sType" items="${cfg.scriptFunctions}">
                      <c:if test="${sType == scriptName}"> 
                      <option value="${sType}" selected>${sType}</option>
                      </c:if>
                      <c:if test="${sType != scriptName}"> 
                      <option value="${sType}">${sType}</option>
                      </c:if>
                    </c:forEach>
                    </select>
                  </label>

                  <label>
                    Number parameter for Script Function:
                    <input type="text" name="txtNumber" value="<c:out value="${featuredNumb}" />"  
                    placeholder="Number" title="If message or transfer or wait or reject is selected as script function, 
                    enter number for transfer-to or message-to or wait-from or reject-from"/>
                  </label>

                  <label>
                    Message To Play:
                    <input type="text" name="txtMessageToPlay" value="<c:out value="${msgToPlay}" />"
                      placeholder="Message" title="enter long message or mp3 audio url, this is used as music on hold"/>
                  </label>

                  <div id="scriptText">
                  <label>
                    Script Source Code:
                  </label>
                    <textarea name="txtCreateSession" rows="2" cols="20" disabled="disabled" id="txtCreateSession">
                      <c:out value="${cfg.scriptContent}">No Script Found!</c:out>
                    </textarea>
                </div>

                  <div>
                    <button type="submit" class="submit" name="btnCreateSession" >
                      Create Session</button>
                  </div>
                </div>
              </form>
              <c:if test="${not empty model.sessionId}">
              <div class="successWide">
                <strong>SUCCESS</strong><br>
                id:&nbsp;</strong>${model.sessionId}<br>
                success:&nbsp;</strong>${model.success}
              </div>
              </c:if>
              <c:if test="${not empty model.sessionError}">
              <div class="errorWide">
                <strong>ERROR:</strong>
                ${model.sessionError}
              </div>
              </c:if>
            </div> <!-- end of Create Session -->

            <div class="lightBorder"></div>

            <div id="sendSignal">
              <h2>Feature 2: Send Signal to Session</h2>
              <form method="post" name="sendSignal" action="CMS.jsp">
                <div class="inputFields">
                  <label class="label">
                    <c:if test="${not empty sessionScope.sessionId}">
                    Session ID: ${sessionScope.sessionId} 
                    </c:if>
                    <c:if test="${empty sessionScope.sessionId}">
                    Session ID:
                    </c:if>
                  </label>

                  <label class="label">
                    Signal to Send:
                    <select name="signal"> 
                      <option value="exit" selected="selected">exit</option>
                      <option value="stopHold">stopHold</option>
                      <option value="dequeue">dequeue</option>
                    </select>
                  </label>

                  <div>
                    <button type="submit" class="submit" name="btnSendSignal">
                      Send Signal
                    </button>
                  </div>

                </div>	
              </form>
              <c:if test="${not empty model.signalStatus}">
                <div class="successWide">
                  <strong>SUCCESS</strong><br />
                  <strong>Status:&nbsp;</strong><c:out value="${model.signalStatus}" />
                </div>
              </c:if>

              <c:if test="${not empty model.signalError}">
                <div class="errorWide">
                  <strong>ERROR:</strong>
                  <c:out value="${model.signalError}" />
                </div>
              </c:if>
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
