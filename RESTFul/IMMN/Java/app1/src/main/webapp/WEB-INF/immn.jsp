<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib uri="http://java.sun.com/jsp/jstl/functions" prefix="fn" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<html lang="en">
  <head>
    <title>AT&amp;T Sample Application - In App Messaging From Mobile Number</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8">
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
         : 'http://www') + '.google-analytics.com/ga.js';
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
         <div class="formBox">
           <div class="formContainer">

             <div class="lightBorder"></div>

             <a id="sendMsgToggle"
               href="javascript:toggle('sendMsg');">Send Message</a>
             <div class="toggle" id="sendMsg">
               <label><i>Max message size must be 1MB or less</i></label>
               <h2>Send Message</h2>
               <form method="post" action="index.jsp" id="sendMessageForm">
                 <div class="inputFields">
                   <c:choose>
                   <c:when test="${not empty address}">
                   <input placeholder="Address" value="${address}" name="address" type="text" />
                   </c:when>
                   <c:otherwise>
                   <input placeholder="Address" name="address" type="text" />
                   </c:otherwise>
                   </c:choose>
                   <label>Group:
                     <c:choose>
                     <c:when test="${not empty groupCheckBox}">
                     <input type="checkbox" name="groupCheckBox" checked />
                     </c:when>
                     <c:otherwise>
                     <input type="checkbox" name="groupCheckBox" />
                     </c:otherwise>
                     </c:choose>
                   </label>
                   <label>
                     Message: <i>max 200 characters are allowed</i>
                     <c:choose>
                     <c:when test="${not empty message}">
                     <input type="text" name="message" placeholder="ATT IMMN sample message" value="${message}" maxlength="200">
                     </c:when>
                     <c:otherwise>
                     <input type="text" name="message" placeholder="ATT IMMN sample message" maxlength="200">
                     </c:otherwise>
                     </c:choose>
                   </label>
                   <label>
                     Subject: <i>max 30 characters are allowed</i>
                     <c:choose>
                     <c:when test="${not empty subject}">
                     <input type="text" name="subject" placeholder="ATT IMMN APP" value="${subject}" maxlength="30">
                     </c:when>
                     <c:otherwise>
                     <input type="text" name="subject" placeholder="ATT IMMN APP" maxlength="30">
                     </c:otherwise>
                     </c:choose>
                   </label>
                   <label>
                     Attachment:
                     <select name="attachment">
                       <option value="None">None</option>
                       <option value="att.gif">att.gif</option>
                     </select>
                   </label>
                   <button name="sendMessage" type="submit" class="submit">Send Message</button>
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
                 <c:out value="${immnResponse.id}" />
               </div>
               </c:if>
             </div> <!-- end of send message -->

             <div class="lightBorder"></div>
             <label>
               <b>Note:</b> In order to use the following features, you must be subscribed to
               <a href="http://messages.att.net">AT&amp;T Messages</a>
             </label>
             <div class="lightBorder"></div>

             <a id="createMsgToggle"
               href="javascript:toggle('createMsg');">Create Message Index</a>
             <div class="toggle" id="createMsg">
               <h2>Create Message Index</h2>
               <form method="post" action="index.jsp" id="createMessageIndexForm">
                 <div class="inputFields">
                   <button name="createMessageIndex" type="submit" class="submit">Create Index</button>
                 </div>
               </form>
               <c:if test="${not empty indexError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${indexError}" />
               </div>
               </c:if>
               <c:if test="${not empty indexResponse}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               </c:if>
             </div> <!-- end of create message index -->

             <div class="lightBorder"></div>

             <a id="getMsgToggle"
               href="javascript:toggle('getMsg');">Get Message</a>
             <div class="toggle" id="getMsg">
               <h2>Get Message List <i>(Displays last 5 messages from the list)</i></h2>
               <form method="post" action="index.jsp" id="getMessageListForm">
                 <div class="inputFields">
                   <label>
                     <c:choose>
                     <c:when test="${not empty favorite}">
                     <input name="favorite" type="checkbox" checked />
                     </c:when>
                     <c:otherwise>
                     <input name="favorite" type="checkbox" />
                     </c:otherwise>
                     </c:choose>
                     Filter by favorite
                   </label>
                   <label>
                     <c:choose>
                     <c:when test="${not empty unread}">
                     <input name="unread" type="checkbox" checked />
                     </c:when>
                     <c:otherwise>
                     <input name="unread" type="checkbox" />
                     </c:otherwise>
                     </c:choose>
                     Filter by unread flag
                   </label>
                   <label>
                     <c:choose>
                     <c:when test="${not empty incoming}">
                     <input name="incoming" type="checkbox" checked />
                     </c:when>
                     <c:otherwise>
                     <input name="incoming" type="checkbox" />
                     </c:otherwise>
                     </c:choose>
                     Filter by incoming flag
                   </label>
                   <label>Filter by recipients:
                     <c:choose>
                     <c:when test="${not empty keyword}">
                     <input name="keyword" type="text" placeholder="555-555-5555, etc..." value="${keyword}">
                     </c:when>
                     <c:otherwise>
                     <input name="keyword" type="text" placeholder="555-555-5555, etc...">
                     </c:otherwise>
                     </c:choose>
                   </label>
                   <button name="getMessageList" type="submit" class="submit">Get Message List</button>
                 </div>
               </form>
               <c:if test="${not empty msgList}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               <table>
                 <thead>
                   <tr>
                     <th>limit</th>
                     <th>offset</th>
                     <th>total</th>
                     <th>state</th>
                     <th>cache status</th>
                     <th>failed messages</th>
                   </tr>
                 </thead>
                 <tbody>
                   <tr>
                     <td data-value="limit"><c:out value="${msgList.limit}"/></td>
                     <td data-value="offset"><c:out value="${msgList.offset}" /></td>
                     <td data-value="total"><c:out value="${msgList.total}" /></td>
                     <td data-value="state"><c:out value="${msgList.state}" /></td>
                     <td data-value="cache status"><c:out value="${msgList.cacheStatus.string}" /></td>
                     <td data-value="failed messages">
                       <c:choose>
                       <c:when test="${not empty msgList.failedMessages}" >
                       <c:out value='${fn:join(msgList.failedMessages, ", ")}' />
                       </c:when>
                       <c:otherwise>
                       None
                       </c:otherwise>
                       </c:choose>
                     </td>
                   </tr>
                 </tbody>
               </table>
                 <c:forEach items="${msgList.messages}" var="msg" varStatus="count">
                 <h3>Message <c:out value="${count.index}" /></h3>
                 <table>
                   <thead>
                     <tr>
                       <th>message id</th>
                       <th>from</th>
                       <th>recipients</th>
                       <th>text</th>
                       <th>timestamp</th>
                       <th>isFavorite</th>
                       <th>isUnread</th>
                       <th>isIncoming</th>
                       <th>type</th>
                       <!-- TODO: implement segment -->
                     </tr>
                   </thead>
                   <tbody>
                     <tr>
                       <td data-value="message id"><c:out value="${msg.messageId}" /></td>
                       <td data-value="from"><c:out value="${msg.from}" /></td>
                       <td data-value="recipients"><c:out value='${fn:join(msg.recipients, ", ")}' /></td>
                       <td data-value="text"><c:out value="${msg.text}" /></td>
                       <td data-value="timestamp"><c:out value="${msg.timeStamp}" /></td>
                       <td data-value="isFavorite"><c:out value="${msg.favorite}" /></td>
                       <td data-value="isUnread"><c:out value="${msg.unread}" /></td>
                       <td data-value="isIncoming"><c:out value="${msg.incoming}" /></td>
                       <td data-value="type"><c:out value="${msg.type}" /></td>
                       <!-- TODO: implement segment -->
                     </tr>
                   </tbody>
                 </table>
                 </c:forEach>
               </c:if>
               <c:if test="${not empty msgListError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${msgListError}" />
               </div>
               </c:if>

               <h2>Get Message</h2>
               <form method="post" action="index.jsp" id="getMessageForm">
                 <div class="inputFields">
                   <c:choose>
                   <c:when test="${not empty messageId}">
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" value="${messageId}" />
                   </c:when>
                   <c:otherwise>
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" />
                   </c:otherwise>
                   </c:choose>
                   <button name="getMessage" type="submit" class="submit">
                     Get Message
                   </button>
                 </div>
               </form>
               <c:if test="${not empty getMsg}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               <table>
                 <thead>
                   <th>message id</th>
                   <th>from</th>
                   <th>recipients</th>
                   <th>text</th>
                   <th>timestamp</th>
                   <th>isFavorite</th>
                   <th>isUnread</th>
                   <th>isIncoming</th>
                   <th>type</th>
                   <!-- TODO: implement segment -->
                 </thead>
                 <tbody>
                   <td data-value="message id"><c:out value="${getMsg.messageId}" /></td>
                   <td data-value="from"><c:out value="${getMsg.from}" /></td>
                   <td data-value="recipients"><c:out value='${fn:join(getMsg.recipients, ", ")}' /></td>
                   <td data-value="text"><c:out value="${getMsg.text}" /></td>
                   <td data-value="timestamp"><c:out value="${getMsg.timeStamp}" /></td>
                   <td data-value="isFavorite"><c:out value="${getMsg.favorite}" /></td>
                   <td data-value="isUnread"><c:out value="${getMsg.unread}" /></td>
                   <td data-value="isIncoming"><c:out value="${getMsg.incoming}" /></td>
                   <td data-value="type"><c:out value="${getMsg.type}" /></td>
                   <!-- TODO: implement segment -->
                 </tbody>
               </table>
               </c:if>
               <c:if test="${not empty getMsgError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${getMsgError}" />
               </div>
               </c:if>

               <h2>Get Message Content</h2>
               <form method="post" action="index.jsp" id="getMessageContentForm">
                 <div class="inputFields">
                   <c:choose>
                   <c:when test="${not empty messageId}">
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" value="${messageId}" />
                   </c:when>
                   <c:otherwise>
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" />
                   </c:otherwise>
                   </c:choose>

                   <c:choose>
                   <c:when test="${not empty partNumber}">
                   <input name="partNumber" type="text" maxlength="30" placeholder="Part Number" value="${partNumber}" />
                   </c:when>
                   <c:otherwise>
                   <input name="partNumber" type="text" maxlength="30" placeholder="Part Number" />
                   </c:otherwise>
                   </c:choose>
                   <button  type="submit" class="submit" name="getMessageContent">
                     Get Message Content
                   </button>
                 </div>
               </form>
               <c:if test="${not empty msgContent}" >
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               ${msgContent}
               </c:if>
               <c:if test="${not empty msgContentError}" >
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${msgContentError}" />
               </div>
               </c:if>

               <h2>Get Delta</h2>
               <form method="post" action="index.jsp" id="getDeltaForm">
                 <div class="inputFields">
                   <c:if test="${not empty state}" >
                   <input name="state" type="text" maxlength="30" placeholder="Message State" value="${state}" />
                   </c:if>
                   <c:if test="${empty state}">
                   <input name="state" type="text" maxlength="30" placeholder="Message State" />
                   </c:if>
                   <button  type="submit" class="submit" name="getDelta">
                     Get Delta
                   </button>
                 </div>
               </form>
               <c:if test="${not empty deltas}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               <c:forEach items="${deltas.deltas}" var="delta">
               <p><b>Delta type:</b> <c:out value="${delta.type}" /></p>
               <table>
                 <thead>
                   <tr>
                     <th>Delta Operation</th>
                     <th>MessageId</th>
                     <th>Favorite</th>
                     <th>Unread</th>
                   </tr>
                 </thead>
                 <tbody>
                   <c:forEach items="${delta.adds}" var="add">
                   <tr>
                     <td data-value="Delta Operation">Add</td>
                     <td data-value="MessageId"><c:out value="${add.messageId}" /></td>
                     <td data-value="Favorite"><c:out value="${add.favorite}" /></td>
                     <td data-value="Unread"><c:out value="${add.unread}" /></td>
                   </tr>
                   </c:forEach>
                   <c:forEach items="${delta.deletes}" var="del">
                   <tr>
                     <td data-value="Delta Operation">Delete</td>
                     <td data-value="MessageId"><c:out value="${del.messageId}" /></td>
                     <td data-value="Favorite"><c:out value="${del.favorite}" /></td>
                     <td data-value="Unread"><c:out value="${del.unread}" /></td>
                   </tr>
                   </c:forEach>
                   <c:forEach items="${delta.updates}" var="update">
                   <tr>
                     <td data-value="Delta Operation">Delete</td>
                     <td data-value="MessageId"><c:out value="${update.messageId}" /></td>
                     <td data-value="Favorite"><c:out value="${update.favorite}" /></td>
                     <td data-value="Unread"><c:out value="${update.unread}" /></td>
                   </tr>
                   </c:forEach>
                 </tbody>
               </table>
               </c:forEach>
               </c:if>
               <c:if test="${not empty getDeltaError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${getDeltaError}" />
               </div>
               </c:if>

               <h2> Get Message Index Info</h2>
               <form method="post" action="index.jsp" id="getMessageIndexInfoForm">
                 <div class="inputFields">
                   <button name="getMessageIndexInfo" type="submit" class="submit">
                     Get Message Index Info
                   </button>
                 </div>
               </form>
               <c:if test="${not empty msgIndexInfo}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               <table>
                 <thead>
                   <tr>
                     <th>Status</th>
                     <th>State</th>
                     <th>Message Count</th>
                   </tr>
                 </thead>
                 <tbody>
                   <tr>
                     <td data-value="Status"><c:out value="${msgIndexInfo.status.string}" /></td>
                     <td data-value="State"><c:out value="${msgIndexInfo.state}" /></td>
                     <td data-value="Message Count"><c:out value="${msgIndexInfo.messageCount}" /></td>
                   </tr>
                 </tbody>
               </table>
               </c:if>
               <c:if test="${not empty msgIndexInfoError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${msgIndexInfoError}" />
               </div>
               </c:if>

             </div> <!-- end of get message toggle -->

             <div class="lightBorder"></div>
             <a id="updateMsgToggle"
               href="javascript:toggle('updateMsg');">Update Message</a>
             <div class="toggle" id="updateMsg">
               <h2>Update Message/Messages</h2>
               <form method="post" action="index.jsp" id="updateMessageForm">
                 <div class="inputFields">
                   <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                   <c:choose>
                   <c:when test="${not empty messageId}">
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" value="${messageId}" />
                   </c:when>
                   <c:otherwise>
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" />
                   </c:otherwise>
                   </c:choose>
                   <label>Change Status:</label>
                   <label>
                     <input type="radio" name="readflag" value="read" checked>
                     Read
                   </label>
                   <label>
                     <input type="radio" name="readflag" value="unread">
                     Unread
                   </label>
                   <button name="updateMessage" type="submit" class="submit">
                     Update Message/Messages
                   </button>
                 </div>
               </form>

               <c:if test="${not empty updateMsg}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               </c:if>
               <c:if test="${not empty updateMsgError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${updateMsgError}" />
               </div>
               </c:if>
             </div> <!-- end of update message toggle -->

             <div class="lightBorder"></div>
             <a id="delMsgToggle" href="javascript:toggle('delMsg');">Delete Message</a>
             <div class="toggle" id="delMsg">
               <h2>Delete Message/Messages</h2>
               <form method="post" action="index.jsp" id="deleteMessageForm">
                 <div class="inputFields">
                   <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                   <c:choose>
                   <c:when test="${not empty messageId}">
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" value="${messageId}" />
                   </c:when>
                   <c:otherwise>
                   <input name="messageId" type="text" maxlength="30" placeholder="Message ID" />
                   </c:otherwise>
                   </c:choose>
                   <button name="deleteMessage" type="submit" class="submit">Delete Message/Messages</button>
                 </div>
               </form>

               <c:if test="${not empty deleteMsg}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               </c:if>
               <c:if test="${not empty deleteMsgError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${deleteMsgError}" />
               </div>
               </c:if>
             </div> <!-- end of delete toggle -->

             <div class="lightBorder"></div>

             <a id="getMsgNotToggle"
               href="javascript:toggle('getMsgNot');">Get Notification Connection Details</a>
             <div class="toggle" id="getMsgNot">
               <form method="post" action="index.jsp" id="getNotifyDetailsForm">
                 <div class="inputFields">
                   <label>Notification Subscription:</label>
                   <label><input type="radio" name="queues" value="TEXT" checked >Text</label>
                   <label><input type="radio" name="queues" value="MMS" >MMS</label>
                   <button name="getNotifyDetails" type="submit" class="submit">Get Details</button>
                 </div>
               </form>
               <c:if test="${not empty notificationDetails}">
               <div class="successWide">
                 <strong>SUCCESS:</strong>
               </div>
               <h3>Connection Details</h3>
               <table>
                 <thead>
                   <tr>
                     <th>Username</th>
                     <th>Password</th>
                     <th>https url</th>
                     <th>wss url</th>
                     <th>queues</th>
                   </tr>
                 </thead>
                 <tbody>
                   <tr>
                     <td data-value="Username"><c:out value="${notificationDetails.username}" /></td>
                     <td data-value="Password"><c:out value="${notificationDetails.password}" /></td>
                     <td data-value="https url"><c:out value="${notificationDetails.httpsUrl}" /></td>
                     <td data-value="wss url"><c:out value="${notificationDetails.wssUrl}" /></td>
                     <td data-value="queues"><c:out value="${notificationDetails.queues}" /></td>
                   </tr>
                 </tbody>
               </table>
               </c:if>
               <c:if test="${not empty notificationDetailsError}">
               <div class="errorWide">
                 <strong>ERROR:</strong>
                 <c:out value="${notificationDetailsError}" />
               </div>
               </c:if>
             </div> <!-- end of getMsgNot toggle -->
           </div> <!-- end of form container -->
         </div> <!-- end of form box -->
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
         &#169; 2013 AT&amp;T Intellectual Property. All rights reserved.
         <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
         </p>
       </div> <!-- end of footer -->
     </div> <!-- end of page_container -->
     <c:if test="${not empty toggleDiv}">
     <script>toggle("${toggleDiv}")</script>
     </c:if>
   </body>
 </html>
