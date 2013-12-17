' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Configuration
Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Web.UI.WebControls

#End Region

''' <summary>
''' Access Token Types
''' </summary>
Public Enum AccessTokenType
    ''' <summary>
    ''' Access Token Type is based on Authorization Code
    ''' </summary>
    Authorization_Code

    ''' <summary>
    ''' Access Token Type is based on Refresh Token
    ''' </summary>
    Refresh_Token
End Enum

''' <summary>
''' TL_App1 class
''' </summary>
Partial Public Class TL_App1
    Inherits System.Web.UI.Page
#Region "Local variables"

    ''' <summary>
    ''' Gets or sets the value of endPoint
    ''' </summary>
    Private endPoint As String

    ''' <summary>
    ''' Access Token Variables
    ''' </summary>
    Private apiKey As String, secretKey As String, accessToken As String, authorizeRedirectUri As String, scope As String, refreshToken As String, _
     accessTokenExpiryTime As String, refreshTokenExpiryTime As String

    ''' <summary>
    ''' Gets or sets the value of authCode
    ''' </summary>
    Private authCode As String

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    ''' <summary>
    ''' Gets or sets the Status Table
    ''' </summary>
    Private getStatusTable As Table

    Public getLocationSuccess As String = String.Empty
    Public getLocationError As String = String.Empty
    Public getLocationResponse As TLResponse
    Public responseTime As String = String.Empty

#End Region

#Region "SSL Handshake Error"

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

#End Region

