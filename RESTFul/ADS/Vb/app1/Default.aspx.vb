' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization


#End Region

''' <summary>
''' Access Token Types
''' </summary>
Public Enum AccessTokenType
    ''' <summary>
    ''' Access Token Type is based on Client Credential Mode
    ''' </summary>
    Client_Credential

    ''' <summary>
    ''' Access Token Type is based on Refresh Token
    ''' </summary>
    Refresh_Token
End Enum

''' <summary>
''' This application demonstrates the usage of Advertisement API of AT&T platform. 
''' The Advertisement API is a service that returns advertisements enabling the developer to insert the advertisements into their application.
''' </summary>
Partial Public Class Ad_App1
    Inherits System.Web.UI.Page
#Region "Instance Variables"

    ''' <summary>
    ''' Application parameters.
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String, scope As String

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
    ''' Expirytimes of refresh and access tokens
    ''' </summary>
    Private refreshTokenExpiryTime As String, accessTokenExpiryTime As String

    ''' <summary>
    ''' No of hours in which refresh token expires.
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    ''' <summary>
    ''' UDID for Ad tracking purpose.
    ''' </summary>
    Private udid As String

    ''' <summary>
    ''' Specifies the AD type.
    ''' </summary>
    Private AdType As String = Nothing

    Public getAdsSuccessResponse As String = String.Empty
    Public getAdsErrorResponse As String = String.Empty
    Public adRequestResponse As AdsResponseObject = Nothing


#End Region

    Protected Sub BtnGetADS_Click(sender As Object, e As EventArgs)
        Me.GetAds()
    End Sub
    ''' <summary>
    ''' This function get the access token based on the type parameter type values.
    ''' If type value is 1, access token is fetch for client credential flow
    ''' If type value is 2, access token is fetch for client credential flow based on the exisiting refresh token
    ''' </summary>
    ''' <param name="type">Type as integer</param>
    ''' <param name="panelParam">Panel details</param>
    ''' <returns>Return boolean</returns>
    Private Function GetAccessToken(type As AccessType, ByRef message As String) As Boolean
        Dim fileStream As FileStream = Nothing
        Dim postStream As Stream = Nothing
        Dim streamWriter As StreamWriter = Nothing

        ' This is client credential flow
        If type = AccessType.ClientCredential Then
            Try
                Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
                accessTokenRequest.Method = "POST"
                Dim oauthParameters As String = String.Empty
                If type = AccessType.ClientCredential Then
                    oauthParameters = "client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&grant_type=client_credentials&scope=" & Me.scope
                Else
                    oauthParameters = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
                End If

                accessTokenRequest.ContentType = "application/x-www-form-urlencoded"

                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
                accessTokenRequest.ContentLength = postBytes.Length

                postStream = accessTokenRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)

                Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
                Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                    Dim jsonAccessToken As String = accessTokenResponseStream.ReadToEnd().ToString()
                    Dim deserializeJsonObject As New JavaScriptSerializer()

                    Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(jsonAccessToken, GetType(AccessTokenResponse)), AccessTokenResponse)

                    Me.accessToken = deserializedJsonObj.access_token
                    Me.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString()
                    Me.refreshToken = deserializedJsonObj.refresh_token

                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(Me.refreshTokenExpiresIn)

                    If deserializedJsonObj.expires_in.Equals("0") Then
                        Dim defaultAccessTokenExpiresIn As Integer = 100
                        ' In Yearsint yearsToAdd = 100;
                        Me.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongDateString() & " " & currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongTimeString()
                    End If

                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()

                    fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
                    streamWriter = New StreamWriter(fileStream)
                    streamWriter.WriteLine(Me.accessToken)
                    streamWriter.WriteLine(Me.accessTokenExpiryTime)
                    streamWriter.WriteLine(Me.refreshToken)
                    streamWriter.WriteLine(Me.refreshTokenExpiryTime)

                    ' Close and clean up the StreamReader
                    accessTokenResponseStream.Close()
                    Return True
                End Using
            Catch we As WebException
                Dim errorResponse As String = String.Empty

                Try
                    Using sr2 As New StreamReader(we.Response.GetResponseStream())
                        errorResponse = sr2.ReadToEnd()
                        sr2.Close()
                    End Using
                Catch
                    errorResponse = "Unable to get response"
                End Try

                message = errorResponse & Environment.NewLine & we.ToString()
            Catch ex As Exception
                message = ex.Message
                Return False
            Finally
                If postStream IsNot Nothing Then
                    postStream.Close()
                End If

                If streamWriter IsNot Nothing Then
                    streamWriter.Close()
                End If

                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        ElseIf type = AccessType.RefreshToken Then
            Try
                Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
                accessTokenRequest.Method = "POST"

                Dim oauthParameters As String = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
                accessTokenRequest.ContentType = "application/x-www-form-urlencoded"

                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
                accessTokenRequest.ContentLength = postBytes.Length

                postStream = accessTokenRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)

                Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
                Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                    Dim accessTokenJSon As String = accessTokenResponseStream.ReadToEnd().ToString()
                    Dim deserializeJsonObject As New JavaScriptSerializer()

                    Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(accessTokenJSon, GetType(AccessTokenResponse)), AccessTokenResponse)
                    Me.accessToken = deserializedJsonObj.access_token.ToString()
                    Dim accessTokenExpiryTime As DateTime = currentServerTime.AddMilliseconds(Convert.ToDouble(deserializedJsonObj.expires_in.ToString()))
                    Me.refreshToken = deserializedJsonObj.refresh_token.ToString()

                    fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
                    streamWriter = New StreamWriter(fileStream)
                    streamWriter.WriteLine(Me.accessToken)
                    streamWriter.WriteLine(Me.accessTokenExpiryTime)
                    streamWriter.WriteLine(Me.refreshToken)

                    ' Refresh token valids for 24 hours
                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(24)
                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()
                    streamWriter.WriteLine(refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString())

                    accessTokenResponseStream.Close()
                    Return True
                End Using
            Catch we As WebException
                Dim errorResponse As String = String.Empty

                Try
                    Using sr2 As New StreamReader(we.Response.GetResponseStream())
                        errorResponse = sr2.ReadToEnd()
                        sr2.Close()
                    End Using
                Catch
                    errorResponse = "Unable to get response"
                End Try

                message = errorResponse & Environment.NewLine & we.ToString()
            Catch ex As Exception
                message = ex.Message
                Return False
            Finally
                If postStream IsNot Nothing Then
                    postStream.Close()
                End If

                If streamWriter IsNot Nothing Then
                    streamWriter.Close()
                End If

                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        End If

        Return False
    End Function
    ''' <summary>
    ''' This function is used to read access token file and validate the access token
    ''' this function returns true if access token is valid, or else false is returned
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <returns>Returns Boolean</returns>
    Private Function ReadAndGetAccessToken(ByRef responseString As String) As Boolean
        Dim result As Boolean = True
        If Me.ReadAccessTokenFile(responseString) = False Then
            result = Me.GetAccessToken(AccessType.ClientCredential, responseString)
        Else
            Dim tokenValidity As String = Me.IsTokenValid()
            If tokenValidity = "REFRESH_TOKEN" Then
                result = Me.GetAccessToken(AccessType.RefreshToken, responseString)
            ElseIf String.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") = 0 Then
                result = Me.GetAccessToken(AccessType.ClientCredential, responseString)
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return False
        Else
            Return result
        End If
    End Function

    ''' <summary>
    ''' This function reads the Access Token File and stores the values of access token, expiry seconds
    ''' refresh token, last access token time and refresh token expiry time
    ''' This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <returns>Returns boolean</returns>    
    Private Function ReadAccessTokenFile(ByRef message As String) As Boolean
        Dim fileStream As FileStream = Nothing
        Dim streamReader As StreamReader = Nothing
        Try
            fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read)
            streamReader = New StreamReader(fileStream)
            Me.accessToken = streamReader.ReadLine()
            Me.accessTokenExpiryTime = streamReader.ReadLine()
            Me.refreshToken = streamReader.ReadLine()
            Me.refreshTokenExpiryTime = streamReader.ReadLine()
        Catch ex As Exception
            message = ex.Message
            Return False
        Finally
            If streamReader IsNot Nothing Then
                streamReader.Close()
            End If

            If fileStream IsNot Nothing Then
                fileStream.Close()
            End If
        End Try

        If (Me.accessToken Is Nothing) OrElse (Me.accessTokenExpiryTime Is Nothing) OrElse (Me.refreshToken Is Nothing) OrElse (Me.refreshTokenExpiryTime Is Nothing) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' This function validates the expiry of the access token and refresh token,
    ''' function compares the current time with the refresh token taken time, if current time is greater then 
    ''' returns INVALID_REFRESH_TOKEN
    ''' function compares the difference of last access token taken time and the current time with the expiry seconds, if its more,
    ''' funciton returns INVALID_ACCESS_TOKEN
    ''' otherwise returns VALID_ACCESS_TOKEN
    ''' </summary>
    ''' <returns>Return String</returns>
    Private Function IsTokenValid() As String
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()
            If currentServerTime >= DateTime.Parse(Me.accessTokenExpiryTime) Then
                If currentServerTime >= DateTime.Parse(Me.refreshTokenExpiryTime) Then
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

    Private Sub GetAds()
        Try
            Dim ableToGetAccessToken As Boolean = Me.ReadAndGetAccessToken(getAdsErrorResponse)
            If ableToGetAccessToken Then
                Dim adsResponse As String
                Dim queryString As String = Me.BuildQueryParameterString()
                Dim adsRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/1/ads?" & queryString), HttpWebRequest)
                adsRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                If String.IsNullOrEmpty(Me.udid) Then
                    adsRequest.Headers.Add("UDID", Guid.NewGuid().ToString())
                Else
                    adsRequest.Headers.Add("UDID", Me.udid)
                End If
                adsRequest.UserAgent = Request.UserAgent
                adsRequest.Accept = "application/json"
                adsRequest.Method = "GET"

                Dim adsResponseObject As HttpWebResponse = DirectCast(adsRequest.GetResponse(), HttpWebResponse)
                Using adResponseStream As New StreamReader(adsResponseObject.GetResponseStream())
                    adsResponse = adResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    adRequestResponse = DirectCast(deserializeJsonObject.Deserialize(adsResponse, GetType(AdsResponseObject)), AdsResponseObject)
                    If adRequestResponse IsNot Nothing AndAlso adRequestResponse.AdsResponse IsNot Nothing AndAlso adRequestResponse.AdsResponse.Ads IsNot Nothing Then
                        If ((adRequestResponse.AdsResponse.Ads.ImageUrl IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ImageUrl.Image))) Then
                            hplImage.ImageUrl = adRequestResponse.AdsResponse.Ads.ImageUrl.Image
                        End If
                        If Not String.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Text) Then
                            hplImage.ImageUrl = String.Empty
                            hplImage.Text = adRequestResponse.AdsResponse.Ads.Text
                        End If
                        If Not String.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ClickUrl) Then
                            hplImage.NavigateUrl = adRequestResponse.AdsResponse.Ads.ClickUrl
                        End If
                        getAdsSuccessResponse = " "
                    Else
                        getAdsSuccessResponse = "No ads returned"
                    End If
                    adResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            Dim errorResponse As String = String.Empty

            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
            Catch
                errorResponse = "Unable to get response"
            End Try

            getAdsErrorResponse = errorResponse & Environment.NewLine & we.Message
        Catch ex As Exception
            getAdsErrorResponse = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' Builds query string based on user input.
    ''' </summary>
    ''' <returns>string; query string to be passed along with API Request.</returns>
    Private Function BuildQueryParameterString() As String
        Dim queryParameter As String = String.Empty
        queryParameter = "Category=" + category.Value

        If Not String.IsNullOrEmpty(gender.Value) Then
            queryParameter += "&Gender=" + gender.Value
        End If

        If Not String.IsNullOrEmpty(zipCode.Value) Then
            queryParameter += "&ZipCode=" + zipCode.Value
        End If

        If Not String.IsNullOrEmpty(areaCode.Value) Then
            queryParameter += "&AreaCode=" + areaCode.Value
        End If

        If Not String.IsNullOrEmpty(city.Value) Then
            queryParameter += "&City=" + city.Value
        End If

        If Not String.IsNullOrEmpty(country.Value) Then
            queryParameter += "&Country=" + country.Value
        End If

        If Not String.IsNullOrEmpty(longitude.Value) Then
            queryParameter += "&Longitude=" + longitude.Value
        End If

        If Not String.IsNullOrEmpty(latitude.Value) Then
            queryParameter += "&Latitude=" + latitude.Value
        End If

        If Not String.IsNullOrEmpty(MMA.Value) Then
            Dim dimensions As String() = Regex.Split(MMA.Value, " x ")
            queryParameter += "&MaxWidth=" & dimensions(0)
            queryParameter += "&MaxHeight=" & dimensions(0)
            queryParameter += "&MinHeight=" & dimensions(1)
            queryParameter += "&MinWidth=" & dimensions(1)
        End If

        If Not String.IsNullOrEmpty(Me.AdType) Then
            queryParameter += "&Type=" & Me.AdType
        End If

        If Not String.IsNullOrEmpty(ageGroup.Value) Then
            queryParameter += "&AgeGroup=" + ageGroup.Value
        End If

        If Not String.IsNullOrEmpty(over18.Value) Then
            queryParameter += "&Over18=" + over18.Value
        End If

        If Not String.IsNullOrEmpty(keywords.Value) Then
            queryParameter += "&Keywords=" + keywords.Value
        End If

        If Not String.IsNullOrEmpty(Premium.Value) Then
            queryParameter += "&Premium=" + Premium.Value
        End If

        Return queryParameter
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs)

        'ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Me.BypassCertificateError()
        Me.ReadConfigFile()
        hplImage.ImageUrl = String.Empty
        hplImage.Text = String.Empty
    End Sub

    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub

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




    Private Function ReadConfigFile() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            getAdsErrorResponse = "apiKey is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            getAdsErrorResponse = "secretKey is not defined in configuration file"
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            getAdsErrorResponse = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "ADS"
        End If

        Me.udid = ConfigurationManager.AppSettings("udid")

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\AdsSApp1AccessToken.txt"
        End If

        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            Me.refreshTokenExpiresIn = 24
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SourceLink")) Then
            SourceLink.HRef = ConfigurationManager.AppSettings("SourceLink")
        Else
            ' Default value
            SourceLink.HRef = "#"
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DownloadLink")) Then
            DownloadLink.HRef = ConfigurationManager.AppSettings("DownloadLink")
        Else
            ' Default value
            DownloadLink.HRef = "#"
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("HelpLink")) Then
            HelpLink.HRef = ConfigurationManager.AppSettings("HelpLink")
        Else
            ' Default value
            HelpLink.HRef = "#"
        End If
        Return True
    End Function

    ''' <summary>
    ''' Access Token Types
    ''' </summary>
    Private Enum AccessType
        ''' <summary>
        ''' Access Token Type is based on Client Credential Mode
        ''' </summary>
        ClientCredential

        ''' <summary>
        ''' Access Token Type is based on Refresh Token
        ''' </summary>
        RefreshToken
    End Enum
