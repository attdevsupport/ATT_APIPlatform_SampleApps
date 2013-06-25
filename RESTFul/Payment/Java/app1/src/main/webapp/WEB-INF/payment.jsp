<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<html lang="en"> 
    <head> 
        <title>AT&amp;T Sample Application - Payment</title>		
        <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
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
                 <div id="menuButton" class="hide">
                     <a id="jump" href="#nav">Main Navigation</a>
                 </div> 
                 <ul class="links" id="nav">
                     <li><a href="#" target="_blank">Full Page<img src="images/max.png" alt="images/max.png" /></a>
                     <span class="divider"> |&nbsp;</span>
                     </li>
                     <li>
                     <a href="${cfg.linkSource}" target="_blank">Source<img src="images/opensource.png" alt="images/opensource.png" /></a>
                     <span class="divider"> |&nbsp;</span>
                     </li>
                     <li>
                     <a href="${cfg.linkDownload}" target="_blank">Download<img src="images/download.png" alt="images/download.png" /></a>
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
                     <h1>AT&amp;T Sample Application - Payment</h1>
                     <div class="border"></div>
                     <div id="introtext">
                         <div><b>Server Time:</b><c:out value="${dateutil.serverTime}" /></div>
                         <div><b>Client Time:</b><script>document.write("" + new Date());</script></div>
                         <div><b>User Agent:</b><script>document.write("" + navigator.userAgent);</script></div>
                     </div> <!-- end of introtext -->
                 </div> <!-- end of contentHeading -->
                 <div class="lightBorder"></div>
                 <div class="formBox" id="formBox">
                     <div id="formContainer" class="formContainer">
                         <a id="transactionToggle" 
                             href="javascript:toggle('transaction','transactionToggle', 'Transaction');">Show Transaction</a>
                         <div class="toggle" id="transaction">
                             <h2>Feature 1: Create New Transaction</h2>
                             <form method="post" name="newTransaction" action="payment">
                                 <div class="inputFields">
                                     <input type="radio" name="product" value="1" 
                                     checked="checked">Buy product 1 for $<c:out value="${cfg.minTransactionAmount}" /><br>
                                     <input type="radio" name="product" 
                                     value="2">Buy product 2 for $<c:out value="${cfg.maxTransactionAmount}" /><br>
                                     <button type="submit" name="newTransaction">Buy Product</button>
                                 </div> <!-- end of inputFields -->
                             </form> <!-- end of newTransaction -->
                             <c:if test="${not empty errorNewTrans}">
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorNewTrans}" />
                             </div>
                             </c:if>
                             <br>
                             <h2>Feature 2: Get Transaction Status</h2>
                             <div class="inputFields">
                                 <strong>Merchant Transaction ID</strong><br>
                                 <form method="post" name="fgetTransactionTID" action="payment">
                                     <select name="getTransactionMTID" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="trans" items="${transactions}">
                                         <option value='<c:out value="${trans.merchantId}" />'><c:out value="${trans.merchantId}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                                 <strong>Auth Code</strong><br>
                                 <form method="post" name="fgetTransactionAuthCode" action="payment">
                                     <select name="getTransactionAuthCode" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="trans" items="${transactions}">
                                         <option value='<c:out value="${trans.authCode}" />'><c:out value="${trans.authCode}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                                 <strong>Transaction ID</strong><br>
                                 <form method="post" name="fgetTransactionTID" action="payment">
                                     <select name="getTransactionTID" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="trans" items="${transactions}">
                                         <option value='<c:out value="${trans.id}" />'><c:out value="${trans.id}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorTransInfo}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorTransInfo}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultTransInfo}">
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Transaction Status Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultTransInfo}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                             <br>
                             <h2>Feature 3: Refund Transaction</h2>
                             <div class="inputFields">
                                 <div id="refundTransIds">
                                     <strong>Transaction ID</strong><br>
                                     <form method="post" name="refundTransactionTID" action="payment">
                                         <select name="refundTransactionId" onChange="this.form.submit()">
                                             <option>Select..</option>
                                             <c:forEach var="entry" items="${transactions}">
                                             <option value="<c:out value="${entry.id}" />"><c:out value="${entry.id}" /></option>
                                             </c:forEach>
                                         </select>
                                     </form>
                                 </div> <!-- end of refundTransIds -->
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorTRefund}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorTRefund}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultTRefund}">
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Refund Status Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultTRefund}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                         </div> <!-- end of transaction -->
                         <div class="lightBorder"></div>
                         <a id="subscriptionToggle" 
                             href="javascript:toggle('subscription','subscriptionToggle', 'Subscription');">Show Subscription</a>
                         <div class="toggle" id="subscription">
                             <h2>Feature 1: Create New Subscription</h2>
                             <form method="post" name="newSubscription" action="payment">
                                 <div class="inputFields">
                                     <input type="radio" name="product" value="1" 
                                     checked="checked" />Subscribe for $<c:out value="${cfg.minSubscriptionAmount}" /> per month
                                     <br>
                                     <input type="radio" name="product" 
                                     value="2" />Subscribe for $<c:out value="${cfg.maxSubscriptionAmount}" /> per month
                                     <br>
                                     <button type="submit" name="newSubscription">Subscribe</button>
                                 </div> <!-- end of inputFields -->
                             </form> <!-- end of newSubscription -->
                             <br>
                             <h2>Feature 2: Get Subscription Status</h2>
                             <div class="inputFields">
                                 <strong>Merchant Transaction ID</strong><br>
                                 <form method="post" name="fgetSubscriptionTID" action="payment">
                                     <select name="getSubscriptionMTID" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="subs" items="${subscriptions}">
                                         <option value='<c:out value="${subs.merchantId}" />'><c:out value="${subs.merchantId}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                                 <strong>Auth Code</strong><br>
                                 <form method="post" name="fgetSubscriptionAuthCode" action="payment">
                                     <select name="getSubscriptionAuthCode" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="subs" items="${subscriptions}">
                                         <option value='<c:out value="${subs.authCode}" />'><c:out value="${subs.authCode}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                                 <strong>Subscription ID</strong><br>
                                 <form method="post" name="fgetSubscriptionTID" action="payment">
                                     <select name="getSubscriptionTID" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="subs" items="${subscriptions}">
                                         <option value='<c:out value="${subs.id}" />'><c:out value="${subs.id}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorSubInfo}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorSubInfo}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultSubInfo}">
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Subscription Status Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultSubInfo}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                             <br>
                             <h2>Feature 3: Get Subscription Details</h2>
                             <div class="inputFields">
                                 <strong>Merchant Subscription ID</strong><br>
                                 <form method="post" name="fgetSubscriptionTID" action="payment">
                                     <select name="getSDetailsMSID" onChange="this.form.submit()">
                                         <option>Select..</option>
                                         <c:forEach var="subs" items="${subscriptions}">
                                         <option value='<c:out value="${subs.merchantSubId}" />'><c:out value="${subs.merchantSubId}" /></option>
                                         </c:forEach>
                                     </select>
                                 </form>
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorSubDetail}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorSubDetail}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultSubDetail}">
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Subscription Details Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultSubDetail}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                             <br>
                             <h2>Feature 4: Cancel Future Subscription</h2>
                             <div class="inputFields">
                                 <div id="cancelIds">
                                    <strong>Subscription ID</strong><br>
                                    <form method="post" name="cancelSubscriptionID" action="payment">
                                        <select name="cancelSubscriptionId" onChange="this.form.submit()">
                                            <option>Select..</option>
                                            <c:forEach var="entry" items="${subscriptions}">
                                            <option value="<c:out value="${entry.id}" />"><c:out value="${entry.id}" /></option>
                                            </c:forEach>
                                        </select>
                                    </form>
                                 </div> <!-- end of cancelIds -->
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorSCancel}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorSCancel}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultSCancel}"> 
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Refund Status Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultSCancel}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                             <h2>Feature 5: Refund Current and Cancel Future Subscription</h2>
                             <div class="inputFields">
                                 <div id="refundSubIds">
                                     <strong>Subscription ID</strong><br>
                                     <form method="post" name="refundSubscriptionID" action="payment">
                                         <select name="refundSubscriptionId" onChange="this.form.submit()">
                                             <option>Select..</option>
                                             <c:forEach var="entry" items="${subscriptions}">
                                             <option value="<c:out value="${entry.id}" />"><c:out value="${entry.id}" /></option>
                                             </c:forEach>
                                         </select>
                                     </form>
                                 </div> <!-- end of refundSubIds -->
                             </div> <!-- end of inputFields -->
                             <c:if test="${not empty errorSRefund}"> 
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorSRefund}" />
                             </div>
                             </c:if>
                             <c:if test="${not empty resultSRefund}"> 
                             <div class="successWide">
                                 <strong>SUCCESS:</strong>
                                 <br>
                                 Refund Status Listed Below:
                             </div>
                             <table class="kvp" id="kvp">
                                 <thead>
                                     <tr>
                                         <th>Parameter</th>
                                         <th>Value</th>
                                     </tr>
                                 </thead>
                                 <tbody>
                                     <c:forEach var="kvp" items="${resultSRefund}">
                                     <tr>
                                         <td data-value="Parameter">
                                             <c:out value="${kvp.key}" />
                                         </td>
                                         <td data-value="Value">
                                             <c:choose>
                                                 <c:when test="${not empty kvp.value}" >
                                                     <c:out value="${kvp.value}" default="-" />
                                                 </c:when>
                                                 <c:otherwise>
                                                     <c:out value="-" />
                                                 </c:otherwise>
                                             </c:choose>
                                         </td>
                                     </tr>
                                     </c:forEach>
                                 </tbody>
                             </table>
                             </c:if>
                         </div> <!-- end of subscription -->
                         <div class="lightBorder"></div>
                         <a id="notaryToggle" href="javascript:toggle('notary','notaryToggle', 'Notary');">Show Notary</a>
                         <div class="toggle" id="notary">
                             <h2>Feature 1: Sign Payload</h2>
                             <form method="post" name="signContent" action="payment">
                                 <div class="inputFields">
                                     <label>Request:
                                         <textarea id="payload" name="payload" placeholder="Payload"><c:if test="${not empty notaryPayload}">${notaryPayload}</c:if></textarea>
                                     </label>
                                     <div id="notaryInfo">
                                         <strong>Signed Payload:</strong><br>
                                         <c:if test="${not empty resultNotaryDoc}">
                                         <c:out value="${resultNotaryDoc}" />
                                         </c:if>
                                         <br>
                                         <strong>Signature:</strong><br>
                                         <c:if test="${not empty resultNotarySig}">
                                         <c:out value="${resultNotarySig}" />
                                         </c:if>
                                         <br>
                                         <button type="submit" name="signPayload" value="signPayload">Sign Payload</button>
                                     </div> <!-- end of notaryInfo -->
                                 </div> <!-- end of inputFields -->
                             </form> <!-- end of signContent -->
                             <c:if test="${errorNotary}">
                             <div class="errorWide">
                                 <strong>ERROR: </strong><br>
                                 <c:out value="${errorNotary}" />
                             </div>
                             </c:if>
                         </div> <!-- end of notary -->
                         <div class="lightBorder"></div>
                         <a id="notificationToggle" href="javascript:toggle('notifications','notificationToggle', 'Notifications');">Show Notifications</a>
                         <div class="toggle" id="notifications">
                             <form method="post" name="refreshNotifications" action="payment">
                                 <div class="inputFields">
                                     <div id="notificationDetails" class="columns">
                                      <c:if test="${not empty notifications}">
                                          <c:forEach var="n" items="${notifications}" varStatus="count">
                                              <c:if test="${count.index eq 0}" >
                                                  <h2>Notification : <c:out value="${count.index + 1}" /> [Displays last 5 notifications]</h2>
                                              </c:if>
                                              <c:if test="${count.index ne 0}" >
                                                  <h2>Notification : <c:out value="${count.index + 1}" /> </h2>
                                              </c:if>
                                              ${n.htmlTable}
                                          </c:forEach>
                                      </c:if>
                                      <button type="submit" name="refreshNotifications">Refresh</button>
                                    </div> <!-- end of notificationDetails -->
                                </div> <!-- end of inputFields -->
                             </form> <!-- end of refreshNotifications -->
                           </div> <!-- end of notifications -->
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
                 For download of tools and documentation, please go to 
                 <a href="https://devconnect-api.att.com/" 
                     target="_blank">https://devconnect-api.att.com</a>
                 <br> For more information contact 
                 <a href="mailto:developer.support@att.com">developer.support@att.com</a>
                 <br><br>
                 &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. 
                 <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
                 </p>
             </div> <!-- end of footer -->
         </div> <!-- end of page_container -->
         <c:if test="${not empty showTrans}">
         <script>toggle('transaction', 'transactionToggle', 'Transaction');</script>
         </c:if>
         <c:if test="${not empty showSub}">
         <script>toggle('subscription', 'subscriptionToggle', 'Subscription');</script>
         </c:if>
         <c:if test="${not empty showNotary}">
         <script>toggle('notary', 'notaryToggle', 'Notary');</script>
         </c:if>
         <c:if test="${not empty showNote}">
         <script>toggle('notifications','notificationToggle', 'Notifications');</script>
         </c:if>
     </body>
 </html>
