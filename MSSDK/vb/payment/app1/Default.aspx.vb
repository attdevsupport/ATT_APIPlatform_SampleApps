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
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.Notaryv1
Imports ATT_MSSDK.Paymentv3
#End Region

' 
'' * This Application demonstrates usage of  payment related methods exposed by AT&T MS SDK wrapper library
'' * for creating a new payment transaction, getting the transaction status, refunding the transaction and 
'' * viewing the notifications received from the platform.
'' *  
'' * Application ptrovides option for creating a new transaction, viewing the status of transaction, refunding
'' * 5 latest transactions and viewing latest 5 notifications from the platform.
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
'' *   i. - Invoke GetNotarizedForNewTransaction()on RequestFactory by passing transaction related parameters like ammount, 
'' *        product description, to get NotaryResponse object. NotaryResponse contains transaction payload that has been 
'' *        notarized/Encrypted by AT&T platform Notary service. 
'' *
'' *      - Get the Transaction redirect url pointing to AT&T platform transaction endpoint by invoking
'' *        GetNewTransactionRedirect(NotaryResponse notaryResponse).
'' *
'' *       -  Redirect the user to AT&T platform transaction endpoint.
'' *
'' *        AT&T platform thows a login page and authenticates the user credentials and requests the user 
'' *        for authorizing the payment transaction.
'' *        Once user authorizes the payment transaction, AT&T platform performs the payment transaction and
'' *        returns the control back to application passing 'TransactionAuthCode' as a query string.
'' *
'' *    ii. Application can use 'TransactionAuthCode' to invoke GetTransactionStatus() on RequestFactory
'' *        to get the status of transaction.
''         
'' *    iii. Application can invoke Refund() on RequestFactory by passing the transaction Id and refund reason
'' *        
' 

''' <summary>
''' Payment class
''' </summary>
''' <remarks>
''' This application allows the user to 
''' Make a new transaction to buy product 1 or product 2, 
''' View the details of the notary signPayload request made in the background, 
''' Get the transaction status of that transaction, 
''' Refund any of the latest five transactions and
''' View the latest five notifications. 
''' </remarks>
Partial Public Class Payment_App1
    Inherits System.Web.UI.Page
    '* \addtogroup Payment_App1
    '     * Description of the application can be referred at \ref Payment_app1 example
    '     * @{
    '     


    '* \example Payment_app1 payment\app1\Default.aspx.vb
    '     * \n \n This application allows the user to 
    '     * \li Make a new transaction to buy product 1 or product 2
    '     * \li View the details of the notary signPayload request made in the background
    '     * \li Get the transaction status of that transaction
    '     * \li Refund any of the latest five transactions
    '     * \li View the latest five notifications. 
    '     * 
    '     * <b>Using Payment Methods:</b>
    '     * \li Import \c ATT_MSSDK and \c ATT_MSSDK.Paymentv3 NameSpace.
    '     * \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    '     * \li Invoke \c GetNotarizedForNewTransaction()on RequestFactory by passing transaction related parameters like ammount, product description, to get NotaryResponse object. 
    '     * \li \c NotaryResponse contains transaction payload that has been notarized/Encrypted by AT&T platform Notary service. 
    '     * \li Get the Transaction redirect url pointing to AT&T platform transaction endpoint by invoking \c GetNewTransactionRedirect(\c NotaryResponse notaryResponse).
    '     * \li Redirect the user to AT&T platform transaction endpoint.
    '     * \li AT&T platform thows a login page and authenticates the user credentials and requests the user for authorizing the payment transaction.
    '     * \li Once user authorizes the payment transaction, AT&T platform performs the payment transaction and returns the control back to application passing 'TransactionAuthCode' as a query string.
    '     * \li Application can use 'TransactionAuthCode' to invoke \c GetTransactionStatus() on \c RequestFactory to get the status of transaction.
    '     * \li Application can invoke \c Refund() on \c RequestFactory by passing the transaction Id and refund reason.
    '     * 
    '     * \n For Registration, Installation, Configuration and Execution, refer \ref Application
    '    * \n \n <b>Additional configuration to be done:</b>
    '    * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
    '    * \li MerchantPaymentRedirectUrl - Set to the URL pointing to the application. AT&T platform uses this URL to return the control back to the application after transaction is completed.
    '    * \li notificationFilePath - This is optional parameter, which points to the file path, where the notification details will be saved by listener.
    '     * \li refundFilePath - This is optional parameter, which points to the file path, where latest transaction IDs will be stored.
    '     * \li noOfNotificationsToDisplay - This is optional key, which will allow to display the defined number of notifications.
    '     * 
    '     * \n Documentation can be referred at \ref Payment_App1 section
    '     * @{
    '    

#Region "Instance Variables"

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private requestFactory As RequestFactory

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private failureTable As Table, successTableGetTransaction As Table, notificationDetailsTable As Table, successTableRefund As Table

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private refundCountToDisplay As Integer

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private amount As Double

    ''' <summary>
    ''' Glboal Variable Declaration
    ''' </summary>
    Private description As String, merchantTransactionId As String, merchantProductId As String, merchantApplicationId As String, merchantRedirectURI As String, transactionTimeString As String, _
     notificationDetailsFile As String

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private transactionTime As DateTime

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private latestFive As Boolean

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private refundList As List(Of KeyValuePair(Of String, String))

    ''' <summary>
    ''' No of notifications to display
    ''' </summary>
    Private noOfNotificationsToDisplay As Integer

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private refundFilePath As String

    ''' <summary>
    ''' Gets or sets the value of transaction amount.
    ''' </summary>
    Private MinTransactionAmount As String, MaxTransactionAmount As String

