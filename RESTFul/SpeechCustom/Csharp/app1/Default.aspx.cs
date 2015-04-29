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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;

#endregion

/// <summary>
/// Speech application
/// </summary>
public partial class Speech_App1 : System.Web.UI.Page
{
    #region Class variables and Data structures
    /// <summary>
    /// Temporary variables for processing
    /// </summary>
    private string fqdn, accessTokenFilePath;

    /// <summary>
    /// Temporary variables for processing
    /// </summary>
    private string apiKey, secretKey, accessToken, scope, refreshToken, refreshTokenExpiryTime, accessTokenExpiryTime, bypassSSL;

    /// <summary>
    /// variable for having the posted file.
    /// </summary>
    private string SpeechFilesDir;

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;

    private string xgrammer = string.Empty;
    private string xdictionary = string.Empty;
    private string xgrammerContent = string.Empty;
    private string xdictionaryContent = string.Empty;


    public string speechErrorMessage = string.Empty;
    public string speechSuccessMessage = string.Empty;
    public SpeechResponse speechResponseData = null;

    /// <summary>
    /// Access Token Types
    /// </summary>
    public enum AccessType
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

    #region Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// This function reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        this.ReadConfigFile();
        this.SetContent();

    }

    /// <summary>
    /// Method that calls SpeechToText api when user clicked on submit button
    /// </summary>
    /// <param name="sender">sender that invoked this event</param>
    /// <param name="e">eventargs of the button</param>
    protected void BtnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            bool IsValid = true;

            IsValid = this.ReadAndGetAccessToken(ref speechErrorMessage);
            if (IsValid == false)
            {
                speechErrorMessage = "Unable to get access token";
                return;
            }
            var speechFile = this.SpeechFilesDir + audio_file.SelectedValue.ToString();
            this.ConvertToSpeech(this.fqdn + "/speech/v3/speechToTextCustom",
                this.accessToken, SpeechContext.SelectedValue.ToString(), x_arg.Text, speechFile);
        }
        catch (Exception ex)
        {
            speechErrorMessage = ex.Message;
            return;
        }
    }


    private void SetContent()
    {
        StreamReader streamReader = new StreamReader(this.xdictionary);
        xdictionaryContent = streamReader.ReadToEnd();
        mimeData.Text = "x-dictionary:" + Environment.NewLine + xdictionaryContent;
        x_dictionary.Text = xdictionaryContent;
        StreamReader streamReader1 = new StreamReader(this.xgrammer);
        xgrammerContent = streamReader1.ReadToEnd();
        mimeData.Text = mimeData.Text + Environment.NewLine + "x-grammar:" + Environment.NewLine + xgrammerContent;
        x_grammer.Text = xgrammerContent;
        streamReader.Close();
        streamReader1.Close();
    }

    #endregion

    #region Access Token Related Functions

    /// <summary>
    /// Read parameters from configuraton file
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\SpeechApp1AccessToken.txt";
        }

        this.fqdn = ConfigurationManager.AppSettings["FQDN"];
        if (string.IsNullOrEmpty(this.fqdn))
        {
            speechErrorMessage = "FQDN is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            speechErrorMessage = "api_key is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            speechErrorMessage = "secret_key is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "SPEECH";
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
        /*if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceLink"]))
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
        }*/

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SpeechFilesDir"]))
        {
            this.SpeechFilesDir = Request.MapPath(ConfigurationManager.AppSettings["SpeechFilesDir"]);
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["xgrammer"]))
        {
            this.xgrammer = Request.MapPath(ConfigurationManager.AppSettings["xgrammer"]);
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["xdictionary"]))
        {
            this.xdictionary = Request.MapPath(ConfigurationManager.AppSettings["xdictionary"]);
        }

        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SpeechContext"]))
            {
                string[] speechContexts = ConfigurationManager.AppSettings["SpeechContext"].ToString().Split(';');
                foreach (string speechContext in speechContexts)
                {
                    SpeechContext.Items.Add(speechContext);
                }
                if (speechContexts.Length > 0)
                    SpeechContext.Items[0].Selected = true;
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NameParameters"]))
            {
                string[] nameParameters = ConfigurationManager.AppSettings["NameParameters"].ToString().Split(';');
                foreach (string nameParameter in nameParameters)
                {
                    nameParam.Items.Add(nameParameter);
                }
                if (nameParameters.Length > 0)
                    nameParam.Items[0].Selected = true;
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["X-ArgGeneric"]))
            {
                x_arg.Text = ConfigurationManager.AppSettings["X-ArgGeneric"];
            }
            if (!string.IsNullOrEmpty(SpeechFilesDir))
            {
                string[] filePaths = Directory.GetFiles(this.SpeechFilesDir);
                foreach (string filePath in filePaths)
                {
                    audio_file.Items.Add(Path.GetFileName(filePath));
                }
                if (filePaths.Length > 0)
                    audio_file.Items[0].Selected = true;
            }
        }

        return true;
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
    /// This function validates the expiry of the access token and refresh token.
    /// function compares the current time with the refresh token taken time, if current time is greater then returns INVALID_REFRESH_TOKEN
    /// function compares the difference of last access token taken time and the current time with the expiry seconds, if its more, returns INVALID_ACCESS_TOKEN    
    /// otherwise returns VALID_ACCESS_TOKEN
    /// </summary>
    /// <returns>string, which specifies the token validity</returns>
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

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.fqdn + "/oauth/v4/token");
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

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.fqdn + "/oauth/v4/token");
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
    /// Neglect the ssl handshake error with authentication server 
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

    #endregion

    #region Speech Service Functions

    /// <summary>
    /// Content type based on the file extension.
    /// </summary>
    /// <param name="extension">file extension</param>
    /// <returns>the Content type mapped to the extension"/> summed memory stream</returns>
    private string MapContentTypeFromExtension(string extension)
    {
        Dictionary<string, string> extensionToContentTypeMapping = new Dictionary<string, string>()
            {
                { ".amr", "audio/amr" }, { ".wav", "audio/wav" }, {".awb", "audio/amr-wb"}, {".spx", "audio/x-speex"}
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
    /// This function invokes api SpeechToText to convert the given wav amr file and displays the result.
    /// </summary>
    private void ConvertToSpeech(string parEndPoint, string parAccessToken, string parXspeechContext, string parXArgs, string parSpeechFilePath)
    {
        Stream postStream = null;
        FileStream audioFileStream = null;
        audioFileStream = new FileStream(parSpeechFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader reader = new BinaryReader(audioFileStream);
        try
        {

            byte[] binaryData = reader.ReadBytes((int)audioFileStream.Length);
            if (null != binaryData)
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(string.Empty + parEndPoint);
                httpRequest.Headers.Add("Authorization", "Bearer " + parAccessToken);
                httpRequest.Headers.Add("X-SpeechContext", parXspeechContext);
                httpRequest.Headers.Add("Content-Language", "en-us");
                httpRequest.ContentType = "multipart/x-srgs-audio; " + "boundary=" + boundary;

                if (!string.IsNullOrEmpty(parXArgs))
                {
                    httpRequest.Headers.Add("X-Arg", parXArgs);
                }
                string filenameArgument = "filename";
                if (!string.IsNullOrEmpty(SpeechContext.SelectedValue))
                {
                    if (string.Compare("GenericHints", SpeechContext.SelectedValue) == 0)
                    {
                        filenameArgument = nameParam.SelectedValue.ToString();
                    }
                }

                string contentType = this.MapContentTypeFromExtension(Path.GetExtension(parSpeechFilePath));

                string data = string.Empty;


                data += "--" +boundary + "\r\n" + "Content-Disposition: form-data; name=\"x-dictionary\"; " + filenameArgument + "=\"speech_alpha.pls\"\r\nContent-Type: application/pls+xml\r\n";

                data += "\r\n" + xdictionaryContent + "\r\n\r\n\r\n";

                data += "--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"x-grammar\"";

                //data += "filename=\"prefix.srgs\" ";

                data += "\r\nContent-Type: application/srgs+xml \r\n" + "\r\n" + xgrammerContent + "\r\n\r\n\r\n" + "--" + boundary + "\r\n";

                data += "Content-Disposition: form-data; name=\"x-voice\"; " + filenameArgument + "=\"" + audio_file.SelectedValue + "\"";
                data += "\r\nContent-Type: " + contentType + "\r\n\r\n";
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] firstPart = encoding.GetBytes(data);
                int newSize = firstPart.Length + binaryData.Length;

                var memoryStream = new MemoryStream(new byte[newSize], 0, newSize, true, true);
                memoryStream.Write(firstPart, 0, firstPart.Length);
                memoryStream.Write(binaryData, 0, binaryData.Length);

                byte[] postBytes = memoryStream.GetBuffer();

                byte[] byteLastBoundary = encoding.GetBytes("\r\n\r\n" + "--" + boundary + "--");
                int totalSize = postBytes.Length + byteLastBoundary.Length;

                var totalMS = new MemoryStream(new byte[totalSize], 0, totalSize, true, true);
                totalMS.Write(postBytes, 0, postBytes.Length);
                totalMS.Write(byteLastBoundary, 0, byteLastBoundary.Length);

                byte[] finalpostBytes = totalMS.GetBuffer();

                httpRequest.ContentLength = totalMS.Length;
                //httpRequest.ContentType = contentType;
                httpRequest.Accept = "application/json";
                httpRequest.Method = "POST";
                httpRequest.KeepAlive = true;
                postStream = httpRequest.GetRequestStream();
                postStream.Write(finalpostBytes, 0, finalpostBytes.Length);
                postStream.Close();

                HttpWebResponse speechResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(speechResponse.GetResponseStream()))
                {
                    string speechRequestResponse = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(speechRequestResponse))
                    {
                        JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                        SpeechResponse deserializedJsonObj = (SpeechResponse)deserializeJsonObject.Deserialize(speechRequestResponse, typeof(SpeechResponse));
                        if (null != deserializedJsonObj)
                        {
                            speechResponseData = new SpeechResponse();
                            speechResponseData = deserializedJsonObj;
                            speechSuccessMessage = "true";
                            //speechErrorMessage = speechRequestResponse;
                        }
                        else
                        {
                            speechErrorMessage = "Empty speech to text response";
                        }
                    }
                    else
                    {
                        speechErrorMessage = "Empty speech to text response";
                    }

                    streamReader.Close();
                }
            }
            else
            {
                speechErrorMessage = "Empty speech to text response";
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

            speechErrorMessage = errorResponse;
        }
        catch (Exception ex)
        {
            speechErrorMessage = ex.ToString();
        }
        finally
        {
            reader.Close();
            audioFileStream.Close();
            if (null != postStream)
            {
                postStream.Close();
            }
        }
    }

    #endregion
    /*protected void SpeechContext_Load(object sender, EventArgs e)
    {

    }*/
}

