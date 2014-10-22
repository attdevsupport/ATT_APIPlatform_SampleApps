<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<html lang="en">
  <head>
    <title>AT&amp;T Sample Speech Application - Speech to Text (Custom)</title>
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

      function enableNameParam(list,nameParam){
        var selectedValue = list.options[list.selectedIndex].value;
        if(selectedValue == "GenericHints"){
          document.getElementById("nameParam").disabled=false;
        }else{
          document.getElementById("nameParam").disabled=true;
          var choices = document.getElementById("nameParam");
          choices.options[0].selected = true;
        }
      }
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
          <h1>AT&amp;T Sample Application - Speech to Text Custom</h1>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><c:out value="${dateutil.serverTime}" /></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <form name="SpeechToText" action="SpeechAction" method="post">
              <div id="formData">
                <h3>Speech Context:</h3>
                <select name="SpeechContext" onchange="enableNameParam(this,'nameParam')">
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
                <h3>Name Parameter:</h3>
                <select name="nameParam" id="nameParam">
                  <option value="x-grammar" selected="selected">x-grammar</option>
                  <option value="x-grammar-prefix">x-grammar-prefix</option>  
                  <option value="x-grammar-altgram">x-grammar-altgram</option>            
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
                <h3>X-Arg:</h3>
                <label>
                    ${xArg}
                </label>
                <br>
                <h3>MIME Data:</h3>
                <textarea id="mimeData" name="mimeData" readonly="readonly" rows="4">${mimeData}</textarea>
                <br>
                <button type="submit" id="SpeechToTextCustom" name="SpeechToTextCustom" id="btnSubmit">Submit</button>
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
                <th>ResponseId</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td data-value="ResponseId">
                  <em> <c:out value="${resultSpeech.responseId}" /></em>
                </td>
                <td data-value="Status">
                  <em><c:out value="${resultSpeech.status}" /></em>
                </td>
              </tr>
            </table>
            <c:if test="${not empty resultSpeech.nbests}">
            NBests:
            <c:forEach var="nbest" items="${resultSpeech.nbests}">
            <table>
            <thead>
              <tr>
                <th>Hypothesis</th>
                <th>LanguageId</th>
                <th>Confidence</th>
                <th>Grade</th>
                <th>ResultText</th>
                <th>Words</th>
                <th>WordScores</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td data-value="Hypothesis">
                  <em><c:out value="${nbest.hypothesis}" /></em>
                </td>
                <td data-value="LanguageId">
                  <em><c:out value="${nbest.languageId}" /></em>
                </td>
                <td data-value="Confidence">
                  <em><c:out value="${nbest.confidence}" /></em>
                </td>
                <td data-value="Grade">
                  <em><c:out value="${nbest.grade}" /></em>
                </td>
                <td data-value="ResultText">
                  <em><c:out value="${nbest.resultText}" /></em>
                </td>
                <td data-value="Words">
                  <em>  
                    <c:forEach var="word" items="${nbest.words}" varStatus="status">
                    <c:out value="${word}" />
                    <c:if test="${!status.last}">,</c:if>
                    </c:forEach>
                  </em>
                </td>
                <td data-value="WordScores">
                  <em> 
                    <c:forEach var="score" items="${nbest.wordScores}" varStatus="status">
                    <c:out value="${score}" />
                    <c:if test="${!status.last}">,</c:if>
                    </c:forEach>
                  </em>
                </td>
                </c:forEach>
                </c:if>
              </tr>
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
            as a reference in how the services perform.
            <br> <br>
            To access your apps, please go to
            <a href="https://developer.att.com/developer/mvc/auth/login"
            target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
            <br> For support refer to
            <a href="https://developer.att.com/support">https://developer.att.com/support</a>
            <br> <br>&copy; 2014 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
            target="_blank">http://developer.att.com</a>
          </p>
        </div> <!-- end of ft -->
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>


