<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Mobo_App1" %>

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
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample Application - In App Messaging</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
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
    <script src="scripts/utils.js" type="text/javascript"></script>
</head>
<body>
    <div id="pageContainer">
        <div id="header">
            <div class="logo" id="top">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
            <ul class="links" id="nav">
                <li><a href="#" target="_blank">Full Page<img src="images/max.png" alt="" /></a> <span
                    class="divider">|&nbsp;</span> </li>
                <li><a id="sourceLink" runat="server" href="<%$ AppSettings:SourceLink %>" target="_blank">
                    Source<img src="images/opensource.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="downloadLink" runat="server" href="<%$ AppSettings:DownloadLink %>" target="_blank">
                    Download<img src="images/download.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="helpLink" runat="server" href="<%$ AppSettings:HelpLink %>" target="_blank">
                    Help </a></li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
        </div>
        <form id="form1" runat="server">
        <div id="content">
            <div id="contentHeading">
                <h1>
                    AT&amp;T Sample Application - In App Messaging from mobile number</h1>
                <div class="border">
                </div>
                <div id="introtext">
                    <div>
                        <b>Server Time:</b>
                        <%= [String].Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) & " UTC"%>
                    </div>
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
            </div>
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="sendMessages">
                        <h2>
                            Send Messages:</h2>
                        <div class="inputFields">
                            <input placeholder="Address" name="Address" type="text" runat="server" id="Address" />
                            <label>
                                Group:
                                <asp:CheckBox ID="groupCheckBox" runat="server" name="groupCheckBox" />
                            </label>
                            <label>
                                Message:
                                <select name="message" runat="server" id="message">
                                    <option selected="selected">ATT IMMN sample message</option>
                                </select>
                            </label>
                            <label>
                                Subject:
                                <select name="subject" runat="server" id="subject">
                                    <option selected="selected">ATT IMMN sample subject</option>
                                </select>
                            </label>
                            <label>
                                Attachment:
                                <select name="attachment" id="attachment" runat="server">
                                    <option>None</option>
                                </select>
                            </label>
                            <asp:Button ID="sendMessage" class="submit" runat="server" Text="Send Message" OnClick="Button1_Click" />
                        </div>
                        <%="" %>
                        <% If Not String.IsNullOrEmpty(sendMessageSuccessResponse) Then
                        %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong><br />
                            messageId: <%= sendMessageSuccessResponse%>
                        </div>
                        <% End If%>
                        <% If Not String.IsNullOrEmpty(sendMessageErrorResponse) Then%>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%= sendMessageErrorResponse%>
                        </div>
                        <% End If%>
                    </div>
                    <!-- end of sendMessages -->
                    <div class="lightBorder">
                    </div>
                    <div id="getMessages">
                        <h2>
                            Read Messages:</h2>
                        <div class="inputFields">
                            <input id="headerCountTextBox" name="headerCountTextBox" type="text" maxlength="3"
                                placeholder="Header Counter" runat="server" />
                            <input id="indexCursorTextBox" name="indexCursorTextBox" type="text" maxlength="30"
                                placeholder="Index Cursor" runat="server" />
                            <asp:Button ID="getMessageHeaders" runat="server" Text="Get Message Headers" OnClick="getMessageHeaders_Click" />
                        </div>
                        <div class="inputFields">
                            <input name="MessageId" id="MessageId" type="text" maxlength="30" placeholder="Message ID"
                                runat="server" />
                            <input name="PartNumber" id="PartNumber" type="text" maxlength="30" placeholder="Part Number"
                                runat="server" />
                            <asp:Button ID="getMessageContent" runat="server" OnClick="Button2_Click" Text="Get Message Content" />
                        </div>
                    </div>
                    <label class="note">
                        To use this feature, you must be a subscriber to My AT&amp;T Messages.</label>
                </div>
                <!-- BEGIN HEADER CONTENT RESULTS -->
                <% If Not String.IsNullOrEmpty(getMessageSuccessResponse) Then
                %>
                <% If receivedBytes IsNot Nothing Then
                %>
                <div class="successWide">
                    <strong>SUCCESS:</strong>
                </div>
                <% If getContentResponseObject.ContentType.ToLower().Contains("text/plain") Then
                %>
                <%= System.Text.Encoding.Default.GetString(receivedBytes)%>
                <% 
                ElseIf getContentResponseObject.ContentType.ToLower().Contains("application/smil") Then
                %>
                <textarea name="TextBox1" rows="2" cols="20" id="TextBox1" disabled="disabled" runat="server"><%= System.Text.Encoding.Default.GetString(receivedBytes)%></textarea>
                <%Else%>
                <img id="fetchedImage" runat="server" alt="Fetched Image" />
                <% End If%>
                <% End If%>
                <% End If%>
                <% If Not String.IsNullOrEmpty(getMessageErrorResponse) Then
                %>
                <div class="errorWide">
                    <strong>ERROR:</strong>
                    <br />
                    <%= getMessageErrorResponse.ToString()%>
                </div>
                <%End If%>
                <!-- BEGIN HEADER RESULTS -->
                <% If Not String.IsNullOrEmpty(getHeadersSuccessResponse) Then
                %>
                <div class="successWide">
                    <strong>SUCCESS:</strong>
                </div>
                <p id="headerCount">
                    Header Count:
                    <%= messageList.HeaderCount %>
                </p>
                <p id="indexCursor">
                    Index Cursor:
                    <%= messageList.IndexCursor%></p>
                <table class="kvp" id="kvp">
                    <thead>
                        <tr>
                            <th>
                                MessageId
                            </th>
                            <th>
                                From
                            </th>
                            <th>
                                To
                            </th>
                            <th>
                                Received
                            </th>
                            <th>
                                Text
                            </th>
                            <th>
                                Favourite
                            </th>
                            <th>
                                Read
                            </th>
                            <th>
                                Type
                            </th>
                            <th>
                                Direction
                            </th>
                            <th>
                                Contents
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% Dim countInt = 0%>
                        <% For Each header As Header In messageList.Headers
                        %>
                        <% Dim rowId = "row" + countInt.ToString()%>
                        <tr id="<%= rowId %>">
                            <td data-value="MessageId">
                                <%= header.MessageId %>
                            </td>
                            <td data-value="From">
                                <%= header.From %>
                            </td>
                            <td data-value="To">
                                <% If Not header.To Is Nothing Then
                                %>
                                <% Dim tolist = String.Join(",", header.To.ToArray())%>
                                <%= tolist%>
                                <% Else%>
                                <%= String.Empty%>
                                <%End If%>
                            </td>
                            <td data-value="Received">
                                <%= header.Received %>
                            </td>
                            <td data-value="Text">
                                <% If Not String.IsNullOrEmpty(header.Text) Then
                                %>
                                <%= header.Text%>
                                <%Else%>
                                &#45;
                                <%End If%>
                            </td>
                            <td data-value="Favorite">
                                <%= header.Favorite %>
                            </td>
                            <td data-value="Read">
                                <%= header.Read %>
                            </td>
                            <td data-value="Type">
                                <%= header.Type %>
                            </td>
                            <td data-value="Direction">
                                <%= header.Direction %>
                            </td>
                            <td data-value="Contents">
                                <% If ((Not (header.Type) Is Nothing) AndAlso (header.Type.ToLower = "mms")) Then
                                %>
                                <select id="attachments" onchange='chooseSelect("<%=rowId%>",this)'>
                                    <option>More..</option>
                                    <% For Each mmsCont As MMSContent In header.MmsContent%>
                                    <option>
                                        <%= mmsCont.PartNumber + " - " + mmsCont.ContentName + " - " + mmsCont.ContentType%></option>
                                    <% Next%>
                                </select>
                                <% Else
                                %>
                                &#45;
                                <% End If%>
                            </td>
                        </tr>
                        <% countInt += 1%>
                        <% Next%>
                    </tbody>
                </table>
                <% End If%>
                <!-- END HEADER RESULTS -->
                <% If Not String.IsNullOrEmpty(getHeadersErrorResponse) Then
                %>
                <div class="errorWide">
                    <strong>ERROR:</strong>
                    <%= getHeadersErrorResponse%>
                </div>
                <% End If%>
                <!-- END HEADER CONTENT RESULTS -->
            </div>
            <!-- SAMPLE APP CONTENT ENDS HERE! -->
        </div>
        </form>
        <br style="clear: both" />
        <div class="border">
        </div>
        <div id="footer">
            <div id="powered_by">
                Powered by AT&amp;T Cloud Architecture
            </div>
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
    <script type="text/javascript">        setup();</script>
</body>
</html>
