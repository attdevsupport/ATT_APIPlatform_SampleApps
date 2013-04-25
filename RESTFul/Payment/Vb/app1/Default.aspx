<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Payment_App1" %>

<!DOCTYPE html>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<head>
    <title>AT&amp;T Sample Application - Payment</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
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
    <script type="text/javascript" src="scripts/utils.js"></script>
</head>
<body onload="setup()">
    <form id="form1" runat="server">
    <div id="pageContainer">
        <div id="header">
            <div class="logo">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a></div>
            <ul class="links" id="nav">
                <li><a href="#" target="_blank">Full Page<img alt="fullpage" src="images/max.png" /></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="SourceLink">Source<img alt="source" src="images/opensource.png" /></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="DownloadLink">Download<img alt="download"
                    src="images/download.png" /></a> <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="HelpLink">Help</a> </li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
            <!-- end of links -->
        </div>
        <!-- end of header -->
        <div id="content">
            <div id="contentHeading">
                <h1>
                    AT&amp;T Sample Application - Payment</h1>
                <div class="border">
                </div>
                <div id="introtext">
                    <div>
                        <b>Server Time:&nbsp;</b><%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %></div>
                    <div>
                        <b>Client Time:</b>
                        <script language="JavaScript" type="text/javascript">
                            var myDate = new Date();
                            document.write(myDate);
                        </script>
                    </div>
                    <div>
                        <b>User Agent:</b>
                        <script language="JavaScript" type="text/javascript">
                            document.write("" + navigator.userAgent);
                        </script>
                    </div>
                </div>
                <!-- end of introtext -->
            </div>
            <!-- end of contentHeading -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="transactionToggle" 
              href="javascript:toggle('transaction','transactionToggle', 'Transaction');">Show Transaction</a>
            <div class="toggle" id="transaction">
              <h2>Feature 1: Create New Transaction</h2>
                <div class="inputFields">
                  <input type="radio" name="product" id="product_1"
                  value="1"  runat="server" />Buy product 1 for $<%= MinTransactionAmount %><br/>
                  <input type="radio" name="product" id="product_2"
                  value="2" runat="server"/>Buy product 2 for $<%= MaxTransactionAmount %><br/>
                  <button id="newTransaction" type="submit" name="newTransaction" runat="server" onserverclick="createNewTransaction">Buy Product</button>
                </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(new_transaction_error) Then%>
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%=new_transaction_error %>
                </div>
              <% End If%>

              <h2>Feature 2: Get Transaction Status</h2>
              <div class="inputFields">
                  <strong>Merchant Transaction ID</strong><br/>
                        <asp:DropDownList ID="getTransactionMTID" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetTransactionStatusForMerTranId_Click" name="getTransactionMTID">
                        </asp:DropDownList>
                        <br />
                  <strong>Auth Code</strong><br/>
                        <asp:DropDownList ID="getTransactionAuthCode" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetTransactionStatusForAuthCode_Click" name="getTransactionAuthCode">
                        </asp:DropDownList>
                        <br />
                  <strong>Transaction ID</strong><br/>
                  <asp:DropDownList ID="getTransactionTID" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetTransactionStatusForTransactionId_Click" name="getTransactionTID">
                        </asp:DropDownList>
              </div> <!-- end of inputFields -->

              <% If Not String.IsNullOrEmpty(transaction_status_error) Then%> 
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%= transaction_status_error %>
                </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(transaction_status_success) Then%>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <br/>
                  Transaction Status Listed Below:
                </div>
                <table class="kvp" id="tkvp">
                  <thead>
                    <tr>
                      <th>Parameter</th>
                      <th>Value</th>
                    </tr>
                  </thead>
                  <tbody>
                            <% For Each pair As KeyValuePair(Of String, String) In getTransactionStatusResponse%>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                  </tbody>
                </table>
              <% End If%>
              <br/>
              <h2>Feature 3: Refund Transaction</h2>
              <div class="inputFields">
                <div id="trefundIds">
                  <div id="tttransactionIds">
                    <strong>Transaction ID</strong><br/>
                        <asp:DropDownList ID="refundTransactionId" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "RefundTransaction_Click" name="refundTransactionId">
                        </asp:DropDownList>
                  </div> <!-- end of transactionIds -->
                  <br/>
                </div> <!-- end of refundIds -->
              </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(refund_error) Then%> 
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%= refund_error %>
                </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(refund_success) Then%>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <br/>
                  Refund Status Listed Below:
                </div>
                <table class="kvp" id="rkvp">
                  <thead>
                    <tr>
                      <th>Parameter</th>
                      <th>Value</th>
                    </tr>
                  </thead>
                  <tbody>
                            <% For Each pair As KeyValuePair(Of String, String) In refundResponse%>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                  </tbody>
                </table>
              <%End If%>
              <h2>Feature 4: Transaction Notifications</h2>
                <div class="inputFields">
                  <div id="tnotificationDetails" class="columns">
                    <div id="tnotificationIds" class="column">
                      <strong>Notification ID</strong><br/>
                    </div> <!-- end of notificationIds -->
                    <div id="tnotificationTypes" class="column">
                      <strong>Notification Type</strong><br/>
                    </div> <!-- end of notificationTypes -->
                    <div id="ttransactionIds" class="column">
                      <strong>Transaction ID</strong><br/>
                    </div> <!-- end of transactionIds -->
                    <br/>
                    <button type="submit" name="refreshTransactionNotifications">Refresh</button>
                  </div> <!-- end of notificationDetails -->
                </div> <!-- end of inputFields -->
            </div> <!-- end of transaction -->

            <div class="lightBorder"></div>

            <a id="subscriptionToggle" 
              href="javascript:toggle('subscription','subscriptionToggle', 'Subscription');">Show Subscription</a>
            <div class="toggle" id="subscription">
              <h2>Feature 1: Create New Subscription</h2>
                <div class="inputFields">
                  <input type="radio" name="subproduct" value="1" 
                  runat="server" id="subproduct_1" />Subscribe for $<%= MinSubscriptionAmount %> per month
                  <br/>
                  <input type="radio" name="subproduct" 
                  value="2" runat="server" id="subproduct_2"/>Subscribe for $<%= MaxSubscriptionAmount %> per month
                  <br/>
                  <button id="newSubscription" type="submit" name="newSubscription" runat="server" onserverclick="createNewSubscription">Subscribe</button>
                </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(new_subscription_error) Then%>
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%= new_subscription_error %>
                </div>
              <% End If%>
              <br/>
              <h2>Feature 2: Get Subscription Status</h2>
              <div class="inputFields">
                  <strong>Merchant Transaction ID</strong><br/>
                        <asp:DropDownList ID="getSubscriptionMTID" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetSubscriptionStatusForMerTranId_Click" name="getTransactionMTID">
                        </asp:DropDownList>
                        <br />
                  <strong>Auth Code</strong><br/>
                        <asp:DropDownList ID="getSubscriptionAuthCode" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetSubscriptionStatusForAuthCode_Click" name="getTransactionAuthCode">
                        </asp:DropDownList>
                        <br />
                  <strong>Subscription ID</strong><br/>
                  <asp:DropDownList ID="getSubscriptionTID" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetSubscriptionStatusForTransactionId_Click" name="getSubscriptionTID">
                        </asp:DropDownList>
              </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(subscription_status_error) Then%> 
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%= subscription_status_error %>
                </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(subscription_status_success) Then%>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <br/>
                  Subscription Status Listed Below:
                </div>
                <table class="kvp" id="sskvp">
                  <thead>
                    <tr>
                      <th>Parameter</th>
                      <th>Value</th>
                    </tr>
                  </thead>
                  <tbody>
                            <% For Each pair As KeyValuePair(Of String, String) In getSubscriptionStatusResponse
                               %>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                  </tbody>
                </table>
              <% End If%>
              <br/>
              <h2>Feature 3: Get Subscription Details</h2>
              <div class="inputFields">
                  <strong>Merchant Subscription ID</strong><br/>
                  <asp:DropDownList ID="getSDetailsMSID" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "GetSubscriptionDetails_Click" name="getSDetailsMSID">
                        </asp:DropDownList>
              </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(subscription_details_error) Then%> 
                <div class="errorWide">
                  <strong>ERROR: </strong><br/>
                  <%= subscription_details_error %>
                </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(subscription_details_success) Then%>
                <div class="successWide">
                  <strong>SUCCESS:</strong>
                  <br/>
                  Subscription Details Listed Below:
                </div>
                <table class="kvp" id="kvp1">
                  <thead>
                    <tr>
                      <th>Parameter</th>
                      <th>Value</th>
                    </tr>
                  </thead>
                  <tbody>
                            <% For Each pair As KeyValuePair(Of String, String) In getSubscriptionDetailsResponse
                               %>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                  </tbody>
                </table>
              <% End If%>
              <br/>
              <h2>Feature 4: Cancel Subscription</h2>
              <div class="inputFields">
                <div id="cancelIds">
                  <div id="cancelTransactionIds">
                    <strong>Subscription ID</strong><br/>
                        <asp:DropDownList ID="cancelSubscriptionId" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "CancelSubscription_Click" name="cancelSubscriptionId">
                        </asp:DropDownList>
                  </div> <!-- end of transactionIds -->
                  <br/>
                </div> <!-- end of refundIds -->
              </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(subscription_cancel_error) Then%> 
              <div class="errorWide">
                <strong>ERROR: </strong><br/>
                <%= subscription_cancel_error%>
              </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(subscription_cancel_success) Then%>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br/>
                Cancel Status Listed Below:
              </div>
              <table class="kvp" id="ckvp">
                <thead>
                  <tr>
                    <th>Parameter</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                                            <% For Each pair As KeyValuePair(Of String, String) In subscriptionRefundResponse
                               %>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                </tbody>
              </table>
              <%End If%>
              <h2>Feature 5: Refund Subscription</h2>
              <div class="inputFields">
                <div id="refundIds">
                  <div id="12transactionIds">
                    <strong>Subscription ID</strong><br/>
                        <asp:DropDownList ID="refundSubscriptionId" runat="server" AutoPostBack="true" OnSelectedIndexChanged= "RefundSubscription_Click" name="refundSubscriptionId">
                        </asp:DropDownList>
                  </div> <!-- end of transactionIds -->
                  <br/>
                </div> <!-- end of refundIds -->
              </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(subscription_refund_error) Then%> 
              <div class="errorWide">
                <strong>ERROR: </strong><br/>
                <%= subscription_refund_error %>
              </div>
              <% End If%>
              <% If Not String.IsNullOrEmpty(subscription_refund_success) Then%>
              <div class="successWide">
                <strong>SUCCESS:</strong>
                <br/>
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
                                            <% For Each pair As KeyValuePair(Of String, String) In subscriptionRefundResponse
                               %>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% Next%>
                </tbody>
              </table>
              <%End If%>
              <h2>Feature 6: Subscription Notifications</h2>
                <div class="inputFields">
                  <div id="notificationDetails" class="columns">
                    <div id="notificationIds" class="column">
                      <strong>Notification ID</strong><br/>
                    </div> <!-- end of notificationIds -->
                    <div id="notificationTypes" class="column">
                      <strong>Notification Type</strong><br/>
                    </div> <!-- end of notificationTypes -->
                    <div id="transactionIds" class="column">
                      <strong>Transaction ID</strong><br/>
                    </div> <!-- end of transactionIds -->
                    <br/>
                    <button type="submit" name="refreshSubscriptionNotifications">Refresh</button>
                  </div> <!-- end of notificationDetails -->
                </div> <!-- end of inputFields -->
            </div> <!-- end of subscription -->
            <div class="lightBorder"></div>
            <a id="notaryToggle" href="javascript:toggle('notary','notaryToggle', 'Notary');">Show Notary</a>
            <div class="toggle" id="notary">
              <h2>Feature 1: Sign Payload</h2>
                <div class="inputFields">
                  <label>Request:
                    <textarea id="payload" name="payload" placeholder="Payload" runat="server"></textarea>
                  </label>
                  <div id="notaryInfo">
                    <strong>Signed Payload:</strong><br/>
                    <% If Not String.IsNullOrEmpty(signedPayload) Then%>
                      <%= signedPayload %>
                    <% End If%>
                    <br/>
                    <strong>Signature:</strong><br/>
                      <%= signedSignature %>
                    <br/>
                    <button id="signPayload" type="submit" name="signPayload" value="signPayload" runat="server" onserverclick="Notary_Click">Sign Payload</button>
                  </div> <!-- end of notaryInfo -->
                </div> <!-- end of inputFields -->
              <% If Not String.IsNullOrEmpty(notary_error) Then%>
              <div class="errorWide">
                <strong>ERROR: </strong><br/>
                <%= notary_error %>
              </div>
              <% End If%>
            </div> <!-- end of notary -->

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
        <br/><br/>
        For download of tools and documentation, please go to 
        <a href="https://developer.att.com/" 
          target="_blank">https://developer.att.com</a>
        <br/> For more information contact 
        <a href="mailto:developer.support@att.com">developer.support@att.com</a>
        <br/><br/>
        &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. 
        <a href="https://developer.att.com/" target="_blank">https://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
    <% If Not String.IsNullOrEmpty(showTransaction) Then%>
    <script type="text/javascript">        toggle('transaction', 'transactionToggle', 'Transaction');</script>
    <% End If%>
    <% If Not String.IsNullOrEmpty(showSubscription) Then%>
    <script type="text/javascript">        toggle('subscription', 'subscriptionToggle', 'Subscription');</script>
    <% End If%>
    <% If Not String.IsNullOrEmpty(showNotary) Then%>
    <script type="text/javascript">        toggle('notary', 'notaryToggle', 'Notary');</script>
    <% End If%>
    </form>
  </body>
</html>
