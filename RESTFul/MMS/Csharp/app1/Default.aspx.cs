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

#endregion

/// <summary>
/// MMS_App1 class
/// </summary>
/// <remarks> This application allows an end user to send an MMS message with up to three attachments of any common format, 
/// and check the delivery status of that MMS message.
/// </remarks>
public partial class MMS_App1 : System.Web.UI.Page
{
    /// <summary>
    /// Instance variables for local processing
    /// </summary>
    private string endPoint, accessTokenFilePath, apiKey, secretKey, accessToken, scope, refreshToken, bypassSSL;

    /// <summary>
    /// Instance variables for local processing
    /// </summary>
    private string expirySeconds, refreshTokenExpiryTime;

    /// <summary>
    /// Instance variables for local processing
    /// </summary>
    private string accessTokenExpiryTime;

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;

    /// <summary>
    /// Instance variables for local processing
    /// </summary>
    private List<string> phoneNumbersList = new List<string>();

    /// <summary>
    /// Instance variables for local processing
    /// </summary>
    private string phoneNumbersParameter = null;

    public string sendMessageResponseSuccess = string.Empty;
    public string sendMessageResponseError = string.Empty;
    public SendMMSResponse sendMMSResponseData = null;
    public string getDeliveryStatusResponseSuccess = string.Empty;
    public string getDeliveryStatusResponseError = string.Empty;
    public Dictionary<string, string> formattedResponse = new Dictionary<string, string>();
    public Dictionary<string, string> getDeliveryStatusResponse = new Dictionary<string, string>();
    public Dictionary<string, string> sendMessageResponse = new Dictionary<string, string>();
    public List<ImageData> imageList = new List<ImageData>();
    public string messageId = string.Empty;
    public string ListenerShortCode = string.Empty;
    public string ImageDirectory = string.Empty;
    public int totalImages = 0;

    public GetDeliveryStatus getMMSDeliveryStatusResponseData = null;



    public List<deliveryInfoNotification> receiveMMSDeliveryStatusResponseData = new List<deliveryInfoNotification>();

    private string SendImageFilesDir;


    private void DisplayDictionary(Dictionary<string, object> dict)
    {
        foreach (string strKey in dict.Keys)
        {
            //string strOutput = "".PadLeft(indentLevel * 8) + strKey + ":";

            object o = dict[strKey];
            if (o is Dictionary<string, object>)
            {
                DisplayDictionary((Dictionary<string, object>)o);
            }
            else if (o is ArrayList)
            {
                foreach (object oChild in ((ArrayList)o))
                {
                    if (oChild is string)
                    {
                        string strOutput = ((string)oChild);
                        //formattedResponse.Add(strOutput, "");
                    }
                    else if (oChild is Dictionary<string, object>)
                    {
                        DisplayDictionary((Dictionary<string, object>)oChild);
                    }
                }
            }
            else
            {
                formattedResponse.Add(strKey.ToString(), o.ToString());
            }
        }
    }

