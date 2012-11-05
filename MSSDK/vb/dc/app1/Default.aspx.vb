' <copyright file="Default.aspx.vb" company="AT&amp;T Intellectual Property">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "Application References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.UI
Imports ATT_MSSDK
Imports ATT_MSSDK.DeviceCapabilitiesv2
#End Region

''' <summary>
''' Device Capabilities sample application
''' </summary>
Partial Public Class DC_App1
    Inherits System.Web.UI.Page
#Region "Instance Variables"
    ''' <summary>
    ''' Instance variables
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String, redirectUrl As String

    ''' <summary>
    ''' RequestFactory instance variable
    ''' </summary>
    Private requestFactory As RequestFactory

#End Region

#Region "Application Events"

    ''' <summary>
    ''' This event gets triggered when the page is loaded into the client browser.
    ''' This will perform the following activities:
    ''' 1. Reads config file and assigns configuration values to instance variables.
    ''' 2. Initializes an instance of RequestFactory class.
    ''' 3. Checks for Auth code and exchanges the auth code for getting access token.
    ''' 4. Call GetDeviceCapabilities().
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = (String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC")
        Dim ableToReadConfigFile As Boolean = Me.ReadConfigFile()
        If Not ableToReadConfigFile Then
            tbDeviceCapabError.Visible = True
            Return
        End If

        Me.InitializeRequestFactory()

        Try
            If Not Page.IsPostBack Then
                If Session("VBDC_ACCESS_TOKEN") Is Nothing Then
                    If Not String.IsNullOrEmpty(Request("code")) Then
                        Me.requestFactory.GetAuthorizeCredentials(Request("code"))
                        Session("VBDC_ACCESS_TOKEN") = Me.requestFactory.AuthorizeCredential
                    End If
                End If
            End If

            Me.GetDeviceCapabilities()
        Catch ie As InvalidResponseException
            lblErrorMessage.Text = ie.Body
            tbDeviceCapabError.Visible = True
        Catch te As TokenExpiredException
            lblErrorMessage.Text = te.Message
            tbDeviceCapabError.Visible = True
        Catch ex As Exception
            lblErrorMessage.Text = ex.Message
            tbDeviceCapabError.Visible = True
        End Try

    End Sub

#End Region

#Region "Application Specific Methods"

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
    ''' This method reads the configuration parameters and assigns to instance variables.
    ''' </summary>
    ''' <returns>true/false; true if able to read config parameters; else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            lblErrorMessage.Text = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            lblErrorMessage.Text = "apiKey is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            lblErrorMessage.Text = "secretKey is not defined in configuration file"
            Return False
        End If

        Me.redirectUrl = ConfigurationManager.AppSettings("redirectUrl")
        If String.IsNullOrEmpty(Me.redirectUrl) Then
            lblErrorMessage.Text = "redirectUrl is not defined in configuration file"
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Initialize a new instance of RequestFactory.
    ''' </summary>
    Private Sub InitializeRequestFactory()
        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(requestFactory.ScopeTypes.DeviceCapability)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Me.redirectUrl, Nothing)

        If Session("VBDC_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBDC_ACCESS_TOKEN"), OAuthToken)
        End If
    End Sub

    ''' <summary>
    ''' This method checks if the access token is already present, and calls GetDeviceCapabilities() of RequestFactory.
    ''' Else, it redirects the user to get the OAuth consent.
    ''' </summary>
    Private Sub GetDeviceCapabilities()
        If Session("VBDC_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBDC_ACCESS_TOKEN"), OAuthToken)
        End If

        If Me.requestFactory.AuthorizeCredential Is Nothing Then
            Response.Redirect(Me.requestFactory.GetOAuthRedirect().ToString())
        End If

        Dim deviceCapabilities As DeviceCapabilities = Me.requestFactory.GetDeviceCapabilities()
        Me.DisplayDeviceCapabilities(deviceCapabilities)
    End Sub

    ''' <summary>
    ''' This method displays the Device Capabilities.
    ''' </summary>
    ''' <param name="deviceCapabilities">Device Capabilities</param>
    Private Sub DisplayDeviceCapabilities(deviceCapabilities As DeviceCapabilities)
        If deviceCapabilities IsNot Nothing Then
            lblTypeAllocationCode.Text = deviceCapabilities.deviceId.TypeAllocationCode
            lblName.Text = deviceCapabilities.capabilities.Name
            lblVendor.Text = deviceCapabilities.capabilities.Vendor
            lblModel.Text = deviceCapabilities.capabilities.Model
            lblFirmwareVersion.Text = deviceCapabilities.capabilities.FirmwareVersion
            lblUAProf.Text = deviceCapabilities.capabilities.UaProf
            lblMMSCapable.Text = deviceCapabilities.capabilities.MmsCapable
            lblAssistedGps.Text = deviceCapabilities.capabilities.AssistedGps
            lblLocationTechnology.Text = deviceCapabilities.capabilities.LocationTechnology
            lblDeviceBrowser.Text = deviceCapabilities.capabilities.DeviceBrowser
            lblWAPPush.Text = deviceCapabilities.capabilities.WapPushCapable
            tb_dc_output.Visible = True
            tbDeviceCapabilities.Visible = True
        End If
    End Sub

#End Region
End Class