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
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

#End Region

''' <summary>
''' MMS_App1 class
''' </summary>
''' <remarks> This application allows an end user to send an MMS message with up to three attachments of any common format, 
''' and check the delivery status of that MMS message.
''' </remarks>
Partial Public Class MMS_App1
    Inherits System.Web.UI.Page
    ''' <summary>
    ''' Instance variables for local processing
    ''' </summary>
    Private endPoint As String, accessTokenFilePath As String, apiKey As String, secretKey As String, accessToken As String, scope As String, _
     refreshToken As String

    ''' <summary>
    ''' Instance variables for local processing
    ''' </summary>
    Private expirySeconds As String, refreshTokenExpiryTime As String

    ''' <summary>
    ''' Instance variables for local processing
    ''' </summary>
    Private accessTokenExpiryTime As String

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    ''' <summary>
    ''' Instance variables for local processing
    ''' </summary>
    Private phoneNumbersList As New List(Of String)()

    ''' <summary>
    ''' Instance variables for local processing
    ''' </summary>
    Private phoneNumbersParameter As String = Nothing

    Public sendMessageResponseSuccess As String = String.Empty
    Public sendMessageResponseError As String = String.Empty
    Public sendMMSResponseData As SendMMSResponse = Nothing
    Public getDeliveryStatusResponseSuccess As String = String.Empty
    Public getDeliveryStatusResponseError As String = String.Empty
    Public formattedResponse As New Dictionary(Of String, String)()
    Public getDeliveryStatusResponse As New Dictionary(Of String, String)()
    Public sendMessageResponse As New Dictionary(Of String, String)()
    Public imageList As New List(Of ImageData)()
    Public messageId As String = String.Empty
    Public ListenerShortCode As String = String.Empty
    Public ImageDirectory As String = String.Empty
    Public totalImages As Integer = 0

    Public getMMSDeliveryStatusResponseData As GetDeliveryStatus = Nothing



    Public receiveMMSDeliveryStatusResponseData As New List(Of deliveryInfoNotification)()

    Private SendImageFilesDir As String


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

    Public Sub DisplayImagesReceived()
        Try

            Me.ListenerShortCode = ConfigurationManager.AppSettings("ListenerShortCode")
            If String.IsNullOrEmpty(Me.ListenerShortCode) Then
                Me.ListenerShortCode = "Not defined"
            End If

            Me.ImageDirectory = ConfigurationManager.AppSettings("ImageDirectory")
            If String.IsNullOrEmpty(Me.ImageDirectory) Then
                Me.ImageDirectory = "~\ReceivedImages\"
            End If

            '''/ Read the refund file for the list of transactions and store locally
            Dim file As New FileStream(Request.MapPath(Me.ImageDirectory & "imageDetails.txt"), FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file)
            Dim line As String

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                Dim imgDetails As String() = Regex.Split(line, ":-:")
                If imgDetails(0) IsNot Nothing AndAlso imgDetails(1) IsNot Nothing AndAlso imgDetails(2) IsNot Nothing AndAlso imgDetails(3) IsNot Nothing Then
                    Dim img As New ImageData()
                    img.senderAddress = imgDetails(0)
                    img.[date] = imgDetails(1)
                    img.path = Path.GetFileName(Path.GetDirectoryName(Me.ImageDirectory)) & "\" & imgDetails(2)
                    img.text = imgDetails(3)
                    imageList.Add(img)
                End If
            End While
            Me.totalImages = Me.imageList.Count
            sr.Close()
            file.Close()
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub

#Region "Bypass SSL Certificate Error"

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

    ''' <summary>
    ''' This method reads config file and assigns values to local variables
    ''' </summary>
    ''' <returns>true/false, true- if able to read from config file</returns>
    Private Function ReadConfigFile() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            sendMessageResponseError = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            sendMessageResponseError = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            sendMessageResponseError = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "MMS"
        End If

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\MMSApp1AccessToken.txt"
        End If

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

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SendImageFilesDir")) Then
            Me.SendImageFilesDir = Request.MapPath(ConfigurationManager.AppSettings("SendImageFilesDir"))
        End If

        If Not IsPostBack Then
            attachment.Items.Add("")
            If Not String.IsNullOrEmpty(SendImageFilesDir) Then
                Dim filePaths As String() = Directory.GetFiles(Me.SendImageFilesDir)
                For Each filePath As String In filePaths
                    attachment.Items.Add(Path.GetFileName(filePath))
                Next
                If filePaths.Length > 0 Then
                    attachment.Items(0).Selected = True
                End If
            End If
        End If

        Return True
    End Function

