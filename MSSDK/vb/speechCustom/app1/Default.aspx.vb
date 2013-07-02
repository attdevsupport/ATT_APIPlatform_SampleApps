' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "Application References"

Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.Speechv3
Imports System.Data

#End Region

' 
' This Application demonstrates usage of  AT&T MS SDK wrapper library for converting speech to text
' by passing inline grammar and inline hints as additional data set along with the audio.
' 
' Pre-requisite:
' -------------
' The developer has to register his application in AT&T Developer Platform website, for the scope 
' of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' and client secret on registering the application.
' 
' Steps to be followed by the application to invoke Speech APIs exposed by MS SDK wrapper library:
' --------------------------------------------------------------------------------------------
' 1. Import ATT_MSSDK and ATT_MSSDK.Speechv3 NameSpace.
' 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' while creating RequestFactory instance.
' 
' Note: Scopes that are not configured for your application will not work.
' For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' 
' 3.Invoke the custom speech related APIs exposed in the RequestFactory class of MS SDK library.
' 
' For speech services MS SDK library provides APIs SpeechToText()
' This methods returns SpeechResponse object.
' 
' Sample code for converting text to speech:
' ----------------------------
' Dim scopes As New List(Of RequestFactory.ScopeTypes)()
' scopes.Add(RequestFactory.ScopeTypes.STTC)
' Dim requestFactory as RequestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
' Dim response As SpeechResponse = Me.requestFactory.SpeechToTextCustom(fileToConvert, dictionaryFile, grammarFile, speechContext)
' 


''' <summary>
''' SpeechCustom_App1 class
''' </summary>
Partial Public Class SpeechCustom_App1
    Inherits System.Web.UI.Page
#Region "Class Variables and Constructor"
    ''' <summary>
    ''' Request Factory object for calling api functions
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Application Service specific variables
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' X-Arg parameter
    ''' A meta parameter to define multiple parameters within a single HTTP header.
    ''' </summary>
    Private xArgData As String

    ''' <summary>
    ''' variable for having the posted file.
    ''' </summary>
    Private SpeechFilesDir As String

    ''' <summary>
    '''  Variable for path of the file containing inline hints 
    ''' </summary>
    Private dictionaryFilePath As String

    ''' <summary>
    '''  Variable for path of the file containing inline grammar 
    ''' </summary>
    Private grammarFilePath As String

#End Region

