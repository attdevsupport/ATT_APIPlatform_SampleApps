<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="MMS_App1" %>

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
    <title>AT&amp;T Sample Application - Multimedia Messaging Service</title>
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
                    AT&amp;T Sample Application - Multimedia Messaging Service</h1>
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
            <div class="lightBorder">
            </div>
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="sendMMS">
                        <h2>
                            Feature 1: Send MMS Message</h2>
                        <div class="inputFields">
                            <input name="address" placeholder="Address" runat="server" id="address" />
                            <label>
                                Message:
                                <asp:DropDownList name="subject" id="subject" runat="server">
                                <asp:ListItem>MMS Sample Message</asp:ListItem>
                                </asp:DropDownList>
                            </label>
                            <label>
                                Attachment:
                                <asp:DropDownList ID="attachment" runat="server" name="attachment">
                                </asp:DropDownList>
                            </label>
                            <label>
                                <asp:CheckBox ID="chkGetOnlineStatus" name="chkGetOnlineStatus" runat="server" title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                                Receive Delivery Status Notification<br />
                            </label>
                            <button type="submit" class="submit" onserverclick="SendMessage_Click" runat="server"
                                name="sendMms">
                                Send MMS Message</button>
                        </div>
                        <!-- end of inputFields -->
                    </div>
                    <!-- end of sendMMS -->
                    <% if (!string.IsNullOrEmpty(sendMessageResponseError))
                       { %>
                    <div class="errorWide">
                        <strong>ERROR: </strong>
                        <br />
                        <%= sendMessageResponseError %>
                    </div>
                    <% } %>
                    <% if (!string.IsNullOrEmpty(sendMessageResponseSuccess))
                       { %>
                    <div class="successWide">
                        <strong>SUCCESS:</strong><br />
                        <strong>messageId: </strong>
                        <%= sendMMSResponseData.outboundMessageResponse.messageId%>
                        <% if (sendMMSResponseData.outboundMessageResponse.resourceReference != null)
                            {%>
                        <br />
                        <strong>resourceURL: </strong>
                        <%= sendMMSResponseData.outboundMessageResponse.resourceReference.resourceURL%>
                        <%} %>
                    </div>
                    <% } %>
                    <div class="lightBorder">
                    </div>
                    <div id="getDeliveryStatus">
                        <h2>
                            Feature 2: Get Delivery Status</h2>
                        <div class="inputFields">
                            <input maxlength="20" name="mmsId" placeholder="Message ID" runat="server" id="mmsId" />
                            <button type="submit" class="submit" name="getStatus" onserverclick="GetStatus_Click"
                                runat="server" id="getStatus">
                                Get Status</button>
                        </div>
                        <!-- end of inputFields -->
                    </div>
                    <!-- end of getDeliveryStatus -->
                    <% if (!string.IsNullOrEmpty(getDeliveryStatusResponseError))
                       { %>
                    <div class="errorWide">
                        <strong>ERROR: </strong>
                        <br />
                        <%= getDeliveryStatusResponseError %>
                    </div>
                    <%} %>
                    <% if (!string.IsNullOrEmpty(getDeliveryStatusResponseSuccess))
                       { %>
                            <div class="successWide">
                                <strong>SUCCESS: </strong>
                                <br />
                                <strong>ResourceURL </strong>
                                <%=getMMSDeliveryStatusResponseData.DeliveryInfoList.ResourceURL%><br />
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
                                    <% foreach (DeliveryInfo delinfo in getMMSDeliveryStatusResponseData.DeliveryInfoList.DeliveryInfo)
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
                            <% if (receiveMMSDeliveryStatusResponseData != null && receiveMMSDeliveryStatusResponseData.Count > 0)
                               { %>
                                    <tbody>
                                        <% foreach (deliveryInfoNotification deinfo in receiveMMSDeliveryStatusResponseData)
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
                        </div>
                    <div class="lightBorder">
                    </div>
                    <div id="webGallery">
                        <h2>
                            Feature 4: Web gallery of MMS photos sent to short code</h2>
                        <p>
                            Photos sent to short code
                            <%= ListenerShortCode %>
                            :
                            <%= totalImages.ToString() %></p>
                        <% foreach (ImageData imgdata in imageList)
                           {%>
                        <img src= "<%= imgdata.path %>" width="150" border="0" alt="image" /><br />
                        <strong>Sent from:&nbsp;</strong><%= imgdata.senderAddress %><br />
                        <strong>On:&nbsp;</strong><%= imgdata.date %><br />
                        <strong>Text:&nbsp;</strong><%= imgdata.text  %><br />
                        <% } %>
                    </div>
                    <!-- end of webGallery -->
                </div>
                <!-- end of formContainer -->
            </div>
            <!-- end of formBox -->
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
                &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
            </p>
        </div>
        <!-- end of footer -->
    </div>
    <!-- end of page_container -->
    </form>
</body>
</html>
