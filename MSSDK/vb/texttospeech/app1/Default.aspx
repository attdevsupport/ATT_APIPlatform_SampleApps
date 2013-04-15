<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="TextToSpeech_App1" %>

<!DOCTYPE html>
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<head>
    <title>AT&amp;T Sample Speech Application - Text to Speech Application</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
        <!-- open HEADER -->
        <div id="header">
            <div>
                <div class="hcRight">
                    <asp:Label ID="lblServerTime" runat="server"></asp:Label>
                </div>
                <div class="hcLeft">
                    Server Time:</div>
            </div>
            <div>
                <div class="hcRight">
                    <script language="JavaScript" type="text/javascript">
                        var myDate = new Date();
                        document.write(myDate);
                    </script>
                </div>
                <div class="hcLeft">
                    Client Time:</div>
            </div>
            <div>
                <div class="hcRight">
                    <script language="JavaScript" type="text/javascript">
                        document.write("" + navigator.userAgent);
                    </script>
                </div>
                <div class="hcLeft">
                    User Agent:</div>
            </div>
            <br clear="all" />
        </div>
        <div>
            <div class="content">
                <h1>
                    AT&amp;T Sample Speech Application - Text to Speech Application</h1>
                <h2>
                    Feature 1: Text to Speech</h2>
            </div>
        </div>
        <br />
        <br />
        <div class="navigation">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Text To Convert
                        </td>
                        <td class="cell">
                           <asp:TextBox ID="txtTextToConvert" runat="server" TextMode="MultiLine" Width="234px" Rows="6"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Content Language
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlContentLanguage" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Content Type
                        </td>
                        <td class="cell">
                             <asp:DropDownList ID="ddlContentType" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>                    
                    <tr>
                        <td valign="middle" class="label" align="right">
                            X-Args Defined:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtXArgs" runat="server" TextMode="MultiLine" Enabled="False" Rows="7"
                                Width="234px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="SpeechToTextButton_Click" Width="84px" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>        
        <br clear="all" />
        <div align="center">
            <asp:Panel ID="statusPanel" runat="server" Font-Names="Calibri" Font-Size="XX-Small">
            </asp:Panel>
        </div>
        <div align="center" >
            <asp:Panel ID="resultsPanel" style="margin-left:60px;" runat="server" BorderWidth="0" Width="80%">
                <table class="successWide">
                      <tr>
                        <td align="left" class="auto-style1"><b>SUCCESS:</b></td>
                    </tr>
                    <tr>
                        <td align="right" class="auto-style1"><i>Content Type</i></td>
                        <td align="left"><asp:Label id="lblContentType" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td align="right" class="auto-style1"><i>Content Length</i></td>
                        <td align="left"><asp:Label ID="lblContentLength" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td  class="auto-style1"></td>
                        <td align="left"><audio controls="controls" autobuffer="autobuffer" autoplay="autoplay" runat="server" id="audioPlay">
                            </audio></td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <br clear="all" />
        <div id="footer">
            <div style="float: right; width: 20%; font-size: 9px; text-align: right">
                Powered by AT&amp;T Cloud Architecture</div>
            <p>
                &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
                    target="_blank">http://developer.att.com</a>
                <br />
                The Application hosted on this site are working examples intended to be used for
                reference in creating products to consume AT&amp;T Services and not meant to be
                used as part of your product. The data in these pages is for test purposes only
                and intended only for use as a reference in how the services perform.
                <br />
                For download of tools and documentation, please go to <a href="http://developer.att.com/SDK"
                        target="_blank">https://developer.att.com/SDK</a> and <a href="http://developer.att.com/docs"
                        target="_blank">https://developer.att.com/docs</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a></p>
        </div>
    </div>
    <p>
        &nbsp;</p>
    </form>
</body>
</html>