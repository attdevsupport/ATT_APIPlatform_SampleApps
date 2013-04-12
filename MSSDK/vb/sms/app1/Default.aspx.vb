' <copyright file="Default.aspx.cs" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
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
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.SMSv3
Imports System.IO

#End Region

' This Application demonstrates usage of  AT&T MS SDK wrapper library for sending SMS,
' getting delivery status and receiving sms.
' 
' Pre-requisite:
' -------------
' The developer has to register his application in AT&T Developer Platform website, for the scope 
' of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' and client secret on registering the application.
' 
' Steps to be followed by the application to invoke SMS APIs exposed by MS SDK wrapper library:
' --------------------------------------------------------------------------------------------
' 1. Import ATT_MSSDK and ATT_MSSDK.SMSv3 NameSpace.
' 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' while creating RequestFactory instance.
' 
' Note: Scopes that are not configured for your application will not work.
' For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' 
' 3.Invoke the sms related APIs exposed in the RequestFactory class of MS SDK library.
' 
' For sms services MS SDK library provides APIs SendSms(),GetSmsDeliveryResponse() and ReceiveSms()
' These methods return response objects SmsResponse, SmsDeliveryResponse and InboundSmsMessageList.
' 
' Sample code for sending sms:
' ----------------------------
' Dim scopes As New List(Of RequestFactory.ScopeTypes)()
' scopes.Add(RequestFactory.ScopeTypes.SMS)
' Dim target As RequestFactory = New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
' Dim response As SmsResponse = target.SendSms(PhoneNumber, Message)
'  
' Sample code for getting SMS delivery status:
' --------------------------------------------
' Dim resp As SmsDeliveryResponse =  requestFactory.GetSmsDeliveryResponse(SmsId)
' 
' Sample code for receiving sms:
' ------------------------------
' Dim message As InboundSmsMessageList =  requestFactory.ReceiveSms(shortCode)


