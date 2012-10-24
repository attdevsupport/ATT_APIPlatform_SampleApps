' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.MMSv2
#End Region

' 
'This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Coupons to the handsets and
'getting delivery status of mms.
'
'Pre-requisite:
'-------------
'The developer has to register his application in AT&T Developer Platform website, for the scope 
'of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
'and client secret on registering the application.
'
'Steps to be followed by the application to invoke MMS APIs exposed by MS SDK wrapper library:
'--------------------------------------------------------------------------------------------
'1. Import ATT_MSSDK and ATT_MSSDK.MMSv2 NameSpace.
'2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
'the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
'while creating RequestFactory instance.
'
'Note: Scopes that are not configured for your application will not work.
'For example, your application may be configured in the AT&T API Platform to support the Payment and MMS scopes.
'The RequestFactory may specify any combination of Payment or MMS.  You may specify other scopes, but they will not work.
'
'3.Invoke the mms related APIs exposed in the RequestFactory class of MS SDK library.
'
'For mms services MS SDK library provides APIs SendMms() and GetMmsDeliveryResponse()
'These methods return response objects MmsResponse, MmsDeliveryResponse.
'
'Sample code for sending mms:
'----------------------------
' Dim scopes As New List(Of RequestFactory.ScopeTypes)()
' scopes.Add(RequestFactory.ScopeTypes.MMS)
' Dim target As New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
' Dim resp As MmsResponse = target.SendMms(PhoneNumber, Subject, mmsAttachments)
' 
'Sample code for getting MMS delivery status:
'--------------------------------------------
' Dim resp As MmsDeliveryResponse = target.GetMmsDeliveryResponse(MmsId)

''' <summary>
''' MMS_App2 class
''' </summary>
''' <remarks>
''' This is a server side application which also has a web interface. 
''' The application looks for a file called numbers.txt containing MSISDNs of desired recipients, and an image called coupon.jpg, 
''' and message text from a file called subject.txt, and then sends an MMS message with the attachment to every recipient in the list. 
''' This can be triggered via a command line on the server, or through the web interface, which then displays all the returned mmsIds or respective errors
''' </remarks>
Partial Public Class MMS_App2
    Inherits System.Web.UI.Page

    '* \addtogroup MMS_App2
    '* Description of the application can be referred at \ref MMS_app2 example
    '* @{
    '* 
    '*  \example MMS_app2 mms\app2\Default.aspx.vb
    '* \n \n This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Coupons to the handsets and getting delivery status of mms.
    '*  
    '* <b>Invoke MMS API:</b>
    '* \li Import \c ATT_MSSDK and \c ATT_MSSDK.MMSv2 NameSpace.
    '* \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '* Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    '* \li Invoke \c SendMms() exposed in the \c RequestFactory class of MS SDK library.
    '* 
    '* <b>Sample code:</b>
    '* <pre>
    '*    Dim scopes As New List(Of RequestFactory.ScopeTypes)()
    '*    scopes.Add(RequestFactory.ScopeTypes.MMS)
    '*    Dim target As New RequestFactory(endPoint, apiKey, secretKey, scopes, Nothing, Nothing)
    '*    Dim resp As MmsResponse = target.SendMms(PhoneNumber, Subject, mmsAttachments)
    '* </pre>
    '* <b>Get MMS Delivery status:</b>
    '* <pre>
    '*    Dim resp As MmsDeliveryResponse = target.GetMmsDeliveryResponse(MmsId)
    '* </pre>
    '* Installing and running the application, refer \ref Application 
    '* \n \n <b>Parameters in web.config</b> refer \ref parameters_sec section
    '* 
    '* \n Documentation can be referred at \ref MMS_App2 section
    '* @{

#Region "Instance Variables"

    ''' <summary>
    ''' Gets or sets the value of requestFactory object
    ''' </summary>
    Private requestFactory As RequestFactory

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

    ''' <summary>
    ''' Gets or sets the values of filepath variables
    ''' </summary>
    Private messageFilePath As String, phoneListFilePath As String, couponFilePath As String

    ''' <summary>
    ''' List of addresses to be delivered
    ''' </summary>
    Private mmsAddressList As List(Of String)

    ''' <summary>
    ''' List of MMS attachments
    ''' </summary>
    Private mmsAttachments As List(Of String)

    ''' <summary>
    ''' Instance variables for get status table
    ''' </summary>
    Private getStatusTable As Table, secondTable As Table

