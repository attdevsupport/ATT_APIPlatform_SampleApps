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
Imports System.Globalization
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.Notaryv1

#End Region

''' <summary>
''' Notary App1 class
''' This application demonstartes the usage of Notary API given by MS SDK library.
''' This application takes payload as argument and returns signed document and signature
''' </summary>
Partial Public Class Notary_App1
    Inherits System.Web.UI.Page
#Region "Instance Variables"

    ''' <summary>
    ''' Gets or sets the value of amount
    ''' </summary>
    Private amount As Double

    ''' <summary>
    ''' Gets or sets the value of category
    ''' </summary>
    Private category As Integer

    ''' <summary>
    ''' Table to draw messages in the page
    ''' </summary>
    Private failureTable As Table

    ''' <summary>
    ''' Transaction parameters
    ''' </summary>
    Private channel As String, description As String, merchantTransactionId As String, merchantProductId As String, merchantApplicationId As String

    ''' <summary>
    ''' Gets or sets the value of merchant Redirect Uri
    ''' </summary>
    Private merchantRedirectURI As Uri

    ''' <summary>
    ''' Instance variables for payment parameters
    ''' </summary>
    Private paymentType As String, goBackURL As String, merchantSubscriptionIdList As String, subscriptionRecurringPeriod As String, isPurchaseOnNoActiveSubscription As String, transactionTimeString As String, _
     payLoadStringFromRequest As String

    ''' <summary>
    ''' Payment parameters
    ''' </summary>
    Private subscriptionRecurringNumber As Integer, subscriptionRecurringPeriodAmount As Integer

    ''' <summary>
    ''' Transaction Date Time
    ''' </summary>
    Private transactionTime As DateTime

    ''' <summary>
    ''' RequestFactory Instance
    ''' </summary>
    Private requestFactory As RequestFactory

    ''' <summary>
    ''' Application parameters
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

#End Region