''' <summary>
''' Default Class
''' </summary>
Partial Public Class SMS_App1
    Inherits System.Web.UI.Page


#Region "Variable Declaration"

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String, shortCode As String

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private shortCodes As String()

#End Region

#Region "SMS Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This fucntion reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            BypassCertificateError()
            If Me.InitializeValues() = False Then
                Return
            End If

            Dim currentServerTime As DateTime = DateTime.UtcNow
            serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

            Me.shortCodes = Me.shortCode.Split(";"c)
            Me.shortCode = Me.shortCodes(0)
            Dim table As New Table()
            table.Font.Size = 8
            For Each srtCode As String In Me.shortCodes
                Dim button As New Button()
                AddHandler button.Click, New EventHandler(AddressOf Me.GetMessagesButton_Click)
                button.Text = "Get Messages for " & srtCode
                Dim rowOne As New TableRow()
                Dim rowOneCellOne As New TableCell()
                rowOne.Controls.Add(rowOneCellOne)
                rowOneCellOne.Controls.Add(button)
                table.Controls.Add(rowOne)
            Next

            receiveMessagePanel.Controls.Add(table)

            If (Not Page.IsPostBack) Then
                notificationsPanel.Visible = False
            End If

        Catch ex As Exception
            Me.DrawPanelForFailure(sendSMSPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This function is called with user clicks on send SMS
    ''' This validates the access token and then calls sendSMS method to invoke send SMS API.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub BtnSendSMS_Click(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrEmpty(txtmsisdn.Text) Then
                Me.DrawPanelForFailure(sendSMSPanel, "Specify phone number")
                Return
            End If

            If String.IsNullOrEmpty(txtmsg.Text) Then
                Me.DrawPanelForFailure(sendSMSPanel, "Specify message to send")
                Return
            End If

            Dim resp As SmsResponse = Me.requestFactory.SendSms(txtmsisdn.Text.Trim(), txtmsg.Text.Trim(), chkReceiveNotification.Checked)
            txtSmsId.Text = resp.MessageId
            Me.DrawPanelForSuccess(sendSMSPanel, resp.MessageId)
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(sendSMSPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(sendSMSPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(sendSMSPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This method is called when user clicks on get delivery status button
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetDeliveryStatusButton_Click(sender As Object, e As EventArgs)
        Try
            Session("smsId") = txtSmsId.Text.Trim()
            Dim resp As SmsDeliveryResponse = Me.requestFactory.GetSmsDeliveryResponse(txtSmsId.Text)
            Me.DrawGetStatusSuccess(resp.DeliveryInfo(0).DeliveryStatus, resp.ResourceURL)
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(getStatusPanel, ex.ToString())
        Catch ex As InvalidResponseException
            'this.DrawPanelForFailure(getStatusPanel, ex.Body + Environment.NewLine + ex.ToString());
            Me.DrawPanelForFailure(getStatusPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(getStatusPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This method retrives received sms for a given short code.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Private Sub GetMessagesButton_Click(sender As Object, e As EventArgs)
        Try
            Dim button As Button = TryCast(sender, Button)
            Dim buttonCaption As String = button.Text.ToString()
            Me.shortCode = buttonCaption.Replace("Get Messages for ", String.Empty)
            Me.RecieveSms()
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(getMessagePanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(getMessagePanel, ex.ToString())
        End Try
    End Sub

#End Region

#Region "SMS Application related functions"

    ''' <summary>
    ''' Neglect the ssl handshake error with authentication server
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Initializes instance members of the <see cref="SMS_App1"/> class.
    ''' </summary>
    ''' <returns>true/false; true if able to read from config file and assigns to instance variables; else false</returns>
    Private Function InitializeValues() As Boolean
        If Me.requestFactory Is Nothing Then

            Me.endPoint = ConfigurationManager.AppSettings("endPoint")
            If String.IsNullOrEmpty(Me.endPoint) Then
                Me.DrawPanelForFailure(sendSMSPanel, "endPoint is not defined in configuration file")
                Return False
            End If

            Me.shortCode = ConfigurationManager.AppSettings("short_code")
            If String.IsNullOrEmpty(Me.shortCode) Then
                Me.DrawPanelForFailure(sendSMSPanel, "short_code is not defined in configuration file")
                Return False
            End If

            Me.apiKey = ConfigurationManager.AppSettings("api_key")
            If String.IsNullOrEmpty(Me.apiKey) Then
                Me.DrawPanelForFailure(sendSMSPanel, "api_key is not defined in configuration file")
                Return False
            End If

            Me.secretKey = ConfigurationManager.AppSettings("secret_key")
            If String.IsNullOrEmpty(Me.secretKey) Then
                Me.DrawPanelForFailure(sendSMSPanel, "secret_key is not defined in configuration file")
                Return False
            End If

            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(requestFactory.ScopeTypes.SMS)
            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        End If

        Return True
    End Function

    ''' <summary>
    ''' This function calls receive sms api to fetch the sms's
    ''' </summary>
    Private Sub RecieveSms()
        Dim message As InboundSmsMessageList = Me.requestFactory.ReceiveSms(Me.shortCode)

        Dim table As New Table()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.Width = Unit.Pixel(650)
        Dim tableRow As New TableRow()
        Dim tableCell As New TableCell()
        tableCell.Width = Unit.Pixel(110)
        tableCell.Text = "SUCCESS:"
        tableCell.Font.Bold = True
        tableRow.Cells.Add(tableCell)
        table.Rows.Add(tableRow)
        tableRow = New TableRow()
        tableCell = New TableCell()
        tableCell.Width = Unit.Pixel(150)
        tableCell.Text = "Messages in this batch:"
        tableCell.Font.Bold = True
        tableRow.Cells.Add(tableCell)
        tableCell = New TableCell()
        tableCell.HorizontalAlign = HorizontalAlign.Left
        tableCell.Text = message.NumberOfMessagesInThisBatch.ToString()
        tableRow.Cells.Add(tableCell)
        table.Rows.Add(tableRow)
        tableRow = New TableRow()
        tableCell = New TableCell()
        tableCell.Width = Unit.Pixel(110)
        tableCell.Text = "Messages pending:"
        tableCell.Font.Bold = True
        tableRow.Cells.Add(tableCell)
        tableCell = New TableCell()
        tableCell.Text = message.TotalNumberOfPendingMessages.ToString()
        tableRow.Cells.Add(tableCell)
        table.Rows.Add(tableRow)
        tableRow = New TableRow()
        table.Rows.Add(tableRow)
        tableRow = New TableRow()
        table.Rows.Add(tableRow)
        Dim secondTable As New Table()
        If message.NumberOfMessagesInThisBatch > 0 Then
            tableRow = New TableRow()
            secondTable.Font.Name = "Sans-serif"
            secondTable.Font.Size = 9
            tableCell = New TableCell()
            tableCell.Width = Unit.Pixel(100)
            tableCell.Text = "Message Index"
            tableCell.HorizontalAlign = HorizontalAlign.Center
            tableCell.Font.Bold = True
            tableRow.Cells.Add(tableCell)
            tableCell = New TableCell()
            tableCell.Font.Bold = True
            tableCell.Width = Unit.Pixel(350)
            tableCell.Wrap = True
            tableCell.Text = "Message Text"
            tableCell.HorizontalAlign = HorizontalAlign.Center
            tableRow.Cells.Add(tableCell)
            tableCell = New TableCell()
            tableCell.Text = "Sender Address"
            tableCell.HorizontalAlign = HorizontalAlign.Center
            tableCell.Font.Bold = True
            tableCell.Width = Unit.Pixel(175)
            tableRow.Cells.Add(tableCell)
            secondTable.Rows.Add(tableRow)

            For Each prime As InboundSmsMessage In message.InboundSmsMessage
                tableRow = New TableRow()
                Dim tableCellmessageId As New TableCell()
                tableCellmessageId.Width = Unit.Pixel(75)
                tableCellmessageId.Text = prime.MessageId.ToString()
                tableCellmessageId.HorizontalAlign = HorizontalAlign.Center
                Dim tableCellmessage As New TableCell()
                tableCellmessage.Width = Unit.Pixel(350)
                tableCellmessage.Wrap = True
                tableCellmessage.Text = prime.Message.ToString()
                tableCellmessage.HorizontalAlign = HorizontalAlign.Center
                Dim tableCellsenderAddress As New TableCell()
                tableCellsenderAddress.Width = Unit.Pixel(175)
                tableCellsenderAddress.Text = prime.SenderAddress.ToString()
                tableCellsenderAddress.HorizontalAlign = HorizontalAlign.Center
                tableRow.Cells.Add(tableCellmessageId)
                tableRow.Cells.Add(tableCellmessage)
                tableRow.Cells.Add(tableCellsenderAddress)
                secondTable.Rows.Add(tableRow)
            Next
        End If

        table.BorderColor = Color.DarkGreen
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc")
        table.BorderWidth = 2

        getMessagePanel.Controls.Add(table)
        getMessagePanel.Controls.Add(secondTable)
    End Sub

    ''' <summary>
    ''' This function calls receive sms api to fetch the sms's
    ''' </summary>
    Private Sub DisplayDeliveryNotificationStatus()
        notificationsPanel.Visible = True
        Dim receivedMessagesFile As String = ConfigurationManager.AppSettings("deliveryStatusFilePath")
        If Not String.IsNullOrEmpty(receivedMessagesFile) Then
            receivedMessagesFile = Server.MapPath(receivedMessagesFile)
        Else
            receivedMessagesFile = Server.MapPath("DeliveryStatus.txt")
        End If
        Dim messagesLine As String = [String].Empty

        Dim receiveSMSDeliveryStatusResponseData As New List(Of DeliveryInfoNotification)()

        If File.Exists(receivedMessagesFile) Then
            Using sr As New StreamReader(receivedMessagesFile)
                While sr.Peek() >= 0
                    Dim dNot As New DeliveryInfoNotification()
                    dNot.DeliveryInfo = New DeliveryInfo()
                    messagesLine = sr.ReadLine()
                    Dim messageValues As String() = Regex.Split(messagesLine, "_-_-")
                    dNot.DeliveryInfo.Id = messageValues(0)
                    dNot.DeliveryInfo.Address = messageValues(1)
                    dNot.DeliveryInfo.DeliveryStatus = messageValues(2)
                    receiveSMSDeliveryStatusResponseData.Add(dNot)
                End While
                sr.Close()
                receiveSMSDeliveryStatusResponseData.Reverse()
            End Using
        End If

        Dim notificationTable As New Table()
        notificationTable.Font.Name = "Sans-serif"
        notificationTable.Font.Size = 9
        notificationTable.BorderStyle = BorderStyle.Outset
        notificationTable.Width = Unit.Pixel(650)

        Dim tableRow As New TableRow()

        tableRow.BorderWidth = 1
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "Message ID"
        tableRow.Controls.Add(rowOneCellOne)

        rowOneCellOne = New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "Address"
        tableRow.Controls.Add(rowOneCellOne)

        rowOneCellOne = New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "Delivery Status"
        tableRow.Controls.Add(rowOneCellOne)

        notificationTable.Controls.Add(tableRow)

        If receiveSMSDeliveryStatusResponseData IsNot Nothing And receiveSMSDeliveryStatusResponseData.Count > 0 Then

            For Each dNot As DeliveryInfoNotification In receiveSMSDeliveryStatusResponseData
                If dNot.DeliveryInfo IsNot Nothing Then
                    tableRow = New TableRow()

                    rowOneCellOne = New TableCell()
                    rowOneCellOne.Font.Bold = True
                    rowOneCellOne.Text = dNot.DeliveryInfo.Id
                    tableRow.Controls.Add(rowOneCellOne)

                    rowOneCellOne = New TableCell()
                    rowOneCellOne.Font.Bold = True
                    rowOneCellOne.Text = dNot.DeliveryInfo.Address
                    tableRow.Controls.Add(rowOneCellOne)

                    rowOneCellOne = New TableCell()
                    rowOneCellOne.Font.Bold = True
                    rowOneCellOne.Text = dNot.DeliveryInfo.DeliveryStatus
                    tableRow.Controls.Add(rowOneCellOne)

                    notificationTable.Controls.Add(tableRow)
                End If
            Next
        End If

        notificationTable.BorderWidth = 1
        notificationsPanel.Controls.Clear()
        notificationsPanel.Controls.Add(notificationTable)

    End Sub

    ''' <summary>
    ''' This function is used to draw the table for get status success response
    ''' </summary>
    ''' <param name="status">Status as string</param>
    ''' <param name="url">url as string</param>
    Private Sub DrawGetStatusSuccess(status As String, url As String)
        Dim table As New Table()
        Dim rowOne As New TableRow()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.Width = Unit.Pixel(650)
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellOne.Text = "Status: "
        rowTwoCellOne.Font.Bold = True
        rowTwo.Controls.Add(rowTwoCellOne)
        rowTwoCellTwo.Text = status.ToString()
        rowTwo.Controls.Add(rowTwoCellTwo)
        table.Controls.Add(rowTwo)
        Dim rowThree As New TableRow()
        Dim rowThreeCellOne As New TableCell()
        Dim rowThreeCellTwo As New TableCell()
        rowThreeCellOne.Text = "ResourceURL: "
        rowThreeCellOne.Font.Bold = True
        rowThree.Controls.Add(rowThreeCellOne)
        rowThreeCellTwo.Text = url.ToString()
        rowThree.Controls.Add(rowThreeCellTwo)
        table.Controls.Add(rowThree)
        table.BorderWidth = 2
        table.BorderColor = Color.DarkGreen
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc")
        getStatusPanel.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' This function is called to draw the table in the panelParam panel for success response
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="message">Message as string</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel, message As String)
        Dim table As New Table()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Font.Bold = True
        rowTwoCellOne.Text = "Message ID:"
        rowTwoCellOne.Width = Unit.Pixel(70)
        rowTwo.Controls.Add(rowTwoCellOne)
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellTwo.Text = message.ToString()
        rowTwo.Controls.Add(rowTwoCellTwo)
        table.Controls.Add(rowTwo)
        table.BorderWidth = 2
        table.BorderColor = Color.DarkGreen
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc")
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' This function draws table for failed response in the panalParam panel
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="message">Message as string</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        Dim table As New Table()
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        table.BorderStyle = BorderStyle.Outset
        table.Width = Unit.Pixel(650)
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
        table.BorderWidth = 2
        table.BorderColor = Color.Red
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc")
        panelParam.Controls.Add(table)
    End Sub

#End Region

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Me.DisplayDeliveryNotificationStatus()
    End Sub
End Class
