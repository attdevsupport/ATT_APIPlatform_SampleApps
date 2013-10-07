// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
#endregion

/// <summary>
/// Default class
/// </summary>
public partial class SMS_App1 : System.Web.UI.Page
{
    #region Variable Declaration
   
    /// <summary>
    /// Global Variables related to application
    /// </summary>
    private string endPoint, apiKey, secretKey, scope, bypassSSL;

    ///<summary>
    ///API URL's
    ///</summary>
    private string sendSMSURL = "/sms/v3/messaging/outbox";
    private string getDeliveryStatusURL = "/sms/v3/messaging/outbox/";
    private string getSMSURL = "/sms/v3/messaging/inbox";

    /// <summary>
    /// Global Variables related to access token
    /// </summary>
    private string accessTokenFilePath, accessToken, accessTokenExpiryTime,
        refreshToken, refreshTokenExpiryTime;
    private int refreshTokenExpiresIn;

    /// <summary>
    /// Variables related to Send SMS
    /// </summary>
    public string sendSMSErrorMessage = string.Empty;
    public string sendSMSSuccessMessage = string.Empty;
    public SendSMSResponse sendSMSResponseData = null;

    /// <summary>
    /// Variables related to Get Delivery Status
    /// </summary>
    public string getSMSDeliveryStatusErrorMessage = string.Empty;
    public string getSMSDeliveryStatusSuccessMessagae = string.Empty;
    public GetDeliveryStatus getSMSDeliveryStatusResponseData = null;

    /// <summary>
    /// Variables related to Get SMS
    /// </summary>

    public string getSMSSuccessMessage = string.Empty;
    public string offlineShortCode = string.Empty;
    public string getSMSErrorMessage = string.Empty;
    public GetSmsResponse getSMSResponseData = null;

    /// <summary>
    /// Variables related to Receive SMS
    /// </summary>
    public string onlineShortCode = string.Empty;
    public string receiveSMSSuccessMessage = string.Empty;
    public string receiveSMSErrorMesssage = string.Empty;
    public List<ReceiveSMS> receivedSMSList = new List<ReceiveSMS>();
    public string receiveSMSFilePath = "\\Messages.txt";
    /// <summary>
    /// Variables related to Get Delivery Status
    /// </summary>
    public string receiveSMSDeliveryStatusErrorMessage = string.Empty;
    public string receiveSMSDeliveryStatusSuccessMessagae = string.Empty;
    public List<deliveryInfoNotification> receiveSMSDeliveryStatusResponseData = new List<deliveryInfoNotification>();
    public string receiveSMSDeliveryStatusFilePath = "\\DeliveryStatus.txt";
    
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

    #region SMS Application Events
    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// This fucntion reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">Sender Details</param>
    /// <param name="e">List of Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();
            bool ableToRead = this.ReadConfigFile();
            if (ableToRead == false)
            {
                return;
            }

