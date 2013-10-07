' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Web.UI
#End Region

''' <summary>
''' Default class
''' </summary>
Partial Public Class SMS_App1
    Inherits System.Web.UI.Page
#Region "Variable Declaration"

    ''' <summary>
    ''' Global Variables related to application
    ''' </summary>
    Private endPoint As String, apiKey As String, secretKey As String, scope As String

    '''<summary>
    '''API URL's
    '''</summary>
    Private sendSMSURL As String = "/sms/v3/messaging/outbox"
    Private getDeliveryStatusURL As String = "/sms/v3/messaging/outbox/"
    Private getSMSURL As String = "/sms/v3/messaging/inbox"

    ''' <summary>
    ''' Global Variables related to access token
    ''' </summary>
    Private accessTokenFilePath As String, accessToken As String, accessTokenExpiryTime As String, refreshToken As String, refreshTokenExpiryTime As String
    Private refreshTokenExpiresIn As Integer

    ''' <summary>
    ''' Variables related to Send SMS
    ''' </summary>
    Public sendSMSErrorMessage As String = String.Empty
    Public sendSMSSuccessMessage As String = String.Empty
    Public sendSMSResponseData As SendSMSResponse = Nothing

    ''' <summary>
    ''' Variables related to Get Delivery Status
    ''' </summary>
    Public getSMSDeliveryStatusErrorMessage As String = String.Empty
    Public getSMSDeliveryStatusSuccessMessagae As String = String.Empty
    Public getSMSDeliveryStatusResponseData As GetDeliveryStatus = Nothing

    ''' <summary>
    ''' Variables related to Get SMS
    ''' </summary>

    Public getSMSSuccessMessage As String = String.Empty
    Public offlineShortCode As String = String.Empty
    Public getSMSErrorMessage As String = String.Empty
    Public getSMSResponseData As GetSmsResponse = Nothing

    ''' <summary>
    ''' Variables related to Receive SMS
    ''' </summary>
    Public onlineShortCode As String = String.Empty
    Public receiveSMSSuccessMessage As String = String.Empty
    Public receiveSMSErrorMesssage As String = String.Empty
    Public receivedSMSList As New List(Of ReceiveSMS)()
    Public receiveSMSFilePath As String = "\Messages.txt"
    ''' <summary>
    ''' Variables related to Get Delivery Status
    ''' </summary>
    Public receiveSMSDeliveryStatusErrorMessage As String = String.Empty
    Public receiveSMSDeliveryStatusSuccessMessagae As String = String.Empty
    Public receiveSMSDeliveryStatusResponseData As New List(Of deliveryInfoNotification)()
    Public receiveSMSDeliveryStatusFilePath As String = "\DeliveryStatus.txt"

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
#End Region

#Region "SMS Application Events"
    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This fucntion reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">Sender Details</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Try
            'ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
            Me.BypassCertificateError()
            Dim ableToRead As Boolean = Me.ReadConfigFile()
            If ableToRead = False Then
                Return
            End If

            If Session("lastSentSMSID") IsNot Nothing Then
                messageId.Value = Session("lastSentSMSID").ToString()
            End If
            'If Not IsPostBack Then
            readOnlineMessages()
            readOnlineDeliveryStatus()
            'End If
        Catch ex As Exception
            sendSMSErrorMessage = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' Reads from config file
    ''' </summary>
    ''' <returns>true/false; true if able to read else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\SMSApp1AccessToken.txt"
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            sendSMSErrorMessage = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.offlineShortCode = ConfigurationManager.AppSettings("OfflineShortCode")
        If String.IsNullOrEmpty(Me.offlineShortCode) Then
            sendSMSErrorMessage = "short_code is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            sendSMSErrorMessage = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            sendSMSErrorMessage = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "SMS"
        End If

        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            ' Default value
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

        Me.onlineShortCode = ConfigurationManager.AppSettings("OnlineShortCode")
        If String.IsNullOrEmpty(Me.onlineShortCode) Then
            sendSMSErrorMessage = "Online ShortCode is not defined in configuration file"
            Return False
        End If

        Me.sendSMSURL = ConfigurationManager.AppSettings("SendSMSURL")
        Me.getDeliveryStatusURL = ConfigurationManager.AppSettings("GetDeliveryStatusURL")
        Me.getSMSURL = ConfigurationManager.AppSettings("GetSMSURL")

        Me.receiveSMSDeliveryStatusFilePath = ConfigurationManager.AppSettings("ReceivedDeliveryStatusFilePath")
        Me.receiveSMSFilePath = ConfigurationManager.AppSettings("ReceivedMessagesFilePath")

        If Not IsPostBack Then
            Dim sampleMessages As String = ConfigurationManager.AppSettings("SMSSampleMessage")
            If Not String.IsNullOrEmpty(sampleMessages) Then
                Dim sample As String() = Regex.Split(sampleMessages, "_-_-")
                For Each sm As String In sample
                    message.Items.Add(sm)
                Next
            Else
                message.Items.Add("ATT SMS sample Message")

            End If
        End If
        Return True
    End Function

    ''' <summary>
    ''' This function is called with user clicks on send SMS
    ''' This validates the access token and then calls sendSMS method to invoke send SMS API.
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub BtnSubmit_Click(sender As Object, e As EventArgs)
        Try
            If Me.ReadAndGetAccessToken(sendSMSErrorMessage) = True Then
                Me.SendSmsMethod()
            Else
                sendSMSErrorMessage = "Unable to get access token."
            End If
        Catch ex As Exception
            sendSMSErrorMessage = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' This method is called when user clicks on get message button
    ''' </summary>
    ''' <param name="sender">Sender Details</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetMessagesButton_Click(sender As Object, e As EventArgs)
        Try
            If Me.ReadAndGetAccessToken(getSMSErrorMessage) = True Then
                Me.GetSms()
            Else
                getSMSErrorMessage = "Unable to get access token."
            End If
        Catch ex As Exception
            getSMSErrorMessage = ex.ToString()
        End Try
    End Sub