#End Region

#Region "Payment Application events"

    ''' <summary>
    ''' Event that gets triggered when the page is loaded initially into the browser.
    ''' This method will read all config parameters and initializes RequestFactory instance, 
    ''' Creates refund radio buttons and processes transaction response.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        transactionSuccessTable.Visible = False
        tranGetStatusTable.Visible = False
        refundSuccessTable.Visible = False

        Dim currentServerTime As DateTime = DateTime.UtcNow
        serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"

        Dim ableToInitialize As Boolean = Me.Initialize()
        If ableToInitialize = False Then
            Return
        End If

        If (Request("TransactionAuthCode") IsNot Nothing) AndAlso (Session("merTranId") IsNot Nothing) Then
            Me.ProcessTransactionResponse()
        ElseIf (Request("shown_notary") IsNot Nothing) AndAlso (Session("processNotary") IsNot Nothing) Then
            Session("processNotary") = Nothing
            GetTransactionMerchantTransID.Text = "Merchant Transaction ID: " & Session("tempMerTranId").ToString()
            GetTransactionAuthCode.Text = "Auth Code: " & Session("TranAuthCode").ToString()
        End If

        refundTable.Controls.Clear()
        Me.DrawRefundSection(False)
        Me.DrawNotificationTableHeaders()
        Me.GetNotificationDetails()
    End Sub

    ''' <summary>
    ''' Event to create a new payment transaction
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub NewTransactionButton_Click(sender As Object, e As EventArgs)
        Try
            Me.transactionTime = DateTime.UtcNow
            Me.transactionTimeString = String.Format("{0:dddMMMddyyyyHHmmss}", Me.transactionTime)
            If Radio_TransactionProductType.SelectedIndex = 0 Then
                Me.amount = Convert.ToDouble(Me.MinTransactionAmount)
            ElseIf Radio_TransactionProductType.SelectedIndex = 1 Then
                Me.amount = Convert.ToDouble(Me.MaxTransactionAmount)
            End If

            Session("tranType") = Radio_TransactionProductType.SelectedIndex.ToString()
            Me.description = "TrDesc" & Me.transactionTimeString
            Me.merchantTransactionId = "TrId" & Me.transactionTimeString
            Session("merTranId") = Me.merchantTransactionId.ToString()
            Me.merchantProductId = "ProdId" & Me.transactionTimeString
            Me.merchantApplicationId = "MerAppId" & Me.transactionTimeString

            Dim notaryResponse As NotaryResponse = Me.requestFactory.GetNotarizedForNewTransaction(Me.amount, PaymentCategories.ApplicationGames, Me.description, Me.merchantTransactionId, Me.merchantProductId, Me.merchantRedirectURI)

            Dim newTransactionRedirectUrl As String = Me.requestFactory.GetNewTransactionRedirect(notaryResponse)
            Response.Redirect(newTransactionRedirectUrl)
        Catch ex As Exception
            Me.DrawPanelForFailure(newTransactionPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Event to get notification details
    ''' </summary>
    ''' <param name="sender">an object that raised the event</param>
    ''' <param name="e">Type EventArgs</param>
    Protected Sub BtnGetNotification_Click(sender As Object, e As EventArgs)
        If Me.notificationDetailsTable.Controls IsNot Nothing Then
            Me.notificationDetailsTable.Controls.Clear()
        End If

        Me.DrawNotificationTableHeaders()
        Me.GetNotificationDetails()
    End Sub

    ''' <summary>
    ''' Event to Refund a transaction
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub BtnRefundTransaction_Click1(sender As Object, e As EventArgs)
        Try
            Dim transactionToRefund As String = String.Empty
            Dim recordFound As Boolean = False

            If Me.refundList.Count > 0 Then
                For Each refundTableRow As Control In refundTable.Controls
                    If TypeOf refundTableRow Is TableRow Then
                        For Each refundTableRowCell As Control In refundTableRow.Controls
                            If TypeOf refundTableRowCell Is TableCell Then
                                For Each refundTableCellControl As Control In refundTableRowCell.Controls
                                    If TypeOf refundTableCellControl Is RadioButton Then
                                        If DirectCast(refundTableCellControl, RadioButton).Checked Then
                                            transactionToRefund = DirectCast(refundTableCellControl, RadioButton).Text.ToString()
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
                    Dim refundResponse As RefundResponseObject = Me.requestFactory.Refund(transactionToRefund, 1, "Customer was not happy")
                    refundSuccessTable.Visible = True
                    Me.DrawPanelForRefundSuccess(refundPanel)
                    Me.AddRowToRefundSuccessPanel(refundPanel, "IsSuccess", refundResponse.IsSuccess.ToString())
                    Me.AddRowToRefundSuccessPanel(refundPanel, "OriginalPurchaseAmount", refundResponse.OriginalPurchaseAmount.ToString())
                    Me.AddRowToRefundSuccessPanel(refundPanel, "TransactionId", refundResponse.TransactionId)
                    Me.AddRowToRefundSuccessPanel(refundPanel, "TransactionStatus", refundResponse.TransactionStatus)
                    Me.AddRowToRefundSuccessPanel(refundPanel, "Version", refundResponse.Version)
                    If Me.latestFive = False Then
                        Me.refundList.RemoveAll(Function(x) x.Key.Equals(transactionToRefund))
                        Me.UpdateRefundListToFile()
                        Me.ResetRefundList()
                        refundTable.Controls.Clear()
                        Me.DrawRefundSection(False)
                        GetTransactionMerchantTransID.Text = "Merchant Transaction ID: "
                        GetTransactionAuthCode.Text = "Auth Code: "
                        GetTransactionTransID.Text = "Transaction ID: "
                    End If
                End If
            End If
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(refundPanel, ex.Message)
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(refundPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(refundPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Event to get transaction details
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetTransactionButton_Click(sender As Object, e As EventArgs)
        Try
            Dim keyValue As String = String.Empty
            Dim transactionStatus As TransactionStatus = Nothing

            If Radio_TransactionStatus.SelectedIndex = 0 Then
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Merchant Transaction ID: ", String.Empty)
                transactionStatus = Me.requestFactory.GetTransactionStatus(TransactionIdTypes.MerchantTransactionId, keyValue)
            End If

            If Radio_TransactionStatus.SelectedIndex = 1 Then
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Auth Code: ", String.Empty)
                transactionStatus = Me.requestFactory.GetTransactionStatus(TransactionIdTypes.TransactionAuthCode, keyValue)
            End If

            If Radio_TransactionStatus.SelectedIndex = 2 Then
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Transaction ID: ", String.Empty)
                transactionStatus = Me.requestFactory.GetTransactionStatus(TransactionIdTypes.TransactionId, keyValue)
            End If

            If transactionStatus IsNot Nothing Then
                GetTransactionTransID.Text = "Transaction ID: " + transactionStatus.Id

                If Me.CheckItemInRefundFile(transactionStatus.Id, transactionStatus.MerchantTransactionId) = False Then
                    Me.WriteRefundToFile(transactionStatus.Id, transactionStatus.MerchantTransactionId)
                End If

                refundTable.Controls.Clear()
                Me.DrawRefundSection(False)
                tranGetStatusTable.Visible = True
                Me.DrawPanelForGetTransactionSuccess(newTransactionStatusPanel)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Amount", transactionStatus.Amount.ToString())
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Channel", transactionStatus.Channel)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "ConsumerId", transactionStatus.ConsumerId)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "ContentCategory", transactionStatus.ContentCategory)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Currency", transactionStatus.Currency)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Description", transactionStatus.Description)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "IsSuccess", transactionStatus.IsSuccess.ToString())
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantApplicationId", transactionStatus.MerchantApplicationId)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantId", transactionStatus.MerchantId)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantProductId", transactionStatus.MerchantProductId)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantTransactionId", transactionStatus.MerchantTransactionId)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "OriginalTransactionId", String.Empty)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionId", transactionStatus.Id)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionStatus", transactionStatus.Status)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionType", transactionStatus.Type)
                Me.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Version", transactionStatus.Version)
            Else
                Me.DrawPanelForFailure(newTransactionStatusPanel, "No response from server.")
            End If
        Catch ex As ArgumentException
            Me.DrawPanelForFailure(newTransactionStatusPanel, ex.Message)
        Catch ex As InvalidResponseException
            Me.DrawPanelForFailure(newTransactionStatusPanel, ex.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(newTransactionStatusPanel, ex.Message)
        End Try
    End Sub

#End Region

#Region "Payment Application specific functions"

    ''' <summary>
    ''' Instantiate RequestFactory of ATT_MSSDK by passing endPoint, apiKey, secretKey, scopes
    ''' scope should contain Payment as RequestFactory.ScopeTypes
    ''' </summary>
    ''' <returns>Returns Boolean</returns>
    Private Function Initialize() As Boolean
        Me.MinTransactionAmount = ConfigurationManager.AppSettings("MinTransactionAmount")
        If String.IsNullOrEmpty(Me.MinTransactionAmount) Then
            Me.MinTransactionAmount = "0.00"
        End If
        lstMinAmount.Text = "Buy product 1 for $" + Me.MinTransactionAmount

        Me.MaxTransactionAmount = ConfigurationManager.AppSettings("MaxTransactionAmount")
        If String.IsNullOrEmpty(Me.MaxTransactionAmount) Then
            Me.MaxTransactionAmount = "2.99"
        End If
        lstMaxAmount.Text = "Buy product 2 for $" + Me.MaxTransactionAmount
        If Me.requestFactory Is Nothing Then
            Me.apiKey = ConfigurationManager.AppSettings("api_key")
            If String.IsNullOrEmpty(Me.apiKey) Then
                Me.DrawPanelForFailure(newTransactionPanel, "api_key is not defined in configuration file.")
                Return False
            End If

            Me.secretKey = ConfigurationManager.AppSettings("secret_key")
            If String.IsNullOrEmpty(Me.secretKey) Then
                Me.DrawPanelForFailure(newTransactionPanel, "secret_key is not defined in configuration file.")
                Return False
            End If

            Me.endPoint = ConfigurationManager.AppSettings("endpoint")
            If String.IsNullOrEmpty(Me.endPoint) Then
                Me.DrawPanelForFailure(newTransactionPanel, "endpoint is not defined in configuration file.")
                Return False
            End If

            Me.merchantRedirectURI = ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl")
            If String.IsNullOrEmpty(Me.merchantRedirectURI) Then
                Me.DrawPanelForFailure(newTransactionPanel, "MerchantPaymentRedirectUrl is not defined in configuration file")
                Return False
            End If

            Me.notificationDetailsFile = ConfigurationManager.AppSettings("notificationFilePath")
            If String.IsNullOrEmpty(Me.notificationDetailsFile) Then
                Me.notificationDetailsFile = "Xmlnotification.txt"
            End If

            Me.refundFilePath = ConfigurationManager.AppSettings("refundFilePath")
            If String.IsNullOrEmpty(Me.refundFilePath) Then
                Me.refundFilePath = "refundDetails.txt"
            End If

            If String.IsNullOrEmpty(ConfigurationManager.AppSettings("noOfNotificationsToDisplay")) Then
                Me.noOfNotificationsToDisplay = 5
            Else
                Me.noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("noOfNotificationsToDisplay"))
            End If

            If String.IsNullOrEmpty(ConfigurationManager.AppSettings("refundCountToDisplay")) Then
                Me.refundCountToDisplay = 5
            Else
                Me.refundCountToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("refundCountToDisplay"))
            End If

            Me.refundList = New List(Of KeyValuePair(Of String, String))()
            Me.latestFive = True

            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(requestFactory.ScopeTypes.Payment)

            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        End If

        Return True
    End Function

    ''' <summary>
    ''' Method to update refund list to file.
    ''' </summary>
    Private Sub UpdateRefundListToFile()
        If Me.refundList.Count <> 0 Then
            Me.refundList.Reverse(0, Me.refundList.Count)
        End If

        Using sr As StreamWriter = File.CreateText(Request.MapPath(Me.refundFilePath))
            Dim tempCount As Integer = 0
            While tempCount < Me.refundList.Count
                Dim lineToWrite As String = Me.refundList(tempCount).Key & ";" & Me.refundList(tempCount).Value
                sr.WriteLine(lineToWrite)
                tempCount += 1
            End While

            sr.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Method to get refung list from file.
    ''' </summary>
    Private Sub GetRefundListFromFile()
        If File.Exists(Request.MapPath(Me.refundFilePath)) Then
            Dim fileStream As FileStream = Nothing
            Dim streamReader As StreamReader = Nothing

            Try
                fileStream = New FileStream(Request.MapPath(Me.refundFilePath), FileMode.Open, FileAccess.Read)
                streamReader = New StreamReader(fileStream)
                Dim line As String = String.Empty

                While (InlineAssignHelper(line, streamReader.ReadLine())) IsNot Nothing
                    Dim refundKeys As String() = Regex.Split(line, ";")
                    If refundKeys IsNot Nothing AndAlso refundKeys.Length >= 2 Then
                        If refundKeys(0) IsNot Nothing AndAlso refundKeys(1) IsNot Nothing Then
                            Me.refundList.Add(New KeyValuePair(Of String, String)(refundKeys(0), refundKeys(1)))
                        End If
                    End If
                End While
            Catch ex As Exception
                Me.DrawPanelForFailure(newTransactionPanel, ex.Message)
            Finally
                If streamReader IsNot Nothing Then
                    streamReader.Close()
                End If

                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try

            Me.refundList.Reverse(0, Me.refundList.Count)
        End If
    End Sub

    ''' <summary>
    ''' Method to reset refund list
    ''' </summary>
    Private Sub ResetRefundList()
        Me.refundList.RemoveRange(0, Me.refundList.Count)
    End Sub

    ''' <summary>
    ''' Method to write refund to file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    Private Sub WriteRefundToFile(transactionid As String, merchantTransactionId As String)
        Using appendContent As StreamWriter = File.AppendText(Request.MapPath(Me.refundFilePath))
            Dim line As String = transactionid & ";" & merchantTransactionId
            appendContent.WriteLine(line)
            appendContent.Flush()
            appendContent.Close()
        End Using
    End Sub

    ''' <summary>
    ''' processTransactionResponse is used to display TransactionAuthCode and merchantTransactionId from query string.
    ''' This method will get called on page load after authorization process.
    ''' </summary>
    Private Sub ProcessTransactionResponse()
        lbltrancode.Text = Request("TransactionAuthCode")
        Session("TransactionAuthCode") = Request("TransactionAuthCode")
        lbltranid.Text = Session("merTranId").ToString()
        transactionSuccessTable.Visible = True
        GetTransactionMerchantTransID.Text = "Merchant Transaction ID: " & Session("merTranId").ToString()
        GetTransactionAuthCode.Text = "Auth Code: " & Session("TransactionAuthCode").ToString()
        GetTransactionTransID.Text = "Transaction ID: "
        Session("tempMerTranId") = Session("merTranId").ToString()
        Session("merTranId") = Nothing
        Session("TranAuthCode") = Request("TransactionAuthCode")
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

            Dim count As Integer = 0
            If noOfNotifications > 0 Then
                If Me.notificationDetailsTable IsNot Nothing AndAlso Me.notificationDetailsTable.Controls IsNot Nothing Then
                    Me.notificationDetailsTable.Controls.Clear()
                End If

                Me.DrawNotificationTableHeaders()
            End If

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
    ''' Method to check items in refund file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    ''' <returns>Returns Boolean</returns>
    Private Function CheckItemInRefundFile(transactionid As String, merchantTransactionId As String) As Boolean
        Dim line As String = String.Empty
        Dim lineToFind As String = transactionid & ";" & merchantTransactionId
        Dim fileStream As StreamReader = Nothing
        Dim ableToFind As Boolean = False
        If File.Exists(Request.MapPath(Me.refundFilePath)) Then
            Try
                fileStream = New StreamReader(Request.MapPath(Me.refundFilePath))

                While (InlineAssignHelper(line, fileStream.ReadLine())) IsNot Nothing
                    If line.CompareTo(lineToFind) = 0 Then
                        ableToFind = True
                        Exit While
                    End If
                End While
            Finally
                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        End If
        Return ableToFind
    End Function

    ''' <summary>
    ''' Method is used to create table dynamically with failure message.
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
    ''' Method is used to create table dynamically with GetTransaction details.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForGetTransactionSuccess(panelParam As Panel)
        Me.successTableGetTransaction = New Table()
        Me.successTableGetTransaction.Font.Name = "Sans-serif"
        Me.successTableGetTransaction.Font.Size = 8
        Me.successTableGetTransaction.Width = Unit.Pixel(650)
        Me.successTableGetTransaction.HorizontalAlign = HorizontalAlign.Center
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
    ''' Method to draw panel for refund success
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    Private Sub DrawPanelForRefundSuccess(panelParam As Panel)
        Me.successTableRefund = New Table()
        Me.successTableRefund.Font.Name = "Sans-serif"
        Me.successTableRefund.Font.Size = 8
        Me.successTableRefund.Width = Unit.Pixel(650)
        Me.successTableRefund.HorizontalAlign = HorizontalAlign.Center
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
        Me.successTableRefund.Controls.Add(rowOne)
        panelParam.Controls.Add(Me.successTableRefund)
    End Sub

    ''' <summary>
    ''' Method to draw refund section
    ''' </summary>
    ''' <param name="onlyRow">Row details</param>
    Private Sub DrawRefundSection(onlyRow As Boolean)
        Try
            If onlyRow = False Then
                Dim headingRow As New TableRow()
                Dim headingCellOne As New TableCell()
                headingCellOne.HorizontalAlign = HorizontalAlign.Left
                headingCellOne.CssClass = "cell"
                headingCellOne.Width = Unit.Pixel(200)
                headingCellOne.Font.Bold = True
                headingCellOne.Text = "Transaction ID"
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
                headingCellThree.Text = "Merchant Transaction ID"
                headingRow.Controls.Add(headingCellThree)
                Dim headingCellFour As New TableCell()
                headingCellFour.CssClass = "warning"
                Dim warningMessage As New LiteralControl("<b>WARNING:</b><br/>You must use Get Transaction Status to get the Transaction ID before you can refund it.")
                headingCellFour.Controls.Add(warningMessage)
                headingRow.Controls.Add(headingCellFour)
                refundTable.Controls.Add(headingRow)
            End If

            Me.ResetRefundList()
            Me.GetRefundListFromFile()

            Dim tempCountToDisplay As Integer = 1
            While (tempCountToDisplay <= Me.refundCountToDisplay) AndAlso (tempCountToDisplay <= Me.refundList.Count) AndAlso (Me.refundList.Count > 0)
                Me.AddRowToRefundSection(Me.refundList(tempCountToDisplay - 1).Key, Me.refundList(tempCountToDisplay - 1).Value)
                tempCountToDisplay += 1
            End While
        Catch ex As Exception
            Me.DrawPanelForFailure(newTransactionPanel, ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' method to display notification response table with headers
    ''' </summary>
    Private Sub DrawNotificationTableHeaders()
        Me.notificationDetailsTable = New Table()
        Me.notificationDetailsTable.Font.Name = "Sans-serif"
        Me.notificationDetailsTable.Font.Size = 8
        Me.notificationDetailsTable.Width = Unit.Pixel(650)
        Me.notificationDetailsTable.HorizontalAlign = HorizontalAlign.Center
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
        Me.notificationPanel.Controls.Add(Me.notificationDetailsTable)
    End Sub

    ''' <summary>
    ''' Method to add row to refund section.
    ''' </summary>
    ''' <param name="transaction">Transaction as String</param>
    ''' <param name="merchant">Merchant as string</param>
    Private Sub AddRowToRefundSection(transaction As String, merchant As String)
        Dim rowOne As New TableRow()
        Dim cellOne As New TableCell()
        cellOne.HorizontalAlign = HorizontalAlign.Left
        cellOne.CssClass = "cell"
        cellOne.Width = Unit.Pixel(150)
        Dim rbutton As New RadioButton()
        rbutton.Text = transaction.ToString()
        rbutton.GroupName = "RefundSection"
        rbutton.ID = transaction
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
        cellThree.Text = merchant
        rowOne.Controls.Add(cellThree)

        Dim cellFour As New TableCell()
        cellFour.CssClass = "cell"
        rowOne.Controls.Add(cellFour)

        refundTable.Controls.Add(rowOne)
    End Sub

    ''' <summary>
    ''' This function adds row to the refund success table.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">Attribute as string</param>
    ''' <param name="value">Value as string</param>
    Private Sub AddRowToRefundSuccessPanel(panelParam As Panel, attribute As String, value As String)
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
        Me.successTableRefund.Controls.Add(row)
    End Sub

    ''' <summary>
    ''' Method is used to add row to the GetTransactionDetails table dynamically with GetTransaction details.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <param name="attribute">Attributes as String</param>
    ''' <param name="value">Value as String</param>
    Private Sub AddRowToGetTransactionSuccessPanel(panelParam As Panel, attribute As String, value As String)
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
        Me.notificationPanel.Controls.Add(Me.notificationDetailsTable)
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

#End Region

    '* }@ 

    '* }@ 

End Class