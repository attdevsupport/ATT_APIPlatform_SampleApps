' <copyright file="Default.aspx.vb" company="AT&amp;T Intellectual Property">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.MIMv1
#End Region

'  This Application demonstrates usage of  AT&T MS SDK wrapper library for Message Inbox Management Api.
'  
'  Pre-requisite:
'  -------------
'  The developer has to register his application in AT&T Developer Platform website, for the scope 
'  of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
'  and client secret on registering the application.
'  
'  Steps to be followed by the application to invoke MIM APIs exposed by MS SDK wrapper library:
'  --------------------------------------------------------------------------------------------
'  1. Import ATT_MSSDK and ATT_MSSDK.MIMv1 NameSpace.
'  2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
'  the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
'  while creating RequestFactory instance.
'  
'  Note: Scopes that are not configured for your application will not work.
'  For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
'  The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
'  
'  3.Invoke the MIM related APIs exposed in the RequestFactory class of MS SDK library.
'  
'  For MIM services MS SDK library provides APIs GetMessageHeaders() and GetMessage()
'  These methods Return response objects GetMessageHeadersResponse, GetMessageContentResponse.
'  
'  Sample code for getting message headers:
'  ----------------------------
'   Dim scopes As List(Of RequestFactory.ScopeTypes) = New List(Of RequestFactory.ScopeTypes)()
'   scopes.Add(requestFactory.ScopeTypes.MIM)
'   Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Me.redirectUrl, Nothing)
'   Dim response As GetMessageHeadersResponse = Me.requestFactory.GetMessageHeaders(headerCount)
'  
'  Sample code for getting message content:
'  ----------------------------------------
'  Dim response As GetMessageContentResponse = requestFactory.GetMessage(messageId, partNumber)

''' <summary>
'''This application allows the AT&amp;T subscriber access to message related data 
'''stored in the AT&amp;T Messages environment.
''' </summary>
Partial Public Class MIM_App1
    Inherits System.Web.UI.Page

