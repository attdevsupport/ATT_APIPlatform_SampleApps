/*
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
*/

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Reflection;
/// <summary>
///  This Sample App requires the third party library: Newtonsoft.Json 
///  which can be found at: http://http://james.newtonking.com/json
///  Make sure that Newtonsoft.Json has been installed, then require the class.
/// </summary>
using Newtonsoft.Json;
/// <summary>
///  This Sample App for the Address Book API requires the C# code kit, 
///  which can be found at: https://github.com/attdevsupport/codekit-csharp
///  Make sure that the ATT.AAB has been compiled and referenced in the project.
/// </summary>
using ATT.Codekit.AAB;
using ATT.Codekit.Authorization;


#endregion
public partial class AAB_App1 : System.Web.UI.Page
{
    public OAuth oauth;
    public AddressBook addressbook;
    public string bypassSSL;
    public string endPoint;
    public string scope;
    public string apiKey;
    public string authorizeRedirectUri;
    public string authCode;
    public string secretKey;
    public string accessToken;
    public int refreshTokenExpiresIn;
    public string refreshToken;
    public string refreshTokenExpiryTime;
    public string refreshExpiry;
    public JavaScriptSerializer serializer;
    public string getContactsJson = string.Empty;
    public string contactID = string.Empty;
    public string queryString = string.Empty;
    public static int contactsResponseCount = 0;
    public QuickContactObj.RootObject qContactResult = null;
    public ContactObj.RootObject myInfoResult = null;
    public GroupObj.RootObject groupResult = null;
    public GroupObj.RootObject contactGroupResult = null;
    public ContactIdObj.RootObject contactIdResult = null;
    public string JSONstring = string.Empty;
    public string groupID = string.Empty;
    public string AttachmentFilesDir = string.Empty;
    public Success create_contact = null;
    public Success success_contact = null;
    public Success update_myinfo = null;
    public Success create_group = null;
    public Success success_group = null;
    public Success manage_groups = null;
    public string oauth_error = null;
    public string config_error = null;
    public string contact_error = null;
    public string myinfo_error = null;
    public string group_error = null;
    public string manage_groups_error = null;

