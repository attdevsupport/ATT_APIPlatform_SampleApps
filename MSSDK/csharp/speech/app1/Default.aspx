<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Speech_App1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Speech Application - Speech to Text Application</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
     <style type="text/css">
    .WaterMarkedTextBox
    {
            border-right: #ccccff thin solid;
            border-top: #ccccff thin solid;
            border-left: #ccccff thin solid;
            border-bottom: #ccccff thin solid;
            color: gray;
            display: inline;
            visibility: visible;
    }
    </style>
    <script language="javascript" type="text/javascript">
        function Focus(objname, waterMarkText) {
            obj = document.getElementById(objname);
            if (obj.value == waterMarkText) {
                obj.value = "";
                obj.className = "label";
            }
        }
        function Blur(objname, waterMarkText) {
            obj = document.getElementById(objname);
            if (obj.value == "") {
                obj.value = waterMarkText;
                obj.className = "WaterMarkedTextBox";
            }
            else {
                obj.className = "label";
            }
        }
    </script>
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
                    AT&amp;T Sample Speech Application - Speech to Text Application</h1>
                <h2>
                    Feature 1: Speech to Text</h2>
            </div>
        </div>
        <br />
        <br />
        <div class="navigation">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            X-Speech Context:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlSpeechContext" runat="server" OnSelectedIndexChanged="ddlSpeechContext_SelectedIndexChanged" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            X-Speech SubContext:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtSubContext" runat="server" 
                                class ="WaterMarkedTextBox"
                                Width="234px"
                                onfocus="Focus(this.id,'Chat (Example) Gaming context only.')" 
                                onblur="Blur(this.id,'Chat (Example) Gaming context only.')"
                                Text ="Chat (Example) Gaming context only.">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Audio File:
                        </td>
                        <td class="cell">
                            <asp:FileUpload runat="server" ID="fileUpload1" Width="238px" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="middle" class="label" align="right">
                            Audio Content Type:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlAudioContentType" runat="server">
                                <asp:ListItem>audio/wav</asp:ListItem>
                                <asp:ListItem>audio/x-wav</asp:ListItem>
                                <asp:ListItem>audio/amr</asp:ListItem>
                                <asp:ListItem>audio/amr-wb</asp:ListItem>
                                <asp:ListItem>audio/x-speex</asp:ListItem>
                                <asp:ListItem>audio/x-speex-with-header-byte;rate=16000</asp:ListItem>
                                <asp:ListItem>audio/x-speex-with-header-byte;rate=8000</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=linear;rate=16000;byteorder=LE</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=linear;rate=16000;byteorder=BE</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=linear;rate=8000;byteorder=LE</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=linear;rate=8000;byteorder=BE</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=ulaw;rate=16000</asp:ListItem>
                                <asp:ListItem>audio/raw;coding=ulaw;rate=8000</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                     <tr>
                        <td valign="middle" class="label" align="right">
                            Content Language:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlContentLang" runat="server">
                                <asp:ListItem>en-US</asp:ListItem>
                                <asp:ListItem>es-US</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="chkChunked" runat="server" CssClass="cell">
                                <asp:ListItem Text="Send Chunked" />
                            </asp:CheckBoxList>
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
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="SpeechToTextButton_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="extra">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td>
                            <div id="extraleft">
                                <div class="warning">
                                    <strong>Note:</strong><br />
                                    If no file is chosen, a <a href="./default.wav">default.wav</a> file will be loaded
                                    on submit.<br />
                                    <strong>Speech file format constraints:</strong>
                                    <br />
                                    •	16 bit PCM WAV, linear or ulaw coding, single channel, 8 kHz sampling
                                    <br />
•	16 bit PCM WAV, linear or ulaw coding, single channel, 16 kHz sampling
                                    <br />
•	AMR (narrowband), 12.2 kbit/s, 8 kHz sampling
                                    <br />
•	AMR-WB (wideband) is 12.65 kbit/s, 16khz sampling
                                    <br />
•	OGG - speex encoding, 8kHz sampling
                                    <br />
•	OGG - speex encoding, 16kHz sampling
                                    <br />
•	Raw, linear or ulaw coding, LE or BE byte order, 8kHz sampling
                                    <br />
•	Raw, linear or ulaw coding, LE or BE byte order, 16kHz sampling

                                </div>
                            </div>
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
        <div align="center">
            <asp:Panel ID="resultsPanel" runat="server" BorderWidth="0" Width="80%">
                <table width="500" cellpadding="1" cellspacing="1" border="0">
                    <thead>
                        <tr>
                            <th width="50%" class="label">
                                Parameter
                            </th>
                            <th width="50%" class="label">
                                Value
                            </th>
                        </tr>
                    </thead>
                    <tr>
                        <td class="cell" align="center">
                            <i>Status </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>ResponseId </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblResponseId" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Hypothesis </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblHypothesis" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>LanguageId </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblLanguageId" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Confidence </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblConfidence" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Grade </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblGrade" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>ResultText </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblResultText" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Words </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblWords" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>WordScores </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblWordScores" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <br clear="all" />
        <div align="center">
            <asp:Panel ID="tvContextPanel" runat="server" BorderWidth="0" Width="80%">
                <table width="500" cellpadding="1" cellspacing="1" border="0">
                    <thead>
                        <tr>
                            <th width="50%" class="label">
                                TV Context Info Parameter
                            </th>
                            <th width="50%" class="label">
                                Value
                            </th>
                        </tr>
                    </thead>
                    <tr>
                        <td class="cell" align="center">
                            <i>Info.ActionType </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblInfoActionType" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Interpretation.genre.id </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblInterpretation_genre_id" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Interpretation.genre.words </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblInterpretation_genre_words" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Interpretation.station.name </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblInterpretation_station_name" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                   
              
                    <tr>
                        <td class="cell" align="center">
                            <i>Metrics.audioBytes</i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblMetrics_audioBytes" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Metrics.audioTime </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblMetrics_audioTime" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>recognized </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblRecognized" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                 
                </table>
                <table width="500" cellpadding="1" cellspacing="1" border="0">
                    <thead>
                        <tr>
                            <th width="50%" class="label">
                                TV Context Search Parameter
                            </th>
                            <th width="50%" class="label">
                                Value
                            </th>
                        </tr>
                    </thead>
                    <tr>
                        <td class="cell" align="center">
                            <i>Description </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblDescription" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>GuideDateStart  </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblGuideDateStart" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>GuideDateEnd </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblGuideDateEnd" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>Lineup </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblLineup" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>            
                    <tr>
                        <td class="cell" align="center">
                            <i>Market </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblMarket" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <i>ResultCount </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblResultCount" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    
                </table>
            </asp:Panel>
        </div>
        <br clear="all" />
         <div align="center">
            <asp:Panel ID="tvContextProgramPanel" runat="server" BorderWidth="0" Width="80%">
                <label class="label">TV Context Program Details</label>
                <asp:GridView ID="gvPrograms" runat="server" BackColor="White" BorderColor="#CCCCCC"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%">
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <RowStyle ForeColor="#000066" HorizontalAlign="Left" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                </asp:GridView>
            </asp:Panel>
        </div>
        <br clear="all" />
        <div align="center">
            <asp:Panel ID="tvContextShowtimePanel" runat="server" BorderWidth="0" Width="80%">
                <label class="label">TV Context ShowTime Details</label>
                <asp:GridView ID="gvShowTimes" runat="server" BackColor="White" BorderColor="#CCCCCC"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%">
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <RowStyle ForeColor="#000066" HorizontalAlign="Left" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                </asp:GridView>
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
