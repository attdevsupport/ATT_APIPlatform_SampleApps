' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions
Imports ATT_MSSDK
Imports ATT_MSSDK.MMSv3
#End Region

' 
' * This Application demonstrates usage of  AT&T MS SDK wrapper library for sending MMS and
' * getting delivery status of mms.
' * 
' * Pre-requisite:
' * -------------
' * The developer has to register his application in AT&T Developer Platform website, for the scope 
' * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
' * and client secret on registering the application.
' * 
' * Steps to be followed by the application to invoke MMS APIs exposed by MS SDK wrapper library:
' * --------------------------------------------------------------------------------------------
' * 1. Import ATT_MSSDK and ATT_MSSDK.MMSv3 NameSpace.
' * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' * while creating RequestFactory instance.
' *
' * Note: Scopes that are not configured for your application will not work.
' * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' * 
' * 3.Invoke the mms related APIs exposed in the RequestFactory class of MS SDK library.
' * 
' * For mms services MS SDK library provides APIs SendMms() and GetMmsDeliveryResponse()
' * These methods return response objects MmsResponse, MmsDeliveryResponse.
' 
' * Sample code for sending mms:
' * ----------------------------
' List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
' scopes.Add(RequestFactory.ScopeTypes.MMS);
' RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
' MmsResponse resp = requestFactory.SendMms(PhoneNumber, Subject, mmsAttachments);
' 
' * Sample code for getting MMS delivery status:
' * --------------------------------------------
'  MmsDeliveryResponse resp = requestFactory.GetMmsDeliveryResponse(MmsId);
'
'


''' <summary>
''' Mms_App1 class
''' </summary>
''' <remarks> This application allows an end user to send an MMS message with up to three attachments of any common format, 
''' and check the delivery status of that MMS message.
''' </remarks>
Partial Public Class MMS_App1
    Inherits System.Web.UI.Page
    '* \addtogroup MMS_App1
    '     * Description of the application can be referred at \ref MMS_app1 example
    '     * @{
    '     


    '* \example MMS_app1 mms\app1\Default.aspx.cs
    '     * \n \n This application allows an end user to send an MMS message with up to three attachments of any common format, and check the delivery status of that MMS message.
    '     * <ul> 
    '     * <li>SDK methods showcased by the application:</li>
    '     * \n \n <b>Send MMS:</b>
    '     * \n This method sends MMS to one or more mobile network devices.
    '     * \n \n Steps followed in the app to invoke the method:
    '     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.MMSv3 NameSpace.</li>
    '     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
    '     * <li>Invoke \c SendMms() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
    '     * Sample code:
    '     * <pre>
    '     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
    '     *    scopes.Add(RequestFactory.ScopeTypes.MMS);
    '     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
    '     *    MmsResponse resp = requestFactory.SendMms(PhoneNumber, Subject, mmsAttachments);</pre>
    '     * <b>Get MMS Delivery status:</b>
    '     * <pre>
    '     *    MmsDeliveryResponse resp = requestFactory.GetMmsDeliveryResponse(MmsId);</pre>
    '     * 
    '     <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
    '     * \n <li>Additional configuration to be done:</li> \n
    '     * refer \ref parameters_sec section
    '     * 
    '     * \n <li>Documentation can be referred at \ref MMS_App1 section</li></ul>
    '     * @{
    '     


