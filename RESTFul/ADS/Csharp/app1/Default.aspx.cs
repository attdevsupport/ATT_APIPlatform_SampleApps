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
/// Access Token Types
/// </summary>
public enum AccessTokenType
{
    /// <summary>
    /// Access Token Type is based on Client Credential Mode
    /// </summary>
    Client_Credential,

    /// <summary>
    /// Access Token Type is based on Refresh Token
    /// </summary>
    Refresh_Token
}

/// <summary>
/// This application demonstrates the usage of Advertisement API of AT&T platform. 
/// The Advertisement API is a service that returns advertisements enabling the developer to insert the advertisements into their application.
/// </summary>
public partial class Ad_App1 : System.Web.UI.Page
{
    #region Instance Variables

    /// <summary>
    /// Application parameters.
    /// </summary>
    private string apiKey, secretKey, endPoint, scope, bypassSSL;

    /// <summary>
    /// Access token file path
    /// </summary>
    private string accessTokenFilePath;

    /// <summary>
    /// OAuth access token
    /// </summary>
    private string accessToken;

    /// <summary>
    /// OAuth refresh token
    /// </summary>
    private string refreshToken;

    /// <summary>
    /// Expirytimes of refresh and access tokens
    /// </summary>
    private string refreshTokenExpiryTime, accessTokenExpiryTime;

    /// <summary>
    /// No of hours in which refresh token expires.
    /// </summary>
    private int refreshTokenExpiresIn;

    /// <summary>
    /// UDID for Ad tracking purpose.
    /// </summary>
    private string udid;

    /// <summary>
    /// Specifies the AD type.
    /// </summary>
    private string AdType = null;

    public string getAdsSuccessResponse = string.Empty;
    public string getAdsErrorResponse = string.Empty;
    public AdsResponseObject adRequestResponse = null;


    #endregion

    protected void BtnGetADS_Click(object sender, EventArgs e)
    {
        this.GetAds();
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

    private void GetAds()
    {
        try
        {
            bool ableToGetAccessToken = this.ReadAndGetAccessToken(ref getAdsErrorResponse);
            if (ableToGetAccessToken)
            {
                string adsResponse;
                string queryString = this.BuildQueryParameterString();
                HttpWebRequest adsRequest = (HttpWebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/1/ads?" + queryString);
                adsRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                if (string.IsNullOrEmpty(this.udid))
                {
                    adsRequest.Headers.Add("UDID", Guid.NewGuid().ToString());
                }
                else
                {
                    adsRequest.Headers.Add("UDID", this.udid);
                }
                adsRequest.UserAgent = Request.UserAgent;
                adsRequest.Accept = "application/json";
                adsRequest.Method = "GET";

                HttpWebResponse adsResponseObject = (HttpWebResponse)adsRequest.GetResponse();
                using (StreamReader adResponseStream = new StreamReader(adsResponseObject.GetResponseStream()))
                {
                    adsResponse = adResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    adRequestResponse = (AdsResponseObject)deserializeJsonObject.Deserialize(adsResponse, typeof(AdsResponseObject));
                    if (null != adRequestResponse && null != adRequestResponse.AdsResponse && null != adRequestResponse.AdsResponse.Ads)
                    {
                        if (adRequestResponse.AdsResponse.Ads.ImageUrl != null && !string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ImageUrl.Image))
                        {
                            hplImage.ImageUrl = adRequestResponse.AdsResponse.Ads.ImageUrl.Image;
                        }
                        if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Text))
                        {
                            hplImage.ImageUrl = string.Empty;
                            hplImage.Text = adRequestResponse.AdsResponse.Ads.Text;
                        }
                        if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ClickUrl))
                        {
                            hplImage.NavigateUrl = adRequestResponse.AdsResponse.Ads.ClickUrl;
                        }
                        getAdsSuccessResponse = " ";
                    }
                    else
                    {
                        getAdsSuccessResponse = "No ads returned";
                    }
                    adResponseStream.Close();
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

            getAdsErrorResponse = errorResponse + Environment.NewLine + we.Message;
        }
        catch (Exception ex)
        {
            getAdsErrorResponse = ex.Message;
        }
    }

    /// <summary>
    /// Builds query string based on user input.
    /// </summary>
    /// <returns>string; query string to be passed along with API Request.</returns>
    private string BuildQueryParameterString()
    {
        string queryParameter = string.Empty;
        queryParameter = "Category=" + category.Value;

        if (!string.IsNullOrEmpty(gender.Value))
        {
            queryParameter += "&Gender=" + gender.Value;
        }

        if (!string.IsNullOrEmpty(zipCode.Value))
        {
            queryParameter += "&ZipCode=" + zipCode.Value;
        }

        if (!string.IsNullOrEmpty(areaCode.Value))
        {
            queryParameter += "&AreaCode=" + areaCode.Value;
        }

        if (!string.IsNullOrEmpty(city.Value))
        {
            queryParameter += "&City=" + city.Value;
        }

        if (!string.IsNullOrEmpty(country.Value))
        {
            queryParameter += "&Country=" + country.Value;
        }

        if (!string.IsNullOrEmpty(longitude.Value))
        {
            queryParameter += "&Longitude=" + longitude.Value;
        }

        if (!string.IsNullOrEmpty(latitude.Value))
        {
            queryParameter += "&Latitude=" + latitude.Value;
        }

        if (!string.IsNullOrEmpty(MMA.Value))
        {
            string[] dimensions = Regex.Split(MMA.Value, " x ");
            queryParameter += "&MaxWidth=" + dimensions[0];
            queryParameter += "&MaxHeight=" + dimensions[0];
            queryParameter += "&MinHeight=" + dimensions[1];
            queryParameter += "&MinWidth=" + dimensions[1];
        }

        if (!string.IsNullOrEmpty(this.AdType))
        {
            queryParameter += "&Type=" + this.AdType;
        }

        if (!string.IsNullOrEmpty(ageGroup.Value))
        {
            queryParameter += "&AgeGroup=" + ageGroup.Value;
        }

        if (!string.IsNullOrEmpty(over18.Value))
        {
            queryParameter += "&Over18=" + over18.Value;
        }

        if (!string.IsNullOrEmpty(keywords.Value))
        {
            queryParameter += "&Keywords=" + keywords.Value;
        }

        if (!string.IsNullOrEmpty(Premium.Value))
        {
            queryParameter += "&Premium=" + Premium.Value;
        }

        return queryParameter;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        this.ReadConfigFile();
        hplImage.ImageUrl = string.Empty;
        hplImage.Text = string.Empty;
    }

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

    private bool ReadConfigFile()
    {
        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            getAdsErrorResponse = "apiKey is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            getAdsErrorResponse = "secretKey is not defined in configuration file";
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            getAdsErrorResponse = "endPoint is not defined in configuration file";
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "ADS";
        }

        this.udid = ConfigurationManager.AppSettings["udid"];

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\AdsSApp1AccessToken.txt";
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
        return true;
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

