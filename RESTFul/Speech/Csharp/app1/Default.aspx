<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Speech_App1" %>
<!DOCTYPE html>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample Speech Application - Speech to Text (Generic) </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
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
                <li>
                    <a href="#" target="_blank">Full Page<img src="images/max.png" alt="" /></a> <span
                    class="divider">|&nbsp;</span> 
                </li>
                <li>
                    <a id="sourceLink" runat="server" href="<%$ AppSettings:SourceLink %>" target="_blank">
                    Source<img src="images/source.png" alt="" />
                    </a>
                    <span class="divider">|&nbsp;</span>
                </li>
                <li>
                    <a id="downloadLink" runat="server" href="<%$ AppSettings:DownloadLink %>" target="_blank">
                    Download<img src="images/download.png" alt="" />
                    </a>
                    <span class="divider">|&nbsp;</span>
                </li>
                <li>
                    <a id="helpLink" runat="server" href="<%$ AppSettings:HelpLink %>" target="_blank">
                    Help
                    </a>
                </li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
        </div>
        <form id="form1" runat="server">
        <div class="content">
            <div class="contentHeading">
                <h1>
                    AT&amp;T Sample Application - Speech to Text</h1>
                <div id="introtext">
                    <div>
                        <b>Server Time:</b>
                        <asp:Label ID="lblServerTime" runat="server"></asp:Label>
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
                            Speech Context:
                        </h3>
                        <asp:DropDownList ID="ddlSpeechContext" runat="server"></asp:DropDownList>
                        <h3>
                            Audio File:
                        </h3>
                        <asp:DropDownList ID="ddlAudioFile" runat="server"></asp:DropDownList>
                        <h3>
                            <asp:CheckBoxList ID="chkChunked" runat="server" CssClass="cell">
                                <asp:ListItem Text=" Send Chunked" />
                            </asp:CheckBoxList>
                        </h3>
                        <h3>
                            X-Args Defined:
                        </h3>
                        <asp:TextBox ID="txtXArgs" runat="server" TextMode="MultiLine" Enabled="False" Rows="7"
                            Width="234px"></asp:TextBox><br />
                        <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="BtnSubmit_Click" />
                    </div>
                </div>
            </div>
            <asp:Panel ID="statusPanel" runat="server">
            </asp:Panel><br clear="all" />
            <asp:Panel ID="resultsPanel" runat="server" HorizontalAlign="Left">
                <table width="95%" cellpadding="1" cellspacing="1" border="0">
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
                            <i>Status </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="hypoRow" runat="server">
                        <td class="cell" align="center">
                            <i>Hypothesis </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblHypothesis" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="langRow" runat="server">
                        <td class="cell" align="center">
                            <i>LanguageId </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblLanguageId" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="confRow" runat="server">
                        <td class="cell" align="center">
                            <i>Confidence </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblConfidence" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="gradeRow" runat="server">
                        <td class="cell" align="center">
                            <i>Grade </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblGrade" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="resultRow" runat="server">
                        <td class="cell" align="center">
                            <i>ResultText </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblResultText" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="wordsRow" runat="server">
                        <td class="cell" align="center">
                            <i>Words </i>
                        </td>
                        <td class="cell" align="center">
                            <i>
                                <asp:Label ID="lblWords" runat="server"></asp:Label>
                            </i>
                        </td>
                    </tr>
                    <tr id="wordScoresRow" runat="server">
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
        </form>
        <div id="footer">
            <div id="ft" class="center">
                <!-- FOOTER BEGIN -->
                <div>
                    <div style="float: right; width: 20%; font-size: 9px; text-align: right">
                        Powered by AT&amp;T Cloud Architecture</div>
                    <p>
                        &#169; 2012 AT&amp;T Intellectual Property. All rights reserved. <a href="http://developer.att.com/"
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
                <!-- FOOTER END -->
            </div>
        </div>
    </div>
</body>
</html>
