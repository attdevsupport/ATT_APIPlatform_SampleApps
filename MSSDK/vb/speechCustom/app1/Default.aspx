<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="SpeechCustom_App1" %>

<!DOCTYPE html>
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Speech Application - Speech to Text Custom Application</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
    <script>
        function ChangeAudioFile() {
            var audioDropdown = document.getElementById("ddlAudioFile");
            var selectedFile = audioDropdown.options[audioDropdown.selectedIndex].text;
            var playme = document.getElementById('playme');
            playme.src = "audio/" + selectedFile;
            playme.load();
        }
    </script>
</head>
<body onload="ChangeAudioFile();">
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
                    AT&amp;T Sample Speech Application - Custom Speech to Text Application</h1>
                <h2>
                    Feature 1: Speech to Text</h2>
            </div>
        </div>
        <br />
        <div>
            <table border="0">
                <tbody>
                    <tr>
                        <td align="right" class="label" style="width:10%;">
                            Speech Context:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlSpeechContext" runat="server" style="margin-left: 0px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Audio File:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlAudioFile" runat="server" onchange="ChangeAudioFile();"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td><audio id="playme" controls="controls" src="audio/pizza-en-US.wav"></audio> </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            X-Arg:
                        </td>
                        <td>
                            <asp:Label ID="txtXArgs" runat="server" TextMode="MultiLine" Enabled="False" Rows="6" Width="650px"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td valign="middle" class="label" align="right">
                           MIME Data:
                        </td>
                        <td>
                            <asp:TextBox ID="txtMimeData" runat="server" TextMode="MultiLine" Enabled="False" Rows="7" Width="650px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="SpeechToTextButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="left">
                            <asp:Panel ID="statusPanel" runat="server" Font-Names="Calibri" Font-Size="XX-Small"></asp:Panel></td>
                        </tr>
               <%-- </tbody>
            </table>
        </div>
        <br clear="all" />
        <div align="left">
            <asp:Panel ID="statusPanel" runat="server" Font-Names="Calibri" Font-Size="XX-Small" HorizontalAlign="Left">
            </asp:Panel>
        </div>
        <div>--%>
                    <tr>
                        <td></td>
                        <td>
            <asp:Panel ID="resultsPanel" runat="server" BorderWidth="0" Width="80%">
                <table cellpadding="5" cellspacing="1" border="0">
                    <thead>
                        <tr>
                            <th width="30%" class="label">
                                Parameter
                            </th>
                            <th width="70%" class="label">
                                Value
                            </th>
                        </tr>
                    </thead>
                    <tr>
                        <td class="cell">
                            <i>Status </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>ResponseId </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblResponseId" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>Hypothesis </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblHypothesis" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>LanguageId </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblLanguageId" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>Confidence </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblConfidence" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>Grade </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblGrade" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>ResultText </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblResultText" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>Words </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblWords" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell">
                            <i>WordScores </i>
                        </td>
                        <td class="cell">
                            <i>
                                <asp:Label ID="lblWordScores" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
                    </td>
                            </tr>
               </tbody>
            </table>
        </div>
        <!--</div>-->
        <br clear="all" />
        <div align="center">
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