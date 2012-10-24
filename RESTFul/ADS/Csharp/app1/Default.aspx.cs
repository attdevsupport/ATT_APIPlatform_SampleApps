// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
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
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;


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
    private string apiKey, secretKey, endPoint, scope;

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
    /// Specifies the mapping between API Gateway and south bound enabler.
    /// </summary>
    private string psedoZone = null;

    /// <summary>
    /// Specifies the AD type.
    /// </summary>
    private string AdType = null;

    #endregion

    #region Ad Application Events

    /// <summary>
    /// This method will be executed upon loading of the page.
    /// Reads the config file and assigns to local variables.
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">Event Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();

        DateTime currentServerTime = DateTime.UtcNow;
        serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
        this.ReadConfigFile();
        hplImage.ImageUrl = string.Empty;
        hplImage.Text = string.Empty;
    }

    /// <summary>
    /// This event will be called when the user clicks on Get Advertisement button.
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event arguments</param>
    protected void GetAdsButton_Click(object sender, EventArgs e)
    {
        bool isUserInputValid = this.IsUserInputValid();
        if (isUserInputValid == true)
        {
            this.GetAds();
        }
    }

    #endregion

    #region Ad Application Methods

    /// <summary>
    /// This method neglects the ssl handshake error with authentication server
    /// </summary>
    private static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(Object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    /// <summary>
    /// Get Ads based on user input.    
    /// This methods performs the following functions:
    /// 1. Reads access token from file.
    /// 2. Validates the access token and determines whether to fetch new access token or use the existing access token.
    /// 3. Once the access token is acquired, query string will be formed based on user's input.
    /// 4. A request will be created and passed to AT&amp;T gateway.
    /// 5. The response will be parsed and displayed in the page.
    /// </summary>
    private void GetAds()
    {
        try
        {
            bool ableToGetAccessToken = this.ReadAndGetAccessToken();
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
                if (!string.IsNullOrEmpty(this.psedoZone))
                {
                    adsRequest.Headers.Add("psedo_zone", this.psedoZone);
                }
                adsRequest.UserAgent = Request.UserAgent;
                adsRequest.Accept = "application/json";
                adsRequest.Method = "GET";

                HttpWebResponse adsResponseObject = (HttpWebResponse)adsRequest.GetResponse();
                using (StreamReader adResponseStream = new StreamReader(adsResponseObject.GetResponseStream()))
                {
                    adsResponse = adResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    AdsResponseObject ads = (AdsResponseObject)deserializeJsonObject.Deserialize(adsResponse, typeof(AdsResponseObject));
                    if (null != ads && null != ads.AdsResponse && null != ads.AdsResponse.Ads)
                    {
                        this.DrawAdResponse(ads.AdsResponse.Ads);
                    }
                    else
                    {
                        this.DrawNoAdsResponse();
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

            this.DrawPanelForFailure(statusPanel, errorResponse + Environment.NewLine + we.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
    }

    /// <summary>
    /// Builds query string based on user input.
    /// </summary>
    /// <returns>string; query string to be passed along with API Request.</returns>
    private string BuildQueryParameterString()
    {
        string queryParameter = string.Empty;
        queryParameter = "Category=" + ddlCategory.SelectedValue;

        if (!string.IsNullOrEmpty(ddlGender.SelectedValue))
        {
            queryParameter += "&Gender=" + ddlGender.SelectedValue;
        }

        if (!string.IsNullOrEmpty(txtZipCode.Text))
        {
            queryParameter += "&ZipCode=" + txtZipCode.Text;
        }

        if (!string.IsNullOrEmpty(txtAreaCode.Text))
        {
            queryParameter += "&AreaCode=" + txtAreaCode.Text;
        }

        if (!string.IsNullOrEmpty(txtCity.Text))
        {
            queryParameter += "&City=" + txtCity.Text;
        }

        if (!string.IsNullOrEmpty(txtCountry.Text))
        {
            queryParameter += "&Country=" + txtCountry.Text;
        }

        if (!string.IsNullOrEmpty(txtLongitude.Text))
        {
            queryParameter += "&Longitude=" + txtLongitude.Text;
        }

        if (!string.IsNullOrEmpty(txtLatitude.Text))
        {
            queryParameter += "&Latitude=" + txtLatitude.Text;
        }

        if (!string.IsNullOrEmpty(ddlMMASize.SelectedValue))
        {
            string[] dimensions = Regex.Split(ddlMMASize.SelectedValue," x ");
            queryParameter += "&MaxWidth=" + dimensions[0];
            queryParameter += "&MaxHeight=" + dimensions[0];
            queryParameter += "&MinHeight=" + dimensions[1];
            queryParameter += "&MinWidth=" + dimensions[1];
        }

        if (!string.IsNullOrEmpty(this.AdType))
        {
            queryParameter += "&Type=" + this.AdType;
        }

        if (!string.IsNullOrEmpty(ddlAgeGroup.SelectedValue))
        {
            queryParameter += "&AgeGroup=" + ddlAgeGroup.SelectedValue;
        }

        if (!string.IsNullOrEmpty(ddlOver18.SelectedValue))
        {
            queryParameter += "&Over18=" + ddlOver18.SelectedValue;
        }

        if (!string.IsNullOrEmpty(txtKeywords.Text))
        {
            queryParameter += "&Keywords=" + txtKeywords.Text;
        }

        if (!string.IsNullOrEmpty(ddlPremium.SelectedValue))
        {
            queryParameter += "&Premium=" + ddlPremium.SelectedValue;
        }

        return queryParameter;
    }
    
    /// <summary>
    /// Validates user input
    /// </summary>
    /// <returns>true/false; true if user input is valid; else false</returns>
    private bool IsUserInputValid()
    {
        bool isInputValid = true;

        if (string.IsNullOrEmpty(ddlCategory.SelectedValue))
        {
            this.DrawPanelForFailure(statusPanel, "Please select Ads category");
            isInputValid = false;
            return isInputValid;
        }

        List<string> errorMessages = new List<string>();
        System.Text.RegularExpressions.Regex regEx;

        string zipCodeRegEx = @"^\d{5}(-\d{4})?$";
        regEx = new System.Text.RegularExpressions.Regex(zipCodeRegEx);
        if (!string.IsNullOrEmpty(txtZipCode.Text))
        {
            if (!regEx.IsMatch(txtZipCode.Text))
            {
                errorMessages.Add("Invalid Zip Code");
            }
        }

        string integerRegEx = @"^\d+$";
        regEx = new System.Text.RegularExpressions.Regex(integerRegEx);
        if (!string.IsNullOrEmpty(txtAreaCode.Text))
        {
            if (!regEx.IsMatch(txtAreaCode.Text))
            {
                errorMessages.Add("Invalid Area Code");
            }
        }

        //string decimalRegEx = @"^\d*[0-9](|.\d*[0-9]|)*$";
        //regEx = new System.Text.RegularExpressions.Regex(decimalRegEx);
        Double number;
        if (!string.IsNullOrEmpty(txtLatitude.Text))
        {
            if ( !Double.TryParse(txtLatitude.Text.ToString(), out number) )
            {
                errorMessages.Add("Invalid Latitude");
            }
        }

        if (!string.IsNullOrEmpty(txtLongitude.Text))
        {
            if (!Double.TryParse(txtLongitude.Text.ToString(), out number))
            {
                errorMessages.Add("Invalid Longitude");
            }
        }

        if (null != errorMessages && 0 != errorMessages.Count)
        {
            string messageToDisplay = "Please correct the following error(s):<ul>";
            foreach (string errorMessage in errorMessages)
            {
                messageToDisplay += "<li>" + errorMessage + "</li>";
            }

            messageToDisplay += "</ul>";
            this.DrawPanelForFailure(statusPanel, messageToDisplay);
            isInputValid = false;
        }

        return isInputValid;
    }

    /// <summary>
    /// Read config file and assign to local variables
    /// </summary>
    /// <remarks>
    /// <para>Validates if the values are configured in web.config file and displays a warning message if not configured.</para>
    /// </remarks>
    /// <returns>true/false; true if able to read and assign; else false</returns>
    private bool ReadConfigFile()
    {
        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(statusPanel, "apiKey is not defined in configuration file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(statusPanel, "secretKey is not defined in configuration file");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(statusPanel, "endPoint is not defined in configuration file");
            return false;
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "Ads";
        }

        this.udid = ConfigurationManager.AppSettings["udid"];
        //if (string.IsNullOrEmpty(this.udid))
        //{
            //this.DrawPanelForFailure(statusPanel, "Udid is not defined in configuration file");
            //return false;
        //}

        this.psedoZone = ConfigurationManager.AppSettings["Psedo_zone"];
        //if (string.IsNullOrEmpty(this.psedoZone))
        //{
            //this.DrawPanelForFailure(statusPanel, "Psedo_zone is not defined in configuration file");
            //return false;
        //}

        this.AdType = ConfigurationManager.AppSettings["AdType"];

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "AdsSApp1AccessToken.txt";
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

    #region Display Status Methods

    /// <summary>
    /// Displays error message
    /// </summary>
    /// <remarks>
    /// <para>
    /// Constructs a table and adds the tables to the given panel with given message
    /// </para>
    /// </remarks>
    /// <param name="panelParam">Panel to draw error message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        if (panelParam.HasControls())
        {
            panelParam.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "errorWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "ERROR:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Text = message.ToString();
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        panelParam.Controls.Add(table);
        hplImage.ImageUrl = string.Empty;
        hplImage.Text = string.Empty;
    }
    
   /// <summary>
   /// Displays the Success for no Ads.
   /// </summary>
   /// <param name="adsResponse">Ad response object</param>
    private void DrawNoAdsResponse()
    {
        if (statusPanel.HasControls())
        {
            statusPanel.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "successWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.HorizontalAlign = HorizontalAlign.Center;
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Text = "Success:";
        rowOneCellOne.Width = Unit.Percentage(20);
        rowOneCellOne.Font.Bold = true;
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellOne.Text = "No Ads are returned";
        rowTwoCellOne.HorizontalAlign = HorizontalAlign.Left;
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        statusPanel.Controls.Add(table);
    }
   /// <summary>
    /// Displays the Ad
   /// </summary>
   /// <param name="adsResponse">Ad response object</param>
    private void DrawAdResponse(Ad adsResponse)
    {
        if (statusPanel.HasControls())
        {
            statusPanel.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "successWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.HorizontalAlign = HorizontalAlign.Center;

        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Text = "Success:";
        rowOneCellOne.Width = Unit.Percentage(20);
        rowOneCellOne.Font.Bold = true;
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        if (!string.IsNullOrEmpty(adsResponse.Type))
        {
            TableRow rowTwo = new TableRow();
            TableCell rowTwoCellOne = new TableCell();
            TableCell rowTwoCellTwo = new TableCell();
            rowTwoCellOne.Text = "Type: ";
            rowTwoCellOne.CssClass = "label";
            rowTwoCellOne.HorizontalAlign = HorizontalAlign.Right;
            rowTwoCellOne.Font.Bold = true;
            rowTwo.Controls.Add(rowTwoCellOne);
            rowTwoCellTwo.Text = adsResponse.Type;
            rowTwoCellTwo.CssClass = "cell";
            rowTwo.Controls.Add(rowTwoCellTwo);
            table.Controls.Add(rowTwo);
        }
        if (!string.IsNullOrEmpty(adsResponse.ClickUrl))
        {
            TableRow rowThree = new TableRow();
            TableCell rowThreeCellOne = new TableCell();
            TableCell rowThreeCellTwo = new TableCell();
            rowThreeCellOne.Text = "ClickUrl: ";
            rowThreeCellOne.Font.Bold = true;
            rowThreeCellOne.CssClass = "label";
            rowThreeCellOne.HorizontalAlign = HorizontalAlign.Right;
            rowThree.Controls.Add(rowThreeCellOne);
            rowThreeCellTwo.Text = adsResponse.ClickUrl;
            rowThreeCellTwo.CssClass = "cell";
            rowThree.Controls.Add(rowThreeCellTwo);
            table.Controls.Add(rowThree);
        }

        if (null != adsResponse.ImageUrl)
        {
            TableRow rowFour = new TableRow();
            TableCell rowFourCellOne = new TableCell();
            TableCell rowFourCellTwo = new TableCell();
            rowFourCellOne.Text = "ImageUrl.Image: ";
            rowFourCellOne.Font.Bold = true;
            rowFourCellOne.CssClass = "label";
            rowFourCellOne.HorizontalAlign = HorizontalAlign.Right;
            rowFour.Controls.Add(rowFourCellOne);
            rowFourCellTwo.CssClass = "cell";
            rowFourCellTwo.Text = string.Empty;
            rowFourCellTwo.Text = adsResponse.ImageUrl.Image;
            rowFour.Controls.Add(rowFourCellTwo);
            table.Controls.Add(rowFour);
        }

        if (!string.IsNullOrEmpty(adsResponse.Text))
        {

            TableRow rowFive = new TableRow();
            TableCell rowFiveCellOne = new TableCell();
            TableCell rowFiveCellTwo = new TableCell();
            rowFiveCellOne.Text = "Text: ";
            rowFiveCellOne.Font.Bold = true;
            rowFiveCellOne.HorizontalAlign = HorizontalAlign.Right;
            rowFiveCellOne.CssClass = "label";
            rowFive.Controls.Add(rowFiveCellOne);
            rowFiveCellTwo.Text = adsResponse.Text;
            rowFiveCellTwo.CssClass = "cell";
            rowFive.Controls.Add(rowFiveCellTwo);
            table.Controls.Add(rowFive);
        }
        if (!string.IsNullOrEmpty(adsResponse.TrackUrl))
        {

            TableRow rowSix = new TableRow();
            TableCell rowSixCellOne = new TableCell();
            TableCell rowSixCellTwo = new TableCell();
            rowSixCellOne.Text = "TrackUrl: ";
            rowSixCellOne.Font.Bold = true;
            rowSixCellOne.CssClass = "label";
            rowSixCellOne.HorizontalAlign = HorizontalAlign.Right;
            rowSix.Controls.Add(rowSixCellOne);
            rowSixCellTwo.Text = adsResponse.TrackUrl;
            rowSixCellTwo.CssClass = "cell";
            rowSix.Controls.Add(rowSixCellTwo);
            table.Controls.Add(rowSix);
        }
        if (!string.IsNullOrEmpty(adsResponse.Content))
        {
            Table contentTable = new Table();
            TableRow contentRow = new TableRow();
            TableCell contentCell = new TableCell();
            contentCell.Text = adsResponse.Content;
            contentRow.Controls.Add(contentCell);
            contentTable.Controls.Add(contentRow);
            contentPanel.Controls.Add(contentTable);
        }

        if (null != adsResponse.ImageUrl)
        {
            hplImage.ImageUrl = adsResponse.ImageUrl.Image;
        }
        else
        {
            hplImage.ImageUrl = string.Empty;
            hplImage.Text = adsResponse.Text;
        }

        hplImage.NavigateUrl = adsResponse.ClickUrl;

        statusPanel.Controls.Add(table);
    }

    #endregion

    #region Access Token Methods

    /// <summary>
    /// This function reads access token file, validates the access token and gets a new access token
    /// </summary>
    /// <returns>true if access token is valid, or else false is returned</returns>
    private bool ReadAndGetAccessToken()
    {
        bool ableToGetToken = true;

        if (this.ReadAccessTokenFile() == false)
        {
            ableToGetToken = this.GetAccessToken(AccessTokenType.Client_Credential);
        }
        else
        {
            string tokenValidity = this.IsTokenValid();

            if (tokenValidity.Equals("REFRESH_TOKEN"))
            {
                ableToGetToken = this.GetAccessToken(AccessTokenType.Refresh_Token);
            }
            else if (tokenValidity.Equals("INVALID_ACCESS_TOKEN"))
            {
                ableToGetToken = this.GetAccessToken(AccessTokenType.Client_Credential);
            }
        }

        return ableToGetToken;
    }

    /// <summary>
    /// This function reads the Access Token File and stores the values of access token, refresh token, 
    /// access token expiry time and refresh token expiry time. 
    /// </summary>
    /// <returns>true, if access token file and all others attributes read successfully otherwise returns false</returns>
    private bool ReadAccessTokenFile()
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        bool ableToRead = true;
        try
        {
            fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read);
            streamReader = new StreamReader(fileStream);
            this.accessToken = streamReader.ReadLine();
            this.refreshToken = streamReader.ReadLine();
            this.accessTokenExpiryTime = streamReader.ReadLine();
            this.refreshTokenExpiryTime = streamReader.ReadLine();
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
            ableToRead = false;
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

        if (this.accessToken == null || this.refreshToken == null || this.accessTokenExpiryTime == null || this.refreshTokenExpiryTime == null)
        {
            ableToRead = false;
        }

        return ableToRead;
    }

    /// <summary>
    /// Validates he expiry of the access token and refresh token
    /// </summary>
    /// <returns>string, returns VALID_ACCESS_TOKEN if token is valid
    /// otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    /// returns REFRESH_TOKEN, if access token in expired and refresh token is valid</returns>
    private string IsTokenValid()
    {
        if (this.accessToken == null)
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

            WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
            accessTokenRequest.Method = "POST";

            string oauthParameters = string.Empty;
            if (type == AccessTokenType.Client_Credential)
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
                string jsonAccessToken = accessTokenResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(jsonAccessToken, typeof(AccessTokenResponse));

                this.accessToken = deserializedJsonObj.access_token;
                this.refreshToken = deserializedJsonObj.refresh_token;
                DateTime accessTokenTakenTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in));
                this.accessTokenExpiryTime = accessTokenTakenTime.ToLongDateString() + " " + accessTokenTakenTime.ToLongTimeString();

                DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                if (deserializedJsonObj.expires_in.Equals("0"))
                {
                    int defaultAccessTokenExpiresIn = 100; // In Years
                    accessTokenTakenTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn);
                    this.accessTokenExpiryTime = accessTokenTakenTime.ToLongDateString() + " " + accessTokenTakenTime.ToLongTimeString();
                }

                this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();

                fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine(this.accessToken);
                streamWriter.WriteLine(this.refreshToken);
                streamWriter.WriteLine(this.accessTokenExpiryTime);
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

            this.DrawPanelForFailure(statusPanel, errorResponse + Environment.NewLine + we.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.ToString());
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

    #endregion

    #endregion    
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
public class ImageUrl
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
    public ImageUrl ImageUrl { get; set; }

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