<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="TL_App1" %>

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
<!DOCTYPE html>
<html lang="en">
<head>
    <title>AT&amp;T Sample Application - Location</title>
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
<body>
    <form id="form1" runat="server">
    <div id="pageContainer">
        <div id="header">
            <div class="logo">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
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
                    AT&amp;T Sample Application - Location</h1>
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
            <!-- SAMPLE APP CONTENT STARTS HERE! -->
            <div class="lightBorder">
            </div>
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <h2>
                        Feature 1: Map of Location</h2>
                    <div class="inputFields">
                        Requested Accuracy:
                        <input type="radio" id="RA1" runat="server" name="requestedAccuracy" value="150" />
                        150 m
                        <input type="radio" name="requestedAccuracy" id="RA2" runat="server" value="1000" />
                        1,000 m
                        <input type="radio" name="requestedAccuracy" id="RA3" runat="server" value="10000" />
                        10,000 m
                        <br />
                        Acceptable Accuracy:
                        <input type="radio" name="acceptableAccuracy" id="AA1" runat="server" value="150" />
                        150 m
                        <input type="radio" name="acceptableAccuracy" id="AA2" runat="server" value="1000" />
                        1,000 m
                        <input type="radio" name="acceptableAccuracy" id="AA3" runat="server" value="10000" />
                        10,000 m
                        <br />
                        Delay Tolerance:
                        <input type="radio" name="tolerance" id="DT1" runat="server" value="NoDelay" />No
                        Delay
                        <input type="radio" name="tolerance" id="DT2" runat="server" value="LowDelay" />Low
                        Delay
                        <input type="radio" name="tolerance" id="DT3" runat="server" value="DelayTolerant" />Delay
                        Tolerant
                        <br />
                        <button type="submit" name="getLocation" runat="server" id="getLocation" onserverclick="GetDeviceLocation_Click">
                            Get Phone Location</button>
                    </div>
                    <!-- end of Device Location -->
                    <!-- end of formContainer -->
                    <!-- end of formBox -->
                    <% If Not String.IsNullOrEmpty(getLocationSuccess) Then
                    %>
                    <div class="successWide">
                        <strong>SUCCESS:</strong>
                        <br />
                        <strong>Latitude:</strong>
                        <%= getLocationResponse.latitude %>
                        <br />
                        <strong>Longitude:</strong>
                        <%= getLocationResponse.longitude%>
                        <br />
                        <strong>Accuracy:</strong>
                        <%= getLocationResponse.accuracy%>
                        <br />
                        <strong>Response Time:</strong>
                        <%= responseTime.ToString() %>
                        seconds
                    </div>
                    <div align="center">
                        <iframe width="600" height="400" frameborder="0" scrolling="no" marginheight="0"
                            marginwidth="0" src="http://maps.google.com/?q=<%= getLocationResponse.latitude%>+<%= getLocationResponse.longitude%>&output=embed">
                        </iframe>
                    </div>
                    <%End If%>
                    <% If Not String.IsNullOrEmpty(getLocationError) Then
                    %>
                    <div class="errorWide">
                        <strong>ERROR:</strong>
                        <br />
                        <%= getLocationError %>
                    </div>
                    <% End If%>
                </div>
                <!-- SAMPLE APP CONTENT ENDS HERE! -->
            </div>
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
    </div>
    </form>
</body>
</html>
