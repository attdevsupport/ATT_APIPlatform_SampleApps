' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text.RegularExpressions
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.CallControlv1

#End Region

''' <summary>
''' CallControl App1 class
''' </summary>
Partial Public Class CallControl_App1
    Inherits System.Web.UI.Page
#Region "Local variables"

    ''' <summary>
    ''' RequestFactory instance
    ''' </summary>
    Private requestFactory As RequestFactory

    ''' <summary>
    ''' Access Token Variables
    ''' </summary>
    Private endPoint As String, apiKey As String, secretKey As String

    ''' <summary>
    ''' Phone numbers registered for Call Control Service.
    ''' </summary>
    Private phoneNumbers As String

    ''' <summary>
    ''' Script location for Call Control Service.
    ''' </summary>
    Private scriptName As String

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

#Region "Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This function reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Try
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)

            Dim currentServerTime As DateTime = DateTime.UtcNow
            serverTimeLabel.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
            Me.WriteNote()
            Dim ableToRead As Boolean = Me.ReadConfigFile()
            If Not ableToRead Then
                Return
            End If

            Me.InitializeRequestFactory()
            lblPhoneNumbers.Text = Me.phoneNumbers
        Catch ex As Exception
            Me.DrawPanelForFailure(pnlCreateSession, ex.ToString())
        End Try
    End Sub
    ''' <summary>
    ''' Event that will be triggered when the user clicks on Send Signal button.
    ''' This method will invoke SendSignal API.
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub btnSendSignal_Click(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(lblSessionId.Text) Then
            Me.DrawPanelForFailure(pnlSendSignal, "Please create a session and then send signal")
            Return
        End If

        Me.SendSignal()
    End Sub

    ''' <summary>
    ''' Event that will be triggered when the user clicks on Create Session button.
    ''' This method will invoke CreateSession API.
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub btnCreateSession_Click(sender As Object, e As EventArgs)
        Me.CreateSession()
    End Sub

#End Region

#Region "API Invokation Methods"

    ''' <summary>
    ''' This method creates a Session for an outgoing call or message.
    ''' </summary>
    Private Sub CreateSession()
        Try
            Dim parameters As New NameValueCollection()
            If Not String.IsNullOrEmpty(txtNumberToDial.Text) Then
                parameters.Add("numberToDial", txtNumberToDial.Text)
            End If
            If Not String.IsNullOrEmpty(txtNumberForFeature.Text) Then
                parameters.Add("featurenumber", txtNumberForFeature.Text)
            End If
            If Not String.IsNullOrEmpty(txtMessageToPlay.Text) Then
                parameters.Add("messageToPlay", txtMessageToPlay.Text)
            End If
            If lstTemplate.SelectedValue <> "" Then
                parameters.Add("feature", lstTemplate.SelectedValue.ToString())
            End If

            Dim responseObject As CreateSessionResponse = Me.requestFactory.CreateSession(parameters)
            If responseObject IsNot Nothing Then
                lblSessionId.Text = responseObject.Id
                Dim displayParam As New NameValueCollection()
                displayParam.Add("id", responseObject.Id)
                displayParam.Add("success", responseObject.Success.ToString())
                Me.DrawPanelForSuccess(pnlCreateSession, displayParam, String.Empty)
            Else
                Me.DrawPanelForFailure(pnlCreateSession, "Unable to create session.")
            End If
        Catch ise As InvalidScopeException
            Me.DrawPanelForFailure(pnlCreateSession, ise.Message)
        Catch ire As InvalidResponseException
            Me.DrawPanelForFailure(pnlCreateSession, ire.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(pnlCreateSession, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' This method sends a Signal to an active Session.
    ''' </summary>
    Private Sub SendSignal()
        Try
            Dim signalResponse As SendSignalResponse = Me.requestFactory.SendSignal(lblSessionId.Text, ddlSignal.SelectedValue)
            If signalResponse IsNot Nothing Then
                Dim displayParam As New NameValueCollection()
                displayParam.Add("status", signalResponse.Status)
                Me.DrawPanelForSuccess(pnlSendSignal, displayParam, String.Empty)
            Else
                Me.DrawPanelForFailure(pnlSendSignal, "Unable to send signal.")
            End If
        Catch ane As ArgumentNullException
            Me.DrawPanelForFailure(pnlSendSignal, ane.Message)
        Catch ise As InvalidScopeException
            Me.DrawPanelForFailure(pnlSendSignal, ise.Message)
        Catch ire As InvalidResponseException
            Me.DrawPanelForFailure(pnlSendSignal, ire.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(pnlSendSignal, ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' This method displays the contents of the note area.
    ''' </summary>
    Private Sub WriteNote()
        Dim description As String = "<strong>Note:</strong> <br/>"
        Label1.Text = "Create Session will trigger an outbound call from application " + " to <strong>""Make call to""</strong> number."
        Select Case lstTemplate.SelectedValue
            Case "ask"
                description += "For <strong>ask()</strong> script function, user is prompted to enter few digits and entered digits are played back. <br/>" + "User is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case "conference"
                description += "For <strong>conference()</strong> script function, user is prompted to join the conference.<br/>" + "After quitting the conference, user is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case "message"
                description += "For <strong>message()</strong> script function, user is played back <strong>""Number parameter for Script Function""</strong> number and an SMS Message is sent to that number.<br/>" + "User is asked to press digit to activate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case "reject"
                description += "For <strong>reject()</strong> script function, if <strong>""Number parameter for Script Function""</strong> matches with calling id, call will be dropped.<br/>" + "If calling id doesnt match, calling id and <strong>""Number parameter for Script Function""</strong> number are played to User.<br/>" + "User is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case "transfer"
                description += "For <strong>transfer()</strong> script function, user is played back with <strong>""Number parameter for Script Function""</strong> and call be transferred to that number.<br/>" + "While doing transfer music on hold <strong>""Message to Play""</strong> is played. Once <strong>""Number parameter for Script Function""</strong> number disconnects the call, " + "user is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case "wait"
                description += "For <strong>wait()</strong> script function, if <strong>""Number parameter for Script Function""</strong> matches with calling id, call will be kept on hold for 3 seconds.<br/>" + "If calling id doesnt match, calling id and <strong>""Number parameter for Script Function""</strong> number are played to User.<br/>" + "User is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case ""
                description += "User is asked to press digit to activiate music on hold <strong>""Message to Play""</strong> to handle the signal (feature 2)"
                notesLiteral.Text = description
                Return
            Case Else
                Return
        End Select
    End Sub
    ''' <summary>
    ''' This method displays the contents of call.js file onto create session textarea.
    ''' </summary>
    Private Sub GetOutboundScriptContent()
        Dim streamReader As StreamReader = Nothing
        Try
            Dim scrFile As String = Request.MapPath(scriptName)
            streamReader = New StreamReader(scrFile)
            Dim javaScript As String = streamReader.ReadToEnd()
            txtCreateSession.Text = "Following is the Java Script Code: " & System.Environment.NewLine & javaScript
        Catch ex As Exception
            Me.DrawPanelForFailure(pnlCreateSession, ex.Message)
        Finally
            If streamReader IsNot Nothing Then
                streamReader.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Read parameters from configuraton file and assigns to local variables.
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(pnlCreateSession, "endPoint is not defined in configuration file")
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(pnlCreateSession, "apiKey is not defined in configuration file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(pnlCreateSession, "secretKey is not defined in configuration file")
            Return False
        End If

        Me.phoneNumbers = ConfigurationManager.AppSettings("phoneNumbers")
        If String.IsNullOrEmpty(Me.phoneNumbers) Then
            Me.DrawPanelForFailure(pnlCreateSession, "phoneNumbers parameter is not defined in configuration file")
            Return False
        End If

        Me.scriptName = ConfigurationManager.AppSettings("scriptName")
        If String.IsNullOrEmpty(Me.scriptName) Then
            Me.DrawPanelForFailure(pnlCreateSession, "scriptName parameter is not defined in configuration file")
            Return False
        End If
        Me.GetOutboundScriptContent()
        Return True
    End Function

    ''' <summary>
    ''' Initialized RequestFactory with instance variable values.
    ''' </summary>
    Private Sub InitializeRequestFactory()
        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.CallControl)
        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
    End Sub

#End Region

#Region "Display Methods"

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
    ''' This function is called to draw the table in the panelParam panel for success response.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="displayParams">Collection of message parameters to display.</param>
    ''' <param name="message">Message as string</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel, displayParams As NameValueCollection, message As String)
        Dim table As New Table()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.CssClass = "successWide"
        table.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)

        If displayParams IsNot Nothing Then
            Dim rowNextCellOne As TableCell = Nothing
            Dim rowNextCellTwo As TableCell = Nothing
            For Each key As String In displayParams.Keys
                Dim rowNext As New TableRow()
                rowNextCellOne = New TableCell()
                rowNextCellOne.Text = key
                rowNextCellOne.Font.Bold = True
                rowNextCellOne.Width = Unit.Pixel(70)
                rowNext.Controls.Add(rowNextCellOne)

                rowNextCellTwo = New TableCell()
                rowNextCellTwo.Text = displayParams(key)
                rowNext.Controls.Add(rowNextCellTwo)
                table.Controls.Add(rowNext)
            Next
        Else
            Dim rowTwo As New TableRow()
            Dim rowTwoCellOne As New TableCell()
            rowTwoCellOne.Text = message
            rowTwo.Controls.Add(rowTwoCellOne)
            table.Controls.Add(rowTwo)
        End If

        panelParam.Controls.Add(table)
    End Sub

#End Region
End Class
