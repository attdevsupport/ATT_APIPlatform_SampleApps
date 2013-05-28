<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TTS_App1" %>

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
    <title>AT&amp;T Sample Speech Application - Text To Speech </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
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
    <div id="pageContainer" class="pageContainer">
        <div id="header">
            <div class="logo" id="top">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
            <ul class="links" id="nav">
                <li><a href="#" target="_blank">Full Page<img src="images/max.png" alt="" /></a> <span
                    class="divider">|&nbsp;</span> </li>
                <li><a id="SourceLink" runat="server" target="_blank">Source<img src="images/source.png"
                    alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="DownloadLink" runat="server" target="_blank">Download<img src="images/download.png"
                    alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="HelpLink" runat="server" target="_blank">Help </a></li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
        </div>
        <form id="form1" runat="server">
        <div class="content">
            <div class="contentHeading">
                <h1>
                    AT&amp;T Sample Application - Text To Speech</h1>
                <div id="introtext">
                    <div>
                        <b>Server Time:&nbsp;</b><%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %>
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
                    <div id="formData">
                        <h3>
                            Content Type:
                        </h3>
                        <asp:DropDownList ID="ContentType" name="ContentType" runat="server">
                            <asp:ListItem value="text/plain" Selected="True" Text="text/plain" />
                            <asp:ListItem value="application/ssml+xml" Text="application/ssml+xml" />
                        </asp:DropDownList>
                        <h3>Content:</h3>
                          <label>text/plain</label><br/>
                          <asp:TextBox ID="plaintext" type="text" runat="server" name="plaintext" TextMode="MultiLine" Enabled="False" Rows="4"></asp:TextBox><br/>
                          <label>application/ssml</label><br/>
                          <asp:TextBox  ID="ssml" type="text" runat="server" name="ssml" TextMode="MultiLine" Enabled="False" Rows="4"></asp:TextBox >
                        <h3>
                            X-Arg:
                        </h3>
                        <asp:TextBox ID="x_arg" runat="server" TextMode="MultiLine" type="text" Enabled="False" Rows="4"></asp:TextBox>
                        <br />
                        <button id="btnSubmit" onserverclick="BtnSubmit_Click" runat="server" name="TextToSpeechButton"
                            type="submit">
                            Submit</button>
                        <% if (!string.IsNullOrEmpty(TTSSuccessMessage)){%>
                        <div class="successWide" align="left">
                            <strong>SUCCESS:</strong>
                            <audio controls="controls" autobuffer="autobuffer" autoplay="autoplay" runat="server" id="audioPlay">
                            </audio>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(TTSErrorMessage)){%>
                        <div class="errorWide">
                            <strong>ERROR:</strong>
                            <br>
                            <%= TTSErrorMessage%>
                        </div>
                        <% } %>
                    </div>
                </div>
            </div>
        </div>
        </form>
        <div id="footer">
            <div id="ft">
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
                    © 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                        target="_blank">https://developer.att.com</a>
                </p>
            </div>
            <!-- end of ft -->
        </div>
        <!-- end of footer -->
    </div>
</body>
</html>
