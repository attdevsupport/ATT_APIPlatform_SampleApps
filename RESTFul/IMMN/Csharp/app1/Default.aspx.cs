// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

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
public partial class MIM_App1 : System.Web.UI.Page
{
    #region Instance variables

    /// <summary>
    /// API Address
    /// </summary>
    private string endPoint;

    /// <summary>
    /// Access token variables - temporary
    /// </summary>
    private string apiKey, authCode, authorizeRedirectUri, secretKey, accessToken,
        scope, refreshToken, refreshTokenExpiryTime, accessTokenExpiryTime;

    /// <summary>
    /// Maximum number of addresses user can specify
    /// </summary>
    private int maxAddresses;

    /// <summary>
    /// List of addresses to send
    /// </summary>
    private List<string> addressList = new List<string>();

    /// <summary>
    /// Variable to hold phone number(s)/email address(es)/short code(s) parameter.
    /// </summary>
    private string phoneNumbersParameter = null;

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;

    private string AttachmentFilesDir = string.Empty;

    #endregion
    public List<string> attachments = null;
    public string sendMessageSuccessResponse = string.Empty;
    public string sendMessageErrorResponse = string.Empty;
    public string getHeadersErrorResponse = string.Empty;
    public string getHeadersSuccessResponse = string.Empty;
    public string getMessageSuccessResponse = string.Empty;
    public string getMessageErrorResponse = string.Empty;
    public string content_result = string.Empty;
    public byte[] receivedBytes = null;
    public WebResponse getContentResponseObject = null;
    public string[] imageData = null;
    public MessageHeaderList messageHeaderList;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.BypassCertificateError();
        this.ReadConfigFile();

