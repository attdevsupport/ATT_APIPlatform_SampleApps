' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports ATT_MSSDK
Imports ATT_MSSDK.WapPush
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Web.UI.WebControls
#End Region

' 
' This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Wap Push
' 
' Pre-requisite:
' -------------
' The developer has to register application in AT&T Developer Platform website, for the scope 
' of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' and client secret on registering the application.
' 
' Steps to be followed by the application to invoke Wap APIs exposed by MS SDK wrapper library:
' --------------------------------------------------------------------------------------------
' 1. Import ATT_MSSDK and ATT_MSSDK.WapPush NameSpace.
' 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' while creating RequestFactory instance.
' 
' Note: Scopes that are not configured for your application will not work.
' For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' 
' 3.Invoke the wap related APIs exposed in the RequestFactory class of MS SDK library.
' 
' For wap services MS SDK library provides APIs SendWapPush().
' This methods return response objects WapPushResponse.
' 
' Sample code for sending wap:
' ----------------------------
' Dim scopes As New List(Of RequestFactory.ScopeTypes)()
' scopes.Add(RequestFactory.ScopeTypes.WAPPush)
' Dim target As New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
' Dim wapResponse As WapPushResponse =  target.SendWapPush(PhoneNumber,"Wap Title",Url,AlertText,Attachment(optional),AttachmentType(optional))

''' <summary>
''' WapPush_App1 application
''' </summary>
''' <remarks>
''' This application allows a user to send a WAP Push message to a mobile device, by entering the address, alert text, and URL to be sent.
''' This application uses Autonomous Client Credentials consumption model to send messages. The user enters the alert text and URL, 
''' but the application in the background must build the push.txt file to attach with the requested values.
''' </remarks>
Partial Public Class WapPush_App1
    Inherits System.Web.UI.Page

    '** \addtogroup WAPPUSH_App1
    '** Description of the application can be referred at \ref WAPPUSH_app1 example
    '** @{
    '** 
    '**  \example WAPPUSH_app1 wap\app1\Default.aspx.vb
    '** \n \n This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Wap Push.
    '**  
    '** <b>Invoke WAP Push:</b>
    '** \li Import \c ATT_MSSDK and \c ATT_MSSDK.WapPush NameSpace.
    '** \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '** Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    '** \li Invoke \c SendWapPush() exposed in the \c RequestFactory class of MS SDK library.
    '** 
    '** <b>Sample code:</b>
    '** <pre>
    '**     Dim scopes As New List(Of RequestFactory.ScopeTypes)()
    '**     scopes.Add(RequestFactory.ScopeTypes.WAPPush)
    '**     Dim target As New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
    '**     Dim wapResponse As WapPushResponse =  target.SendWapPush(PhoneNumber,"Wap Title",Url,AlertText,Attachment(optional),AttachmentType(optional))
    '** </pre>
    '**
    '** Installing and running the application, refer \ref Application 
    '** \n \n <b>Parameters in web.config</b> refer \ref parameters_sec section
    '** 
    '** \n Documentation can be referred at \ref WAPPUSH_App1 section
    '** @{

#Region "Instance Variables"

    ''' <summary>
    ''' Gets or sets the value of requestFactory
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Gets or sets the value of apiKey
    ''' </summary>
    Private apiKey As String

    ''' <summary>
    ''' Gets or sets the value of secretKey
    ''' </summary>
    Private secretKey As String

    ''' <summary>
    ''' Gets or sets the value of endPoint
    ''' </summary>
    Private endPoint As String

#End Region

#Region "WapPush Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

        Me.Initialize()
    End Sub

    ''' <summary>
    ''' Event that invokes send wap API of MS SDK wrapper library.
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">eventargs of the button</param>
    Protected Sub SendWAPButton_Click(sender As Object, e As EventArgs)
        Try
            Dim wapResponse As WapPushResponse = Me.requestFactory.SendWapPush(txtAddressWAPPush.Text.Trim().ToString(), txtUrl.Text, txtAlert.Text)
            Me.DrawPanelForSuccess(wapPanel, wapResponse.Id.ToString())
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(wapPanel, ex.Message)
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(wapPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(wapPanel, ex.ToString())
        End Try
    End Sub

#End Region

#Region "WapPush Application specific functions"

    ''' <summary>
    ''' Display success message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel, message As String)
        If panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "successWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOneCellOne.Width = Unit.Pixel(75)
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)

        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Font.Bold = True
        rowTwoCellOne.Text = "Message ID:"
        rowTwoCellOne.Width = Unit.Pixel(75)
        rowTwo.Controls.Add(rowTwoCellOne)

        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellTwo.Text = message
        rowTwoCellTwo.HorizontalAlign = HorizontalAlign.Left
        rowTwo.Controls.Add(rowTwoCellTwo)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        If panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "errorWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Text = message.ToString()
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' This method neglects the ssl handshake error with authentication server
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Initialized the application variables with values from config file and displays errors, if any.
    ''' </summary>
    ''' <returns>true/false; true if able to read and assign to local variables; else false</returns>
    Private Function Initialize() As Boolean
        If Me.requestFactory Is Nothing Then
            Me.apiKey = ConfigurationManager.AppSettings("api_key")
            If String.IsNullOrEmpty(Me.apiKey) Then
                Me.DrawPanelForFailure(wapPanel, "api_key is not defined in configuration file")
                Return False
            End If

            Me.secretKey = ConfigurationManager.AppSettings("secret_key")
            If String.IsNullOrEmpty(Me.secretKey) Then
                Me.DrawPanelForFailure(wapPanel, "secret_key is not defined in configuration file")
                Return False
            End If

            Me.endPoint = ConfigurationManager.AppSettings("endPoint")
            If String.IsNullOrEmpty(Me.endPoint) Then
                Me.DrawPanelForFailure(wapPanel, "endPoint is not defined in configuration file")
                Return False
            End If

            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(RequestFactory.ScopeTypes.WAPPush)
            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        End If
        Return True
    End Function

#End Region
    '** }@
    '** }@
End Class