#Region "SpeechToTextCustom Application Events"

    ''' <summary>
    ''' This function is called when the application page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        BypassCertificateError()
        resultsPanel.Visible = False
        Me.Initialize()
        Me.SetContent()
    End Sub

    ''' <summary>
    ''' Method that calls SpeechToText Custom method of RequestFactory when user clicked on submit button
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">event args of the button</param>
    Protected Sub SpeechToTextButton_Click(sender As Object, e As EventArgs)
        Try
            Dim fileToConvert As String = (Me.SpeechFilesDir & "/") + ddlAudioFile.SelectedValue

            Dim speechContext As XSpeechCustomContext = XSpeechCustomContext.GenericHints
            Dim contentLanguage As String = String.Empty

            Select Case ddlSpeechContext.SelectedValue
                Case "GenericHints"
                    speechContext = XSpeechCustomContext.GenericHints
                    Exit Select

                Case "GrammarList"
                    speechContext = XSpeechCustomContext.GrammarList
                    Exit Select
            End Select

            Dim dictionaryFile As String = Request.MapPath(Me.dictionaryFilePath)
            Dim grammarFile As String = Request.MapPath(Me.grammarFilePath)

            Dim response As SpeechResponse = Me.requestFactory.SpeechToTextCustom(fileToConvert, dictionaryFile, grammarFile, speechContext)

            If response IsNot Nothing Then
                resultsPanel.Visible = True
                Me.DrawPanelForSuccess(statusPanel, "Response Parameters listed below")
                Me.DisplayResult(response)
            End If
        Catch invalidscope As InvalidScopeException
            Me.DrawPanelForFailure(statusPanel, invalidscope.Message)
        Catch argex As ArgumentException
            Me.DrawPanelForFailure(statusPanel, argex.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

#End Region

#Region "SpeechToTextCustom service functions"

    ''' <summary>
    ''' Initializes a new instance of the Speech_app1 class. This constructor reads from Config file and initializes Request Factory object
    ''' </summary>
    Private Sub Initialize()
        Dim currentServerTime As DateTime = DateTime.UtcNow
        lblServerTime.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
        Me.ReadConfigAndInitialize()
    End Sub

    ''' <summary>
    ''' Display success message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel, message As String)
        If panelParam IsNot Nothing AndAlso panelParam.HasControls() Then
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
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellTwo.Text = message.ToString()
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
        If panelParam IsNot Nothing AndAlso panelParam.HasControls() Then
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
    ''' Displays the result onto the page
    ''' </summary>
    ''' <param name="speechResponse">SpeechResponse received from api</param>
    Private Sub DisplayResult(speechResponse As SpeechResponse)
        statusPanel.Visible = True
        lblStatus.Text = speechResponse.Recognition.Status
        lblResponseId.Text = speechResponse.Recognition.Responseid
        For Each nbest As NBest In speechResponse.Recognition.NBest
            lblHypothesis.Text = nbest.Hypothesis
            lblLanguageId.Text = nbest.LanguageId
            lblResultText.Text = nbest.ResultText
            lblGrade.Text = nbest.Grade
            lblConfidence.Text = nbest.Confidence.ToString()
            Dim words As String = "[ "
            If nbest.Words IsNot Nothing Then
                For Each word As String In nbest.Words
                    words += """" & word & """, "
                Next
                words = words.Substring(0, words.LastIndexOf(","))
                words = words & " ]"
            End If

            lblWords.Text = If(nbest.Words IsNot Nothing, words, String.Empty)

            If nbest.WordScores IsNot Nothing Then
                lblWordScores.Text = "[ " & String.Join(", ", nbest.WordScores.ToArray()) & " ]"
            End If
        Next

    End Sub

    ''' <summary>
    ''' Neglect the ssl handshake error with authentication server 
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Read from config and initialize RequestFactory object
    ''' </summary>
    ''' <returns>true/false; true - if able to read from config file; else false</returns>
    Private Function ReadConfigAndInitialize() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(statusPanel, "apiKey is not defined in the config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(statusPanel, "secretKey is not defined in the config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(statusPanel, "endpoint is not defined in the config file")
            Return False
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SpeechFilesDir")) Then
            Me.SpeechFilesDir = Request.MapPath(ConfigurationManager.AppSettings("SpeechFilesDir"))
        End If

        Me.xArgData = ConfigurationManager.AppSettings("X-Arg")
        Me.grammarFilePath = ConfigurationManager.AppSettings("X-Grammar")
        Me.dictionaryFilePath = ConfigurationManager.AppSettings("X-Dictionary")
        txtXArgs.Text = Me.xArgData.Replace(",", "," & Environment.NewLine)

        If Not Page.IsPostBack Then
            Dim speechContxt As String = ConfigurationManager.AppSettings("SpeechContext")
            If Not String.IsNullOrEmpty(speechContxt) Then
                Dim speechContexts As String() = speechContxt.Split(";"c)
                For Each speechContext As String In speechContexts
                    ddlSpeechContext.Items.Add(speechContext)
                Next

                If ddlSpeechContext.Items.Count > 0 Then
                    ddlSpeechContext.Items(0).Selected = True
                End If
            End If

            If Not String.IsNullOrEmpty(SpeechFilesDir) Then
                Dim filePaths As String() = Directory.GetFiles(Me.SpeechFilesDir)
                For Each filePath As String In filePaths
                    ddlAudioFile.Items.Add(Path.GetFileName(filePath))
                Next

                If filePaths.Length > 0 Then
                    ddlAudioFile.Items(0).Selected = True
                End If
            End If
        End If

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.STTC)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)

        Return True
    End Function

    ''' <summary>
    ''' Populate the controls on the page.
    ''' </summary>
    Private Sub SetContent()
        Dim xdictionaryContent As String = String.Empty
        Dim xgrammerContent As String = String.Empty
        Dim dictionaryFile As String = Request.MapPath(Me.dictionaryFilePath)
        Dim grammarFile As String = Request.MapPath(Me.grammarFilePath)

        If Not String.IsNullOrEmpty(dictionaryFilePath) AndAlso File.Exists(dictionaryFile) Then
            Dim streamReader As New StreamReader(dictionaryFile)
            xdictionaryContent = streamReader.ReadToEnd()
            streamReader.Close()
            txtMimeData.Text = "x-dictionary:" & Environment.NewLine & Environment.NewLine & xdictionaryContent & Environment.NewLine
        End If

        If Not String.IsNullOrEmpty(grammarFilePath) AndAlso File.Exists(grammarFile) Then
            Dim streamReader As New StreamReader(grammarFile)
            xgrammerContent = streamReader.ReadToEnd()
            streamReader.Close()
            txtMimeData.Text += Environment.NewLine & "x-grammar:" & Environment.NewLine & Environment.NewLine & xgrammerContent
        End If
    End Sub

#End Region
End Class