    public void DisplayImagesReceived()
    {
        try
        {

            this.ListenerShortCode = ConfigurationManager.AppSettings["ListenerShortCode"];
            if (string.IsNullOrEmpty(this.ListenerShortCode))
            {
                this.ListenerShortCode = "Not defined";
            }

            this.ImageDirectory = ConfigurationManager.AppSettings["ImageDirectory"];
            if (string.IsNullOrEmpty(this.ImageDirectory))
            {
                this.ImageDirectory = "~\\ReceivedImages\\";
            }

            //// Read the refund file for the list of transactions and store locally
            FileStream file = new FileStream(Request.MapPath(this.ImageDirectory + "imageDetails.txt"), FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] imgDetails = Regex.Split(line, ":-:");
                if (imgDetails[0] != null && imgDetails[1] != null && imgDetails[2] != null && imgDetails[3] != null)
                {
                    ImageData img = new ImageData();
                    img.senderAddress = imgDetails[0];
                    img.date = imgDetails[1];
                    img.path = Path.GetFileName(Path.GetDirectoryName(this.ImageDirectory))+ "\\" + imgDetails[2];
                    img.text = imgDetails[3];
                    imageList.Add(img);
                }
            }
            this.totalImages = this.imageList.Count;
            sr.Close();
            file.Close();
        }
        catch (Exception ex)
        {
            return;
        }
    }
    #region Bypass SSL Certificate Error

    /// <summary>
    /// This method neglects the ssl handshake error with authentication server
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
    /// This method reads config file and assigns values to local variables
    /// </summary>
    /// <returns>true/false, true- if able to read from config file</returns>
    private bool ReadConfigFile()
    {
        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            sendMessageResponseError =  "api_key is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            sendMessageResponseError =  "secret_key is not defined in configuration file";
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            sendMessageResponseError =  "endPoint is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "MMS";
        }

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\MMSApp1AccessToken.txt";
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

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SendImageFilesDir"]))
        {
            this.SendImageFilesDir = Request.MapPath(ConfigurationManager.AppSettings["SendImageFilesDir"]);
        }

        if (!IsPostBack)
        {
            attachment.Items.Add("");
            if (!string.IsNullOrEmpty(SendImageFilesDir))
            {
                string[] filePaths = Directory.GetFiles(this.SendImageFilesDir);
                foreach (string filePath in filePaths)
                {
                    attachment.Items.Add(Path.GetFileName(filePath));
                }
                if (filePaths.Length > 0)
                    attachment.Items[0].Selected = true;
            }
        }

        return true;
    }

    #endregion
    /// <summary>
    /// Event, that triggers when the applicaiton page is loaded into the browser, reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();

            this.ReadConfigFile();

            this.DisplayImagesReceived();

            readOnlineDeliveryStatus();
        }
        catch (Exception ex)
        {
            sendMessageResponseError = ex.ToString();
        }
    }
    /// <summary>
    /// This funciton initiates send mms api call to send selected files as an mms
    /// </summary>
    private void SendMMS()
    {
        try
        {
            string mmsAddress = this.GetPhoneNumbers();
            string mmsMessage = subject.SelectedValue.ToString();

            if (string.Compare(attachment.SelectedValue.ToString(),"")==0)
            {
                this.SendMessageNoAttachments(mmsAddress, mmsMessage);
            }
            else
            {
                string mmsFile = this.SendImageFilesDir + attachment.SelectedValue.ToString();
                this.SendMultimediaMessage(mmsAddress, mmsMessage, mmsFile);
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
            sendMessageResponseError = errorResponse + Environment.NewLine + we.Message;
        }
        catch (Exception ex)
        {
            sendMessageResponseError =  ex.ToString();
        }
    }

    /// <summary>
    /// Method will be called when the user clicks on Update Votes Total button
    /// </summary>
    /// <param name="sender">object, that invoked this method</param>
    /// <param name="e">EventArgs, specific to this method</param>
    protected void receiveStatusBtn_Click(object sender, EventArgs e)
    {
    }

    protected void GetStatus_Click(object sender, EventArgs e)
    {
        try
        {
            string messageId = mmsId.Value;
            if (messageId == null || messageId.Length <= 0)
            {
                getDeliveryStatusResponseError = "Message Id is null or empty";
                return;
            }

            if (this.ReadAndGetAccessToken(ref getDeliveryStatusResponseError) == true)
            {
                string mmsDeliveryStatus;
                HttpWebRequest mmsStatusRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/mms/v3/messaging/outbox/" + messageId);
                mmsStatusRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);
                mmsStatusRequestObject.Method = "GET";

                HttpWebResponse mmsStatusResponseObject = (HttpWebResponse)mmsStatusRequestObject.GetResponse();
                using (StreamReader mmsStatusResponseStream = new StreamReader(mmsStatusResponseObject.GetResponseStream()))
                {
                    mmsDeliveryStatus = mmsStatusResponseStream.ReadToEnd();
                    //mmsDeliveryStatus = mmsDeliveryStatus.Replace("-", string.Empty);
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    //Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsDeliveryStatus);
                    //DisplayDictionary(dict);
                    //getDeliveryStatusResponse = formattedResponse;
                    getMMSDeliveryStatusResponseData = new GetDeliveryStatus();
                    getMMSDeliveryStatusResponseData = (GetDeliveryStatus)deserializeJsonObject.Deserialize(mmsDeliveryStatus, typeof(GetDeliveryStatus));
                    getDeliveryStatusResponseSuccess = "true"; mmsStatusResponseStream.Close();
                }
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
            getDeliveryStatusResponseError = errorResponse + Environment.NewLine + we.Message;
        }
        catch (Exception ex)
        {
            getDeliveryStatusResponseError = ex.ToString();
        }

    }

    /// <summary>
    /// Sends MMS by calling messaging api
    /// </summary>
    /// <param name="mmsAddress">string, phone number</param>
    /// <param name="mmsMessage">string, mms message</param>
    private void SendMultimediaMessage(string mmsAddress, string mmsMessage, string mmsFile)
    {
        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        HttpWebRequest mmsRequestObject = (HttpWebRequest)WebRequest.Create(string.Empty + this.endPoint + "/mms/v3/messaging/outbox");
        mmsRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);
        mmsRequestObject.ContentType = "multipart/related; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"" + boundary + "\"\r\n";
        mmsRequestObject.Method = "POST";
        mmsRequestObject.KeepAlive = true;

        UTF8Encoding encoding = new UTF8Encoding();

        byte[] totalpostBytes = encoding.GetBytes(string.Empty);
        string sendMMSData = mmsAddress + "subject=" + Server.UrlEncode(mmsMessage) + "&notifyDeliveryStatus=" + chkGetOnlineStatus.Checked;

        string data = string.Empty;
        data += "--" + boundary + "\r\n";
        data += "Content-Type: application/x-www-form-urlencoded;charset=UTF-8\r\nContent-Transfer-Encoding: 8bit\r\nContent-Disposition: form-data; name=\"root-fields\"\r\nContent-ID:<startpart>\r\n\r\n" + sendMMSData;// +"\r\n";

        totalpostBytes = this.FormMIMEParts(boundary, data, mmsFile);

        byte[] byteLastBoundary = encoding.GetBytes("\r\n--" + boundary + "--\r\n");
        int totalSize = totalpostBytes.Length + byteLastBoundary.Length;

        var totalMS = new MemoryStream(new byte[totalSize], 0, totalSize, true, true);
        totalMS.Write(totalpostBytes, 0, totalpostBytes.Length);
        totalMS.Write(byteLastBoundary, 0, byteLastBoundary.Length);

        byte[] finalpostBytes = totalMS.GetBuffer();
        mmsRequestObject.ContentLength = finalpostBytes.Length;

        Stream postStream = null;
        try
        {
            postStream = mmsRequestObject.GetRequestStream();
            postStream.Write(finalpostBytes, 0, finalpostBytes.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != postStream)
            {
                postStream.Close();
            }
        }

        WebResponse mmsResponseObject = mmsRequestObject.GetResponse();
        using (StreamReader streamReader = new StreamReader(mmsResponseObject.GetResponseStream()))
        {
            string mmsResponseData = streamReader.ReadToEnd();
            JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
            //Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsResponseData);
            //DisplayDictionary(dict);
            //sendMessageResponse = formattedResponse;
            sendMMSResponseData = new SendMMSResponse();
            sendMMSResponseData.outboundMessageResponse = new OutBoundMMSResponse();
            sendMMSResponseData.outboundMessageResponse.resourceReference = new ResourceReference();
            sendMMSResponseData = (SendMMSResponse)deserializeJsonObject.Deserialize(mmsResponseData, typeof(SendMMSResponse));
            if (!chkGetOnlineStatus.Checked)
                mmsId.Value = sendMMSResponseData.outboundMessageResponse.messageId;
            sendMessageResponseSuccess = "true";
            streamReader.Close();
        }
    }

    /// <summary>
    /// This function reads the Access Token File and stores the values of access token, expiry seconds
    /// refresh token, last access token time and refresh token expiry time
    /// This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns boolean</returns>    
    private bool ReadAccessTokenFile(ref string message)
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
            message = ex.Message;
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

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/v4/token");
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

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/v4/token");
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
    /// This method will be called when user clicks on send mms button
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void SendMessage_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.ReadAndGetAccessToken(ref sendMessageResponseError) == true)
            {
                this.SendMMS();
            }

        }
        catch (Exception ex)
        {
            sendMessageResponseError = ex.ToString();
            return;
        }
    }

    /// <summary>
    /// Form mime parts for the user input files
    /// </summary>
    /// <param name="boundary">string, boundary data</param>
    /// <param name="data">string, mms message</param>
    /// <returns>returns byte array of files</returns>
    private byte[] FormMIMEParts(string boundary, string data, string mmsFile)
    {
        UTF8Encoding encoding = new UTF8Encoding();

        byte[] postBytes = encoding.GetBytes(string.Empty);
        byte[] totalpostBytes = encoding.GetBytes(string.Empty);

        byte[] Head = encoding.GetBytes(data);
        int totalSizeWithHead = totalpostBytes.Length + Head.Length;

        var totalMSWithHead = new MemoryStream(new byte[totalSizeWithHead], 0, totalSizeWithHead, true, true);
        totalMSWithHead.Write(totalpostBytes, 0, totalpostBytes.Length);
        totalMSWithHead.Write(Head, 0, Head.Length);
        totalpostBytes = totalMSWithHead.GetBuffer();

        postBytes = this.GetBytesOfFile(boundary, mmsFile);
        var msOne = JoinTwoByteArrays(totalpostBytes, postBytes);
        totalpostBytes = msOne.GetBuffer();

        return totalpostBytes;
    }

    /// <summary>
    /// Gets the bytes representation of file along with mime part
    /// </summary>
    /// <param name="boundary">string, boundary message</param>
    /// <param name="data">string, mms message</param>
    /// <param name="filePath">string, filepath</param>
    /// <returns>byte[], representation of file in bytes</returns>
    private byte[] GetBytesOfFile(string boundary, string filePath)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] postBytes = encoding.GetBytes(string.Empty);
        FileStream fileStream = null;
        BinaryReader binaryReader = null;

        try
        {
            string mmsFileName = Path.GetFileName(filePath);
            string mmsFileExtension = Path.GetExtension(filePath);
            string attachmentContentType = MapContentTypeFromExtension(mmsFileExtension);
            fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            binaryReader = new BinaryReader(fileStream);

            byte[] image = binaryReader.ReadBytes((int)fileStream.Length);

            string data = "\r\n--" + boundary + "\r\n";
            data += "Content-Disposition: attachment; filename=" + mmsFileName + "\r\n";
            data += "Content-Type: " + attachmentContentType + ";name=" + mmsFileName + "\r\n";
            data += "Content-ID: " + mmsFileName + "\r\n";
            data += "Content-Transfer-Encoding: Binary\r\n\r\n";

            byte[] firstPart = encoding.GetBytes(data);
            int newSize = firstPart.Length + image.Length;

            var memoryStream = new MemoryStream(new byte[newSize], 0, newSize, true, true);
            memoryStream.Write(firstPart, 0, firstPart.Length);
            memoryStream.Write(image, 0, image.Length);

            postBytes = memoryStream.GetBuffer();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != binaryReader)
            {
                binaryReader.Close();
            }

            if (null != fileStream)
            {
                fileStream.Close();
            }
        }

        return postBytes;
    }
    /// <summary>
    /// Invokes messaging api to send message without any attachments
    /// </summary>
    /// <param name="mmsAddress">string, phone number</param>
    /// <param name="mmsMessage">string, mms message</param>
    private void SendMessageNoAttachments(string mmsAddress, string mmsMessage)
    {
        string boundaryToSend = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        HttpWebRequest mmsRequestObject = (HttpWebRequest)WebRequest.Create(string.Empty + this.endPoint + "/mms/v3/messaging/outbox");
        mmsRequestObject.Headers.Add("Authorization", "Bearer " + this.accessToken);
        mmsRequestObject.ContentType = "multipart/form-data; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"" + boundaryToSend + "\"\r\n";
        mmsRequestObject.Method = "POST";
        mmsRequestObject.KeepAlive = true;

        UTF8Encoding encoding = new UTF8Encoding();
        byte[] bytesToSend = encoding.GetBytes(string.Empty);
        string mmsParameters = mmsAddress + "subject=" + Server.UrlEncode(mmsMessage) + "&notifyDeliveryStatus=" + chkGetOnlineStatus.Checked;

        string dataToSend = string.Empty;
        dataToSend += "--" + boundaryToSend + "\r\n";
        dataToSend += "Content-Type: application/x-www-form-urlencoded; charset=UTF-8\r\nContent-Transfer-Encoding: 8bit\r\nContent-Disposition: form-data; name=\"root-fields\"\r\nContent-ID: <startpart>\r\n\r\n" + mmsParameters + "\r\n";
        dataToSend += "--" + boundaryToSend + "--\r\n";
        bytesToSend = encoding.GetBytes(dataToSend);

        int sizeToSend = bytesToSend.Length;
        var memBufToSend = new MemoryStream(new byte[sizeToSend], 0, sizeToSend, true, true);
        memBufToSend.Write(bytesToSend, 0, bytesToSend.Length);
        byte[] finalData = memBufToSend.GetBuffer();
        mmsRequestObject.ContentLength = finalData.Length;

        Stream postStream = null;
        try
        {
            postStream = mmsRequestObject.GetRequestStream();
            postStream.Write(finalData, 0, finalData.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != postStream)
            {
                postStream.Close();
            }
        }

        WebResponse mmsResponseObject = mmsRequestObject.GetResponse();
        using (StreamReader streamReader = new StreamReader(mmsResponseObject.GetResponseStream()))
        {
            string mmsResponseData = streamReader.ReadToEnd();
            JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
            //Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsResponseData);
            //DisplayDictionary(dict);
            sendMMSResponseData = new SendMMSResponse();
            sendMMSResponseData.outboundMessageResponse = new OutBoundMMSResponse();
            sendMMSResponseData.outboundMessageResponse.resourceReference = new ResourceReference();
            sendMMSResponseData = (SendMMSResponse)deserializeJsonObject.Deserialize(mmsResponseData, typeof(SendMMSResponse));
            if (!chkGetOnlineStatus.Checked)
                mmsId.Value = sendMMSResponseData.outboundMessageResponse.messageId;
            sendMessageResponseSuccess = "true";
            streamReader.Close();
        }
    }

    /// <summary>
    /// Content type based on the file extension.
    /// </summary>
    /// <param name="extension">file extension</param>
    /// <returns>the Content type mapped to the extension"/> summed memory stream</returns>
    private static string MapContentTypeFromExtension(string extension)
    {
        Dictionary<string, string> extensionToContentTypeMapping = new Dictionary<string, string>()
            {
                { ".jpg", "image/jpeg" }, { ".bmp", "image/bmp" }, { ".mp3", "audio/mp3" },
                { ".m4a", "audio/m4a" }, { ".gif", "image/gif" }, { ".3gp", "video/3gpp" },
                { ".3g2", "video/3gpp2" }, { ".wmv", "video/x-ms-wmv" }, { ".m4v", "video/x-m4v" },
                { ".amr", "audio/amr" }, { ".mp4", "video/mp4" }, { ".avi", "video/x-msvideo" },
                { ".mov", "video/quicktime" }, { ".mpeg", "video/mpeg" }, { ".wav", "audio/x-wav" },
                { ".aiff", "audio/x-aiff" }, { ".aifc", "audio/x-aifc" }, { ".midi", ".midi" },
                { ".au", "audio/basic" }, { ".xwd", "image/x-xwindowdump" }, { ".png", "image/png" },
                { ".tiff", "image/tiff" }, { ".ief", "image/ief" }, { ".txt", "text/plain" },
                { ".html", "text/html" }, { ".vcf", "text/x-vcard" }, { ".vcs", "text/x-vcalendar" },
                { ".mid", "application/x-midi" }, { ".imy", "audio/iMelody" }
            };
        if (extensionToContentTypeMapping.ContainsKey(extension))
        {
            return extensionToContentTypeMapping[extension];
        }
        else
        {
            throw new ArgumentException("invalid attachment extension");
        }
    }

    /// <summary>
    /// This function adds two byte arrays
    /// </summary>
    /// <param name="firstByteArray">first array of bytes</param>
    /// <param name="secondByteArray">second array of bytes</param>
    /// <returns>returns MemoryStream after joining two byte arrays</returns>
    private static MemoryStream JoinTwoByteArrays(byte[] firstByteArray, byte[] secondByteArray)
    {
        int newSize = firstByteArray.Length + secondByteArray.Length;
        var ms = new MemoryStream(new byte[newSize], 0, newSize, true, true);
        ms.Write(firstByteArray, 0, firstByteArray.Length);
        ms.Write(secondByteArray, 0, secondByteArray.Length);
        return ms;
    }

    /// <summary>
    /// Gets formatted phone number
    /// </summary>
    /// <returns>string, phone number</returns>
    private string GetPhoneNumber(ref string error)
    {
        long tryParseResult = 0;

        string smsAddressInput = address.Value;

        string smsAddressFormatted = string.Empty;

        string phoneStringPattern = "^\\d{3}-\\d{3}-\\d{4}$";
        if (System.Text.RegularExpressions.Regex.IsMatch(smsAddressInput, phoneStringPattern))
        {
            smsAddressFormatted = smsAddressInput.Replace("-", string.Empty);
        }
        else
        {
            smsAddressFormatted = smsAddressInput;
        }

        if (smsAddressFormatted.Length == 16 && smsAddressFormatted.StartsWith("tel:+1"))
        {
            smsAddressFormatted = smsAddressFormatted.Substring(6, 10);
        }
        else if (smsAddressFormatted.Length == 15 && smsAddressFormatted.StartsWith("tel:+"))
        {
            smsAddressFormatted = smsAddressFormatted.Substring(5, 10);
        }
        else if (smsAddressFormatted.Length == 14 && smsAddressFormatted.StartsWith("tel:"))
        {
            smsAddressFormatted = smsAddressFormatted.Substring(4, 10);
        }
        else if (smsAddressFormatted.Length == 12 && smsAddressFormatted.StartsWith("+1"))
        {
            smsAddressFormatted = smsAddressFormatted.Substring(2, 10);
        }
        else if (smsAddressFormatted.Length == 11 && smsAddressFormatted.StartsWith("1"))
        {
            smsAddressFormatted = smsAddressFormatted.Substring(1, 10);
        }

        if ((smsAddressFormatted.Length != 10) || (!long.TryParse(smsAddressFormatted, out tryParseResult)))
        {
            error = "Invalid phone number: " + smsAddressInput;
            smsAddressFormatted = string.Empty;
        }

        return smsAddressFormatted;
    }

    /// <summary>
    /// This method gets the phone numbers present in phonenumber text box and validates each phone number and prepares valid and invalid phone number lists
    /// and returns a bool value indicating if able to get the phone numbers.
    /// </summary>
    /// <returns>true/false; true if able to get valis phone numbers, else false</returns>
    private string GetPhoneNumbers()
    {

        string[] phoneNumbers = address.Value.Split(',');
        foreach (string phoneNum in phoneNumbers)
        {
                this.phoneNumbersList.Add(phoneNum);
        }

        foreach (string phoneNo in this.phoneNumbersList)
        {
                if (phoneNo.StartsWith("tel:"))
                {
                    this.phoneNumbersParameter = this.phoneNumbersParameter + "address=" + Server.UrlEncode(phoneNo.ToString()) + "&";
                }
                else
                {
                    this.phoneNumbersParameter = this.phoneNumbersParameter + "address=" + Server.UrlEncode("tel:" + phoneNo.ToString()) + "&";
                }
        }
        if (!string.IsNullOrEmpty(this.phoneNumbersParameter))
            return this.phoneNumbersParameter;
        else
            return "";
    }

    /// <summary>
    /// This method reads the messages file and draw the table.
    /// </summary>
    private void readOnlineDeliveryStatus()
    {
        try
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
                        dNot.deliveryInfo.deliveryStatus = messageValues[2];
                        receiveMMSDeliveryStatusResponseData.Add(dNot);
                    }
                    sr.Close();
                    receiveMMSDeliveryStatusResponseData.Reverse();
                }

            }
        }
        catch (Exception ex)
        {
            return;
        }
    }

    #region Data Structures

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

    public class SendMMSResponse
    {
        public OutBoundMMSResponse outboundMessageResponse;
    }
    /// <summary>
    /// Class to hold send sms response
    /// </summary>
    public class OutBoundMMSResponse
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

    /// <summary>
    /// Image structure Object
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// Gets or sets the value of path
        /// </summary>
        public string path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of date
        /// </summary>
        public string date
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of senderAddress
        /// </summary>
        public string senderAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of text
        /// </summary>
        public string text
        {
            get;
            set;
        }
    }

    /// <summary>
    /// AccessTokenResponse Object
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Gets or sets the value of access_token
        /// </summary>
        public string access_token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of refresh_token
        /// </summary>
        public string refresh_token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of expires_in
        /// </summary>
        public string expires_in
        {
            get;
            set;
        }
    }


    #endregion
}