#End Region

#Region "MMS Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow
            serverTimeLabel.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

            If Me.Initialize() = False Then
                Return
            End If

            Me.mmsAttachments.Add(Request.MapPath(Me.couponFilePath))
            Image1.ImageUrl = Me.couponFilePath

            Using mmsSubjectReader As New StreamReader(Request.MapPath(Me.messageFilePath))
                subjectLabel.Text = mmsSubjectReader.ReadToEnd()
                mmsSubjectReader.Close()
            End Using

            If Not Page.IsPostBack Then
                Using phoneNumberReader As New StreamReader(Request.MapPath(Me.phoneListFilePath))
                    phoneListTextBox.Text = phoneNumberReader.ReadToEnd()
                    phoneNumberReader.Close()
                End Using
            End If
        Catch ex As Exception
            Me.DrawPanelForFailure(sendMMSPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This method will be called when user clicks on send mms button
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub SendButton_Click(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrEmpty(phoneListTextBox.Text) Then
                Me.DrawPanelForFailure(sendMMSPanel, "Specify phone number")
                Return
            End If

            Dim phoneNumbers As String() = phoneListTextBox.Text.Split(","c)
            Dim phoneNumbersList As New List(Of String)(phoneNumbers.Length)
            phoneNumbersList.AddRange(phoneNumbers)

            Dim resp As MmsResponse = Me.requestFactory.SendMms(phoneNumbersList, subjectLabel.Text.Trim(), Me.mmsAttachments)
            msgIdLabel.Text = resp.Id
            Me.DrawPanelForSuccess(sendMMSPanel, resp.Id)
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(sendMMSPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(sendMMSPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(sendMMSPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This method will be called when user clicks on  get status button
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub StatusButton_Click(sender As Object, e As EventArgs)
        Try
            Dim mmsDeliveryResponseObj As MmsDeliveryResponse = Me.requestFactory.GetMmsDeliveryResponse(msgIdLabel.Text.Trim())
            Me.DrawPanelForGetStatusResult(Nothing, Nothing, Nothing, True)

            For Each deliveryInfo As DeliveryInfo In mmsDeliveryResponseObj.DeliveryInfo
                Me.DrawPanelForGetStatusResult(deliveryInfo.Id, deliveryInfo.Address, deliveryInfo.DeliveryStatus, False)
            Next

            msgIdLabel.Text = String.Empty
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(statusPanel, ex.ToString())
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.ToString())
        End Try
    End Sub

#End Region

#Region "MMS Application specific functions"

    ''' <summary>
    ''' Instantiate RequestFactory of ATT_MSSDK by passing endPoint, apiKey, secretKey, scopes
    ''' </summary>
    ''' <returns>true/false; true if able to read else false</returns>
    Private Function Initialize() As Boolean
        If Me.requestFactory Is Nothing Then
            Me.apiKey = ConfigurationManager.AppSettings("api_key")
            If String.IsNullOrEmpty(Me.apiKey) Then
                Me.DrawPanelForFailure(sendMMSPanel, "api_key is not defined in configuration file")
                Return False
            End If

            Me.secretKey = ConfigurationManager.AppSettings("secret_key")
            If String.IsNullOrEmpty(Me.secretKey) Then
                Me.DrawPanelForFailure(sendMMSPanel, "secret_key is not defined in configuration file")
                Return False
            End If

            Me.endPoint = ConfigurationManager.AppSettings("endPoint")
            If String.IsNullOrEmpty(Me.endPoint) Then
                Me.DrawPanelForFailure(sendMMSPanel, "endPoint is not defined in configuration file")
                Return False
            End If

            Me.messageFilePath = ConfigurationManager.AppSettings("messageFilePath")
            If String.IsNullOrEmpty(Me.messageFilePath) Then
                Me.DrawPanelForFailure(sendMMSPanel, "Message file path is missing in configuration file")
                Return False
            End If

            Me.phoneListFilePath = ConfigurationManager.AppSettings("phoneListFilePath")
            If String.IsNullOrEmpty(Me.phoneListFilePath) Then
                Me.DrawPanelForFailure(sendMMSPanel, "Phone list file path is missing in configuration file")
                Return False
            End If

            Me.couponFilePath = ConfigurationManager.AppSettings("couponFilePath")
            If String.IsNullOrEmpty(Me.couponFilePath) Then
                Me.DrawPanelForFailure(sendMMSPanel, "Coupon file name is missing in configuration file")
                Return False
            End If

            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(RequestFactory.ScopeTypes.MMS)

            Me.mmsAddressList = New List(Of String)()
            Me.mmsAttachments = New List(Of String)()

            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        End If

        Return True
    End Function

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
    ''' This method draws table for get status response
    ''' </summary>
    ''' <param name="msgid">string, Message Id</param>
    ''' <param name="phone">string, phone number</param>
    ''' <param name="status">string, status</param>
    ''' <param name="headerFlag">bool, headerFlag</param>
    Private Sub DrawPanelForGetStatusResult(msgid As String, phone As String, status As String, headerFlag As Boolean)
        If headerFlag = True Then
            Me.getStatusTable = New Table()
            Me.getStatusTable.CssClass = "successWide"
            Me.getStatusTable.Font.Name = "Sans-serif"
            Me.getStatusTable.Font.Size = 9

            Dim rowOne As New TableRow()
            Dim rowOneCellOne As New TableCell()
            rowOneCellOne.Width = Unit.Pixel(110)
            rowOneCellOne.Font.Bold = True
            rowOneCellOne.Text = "SUCCESS:"
            rowOne.Controls.Add(rowOneCellOne)
            Me.getStatusTable.Controls.Add(rowOne)
            Dim rowTwo As New TableRow()
            Dim rowTwoCellOne As New TableCell()
            rowTwoCellOne.Width = Unit.Pixel(250)
            rowTwoCellOne.Text = "Messages Delivered"

            rowTwo.Controls.Add(rowTwoCellOne)
            Me.getStatusTable.Controls.Add(rowTwo)
            Me.getStatusTable.Controls.Add(rowOne)
            Me.getStatusTable.Controls.Add(rowTwo)
            statusPanel.Controls.Add(getStatusTable)

            Me.secondTable = New Table()
            Me.secondTable.Font.Name = "Sans-serif"
            Me.secondTable.Font.Size = 9
            Me.secondTable.Width = Unit.Pixel(650)
            Dim tableRow As New TableRow()
            Dim tableCell As New TableCell()
            tableCell.Width = Unit.Pixel(300)
            tableCell.Text = "Recipient"
            tableCell.HorizontalAlign = HorizontalAlign.Center
            tableCell.Font.Bold = True
            tableRow.Cells.Add(tableCell)
            tableCell = New TableCell()
            tableCell.Font.Bold = True
            tableCell.Width = Unit.Pixel(300)
            tableCell.Wrap = True
            tableCell.Text = "Status"
            tableCell.HorizontalAlign = HorizontalAlign.Center
            tableRow.Cells.Add(tableCell)
            Me.secondTable.Rows.Add(tableRow)
            statusPanel.Controls.Add(secondTable)
        Else
            Dim row As New TableRow()
            Dim cell1 As New TableCell()
            Dim cell2 As New TableCell()
            cell1.Text = phone.ToString()
            cell1.Width = Unit.Pixel(300)
            cell1.HorizontalAlign = HorizontalAlign.Center
            row.Controls.Add(cell1)
            cell2.Text = status.ToString()
            cell2.Width = Unit.Pixel(300)
            cell2.HorizontalAlign = HorizontalAlign.Center
            row.Controls.Add(cell2)
            Me.secondTable.Controls.Add(row)
            statusPanel.Controls.Add(secondTable)
        End If
    End Sub

#End Region
    '* }@
    '* }@
End Class