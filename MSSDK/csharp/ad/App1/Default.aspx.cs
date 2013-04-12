// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
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
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.Advertisementv1;

#endregion

/// <summary>
/// This application demonstrates the usage of Advertisement API of AT&amp;T platform. 
/// The Advertisement API is a service that returns advertisements enabling the developer to insert the advertisements into their application.
/// </summary>
public partial class Ad_App1 : System.Web.UI.Page
{
    #region Instance Variables

    /// <summary>
    /// Application parameters.
    /// </summary>
    private string apiKey, secretKey, endPoint;

    /// <summary>
    /// Access token file path
    /// </summary>
    private string accessTokenFilePath;

    /// <summary>
    /// UDID for Ad tracking purpose.
    /// </summary>
    private string udid;

    /// <summary>
    /// RequestFactory instance
    /// </summary>
    private RequestFactory requestFactory;

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
        bool ableToReadConfigFile = this.ReadConfigFile();

        if (ableToReadConfigFile == true)
        {
            this.InitializeRequestFactory();
        }

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
    /// This function reads the Access Token File and creates OAuthToken object.
    /// </summary>
    /// <returns>OAuthToken object</returns>
    private OAuthToken ReadAccessTokenFile()
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        OAuthToken clientToken = null;
        try
        {
            if (System.IO.File.Exists(Request.MapPath(this.accessTokenFilePath)))
            {
                fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read);
                streamReader = new StreamReader(fileStream);
                string accessToken = streamReader.ReadLine();
                string refreshToken = streamReader.ReadLine();
                string expiresIn = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    clientToken = new OAuthToken(accessToken, refreshToken, expiresIn);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
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

        return clientToken;
    }

