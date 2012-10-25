// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

#endregion

/// <summary>
/// CallControl App1 class
/// </summary>
public partial class CallControl_App1 : System.Web.UI.Page
{
    #region Local variables

    /// <summary>
    /// Access Token Variables
    /// </summary>
    private string endPoint, accessTokenFilePath, apiKey, secretKey, accessToken, accessTokenExpiryTime,
    scope, refreshToken, refreshTokenExpiryTime;

    /// <summary>
    /// Phone numbers registered for Call Control Service.
    /// </summary>
    private string phoneNumbers;

    /// <summary>
    /// Script for Call Control Service.
    /// </summary>
    private string scriptName;

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;

    /// <summary>
    /// Access Token Types
    /// </summary>
    private enum AccessType
    {
        /// <summary>
        /// Access Token Type is based on Client Credential Mode
        /// </summary>
        ClientCredential,

        /// <summary>
        /// Access Token Type is based on Refresh Token
        /// </summary>
        RefreshToken
    }
#endregion

    #region SSL Handshake Error
    
    /// <summary>
    /// Neglect the ssl handshake error with authentication server
    /// </summary>
    public static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    #endregion

    #region Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// This function reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();

            DateTime currentServerTime = DateTime.UtcNow;
            serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
            WriteNote();
            bool ableToRead = this.ReadConfigFile();

            if (!ableToRead)
            {
                return;
            }

