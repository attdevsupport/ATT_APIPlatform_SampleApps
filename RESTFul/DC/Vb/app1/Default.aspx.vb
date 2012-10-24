' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "Application References"

Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web.Script.Serialization

#End Region

''' <summary>
''' Access Token Types
''' </summary>
Public Enum AccessTokenType
    ''' <summary>
    ''' Access Token Type is based on Authorize Credential Mode
    ''' </summary>
    Authorize_Credential

    ''' <summary>
    ''' Access Token Type is based on Refresh Token
    ''' </summary>
    Refresh_Token
End Enum

''' <summary>
''' Device Capabilities Application class.
''' This application will allow an user to get the device capabilities.
''' </summary>
Partial Public Class DC_App1
    Inherits System.Web.UI.Page
#Region "Application Instance Variables"

    ''' <summary>
    ''' Instance variables
    ''' </summary>
    Private endPoint As String, apiKey As String, secretKey As String, authorizeRedirectUri As String, authCode As String, scope As String

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

#End Region

#Region "Neglect SSL error"
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

#Region "Application Events"

    ''' <summary>
    ''' This method will be executed upon loading of the page.
    ''' Reads the config file and assigns to local variables.
    ''' </summary>
    ''' <param name="sender">sender object</param>
    ''' <param name="e">event arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
            tbDeviceCapabSuccess.Visible = False
            tbDeviceCapabilities.Visible = False
            tbDeviceCapabError.Visible = False
            lblServerTime.Text = DateTime.UtcNow.ToString("ddd MMM dd yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) & " UTC"

            Dim ableToReadConfig As Boolean = Me.ReadConfigFile()
            If ableToReadConfig = False Then
                Return
            End If

            If Not IsPostBack Then
                If Request.QueryString("error") IsNot Nothing AndAlso Session("vb_dc_state") IsNot Nothing Then
                    Session("vb_dc_state") = Nothing
                    Dim errorString As String = Request.Url.Query.Remove(0, 1)
                    lblErrorMessage.Text = HttpUtility.UrlDecode(errorString)
                    tbDeviceCapabError.Visible = True
                    Return
                End If
                If Session("Vb_DC_App1_AccessToken") Is Nothing Then
                    'Retrive the query string 'code' from redirect response of AT&T authorization endpoint
                    ' get the AccessToken and device info
                    If Not String.IsNullOrEmpty(Convert.ToString(Request("code"))) Then
                        authCode = Request("code").ToString()
                        If GetAccessToken(AccessTokenType.Authorize_Credential) Then
                            GetDeviceInfo()
                        Else
                            lblErrorMessage.Text = "Failed to get Access token"
                            tbDeviceCapabError.Visible = True

                        End If
                    Else
                        Session("Vb_DC_App1_AccessToken") = Nothing
                        GetAuthCode()
                    End If
                Else
                    GetDeviceInfo()
                End If
            Else
                Session("Vb_DC_App1_AccessToken") = Nothing
                GetAuthCode()
            End If
        Catch ex As Exception
            lblErrorMessage.Text = ex.ToString()
            tbDeviceCapabError.Visible = True
        End Try
    End Sub

#End Region

#Region "Application specific methods"

    ''' <summary>
    ''' Read config file and assign to local variables
    ''' </summary>
    ''' <remarks>
    ''' <para>Validates if the values are configured in web.config file and displays a warning message if not configured.</para>
    ''' </remarks>
    ''' <returns>true/false; true if able to read and assign; else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            lblErrorMessage.Text = "endPoint is not defined in configuration file"
            tbDeviceCapabError.Visible = True
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            lblErrorMessage.Text = "apiKey is not defined in configuration file"
            tbDeviceCapabError.Visible = True
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            lblErrorMessage.Text = "secretKey is not defined in configuration file"
            tbDeviceCapabError.Visible = True
            Return False
        End If

        Me.authorizeRedirectUri = ConfigurationManager.AppSettings("authorizeRedirectUri")
        If String.IsNullOrEmpty(Me.authorizeRedirectUri) Then
            lblErrorMessage.Text = "authorizeRedirectUri is not defined in configuration file"
            tbDeviceCapabError.Visible = True
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "DC"
        End If

        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            Me.refreshTokenExpiresIn = 24
        End If

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "DCApp1AccessToken.txt"
        End If

        Return True
    End Function





    ''' <summary>
    ''' This method gets access token based on either client credentials mode or refresh token.
    ''' </summary>
    ''' <param name="type">AccessTokenType; either Client_Credential or Refresh_Token</param>
    ''' <returns>true/false; true if able to get access token, else false</returns>
    Private Function GetAccessToken(type As AccessTokenType) As Boolean
        Dim postStream As Stream = Nothing
        Dim streamWriter As StreamWriter = Nothing
        Dim fileStream As FileStream = Nothing
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()

            Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
            accessTokenRequest.Method = "POST"

            Dim oauthParameters As String = String.Empty
            If type = AccessTokenType.Authorize_Credential Then
                oauthParameters = "client_id=" & Me.apiKey.ToString() & "&client_secret=" & Me.secretKey & "&code=" & Me.authCode & "&grant_type=authorization_code&scope=" & Me.scope
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
                Dim jsonAccessToken As String = accessTokenResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(jsonAccessToken, GetType(AccessTokenResponse)), AccessTokenResponse)
                Dim accessTokenExpireTime As DateTime = DateTime.Now
                Session("Vb_DC_App1_AccessToken") = deserializedJsonObj.access_token.ToString()
                Dim expiryMilliSeconds As Double = Convert.ToDouble(deserializedJsonObj.expires_in)
                Session("refresh_token") = deserializedJsonObj.refresh_token.ToString()
                If expiryMilliSeconds = 0 Then
                    Session("Vb_accessTokenExpireTime") = accessTokenExpireTime.AddYears(100)
                Else
                    Session("Vb_accessTokenExpireTime") = accessTokenExpireTime.AddMilliseconds(expiryMilliSeconds)
                End If
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

            lblErrorMessage.Text = errorResponse & Environment.NewLine & we.Message
            tbDeviceCapabError.Visible = True
        Catch ex As Exception
            lblErrorMessage.Text = ex.Message
            tbDeviceCapabError.Visible = True
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

        Return False
    End Function

    ''' <summary>
    ''' Redirects to authentication server to get the access code
    ''' </summary>
    Private Sub GetAuthCode()
        Session("vb_dc_state") = "FetchAuthCode"
        Response.Redirect(Me.endPoint & "/oauth/authorize?scope=" & Me.scope & "&client_id=" & Me.apiKey & "&redirect_url=" & Me.authorizeRedirectUri)
    End Sub

    ''' <summary>
    ''' This method invokes DeviceInfo API of AT&amp;T platform to get the device information.
    ''' </summary>
    Private Sub GetDeviceInfo()
        Try
            Dim deviceInfoRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(Me.endPoint & "/rest/2/Devices/Info"), HttpWebRequest)
            deviceInfoRequestObject.Method = "GET"
            deviceInfoRequestObject.Headers.Add("Authorization", "Bearer " & Convert.ToString(Session("Vb_DC_App1_AccessToken")))

            Dim deviceInfoResponse As HttpWebResponse = DirectCast(deviceInfoRequestObject.GetResponse(), HttpWebResponse)
            Using accessTokenResponseStream As New StreamReader(deviceInfoResponse.GetResponseStream())
                Dim deviceInfo_jsonObj As String = accessTokenResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As DeviceCapabilities = DirectCast(deserializeJsonObject.Deserialize(deviceInfo_jsonObj, GetType(DeviceCapabilities)), DeviceCapabilities)
                If deserializedJsonObj IsNot Nothing Then
                    lblTypeAllocationCode.Text = deserializedJsonObj.DeviceInfo.DeviceId.TypeAllocationCode
                    lblName.Text = deserializedJsonObj.DeviceInfo.Capabilities.Name
                    lblVendor.Text = deserializedJsonObj.DeviceInfo.Capabilities.Vendor
                    lblFirmwareVersion.Text = deserializedJsonObj.DeviceInfo.Capabilities.FirmwareVersion
                    lblUAProf.Text = deserializedJsonObj.DeviceInfo.Capabilities.UaProf
                    lblMMSCapable.Text = deserializedJsonObj.DeviceInfo.Capabilities.MmsCapable
                    lblAGPS.Text = deserializedJsonObj.DeviceInfo.Capabilities.AssistedGps
                    lblLocationTechnology.Text = deserializedJsonObj.DeviceInfo.Capabilities.LocationTechnology
                    lblDeviceBrowser.Text = deserializedJsonObj.DeviceInfo.Capabilities.DeviceBrowser
                    lblWAPPush.Text = deserializedJsonObj.DeviceInfo.Capabilities.WapPushCapable
                    tbDeviceCapabSuccess.Visible = True
                    tbDeviceCapabilities.Visible = True
                Else
                    lblErrorMessage.Text = "No response from the platform."
                    tbDeviceCapabError.Visible = True
                End If
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

            lblErrorMessage.Text = errorResponse & Environment.NewLine & we.Message
            tbDeviceCapabError.Visible = True
        Catch ex As Exception
            lblErrorMessage.Text = ex.Message
            tbDeviceCapabError.Visible = True
        End Try
    End Sub

