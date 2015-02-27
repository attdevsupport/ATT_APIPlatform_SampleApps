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

#region Application References

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using System.Web;
using System.Collections.Generic;
using System.Collections;

#endregion

/// <summary>
/// Access Token Types
/// </summary>
public enum AccessTokenType
{
    /// <summary>
    /// Access Token Type is based on Authorize Credential Mode
    /// </summary>
    Authorize_Credential,

    /// <summary>
    /// Access Token Type is based on Refresh Token
    /// </summary>
    Refresh_Token
}

/// <summary>
/// Device Capabilities Application class.
/// This application will allow an user to get the device capabilities.
/// </summary>
public partial class DC_App1 : System.Web.UI.Page
{
    #region Application Instance Variables

    /// <summary>
    /// Instance variables
    /// </summary>
    private string endPoint, apiKey, secretKey, authorizeRedirectUri, authCode, scope, bypassSSL;

    /// <summary>
    /// OAuth access token
    /// </summary>
    private string accessToken;

    public int indentLevel = 0;
    public string tbOutput = string.Empty;
    public string getDCSuccess = string.Empty;
    public string getDCError = string.Empty;
    public Dictionary<string, string> getDCResponse = new Dictionary<string, string>();

    /// <summary>
    /// OAuth refresh token
    /// </summary>
    private string refreshToken;

    /// <summary>
    /// Expirytimes of refresh and access tokens
    /// </summary>
    private string refreshTokenExpiryTime, accessTokenExpiryTime;

    #endregion

    #region Neglect SSL error
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

    #region Application Events

    /// <summary>
    /// This method will be executed upon loading of the page.
    /// Reads the config file and assigns to local variables.
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();

            bool ableToReadConfig = this.ReadConfigFile();
            if (ableToReadConfig == false)
            {
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["error"] != null && Session["cs_dc_state"]!=null)
                {
                    Session["cs_dc_state"] = null;
                    string errorString = Request.Url.Query.Remove(0,1);
                    getDCError  = HttpUtility.UrlDecode(errorString);
                    return;
                }
                if (Session["Cs_DC_App1_AccessToken"] == null)
                {
                    //Retrive the query string 'code' from redirect response of AT&T authorization endpoint
                    // get the AccessToken and device info
                    if (!string.IsNullOrEmpty(Convert.ToString(Request["code"])))
                    {
                        authCode = Request["code"].ToString();
                        if (GetAccessToken(AccessTokenType.Authorize_Credential))
                        {
                            GetDeviceInfo();
                        }
                        else
                        {
                            getDCError = "Failed to get Access token";
                        }

                    }
                    else
                    {
                        Session["Cs_DC_App1_AccessToken"] = null;
                        GetAuthCode();
                    }
                }
                else
                {
                    GetDeviceInfo();
                }
            }
            else
            {
                Session["Cs_DC_App1_AccessToken"] = null;
                GetAuthCode();
            }
        }
        catch (Exception ex)
        {
            getDCError = ex.ToString();
        }
    }