#Region "Instance Variables"

    ''' <summary>
    ''' Gets or sets the value of requestFactory object
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Temporary variables for processing requests
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' List of attachments
    ''' </summary>
    Private mmsAttachments As List(Of String)

#End Region

#Region "MMS Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        BypassCertificateError()

        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
        If Me.requestFactory Is Nothing Then
            Me.Initialize()
        End If
    End Sub

    ''' <summary>
    ''' This method will be called when user clicks on send mms button
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub SendMMSMessageButton_Click(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrEmpty(phoneTextBox.Text) Then
                Me.DrawPanelForFailure(sendMessagePanel, "Specify phone number")
                Return
            End If

            Dim fileSize As Long = 0
            If Not String.IsNullOrEmpty(FileUpload1.FileName) Then
                Me.mmsAttachments.Add(Request.MapPath(FileUpload1.FileName))
                FileUpload1.PostedFile.SaveAs(Request.MapPath(FileUpload1.FileName))
                fileSize = fileSize + (FileUpload1.PostedFile.ContentLength / 1024)
            End If

            If Not String.IsNullOrEmpty(FileUpload2.FileName) Then
                Me.mmsAttachments.Add(Request.MapPath(FileUpload2.FileName))
                FileUpload2.PostedFile.SaveAs(Request.MapPath(FileUpload2.FileName))
                fileSize = fileSize + (FileUpload2.PostedFile.ContentLength / 1024)
            End If

            If Not String.IsNullOrEmpty(FileUpload3.FileName) Then
                Me.mmsAttachments.Add(Request.MapPath(FileUpload3.FileName))
                FileUpload3.PostedFile.SaveAs(Request.MapPath(FileUpload3.FileName))
                fileSize = fileSize + (FileUpload3.PostedFile.ContentLength / 1024)
            End If

            If fileSize <= 600 Then
                Me.SendMMS()

                If File.Exists(Request.MapPath(FileUpload1.FileName)) Then
                    File.Delete(Request.MapPath(FileUpload1.FileName))
                End If

                If File.Exists(Request.MapPath(FileUpload2.FileName)) Then
                    File.Delete(Request.MapPath(FileUpload2.FileName))
                End If

                If File.Exists(Request.MapPath(FileUpload3.FileName)) Then
                    File.Delete(Request.MapPath(FileUpload3.FileName))
                End If
            Else
                Me.DrawPanelForFailure(sendMessagePanel, "Attachment file size exceeded 600kb")
                Return
            End If
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(sendMessagePanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(sendMessagePanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(sendMessagePanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This method will be called when user click on get status button
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub GetStatusButton_Click(sender As Object, e As EventArgs)
        Try
            Dim mmsId As String = messageIDTextBox.Text.Trim()
            If mmsId Is Nothing OrElse mmsId.Length <= 0 Then
                Me.DrawPanelForFailure(getStatusPanel, "Message Id is null or empty")
                Return
            End If

            Dim mmsDeliveryResponseObj As MmsDeliveryResponse = Me.requestFactory.GetMmsDeliveryResponse(mmsId)
            Me.DrawGetStatusSuccess(mmsDeliveryResponseObj.DeliveryInfo(0).DeliveryStatus, mmsDeliveryResponseObj.ResourceURL)
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(getStatusPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(getStatusPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(getStatusPanel, ex.ToString())
        End Try
    End Sub

#End Region

#Region "MMS Application specific functions"

    ''' <summary>
    ''' This method neglects the ssl handshake error with authentication server
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Initializes local variables with values from config file and creates requestFactory object
    ''' </summary>
    Private Sub Initialize()
        If Me.requestFactory Is Nothing Then
            Me.apiKey = ConfigurationManager.AppSettings("api_key")
            If String.IsNullOrEmpty(Me.apiKey) Then
                Me.DrawPanelForFailure(sendMessagePanel, "api_key is not defined in configuration file")
                Return
            End If

            Me.secretKey = ConfigurationManager.AppSettings("secret_key")
            If String.IsNullOrEmpty(Me.secretKey) Then
                Me.DrawPanelForFailure(sendMessagePanel, "secret_key is not defined in configuration file")
                Return
            End If

            Me.endPoint = ConfigurationManager.AppSettings("endPoint")
            If String.IsNullOrEmpty(Me.endPoint) Then
                Me.DrawPanelForFailure(sendMessagePanel, "endPoint is not defined in configuration file")
                Return
            End If

            Me.mmsAttachments = New List(Of String)()

            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(RequestFactory.ScopeTypes.MMS)
            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        End If
    End Sub

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
    ''' Displays Resource url upon success of GetMmsDelivery
    ''' </summary>
    ''' <param name="status">string, status of the request</param>
    ''' <param name="url">string, url of the resource</param>
    Private Sub DrawGetStatusSuccess(status As String, url As String)
        If getStatusPanel.HasControls() Then
            getStatusPanel.Controls.Clear()
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

        getStatusPanel.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Invoke SendMms method of requestFactory object
    ''' </summary>
    Private Sub SendMMS()
        Try
            Dim resp As MmsResponse
            If Me.mmsAttachments IsNot Nothing AndAlso Me.mmsAttachments.Count = 0 Then
                resp = Me.requestFactory.SendMms(phoneTextBox.Text.Trim(), messageTextBox.Text.Trim(), MmsPriority.Normal, chkReceiveNotification.Checked)
            Else
                resp = Me.requestFactory.SendMms(phoneTextBox.Text.Trim(), messageTextBox.Text.Trim(), Me.mmsAttachments, MmsPriority.Normal, 600, chkReceiveNotification.Checked)
            End If

            If Not chkReceiveNotification.Checked Then
                messageIDTextBox.Text = resp.MessageId
            End If
            Me.DrawPanelForSuccess(sendMessagePanel, resp.MessageId)
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(sendMessagePanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(sendMessagePanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(sendMessagePanel, ex.ToString())
        End Try
    End Sub

#End Region

    '* }@ 

    '* }@ 

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs)
        Me.DisplayDeliveryNotificationStatus()
    End Sub

    Private Sub DisplayDeliveryNotificationStatus()
        Dim receivedMessagesFile As String = ConfigurationManager.AppSettings("deliveryStatusFilePath")
        If Not String.IsNullOrEmpty(receivedMessagesFile) Then
            receivedMessagesFile = Server.MapPath(receivedMessagesFile)
        Else
            receivedMessagesFile = Server.MapPath("DeliveryStatus.txt")
        End If
        Dim messagesLine As String = [String].Empty

        Dim receiveMMSDeliveryStatusResponseData As New List(Of DeliveryInfoNotification)()

        If File.Exists(receivedMessagesFile) Then
            Using sr As New StreamReader(receivedMessagesFile)
                While sr.Peek() >= 0
                    Dim dNot As New DeliveryInfoNotification()
                    dNot.deliveryInfo = New DeliveryInfo()
                    messagesLine = sr.ReadLine()
                    Dim messageValues As String() = Regex.Split(messagesLine, "_-_-")
                    dNot.messageId = messageValues(0)
                    dNot.deliveryInfo.address = messageValues(1)
                    dNot.deliveryInfo.deliveryStatus = messageValues(2)
                    receiveMMSDeliveryStatusResponseData.Add(dNot)
                End While
                sr.Close()
                receiveMMSDeliveryStatusResponseData.Reverse()
            End Using
        End If

        Dim notificationTable As New Table()
        notificationTable.Font.Name = "Sans-serif"
        notificationTable.Font.Size = 9
        notificationTable.BorderStyle = BorderStyle.Outset
        notificationTable.Width = Unit.Pixel(650)

        Dim tableRow As New TableRow()

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

        For Each dNot As DeliveryInfoNotification In receiveMMSDeliveryStatusResponseData
            If dNot.deliveryInfo IsNot Nothing Then
                tableRow = New TableRow()

                rowOneCellOne = New TableCell()
                rowOneCellOne.Font.Bold = True
                rowOneCellOne.Text = dNot.messageId
                tableRow.Controls.Add(rowOneCellOne)

                rowOneCellOne = New TableCell()
                rowOneCellOne.Font.Bold = True
                rowOneCellOne.Text = dNot.deliveryInfo.address
                tableRow.Controls.Add(rowOneCellOne)

                rowOneCellOne = New TableCell()
                rowOneCellOne.Font.Bold = True
                rowOneCellOne.Text = dNot.deliveryInfo.deliveryStatus
                tableRow.Controls.Add(rowOneCellOne)
                notificationTable.Controls.Add(tableRow)
            End If
        Next

        notificationTable.BorderWidth = 1
        notificationsPanel.Controls.Clear()
        notificationsPanel.Controls.Add(notificationTable)

    End Sub
End Class