<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ page language="java" contentType="text/html; charset=ISO-8859-1" pageEncoding="ISO-8859-1"%>
<%@ include file="config.jsp" %>    
<%@ page import="org.json.*" %>
<%@ page import="com.att.api.immn.model.IMMNResponse" %>
<%@ page import="com.att.api.mim.model.MIMResponse" %>
<%@ page import="com.att.api.mim.model.MIMContentResponse" %>
<%@ page import="com.att.api.immn.controller.IMMNController" %>
<%@ page import="com.att.api.mim.controller.MIMHeaderController" %>
<%@ page import="com.att.api.mim.controller.MIMContentController" %>
<%@ page import="com.att.api.util.DateUtil" %>
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
          <a href="<%= cfg.linkSource %>" target="_blank">Source<img src="images/opensource.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<%= cfg.linkDownload %>" target="_blank">Download<img src="images/download.png"></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<%= cfg.linkHelp %>" target="_blank">Help</a>
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
            <div><b>Server Time:</b> <%= new DateUtil().getServerTime() %></div>
            <div><b>Client Time:</b> <script>document.write("" + new Date());</script></div>
            <div><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- SAMPLE APP CONTENT STARTS HERE! -->

        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <div id="sendMessages">
              <h2>Send Messages:</h2>
              <form method="post" action="sendMessage" name="msgContentForm" >
                <div class="inputFields">
                  <input placeholder="Address" name="Address" type="text" />     
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
                  <button type="submit" class="submit">Send Message</button>
                </div>
              </form>
              <% 
              IMMNResponse sendResponse = (IMMNResponse) request.getAttribute(IMMNController.NAME);
              if (sendResponse != null && !sendResponse.hasError()){ %>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <%= sendResponse.getId() %>
              </div>
              <% } else if (sendResponse != null && sendResponse.hasError()) { %>
              <div class="errorWide">
                <strong>ERROR:</strong>
                <%= sendResponse.getError() %>
              </div>
              <% } %>
            </div> <!-- end of sendMessages -->

            <div class="lightBorder"></div>

            <div id="getMessages">
              <h2>Read Messages:</h2>
              <form method="post" action="submitGetHeaders" name="msgHeaderForm" id="msgHeaderForm">
                <div class="inputFields">
                  <input name="headerCountTextBox" type="text" maxlength="3" placeholder="Header Counter" />     
                  <input name="indexCursorTextBox" type="text" maxlength="30" placeholder="Index Cursor" />     
                  <button type="submit" class="submit" name="getMessageHeaders">Get Message Headers</button>
                </div>
              </form>

              <form method="post" action="submitGetHeaderContent" name="msgContentForm" id="msgContentForm">
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
          <% 
          MIMContentResponse content = (MIMContentResponse) request.getAttribute(MIMContentController.NAME);
          if (content != null && !content.hasError()) { 
          %>
          <div class="successWide">
            <strong>SUCCESS:</strong>
          </div>
          <% if (content.getType().contains("TEXT")) { %>
          <%= content.getContent() %>
          <% } else if (content.getType().contains("APPLICATION/SMIL")) { %>
          <textarea name="TextBox1" rows="2" cols="20" id="TextBox1" disabled="disabled">
            <%= content.getContent() %>
          </textarea>
          <% } else if (content.getType().contains("IMAGE")) { %>
          <img src="data:<%= content.getType() %>;base64,<%= content.getImage() %>" />
          <% } %>
          <% } else if (content != null && content.hasError()) { %>
          <div class="errorWide">
            <strong>Error:</strong>
            <%= content.getError() %>
          </div>
          <% } %>
          <!-- END HEADER CONTENT RESULTS -->

          <!-- BEGIN HEADER RESULTS -->
          <% 
          MIMResponse mimResponse = (MIMResponse)request.getAttribute(MIMHeaderController.NAME);
          if ( mimResponse != null && !mimResponse.hasError()) {
          //out.write(mimResponse.toString()); 
          %>
          <div class="successWide">
            <strong>SUCCESS:</strong>
          </div>
          <p id="headerCount">Header Count: <%= mimResponse.getHeaders().length() %></p>
          <p id="indexCursor">Index Cursor: <%= mimResponse.getIndexCursor() %></p>
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
              <%
              JSONArray mim = mimResponse.getHeaders();
              if (mim != null){
              for (int i = 0; i < mim.length(); ++i) { 
              JSONObject mimHeaders = mim.getJSONObject(i);
              String rowId = "row" + i;
              %>
              <tr id="<%= rowId %>">
                <td data-value="MessageId">
                  <%= mimHeaders.getString("MessageId") %>
                </td>
                <td data-value="From">
                  <%= mimHeaders.getString("From") %>
                </td>
                <td data-value="To">
                  <%= mimHeaders.getString("To") %>
                </td>
                <td data-value="Received">
                  <%= mimHeaders.getString("Received") %>
                </td>
                <td data-value="Text">
                  <% if (mimHeaders.has("Text")) { %>
                  <%= mimHeaders.getString("Text") %>
                  <% } else { %>
                  &#45;
                  <% } %>
                </td>
                <td data-value="Favorite">
                  <%= mimHeaders.getString("Favorite") %>
                </td>
                <td data-value="Read">
                  <%= mimHeaders.getString("Read") %>
                </td>
                <td data-value="Type">
                  <%= mimHeaders.getString("Type") %>
                </td>
                <td data-value="Direction">
                  <%= mimHeaders.getString("Direction") %>
                </td>
                <td data-value="Contents">
                  <% 
                  if (mimHeaders.has("MmsContent")) {
                  JSONArray mmsContent = mimHeaders.getJSONArray("MmsContent");
                  %>
                  <select id="attachments" onchange='chooseSelect("<%=rowId%>",this)'>
                    <option value="More..">More..</option>
                    <% 
                    for (int j = 0; j < mmsContent.length(); ++j) { 
                    JSONObject current = mmsContent.getJSONObject(j);
                    %>
                    <option value='<%= current.getString("PartNumber").trim() + " - " + current.getString("ContentName") + " - " + current.getString("ContentType") %>'>
                    <%= current.getString("PartNumber").trim() + " - " + current.getString("ContentName") + " - " + current.getString("ContentType") %>
                    </option>
                    <% } %>
                  </select>
                  <% } else { %>
                  &#45;
                  <% } %>
                </td>
              </tr>
              <% }
              } %>
            </tbody>
          </table>
          <!-- END HEADER result -->

          <% } else if (mimResponse != null && mimResponse.hasError()){ %>
          <div class="errorWide">
            <strong>ERROR:</strong>
            <%= mimResponse.getError() %>
          </div>
          <% } %>
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