#Region "Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This function reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Try
            'ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
            Me.BypassCertificateError()
            Dim ableToRead As Boolean = Me.ReadConfigFile()
            If Not ableToRead Then
                Return
            End If

            If Not IsPostBack AndAlso (Session("VbRestTLSelectedValues") Is Nothing) Then
                RA2.Checked = True
                AA3.Checked = True
                DT2.Checked = True
            End If
            If (Session("vb_tl_session_appState") Is "GetToken") AndAlso (Request("Code") IsNot Nothing) Then
                Me.authCode = Request("code")
                Dim ableToGetToken As Boolean = Me.GetAccessToken(AccessTokenType.Authorization_Code)
                FetchSelectedValuesFromSessionVariables()
                Session("VbRestTLSelectedValues") = Nothing
                If ableToGetToken Then

                    Me.GetDeviceLocation()
                Else
                    getLocationError = "Failed to get Access token"
                    Me.ResetTokenSessionVariables()
                    Me.ResetTokenVariables()
                End If
            End If
        Catch ex As Exception
            getLocationError = ex.ToString()
        End Try
    End Sub

    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub

    ''' <summary>
    ''' Event that will be triggered when the user clicks on GetPhoneLocation button
    ''' This method calls GetDeviceLocation Api
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub GetDeviceLocation_Click(sender As Object, e As EventArgs)
        Try
            Dim ableToGetAccessToken As Boolean = Me.ReadAndGetAccessToken()
            If ableToGetAccessToken Then
                Me.GetDeviceLocation()
            Else
                getLocationError = "Unable to get access token"
            End If
        Catch ex As Exception
            getLocationError = ex.Message
        End Try
    End Sub

#End Region

#Region "session and radio buttons"

    Public Sub FetchSelectedValuesFromSessionVariables()
        Dim sessionValue As String = Session("VbRestTLSelectedValues").ToString()

        Dim selectedValues As String() = sessionValue.Split(";"c)
        If Not String.IsNullOrEmpty(selectedValues(0)) Then
            If selectedValues(0).CompareTo("AA1") = 0 Then
                AA1.Checked = True
            ElseIf selectedValues(0).CompareTo("AA2") = 0 Then
                AA2.Checked = True
            ElseIf selectedValues(0).CompareTo("AA3") = 0 Then
                AA3.Checked = True
            End If
        End If

        If Not String.IsNullOrEmpty(selectedValues(1)) Then
            If selectedValues(1).CompareTo("RA1") = 0 Then
                RA1.Checked = True
            ElseIf selectedValues(1).CompareTo("RA2") = 0 Then
                RA2.Checked = True
            ElseIf selectedValues(1).CompareTo("RA3") = 0 Then
                RA3.Checked = True
            End If
        End If

        If Not String.IsNullOrEmpty(selectedValues(2)) Then
            If selectedValues(2).CompareTo("DT1") = 0 Then
                DT1.Checked = True
            ElseIf selectedValues(2).CompareTo("DT2") = 0 Then
                DT2.Checked = True
            ElseIf selectedValues(2).CompareTo("DT3") = 0 Then
                DT3.Checked = True
            End If
        End If
    End Sub
    Public Sub StoreSelectedValuesToSessionVariables()
        Dim selectedValues As String = String.Empty
        If AA1.Checked Then
            selectedValues = selectedValues & "AA1"
        ElseIf AA2.Checked Then
            selectedValues = selectedValues & "AA2"
        ElseIf AA3.Checked Then
            selectedValues = selectedValues & "AA3"
        End If
        selectedValues = selectedValues & ";"
        If RA1.Checked Then
            selectedValues = selectedValues & "RA1"
        ElseIf RA2.Checked Then
            selectedValues = selectedValues & "RA2"
        ElseIf RA3.Checked Then
            selectedValues = selectedValues & "RA3"
        End If
        selectedValues = selectedValues & ";"
        If DT1.Checked Then
            selectedValues = selectedValues & "DT1"
        ElseIf DT2.Checked Then
            selectedValues = selectedValues & "DT2"
        ElseIf DT3.Checked Then
            selectedValues = selectedValues & "DT3"
        End If

        Session("VbRestTLSelectedValues") = selectedValues
    End Sub

    Public Function getAcceptableAccuracy() As Integer
        If AA1.Checked Then
            Return Convert.ToInt32(AA1.Value)
        ElseIf AA2.Checked Then
            Return Convert.ToInt32(AA2.Value)
        End If
        Return Convert.ToInt32(AA3.Value)
    End Function
    Public Function getRequestedAccuracry() As Integer
        If RA1.Checked Then
            Return Convert.ToInt32(RA1.Value)
        ElseIf RA2.Checked Then
            Return Convert.ToInt32(RA2.Value)
        End If
        Return Convert.ToInt32(RA3.Value)
    End Function
    Public Function getDelayTolerance() As String
        If DT1.Checked Then
            Return DT1.Value
        ElseIf DT2.Checked Then
            Return DT2.Value
        End If
        Return DT3.Value
    End Function
#End Region

#Region "API Invokation"

    ''' <summary>
    ''' This method invokes Device Location API and displays the location
    ''' </summary>
    Private Sub GetDeviceLocation()
        Try

            Dim requestedAccuracyVal As Integer, acceptableAccuracyVal As Integer
            Dim toleranceVal As String

            acceptableAccuracyVal = getAcceptableAccuracy()
            requestedAccuracyVal = getRequestedAccuracry()
            toleranceVal = getDelayTolerance()

            Dim strResult As String

            Dim webRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/2/devices/location?requestedAccuracy=" & requestedAccuracyVal & "&acceptableAccuracy=" & acceptableAccuracyVal & "&tolerance=" & toleranceVal), HttpWebRequest)
            webRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
            webRequest.Method = "GET"

            Dim msgSentTime As DateTime = DateTime.UtcNow.ToLocalTime()
            Dim webResponse As HttpWebResponse = DirectCast(webRequest.GetResponse(), HttpWebResponse)
            Dim msgReceivedTime As DateTime = DateTime.UtcNow.ToLocalTime()
            Dim tokenSpan As TimeSpan = msgReceivedTime.Subtract(msgSentTime)

            Using responseStream As New StreamReader(webResponse.GetResponseStream())
                strResult = responseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                getLocationResponse = DirectCast(deserializeJsonObject.Deserialize(strResult, GetType(TLResponse)), TLResponse)
                responseTime = tokenSpan.Seconds.ToString()
                getLocationSuccess = "Success"
                responseStream.Close()
            End Using
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    Dim streamReader As New StreamReader(stream)
                    getLocationError = streamReader.ReadToEnd()
                    streamReader.Close()
                End Using
            End If
        Catch ex As Exception
            getLocationError = ex.Message
        End Try
    End Sub