#End Region
    ''' <summary>
    ''' Event, that triggers when the applicaiton page is loaded into the browser, reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Try
            'ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
            Me.BypassCertificateError()

            Me.ReadConfigFile()

            Me.DisplayImagesReceived()

            readOnlineDeliveryStatus()
        Catch ex As Exception
            sendMessageResponseError = ex.ToString()
        End Try
    End Sub
    ''' <summary>
    ''' This funciton initiates send mms api call to send selected files as an mms
    ''' </summary>
    Private Sub SendMMSMethod()
        Try
            Dim mmsAddress As String = Me.GetPhoneNumbers()
            Dim mmsMessage As String = subject.SelectedValue.ToString()

            If String.Compare(attachment.SelectedValue.ToString(), "") = 0 Then
                Me.SendMessageNoAttachments(mmsAddress, mmsMessage)
            Else
                Dim mmsFile As String = Me.SendImageFilesDir & attachment.SelectedValue.ToString()
                Me.SendMultimediaMessage(mmsAddress, mmsMessage, mmsFile)
            End If
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
            sendMessageResponseError = errorResponse & Environment.NewLine & we.Message
        Catch ex As Exception
            sendMessageResponseError = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' Method will be called when the user clicks on Update Votes Total button
    ''' </summary>
    ''' <param name="sender">object, that invoked this method</param>
    ''' <param name="e">EventArgs, specific to this method</param>
    Protected Sub receiveStatusBtn_Click(sender As Object, e As EventArgs)
    End Sub

    Protected Sub GetStatus_Click(sender As Object, e As EventArgs)
        Try
            Dim messageId As String = mmsId.Value
            If messageId Is Nothing OrElse messageId.Length <= 0 Then
                getDeliveryStatusResponseError = "Message Id is null or empty"
                Return
            End If

            If Me.ReadAndGetAccessToken(getDeliveryStatusResponseError) = True Then
                Dim mmsDeliveryStatus As String
                Dim mmsStatusRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/mms/v3/messaging/outbox/" & messageId), HttpWebRequest)
                mmsStatusRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)
                mmsStatusRequestObject.Method = "GET"

                Dim mmsStatusResponseObject As HttpWebResponse = DirectCast(mmsStatusRequestObject.GetResponse(), HttpWebResponse)
                Using mmsStatusResponseStream As New StreamReader(mmsStatusResponseObject.GetResponseStream())
                    mmsDeliveryStatus = mmsStatusResponseStream.ReadToEnd()
                    'mmsDeliveryStatus = mmsDeliveryStatus.Replace("-", string.Empty);
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    'Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsDeliveryStatus);
                    'DisplayDictionary(dict);
                    'getDeliveryStatusResponse = formattedResponse;
                    getMMSDeliveryStatusResponseData = New GetDeliveryStatus()
                    getMMSDeliveryStatusResponseData = DirectCast(deserializeJsonObject.Deserialize(mmsDeliveryStatus, GetType(GetDeliveryStatus)), GetDeliveryStatus)
                    getDeliveryStatusResponseSuccess = "true"
                    mmsStatusResponseStream.Close()
                End Using
            End If
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
            getDeliveryStatusResponseError = errorResponse & Environment.NewLine & we.Message
        Catch ex As Exception
            getDeliveryStatusResponseError = ex.ToString()
        End Try

    End Sub

    ''' <summary>
    ''' Sends MMS by calling messaging api
    ''' </summary>
    ''' <param name="mmsAddress">string, phone number</param>
    ''' <param name="mmsMessage">string, mms message</param>
    Private Sub SendMultimediaMessage(mmsAddress As String, mmsMessage As String, mmsFile As String)
        Dim boundary As String = "----------------------------" & DateTime.Now.Ticks.ToString("x")

        Dim mmsRequestObject As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & Me.endPoint & "/mms/v3/messaging/outbox"), HttpWebRequest)
        mmsRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)
        mmsRequestObject.ContentType = "multipart/related; type=""application/x-www-form-urlencoded""; start=""<startpart>""; boundary=""" & boundary & """" & vbCr & vbLf
        mmsRequestObject.Method = "POST"
        mmsRequestObject.KeepAlive = True

        Dim encoding As New UTF8Encoding()

        Dim totalpostBytes As Byte() = encoding.GetBytes(String.Empty)
        Dim sendMMSData As String = mmsAddress & "subject=" & Server.UrlEncode(mmsMessage) & "&notifyDeliveryStatus=" & chkGetOnlineStatus.Checked

        Dim data As String = String.Empty
        data += "--" & boundary & vbCr & vbLf
        data += "Content-Type: application/x-www-form-urlencoded;charset=UTF-8" & vbCr & vbLf & "Content-Transfer-Encoding: 8bit" & vbCr & vbLf & "Content-Disposition: form-data; name=""root-fields""" & vbCr & vbLf & "Content-ID:<startpart>" & vbCr & vbLf & vbCr & vbLf & sendMMSData
        ' +"\r\n";
        totalpostBytes = Me.FormMIMEParts(boundary, data, mmsFile)

        Dim byteLastBoundary As Byte() = encoding.GetBytes(vbCr & vbLf & "--" & boundary & "--" & vbCr & vbLf)
        Dim totalSize As Integer = totalpostBytes.Length + byteLastBoundary.Length

        Dim totalMS = New MemoryStream(New Byte(totalSize - 1) {}, 0, totalSize, True, True)
        totalMS.Write(totalpostBytes, 0, totalpostBytes.Length)
        totalMS.Write(byteLastBoundary, 0, byteLastBoundary.Length)

        Dim finalpostBytes As Byte() = totalMS.GetBuffer()
        mmsRequestObject.ContentLength = finalpostBytes.Length

        Dim postStream As Stream = Nothing
        Try
            postStream = mmsRequestObject.GetRequestStream()
            postStream.Write(finalpostBytes, 0, finalpostBytes.Length)
        Catch ex As Exception
            Throw ex
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try

        Dim mmsResponseObject As WebResponse = mmsRequestObject.GetResponse()
        Using streamReader As New StreamReader(mmsResponseObject.GetResponseStream())
            Dim mmsResponseData As String = streamReader.ReadToEnd()
            Dim deserializeJsonObject As New JavaScriptSerializer()
            'Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsResponseData);
            'DisplayDictionary(dict);
            'sendMessageResponse = formattedResponse;
            sendMMSResponseData = New SendMMSResponse()
            sendMMSResponseData.outboundMessageResponse = New OutBoundMMSResponse()
            sendMMSResponseData.outboundMessageResponse.resourceReference = New ResourceReference()
            sendMMSResponseData = DirectCast(deserializeJsonObject.Deserialize(mmsResponseData, GetType(SendMMSResponse)), SendMMSResponse)
            If Not chkGetOnlineStatus.Checked Then
                mmsId.Value = sendMMSResponseData.outboundMessageResponse.messageId
            End If
            sendMessageResponseSuccess = "true"
            streamReader.Close()
        End Using
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
    ''' This method will be called when user clicks on send mms button
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub SendMessage_Click(sender As Object, e As EventArgs)
        Try
            If Me.ReadAndGetAccessToken(sendMessageResponseError) = True Then
                Me.SendMMSMethod()

            End If
        Catch ex As Exception
            sendMessageResponseError = ex.ToString()
            Return
        End Try
    End Sub

    ''' <summary>
    ''' Form mime parts for the user input files
    ''' </summary>
    ''' <param name="boundary">string, boundary data</param>
    ''' <param name="data">string, mms message</param>
    ''' <returns>returns byte array of files</returns>
    Private Function FormMIMEParts(boundary As String, data As String, mmsFile As String) As Byte()
        Dim encoding As New UTF8Encoding()

        Dim postBytes As Byte() = encoding.GetBytes(String.Empty)
        Dim totalpostBytes As Byte() = encoding.GetBytes(String.Empty)

        Dim Head As Byte() = encoding.GetBytes(data)
        Dim totalSizeWithHead As Integer = totalpostBytes.Length + Head.Length

        Dim totalMSWithHead = New MemoryStream(New Byte(totalSizeWithHead - 1) {}, 0, totalSizeWithHead, True, True)
        totalMSWithHead.Write(totalpostBytes, 0, totalpostBytes.Length)
        totalMSWithHead.Write(Head, 0, Head.Length)
        totalpostBytes = totalMSWithHead.GetBuffer()

        postBytes = Me.GetBytesOfFile(boundary, mmsFile)
        Dim msOne = JoinTwoByteArrays(totalpostBytes, postBytes)
        totalpostBytes = msOne.GetBuffer()

        Return totalpostBytes
    End Function

    ''' <summary>
    ''' Gets the bytes representation of file along with mime part
    ''' </summary>
    ''' <param name="boundary">string, boundary message</param>
    ''' <param name="data">string, mms message</param>
    ''' <param name="filePath">string, filepath</param>
    ''' <returns>byte[], representation of file in bytes</returns>
    Private Function GetBytesOfFile(boundary As String, filePath As String) As Byte()
        Dim encoding As New UTF8Encoding()
        Dim postBytes As Byte() = encoding.GetBytes(String.Empty)
        Dim fileStream As FileStream = Nothing
        Dim binaryReader As BinaryReader = Nothing

        Try
            Dim mmsFileName As String = Path.GetFileName(filePath)
            Dim mmsFileExtension As String = Path.GetExtension(filePath)
            Dim attachmentContentType As String = MapContentTypeFromExtension(mmsFileExtension)
            fileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
            binaryReader = New BinaryReader(fileStream)

            Dim image As Byte() = binaryReader.ReadBytes(CInt(fileStream.Length))

            Dim data As String = vbCr & vbLf & "--" & boundary & vbCr & vbLf
            data += "Content-Disposition: attachment; filename=" & mmsFileName & vbCr & vbLf
            data += "Content-Type: " & attachmentContentType & ";name=" & mmsFileName & vbCr & vbLf
            data += "Content-ID: " & mmsFileName & vbCr & vbLf
            data += "Content-Transfer-Encoding: Binary" & vbCr & vbLf & vbCr & vbLf

            Dim firstPart As Byte() = encoding.GetBytes(data)
            Dim newSize As Integer = firstPart.Length + image.Length

            Dim memoryStream = New MemoryStream(New Byte(newSize - 1) {}, 0, newSize, True, True)
            memoryStream.Write(firstPart, 0, firstPart.Length)
            memoryStream.Write(image, 0, image.Length)

            postBytes = memoryStream.GetBuffer()
        Catch ex As Exception
            Throw ex
        Finally
            If binaryReader IsNot Nothing Then
                binaryReader.Close()
            End If

            If fileStream IsNot Nothing Then
                fileStream.Close()
            End If
        End Try

        Return postBytes
    End Function
    ''' <summary>
    ''' Invokes messaging api to send message without any attachments
    ''' </summary>
    ''' <param name="mmsAddress">string, phone number</param>
    ''' <param name="mmsMessage">string, mms message</param>
    Private Sub SendMessageNoAttachments(mmsAddress As String, mmsMessage As String)
        Dim boundaryToSend As String = "----------------------------" & DateTime.Now.Ticks.ToString("x")

        Dim mmsRequestObject As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & Me.endPoint & "/mms/v3/messaging/outbox"), HttpWebRequest)
        mmsRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)
        mmsRequestObject.ContentType = "multipart/form-data; type=""application/x-www-form-urlencoded""; start=""<startpart>""; boundary=""" & boundaryToSend & """" & vbCr & vbLf
        mmsRequestObject.Method = "POST"
        mmsRequestObject.KeepAlive = True

        Dim encoding As New UTF8Encoding()
        Dim bytesToSend As Byte() = encoding.GetBytes(String.Empty)
        Dim mmsParameters As String = mmsAddress & "subject=" & Server.UrlEncode(mmsMessage) & "&notifyDeliveryStatus=" & chkGetOnlineStatus.Checked

        Dim dataToSend As String = String.Empty
        dataToSend += "--" & boundaryToSend & vbCr & vbLf
        dataToSend += "Content-Type: application/x-www-form-urlencoded; charset=UTF-8" & vbCr & vbLf & "Content-Transfer-Encoding: 8bit" & vbCr & vbLf & "Content-Disposition: form-data; name=""root-fields""" & vbCr & vbLf & "Content-ID: <startpart>" & vbCr & vbLf & vbCr & vbLf & mmsParameters & vbCr & vbLf
        dataToSend += "--" & boundaryToSend & "--" & vbCr & vbLf
        bytesToSend = encoding.GetBytes(dataToSend)

        Dim sizeToSend As Integer = bytesToSend.Length
        Dim memBufToSend = New MemoryStream(New Byte(sizeToSend - 1) {}, 0, sizeToSend, True, True)
        memBufToSend.Write(bytesToSend, 0, bytesToSend.Length)
        Dim finalData As Byte() = memBufToSend.GetBuffer()
        mmsRequestObject.ContentLength = finalData.Length

        Dim postStream As Stream = Nothing
        Try
            postStream = mmsRequestObject.GetRequestStream()
            postStream.Write(finalData, 0, finalData.Length)
        Catch ex As Exception
            Throw ex
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try

        Dim mmsResponseObject As WebResponse = mmsRequestObject.GetResponse()
        Using streamReader As New StreamReader(mmsResponseObject.GetResponseStream())
            Dim mmsResponseData As String = streamReader.ReadToEnd()
            Dim deserializeJsonObject As New JavaScriptSerializer()
            'Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(mmsResponseData);
            'DisplayDictionary(dict);
            sendMMSResponseData = New SendMMSResponse()
            sendMMSResponseData.outboundMessageResponse = New OutBoundMMSResponse()
            sendMMSResponseData.outboundMessageResponse.resourceReference = New ResourceReference()
            sendMMSResponseData = DirectCast(deserializeJsonObject.Deserialize(mmsResponseData, GetType(SendMMSResponse)), SendMMSResponse)
            If Not chkGetOnlineStatus.Checked Then
                mmsId.Value = sendMMSResponseData.outboundMessageResponse.messageId
            End If
            sendMessageResponseSuccess = "true"
            streamReader.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Content type based on the file extension.
    ''' </summary>
    ''' <param name="extension">file extension</param>
    ''' <returns>the Content type mapped to the extension"/> summed memory stream</returns>
    Private Shared Function MapContentTypeFromExtension(extension As String) As String
        Dim extensionToContentTypeMapping As New Dictionary(Of String, String)() From { _
         {".jpg", "image/jpeg"}, _
         {".bmp", "image/bmp"}, _
         {".mp3", "audio/mp3"}, _
         {".m4a", "audio/m4a"}, _
         {".gif", "image/gif"}, _
         {".3gp", "video/3gpp"}, _
         {".3g2", "video/3gpp2"}, _
         {".wmv", "video/x-ms-wmv"}, _
         {".m4v", "video/x-m4v"}, _
         {".amr", "audio/amr"}, _
         {".mp4", "video/mp4"}, _
         {".avi", "video/x-msvideo"}, _
         {".mov", "video/quicktime"}, _
         {".mpeg", "video/mpeg"}, _
         {".wav", "audio/x-wav"}, _
         {".aiff", "audio/x-aiff"}, _
         {".aifc", "audio/x-aifc"}, _
         {".midi", ".midi"}, _
         {".au", "audio/basic"}, _
         {".xwd", "image/x-xwindowdump"}, _
         {".png", "image/png"}, _
         {".tiff", "image/tiff"}, _
         {".ief", "image/ief"}, _
         {".txt", "text/plain"}, _
         {".html", "text/html"}, _
         {".vcf", "text/x-vcard"}, _
         {".vcs", "text/x-vcalendar"}, _
         {".mid", "application/x-midi"}, _
         {".imy", "audio/iMelody"} _
        }
        If extensionToContentTypeMapping.ContainsKey(extension) Then
            Return extensionToContentTypeMapping(extension)
        Else
            Throw New ArgumentException("invalid attachment extension")
        End If
    End Function

    ''' <summary>
    ''' This function adds two byte arrays
    ''' </summary>
    ''' <param name="firstByteArray">first array of bytes</param>
    ''' <param name="secondByteArray">second array of bytes</param>
    ''' <returns>returns MemoryStream after joining two byte arrays</returns>
    Private Shared Function JoinTwoByteArrays(firstByteArray As Byte(), secondByteArray As Byte()) As MemoryStream
        Dim newSize As Integer = firstByteArray.Length + secondByteArray.Length
        Dim ms = New MemoryStream(New Byte(newSize - 1) {}, 0, newSize, True, True)
        ms.Write(firstByteArray, 0, firstByteArray.Length)
        ms.Write(secondByteArray, 0, secondByteArray.Length)
        Return ms
    End Function

    ''' <summary>
    ''' Gets formatted phone number
    ''' </summary>
    ''' <returns>string, phone number</returns>
    Private Function GetPhoneNumber(ByRef [error] As String) As String
        Dim tryParseResult As Long = 0

        Dim smsAddressInput As String = address.Value

        Dim smsAddressFormatted As String = String.Empty

        Dim phoneStringPattern As String = "^\d{3}-\d{3}-\d{4}$"
        If System.Text.RegularExpressions.Regex.IsMatch(smsAddressInput, phoneStringPattern) Then
            smsAddressFormatted = smsAddressInput.Replace("-", String.Empty)
        Else
            smsAddressFormatted = smsAddressInput
        End If

        If smsAddressFormatted.Length = 16 AndAlso smsAddressFormatted.StartsWith("tel:+1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(6, 10)
        ElseIf smsAddressFormatted.Length = 15 AndAlso smsAddressFormatted.StartsWith("tel:+") Then
            smsAddressFormatted = smsAddressFormatted.Substring(5, 10)
        ElseIf smsAddressFormatted.Length = 14 AndAlso smsAddressFormatted.StartsWith("tel:") Then
            smsAddressFormatted = smsAddressFormatted.Substring(4, 10)
        ElseIf smsAddressFormatted.Length = 12 AndAlso smsAddressFormatted.StartsWith("+1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(2, 10)
        ElseIf smsAddressFormatted.Length = 11 AndAlso smsAddressFormatted.StartsWith("1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(1, 10)
        End If

        If (smsAddressFormatted.Length <> 10) OrElse (Not Long.TryParse(smsAddressFormatted, tryParseResult)) Then
            [error] = "Invalid phone number: " & smsAddressInput
            smsAddressFormatted = String.Empty
        End If

        Return smsAddressFormatted
    End Function

    ''' <summary>
    ''' This method gets the phone numbers present in phonenumber text box and validates each phone number and prepares valid and invalid phone number lists
    ''' and returns a bool value indicating if able to get the phone numbers.
    ''' </summary>
    ''' <returns>true/false; true if able to get valis phone numbers, else false</returns>
    Private Function GetPhoneNumbers() As String

        Dim phoneNumbers As String() = address.Value.Split(","c)
        For Each phoneNum As String In phoneNumbers
            Me.phoneNumbersList.Add(phoneNum)
        Next

        For Each phoneNo As String In Me.phoneNumbersList
            If phoneNo.StartsWith("tel:") Then
                Me.phoneNumbersParameter = Me.phoneNumbersParameter & "address=" & Server.UrlEncode(phoneNo.ToString()) & "&"
            Else
                Me.phoneNumbersParameter = Me.phoneNumbersParameter & "address=" & Server.UrlEncode("tel:" & phoneNo.ToString()) & "&"
            End If
        Next
        If Not String.IsNullOrEmpty(Me.phoneNumbersParameter) Then
            Return Me.phoneNumbersParameter
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' This method reads the messages file and draw the table.
    ''' </summary>
    Private Sub readOnlineDeliveryStatus()
        Try
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
                        receiveMMSDeliveryStatusResponseData.Add(dNot)
                    End While
                    sr.Close()
                    receiveMMSDeliveryStatusResponseData.Reverse()

                End Using
            End If
        Catch ex As Exception
            Return
        End Try
    End Sub

#Region "Data Structures"

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

    Public Class SendMMSResponse
        Public outboundMessageResponse As OutBoundMMSResponse
    End Class
    ''' <summary>
    ''' Class to hold send sms response
    ''' </summary>
    Public Class OutBoundMMSResponse
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
    ''' Image structure Object
    ''' </summary>
    Public Class ImageData
        ''' <summary>
        ''' Gets or sets the value of path
        ''' </summary>
        Public Property path() As String
            Get
                Return m_path
            End Get
            Set(value As String)
                m_path = Value
            End Set
        End Property
        Private m_path As String

        ''' <summary>
        ''' Gets or sets the value of date
        ''' </summary>
        Public Property [date]() As String
            Get
                Return m_date
            End Get
            Set(value As String)
                m_date = Value
            End Set
        End Property
        Private m_date As String

        ''' <summary>
        ''' Gets or sets the value of senderAddress
        ''' </summary>
        Public Property senderAddress() As String
            Get
                Return m_senderAddress
            End Get
            Set(value As String)
                m_senderAddress = Value
            End Set
        End Property
        Private m_senderAddress As String

        ''' <summary>
        ''' Gets or sets the value of text
        ''' </summary>
        Public Property text() As String
            Get
                Return m_text
            End Get
            Set(value As String)
                m_text = Value
            End Set
        End Property
        Private m_text As String
    End Class

    ''' <summary>
    ''' AccessTokenResponse Object
    ''' </summary>
    Public Class AccessTokenResponse
        ''' <summary>
        ''' Gets or sets the value of access_token
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
        ''' Gets or sets the value of refresh_token
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
        ''' Gets or sets the value of expires_in
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
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function


#End Region
End Class