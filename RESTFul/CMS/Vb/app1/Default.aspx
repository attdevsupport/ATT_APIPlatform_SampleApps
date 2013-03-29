<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="CallControl_App1" %>

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
    <title>AT&amp;T Sample Application - Call Management</title>
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
    <script src="scripts/utils.js" type="text/javascript"></script>
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
                <li><a href="#" target="_blank">Full Page<img src="images/max.png" /></a> <span class="divider">
                    |&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="SourceLink">Source<img src="images/opensource.png" /></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="DownloadLink">Download<img src="images/download.png"></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="HelpLink">Help</a> </li>
                <li id="back"><a href="#top">Back to top</a> </li>
            </ul>
            <!-- end of links -->
        </div>
        <form id="form1" runat="server">
        <!-- end of header -->
        <div id="content">
            <div id="contentHeading">
                <h1>
                    AT&amp;T Sample Application - Call Management</h1>
                <div class="border">
                </div>
                <div id="introtext">
                    <div>
                        <b>Server Time:</b>
                        <%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %>
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
                <!-- end of introtext -->
            </div>
            <!-- end of contentHeading -->
            <!-- SAMPLE APP CONTENT STARTS HERE! -->
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="sendMessages">
                        <h2>
                            Feature 1: Outbound Session from
                            <% If Not String.IsNullOrEmpty(phoneNumbers) Then
                            %>
                            <%=phoneNumbers%>
                            <%End If%></h2>
                        <div class="inputFields">
                            <label>
                                Make call to:
                                <input type="text" name="txtNumberToDial" placeholder="Address" title="telephone number or sip address"
                                    id="txtNumberToDial" runat="server" />
                            </label>
                            <label>
                                Script Function:
                                <select name="scriptType" id="scriptType" runat="server">
                                    <option value=""></option>
                                    <option value="ask">ask</option>
                                    <option value="conference">conference</option>
                                    <option value="message">message</option>
                                    <option value="reject">reject</option>
                                    <option value="transfer">transfer</option>
                                    <option value="wait">wait</option>
                                </select>
                            </label>
                            <label>
                                Number parameter for Script Function:
                                <input type="text" name="txtNumber" id="txtNumber" runat="server" placeholder="Number"
                                    title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from" />
                            </label>
                            <label>
                                Message To Play:
                                <input type="text" name="txtMessageToPlay" placeholder="Message" title="enter long message or mp3 audio url, this is used as music on hold"
                                    id="txtMessageToPlay" runat="server" />
                            </label>
                            <div id="scriptText">
                                <label>
                                    Script Source Code:
                                </label>
                                <textarea name="txtCreateSession" rows="2" cols="20" disabled="disabled" id="txtCreateSession"
                                    runat="server">
										</textarea>
                            </div>
                            <div>
                                <button type="submit" class="submit" name="btnCreateSession" id="btnCreateSession"
                                    runat="server" onserverclick="btnCreateSession_Click">
                                    Create Session</button>
                            </div>
                            <% If Not String.IsNullOrEmpty(successOfCreateSessionResponse) Then
                            %>
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                                <br />
                                id:
                                <%=sessionIdOfCreateSessionResponse.ToString()%><br />
                                success:
                                <%=successOfCreateSessionResponse%>
                            </div>
                            <% End If%>
                            <% If Not String.IsNullOrEmpty(createSessionErrorResponse) Then
                            %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=createSessionErrorResponse.ToString()%>
                            </div>
                            <% End If%>
                        </div>
                    </div>
                    <!-- end of Create Session -->
                    <div class="lightBorder">
                    </div>
                    <div id="sendSignal">
                        <h2>
                            Feature 2: Send Signal to Session</h2>
                        <br />
                        <div class="inputFields">
                            <label class="label">
                                Session ID:
                                <%If Not String.IsNullOrEmpty(sessionIdOfCreateSessionResponse) Then
                                %>
                                <%=sessionIdOfCreateSessionResponse.ToString()%>
                                <% End If%>
                            </label>
                            <label class="label">
                                Signal to Send:
                                <select name="signal" style="display: inline" id="signal" runat="server">
                                    <option value="exit" selected="selected">exit</option>
                                    <option value="stopHold">stopHold</option>
                                    <option value="dequeue">dequeue</option>
                                </select>
                            </label>
                            <div>
                                <button type="submit" class="submit" name="btnSendSignal" id="btnSendSignal" runat="server"
                                    onserverclick="btnSendSignal_Click">
                                    Send Signal</button>
                            </div>
                            <% If Not String.IsNullOrEmpty(statusOfSendSignalResponse) Then
                            %>
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                                <br />
                                status:
                                <%=statusOfSendSignalResponse.ToString()%>
                            </div>
                            <% End If%>
                            <% If Not String.IsNullOrEmpty(sendSignalErrorResponse) Then
                            %>
                            <div class="errorWide">
                                <strong>ERROR:</strong><br />
                                <%=sendSignalErrorResponse.ToString()%>
                            </div>
                            <% End If%>
                        </div>
                    </div>
                    <!-- end of Send Signal -->
                </div>
                <!-- end of formContainer -->
                <!-- BEGIN HEADER CONTENT RESULTS -->
                <!-- END HEADER CONTENT RESULTS -->
                <!-- BEGIN HEADER RESULTS -->
                <!-- END HEADER RESULTS -->
            </div>
            <!-- end of formBox -->
            <!-- SAMPLE APP CONTENT ENDS HERE! -->
        </div>
        </form>
        <!-- end of content -->
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
                For download of tools and documentation, please go to <a href="https://devconnect-api.att.com/"
                    target="_blank">https://devconnect-api.att.com</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a>
                <br />
                <br />
                &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
                    target="_blank">http://developer.att.com</a>
            </p>
        </div>
        <!-- end of footer -->
    </div>
    <!-- end of page_container -->
    <script type="text/javascript">        setup();</script>
</body>
</html>
