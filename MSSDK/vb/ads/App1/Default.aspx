<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Ad_App1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample Advertisement Application - Get Advertisement Application
    </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
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
                    AT&amp;T Sample Advertisement Application - Get Advertisement Application</h1>
                <h2>
                    Feature 1: Get Advertisement</h2>
            </div>
        </div>
        <br />
        <br />
        <div class="extra">
            <table border="0" style="width: 100%">
                <tbody>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="Zip/Postal code of a user. For US only. (Integer)">
                            Zip Code:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtZipCode" runat="server" MaxLength="10"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="Country of user. An ISO 3166 code to be used for specifying a country code.">
                            Country:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtCountry" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="label">
                            Over 18 Ad Content:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlOver18" runat="server">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="0">Deny Over 18</asp:ListItem>
                                <asp:ListItem Value="2">Only Over 18</asp:ListItem>
                                <asp:ListItem Value="3">Allow All Ads</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                    </tr>
                </tbody>
            </table>
            <br />
            <div id="extraleft">
                <div class="warning">
                    <strong>Note:</strong><br />
                            All Parameters are optional except Category.<br />
                            If this application is accessed from desktop browser, 
                            you may see successful response without Ads (HTTP 204).
                </div>
            </div>
        </div>
        <div class="navigation">
            <table border="0" width="100%">
                <tbody>
                    <tr>
                        <td width="10%" class="label">
                            *Category:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlCategory" runat="server">
                                <asp:ListItem Selected="True">auto</asp:ListItem>
                                <asp:ListItem>business</asp:ListItem>
                                <asp:ListItem>chat</asp:ListItem>
                                <asp:ListItem>communication</asp:ListItem>
                                <asp:ListItem>community</asp:ListItem>
                                <asp:ListItem>entertainment</asp:ListItem>
                                <asp:ListItem>finance</asp:ListItem>
                                <asp:ListItem>games</asp:ListItem>
                                <asp:ListItem>health</asp:ListItem>
                                <asp:ListItem>local</asp:ListItem>
                                <asp:ListItem>maps</asp:ListItem>
                                <asp:ListItem>medical</asp:ListItem>
                                <asp:ListItem>movies</asp:ListItem>
                                <asp:ListItem>music</asp:ListItem>
                                <asp:ListItem>news</asp:ListItem>
                                <asp:ListItem>other</asp:ListItem>
                                <asp:ListItem>personals</asp:ListItem>
                                <asp:ListItem>photos</asp:ListItem>
                                <asp:ListItem>shopping</asp:ListItem>
                                <asp:ListItem>social</asp:ListItem>
                                <asp:ListItem>sports</asp:ListItem>
                                <asp:ListItem>technology</asp:ListItem>
                                <asp:ListItem>tools</asp:ListItem>
                                <asp:ListItem>travel</asp:ListItem>
                                <asp:ListItem>tv</asp:ListItem>
                                <asp:ListItem>video</asp:ListItem>
                                <asp:ListItem>weather</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 10%" class="label">
                            MMA Size:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlMMASize" runat="server">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="120 x 20">120 x 20</asp:ListItem>
                                <asp:ListItem Value="168 x 28">168 x 28</asp:ListItem>
                                <asp:ListItem Value="216 x 36">216 x 36</asp:ListItem>
                                <asp:ListItem Value="300 x 50">300 x 50</asp:ListItem>
                                <asp:ListItem Value="300 x 250">300 x 250</asp:ListItem>
                                <asp:ListItem Value="320 x 50">320 x 50</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="The City of the user. For US only.">
                            City:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                        </td>
                        <td width="10%" class="label" title="Area code of a user. For US only. (Integer)">
                            Area Code:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtAreaCode" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td width="10%" class="label" title="User location latitude value (in degrees).">
                            Latitude:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtLatitude" runat="server"></asp:TextBox>
                        </td>
                        <td width="10%" class="label" title="User location longitude value (in degrees).">
                            Longitude:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtLongitude" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="label">
                            Age Group:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlAgeGroup" runat="server">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="1-13">1-13</asp:ListItem>
                                <asp:ListItem Value="14-25">14-25</asp:ListItem>
                                <asp:ListItem Value="26-35">26-35</asp:ListItem>
                                <asp:ListItem Value="36-55">36-55</asp:ListItem>
                                <asp:ListItem Value="55-100">55-100</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 10%" class="label">
                            Premium:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlPremium" runat="server">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="0">Non Premium</asp:ListItem>
                                <asp:ListItem Value="1">Premium Only</asp:ListItem>
                                <asp:ListItem Value="2">Both</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="label">
                            Gender:
                        </td>
                        <td class="cell">
                            <asp:DropDownList ID="ddlGender" runat="server">
                                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                <asp:ListItem Value="M">Male</asp:ListItem>
                                <asp:ListItem Value="F">Female</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td width="10%" class="label" title="Filter ads by keywords (delimited by commas Ex: music,singer)">
                            Keywords:
                        </td>
                        <td class="cell">
                            <asp:TextBox ID="txtKeywords" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <br />
            <table border="0" width="100%">
                <tbody>
                    <tr valign="middle" align="right">
                        <td class="cell" width="35%">
                            <asp:Button ID="GetAdsButton" runat="server" Text="Get Advertisement" OnClick="GetAdsButton_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <br style="clear: both;" />
        <div align="center">
            <asp:Panel ID="statusPanel" runat="server" Font-Names="Calibri" Font-Size="XX-Small">
            </asp:Panel>
        </div>
        <br style="clear: both;" />
        <div align="center">
            <asp:HyperLink ID="hplImage" runat="server" Target="_blank"></asp:HyperLink>
        </div>
        <div align="center">
            <asp:Panel ID="contentPanel" runat="server" Width="100%">
            </asp:Panel>
        </div>
        <div id="footer">
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
    </div>
    <p>
        &nbsp;</p>
    </form>
</body>
</html>