#End Region

#Region "Access Token Methods"

    ''' <summary>
    ''' Reads from session variables and gets access token
    ''' </summary>
    ''' <returns>true/false; true on successfully getting access token, else false</returns>
    Private Function ReadAndGetAccessToken() As Boolean
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()
        If tokentResult.Equals("INVALID_ACCESS_TOKEN") Then
            StoreSelectedValuesToSessionVariables()
            Session("vb_tl_session_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.Equals("REFRESH_TOKEN") Then
            Dim ableToGetToken As Boolean = Me.GetAccessToken(AccessTokenType.Refresh_Token)
            If ableToGetToken = False Then
                getLocationError = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return False
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' This function reads access token related session variables to local variables 
    ''' </summary>
    Private Sub ReadTokenSessionVariables()
        Me.accessToken = String.Empty
        If Session("vb_tl_session_access_token") IsNot Nothing Then
            Me.accessToken = Session("vb_tl_session_access_token").ToString()
        End If

        Me.refreshToken = Nothing
        If Session("vb_tl_session_refresh_token") IsNot Nothing Then
            Me.refreshToken = Session("vb_tl_session_refresh_token").ToString()
        End If

        Me.accessTokenExpiryTime = Nothing
        If Session("vb_tl_session_accessTokenExpiryTime") IsNot Nothing Then
            Me.accessTokenExpiryTime = Session("vb_tl_session_accessTokenExpiryTime").ToString()
        End If

        Me.refreshTokenExpiryTime = Nothing
        If Session("vb_tl_session_refreshTokenExpiryTime") IsNot Nothing Then
            Me.refreshTokenExpiryTime = Session("vb_tl_session_refreshTokenExpiryTime").ToString()
        End If
    End Sub

    ''' <summary>
    ''' This function resets access token related session variable to null 
    ''' </summary>
    Private Sub ResetTokenSessionVariables()
        Session("vb_tl_session_access_token") = Nothing
        Session("vb_tl_session_refresh_token") = Nothing
        Session("vb_tl_session_accessTokenExpiryTime") = Nothing
        Session("vb_tl_session_refreshTokenExpiryTime") = Nothing
    End Sub

    ''' <summary>
    ''' This function resets access token related  variable to null 
    ''' </summary>
    Private Sub ResetTokenVariables()
        Me.accessToken = Nothing
        Me.refreshToken = Nothing
        Me.accessTokenExpiryTime = Nothing
        Me.refreshTokenExpiryTime = Nothing
    End Sub

    ''' <summary>
    ''' This function validates access token related variables and returns VALID_ACCESS_TOKEN if its valid
    ''' otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    ''' return REFRESH_TOKEN, if access token in expired and refresh token is valid 
    ''' </summary>
    ''' <returns>string variable containing valid/invalid access/refresh token</returns>
    Private Function IsTokenValid() As String
        If Session("vb_tl_session_access_token") Is Nothing Then
            Return "INVALID_ACCESS_TOKEN"
        End If

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

    ''' <summary>
    ''' Redirects to authentication server to get the access code
    ''' </summary>
    Private Sub GetAuthCode()
        Response.Redirect(String.Empty & Me.endPoint & "/oauth/authorize?scope=" & Me.scope & "&client_id=" & Me.apiKey & "&redirect_url=" & Me.authorizeRedirectUri)
    End Sub

    ''' <summary>
    ''' Get access token based on the type parameter type values.
    ''' </summary>
    ''' <param name="type">If type value is Authorization_code, access token is fetch for authorization code flow
    ''' If type value is Refresh_Token, access token is fetch for authorization code floww based on the exisiting refresh token</param>
    ''' <returns>true/false; true if success, else false</returns>
    Private Function GetAccessToken(type As AccessTokenType) As Boolean
        Dim postStream As Stream = Nothing
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()
            Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
            accessTokenRequest.Method = "POST"

            Dim oauthParameters As String = String.Empty

            If type = AccessTokenType.Authorization_Code Then
                oauthParameters = "client_id=" & Me.apiKey.ToString() & "&client_secret=" & Me.secretKey & "&code=" & Me.authCode & "&grant_type=authorization_code&scope=" & Me.scope
            ElseIf type = AccessTokenType.Refresh_Token Then
                oauthParameters = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
            End If

            accessTokenRequest.ContentType = "application/x-www-form-urlencoded"
            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
            accessTokenRequest.ContentLength = postBytes.Length
            postStream = accessTokenRequest.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
            Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                Dim access_token_json As String = accessTokenResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(access_token_json, GetType(AccessTokenResponse)), AccessTokenResponse)
                If deserializedJsonObj.access_token IsNot Nothing Then
                    Me.accessToken = deserializedJsonObj.access_token
                    Me.refreshToken = deserializedJsonObj.refresh_token
                    Me.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString()

                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(Me.refreshTokenExpiresIn)

                    If deserializedJsonObj.expires_in.Equals("0") Then
                        Dim defaultAccessTokenExpiresIn As Integer = 100
                        ' In Years
                        Me.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToString()
                    End If

                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()

                    Session("vb_tl_session_access_token") = Me.accessToken
                    Session("vb_tl_session_refresh_token") = Me.refreshToken
                    Session("vb_tl_session_accessTokenExpiryTime") = Me.accessTokenExpiryTime
                    Session("vb_tl_session_refreshTokenExpiryTime") = Me.refreshTokenExpiryTime
                    Session("vb_tl_session_appState") = "TokenReceived"

                    accessTokenResponseStream.Close()
                    Return True
                Else
                    getLocationError = "Auth server returned null access token"
                End If
            End Using
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    Dim streamReader As New StreamReader(stream)
                    getLocationError = streamReader.ReadToEnd()
                    streamReader.Close()
                End Using
            End If
        Catch ex As Exception
            getLocationError = ex.Message
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Read parameters from configuraton file
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            getLocationError = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            getLocationError = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            getLocationError = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.authorizeRedirectUri = ConfigurationManager.AppSettings("authorize_redirect_uri")
        If String.IsNullOrEmpty(Me.authorizeRedirectUri) Then
            getLocationError = "authorize_redirect_uri is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "TL"
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