#region Access Token and Speech Response Data Structures

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
/// Speech Response to an audio file
/// </summary>
public class SpeechResponse
{
    /// <summary>
    /// Gets or sets the Recognition value returned by api
    /// </summary>
    public Recognition Recognition { get; set; }
}

/// <summary>
/// Recognition returned by the server for Speech to text request.
/// </summary>
public class Recognition
{
    /// <summary>
    /// Gets or sets a unique string that identifies this particular transaction.
    /// </summary>
    public string ResponseId { get; set; }

    /// <summary>
    /// Gets or sets NBest Complex structure that holds the results of the transcription. Supports multiple transcriptions.
    /// </summary>
    public List<NBest> NBest { get; set; }

    /// <summary>
    /// Gets or sets the Status of the transcription.
    /// </summary>
    public string Status { get; set; }

}
public class nluHypothesis
{
    public outComposite[] OutComposite { get; set; }
}
public class outComposite
{
    public string Grammar { get; set; }
    public string Out { get; set; }
}

/// <summary>
/// Complex structure that holds the results of the transcription. Supports multiple transcriptions.
/// </summary>
public class NBest
{
    /// <summary>
    /// Gets or sets the transcription of the audio. 
    /// </summary>
    public string Hypothesis { get; set; }

    /// <summary>
    /// Gets or sets the language used to decode the Hypothesis. 
    /// Represented using the two-letter ISO 639 language code, hyphen, two-letter ISO 3166 country code in lower case, e.g. “en-us”.
    /// </summary>
    public string LanguageId { get; set; }

