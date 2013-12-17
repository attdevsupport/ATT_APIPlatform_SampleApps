<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SMS_App1" %>

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
    <title>AT&amp;T Sample Application - Basic SMS Service Application</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <meta http-equiv="refresh" content="300" />
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
                    AT&amp;T Sample Application - Basic SMS Service Application</h1>
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
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div class="inputFields">
                        <div id="sendSMSdiv">
                            <h2>
                                Feature 1: Send SMS</h2>
                            <input placeholder="Address" name="address" id="address" type="text" runat="server" />
                            <label>
                                Message
                                <asp:DropDownList ID="message" runat="server" name="message">
                                </asp:DropDownList>
                            </label>
                            <label>
                                <asp:CheckBox ID="chkGetOnlineStatus" runat="server" ToolTip="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                                Receive Delivery Status Notification<br />
                            </label>
                            <button type="submit" class="submit" name="sendSMS" id="sendSMS" runat="server" onserverclick="BtnSubmit_Click">
                                Send SMS</button>
                            <% if (!string.IsNullOrEmpty(sendSMSSuccessMessage))
                               { %>
                            <div class="successWide">
                                <strong>SUCCESS:</strong><br />
                                <strong>messageId: </strong>
                                <%= sendSMSResponseData.outBoundSMSResponse.messageId%>
                                <% if (sendSMSResponseData.outBoundSMSResponse.resourceReference != null)
                                   {%>
                                <br />
                                <strong>resourceURL: </strong>
                                <%= sendSMSResponseData.outBoundSMSResponse.resourceReference.resourceURL%>
                                <%} %>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(sendSMSErrorMessage))
                               { %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=sendSMSErrorMessage.ToString()%>
                            </div>
                            <% } %>
                        </div>
                        <!-- end of sendSMS -->
                        <div class="lightBorder">
                        </div>
                        <div id="getStatusdiv">
                            <h2>
                                Feature 2: Get Delivery Status</h2>
                            <input placeholder="Message ID" name="messageId" id="messageId" type="text" runat="server" />
                            <button type="submit" class="submit" name="getStatus" id="getStatus" runat="server"
                                onserverclick="GetDeliveryStatusButton_Click">
                                Get Status</button>
                            <% if (!string.IsNullOrEmpty(getSMSDeliveryStatusSuccessMessagae))
                               { %>
                            <div class="successWide">
                                <strong>SUCCESS: </strong>
                                <br />
                                <strong>ResourceUrl: </strong>
                                <%=getSMSDeliveryStatusResponseData.DeliveryInfoList.ResourceURL%><br />
                            </div>
                            <table>
                                <thead>
                                    <tr>
                                        <th>
                                            Id
                                        </th>
                                        <th>
                                            Address
                                        </th>
                                        <th>
                                            DeliveryStatus
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% foreach (DeliveryInfo delinfo in getSMSDeliveryStatusResponseData.DeliveryInfoList.DeliveryInfo)
                                       {%>
                                    <tr>
                                        <td data-value="Id">
                                            <%=delinfo.Id %>
                                        </td>
                                        <td data-value="Address">
                                            <%= delinfo.Address %>
                                        </td>
                                        <td data-value="DeliveryStatus">
                                            <%= delinfo.Deliverystatus %>
                                        </td>
                                    </tr>
                                    <% }%>
                                </tbody>
                            </table>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(getSMSDeliveryStatusErrorMessage))
                               { %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=getSMSDeliveryStatusErrorMessage.ToString()%>
                            </div>
                            <% } %>
                        </div>
                        <!-- end of getStatus -->
                        <div class="lightBorder">
                        </div>
                        <div id="receiveStatusdiv">
                            <h2>
                                Feature 3: Receive Delivery Status</h2>
                            <button type="submit" class="submit" name="receiveStatusBtn" id="receiveStatusBtn"
                                runat="server" onserverclick="receiveStatusBtn_Click">
                                Refresh Notifications</button>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>
                                                messageId
                                            </th>
                                            <th>
                                                address
                                            </th>
                                            <th>
                                                deliveryStatus
                                            </th>
                                        </tr>
                                    </thead>
                            <% if (receiveSMSDeliveryStatusResponseData != null && receiveSMSDeliveryStatusResponseData.Count > 0)
                               { %>
                                    <tbody>
                                        <% foreach (deliveryInfoNotification deinfo in receiveSMSDeliveryStatusResponseData)
                                           { %>
                                        <tr>
                                            <td data-value="messageId">
                                                <%=deinfo.messageId%>
                                            </td>
                                            <td data-value="address">
                                                <%=deinfo.deliveryInfo.address%>
                                            </td>
                                            <td data-value="deliveryStatus">
                                                <%=deinfo.deliveryInfo.deliveryStatus%>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                            <% } %>
                            </table>
                            <br />
                            <% if (!string.IsNullOrEmpty(receiveSMSDeliveryStatusErrorMessage))
                               { %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=receiveSMSDeliveryStatusErrorMessage.ToString()%>
                            </div>
                            <% } %>
                        </div>
                        <!-- end of receiveStatus -->
                        <div class="lightBorder">
                        </div>
                        <div id="getMessagesDiv">
                            <h2>
                                Feature 4: Get Messages (<%= offlineShortCode%>)</h2>
                            <%if (!string.IsNullOrEmpty(offlineShortCode))
                              {%>
                            <button type="submit" runat="server" class="submit" name="getMessages" id="getMessages" onserverclick="GetMessagesButton_Click">
                                Get Messages</button>
                            <%} %>
                            <% if (getSMSResponseData != null)
                               { %>
                            <div class="successWide">
                                <strong>SUCCESS:</strong><br />
                                <strong>Messages in this batch: </strong>
                                <%=getSMSResponseData.InboundSMSMessageList.NumberOfMessagesInThisBatch.ToString()%><br />
                                <strong>Messages pending: </strong>
                                <%= getSMSResponseData.InboundSMSMessageList.TotalNumberOfPendingMessages.ToString()%>
                            </div>
                            <br />
                            <table>
                                <thead>
                                    <tr>
                                        <th>
                                            Message Index
                                        </th>
                                        <th>
                                            Message Text
                                        </th>
                                        <th>
                                            Sender Address
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% foreach (InboundSMSMessage msg in getSMSResponseData.InboundSMSMessageList.InboundSMSMessage)
                                       {%>
                                    <tr>
                                        <td data-value="Message Index">
                                            <%=msg.MessageId.ToString() %>
                                        </td>
                                        <td data-value="Message Text">
                                            <%=msg.Message.ToString() %>
                                        </td>
                                        <td data-value="Sender Address">
                                            <%=msg.SenderAddress.ToString() %>
                                        </td>
                                    </tr>
                                    <%} %>
                                </tbody>
                            </table>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(getSMSErrorMessage))
                           { %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=getSMSErrorMessage.ToString()%>
                        </div>
                        <% } %>
                        <!-- end of getMessages -->
                        <div class="lightBorder">
                        </div>
                        <div id="votes">
                            <h2>
                                Feature 5: Receive Messages (<%= onlineShortCode.ToString() %>)
                            </h2>
                            <button type="submit" class="submit" name="receiveMessages" id="receiveMessages"
                                runat="server" onserverclick="receiveMessagesBtn_Click">
                                Refresh Received Messages</button>
                            <table>
                                <thead>
                                    <tr>
                                        <th>
                                            DateTime
                                        </th>
                                        <th>
                                            SenderAddress
                                        </th>
                                        <th>
                                            Message
                                        </th>
                                        <th>
                                            DestinationAddress
                                        </th>
                                        <th>
                                            MessageId
                                        </th>
                                    </tr>
                                </thead>
                            <% if (receivedSMSList != null && receivedSMSList.Count > 0)
                               {%>
                                <tbody>
                                    <% foreach (ReceiveSMS msg in receivedSMSList)
                                       { %>
                                    <tr>
                                        <td data-value="DateTime">
                                            <%=msg.DateTime.ToString() %>
                                        </td>
                                        <td data-value="SenderAddress">
                                            <%=msg.SenderAddress.ToString() %>
                                        </td>
                                        <td data-value="Message">
                                            <%=msg.Message.ToString() %>
                                        </td>
                                        <td data-value="DestinationAddress">
                                            <%=msg.DestinationAddress.ToString() %>
                                        </td>
                                        <td data-value="MessageId">
                                            <% var mid = "-"; if (!string.IsNullOrEmpty(msg.MessageId))
                                               {
                                                   mid = msg.MessageId.ToString();
                                               }%>
                                            <%= mid.ToString() %>
                                        </td>
                                    </tr>
                                    <%} %>
                                </tbody>
                            <% } %>
                            </table>
                            <% if (!string.IsNullOrEmpty(receiveSMSErrorMesssage))
                               { %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=receiveSMSErrorMesssage.ToString()%>
                            </div>
                            <% } %>
                        </div>
                        <!-- end of votes -->
                    </div>
                    <!-- end of inputFields -->
                </div>
                <!-- end of formContainer -->
            </div>
            <!-- end of formBox -->
        </div>
        <!-- end of content -->
        <div class="border">
        </div>
        <div id="footer">
            <div id="powered_by">
                Powered by AT&amp;T Cloud Architecture</div>
            <p>
                The Application hosted on this site are working examples intended to be used for
                reference in creating products to consume AT&amp;T Services and not meant to be
                used as part of your product. The data in these pages is for test purposes only
                and intended only for use as a reference in how the services perform.
                <br />
                <br />
                For download of tools and documentation, please go to <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a>
                <br />
                <br />
                &copy; 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
            </p>
        </div>
        <!-- end of footer -->
    </div>
    <!-- end of page_container -->
    </form>
    <script type="text/javascript">        setup();</script>
</body>
</html>
