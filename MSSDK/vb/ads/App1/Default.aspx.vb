' <copyright file="Default.aspx.vb" company="AT&T">
' Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.Advertisementv1
#End Region

''' <summary>
''' This application demonstrates the usage of Advertisement API of AT&amp;T platform. 
''' The Advertisement API is a service that returns advertisements enabling the developer 
''' to insert the advertisements into their application.
''' </summary>
''' <remarks></remarks>
Public Class Ad_App1
    Inherits System.Web.UI.Page

#Region "Instance Variables"

    ''' <summary>
    ''' API Key registered in the dev portal
    ''' </summary>
    Private apiKey As String

    ''' <summary>
    ''' Secret Key associated with api key
    ''' </summary>
    Private secretKey As String

    ''' <summary>
    ''' API gateway
    ''' </summary>
    Private endPoint As String

    ''' <summary>
    ''' Access token file path
    ''' </summary>
    Private accessTokenFilePath As String

    ''' <summary>
    ''' UDID for Ad tracking purpose.
    ''' </summary>
    Private udid As String

    ''' <summary>
    ''' Specifies the mapping between API Gateway and south bound enabler.
    ''' </summary>
    Private psedoZone As String = Nothing

    ''' <summary>
    ''' Specifies the AD type.
    ''' </summary>
    Private AdType As String = Nothing

    ''' <summary>
    ''' RequestFactory instance.
    ''' </summary>
    ''' <remarks></remarks>
    Private requestFactory As RequestFactory

#End Region

#Region "Ad Application Events"

    ''' <summary>
    ''' This method will be executed upon loading of the page.
    ''' Reads the config file and assigns to local variables.
    ''' </summary>
    ''' <param name="sender">sender object</param>
    ''' <param name="e">Event Arguments</param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = (String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC")
        Dim ableToReadConfigFile As Boolean = Me.ReadConfigFile
        If (ableToReadConfigFile = True) Then
            Me.InitializeRequestFactory()
        End If
        hplImage.ImageUrl = String.Empty
        hplImage.Text = String.Empty
    End Sub

    ''' <summary>
    ''' This event will be called when the user clicks on Get Advertisement button.
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event arguments</param>
    Protected Sub GetAdsButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim isUserInputValid As Boolean = Me.IsUserInputValid
        If (isUserInputValid = True) Then
            Me.GetAds()
        End If
    End Sub

#End Region

#Region "Ad Application Methods"

    ''' <summary>
    ''' Neglect the ssl handshake error with authentication server
    ''' </summary>
    Function CertificateValidationCallBack( _
    ByVal sender As Object, _
    ByVal certificate As X509Certificate, _
    ByVal chain As X509Chain, _
    ByVal sslPolicyErrors As SslPolicyErrors _
) As Boolean

        Return True
    End Function

    ''' <summary>
    ''' This function reads the Access Token File and creates OAuthToken object.
    ''' </summary>
    ''' <returns>OAuthToken object</returns>
    Private Function ReadAccessTokenFile() As OAuthToken
        Dim fileStream As FileStream = Nothing
        Dim streamReader As StreamReader = Nothing
        Dim clientToken As OAuthToken = Nothing
        Try
            If (System.IO.File.Exists(Request.MapPath(Me.accessTokenFilePath))) Then
                fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read)
                streamReader = New StreamReader(fileStream)
                Dim accessToken As String = streamReader.ReadLine
                Dim refreshToken As String = streamReader.ReadLine
                Dim expiresIn As String = streamReader.ReadLine
                If (Not String.IsNullOrEmpty(accessToken)) Then
                    clientToken = New OAuthToken(accessToken, refreshToken, expiresIn)
                End If
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If (Not (streamReader) Is Nothing) Then
                streamReader.Close()
            End If
            If (Not (fileStream) Is Nothing) Then
                fileStream.Close()
            End If
        End Try
        Return clientToken
    End Function

    ''' <summary>
    ''' Write access token information to file.
    ''' </summary>
    ''' <param name="clientToken">OAuthToken of the current request.</param>
    Private Sub WriteToAccessTokenFile(ByVal clientToken As OAuthToken)
        Dim fileStream As FileStream = Nothing
        Dim streamWriter As StreamWriter = Nothing
        Try
            fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
            streamWriter = New StreamWriter(fileStream)
            Dim expiresIn As Double = clientToken.CreationTime.Subtract(clientToken.Expiration).TotalSeconds
            streamWriter.WriteLine(clientToken.AccessToken)
            streamWriter.WriteLine(clientToken.RefreshToken)
            streamWriter.WriteLine(expiresIn.ToString)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        Finally
            If (Not (streamWriter) Is Nothing) Then
                streamWriter.Close()
            End If
            If (Not (fileStream) Is Nothing) Then
                fileStream.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Get Ads based on user input.    
    ''' This methods performs the following functions:
    ''' 1. If clientCredential of RequestFactory instance is null
    '''         a. Reads access token from file 
    '''         b. Set the ClientCredential of requestFactory instance with the access token read from file.
    ''' 2. Build AdRequest object.
    ''' 3. Call GetAdvertisement() of requestFactory by passing AdRequest object.
    ''' 4. Display the AdResponse.
    ''' 5. Save the access token information to file.
    ''' </summary>
    Private Sub GetAds()
        Try
            Dim clientToken As OAuthToken = Nothing
            If Me.requestFactory.ClientCredential Is Nothing Then
                clientToken = Me.ReadAccessTokenFile()
                Me.requestFactory.ClientCredential = clientToken
            End If

            Dim adsRequest As AdRequest = Me.BuildAdRequestObject()
            Dim adsResponse As AdResponse = Me.requestFactory.GetAdvertisement(adsRequest, Me.psedoZone)

            If clientToken Is Nothing Or (clientToken IsNot Nothing AndAlso Not (clientToken.AccessToken = Me.requestFactory.ClientCredential.AccessToken)) Then
                Me.WriteToAccessTokenFile(Me.requestFactory.ClientCredential)
            End If

            If adsResponse IsNot Nothing AndAlso adsResponse.AdsResponse IsNot Nothing AndAlso adsResponse.AdsResponse.Ads IsNot Nothing Then
                Me.DrawAdResponse(adsResponse.AdsResponse.Ads)
            Else
                Me.DrawNoAdsResponse()
            End If
        Catch ae As ArgumentNullException
            Me.DrawPanelForFailure(statusPanel, ae.Message)
        Catch ire As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ire.Body)
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(statusPanel, ae.Message)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Builds AdRequest object based on user input.
    ''' </summary>
    ''' <returns>AdRequest; object that will be used to call SDK method.</returns>
    Private Function BuildAdRequestObject() As AdRequest
        Dim adsRequest As AdRequest = New AdRequest
        'adsRequest.Udid = Me.udid
        'adsRequest.Psedo_zone = Me.psedoZone
        adsRequest.Category = ddlCategory.SelectedValue.ToString()

        If Not String.IsNullOrEmpty(ddlGender.SelectedValue) Then
            adsRequest.Gender = ddlGender.SelectedValue.ToString()
        End If

        If Not String.IsNullOrEmpty(txtZipCode.Text) Then
            adsRequest.ZipCode = Convert.ToInt32(txtZipCode.Text)
        End If

        If Not String.IsNullOrEmpty(txtAreaCode.Text) Then
            adsRequest.AreaCode = Convert.ToInt32(txtAreaCode.Text)
        End If

        If Not String.IsNullOrEmpty(txtCity.Text) Then
            adsRequest.City = txtCity.Text
        End If

        If Not String.IsNullOrEmpty(txtCountry.Text) Then
            adsRequest.Country = txtCountry.Text
        End If

        If Not String.IsNullOrEmpty(txtLongitude.Text) Then
            adsRequest.Longitude = Convert.ToDecimal(txtLongitude.Text)
        End If

        If Not String.IsNullOrEmpty(txtLatitude.Text) Then
            adsRequest.Latitude = Convert.ToDecimal(txtLatitude.Text)
        End If

        If Not String.IsNullOrEmpty(ddlMMASize.SelectedValue) Then
            Dim dimensions As String() = Regex.Split(ddlMMASize.SelectedValue, " x ")
            adsRequest.MaxWidth = Convert.ToInt32(dimensions(0))
            adsRequest.MaxHeight = Convert.ToInt32(dimensions(0))
            adsRequest.MinWidth = Convert.ToInt32(dimensions(1))
            adsRequest.MinHeight = Convert.ToInt32(dimensions(1))
        End If

        If Not String.IsNullOrEmpty(Me.AdType) Then
            adsRequest.Type = Convert.ToUInt32(AdType)
        End If

        If Not String.IsNullOrEmpty(ddlAgeGroup.SelectedValue) Then
            adsRequest.AgeGroup = ddlAgeGroup.SelectedValue
        End If

        If Not String.IsNullOrEmpty(ddlOver18.SelectedValue) Then
            adsRequest.Over18Content = Convert.ToUInt32(ddlOver18.SelectedValue)
        End If

        If Not String.IsNullOrEmpty(txtKeywords.Text) Then
            adsRequest.Keywords = txtKeywords.Text
        End If

        If Not String.IsNullOrEmpty(ddlPremium.SelectedValue) Then
            adsRequest.Premium = Convert.ToUInt32(ddlPremium.SelectedValue)
        End If

        Return adsRequest
    End Function

    ''' <summary>
    ''' Validates user input
    ''' </summary>
    ''' <returns>true/false; true if user input is valid; else false</returns>
    Private Function IsUserInputValid() As Boolean
        Dim isInputValid As Boolean = True
        Dim errorMessages As List(Of String) = New List(Of String)
        Dim regEx As System.Text.RegularExpressions.Regex
        Dim zipCodeRegEx As String = "^\d{5}(-\d{4})?$"
        regEx = New System.Text.RegularExpressions.Regex(zipCodeRegEx)
        If Not String.IsNullOrEmpty(txtZipCode.Text) Then
            If Not regEx.IsMatch(txtZipCode.Text) Then
                errorMessages.Add("Invalid Zip Code")
            End If
        End If
        Dim integerRegEx As String = "^\d+$"
        regEx = New System.Text.RegularExpressions.Regex(integerRegEx)
        If Not String.IsNullOrEmpty(txtAreaCode.Text) Then
            If Not regEx.IsMatch(txtAreaCode.Text) Then
                errorMessages.Add("Invalid Area Code")
            End If
        End If
        Dim decimalRegEx As String = "^\d*[0-9](|.\d*[0-9]|)*$"
        regEx = New System.Text.RegularExpressions.Regex(decimalRegEx)
        If Not String.IsNullOrEmpty(txtLatitude.Text) Then
            If Not regEx.IsMatch(txtLatitude.Text) Then
                errorMessages.Add("Invalid Latitude")
            End If
        End If
        If Not String.IsNullOrEmpty(txtLongitude.Text) Then
            If Not regEx.IsMatch(txtLongitude.Text) Then
                errorMessages.Add("Invalid Longitude")
            End If
        End If
        If ((Not (errorMessages) Is Nothing) AndAlso (0 <> errorMessages.Count)) Then
            Dim messageToDisplay As String = "Please correct the following error(s):<ul>"
            For Each errorMessage As String In errorMessages
                messageToDisplay = (messageToDisplay + ("<li>" _
                            + (errorMessage + "</li>")))
            Next
            messageToDisplay = (messageToDisplay + "</ul>")
            Me.DrawPanelForFailure(statusPanel, messageToDisplay)
            isInputValid = False
        End If
        Return isInputValid
    End Function

    ''' <summary>
    ''' Read config file and assign to local variables
    ''' </summary>
    ''' <remarks>
    ''' <para>Validates if the values are configured in web.config file and displays a warning message if not configured.</para>
    ''' </remarks>
    ''' <returns>true/false; true if able to read and assign; else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(statusPanel, "apiKey is not defined in configuration file")
            Return False
        End If
        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(statusPanel, "secretKey is not defined in configuration file")
            Return False
        End If
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(statusPanel, "endPoint is not defined in configuration file")
            Return False
        End If
        Me.udid = ConfigurationManager.AppSettings("udid")
        'If String.IsNullOrEmpty(Me.udid) Then
        'Me.DrawPanelForFailure(statusPanel, "Udid is not defined in configuration file")
        'Return False
        'End If
        Me.psedoZone = ConfigurationManager.AppSettings("Psedo_zone")
        'If String.IsNullOrEmpty(Me.psedoZone) Then
        'Me.DrawPanelForFailure(statusPanel, "Psedo_zone is not defined in configuration file")
        'Return False
        'End If

        Me.AdType = ConfigurationManager.AppSettings("AdType")

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "AdsApp1AccessToken.txt"
        End If
        Return True
    End Function

    ''' <summary>
    ''' Initialized RequestFactory with instance variable values.
    ''' </summary>
    Private Sub InitializeRequestFactory()
        Dim scopes As List(Of RequestFactory.ScopeTypes) = New List(Of RequestFactory.ScopeTypes)
        scopes.Add(RequestFactory.ScopeTypes.ADS)
        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Constructs a table and adds the tables to the given panel with given message
    ''' </para>
    ''' </remarks>
    ''' <param name="panelParam">Panel to draw error message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(ByVal panelParam As Panel, ByVal message As String)
        If panelParam.HasControls Then
            panelParam.Controls.Clear()
        End If
        Dim table As Table = New Table
        table.CssClass = "errorWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As TableRow = New TableRow
        Dim rowOneCellOne As TableCell = New TableCell
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As TableRow = New TableRow
        Dim rowTwoCellOne As TableCell = New TableCell
        rowTwoCellOne.Text = message
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
        hplImage.ImageUrl = String.Empty
        hplImage.Text = String.Empty
    End Sub
    ''' <summary>
    ''' Displays the Success for no Ads.
    ''' </summary>
    Private Sub DrawNoAdsResponse()
        If statusPanel.HasControls() Then
            statusPanel.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "successWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.HorizontalAlign = HorizontalAlign.Center
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Text = "Success:"
        rowOneCellOne.Width = Unit.Percentage(20)
        rowOneCellOne.Font.Bold = True
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellOne.Text = "No Ads are returned"
        rowTwoCellOne.HorizontalAlign = HorizontalAlign.Left
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        statusPanel.Controls.Add(table)
    End Sub
    ''' <summary>
    ''' Displays the Ad
    ''' </summary>
    ''' <param name="adsResponse">Ad response object</param>
    Private Sub DrawAdResponse(ByVal adsResponse As Ad)
        If statusPanel.HasControls() Then
            statusPanel.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "successWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.HorizontalAlign = HorizontalAlign.Center

        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Text = "Success:"
        rowOneCellOne.Width = Unit.Percentage(20)
        rowOneCellOne.Font.Bold = True
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        If Not String.IsNullOrEmpty(adsResponse.Type) Then
            Dim rowTwo As New TableRow()
            Dim rowTwoCellOne As New TableCell()
            Dim rowTwoCellTwo As New TableCell()
            rowTwoCellOne.Text = "Type: "
            rowTwoCellOne.CssClass = "label"
            rowTwoCellOne.HorizontalAlign = HorizontalAlign.Right
            rowTwoCellOne.Font.Bold = True
            rowTwo.Controls.Add(rowTwoCellOne)
            rowTwoCellTwo.Text = adsResponse.Type
            rowTwoCellTwo.CssClass = "cell"
            rowTwo.Controls.Add(rowTwoCellTwo)
            table.Controls.Add(rowTwo)
        End If
        If Not String.IsNullOrEmpty(adsResponse.ClickUrl) Then
            Dim rowThree As New TableRow()
            Dim rowThreeCellOne As New TableCell()
            Dim rowThreeCellTwo As New TableCell()
            rowThreeCellOne.Text = "ClickUrl: "
            rowThreeCellOne.Font.Bold = True
            rowThreeCellOne.CssClass = "label"
            rowThreeCellOne.HorizontalAlign = HorizontalAlign.Right
            rowThree.Controls.Add(rowThreeCellOne)
            rowThreeCellTwo.Text = adsResponse.ClickUrl
            rowThreeCellTwo.CssClass = "cell"
            rowThree.Controls.Add(rowThreeCellTwo)
            table.Controls.Add(rowThree)
        End If

        If adsResponse.ImageUrl IsNot Nothing Then
            Dim rowFour As New TableRow()
            Dim rowFourCellOne As New TableCell()
            Dim rowFourCellTwo As New TableCell()
            rowFourCellOne.Text = "ImageUrl.Image: "
            rowFourCellOne.Font.Bold = True
            rowFourCellOne.CssClass = "label"
            rowFourCellOne.HorizontalAlign = HorizontalAlign.Right
            rowFour.Controls.Add(rowFourCellOne)
            rowFourCellTwo.CssClass = "cell"
            rowFourCellTwo.Text = String.Empty
            rowFourCellTwo.Text = adsResponse.ImageUrl.Image
            rowFour.Controls.Add(rowFourCellTwo)
            table.Controls.Add(rowFour)
        End If

        If Not String.IsNullOrEmpty(adsResponse.Text) Then

            Dim rowFive As New TableRow()
            Dim rowFiveCellOne As New TableCell()
            Dim rowFiveCellTwo As New TableCell()
            rowFiveCellOne.Text = "Text: "
            rowFiveCellOne.Font.Bold = True
            rowFiveCellOne.HorizontalAlign = HorizontalAlign.Right
            rowFiveCellOne.CssClass = "label"
            rowFive.Controls.Add(rowFiveCellOne)
            rowFiveCellTwo.Text = adsResponse.Text
            rowFiveCellTwo.CssClass = "cell"
            rowFive.Controls.Add(rowFiveCellTwo)
            table.Controls.Add(rowFive)
        End If
        If Not String.IsNullOrEmpty(adsResponse.TrackUrl) Then

            Dim rowSix As New TableRow()
            Dim rowSixCellOne As New TableCell()
            Dim rowSixCellTwo As New TableCell()
            rowSixCellOne.Text = "TrackUrl: "
            rowSixCellOne.Font.Bold = True
            rowSixCellOne.CssClass = "label"
            rowSixCellOne.HorizontalAlign = HorizontalAlign.Right
            rowSix.Controls.Add(rowSixCellOne)
            rowSixCellTwo.Text = adsResponse.TrackUrl
            rowSixCellTwo.CssClass = "cell"
            rowSix.Controls.Add(rowSixCellTwo)
            table.Controls.Add(rowSix)
        End If
        If Not String.IsNullOrEmpty(adsResponse.Content) Then
            Dim contentTable As New Table()
            Dim contentRow As New TableRow()
            Dim contentCell As New TableCell()
            contentCell.Text = adsResponse.Content
            contentRow.Controls.Add(contentCell)
            contentTable.Controls.Add(contentRow)
            contentPanel.Controls.Add(contentTable)
        End If

        If adsResponse.ImageUrl IsNot Nothing Then
            hplImage.ImageUrl = adsResponse.ImageUrl.Image
        Else
            hplImage.ImageUrl = String.Empty
            hplImage.Text = adsResponse.Text
        End If

        hplImage.NavigateUrl = adsResponse.ClickUrl

        statusPanel.Controls.Add(table)

    End Sub

#End Region

End Class