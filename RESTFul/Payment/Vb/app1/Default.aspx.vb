' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Xml
#End Region

''' <summary>
''' Default Class
''' </summary>
Partial Public Class Payment_App1
    Inherits System.Web.UI.Page
    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private accessTokenFilePath As String, apiKey As String, secretKey As String, accessToken As String, endPoint As String, scope As String, _
     expirySeconds As String, refreshToken As String, accessTokenExpiryTime As String, refreshTokenExpiryTime As String, amount As String, channel As String, _
     description As String, merchantTransactionId As String, merchantProductId As String, merchantApplicationId As String, transactionTimeString As String, notaryURL As String, _
     notificationDetailsFile As String

    Private merchantSubscriptionIdList As String, subscriptionRecurringPeriod As String, subscriptionRecurringNumber As String, subscriptionRecurringPeriodAmount As String, isPurchaseOnNoActiveSubscription As String


    Public TransactionIdFile As String = String.Empty
    Public MerchantTransactionIdFile As String = String.Empty
    Public TransactionAuthorizationCodeFile As String = String.Empty

    Public SubTransactionIdFile As String = String.Empty
    Public SubMerchantTransactionIdFile As String = String.Empty
    Public SubTransactionAuthorizationCodeFile As String = String.Empty
    Public SubMerchantSubscriptionIdFile As String = String.Empty

    Public ConsumerId As String = String.Empty

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private category As Integer, noOfNotificationsToDisplay As Integer

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private merchantRedirectURI As Uri

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private transactionTime As DateTime

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private recordsToDisplay As Integer = 0

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private refundList As New List(Of KeyValuePair(Of String, String))()

    Public notificationDetails As New List(Of Dictionary(Of String, String))()

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer


    ''' <summary>
    ''' Gets or sets the value of transaction amount.
    ''' </summary>
    Public MinTransactionAmount As String, MaxTransactionAmount As String
    Public MinSubscriptionAmount As String, MaxSubscriptionAmount As String

    Public last_transaction_id As String = String.Empty
    Public last_subscription_id As String = String.Empty

    Public MerTransactionIds As New List(Of String)()
    Public transactionAuthCodes As New List(Of String)()
    Public transactionIds As New List(Of String)()

    Public SubMerTransactionIds As New List(Of String)()
    Public SubtransactionAuthCodes As New List(Of String)()
    Public SubtransactionIds As New List(Of String)()
    Public SubMerchantSubscriptionIds As New List(Of String)()

    Public new_transaction_error As String = String.Empty
    Public new_transaction_success As String = String.Empty
    Public transaction_status_error As String = String.Empty
    Public transaction_status_success As String = String.Empty
    Public refund_error As String = String.Empty
    Public refund_success As String = String.Empty

    Public MerSubscriptionIds As New List(Of String)()
    Public SubscriptionAuthCodes As New List(Of String)()
    Public SubscriptionIds As New List(Of String)()

    Public new_subscription_error As String = String.Empty
    Public new_subscription_success As String = String.Empty
    Public subscription_status_error As String = String.Empty
    Public subscription_status_success As String = String.Empty
    Public subscription_refund_error As String = String.Empty
    Public subscription_refund_success As String = String.Empty
    Public subscription_cancel_error As String = String.Empty
    Public subscription_cancel_success As String = String.Empty
    Public subscription_details_error As String = String.Empty
    Public subscription_details_success As String = String.Empty

    Public signedPayload As String = String.Empty
    Public notary_error As String = String.Empty
    Public notary_success As String = String.Empty
    Public signedSignature As String = String.Empty

    Public formattedResponse As New Dictionary(Of String, String)()

    Public getTransactionStatusResponse As New Dictionary(Of String, String)()
    Public refundResponse As New Dictionary(Of String, String)()

    Public getSubscriptionStatusResponse As New Dictionary(Of String, String)()
    Public subscriptionRefundResponse As New Dictionary(Of String, String)()
    Public getSubscriptionDetailsResponse As New Dictionary(Of String, String)()

    Public showTransaction As String = String.Empty
    Public showSubscription As String = String.Empty
    Public showNotary As String = String.Empty
    Public showNotification As String = String.Empty


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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Dim ableToReadFromConfig As Boolean = Me.ReadConfigFile()

        If ableToReadFromConfig = False Then
            Return
        End If

        If (Request("TransactionAuthCode") IsNot Nothing) AndAlso (Session("vb_merTranId") IsNot Nothing) AndAlso (Session("vb_tranType") IsNot Nothing) Then
            showTransaction = "true"
            'return;
            Me.ProcessCreateTransactionResponse()
        End If
        If (Request("SubscriptionAuthCode") IsNot Nothing) AndAlso (Session("vb_sub_merTranId") IsNot Nothing) AndAlso (Session("vb_subType") IsNot Nothing) Then
            showSubscription = "true"
            'return;
            Me.ProcessCreateSubscriptionResponse()
        End If

        GetNotificationsFromFile()

        If Not IsPostBack Then
            updateListForTransactionIds()
            updateListsForAuthCode()
            updateListsForMerchantTransactionId()

            updateListForSubMerchantSubscriptionIds()
            updateListForSubTransactionIds()
            updateListsForSubAuthCode()
            updateListsForSubMerchantTransactionId()
        End If

        If Session("vb_tranType") Is Nothing Then
            product_1.Checked = True
        End If
        If Session("vb_subType") Is Nothing Then
            subproduct_1.Checked = True
        End If
    End Sub

    Private Sub DisplayDictionary(dict As Dictionary(Of String, Object))
        For Each strKey As String In dict.Keys
            'string strOutput = "".PadLeft(indentLevel * 8) + strKey + ":";

            Dim o As Object = dict(strKey)
            If TypeOf o Is Dictionary(Of String, Object) Then
                DisplayDictionary(DirectCast(o, Dictionary(Of String, Object)))
            ElseIf TypeOf o Is ArrayList Then
                For Each oChild As Object In DirectCast(o, ArrayList)
                    If TypeOf oChild Is String Then
                        'formattedResponse.Add(strOutput, "");
                        Dim strOutput As String = DirectCast(oChild, String)
                    ElseIf TypeOf oChild Is Dictionary(Of String, Object) Then
                        DisplayDictionary(DirectCast(oChild, Dictionary(Of String, Object)))
                    End If
                Next
            Else
                formattedResponse.Add(strKey.ToString(), o.ToString())
            End If
        Next
    End Sub

    ''' <summary>
    ''' Method to process create transaction response
    ''' </summary>
    Public Sub ProcessCreateTransactionResponse()
        If Me.CheckItemInFile(Session("vb_merTranId").ToString(), Me.MerchantTransactionIdFile) = False Then
            Me.WriteRecordToFile(Session("vb_merTranId").ToString(), Me.MerchantTransactionIdFile)
            updateListsForMerchantTransactionId()
        End If
        If Me.CheckItemInFile(Request("TransactionAuthCode").ToString(), Me.TransactionAuthorizationCodeFile) = False Then
            Me.WriteRecordToFile(Request("TransactionAuthCode").ToString(), Me.TransactionAuthorizationCodeFile)
            updateListsForAuthCode()
        End If
        Dim sessionValue As String = Session("vb_tranType").ToString()
        If Not String.IsNullOrEmpty(sessionValue) Then
            If sessionValue.CompareTo("product_1") = 0 Then
                product_1.Checked = True
            Else
                product_2.Checked = True
            End If
        End If
        'Session["tranType"] = null;
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/MerchantTransactionId/" & Session("vb_merTranId").ToString()
        Me.getTransactionStatus(resourcePathString, new_transaction_error, transaction_status_success)
        payload.Value = Session("vb_signedData").ToString()
        signedPayload = Session("vb_signedPayload").ToString()
        signedSignature = Session("vb_signedSignature").ToString()
        Session("vb_signedData") = Nothing
        Session("vb_signedPayload") = Nothing
        Session("vb_signedSignature") = Nothing
        Session("vb_merTranId") = Nothing
        Return
    End Sub

    ''' <summary>
    ''' Method to process create transaction response
    ''' </summary>
    Public Sub ProcessCreateSubscriptionResponse()
        If Me.CheckItemInFile(Session("vb_sub_merTranId").ToString(), Me.SubMerchantTransactionIdFile) = False Then
            Me.WriteRecordToFile(Session("vb_sub_merTranId").ToString(), Me.SubMerchantTransactionIdFile)
            updateListsForSubMerchantTransactionId()
        End If
        If Me.CheckItemInFile(Request("SubscriptionAuthCode").ToString(), Me.SubTransactionAuthorizationCodeFile) = False Then
            Me.WriteRecordToFile(Request("SubscriptionAuthCode").ToString(), Me.SubTransactionAuthorizationCodeFile)
            updateListsForSubAuthCode()
        End If
        Dim sessionValue As String = Session("vb_subType").ToString()
        If Not String.IsNullOrEmpty(sessionValue) Then
            If sessionValue.CompareTo("subproduct_1") = 0 Then
                subproduct_1.Checked = True
            Else
                subproduct_2.Checked = True
            End If
        End If
        'Session["subType"] = null;
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions/MerchantTransactionId/" & Session("vb_sub_merTranId").ToString()
        Me.getSubscriptionStatus(resourcePathString, new_subscription_error, subscription_status_success)
        payload.Value = Session("vb_signedData").ToString()
        signedPayload = Session("vb_signedPayload").ToString()
        signedSignature = Session("vb_signedSignature").ToString()
        Session("vb_signedData") = Nothing
        Session("vb_signedPayload") = Nothing
        Session("vb_signedSignature") = Nothing
        Session("vb_sub_merTranId") = Nothing
        Return
    End Sub

    ''' <summary>
    ''' Get Subscription button click event
    ''' </summary>
    ''' <param name="sender">Sender Details</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub getSubscriptionStatus(resourcePathString As String, ByRef [error] As String, ByRef success As String)
        Try
            If Me.ReadAndGetAccessToken([error]) = True Then
                If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
                    Return
                End If

                Dim objRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(resourcePathString), HttpWebRequest)
                objRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                objRequest.Method = "GET"

                Dim getTransactionStatusResponseObject As HttpWebResponse = DirectCast(objRequest.GetResponse(), HttpWebResponse)

                Using getTransactionStatusResponseStream As New StreamReader(getTransactionStatusResponseObject.GetResponseStream())
                    Dim getTransactionStatusResponseData As String = getTransactionStatusResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim dict As Dictionary(Of String, Object) = deserializeJsonObject.Deserialize(Of Dictionary(Of String, Object))(getTransactionStatusResponseData)
                    DisplayDictionary(dict)
                    getSubscriptionStatusResponse = formattedResponse
                    success = "true"
                    Session("vb_ConsumerId") = getSubscriptionStatusResponse("ConsumerId")
                    If Me.CheckItemInFile(getSubscriptionStatusResponse("SubscriptionId"), Me.SubTransactionIdFile) = False Then
                        Me.WriteRecordToFile(getSubscriptionStatusResponse("SubscriptionId"), Me.SubTransactionIdFile)
                        updateListForSubTransactionIds()
                    End If
                    If Me.CheckItemInFile(getSubscriptionStatusResponse("MerchantSubscriptionId"), Me.SubMerchantSubscriptionIdFile) = False Then
                        Me.WriteRecordToFile(getSubscriptionStatusResponse("MerchantSubscriptionId"), Me.SubMerchantSubscriptionIdFile)
                        updateListForSubMerchantSubscriptionIds()
                    End If
                    getTransactionStatusResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    Dim reader As New StreamReader(stream)
                    [error] = reader.ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            [error] = ex.ToString()
        End Try
    End Sub

    Protected Sub GetSubscriptionDetails_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(getSDetailsMSID.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim merSubsID As String = getSDetailsMSID.SelectedValue.ToString()
        Dim consID As String = String.Empty
        If Session("vb_ConsumerId") IsNot Nothing Then
            consID = Session("vb_ConsumerId").ToString()
        Else
            consID = Me.ConsumerId
        End If
        Try
            If Me.ReadAndGetAccessToken(subscription_details_error) = True Then
                If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
                    Return
                End If

                Dim objRequest As WebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions/" & merSubsID & "/Detail/" & consID), WebRequest)

                objRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                objRequest.Method = "GET"
                objRequest.ContentType = "application/json"

                Dim subsDetailsResponeObject As WebResponse = DirectCast(objRequest.GetResponse(), WebResponse)

                Using subsDetailsResponseStream As New StreamReader(subsDetailsResponeObject.GetResponseStream())
                    Dim subsDetailsResponseData As String = subsDetailsResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim dict As Dictionary(Of String, Object) = deserializeJsonObject.Deserialize(Of Dictionary(Of String, Object))(subsDetailsResponseData)
                    DisplayDictionary(dict)
                    getSubscriptionDetailsResponse = formattedResponse
                    subscription_details_success = "true"
                    subsDetailsResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    Dim reader As New StreamReader(stream)
                    subscription_details_error = reader.ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            subscription_details_error = ex.ToString()
        End Try
    End Sub


    Protected Sub CancelSubscription_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(cancelSubscriptionId.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim subscriptionToCancel As String = cancelSubscriptionId.SelectedValue.ToString()
        Dim strReq As String = "{""TransactionOperationStatus"":""SubscriptionCancelled"",""RefundReasonCode"":1,""RefundReasonText"":""Customer was not happy""}"
        RefundSubscriptionOperation(strReq, subscriptionToCancel, subscription_cancel_success, subscription_cancel_error)
    End Sub



    Protected Sub RefundSubscription_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(refundSubscriptionId.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim subscriptionToRefund As String = refundSubscriptionId.SelectedValue.ToString()
        Dim strReq As String = "{""TransactionOperationStatus"":""Refunded"",""RefundReasonCode"":1,""RefundReasonText"":""Customer was not happy""}"
        RefundSubscriptionOperation(strReq, subscriptionToRefund, subscription_refund_success, subscription_refund_error)
    End Sub

    Protected Sub RefundSubscriptionOperation(strRequest As String, subid As String, ByRef success As String, ByRef [error] As String)
        Dim dataLength As String = String.Empty
        Try
            If Me.ReadAndGetAccessToken(subscription_refund_error) = True Then
                If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
                    Return
                End If
                Dim objRequest As WebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/" & subid.ToString()), WebRequest)
                objRequest.Method = "PUT"
                objRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                objRequest.ContentType = "application/json"
                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(strRequest)
                objRequest.ContentLength = postBytes.Length
                Dim postStream As Stream = objRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)
                dataLength = postBytes.Length.ToString()
                postStream.Close()
                Dim refundTransactionResponeObject As WebResponse = DirectCast(objRequest.GetResponse(), WebResponse)
                Using refundResponseStream As New StreamReader(refundTransactionResponeObject.GetResponseStream())
                    Dim refundTransactionResponseData As String = refundResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim dict As Dictionary(Of String, Object) = deserializeJsonObject.Deserialize(Of Dictionary(Of String, Object))(refundTransactionResponseData)
                    DisplayDictionary(dict)
                    subscriptionRefundResponse = formattedResponse
                    success = "true"
                    refundResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    [error] = New StreamReader(stream).ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            [error] = ex.ToString()
        End Try
    End Sub


    ''' <summary>
    ''' Event to get transaction.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetSubscriptionStatusForAuthCode_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(getSubscriptionAuthCode.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions/SubscriptionAuthCode/" & getSubscriptionAuthCode.SelectedValue.ToString()
        Me.getSubscriptionStatus(resourcePathString, subscription_status_error, subscription_status_success)
    End Sub
    Protected Sub GetSubscriptionStatusForMerTranId_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(getSubscriptionMTID.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions/MerchantTransactionId/" & getSubscriptionMTID.SelectedValue.ToString()
        Me.getSubscriptionStatus(resourcePathString, subscription_status_error, subscription_status_success)
    End Sub
    Protected Sub GetSubscriptionStatusForTransactionId_Click(sender As Object, e As EventArgs)
        showSubscription = "true"
        If String.Compare(getSubscriptionTID.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions/SubscriptionId/" & getSubscriptionTID.SelectedValue.ToString()
        Me.getSubscriptionStatus(resourcePathString, subscription_status_error, subscription_status_success)
    End Sub
    ''' <summary>
    ''' Event to get transaction.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetTransactionStatusForAuthCode_Click(sender As Object, e As EventArgs)
        showTransaction = "true"
        If String.Compare(getTransactionAuthCode.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/TransactionAuthCode/" & getTransactionAuthCode.SelectedValue.ToString()
        Me.getTransactionStatus(resourcePathString, transaction_status_error, transaction_status_success)
    End Sub
    Protected Sub GetTransactionStatusForMerTranId_Click(sender As Object, e As EventArgs)
        showTransaction = "true"
        If String.Compare(getTransactionMTID.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/MerchantTransactionId/" & getTransactionMTID.SelectedValue.ToString()
        Me.getTransactionStatus(resourcePathString, transaction_status_error, transaction_status_success)
    End Sub
    Protected Sub GetTransactionStatusForTransactionId_Click(sender As Object, e As EventArgs)
        showTransaction = "true"
        If String.Compare(getTransactionTID.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim resourcePathString As String = String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/TransactionId/" & getTransactionTID.SelectedValue.ToString()
        Me.getTransactionStatus(resourcePathString, transaction_status_error, transaction_status_success)
    End Sub

    Protected Sub getTransactionStatus(resourcePathString As String, ByRef [error] As String, ByRef success As String)
        Try
            If Me.ReadAndGetAccessToken([error]) = True Then
                If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
                    Return
                End If

                Dim objRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(resourcePathString), HttpWebRequest)
                objRequest.Method = "GET"
                objRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                Dim getTransactionStatusResponseObject As HttpWebResponse = DirectCast(objRequest.GetResponse(), HttpWebResponse)
                Using getTransactionStatusResponseStream As New StreamReader(getTransactionStatusResponseObject.GetResponseStream())
                    Dim getTransactionStatusResponseData As String = getTransactionStatusResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim dict As Dictionary(Of String, Object) = deserializeJsonObject.Deserialize(Of Dictionary(Of String, Object))(getTransactionStatusResponseData)
                    DisplayDictionary(dict)
                    getTransactionStatusResponse = formattedResponse
                    success = "true"
                    If Me.CheckItemInFile(getTransactionStatusResponse("TransactionId"), Me.TransactionIdFile) = False Then
                        Me.WriteRecordToFile(getTransactionStatusResponse("TransactionId"), Me.TransactionIdFile)
                        updateListForTransactionIds()
                    End If
                    getTransactionStatusResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    [error] = New StreamReader(stream).ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            [error] = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' Reads from config file
    ''' </summary>
    ''' <returns>true/false; true if able to read else false</returns>
    Private Function ReadConfigFile() As Boolean

        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("MinTransactionAmount")) Then
            Me.MinTransactionAmount = "0.00"
        Else
            Me.MinTransactionAmount = ConfigurationManager.AppSettings("MinTransactionAmount")
        End If


        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("MaxTransactionAmount")) Then
            Me.MaxTransactionAmount = "2.99"
        Else
            Me.MaxTransactionAmount = ConfigurationManager.AppSettings("MaxTransactionAmount")
        End If

        Me.MinSubscriptionAmount = ConfigurationManager.AppSettings("MinSubscriptionAmount")
        If String.IsNullOrEmpty(Me.MinSubscriptionAmount) Then
            Me.MinSubscriptionAmount = "0.00"
        End If

        Me.MaxSubscriptionAmount = ConfigurationManager.AppSettings("MaxSubscriptionAmount")
        If String.IsNullOrEmpty(Me.MaxSubscriptionAmount) Then
            Me.MaxSubscriptionAmount = "3.99"
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            new_transaction_error = "api_key is not defined in configuration file"
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            new_transaction_error = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            new_transaction_error = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\PayApp1AccessToken.txt"
        End If

        Me.TransactionIdFile = ConfigurationManager.AppSettings("TransactionIdFile")
        If String.IsNullOrEmpty(Me.TransactionIdFile) Then
            Me.TransactionIdFile = "~\TransactionIdFile.txt"
        End If

        Me.MerchantTransactionIdFile = ConfigurationManager.AppSettings("MerchantTransactionIdFile")
        If String.IsNullOrEmpty(Me.MerchantTransactionIdFile) Then
            Me.MerchantTransactionIdFile = "~\MerchantTransactionIdFile.txt"
        End If

        Me.TransactionAuthorizationCodeFile = ConfigurationManager.AppSettings("TransactionAuthorizationCodeFile")
        If String.IsNullOrEmpty(Me.TransactionAuthorizationCodeFile) Then
            Me.TransactionAuthorizationCodeFile = "~\TransactionAuthorizationCodeFile.txt"
        End If

        Me.SubTransactionIdFile = ConfigurationManager.AppSettings("SubTransactionIdFile")
        If String.IsNullOrEmpty(Me.SubTransactionIdFile) Then
            Me.SubTransactionIdFile = "~\SubTransactionIdFile.txt"
        End If

        Me.SubMerchantTransactionIdFile = ConfigurationManager.AppSettings("SubMerchantTransactionIdFile")
        If String.IsNullOrEmpty(Me.SubMerchantTransactionIdFile) Then
            Me.SubMerchantTransactionIdFile = "~\SubMerchantTransactionIdFile.txt"
        End If

        Me.SubTransactionAuthorizationCodeFile = ConfigurationManager.AppSettings("SubTransactionAuthorizationCodeFile")
        If String.IsNullOrEmpty(Me.SubTransactionAuthorizationCodeFile) Then
            Me.SubTransactionAuthorizationCodeFile = "~\SubTransactionAuthorizationCodeFile.txt"
        End If

        Me.SubMerchantSubscriptionIdFile = ConfigurationManager.AppSettings("SubMerchantSubscriptionIdFile")
        If String.IsNullOrEmpty(Me.SubMerchantSubscriptionIdFile) Then
            Me.SubMerchantSubscriptionIdFile = "~\SubMerchantSubscriptionIdFile.txt"
        End If

        Me.ConsumerId = ConfigurationManager.AppSettings("ConsumerId")

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("recordsToDisplay")) Then
            Me.recordsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("recordsToDisplay"))
        Else
            Me.recordsToDisplay = 5
        End If

        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("noOfNotificationsToDisplay")) Then
            Me.noOfNotificationsToDisplay = 5
        Else
            noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("noOfNotificationsToDisplay"))
        End If

        Me.notificationDetailsFile = ConfigurationManager.AppSettings("notificationDetailsFile")
        If String.IsNullOrEmpty(Me.notificationDetailsFile) Then
            Me.notificationDetailsFile = "~\notificationDetailsFile.txt"
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "PAYMENT"
        End If

        If ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl") Is Nothing Then
            new_transaction_error = "MerchantPaymentRedirectUrl is not defined in configuration file"
            Return False


        End If

        Me.merchantRedirectURI = New Uri(ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl"))

        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            Me.refreshTokenExpiresIn = 24
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SourceLink")) Then
            SourceLink.HRef = ConfigurationManager.AppSettings("SourceLink")
        Else
            ' Default value
            SourceLink.HRef = "#"
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DownloadLink")) Then
            DownloadLink.HRef = ConfigurationManager.AppSettings("DownloadLink")
        Else
            ' Default value
            DownloadLink.HRef = "#"
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("HelpLink")) Then
            HelpLink.HRef = ConfigurationManager.AppSettings("HelpLink")
        Else
            ' Default value
            HelpLink.HRef = "#"
        End If

        Return True
    End Function

    Protected Sub createNewTransaction(sender As Object, e As EventArgs)
        Me.ReadTransactionParametersFromConfigurationFile()
        Dim payLoadString As String = "{""Amount"":" & Me.amount.ToString() & ",""Category"":" & Me.category.ToString() & ",""Channel"":""" & Me.channel.ToString() & """,""Description"":""" & Me.description.ToString() & """,""MerchantTransactionId"":""" & Me.merchantTransactionId.ToString() & """,""MerchantProductId"":""" & Me.merchantProductId.ToString() & """,""MerchantPaymentRedirectUrl"":""" & Me.merchantRedirectURI.ToString() & """}"
        SubmitToNotary(payLoadString, new_transaction_error, new_transaction_success)
        Session("vb_signedData") = payLoadString
        Session("vb_signedPayload") = signedPayload
        Session("vb_signedSignature") = signedSignature
        Response.Redirect(Me.endPoint & "/rest/3/Commerce/Payment/Transactions?clientid=" & Me.apiKey.ToString() & "&SignedPaymentDetail=" & Me.signedPayload.ToString() & "&Signature=" & Me.signedSignature.ToString())
    End Sub

    Protected Sub createNewSubscription(sender As Object, e As EventArgs)
        Me.ReadSubscriptionParametersFromConfigurationFile()
        Dim payLoadString As String = "{""Amount"":" & Me.amount & ",""Category"":" & Me.category & ",""Channel"":""" & Me.channel & """,""Description"":""" & Me.description & """,""MerchantTransactionId"":""" & Me.merchantTransactionId & """,""MerchantProductId"":""" & Me.merchantProductId & """,""MerchantPaymentRedirectUrl"":""" & Convert.ToString(Me.merchantRedirectURI) & """,""MerchantSubscriptionIdList"":""" & Me.merchantSubscriptionIdList & """,""IsPurchaseOnNoActiveSubscription"":" & Me.isPurchaseOnNoActiveSubscription & ",""SubscriptionRecurrences"":" & Me.subscriptionRecurringNumber & ",""SubscriptionPeriod"":""" & Me.subscriptionRecurringPeriod & """,""SubscriptionPeriodAmount"":" & Me.subscriptionRecurringPeriodAmount & "}"
        SubmitToNotary(payLoadString, new_subscription_error, new_subscription_success)
        Session("vb_signedData") = payLoadString
        Session("vb_signedPayload") = signedPayload
        Session("vb_signedSignature") = signedSignature
        Response.Redirect(Me.endPoint & "/rest/3/Commerce/Payment/Subscriptions?clientid=" & Me.apiKey.ToString() & "&SignedPaymentDetail=" & Me.signedPayload.ToString() & "&Signature=" & Me.signedSignature.ToString())
    End Sub

    Protected Sub Notary_Click(sender As Object, e As EventArgs)
        showNotary = "true"
        SubmitToNotary(payload.Value, notary_error, notary_success)
    End Sub

    Public Sub SubmitToNotary(sendingData As String, ByRef [error] As String, ByRef success As String)
        Try
            Dim newTransactionResponseData As [String]
            Dim notaryAddress As String
            notaryAddress = "" & Me.endPoint & "/Security/Notary/Rest/1/SignedPayload"
            Dim newTransactionRequestObject As WebRequest = DirectCast(System.Net.WebRequest.Create(notaryAddress), WebRequest)
            newTransactionRequestObject.Headers.Add("client_id", Me.apiKey.ToString())
            newTransactionRequestObject.Headers.Add("client_secret", Me.secretKey.ToString())
            newTransactionRequestObject.Method = "POST"
            newTransactionRequestObject.ContentType = "application/json"
            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(sendingData)
            newTransactionRequestObject.ContentLength = postBytes.Length

            Dim postStream As Stream = newTransactionRequestObject.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim newTransactionResponseObject As WebResponse = DirectCast(newTransactionRequestObject.GetResponse(), HttpWebResponse)
            Using newTransactionResponseStream As New StreamReader(newTransactionResponseObject.GetResponseStream())
                newTransactionResponseData = newTransactionResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As NotaryResponse = DirectCast(deserializeJsonObject.Deserialize(newTransactionResponseData, GetType(NotaryResponse)), NotaryResponse)
                signedPayload = deserializedJsonObj.SignedDocument
                signedSignature = deserializedJsonObj.Signature
                success = "Success"
                payload.Value = sendingData
                newTransactionResponseStream.Close()
            End Using
        Catch ex As Exception
            [error] = ex.Message
        End Try
    End Sub
    ''' <summary>
    ''' Method to read transaction parameters from configuration file.
    ''' </summary>
    Private Sub ReadTransactionParametersFromConfigurationFile()
        Me.transactionTime = DateTime.UtcNow
        Me.transactionTimeString = String.Format("{0:dddMMMddyyyyHHmmss}", Me.transactionTime)
        If ConfigurationManager.AppSettings("Category") Is Nothing Then
            new_transaction_error = "Category is not defined in configuration file"
            Return
        End If

        Me.category = Convert.ToInt32(ConfigurationManager.AppSettings("Category"))
        If ConfigurationManager.AppSettings("Channel") Is Nothing Then
            Me.channel = "MOBILE_WEB"
        Else
            Me.channel = ConfigurationManager.AppSettings("Channel")
        End If

        Me.description = "TrDesc" & Me.transactionTimeString
        Me.merchantTransactionId = "V" & Me.transactionTimeString
        Session("vb_merTranId") = Me.merchantTransactionId.ToString()
        Me.merchantProductId = "ProdId" & Me.transactionTimeString
        Me.merchantApplicationId = "MerAppId" & Me.transactionTimeString
        If product_1.Checked Then
            Me.amount = Me.MinTransactionAmount
            Session("vb_tranType") = "product_1"
        ElseIf product_2.Checked Then
            Session("vb_tranType") = "product_2"
            Me.amount = Me.MaxTransactionAmount
        End If
    End Sub

    Private Sub ReadSubscriptionParametersFromConfigurationFile()
        Me.transactionTime = DateTime.UtcNow
        Me.transactionTimeString = [String].Format("{0:dddMMMddyyyyHHmmss}", Me.transactionTime)

        If ConfigurationManager.AppSettings("Category") Is Nothing Then
            new_subscription_error = "Category is not defined in configuration file"
            Return
        End If

        Me.category = Convert.ToInt32(ConfigurationManager.AppSettings("Category"))
        Me.channel = ConfigurationManager.AppSettings("Channel")
        If String.IsNullOrEmpty(Me.channel) Then
            Me.channel = "MOBILE_WEB"
        End If

        Me.description = "TrDesc" & Me.transactionTimeString
        Me.merchantTransactionId = "V" & Me.transactionTimeString
        Session("vb_sub_merTranId") = Me.merchantTransactionId
        Me.merchantProductId = "ProdId" & Me.transactionTimeString
        Me.merchantApplicationId = "MerAppId" & Me.transactionTimeString
        Me.merchantSubscriptionIdList = "VML" & New Random().[Next]()
        Session("vb_MerchantSubscriptionIdList") = Me.merchantSubscriptionIdList

        Me.isPurchaseOnNoActiveSubscription = ConfigurationManager.AppSettings("IsPurchaseOnNoActiveSubscription")

        If String.IsNullOrEmpty(Me.isPurchaseOnNoActiveSubscription) Then
            Me.isPurchaseOnNoActiveSubscription = "false"
        End If

        Me.subscriptionRecurringNumber = ConfigurationManager.AppSettings("SubscriptionRecurringNumber")
        If String.IsNullOrEmpty(Me.subscriptionRecurringNumber) Then
            Me.subscriptionRecurringNumber = "99999"
        End If

        Me.subscriptionRecurringPeriod = ConfigurationManager.AppSettings("SubscriptionRecurringPeriod")
        If String.IsNullOrEmpty(Me.subscriptionRecurringPeriod) Then
            Me.subscriptionRecurringPeriod = "MONTHLY"
        End If

        Me.subscriptionRecurringPeriodAmount = ConfigurationManager.AppSettings("SubscriptionRecurringPeriodAmount")
        If String.IsNullOrEmpty(Me.subscriptionRecurringPeriodAmount) Then
            Me.subscriptionRecurringPeriodAmount = "1"
        End If

        If subproduct_1.Checked Then
            Me.amount = Me.MinSubscriptionAmount
            Session("vb_subType") = "subproduct_1"
        ElseIf subproduct_2.Checked Then
            Session("vb_subType") = "subproduct_2"
            Me.amount = Me.MaxSubscriptionAmount
        End If
    End Sub

    ''' <summary>
    ''' This function reads the Access Token File and stores the values of access token, expiry seconds
    ''' refresh token, last access token time and refresh token expiry time
    ''' This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <returns>Returns boolean</returns>    
    Private Function ReadAccessTokenFile(ByRef message As String) As Boolean
        Dim fileStream As FileStream = Nothing
        Dim streamReader As StreamReader = Nothing
        Try
            fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read)
            streamReader = New StreamReader(fileStream)
            Me.accessToken = streamReader.ReadLine()
            Me.accessTokenExpiryTime = streamReader.ReadLine()
            Me.refreshToken = streamReader.ReadLine()
            Me.refreshTokenExpiryTime = streamReader.ReadLine()
        Catch ex As Exception
            message = ex.Message
            Return False
        Finally
            If streamReader IsNot Nothing Then
                streamReader.Close()
            End If

            If fileStream IsNot Nothing Then
                fileStream.Close()
            End If
        End Try

        If (Me.accessToken Is Nothing) OrElse (Me.accessTokenExpiryTime Is Nothing) OrElse (Me.refreshToken Is Nothing) OrElse (Me.refreshTokenExpiryTime Is Nothing) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' This function validates the expiry of the access token and refresh token,
    ''' function compares the current time with the refresh token taken time, if current time is greater then 
    ''' returns INVALID_REFRESH_TOKEN
    ''' function compares the difference of last access token taken time and the current time with the expiry seconds, if its more,
    ''' funciton returns INVALID_ACCESS_TOKEN
    ''' otherwise returns VALID_ACCESS_TOKEN
    ''' </summary>
    ''' <returns>Return String</returns>
    Private Function IsTokenValid() As String
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()
            If currentServerTime >= DateTime.Parse(Me.accessTokenExpiryTime) Then
                If currentServerTime >= DateTime.Parse(Me.refreshTokenExpiryTime) Then
                    Return "INVALID_ACCESS_TOKEN"
                Else
                    Return "REFRESH_TOKEN"
                End If
            Else
                Return "VALID_ACCESS_TOKEN"
            End If
        Catch
            Return "INVALID_ACCESS_TOKEN"
        End Try
    End Function

    ''' <summary>
    ''' This function is used to read access token file and validate the access token
    ''' this function returns true if access token is valid, or else false is returned
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <returns>Returns Boolean</returns>
    Private Function ReadAndGetAccessToken(ByRef responseString As String) As Boolean
        Dim result As Boolean = True
        If Me.ReadAccessTokenFile(responseString) = False Then
            result = Me.GetAccessToken(AccessType.ClientCredential, responseString)
        Else
            Dim tokenValidity As String = Me.IsTokenValid()
            If tokenValidity = "REFRESH_TOKEN" Then
                result = Me.GetAccessToken(AccessType.RefreshToken, responseString)
            ElseIf String.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") = 0 Then
                result = Me.GetAccessToken(AccessType.ClientCredential, responseString)
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return False
        Else
            Return result
        End If
    End Function

    ''' <summary>
    ''' Method to read the entries from file and update list.
    ''' </summary>
    Public Sub GetListFromFile(filename As String, ByRef list As List(Of String))
        Try

            Dim file As New FileStream(Request.MapPath(filename), FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file)
            Dim line As String

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                list.Add(line)
            End While

            sr.Close()
            file.Close()
            list.Reverse(0, list.Count)
        Catch ex As Exception
            Return
        End Try
    End Sub

    ''' <summary>
    ''' Method to update refund list to file.
    ''' </summary>
    Public Sub UpdateListToFile(filename As String, ByRef list As List(Of String))
        Try
            If list.Count <> 0 Then
                list.Reverse(0, list.Count)
            End If

            Using sr As StreamWriter = File.CreateText(Request.MapPath(filename))
                Dim tempCount As Integer = 0
                While tempCount < list.Count
                    Dim lineToWrite As String = list(tempCount)
                    sr.WriteLine(lineToWrite)
                    tempCount += 1
                End While
                sr.Close()
            End Using
        Catch ex As Exception
            Return
        End Try
    End Sub

    ''' <summary>
    ''' Event for refund transaction
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub RefundTransaction_Click(sender As Object, e As EventArgs)
        showTransaction = "true"
        If String.Compare(refundTransactionId.SelectedValue.ToString(), "Select") = 0 Then
            Return
        End If
        Dim transactionToRefund As String = refundTransactionId.SelectedValue.ToString()
        Dim strReq As String = "{""TransactionOperationStatus"":""Refunded"",""RefundReasonCode"":1,""RefundReasonText"":""Customer was not happy""}"
        ' string strReq = "{\"RefundReasonCode\":1,\"RefundReasonText\":\"Customer was not happy\"}";
        Dim dataLength As String = String.Empty
        Try
            If Me.ReadAndGetAccessToken(refund_error) = True Then
                If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
                    Return
                End If
                Dim objRequest As WebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/3/Commerce/Payment/Transactions/" & transactionToRefund.ToString()), WebRequest)
                objRequest.Method = "PUT"
                objRequest.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                objRequest.ContentType = "application/json"
                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(strReq)
                objRequest.ContentLength = postBytes.Length
                Dim postStream As Stream = objRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)
                dataLength = postBytes.Length.ToString()
                postStream.Close()
                Dim refundTransactionResponeObject As WebResponse = DirectCast(objRequest.GetResponse(), WebResponse)
                Using refundResponseStream As New StreamReader(refundTransactionResponeObject.GetResponseStream())
                    Dim refundTransactionResponseData As String = refundResponseStream.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim dict As Dictionary(Of String, Object) = deserializeJsonObject.Deserialize(Of Dictionary(Of String, Object))(refundTransactionResponseData)
                    DisplayDictionary(dict)
                    refundResponse = formattedResponse
                    refund_success = "true"
                    refundResponseStream.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    refund_error = New StreamReader(stream).ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            '''/ + strReq + transactionToRefund.ToString() + dataLength
            refund_error = ex.ToString()
        End Try
    End Sub

    Public Sub updateListForTransactionIds()
        getTransactionTID.Items.Clear()
        refundTransactionId.Items.Clear()
        ResetList(transactionIds)
        GetListFromFile(TransactionIdFile, transactionIds)
        getTransactionTID.Items.Add("Select")
        refundTransactionId.Items.Add("Select")
        For Each id As String In transactionIds
            refundTransactionId.Items.Add(id)
            getTransactionTID.Items.Add(id)
        Next
    End Sub

    Public Sub updateListsForAuthCode()
        getTransactionAuthCode.Items.Clear()
        ResetList(transactionAuthCodes)
        GetListFromFile(TransactionAuthorizationCodeFile, transactionAuthCodes)
        getTransactionAuthCode.Items.Add("Select")
        For Each id As String In transactionAuthCodes
            getTransactionAuthCode.Items.Add(id)
        Next
    End Sub

    Public Sub updateListsForMerchantTransactionId()
        getTransactionMTID.Items.Clear()
        ResetList(MerTransactionIds)
        GetListFromFile(MerchantTransactionIdFile, MerTransactionIds)
        getTransactionMTID.Items.Add("Select")
        For Each id As String In MerTransactionIds
            getTransactionMTID.Items.Add(id)
        Next
    End Sub

    Public Sub updateListForSubMerchantSubscriptionIds()
        getSDetailsMSID.Items.Clear()
        ResetList(SubMerchantSubscriptionIds)
        GetListFromFile(SubMerchantSubscriptionIdFile, SubMerchantSubscriptionIds)
        getSDetailsMSID.Items.Add("Select")
        For Each id As String In SubMerchantSubscriptionIds
            getSDetailsMSID.Items.Add(id)
        Next
    End Sub

    Public Sub updateListForSubTransactionIds()
        refundSubscriptionId.Items.Clear()
        cancelSubscriptionId.Items.Clear()
        getSubscriptionTID.Items.Clear()
        ResetList(SubtransactionIds)
        GetListFromFile(SubTransactionIdFile, SubtransactionIds)
        getSubscriptionTID.Items.Add("Select")
        refundSubscriptionId.Items.Add("Select")
        cancelSubscriptionId.Items.Add("Select")
        For Each id As String In SubtransactionIds
            refundSubscriptionId.Items.Add(id)
            cancelSubscriptionId.Items.Add(id)
            getSubscriptionTID.Items.Add(id)
        Next


    End Sub

    Public Sub updateListsForSubAuthCode()
        getSubscriptionAuthCode.Items.Clear()
        ResetList(SubtransactionAuthCodes)
        GetListFromFile(SubTransactionAuthorizationCodeFile, SubtransactionAuthCodes)
        getSubscriptionAuthCode.Items.Add("Select")
        For Each id As String In SubtransactionAuthCodes
            getSubscriptionAuthCode.Items.Add(id)
        Next
    End Sub

    Public Sub updateListsForSubMerchantTransactionId()
        getSubscriptionMTID.Items.Clear()
        ResetList(SubMerTransactionIds)
        GetListFromFile(SubMerchantTransactionIdFile, SubMerTransactionIds)
        getSubscriptionMTID.Items.Add("Select")
        For Each id As String In SubMerTransactionIds
            getSubscriptionMTID.Items.Add(id)
        Next
    End Sub

    ''' <summary>
    ''' Method to reset refund list
    ''' </summary>
    Public Sub ResetList(ByRef list As List(Of String))
        If list.Count > 0 Then
            list.RemoveRange(0, list.Count)
        End If
    End Sub

    ''' <summary>
    ''' Method to check item in file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    ''' <returns>Return Boolean</returns>
    Public Function CheckItemInFile(valueToSearch As String, filename As String) As Boolean
        Try
            Dim line As String
            Dim file As New System.IO.StreamReader(Request.MapPath(filename))
            While (InlineAssignHelper(line, file.ReadLine())) IsNot Nothing
                If line.CompareTo(valueToSearch) = 0 Then
                    file.Close()
                    Return True
                End If
            End While
            file.Close()
            Return False
        Catch ex As Exception
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Method to update file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    Public Sub WriteRecordToFile(value As String, filename As String)
        Try

            Dim list As New List(Of String)()
            Dim file__1 As New FileStream(Request.MapPath(filename), FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file__1)
            Dim line As String

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                list.Add(line)
            End While

            sr.Close()
            file__1.Close()

            If list.Count > Me.recordsToDisplay Then
                Dim diff As Integer = list.Count - Me.recordsToDisplay
                list.RemoveRange(0, diff)
            End If

            If list.Count = Me.recordsToDisplay Then
                list.RemoveAt(0)
            End If
            list.Add(value)
            Using sw As StreamWriter = File.CreateText(Request.MapPath(filename))
                Dim tempCount As Integer = 0
                While tempCount < list.Count
                    Dim lineToWrite As String = list(tempCount)
                    sw.WriteLine(lineToWrite)
                    tempCount += 1
                End While
                sw.Close()
            End Using
        Catch ex As Exception
            Return
        End Try
    End Sub

    Protected Sub ShowNotifications_Click(sender As Object, e As EventArgs)
        showNotification = "true"
        GetNotificationsFromFile()
        
    End Sub

    Private Sub GetNotificationsFromFile()
        Dim notifications As New List(Of String)()
        Me.GetListFromFile(Me.notificationDetailsFile, notifications)

        Me.notificationDetails.Clear()
        Dim notificationPair As Dictionary(Of String, String) = Nothing
        Dim count As Integer = 1
        For Each notification As String In notifications
            If count > Me.recordsToDisplay Then
                Exit For
            End If

            Dim kvps As String() = notification.Split("$"c)
            notificationPair = New Dictionary(Of String, String)()
            For Each kvp As String In kvps
                Dim values As String() = kvp.Split("%"c)
                If values IsNot Nothing Then
                    If values.Length > 1 Then
                        notificationPair.Add(values(0), values(1))
                    End If
                End If
            Next

            notificationDetails.Add(notificationPair)
            count += 1
        Next
    End Sub

    ''' <summary>
    ''' This function get the access token based on the type parameter type values.
    ''' If type value is 1, access token is fetch for client credential flow
    ''' If type value is 2, access token is fetch for client credential flow based on the exisiting refresh token
    ''' </summary>
    ''' <param name="type">Type as integer</param>
    ''' <param name="panelParam">Panel details</param>
    ''' <returns>Return boolean</returns>
    Private Function GetAccessToken(type As AccessType, ByRef message As String) As Boolean
        Dim fileStream As FileStream = Nothing
        Dim postStream As Stream = Nothing
        Dim streamWriter As StreamWriter = Nothing

        ' This is client credential flow
        If type = AccessType.ClientCredential Then
            Try
                Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
                accessTokenRequest.Method = "POST"
                Dim oauthParameters As String = String.Empty
                If type = AccessType.ClientCredential Then
                    oauthParameters = "client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&grant_type=client_credentials&scope=" & Me.scope
                Else
                    oauthParameters = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
                End If

                accessTokenRequest.ContentType = "application/x-www-form-urlencoded"

                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
                accessTokenRequest.ContentLength = postBytes.Length

                postStream = accessTokenRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)

                Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
                Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                    Dim jsonAccessToken As String = accessTokenResponseStream.ReadToEnd().ToString()
                    Dim deserializeJsonObject As New JavaScriptSerializer()

                    Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(jsonAccessToken, GetType(AccessTokenResponse)), AccessTokenResponse)

                    Me.accessToken = deserializedJsonObj.access_token
                    Me.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString()
                    Me.refreshToken = deserializedJsonObj.refresh_token

                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(Me.refreshTokenExpiresIn)

                    If deserializedJsonObj.expires_in.Equals("0") Then
                        Dim defaultAccessTokenExpiresIn As Integer = 100
                        ' In Yearsint yearsToAdd = 100;
                        Me.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongDateString() & " " & currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongTimeString()
                    End If

                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()

                    fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
                    streamWriter = New StreamWriter(fileStream)
                    streamWriter.WriteLine(Me.accessToken)
                    streamWriter.WriteLine(Me.accessTokenExpiryTime)
                    streamWriter.WriteLine(Me.refreshToken)
                    streamWriter.WriteLine(Me.refreshTokenExpiryTime)

                    ' Close and clean up the StreamReader
                    accessTokenResponseStream.Close()
                    Return True
                End Using
            Catch we As WebException
                Dim errorResponse As String = String.Empty

                Try
                    Using sr2 As New StreamReader(we.Response.GetResponseStream())
                        errorResponse = sr2.ReadToEnd()
                        sr2.Close()
                    End Using
                Catch
                    errorResponse = "Unable to get response"
                End Try

                message = errorResponse & Environment.NewLine & we.ToString()
            Catch ex As Exception
                message = ex.Message
                Return False
            Finally
                If postStream IsNot Nothing Then
                    postStream.Close()
                End If

                If streamWriter IsNot Nothing Then
                    streamWriter.Close()
                End If

                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        ElseIf type = AccessType.RefreshToken Then
            Try
                Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
                accessTokenRequest.Method = "POST"

                Dim oauthParameters As String = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
                accessTokenRequest.ContentType = "application/x-www-form-urlencoded"

                Dim encoding As New UTF8Encoding()
                Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
                accessTokenRequest.ContentLength = postBytes.Length

                postStream = accessTokenRequest.GetRequestStream()
                postStream.Write(postBytes, 0, postBytes.Length)

                Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
                Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                    Dim accessTokenJSon As String = accessTokenResponseStream.ReadToEnd().ToString()
                    Dim deserializeJsonObject As New JavaScriptSerializer()

                    Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(accessTokenJSon, GetType(AccessTokenResponse)), AccessTokenResponse)
                    Me.accessToken = deserializedJsonObj.access_token.ToString()
                    Dim accessTokenExpiryTime As DateTime = currentServerTime.AddMilliseconds(Convert.ToDouble(deserializedJsonObj.expires_in.ToString()))
                    Me.refreshToken = deserializedJsonObj.refresh_token.ToString()

                    fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write)
                    streamWriter = New StreamWriter(fileStream)
                    streamWriter.WriteLine(Me.accessToken)
                    streamWriter.WriteLine(Me.accessTokenExpiryTime)
                    streamWriter.WriteLine(Me.refreshToken)

                    ' Refresh token valids for 24 hours
                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(24)
                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()
                    streamWriter.WriteLine(refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString())

                    accessTokenResponseStream.Close()
                    Return True
                End Using
            Catch we As WebException
                Dim errorResponse As String = String.Empty

                Try
                    Using sr2 As New StreamReader(we.Response.GetResponseStream())
                        errorResponse = sr2.ReadToEnd()
                        sr2.Close()
                    End Using
                Catch
                    errorResponse = "Unable to get response"
                End Try

                message = errorResponse & Environment.NewLine & we.ToString()
            Catch ex As Exception
                message = ex.Message
                Return False
            Finally
                If postStream IsNot Nothing Then
                    postStream.Close()
                End If

                If streamWriter IsNot Nothing Then
                    streamWriter.Close()
                End If

                If fileStream IsNot Nothing Then
                    fileStream.Close()
                End If
            End Try
        End If

        Return False
    End Function

    Public Class NotaryResponse
        Public Property SignedDocument() As String
            Get
                Return m_SignedDocument
            End Get
            Set(value As String)
                m_SignedDocument = value
            End Set
        End Property
        Private m_SignedDocument As String
        Public Property Signature() As String
            Get
                Return m_Signature
            End Get
            Set(value As String)
                m_Signature = value
            End Set
        End Property
        Private m_Signature As String
    End Class
    ''' <summary>
    ''' Access Token Types
    ''' </summary>
    Private Enum AccessType
        ''' <summary>
        ''' Access Token Type is based on Client Credential Mode
        ''' </summary>
        ClientCredential

        ''' <summary>
        ''' Access Token Type is based on Refresh Token
        ''' </summary>
        RefreshToken
    End Enum

    ''' <summary>
    ''' Class to hold access token response
    ''' </summary>
    Public Class AccessTokenResponse
        ''' <summary>
        ''' Gets or sets access token
        ''' </summary>
        Public Property access_token() As String
            Get
                Return m_access_token
            End Get
            Set(value As String)
                m_access_token = value
            End Set
        End Property
        Private m_access_token As String

        ''' <summary>
        ''' Gets or sets refresh token
        ''' </summary>
        Public Property refresh_token() As String
            Get
                Return m_refresh_token
            End Get
            Set(value As String)
                m_refresh_token = value
            End Set
        End Property
        Private m_refresh_token As String

        ''' <summary>
        ''' Gets or sets expires in
        ''' </summary>
        Public Property expires_in() As String
            Get
                Return m_expires_in
            End Get
            Set(value As String)
                m_expires_in = value
            End Set
        End Property
        Private m_expires_in As String
    End Class
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class