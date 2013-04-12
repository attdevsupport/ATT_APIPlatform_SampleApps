' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Collections.Generic
Imports System.Configuration
Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.Paymentv3

#End Region

' 
'' * This Application demonstrates usage of  payment related methods exposed by AT&T MS SDK wrapper library
'' * for creating a new subscription, getting the subscription status, refunding the subscription and 
'' * viewing the notifications received from the platform.
'' *  
'' * Application provides option for creating a new subscription, viewing the status of subscription, refunding
'' * 5 latest subscription and viewing latest 5 notifications from the platform.
'' *   
'' * Pre-requisite:
'' * -------------
'' * The developer has to register his application in AT&T Developer Platform website, for the Payment scope 
'' * of AT&T service. AT&T Developer Platform website provides a ClientId and client secret on registering 
'' * the application.
'' * Developper has configured the merchantRedirectURL to point to his application in AT&T Developer Platform 
'' * website.
'' * 
'' * Steps to be followed by the application to invoke Payment APIs exposed by MS SDK wrapper library:
'' * -------------------------------------------------------------------------------------------------
'' * 1. Import ATT_MSSDK and ATT_MSSDK.Paymentv3 NameSpace.
'' * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
'' *    the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
'' *    while creating RequestFactory instance.
'' *
'' *  Note: Scopes that are not configured for your application will not work.
'' *  For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
'' *  The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
'' * 
'' * 3.Invoke Payment related APIs exposed in the RequestFactory class of MS SDK library.
'' *    
'' *   i. - Invoke GetNewSubscriptionRedirect()on RequestFactory by passing subscription related parameters like amount, 
'' *        product description, to get subscriptionRedirect url. 
'' *
'' *       -  Redirect the user to AT&T platform subscription endpoint.
'' *
'' *        AT&T platform thows a login page and authenticates the user credentials and requests the user 
'' *        for authorizing the subscription.
'' *        Once user authorizes the payment subcription, AT&T platform performs the payment subscription and
'' *        returns the control back to application passing 'SubscriptionAuthCode' as a query string.
'' *
'' *    ii. Application can use 'SubscriptionAuthCode' to invoke GetSubscriptionStatus() on RequestFactory
'' *        to get the status of subscription.
''         
'' *    iii. Application can invoke Refund() on RequestFactory by passing the subscription Id and refund reason.
'' *        
''
' *    iv.  Application can invoke CancelSubscription() on RequestFactory by passing the subscriptionId and refundReason to cancel a subscription.