End Class

#Region "Access Token Data Structure"

''' <summary>
''' AccessTokenResponse Object, returned upon calling get auth token api.
''' </summary>
Public Class AccessTokenResponse
    ''' <summary>
    ''' Gets or sets the value of access_token
    ''' </summary>
    Public Property access_token() As String
        Get
            Return m_access_token
        End Get
        Set(value As String)
            m_access_token = Value
        End Set
    End Property
    Private m_access_token As String

    ''' <summary>
    ''' Gets or sets the value of refresh_token
    ''' </summary>
    Public Property refresh_token() As String
        Get
            Return m_refresh_token
        End Get
        Set(value As String)
            m_refresh_token = Value
        End Set
    End Property
    Private m_refresh_token As String

    ''' <summary>
    ''' Gets or sets the value of expires_in
    ''' </summary>
    Public Property expires_in() As String
        Get
            Return m_expires_in
        End Get
        Set(value As String)
            m_expires_in = Value
        End Set
    End Property
    Private m_expires_in As String
End Class

#End Region

#Region "AdResponse Data Structures"

''' <summary>
''' Object that containes the link to the image of the advertisement and tracking Url.
''' </summary>
Public Class ImageUrlResponse
    ''' <summary>
    ''' Gets or sets the value of Image.
    ''' This parameter returns the link to the image of the advertisement.
    ''' </summary>
    Public Property Image() As String
        Get
            Return m_Image
        End Get
        Set(value As String)
            m_Image = Value
        End Set
    End Property
    Private m_Image As String

    ''' <summary>
    ''' Gets or sets the value of Track.
    ''' This parameter contains the pixel tracking URL.
    ''' </summary>
    Public Property Track() As String
        Get
            Return m_Track
        End Get
        Set(value As String)
            m_Track = Value
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
    Public Property Type() As String
        Get
            Return m_Type
        End Get
        Set(value As String)
            m_Type = Value
        End Set
    End Property
    Private m_Type As String

    ''' <summary>
    ''' Gets or sets the value of ClickUrl.
    ''' This parameter contains the click URLs. It returns all the possible sizes available. 
    ''' For SMS ads, the URL is shortened to 35-40 characters.
    ''' </summary>
    Public Property ClickUrl() As String
        Get
            Return m_ClickUrl
        End Get
        Set(value As String)
            m_ClickUrl = Value
        End Set
    End Property
    Private m_ClickUrl As String

    ''' <summary>
    ''' Gets or sets the value of ImageUrl.
    ''' This parameter returns the link to the image of the advertisement.
    ''' </summary>
    Public Property ImageUrl() As ImageUrlResponse
        Get
            Return m_ImageUrl
        End Get
        Set(value As ImageUrlResponse)
            m_ImageUrl = Value
        End Set
    End Property
    Private m_ImageUrl As ImageUrlResponse

    ''' <summary>
    ''' Gets or sets the value of Text.
    ''' Any ad text(either independent or below the ad)
    ''' </summary>
    Public Property Text() As String
        Get
            Return m_Text
        End Get
        Set(value As String)
            m_Text = Value
        End Set
    End Property
    Private m_Text As String

    ''' <summary>
    ''' Gets or sets the value of TrackUrl.
    ''' This parameter contains the pixel tracking URL.
    ''' </summary>
    Public Property TrackUrl() As String
        Get
            Return m_TrackUrl
        End Get
        Set(value As String)
            m_TrackUrl = Value
        End Set
    End Property
    Private m_TrackUrl As String

    ''' <summary>
    ''' Gets or sets the value of Content
    ''' All of the ad content is placed in this node as is from 3rd party.
    ''' </summary>
    Public Property Content() As String
        Get
            Return m_Content
        End Get
        Set(value As String)
            m_Content = Value
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
    Public Property Ads() As Ad
        Get
            Return m_Ads
        End Get
        Set(value As Ad)
            m_Ads = Value
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
    Public Property AdsResponse() As AdResponse
        Get
            Return m_AdsResponse
        End Get
        Set(value As AdResponse)
            m_AdsResponse = Value
        End Set
    End Property
    Private m_AdsResponse As AdResponse
End Class

#End Region