#endregion

    #region Application specific methods

    /// <summary>
    /// Read config file and assign to local variables
    /// </summary>
    /// <remarks>
    /// <para>Validates if the values are configured in web.config file and displays a warning message if not configured.</para>
    /// </remarks>
    /// <returns>true/false; true if able to read and assign; else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            getDCError = "endPoint is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            getDCError = "apiKey is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            getDCError = "secretKey is not defined in configuration file";
            return false;
        }

        this.authorizeRedirectUri = ConfigurationManager.AppSettings["authorizeRedirectUri"];
        if (string.IsNullOrEmpty(this.authorizeRedirectUri))
        {
            getDCError = "authorizeRedirectUri is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "DC";
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

    /// <summary>
    /// This method gets access token based on either client credentials mode or refresh token.
    /// </summary>
    /// <param name="type">AccessTokenType; either Client_Credential or Refresh_Token</param>
    /// <returns>true/false; true if able to get access token, else false</returns>
    private bool GetAccessToken(AccessTokenType type)
    {
        Stream postStream = null;
        StreamWriter streamWriter = null;
        FileStream fileStream = null;
        try
        {
            DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

            WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/v4/token");
            accessTokenRequest.Method = "POST";

            string oauthParameters = string.Empty;
            if (type == AccessTokenType.Authorize_Credential)
            {
                oauthParameters = "client_id=" + this.apiKey.ToString() + "&client_secret=" + this.secretKey + "&code=" + this.authCode + "&grant_type=authorization_code&scope=" + this.scope;
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
                string jsonAccessToken = accessTokenResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(jsonAccessToken, typeof(AccessTokenResponse));
                DateTime accessTokenExpireTime = DateTime.Now;
                Session["Cs_DC_App1_AccessToken"] = deserializedJsonObj.access_token.ToString();
                double expiryMilliSeconds = Convert.ToDouble(deserializedJsonObj.expires_in);
                Session["refresh_token"] = deserializedJsonObj.refresh_token.ToString();
                if (expiryMilliSeconds == 0)
                {
                    Session["accessTokenExpireTime"] = accessTokenExpireTime.AddYears(100);
                }
                else
                {
                    Session["accessTokenExpireTime"] = accessTokenExpireTime.AddMilliseconds(expiryMilliSeconds);
                }
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

            getDCError = errorResponse + Environment.NewLine + we.Message;
        }
        catch (Exception ex)
        {
            getDCError = ex.Message;
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
    /// Redirects to authentication server to get the access code
    /// </summary>
    private void GetAuthCode()
    {
        Session["cs_dc_state"] = "FetchAuthCode";
        Response.Redirect(this.endPoint + "/oauth/v4/authorize?scope=" + this.scope + "&client_id=" + this.apiKey + "&redirect_url=" + this.authorizeRedirectUri);
    }      
      
     /// <summary>
    /// This method invokes DeviceInfo API of AT&amp;T platform to get the device information.
     /// </summary>
    private void GetDeviceInfo()
    {
        try
        {
            HttpWebRequest deviceInfoRequestObject = (HttpWebRequest)System.Net.WebRequest.Create(this.endPoint + "/rest/2/Devices/Info");
            deviceInfoRequestObject.Method = "GET";
            deviceInfoRequestObject.Headers.Add("Authorization", "Bearer " + Session["Cs_DC_App1_AccessToken"]);

            HttpWebResponse deviceInfoResponse = (HttpWebResponse)deviceInfoRequestObject.GetResponse();
            using (StreamReader accessTokenResponseStream = new StreamReader(deviceInfoResponse.GetResponseStream()))
            {
                string deviceInfo_jsonObj = accessTokenResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(deviceInfo_jsonObj);
                DisplayDictionary(dict);
                getDCSuccess = "success";
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

            getDCError = errorResponse + Environment.NewLine + we.Message;
        }
        catch (Exception ex)
        {
            getDCError = ex.Message;
        }
    }

    #endregion
    private void DisplayDictionary(Dictionary<string,object> dict)
    {
        foreach (string strKey in dict.Keys)
        {
            string strOutput = "".PadLeft(indentLevel * 8) + strKey + ":";

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
                        strOutput = ((string)oChild);
                        //tbOutput = tbOutput  + strOutput + ",";
                    }
                    else if (oChild is Dictionary<string, object>)
                    {
                        DisplayDictionary((Dictionary<string, object>)oChild);
                        //tbOutput = tbOutput  + "\r\n";
                    }
                }
            }
            else
            {
                getDCResponse.Add(strKey.ToString(),o.ToString());
                //strOutput = o.ToString();
            }
        }
    }
}

#region Access Token Data Structure

/// <summary>
/// AccessTokenResponse Object, returned upon calling get auth token api.
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

#region Device Capabilities Response Structure

/// <summary>
/// This object is used to return the Device Capabilities details from the network for a particular mobile terminal.
/// </summary>
public class DeviceCapabilities
{
    /// <summary>
    /// Gets or sets the value of DeviceInfo object.
    /// This Root Object will return two nested objects i-e DeviceId and Capabilities.
    /// </summary>
    public DeviceInfo DeviceInfo { get; set; }
}

/// <summary>
/// Encapsulates the device capabilities response from API gateway.
/// </summary>
public class DeviceInfo
{
    /// <summary>
    /// Gets or sets the value of DeviceId.        
    /// </summary>
    public DeviceId DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the value of Capabilities object.
    /// </summary>
    public Capabilities Capabilities { get; set; }
}

/// <summary>
/// Encapsulates the first 8 digits of the IMEI of the mobile device.
/// </summary>
public class DeviceId
{
    /// <summary>
    /// Gets or sets the value of TypeAllocationCode.
    /// This will return ONLY the first 8 digits of the International Mobile Equipment Identity of the mobile device in question.
    /// </summary>
    public string TypeAllocationCode { get; set; }
}

/// <summary>
/// Capabilities object is returned as part of Device #Capabilities AT&amp;T's API.
/// The Device Capabilities attributes, which include Name, Vendor, Model, FirmwareVersion,
/// UaProf, MmsCapable, AssistedGps, Location Technology, Device Browser and WAP Push Capable to
/// allow developers to deliver applications that are tailored to a specific customer device.
/// </summary>
public class Capabilities
{
    /// <summary>
    /// Gets or sets the value of Name.
    /// AT&amp;T's abbreviated code for the mobile device manufacturer and model number. These may or may not correspond to the device manufacturer's name and model number provided in the href.        
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of Vendor.
    /// AT&amp;T's abbreviated code for the mobile device manufacturer.
    /// </summary>
    public string Vendor { get; set; }

    /// <summary>
    /// Gets or sets the value of Model.
    /// AT&amp;T's model number for the mobile device.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Gets or sets the value of FirmwareVersion.
    /// AT&amp;T's shall map the AT&amp;T's specific device firmware release number (if available) to this parameter. 
    /// This may or may not correspond to the mobile device manufacturer's firmware release numbers.
    /// </summary>
    public string FirmwareVersion { get; set; }

    /// <summary>
    /// Gets or sets the value of UaProf.
    /// Contains a URL to the device manufacturer website which may provide specific 
    /// capability details for the mobile device in the request. 
    /// </summary>
    public string UaProf { get; set; }

    /// <summary>
    /// Gets or sets the value of MmsCapable.
    /// A value that indicates whether the device is MMS capable “Y” or “N”.
    /// </summary>
    public string MmsCapable { get; set; }

    /// <summary>
    /// Gets or sets the value of AssistedGps.
    /// A value that indicates whether the device is assisted-GPS capable “Y” or “N”.
    /// </summary>
    public string AssistedGps { get; set; }

    /// <summary>
    /// Gets or sets the value of LocationTechnology.
    /// A value that indicates the location technology network that is supported by the device. 
    /// The value is expressed in terms of network Location service technology types (i.e. “SUPL” or “SUPL2”) as supporting the Location query on 3G devices and 4G (LTE) devices respectively. 
    /// </summary>
    public string LocationTechnology { get; set; }

    /// <summary>
    /// Gets or sets the value of DeviceBrowser.
    /// A value that indicates the name of the browser that is resident on the device e.g. “RIM” for Blackberry devices.
    /// </summary>
    public string DeviceBrowser { get; set; }

    /// <summary>
    /// Gets or sets the value of WAPPushCapable.
    /// A value that indicates whether the device is WAP Push capable “Y” or “N”.
    /// </summary>
    public string WapPushCapable { get; set; }
}
#endregion

