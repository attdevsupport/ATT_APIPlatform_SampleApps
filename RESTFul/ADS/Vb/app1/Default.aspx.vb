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
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Web.UI.WebControls
#End Region

''' <summary>
''' Access Token Types
''' </summary>
Public Enum AccessTokenType

    Client_Credential

    Refresh_Token

End Enum

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
    ''' Scope of the application
    ''' </summary>
    Private scope As String

    ''' <summary>
    ''' Access token file path
    ''' </summary>
    Private accessTokenFilePath As String

    ''' <summary>
    ''' OAuth access token
    ''' </summary>
    Private accessToken As String

    ''' <summary>
    ''' OAuth refresh token
    ''' </summary>
    Private refreshToken As String

    ''' <summary>
    ''' Refresh Token Expiry Time
    ''' </summary>
    Private refreshTokenExpiryTime As String

    ''' <summary>
    ''' Access Token Expiry Time
    ''' </summary>
    Private accessTokenExpiryTime As String

    ''' <summary>
    ''' No of hours in which refresh token expires.
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

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

#End Region

#Region "Ad Application Events"

    ''' <summary>
    '''  This method will be executed upon loading of the page. 
    '''  Reads the config file and assigns to local variables.
    ''' </summary>
    ''' <param name="sender">sender object</param>
    ''' <param name="e">Event arguments</param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = (String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC")
        Me.ReadConfigFile()
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
    ''' Get Ads based on user input.    
    ''' This methods performs the following functions:
    ''' 1. Reads access token from file.
    ''' 2. Validates the access token and determines whether to fetch new access token or use the existing access token.
    ''' 3. Once the access token is acquired, query string will be formed based on user's input.
    ''' 4. A request will be created and passed to AT&amp;T gateway.
    ''' 5. The response will be parsed and displayed in the page.
    ''' </summary>
    Private Sub GetAds()
        Try
            Dim ableToGetAccessToken As Boolean = Me.ReadAndGetAccessToken
            If ableToGetAccessToken Then
                Dim adsResponse As String
                Dim queryString As String = Me.BuildQueryParameterString
                Dim adsRequest As HttpWebRequest = CType(System.Net.WebRequest.Create((String.Empty _
                                + (Me.endPoint + ("/rest/1/ads?" + queryString)))), HttpWebRequest)
                adsRequest.Headers.Add("Authorization", ("Bearer " + Me.accessToken))
                If String.IsNullOrEmpty(Me.udid) Then
                    adsRequest.Headers.Add("UDID", Guid.NewGuid().ToString())
                Else
                    adsRequest.Headers.Add("UDID", Me.udid)
                End If
                If Not String.IsNullOrEmpty(Me.psedoZone) Then
                    adsRequest.Headers.Add("psedo_zone", Me.psedoZone)
                End If
                adsRequest.UserAgent = Request.UserAgent
                adsRequest.Accept = "application/json"
                adsRequest.Method = "GET"
                Dim adsResponseObject As HttpWebResponse = CType(adsRequest.GetResponse, HttpWebResponse)
                Dim adResponseStream As StreamReader = New StreamReader(adsResponseObject.GetResponseStream)
                adsResponse = adResponseStream.ReadToEnd
                Dim deserializeJsonObject As JavaScriptSerializer = New JavaScriptSerializer
                Dim ads As AdsResponseObject = CType(deserializeJsonObject.Deserialize(adsResponse, GetType(AdsResponseObject)), AdsResponseObject)
                If ((Not (ads) Is Nothing) _
                            AndAlso ((Not (ads.AdsResponse) Is Nothing) AndAlso (Not (ads.AdsResponse.Ads) Is Nothing))) Then
                    Me.DrawAdResponse(ads.AdsResponse.Ads)
                Else
                    Me.DrawNoAdsResponse()
                End If
                adResponseStream.Close()
            End If
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Dim sr2 As StreamReader = New StreamReader(we.Response.GetResponseStream)
                errorResponse = sr2.ReadToEnd
                sr2.Close()
            Catch
                errorResponse = "Unable to get response"
            End Try
            Me.DrawPanelForFailure(statusPanel, (errorResponse _
                            + (Environment.NewLine + we.Message)))
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Builds query string based on user input.
    ''' </summary>
    ''' <returns>string; query string to be passed along with API Request.</returns>
    Private Function BuildQueryParameterString() As String
        Dim queryParameter As String = String.Empty
        queryParameter = ("Category=" + ddlCategory.SelectedValue)
        If Not String.IsNullOrEmpty(ddlGender.SelectedValue) Then
            queryParameter = (queryParameter + ("&Gender=" + ddlGender.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(txtZipCode.Text) Then
            queryParameter = (queryParameter + ("&ZipCode=" + txtZipCode.Text))
        End If
        If Not String.IsNullOrEmpty(txtAreaCode.Text) Then
            queryParameter = (queryParameter + ("&AreaCode=" + txtAreaCode.Text))
        End If
        If Not String.IsNullOrEmpty(txtCity.Text) Then
            queryParameter = (queryParameter + ("&City=" + txtCity.Text))
        End If
        If Not String.IsNullOrEmpty(txtCountry.Text) Then
            queryParameter = (queryParameter + ("&Country=" + txtCountry.Text))
        End If
        If Not String.IsNullOrEmpty(txtLongitude.Text) Then
            queryParameter = (queryParameter + ("&Longitude=" + txtLongitude.Text))
        End If
        If Not String.IsNullOrEmpty(txtLatitude.Text) Then
            queryParameter = (queryParameter + ("&Latitude=" + txtLatitude.Text))
        End If
        If Not String.IsNullOrEmpty(ddlMMASize.SelectedValue) Then
            Dim dimensions As String() = Regex.Split(ddlMMASize.SelectedValue, " x ")
            queryParameter += "&MaxWidth=" + dimensions(0)
            queryParameter += "&MaxHeight=" + dimensions(0)
            queryParameter += "&MinHeight=" + dimensions(1)
            queryParameter += "&MinWidth=" + dimensions(1)
        End If
        If Not String.IsNullOrEmpty(Me.AdType) Then
            queryParameter += "&Type=" + Me.AdType
        End If
        If Not String.IsNullOrEmpty(ddlAgeGroup.SelectedValue) Then
            queryParameter = (queryParameter + ("&AgeGroup=" + ddlAgeGroup.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(ddlOver18.SelectedValue) Then
            queryParameter = (queryParameter + ("&Over18=" + ddlOver18.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(txtKeywords.Text) Then
            queryParameter = (queryParameter + ("&Keywords=" + txtKeywords.Text))
        End If
        If Not String.IsNullOrEmpty(ddlPremium.SelectedValue) Then
            queryParameter = (queryParameter + ("&Premium=" + ddlPremium.SelectedValue))
        End If
        Return queryParameter
    End Function

    ''' <summary>
    ''' Validates user input
    ''' </summary>
    ''' <returns>true/false; true if user input is valid; else false</returns>
    Private Function IsUserInputValid() As Boolean
        Dim isInputValid As Boolean = True
        If String.IsNullOrEmpty(ddlCategory.SelectedValue) Then
            Me.DrawPanelForFailure(statusPanel, "Please select Ads category")
            isInputValid = False
            Return isInputValid
        End If
        Dim errorMessages As List(Of String) = New List(Of String)
        Dim integerRegEx As String = "^\d+$"
        Dim zipCodeRegEx = "^\d{5}(-\d{4})?$"
        Dim regEx As System.Text.RegularExpressions.Regex
        regEx = New System.Text.RegularExpressions.Regex(zipCodeRegEx)
        If Not String.IsNullOrEmpty(txtZipCode.Text) Then
            If Not regEx.IsMatch(txtZipCode.Text) Then
                errorMessages.Add("Invalid Zip Code")
            End If
        End If

        regEx = New System.Text.RegularExpressions.Regex(integerRegEx)
        If Not String.IsNullOrEmpty(txtAreaCode.Text) Then
            If Not regEx.IsMatch(txtAreaCode.Text) Then
                errorMessages.Add("Invalid Area Code")
            End If
        End If

        'Dim decimalRegEx As String = "\d*[0-9](|.\d*[0-9]|)*$"
        'regEx = New System.Text.RegularExpressions.Regex(decimalRegEx)
        Dim number As Double
        If Not String.IsNullOrEmpty(txtLatitude.Text) Then
            If Not Double.TryParse(txtLatitude.Text.ToString, number) Then
                errorMessages.Add("Invalid Latitude")
            End If
        End If
        If Not String.IsNullOrEmpty(txtLongitude.Text) Then
            If Not Double.TryParse(txtLongitude.Text.ToString, number) Then
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
        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "ADS"
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
            Me.accessTokenFilePath = "AdsSApp1AccessToken.txt"
        End If
        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            Me.refreshTokenExpiresIn = 24
        End If
        Return True
    End Function
#Region "Display Status Methods"

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
        rowTwoCellOne.Text = message.ToString
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
        hplImage.ImageUrl = String.Empty
        hplImage.Text = String.Empty
    End Sub

    ''' <summary>
    ''' Displays the Success for no Ads.
    ''' </summary>
    ''' <param name="adsResponse">Ad response object</param>
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
#Region "Access Token Methods"

    Private Function ReadAndGetAccessToken() As Boolean
        Dim ableToGetToken As Boolean = True
        If (Me.ReadAccessTokenFile = False) Then
            ableToGetToken = Me.GetAccessToken(AccessTokenType.Client_Credential)
        Else
            Dim tokenValidity As String = Me.IsTokenValid
            If tokenValidity.Equals("REFRESH_TOKEN") Then
                ableToGetToken = Me.GetAccessToken(AccessTokenType.Refresh_Token)
            ElseIf tokenValidity.Equals("INVALID_ACCESS_TOKEN") Then
                ableToGetToken = Me.GetAccessToken(AccessTokenType.Client_Credential)
            End If
        End If
        Return ableToGetToken
    End Function

    ''' <summary>
    ''' This function reads the Access Token File and stores the values of access token, refresh token, 
    ''' access token expiry time and refresh token expiry time. 
    ''' </summary>
    ''' <returns>true, if access token file and all others attributes read successfully otherwise returns false</returns>
    Private Function ReadAccessTokenFile() As Boolean
        Dim fileStream As FileStream = Nothing
        Dim streamReader As StreamReader = Nothing
        Dim ableToRead As Boolean = True
        Try
            fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read)
            streamReader = New StreamReader(fileStream)
            Me.accessToken = streamReader.ReadLine
            Me.refreshToken = streamReader.ReadLine
            Me.accessTokenExpiryTime = streamReader.ReadLine
            Me.refreshTokenExpiryTime = streamReader.ReadLine
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
            ableToRead = False
        Finally
            If (Not (streamReader) Is Nothing) Then
                streamReader.Close()
            End If
            If (Not (fileStream) Is Nothing) Then
                fileStream.Close()
            End If
        End Try
        If ((Me.accessToken Is Nothing) _
                    OrElse ((Me.refreshToken Is Nothing) _
                    OrElse ((Me.accessTokenExpiryTime Is Nothing) _
                    OrElse (Me.refreshTokenExpiryTime Is Nothing)))) Then
            ableToRead = False
        End If
        Return ableToRead
    End Function

    ''' <summary>
    ''' Validates he expiry of the access token and refresh token
    ''' </summary>
    ''' <returns>string, returns VALID_ACCESS_TOKEN if token is valid
    ''' otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    ''' returns REFRESH_TOKEN, if access token in expired and refresh token is valid</returns>
    Private Function IsTokenValid() As String
        If (Me.accessToken Is Nothing) Then
            Return "INVALID_ACCESS_TOKEN"
        End If
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime
            If (currentServerTime >= DateTime.Parse(Me.accessTokenExpiryTime)) Then
                If (currentServerTime >= DateTime.Parse(Me.refreshTokenExpiryTime)) Then
                    Return "INVALID_ACCESS_TOKEN"
                Else
                    Return "REFRESH_TOKEN"
                End If
            Else
                Return "VALID_ACCESS_TOKEN"
            End If
        Catch
            Return "INVALID_ACCESS_TOKEN"
        End Try
    End Function

    ''' <summary>
    ''' This method gets access token based on either client credentials mode or refresh token.
    ''' </summary>
    ''' <param name="type">AccessTokenType; either Client_Credential or Refresh_Token</param>
    ''' <returns>true/false; true if able to get access token, else false</returns>
    Private Function GetAccessToken(ByVal type As AccessTokenType) As Boolean
        Dim postStream As Stream = Nothing
        Dim streamWriter As StreamWriter = Nothing
        Dim fileStream As FileStream = Nothing
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime
            Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create((String.Empty _
                            + (Me.endPoint + "/oauth/token")))
            accessTokenRequest.Method = "POST"
            Dim oauthParameters As String = String.Empty
            If (type = AccessTokenType.Client_Credential) Then
                oauthParameters = ("client_id=" _
                            + (Me.apiKey + ("&client_secret=" _
                            + (Me.secretKey + ("&grant_type=client_credentials&scope=" + Me.scope)))))
            Else
                oauthParameters = ("grant_type=refresh_token&client_id=" _
                            + (Me.apiKey + ("&client_secret=" _
                            + (Me.secretKey + ("&refresh_token=" + Me.refreshToken)))))
            End If
            accessTokenRequest.ContentType = "application/x-www-form-urlencoded"
            Dim encoding As UTF8Encoding = New UTF8Encoding
            Dim postBytes() As Byte = encoding.GetBytes(oauthParameters)
            accessTokenRequest.ContentLength = postBytes.Length
            postStream = accessTokenRequest.GetRequestStream
            postStream.Write(postBytes, 0, postBytes.Length)
            Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse
            Dim accessTokenResponseStream As StreamReader = New StreamReader(accessTokenResponse.GetResponseStream)
            Dim jsonAccessToken As String = accessTokenResponseStream.ReadToEnd
            Dim deserializeJsonObject As JavaScriptSerializer = New JavaScriptSerializer
            Dim deserializedJsonObj As AccessTokenResponse = CType(deserializeJsonObject.Deserialize(jsonAccessToken, GetType(AccessTokenResponse)), AccessTokenResponse)
            Me.accessToken = deserializedJsonObj.access_token
            Me.refreshToken = deserializedJsonObj.refresh_token
            Dim accessTokenTakenTime As DateTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in))
            Me.accessTokenExpiryTime = (accessTokenTakenTime.ToLongDateString + (" " + accessTokenTakenTime.ToLongTimeString))
            Dim refreshExpiry As DateTime = currentServerTime.AddHours(Me.refreshTokenExpiresIn)
            If deserializedJsonObj.expires_in.Equals("0") Then
                Dim defaultAccessTokenExpiresIn As Integer = 100
                ' In Years
                accessTokenTakenTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn)
                Me.accessTokenExpiryTime = (accessTokenTakenTime.ToLongDateString + (" " + accessTokenTakenTime.ToLongTimeString))
            End If
            Me.refreshTokenExpiryTime = (refreshExpiry.ToLongDateString + (" " + refreshExpiry.ToLongTimeString))
            fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
            streamWriter = New StreamWriter(fileStream)
            streamWriter.WriteLine(Me.accessToken)
            streamWriter.WriteLine(Me.refreshToken)
            streamWriter.WriteLine(Me.accessTokenExpiryTime)
            streamWriter.WriteLine(Me.refreshTokenExpiryTime)
            ' Close and clean up the StreamReader
            accessTokenResponseStream.Close()
            Return True
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Dim sr2 As StreamReader = New StreamReader(we.Response.GetResponseStream)
                errorResponse = sr2.ReadToEnd
                sr2.Close()
            Catch
                errorResponse = "Unable to get response"
            End Try
            Me.DrawPanelForFailure(statusPanel, (errorResponse _
                            + (Environment.NewLine + we.Message)))
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.ToString)
        Finally
            If (Not (postStream) Is Nothing) Then
                postStream.Close()
            End If
            If (Not (streamWriter) Is Nothing) Then
                streamWriter.Close()
            End If
            If (Not (fileStream) Is Nothing) Then
                fileStream.Close()
            End If
        End Try
        Return False
    End Function
#End Region
#End Region
End Class

#Region "Data Structures"

''' <summary>
''' AccessTokenResponse Object, returned upon calling get auth token api.
''' </summary>
Public Class AccessTokenResponse
    ''' <summary>
    ''' Gets or sets the value of access_token
    ''' </summary>
    Public Property access_token As String
        Get
            Return m_accessToken
        End Get
        Set(value As String)
            m_accessToken = value
        End Set
    End Property
    Private m_accessToken As String

    ''' <summary>
    ''' Gets or sets the value of refresh_token
    ''' </summary>
    Public Property refresh_token As String
        Get
            Return m_refreshToken
        End Get
        Set(value As String)
            m_refreshToken = value
        End Set
    End Property
    Private m_refreshToken As String

    ''' <summary>
    ''' Gets or sets the value of expires_in
    ''' </summary>
    Public Property expires_in As String
        Get
            Return m_expiresIn
        End Get
        Set(value As String)
            m_expiresIn = value
        End Set
    End Property
    Private m_expiresIn As String
End Class

''' <summary>
''' Object that containes the link to the image of the advertisement and tracking Url.
''' </summary>
Public Class ImageUrl

    ''' <summary>
    ''' Gets or sets the value of Image.
    ''' This parameter returns the link to the image of the advertisement.
    ''' </summary>
    Public Property Image As String
        Get
            Return m_Image
        End Get
        Set(value As String)
            m_Image = value
        End Set
    End Property
    Private m_Image As String

    ''' <summary>
    ''' Gets or sets the value of Track.
    ''' This parameter contains the pixel tracking URL.
    ''' </summary>
    Public Property Track As String
        Get
            Return m_Track
        End Get
        Set(value As String)
            m_Track = value
        End Set
    End Property
    Private m_Track As String
End Class

''' <summary>
''' Container structure for the advertisement details
''' </summary>
Public Class Ad

    ''' <summary>
    ''' Gets or sets the value of Text.
    ''' Specifies the type of advertisement.
    ''' </summary>
    Public Property Type As String
        Get
            Return m_Type
        End Get
        Set(value As String)
            m_Type = value
        End Set
    End Property
    Private m_Type As String

    ''' <summary>
    ''' Gets or sets the value of ClickUrl.
    ''' This parameter contains the click URLs. It returns all the possible sizes available. 
    ''' For SMS ads, the URL is shortened to 35-40 characters.
    ''' </summary>
    Public Property ClickUrl As String
        Get
            Return m_clickUrl
        End Get
        Set(value As String)
            m_clickUrl = value
        End Set
    End Property
    Private m_clickUrl As String

    ''' <summary>
    ''' Gets or sets the value of ImageUrl.
    ''' This parameter returns the link to the image of the advertisement.
    ''' </summary>
    Public Property ImageUrl As ImageUrl
        Get
            Return m_ImageUrl
        End Get
        Set(value As ImageUrl)
            m_ImageUrl = value
        End Set
    End Property
    Private m_ImageUrl As ImageUrl

    ''' <summary>
    ''' Gets or sets the value of Text.
    ''' Any ad text(either independent or below the ad)
    ''' </summary>
    Public Property Text As String
        Get
            Return m_Text
        End Get
        Set(value As String)
            m_Text = value
        End Set
    End Property
    Private m_Text As String

    ''' <summary>
    ''' Gets or sets the value of TrackUrl.
    ''' This parameter contains the pixel tracking URL.
    ''' </summary>
    Public Property TrackUrl As String
        Get
            Return m_TrackUrl
        End Get
        Set(value As String)
            m_TrackUrl = value
        End Set
    End Property
    Private m_TrackUrl As String

    ''' <summary>
    ''' Gets or sets the value of Content
    ''' All of the ad content is placed in this node as is from 3rd party.
    ''' </summary>
    Public Property Content As String
        Get
            Return m_Content
        End Get
        Set(value As String)
            m_Content = value
        End Set
    End Property
    Private m_Content As String
End Class

''' <summary>
''' Container structure for AdResponse
''' </summary>
Public Class AdResponse

    ''' <summary>
    ''' Gets or sets the value of Ads
    ''' Advertisement details
    ''' </summary>
    Public Property Ads As Ad
        Get
            Return m_Ads
        End Get
        Set(value As Ad)
            m_Ads = value
        End Set
    End Property
    Private m_Ads As Ad
End Class

''' <summary>
''' High level container structure for AdsResponse
''' </summary>
Public Class AdsResponseObject

    ''' <summary>
    ''' AdsResponse object
    ''' </summary>
    Public Property AdsResponse As AdResponse
        Get
            Return m_adsResponse
        End Get
        Set(value As AdResponse)
            m_adsResponse = value
        End Set
    End Property
    Private m_adsResponse As AdResponse
End Class

#End Region