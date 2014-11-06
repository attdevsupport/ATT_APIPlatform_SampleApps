<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AAB_App1" %>

<!DOCTYPE html>
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
<html lang="en">
<head runat="server">
    <title>AT&amp;T Sample Application - Contact Services (Address Book)</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" >
    <link rel="stylesheet" type="text/css" href="style/common.css">
    <link rel="stylesheet" type="text/css" href="style/contacts.css">
    <script src="scripts/utils.js"></script>
    <script src="scripts/contacts.js"></script>
</head>
<body>
    <div id="pageContainer">
        <div id="header">
            <div class="logo" id="top">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
            <ul class="links" id="nav">
                <li><a href="#" target="_blank">Full Page<img src="images/max.png" alt="" /></a> <span
                    class="divider">|&nbsp;</span> </li>
                <li><a id="sourceLink" runat="server" href="<%$ AppSettings:SourceLink %>" target="_blank">Source<img src="images/opensource.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="downloadLink" runat="server" href="<%$ AppSettings:DownloadLink %>" target="_blank">Download<img src="images/download.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="helpLink" runat="server" href="<%$ AppSettings:HelpLink %>" target="_blank">Help </a></li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
            <!-- end of links -->
        </div>
        <!-- end of header -->
        <form id="form1" runat="server">
            <div id="content">
                <div id="contentHeading">
                    <h1>AT&amp;T Sample Application - Contact Services (Address Book)</h1>
                    <div class="border">
                    </div>
                    <div id="introtext">
                        <div>
                            <b>Server Time:</b>
                            <%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %>
                        </div>
                        <div>
                            <b>Client Time:</b>
                            <script type="text/javascript">
                                var myDate = new Date();
                                document.write(myDate);
                            </script>
                        </div>
                        <div>
                            <b>User Agent:</b>
                            <script type="text/javascript">
                                document.write("" + navigator.userAgent);
                            </script>
                        </div>
                    </div>
                </div>
                <!-- end of contentHeading -->
                <!-- Start of Contacts -->
                <div class="lightBorder"></div>
                <div class="formBox" id="formBox">
                    <div id="formContainer" class="formContainer">
                       <% if (this.oauth_error != null)
                          {%> 
                        <div class="errorWide">
                            <strong>ERROR:</strong>
                            <%=this.oauth_error %>
                        </div>
                        <% } %>

                        <% if (this.config_error != null)
                          {%> 
                        <div class="errorWide">
                            <strong>ERROR:</strong>
                            <%=this.config_error %>
                        </div>
                        <% } %>
                        
                        <a id="contactsToggle"
                            href="javascript:toggle('contacts','contactsToggle', 'Contacts');">Contacts</a>
                        <div class="toggle" id="contacts">

                                <p>
                                    <input name="pagetype" runat="server" type="radio" value="1" onclick="showWindows(this);" />Create Contact 
                                <input name="pagetype" runat="server" type="radio" value="2" onclick="showWindows(this);" />Update/Delete Contact
                                <input name="pagetype" runat="server" type="radio" value="3" onclick="showWindows(this);" />Get Contacts
                                </p>
                                <div id="createContact" style="display: none;" class="CF">

                                    <div id="fields" class="menu">
                                        <table>
                                            <tr>
                                                <td>firstName</td>
                                                <td>
                                                    <asp:TextBox ID="firstName" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>middleName</td>
                                                <td>
                                                    <asp:TextBox ID="middleName" runat="server" name="middleName" placeholder="middleName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>lastName</td>
                                                <td>
                                                    <asp:TextBox ID="lastName" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>prefix</td>
                                                <td>
                                                    <asp:TextBox ID="prefix" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>suffix</td>
                                                <td>
                                                    <asp:TextBox ID="suffix" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>nickname</td>
                                                <td>
                                                    <asp:TextBox ID="nickname" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>organization</td>
                                                <td>
                                                    <asp:TextBox ID="organization" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>jobTitle</td>
                                                <td>
                                                    <asp:TextBox ID="jobTitle" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>anniversary</td>
                                                <td>
                                                    <asp:TextBox ID="anniversary" runat="server" name="anniversary" placeholder="anniversary"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>gender</td>
                                                <td>
                                                    <asp:TextBox ID="gender" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>spouse</td>
                                                <td>
                                                    <asp:TextBox ID="spouse" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>children</td>
                                                <td>
                                                    <asp:TextBox ID="children" runat="server" name="children" placeholder="children"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>hobby</td>
                                                <td>
                                                    <asp:TextBox ID="hobby" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>assistant</td>
                                                <td>
                                                    <asp:TextBox ID="assistant" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                        <fieldset>
                                            <legend>
                                                <button id="addPhone" type="button" onclick="addFields('phone', 'number', '8', '<asp:Literal runat=server Text="<%$ AppSettings:phone_class%>" />', 'phone');">
                                                    [+]
                                                </button>
                                                Phones 
                                            </legend>
                                            <table id="phone">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addIm" type="button" onclick="addFields('im', 'uri', '4', '<asp:Literal runat=server Text="<%$ AppSettings:im_class%>" />', 'im');">
                                                    [+]
                                                </button>
                                                IM 
                                            </legend>
                                            <table id="im">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addAddress" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<asp:Literal runat=server Text="<%$ AppSettings:address_class%>" />', 'address');">[+]</button>
                                                Addresses</legend>
                                            <table id="address">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addEmail" type="button" onclick="addFields('email', 'email_address', '4', '<asp:Literal runat=server Text="<%$ AppSettings:email_class%>" />', 'email');">[+]</button>
                                                Emails </legend>
                                            <table id="email">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addWebURLS" type="button" onclick="addFields('weburl', 'url', '3', '<asp:Literal runat=server Text="<%$ AppSettings:weburl_class%>" />', 'weburl');">
                                                    [+]
                                                </button>
                                                WebURLS 
                                            </legend>
                                            <table id="weburl">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>Photo</legend>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <label>Upload Photo : </label>
                                                    </td>
                                                    <td>
                                                        <select id="attachPhoto" runat="server">
                                                            <option>None</option>
                                                        </select><br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                        <asp:Button ID="btnCreateContact" class="submit" runat="server" Text="Create Contact" OnClick="createContact_Click" />
                                        <button type="button" class="submit" onclick="this.form.reset();return false;">Rest Fields</button>
                                    </div>
                                </div>
                                <div id="updateContact" style="display: none;" class="CF">
                                    <div id="fields" class="menu">

                                        <table>
                                            <tr>
                                                <td>contactId</td>
                                                <td>
                                                    <asp:TextBox ID="contactIdUpd" runat="server" name="contactIdUpd" placeholder="contactId"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>formattedName</td>
                                                <td>
                                                    <asp:TextBox ID="formattedNameUpd" runat="server" name="formattedNameUpd" placeholder="formattedName"></asp:TextBox>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td>firstName</td>
                                                <td>
                                                    <asp:TextBox ID="firstNameUpd" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>middleName</td>
                                                <td>
                                                    <asp:TextBox ID="middleNameUpd" runat="server" name="middleName" placeholder="middleName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>lastName</td>
                                                <td>
                                                    <asp:TextBox ID="lastNameUpd" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>prefix</td>
                                                <td>
                                                    <asp:TextBox ID="prefixUpd" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>suffix</td>
                                                <td>
                                                    <asp:TextBox ID="suffixUpd" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>nickname</td>
                                                <td>
                                                    <asp:TextBox ID="nicknameUpd" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>organization</td>
                                                <td>
                                                    <asp:TextBox ID="organizationUpd" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>jobTitle</td>
                                                <td>
                                                    <asp:TextBox ID="jobTitleUpd" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>anniversary</td>
                                                <td>
                                                    <asp:TextBox ID="anniversaryUpd" runat="server" name="anniversary" placeholder="anniversary"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>gender</td>
                                                <td>
                                                    <asp:TextBox ID="genderUpd" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>spouse</td>
                                                <td>
                                                    <asp:TextBox ID="spouseUpd" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>children</td>
                                                <td>
                                                    <asp:TextBox ID="childrenUpd" runat="server" name="children" placeholder="children"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>hobby</td>
                                                <td>
                                                    <asp:TextBox ID="hobbyUpd" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>assistant</td>
                                                <td>
                                                    <asp:TextBox ID="assistantUpd" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                        <fieldset>
                                            <legend>
                                                <button id="addPhoneUpd" type="button" onclick="addFields('phone', 'number', '8', '<asp:Literal runat=server Text="<%$ AppSettings:phone_class%>" />', 'phoneUpd');">[+]</button>
                                                Phones </legend>
                                            <table id="phoneUpd">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addImUpd" type="button" onclick="addFields('im', 'uri', '4', '<asp:Literal runat=server Text="<%$ AppSettings:im_class%>" />', 'imUpd');">[+]</button>
                                                IM </legend>
                                            <table id="imUpd">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addAddressUpd" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<asp:Literal runat="server" Text="<%$ AppSettings:address_class%>" />', 'addressUpd');">[+]</button>
                                                Addresses</legend>
                                            <table id="addressUpd">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addEmailUpd" type="button" onclick="addFields('email', 'email_address', '4', '<asp:Literal runat=server Text="<%$ AppSettings:email_class%>" />', 'emailUpd');">[+]</button>
                                                Emails </legend>
                                            <table id="emailUpd">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addWebURLSUpd" type="button" onclick="addFields('webUrl', 'url', '3', '<asp:Literal runat=server Text="<%$ AppSettings:weburl_class%>" />', 'webUrlsUpd');">[+]</button>
                                                WebURLS </legend>
                                            <table id="webUrlsUpd">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>Photo <i>(only for update operation)</i></legend>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <label>Upload Photo : </label>
                                                    </td>
                                                    <td>
                                                        <select id="attachPhotoUpd" runat="server">
                                                            <option>None</option>
                                                        </select><br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                        <asp:Button ID="btnUpdateContact" class="submit" runat="server" Text="Update Contact" OnClick="updateContact_Click" />
                                        <asp:Button ID="btnDeleteContact" class="submit" runat="server" Text="Delete Contact" OnClick="deleteContact_Click" />
                                        <button type="button" onclick="this.form.reset();return false;">Rest Fields</button>
                                    </div>
                                </div>
                                <div id="getContacts" style="display: none;" class="CF">
                                    <div id="fields" class="menu">
                                        <table>
                                            <tr>
                                                <td>Search Field</td>
                                                <td>
                                                    <asp:TextBox ID="searchVal" runat="server" name="searchVal" placeholder="Search Value"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <asp:Button ID="GetContacts" class="submit" runat="server" Text="Get Contacts" OnClick="getContacts_Click" />
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        <!-- end of Contacts -->
                    </div>
                    <!--end formContainer-->
                        <% if (this.create_contact != null)
                            {%> 
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                                <%= this.create_contact.location %>
                            </div>
                            <% } %>
                        <% if (this.success_contact != null)
                            {%> 
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                                <%= this.success_contact.last_modified %>
                            </div>
                            <% } %>

                        <% if (this.qContactResult != null && this.qContactResult.resultSet.quickContacts != null)
                            {%>
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                            </div>
                            <% foreach (var quickContact in qContactResult.resultSet.quickContacts.quickContact)
                             {%>
                        <fieldset>
                            <legend>Individual</legend>
                            <fieldset>
                                <legend>Information</legend>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>Contact ID</th>
                                            <th>formattedName</th>
                                            <th>firstName</th>
                                            <th>middleName</th>
                                            <th>lastName</th>
                                            <th>prefix</th>
                                            <th>suffix</th>
                                            <th>nickName</th>
                                            <th>organization</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="contactId"><%=quickContact.contactId%></td>
                                            <td data-value="formattedName"><%=quickContact.formattedName%></td>
                                            <td data-value="firstName"><%=quickContact.firstName%></td>
                                            <td data-value="middleName"><%=quickContact.middleName%></td>
                                            <td data-value="lastName"><%=quickContact.lastName%></td>
                                            <td data-value="prefix"><%=quickContact.prefix%></td>
                                            <td data-value="suffix"><%=quickContact.suffix%></td>
                                            <td data-value="nickName"><%=quickContact.nickName%></td>
                                            <td data-value="organization"><%=quickContact.organization%></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </fieldset>
                            <% if (quickContact.phone != null)
                                {
                                    %>
                            <fieldset>
                                <legend>Phones</legend>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>number</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.phone.type %>
                                            </td>
                                            <td data-value="number">
                                                <%= quickContact.phone.number %>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </fieldset>
                            <%}%>
                            <% if (quickContact.email != null)
                                {
                                    %>
                            <fieldset>
                                <legend>Emails</legend>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>address</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.email.type %>
                                            </td>
                                            <td data-value="emailaddress">
                                                <%= quickContact.email.emailAddress %>
                                            </td>
                                    </tbody>
                                </table>
                            </fieldset>
                            <%}%>
                            <% if (quickContact.im != null)
                                {
                                    %>
                            <fieldset>
                                <legend>IMs</legend>
                                <table>
                                    <thead>
                                        <th>type</th>
                                        <th>uri</th>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.im.type %>
                                            </td>
                                            <td data-value="im">
                                                <%= quickContact.im.imUri %>
                                            </td>
                                    </tbody>
                                </table>
                            </fieldset>
                            <%}%>
                            <% if (quickContact.address != null)
                                {
                                    %>
                            <fieldset>
                                <legend>Addresses</legend>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>po box</th>
                                            <th>address line 1</th>
                                            <th>address line 2</th>
                                            <th>city</th>
                                            <th>state</th>
                                            <th>zipcode</th>
                                            <th>country</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.address.type %>
                                            </td>
                                            <td data-value="po box">
                                                <%= quickContact.address.poBox %>
                                            </td>
                                            <td data-value="address line 1">
                                                <%= quickContact.address.addressLine1 %>
                                            </td>
                                            <td data-value="address line 2">
                                                <%= quickContact.address.addressLine2 %>
                                            </td>
                                            <td data-value="city">
                                                <%= quickContact.address.city %>
                                            </td>
                                            <td data-value="state">
                                                <%= quickContact.address.state %>
                                            </td>
                                            <td data-value="zipcode">
                                                <%= quickContact.address.zip %>
                                            </td>
                                            <td data-value="country">
                                                <%= quickContact.address.country %>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </fieldset>
                                <% } %>
                        </fieldset>
                            <% } %>
                        <% } %>
                           
                            
                         <% if (contact_error != null)
                                {%> 
                                <div class="errorWide">
                                    <strong>ERROR:</strong>
                                    <%=this.contact_error %>
                                </div>
                                <% } %>
                    </div>
                    <!--end formBox-->


                    <!-- Start of My User Profile -->
                    <div class="lightBorder"></div>
                    <div class="formBox" id="formBox">
                        <div id="formContainer" class="formContainer">
                            <a id="userProfileToggle"
                                href="javascript:toggle('userProfile','userProfileToggle', 'My User Profile');">My User Profile</a>
                            <div class="toggle" id="userProfile">
                                <asp:Button ID="getMyInfo" class="submit" runat="server" Text="Get MyInfo" OnClick="getMyInfo_Click" />
                                <p>
                                    <input name="pagetype" type="radio" value="4" onclick="showWindows(this);" />Update MyInfo
                                </p>
                                <div id="updateMyinfo" style="display: none;" class="CF">
                                    <div id="fields" class="menu">
                                        <table>
                                            <tr>
                                                <td>firstName</td>
                                                <td>
                                                    <asp:TextBox ID="firstNameMyInf" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>lastName</td>
                                                <td>
                                                    <asp:TextBox ID="lastNameMyInf" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>prefix</td>
                                                <td>
                                                    <asp:TextBox ID="prefixMyInf" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>suffix</td>
                                                <td>
                                                    <asp:TextBox ID="suffixMyInf" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>nickname</td>
                                                <td>
                                                    <asp:TextBox ID="nicknameMyInf" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>organization</td>
                                                <td>
                                                    <asp:TextBox ID="organizationMyInf" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>jobTitle</td>
                                                <td>
                                                    <asp:TextBox ID="jobTitleMyInf" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>anniversary</td>
                                                <td>
                                                    <asp:TextBox ID="anniversaryMyInf" runat="server" name="anniversary" placeholder="anniversary"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>gender</td>
                                                <td>
                                                    <asp:TextBox ID="genderMyInf" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>spouse</td>
                                                <td>
                                                    <asp:TextBox ID="spouseMyInf" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>children</td>
                                                <td>
                                                    <asp:TextBox ID="childrenMyInf" runat="server" name="children" placeholder="children"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>hobby</td>
                                                <td>
                                                    <asp:TextBox ID="hobbyMyInf" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>assistant</td>
                                                <td>
                                                    <asp:TextBox ID="assistantMyInf" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                        <fieldset>
                                            <legend>
                                                <button id="addPhoneMyInf" type="button" onclick="addFields('phone', 'number', '8', '<asp:Literal runat=server Text="<%$ AppSettings:phone_class%>" />', 'phoneMyInf');">
                                                    [+]
                                                </button>
                                                Phones 
                                            </legend>
                                            <table id="phoneMyInf">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addImMyInf" type="button" onclick="addFields('im', 'uri', '4', '<asp:Literal runat=server Text="<%$ AppSettings:im_class%>" />', 'imMyInf');">[+]</button>
                                                IM </legend>
                                            <table id="imMyInf">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addAddressMyInf" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<asp:Literal runat=server Text="<%$ AppSettings:address_class%>" />', 'addressMyInf');">[+]</button>
                                                Addresses</legend>
                                            <table id="addressMyInf">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addEmailMyInf" type="button" onclick="addFields('email', 'email_address', '4', '<asp:Literal runat=server Text="<%$ AppSettings:email_class%>" />', 'emailMyInf');">[+]</button>
                                                Emails </legend>
                                            <table id="emailMyInf">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>
                                                <button id="addWebURLSMyInf" type="button" onclick="addFields('weburl', 'url', '3', '<asp:Literal runat=server Text="<%$ AppSettings:weburl_class%>" />', 'webUrlsMyInf');">[+]</button>
                                                WebURLS </legend>
                                            <table id="webUrlsMyInf">
                                            </table>
                                        </fieldset>
                                        <fieldset>
                                            <legend>Photo <i>(only for update operation)</i></legend>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <label>Upload Photo : </label>
                                                    </td>
                                                    <td>
                                                        <select id="attachPhotoMyInf" runat="server">
                                                            <option>None</option>
                                                        </select><br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                        <asp:Button ID="btnUpdateMyinfo" class="submit" runat="server" Text="Update MyInfo" OnClick="updateMyInfo_Click" />
                                        <button type="button" onclick="this.form.reset();return false;">Rest Fields</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <% if (this.update_myinfo !=null)
                            {%> 
                            <div class="successWide">
                                <strong>SUCCESS:</strong>
                                <%= this.update_myinfo.last_modified %>
                            </div>
                            <% } %>
                        <% if (this.myInfoResult != null)
                            {%>
                        <fieldset>
                            <legend>Individual</legend>
                            <fieldset>
                                <legend>Information</legend>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>Contact ID</th>
                                            <th>Creation Date</th>
                                            <th>Modification Date</th>
                                            <th>formattedName</th>
                                            <th>firstName</th>
                                            <th>lastName</th>
                                            <th>prefix</th>
                                            <th>suffix</th>
                                            <th>nickName</th>
                                            <th>organization</th>
                                            <th>Job Title</th>
                                            <th>Anniversary</th>
                                            <th>Gender</th>
                                            <th>Spouse</th>
                                            <th>Hobby</th>
                                            <th>Assistant</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="contactId"><%=myInfoResult.myInfo.contactId%></td>
                                            <td data-value="creationDate"><%=myInfoResult.myInfo.creationDate%></td>
                                            <td data-value="modificationDate"><%=myInfoResult.myInfo.modificationDate%></td>
                                            <td data-value="formattedName"><%=myInfoResult.myInfo.formattedName%></td>
                                            <td data-value="firstName"><%=myInfoResult.myInfo.firstName%></td>
                                            <td data-value="lastName"><%=myInfoResult.myInfo.lastName%></td>
                                            <td data-value="prefix"><%=myInfoResult.myInfo.prefix%></td>
                                            <td data-value="suffix"><%=myInfoResult.myInfo.suffix%></td>
                                            <td data-value="nickName"><%=myInfoResult.myInfo.nickName%></td>
                                            <td data-value="organization"><%=myInfoResult.myInfo.organization%></td>
                                            <td data-value="jobTitle"><%=myInfoResult.myInfo.jobTitle%></td>
                                            <td data-value="anniversary"><%=myInfoResult.myInfo.anniversary%></td>
                                            <td data-value="gender"><%=myInfoResult.myInfo.gender%></td>
                                            <td data-value="spouse"><%=myInfoResult.myInfo.spouse%></td>
                                            <td data-value="hobby"><%=myInfoResult.myInfo.hobby%></td>
                                            <td data-value="assistant"><%=myInfoResult.myInfo.assistant%></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </fieldset>
                            <% if (myInfoResult.myInfo.phones != null)
                                {
                                    %>
                                <% foreach(var phone in myInfoResult.myInfo.phones.phone)
                                    {
                                        %>
                                <fieldset>
                                    <legend>Phones</legend>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>number</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= phone.type %>
                                                </td>
                                                <td data-value="number">
                                                    <%= phone.number %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </fieldset>
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.emails != null)
                                {
                                    %>
                                <% foreach(var email in myInfoResult.myInfo.emails.email)
                                        {
                                            %>
                                <fieldset>
                                    <legend>Emails</legend>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>address</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= email.type %>
                                                </td>
                                                <td data-value="emailaddress">
                                                    <%= email.emailAddress %>
                                                </td>
                                        </tbody>
                                    </table>
                                </fieldset>
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.ims != null)
                                {
                                    %>
                                <% foreach(var im in myInfoResult.myInfo.ims.im)
                                            {
                                                %>
                                <fieldset>
                                    <legend>IMs</legend>
                                    <table>
                                        <thead>
                                            <th>type</th>
                                            <th>uri</th>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= im.type %>
                                                </td>
                                                <td data-value="im">
                                                    <%= im.imUri %>
                                                </td>
                                        </tbody>
                                    </table>
                                </fieldset>
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.addresses != null)
                                {
                                    %>
                                <% foreach(var address in myInfoResult.myInfo.addresses.address)
                                                {
                                                    %>
                                <fieldset>
                                    <legend>Addresses</legend>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>po box</th>
                                                <th>address line 1</th>
                                                <th>address line 2</th>
                                                <th>city</th>
                                                <th>state</th>
                                                <th>zipcode</th>
                                                <th>country</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= address.type %>
                                                </td>
                                                <td data-value="po box">
                                                    <%= address.poBox %>
                                                </td>
                                                <td data-value="address line 1">
                                                    <%= address.addressLine1 %>
                                                </td>
                                                <td data-value="address line 2">
                                                    <%= address.addressLine2 %>
                                                </td>
                                                <td data-value="city">
                                                    <%= address.city %>
                                                </td>
                                                <td data-value="state">
                                                    <%= address.state %>
                                                </td>
                                                <td data-value="zipcode">
                                                    <%= address.zipcode %>
                                                </td>
                                                <td data-value="country">
                                                    <%= address.country %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </fieldset>
                                <% } %>
                            <% } %>
                        </fieldset>
                        <% } %>
                        <% if (this.myinfo_error != null)
                            {%> 
                            <div class="errorWide">
                                <strong>ERROR:</strong>
                                <%= this.myinfo_error %>
                            </div>
                            <% } %>
                    </div>
                    <!-- end of My User Profile -->

                    <!-- Start of Groups -->
                    <div class="lightBorder"></div>
                    <div class="formBox" id="formBox">
                        <div id="formContainer" class="formContainer">
                            <a id="groupsToggle"
                                href="javascript:toggle('groups','groupsToggle', 'Groups');">Groups</a>
                            <div class="toggle" id="groups">
                                    <p>
                                        <input name="pagetype" type="radio" value="5" onclick="showWindows(this);" />Create Group
              <input name="pagetype" type="radio" value="6" onclick="showWindows(this);" />Update Group
              <input name="pagetype" type="radio" value="7" onclick="showWindows(this);" />Delete Group
              <input name="pagetype" type="radio" value="8" onclick="showWindows(this);" />Get Groups
                                    </p>
                                    <div id="createGroup" style="display: none;" class="CF">
                                        <div id="fields" class="menu">
                                            <table>
                                                <tr>
                                                    <td>groupName</td>
                                                    <td>
                                                        <asp:TextBox ID="groupName" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>groupType</td>
                                                    <td>
                                                        <asp:TextBox ID="groupType" runat="server" name="groupType" placeholder="USER" Enabled="False"></asp:TextBox>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Button ID="btnCreatGroup" class="submit" runat="server" Text="Create Group" OnClick="createGroup_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div id="updateGroup" style="display: none;" class="CF">
                                        <div id="fields" class="menu">
                                            <table>
                                                <tr>
                                                    <td>groupId</td>
                                                    <td>
                                                        <asp:TextBox ID="groupIdUpd" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>groupName</td>
                                                    <td>
                                                        <asp:TextBox ID="groupNameUpd" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>groupType</td>
                                                    <td>
                                                        <asp:TextBox ID="groupTypeUpd" runat="server" name="groupType" placeholder="USER" Enabled="false"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Button ID="btnUpdateGrp" class="submit" runat="server" Text="Update Group" OnClick="updateGroup_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div id="deleteGroup" style="display: none;" class="CF">
                                        <div id="fields" class="menu">
                                            <table>
                                                <tr>
                                                    <td>groupId</td>
                                                    <td>
                                                        <asp:TextBox ID="groupIdDel" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Button ID="btnDeleteGroup" class="submit" runat="server" Text="Delete Group" OnClick="deleteGroup_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div id="getGroups" style="display: none;" class="CF">
                                            <div id="fields" class="menu">
                                                <table>
                                                    <tr>
                                                        <td>groupName</td>
                                                        <td>
                                                            <asp:TextBox ID="getGroupName" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>order</td>
                                                        <td>
                                                            <asp:ListBox ID="order" runat="server">
                                                                <asp:ListItem Value="ASC">Ascending</asp:ListItem>
                                                                <asp:ListItem Value="DESC">Descending</asp:ListItem>
                                                            </asp:ListBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Button ID="btnGetGroups" class="submit" runat="server" Text="Get Group" OnClick="getGroups_Click" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <label><b>Search Results </b><i>(Displays Max 3 Search Results)</i></label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <% if (this.create_group!=null)
                                {%> 
                                <div class="successWide">
                                    <strong>SUCCESS:</strong>
                                    <%= this.create_group.location %>
                                </div>
                                <% } %>
                                <% if (this.success_group != null)
                                {%> 
                                <div class="successWide">
                                    <strong>SUCCESS:</strong>
                                    <%= this.success_group.last_modified %>
                                </div>
                                <% } %>
                                <% if (this.group_error != null)
                                {%> 
                                <div class="errorWide">
                                    <strong>ERROR:</strong>
                                    <%= this.group_error %>
                                </div>
                                <% } %>
                        
                        
                                <% if (this.groupResult != null && this.groupResult.resultSet.totalRecords != "0")
                                    {%>
                                    <div class="successWide">
                                        <strong>SUCCESS:</strong>
                                    </div>
                                    <table>
                                        <thead>
                                            <th>groupId</th>
                                            <th>groupName</th>
                                            <th>groupType</th>
                                        </thead>
                                        <tbody>
                                            <%  foreach (var group in groupResult.resultSet.groups.group)
                                                {%>
                                            <tr>
                                                <td data-value="groupId">
                                                    <%= group.groupId %>
                                                </td>
                                                <td data-value="groupName">
                                                    <%= group.groupName %>
                                                </td>
                                                <td data-value="groupType">
                                                    <%= group.groupType %>
                                                </td>
                                            </tr>
                                            <% } %>
                                        </tbody>
                                    </table>
                                <% } %>
                            </div>
                            <!-- end of Groups -->
                            <!-- Start of Managing Groups/Contacts -->
                            <div class="lightBorder"></div>
                            <div class="formBox" id="formBox">
                                <a id="grpContactsToggle"
                                    href="javascript:toggle('grpContacts','grpContactsToggle', 'Managing Groups/Contacts');">Managing Groups/Contacts</a>

                                <div class="toggle" id="grpContacts">
                                        <p>
                                            <input name="pagetype" type="radio" value="9" onclick="showWindows(this);" />Contact and Group Operations
                                        </p>
                                        <div id="getGroupContacts" style="display: none;" class="CF">
                                                <div id="fields" class="menu">
                                                    <fieldset>
                                                        <legend>Get Group Contacts</legend>
                                                        <table>
                                                            <tr>
                                                                <td>groupId</td>
                                                                <td>
                                                                    <asp:TextBox ID="groupIdContacts" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button ID="groupIdContactsBtn" class="submit" runat="server" Text="Get Group Contacts" OnClick="groupIdContacts_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Add Contacts to Group</legend>
                                                        <table>
                                                            <tr>
                                                                <td>groupId</td>
                                                                <td>
                                                                    <asp:TextBox ID="groupIdAddDel" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Contact Id's <i>(comma (,) delimited)</i></td>
                                                                <td>
                                                                    <asp:TextBox ID="addContactsGrp" runat="server" name="contactIds" placeholder="contactId(s)"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button ID="btnGroupIdContactsAdd" class="submit" runat="server" Text="Add Contacts to Group" OnClick="addContctsToGroup_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Remove Contacts from Group</legend>
                                                        <table>
                                                            <tr>
                                                                <td>groupId</td>
                                                                <td>
                                                                    <asp:TextBox ID="groupIdRemDel" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Contact Id's <i>(comma (,) delimited)</i></td>
                                                                <td>
                                                                    <asp:TextBox ID="remContactsGrp" runat="server" name="contactIds" placeholder="contactId(s)"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button ID="btnGroupIdContactsRem" class="submit" runat="server" Text="Remove Contacts from Group" OnClick="removeContctsFromGroup_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Get Contact Groups</legend>
                                                        <table>
                                                            <tr>
                                                                <td>contactId</td>
                                                                <td>
                                                                    <asp:TextBox ID="contactsIdGroups" runat="server" name="contactId" placeholder="contactId"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    <asp:Button ID="btnContactsIdGroups" class="submit" runat="server" Text="Get Contact Groups" OnClick="getContactGroups_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                        <% if (contactIdResult != null)
                                        {%>
                                        <div class="successWide">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        <table>
                                            <thead>
                                                <th>Contact Id</th>
                                            </thead>
                                            <tbody>
                                                <% foreach (var id in contactIdResult.contactIds.id)
                                                    { %>
                                                <tr>
                                                    <td data-value="Contact Id">
                                                        <%= id %>
                                                    </td>
                                                </tr>
                                                <% } %>
                                            </tbody>
                                        </table>
                                        <% } %>
                                        <% if (this.contactGroupResult != null)
                                            {%>
                                        <div class="successWide">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        <table>
                                            <thead>
                                                <th>groupId</th>
                                                <th>groupName</th>
                                                <th>groupType</th>
                                            </thead>
                                            <tbody>
                                                <%  foreach (var group in contactGroupResult.resultSet.groups.group)
                                                    {%>
                                                <tr>
                                                    <td data-value="groupId">
                                                        <%= group.groupId %>
                                                    </td>
                                                    <td data-value="groupName">
                                                        <%= group.groupName %>
                                                    </td>
                                                    <td data-value="groupType">
                                                        <%= group.groupType %>
                                                    </td>
                                                </tr>
                                                <% } %>
                                            </tbody>
                                        </table>
                                        <% } %>
                                        <% if (this.manage_groups != null)
                                            {%>
                                            <div class="successWide">
                                                <strong>SUCCESS:</strong>
                                                <%= this.manage_groups.last_modified %>
                                            </div>
                                        <% } %>
                                        <% if (this.manage_groups_error != null)
                                            { %>
                                        <div class="errorWide">
                                            <strong>Error</strong><br />
                                            <%= this.manage_groups_error %>
                                        </div>
                                        <% } %>
                                    </div>

                                    <!-- end of Managing Groups/Contacts -->

<%--
                                    <% if (successResponse != "")
                                       { %>
                                    <div class="successWide">
                                        <strong>Success</strong><br />
                                        <%=successResponse%>
                                    </div>
                                    <% } %>
                                    <% if (errorResponse != "")
                                       { %>
                                    <div class="errorWide">
                                        <strong>Error</strong><br />
                                        <%=errorResponse%>
                                    </div>
                                    <% } %>--%>
                                    <div class="border"></div>
                                    <div id="footer">
                                        <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
                                        <p>
                                            The Application hosted on this site are working examples intended to
        be used for reference in creating products to consume AT&amp;T
        Services and not meant to be used as part of your product. The data
        in these pages is for test purposes only and intended only for use
        as a reference in how the services perform.
                        <br>
                                            <br>
                                            For download of tools and documentation, please go to <a
                                                href="https://developer.att.com/developer/mvc/auth/login" target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
                                            <br>
                                            For more information please go to <a
                                                href="https://developer.att.com/support" target="_blank">https://developer.att.com/support</a>
                                            <br>
                                            <br>
                                            &#169; 2014 AT&amp;T Intellectual Property. All rights
        reserved. <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
                                        </p>
                                    </div>
                                    <!-- end of footer -->
                                </div>
        </form>
    </div>
</body>
</html>