#End Region


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
    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub



    ''' <summary>
    ''' This function calls receive sms api to fetch the sms's
    ''' </summary>
    Private Sub GetSms()
        Try
            Dim receiveSmsResponseData As String
            If String.IsNullOrEmpty(Me.offlineShortCode) Then
                getSMSErrorMessage = "Short code is null or empty"
                Return
            End If

            Dim objRequest As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & Me.getSMSURL & "/" & Me.offlineShortCode.ToString()), HttpWebRequest)
            objRequest.Method = "GET"
            objRequest.Headers.Add("Authorization", "BEARER " & Me.accessToken)
            Dim receiveSmsResponseObject As HttpWebResponse = DirectCast(objRequest.GetResponse(), HttpWebResponse)
            Using receiveSmsResponseStream As New StreamReader(receiveSmsResponseObject.GetResponseStream())
                receiveSmsResponseData = receiveSmsResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                getSMSResponseData = New GetSmsResponse()
                getSMSResponseData = DirectCast(deserializeJsonObject.Deserialize(receiveSmsResponseData, GetType(GetSmsResponse)), GetSmsResponse)
                receiveSmsResponseStream.Close()
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

            getSMSErrorMessage = errorResponse & Environment.NewLine & we.ToString()
        Catch ex As Exception
            getSMSErrorMessage = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' Method will be called when the user clicks on Update Votes Total button
    ''' </summary>
    ''' <param name="sender">object, that invoked this method</param>
    ''' <param name="e">EventArgs, specific to this method</param>
    Protected Sub receiveStatusBtn_Click(sender As Object, e As EventArgs)
        Try
            'Me.readOnlineDeliveryStatus()
        Catch ex As Exception
            receiveSMSErrorMesssage = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' Method will be called when the user clicks on Update Votes Total button
    ''' </summary>
    ''' <param name="sender">object, that invoked this method</param>
    ''' <param name="e">EventArgs, specific to this method</param>
    Protected Sub receiveMessagesBtn_Click(sender As Object, e As EventArgs)
        Try
            'Me.readOnlineMessages()
        Catch ex As Exception
            receiveSMSErrorMesssage = ex.ToString()
        End Try
    End Sub



    ''' <summary>
    ''' This method reads the messages file and draw the table.
    ''' </summary>
    Private Sub readOnlineDeliveryStatus()
        Dim receivedMessagesFile As String = ConfigurationManager.AppSettings("ReceivedDeliveryStatusFilePath")
        If Not String.IsNullOrEmpty(receivedMessagesFile) Then
            receivedMessagesFile = Request.MapPath(receivedMessagesFile)
        Else
            receivedMessagesFile = Request.MapPath("~\DeliveryStatus.txt")
        End If
        Dim messagesLine As String = [String].Empty
        If File.Exists(receivedMessagesFile) Then
            Using sr As New StreamReader(receivedMessagesFile)
                While sr.Peek() >= 0
                    Dim dNot As New deliveryInfoNotification()
                    dNot.deliveryInfo = New ReceiveDeliveryInfo()
                    messagesLine = sr.ReadLine()
                    Dim messageValues As String() = Regex.Split(messagesLine, "_-_-")
                    dNot.messageId = messageValues(0)
                    dNot.deliveryInfo.address = messageValues(1)
                    dNot.deliveryInfo.deliveryStatus = messageValues(2)
                    receiveSMSDeliveryStatusResponseData.Add(dNot)
                End While
                receiveSMSDeliveryStatusResponseData.Reverse()
                sr.Close()

            End Using
        End If
    End Sub

    ''' <summary>
    ''' This method reads the messages file and draw the table.
    ''' </summary>
    Private Sub readOnlineMessages()
        Dim receivedMessagesFile As String = ConfigurationManager.AppSettings("ReceivedMessagesFilePath")
        If Not String.IsNullOrEmpty(receivedMessagesFile) Then
            receivedMessagesFile = Request.MapPath(receivedMessagesFile)
        Else
            receivedMessagesFile = Request.MapPath("~\Messages.txt")
        End If
        Dim messagesLine As String = [String].Empty
        If File.Exists(receivedMessagesFile) Then
            Using sr As New StreamReader(receivedMessagesFile)
                While sr.Peek() >= 0
                    Dim inboundMsg As New ReceiveSMS()
                    messagesLine = sr.ReadLine()
                    Dim messageValues As String() = Regex.Split(messagesLine, "_-_-")
                    inboundMsg.DateTime = messageValues(0)
                    inboundMsg.MessageId = messageValues(1)
                    inboundMsg.Message = messageValues(2)
                    inboundMsg.SenderAddress = messageValues(3)
                    inboundMsg.DestinationAddress = messageValues(4)
                    receivedSMSList.Add(inboundMsg)
                End While
                receivedSMSList.Reverse()
                sr.Close()

            End Using
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

    ''' <summary>
    ''' This function validates the input fields and if they are valid send sms api is invoked
    ''' </summary>
    Private Sub SendSmsMethod()
        Try
            Dim outBoundSmsJson As String = String.Empty
            Dim destinationNumbers As New List(Of String)()
            If Not String.IsNullOrEmpty(address.Value) Then
                Dim addressInput As String = address.Value
                Dim multipleAddresses As String() = addressInput.Split(","c)
                For Each addr As String In multipleAddresses
                    If addr.StartsWith("tel:") Then
                        destinationNumbers.Add(addr)
                    Else
                        Dim phoneNumberWithTel As String = "tel:" & addr
                        destinationNumbers.Add(phoneNumberWithTel)
                    End If
                Next
                If multipleAddresses.Length = 1 Then
                    Dim outBoundSms As New SendSMSDataForSingle()
                    outBoundSms.outboundSMSRequest = New OutboundSMSRequestForSingle()
                    outBoundSms.outboundSMSRequest.notifyDeliveryStatus = chkGetOnlineStatus.Checked
                    outBoundSms.outboundSMSRequest.address = destinationNumbers(0)
                    outBoundSms.outboundSMSRequest.message = message.SelectedValue
                    Dim javaScriptSerializer As New JavaScriptSerializer()
                    outBoundSmsJson = javaScriptSerializer.Serialize(outBoundSms)
                Else
                    Dim outBoundSms As New SendSMSDataForMultiple()
                    outBoundSms.outboundSMSRequest = New OutboundSMSRequestForMultiple()
                    outBoundSms.outboundSMSRequest.notifyDeliveryStatus = chkGetOnlineStatus.Checked
                    outBoundSms.outboundSMSRequest.address = destinationNumbers
                    outBoundSms.outboundSMSRequest.message = message.SelectedValue
                    Dim javaScriptSerializer As New JavaScriptSerializer()
                    outBoundSmsJson = javaScriptSerializer.Serialize(outBoundSms)
                End If
            Else
                sendSMSErrorMessage = "No input provided for Address"
                Return
            End If



            Dim sendSmsResponseData__1 As String
            Dim sendSmsRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & Me.sendSMSURL), HttpWebRequest)
            sendSmsRequestObject.Method = "POST"
            sendSmsRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)
            sendSmsRequestObject.ContentType = "application/json"
            sendSmsRequestObject.Accept = "application/json"

            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(outBoundSmsJson)
            sendSmsRequestObject.ContentLength = postBytes.Length

            Dim postStream As Stream = sendSmsRequestObject.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim sendSmsResponseObject As HttpWebResponse = DirectCast(sendSmsRequestObject.GetResponse(), HttpWebResponse)
            Using sendSmsResponseStream As New StreamReader(sendSmsResponseObject.GetResponseStream())
                sendSmsResponseData__1 = sendSmsResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                sendSMSResponseData = New SendSMSResponse()
                sendSMSResponseData.outBoundSMSResponse = New OutBoundSMSResponse()
                sendSMSResponseData = DirectCast(deserializeJsonObject.Deserialize(sendSmsResponseData__1, GetType(SendSMSResponse)), SendSMSResponse)
                If Not chkGetOnlineStatus.Checked Then
                    Session("lastSentSMSID") = sendSMSResponseData.outBoundSMSResponse.messageId
                    messageId.Value = Session("lastSentSMSID").ToString()
                End If
                sendSMSSuccessMessage = "Success"
                sendSmsResponseStream.Close()

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

            sendSMSErrorMessage = errorResponse
        Catch ex As Exception
            sendSMSErrorMessage = ex.ToString()
        End Try
    End Sub
