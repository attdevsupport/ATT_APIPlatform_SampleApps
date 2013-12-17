// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
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

/// <summary>
/// TL_App1 class
/// </summary>
public partial class TL_App1 : System.Web.UI.Page
{
    #region Local variables

    /// <summary>
    /// Gets or sets the value of endPoint
    /// </summary>
    private string endPoint;

    /// <summary>
    /// Access Token Variables
    /// </summary>
    private string apiKey, secretKey, accessToken, authorizeRedirectUri, scope, refreshToken, accessTokenExpiryTime, refreshTokenExpiryTime, bypassSSL;

    /// <summary>
    /// Gets or sets the value of authCode
    /// </summary>
    private string authCode;

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;

    /// <summary>
    /// Gets or sets the Status Table
    /// </summary>
    private Table getStatusTable;

    public string getLocationSuccess = string.Empty;
    public string getLocationError = string.Empty;
    public TLResponse getLocationResponse;
    public string responseTime = string.Empty;

#endregion

    #region SSL Handshake Error
    
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

    #endregion

    #region Events

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
            bool ableToRead = this.ReadConfigFile();
            if (!ableToRead)
            {
                return;
            }

            if (!IsPostBack && (Session["CsharpRestTLSelectedValues"] == null))
            {
                RA2.Checked = true;
                AA3.Checked = true;
                DT2.Checked = true;
            }
            if ((Session["tl_session_appState"] == "GetToken") && (Request["Code"] != null))
            {
                this.authCode = Request["code"];
                bool ableToGetToken = this.GetAccessToken(AccessTokenType.Authorization_Code);
                FetchSelectedValuesFromSessionVariables();
                Session["CsharpRestTLSelectedValues"] = null;
                if (ableToGetToken)
                {
                    
                    this.GetDeviceLocation();
                }
                else
                {
                    getLocationError = "Failed to get Access token";
                    this.ResetTokenSessionVariables();
                    this.ResetTokenVariables();
                }
            }
        }
        catch (Exception ex)
        {
            getLocationError = ex.ToString();
        }
    }

    /// <summary>
    /// Event that will be triggered when the user clicks on GetPhoneLocation button
    /// This method calls GetDeviceLocation Api
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void GetDeviceLocation_Click(object sender, EventArgs e)
    {
        try
        {
            bool ableToGetAccessToken = this.ReadAndGetAccessToken();
            if (ableToGetAccessToken)
            {
                this.GetDeviceLocation();
            }
            else
            {
                getLocationError = "Unable to get access token";
            }
        }
        catch (Exception ex)
        {
            getLocationError = ex.Message;
        }
    }

    #endregion

    #region session and radio buttons

    public void FetchSelectedValuesFromSessionVariables()
    {
        string sessionValue = Session["CsharpRestTLSelectedValues"].ToString();

        string[] selectedValues = sessionValue.Split(';');
        if ( !string.IsNullOrEmpty(selectedValues[0]))
        {
            if (selectedValues[0].CompareTo("AA1") == 0)
                AA1.Checked = true;
            else if (selectedValues[0].CompareTo("AA2") == 0)
                AA2.Checked = true;
            else if (selectedValues[0].CompareTo("AA3") == 0)
                AA3.Checked = true;
        }

        if (!string.IsNullOrEmpty(selectedValues[1]))
        {
            if (selectedValues[1].CompareTo("RA1") == 0)
                RA1.Checked = true;
            else if (selectedValues[1].CompareTo("RA2") == 0)
                RA2.Checked = true;
            else if (selectedValues[1].CompareTo("RA3") == 0)
                RA3.Checked = true;
        }

        if (!string.IsNullOrEmpty(selectedValues[2]))
        {
            if (selectedValues[2].CompareTo("DT1") == 0)
                DT1.Checked = true;
            else if (selectedValues[2].CompareTo("DT2") == 0)
                DT2.Checked = true;
            else if (selectedValues[2].CompareTo("DT3") == 0)
                DT3.Checked = true;
        }
    }
    public void StoreSelectedValuesToSessionVariables()
    {
        string selectedValues = string.Empty;
        if (AA1.Checked)
            selectedValues = selectedValues + "AA1";
        else if (AA2.Checked)
            selectedValues = selectedValues + "AA2";
        else if (AA3.Checked)
            selectedValues = selectedValues + "AA3";
        selectedValues = selectedValues + ";";
        if (RA1.Checked )
            selectedValues = selectedValues + "RA1";
        else if (RA2.Checked)
            selectedValues = selectedValues + "RA2";
        else if (RA3.Checked)
            selectedValues = selectedValues + "RA3";
        selectedValues = selectedValues + ";";
        if (DT1.Checked )
            selectedValues = selectedValues + "DT1";
        else if (DT2.Checked)
            selectedValues = selectedValues + "DT2";
        else if (DT3.Checked)
            selectedValues = selectedValues + "DT3";

        Session["CsharpRestTLSelectedValues"] = selectedValues;
    }

    public int getAcceptableAccuracy()
    {
        if (AA1.Checked)
            return Convert.ToInt32(AA1.Value);
        else if (AA2.Checked)
            return  Convert.ToInt32(AA2.Value);
        return  Convert.ToInt32(AA3.Value);
    }
    public int getRequestedAccuracry()
    {
        if (RA1.Checked)
            return Convert.ToInt32(RA1.Value);
        else if (RA2.Checked)
            return Convert.ToInt32(RA2.Value);
        return Convert.ToInt32(RA3.Value);
    }
    public string getDelayTolerance()
    {
        if (DT1.Checked)
            return DT1.Value;
        else if (DT2.Checked)
            return DT2.Value;
        return DT3.Value;
    }
    #endregion

    #region API Invokation

    /// <summary>
    /// This method invokes Device Location API and displays the location
    /// </summary>
    private void GetDeviceLocation()
    {
        try
        {

            int requestedAccuracyVal, acceptableAccuracyVal;
            string toleranceVal;

            acceptableAccuracyVal = getAcceptableAccuracy();
            requestedAccuracyVal = getRequestedAccuracry();
            toleranceVal = getDelayTolerance();

            string strResult;

            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/2/devices/location?requestedAccuracy=" + requestedAccuracyVal + "&acceptableAccuracy=" + acceptableAccuracyVal + "&tolerance=" + toleranceVal);
            webRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
            webRequest.Method = "GET";
            
            DateTime msgSentTime = DateTime.UtcNow.ToLocalTime();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            DateTime msgReceivedTime = DateTime.UtcNow.ToLocalTime();
            TimeSpan tokenSpan = msgReceivedTime.Subtract(msgSentTime);
            
            using (StreamReader responseStream = new StreamReader(webResponse.GetResponseStream()))
            {
                strResult = responseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                getLocationResponse = (TLResponse)deserializeJsonObject.Deserialize(strResult, typeof(TLResponse));
                responseTime = tokenSpan.Seconds.ToString();
                getLocationSuccess = "Success";
                responseStream.Close();
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    getLocationError = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }
        }
        catch (Exception ex)
        {
            getLocationError =  ex.Message;
        }
    }

    #endregion

    #region Access Token Methods

    /// <summary>
    /// Reads from session variables and gets access token
    /// </summary>
    /// <returns>true/false; true on successfully getting access token, else false</returns>
    private bool ReadAndGetAccessToken()
    {
        this.ReadTokenSessionVariables();

        string tokentResult = this.IsTokenValid();
        if (tokentResult.Equals("INVALID_ACCESS_TOKEN"))
        {
            StoreSelectedValuesToSessionVariables();
            Session["tl_session_appState"] = "GetToken";
            this.GetAuthCode();
        }
        else if (tokentResult.Equals("REFRESH_TOKEN"))
        {
            bool ableToGetToken = this.GetAccessToken(AccessTokenType.Refresh_Token);
            if (ableToGetToken == false)
            {
                getLocationError = "Failed to get Access token";
                this.ResetTokenSessionVariables();
                this.ResetTokenVariables();
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// This function reads access token related session variables to local variables 
    /// </summary>
    private void ReadTokenSessionVariables()
    {
        this.accessToken = string.Empty;
        if (Session["tl_session_access_token"] != null)
        {
            this.accessToken = Session["tl_session_access_token"].ToString();
        }

        this.refreshToken = null;
        if (Session["tl_session_refresh_token"] != null)
        {
            this.refreshToken = Session["tl_session_refresh_token"].ToString();
        }

        this.accessTokenExpiryTime = null;
        if (Session["tl_session_accessTokenExpiryTime"] != null)
        {
            this.accessTokenExpiryTime = Session["tl_session_accessTokenExpiryTime"].ToString();
        }

        this.refreshTokenExpiryTime = null;
        if (Session["tl_session_refreshTokenExpiryTime"] != null)
        {
            this.refreshTokenExpiryTime = Session["tl_session_refreshTokenExpiryTime"].ToString();
        }        
    }

    /// <summary>
    /// This function resets access token related session variable to null 
    /// </summary>
    private void ResetTokenSessionVariables()
    {
        Session["tl_session_access_token"] = null;
        Session["tl_session_refresh_token"] = null;
        Session["tl_session_accessTokenExpiryTime"] = null;
        Session["tl_session_refreshTokenExpiryTime"] = null;
    }

    /// <summary>
    /// This function resets access token related  variable to null 
    /// </summary>
    private void ResetTokenVariables()
    {
        this.accessToken = null;
        this.refreshToken = null;
        this.accessTokenExpiryTime = null;
        this.refreshTokenExpiryTime = null;
    }

    /// <summary>
    /// This function validates access token related variables and returns VALID_ACCESS_TOKEN if its valid
    /// otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    /// return REFRESH_TOKEN, if access token in expired and refresh token is valid 
    /// </summary>
    /// <returns>string variable containing valid/invalid access/refresh token</returns>
    private string IsTokenValid()
    {
        if (Session["tl_session_access_token"] == null)
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
    /// Redirects to authentication server to get the access code
    /// </summary>
    private void GetAuthCode()
    {
        Response.Redirect(string.Empty + this.endPoint + "/oauth/authorize?scope=" + this.scope + "&client_id=" + this.apiKey + "&redirect_url=" + this.authorizeRedirectUri);
    }

    /// <summary>
    /// Get access token based on the type parameter type values.
    /// </summary>
    /// <param name="type">If type value is Authorization_code, access token is fetch for authorization code flow
    /// If type value is Refresh_Token, access token is fetch for authorization code floww based on the exisiting refresh token</param>
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
                oauthParameters = "client_id=" + this.apiKey.ToString() + "&client_secret=" + this.secretKey + "&code=" + this.authCode + "&grant_type=authorization_code&scope=" + this.scope;
            }
            else if (type == AccessTokenType.Refresh_Token)
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
                    this.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString();

                    DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                    if (deserializedJsonObj.expires_in.Equals("0"))
                    {
                        int defaultAccessTokenExpiresIn = 100; // In Years
                        this.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToString();
                    }

                    this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();                    
                    
                    Session["tl_session_access_token"] = this.accessToken;
                    Session["tl_session_refresh_token"] = this.refreshToken;
                    Session["tl_session_accessTokenExpiryTime"] = this.accessTokenExpiryTime;
                    Session["tl_session_refreshTokenExpiryTime"] = this.refreshTokenExpiryTime;
                    Session["tl_session_appState"] = "TokenReceived";

                    accessTokenResponseStream.Close();
                    return true;
                }
                else
                {
                    getLocationError =  "Auth server returned null access token";
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    getLocationError = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }
        }
        catch (Exception ex)
        {
            getLocationError = ex.Message;
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

    /// <summary>
    /// Read parameters from configuraton file
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            getLocationError =  "endPoint is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            getLocationError = "api_key is not defined in configuration file";
            return false; 
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            getLocationError = "secret_key is not defined in configuration file";
            return false;
        }

        this.authorizeRedirectUri = ConfigurationManager.AppSettings["authorize_redirect_uri"];
        if (string.IsNullOrEmpty(this.authorizeRedirectUri))
        {
            getLocationError =  "authorize_redirect_uri is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "TL";
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
        return true;
    }

    #endregion

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
/// Terminal Location Response object
/// </summary>
public class TLResponse
{
    /// <summary>
    /// Gets or sets the value of accuracy - This is the target MSISDN that was used in the Device Location request
    /// </summary>
    public string accuracy { get; set; }

    /// <summary>
    /// Gets or sets the value of latitude - The current latitude of the device's geo-position.
    /// </summary>
    public string latitude { get; set; }

    /// <summary>
    /// Gets or sets the value of longitude - The current longitude of the device geo-position.
    /// </summary>
    public string longitude { get; set; }

    /// <summary>
    /// Gets or sets the value of timestamp - Timestamp of the location data.
    /// </summary>
    public string timestamp { get; set; }
}
#endregion