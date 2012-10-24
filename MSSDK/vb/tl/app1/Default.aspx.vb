' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Collections.Generic
Imports System.Configuration
Imports System.Drawing
Imports System.Globalization
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.TLv1

#End Region

' This Application demonstrates usage of  AT&T MS SDK wrapper library for getting  device location 
' 
' Pre-requisite:
' -------------
' The developer has to register his application in AT&T Developer Platform website, for the scope 
' of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' and client secret on registering the application.
' 
' Steps to be followed by the application to invoke SMS APIs exposed by MS SDK wrapper library:
' --------------------------------------------------------------------------------------------
' 1. Import ATT_MSSDK and ATT_MSSDK.TLv1 NameSpace.
' 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' while creating RequestFactory instance.Also pass redirectUri(Uri ponting to the application).
' 
' Note: Scopes that are not configured for your application will not work.
' For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' 
' 3. Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform)
'    Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization.
' 4. Retrive the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory 
'    by passing 'code' as the argument.
'    
' 5. Invoke GetTerminalLocation() on requestFactory to get the device location by passing the device Id
'    
' the sms related APIs exposed in the RequestFactory class of MS SDK library.
' 
' For device location service MS SDK library provides API GetTerminalLocation()
' This methods returns response objects DeviceLocation.
' 
' Sample code for getting device location:
' ----------------------------
' DeviceLocation deviceLocationRequest = requestFactory.GetTerminalLocation(PhoneNumber,RequestedAccuracy,TerminalLocationTolerance,AcceptableAccuracy);

''' <summary>
''' TL_App1 MSSDK Sample App for terminal Location
''' </summary>
Partial Public Class TL_App1
    Inherits System.Web.UI.Page

    ''** \addtogroup TL_App1
    ' * Description of the application can be referred at \ref TL_app1 example
    ' * @{
    ' */
    '
    '/** \example TL_app1 tl\app1\Default.aspx.cs
    ' * \n \n This application allows the AT&T subscriber access to message related data stored in the AT&T Messages environment. 
    ' * \n \n <b>GetDeviceLocation</b>
    ' * \n This method is used to to query  the location of an AT&amp;T MSISDN for latitude, longitude and accuracy coordinates.
    ' * 
    ' * \li Import \c ATT_MSSDK and \c ATT_MSSDK.TLv1 NameSpace.
    ' * \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    ' * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    ' * \li Invoke \c GetTerminalLocation() exposed in the \c RequestFactory class of MS SDK library.
    ' * 
    ' * Sample code:
    ' * <pre>
    ' *    Dim scopes As New List(Of RequestFactory.ScopeTypes)()
    ' *    scopes.Add(RequestFactory.ScopeTypes.TerminalLocation)
    ' *    Dim target As RequestFactory = New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
    ' *    Dim deviceLocationRequest as DeviceLocation = requestFactory.GetTerminalLocation(requestedAccuracy, tolerance, acceptableAccuracy)</pre>
    ' * \n Installing and running the application, refer \ref Application 
    ' * \n \n <b>Parameters in web.config</b>
    ' * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
    ' * \li authorize_redirect_uri - This is mandatory key and value should be equal to  TL Service registered applicaiton 'OAuth Redirect URL'
    ' * 
    ' * \n Documentation can be referred at \ref TL_App1 section
    ' * @{
    ' */

#Region "Instance Variables"

    ''' <summary>
    ''' Gets or sets instance of requestFactory
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Local variables for storing app configuration details
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' OAuth redirect Url
    ''' </summary>
    Private authorizeRedirectUri As String

    ''' <summary>
    ''' Local variable to store request start time
    ''' </summary>
    Private startTime As DateTime

    ''' <summary>
    ''' Gets or sets the Status Table
    ''' </summary>
    Private getStatusTable As Table

#End Region

