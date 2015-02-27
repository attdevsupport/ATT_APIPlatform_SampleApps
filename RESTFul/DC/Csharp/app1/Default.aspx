<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="DC_App1" %>

<!-- 
* Copyright 2014 AT&T
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample DC Application - Get Device Capabilities Application</title>
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
                <li><a runat="server" target="_blank" id="SourceLink">Source<img src="images/opensource.png" /></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="DownloadLink">Download<img src="images/download.png" /></a>
                    <span class="divider">|&nbsp;</span> </li>
                <li><a runat="server" target="_blank" id="HelpLink">Help</a> </li>
                <li id="back"><a href="#top">Back to top</a> </li>
            </ul>
            <!-- end of links -->
        </div>
        <!-- end of header -->
        <div id="content">
            <div id="contentHeading">
                <h1>
                    AT&amp;T Sample DC Application - Get Device Capabilities Application</h1>
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
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <h2>
                        Feature 1: Get Device Capabilities</h2>
                    <div class="lightBorder">
                    </div>
                    <div class="note">
                        <strong>OnNet Flow:</strong> Request Device Capabilities details from the AT&amp;T
                        network for the mobile device of an AT&amp;T subscriber who is using an AT&amp;T
                        direct Mobile data connection to access this application.
                        <br />
                        <strong>OffNet Flow:</strong> Where the end-user is not on an AT&amp;T Mobile data
                        connection or using a Wi-Fi or tethering connection while accessing this application.
                        This will result in an HTTP 400 error.
                    </div>
                    <!-- end note -->
                    <% if (!string.IsNullOrEmpty(getDCSuccess))
                       { %>
                    <div class="successWide">
                        <strong>SUCCESS:</strong>
                        <br />
                        Device parameters listed below.
                    </div>
                    <table class="kvp" id="kvp">
                        <thead>
                            <tr>
                                <th>
                                    Parameter
                                </th>
                                <th>
                                    Value
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <% foreach (var pair in getDCResponse)
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    <%= pair.Key.ToString()%>
                                </td>
                                <td data-value="Value">
                                    <%= pair.Value.ToString()%>
                                </td>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>
                    <% } %>
                    <% if (!string.IsNullOrEmpty(getDCError))
                       { %>
                    <div class="errorWide">
                        <b>ERROR:</b><br />
                        <%= getDCError %>
                    </div>
                    <% } %>
                </div>
                <!-- end of formContainer -->
            </div>
            <!-- end of formBox -->
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
                For more information please go to <a href="https://developer.att.com/support"
                    target="_blank">https://developer.att.com/support</a>
                <br />
                <br />
                &#169; 2014 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
            </p>
        </div>
        <!-- end of footer -->
    </div>
    <!-- end of page_container -->
    </form>
</body>
</html>