#End Region
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

#Region "Device Capabilities Response Structure"

''' <summary>
''' This object is used to return the Device Capabilities details from the network for a particular mobile terminal.
''' </summary>
Public Class DeviceCapabilities
    ''' <summary>
    ''' Gets or sets the value of DeviceInfo object.
    ''' This Root Object will return two nested objects i-e DeviceId and Capabilities.
    ''' </summary>
    Public Property DeviceInfo() As DeviceInfo
        Get
            Return m_DeviceInfo
        End Get
        Set(value As DeviceInfo)
            m_DeviceInfo = Value
        End Set
    End Property
    Private m_DeviceInfo As DeviceInfo
End Class

''' <summary>
''' Encapsulates the device capabilities response from API gateway.
''' </summary>
Public Class DeviceInfo
    ''' <summary>
    ''' Gets or sets the value of DeviceId.        
    ''' </summary>
    Public Property DeviceId() As DeviceId
        Get
            Return m_DeviceId
        End Get
        Set(value As DeviceId)
            m_DeviceId = Value
        End Set
    End Property
    Private m_DeviceId As DeviceId

    ''' <summary>
    ''' Gets or sets the value of Capabilities object.
    ''' </summary>
    Public Property Capabilities() As Capabilities
        Get
            Return m_Capabilities
        End Get
        Set(value As Capabilities)
            m_Capabilities = Value
        End Set
    End Property
    Private m_Capabilities As Capabilities