        if ((Session["cs_rest_appState"] == "GetToken") && (Request["Code"] != null))
        {
            this.authCode = Request["code"].ToString();
            if (this.GetAccessToken(AccessTokenType.Authorization_Code) == true)
            {
                RestoreRequestSessionVariables();
                ResetRequestSessionVariables();
                if (string.Compare(Session["cs_rest_ServiceRequest"].ToString(), "sendmessasge") == 0)
                    this.SendMessageRequest();
                else if (string.Compare(Session["cs_rest_ServiceRequest"].ToString(), "getmessageheader") == 0)
                    this.GetMessageHeaders();
                else if (string.Compare(Session["cs_rest_ServiceRequest"].ToString(), "getmessagecontent") == 0)
                    this.GetMessageContentByIDnPartNumber();

            }
            else
            {
                sendMessageErrorResponse = "Failed to get Access token";
                this.ResetTokenSessionVariables();
                this.ResetTokenVariables();
                return;
            }
        }            

    }

    #region Access Token functions

    /// <summary>
    /// This function resets access token related session variable to null 
    /// </summary>
    private void ResetTokenSessionVariables()
    {
        Session["cs_rest_AccessToken"] = null;
        Session["cs_rest_AccessTokenExpirtyTime"] = null;
        Session["cs_rest_RefreshToken"] = null;
        Session["cs_rest_RefreshTokenExpiryTime"] = null;
    }

    /// <summary>
    /// This function resets access token related  variable to null 
    /// </summary>
    private void ResetTokenVariables()
    {
        this.accessToken = null;
        this.refreshToken = null;
        this.refreshTokenExpiryTime = null;
        this.accessTokenExpiryTime = null;
    }

    /// <summary>
    /// Redirect to OAuth and get Authorization Code
    /// </summary>
    private void GetAuthCode()
    {
        try
        {
            Response.Redirect(string.Empty + this.endPoint + "/oauth/authorize?scope=" + this.scope + "&client_id=" + this.apiKey + "&redirect_url=" + this.authorizeRedirectUri);
        }
        catch (Exception ex)
        {
            if (Session["cs_rest_ServiceRequest"] != null && (string.Compare(Session["cs_rest_ServiceRequest"].ToString(),
                                                                        "sendmessasge") == 0))
            {
              sendMessageErrorResponse = ex.Message;
            }
            else
            {
                getMessageErrorResponse = ex.Message;
            }
        }
    }

    /// <summary>
    /// Reads access token related session variables to local variables
    /// </summary>
    /// <returns>true/false depending on the session variables</returns>
    private bool ReadTokenSessionVariables()
    {
        if (Session["cs_rest_AccessToken"] != null)
        {
            this.accessToken = Session["cs_rest_AccessToken"].ToString();
        }
        else
        {
            this.accessToken = null;
        }

        if (Session["cs_rest_AccessTokenExpirtyTime"] != null)
        {
            this.accessTokenExpiryTime = Session["cs_rest_AccessTokenExpirtyTime"].ToString();
        }
        else
        {
            this.accessTokenExpiryTime = null;
        }

        if (Session["cs_rest_RefreshToken"] != null)
        {
            this.refreshToken = Session["cs_rest_RefreshToken"].ToString();
        }
        else
        {
            this.refreshToken = null;
        }

        if (Session["cs_rest_RefreshTokenExpiryTime"] != null)
        {
            this.refreshTokenExpiryTime = Session["cs_rest_RefreshTokenExpiryTime"].ToString();
        }
        else
        {
            this.refreshTokenExpiryTime = null;
        }

        if ((this.accessToken == null) || (this.accessTokenExpiryTime == null) || (this.refreshToken == null) || (this.refreshTokenExpiryTime == null))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates access token related variables
    /// </summary>
    /// <returns>string, returns VALID_ACCESS_TOKEN if its valid
    /// otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    /// return REFRESH_TOKEN, if access token in expired and refresh token is valid</returns>
    private string IsTokenValid()
    {
        if (Session["cs_rest_AccessToken"] == null)
        {
            return "INVALID_ACCESS_TOKEN";
        }

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
    /// <param name="type">If type value is 0, access token is fetch for authorization code flow
    /// If type value is 2, access token is fetch for authorization code floww based on the exisiting refresh token</param>
    /// <returns>true/false; true if success, else false</returns>
    private bool GetAccessToken(AccessTokenType type)
    {
        Stream postStream = null;
        try
        {
            DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();
            WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
            accessTokenRequest.Method = "POST";
            string oauthParameters = string.Empty;

            if (type == AccessTokenType.Authorization_Code)
            {
                oauthParameters = "client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&code=" + this.authCode + "&grant_type=authorization_code&scope=" + this.scope;
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
            postStream.Close();

            WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
            using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
            {
                string access_token_json = accessTokenResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(access_token_json, typeof(AccessTokenResponse));
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

                    this.accessTokenExpiryTime = Session["cs_rest_AccessTokenExpirtyTime"].ToString();
                    Session["cs_rest_RefreshToken"] = this.refreshToken;
                    Session["cs_rest_RefreshTokenExpiryTime"] = this.refreshTokenExpiryTime.ToString();
                    Session["cs_rest_appState"] = "TokenReceived";
                    accessTokenResponseStream.Close();
                    return true;
                }
                else
                {
                    string errorMessage= "Auth server returned null access token";
                    if (Session["cs_rest_ServiceRequest"] != null && (string.Compare(Session["cs_rest_ServiceRequest"].ToString(),
                                                            "sendmessasge") == 0))
                    {
                        sendMessageErrorResponse = errorMessage;
                    }
                    else
                    {
                        getMessageErrorResponse = errorMessage;
                    }
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;
            if (Session["cs_rest_ServiceRequest"] != null && (string.Compare(Session["cs_rest_ServiceRequest"].ToString(),
                                                    "sendmessasge") == 0))
            {
                sendMessageErrorResponse = errorMessage;
            }
            else
            {
                getMessageErrorResponse = errorMessage;
            }
        }
        finally
        {
            if (null != postStream)
            {
                postStream.Close();
            }
        }

        return false;
    }

    #endregion
    /// <summary>
    /// Read parameters from configuraton file
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            sendMessageErrorResponse = "endPoint is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            sendMessageErrorResponse =  "api_key is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            sendMessageErrorResponse =  "secret_key is not defined in configuration file";
            return false;
        }

        this.authorizeRedirectUri = ConfigurationManager.AppSettings["authorize_redirect_uri"];
        if (string.IsNullOrEmpty(this.authorizeRedirectUri))
        {
            sendMessageErrorResponse =  "authorize_redirect_uri is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "IMMN,MIM";
        }

        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["max_addresses"]))
        {
            this.maxAddresses = 10;
        }
        else
        {
            this.maxAddresses = Convert.ToInt32(ConfigurationManager.AppSettings["max_addresses"]);
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
                    attachment.Items.Add(Path.GetFileName(filePath));
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Gets the mapping of extension with predefined content types
    /// </summary>
    /// <param name="extension">file extension</param>
    /// <returns>string, content type</returns>
    private string GetContentTypeFromExtension(string extension)
    {
        Dictionary<string, string> extensionToContentType = new Dictionary<string, string>()
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
        if (extensionToContentType.ContainsKey(extension))
        {
            return extensionToContentType[extension];
        }
        else
        {
            return "Not Found";
        }
    }

    /// <summary>
    /// Sends message to the list of addresses provided.
    /// </summary>
    /// <param name="attachments">List of attachments</param>
    private void SendMessageRequest( string accToken, string edPoint,string subject, string message, string groupflag, ArrayList attachments)
    {
        Stream postStream = null;
        try
        {
            string boundaryToSend = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest msgRequestObject = (HttpWebRequest)WebRequest.Create(string.Empty + edPoint + "/rest/1/MyMessages");
            msgRequestObject.Headers.Add("Authorization", "Bearer " + accToken);
            msgRequestObject.Method = "POST";
            string contentType = "multipart/form-data; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"" + boundaryToSend + "\"\r\n";
            msgRequestObject.ContentType = contentType;
            string mmsParameters = this.phoneNumbersParameter + "Subject=" + Server.UrlEncode(subject) + "&Text=" + Server.UrlEncode(message) + "&Group=" + groupflag;

            string dataToSend = string.Empty;
            dataToSend += "--" + boundaryToSend + "\r\n";
            dataToSend += "Content-Type: application/x-www-form-urlencoded; charset=UTF-8\r\nContent-Transfer-Encoding: 8bit\r\nContent-Disposition: form-data; name=\"root-fields\"\r\nContent-ID: <startpart>\r\n\r\n" + mmsParameters + "\r\n";

            UTF8Encoding encoding = new UTF8Encoding();
            if ((attachments == null) || (attachments.Count == 0))
            {
                if (!groupCheckBox.Checked)
                {
                    msgRequestObject.ContentType = "application/x-www-form-urlencoded";
                    byte[] postBytes = encoding.GetBytes(mmsParameters);
                    msgRequestObject.ContentLength = postBytes.Length;

                    postStream = msgRequestObject.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Close();

                    WebResponse mmsResponseObject1 = msgRequestObject.GetResponse();
                    using (StreamReader sr = new StreamReader(mmsResponseObject1.GetResponseStream()))
                    {
                        string mmsResponseData = sr.ReadToEnd();
                        JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                        MsgResponseId deserializedJsonObj = (MsgResponseId)deserializeJsonObject.Deserialize(mmsResponseData, typeof(MsgResponseId));
                        sendMessageSuccessResponse = deserializedJsonObj.Id.ToString();
                        sr.Close();
                    }
                }
                else
                {
                    dataToSend += "--" + boundaryToSend + "--\r\n";
                    byte[] bytesToSend = encoding.GetBytes(dataToSend);

                    int sizeToSend = bytesToSend.Length;

                    var memBufToSend = new MemoryStream(new byte[sizeToSend], 0, sizeToSend, true, true);
                    memBufToSend.Write(bytesToSend, 0, bytesToSend.Length);

                    byte[] finalData = memBufToSend.GetBuffer();
                    msgRequestObject.ContentLength = finalData.Length;

                    postStream = msgRequestObject.GetRequestStream();
                    postStream.Write(finalData, 0, finalData.Length);

                    WebResponse mmsResponseObject1 = msgRequestObject.GetResponse();
                    using (StreamReader sr = new StreamReader(mmsResponseObject1.GetResponseStream()))
                    {
                        string mmsResponseData = sr.ReadToEnd();
                        JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                        MsgResponseId deserializedJsonObj = (MsgResponseId)deserializeJsonObject.Deserialize(mmsResponseData, typeof(MsgResponseId));
                        sendMessageSuccessResponse =  deserializedJsonObj.Id.ToString();
                        sr.Close();
                    }
                }
            }
            else
            {
                byte[] dataBytes = encoding.GetBytes(string.Empty);
                byte[] totalDataBytes = encoding.GetBytes(string.Empty);
                int count = 0;
                foreach (string attachment in attachments)
                {
                    string mmsFileName = Path.GetFileName(attachment.ToString());
                    string mmsFileExtension = Path.GetExtension(attachment.ToString());
                    string attachmentContentType = this.GetContentTypeFromExtension(mmsFileExtension);
                    FileStream imageFileStream = new FileStream(attachment.ToString(), FileMode.Open, FileAccess.Read);
                    BinaryReader imageBinaryReader = new BinaryReader(imageFileStream);
                    byte[] image = imageBinaryReader.ReadBytes((int)imageFileStream.Length);
                    imageBinaryReader.Close();
                    imageFileStream.Close();
                    if (count == 0)
                    {
                        dataToSend += "\r\n--" + boundaryToSend + "\r\n";
                    }
                    else
                    {
                        dataToSend = "\r\n--" + boundaryToSend + "\r\n";
                    }

                    dataToSend += "Content-Disposition: form-data; name=\"file" + count + "\"; filename=\"" + mmsFileName + "\"\r\n";
                    dataToSend += "Content-Type:" + attachmentContentType + "\r\n";
                    dataToSend += "Content-ID:<" + mmsFileName + ">\r\n";
                    dataToSend += "Content-Transfer-Encoding:binary\r\n\r\n";
                    byte[] dataToSendByte = encoding.GetBytes(dataToSend);
                    int dataToSendSize = dataToSendByte.Length + image.Length;
                    var tempMemoryStream = new MemoryStream(new byte[dataToSendSize], 0, dataToSendSize, true, true);
                    tempMemoryStream.Write(dataToSendByte, 0, dataToSendByte.Length);
                    tempMemoryStream.Write(image, 0, image.Length);
                    dataBytes = tempMemoryStream.GetBuffer();
                    if (count == 0)
                    {
                        totalDataBytes = dataBytes;
                    }
                    else
                    {
                        byte[] tempForTotalBytes = totalDataBytes;
                        var tempMemoryStreamAttach = this.JoinTwoByteArrays(tempForTotalBytes, dataBytes);
                        totalDataBytes = tempMemoryStreamAttach.GetBuffer();
                    }

                    count++;
                }

                byte[] byteLastBoundary = encoding.GetBytes("\r\n--" + boundaryToSend + "--\r\n");
                int totalDataSize = totalDataBytes.Length + byteLastBoundary.Length;
                var totalMemoryStream = new MemoryStream(new byte[totalDataSize], 0, totalDataSize, true, true);
                totalMemoryStream.Write(totalDataBytes, 0, totalDataBytes.Length);
                totalMemoryStream.Write(byteLastBoundary, 0, byteLastBoundary.Length);
                byte[] finalpostBytes = totalMemoryStream.GetBuffer();

                msgRequestObject.ContentLength = finalpostBytes.Length;

                postStream = msgRequestObject.GetRequestStream();
                postStream.Write(finalpostBytes, 0, finalpostBytes.Length);

                WebResponse mmsResponseObject1 = msgRequestObject.GetResponse();
                using (StreamReader sr = new StreamReader(mmsResponseObject1.GetResponseStream()))
                {
                    string mmsResponseData = sr.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    MsgResponseId deserializedJsonObj = (MsgResponseId)deserializeJsonObject.Deserialize(mmsResponseData, typeof(MsgResponseId));
                    sendMessageSuccessResponse = deserializedJsonObj.Id.ToString();
                    sr.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    sendMessageErrorResponse =  reader.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            sendMessageErrorResponse =   ex.ToString();
        }
        finally
        {
            if (null != postStream)
            {
                postStream.Close();
            }
        }
    }

    /// <summary>
    /// Sums up two byte arrays.
    /// </summary>
    /// <param name="firstByteArray">First byte array</param>
    /// <param name="secondByteArray">second byte array</param>
    /// <returns>The memorystream"/> summed memory stream</returns>
    private MemoryStream JoinTwoByteArrays(byte[] firstByteArray, byte[] secondByteArray)
    {
        int newSize = firstByteArray.Length + secondByteArray.Length;
        var totalMemoryStream = new MemoryStream(new byte[newSize], 0, newSize, true, true);
        totalMemoryStream.Write(firstByteArray, 0, firstByteArray.Length);
        totalMemoryStream.Write(secondByteArray, 0, secondByteArray.Length);
        return totalMemoryStream;
    }

    protected void getMessageHeaders_Click(object sender, EventArgs e)
    {
        this.ReadTokenSessionVariables();

        string tokentResult = this.IsTokenValid();

        if (tokentResult.CompareTo("INVALID_ACCESS_TOKEN") == 0)
        {
            SetRequestSessionVariables();
            Session["cs_rest_ServiceRequest"] = "getmessageheader";
            Session["cs_rest_appState"] = "GetToken";
            this.GetAuthCode();
        }
        else if (tokentResult.CompareTo("REFRESH_TOKEN") == 0)
        {
            if (this.GetAccessToken(AccessTokenType.Refresh_Token) == false)
            {
                sendMessageErrorResponse = "Failed to get Access token";
                this.ResetTokenSessionVariables();
                this.ResetTokenVariables();
                return;
            }
        }

        if (this.accessToken == null || this.accessToken.Length <= 0)
        {
            return;
        }
        GetMessageHeaders();
    }

    protected void GetMessageHeaders()
    {
        this.GetMessageHeaders(this.accessToken, this.endPoint, headerCountTextBox.Value, indexCursorTextBox.Value);
    }
    private void GetMessageHeaders(string acctoken, string epoint, string hCount, string iCursor)
    {
        try
        {
            HttpWebRequest mimRequestObject1;

            string getHeadersURL = string.Empty + endPoint + "/rest/1/MyMessages?HeaderCount=" + hCount;
            if (!string.IsNullOrEmpty(iCursor))
            {
                getHeadersURL += "&IndexCursor=" + iCursor;
            }
            mimRequestObject1 = (HttpWebRequest)WebRequest.Create(getHeadersURL);
            mimRequestObject1.Headers.Add("Authorization", "Bearer " + accessToken);
            mimRequestObject1.Method = "GET";
            mimRequestObject1.KeepAlive = true;

            WebResponse mimResponseObject1 = mimRequestObject1.GetResponse();
            using (StreamReader sr = new StreamReader(mimResponseObject1.GetResponseStream()))
            {
                string mimResponseData = sr.ReadToEnd();

                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                MIMResponse deserializedJsonObj = (MIMResponse)deserializeJsonObject.Deserialize(mimResponseData, typeof(MIMResponse));

                if (null != deserializedJsonObj)
                {
                    getHeadersSuccessResponse = "Success";
                    messageHeaderList = deserializedJsonObj.MessageHeadersList;
                }
                else
                {
                    getHeadersErrorResponse =  "No response from server";
                }

                sr.Close();
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
                getHeadersErrorResponse = errorResponse;
            }
            catch
            {
                errorResponse = "Unable to get response";
                getHeadersErrorResponse = errorResponse;
            }
        }
        catch (Exception ex)
        {
            getHeadersErrorResponse= ex.Message;
            return;
        }
    }


    /// <summary>
    /// Gets the message content for MMS messages based on Message ID and Part Number
    /// </summary>
    private void GetMessageContentByIDnPartNumber(string accTok, string endP, string messId, string partNum)
    {
        try
        {
            HttpWebRequest mimRequestObject1 = (HttpWebRequest)WebRequest.Create(string.Empty + endP + "/rest/1/MyMessages/" + messId + "/" + partNum);
            mimRequestObject1.Headers.Add("Authorization", "Bearer " + accTok);
            mimRequestObject1.Method = "GET";
            mimRequestObject1.KeepAlive = true;
            int offset = 0;
            getContentResponseObject = mimRequestObject1.GetResponse();
            int remaining = Convert.ToInt32(getContentResponseObject.ContentLength);
            using (var stream = getContentResponseObject.GetResponseStream())
            {
                receivedBytes = new byte[getContentResponseObject.ContentLength];
                while (remaining > 0)
                {
                    int read = stream.Read(receivedBytes, offset, remaining);
                    if (read <= 0)
                    {
                        getMessageErrorResponse = String.Format("End of stream reached with {0} bytes left to read", remaining);
                        return;
                    }

                    remaining -= read;
                    offset += read;
                }

                imageData = Regex.Split(getContentResponseObject.ContentType.ToLower(), ";");
                string[] ext = Regex.Split(imageData[0], "/");
                fetchedImage.Src = "data:" + imageData[0] + ";base64," + Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None);
                getMessageSuccessResponse = "Success";
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
                getMessageErrorResponse = errorResponse;
            }
            catch
            {
                errorResponse = "Unable to get response";
                getMessageErrorResponse = errorResponse;
            }
        }
        catch (Exception ex)
        {
            getMessageErrorResponse = ex.Message;
            return;
        }

    }

    private void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    protected void SetRequestSessionVariables()
    {
        Session["cs_rest_Address"] = Address.Value;
        Session["cs_rest_Message"] = message.Value;
        Session["cs_rest_Subject"] = subject.Value;
        Session["cs_rest_Group"] = groupCheckBox.Checked.ToString();
        Session["cs_rest_Attachments"] = attachment.Value;
        Session["cs_rest_GetHeadercount"] = headerCountTextBox.Value;
        Session["cs_rest_GetHeaderIndex"] = indexCursorTextBox.Value;
        Session["cs_rest_GetMessageId"] = MessageId.Value;
        Session["cs_rest_GetMessagePart"] = PartNumber.Value;
    }

    protected void ResetRequestSessionVariables()
    {
        Session["cs_rest_Address"] = null;
        Session["cs_rest_Message"] = null;
        Session["cs_rest_Subject"] = null;
        Session["cs_rest_Group"] = null;
        Session["cs_rest_Attachments"] = null;
        Session["cs_rest_GetHeadercount"] = null;
        Session["cs_rest_GetHeaderIndex"] = null;
        Session["cs_rest_GetMessageId"] = null;
        Session["cs_rest_GetMessagePart"] = null;
    }

    protected void RestoreRequestSessionVariables()
    {
        Address.Value = Session["cs_rest_Address"].ToString();
        message.Value = Session["cs_rest_Message"].ToString(); ;
        subject.Value = Session["cs_rest_Subject"].ToString() ;
        groupCheckBox.Checked = Convert.ToBoolean(Session["cs_rest_Group"].ToString());
        attachment.Value = Session["cs_rest_Attachments"].ToString();
        headerCountTextBox.Value = Session["cs_rest_GetHeadercount"].ToString();
        indexCursorTextBox.Value = Session["cs_rest_GetHeaderIndex"].ToString();
        MessageId.Value = Session["cs_rest_GetMessageId"].ToString();
        PartNumber.Value = Session["cs_rest_GetMessagePart"].ToString();
    }
   

    protected void Button1_Click(object sender, EventArgs e)
    {
        this.ReadTokenSessionVariables();

        string tokentResult = this.IsTokenValid();

        if (tokentResult.CompareTo("INVALID_ACCESS_TOKEN") == 0)
        {
            SetRequestSessionVariables();
            Session["cs_rest_ServiceRequest"] = "sendmessasge";
            Session["cs_rest_appState"] = "GetToken";
            this.GetAuthCode();
        }
        else if (tokentResult.CompareTo("REFRESH_TOKEN") == 0)
        {
            if (this.GetAccessToken(AccessTokenType.Refresh_Token) == false)
            {
                sendMessageErrorResponse =  "Failed to get Access token";
                this.ResetTokenSessionVariables();
                this.ResetTokenVariables();
                return;
            }
        }

        if (this.accessToken == null || this.accessToken.Length <= 0)
        {
            return;
        }
        this.SendMessageRequest();
    }

    protected void SendMessageRequest()
    {
        this.IsValidAddress();
        string attachFile = this.AttachmentFilesDir + attachment.Value.ToString();
        ArrayList attachmentList = new ArrayList();
        if (string.Compare(attachment.Value.ToString().ToLower(), "none") != 0)
            attachmentList.Add(attachFile);
        string accessToken = this.accessToken;
        string endpoint = this.endPoint;
        this.SendMessageRequest(accessToken, endpoint, subject.Value, message.Value, groupCheckBox.Checked.ToString().ToLower(),
                                      attachmentList);
    }
    
    protected void Button2_Click(object sender, EventArgs e)
    {
        this.ReadTokenSessionVariables();

        string tokentResult = this.IsTokenValid();

        if (tokentResult.CompareTo("INVALID_ACCESS_TOKEN") == 0)
        {
            SetRequestSessionVariables();
            Session["cs_rest_ServiceRequest"] = "getmessagecontent";
            Session["cs_rest_appState"] = "GetToken";
            this.GetAuthCode();
        }
        else if (tokentResult.CompareTo("REFRESH_TOKEN") == 0)
        {
            if (this.GetAccessToken(AccessTokenType.Refresh_Token) == false)
            {
                sendMessageErrorResponse = "Failed to get Access token";
                this.ResetTokenSessionVariables();
                this.ResetTokenVariables();
                return;
            }
        }

        if (this.accessToken == null || this.accessToken.Length <= 0)
        {
            return;
        }

        GetMessageContentByIDnPartNumber();
    }

    protected void GetMessageContentByIDnPartNumber()
    {
        this.GetMessageContentByIDnPartNumber(this.accessToken, this.endPoint, MessageId.Value, PartNumber.Value);
    }
    /// <summary>
    /// Validates the given addresses based on following conditions
    /// 1. Group messages should not allow short codes
    /// 2. Short codes should be 3-8 digits in length
    /// 3. Valid Email Address
    /// 4. Group message must contain more than one address
    /// 5. Valid Phone number
    /// </summary>
    /// <returns>true/false; true - if address specified met the validation criteria, else false</returns>
    private bool IsValidAddress()
    {
        string phonenumbers = string.Empty;

        bool isValid = true;
        if (string.IsNullOrEmpty(Address.Value))
        {
            sendMessageErrorResponse = "Address field cannot be blank.";
            return false;
        }

        string[] addresses = Address.Value.Trim().Split(',');

        if (addresses.Length > this.maxAddresses)
        {
            sendMessageErrorResponse = "Message cannot be delivered to more than 10 receipients.";
            return false;
        }

        if (groupCheckBox.Checked && addresses.Length < 2)
        {
            sendMessageErrorResponse = "Specify more than one address for Group message.";
            return false;
        }

        foreach (string addressraw in addresses)
        {
            string address = addressraw.Trim();
            if (string.IsNullOrEmpty(address))
            {
                break;
            }

            if (address.Length < 3)
            {
                sendMessageErrorResponse = "Invalid address specified.";
                return false;
            }

            // Verify if short codes are present in address
            if (!address.StartsWith("short") && (address.Length > 2 && address.Length < 9))
            {
                if (groupCheckBox.Checked)
                {
                    sendMessageErrorResponse =  "Group Message with short codes is not allowed.";
                    return false;
                }

                this.addressList.Add(address);
                this.phoneNumbersParameter = this.phoneNumbersParameter + "Addresses=short:" + Server.UrlEncode(address.ToString()) + "&";
            }

            if (address.StartsWith("short"))
            {
                if (groupCheckBox.Checked)
                {
                    sendMessageErrorResponse=  "Group Message with short codes is not allowed.";
                    return false;
                }

                System.Text.RegularExpressions.Regex regex = new Regex("^[0-9]*$");
                if (!regex.IsMatch(address.Substring(6)))
                {
                    sendMessageErrorResponse = "Invalid short code specified.";
                    return false;
                }

                this.addressList.Add(address);
                this.phoneNumbersParameter = this.phoneNumbersParameter + "Addresses=" + Server.UrlEncode(address.ToString()) + "&";
            }
            else if (address.Contains("@"))
            {
                isValid = this.IsValidEmail(address);
                if (isValid == false)
                {
                    sendMessageErrorResponse = "Specified Email Address is invalid.";
                    return false;
                }
                else
                {
                    this.addressList.Add(address);
                    this.phoneNumbersParameter = this.phoneNumbersParameter + "Addresses=" + Server.UrlEncode(address.ToString()) + "&";
                }
            }
            else
            {
                if (this.IsValidMISDN(address) == true)
                {
                    if (address.StartsWith("tel:"))
                    {
                        phonenumbers = address.Replace("-", string.Empty);
                        this.phoneNumbersParameter = this.phoneNumbersParameter + "Addresses=" + Server.UrlEncode(phonenumbers.ToString()) + "&";
                    }
                    else
                    {
                        phonenumbers = address.Replace("-", string.Empty);
                        this.phoneNumbersParameter = this.phoneNumbersParameter + "Addresses=" + Server.UrlEncode("tel:" + phonenumbers.ToString()) + "&";
                    }

                    this.addressList.Add(address);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Validate given string for MSISDN
    /// </summary>
    /// <param name="number">Phone number to be validated</param>
    /// <returns>true/false; true - if valid MSISDN, else false</returns>
    private bool IsValidMISDN(string number)
    {
        string smsAddressInput = number;
        long tryParseResult = 0;
        string smsAddressFormatted;
        string phoneStringPattern = "^\\d{3}-\\d{3}-\\d{4}$";
        if (Regex.IsMatch(smsAddressInput, phoneStringPattern))
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
        else if (smsAddressFormatted.Length == 15 && smsAddressFormatted.StartsWith("tel:1"))
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
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates given mail ID for standard mail format
    /// </summary>
    /// <param name="emailID">Mail Id to be validated</param>
    /// <returns> true/false; true - if valid email id, else false</returns>
    private bool IsValidEmail(string emailID)
    {
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
              @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(emailID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a given string for digits
    /// </summary>
    /// <param name="address">string to be validated</param>
    /// <returns>true/false; true - if passed string has all digits, else false</returns>
    private bool IsNumber(string address)
    {
        bool isValid = false;
        Regex regex = new Regex("^[0-9]*$");
        if (regex.IsMatch(address))
        {
            isValid = true;
        }

        return isValid;
    }
    #region Data Structures

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
    /// Response returned from MyMessages api
    /// </summary>
    public class MIMResponse
    {
        /// <summary>
        /// Gets or sets the value of message header list.
        /// </summary>
        public MessageHeaderList MessageHeadersList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Message Header List
    /// </summary>
    public class MessageHeaderList
    {
        /// <summary>
        /// Gets or sets the value of object containing a List of Messages Headers
        /// </summary>
        public List<Header> Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of a number representing the number of headers returned for this request.
        /// </summary>
        public int HeaderCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of a string which defines the start of the next block of messages for the current request.
        /// A value of zero (0) indicates the end of the block.
        /// </summary>
        public string IndexCursor
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Object containing a List of Messages Headers
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Gets or sets the value of Unique message identifier
        /// </summary>
        public string MessageId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of message sender
        /// </summary>
        public string From
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the addresses, whom the message need to be delivered. 
        /// If Group Message, this will contain multiple Addresses.
        /// </summary>
        public List<string> To
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value of message text
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value of message part descriptions
        /// </summary>
        public List<MMSContent> MmsContent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of date/time message received
        /// </summary>
        public DateTime Received
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether its a favourite or not
        /// </summary>
        public bool Favorite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether message is read or not
        /// </summary>
        public bool Read
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of type of message, TEXT or MMS
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of indicator, which indicates if message is Incoming or Outgoing “IN” or “OUT”
        /// </summary>
        public string Direction
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Message part descriptions
    /// </summary>
    public class MMSContent
    {
        /// <summary>
        /// Gets or sets the value of content name
        /// </summary>
        public string ContentName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of content type
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of part number
        /// </summary>
        public string PartNumber
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Response from IMMN api
    /// </summary>
    public class MsgResponseId
    {
        /// <summary>
        /// Gets or sets Message ID
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Access Token Types
    /// </summary>
    public enum AccessTokenType
    {
        /// <summary>
        /// Access Token Type is based on Authorization Code
        /// </summary>
        Authorization_Code,

        /// <summary>
        /// Access Token Type is based on Refresh Token
        /// </summary>
        Refresh_Token
    }
    #endregion
}