    public string showCreateContact = string.Empty;
    public string showUpdateContact = string.Empty;
    public string showDeleteContact = string.Empty;
    public string showGetContacts = string.Empty;
    public string showGetMyInfo = string.Empty;
    public string showUpdateMyInfo = string.Empty;
    public string showCreateGroup = string.Empty;
    public string showUpdateGroup = string.Empty;
    public string showDeleteGroup = string.Empty;
    public string showGetGroups = string.Empty;
    public string showGetGroupContacts = string.Empty;
    public string showAddContactsToGroup = string.Empty;
    public string showRemoveContactsfrmGroup = string.Empty;
    public string showGetContactGroups = string.Empty;
    /// <summary>
    /// Initial set up for the application. We read the config file and create the instance of Oauth and addressbook object.
    /// If it's post back from getting Auth code then perform operations.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ReadConfigFile();
        oauth = new OAuth(endPoint, scope, apiKey, secretKey, authorizeRedirectUri, refreshTokenExpiresIn, bypassSSL);
        this.serializer = new JavaScriptSerializer();
        if (Session["cs_rest_AccessToken"] != null && this.addressbook == null)
        {
            this.addressbook = new AddressBook(this.endPoint, Session["cs_rest_AccessToken"].ToString());
        }
        if ((string)Session["cs_rest_appState"] == "GetToken" && Request["Code"] != null)
        {
            this.oauth.authCode = Request["code"].ToString();
            try
            {
                if (oauth.GetAccessToken(OAuth.AccessTokenType.AuthorizationCode) == true)
                {
                    StoreAccessTokenToSession(oauth.accessTokenJson);
                    this.addressbook = new AddressBook(this.endPoint, this.accessToken);
                    Operation operation = (Operation)Session["cs_rest_ServiceRequest"];
                    switch (operation)
                    {
                        case Operation.CreateContactOperation:
                            showCreateContact = "true";
                            try
                            {
                                if (null == addressbook.createContact(Session["JSONstring"].ToString()))
                                {
                                    this.contact_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.create_contact = new Success();
                                    this.create_contact.location = this.addressbook.apiService.apiResponse.location;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.contact_error = ex.Message;
                            }
                            break;
                        case Operation.UpdateContactOperation:
                            showUpdateContact = "true";
                            try
                            {
                                if (!addressbook.updateContact(Session["contactid"].ToString(), Session["JSONstring"].ToString()))
                                {
                                    this.contact_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.success_contact = new Success();
                                    this.success_contact.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.contact_error = ex.Message;
                            }
                            break;
                        case Operation.DeleteContactOperation:
                            showDeleteContact = "true";
                            try
                            {
                                if (!addressbook.deleteContact(Session["contactid"].ToString()))
                                {
                                    this.contact_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.success_contact = new Success();
                                    this.success_contact.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.contact_error = ex.Message;
                            }
                            break;
                        case Operation.GetContactsOperation:
                            showGetContacts = "true";
                            try
                            {
                                this.qContactResult = addressbook.getContacts(Session["querystring"].ToString());
                                if (null == this.qContactResult)
                                {
                                    this.contact_error = this.addressbook.apiService.errorResponse;
                                }
                                //else
                                //{
                                    //try
                                    //{
                                        //this.qContactResult = serializer.Deserialize<QuickContactObj.RootObject>(addressbook.JSONstring);
                                      //  this.qContactResult = addressbook.getContacts(Session["querystring"].ToString());
                                    //}
                                    //catch (Exception ex)
                                    //{
                                     //   this.contact_error = ex.Message;
                                    //}
                                //}
                            }
                            catch (Exception ex)
                            {
                                this.contact_error = ex.Message;
                            }
                            break;
                        case Operation.UpdateMyInfoOperation:
                            showUpdateMyInfo = "true";
                            try
                            {
                                if (!addressbook.updateMyInfo(Session["JSONstring"].ToString()))
                                {
                                    this.myinfo_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.update_myinfo = new Success();
                                    this.update_myinfo.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.myinfo_error = ex.Message;
                            }
                            break;
                        case Operation.GetMyInfoOperation:
                            showGetMyInfo = "true";
                            try
                            {
                                string result = addressbook.getMyInfo();
                                if (null == result)
                                {
                                    this.myinfo_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    //try
                                    //{
                                    this.myInfoResult = serializer.Deserialize<ContactObj.RootObject>(result);
                                    //}

                                }
                            }
                            catch (Exception ex)
                            {
                                this.myinfo_error = ex.Message;
                            }
                            break;
                        case Operation.CreateGroupOperation:
                            showCreateGroup = "true";
                            try
                            {
                                if (null == addressbook.createGroup(Session["JSONstring"].ToString()))
                                {
                                    this.group_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.create_group = new Success();
                                    this.create_group.location = this.addressbook.apiService.apiResponse.location;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.group_error = ex.Message;
                            }
                            break;
                        case Operation.UpdateGroupOperation:
                            showUpdateGroup = "true";
                            try
                            {
                                if (!addressbook.updateGroup(Session["groupid"].ToString(), Session["JSONstring"].ToString()))
                                {
                                    this.group_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.success_group = new Success();
                                    this.success_group.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.group_error = ex.Message;
                            }
                            break;
                        case Operation.DeleteGroupOperation:
                            showDeleteGroup = "true";
                            if (!addressbook.deleteGroup(Session["groupid"].ToString()))
                            {
                                this.group_error = this.addressbook.apiService.errorResponse;
                            }
                            else
                            {
                                this.success_group = new Success();
                                this.success_group.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                            }
                            break;
                        case Operation.GetGroupsOperation:
                            showGetGroups = "true";
                            try
                            {
                                this.groupResult = addressbook.getGroups(Session["querystring"].ToString());
                                if (null == this.groupResult)
                                {
                                    this.group_error = this.addressbook.apiService.errorResponse;
                                }
                                //else
                                //{
                                //  try
                                //{
                                //this.groupResult = serializer.Deserialize<GroupObj.RootObject>(addressbook.JSONstring);

                                //}

                                //}
                            }
                            catch (Exception ex)
                            {
                                this.group_error = ex.Message;
                            }
                            break;
                        case Operation.GetGroupContactsOperation:
                            showGetGroupContacts = "true";
                            try
                            {
                                String result = addressbook.getGroupContacts(Session["groupid"].ToString());
                                if (null == result)
                                {
                                    this.manage_groups_error = this.addressbook.apiService.errorResponse;
                                }
                                //else
                                //{
                                //  try
                                //{

                                //}
                                this.contactIdResult = serializer.Deserialize<ContactIdObj.RootObject>(result);

                            }
                            catch (Exception ex)
                            {
                                this.manage_groups_error = ex.Message;
                            }
                            break;
                        case Operation.AddContctsToGroupOperation:
                            showAddContactsToGroup = "true";
                            try
                            {

                                if (!addressbook.addContactToGroup(Session["groupid"].ToString(), Session["contactids"].ToString()))
                                {
                                    this.manage_groups_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.manage_groups = new Success();
                                    this.manage_groups.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.manage_groups_error = ex.Message;
                            }
                            break;
                        case Operation.RemoveContactsFromGroupOperation:
                            showRemoveContactsfrmGroup = "true";
                            try
                            {
                                if (!addressbook.removeContactsFromGroup(Session["groupid"].ToString(), Session["contactids"].ToString()))
                                {
                                    this.manage_groups_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    this.manage_groups = new Success();
                                    this.manage_groups.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.manage_groups_error = ex.Message;
                            }
                            break;
                        case Operation.GetContactGroupsOperation:
                            showGetContactGroups = "true";
                            try
                            {
                                String result = addressbook.getContactGroups(Session["contactid"].ToString());
                                if (null == result)
                                {
                                    this.manage_groups_error = this.addressbook.apiService.errorResponse;
                                }
                                else
                                {
                                    //   try
                                    //  {
                                    //this.contactGroupResult = serializer.Deserialize<GroupObj.RootObject>(result);
                                    //}

                                    //}

                                    this.contactGroupResult = serializer.Deserialize<GroupObj.RootObject>(result);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.manage_groups_error = ex.Message;
                            }

                            break;
                    }
                    ResetRequestSessionVariables(operation);
                }
                else
                {
                    if (oauth.getAuthCodeError != null)
                    {
                        this.oauth_error = "GetAuthCodeError: " + oauth.getAuthCodeError;
                    }
                    if (oauth.GetAccessTokenError != null)
                    {
                        this.oauth_error = "GetAccessTokenError: " + oauth.GetAccessTokenError;
                    }
                    this.ResetTokenSessionVariables();
                    return;
                }
            }
            catch (Exception ex)
            {
                this.oauth_error = "GetAuthCodeError: " + ex.Message;
                this.ResetTokenSessionVariables();
            }

        }
    }

    /// <summary>
    /// This region is collection of onclick buttons which make the request to addressbook API 
    /// </summary>
    #region Contact Operations
    public void createContact_Click(object sender, EventArgs e)
    {
        showCreateContact = "true";
        try
        {
            checkAccessToken(Operation.CreateContactOperation);
            var json = this.getContactJSON(Operation.CreateContactOperation);
        
            if (null == addressbook.createContact(json))
            {
                this.contact_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.create_contact = new Success();
                this.create_contact.location = this.addressbook.apiService.apiResponse.location;
            }
        }
        catch (Exception ex)
        {
            this.contact_error = ex.Message;
        }
    }

    public void updateContact_Click(object sender, EventArgs e)
    {
        showUpdateContact = "true";
        var contactid = contactIdUpd.Text.Trim();
        try
        {
            if (contactid == "")
            {
                this.contact_error = "Please enter contact ID.";
            }
            else
            {
                checkAccessToken(Operation.UpdateContactOperation);
                var json = this.getContactJSON(Operation.UpdateContactOperation);
                if (!addressbook.updateContact(contactid, json))
                {
                    this.contact_error = this.addressbook.apiService.errorResponse;
                }
                else
                {
                    this.success_contact = new Success();
                    this.success_contact.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                }
            }
        }
        catch (Exception ex)
        {
            this.contact_error = ex.Message;
        }
    }

    public void deleteContact_Click(object sender, EventArgs e)
    {
        showDeleteContact = "true";
        var contactid = contactIdDel.Text;
        try
        {
            if (contactid == "")
            {
                this.contact_error = "Please enter contact ID.";
            }
            else
            {
                checkAccessToken(Operation.DeleteContactOperation);
                if (!addressbook.deleteContact(contactid))
                {
                    this.contact_error = this.addressbook.apiService.errorResponse;
                }
                else
                {
                    this.success_contact = new Success();
                    this.success_contact.last_modified = this.addressbook.apiService.apiResponse.lastModified;
                }
            }
        }
        catch (Exception ex)
        {
            this.contact_error = ex.Message;
        }
    }

    public void getContacts_Click(object sender, EventArgs e)
    {
        showGetContacts = "true";
        try
        {
            checkAccessToken(Operation.GetContactsOperation);
            var qs = searchVal.Text;
            this.qContactResult = addressbook.getContacts(qs);
            if (null == this.qContactResult)
            {
                this.contact_error = this.addressbook.apiService.errorResponse;
            }
            //else
            //{
            //try
            //{
            //  this.qContactResult = serializer.Deserialize<QuickContactObj.RootObject>(addressbook.JSONstring);
            //}

            //}
        }
        catch (Exception ex)
        {
            this.contact_error = ex.Message;
        }
    }

    #endregion

    #region MyInfo Operations
    public void updateMyInfo_Click(object sender, EventArgs e)
    {
        showUpdateMyInfo = "true";
        try
        {
            checkAccessToken(Operation.UpdateMyInfoOperation);
            var json = this.getContactJSON(Operation.UpdateMyInfoOperation);
            if (!addressbook.updateMyInfo(json))
            {
                this.myinfo_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.update_myinfo = new Success();
                this.update_myinfo.last_modified = this.addressbook.apiService.apiResponse.lastModified;
            }
        }
        catch (Exception ex)
        {
            this.myinfo_error = ex.Message;
        }
    }

    public void getMyInfo_Click(object sender, EventArgs e)
    {
        showGetMyInfo = "true";
        checkAccessToken(Operation.GetMyInfoOperation);
        try
        {

            string result = addressbook.getMyInfo();
            if (null == result)
            {
                this.myinfo_error = this.addressbook.apiService.errorResponse;
            }
            else
            {

                this.myInfoResult = serializer.Deserialize<ContactObj.RootObject>(result);


            }
        }
        catch (Exception ex)
        {
            this.myinfo_error = ex.Message;
        }
    }

    #endregion

    #region group
    public void createGroup_Click(object sender, EventArgs e)
    {
        showCreateGroup = "true";
        checkAccessToken(Operation.CreateGroupOperation);
        var json = this.getGroupJSON(Operation.CreateGroupOperation);
        try
        {
            if (null == addressbook.createGroup(json))
            {
                this.group_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.create_group = new Success();
                this.create_group.location = this.addressbook.apiService.apiResponse.location;
            }
        }
        catch (Exception ex)
        {
            this.group_error = ex.Message;
        }
    }

    public void updateGroup_Click(object sender, EventArgs e)
    {
        showUpdateGroup = "true";
        checkAccessToken(Operation.UpdateGroupOperation);
        var json = this.getGroupJSON(Operation.UpdateGroupOperation);
        try
        {
            if (!addressbook.updateGroup(groupIdUpd.Text, json))
            {
                this.group_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.success_group = new Success();
                this.success_group.last_modified = this.addressbook.apiService.apiResponse.lastModified;
            }
        }
        catch (Exception ex)
        {
            this.group_error = ex.Message;
        }
    }

    public void deleteGroup_Click(object sender, EventArgs e)
    {
        showDeleteGroup = "true";
        checkAccessToken(Operation.DeleteGroupOperation);
        try
        {
            if (!addressbook.deleteGroup(groupIdDel.Text))
            {
                this.group_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.success_group = new Success();
                this.success_group.last_modified = this.addressbook.apiService.apiResponse.lastModified;
            }
        }
        catch (Exception ex)
        {
            this.group_error = ex.Message;
        }
    }

    public void getGroups_Click(object sender, EventArgs e)
    {
        showGetGroups = "true";
        checkAccessToken(Operation.GetGroupsOperation);
        var qs = "?order=" + order.SelectedValue + "&groupName=" + getGroupName.Text;
        try
        {
            this.groupResult = addressbook.getGroups(qs);
            if (null == this.groupResult)
            {
                this.group_error = this.addressbook.apiService.errorResponse;
            }
            //else
            //{
            // try
            //{
            //  this.groupResult = serializer.Deserialize<GroupObj.RootObject>(addressbook.JSONstring);
            //}
            //}
        }
        catch (Exception ex)
        {
            this.group_error = ex.Message;
        }
    }
    #endregion

    #region groups
    public void groupIdContacts_Click(object sender, EventArgs e)
    {
        showGetGroupContacts = "true";
        checkAccessToken(Operation.GetGroupContactsOperation);
        try
        {
            string result = addressbook.getGroupContacts(groupIdContacts.Text);
            if (null == result)
            {
                this.manage_groups_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                //try
                //{
                this.contactIdResult = serializer.Deserialize<ContactIdObj.RootObject>(result);
                //}

            }
        }
        catch (Exception ex)
        {
            this.manage_groups_error = ex.Message;
        }
    }

    public void addContctsToGroup_Click(object sender, EventArgs e)
    {
        showAddContactsToGroup = "true";
        try
        {
            checkAccessToken(Operation.AddContctsToGroupOperation);
            if (!addressbook.addContactToGroup(groupIdAddDel.Text.Trim(), addContactsGrp.Text.Trim()))
            {
                this.manage_groups_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.manage_groups = new Success();
                this.manage_groups.last_modified = this.addressbook.apiService.apiResponse.lastModified;
            }
        }
        catch (Exception ex)
        {
            this.manage_groups_error = ex.Message;
        }
    }

    public void removeContctsFromGroup_Click(object sender, EventArgs e)
    {
        showRemoveContactsfrmGroup = "true";
        try
        {
            checkAccessToken(Operation.RemoveContactsFromGroupOperation);
            if (!addressbook.removeContactsFromGroup(groupIdRemDel.Text.Trim(), remContactsGrp.Text.Trim()))
            {
                this.manage_groups_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                this.manage_groups = new Success();
                this.manage_groups.last_modified = this.addressbook.apiService.apiResponse.lastModified;
            }
        }
        catch (Exception ex)
        {
            this.manage_groups_error = ex.Message;
        }
    }

    public void getContactGroups_Click(object sender, EventArgs e)
    {
        showGetContactGroups = "true";
        try
        {
            checkAccessToken(Operation.GetContactGroupsOperation);
        
            string result = addressbook.getContactGroups(contactsIdGroups.Text);
            if (null == result)
            {
                this.manage_groups_error = this.addressbook.apiService.errorResponse;
            }
            else
            {
                //try
                //{
                this.contactGroupResult = serializer.Deserialize<GroupObj.RootObject>(result);
                //}

            }
        }
        catch (Exception ex)
        {
            this.manage_groups_error = ex.Message;
        }
    }
    #endregion
    /// <summary>
    /// This region is a collection of function that are related to setting varaibles and session that are needed for API calls
    /// </summary>
    #region operation session variables
    /// <summary>
    /// This function is for storing the user input in the session and it can be retrieved when user is post back from getting auth code. 
    /// </summary>
    private void SetRequestSessionVariables(Operation op)
    {
        switch (op)
        {
            case Operation.CreateContactOperation:
            case Operation.UpdateMyInfoOperation:
                Session["JSONstring"] = this.getContactJSON(op);
                break;
            case Operation.UpdateContactOperation:
                Session["JSONstring"] = this.getContactJSON(op);
                Session["contactid"] = contactIdUpd.Text.Trim();
                break;
            case Operation.DeleteContactOperation:
                Session["contactid"] = contactIdUpd.Text.Trim();
                break;
            case Operation.CreateGroupOperation:
                Session["JSONstring"] = this.getGroupJSON(op);
                break;
            case Operation.UpdateGroupOperation:
                Session["JSONstring"] = this.getGroupJSON(op);
                Session["groupid"] = groupIdUpd.Text;
                break;
            case Operation.GetContactsOperation:
                Session["querystring"] = searchVal.Text;
                break;
            case Operation.DeleteGroupOperation:
                Session["groupid"] = groupIdDel.Text.Trim();
                break;
            case Operation.GetGroupsOperation:
                Session["querystring"] = "?order=" + order.SelectedValue + "&groupName=" + getGroupName.Text;
                break;
            case Operation.GetGroupContactsOperation:
                Session["groupid"] = groupIdContacts.Text;
                break;
            case Operation.AddContctsToGroupOperation:
                Session["groupid"] = groupIdAddDel.Text.Trim();
                Session["contactids"] = addContactsGrp.Text.Replace(",", "%2C").Trim();
                break;
            case Operation.RemoveContactsFromGroupOperation:
                Session["groupid"] = groupIdRemDel.Text.Trim();
                Session["contactids"] = remContactsGrp.Text.Replace(",", "%2C").Trim();
                break;
            case Operation.GetContactGroupsOperation:
                Session["contactid"] = contactsIdGroups.Text.Trim();
                break;
        }
    }
    /// <summary>
    /// This function is for reseting the sessions and variables to null
    /// </summary>
    private void ResetRequestSessionVariables(Operation op)
    {
        switch (op)
        {
            case Operation.CreateContactOperation:
            case Operation.UpdateContactOperation:
            case Operation.UpdateMyInfoOperation:
            case Operation.CreateGroupOperation:
            case Operation.UpdateGroupOperation:
                Session["JSONstring"] = null;
                if (Session["contactid"] != null)
                    Session["contactid"] = null;
                break;
            case Operation.GetGroupsOperation:
            case Operation.GetContactsOperation:
                Session["querystring"] = null;
                break;
            case Operation.DeleteGroupOperation:
            case Operation.GetGroupContactsOperation:
                Session["groupid"] = null;
                break;
            case Operation.AddContctsToGroupOperation:
            case Operation.RemoveContactsFromGroupOperation:
                Session["contactids"] = null;
                goto case Operation.GetGroupContactsOperation;
            case Operation.GetContactGroupsOperation:
                Session["contactid"] = null;
                break;
        }
    }

    #endregion
    /// <summary>
    /// This region is oauth related helper function
    /// </summary>
    #region oauth helper
    /// <summary>
    /// This function is for reading session and storing to token variables
    /// </summary>
    private bool ReadTokenSessionVariables(OAuth obj)
    {
        if (Session["cs_rest_AccessToken"] != null)
        {
            obj.accessToken = Session["cs_rest_AccessToken"].ToString();
        }
        else
        {
            obj.accessToken = null;
        }

        if (Session["cs_rest_AccessTokenExpirtyTime"] != null)
        {
            obj.accessTokenExpiryTime = Session["cs_rest_AccessTokenExpirtyTime"].ToString();
        }
        else
        {
            obj.accessTokenExpiryTime = null;
        }
        if (Session["cs_rest_RefreshToken"] != null)
        {
            obj.refreshToken = Session["cs_rest_RefreshToken"].ToString();
        }
        else
        {
            obj.refreshToken = null;
        }
        if (Session["cs_rest_RefreshTokenExpiryTime"] != null)
        {
            obj.refreshTokenExpiryTime = Session["cs_rest_RefreshTokenExpiryTime"].ToString();
        }
        else
        {
            obj.refreshTokenExpiryTime = null;
        }

        if ((obj.accessToken == null) || (obj.accessTokenExpiryTime == null) || (obj.refreshToken == null) || (obj.refreshTokenExpiryTime == null))
        {
            return false;
        }

        return true;
    }
    /// <summary>
    /// This function is for setting seesions back to null
    /// </summary>
    private void ResetTokenSessionVariables()
    {
        Session["cs_rest_AccessToken"] = null;
        Session["cs_rest_AccessTokenExpirtyTime"] = null;
        Session["cs_rest_RefreshToken"] = null;
        Session["cs_rest_RefreshTokenExpiryTime"] = null;
    }
    /// <summary>
    /// This function deserialize json string and store session variables
    /// </summary>
    private bool StoreAccessTokenToSession(string access_token_json)
    {
        DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();
        try
        {
            var deserializedJsonObj = serializer.Deserialize<OAuthToken.AccessToken>(access_token_json);
            if (deserializedJsonObj.access_token != null)
            {
                this.accessToken = deserializedJsonObj.access_token;
                this.refreshToken = deserializedJsonObj.refresh_token;

                DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                Session["cs_rest_AccessTokenExpirtyTime"] = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in));

                if (deserializedJsonObj.expires_in.Equals("0"))
                {
                    int defaultAccessTokenExpiresIn = 100; // In Years
                    Session["cs_rest_AccessTokenExpirtyTime"] = currentServerTime.AddYears(defaultAccessTokenExpiresIn);
                }

                this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();

                Session["cs_rest_AccessToken"] = this.accessToken;

                var accessTokenExpiryTime = Session["cs_rest_AccessTokenExpirtyTime"].ToString();
                Session["cs_rest_RefreshToken"] = this.refreshToken;
                Session["cs_rest_RefreshTokenExpiryTime"] = this.refreshTokenExpiryTime.ToString();
                Session["cs_rest_appState"] = "TokenReceived";
                return true;
            }
            else
            {
                this.oauth_error = "Auth server returned null access token";
                return false;
            }
        }
        catch (Exception ex)
        {
            this.oauth_error = ex.Message;
            return false;
        }
    }
    #endregion
    //pre&post operation
    /// <summary>
    /// This function is called before making an addressbook API call. It checks if token is valid also sets the request session variable.
    /// </summary>
    private void checkAccessToken(Operation operation)
    {
        ReadTokenSessionVariables(oauth);
        string tokentResult = oauth.IsTokenValid();
        if (tokentResult == "INVALID_ACCESS_TOKEN")
        {
            Session["cs_rest_ServiceRequest"] = operation;
            Session["cs_rest_appState"] = "GetToken";
            SetRequestSessionVariables(operation);
            if (!oauth.GetAuthCode())
                this.oauth_error = oauth.getAuthCodeError;
        }
        else if (tokentResult == "REFRESH_TOKEN")
        {
            if (!oauth.GetAccessToken(OAuth.AccessTokenType.RefreshToken))
            {
                this.oauth_error = oauth.getAuthCodeError; ;
                this.ResetTokenSessionVariables();
                return;
            }
            else
            {
                StoreAccessTokenToSession(oauth.accessTokenJson);
            }
        }
        if (oauth.accessToken == null || oauth.accessToken.Length <= 0)
        {
            return;
        }
    }

    /// <summary>
    /// This function sets the variables from config file
    /// </summary>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.config_error = "endPoint is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.config_error = "api_key is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.config_error = "secret_key is not defined in configuration file";
            return false;
        }

        this.authorizeRedirectUri = ConfigurationManager.AppSettings["authorize_redirect_uri"];
        if (string.IsNullOrEmpty(this.authorizeRedirectUri))
        {
            this.config_error = "authorize_redirect_uri is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "AAB";
        }
        string refreshTokenExpires = ConfigurationManager.AppSettings["refreshTokenExpiresIn"];
        if (!string.IsNullOrEmpty(refreshTokenExpires))
        {
            this.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires);
        }
        else
        {
            this.refreshTokenExpiresIn = 24;
        }
        this.bypassSSL = ConfigurationManager.AppSettings["IgnoreSSL"];
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AttachmentFilesDir"]))
        {
            this.AttachmentFilesDir = Request.MapPath(ConfigurationManager.AppSettings["AttachmentFilesDir"]);
        }
        if (!IsPostBack)
        {

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AttachmentFilesDir"]))
            {
                string[] filePaths = Directory.GetFiles(this.AttachmentFilesDir);
                foreach (string filePath in filePaths)
                {
                    attachPhoto.Items.Add(Path.GetFileName(filePath));
                    attachPhotoUpd.Items.Add(Path.GetFileName(filePath));
                    attachPhotoMyInf.Items.Add(Path.GetFileName(filePath));
                }
            }
        }
        return true;
    }

    /// <summary>
    /// This region is collection of function that reads user input and create a json string
    /// </summary>
    #region json helper
    private string[] getInputValue(string str)
    {
        string val = Request[str] != null ? Request[str].ToString() : string.Empty;
        return val.Split(',');
    }

    private string[] getInputType(string str)
    {
        string val = Request[str] != null ? Request[str].ToString() : string.Empty;
        string[] type = val.Split(',');
        for (int i = 0; i < type.Length; i++)
        {
            type[i] = type[i].Replace('-', ',');
        }
        return type;
    }

    private string[] getValueFromRequest(string str)
    {
        string [] type = new String[3];
        for (int i = 0; i < 3; i++)
        {
            str += Convert.ToString(i);
            type[i] = Request[str] != null ? Request[str].ToString() : string.Empty;
        }
        return type;
    }

    private string getContactJSON(Operation operation)
    {
        int maxNumberOfEntries = 3;
        string[] pPref = new String[maxNumberOfEntries];
        string[] ptype = new String[maxNumberOfEntries];
        string[] pnumber = new String[maxNumberOfEntries];

        if (operation == Operation.CreateContactOperation)
        {
            pPref = getValueFromRequest("createPhonePref");
            ptype = getValueFromRequest("createPhoneType");
            pnumber = getValueFromRequest("createPhoneNumber");
        }
        else if (operation == Operation.UpdateContactOperation)
        {
            pPref = getValueFromRequest("updatePhonePref");
            ptype = getValueFromRequest("updatePhoneType");
            pnumber = getValueFromRequest("updatePhoneNumber");
        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            pPref = getValueFromRequest("myInfoPhonePref");
            ptype = getValueFromRequest("myInfoPhoneType");
            pnumber = getValueFromRequest("myInfoPhoneNumber");
        }

        string[] phone_list = new String[ptype.Length];
        if (ptype.Length != pnumber.Length)
        {
            throw new Exception("Number of fields does not match the number of values");
        }
        
        var phone = new List<ContactObj.Phone>();
        for (int i = 0; i < ptype.Length; i++)
        {
            bool pref = (pPref[i].Equals("True")) ? true : false;
            phone.Add(new ContactObj.Phone() { type = ptype[i], number = pnumber[i], preferred = pref });
        }
        var phones = new ContactObj.Phones();
        phones.phone = phone;

        var empty_phone = new List<ContactObj.Phone>();
        empty_phone.Add(new ContactObj.Phone() { type = string.Empty, number = string.Empty, preferred = true });
        var empty_phones = new ContactObj.Phones();
        empty_phones.phone = empty_phone;

        string[] im_type = new String[maxNumberOfEntries];
        string[] im_im = new String[maxNumberOfEntries];
        string[] im_pref = new String[maxNumberOfEntries];

        if (operation == Operation.CreateContactOperation)
        {
            im_pref = getValueFromRequest("createIMPref");
            im_type = getValueFromRequest("createIMType");
            im_im = getValueFromRequest("createIMUri");
        }
        else if (operation == Operation.UpdateContactOperation)
        {
            im_pref = getValueFromRequest("updateIMPref");
            im_type = getValueFromRequest("updateIMType");
            im_im = getValueFromRequest("updateIMUri");
        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            im_pref = getValueFromRequest("myInfoIMPref");
            im_type = getValueFromRequest("myInfoIMType");
            im_im = getValueFromRequest("myInfoIMUri");
        }
        
        string[] im_list = new String[im_type.Length];
        if (im_type.Length != im_im.Length)
        {
            throw new Exception("Number of fields does not match the number of values");
        }
        
        var im = new List<ContactObj.Im>();
        for (int i = 0; i < im_type.Length; i++)
        {
            bool pref = (im_pref[i].Equals("True")) ? true : false;
            im.Add(new ContactObj.Im() { type = im_type[i], imUri = im_im[i], preferred = pref });
        };
        var ims = new ContactObj.Ims();
        ims.im = im;

        var empty_im = new List<ContactObj.Im>();
        empty_im.Add(new ContactObj.Im() { type = string.Empty, imUri = string.Empty, preferred = true });
        var empty_ims = new ContactObj.Ims();
        empty_ims.im = empty_im;

        string[] email_pref = new String[maxNumberOfEntries];
        string[] email_type = new String[maxNumberOfEntries];
        string[] email_email = new String[maxNumberOfEntries];

        if (operation == Operation.CreateContactOperation)
        {
            email_pref = getValueFromRequest("createEmailPref");
            email_type = getValueFromRequest("createEmailType");
            email_email = getValueFromRequest("createEmailAddress");
        }
        else if (operation == Operation.UpdateContactOperation)
        {
            email_pref = getValueFromRequest("updateEmailPref");
            email_type = getValueFromRequest("updateEmailType");
            email_email = getValueFromRequest("updateEmailAddress");
        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            email_pref = getValueFromRequest("myInfoEmailPref");
            email_type = getValueFromRequest("myInfoEmailType");
            email_email = getValueFromRequest("myInfoEmailAddress");
        }
        
        string[] email_list = new String[email_type.Length];
        if (email_type.Length != email_email.Length)
        {
            throw new Exception("Number of fields does not match the number of values");
        }
        
        var email = new List<ContactObj.Email>();
        for (int i = 0; i < email_type.Length; i++)
        {
            bool pref = (email_pref[i].Equals("True")) ? true : false;
            email.Add(new ContactObj.Email() { type = email_type[i], emailAddress = email_email[i], preferred = pref });
        }
        var emails = new ContactObj.Emails();
        emails.email = email;

        var empty_email = new List<ContactObj.Email>();
        empty_email.Add(new ContactObj.Email() { type = string.Empty, emailAddress = string.Empty, preferred = true });
        var empty_emails = new ContactObj.Emails();
        empty_emails.email = empty_email;

        string[] weburl_type = new String[maxNumberOfEntries];
        string[] weburl_weburl = new String[maxNumberOfEntries];
        string[] weburl_pref = new String[maxNumberOfEntries];
        
        if (operation == Operation.CreateContactOperation)
        {
            weburl_pref = getValueFromRequest("createWeburlPref");
            weburl_type = getValueFromRequest("createWeburlType");
            weburl_weburl = getValueFromRequest("createWeburl");
        }
        else if (operation == Operation.UpdateContactOperation)
        {
            weburl_pref = getValueFromRequest("updateWeburlPref");
            weburl_type = getValueFromRequest("updateWeburlType");
            weburl_weburl = getValueFromRequest("updateWeburl");
        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            weburl_pref = getValueFromRequest("myInfoWeburlPref");
            weburl_type = getValueFromRequest("myInfoWeburlType");
            weburl_weburl = getValueFromRequest("myInfoWeburl");
        }

        
        string[] weburl_list = new String[weburl_type.Length];
        if (weburl_type.Length != weburl_weburl.Length)
        {
            throw new Exception("Number of fields does not match the number of values");
        }
        
        var weburl = new List<ContactObj.WebUrl>();
        for (int i = 0; i < weburl_type.Length; i++)
        {

            bool pref = (weburl_pref[i].Equals("True")) ? true : false;
            weburl.Add(new ContactObj.WebUrl() { type = weburl_type[i], url = weburl_weburl[i], preferred = pref });
        }
        var weburls = new ContactObj.Weburls();
        weburls.webUrl = weburl;

        var empty_weburl = new List<ContactObj.WebUrl>();
        empty_weburl.Add(new ContactObj.WebUrl() { type = string.Empty, url = string.Empty, preferred = true });
        var empty_weburls = new ContactObj.Weburls();
        empty_weburls.webUrl = empty_weburl;

        //address
        string[] address_pref = new String[maxNumberOfEntries];
        string[] atype = new String[maxNumberOfEntries];
        string[] apoBox = new String[maxNumberOfEntries];
        string[] aaddressLine1 = new String[maxNumberOfEntries];
        string[] aaddressLine2 = new String[maxNumberOfEntries];
        string[] acity = new String[maxNumberOfEntries];
        string[] astate = new String[maxNumberOfEntries];
        string[] azip = new String[maxNumberOfEntries];
        string[] acountry = new String[maxNumberOfEntries];


        if (operation == Operation.CreateContactOperation)
        {
            address_pref = getValueFromRequest("createAddressPref");
            atype = getValueFromRequest("createAddressType");
            apoBox = getValueFromRequest("createAddressPoBox");
            aaddressLine1 = getValueFromRequest("createAddressLineOne1");
            aaddressLine2 = getValueFromRequest("createAddressLineTwo");
            acity = getValueFromRequest("createAddressCity");
            astate = getValueFromRequest("createAddressState");
            azip = getValueFromRequest("createAddressZip");
            acountry = getValueFromRequest("createAddressCountry");

        }
        else if (operation == Operation.UpdateContactOperation)
        {
            address_pref = getValueFromRequest("updateAddressPref");
            atype = getValueFromRequest("updateAddressType");
            apoBox = getValueFromRequest("updateAddressPoBox");
            aaddressLine1 = getValueFromRequest("updateAddressLineOne1");
            aaddressLine2 = getValueFromRequest("updateAddressLineTwo");
            acity = getValueFromRequest("updateAddressCity");
            astate = getValueFromRequest("updateAddressState");
            azip = getValueFromRequest("updateAddressZip");
            acountry = getValueFromRequest("updateAddressCountry");

        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            address_pref = getValueFromRequest("myInfoAddressPref");
            atype = getValueFromRequest("myInfoAddressType");
            apoBox = getValueFromRequest("myInfoAddressPoBox");
            aaddressLine1 = getValueFromRequest("myInfoAddressLineOne1");
            aaddressLine2 = getValueFromRequest("myInfoAddressLineTwo");
            acity = getValueFromRequest("myInfoAddressCity");
            astate = getValueFromRequest("myInfoAddressState");
            azip = getValueFromRequest("myInfoAddressZip");
            acountry = getValueFromRequest("myInfoAddressCountry");
        }

        int entryCount = atype.Length;
        string[] address_list = new String[entryCount];
        
        var address = new List<ContactObj.Address>();
        for (int i = 0; i < entryCount; i++)
        {
            bool pref = (address_pref[i].Equals("True")) ? true : false; 
            
            address.Add(new ContactObj.Address()
            {
                type = atype[i],
                poBox = apoBox[i],
                preferred = pref,
                addressLine1 = aaddressLine1[i],
                addressLine2 = aaddressLine2[i],
                city = acity[i],
                state = astate[i],
                zipcode = azip[i],
                country = acountry[i]
            });
        }
        var addresses = new ContactObj.Addresses();
        addresses.address = address;

        var empty_address = new List<ContactObj.Address>();
        empty_address.Add(new ContactObj.Address()
        {
            type = string.Empty,
            poBox = string.Empty,
            preferred = true,
            addressLine1 = string.Empty,
            addressLine2 = string.Empty,
            city = string.Empty,
            state = string.Empty,
            zipcode = string.Empty,
            country = string.Empty
        });
        var empty_addresses = new ContactObj.Addresses();
        empty_addresses.address = empty_address;

        //var contact = new List<ContactJSON.Contact>();
        var contact_obj = new ContactObj.Contact();
        var photo = new ContactObj.Photo();
        photo.encoding = "BASE64";

        var empty_photo = new ContactObj.Photo();
        empty_photo.encoding = "BASE64";
        empty_photo.value = "";
        if (JsonConvert.SerializeObject(phones) != JsonConvert.SerializeObject(empty_phones))
        {
            contact_obj.phones = phones;
        }
        if (JsonConvert.SerializeObject(ims) != JsonConvert.SerializeObject(empty_ims))
        {
            contact_obj.ims = ims;
        }
        if (JsonConvert.SerializeObject(addresses) != JsonConvert.SerializeObject(empty_addresses))
        {
            contact_obj.addresses = addresses;
        }
        if (JsonConvert.SerializeObject(emails) != JsonConvert.SerializeObject(empty_emails))
        {
            contact_obj.emails = emails;
        }
        if (JsonConvert.SerializeObject(weburls) != JsonConvert.SerializeObject(empty_weburls))
        {
            contact_obj.weburls = weburls;
        }
        if (JsonConvert.SerializeObject(photo) != JsonConvert.SerializeObject(empty_photo))
        {
            contact_obj.photo = photo;
        }
        var contactRoot = new ContactObj.RootObject();
        if (operation == Operation.CreateContactOperation)
        {
            string imgString = getImageto64base(attachPhoto.Value);
            photo.value = imgString;

            if (firstName.Text != "")
            {
                contact_obj.firstName = firstName.Text;
            }
            if (lastName.Text != "")
            {
                contact_obj.lastName = lastName.Text;
            }
            if (prefix.Text != "")
            {
                contact_obj.prefix = prefix.Text;
            }
            if (suffix.Text != "")
            {
                contact_obj.suffix = suffix.Text;
            }
            if (nickname.Text != "")
            {
                contact_obj.nickName = nickname.Text;
            }
            if (organization.Text != "")
            {
                contact_obj.organization = organization.Text;
            }
            if (jobTitle.Text != "")
            {
                contact_obj.jobTitle = jobTitle.Text;
            }
            if (anniversary.Text != "")
            {
                contact_obj.anniversary = anniversary.Text;
            }
            if (gender.Text != "")
            {
                contact_obj.gender = gender.Text;
            }
            if (hobby.Text != "")
            {
                contact_obj.hobby = hobby.Text;
            }
            if (assistant.Text != "")
            {
                contact_obj.assistant = assistant.Text;
            }
            contactRoot.contact = contact_obj;
        }
        else if (operation == Operation.UpdateContactOperation)
        {
            string imgString = getImageto64base(attachPhotoUpd.Value);
            photo.value = imgString;
            //if (formattedNameUpd.Text != "")
            //{
                contact_obj.formattedName = "";
            //}
            if (firstNameUpd.Text != "")
            {
                contact_obj.firstName = firstNameUpd.Text;
            }
            if (lastNameUpd.Text != "")
            {
                contact_obj.lastName = lastNameUpd.Text;
            }
            if (prefixUpd.Text != "")
            {
                contact_obj.prefix = prefixUpd.Text;
            }
            if (suffixUpd.Text != "")
            {
                contact_obj.suffix = suffixUpd.Text;
            }
            if (nicknameUpd.Text != "")
            {
                contact_obj.nickName = nicknameUpd.Text;
            }
            if (organizationUpd.Text != "")
            {
                contact_obj.organization = organizationUpd.Text;
            }
            if (jobTitleUpd.Text != "")
            {
                contact_obj.jobTitle = jobTitleUpd.Text;
            }
            if (anniversaryUpd.Text != "")
            {
                contact_obj.anniversary = anniversaryUpd.Text;
            }
            if (genderUpd.Text != "")
            {
                contact_obj.gender = genderUpd.Text;
            }
            if (hobbyUpd.Text != "")
            {
                contact_obj.hobby = hobbyUpd.Text;
            }
            if (assistantUpd.Text != "")
            {
                contact_obj.assistant = assistantUpd.Text;
            }
            contactRoot.contact = contact_obj;
        }
        else if (operation == Operation.UpdateMyInfoOperation)
        {
            string imgString = getImageto64base(attachPhotoMyInf.Value);

            photo.value = imgString;

            if (firstNameMyInf.Text != "")
            {
                contact_obj.firstName = firstNameMyInf.Text;
            }
            if (lastNameMyInf.Text != "")
            {
                contact_obj.lastName = lastNameMyInf.Text;
            }
            if (prefixMyInf.Text != "")
            {
                contact_obj.prefix = prefixMyInf.Text;
            }
            if (suffixMyInf.Text != "")
            {
                contact_obj.suffix = suffixMyInf.Text;
            }
            if (nicknameMyInf.Text != "")
            {
                contact_obj.nickName = nicknameMyInf.Text;
            }
            if (organizationMyInf.Text != "")
            {
                contact_obj.organization = organizationMyInf.Text;
            }
            if (jobTitleMyInf.Text != "")
            {
                contact_obj.jobTitle = jobTitleMyInf.Text;
            }
            if (anniversaryMyInf.Text != "")
            {
                contact_obj.anniversary = anniversaryMyInf.Text;
            }
            if (genderMyInf.Text != "")
            {
                contact_obj.gender = genderMyInf.Text;
            }
            if (hobbyMyInf.Text != "")
            {
                contact_obj.hobby = hobbyMyInf.Text;
            }
            if (assistantMyInf.Text != "")
            {
                contact_obj.assistant = assistantMyInf.Text;
            }
            contactRoot.myInfo = contact_obj;
        }
        return JsonConvert.SerializeObject(contactRoot, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }

    private string getGroupJSON(Operation operation)
    {
        var group = new GroupObj.Group();
        if (operation == Operation.CreateGroupOperation)
        {
            group.groupName = groupName.Text;
            group.groupType = "USER";
        }
        else if (operation == Operation.UpdateGroupOperation)
        {
            group.groupName = groupNameUpd.Text;
            group.groupType = "USER";
        }

        var serializer = new JavaScriptSerializer();
        var groupRoot = new GroupObj.RootObject();
        groupRoot.group = group;
        return JsonConvert.SerializeObject(groupRoot, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }
    #endregion

    /// <summary>
    /// This funtion coverts image to base 64 string
    /// </summary>
    private string getImageto64base(string imageName)
    {
        string path = this.AttachmentFilesDir + "/" + imageName;
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(imageBytes);
        }
        else
        {
            return "";
        }

    }

    public class Success
    {
        public string location { get; set; }
        public string last_modified { get; set; }
    }

    public enum Operation
    {
        CreateContactOperation,
        UpdateContactOperation,
        DeleteContactOperation,
        GetContactsOperation,
        GetMyInfoOperation,
        UpdateMyInfoOperation,
        CreateGroupOperation,
        UpdateGroupOperation,
        DeleteGroupOperation,
        GetGroupsOperation,
        GetGroupContactsOperation,
        AddContctsToGroupOperation,
        RemoveContactsFromGroupOperation,
        GetContactGroupsOperation
    }

}