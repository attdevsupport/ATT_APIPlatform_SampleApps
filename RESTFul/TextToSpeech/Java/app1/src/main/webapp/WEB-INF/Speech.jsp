<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<html lang="en">
  <head>
    <title>AT&amp;T Sample Application - Text to Speech</title>
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
  <body onload="javascript:doLoad();">
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
      <div class="content">
        <div class="contentHeading">
          <h1>AT&amp;T Sample Application - Text to Speech</h1>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <form name="TextToSpeech" action="SpeechAction" method="post">
              <div id="formData">
                <h3>Content Type:</h3>
                <select name="contentType" onchange="javascript:handleContentTypeChanged(this);">
                <c:forEach var="cname" items="${contentTypes}">
                  <c:choose>
                    <c:when test="${sessionContentType eq cname}">
                    <option value="${cname}" selected>${cname}</option>
                    </c:when>
                    <c:otherwise>
                    <option value="${cname}">${cname}</option>
                    </c:otherwise>
                  </c:choose>
                </c:forEach>
                </select>
                <h3>Content:</h3>
                <textarea id="speechText" name="speechText" rows="4"></textarea>
                <h3>X-Arg:</h3>
                <textarea id="x_arg" name="x_arg" rows="4" value="${xArg}">${xArg}</textarea>
                <br>
                <button type="submit" name="TextToSpeech">Submit</button>
              </div> <!-- end of formData -->
            </form>
          </div> <!-- end of formContainer -->
          <c:if test="${not empty errorSpeech}">
          <div class="errorWide">
            <strong>ERROR:</strong><br>
            <c:out value="${errorSpeech}" />
          </div>
          </c:if>
          <c:if test="${not empty encodedWavBytes}">
          <div class="successWide">
            <strong>SUCCESS</strong>
          </div>
          <script type="text/javascript">
              audio = document.createElement("audio");
              audio.controls = true;
              audio.autoplay = true;
              audio.src = "data:audio/x-wav;base64,${encodedWavBytes}";
              var success = document.querySelector("div.successWide");
              success.appendChild(audio);
          </script>
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
            <br> <br> &copy; 2013 AT&amp;T Intellectual Property. All
            rights reserved. <a href="http://developer.att.com/"
            target="_blank">http://developer.att.com</a>
          </p>
        </div> <!-- end of ft -->
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <script type="text/javascript">
        function doLoad()
        {
            plainSpeechText = "${plainSpeechText}";
            ssmlSpeechText = "${ssmlSpeechText}";
            var contentElement = document.getElementById("speechText");
            setContent(contentElement, plainSpeechText);
        }
        function handleContentTypeChanged(contentTypeElement)
        {
            var contentElement = document.getElementById("speechText");
            var currentContent = contentElement.value;
            if (contentTypeElement.value === "text/plain") {
                ssmlSpeechText = currentContent;
                setContent(contentElement, plainSpeechText);
            }
            else {
                plainSpeechText = currentContent;
                setContent(contentElement, ssmlSpeechText);
            }
        }
        function setContent(contentElement, text)
        {
            contentElement.value = text;
            contentElement.innerText = text;
        }
    </script>
  </body>
</html>