#Region "Get SMS Delivery Status code."
    ''' <summary>
    ''' This method is called when user clicks on get delivery status button
    ''' </summary>
    ''' <param name="sender">Sender Information</param>
    ''' <param name="e">List of Arguments</param>
    Protected Sub GetDeliveryStatusButton_Click(sender As Object, e As EventArgs)
        Try
            Session("lastSentSMSID") = System.Web.HttpUtility.HtmlEncode(messageId.Value)
            If Me.ReadAndGetAccessToken(getSMSDeliveryStatusErrorMessage) = True Then
                Me.GetSmsDeliveryStatus()
            Else
                getSMSDeliveryStatusErrorMessage = "Unable to get access token."
            End If
        Catch ex As Exception
            getSMSDeliveryStatusErrorMessage = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' This function is called when user clicks on get delivery status button.
    ''' this funciton calls get sms delivery status API to fetch the status.
    ''' </summary>
    Private Sub GetSmsDeliveryStatus()
        Try
            Dim getSmsDeliveryStatusResponseData__1 As String
            Dim getSmsDeliveryStatusRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create((String.Empty & Me.endPoint & Me.getDeliveryStatusURL) + messageId.Value), HttpWebRequest)
            getSmsDeliveryStatusRequestObject.Method = "GET"
            getSmsDeliveryStatusRequestObject.Headers.Add("Authorization", "BEARER " & Me.accessToken)
            getSmsDeliveryStatusRequestObject.ContentType = "application/JSON"
            getSmsDeliveryStatusRequestObject.Accept = "application/json"
            Dim getSmsDeliveryStatusResponse As HttpWebResponse = DirectCast(getSmsDeliveryStatusRequestObject.GetResponse(), HttpWebResponse)
            Using getSmsDeliveryStatusResponseStream As New StreamReader(getSmsDeliveryStatusResponse.GetResponseStream())
                getSmsDeliveryStatusResponseData__1 = getSmsDeliveryStatusResponseStream.ReadToEnd()
                getSmsDeliveryStatusResponseData__1 = getSmsDeliveryStatusResponseData__1.Replace("-", String.Empty)
                Dim deserializeJsonObject As New JavaScriptSerializer()
                getSMSDeliveryStatusResponseData = New GetDeliveryStatus()
                getSMSDeliveryStatusResponseData = DirectCast(deserializeJsonObject.Deserialize(getSmsDeliveryStatusResponseData__1, GetType(GetDeliveryStatus)), GetDeliveryStatus)
                getSMSDeliveryStatusSuccessMessagae = "Success"
                getSmsDeliveryStatusResponseStream.Close()
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
            getSMSDeliveryStatusErrorMessage = errorResponse
        Catch ex As Exception
            getSMSDeliveryStatusErrorMessage = ex.ToString()
        End Try
    End Sub
#End Region


#Region "SMS Application related Data Structures"

#Region "Data structure for get access token"
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
                m_access_token = Value
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
                m_refresh_token = Value
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
                m_expires_in = Value
            End Set
        End Property
        Private m_expires_in As String
    End Class
#End Region
#Region "Data structure for send sms response"

    '''<summary>
    '''Class to hold ResourceReference
    '''</summary>
    Public Class ResourceReference
        '''<summary>
        '''Gets or sets resourceURL
        '''</summary>
        Public Property resourceURL() As String
            Get
                Return m_resourceURL
            End Get
            Set(value As String)
                m_resourceURL = Value
            End Set
        End Property
        Private m_resourceURL As String
    End Class

    Public Class SendSMSResponse
        Public outBoundSMSResponse As OutBoundSMSResponse
    End Class
    ''' <summary>
    ''' Class to hold send sms response
    ''' </summary>
    Public Class OutBoundSMSResponse
        ''' <summary>
        ''' Gets or sets messageId
        ''' </summary>
        Public Property messageId() As String
            Get
                Return m_messageId
            End Get
            Set(value As String)
                m_messageId = Value
            End Set
        End Property
        Private m_messageId As String
        ''' <summary>
        ''' Gets or sets ResourceReference
        ''' </summary>
        Public Property resourceReference() As ResourceReference
            Get
                Return m_resourceReference
            End Get
            Set(value As ResourceReference)
                m_resourceReference = Value
            End Set
        End Property
        Private m_resourceReference As ResourceReference
    End Class

    Public Class SendSMSDataForSingle
        Public Property outboundSMSRequest() As OutboundSMSRequestForSingle
            Get
                Return m_outboundSMSRequest
            End Get
            Set(value As OutboundSMSRequestForSingle)
                m_outboundSMSRequest = Value
            End Set
        End Property
        Private m_outboundSMSRequest As OutboundSMSRequestForSingle
    End Class

    Public Class OutboundSMSRequestForSingle
        ''' <summary>
        ''' Gets or sets the address to send.
        ''' </summary>
        Public Property address() As String
            Get
                Return m_address
            End Get
            Set(value As String)
                m_address = Value
            End Set
        End Property
        Private m_address As String

        ''' <summary>
        ''' Gets or sets message text to send.
        ''' </summary>
        Public Property message() As String
            Get
                Return m_message
            End Get
            Set(value As String)
                m_message = Value
            End Set
        End Property
        Private m_message As String

        Public Property notifyDeliveryStatus() As Boolean
            Get
                Return m_notifyDeliveryStatus
            End Get
            Set(value As Boolean)
                m_notifyDeliveryStatus = Value
            End Set
        End Property
        Private m_notifyDeliveryStatus As Boolean
    End Class
    Public Class SendSMSDataForMultiple
        Public Property outboundSMSRequest() As OutboundSMSRequestForMultiple
            Get
                Return m_outboundSMSRequest
            End Get
            Set(value As OutboundSMSRequestForMultiple)
                m_outboundSMSRequest = Value
            End Set
        End Property
        Private m_outboundSMSRequest As OutboundSMSRequestForMultiple
    End Class

    Public Class OutboundSMSRequestForMultiple
        ''' <summary>
        ''' Gets or sets the list of address to send.
        ''' </summary>
        Public Property address() As List(Of String)
            Get
                Return m_address
            End Get
            Set(value As List(Of String))
                m_address = Value
            End Set
        End Property
        Private m_address As List(Of String)

        ''' <summary>
        ''' Gets or sets message text to send.
        ''' </summary>
        Public Property message() As String
            Get
                Return m_message
            End Get
            Set(value As String)
                m_message = Value
            End Set
        End Property
        Private m_message As String

        Public Property notifyDeliveryStatus() As Boolean
            Get
                Return m_notifyDeliveryStatus
            End Get
            Set(value As Boolean)
                m_notifyDeliveryStatus = Value
            End Set
        End Property
        Private m_notifyDeliveryStatus As Boolean
    End Class