            lblPhoneNumbers.Text = this.phoneNumbers;            
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.ToString());
        }
    }

    /// <summary>
    /// Event that will be triggered when the user clicks on Send Signal button.
    /// This method will invoke SendSignal API.
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void btnSendSignal_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(lblSessionId.Text))
            {
                this.DrawPanelForFailure(pnlSendSignal, "Please create a session and then send signal");
                return;
            }

            bool ableToGetAccessToken = this.ReadAndGetAccessToken(pnlSendSignal);
            if (ableToGetAccessToken)
            {
                this.SendSignal();
            }
            else
            {
                this.DrawPanelForFailure(pnlSendSignal, "Unable to get access token");
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlSendSignal, ex.Message);
        }
    }   

    /// <summary>
    /// Event that will be triggered when the user clicks on Create Session button.
    /// This method will invoke CreateSession API.
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void btnCreateSession_Click(object sender, EventArgs e)
    {
        try
        {
            bool ableToGetAccessToken = this.ReadAndGetAccessToken(pnlCreateSession);
            if (ableToGetAccessToken)
            {
                this.CreateSession();
            }
            else
            {
                this.DrawPanelForFailure(pnlCreateSession, "Unable to get access token");
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.Message);
        }
    }

    #endregion

    #region API Invokation Methods

    /// <summary>
    /// This method creates a Session for an outgoing call or message.
    /// </summary>
    private void CreateSession()
    {
        try
        {
            CreateSessionClass createSessionData = new CreateSessionClass();
            createSessionData.numberToDial = txtNumberToDial.Text.ToString();
            if (lstTemplate.SelectedValue != "")
                createSessionData.feature = lstTemplate.SelectedValue.ToString();
            else
                createSessionData.feature = string.Empty;
            createSessionData.messageToPlay = txtMessageToPlay.Text.ToString();
            createSessionData.featurenumber = txtNumberForFeature.Text.ToString();
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = 
                        new System.Web.Script.Serialization.JavaScriptSerializer();

            string requestParams = oSerializer.Serialize(createSessionData);
            string createSessionResponse;
            HttpWebRequest createSessionRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/1/Sessions");
            createSessionRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);

            createSessionRequestObject.Method = "POST";
            createSessionRequestObject.ContentType = "application/json";
            createSessionRequestObject.Accept = "application/json";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(requestParams);
            createSessionRequestObject.ContentLength = postBytes.Length;

            Stream postStream = createSessionRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            HttpWebResponse createSessionResponseObject = (HttpWebResponse)createSessionRequestObject.GetResponse();
            using (StreamReader createSessionResponseStream = new StreamReader(createSessionResponseObject.GetResponseStream()))
            {
                createSessionResponse = createSessionResponseStream.ReadToEnd();
                if (!string.IsNullOrEmpty(createSessionResponse))
                {
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    CreateSessionResponse deserializedJsonObj = (CreateSessionResponse)deserializeJsonObject.Deserialize(createSessionResponse, typeof(CreateSessionResponse));
                    if (null != deserializedJsonObj)
                    {
                        lblSessionId.Text = deserializedJsonObj.id.ToString();
                        NameValueCollection displayParam = new NameValueCollection();
                        displayParam.Add("id", deserializedJsonObj.id);
                        displayParam.Add("success", deserializedJsonObj.success.ToString());
                        this.DrawPanelForSuccess(pnlCreateSession, displayParam, string.Empty);
                    }
                    else
                    {
                        this.DrawPanelForFailure(pnlCreateSession, "Got response but not able to deserialize json" + createSessionResponse);
                    }
                }
                else
                {
                    this.DrawPanelForFailure(pnlCreateSession, "Success response but with empty ad");
                }

                createSessionResponseStream.Close();
            }
        }
        catch (WebException we)
        {
            string errorResponse = string.Empty;

            try
            {
                using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                {
                    errorResponse = sr2.ReadToEnd();
                    sr2.Close();
                }
            }
            catch
            {
                errorResponse = "Unable to get response";
            }

            this.DrawPanelForFailure(pnlCreateSession, errorResponse + Environment.NewLine + we.ToString());
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.ToString());
        }
    }

    /// <summary>
    /// This method sends a Signal to an active Session.
    /// </summary>
    private void SendSignal()
    {
        try
        {
            string sendSignalResponse;
            HttpWebRequest sendSignalRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/1/Sessions/" + lblSessionId.Text + "/Signals");
            string strReq = "{\"signal\":\"" + ddlSignal.SelectedValue.ToString() + "\"}";
            sendSignalRequestObject.Method = "POST";
            sendSignalRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);
            sendSignalRequestObject.ContentType = "application/json";
            sendSignalRequestObject.Accept = "application/json";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(strReq);
            sendSignalRequestObject.ContentLength = postBytes.Length;

            Stream postStream = sendSignalRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            HttpWebResponse sendSignalResponseObject = (HttpWebResponse)sendSignalRequestObject.GetResponse();
            using (StreamReader sendSignalResponseStream = new StreamReader(sendSignalResponseObject.GetResponseStream()))
            {
                sendSignalResponse = sendSignalResponseStream.ReadToEnd();
                if (!string.IsNullOrEmpty(sendSignalResponse))
                {
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    SendSignalResponse deserializedJsonObj = (SendSignalResponse)deserializeJsonObject.Deserialize(sendSignalResponse, typeof(SendSignalResponse));
                    if (null != deserializedJsonObj)
                    {
                        NameValueCollection displayParam = new NameValueCollection();
                        displayParam.Add("status", deserializedJsonObj.status);
                        this.DrawPanelForSuccess(pnlSendSignal, displayParam, string.Empty);
                    }
                    else
                    {
                        this.DrawPanelForFailure(pnlSendSignal, "Got response but not able to deserialize json " + sendSignalResponse);
                    }
                }
                else
                {
                    this.DrawPanelForFailure(pnlSendSignal, "No response from the gateway.");
                }

                sendSignalResponseStream.Close();
            }
        }
        catch (WebException we)
        {
            string errorResponse = string.Empty;

            try
            {
                using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                {
                    errorResponse = sr2.ReadToEnd();
                    sr2.Close();
                }
            }
            catch
            {
                errorResponse = "Unable to get response";
            }

            this.DrawPanelForFailure(pnlSendSignal, errorResponse + Environment.NewLine + we.ToString());
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlSendSignal, ex.ToString());
        }
    }

    /// <summary>
    /// This method displays the contents of the note area.
    /// </summary>
    private void WriteNote()
    {
        string description = "<strong>Note:</strong> <br/>";
        Label1.Text = "Create Session will trigger an outbound call from application " + " to <strong>\"Make call to\"</strong> number.";
        switch (lstTemplate.SelectedValue)
        {
            case "ask":
                description += 
                    "For <strong>ask()</strong> script function, user is prompted to enter few digits and entered digits are played back. <br/>" +
                    "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                    return;
            case "conference":
                    description +=
                    "For <strong>conference()</strong> script function, user is prompted to join the conference.<br/>" +
                    "After quitting the conference, user is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                    return;
            case "reject":
                    description +=
                    "For <strong>reject()</strong> script function, if <strong>\"Number parameter for Script Function\"</strong> matches with calling id, call will be dropped.<br/>" +
                    "If calling id doesnt match, calling id and <strong>\"Number parameter for Script Function\"</strong> number are played to User.<br/>" +
                    "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                    notesLiteral.Text = description;
                    return;
            case "transfer":
                    description +=
                    "For <strong>transfer()</strong> script function, user is played back with <strong>\"Number parameter for Script Function\"</strong> and call be transferred to that number.<br/>" +
                    "While doing transfer music on hold <strong>\"Message to Play\"</strong> is played. Once <strong>\"Number parameter for Script Function\"</strong> number disconnects the call, " +
                    "user is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                    notesLiteral.Text = description;
                    return;
            case "wait":
                    description +=
                    "For <strong>wait()</strong> script function, if <strong>\"Number parameter for Script Function\"</strong> matches with calling id, call will be kept on hold for 3 seconds.<br/>" +
                    "If calling id doesnt match, calling id and <strong>\"Number parameter for Script Function\"</strong> number are played to User.<br/>" +
                    "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                    notesLiteral.Text = description;
                    return;
            case "":
                    description +=
                    "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                    notesLiteral.Text = description;
                    return;
            default:
                    return;
        }
    }

    /// <summary>
    /// This method displays the contents of call.js file onto create session textarea.
    /// </summary>
    private void GetOutboundScriptContent()
    {
        StreamReader streamReader = null;
        try
        {
            string scrFile = Request.MapPath(scriptName);
            streamReader = new StreamReader(scrFile);
            string javaScript = streamReader.ReadToEnd();
            txtCreateSession.Text = "Following is the Java Script Code: " + System.Environment.NewLine + javaScript;
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.Message);
        }
        finally
        {
            if (null != streamReader)
            {
                streamReader.Close();
            }
        }
    }

    #endregion

    #region Access Token Methods

    /// <summary>
    /// This function reads the Access Token File and stores the values of access token, expiry seconds
    /// refresh token, last access token time and refresh token expiry time
    /// This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns boolean</returns>    
    private bool ReadAccessTokenFile(Panel panelParam)
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        try
        {
            if (File.Exists(Request.MapPath(this.accessTokenFilePath)))
            {
                fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read);
                streamReader = new StreamReader(fileStream);
                this.accessToken = streamReader.ReadLine();
                this.accessTokenExpiryTime = streamReader.ReadLine();
                this.refreshToken = streamReader.ReadLine();
                this.refreshTokenExpiryTime = streamReader.ReadLine();
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(panelParam, ex.Message);
            return false;
        }
        finally
        {
            if (null != streamReader)
            {
                streamReader.Close();
            }

            if (null != fileStream)
            {
                fileStream.Close();
            }
        }

        if ((this.accessToken == null) || (this.accessTokenExpiryTime == null) || (this.refreshToken == null) || (this.refreshTokenExpiryTime == null))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Reads from session variables and gets access token.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>true/false; true on successfully getting access token, else false</returns>
    private bool ReadAndGetAccessToken(Panel panelParam)
    {
        bool result = true;
        if (this.ReadAccessTokenFile(panelParam) == false)
        {
            result = this.GetAccessToken(AccessType.ClientCredential, panelParam);
        }
        else
        {
            string tokenValidity = this.IsTokenValid();
            if (tokenValidity == "REFRESH_TOKEN")
            {
                result = this.GetAccessToken(AccessType.RefreshToken, panelParam);
            }
            else if (string.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") == 0)
            {
                result = this.GetAccessToken(AccessType.ClientCredential, panelParam);
            }
        }

        if (this.accessToken == null || this.accessToken.Length <= 0)
        {
            return false;
        }
        else
        {
            return result;
        }
    }

    /// <summary>
    /// This function validates the expiry of the access token and refresh token,
    /// function compares the current time with the refresh token taken time, if current time is greater then 
    /// returns INVALID_REFRESH_TOKEN
    /// function compares the difference of last access token taken time and the current time with the expiry seconds, if its more,
    /// funciton returns INVALID_ACCESS_TOKEN
    /// otherwise returns VALID_ACCESS_TOKEN
    /// </summary>
    /// <returns>Return String</returns>
    private string IsTokenValid()
    {
        try
        {
            DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();
            if (currentServerTime >= DateTime.Parse(this.accessTokenExpiryTime))
            {
                if (currentServerTime >= DateTime.Parse(this.refreshTokenExpiryTime))
                {
                    return "INVALID_ACCESS_TOKEN";
                }
                else
                {
                    return "REFRESH_TOKEN";
                }
            }
            else
            {
                return "VALID_ACCESS_TOKEN";
            }
        }
        catch
        {
            return "INVALID_ACCESS_TOKEN";
        }
    }

    /// <summary>
    /// Get access token based on the type parameter type values.
    /// </summary>
    /// <param name="type">If type value is Authorization_code, access token is fetch for authorization code flow
    /// If type value is Refresh_Token, access token is fetch for authorization code floww based on the exisiting refresh token</param>
    /// <param name="panelParam">Panel to display status message</param>
    /// <returns>true/false; true if success, else false</returns>  
    private bool GetAccessToken(AccessType type, Panel panelParam)
    {
        FileStream fileStream = null;
        Stream postStream = null;
        StreamWriter streamWriter = null;

        try
        {
            DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

            WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
            accessTokenRequest.Method = "POST";
            string oauthParameters = string.Empty;
            if (type == AccessType.ClientCredential)
            {
                oauthParameters = "client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&grant_type=client_credentials&scope=" + this.scope;
            }
            else
            {
                oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
            }

            accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(oauthParameters);
            accessTokenRequest.ContentLength = postBytes.Length;

            postStream = accessTokenRequest.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);

            WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
            using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
            {
                string jsonAccessToken = accessTokenResponseStream.ReadToEnd().ToString();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(jsonAccessToken, typeof(AccessTokenResponse));

                this.accessToken = deserializedJsonObj.access_token;
                this.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString();
                this.refreshToken = deserializedJsonObj.refresh_token;

                DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                if (deserializedJsonObj.expires_in.Equals("0"))
                {
                    int defaultAccessTokenExpiresIn = 100; // In Yearsint yearsToAdd = 100;
                    this.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongDateString() + " " + currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongTimeString();
                }

                this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();

                fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(this.accessToken);
                streamWriter.WriteLine(this.accessTokenExpiryTime);
                streamWriter.WriteLine(this.refreshToken);
                streamWriter.WriteLine(this.refreshTokenExpiryTime);

                // Close and clean up the StreamReader
                accessTokenResponseStream.Close();
                return true;
            }
        }
        catch (WebException we)
        {
            string errorResponse = string.Empty;

            try
            {
                using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                {
                    errorResponse = sr2.ReadToEnd();
                    sr2.Close();
                }
            }
            catch
            {
                errorResponse = "Unable to get response";
            }

            this.DrawPanelForFailure(panelParam, errorResponse + Environment.NewLine + we.ToString());
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(panelParam, ex.Message);
            return false;
        }
        finally
        {
            if (null != postStream)
            {
                postStream.Close();
            }

            if (null != streamWriter)
            {
                streamWriter.Close();
            }

            if (null != fileStream)
            {
                fileStream.Close();
            }
        }

        return false;
    }

    /// <summary>
    /// Read parameters from configuraton file and assigns to local variables.
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(pnlCreateSession, "endPoint is not defined in configuration file");
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(pnlCreateSession, "apiKey is not defined in configuration file");
            return false; 
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(pnlCreateSession, "secretKey is not defined in configuration file");
            return false;
        }

        this.phoneNumbers = ConfigurationManager.AppSettings["phoneNumbers"];
        if (string.IsNullOrEmpty(this.phoneNumbers))
        {
            this.DrawPanelForFailure(pnlCreateSession, "phoneNumbers parameter is not defined in configuration file");
            return false;
        }

        this.scriptName = ConfigurationManager.AppSettings["scriptName"];
        if (string.IsNullOrEmpty(this.scriptName))
        {
            this.DrawPanelForFailure(pnlCreateSession, "scriptName parameter is not defined in configuration file");
            return false;
        }
        GetOutboundScriptContent();
        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "CCS";
        }

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\CCSApp1AccessToken.txt";
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

        return true;
    }

    #endregion

    #region Display Methods

    /// <summary>
    /// Displays error message
    /// </summary>
    /// <param name="panelParam">Panel to draw error message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.CssClass = "errorWide";
        table.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "ERROR:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Text = message;
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        table.BorderWidth = 2;
        table.BorderColor = Color.Red;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc");
        panelParam.Controls.Add(table);
    }

    /// <summary>
    /// This function is called to draw the table in the panelParam panel for success response.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="displayParams">Collection of message parameters to display.</param>
    /// <param name="message">Message as string</param>
    private void DrawPanelForSuccess(Panel panelParam, NameValueCollection displayParams, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.CssClass = "successWide";
        table.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);

        if (null != displayParams)
        {
            TableCell rowNextCellOne = null;
            TableCell rowNextCellTwo = null;
            foreach (string key in displayParams.Keys)
            {
                TableRow rowNext = new TableRow();
                rowNextCellOne = new TableCell();
                rowNextCellOne.Text = key;
                rowNextCellOne.Font.Bold = true;
                rowNextCellOne.Width = Unit.Pixel(70);
                rowNext.Controls.Add(rowNextCellOne);

                rowNextCellTwo = new TableCell();
                rowNextCellTwo.Text = displayParams[key];
                rowNext.Controls.Add(rowNextCellTwo);
                table.Controls.Add(rowNext);
            }
        }
        else
        {
            TableRow rowTwo = new TableRow();
            TableCell rowTwoCellOne = new TableCell();            
            rowTwoCellOne.Text = message;
            rowTwo.Controls.Add(rowTwoCellOne);
            table.Controls.Add(rowTwo);
        }

        panelParam.Controls.Add(table);
    }

    #endregion
}

