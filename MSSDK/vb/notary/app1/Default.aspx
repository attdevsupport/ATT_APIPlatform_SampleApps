<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Notary_App1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Notary Application - Sign Payload Application</title>
    <meta content="text/html; charset=ISO-8859-1" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
    <div id="container">
        <!-- open HEADER -->
        <div id="header">
            <div>
                <div class="hcRight">
                    <asp:Label runat="server" ID="lblServerTime"></asp:Label>
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
        <!-- close HEADER -->
        <div>
            <div class="content">
                <h1>
                    AT&amp;T Sample Notary Application - Sign Payload Application</h1>
            </div>
        </div>
        <div>
            <div class="content">
                <h2>
                    <br />
                    Feature 1: Sign Payload</h2>
                <br />
            </div>
        </div>
        <form id="form1" runat="server">
        <div id="navigation">
            <table border="0" width="950px">
                <tbody>
                    <tr>
                        <td valign="top" class="label">
                            Request:
                        </td>
                        <td class="cell">
                            <asp:TextBox runat="server" ID="requestText" Height="223px" Width="400px" TextMode="MultiLine"></asp:TextBox>
                        <td width="50px">
                        </td>
                        <td valign="top" class="label">
                            Signed Payload:
                        </td>
                        <td class="cell" width="400px">
                            <asp:TextBox ID="SignedPayLoadTextBox" runat="server" Height="223px" TextMode="MultiLine"
                                Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td width="50px">
                        </td>
                        <td valign="top" class="label">
                            Signature:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="SignatureTextBox" runat="server" Height="73px" TextMode="MultiLine"
                                Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="cell" align="right">
                            <asp:Button runat="server" Text="Sign Payload" ID="signPayLoadButton" OnClick="SignPayLoadButton_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <br clear="all" />
        <div align="center">
            <asp:Panel runat="server" ID="notaryPanel">
            </asp:Panel>
        </div>
        </form>
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
                    target ="_blank">https://developer.att.com/docs</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a></p>
        </div>
    </div>
</body>
</html>