#region AdResponse Data Structures

/// <summary>
/// Object that containes the link to the image of the advertisement and tracking Url.
/// </summary>
public class ImageUrlResponse
{
    /// <summary>
    /// Gets or sets the value of Image.
    /// This parameter returns the link to the image of the advertisement.
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// Gets or sets the value of Track.
    /// This parameter contains the pixel tracking URL.
    /// </summary>
    public string Track { get; set; }
}

/// <summary>
/// Container structure for the advertisement details
/// </summary>
public class Ad
{
    /// <summary>
    /// Gets or sets the value of Text.
    /// Specifies the type of advertisement.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the value of ClickUrl.
    /// This parameter contains the click URLs. It returns all the possible sizes available. 
    /// For SMS ads, the URL is shortened to 35-40 characters.
    /// </summary>
    public string ClickUrl { get; set; }

    /// <summary>
    /// Gets or sets the value of ImageUrl.
    /// This parameter returns the link to the image of the advertisement.
    /// </summary>
    public ImageUrlResponse ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the value of Text.
    /// Any ad text(either independent or below the ad)
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the value of TrackUrl.
    /// This parameter contains the pixel tracking URL.
    /// </summary>
    public string TrackUrl { get; set; }

    /// <summary>
    /// Gets or sets the value of Content
    /// All of the ad content is placed in this node as is from 3rd party.
    /// </summary>
    public string Content { get; set; }
}

/// <summary>
/// Container structure for AdResponse
/// </summary>
public class AdResponse
{
    /// <summary>
    /// Gets or sets the value of Ads
    /// Advertisement details
    /// </summary>
    public Ad Ads { get; set; }
}

/// <summary>
/// High level container structure for AdsResponse
/// </summary>
public class AdsResponseObject
{
    /// <summary>
    /// AdsResponse object
    /// </summary>
    public AdResponse AdsResponse { get; set; }
}

#endregion