    /// <summary>
    /// Write access token information to file.
    /// </summary>
    /// <param name="clientToken">OAuthToken of the current request.</param>
    private void WriteToAccessTokenFile(OAuthToken clientToken)
    {
        FileStream fileStream = null;
        StreamWriter streamWriter = null;
        try
        {
            fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream);

            double expiresIn = clientToken.CreationTime.Subtract(clientToken.Expiration).TotalSeconds;

            streamWriter.WriteLine(clientToken.AccessToken);
            streamWriter.WriteLine(clientToken.RefreshToken);
            streamWriter.WriteLine(expiresIn.ToString());
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
        finally
        {
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

    /// <summary>
    /// Get Ads based on user input.    
    /// This methods performs the following functions:
    /// 1. If clientCredential of RequestFactory instance is null
    ///         a. Reads access token from file 
    ///         b. Set the ClientCredential of requestFactory instance with the access token read from file.
    /// 2. Build AdRequest object.
    /// 3. Call GetAdvertisement() of requestFactory by passing AdRequest object.
    /// 4. Display the AdResponse.
    /// 5. Save the access token information to file.
    /// </summary>
    private void GetAds()
    {
        try
        {
            OAuthToken clientToken = null;
            if (this.requestFactory.ClientCredential == null)
            {
                clientToken = this.ReadAccessTokenFile();
                this.requestFactory.ClientCredential = clientToken;
            }

            AdRequest adsRequest = this.BuildAdRequestObject();
            AdResponse adsResponse = this.requestFactory.GetAdvertisement(adsRequest, this.udid);

            if (null == clientToken || (null != clientToken && clientToken.AccessToken != this.requestFactory.ClientCredential.AccessToken))
            {
                this.WriteToAccessTokenFile(this.requestFactory.ClientCredential);
            }
            
            if (null != adsResponse && null != adsResponse.AdsResponse && null != adsResponse.AdsResponse.Ads)
            {
                this.DrawAdResponse(adsResponse.AdsResponse.Ads);
            }

            if (null == adsResponse || adsResponse.AdsResponse == null)
            {
                this.DrawPanelForFailure(statusPanel, "No content received");
            }
        }
        catch (ArgumentNullException ae)
        {
            this.DrawPanelForFailure(statusPanel, ae.Message);
        }
        catch (InvalidResponseException ire)
        {
            this.DrawPanelForFailure(statusPanel, ire.Body);
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(statusPanel, ae.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
    }

    /// <summary>
    /// Builds AdRequest object based on user input.
    /// </summary>
    /// <returns>AdRequest; object that will be used to call SDK method.</returns>
    private AdRequest BuildAdRequestObject()
    {
        AdRequest adsRequest = new AdRequest();
        
        adsRequest.Category = ddlCategory.SelectedValue.ToString();

        if (!string.IsNullOrEmpty(ddlGender.SelectedValue))
        {
            adsRequest.Gender = ddlGender.SelectedValue.ToString();
        }

        if (!string.IsNullOrEmpty(txtZipCode.Text))
        {
            adsRequest.ZipCode = Convert.ToInt32(txtZipCode.Text);
        }

        if (!string.IsNullOrEmpty(txtAreaCode.Text))
        {
            adsRequest.AreaCode = Convert.ToInt32(txtAreaCode.Text);
        }

        if (!string.IsNullOrEmpty(txtCity.Text))
        {
            adsRequest.City = txtCity.Text;
        }

        if (!string.IsNullOrEmpty(txtCountry.Text))
        {
            adsRequest.Country = txtCountry.Text;
        }

        if (!string.IsNullOrEmpty(txtLongitude.Text))
        {
            adsRequest.Longitude = Convert.ToDecimal(txtLongitude.Text);
        }

        if (!string.IsNullOrEmpty(txtLatitude.Text))
        {
            adsRequest.Latitude = Convert.ToDecimal(txtLatitude.Text);
        }

        if (!string.IsNullOrEmpty(txtMaxWidth.Text))
        {
            adsRequest.MaxWidth = Convert.ToInt32(txtMaxWidth.Text);
        }

        if (!string.IsNullOrEmpty(txtMaxHeight.Text))
        {
            adsRequest.MinHeight = Convert.ToInt32(txtMaxHeight.Text);
        }

        if (!string.IsNullOrEmpty(txtMinWidth.Text))
        {
            adsRequest.MinWidth = Convert.ToInt32(txtMinWidth.Text);
        }

        if (!string.IsNullOrEmpty(txtMinHeight.Text))
        {
            adsRequest.MinHeight = Convert.ToInt32(txtMinHeight.Text);
        }

        int type = 0;
        foreach (ListItem li in chkAdType.Items)
        {
            if (li.Selected)
            {
                type += Convert.ToInt32(li.Value);
            }
        }

        if (type != 0)
        {
            adsRequest.Type = Convert.ToUInt32(type);
        }

        if (!string.IsNullOrEmpty(ddlAgeGroup.SelectedValue))
        {
            adsRequest.AgeGroup = ddlAgeGroup.SelectedValue;
        }

        if (!string.IsNullOrEmpty(ddlOver18.SelectedValue))
        {
            adsRequest.Over18Content = Convert.ToUInt32(ddlOver18.SelectedValue);
        }

        if (!string.IsNullOrEmpty(txtKeywords.Text))
        {
            adsRequest.Keywords = txtKeywords.Text;
        }

        if (!string.IsNullOrEmpty(ddlPremium.SelectedValue))
        {
            adsRequest.Premium = Convert.ToUInt32(ddlPremium.SelectedValue);
        }

        return adsRequest;
    }

    /// <summary>
    /// Validates user input
    /// </summary>
    /// <returns>true/false; true if user input is valid; else false</returns>
    private bool IsUserInputValid()
    {
        bool isInputValid = true;

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

        string decimalRegEx = @"^[-]?\d+(\.\d+)?$";
        regEx = new System.Text.RegularExpressions.Regex(decimalRegEx);

        if (!string.IsNullOrEmpty(txtLatitude.Text))
        {
            if (!regEx.IsMatch(txtLatitude.Text))
            {
                errorMessages.Add("Invalid Latitude");
            }
        }

        if (!string.IsNullOrEmpty(txtLongitude.Text))
        {
            if (!regEx.IsMatch(txtLongitude.Text))
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

        this.udid = ConfigurationManager.AppSettings["udid"];

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "AdsApp1AccessToken.txt";
        }

        return true;
    }

    /// <summary>
    /// Initialized RequestFactory with instance variable values.
    /// </summary>
    private void InitializeRequestFactory()
    {
        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.ADS);
        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
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
        rowTwoCellOne.Text = message;
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        panelParam.Controls.Add(table);

        hplImage.ImageUrl = string.Empty;
        hplImage.Text = string.Empty;
    }

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

        else
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

        imageContent.InnerHtml = adsResponse.Content;

        statusPanel.Controls.Add(table);
    }

    #endregion

    #endregion
}