            if (Session["lastSentSMSID"] != null)
            {
                messageId.Value = Session["lastSentSMSID"].ToString();
            }
            //if (!IsPostBack)
            //{
                readOnlineMessages();
                readOnlineDeliveryStatus();
            //}
        }
        catch (Exception ex)
        {
            sendSMSErrorMessage =  ex.ToString();
        }
    }

    /// <summary>
    /// Reads from config file
    /// </summary>
    /// <returns>true/false; true if able to read else false</returns>
    private bool ReadConfigFile()
    {
        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\SMSApp1AccessToken.txt";
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            sendSMSErrorMessage= "endPoint is not defined in configuration file";
            return false;
        }

        this.offlineShortCode = ConfigurationManager.AppSettings["OfflineShortCode"];
        if (string.IsNullOrEmpty(this.offlineShortCode))
        {
            sendSMSErrorMessage= "short_code is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            sendSMSErrorMessage=  "api_key is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            sendSMSErrorMessage= "secret_key is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "SMS";
        }

        string refreshTokenExpires = ConfigurationManager.AppSettings["refreshTokenExpiresIn"];
        if (!string.IsNullOrEmpty(refreshTokenExpires))
        {
            this.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires);
        }
        else
        {
            this.refreshTokenExpiresIn = 24; // Default value
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceLink"]))
        {
            SourceLink.HRef = ConfigurationManager.AppSettings["SourceLink"];
        }
        else
        {
            SourceLink.HRef = "#"; // Default value
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DownloadLink"]))
        {
            DownloadLink.HRef = ConfigurationManager.AppSettings["DownloadLink"];
        }
        else
        {
            DownloadLink.HRef = "#"; // Default value
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["HelpLink"]))
        {
            HelpLink.HRef = ConfigurationManager.AppSettings["HelpLink"];
        }
        else
        {
            HelpLink.HRef = "#"; // Default value
        }

        this.onlineShortCode = ConfigurationManager.AppSettings["OnlineShortCode"];
        if (string.IsNullOrEmpty(this.onlineShortCode))
        {
            sendSMSErrorMessage = "Online ShortCode is not defined in configuration file";
            return false;
        }

        this.sendSMSURL = ConfigurationManager.AppSettings["SendSMSURL"];
        this.getDeliveryStatusURL = ConfigurationManager.AppSettings["GetDeliveryStatusURL"];
        this.getSMSURL = ConfigurationManager.AppSettings["GetSMSURL"];

        this.receiveSMSDeliveryStatusFilePath = ConfigurationManager.AppSettings["ReceivedDeliveryStatusFilePath"];
        this.receiveSMSFilePath = ConfigurationManager.AppSettings["ReceivedMessagesFilePath"];

        if (!IsPostBack)
        {
            string sampleMessages = ConfigurationManager.AppSettings["SMSSampleMessage"];
            if (!string.IsNullOrEmpty(sampleMessages))
            {
                string[] sample = Regex.Split(sampleMessages, "_-_-");
                foreach (string sm in sample)
                {
                    message.Items.Add(sm);
                }
            }
            else
            {
                message.Items.Add("ATT SMS sample Message");
            }

        }
        return true;
    }

    /// <summary>
    /// This function is called with user clicks on send SMS
    /// This validates the access token and then calls sendSMS method to invoke send SMS API.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void BtnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.ReadAndGetAccessToken(ref sendSMSErrorMessage) == true)
            {
                this.SendSms();
            }
            else
            {
                sendSMSErrorMessage =  "Unable to get access token.";
            }
        }
        catch (Exception ex)
        {
            sendSMSErrorMessage = ex.ToString();
        }
    }

    /// <summary>
    /// This method is called when user clicks on get message button
    /// </summary>
    /// <param name="sender">Sender Details</param>
    /// <param name="e">List of Arguments</param>
    protected void GetMessagesButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.ReadAndGetAccessToken(ref getSMSErrorMessage) == true)
            {
                    this.GetSms();
            }
            else
            {
                getSMSErrorMessage = "Unable to get access token.";
            }
        }
        catch (Exception ex)
        {
            getSMSErrorMessage = ex.ToString();
        }
    }
    #endregion


    /// <summary>
    /// This function is used to neglect the ssl handshake error with authentication server
    /// </summary>
    private static void BypassCertificateError()
    {
        string bypassSSL = ConfigurationManager.AppSettings["IgnoreSSL"];

        if ((!string.IsNullOrEmpty(bypassSSL))
            && (string.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)))
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate(Object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }
    }

    /// <summary>
    /// This function calls receive sms api to fetch the sms's
    /// </summary>
    private void GetSms()
    {
        try
        {
            string receiveSmsResponseData;
            if (string.IsNullOrEmpty(this.offlineShortCode))
            {
                getSMSErrorMessage = "Short code is null or empty";
                return;
            }

            HttpWebRequest objRequest = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + this.getSMSURL +"/" + this.offlineShortCode.ToString());
            objRequest.Method = "GET";
            objRequest.Headers.Add("Authorization", "BEARER " + this.accessToken);
            HttpWebResponse receiveSmsResponseObject = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader receiveSmsResponseStream = new StreamReader(receiveSmsResponseObject.GetResponseStream()))
            {
                receiveSmsResponseData = receiveSmsResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                getSMSResponseData = new GetSmsResponse();
                getSMSResponseData = (GetSmsResponse)deserializeJsonObject.Deserialize(receiveSmsResponseData, typeof(GetSmsResponse));
                receiveSmsResponseStream.Close();
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

            getSMSErrorMessage =  errorResponse + Environment.NewLine + we.ToString();
        }
        catch (Exception ex)
        {
            getSMSErrorMessage = ex.ToString();
        }
    }

    /// <summary>
    /// Method will be called when the user clicks on Update Votes Total button
    /// </summary>
    /// <param name="sender">object, that invoked this method</param>
    /// <param name="e">EventArgs, specific to this method</param>
    protected void receiveStatusBtn_Click(object sender, EventArgs e)
    {
        try
        {
            //this.readOnlineDeliveryStatus();
        }
        catch (Exception ex)
        {
            receiveSMSErrorMesssage = ex.ToString();
        }
    }

            /// <summary>
    /// Method will be called when the user clicks on Update Votes Total button
    /// </summary>
    /// <param name="sender">object, that invoked this method</param>
    /// <param name="e">EventArgs, specific to this method</param>
    protected void receiveMessagesBtn_Click(object sender, EventArgs e)
    {
        try
        {
            //this.readOnlineMessages();
        }
        catch (Exception ex)
        {
            receiveSMSErrorMesssage = ex.ToString();
        }
    }



    /// <summary>
    /// This method reads the messages file and draw the table.
    /// </summary>
    private void readOnlineDeliveryStatus()
    {
        string receivedMessagesFile = ConfigurationManager.AppSettings["ReceivedDeliveryStatusFilePath"];
        if (!string.IsNullOrEmpty(receivedMessagesFile))
            receivedMessagesFile = Request.MapPath(receivedMessagesFile);
        else
            receivedMessagesFile = Request.MapPath("~\\DeliveryStatus.txt");
        string messagesLine = String.Empty;
        if (File.Exists(receivedMessagesFile))
        {
            using (StreamReader sr = new StreamReader(receivedMessagesFile))
            {
                while (sr.Peek() >= 0)
                {
                    deliveryInfoNotification dNot = new deliveryInfoNotification();
                    dNot.deliveryInfo = new ReceiveDeliveryInfo();
                    messagesLine = sr.ReadLine();
                    string[] messageValues = Regex.Split(messagesLine, "_-_-");
                    dNot.messageId = messageValues[0];
                    dNot.deliveryInfo.address = messageValues[1];
                    dNot.deliveryInfo.deliveryStatus  = messageValues[2];
                    receiveSMSDeliveryStatusResponseData.Add(dNot);
                }
                sr.Close();
                receiveSMSDeliveryStatusResponseData.Reverse();
            }

        }
    }

    /// <summary>
    /// This method reads the messages file and draw the table.
    /// </summary>
    private void readOnlineMessages()
    {
        string receivedMessagesFile = ConfigurationManager.AppSettings["ReceivedMessagesFilePath"];
        if (!string.IsNullOrEmpty(receivedMessagesFile))
            receivedMessagesFile = Request.MapPath(receivedMessagesFile);
        else
            receivedMessagesFile = Request.MapPath("~\\Messages.txt");
        string messagesLine = String.Empty;
        if (File.Exists(receivedMessagesFile))
        {
            using (StreamReader sr = new StreamReader(receivedMessagesFile))
            {
                while (sr.Peek() >= 0)
                {
                    ReceiveSMS inboundMsg = new ReceiveSMS();
                    messagesLine = sr.ReadLine();
                    string[] messageValues = Regex.Split(messagesLine, "_-_-");
                    inboundMsg.DateTime = messageValues[0];
                    inboundMsg.MessageId = messageValues[1];
                    inboundMsg.Message = messageValues[2];
                    inboundMsg.SenderAddress = messageValues[3];
                    inboundMsg.DestinationAddress = messageValues[4];
                    receivedSMSList.Add(inboundMsg);
                }
                sr.Close();
                receivedSMSList.Reverse();
            }

        }
    }

    /// <summary>
    /// This function reads the Access Token File and stores the values of access token, expiry seconds
    /// refresh token, last access token time and refresh token expiry time
    /// This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns boolean</returns>    
    private bool ReadAccessTokenFile(ref string  message)
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        try
        {
            fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read);
            streamReader = new StreamReader(fileStream);
            this.accessToken = streamReader.ReadLine();
            this.accessTokenExpiryTime = streamReader.ReadLine();
            this.refreshToken = streamReader.ReadLine();
            this.refreshTokenExpiryTime = streamReader.ReadLine();
        }
        catch (Exception ex)
        {
            message= ex.Message;
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
    /// This function is used to read access token file and validate the access token
    /// this function returns true if access token is valid, or else false is returned
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns Boolean</returns>
    private bool ReadAndGetAccessToken(ref string responseString)
    {
        bool result = true;
        if (this.ReadAccessTokenFile(ref responseString) == false)
        {
            result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
        }
        else
        {
            string tokenValidity = this.IsTokenValid();
            if (tokenValidity == "REFRESH_TOKEN")
            {
                result = this.GetAccessToken(AccessType.RefreshToken, ref responseString);
            }
            else if (string.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") == 0)
            {
                result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
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
    /// This function get the access token based on the type parameter type values.
    /// If type value is 1, access token is fetch for client credential flow
    /// If type value is 2, access token is fetch for client credential flow based on the exisiting refresh token
    /// </summary>
    /// <param name="type">Type as integer</param>
    /// <param name="panelParam">Panel details</param>
    /// <returns>Return boolean</returns>
    private bool GetAccessToken(AccessType type, ref string message)
    {
        FileStream fileStream = null;
        Stream postStream = null;
        StreamWriter streamWriter = null;

        // This is client credential flow
        if (type == AccessType.ClientCredential)
        {
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

                message = errorResponse + Environment.NewLine + we.ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
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
        }
        else if (type == AccessType.RefreshToken)
        {
            try
            {
                DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
                accessTokenRequest.Method = "POST";

                string oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
                accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(oauthParameters);
                accessTokenRequest.ContentLength = postBytes.Length;

                postStream = accessTokenRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);

                WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
                using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
                {
                    string accessTokenJSon = accessTokenResponseStream.ReadToEnd().ToString();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                    AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(accessTokenJSon, typeof(AccessTokenResponse));
                    this.accessToken = deserializedJsonObj.access_token.ToString();
                    DateTime accessTokenExpiryTime = currentServerTime.AddMilliseconds(Convert.ToDouble(deserializedJsonObj.expires_in.ToString()));
                    this.refreshToken = deserializedJsonObj.refresh_token.ToString();

                    fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.WriteLine(this.accessToken);
                    streamWriter.WriteLine(this.accessTokenExpiryTime);
                    streamWriter.WriteLine(this.refreshToken);

                    // Refresh token valids for 24 hours
                    DateTime refreshExpiry = currentServerTime.AddHours(24);
                    this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();
                    streamWriter.WriteLine(refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString());

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

                message = errorResponse + Environment.NewLine + we.ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
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
        }

        return false;
    }

    /// <summary>
    /// This function validates the input fields and if they are valid send sms api is invoked
    /// </summary>
    private void SendSms()
    {
        try
        {
            string outBoundSmsJson = string.Empty; 
            List<string> destinationNumbers = new List<string>();
            if (!string.IsNullOrEmpty(address.Value))
            {
                string addressInput = address.Value;
                string[] multipleAddresses = addressInput.Split(',');
                foreach (string addr in multipleAddresses)
                {
                    if (addr.StartsWith("tel:"))
                    {
                        destinationNumbers.Add(addr);
                    }
                    else
                    {
                        string phoneNumberWithTel = "tel:" + addr;
                        destinationNumbers.Add(phoneNumberWithTel);
                    }
                }
                if (multipleAddresses.Length == 1)
                {
                    SendSMSDataForSingle outBoundSms = new SendSMSDataForSingle();
                    outBoundSms.outboundSMSRequest = new OutboundSMSRequestForSingle();
                    outBoundSms.outboundSMSRequest.notifyDeliveryStatus = chkGetOnlineStatus.Checked;
                    outBoundSms.outboundSMSRequest.address = destinationNumbers[0];
                    outBoundSms.outboundSMSRequest.message = message.SelectedValue;
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    outBoundSmsJson = javaScriptSerializer.Serialize(outBoundSms);
                }
                else
                {
                    SendSMSDataForMultiple outBoundSms = new SendSMSDataForMultiple();
                    outBoundSms.outboundSMSRequest = new OutboundSMSRequestForMultiple();
                    outBoundSms.outboundSMSRequest.notifyDeliveryStatus = chkGetOnlineStatus.Checked;
                    outBoundSms.outboundSMSRequest.address = destinationNumbers;
                    outBoundSms.outboundSMSRequest.message = message.SelectedValue;
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    outBoundSmsJson = javaScriptSerializer.Serialize(outBoundSms);
                }
            }
            else
            {
                sendSMSErrorMessage = "No input provided for Address";
                return;
            }
            
            

            string sendSmsResponseData;
            HttpWebRequest sendSmsRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + this.sendSMSURL);
            sendSmsRequestObject.Method = "POST";
            sendSmsRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);
            sendSmsRequestObject.ContentType = "application/json";
            sendSmsRequestObject.Accept = "application/json";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(outBoundSmsJson);
            sendSmsRequestObject.ContentLength = postBytes.Length;

            Stream postStream = sendSmsRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            HttpWebResponse sendSmsResponseObject = (HttpWebResponse)sendSmsRequestObject.GetResponse();
            using (StreamReader sendSmsResponseStream = new StreamReader(sendSmsResponseObject.GetResponseStream()))
            {
                sendSmsResponseData = sendSmsResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                sendSMSResponseData = new SendSMSResponse();
                sendSMSResponseData.outBoundSMSResponse = new OutBoundSMSResponse();
                sendSMSResponseData = (SendSMSResponse)deserializeJsonObject.Deserialize(sendSmsResponseData, typeof(SendSMSResponse));
                if (!chkGetOnlineStatus.Checked)
                {
                    Session["lastSentSMSID"] = sendSMSResponseData.outBoundSMSResponse.messageId;
                    messageId.Value = Session["lastSentSMSID"].ToString();
                }
                sendSMSSuccessMessage = "Success";
                sendSmsResponseStream.Close();
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

            sendSMSErrorMessage= errorResponse;
        }
        catch (Exception ex)
        {
            sendSMSErrorMessage = ex.ToString();
        }
    }
    #region Get SMS Delivery Status code.
    /// <summary>
    /// This method is called when user clicks on get delivery status button
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void GetDeliveryStatusButton_Click(object sender, EventArgs e)
    {
        try
        {
            Session["lastSentSMSID"] = System.Web.HttpUtility.HtmlEncode(messageId.Value);
            if (this.ReadAndGetAccessToken(ref getSMSDeliveryStatusErrorMessage) == true)
            {
                this.GetSmsDeliveryStatus();
            }
            else
            {
                getSMSDeliveryStatusErrorMessage = "Unable to get access token.";
            }
        }
        catch (Exception ex)
        {
            getSMSDeliveryStatusErrorMessage = ex.ToString();
        }
    }

    /// <summary>
    /// This function is called when user clicks on get delivery status button.
    /// this funciton calls get sms delivery status API to fetch the status.
    /// </summary>
    private void GetSmsDeliveryStatus()
    {
        try
        {
            string getSmsDeliveryStatusResponseData;
            HttpWebRequest getSmsDeliveryStatusRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + this.getDeliveryStatusURL + messageId.Value);
            getSmsDeliveryStatusRequestObject.Method = "GET";
            getSmsDeliveryStatusRequestObject.Headers.Add("Authorization", "BEARER " + this.accessToken);
            getSmsDeliveryStatusRequestObject.ContentType = "application/JSON";
            getSmsDeliveryStatusRequestObject.Accept = "application/json";
            HttpWebResponse getSmsDeliveryStatusResponse = (HttpWebResponse)getSmsDeliveryStatusRequestObject.GetResponse();
            using (StreamReader getSmsDeliveryStatusResponseStream = new StreamReader(getSmsDeliveryStatusResponse.GetResponseStream()))
            {
                getSmsDeliveryStatusResponseData = getSmsDeliveryStatusResponseStream.ReadToEnd();
                getSmsDeliveryStatusResponseData = getSmsDeliveryStatusResponseData.Replace("-", string.Empty);
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                getSMSDeliveryStatusResponseData = new GetDeliveryStatus();
                getSMSDeliveryStatusResponseData = (GetDeliveryStatus)deserializeJsonObject.Deserialize(getSmsDeliveryStatusResponseData, typeof(GetDeliveryStatus));
                getSMSDeliveryStatusSuccessMessagae = "Success";
                getSmsDeliveryStatusResponseStream.Close();
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
            getSMSDeliveryStatusErrorMessage =   errorResponse ;
        }
        catch (Exception ex)
        {
            getSMSDeliveryStatusErrorMessage = ex.ToString();
        }
    }
    #endregion


    #region SMS Application related Data Structures

    #region Data structure for get access token
    /// <summary>
    /// Class to hold access token response
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Gets or sets access token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Gets or sets refresh token
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// Gets or sets expires in
        /// </summary>
        public string expires_in { get; set; }
    }
    #endregion
    #region Data structure for send sms response

    ///<summary>
    ///Class to hold ResourceReference
    ///</summary>
    public class ResourceReference
    {
        ///<summary>
        ///Gets or sets resourceURL
        ///</summary>
        public string resourceURL { get; set; }
    }

    public class SendSMSResponse
    {
        public OutBoundSMSResponse outBoundSMSResponse;
    }
    /// <summary>
    /// Class to hold send sms response
    /// </summary>
    public class OutBoundSMSResponse
    {
        /// <summary>
        /// Gets or sets messageId
        /// </summary>
        public string messageId { get; set; }
        /// <summary>
        /// Gets or sets ResourceReference
        /// </summary>
        public ResourceReference resourceReference { get; set; }
    }

    public class SendSMSDataForSingle
    {
        public OutboundSMSRequestForSingle outboundSMSRequest { get; set; }
    }

    public class OutboundSMSRequestForSingle
    {
        /// <summary>
        /// Gets or sets the address to send.
        /// </summary>
        public string address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets message text to send.
        /// </summary>
        public string message
        {
            get;
            set;
        }

        public bool notifyDeliveryStatus
        {
            get;
            set;
        }
    }
    public class SendSMSDataForMultiple
    {
        public OutboundSMSRequestForMultiple outboundSMSRequest { get; set; }
    }

    public class OutboundSMSRequestForMultiple
    {
        /// <summary>
        /// Gets or sets the list of address to send.
        /// </summary>
        public List<string> address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets message text to send.
        /// </summary>
        public string message
        {
            get;
            set;
        }

        public bool notifyDeliveryStatus
        {
            get;
            set;
        }
    }
    #endregion
    #region Data structure for Get SMS (offline)
    /// <summary>
    /// Class to hold rececive sms response
    /// </summary>
    public class GetSmsResponse
    {
        /// <summary>
        /// Gets or sets inbound sms message list
        /// </summary>
        public InboundSMSMessageList InboundSMSMessageList { get; set; }
    }

    /// <summary>
    /// Class to hold inbound sms message list
    /// </summary>
    public class InboundSMSMessageList
    {
        /// <summary>
        /// Gets or sets inbound sms message
        /// </summary>
        public List<InboundSMSMessage> InboundSMSMessage { get; set; }

        /// <summary>
        /// Gets or sets number of messages in a batch
        /// </summary>
        public int NumberOfMessagesInThisBatch { get; set; }

        /// <summary>
        /// Gets or sets resource url
        /// </summary>
        public string ResourceURL { get; set; }

        /// <summary>
        /// Gets or sets total number of pending messages
        /// </summary>
        public int TotalNumberOfPendingMessages { get; set; }
    }

    /// <summary>
    /// Class to hold inbound sms message
    /// </summary>
    public class InboundSMSMessage
    {
        /// <summary>
        /// Gets or sets message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets sender address
        /// </summary>
        public string SenderAddress { get; set; }
    }
    #endregion
    #region Data structure for Receive SMS (online)
    /// <summary>
    /// Class to hold inbound sms message
    /// </summary>
    public class ReceiveSMS
    {
        /// <summary>
        /// Gets or sets datetime
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// Gets or sets destination address
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        /// Gets or sets message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets sender address
        /// </summary>
        public string SenderAddress { get; set; }
    }
    #endregion
    #region Data structure for Get Delivery Status (offline)

    /// <summary>
    /// Class to hold delivery status
    /// </summary>
    public class GetDeliveryStatus
    {
        /// <summary>
        /// Gets or sets delivery info list
        /// </summary>
        public DeliveryInfoList DeliveryInfoList { get; set; }
    }

    /// <summary>
    /// Class to hold delivery info list
    /// </summary>
    public class DeliveryInfoList
    {
        /// <summary>
        /// Gets or sets resource url
        /// </summary>
        public string ResourceURL { get; set; }

        /// <summary>
        /// Gets or sets delivery info
        /// </summary>
        public List<DeliveryInfo> DeliveryInfo { get; set; }
    }

    /// <summary>
    /// Class to hold delivery info
    /// </summary>
    public class DeliveryInfo
    {
        /// <summary>
        /// Gets or sets id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets delivery status
        /// </summary>
        public string Deliverystatus { get; set; }
    }

    #endregion 
    #region Data structure for receive delivery status

    public class ReceiveDeliveryInfo
    {
        /// <summary>
        /// Gets or sets the list of address.
        /// </summary>
        public string address
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the list of deliveryStatus.
        /// </summary>
        public string deliveryStatus
        {
            get;
            set;
        }
    }
    public class deliveryInfoNotification
    {
        /// <summary>
        /// Gets or sets the list of messageId.
        /// </summary>
        public string messageId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets message text to send.
        /// </summary>
        public ReceiveDeliveryInfo deliveryInfo
        {
            get;
            set;
        }
    }
    #endregion


    #endregion
}