End Class

''' <summary>
''' Encapsulates the first 8 digits of the IMEI of the mobile device.
''' </summary>
Public Class DeviceId
    ''' <summary>
    ''' Gets or sets the value of TypeAllocationCode.
    ''' This will return ONLY the first 8 digits of the International Mobile Equipment Identity of the mobile device in question.
    ''' </summary>
    Public Property TypeAllocationCode() As String
        Get
            Return m_TypeAllocationCode
        End Get
        Set(value As String)
            m_TypeAllocationCode = Value
        End Set
    End Property
    Private m_TypeAllocationCode As String
End Class

''' <summary>
''' Capabilities object is returned as part of Device #Capabilities AT&amp;T's API.
''' The Device Capabilities attributes, which include Name, Vendor, Model, FirmwareVersion,
''' UaProf, MmsCapable, AssistedGps, Location Technology, Device Browser and WAP Push Capable to
''' allow developers to deliver applications that are tailored to a specific customer device.
''' </summary>
Public Class Capabilities
    ''' <summary>
    ''' Gets or sets the value of Name.
    ''' AT&amp;T's abbreviated code for the mobile device manufacturer and model number. These may or may not correspond to the device manufacturer's name and model number provided in the href.        
    ''' </summary>
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = Value
        End Set
    End Property
    Private m_Name As String

    ''' <summary>
    ''' Gets or sets the value of Vendor.
    ''' AT&amp;T's abbreviated code for the mobile device manufacturer.
    ''' </summary>
    Public Property Vendor() As String
        Get
            Return m_Vendor
        End Get
        Set(value As String)
            m_Vendor = Value
        End Set
    End Property
    Private m_Vendor As String

    ''' <summary>
    ''' Gets or sets the value of Model.
    ''' AT&amp;T's model number for the mobile device.
    ''' </summary>
    Public Property Model() As String
        Get
            Return m_Model
        End Get
        Set(value As String)
            m_Model = Value
        End Set
    End Property
    Private m_Model As String

    ''' <summary>
    ''' Gets or sets the value of FirmwareVersion.
    ''' AT&amp;T's shall map the AT&amp;T's specific device firmware release number (if available) to this parameter. 
    ''' This may or may not correspond to the mobile device manufacturer's firmware release numbers.
    ''' </summary>
    Public Property FirmwareVersion() As String
        Get
            Return m_FirmwareVersion
        End Get
        Set(value As String)
            m_FirmwareVersion = Value
        End Set
    End Property
    Private m_FirmwareVersion As String

    ''' <summary>
    ''' Gets or sets the value of UaProf.
    ''' Contains a URL to the device manufacturer website which may provide specific 
    ''' capability details for the mobile device in the request. 
    ''' </summary>
    Public Property UaProf() As String
        Get
            Return m_UaProf
        End Get
        Set(value As String)
            m_UaProf = Value
        End Set
    End Property
    Private m_UaProf As String

    ''' <summary>
    ''' Gets or sets the value of MmsCapable.
    ''' A value that indicates whether the device is MMS capable “Y” or “N”.
    ''' </summary>
    Public Property MmsCapable() As String
        Get
            Return m_MmsCapable
        End Get
        Set(value As String)
            m_MmsCapable = Value
        End Set
    End Property
    Private m_MmsCapable As String

    ''' <summary>
    ''' Gets or sets the value of AssistedGps.
    ''' A value that indicates whether the device is assisted-GPS capable “Y” or “N”.
    ''' </summary>
    Public Property AssistedGps() As String
        Get
            Return m_AssistedGps
        End Get
        Set(value As String)
            m_AssistedGps = Value
        End Set
    End Property
    Private m_AssistedGps As String

    ''' <summary>
    ''' Gets or sets the value of LocationTechnology.
    ''' A value that indicates the location technology network that is supported by the device. 
    ''' The value is expressed in terms of network Location service technology types (i.e. “SUPL” or “SUPL2”) as supporting the Location query on 3G devices and 4G (LTE) devices respectively. 
    ''' </summary>
    Public Property LocationTechnology() As String
        Get
            Return m_LocationTechnology
        End Get
        Set(value As String)
            m_LocationTechnology = Value
        End Set
    End Property
    Private m_LocationTechnology As String

    ''' <summary>
    ''' Gets or sets the value of DeviceBrowser.
    ''' A value that indicates the name of the browser that is resident on the device e.g. “RIM” for Blackberry devices.
    ''' </summary>
    Public Property DeviceBrowser() As String
        Get
            Return m_DeviceBrowser
        End Get
        Set(value As String)
            m_DeviceBrowser = Value
        End Set
    End Property
    Private m_DeviceBrowser As String

    ''' <summary>
    ''' Gets or sets the value of WAPPushCapable.
    ''' A value that indicates whether the device is WAP Push capable “Y” or “N”.
    ''' </summary>
    Public Property WapPushCapable() As String
        Get
            Return m_WapPushCapable
        End Get
        Set(value As String)
            m_WapPushCapable = Value
        End Set
    End Property
    Private m_WapPushCapable As String
End Class
#End Region
