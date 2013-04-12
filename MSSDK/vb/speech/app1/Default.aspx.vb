' <copyright file="Default.aspx.cs" company="AT&amp;T">
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
' List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
' scopes.Add(RequestFactory.ScopeTypes.Speech);
' RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
' SpeechResponse resp = requestFactory.SpeechToText(filePath, XSpeechContext.Generic, xArgNameValueCollection);
' 


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
    Private commonXArg As String
    Private xArgTVContext As String
    Private xArgSocialMediaContext As String

#End Region

#Region "Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        BypassCertificateError()
        If Not Page.IsPostBack Then
            resultsPanel.Visible = False
            tvContextPanel.Visible = False
            tvContextProgramPanel.Visible = False
            tvContextShowtimePanel.Visible = False

            Me.Initialize()
            txtXArgs.Text = Me.commonXArg.Replace(",", "," & Environment.NewLine)
            Dim speechContxt As String = ConfigurationManager.AppSettings("SpeechContext")
            If Not String.IsNullOrEmpty(speechContxt) Then
                Dim speechContexts As String() = speechContxt.Split(";"c)
                For Each speechContext As String In speechContexts
                    ddlSpeechContext.Items.Add(speechContext)
                Next

                ddlSpeechContext.Items(0).Selected = True
            End If
        End If

        Dim currentServerTime As DateTime = DateTime.UtcNow
        lblServerTime.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
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

            Dim speechContext As XSpeechContext = XSpeechContext.Generic
            Dim contentLanguage As String = String.Empty
            Me.xArgData = Me.commonXArg
            Select Case ddlSpeechContext.SelectedValue
                Case "Generic"
                    speechContext = XSpeechContext.Generic
                    contentLanguage = ddlContentLang.SelectedValue
                    Exit Select
                Case "BusinessSearch"
                    speechContext = XSpeechContext.BusinessSearch
                    Exit Select
                Case "TV"
                    speechContext = XSpeechContext.TV
                    Me.xArgData = Me.xArgTVContext
                    Exit Select
                Case "Gaming"
                    speechContext = XSpeechContext.Gaming
                    Exit Select
                Case "SocialMedia"
                    speechContext = XSpeechContext.SocialMedia
                    Me.xArgData = Me.xArgSocialMediaContext
                    Exit Select
                Case "WebSearch"
                    speechContext = XSpeechContext.WebSearch
                    Exit Select
                Case "SMS"
                    speechContext = XSpeechContext.SMS
                    Exit Select
                Case "VoiceMail"
                    speechContext = XSpeechContext.VoiceMail
                    Exit Select
                Case "QuestionAndAnswer"
                    speechContext = XSpeechContext.QuestionAndAnswer
                    Exit Select
            End Select

            Dim subContext As String = txtSubContext.Text
            If subContext.ToLower().Contains("example") Then
                subContext = String.Empty
            End If

            Dim response As SpeechResponse = Me.requestFactory.SpeechToText(fileToConvert, speechContext, Me.xArgData, contentLanguage, subContext, ddlAudioContentType.SelectedValue)

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

        If ddlSpeechContext.SelectedValue <> "TV" Then
            tvContextPanel.Visible = False
            tvContextProgramPanel.Visible = False
            tvContextShowtimePanel.Visible = False
        End If

        If ddlSpeechContext.SelectedValue = "TV" Then
            tvContextPanel.Visible = True
            If speechResponse.Recognition.Info IsNot Nothing Then
                lblInfoActionType.Text = speechResponse.Recognition.Info.ActionType
                Me.lblRecognized.Text = speechResponse.Recognition.Info.Recognized
            End If

            If speechResponse.Recognition.Info.Interpretation IsNot Nothing Then
                lblInterpretation_genre_id.Text = speechResponse.Recognition.Info.Interpretation.Genre_id
                lblInterpretation_genre_words.Text = speechResponse.Recognition.Info.Interpretation.Genre_words
            End If

            If speechResponse.Recognition.Info.metrics IsNot Nothing Then
                lblMetrics_audioBytes.Text = speechResponse.Recognition.Info.Metrics.AudioBytes.ToString()
                Me.lblMetrics_audioTime.Text = speechResponse.Recognition.Info.Metrics.AudioTime.ToString()
            End If

            Dim programs As List(Of Program) = Nothing

            If speechResponse.Recognition.Info.Search IsNot Nothing AndAlso speechResponse.Recognition.Info.Search.Meta IsNot Nothing Then
                Me.lblDescription.Text = speechResponse.Recognition.Info.Search.Meta.Description

                If speechResponse.Recognition.Info.Search.Meta.GuideDateStart IsNot Nothing Then
                    Me.lblGuideDateStart.Text = speechResponse.Recognition.Info.Search.Meta.GuideDateStart.ToString()
                End If

                If speechResponse.Recognition.Info.Search.Meta.GuideDateEnd IsNot Nothing Then
                    Me.lblGuideDateEnd.Text = speechResponse.Recognition.Info.Search.Meta.GuideDateEnd.ToString()
                End If

                Me.lblLineup.Text = speechResponse.Recognition.Info.Search.Meta.Lineup
                Me.lblMarket.Text = speechResponse.Recognition.Info.Search.Meta.Market
                Me.lblResultCount.Text = speechResponse.Recognition.Info.Search.Meta.ResultCount.ToString()

                programs = speechResponse.Recognition.Info.Search.Programs

                If programs IsNot Nothing Then
                    Me.DisplayProgramDetails(programs)
                End If
            End If

            Dim showtimes As List(Of Showtime) = Nothing

            If speechResponse.Recognition.Info.Search IsNot Nothing Then
                showtimes = speechResponse.Recognition.Info.Search.Showtimes

                If showtimes IsNot Nothing Then
                    Me.DisplayShowTimeDetails(showtimes)
                End If
            End If
        End If
    End Sub

    Private Sub DisplayProgramDetails(programs As List(Of Program))
        Dim programDetailsTable As DataTable = Me.GetProgramDetailsTable()

        Dim row As DataRow
        For Each program As Program In programs
            row = programDetailsTable.NewRow()

            row("cast") = program.Cast
            row("category") = program.Category
            row("description") = program.Description
            row("director") = program.Director
            row("language") = program.Language
            row("mpaaRating") = program.MpaaRating
            row("originalAirDate") = program.OriginalAirDate
            row("pid") = program.Pid
            row("runTime") = program.RunTime
            row("showType") = program.ShowType
            row("starRating") = program.StarRating
            row("title") = program.Title
            row("subtitle") = program.Subtitle
            row("year") = program.Year

            programDetailsTable.Rows.Add(row)
        Next

        tvContextProgramPanel.Visible = True
        gvPrograms.DataSource = programDetailsTable
        gvPrograms.DataBind()
    End Sub

    Private Sub DisplayShowTimeDetails(showtimes As List(Of Showtime))
        Dim showDetailsTable As DataTable = Me.GetShowTimeDetailsTable()

        Dim row As DataRow
        For Each showtime As Showtime In showtimes
            row = showDetailsTable.NewRow()

            row("affiliate") = showtime.Affiliate
            row("callSign") = showtime.CallSign
            row("channel") = showtime.Channel
            row("closeCaptioned") = showtime.CloseCaptioned
            row("dolby") = showtime.Dolby
            row("duration") = showtime.Duration
            row("endTime") = showtime.EndTime
            row("finale") = showtime.Finale
            row("hdtv") = showtime.Hdtv
            row("newShow") = showtime.NewShow
            row("pid") = showtime.Pid.ToString()
            row("premier") = showtime.Premier
            row("repeat") = showtime.Repeat
            row("showTime") = showtime.ShowTime
            row("station") = showtime.Station
            row("stereo") = showtime.Stereo
            row("subtitled") = showtime.Subtitled
            row("weekday") = showtime.Weekday

            showDetailsTable.Rows.Add(row)
        Next

        tvContextShowtimePanel.Visible = True
        gvShowTimes.DataSource = showDetailsTable
        gvShowTimes.DataBind()
    End Sub

    Private Function GetShowTimeDetailsTable() As DataTable
        Dim showtimeDetailsTable As New DataTable()
        Dim column As New DataColumn("affiliate")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("callsign")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("channel")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("closecaptioned")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("dolby")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("showtime")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("endtime")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("duration")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("finale")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("hdtv")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("newshow")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("pid")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("premier")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("repeat")
        showtimeDetailsTable.Columns.Add(column)

        column = New DataColumn("station")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("stereo")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("subtitled")
        showtimeDetailsTable.Columns.Add(column)
        column = New DataColumn("weekday")
        showtimeDetailsTable.Columns.Add(column)
        Return showtimeDetailsTable
    End Function

    Private Function GetProgramDetailsTable() As DataTable
        Dim programDetailsTable As New DataTable()
        Dim column As New DataColumn("cast")
        programDetailsTable.Columns.Add(column)

        column = New DataColumn("category")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("description")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("director")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("language")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("mpaaRating")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("originalAirDate")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("pid")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("runTime")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("showType")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("starRating")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("title")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("subtitle")
        programDetailsTable.Columns.Add(column)
        column = New DataColumn("year")
        programDetailsTable.Columns.Add(column)
        Return programDetailsTable
    End Function

    Private Sub addCelltoTable(table As Table, cellOneEntry As String, cellTwoEntry As String)
        Dim row As New TableRow()
        Dim rowCellOne As New TableCell()
        rowCellOne.Text = cellOneEntry
        Dim rowCellTwo As New TableCell()
        rowCellTwo.Text = cellTwoEntry
        row.Controls.Add(rowCellOne)
        row.Controls.Add(rowCellTwo)
        table.Controls.Add(row)
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

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.Speech)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)

        Me.commonXArg = ConfigurationManager.AppSettings("X-Arg")
        Me.xArgTVContext = ConfigurationManager.AppSettings("X-ArgTVContext")
        Me.xArgSocialMediaContext = ConfigurationManager.AppSettings("X-ArgSocialMedia")

        Return True
    End Function

#End Region

    Protected Sub ddlSpeechContext_SelectedIndexChanged(sender As Object, e As EventArgs)
        Me.Initialize()
        ddlContentLang.Enabled = False
        Select Case ddlSpeechContext.Text
            Case "TV"
                txtXArgs.Text = Me.xArgTVContext
                Exit Select
            Case "SocialMedia"
                txtXArgs.Text = Me.xArgSocialMediaContext
                Exit Select
            Case "Generic"
                ddlContentLang.Enabled = True
                txtXArgs.Text = Me.commonXArg
                Exit Select
            Case Else
                txtXArgs.Text = Me.commonXArg
                Exit Select
        End Select

        Me.xArgData = txtXArgs.Text
        txtXArgs.Text = txtXArgs.Text.Replace(",", "," & Environment.NewLine)

        resultsPanel.Visible = False
        tvContextPanel.Visible = False
        tvContextProgramPanel.Visible = False
        tvContextShowtimePanel.Visible = False
        statusPanel.Visible = False

    End Sub
End Class