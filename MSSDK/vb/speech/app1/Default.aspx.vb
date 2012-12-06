' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
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
Imports ATT_MSSDK.Speechv2

#End Region

' 
' * This Application demonstrates usage of  AT&T MS SDK wrapper library for converting speech to text
' * 
' * Pre-requisite:
' * -------------
' * The developer has to register his application in AT&T Developer Platform website, for the scope 
' * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' * and client secret on registering the application.
' * 
' * Steps to be followed by the application to invoke Speech APIs exposed by MS SDK wrapper library:
' * --------------------------------------------------------------------------------------------
' * 1. Import ATT_MSSDK and ATT_MSSDK.Speechv1 NameSpace.
' * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' * while creating RequestFactory instance.
' *
' * Note: Scopes that are not configured for your application will not work.
' * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' * 
' * 3.Invoke the speech related APIs exposed in the RequestFactory class of MS SDK library.
' * 
' * For speech services MS SDK library provides APIs SpeechToText()
' * This methods returns SpeechResponse object.
' 
' * Sample code for converting text to speech:
' * ----------------------------
'  Dim scopes As New List(Of RequestFactory.ScopeTypes)()
'  scopes.Add(RequestFactory.ScopeTypes.Speech)
'  Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
'  Dim response As SpeechResponse = Me.requestFactory.SpeechToText(fileName, speechContext, xArgs)


''' <summary>
''' Speech_App1 class
''' </summary>
Partial Public Class Speech_App1
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
    ''' variable for having the posted file.
    ''' </summary>
    Private fileToConvert As String

    ''' <summary>
    ''' Flag for deletion of the temporary file
    ''' </summary>
    Private deleteFile As Boolean

    ''' <summary>
    ''' X-Arg parameter
    ''' A meta parameter to define multiple parameters within a single HTTP header.
    ''' </summary>
    Private xArgData As String

#End Region

#Region "Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        If Not Page.IsPostBack Then
            resultsPanel.Visible = False
            Me.Initialize()
            Dim speechContxt As String = ConfigurationManager.AppSettings("SpeechContext")
            If Not String.IsNullOrEmpty(speechContxt) Then
                Dim speechContexts As String() = speechContxt.Split(";"c)
                For Each speechContext As String In speechContexts
                    ddlSpeechContext.Items.Add(speechContext)
                Next

                ddlSpeechContext.Items(0).Selected = True
            End If

            Me.xArgData = ConfigurationManager.AppSettings("X-Arg")
            txtXArgs.Text = Me.xArgData.Replace(",", "," & Environment.NewLine)
        End If

        Dim currentServerTime As DateTime = DateTime.UtcNow
        lblServerTime.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
        Me.ResetDisplay()
    End Sub

    ''' <summary>
    ''' Method that calls SpeechToText method of RequestFactory when user clicked on submit button
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">eventargs of the button</param>
    Protected Sub SpeechToTextButton_Click(sender As Object, e As EventArgs)
        Try
            resultsPanel.Visible = False
            Me.Initialize()
            If String.IsNullOrEmpty(fileUpload1.FileName) Then
                If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DefaultFile")) Then
                    Me.fileToConvert = Request.MapPath(ConfigurationManager.AppSettings("DefaultFile"))
                Else
                    Me.DrawPanelForFailure(statusPanel, "No file selected, and default file is not defined in web.config")
                    Return
                End If
            Else
                Dim fileName As String = fileUpload1.FileName
                If fileName.CompareTo("default.wav") = 0 Then
                    fileName = "1" + fileUpload1.FileName
                End If
                fileUpload1.PostedFile.SaveAs(Request.MapPath("") & "/" & fileName)
                Me.fileToConvert = Request.MapPath("").ToString() & "/" & fileName
                Me.deleteFile = True
            End If

            Dim response As SpeechResponse = Me.requestFactory.SpeechToText(Me.fileToConvert, ddlSpeechContext.SelectedValue, Me.xArgData)
            If response IsNot Nothing Then
                Me.DrawPanelForSuccess(statusPanel, "Response Parameters listed below")
                resultsPanel.Visible = True
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
        Finally
            If (Me.deleteFile = True) AndAlso (File.Exists(Me.fileToConvert)) Then
                File.Delete(Me.fileToConvert)
                Me.deleteFile = False
            End If
        End Try
    End Sub

#End Region

#Region "SpeechToText service functions"

    ''' <summary>
    ''' Initializes a new instance of the Speech_app1 class. This constructor reads from Config file and initializes Request Factory object
    ''' </summary>
    Private Sub Initialize()
        If Me.requestFactory Is Nothing Then
            Me.ReadConfigAndInitialize()
        End If
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
    ''' Reset Display of success response
    ''' </summary>
    Private Sub ResetDisplay()
        lblResponseId.Text = String.Empty
        lblStatus.Text = String.Empty
        lblHypothesis.Text = String.Empty
        lblLanguageId.Text = String.Empty
        lblResultText.Text = String.Empty
        lblGrade.Text = String.Empty
        lblConfidence.Text = String.Empty
        lblWords.Text = String.Empty
        lblWordScores.Text = String.Empty
        hypoRow.Visible = True
        langRow.Visible = True
        confRow.Visible = True
        gradeRow.Visible = True
        resultRow.Visible = True
        wordsRow.Visible = True
        wordScoresRow.Visible = True
    End Sub

    ''' <summary>
    ''' Displays the result onto the page
    ''' </summary>
    ''' <param name="speechResponse">SpeechResponse received from api</param>
    Private Sub DisplayResult(speechResponse As SpeechResponse)
        lblResponseId.Text = speechResponse.Recognition.ResponseId
        lblStatus.Text = speechResponse.Recognition.Status
        If (speechResponse.Recognition.NBest IsNot Nothing) AndAlso (speechResponse.Recognition.NBest.Count > 0) Then
            For Each nbest As NBest In speechResponse.Recognition.NBest
                lblHypothesis.Text = nbest.Hypothesis
                lblLanguageId.Text = nbest.LanguageId
                lblResultText.Text = nbest.ResultText
                lblGrade.Text = nbest.Grade
                lblConfidence.Text = nbest.Confidence.ToString()

                Dim strText As String = "["
                For Each word As String In nbest.Words
                    strText += """" + word + """, "
                Next
                strText = strText.Substring(0, strText.LastIndexOf(","))
                strText = strText + "]"

                lblWords.Text = If(nbest.Words IsNot Nothing, strText, String.Empty)

                lblWordScores.Text = "[" + String.Join(", ", nbest.WordScores.ToArray()) + "]"
            Next
        Else
            hypoRow.Visible = False
            langRow.Visible = False
            confRow.Visible = False
            gradeRow.Visible = False
            resultRow.Visible = False
            wordsRow.Visible = False
            wordScoresRow.Visible = False
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

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.Speech)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        Return True
    End Function

#End Region
End Class