#region Data Structures

/// <summary>
/// Access Token Data Structure
/// </summary>
public class CreateSessionClass
{
    /// <summary>
    /// Gets or sets numberToDial
    /// </summary>
    public string numberToDial
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets messageToPlay
    /// </summary>
    public string messageToPlay
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets featureNumber
    /// </summary>
    public string featurenumber
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets feature
    /// </summary>
    public string feature
    {
        get;
        set;
    }
}

/// <summary>
/// Access Token Data Structure
/// </summary>
public class AccessTokenResponse
{
    /// <summary>
    /// Gets or sets Access Token ID
    /// </summary>
    public string access_token
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets Refresh Token ID
    /// </summary>
    public string refresh_token
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets Expires in milli seconds
    /// </summary>
    public string expires_in
    {
        get;
        set;
    }
}

/// <summary>
/// Create Session Response object
/// </summary>
public class CreateSessionResponse
{
    /// <summary>
    /// Gets or sets the value of id.
    /// The SessionID for the newly created session.
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the token launch was successful.
    /// </summary>
    public bool success { get; set; }
}

/// <summary>
/// Send Signal Response object
/// </summary>
public class SendSignalResponse
{
    /// <summary>
    /// Gets or sets the value of status.
    /// Returns the status of the request. Possible values are:
    /// QUEUED - The event delivered successfully to the event queue of the target.
    /// NOTFOUND - The target session could not be found.
    /// FAILED - Some other failure occurred.
    /// </summary>
    public string status { get; set; }
}

#endregion