''' <summary>
''' Payment App2 class.
''' This application provides option for creating a new subscription, viewing the status of subscription, refunding
''' 5 latest subscription and viewing latest 5 notifications from the platform.
''' </summary>
Partial Public Class Payment_App2
    Inherits System.Web.UI.Page
    '* \addtogroup Payment_App2
    '     * Description of the application can be referred at \ref Payment_app2 example
    '     * @{
    '     


    '* \example Payment_app2 payment\app2\Default.aspx.vb
    '     * \n \n This application allows the user to 
    '     * \li Make a new subscription to buy product 1 or product 2
    '     * \li Get the subscription status
    '     * \li Refund any of the latest five subscriptions
    '     * \li View the latest five notifications
    '     * 
    '     * <b>Using Payment Methods:</b>
    '     * \li Import \c ATT_MSSDK and \c ATT_MSSDK.Paymentv3 NameSpace.
    '     * \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    '     * \li Invoke GetNewSubscriptionRedirect()on RequestFactory by passing subscription related parameters like amount, product description, to get subscriptionRedirect url. 
    '     * \li Redirect the user to AT&T platform subscription endpoint.
    '     * \li AT&T platform thows a login page and authenticates the user credentials and requests the user for authorizing the payment subscription.
    '     * \li Once user authorizes the payment subscription, AT&T platform performs the payment subscription and returns the control back to application passing 'SubscriptionAuthCode' as a query string.
    '     * \li Application can use 'SubscriptionAuthCode' to invoke \c GetSubscriptionStatus() on \c RequestFactory to get the status of subscription.
    '     * \li Application can invoke \c Refund() on \c RequestFactory by passing the subscription Id and refund reason.
    '     * 
    '     * \n For Registration, Installation, Configuration and Execution, refer \ref Application
    '    * \n \n <b>Additional configuration to be done:</b>
    '    * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
    '    * \li MerchantPaymentRedirectUrl - Set to the URL pointing to the application. ATT platform uses this URL to return the control back to the application after subscription processing is completed.
    '     * \li subscriptionDetailsFilePath - This is optional parameter, which points to the file path, where subscription details will be stored by the application.
    '     * \li subscriptionRefundFilePath - This is optional parameter, which points to the file path, where latest subscription IDs will be stored.
    '     * \li notificationFilePath - This is optional parameter, which points to the file path, where latest notification details will be stored.
    '     * \li notificationCountToDisplay - This is optional key, which will allow to display the defined number of notifications.
    '     * 
    '     * \n Documentation can be referred at \ref Payment_App2 section
    '     * @{
    '    

#Region "Instance Variables"

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private description As String, merchantTransactionId As String, merchantProductId As String, merchantApplicationId As String, merchantRedirectURI As String, transactionTimeString As String, _
     notificationDetailsFile As String, refundFilePath As String

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private successTable As Table, failureTable As Table, successTableGetTransaction As Table, successTableGetSubscriptionDetails As Table, successTableSubscriptionRefund As Table

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private amount As Double

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private noOfNotificationsToDisplay As Integer

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private transactionTime As DateTime

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private subscriptionDetailsFilePath As String

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private latestFive As Boolean = True

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private subsDetailsCountToDisplay As Integer = 0

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private notificationDetailsTable As Table

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private subsDetailsList As New List(Of KeyValuePair(Of String, String))()

    ''' <summary>
    ''' Local Variables
    ''' </summary>
    Private subsRefundList As New List(Of KeyValuePair(Of String, String))()


    ''' <summary>
    ''' Gets or sets the value of transaction amount.
    ''' </summary>
    Private MinSubscriptionAmount As String, MaxSubscriptionAmount As String

#End Region

#Region "Payment App2 events"

    ''' <summary>
    ''' Event that gets triggered when the page is loaded initially into the browser.
    ''' This method will read all config parameters and initializes RequestFactory instance, 
    ''' Creates refund radio buttons and processes subscription response.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            subsRefundSuccessTable.Visible = False
            subsDetailsSuccessTable.Visible = False
            subscriptionSuccessTable.Visible = False
            subsGetStatusTable.Visible = False

            Dim currentServerTime As DateTime = DateTime.UtcNow
            lblServerTime.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

            Dim ableToInitialize As Boolean = Me.Initialize()
            If ableToInitialize = False Then
                Return
            End If

            If (Request("SubscriptionAuthCode") IsNot Nothing) AndAlso (Session("sub_merTranId") IsNot Nothing) Then
                Me.ProcessCreateSubscriptionResponse()
            ElseIf (Request("shown_notary") IsNot Nothing) AndAlso (Session("sub_processNotary") IsNot Nothing) Then
                Session("sub_processNotary") = Nothing
                GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: " & Session("sub_tempMerTranId").ToString()
                GetSubscriptionAuthCode.Text = "Auth Code: " & Session("sub_TranAuthCode").ToString()
            End If

            subsDetailsTable.Controls.Clear()
            Me.DrawSubsDetailsSection(False)
            subsRefundTable.Controls.Clear()
            Me.DrawSubsRefundSection(False)
            Me.DrawNotificationTableHeaders()
            Me.GetNotificationDetails()
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' This method gets subscription redirect Url and redirects the user for getting consent.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub NewSubscriptionButton_Click1(sender As Object, e As EventArgs)
        Try
            Me.ReadTransactionParametersFromConfigurationFile()
            Dim subscriptionRedirect As String = Me.requestFactory.GetNewSubscriptionRedirect(Me.amount, PaymentCategories.ApplicationGames, Me.description, Me.merchantTransactionId, Me.merchantProductId, Me.merchantRedirectURI)
            Response.Redirect(subscriptionRedirect)
        Catch ire As InvalidResponseException
            Me.DrawPanelForFailure(newSubscriptionPanel, ire.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(newSubscriptionPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' View notary button click event
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub ViewNotaryButton_Click(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' Get subscription details for a selected subscription Id
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub GetSubscriptionButton_Click(sender As Object, e As EventArgs)
        Try
            Dim subscriptionStatus As SubscriptionStatus = Nothing
            Dim keyValue As String = String.Empty
            If Radio_SubscriptionStatus.SelectedIndex = 0 Then
                keyValue = Radio_SubscriptionStatus.SelectedItem.Value.ToString().Replace("Merchant Transaction ID: ", String.Empty)
                subscriptionStatus = Me.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.MerchantTransactionId, keyValue)
            End If

            If Radio_SubscriptionStatus.SelectedIndex = 1 Then
                keyValue = Radio_SubscriptionStatus.SelectedItem.Value.ToString().Replace("Auth Code: ", String.Empty)
                subscriptionStatus = Me.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.SubscriptionAuthCode, keyValue)
            End If

            If Radio_SubscriptionStatus.SelectedIndex = 2 Then
                subscriptionStatus = Me.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.SubscriptionId, Session("subscriptionId").ToString())
            End If

            Session("subscriptionId") = subscriptionStatus.Id
            GetSubscriptionID.Text = "Subscription ID: " + subscriptionStatus.Id

            If Me.CheckItemInSubsDetailsFile(subscriptionStatus.MerchantSubscriptionId, subscriptionStatus.ConsumerId) = False Then
                Me.WriteSubsDetailsToFile(subscriptionStatus.MerchantSubscriptionId, subscriptionStatus.ConsumerId)
            End If

            If Me.CheckItemInSubsRefundFile(subscriptionStatus.Id, subscriptionStatus.MerchantSubscriptionId) = False Then
                Me.WriteSubsRefundToFile(subscriptionStatus.Id, subscriptionStatus.MerchantSubscriptionId)
            End If

            subsDetailsTable.Controls.Clear()
            Me.DrawSubsDetailsSection(False)
            subsRefundTable.Controls.Clear()
            Me.DrawSubsRefundSection(False)
            subsGetStatusTable.Visible = True

            Me.DrawPanelForGetSubscriptionSuccess(getSubscriptionStatusPanel)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Amount", subscriptionStatus.Amount.ToString())
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Channel", subscriptionStatus.Channel)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "ConsumerId", subscriptionStatus.ConsumerId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "ContentCategory", subscriptionStatus.ContentCategory)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Currency", subscriptionStatus.Currency)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Description", subscriptionStatus.Description)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "IsAutoCommitted", subscriptionStatus.IsAutoCommitted)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "IsSuccess", subscriptionStatus.IsSuccess.ToString())
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantApplicationId", subscriptionStatus.MerchantApplicationId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantId", subscriptionStatus.MerchantId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantProductId", subscriptionStatus.MerchantProductId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantSubscriptionId", subscriptionStatus.MerchantSubscriptionId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantTransactionId", subscriptionStatus.MerchantTransactionId)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "OriginalTransactionId", String.Empty)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionId", subscriptionStatus.Id)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionPeriod", subscriptionStatus.SubscriptionPeriod)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionPeriodAmount", subscriptionStatus.PeriodAmount.ToString())
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionRecurrences", subscriptionStatus.Recurrences)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionStatus", subscriptionStatus.Status)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionType", subscriptionStatus.Type)
            Me.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Version", subscriptionStatus.Version)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(getSubscriptionStatusPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(getSubscriptionStatusPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Get subscription details button click event
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub BtnGetSubscriptionDetails_Click(sender As Object, e As EventArgs)
        Dim merSubsID As String = String.Empty
        Dim recordFound As Boolean = False
        Try
            If Me.subsDetailsList.Count > 0 Then
                For Each subDetailsTableRow As Control In subsDetailsTable.Controls
                    If TypeOf subDetailsTableRow Is TableRow Then
                        For Each subDetailsTableRowCell As Control In subDetailsTableRow.Controls
                            If TypeOf subDetailsTableRowCell Is TableCell Then
                                For Each subDetailsTableCellControl As Control In subDetailsTableRowCell.Controls
                                    If TypeOf subDetailsTableCellControl Is RadioButton Then
                                        If DirectCast(subDetailsTableCellControl, RadioButton).Checked Then
                                            merSubsID = DirectCast(subDetailsTableCellControl, RadioButton).Text.ToString()
                                            recordFound = True
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

                If recordFound = True Then
                    Dim subscriptionDetails As SubscriptionDetails = Nothing
                    Dim consID As String = Me.GetValueOfKey(merSubsID)
                    If String.IsNullOrEmpty(consID) Then
                        Return
                    End If

                    subscriptionDetails = Me.requestFactory.GetSubscriptionDetails(merSubsID, consID)

                    subsDetailsSuccessTable.Visible = True

                    Me.DrawPanelForGetSubscriptionDetailsSuccess(subsDetailsPanel)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CreationDate", subscriptionDetails.CreationDate)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Currency", subscriptionDetails.Currency)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CurrentEndDate", subscriptionDetails.CurrentEndDate)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CurrentStartDate", subscriptionDetails.CurrentStartDate)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "GrossAmount", subscriptionDetails.GrossAmount)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "IsActiveSubscription", subscriptionDetails.IsActiveSubscription)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "IsSuccess", subscriptionDetails.IsSuccess)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Recurrences", subscriptionDetails.Recurrences)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "RecurrencesLeft", subscriptionDetails.RecurrencesLeft)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Status", subscriptionDetails.Status)
                    Me.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Version", subscriptionDetails.Version)
                End If
            End If
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(subsDetailsPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(subsDetailsPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Refunds a subscription.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub BtnGetSubscriptionRefund_Click(sender As Object, e As EventArgs)
        Dim subsID As String = String.Empty
        Dim recordFound As Boolean = False

        Try
            If Me.subsRefundList.Count > 0 Then
                For Each subRefundTableRow As Control In subsRefundTable.Controls
                    If TypeOf subRefundTableRow Is TableRow Then
                        For Each subRefundTableRowCell As Control In subRefundTableRow.Controls
                            If TypeOf subRefundTableRowCell Is TableCell Then
                                For Each subRefundTableCellControl As Control In subRefundTableRowCell.Controls
                                    If TypeOf subRefundTableCellControl Is RadioButton Then
                                        If DirectCast(subRefundTableCellControl, RadioButton).Checked Then
                                            subsID = DirectCast(subRefundTableCellControl, RadioButton).Text.ToString()
                                            recordFound = True
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

                If recordFound = True Then
                    Dim refundResponse As RefundResponseObject = Me.requestFactory.Refund(subsID, 1, "Customer was not happy")
                    subsRefundSuccessTable.Visible = True

                    Me.DrawPanelForSubscriptionRefundSuccess(subsRefundPanel)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "IsSuccess", refundResponse.IsSuccess.ToString())
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "OriginalPurchaseAmount", refundResponse.OriginalPurchaseAmount.ToString())
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionId", refundResponse.TransactionId)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionStatus", refundResponse.TransactionStatus)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "Version", refundResponse.Version)

                    If Me.latestFive = False Then
                        Me.subsRefundList.RemoveAll(Function(x) x.Key.Equals(subsID))
                        Me.UpdatesSubsRefundListToFile()
                        Me.ResetSubsRefundList()
                        subsRefundTable.Controls.Clear()
                        Me.DrawSubsRefundSection(False)
                        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: "
                        GetSubscriptionAuthCode.Text = "Auth Code: "
                        GetSubscriptionID.Text = "Subscription ID: "
                    End If
                End If
            End If
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(subsRefundPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(subsRefundPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Cancels a subscription.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of arguments</param>
    Protected Sub BtnCancelSubscription_Click(sender As Object, e As EventArgs)
        Dim subsID As String = String.Empty
        Dim recordFound As Boolean = False

        Try
            If Me.subsRefundList.Count > 0 Then
                For Each subRefundTableRow As Control In subsRefundTable.Controls
                    If TypeOf subRefundTableRow Is TableRow Then
                        For Each subRefundTableRowCell As Control In subRefundTableRow.Controls
                            If TypeOf subRefundTableRowCell Is TableCell Then
                                For Each subRefundTableCellControl As Control In subRefundTableRowCell.Controls
                                    If TypeOf subRefundTableCellControl Is RadioButton Then
                                        If DirectCast(subRefundTableCellControl, RadioButton).Checked Then
                                            subsID = DirectCast(subRefundTableCellControl, RadioButton).Text.ToString()
                                            recordFound = True
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

                If recordFound = True Then
                    Dim cancelSubscriptionResponse As CancelSubscriptionResponse = Me.requestFactory.CancelSubscription(subsID, 1, "Customer was not happy")
                    subsRefundSuccessTable.Visible = True

                    Me.DrawPanelForSubscriptionRefundSuccess(subsRefundPanel)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "IsSuccess", cancelSubscriptionResponse.IsSuccess.ToString())
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "OriginalPurchaseAmount", cancelSubscriptionResponse.OriginalPurchaseAmount.ToString())
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionId", cancelSubscriptionResponse.TransactionId)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionStatus", cancelSubscriptionResponse.TransactionStatus)
                    Me.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "Version", cancelSubscriptionResponse.Version)

                    If Me.latestFive = False Then
                        Me.subsRefundList.RemoveAll(Function(x) x.Key.Equals(subsID))
                        Me.UpdatesSubsRefundListToFile()
                        Me.ResetSubsRefundList()
                        subsRefundTable.Controls.Clear()
                        Me.DrawSubsRefundSection(False)
                        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: "
                        GetSubscriptionAuthCode.Text = "Auth Code: "
                        GetSubscriptionID.Text = "Subscription ID: "
                    End If
                End If
            End If
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(subsRefundPanel, ie.Message.ToString())
        Catch ex As Exception
            Me.DrawPanelForFailure(subsRefundPanel, ex.Message.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Refresh notification messages
    ''' </summary>
    ''' <param name="sender">Sender Details</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub BtnRefreshNotifications_Click(sender As Object, e As EventArgs)
        Me.notificationDetailsTable.Controls.Clear()
        Me.DrawNotificationTableHeaders()
        Me.GetNotificationDetails()
    End Sub

#End Region

#Region "Payment App2 specific functions"

    ''' <summary>
    ''' This function is used to neglect the ssl handshake error with authentication server.
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = DirectCast([Delegate].Combine(ServicePointManager.ServerCertificateValidationCallback, Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True), RemoteCertificateValidationCallback)
    End Sub

    ''' <summary>
    ''' Method to read Transaction Parameters from Configuration file.
    ''' </summary>
    Private Sub ReadTransactionParametersFromConfigurationFile()
        Me.transactionTime = DateTime.UtcNow
        Me.transactionTimeString = [String].Format("{0:dddMMMddyyyyHHmmss}", Me.transactionTime)
        If Radio_SubscriptionProductType.SelectedIndex = 0 Then
            Me.amount = Convert.ToDouble(Me.MinSubscriptionAmount)
        ElseIf Radio_SubscriptionProductType.SelectedIndex = 1 Then
            Me.amount = Convert.ToDouble(Me.MaxSubscriptionAmount)
        End If

        Me.description = "TrDesc" & Me.transactionTimeString
        Me.merchantTransactionId = "TrId" & Me.transactionTimeString
        Session("sub_merTranId") = Me.merchantTransactionId
        Me.merchantProductId = "ProdId" & Me.transactionTimeString
        Me.merchantApplicationId = "MerAppId" & Me.transactionTimeString
    End Sub

    ''' <summary>
    ''' Method to get the value of key from the selected row in Refund Section
    ''' </summary>
    ''' <param name="key">Key Value to be found</param>
    ''' <returns>Returns the value in String</returns>
    Private Function GetValueOfKeyFromRefund(key As String) As String
        Dim tempCount As Integer = 0
        While tempCount < Me.subsRefundList.Count
            If Me.subsRefundList(tempCount).Key.CompareTo(key) = 0 Then
                Return Me.subsRefundList(tempCount).Value
            End If

            tempCount += 1
        End While

        Return String.Empty
    End Function

    ''' <summary>
    ''' Method to get the value from Key value
    ''' </summary>
    ''' <param name="key">Key Value to be found</param>
    ''' <returns>Returns the value in String</returns>
    Private Function GetValueOfKey(key As String) As String
        Dim tempCount As Integer = 0
        While tempCount < Me.subsDetailsList.Count
            If Me.subsDetailsList(tempCount).Key.CompareTo(key) = 0 Then
                Return Me.subsDetailsList(tempCount).Value
            End If

            tempCount += 1
        End While

        Return String.Empty
    End Function

    ''' <summary>
    ''' Method to add row to success table.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">Attribute as String</param>
    ''' <param name="value">Value as String</param>
    Private Sub AddRowToSubscriptionRefundSuccessPanel(panelParam As Panel, attribute As String, value As String)
        Dim row As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Right
        cellOne.Text = attribute
        cellOne.Width = Unit.Pixel(300)
        row.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.Width = Unit.Pixel(50)
        row.Controls.Add(cellTwo)
        Dim cellThree As New TableCell()
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Text = value
        cellThree.Width = Unit.Pixel(300)
        row.Controls.Add(cellThree)
        Me.successTableSubscriptionRefund.Controls.Add(row)
    End Sub

    ''' <summary>
    ''' Method to draw panel for successful refund.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForSubscriptionRefundSuccess(panelParam As Panel)
        Me.successTableSubscriptionRefund = New Table()
        Me.successTableSubscriptionRefund.Font.Name = "Sans-serif"
        Me.successTableSubscriptionRefund.Font.Size = 8
        Me.successTableSubscriptionRefund.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right
        rowOneCellOne.Text = "Parameter"
        rowOneCellOne.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellOne)
        Dim rowOneCellTwo As New TableCell()
        rowOneCellTwo.Width = Unit.Pixel(50)
        rowOne.Controls.Add(rowOneCellTwo)

        Dim rowOneCellThree As New TableCell()
        rowOneCellThree.Font.Bold = True
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left
        rowOneCellThree.Text = "Value"
        rowOneCellThree.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellThree)
        Me.successTableSubscriptionRefund.Controls.Add(rowOne)
        panelParam.Controls.Add(Me.successTableSubscriptionRefund)
    End Sub

    ''' <summary>
    ''' Method to draw panel for successful transaction.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForGetSubscriptionDetailsSuccess(panelParam As Panel)
        Me.successTableGetSubscriptionDetails = New Table()
        Me.successTableGetSubscriptionDetails.Font.Name = "Sans-serif"
        Me.successTableGetSubscriptionDetails.Font.Size = 8
        Me.successTableGetSubscriptionDetails.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right
        rowOneCellOne.Text = "Parameter"
        rowOneCellOne.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellOne)
        Dim rowOneCellTwo As New TableCell()
        rowOneCellTwo.Width = Unit.Pixel(50)
        rowOne.Controls.Add(rowOneCellTwo)

        Dim rowOneCellThree As New TableCell()
        rowOneCellThree.Font.Bold = True
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left
        rowOneCellThree.Text = "Value"
        rowOneCellThree.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellThree)
        Me.successTableGetSubscriptionDetails.Controls.Add(rowOne)
        panelParam.Controls.Add(Me.successTableGetSubscriptionDetails)
    End Sub

    ''' <summary>
    ''' Method to add row to success table.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">Attribute as String</param>
    ''' <param name="value">Value as String</param>
    Private Sub AddRowToGetSubscriptionDetailsSuccessPanel(panelParam As Panel, attribute As String, value As String)
        Dim row As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Right
        cellOne.Text = attribute
        cellOne.Width = Unit.Pixel(300)
        row.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.Width = Unit.Pixel(50)
        row.Controls.Add(cellTwo)
        Dim cellThree As New TableCell()
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Text = value
        cellThree.Width = Unit.Pixel(300)
        row.Controls.Add(cellThree)
        Me.successTableGetSubscriptionDetails.Controls.Add(row)
    End Sub

    ''' <summary>
    ''' Method to update Subscription Refund list to the file.
    ''' </summary>
    Private Sub UpdatesSubsRefundListToFile()
        If Me.subsRefundList.Count <> 0 Then
            Me.subsRefundList.Reverse(0, Me.subsRefundList.Count)
        End If

        Using sr As StreamWriter = File.CreateText(Request.MapPath(Me.refundFilePath))
            Dim tempCount As Integer = 0
            While tempCount < Me.subsRefundList.Count
                Dim lineToWrite As String = Me.subsRefundList(tempCount).Key & ":-:" & Me.subsRefundList(tempCount).Value
                sr.WriteLine(lineToWrite)
                tempCount += 1
            End While

            sr.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Method to update Subscription Details list to the file.
    ''' </summary>
    Private Sub UpdateSubsDetailsListToFile()
        If Me.subsDetailsList.Count <> 0 Then
            Me.subsDetailsList.Reverse(0, Me.subsDetailsList.Count)
        End If

        Using sr As StreamWriter = File.CreateText(Me.subscriptionDetailsFilePath)
            Dim tempCount As Integer = 0
            While tempCount < Me.subsDetailsList.Count
                Dim lineToWrite As String = Me.subsDetailsList(tempCount).Key & ":-:" & Me.subsDetailsList(tempCount).Value
                sr.WriteLine(lineToWrite)
                tempCount += 1
            End While

            sr.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Method to check item in Subscription Refund file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id details</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id details</param>
    ''' <returns>Returns True or False</returns>
    Private Function CheckItemInSubsRefundFile(transactionid As String, merchantTransactionId As String) As Boolean
        Dim line As String = String.Empty
        Dim lineToFind As String = transactionid & ":-:" & merchantTransactionId
        If File.Exists(Request.MapPath(Me.refundFilePath)) Then
            Dim file As New System.IO.StreamReader(Request.MapPath(Me.refundFilePath))
            While (InlineAssignHelper(line, file.ReadLine())) IsNot Nothing
                If line.CompareTo(lineToFind) = 0 Then
                    file.Close()
                    Return True
                End If
            End While

            file.Close()
        End If
        Return False
    End Function

    ''' <summary>
    ''' Method to check item in Subscription Details file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id details</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id details</param>
    ''' <returns>Returns True or False</returns>
    Private Function CheckItemInSubsDetailsFile(transactionid As String, merchantTransactionId As String) As Boolean
        Dim line As String = String.Empty
        Dim lineToFind As String = transactionid & ":-:" & merchantTransactionId
        If File.Exists(Me.subscriptionDetailsFilePath) Then
            Dim file As New System.IO.StreamReader(Me.subscriptionDetailsFilePath)
            While (InlineAssignHelper(line, file.ReadLine())) IsNot Nothing
                If line.CompareTo(lineToFind) = 0 Then
                    file.Close()
                    Return True
                End If
            End While

            file.Close()
        End If
        Return False
    End Function

    ''' <summary>
    ''' Method to write Subscription Refund to file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    Private Sub WriteSubsRefundToFile(transactionid As String, merchantTransactionId As String)
        Using appendContent As StreamWriter = File.AppendText(Request.MapPath(Me.refundFilePath))
            Dim line As String = transactionid & ":-:" & merchantTransactionId
            appendContent.WriteLine(line)
            appendContent.Flush()
            appendContent.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Method to write Subscription Details to file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    Private Sub WriteSubsDetailsToFile(transactionid As String, merchantTransactionId As String)
        Using appendContent As StreamWriter = File.AppendText(Me.subscriptionDetailsFilePath)
            Dim line As String = transactionid & ":-:" & merchantTransactionId
            appendContent.WriteLine(line)
            appendContent.Flush()
            appendContent.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Initializes instance variables from Config file
    ''' </summary>
    ''' <returns>true/false; true if able to read from config file and able to instantiate values; else false</returns>
    Private Function Initialize() As Boolean
        Me.MinSubscriptionAmount = ConfigurationManager.AppSettings("MinSubscriptionAmount")
        If String.IsNullOrEmpty(Me.MinSubscriptionAmount) Then
            Me.MinSubscriptionAmount = "0.00"
        End If
        lstMinAmount.Text = "Subscribe for " + Me.MinSubscriptionAmount + " per month"

        Me.MaxSubscriptionAmount = ConfigurationManager.AppSettings("MaxSubscriptionAmount")
        If String.IsNullOrEmpty(Me.MaxSubscriptionAmount) Then
            Me.MaxSubscriptionAmount = "3.99"
        End If
        lstMaxAmount.Text = "Subscribe for " + Me.MaxSubscriptionAmount + " per month"
        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(newSubscriptionPanel, "api_key is not defined in config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(newSubscriptionPanel, "secret_key is not defined in config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(newSubscriptionPanel, "endPoint is not defined in config file")
            Return False
        End If

        Me.merchantRedirectURI = ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl")
        If String.IsNullOrEmpty(Me.merchantRedirectURI) Then
            Me.DrawPanelForFailure(newSubscriptionPanel, "MerchantPaymentRedirectUrl is not defined in config file")
            Return False
        End If

        Me.refundFilePath = ConfigurationManager.AppSettings("subscriptionRefundFilePath")
        If String.IsNullOrEmpty(Me.refundFilePath) Then
            Me.refundFilePath = "subscriptionRefund.txt"
        End If

        Me.subscriptionDetailsFilePath = ConfigurationManager.AppSettings("subscriptionDetailsFilePath")
        If String.IsNullOrEmpty(Me.subscriptionDetailsFilePath) Then
            Me.subscriptionDetailsFilePath = Request.MapPath("subscriptionDetails.txt")
        Else
            Me.subscriptionDetailsFilePath = Request.MapPath(Me.subscriptionDetailsFilePath)
        End If

        Me.notificationDetailsFile = ConfigurationManager.AppSettings("notificationFilePath")
        If String.IsNullOrEmpty(Me.notificationDetailsFile) Then
            Me.notificationDetailsFile = "notificationDetails.txt"
        End If

        Me.noOfNotificationsToDisplay = 5
        If ConfigurationManager.AppSettings("notificationCountToDisplay") IsNot Nothing Then
            Me.noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("notificationCountToDisplay"))
        End If

        Me.subsDetailsCountToDisplay = 5
        If ConfigurationManager.AppSettings("subsDetailsCountToDisplay") IsNot Nothing Then
            Me.subsDetailsCountToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("subsDetailsCountToDisplay"))
        End If

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(requestFactory.ScopeTypes.Payment)
        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)

        Return True
    End Function

    ''' <summary>
    ''' Method to check item in refund file
    ''' </summary>
    ''' <param name="subscriptionid">Subscription Id</param>
    ''' <param name="merchantSubscriptionId">Merchant Subscription Id</param>
    ''' <returns>Return boolean</returns>
    Private Function CheckItemInRefundFile(subscriptionid As String, merchantSubscriptionId As String) As Boolean
        Dim line As String = String.Empty
        Dim lineToFind As String = subscriptionid & ":-:" & Me.merchantTransactionId
        Dim fileStream As StreamReader = Nothing
        If File.Exists(Request.MapPath(Me.refundFilePath)) Then
            Try
                fileStream = New System.IO.StreamReader(Request.MapPath(Me.refundFilePath))
                While (InlineAssignHelper(line, fileStream.ReadLine())) IsNot Nothing
                    If line.Equals(lineToFind) Then
                        Return True
                    End If
                End While
            Catch ex As Exception
                Me.DrawPanelForFailure(newSubscriptionPanel, ex.Message)
            Finally
                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        End If
        Return False
    End Function

    ''' <summary>
    ''' Method to check item in Subscription file
    ''' </summary>
    ''' <param name="consumerId">Consumer Id</param>
    ''' <param name="merchantSubscriptionId">Merchant Subscription If</param>
    ''' <returns>Return Boolean</returns>
    Private Function CheckItemInSubscriptionFile(consumerId As String, merchantSubscriptionId As String) As Boolean
        Dim line As String = String.Empty
        Dim lineToFind As String = consumerId & ":-:" & merchantSubscriptionId
        Dim fileStream As StreamReader = Nothing
        If File.Exists(Me.subscriptionDetailsFilePath) Then
            Try
                fileStream = New StreamReader(Me.subscriptionDetailsFilePath)
                While (InlineAssignHelper(line, fileStream.ReadLine())) IsNot Nothing
                    If line.Equals(lineToFind) Then
                        Return True
                    End If
                End While
            Catch ex As Exception
                Me.DrawPanelForFailure(newSubscriptionPanel, ex.Message)
            Finally
                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        End If
        Return False
    End Function

    ''' <summary>
    ''' Method display SubscriptionAuthCode and Subscription Id from query string.This method will get called
    ''' on page load after authorization process.
    ''' </summary>
    Private Sub ProcessCreateSubscriptionResponse()
        Dim radioButtonValueList As New List(Of String)()
        lblsubscode.Text = Request("SubscriptionAuthCode")
        lblsubsid.Text = Session("sub_merTranId").ToString()
        subscriptionSuccessTable.Visible = True
        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: " & Session("sub_merTranId").ToString()
        GetSubscriptionAuthCode.Text = "Auth Code: " & Request("SubscriptionAuthCode")
        GetSubscriptionID.Text = "Subscription ID: "
        Session("sub_merTranId") = Nothing
    End Sub

    ''' <summary>
    ''' This medthod is used for Drawing Subscription Details Section
    ''' </summary>
    ''' <param name="onlyRow">Row Details</param>
    Private Sub DrawSubsDetailsSection(onlyRow As Boolean)
        Try
            If onlyRow = False Then
                Dim headingRow As New TableRow()
                Dim headingCellOne As New TableCell()
                headingCellOne.HorizontalAlign = HorizontalAlign.Left
                headingCellOne.CssClass = "cell"
                headingCellOne.Width = Unit.Pixel(200)
                headingCellOne.Font.Bold = True
                headingCellOne.Text = "Merchant Subscription ID"
                headingRow.Controls.Add(headingCellOne)
                Dim headingCellTwo As New TableCell()
                headingCellTwo.CssClass = "cell"
                headingCellTwo.Width = Unit.Pixel(100)
                headingRow.Controls.Add(headingCellTwo)
                Dim headingCellThree As New TableCell()
                headingCellThree.CssClass = "cell"
                headingCellThree.HorizontalAlign = HorizontalAlign.Left
                headingCellThree.Width = Unit.Pixel(240)
                headingCellThree.Font.Bold = True
                headingCellThree.Text = "Consumer ID"
                headingRow.Controls.Add(headingCellThree)
                Dim headingCellFour As New TableCell()
                headingCellFour.CssClass = "warning"
                Dim warningMessage As New LiteralControl("<b>WARNING:</b><br/>You must use Get Subscription Status before you can view details of it.")
                headingCellFour.Controls.Add(warningMessage)
                headingRow.Controls.Add(headingCellFour)
                subsDetailsTable.Controls.Add(headingRow)
            End If

            Me.ResetSubsDetailsList()
            Me.GetSubsDetailsFromFile()

            Dim tempCountToDisplay As Integer = 1
            While (tempCountToDisplay <= Me.subsDetailsCountToDisplay) AndAlso (tempCountToDisplay <= Me.subsDetailsList.Count) AndAlso (Me.subsDetailsList.Count > 0)
                Me.AddRowToSubsDetailsSection(Me.subsDetailsList(tempCountToDisplay - 1).Key, Me.subsDetailsList(tempCountToDisplay - 1).Value)
                tempCountToDisplay += 1
            End While
        Catch ex As Exception
            Me.DrawPanelForFailure(subsDetailsPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This medthod is used for adding row in Subscription Details Section
    ''' </summary>
    ''' <param name="subscription">Subscription Details</param>
    ''' <param name="merchantsubscription">Merchant Details</param>
    Private Sub AddRowToSubsDetailsSection(subscription As String, merchantsubscription As String)
        Dim rowOne As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Left
        cellOne.CssClass = "cell"
        cellOne.Width = Unit.Pixel(150)
        Dim rbutton As New RadioButton()
        rbutton.Text = subscription
        rbutton.GroupName = "SubsDetailsSection"
        rbutton.ID = subscription & "ctl " & merchantsubscription
        cellOne.Controls.Add(rbutton)
        rowOne.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.CssClass = "cell"
        cellTwo.Width = Unit.Pixel(100)
        rowOne.Controls.Add(cellTwo)
        Dim cellThree As New TableCell()
        cellThree.CssClass = "cell"
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Width = Unit.Pixel(240)
        cellThree.Text = merchantsubscription
        rowOne.Controls.Add(cellThree)
        Dim cellFour As New TableCell()
        cellFour.CssClass = "cell"
        rowOne.Controls.Add(cellFour)

        subsDetailsTable.Controls.Add(rowOne)
    End Sub

    ''' <summary>
    ''' Method to reset Subscription Details List
    ''' </summary>
    Private Sub ResetSubsDetailsList()
        Me.subsDetailsList.RemoveRange(0, Me.subsDetailsList.Count)
    End Sub

    ''' <summary>
    ''' Method to get Subscription Details from the file.
    ''' </summary>
    Private Sub GetSubsDetailsFromFile()
        If File.Exists(Me.subscriptionDetailsFilePath) Then
            Dim file As New FileStream(Me.subscriptionDetailsFilePath, FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file)
            Dim line As String = String.Empty

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                Dim subsDetailsKeys As String() = Regex.Split(line, ":-:")
                If subsDetailsKeys(0) IsNot Nothing AndAlso subsDetailsKeys(1) IsNot Nothing Then
                    Me.subsDetailsList.Add(New KeyValuePair(Of String, String)(subsDetailsKeys(0), subsDetailsKeys(1)))
                End If
            End While

            sr.Close()
            file.Close()
            Me.subsDetailsList.Reverse(0, Me.subsDetailsList.Count)
        End If
    End Sub

    ''' <summary>
    ''' This medthod is used for drawing Subscription Refund Section
    ''' </summary>
    ''' <param name="onlyRow">Row Details</param>
    Private Sub DrawSubsRefundSection(onlyRow As Boolean)
        Try
            If onlyRow = False Then
                Dim headingRow As New TableRow()
                Dim headingCellOne As New TableCell()
                headingCellOne.HorizontalAlign = HorizontalAlign.Left
                headingCellOne.CssClass = "cell"
                headingCellOne.Width = Unit.Pixel(200)
                headingCellOne.Font.Bold = True
                headingCellOne.Text = "Subscription ID"
                headingRow.Controls.Add(headingCellOne)
                Dim headingCellTwo As New TableCell()
                headingCellTwo.CssClass = "cell"
                headingCellTwo.Width = Unit.Pixel(100)
                headingRow.Controls.Add(headingCellTwo)
                Dim headingCellThree As New TableCell()
                headingCellThree.CssClass = "cell"
                headingCellThree.HorizontalAlign = HorizontalAlign.Left
                headingCellThree.Width = Unit.Pixel(240)
                headingCellThree.Font.Bold = True
                headingCellThree.Text = "Merchant Subscription ID"
                headingRow.Controls.Add(headingCellThree)
                Dim headingCellFour As New TableCell()
                headingCellFour.CssClass = "warning"
                Dim warningMessage As New LiteralControl("<b>WARNING:</b><br/>You must use Get Subscription Status before you can refund or cancel it.")
                headingCellFour.Controls.Add(warningMessage)
                headingRow.Controls.Add(headingCellFour)
                subsRefundTable.Controls.Add(headingRow)
            End If

            Me.ResetSubsRefundList()
            Me.GetSubsRefundFromFile()

            Dim tempCountToDisplay As Integer = 1
            While (tempCountToDisplay <= Me.subsDetailsCountToDisplay) AndAlso (tempCountToDisplay <= Me.subsRefundList.Count) AndAlso (Me.subsRefundList.Count > 0)
                Me.AddRowToSubsRefundSection(Me.subsRefundList(tempCountToDisplay - 1).Key, Me.subsRefundList(tempCountToDisplay - 1).Value)
                tempCountToDisplay += 1
            End While
        Catch ex As Exception
            Me.DrawPanelForFailure(subsRefundPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' This medthod is used for adding row in Subscription Refund Section
    ''' </summary>
    ''' <param name="subscription">Subscription Details</param>
    ''' <param name="merchantsubscription">Merchant Details</param>
    Private Sub AddRowToSubsRefundSection(subscription As String, merchantsubscription As String)
        Dim rowOne As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Left
        cellOne.CssClass = "cell"
        cellOne.Width = Unit.Pixel(150)
        Dim rbutton As New RadioButton()
        rbutton.Text = subscription
        rbutton.GroupName = "SubsRefundSection"
        rbutton.ID = subscription
        cellOne.Controls.Add(rbutton)
        rowOne.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.CssClass = "cell"
        cellTwo.Width = Unit.Pixel(100)
        rowOne.Controls.Add(cellTwo)
        Dim cellThree As New TableCell()
        cellThree.CssClass = "cell"
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Width = Unit.Pixel(240)
        cellThree.Text = merchantsubscription
        rowOne.Controls.Add(cellThree)
        Dim cellFour As New TableCell()
        cellFour.CssClass = "cell"
        rowOne.Controls.Add(cellFour)

        subsRefundTable.Controls.Add(rowOne)
    End Sub

    ''' <summary>
    ''' Method to get Subscription Refund from the file.
    ''' </summary>
    Private Sub GetSubsRefundFromFile()
        If File.Exists(Request.MapPath(Me.refundFilePath)) Then
            Dim file As New FileStream(Request.MapPath(Me.refundFilePath), FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file)
            Dim line As String = String.Empty

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                Dim subsRefundKeys As String() = Regex.Split(line, ":-:")
                If subsRefundKeys.Length = 2 Then
                    If subsRefundKeys(0) IsNot Nothing AndAlso subsRefundKeys(1) IsNot Nothing Then
                        Me.subsRefundList.Add(New KeyValuePair(Of String, String)(subsRefundKeys(0), subsRefundKeys(1)))
                    End If
                End If
            End While

            sr.Close()
            file.Close()
            If Me.subsRefundList.Count <> 0 Then
                Me.subsRefundList.Reverse(0, Me.subsRefundList.Count)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Method to reset Subscription Refund List
    ''' </summary>
    Private Sub ResetSubsRefundList()
        Me.subsRefundList.RemoveRange(0, Me.subsRefundList.Count)
    End Sub

    ''' <summary>
    ''' This function draws the success table
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel)
        Me.successTable = New Table()
        Me.successTable.Font.Name = "Sans-serif"
        Me.successTable.Font.Size = 8
        Me.successTable.BorderStyle = BorderStyle.Outset
        Me.successTable.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        Me.successTable.Controls.Add(rowOne)
        Me.successTable.BorderWidth = 2
        Me.successTable.BorderColor = Color.DarkGreen
        Me.successTable.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc")
        panelParam.Controls.Add(Me.successTable)
    End Sub

    ''' <summary>
    ''' This function adds row to the success table
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">List of attributes</param>
    ''' <param name="value">Value as string</param>
    Private Sub AddRowToSuccessPanel(panelParam As Panel, attribute As String, value As String)
        Dim row As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.Text = attribute
        cellOne.Font.Bold = True
        row.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.Text = value
        row.Controls.Add(cellTwo)
        Me.successTable.Controls.Add(row)
    End Sub

    ''' <summary>
    ''' This function draws error table
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="message">Message as String</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        Me.failureTable = New Table()
        Me.failureTable.Font.Name = "Sans-serif"
        Me.failureTable.Font.Size = 8
        Me.failureTable.BorderStyle = BorderStyle.Outset
        Me.failureTable.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        Me.failureTable.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Text = message.ToString()
        rowTwo.Controls.Add(rowTwoCellOne)
        Me.failureTable.Controls.Add(rowTwo)
        Me.failureTable.BorderWidth = 2
        Me.failureTable.BorderColor = Color.Red
        Me.failureTable.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc")
        panelParam.Controls.Add(Me.failureTable)
    End Sub

    ''' <summary>
    ''' This function draws success table transaction status details
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForGetSubscriptionSuccess(panelParam As Panel)
        Me.successTableGetTransaction = New Table()
        Me.successTableGetTransaction.HorizontalAlign = HorizontalAlign.Center
        Me.successTableGetTransaction.Font.Name = "Sans-serif"
        Me.successTableGetTransaction.Font.Size = 8
        Me.successTableGetTransaction.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right
        rowOneCellOne.Text = "Parameter"
        rowOne.Controls.Add(rowOneCellOne)
        Dim rowOneCellTwo As New TableCell()
        rowOneCellTwo.Width = Unit.Pixel(50)
        rowOne.Controls.Add(rowOneCellTwo)

        Dim rowOneCellThree As New TableCell()
        rowOneCellThree.Font.Bold = True
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left
        rowOneCellThree.Text = "Value"
        rowOne.Controls.Add(rowOneCellThree)
        Me.successTableGetTransaction.Controls.Add(rowOne)
        panelParam.Controls.Add(Me.successTableGetTransaction)
    End Sub

    ''' <summary>
    ''' This function adds row to the success table
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">List of attributes</param>
    ''' <param name="value">Value as String</param>
    Private Sub AddRowToGetSubscriptionSuccessPanel(panelParam As Panel, attribute As String, value As String)
        Dim row As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Right
        cellOne.Text = attribute
        row.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.Width = Unit.Pixel(50)
        row.Controls.Add(cellTwo)
        Dim cellThree As New TableCell()
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Text = value
        row.Controls.Add(cellThree)
        Me.successTableGetTransaction.Controls.Add(row)
    End Sub

    ''' <summary>
    ''' Method to get notification details
    ''' </summary>
    Private Sub GetNotificationDetails()
        Dim notificationDetailsStream As StreamReader = Nothing
        Dim notificationDetail As String = String.Empty
        If Not File.Exists(Request.MapPath(Me.notificationDetailsFile)) Then
            Return
        End If

        Try
            notificationDetailsStream = File.OpenText(Request.MapPath(Me.notificationDetailsFile))
            notificationDetail = notificationDetailsStream.ReadToEnd()
            notificationDetailsStream.Close()

            Dim notificationDetailArray As String() = notificationDetail.Split("$"c)
            Dim noOfNotifications As Integer = 0
            If notificationDetailArray IsNot Nothing Then
                noOfNotifications = notificationDetailArray.Length - 1
            End If

            If noOfNotifications > 0 Then
                If Me.notificationDetailsTable IsNot Nothing AndAlso Me.notificationDetailsTable.Controls IsNot Nothing Then
                    Me.notificationDetailsTable.Controls.Clear()
                End If

                Me.DrawNotificationTableHeaders()
            End If

            Dim count As Integer = 0

            While noOfNotifications >= 0
                Dim notificationDetails As String() = notificationDetailArray(noOfNotifications).Split(":"c)
                If count <= Me.noOfNotificationsToDisplay Then
                    If notificationDetails.Length = 3 Then
                        Me.AddRowToNotificationTable(notificationDetails(0), notificationDetails(1), notificationDetails(2))
                    End If
                Else
                    Exit While
                End If

                count += 1
                noOfNotifications -= 1
            End While
        Catch ex As Exception
            Me.DrawPanelForFailure(notificationPanel, ex.ToString())
        Finally
            If notificationDetailsStream IsNot Nothing Then
                notificationDetailsStream.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Method to display notification response table with headers
    ''' </summary>
    Private Sub DrawNotificationTableHeaders()
        Me.notificationDetailsTable = New Table()
        Me.notificationDetailsTable.Font.Name = "Sans-serif"
        Me.notificationDetailsTable.Font.Size = 8
        Me.notificationDetailsTable.Width = Unit.Pixel(650)
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Left
        rowOneCellOne.Text = "Notification ID"
        rowOneCellOne.Width = Unit.Pixel(400)
        rowOne.Controls.Add(rowOneCellOne)
        Dim rowOneCellTwo As New TableCell()
        rowOneCellTwo.Width = Unit.Pixel(50)
        rowOne.Controls.Add(rowOneCellTwo)

        Dim rowOneCellThree As New TableCell()
        rowOneCellThree.Font.Bold = True
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left
        rowOneCellThree.Text = "Notification Type"
        rowOneCellThree.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellThree)
        Me.notificationDetailsTable.Controls.Add(rowOne)
        Dim rowOneCellFour As New TableCell()
        rowOneCellFour.Width = Unit.Pixel(50)
        rowOne.Controls.Add(rowOneCellFour)

        Dim rowOneCellFive As New TableCell()
        rowOneCellFive.Font.Bold = True
        rowOneCellFive.HorizontalAlign = HorizontalAlign.Left
        rowOneCellFive.Text = "Transaction ID"
        rowOneCellFive.Width = Unit.Pixel(300)
        rowOne.Controls.Add(rowOneCellFive)
        Me.notificationDetailsTable.Controls.Add(rowOne)
        notificationPanel.Controls.Add(Me.notificationDetailsTable)
    End Sub

    ''' <summary>
    ''' Method to add rows to notification response table with notification details
    ''' </summary>
    ''' <param name="notificationId">Notification Id</param>
    ''' <param name="notificationType">Notification Type</param>
    ''' <param name="transactionId">Transaction Id</param>
    Private Sub AddRowToNotificationTable(notificationId As String, notificationType As String, transactionId As String)
        Dim row As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Left
        cellOne.Text = notificationId
        cellOne.Width = Unit.Pixel(400)
        row.Controls.Add(cellOne)
        Dim cellTwo As New TableCell()
        cellTwo.Width = Unit.Pixel(50)
        row.Controls.Add(cellTwo)

        Dim cellThree As New TableCell()
        cellThree.HorizontalAlign = HorizontalAlign.Left
        cellThree.Text = notificationType
        cellThree.Width = Unit.Pixel(300)
        row.Controls.Add(cellThree)
        Dim cellFour As New TableCell()
        cellFour.Width = Unit.Pixel(50)
        row.Controls.Add(cellFour)

        Dim cellFive As New TableCell()
        cellFive.HorizontalAlign = HorizontalAlign.Left
        cellFive.Text = transactionId
        cellFive.Width = Unit.Pixel(300)
        row.Controls.Add(cellFive)
        Dim cellSix As New TableCell()
        cellSix.Width = Unit.Pixel(50)
        row.Controls.Add(cellSix)

        Me.notificationDetailsTable.Controls.Add(row)
        notificationPanel.Controls.Add(Me.notificationDetailsTable)
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

#End Region

    '* }@ 

    '* }@ 

End Class