#End Region
#Region "Data structure for Get SMS (offline)"
    ''' <summary>
    ''' Class to hold rececive sms response
    ''' </summary>
    Public Class GetSmsResponse
        ''' <summary>
        ''' Gets or sets inbound sms message list
        ''' </summary>
        Public Property InboundSMSMessageList() As InboundSMSMessageList
            Get
                Return m_InboundSMSMessageList
            End Get
            Set(value As InboundSMSMessageList)
                m_InboundSMSMessageList = Value
            End Set
        End Property
        Private m_InboundSMSMessageList As InboundSMSMessageList
    End Class

    ''' <summary>
    ''' Class to hold inbound sms message list
    ''' </summary>
    Public Class InboundSMSMessageList
        ''' <summary>
        ''' Gets or sets inbound sms message
        ''' </summary>
        Public Property InboundSMSMessage() As List(Of InboundSMSMessage)
            Get
                Return m_InboundSMSMessage
            End Get
            Set(value As List(Of InboundSMSMessage))
                m_InboundSMSMessage = Value
            End Set
        End Property
        Private m_InboundSMSMessage As List(Of InboundSMSMessage)

        ''' <summary>
        ''' Gets or sets number of messages in a batch
        ''' </summary>
        Public Property NumberOfMessagesInThisBatch() As Integer
            Get
                Return m_NumberOfMessagesInThisBatch
            End Get
            Set(value As Integer)
                m_NumberOfMessagesInThisBatch = Value
            End Set
        End Property
        Private m_NumberOfMessagesInThisBatch As Integer

        ''' <summary>
        ''' Gets or sets resource url
        ''' </summary>
        Public Property ResourceURL() As String
            Get
                Return m_ResourceURL
            End Get
            Set(value As String)
                m_ResourceURL = Value
            End Set
        End Property
        Private m_ResourceURL As String

        ''' <summary>
        ''' Gets or sets total number of pending messages
        ''' </summary>
        Public Property TotalNumberOfPendingMessages() As Integer
            Get
                Return m_TotalNumberOfPendingMessages
            End Get
            Set(value As Integer)
                m_TotalNumberOfPendingMessages = Value
            End Set
        End Property
        Private m_TotalNumberOfPendingMessages As Integer
    End Class

    ''' <summary>
    ''' Class to hold inbound sms message
    ''' </summary>
    Public Class InboundSMSMessage
        ''' <summary>
        ''' Gets or sets message id
        ''' </summary>
        Public Property MessageId() As String
            Get
                Return m_MessageId
            End Get
            Set(value As String)
                m_MessageId = Value
            End Set
        End Property
        Private m_MessageId As String

        ''' <summary>
        ''' Gets or sets message
        ''' </summary>
        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(value As String)
                m_Message = Value
            End Set
        End Property
        Private m_Message As String

        ''' <summary>
        ''' Gets or sets sender address
        ''' </summary>
        Public Property SenderAddress() As String
            Get
                Return m_SenderAddress
            End Get
            Set(value As String)
                m_SenderAddress = Value
            End Set
        End Property
        Private m_SenderAddress As String
    End Class
#End Region
#Region "Data structure for Receive SMS (online)"
    ''' <summary>
    ''' Class to hold inbound sms message
    ''' </summary>
    Public Class ReceiveSMS
        ''' <summary>
        ''' Gets or sets datetime
        ''' </summary>
        Public Property DateTime() As String
            Get
                Return m_DateTime
            End Get
            Set(value As String)
                m_DateTime = Value
            End Set
        End Property
        Private m_DateTime As String

        ''' <summary>
        ''' Gets or sets destination address
        ''' </summary>
        Public Property DestinationAddress() As String
            Get
                Return m_DestinationAddress
            End Get
            Set(value As String)
                m_DestinationAddress = Value
            End Set
        End Property
        Private m_DestinationAddress As String

        ''' <summary>
        ''' Gets or sets message id
        ''' </summary>
        Public Property MessageId() As String
            Get
                Return m_MessageId
            End Get
            Set(value As String)
                m_MessageId = Value
            End Set
        End Property
        Private m_MessageId As String

        ''' <summary>
        ''' Gets or sets message
        ''' </summary>
        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(value As String)
                m_Message = Value
            End Set
        End Property
        Private m_Message As String

        ''' <summary>
        ''' Gets or sets sender address
        ''' </summary>
        Public Property SenderAddress() As String
            Get
                Return m_SenderAddress
            End Get
            Set(value As String)
                m_SenderAddress = Value
            End Set
        End Property
        Private m_SenderAddress As String
    End Class
#End Region
#Region "Data structure for Get Delivery Status (offline)"

    ''' <summary>
    ''' Class to hold delivery status
    ''' </summary>
    Public Class GetDeliveryStatus
        ''' <summary>
        ''' Gets or sets delivery info list
        ''' </summary>
        Public Property DeliveryInfoList() As DeliveryInfoList
            Get
                Return m_DeliveryInfoList
            End Get
            Set(value As DeliveryInfoList)
                m_DeliveryInfoList = Value
            End Set
        End Property
        Private m_DeliveryInfoList As DeliveryInfoList
    End Class

    ''' <summary>
    ''' Class to hold delivery info list
    ''' </summary>
    Public Class DeliveryInfoList
        ''' <summary>
        ''' Gets or sets resource url
        ''' </summary>
        Public Property ResourceURL() As String
            Get
                Return m_ResourceURL
            End Get
            Set(value As String)
                m_ResourceURL = Value
            End Set
        End Property
        Private m_ResourceURL As String

        ''' <summary>
        ''' Gets or sets delivery info
        ''' </summary>
        Public Property DeliveryInfo() As List(Of DeliveryInfo)
            Get
                Return m_DeliveryInfo
            End Get
            Set(value As List(Of DeliveryInfo))
                m_DeliveryInfo = Value
            End Set
        End Property
        Private m_DeliveryInfo As List(Of DeliveryInfo)
    End Class

    ''' <summary>
    ''' Class to hold delivery info
    ''' </summary>
    Public Class DeliveryInfo
        ''' <summary>
        ''' Gets or sets id
        ''' </summary>
        Public Property Id() As String
            Get
                Return m_Id
            End Get
            Set(value As String)
                m_Id = Value
            End Set
        End Property
        Private m_Id As String

        ''' <summary>
        ''' Gets or sets address
        ''' </summary>
        Public Property Address() As String
            Get
                Return m_Address
            End Get
            Set(value As String)
                m_Address = Value
            End Set
        End Property
        Private m_Address As String

        ''' <summary>
        ''' Gets or sets delivery status
        ''' </summary>
        Public Property Deliverystatus() As String
            Get
                Return m_Deliverystatus
            End Get
            Set(value As String)
                m_Deliverystatus = Value
            End Set
        End Property
        Private m_Deliverystatus As String
    End Class

