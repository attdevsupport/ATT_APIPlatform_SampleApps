<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->

<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="CallControl_App1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Application for Call Management </title>
    <meta content="text/html; charset=ISO-8859-1" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
    <style type="text/css">
        .style4
        {
            font-style: normal;
            font-variant: normal;
            font-weight: bold;
            font-size: 12px;
            line-height: normal;
            font-family: Arial, Sans-serif;
            width: 21%;
        }
        .style6
        {
            font-style: normal;
            font-variant: normal;
            font-weight: bold;
            font-size: 12px;
            line-height: normal;
            font-family: Arial, Sans-serif;
            width: 25%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
        <!-- open HEADER -->
        <div id="header">
            <div>
                <div class="hcLeft">
                    Server Time:</div>
                <div class="hcRight">
                    <asp:Label ID="serverTimeLabel" runat="server" Text="Label"></asp:Label>
                </div>
            </div>
            <div>
                <div class="hcLeft">
                    Client Time:</div>
                <div class="hcRight">
                    <script language="JavaScript" type="text/javascript">
                        var myDate = new Date();
                        document.write(myDate);
                    </script>
                </div>
            </div>
            <div>
                <div class="hcLeft">
                    User Agent:</div>
                <div class="hcRight">
                    <script language="JavaScript" type="text/javascript">
                        document.write("" + navigator.userAgent);
                    </script>
                </div>
            </div>
            <br style="clear: both;" />
        </div>
        <!-- close HEADER -->
        <div>
            <div class="content">
                <h1>
                    AT&T Sample Application for Call Management</h1>
                <h2>
                    Feature 1: Outbound Session from
                    <asp:Label ID="lblPhoneNumbers" runat="server" Text=""></asp:Label></h2>
            </div>
        </div>
        <div class="navigation">
            <br />
            <table>
                <tbody>
                    <tr>
                        <td class="style4">
                            Make call to:
                        </td>
                        <td class="cell" style="width: 60%">
                            <asp:TextBox ID="txtNumberToDial" runat="server" title="telephone number or sip address"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                            Script Function:
                        </td>
                        <td class="cell" style="width: 20%">
                            <asp:DropDownList ID="lstTemplate" runat="server" AutoPostBack="true">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="ask">ask</asp:ListItem>
                                <asp:ListItem Value="conference">conference</asp:ListItem>
                                <asp:ListItem Value="message">message</asp:ListItem>
                                <asp:ListItem Value="reject">reject</asp:ListItem>
                                <asp:ListItem Value="transfer">transfer</asp:ListItem>
                                <asp:ListItem Value="wait">wait</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                            Number parameter for Script Function:
                        </td>
                        <td class="cell" style="width: 60%">
                            <asp:TextBox ID="txtNumberForFeature" runat="server" title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                            Message To Play:
                        </td>
                        <td class="cell" style="width: 60%">
                            <asp:TextBox ID="txtMessageToPlay" runat="server" title="enter long message or mp3 audio url, this is used as music on hold for transfer and signals"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                            Script Source Code:
                        </td>
                        <td class="cell" style="width: 60%">
                            <asp:TextBox ID="txtCreateSession" runat="server" TextMode="MultiLine" Height="141px"
                                Width="400px" Enabled="False" title="Create Session will trigger an outbound call from application to <Make call to> number."></asp:TextBox>
                        </td>

                    </tr>
                    <tr>
                    <td class="style4">
                    </td>
                                            <td align="center">
                            <asp:Button ID="btnCreateSession" runat="server" Text="Create Session" OnClick="btnCreateSession_Click" />
                                                <asp:Label class="warning" ID="Label1" runat="server" Text="Label" Width="200px"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="extra" align="left">
                        <div class="warning">
                            <asp:Panel ID="notesPanel" runat="server">
                                <asp:Literal ID="notesLiteral" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                        </div>
        <br style="clear: both;" />
        <div>
            <asp:Panel ID="pnlCreateSession" runat="server" Font-Names="Calibri">
            </asp:Panel>
        </div>
        <br style="clear: both;" />
        <div>
            <div class="content">
                <h2>
                    &nbsp;</h2>
                <h2>
                    Feature 2: Send Signal to Session
                </h2>
                <p>
                    &nbsp;</p>
            </div>
        </div>
        <div class="navigation">
            <table style="width: 100%">
                <tbody>
                    <tr>
                        <td class="style6">
                            <asp:Label ID="lblSession" runat="server" Text="Session ID: " CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblSessionId" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="style6">
                            <asp:Label ID="lblSignal" runat="server" Text="Signal to Send: " CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSignal" runat="server">
                                <asp:ListItem Selected="True" Value="exit">exit</asp:ListItem>
                                <asp:ListItem Value="stopHold">stopHold</asp:ListItem>
                                <asp:ListItem Value="dequeue">dequeue</asp:ListItem>
                            </asp:DropDownList>
                        </td>

                    </tr>
                    <tr>
                    <td class="style6"></td>
                                            <td align="center">
                            <asp:Button ID="btnSendSignal" runat="server" Text="Send Signal" OnClick="btnSendSignal_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="extra">
        </div>
        <br style="clear: both;" />
        <div>
            <asp:Panel ID="pnlSendSignal" runat="server" Font-Names="Calibri">
            </asp:Panel>
        </div>
        <br style="clear: both;" />
        <div id="footer">
            <div style="float: right; width: 20%; font-size: 9px; text-align: right">
                Powered by AT&amp;T Cloud Architecture</div>
            <p>
                © 2012 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
                    target="_blank">http://developer.att.com</a>
                <br />
                The Application hosted on this site are working examples intended to be used for
                reference in creating products to consume AT&amp;T Services and not meant to be
                used as part of your product. The data in these pages is for test purposes only
                and intended only for use as a reference in how the services perform.
                <br />
                For download of tools and documentation, please go to <a href="https://devconnect-api.att.com/"
                    target="_blank">https://devconnect-api.att.com</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a></p>
        </div>
    </div>
    </form>
</body>
</html>
