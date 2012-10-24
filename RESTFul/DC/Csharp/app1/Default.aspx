<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="DC_App1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>AT&amp;T Sample DC Application - Get Device Capabilities Application</title>
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
                    <asp:Label ID="lblServerTime" runat="server" Text="Label"></asp:Label>
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
                    AT&amp;T Sample DC Application - Get Device Capabilities Application</h1>
                <h2>
                    Feature 1: Get Device Capabilities</h2>
            </div>
        </div>
        <br />
        <br />

        <div class="extra">
            <table>
                <tbody>
                    <div id="extraleft">
                        <div class="warning">
                            <strong>Note:</strong><br />
                            <strong>OnNet Flow:</strong> Request Device Capabilities details from the AT&T network for the mobile device of an AT&T subscriber who is using an AT&T direct Mobile data connection to access this application. <br />
                            <strong>OffNet Flow:</strong> Where the end-user is not on an AT&T Mobile data connection or using a Wi-Fi or tethering connection while accessing this application.  This will result in an HTTP 400 error.
                        </div>
                    </div>
                </tbody>
            </table>
        </div>
        <br clear="all" />
        <div class="successWide" runat="server" id="tbDeviceCapabSuccess" visible="false">
            <strong>SUCCESS:</strong><br />
            Device parameters listed below.
        </div>
        <br />
        <div align="center">
            <table width="500" cellpadding="1" cellspacing="1" border="0" runat="server" id="tbDeviceCapabilities"
                visible="false">
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
                <tbody>
                    <tr>
                        <td class="cell" align="center">
                            <em>TypeAllocationCode</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblTypeAllocationCode" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>Name</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblName" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>Vendor</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblVendor" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>Model</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblModel" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>FirmwareVersion</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblFirmwareVersion" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>UaProf</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblUAProf" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>MmsCapable</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblMMSCapable" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>AssistedGps</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblAGPS" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>LocationTechnology</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblLocationTechnology" runat="server" /></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="cell" align="center">
                            <em>DeviceBrowser</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblDeviceBrowser" runat="server" /></em>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="cell" align="center">
                            <em>WapPushCapable</em>
                        </td>
                        <td class="cell" align="center">
                            <em>
                                <asp:Label ID="lblWAPPush" runat="server" /></em>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <br />
        <div id="tbDeviceCapabError" runat="server" cellspacing="1" class="errorWide" visible="false">
            <b>ERROR:</b><br />
            <asp:Label ID="lblErrorMessage" runat="server" Text="" />
        </div>
        <br clear="all" />
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