#End Region

End Class

#Region "Data Structures"

''' <summary>
''' Access Token Data Structure
''' </summary>
Public Class AccessTokenResponse
    ''' <summary>
    ''' Gets or sets Access Token ID
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
    ''' Gets or sets Refresh Token ID
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
    ''' Gets or sets Expires in milli seconds
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

''' <summary>
''' Terminal Location Response object
''' </summary>
Public Class TLResponse
    ''' <summary>
    ''' Gets or sets the value of accuracy - This is the target MSISDN that was used in the Device Location request
    ''' </summary>
    Public Property accuracy() As String
        Get
            Return m_accuracy
        End Get
        Set(value As String)
            m_accuracy = Value
        End Set
    End Property
    Private m_accuracy As String

    ''' <summary>
    ''' Gets or sets the value of latitude - The current latitude of the device's geo-position.
    ''' </summary>
    Public Property latitude() As String
        Get
            Return m_latitude
        End Get
        Set(value As String)
            m_latitude = Value
        End Set
    End Property
    Private m_latitude As String

    ''' <summary>
    ''' Gets or sets the value of longitude - The current longitude of the device geo-position.
    ''' </summary>
    Public Property longitude() As String
        Get
            Return m_longitude
        End Get
        Set(value As String)
            m_longitude = Value
        End Set
    End Property
    Private m_longitude As String

    ''' <summary>
    ''' Gets or sets the value of timestamp - Timestamp of the location data.
    ''' </summary>
    Public Property timestamp() As String
        Get
            Return m_timestamp
        End Get
        Set(value As String)
            m_timestamp = Value
        End Set
    End Property
    Private m_timestamp As String
End Class
#End Region