    /// <summary>
    /// Gets or sets the confidence value of the Hypothesis, a value between 0.0 and 1.0 inclusive.
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Gets or sets a machine-readable string indicating an assessment of utterance/result quality and the recommended treatment of the Hypothesis. 
    /// The assessment reflects a confidence region based on prior experience with similar results. 
    /// accept - the hypothesis value has acceptable confidence
    /// confirm - the hypothesis should be independently confirmed due to lower confidence
    /// reject - the hypothesis should be rejected due to low confidence
    /// </summary>
    public string Grade { get; set; }

    /// <summary>
    /// Gets or sets a text string prepared according to the output domain of the application package. 
    /// The string will generally be a formatted version of the hypothesis, but the words may have been altered through 
    /// insertions/deletions/substitutions to make the result more readable or usable for the client.  
    /// </summary>
    public string ResultText { get; set; }

    /// <summary>
    /// Gets or sets the words of the Hypothesis split into separate strings.  
    /// May omit some of the words of the Hypothesis string, and can be empty.  Never contains words not in hypothesis string.  
    /// </summary>
    public List<string> Words { get; set; }

    /// <summary>
    /// Gets or sets the confidence scores for each of the strings in the words array.  Each value ranges from 0.0 to 1.0 inclusive.
    /// </summary>
    public List<double> WordScores { get; set; }

    public nluHypothesis NluHypothesis { get; set; }
}
#endregion