#Region "TL Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This function reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            BypassCertificateError()
            map_canvas.Visible = False

            Dim currentServerTime As DateTime = DateTime.UtcNow
            serverTimeLabel.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

            Dim ableToRead As Boolean = Me.ReadConfigFile()
            If Not ableToRead Then
                Return
            End If

            Dim authCode As String

            If Session("tl_session_acceptableAccuracy") IsNot Nothing Then
                Radio_AcceptedAccuracy.SelectedIndex = Convert.ToInt32(Session("tl_session_acceptableAccuracy").ToString())
                Radio_RequestedAccuracy.SelectedIndex = Convert.ToInt32(Session("tl_session_requestedAccuracy").ToString())
                Radio_DelayTolerance.SelectedIndex = Convert.ToInt32(Session("tl_session_tolerance").ToString())
            End If

            If Not Page.IsPostBack Then
                If Not String.IsNullOrEmpty(Convert.ToString(Request("code"))) Then
                    authCode = Request("code")
                    Me.requestFactory.GetAuthorizeCredentials(authCode)
                    Session("VBTL_ACCESS_TOKEN") = Me.requestFactory.AuthorizeCredential

                    If Convert.ToString(Session("buttonStatus")) = "true" Then
                        Me.GetDeviceLocation()
                        Session("buttonStatus") = String.Empty
                    End If
                End If
            End If
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(tlPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Event that will be triggered when the user clicks on GetPhoneLocation button
    ''' This method calls GetDeviceLocation Api
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub GetDeviceLocation_Click(sender As Object, e As EventArgs)
        Try
            Session("buttonStatus") = "true"
            Me.startTime = DateTime.Now
            Session("tl_session_acceptableAccuracy") = Radio_AcceptedAccuracy.SelectedIndex
            Session("tl_session_requestedAccuracy") = Radio_RequestedAccuracy.SelectedIndex
            Session("tl_session_tolerance") = Radio_DelayTolerance.SelectedIndex

            If Session("VBTL_ACCESS_TOKEN") Is Nothing Then
                Dim redirectUrl As String = Me.requestFactory.GetOAuthRedirect().ToString()
                Response.Redirect(redirectUrl)
            Else
                Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBTL_ACCESS_TOKEN"), OAuthToken)
                Me.GetDeviceLocation()
                Session("buttonStatus") = String.Empty
            End If
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(tlPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        End Try
    End Sub

#End Region

#Region "TL Application specific functions"

    ''' <summary>
    ''' Neglect the ssl handshake error with authentication server
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Reads from config file and assigns to local variables
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(tlPanel, "endPoint is not defined in configuration file")
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(tlPanel, "api_key is not defined in configuration file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(tlPanel, "secret_key is not defined in configuration file")
            Return False
        End If

        Me.authorizeRedirectUri = ConfigurationManager.AppSettings("authorize_redirect_uri")
        If String.IsNullOrEmpty(Me.authorizeRedirectUri) Then
            Me.DrawPanelForFailure(tlPanel, "authorize_redirect_uri is not defined in configuration file")
            Return False
        End If

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.TerminalLocation)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Me.authorizeRedirectUri, Nothing)
        If Session("VBTL_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBTL_ACCESS_TOKEN"), OAuthToken)
        End If

        Return True
    End Function

    ''' <summary>
    ''' Invokes GetTerminal Location method of SDK and displays the result
    ''' </summary>
    Private Sub GetDeviceLocation()
        Try
            Dim definedReqAccuracy As Integer() = New Integer(2) {100, 1000, 10000}
            Dim requestedAccuracy As Integer, acceptableAccuracy As Integer

            acceptableAccuracy = definedReqAccuracy(Radio_AcceptedAccuracy.SelectedIndex)
            requestedAccuracy = definedReqAccuracy(Radio_RequestedAccuracy.SelectedIndex)

            Dim tolerance As TerminalLocationTolerance = TerminalLocationTolerance.DelayTolerant
            Select Case Radio_DelayTolerance.SelectedIndex
                Case 0
                    tolerance = TerminalLocationTolerance.NoDelay
                    Exit Select
                Case 1
                    tolerance = TerminalLocationTolerance.LowDelay
                    Exit Select
                Case Else
                    tolerance = TerminalLocationTolerance.DelayTolerant
                    Exit Select
            End Select

            Dim deviceLocationRequest As DeviceLocation = Me.requestFactory.GetTerminalLocation(requestedAccuracy, tolerance, acceptableAccuracy)

            Dim endTime As DateTime = DateTime.Now
            Dim timeSpan As TimeSpan = endTime - Me.startTime

            Me.DrawPanelForGetLocationResult(String.Empty, String.Empty, True)
            Me.DrawPanelForGetLocationResult("Accuracy:", deviceLocationRequest.Accuracy.ToString(), False)
            Me.DrawPanelForGetLocationResult("Latitude:", deviceLocationRequest.Latitude.ToString(), False)
            Me.DrawPanelForGetLocationResult("Longitude:", deviceLocationRequest.Longitude.ToString(), False)
            Me.DrawPanelForGetLocationResult("TimeStamp:", deviceLocationRequest.TimeStamp.ToString(), False)
            Me.DrawPanelForGetLocationResult("Response Time:", timeSpan.Seconds.ToString() & "seconds", False)

            MapTerminalLocation.Visible = True
            map_canvas.Visible = True
            Dim googleString As New StringBuilder()
            googleString.Append("http://maps.google.com/?q=" & deviceLocationRequest.Latitude.ToString() & "+" & deviceLocationRequest.Longitude.ToString() & "&output=embed")
            MapTerminalLocation.Attributes("src") = googleString.ToString()
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(tlPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(tlPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw error message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        Dim table As New Table()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.CssClass = "errorWide"
        table.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Text = message
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        table.BorderWidth = 2
        table.BorderColor = Color.Red
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc")
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' This method is used to draw table for successful response of get device locations
    ''' </summary>
    ''' <param name="attribute">string, attribute to be displayed</param>
    ''' <param name="value">string, value to be displayed</param>
    ''' <param name="headerFlag">boolean, flag indicating to draw header panel</param>
    Private Sub DrawPanelForGetLocationResult(attribute As String, value As String, headerFlag As Boolean)
        If headerFlag = True Then
            Me.getStatusTable = New Table()
            Me.getStatusTable.CssClass = "successWide"
            Dim rowOne As New TableRow()
            Dim rowOneCellOne As New TableCell()
            rowOneCellOne.Font.Bold = True
            rowOneCellOne.Text = "SUCCESS:"
            rowOne.Controls.Add(rowOneCellOne)
            Me.getStatusTable.Controls.Add(rowOne)
            tlPanel.Controls.Add(Me.getStatusTable)
        Else
            Dim row As New TableRow()
            Dim cell1 As New TableCell()
            Dim cell2 As New TableCell()
            cell1.Text = attribute.ToString()
            cell1.Font.Bold = True
            cell1.Width = Unit.Pixel(100)
            row.Controls.Add(cell1)
            cell2.Text = value.ToString()
            row.Controls.Add(cell2)
            Me.getStatusTable.Controls.Add(row)
        End If
    End Sub

#End Region
    '** }@
    '** }@
End Class