#Region "Instance variables"
    ''' <summary>
    ''' Request Factory object for calling api functions
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Application Service specific variables
    ''' </summary>
    Private authCode As String, apiKey As String, secretKey As String, endPoint As String, redirectUrl As String

#End Region

#Region "Application events"

    ''' <summary>
    ''' This menthod is called when the page is loading
    ''' </summary>
    ''' <param name="sender">object pointing to the caller</param>
    ''' <param name="e">Event arguments</param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Try
            pnlHeader.Visible = False
            imagePanel.Visible = False
            smilpanel.Visible = False
            Dim currentServerTime As DateTime = DateTime.UtcNow
            lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC"

            If Me.requestFactory Is Nothing Then
                Dim ableToReadConfigFile As Boolean = Me.ReadConfigFile()
                If ableToReadConfigFile = False Then
                    Return
                End If

                Me.InitializeRequestFactory()
            End If

            ' Get Access Token
            If Not Page.IsPostBack Then
                If Session("VBMIM_ACCESS_TOKEN") Is Nothing Then
                    If Not String.IsNullOrEmpty(Request("code")) Then
                        Me.authCode = Request("code")

                        If Me.requestFactory.AuthorizeCredential Is Nothing Then
                            Me.requestFactory.GetAuthorizeCredentials(Me.authCode)
                        End If

                        Session("VBMIM_ACCESS_TOKEN") = Me.requestFactory.AuthorizeCredential
                    End If
                End If

                ' Process the requests stored in the session
                Dim userEnteredAnyValues As Boolean = Me.GetSessionValues()
                If userEnteredAnyValues = True Then
                    If Session("VBSDKRequest") IsNot Nothing AndAlso Session("VBSDKRequest").ToString().Equals("GetMessageHeaders") Then
                        Me.GetMessageHeaders()
                    ElseIf Session("VBSDKRequest") IsNot Nothing AndAlso Session("VBSDKRequest").ToString().Equals("GetMessageContent") Then
                        Me.GetMessageContentByMessage()
                    End If
                End If
            End If
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ie.Message)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Event, that gets called when user clicks on Get message headers button, 
    ''' performs validations and calls GetMessageHeaders() in RequestFactory instance.
    ''' </summary>
    ''' <param name="sender">object that initiated this method</param>
    ''' <param name="e">Event Agruments</param>
    Protected Sub GetHeaderButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrEmpty(txtHeaderCount.Text.Trim()) Then
            Me.DrawPanelForFailure(statusPanel, "Specify number of messages to be retrieved")
            Return
        End If

        Dim regex As Regex = New Regex("\d+")
        If Not regex.IsMatch(txtHeaderCount.Text.Trim()) Then
            Me.DrawPanelForFailure(statusPanel, "Specify valid header count")
            Return
        End If

        txtHeaderCount.Text = txtHeaderCount.Text.Trim()
        Session("VBSDKHeaderCount") = txtHeaderCount.Text
        Session("VBSDKIndexCursor") = txtIndexCursor.Text

        Dim headerCount As Integer = Convert.ToInt32(txtHeaderCount.Text.Trim())
        If headerCount < 1 Or headerCount > 500 Then
            Me.DrawPanelForFailure(statusPanel, "Header Count must be a number between 1-500")
            Return
        End If

        Session("VBSDKRequest") = "GetMessageHeaders"

        Me.GetMessageHeaders()
    End Sub

    ''' <summary>
    ''' Event, that gets called when user clicks on Get message content button, 
    ''' performs validations and calls GetMessage() in RequestFactory instance.
    ''' </summary>
    ''' <param name="sender">object that initiated this method</param>
    ''' <param name="e">Event Agruments</param>    
    Protected Sub GetMessageContent_Click(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrEmpty(txtMessageId.Text) Then
            Me.DrawPanelForFailure(ContentPanelStatus, "Specify Message ID")
            Return
        End If

        If String.IsNullOrEmpty(txtPartNumber.Text) Then
            Me.DrawPanelForFailure(ContentPanelStatus, "Specify Part Number of the message")
            Return
        End If

        Session("VBSDKMessageID") = txtMessageId.Text
        Session("VBSDKPartNumber") = txtPartNumber.Text

        Session("VBSDKRequest") = "GetMessageContent"

        Me.GetMessageContentByMessage()
    End Sub
#End Region

#Region "MIM Invocation methods"

    ''' <summary>
    ''' Gets message headers based on header count and index cursor.
    ''' </summary>
    Private Sub GetMessageHeaders()
        Try
            If Session("VBMIM_ACCESS_TOKEN") IsNot Nothing Then
                Me.requestFactory.AuthorizeCredential = CType(Session("VBMIM_ACCESS_TOKEN"), OAuthToken)
            End If

            If Me.requestFactory.AuthorizeCredential Is Nothing Then
                Response.Redirect(Me.requestFactory.GetOAuthRedirect().ToString())
            End If

            Dim headerResponse As GetMessageHeadersResponse = Me.requestFactory.GetMessageHeaders(Convert.ToInt32(txtHeaderCount.Text), txtIndexCursor.Text)
            If headerResponse IsNot Nothing Then
                Dim listData As MessageHeadersListData = headerResponse.MessageHeadersList
                If listData IsNot Nothing Then
                    Me.DisplayGrid(listData)
                End If
            End If
        Catch te As TokenExpiredException
            Me.DrawPanelForFailure(statusPanel, te.Message)
        Catch ur As UnauthorizedRequest
            Me.DrawPanelForFailure(statusPanel, ur.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ie.Body)
        Catch are As ArgumentNullException
            Me.DrawPanelForFailure(statusPanel, are.Message)
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(statusPanel, ae.Message)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Displays the deserialized output to a grid.
    ''' </summary>
    ''' <param name="messageHeaders">Deserialized message header list</param>
    Private Sub DisplayGrid(ByVal messageHeaders As MessageHeadersListData)
        Try
            Dim headerTable As DataTable = Me.GetHeaderDataTable()

            If messageHeaders IsNot Nothing AndAlso messageHeaders.Headers IsNot Nothing Then
                pnlHeader.Visible = True
                lblHeaderCount.Text = messageHeaders.HeaderCount.ToString()
                lblIndexCursor.Text = messageHeaders.IndexCursor

                Dim row As DataRow
                Dim header As HeadersData
                For Each header In messageHeaders.Headers
                    row = headerTable.NewRow()

                    row("MessageId") = header.MessageId
                    row("From") = header.From
                    Dim sendTo As String
                    If header.To Is Nothing Then
                        sendTo = String.Empty
                    Else
                        sendTo = String.Join("," + Environment.NewLine, header.To.ToArray())
                    End If

                    row("To") = sendTo
                    row("Received") = header.Received
                    row("Text") = header.Text
                    row("Favourite") = header.Favorite
                    row("Read") = header.Read
                    row("Direction") = header.Direction
                    row("Type") = header.Type
                    headerTable.Rows.Add(row)
                    If Nothing <> header.Type And header.Type.ToLower() = "mms" Then
                        Dim mmsCont As MMSContentData
                        For Each mmsCont In header.MmsContent
                            Dim mmsDetailsRow As DataRow = headerTable.NewRow()
                            mmsDetailsRow("PartNumber") = mmsCont.PartNumber
                            mmsDetailsRow("ContentType") = mmsCont.ContentType
                            mmsDetailsRow("ContentName") = mmsCont.ContentName
                            headerTable.Rows.Add(mmsDetailsRow)
                        Next
                    End If
                Next

                gvMessageHeaders.DataSource = headerTable
                gvMessageHeaders.DataBind()
            End If
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Creates a datatable with message header columns.
    ''' </summary>
    ''' <returns>data table with the structure of the grid</returns>
    Private Function GetHeaderDataTable() As DataTable
        Dim messageTable As DataTable = New DataTable()
        Dim column As DataColumn = New DataColumn("MessageId")
        messageTable.Columns.Add(column)

        column = New DataColumn("PartNumber")
        messageTable.Columns.Add(column)

        column = New DataColumn("ContentType")
        messageTable.Columns.Add(column)

        column = New DataColumn("ContentName")
        messageTable.Columns.Add(column)

        column = New DataColumn("From")
        messageTable.Columns.Add(column)

        column = New DataColumn("To")
        messageTable.Columns.Add(column)

        column = New DataColumn("Received")
        messageTable.Columns.Add(column)

        column = New DataColumn("Text")
        messageTable.Columns.Add(column)

        column = New DataColumn("Favourite")
        messageTable.Columns.Add(column)

        column = New DataColumn("Read")
        messageTable.Columns.Add(column)

        column = New DataColumn("Type")
        messageTable.Columns.Add(column)

        column = New DataColumn("Direction")
        messageTable.Columns.Add(column)

        Return messageTable
    End Function

    ''' <summary>
    ''' Gets message content my message id and part number
    ''' </summary>
    Private Sub GetMessageContentByMessage()
        If Session("VBMIM_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = CType(Session("VBMIM_ACCESS_TOKEN"), OAuthToken)
        End If

        If Me.requestFactory.AuthorizeCredential Is Nothing Then
            Response.Redirect(Me.requestFactory.GetOAuthRedirect().ToString())
        End If

        Try
            Dim contentResponse As GetMessageContentResponse = Me.requestFactory.GetMessage(txtMessageId.Text, txtPartNumber.Text)
            If contentResponse IsNot Nothing Then
                Dim splitData() As String = Regex.Split(contentResponse.MessageType.ToLower(), ";")
                If contentResponse.MessageType.ToLower().Contains("application/smil") Then
                    smilpanel.Visible = True
                    txtSmil.Text = System.Text.Encoding.Default.GetString(contentResponse.MessageContent)
                    Me.DrawPanelForSuccess(ContentPanelStatus, String.Empty)
                ElseIf contentResponse.MessageType.ToLower().Contains("text/plain") Then
                    Me.DrawPanelForSuccess(ContentPanelStatus, System.Text.Encoding.Default.GetString(contentResponse.MessageContent))
                Else
                    imagePanel.Visible = True
                    Me.DrawPanelForSuccess(ContentPanelStatus, String.Empty)
                    imagetoshow.Src = "data:" + splitData(0) + ";base64," + Convert.ToBase64String(contentResponse.MessageContent, Base64FormattingOptions.None)
                End If
            End If
        Catch te As TokenExpiredException
            Me.DrawPanelForFailure(ContentPanelStatus, te.Message)
        Catch ur As UnauthorizedRequest
            Me.DrawPanelForFailure(ContentPanelStatus, ur.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(ContentPanelStatus, ie.Body)
        Catch are As ArgumentNullException
            Me.DrawPanelForFailure(ContentPanelStatus, are.Message)
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(ContentPanelStatus, ae.Message)
        Catch ex As Exception
            Me.DrawPanelForFailure(ContentPanelStatus, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Get session values, user supplied and assign to controls.
    ''' </summary>
    ''' <returns>true/false; true if values supplied, else false</returns>
    Private Function GetSessionValues() As Boolean
        Dim isValuesPresent As Boolean = False

        If Session("VBSDKHeaderCount") IsNot Nothing Then
            txtHeaderCount.Text = Session("VBSDKHeaderCount").ToString()
            isValuesPresent = True
        End If

        If Session("VBSDKIndexCursor") IsNot Nothing Then
            txtIndexCursor.Text = Session("VBSDKIndexCursor").ToString()
            isValuesPresent = True
        End If

        If Session("VBSDKMessageID") IsNot Nothing Then
            txtMessageId.Text = Session("VBSDKMessageID").ToString()
            isValuesPresent = True
        End If

        If Session("VBSDKPartNumber") IsNot Nothing Then
            txtPartNumber.Text = Session("VBSDKPartNumber").ToString()
            isValuesPresent = True
        End If

        Return isValuesPresent
    End Function

    ''' <summary>
    ''' Initializes RequestFactory object.
    ''' </summary>
    ''' <returns>true/false; true if able to read from config file; else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(statusPanel, "api_key is not specified in Config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(statusPanel, "secret_key is not specified in Config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(statusPanel, "endpoint is not specified in Config file")
            Return False
        End If

        Me.redirectUrl = ConfigurationManager.AppSettings("authorize_redirect_uri")
        If String.IsNullOrEmpty(Me.redirectUrl) Then
            Me.DrawPanelForFailure(statusPanel, "authorize_redirect_uri is not specified in Config file")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Initialize an instance of RequestFactory class.
    ''' </summary>
    Private Sub InitializeRequestFactory()
        Dim scopes As List(Of RequestFactory.ScopeTypes) = New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(requestFactory.ScopeTypes.MIM)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Me.redirectUrl, Nothing)

        If Session("VBMIM_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = CType(Session("VBMIM_ACCESS_TOKEN"), OAuthToken)
        End If
    End Sub

    ''' <summary>
    ''' Displays success message.
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForSuccess(ByVal panelParam As Panel, ByVal message As String)
        If panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As Table = New Table()
        table.CssClass = "successWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As TableRow = New TableRow()
        Dim rowOneCellOne As TableCell = New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As TableRow = New TableRow()
        Dim rowTwoCellOne As TableCell = New TableCell()
        rowTwoCellOne.Text = message
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(ByVal panelParam As Panel, ByVal message As String)
        If panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As Table = New Table()
        table.CssClass = "errorWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As TableRow = New TableRow()
        Dim rowOneCellOne As TableCell = New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As TableRow = New TableRow()
        Dim rowTwoCellOne As TableCell = New TableCell()
        rowTwoCellOne.Text = message
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

#End Region
End Class