<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<html lang="en">
  <head>
    <title>AT&amp;T Sample Speech Application - Speech to Text (Generic)</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type"> 
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
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
    <div id="pageContainer" class="pageContainer">
      <div id="header">
        <div class="logo" id="top"></div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div>
        <ul class="links" id="nav">
          <li><a href="#" target="_blank">Full Page<img 
              src="images/max.png"></img></a> <span class="divider"> |&nbsp;</span>
          </li>
          <li><a href="${cfg.linkSource}" target="_blank">Source<img
          src="images/source.png" /></a> <span class="divider"> |&nbsp;</span></li>
          <li><a href="${cfg.linkDownload}" target="_blank">Download<img
          src="images/download.png"></a> <span class="divider">|&nbsp;</span></li>
          <li><a href="${cfg.linkHelp}" target="_blank">Help</a></li>
          <li id="back"><a href="#top">Back to top</a></li>
        </ul> <!-- end of links -->
      </div> <!-- end of header -->
      <div id="content" class="content">
        <div class="contentHeading">
          <h1>AT&amp;T Sample Application - Speech to Text</h1>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <form name="SpeechToText" action="index.jsp" method="post">
              <div id="formData">
                <h3>Speech Context:</h3>
                <select name="SpeechContext">
                <c:forEach var="cname" items="${speechContexts}">
                  <c:choose>
                    <c:when test="${sessionContextName eq cname}">
                    <option value="${cname}" selected>${cname}</option>
                    </c:when>
                    <c:otherwise>
                    <option value="${cname}">${cname}</option>
                    </c:otherwise>
                  </c:choose>
                </c:forEach>
                </select>
                <h3>Audio File:</h3>
                <select name="audio_file">
                <c:forEach var="fname" items="${fnames}">
                  <c:choose>
                    <c:when test="${sessionFileName eq fname}">
                    <option value="${fname}" selected>${fname}</option>
                    </c:when>
                    <c:otherwise>
                    <option value="${fname}">${fname}</option>
                    </c:otherwise>
                  </c:choose>
                </c:forEach>
                </select>
                <div id="chunked">
                  <br>
                  <b>Send Chunked:</b>
                  <c:if test="${not empty sessionChunked}">
                  <input name="chkChunked" value="Send Chunked" type="checkbox" checked>
                  </c:if>
                  <c:if test="${empty sessionChunked}">
                  <input name="chkChunked" value="Send Chunked" type="checkbox">
                  </c:if>
                </div> <!-- end of chunked -->
                <h3>X-Arg:</h3>
                <textarea id="x_arg" name="x_arg" readonly="readonly" rows="4" value="${xArg}">${xArg}</textarea>
                <br>
                <h3>X-SpeechSubContext:</h3>
                <textarea id="x_subContext" name="x_subContext" readonly="readonly" rows="4" value="${x_subContext}">${x_subContext}</textarea>
                <br>
                <button type="submit" name="SpeechToText" id="btnSubmit">Submit</button>
              </div> <!-- end of formData -->
            </form>
          </div> <!-- end of formContainer -->
          <c:if test="${not empty errorSpeech}">
          <div class="errorWide">
            <strong>ERROR:</strong><br>
            <c:out value="${errorSpeech}" />
          </div>
          </c:if>
          <c:if test="${not empty resultSpeech}">
          <div class="successWide">
            <strong>SUCCESS:</strong> <br />Response parameters listed below.
          </div>
          <table class="kvp">
            <thead>
              <tr>
                <th class="label">Parameter</th>
                <th class="label">Value</th>
              </tr>
            </thead>
            <tbody>
            <c:forEach var="kvp" items="${resultSpeech}">
              <tr>
                <td data-value="Parameter"><em><c:out value="${kvp[0]}" /></em></td>
                <td data-value="Value"><em><c:out value="${kvp[1]}" /></em></td>
              </tr>
            </c:forEach>
            </tbody>
          </table>
          </c:if>
        </div> <!-- end of formBox -->
      </div> <!-- end of content -->
      <div id="footer">
        <div id="ft">
          <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
          <p>
            The Application hosted on this site are working examples intended
            to be used for reference in creating products to consume AT&amp;T
            Services and not meant to be used as part of your product. The data
            in these pages is for test purposes only and intended only for use
            as a reference in how the services perform. <br> <br> For
            download of tools and documentation, please go to <a
            href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
            <br> For more information contact <a
            href="mailto:developer.support@att.com">developer.support@att.com</a>
            <br> <br> � 2013 AT&amp;T Intellectual Property. All
            rights reserved. <a href="http://developer.att.com/"
            target="_blank">http://developer.att.com</a>
          </p>
        </div> <!-- end of ft -->
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