#Region "Notary Application Events"

    ''' <summary>
    ''' This method will be called during loading the page
    ''' </summary>
    ''' <param name="sender">Sender object</param>
    ''' <param name="e">event arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblServerTime.Text = DateTime.UtcNow.ToString("ddd MMM dd yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) & " UTC"

        If (Request("signed_payload") IsNot Nothing) AndAlso (Request("signed_signature") IsNot Nothing) AndAlso (Request("goBackURL") IsNot Nothing) AndAlso (Request("signed_request") IsNot Nothing) Then
            signPayLoadButton.Text = "Back"
            requestText.Text = Request("signed_request")
            SignedPayLoadTextBox.Text = Request("signed_payload")
            SignatureTextBox.Text = Request("signed_signature")
            Me.goBackURL = Request("goBackURL")
        ElseIf (Request("request_to_sign") IsNot Nothing) AndAlso (Request("goBackURL") IsNot Nothing) AndAlso (Request("api_key") IsNot Nothing) AndAlso (Request("secret_key") IsNot Nothing) Then
            Me.payLoadStringFromRequest = Request("request_to_sign")
            Me.goBackURL = Request("goBackURL")
            SignedPayLoadTextBox.Text = Me.payLoadStringFromRequest
            Me.apiKey = Request("api_key")
            Me.secretKey = Request("secret_key")
            Me.requestFactory = Nothing
            Me.endPoint = ConfigurationManager.AppSettings("endpoint")
            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(requestFactory.ScopeTypes.Payment)
            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
            Me.ExecuteSignedPayloadFromRequest()
        Else
            If ConfigurationManager.AppSettings("paymentType") Is Nothing Then
                Me.DrawPanelForFailure(notaryPanel, "paymentType is not defined in configuration file")
                Return
            End If

            Me.paymentType = ConfigurationManager.AppSettings("paymentType")
            If Me.paymentType.Equals("Transaction", StringComparison.OrdinalIgnoreCase) Then
                Me.ReadTransactionParametersFromConfigurationFile()
            ElseIf Me.paymentType.Equals("Subscription", StringComparison.OrdinalIgnoreCase) Then
                If Not Page.IsPostBack Then
                    Me.ReadTransactionParametersFromConfigurationFile()
                    Me.ReadSubscriptionParametersFromConfigurationFile()
                End If
            Else
                Me.DrawPanelForFailure(notaryPanel, "paymentType is  defined with invalid value in configuration file.  Valid values are Transaction or Subscription.")
                Return
            End If

            If Not Page.IsPostBack Then
                Dim payLoadString As String = "{'Amount':'" & Me.amount.ToString() & "','Category':'" & Me.category.ToString() & "','Channel':'" & Me.channel & "','Description':'" & Me.description & "','MerchantTransactionId':'" & Me.merchantTransactionId & "','MerchantProductId':'" & Me.merchantProductId & "','MerchantApplicaitonId':'" & Me.merchantApplicationId & "','MerchantPaymentRedirectUrl':'" & Me.merchantRedirectURI.ToString() & "','MerchantSubscriptionIdList':'" & Me.merchantSubscriptionIdList & "','IsPurchaseOnNoActiveSubscription':'" & Me.isPurchaseOnNoActiveSubscription & "','SubscriptionRecurringNumber':'" & Me.subscriptionRecurringNumber.ToString() & "','SubscriptionRecurringPeriod':'" & Me.subscriptionRecurringPeriod & "','SubscriptionRecurringPeriodAmount':'" & Me.subscriptionRecurringPeriodAmount.ToString()

                requestText.Text = payLoadString
            End If
        End If
    End Sub

    ''' <summary>
    ''' This method will be called when the user clicks on SignPayload button.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">Event Arguments</param>
    Protected Sub SignPayLoadButton_Click(sender As Object, e As EventArgs)
        If signPayLoadButton.Text.Equals("Back", StringComparison.CurrentCultureIgnoreCase) Then
            Try
                Response.Redirect(Me.goBackURL & "?shown_notary=true")
            Catch ex As Exception
                Me.DrawPanelForFailure(notaryPanel, ex.Message)
            End Try
        Else
            Me.ExecuteSignedPayload()
        End If
    End Sub

#End Region

#Region "Notary Application specific methods"

    ''' <summary>
    ''' Reads the values from config file and initializes the instance of RequestFactory
    ''' </summary>
    ''' <returns>true/false; true if able to initialize successfully, else false</returns>
    Private Function Initialize() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(notaryPanel, "api_key is not defined in the config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(notaryPanel, "secret_key is not defined in the config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(notaryPanel, "endpoint is not defined in the config file")
            Return False
        End If

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.Payment)
        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)

        Return True
    End Function

    ''' <summary>
    ''' Read Transaction Parameters From ConfigurationFile is used to read all transaction related values from web.config file
    ''' </summary>
    Private Sub ReadTransactionParametersFromConfigurationFile()
        Me.transactionTime = DateTime.UtcNow
        Me.transactionTimeString = [String].Format("{0:ddd-MMM-dd-yyyy-HH-mm-ss}", Me.transactionTime)
        If ConfigurationManager.AppSettings("Amount") Is Nothing Then
            Me.DrawPanelForFailure(notaryPanel, "Amount is not defined in configuration file")
            Return
        End If

        Me.amount = Convert.ToDouble(ConfigurationManager.AppSettings("Amount"))
        requestText.Text = "Amount: " & Me.amount & vbCr & vbLf
        If ConfigurationManager.AppSettings("Category") Is Nothing Then
            Me.DrawPanelForFailure(notaryPanel, "Category is not defined in configuration file")
            Return
        End If

        Me.category = Convert.ToInt32(ConfigurationManager.AppSettings("Category"))
        requestText.Text = requestText.Text + "Category: " & Me.category & vbCr & vbLf

        Me.channel = ConfigurationManager.AppSettings("Channel")
        If String.IsNullOrEmpty(Me.channel) Then
            Me.channel = "MOBILE_WEB"
        End If

        Me.description = "TrDesc" & Me.transactionTimeString
        requestText.Text = requestText.Text + "Description: " & Me.description & vbCr & vbLf
        Me.merchantTransactionId = "TrId" & Me.transactionTimeString
        requestText.Text = requestText.Text + "MerchantTransactionId: " & Me.merchantTransactionId & vbCr & vbLf
        Me.merchantProductId = "ProdId" & Me.transactionTimeString
        requestText.Text = requestText.Text + "MerchantProductId: " & Me.merchantProductId & vbCr & vbLf
        Me.merchantApplicationId = "MerAppId" & Me.transactionTimeString
        requestText.Text = requestText.Text + "MerchantApplicationId: " & Me.merchantApplicationId & vbCr & vbLf
        If ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl") Is Nothing Then
            Me.DrawPanelForFailure(notaryPanel, "MerchantPaymentRedirectUrl is not defined in configuration file")
            Return
        End If

        Me.merchantRedirectURI = New Uri(ConfigurationManager.AppSettings("MerchantPaymentRedirectUrl"))

        requestText.Text = requestText.Text + "MerchantPaymentRedirectUrl: " & Convert.ToString(Me.merchantRedirectURI)
    End Sub

    ''' <summary>
    ''' Read subscription Parameters From ConfigurationFile is used to read all subscription related values from web.config file
    ''' </summary>
    Private Sub ReadSubscriptionParametersFromConfigurationFile()
        Me.merchantSubscriptionIdList = ConfigurationManager.AppSettings("MerchantSubscriptionIdList")
        If String.IsNullOrEmpty(Me.merchantSubscriptionIdList) Then
            Me.merchantSubscriptionIdList = "merSubIdList" & Me.transactionTimeString
        End If

        requestText.Text = requestText.Text + vbCr & vbLf & "MerchantSubscriptionIdList: " & Me.merchantSubscriptionIdList & vbCr & vbLf
        Me.subscriptionRecurringPeriod = ConfigurationManager.AppSettings("SubscriptionRecurringPeriod")
        If String.IsNullOrEmpty(Me.subscriptionRecurringPeriod) Then
            Me.subscriptionRecurringPeriod = "MONTHLY"
        End If

        requestText.Text = requestText.Text + "SubscriptionRecurringPeriod: " & Me.subscriptionRecurringPeriod & vbCr & vbLf

        Me.subscriptionRecurringNumber = 9999
        If ConfigurationManager.AppSettings("SubscriptionRecurringNumber") IsNot Nothing Then
            Me.subscriptionRecurringNumber = Convert.ToInt32(ConfigurationManager.AppSettings("SubscriptionRecurringNumber"))
        End If

        requestText.Text = requestText.Text + "SubscriptionRecurringNumber: " & Me.subscriptionRecurringNumber & vbCr & vbLf

        Me.subscriptionRecurringPeriodAmount = 1
        If ConfigurationManager.AppSettings("SubscriptionRecurringPeriodAmount") IsNot Nothing Then
            Me.subscriptionRecurringPeriodAmount = Convert.ToInt32(ConfigurationManager.AppSettings("SubscriptionRecurringPeriodAmount"))
        End If

        requestText.Text = requestText.Text + "SubscriptionRecurringPeriodAmount: " & Me.subscriptionRecurringPeriodAmount & vbCr & vbLf

        Me.isPurchaseOnNoActiveSubscription = ConfigurationManager.AppSettings("IsPurchaseOnNoActiveSubscription")
        If String.IsNullOrEmpty(Me.isPurchaseOnNoActiveSubscription) Then
            Me.isPurchaseOnNoActiveSubscription = "false"
        End If

        requestText.Text = requestText.Text + "IsPurchaseOnNoActiveSubscription: " & Me.isPurchaseOnNoActiveSubscription
    End Sub

    ''' <summary>
    ''' This method invokes GetNotaryResponse() method of RequestFactory by passing payload string.
    ''' and displays the notarized payload in the text box.
    ''' </summary>
    Private Sub ExecuteSignedPayload()
        Try
            Me.Initialize()
            Dim payLoadString As String = requestText.Text
            Dim notaryResponse As NotaryResponse = Me.requestFactory.GetNotaryResponse(payLoadString)
            If notaryResponse IsNot Nothing Then
                SignedPayLoadTextBox.Text = notaryResponse.SignedDocument
                SignatureTextBox.Text = notaryResponse.Signature
            End If
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(notaryPanel, ae.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(notaryPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(notaryPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' This method get called on new transaction or new subscription payload notarization.
    ''' </summary>
    Private Sub ExecuteSignedPayloadFromRequest()
        Try
            Dim payLoadString As String = requestText.Text
            Dim notaryResponse As NotaryResponse = Me.requestFactory.GetNotaryResponse(payLoadString)
            If notaryResponse IsNot Nothing Then
                Response.Redirect(((Me.goBackURL & "?ret_signed_payload=") + notaryResponse.SignedDocument & "&ret_signature=") + notaryResponse.Signature)
            End If
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(notaryPanel, ae.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(notaryPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(notaryPanel, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' This method displays error information in the panel.
    ''' </summary>
    ''' <param name="panelParam">Panel, where the message needs to be displayed</param>
    ''' <param name="message">string, the message to be displayed</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        Me.failureTable = New Table()
        Me.failureTable.Font.Name = "Sans-serif"
        Me.failureTable.Font.Size = 9
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

#End Region
End Class