#End Region
#Region "Data structure for receive delivery status"

    Public Class ReceiveDeliveryInfo
        ''' <summary>
        ''' Gets or sets the list of address.
        ''' </summary>
        Public Property address() As String
            Get
                Return m_address
            End Get
            Set(value As String)
                m_address = Value
            End Set
        End Property
        Private m_address As String
        ''' <summary>
        ''' Gets or sets the list of deliveryStatus.
        ''' </summary>
        Public Property deliveryStatus() As String
            Get
                Return m_deliveryStatus
            End Get
            Set(value As String)
                m_deliveryStatus = Value
            End Set
        End Property
        Private m_deliveryStatus As String
    End Class
    Public Class deliveryInfoNotification
        ''' <summary>
        ''' Gets or sets the list of messageId.
        ''' </summary>
        Public Property messageId() As String
            Get
                Return m_messageId
            End Get
            Set(value As String)
                m_messageId = Value
            End Set
        End Property
        Private m_messageId As String

        ''' <summary>
        ''' Gets or sets message text to send.
        ''' </summary>
        Public Property deliveryInfo() As ReceiveDeliveryInfo
            Get
                Return m_deliveryInfo
            End Get
            Set(value As ReceiveDeliveryInfo)
                m_deliveryInfo = Value
            End Set
        End Property
        Private m_deliveryInfo As ReceiveDeliveryInfo
    End Class
#End Region


#End Region
End Class