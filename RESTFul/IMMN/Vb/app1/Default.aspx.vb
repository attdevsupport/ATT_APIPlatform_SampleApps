' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>


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

Partial Public Class Mobo_App1
    Inherits System.Web.UI.Page

    ''' <summary>
    ''' API Address
    ''' </summary>
    Private endPoint As String

    ''' <summary>
    ''' Access token variables - temporary
    ''' </summary>
    Private apiKey As String, authCode As String, authorizeRedirectUri As String, secretKey As String, accessToken As String, scope As String, _
     refreshToken As String, refreshTokenExpiryTime As String, accessTokenExpiryTime As String

    ''' <summary>
    ''' Maximum number of addresses user can specify
    ''' </summary>
    Private maxAddresses As Integer

    ''' <summary>
    ''' List of addresses to send
    ''' </summary>
    Private addressList As New List(Of String)()

    ''' <summary>
    ''' Variable to hold phone number(s)/email address(es)/short code(s) parameter.
    ''' </summary>
    Private phoneNumbersParameter As String = Nothing

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    Private AttachmentFilesDir As String = String.Empty
    Public attachments As List(Of String) = Nothing
    Public sendMessageSuccessResponse As String = String.Empty
    Public sendMessageErrorResponse As String = String.Empty
    Public getHeadersErrorResponse As String = String.Empty
    Public getHeadersSuccessResponse As String = String.Empty
    Public getMessageSuccessResponse As String = String.Empty
    Public getMessageContentSuccessResponse As String = String.Empty
    Public getMessageContentErrorResponse As String = String.Empty
    Public getMessageErrorResponse As String = String.Empty
    Public filters As String = String.Empty
    Public createMessageIndexSuccessResponse As String = String.Empty
    Public createMessageIndexErrorResponse As String = String.Empty
    Public csGetMessageListDetailsErrorResponse As String = String.Empty
    Public csGetMessageListDetailsSuccessResponse As String = String.Empty
    Public getNotificationConnectionDetailsSuccessResponse As String = String.Empty
    Public getNotificationConnectionDetailsErrorResponse As String = String.Empty
    Public getNotificationConnectionDetailsResponse As New csNotificationConnectionDetails()
    Public csGetMessageListDetailsResponse As New MessageList()
    Public csDeltaResponse As New DeltaResponse()
    Public getMessageContentResponse As New csMessageContentDetails()
    Public getMessageDetailsResponse As New Message()
    Public getMessageIndexInfoResponse As New MessageIndexInfo()
    Public deleteMessageSuccessResponse As String = String.Empty
    Public deleteMessageErrorResponse As String = String.Empty
    Public updateMessageSuccessResponse As String = String.Empty
    Public updateMessageErrorResponse As String = String.Empty
    Public messageIndexSuccessResponse As String = String.Empty
    Public messageIndexErrorResponse As String = String.Empty
    Public deltaSuccessResponse As String = String.Empty
    Public deltaErrorResponse As String = String.Empty
    Public getMessageListSuccessResponse As String = String.Empty
    Public getMessageListErrorResponse As String = String.Empty
    Public content_result As String = String.Empty
    Public receivedBytes As Byte() = Nothing
    Public getContentResponseObject As WebResponse = Nothing
    Public imageData As String() = Nothing
    Public showSendMsg As String = String.Empty
    Public showCreateMessageIndex As String = String.Empty
    Public showGetNotificationConnectionDetails As String = String.Empty
    Public showDeleteMessage As String = String.Empty
    Public showUpdateMessage As String = String.Empty
    Public showGetMessage As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Me.BypassCertificateError()
        Me.ReadConfigFile()

        If (Session("cs_rest_appState") = "GetToken") AndAlso (Request("Code") IsNot Nothing) Then
            Me.authCode = Request("code").ToString()
            If Me.GetAccessToken(AccessTokenType.Authorization_Code) = True Then
                RestoreRequestSessionVariables()
                ResetRequestSessionVariables()
                If String.Compare(Session("cs_rest_ServiceRequest").ToString(), "sendmessasge") = 0 Then
                    Me.SendMessageRequest()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getmessagecontent") = 0 Then
                    Me.GetMessageContentByIDnPartNumber()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "createmessageindex") = 0 Then
                    Me.createMessageIndex()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getnotificationconnectiondetails") = 0 Then
                    Me.getNotificationConnectionDetails()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deletemessage") = 0 Then
                    Me.deleteMessage()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deltamessage") = 0 Then
                    Me.getDelta()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getmessagelist") = 0 Then
                    Me.getMessageList()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "messageindex") = 0 Then
                    Me.getMessageIndex()
                ElseIf String.Compare(Session("cs_rest_ServiceRequest").ToString(), "updatemessage") = 0 Then
                    Me.updateMessage()
                End If
            Else
                sendMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

    End Sub

    ''' <summary>
    ''' This function resets access token related session variable to null 
    ''' </summary>
    Private Sub ResetTokenSessionVariables()
        Session("cs_rest_AccessToken") = Nothing
        Session("cs_rest_AccessTokenExpirtyTime") = Nothing
        Session("cs_rest_RefreshToken") = Nothing
        Session("cs_rest_RefreshTokenExpiryTime") = Nothing
    End Sub

    ''' <summary>
    ''' This function resets access token related  variable to null 
    ''' </summary>
    Private Sub ResetTokenVariables()
        Me.accessToken = Nothing
        Me.refreshToken = Nothing
        Me.refreshTokenExpiryTime = Nothing
        Me.accessTokenExpiryTime = Nothing
    End Sub

    ''' <summary>
    ''' Redirect to OAuth and get Authorization Code
    ''' </summary>
    Private Sub GetAuthCode()
        Try
            Response.Redirect(Convert.ToString((Convert.ToString((Convert.ToString(String.Empty + Me.endPoint & Convert.ToString("/oauth/authorize?scope=")) & Me.scope) + "&client_id=") & Me.apiKey) + "&redirect_url=") & Me.authorizeRedirectUri)
        Catch ex As Exception
            If Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                sendMessageErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "createmessageindex") = 0) Then
                createMessageIndexErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getnotificationconnectiondetails") = 0) Then
                getNotificationConnectionDetailsErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deletemessage") = 0) Then
                deleteMessageErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deltamessage") = 0) Then
                deltaErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getmessagelist") = 0) Then
                getMessageListErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "messageindex") = 0) Then
                messageIndexErrorResponse = ex.Message
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "updatemessage") = 0) Then
                updateMessageErrorResponse = ex.Message
            Else
                getMessageErrorResponse = ex.Message
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Reads access token related session variables to local variables
    ''' </summary>
    ''' <returns>true/false depending on the session variables</returns>
    Private Function ReadTokenSessionVariables() As Boolean
        If Session("cs_rest_AccessToken") IsNot Nothing Then
            Me.accessToken = Session("cs_rest_AccessToken").ToString()
        Else
            Me.accessToken = Nothing
        End If

        If Session("cs_rest_AccessTokenExpirtyTime") IsNot Nothing Then
            Me.accessTokenExpiryTime = Session("cs_rest_AccessTokenExpirtyTime").ToString()
        Else
            Me.accessTokenExpiryTime = Nothing
        End If

        If Session("cs_rest_RefreshToken") IsNot Nothing Then
            Me.refreshToken = Session("cs_rest_RefreshToken").ToString()
        Else
            Me.refreshToken = Nothing
        End If

        If Session("cs_rest_RefreshTokenExpiryTime") IsNot Nothing Then
            Me.refreshTokenExpiryTime = Session("cs_rest_RefreshTokenExpiryTime").ToString()
        Else
            Me.refreshTokenExpiryTime = Nothing
        End If

        If (Me.accessToken Is Nothing) OrElse (Me.accessTokenExpiryTime Is Nothing) OrElse (Me.refreshToken Is Nothing) OrElse (Me.refreshTokenExpiryTime Is Nothing) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Validates access token related variables
    ''' </summary>
    ''' <returns>string, returns VALID_ACCESS_TOKEN if its valid
    ''' otherwise, returns INVALID_ACCESS_TOKEN if refresh token expired or not able to read session variables
    ''' return REFRESH_TOKEN, if access token in expired and refresh token is valid</returns>
    Private Function IsTokenValid() As String
        If Session("cs_rest_AccessToken") Is Nothing Then
            Return "INVALID_ACCESS_TOKEN"
        End If

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
    ''' Get access token based on the type parameter type values.
    ''' </summary>
    ''' <param name="type">If type value is 0, access token is fetch for authorization code flow
    ''' If type value is 2, access token is fetch for authorization code floww based on the exisiting refresh token</param>
    ''' <returns>true/false; true if success, else false</returns>
    Private Function GetAccessToken(ByVal type As AccessTokenType) As Boolean
        Dim postStream As Stream = Nothing
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()
            Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty + Me.endPoint & Convert.ToString("/oauth/token"))
            accessTokenRequest.Method = "POST"
            Dim oauthParameters As String = String.Empty

            If type = AccessTokenType.Authorization_Code Then
                oauthParameters = Convert.ToString((Convert.ToString((Convert.ToString((Convert.ToString("client_id=") & Me.apiKey) + "&client_secret=") & Me.secretKey) + "&code=") & Me.authCode) + "&grant_type=authorization_code&scope=") & Me.scope
            Else
                oauthParameters = Convert.ToString((Convert.ToString((Convert.ToString("grant_type=refresh_token&client_id=") & Me.apiKey) + "&client_secret=") & Me.secretKey) + "&refresh_token=") & Me.refreshToken
            End If

            accessTokenRequest.ContentType = "application/x-www-form-urlencoded"
            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(oauthParameters)
            accessTokenRequest.ContentLength = postBytes.Length
            postStream = accessTokenRequest.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim accessTokenResponse As WebResponse = accessTokenRequest.GetResponse()
            Using accessTokenResponseStream As New StreamReader(accessTokenResponse.GetResponseStream())
                Dim access_token_json As String = accessTokenResponseStream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As AccessTokenResponse = DirectCast(deserializeJsonObject.Deserialize(access_token_json, GetType(AccessTokenResponse)), AccessTokenResponse)
                If deserializedJsonObj.access_token IsNot Nothing Then
                    Me.accessToken = deserializedJsonObj.access_token
                    Me.refreshToken = deserializedJsonObj.refresh_token

                    Dim refreshExpiry As DateTime = currentServerTime.AddHours(Me.refreshTokenExpiresIn)

                    Session("cs_rest_AccessTokenExpirtyTime") = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in))

                    If deserializedJsonObj.expires_in.Equals("0") Then
                        Dim defaultAccessTokenExpiresIn As Integer = 100
                        ' In Years
                        Session("cs_rest_AccessTokenExpirtyTime") = currentServerTime.AddYears(defaultAccessTokenExpiresIn)
                    End If

                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString()

                    Session("cs_rest_AccessToken") = Me.accessToken

                    Me.accessTokenExpiryTime = Session("cs_rest_AccessTokenExpirtyTime").ToString()
                    Session("cs_rest_RefreshToken") = Me.refreshToken
                    Session("cs_rest_RefreshTokenExpiryTime") = Me.refreshTokenExpiryTime.ToString()
                    Session("cs_rest_appState") = "TokenReceived"
                    accessTokenResponseStream.Close()
                    Return True
                Else
                    Dim errorMessage As String = "Auth server returned null access token"
                    If Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                        sendMessageErrorResponse = errorMessage
                    ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "createmessageindex") = 0) Then
                        createMessageIndexErrorResponse = errorMessage
                    ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getnotificationconnectiondetails") = 0) Then
                        getNotificationConnectionDetailsErrorResponse = errorMessage
                    ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deletemessage") = 0) Then
                        deleteMessageErrorResponse = errorMessage
                    Else
                        getMessageErrorResponse = errorMessage
                    End If
                    Return False
                End If
            End Using
        Catch ex As Exception
            Dim errorMessage As String = ex.Message
            If Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                sendMessageErrorResponse = errorMessage
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "createmessageindex") = 0) Then
                createMessageIndexErrorResponse = errorMessage
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "getnotificationconnectiondetails") = 0) Then
                getNotificationConnectionDetailsErrorResponse = errorMessage
            ElseIf Session("cs_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("cs_rest_ServiceRequest").ToString(), "deletemessage") = 0) Then
                deleteMessageErrorResponse = errorMessage
            End If
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Read parameters from configuraton file
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            sendMessageErrorResponse = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            sendMessageErrorResponse = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            sendMessageErrorResponse = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.authorizeRedirectUri = ConfigurationManager.AppSettings("authorize_redirect_uri")
        If String.IsNullOrEmpty(Me.authorizeRedirectUri) Then
            sendMessageErrorResponse = "authorize_redirect_uri is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "IMMN,MIM"
        End If

        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("max_addresses")) Then
            Me.maxAddresses = 10
        Else
            Me.maxAddresses = Convert.ToInt32(ConfigurationManager.AppSettings("max_addresses"))
        End If

        Dim refreshTokenExpires As String = ConfigurationManager.AppSettings("refreshTokenExpiresIn")
        If Not String.IsNullOrEmpty(refreshTokenExpires) Then
            Me.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires)
        Else
            Me.refreshTokenExpiresIn = 24
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("AttachmentFilesDir")) Then
            Me.AttachmentFilesDir = Request.MapPath(ConfigurationManager.AppSettings("AttachmentFilesDir"))
        End If

        If Not IsPostBack Then

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("AttachmentFilesDir")) Then
                Dim filePaths As String() = Directory.GetFiles(Me.AttachmentFilesDir)
                For Each filePath As String In filePaths
                    attachment.Items.Add(Path.GetFileName(filePath))
                Next
            End If
        End If
        Return True
    End Function

    ''' <summary>
    ''' Gets the mapping of extension with predefined content types
    ''' </summary>
    ''' <param name="extension">file extension</param>
    ''' <returns>string, content type</returns>
    Private Function GetContentTypeFromExtension(ByVal extension As String) As String
		Dim extensionToContentType As New Dictionary(Of String, String)() From { _
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
        If extensionToContentType.ContainsKey(extension) Then
            Return extensionToContentType(extension)
        Else
            Return "Not Found"
        End If
    End Function

    ''' <summary>
    ''' Sends message to the list of addresses provided.
    ''' </summary>
    ''' <param name="attachments">List of attachments</param>
    Private Sub SendMessageRequest(ByVal accToken As String, ByVal edPoint As String, ByVal subject As String, ByVal message As String, ByVal groupflag As String, ByVal attachments As ArrayList)
        Dim postStream As Stream = Nothing
        Try
            Dim boundaryToSend As String = "----------------------------" + DateTime.Now.Ticks.ToString("x")

            Dim msgRequestObject As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & edPoint & "/myMessages/v2/messages"), HttpWebRequest)
            msgRequestObject.Headers.Add("Authorization", "Bearer " & accToken)
            msgRequestObject.Method = "POST"
            Dim contentType As String = "multipart/related; type=""application/x-www-form-urlencoded""; start=""startpart""; boundary=""" & boundaryToSend & """" & vbCr & vbLf
            msgRequestObject.ContentType = contentType
            'msgRequestObject.Accept = "application/xml";
            Dim mmsParameters As String = Convert.ToString(Me.phoneNumbersParameter) & "subject=" & Server.UrlEncode(subject) & "&text=" & Server.UrlEncode(message) & "&isGroup=" & groupflag
            Dim dataToSend As String = String.Empty
            dataToSend += "--" & boundaryToSend & vbCr & vbLf
            dataToSend += "Content-Disposition: form-data; name=""root-fields""" & vbCr & vbLf & "Content-Type: application/x-www-form-urlencoded; charset=UTF-8" & vbCr & vbLf & "Content-Transfer-Encoding: 8bit" & vbCr & vbLf & "Content-ID: startpart" & vbCr & vbLf & vbCr & vbLf & mmsParameters & vbCr & vbLf

            Dim encoding As New UTF8Encoding()
            If (attachments Is Nothing) OrElse (attachments.Count = 0) Then
                If Not groupCheckBox.Checked Then
                    msgRequestObject.ContentType = "application/x-www-form-urlencoded"
                    Dim postBytes As Byte() = encoding.GetBytes(mmsParameters)
                    msgRequestObject.ContentLength = postBytes.Length

                    postStream = msgRequestObject.GetRequestStream()
                    postStream.Write(postBytes, 0, postBytes.Length)
                    postStream.Close()

                    Dim mmsResponseObject1 As WebResponse = msgRequestObject.GetResponse()
                    Using sr As New StreamReader(mmsResponseObject1.GetResponseStream())
                        Dim mmsResponseData As String = sr.ReadToEnd()
                        Dim deserializeJsonObject As New JavaScriptSerializer()
                        Dim deserializedJsonObj As MsgResponseId = DirectCast(deserializeJsonObject.Deserialize(mmsResponseData, GetType(MsgResponseId)), MsgResponseId)
                        sendMessageSuccessResponse = deserializedJsonObj.Id.ToString()
                        sr.Close()
                    End Using
                Else
                    dataToSend += (Convert.ToString("--") & boundaryToSend) + "--" & vbCr & vbLf
                    Dim bytesToSend As Byte() = encoding.GetBytes(dataToSend)

                    Dim sizeToSend As Integer = bytesToSend.Length

                    Dim memBufToSend = New MemoryStream(New Byte(sizeToSend - 1) {}, 0, sizeToSend, True, True)
                    memBufToSend.Write(bytesToSend, 0, bytesToSend.Length)

                    Dim finalData As Byte() = memBufToSend.GetBuffer()
                    msgRequestObject.ContentLength = finalData.Length

                    postStream = msgRequestObject.GetRequestStream()
                    postStream.Write(finalData, 0, finalData.Length)

                    Dim mmsResponseObject1 As WebResponse = msgRequestObject.GetResponse()
                    Using sr As New StreamReader(mmsResponseObject1.GetResponseStream())
                        Dim mmsResponseData As String = sr.ReadToEnd()
                        Dim deserializeJsonObject As New JavaScriptSerializer()
                        Dim deserializedJsonObj As MsgResponseId = DirectCast(deserializeJsonObject.Deserialize(mmsResponseData, GetType(MsgResponseId)), MsgResponseId)
                        sendMessageSuccessResponse = deserializedJsonObj.Id.ToString()
                        sr.Close()
                    End Using
                End If
            Else
                Dim dataBytes As Byte() = encoding.GetBytes(String.Empty)
                Dim totalDataBytes As Byte() = encoding.GetBytes(String.Empty)
                Dim count As Integer = 0
                For Each attachment As String In attachments
                    Dim mmsFileName As String = Path.GetFileName(attachment.ToString())
                    Dim mmsFileExtension As String = Path.GetExtension(attachment.ToString())
                    Dim attachmentContentType As String = Me.GetContentTypeFromExtension(mmsFileExtension)
                    Dim imageFileStream As New FileStream(attachment.ToString(), FileMode.Open, FileAccess.Read)
                    Dim imageBinaryReader As New BinaryReader(imageFileStream)
                    Dim image As Byte() = imageBinaryReader.ReadBytes(CInt(imageFileStream.Length))
                    imageBinaryReader.Close()
                    imageFileStream.Close()
                    If count = 0 Then
                        dataToSend += (Convert.ToString(vbCr & vbLf & "--") & boundaryToSend) + vbCr & vbLf
                    Else
                        dataToSend = (Convert.ToString(vbCr & vbLf & "--") & boundaryToSend) + vbCr & vbLf
                    End If

                    dataToSend += "Content-Disposition: form-data; name=""" & mmsFileName & """; filename=" & mmsFileName & vbCr & vbLf
                    dataToSend += "Content-Type: " & attachmentContentType & "; charset=UTF-8" & vbCr & vbLf
                    dataToSend += "Content-ID:" & mmsFileName & vbCr & vbLf
                    dataToSend += "Content-Transfer-Encoding: binary " & vbCr & vbLf
                    dataToSend += "Content-Location: " & mmsFileName & vbCr & vbLf & vbCr & vbLf


                    Dim dataToSendByte As Byte() = encoding.GetBytes(dataToSend)
                    Dim dataToSendSize As Integer = dataToSendByte.Length + image.Length
                    Dim tempMemoryStream = New MemoryStream(New Byte(dataToSendSize - 1) {}, 0, dataToSendSize, True, True)
                    tempMemoryStream.Write(dataToSendByte, 0, dataToSendByte.Length)
                    tempMemoryStream.Write(image, 0, image.Length)
                    dataBytes = tempMemoryStream.GetBuffer()
                    If count = 0 Then
                        totalDataBytes = dataBytes
                    Else
                        Dim tempForTotalBytes As Byte() = totalDataBytes
                        Dim tempMemoryStreamAttach = Me.JoinTwoByteArrays(tempForTotalBytes, dataBytes)
                        totalDataBytes = tempMemoryStreamAttach.GetBuffer()
                    End If

                    count += 1
                Next

                Dim byteLastBoundary As Byte() = encoding.GetBytes((Convert.ToString(vbCr & vbLf & "--") & boundaryToSend) + "--" & vbCr & vbLf)
                Dim totalDataSize As Integer = totalDataBytes.Length + byteLastBoundary.Length
                Dim totalMemoryStream = New MemoryStream(New Byte(totalDataSize - 1) {}, 0, totalDataSize, True, True)
                totalMemoryStream.Write(totalDataBytes, 0, totalDataBytes.Length)
                totalMemoryStream.Write(byteLastBoundary, 0, byteLastBoundary.Length)
                Dim finalpostBytes As Byte() = totalMemoryStream.GetBuffer()

                msgRequestObject.ContentLength = finalpostBytes.Length

                postStream = msgRequestObject.GetRequestStream()
                postStream.Write(finalpostBytes, 0, finalpostBytes.Length)

                Dim mmsResponseObject1 As WebResponse = msgRequestObject.GetResponse()
                Using sr As New StreamReader(mmsResponseObject1.GetResponseStream())
                    Dim mmsResponseData As String = sr.ReadToEnd()
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim deserializedJsonObj As MsgResponseId = DirectCast(deserializeJsonObject.Deserialize(mmsResponseData, GetType(MsgResponseId)), MsgResponseId)
                    sendMessageSuccessResponse = deserializedJsonObj.Id.ToString()
                    sr.Close()
                End Using
            End If
        Catch we As WebException
            If we.Response IsNot Nothing Then
                Using stream As Stream = we.Response.GetResponseStream()
                    Dim reader As New StreamReader(stream)
                    sendMessageErrorResponse = reader.ReadToEnd()
                End Using
            End If
        Catch ex As Exception
            sendMessageErrorResponse = ex.ToString()
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Sums up two byte arrays.
    ''' </summary>
    ''' <param name="firstByteArray">First byte array</param>
    ''' <param name="secondByteArray">second byte array</param>
    ''' <returns>The memorystream"/> summed memory stream</returns>
    Private Function JoinTwoByteArrays(ByVal firstByteArray As Byte(), ByVal secondByteArray As Byte()) As MemoryStream
        Dim newSize As Integer = firstByteArray.Length + secondByteArray.Length
        Dim totalMemoryStream = New MemoryStream(New Byte(newSize - 1) {}, 0, newSize, True, True)
        totalMemoryStream.Write(firstByteArray, 0, firstByteArray.Length)
        totalMemoryStream.Write(secondByteArray, 0, secondByteArray.Length)
        Return totalMemoryStream
    End Function

    ''' <summary>
    ''' Gets the message content for MMS messages based on Message ID and Part Number
    ''' </summary>
    Private Sub GetMessageContentByIDnPartNumber(ByVal accTok As String, ByVal endP As String, ByVal messId As String, ByVal partNum As String)
        Try
            Dim mimRequestObject1 As HttpWebRequest = DirectCast(WebRequest.Create(Convert.ToString((Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/messages/")) & messId) + "/parts/") & partNum), HttpWebRequest)
            mimRequestObject1.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            mimRequestObject1.Method = "GET"
            mimRequestObject1.KeepAlive = True
            Dim offset As Integer = 0

            getContentResponseObject = mimRequestObject1.GetResponse()
            Dim remaining As Integer = Convert.ToInt32(getContentResponseObject.ContentLength)
            Using stream = getContentResponseObject.GetResponseStream()
                receivedBytes = New Byte(getContentResponseObject.ContentLength - 1) {}
                While remaining > 0
                    Dim read As Integer = stream.Read(receivedBytes, offset, remaining)
                    If read <= 0 Then
                        getMessageContentErrorResponse = [String].Format("End of stream reached with {0} bytes left to read", remaining)
                        Return
                    End If

                    remaining -= read
                    offset += read
                End While

                'imageData = Regex.Split(getContentResponseObject.ContentType.ToLower(), ";");
                'string[] ext = Regex.Split(imageData[0], "/");
                'fetchedImage.Src = "data:" + imageData[0] + ";base64," + Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None);

                getMessageContentSuccessResponse = "Success"
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                getMessageContentErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                getMessageContentErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            getMessageContentErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub

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

    Protected Sub SetRequestSessionVariables()
        Session("cs_rest_Address") = Address.Text
        Session("cs_rest_Message") = message.Text
        Session("cs_rest_Subject") = subject.Text
        Session("cs_rest_Group") = groupCheckBox.Checked.ToString()
        Session("cs_rest_Attachments") = attachment.Value
        Session("cs_rest_GetHeadercount") = "abc"
        Session("cs_rest_GetHeaderIndex") = "abc"
        Session("cs_rest_GetMessageId") = ""
        Session("cs_rest_GetMessagePart") = ""
        If notificationMms.Checked Then
            Session("cs_rest_GetNotificationConnectionDetailsQueue") = notificationMms.Value
        ElseIf notificationText.Checked Then
            Session("cs_rest_GetNotificationConnectionDetailsQueue") = notificationText.Value
        End If
        Session("cs_rest_deleteMessageId") = deleteMessageId.Text
        Session("cs_rest_updateMessageId") = updateMessageId.Text
    End Sub

    Protected Sub ResetRequestSessionVariables()
        Session("cs_rest_Address") = Nothing
        Session("cs_rest_Message") = Nothing
        Session("cs_rest_Subject") = Nothing
        Session("cs_rest_Group") = Nothing
        Session("cs_rest_Attachments") = Nothing
        Session("cs_rest_GetHeadercount") = Nothing
        Session("cs_rest_GetHeaderIndex") = Nothing
        Session("cs_rest_GetMessageId") = Nothing
        Session("cs_rest_GetMessagePart") = Nothing
        Session("cs_rest_GetNotificationConnectionDetailsQueue") = Nothing
        Session("cs_rest_deleteMessageId") = Nothing
        Session("cs_rest_updateMessageId") = Nothing
    End Sub

    Protected Sub RestoreRequestSessionVariables()
        Address.Text = Session("cs_rest_Address").ToString()
        message.Text = Session("cs_rest_Message").ToString()


        subject.Text = Session("cs_rest_Subject").ToString()
        groupCheckBox.Checked = Convert.ToBoolean(Session("cs_rest_Group").ToString())
        attachment.Value = Session("cs_rest_Attachments").ToString()
        'headerCountTextBox.Value = Session["cs_rest_GetHeadercount"].ToString();
        'indexCursorTextBox.Value = Session["cs_rest_GetHeaderIndex"].ToString();
        MessageId.Text = Session("cs_rest_GetMessageId").ToString()
        'PartNumber.Value = Session["cs_rest_GetMessagePart"].ToString();
        If String.Compare(Session("cs_rest_GetNotificationConnectionDetailsQueue").ToString(), notificationMms.Value) = 0 Then
            notificationMms.Checked = True
        ElseIf String.Compare(Session("cs_rest_GetNotificationConnectionDetailsQueue").ToString(), notificationText.Value) = 0 Then
            notificationText.Checked = True
        End If
        deleteMessageId.Text = Session("cs_rest_deleteMessageId").ToString()
        updateMessageId.Text = Session("cs_rest_updateMessageId").ToString()
    End Sub



    Protected Sub updateMessage_Click(ByVal sender As Object, ByVal e As EventArgs)
        showUpdateMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "updatemessage"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                updateMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If

        Me.updateMessage(Me.accessToken, Me.endPoint, updateMessageId.Text, read.Checked)
    End Sub

    Private Sub updateMessage()
        showUpdateMessage = "show"
        Me.updateMessage(Me.accessToken, Me.endPoint, updateMessageId.Text, read.Checked)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub updateMessage(ByVal accTok As String, ByVal endP As String, ByVal updateMessages As String, ByVal read As Boolean)
        Try
            Dim contextURL As String = String.Empty
            Dim messagesJSON As String = String.Empty
            Dim dataStream As Stream
            Dim message As Message
            Dim messageList As MessagesList
            Dim messages As New List(Of Message)()
            Dim serializeJsonObject As JavaScriptSerializer
            contextURL = String.Empty + endP & Convert.ToString("/myMessages/v2/messages")
            Dim messageIds As String() = updateMessages.Split(","c)
            For Each messageId As [String] In messageIds
                message = New Message()
                message.isUnread = Convert.ToBoolean(read)
                message.messageId = messageId
                messages.Add(message)
            Next
            messageList = New MessagesList()
            messageList.messages = messages
            serializeJsonObject = New JavaScriptSerializer()
            messagesJSON = serializeJsonObject.Serialize(messageList)
            Dim encoding As New UTF8Encoding()
            Dim msgBytes As Byte() = encoding.GetBytes(messagesJSON)
            Dim updateMessageWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            updateMessageWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            updateMessageWebRequest.Method = "PUT"
            updateMessageWebRequest.KeepAlive = True
            updateMessageWebRequest.ContentType = "application/json"
            dataStream = updateMessageWebRequest.GetRequestStream()
            dataStream.Write(msgBytes, 0, msgBytes.Length)
            dataStream.Close()

            Dim deleteMessageWebResponse As WebResponse = updateMessageWebRequest.GetResponse()
            Using stream = deleteMessageWebResponse.GetResponseStream()
                updateMessageSuccessResponse = "Success"
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                updateMessageErrorResponse = Convert.ToString((updateMessages & Convert.ToString("@")) + read + "@") & errorResponse
            Catch
                errorResponse = "Unable to get response"
                updateMessageErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            updateMessageErrorResponse = read + "@" + ex.Message
            Return
        End Try

    End Sub


    Protected Sub deleteMessage_Click(ByVal sender As Object, ByVal e As EventArgs)
        showDeleteMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "deletemessage"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                deleteMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.deleteMessage(Me.accessToken, Me.endPoint, deleteMessageId.Text)
    End Sub

    Private Sub deleteMessage()
        showDeleteMessage = "show"
        Me.deleteMessage(Me.accessToken, Me.endPoint, deleteMessageId.Text)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub deleteMessage(ByVal accTok As String, ByVal endP As String, ByVal deleteMessages As String)
        Try
            Dim contextURL As String = String.Empty
            If Not deleteMessages.Contains(",") Then
                contextURL = Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/messages/")) & deleteMessages
            Else
                contextURL = (String.Empty + endP & Convert.ToString("/myMessages/v2/messages?messageIds=")) + System.Web.HttpUtility.UrlEncode(deleteMessages)
            End If
            Dim deleteMessageWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            deleteMessageWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            deleteMessageWebRequest.Method = "DELETE"
            deleteMessageWebRequest.KeepAlive = True
            Dim deleteMessageWebResponse As WebResponse = deleteMessageWebRequest.GetResponse()
            Using stream = deleteMessageWebResponse.GetResponseStream()
                deleteMessageSuccessResponse = "Success"
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                deleteMessageErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                deleteMessageErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            deleteMessageErrorResponse = ex.Message
            Return
        End Try

    End Sub


    Protected Sub getMessageList_Click(ByVal sender As Object, ByVal e As EventArgs)
        showGetMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "getmessagelist"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                deleteMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.filters = ""
        If CheckBox1.Checked = True Then
            If Me.filters.CompareTo(String.Empty) = 0 Then
                Me.filters = "isFavorite=true&"
            Else
                Me.filters = Me.filters & Convert.ToString("&isFavorite=true") + "&"
            End If
        End If
        If CheckBox2.Checked = True Then
            If Me.filters.CompareTo(String.Empty) = 0 Then
                Me.filters = "isUnread=true&"
            Else
                Me.filters = (Me.filters & Convert.ToString("&isUnread=true")) + "&"
            End If
        End If
        If CheckBox3.Checked = True Then
            If Me.filters.CompareTo(String.Empty) = 0 Then
                Me.filters = "isIncoming=true" + "&"
            Else
                Me.filters = (Me.filters & Convert.ToString("&isIncoming=true")) + "&"
            End If
        End If
        If Not String.IsNullOrEmpty(FilterKeyword.Text) Then
            If Me.filters.CompareTo(String.Empty) = 0 Then
                Me.filters = "keyword=" + FilterKeyword.Text.ToString() + "&"
            Else
                Me.filters = (Me.filters & Convert.ToString("&keyword=")) + FilterKeyword.Text.ToString() + "&"
            End If
        End If
        Me.getMessageList(Me.accessToken, Me.endPoint, Me.filters)
    End Sub

    Private Sub getMessageList()
        showDeleteMessage = "show"
        Me.getMessageList(Me.accessToken, Me.endPoint, Me.filters)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub getMessageList(ByVal accTok As String, ByVal endP As String, ByVal filters As String)
        Try
            Dim contextURL As String = String.Empty
            contextURL = (Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/messages?")) & filters) + "limit=5&offset=1"

            Dim getMessageListWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            getMessageListWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            getMessageListWebRequest.Method = "GET"
            getMessageListWebRequest.KeepAlive = True
            Dim getMessageListWebResponse As WebResponse = getMessageListWebRequest.GetResponse()
            Using stream = getMessageListWebResponse.GetResponseStream()
                Dim sr As New StreamReader(stream)
                Dim csGetMessageListDetailsData As String = sr.ReadToEnd()

                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As csGetMessageListDetails = DirectCast(deserializeJsonObject.Deserialize(csGetMessageListDetailsData, GetType(csGetMessageListDetails)), csGetMessageListDetails)

                If deserializedJsonObj IsNot Nothing Then
                    getMessageListSuccessResponse = "Success"
                    csGetMessageListDetailsResponse = deserializedJsonObj.messageList
                Else
                    getMessageListErrorResponse = "No response from server"
                End If


                sr.Close()
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                getMessageListErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                getMessageListErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            getMessageListErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Protected Sub getMessage_Click(ByVal sender As Object, ByVal e As EventArgs)
        showGetMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "getmessage"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                getMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.getMessage(Me.accessToken, Me.endPoint, MessageId.Text)
    End Sub

    Private Sub getMessage()
        showGetMessage = "show"
        Me.getMessage(Me.accessToken, Me.endPoint, MessageId.Text)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub getMessage(ByVal accTok As String, ByVal endP As String, ByVal getMessage1 As String)
        Try
            Dim contextURL As String = String.Empty
            contextURL = Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/messages/")) & getMessage1

            Dim getMessageWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            getMessageWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            getMessageWebRequest.Method = "GET"
            getMessageWebRequest.KeepAlive = True
            getMessageWebRequest.Accept = "application/json"
            Dim getMessageWebResponse As WebResponse = getMessageWebRequest.GetResponse()
            Using stream As New StreamReader(getMessageWebResponse.GetResponseStream())
                Dim getMessageData As String = stream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As csGetMessageDetails = DirectCast(deserializeJsonObject.Deserialize(getMessageData, GetType(csGetMessageDetails)), csGetMessageDetails)

                If deserializedJsonObj IsNot Nothing Then
                    getMessageSuccessResponse = getMessageData & Convert.ToString(":Success")
                    getMessageDetailsResponse = deserializedJsonObj.message
                Else
                    getMessageErrorResponse = "No response from server"
                End If

                stream.Close()
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                getMessageErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                getMessageErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            getMessageErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Protected Sub getDelta_Click(ByVal sender As Object, ByVal e As EventArgs)
        showGetMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "deltamessage"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                deltaErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.getDelta(Me.accessToken, Me.endPoint, MessageIdForDelta.Text)
    End Sub

    Private Sub getDelta()
        'showDeltaMessage = "show";
        Me.getDelta(Me.accessToken, Me.endPoint, MessageIdForDelta.Text)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub getDelta(ByVal accTok As String, ByVal endP As String, ByVal delta As String)
        Try
            Dim contextURL As String = String.Empty
            contextURL = Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/delta?state=")) & delta
            Dim deltaWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            deltaWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            deltaWebRequest.Method = "GET"
            deltaWebRequest.KeepAlive = True
            Dim deltaWebResponse As WebResponse = deltaWebRequest.GetResponse()
            Using stream = deltaWebResponse.GetResponseStream()
                Dim sr As New StreamReader(stream)
                Dim deltaMessageData As String = sr.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As csGetDeltaDetails = DirectCast(deserializeJsonObject.Deserialize(deltaMessageData, GetType(csGetDeltaDetails)), csGetDeltaDetails)
                If deserializedJsonObj IsNot Nothing Then
                    deltaSuccessResponse = deltaMessageData & Convert.ToString(":Success")
                    csDeltaResponse = deserializedJsonObj.deltaResponse
                Else
                    deltaErrorResponse = "No response from server"
                End If

                stream.Close()
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                deltaErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                deltaErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            deltaErrorResponse = ex.Message
            Return
        End Try

    End Sub


    Protected Sub getMessageIndex_Click(ByVal sender As Object, ByVal e As EventArgs)
        showGetMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "messageindex"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                messageIndexErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.getMessageIndex(Me.accessToken, Me.endPoint)
    End Sub

    Private Sub getMessageIndex()
        'showDeltaMessage = "show";
        Me.getMessageIndex(Me.accessToken, Me.endPoint)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub getMessageIndex(ByVal accTok As String, ByVal endP As String)
        Try
            Dim contextURL As String = String.Empty
            contextURL = String.Empty + endP & Convert.ToString("/myMessages/v2/messages/index/info")
            Dim msgIndxWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(contextURL), HttpWebRequest)
            msgIndxWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            msgIndxWebRequest.Method = "GET"
            msgIndxWebRequest.KeepAlive = True
            Dim msgIndxWebResponse As WebResponse = msgIndxWebRequest.GetResponse()
            Using stream As New StreamReader(msgIndxWebResponse.GetResponseStream())
                Dim getMessageIndexData As String = stream.ReadToEnd()
                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As csMessageIndexInfo = DirectCast(deserializeJsonObject.Deserialize(getMessageIndexData, GetType(csMessageIndexInfo)), csMessageIndexInfo)
                If deserializedJsonObj IsNot Nothing Then

                    messageIndexSuccessResponse = getMessageIndexData & Convert.ToString(":Success")
                    getMessageIndexInfoResponse = deserializedJsonObj.messageIndexInfo
                Else
                    messageIndexErrorResponse = "No response from server"
                End If


                stream.Close()
            End Using
            messageIndexSuccessResponse = ":Success"
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                messageIndexErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                messageIndexErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            messageIndexErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Protected Sub getNotificationConnectionDetails_Click(ByVal sender As Object, ByVal e As EventArgs)
        showGetNotificationConnectionDetails = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "getnotificationconnectiondetails"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                getNotificationConnectionDetailsErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Dim queueType As String = String.Empty
        If notificationMms.Checked Then
            queueType = notificationMms.Value
        ElseIf notificationText.Checked Then
            queueType = notificationText.Value
        End If
        Me.getNotificationConnectionDetails(Me.accessToken, Me.endPoint, queueType)
    End Sub

    Protected Sub getNotificationConnectionDetails()
        showGetNotificationConnectionDetails = "show"
        Dim queueType As String = String.Empty
        If notificationMms.Checked Then
            queueType = notificationMms.Value
        ElseIf notificationText.Checked Then
            queueType = notificationText.Value
        End If
        Me.getNotificationConnectionDetails(Me.accessToken, Me.endPoint, queueType)
    End Sub

    Private Sub getNotificationConnectionDetails(ByVal accTok As String, ByVal endP As String, ByVal queues As String)
        Try
            Dim getNotificationConnectionDetailsWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(Convert.ToString(String.Empty + endP & Convert.ToString("/myMessages/v2/notificationConnectionDetails?queues=")) & queues), HttpWebRequest)
            getNotificationConnectionDetailsWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            getNotificationConnectionDetailsWebRequest.Method = "GET"
            getNotificationConnectionDetailsWebRequest.KeepAlive = True
            getNotificationConnectionDetailsWebRequest.Accept = "application/json"
            Dim getNotificationConnectionDetailsWebResponse As WebResponse = getNotificationConnectionDetailsWebRequest.GetResponse()
            Using sr As New StreamReader(getNotificationConnectionDetailsWebResponse.GetResponseStream())
                Dim getNotificationConnectionDetailsData As String = sr.ReadToEnd()

                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As csGetNotificationConnectionDetails = DirectCast(deserializeJsonObject.Deserialize(getNotificationConnectionDetailsData, GetType(csGetNotificationConnectionDetails)), csGetNotificationConnectionDetails)

                If deserializedJsonObj IsNot Nothing Then
                    getNotificationConnectionDetailsSuccessResponse = "SUCCESS"
                    getNotificationConnectionDetailsResponse = deserializedJsonObj.notificationConnectionDetails
                Else
                    getNotificationConnectionDetailsErrorResponse = "No response from server"
                End If

                sr.Close()
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                getNotificationConnectionDetailsErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                getNotificationConnectionDetailsErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            getNotificationConnectionDetailsErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Protected Sub createMessageIndex_Click(ByVal sender As Object, ByVal e As EventArgs)
        showCreateMessageIndex = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "createmessageindex"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                createMessageIndexErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.createMessageIndex(Me.accessToken, Me.endPoint)
    End Sub

    Private Sub createMessageIndex()
        showCreateMessageIndex = "show"
        Me.createMessageIndex(Me.accessToken, Me.endPoint)
    End Sub

    ''' <summary>
    ''' send the create message index request to api platform.
    ''' </summary>
    Private Sub createMessageIndex(ByVal accTok As String, ByVal endP As String)
        Try
            Dim createMessageIndexWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty + endP & Convert.ToString("/myMessages/v2/messages/index")), HttpWebRequest)
            createMessageIndexWebRequest.Headers.Add("Authorization", Convert.ToString("Bearer ") & accTok)
            createMessageIndexWebRequest.Method = "POST"
            createMessageIndexWebRequest.KeepAlive = True
            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes("TEST")
            createMessageIndexWebRequest.ContentLength = postBytes.Length
            Dim postStream As Stream = createMessageIndexWebRequest.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()
            Dim createMessageIndexWebResponse As WebResponse = createMessageIndexWebRequest.GetResponse()
            Using stream = createMessageIndexWebResponse.GetResponseStream()
                createMessageIndexSuccessResponse = "Success"
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                createMessageIndexErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                createMessageIndexErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            createMessageIndexErrorResponse = ex.Message
            Return
        End Try

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        showSendMsg = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "sendmessasge"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                sendMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If
        Me.SendMessageRequest()
    End Sub

    Protected Sub SendMessageRequest()
        Me.IsValidAddress()
        Dim attachFile As String = Me.AttachmentFilesDir & attachment.Value.ToString()
        Dim attachmentList As New ArrayList()
        If String.Compare(attachment.Value.ToString().ToLower(), "none") <> 0 Then
            attachmentList.Add(attachFile)
        End If
        Dim accessToken As String = Me.accessToken
        Dim endpoint As String = Me.endPoint
        Me.SendMessageRequest(accessToken, endpoint, subject.Text, message.Text, groupCheckBox.Checked.ToString().ToLower(), attachmentList)
    End Sub

    Protected Sub GetMessageContentByIDnPartNumber(ByVal sender As Object, ByVal e As EventArgs)
        showGetMessage = "true"
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("cs_rest_ServiceRequest") = "getmessagecontent"
            Session("cs_rest_appState") = "GetToken"
            Me.GetAuthCode()
        ElseIf tokentResult.CompareTo("REFRESH_TOKEN") = 0 Then
            If Me.GetAccessToken(AccessTokenType.Refresh_Token) = False Then
                sendMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

        If Me.accessToken Is Nothing OrElse Me.accessToken.Length <= 0 Then
            Return
        End If

        GetMessageContentByIDnPartNumber(Me.accessToken, Me.endPoint, MessageIdForContent.Text, PartNumberForContent.Text)
    End Sub

    Protected Sub GetMessageContentByIDnPartNumber()
        Me.GetMessageContentByIDnPartNumber(Me.accessToken, Me.endPoint, MessageIdForContent.Text, PartNumberForContent.Text)
    End Sub
    ''' <summary>
    ''' Validates the given addresses based on following conditions
    ''' 1. Group messages should not allow short codes
    ''' 2. Short codes should be 3-8 digits in length
    ''' 3. Valid Email Address
    ''' 4. Group message must contain more than one address
    ''' 5. Valid Phone number
    ''' </summary>
    ''' <returns>true/false; true - if address specified met the validation criteria, else false</returns>
    Private Function IsValidAddress() As Boolean
        Dim phonenumbers As String = String.Empty

        Dim isValid As Boolean = True
        If String.IsNullOrEmpty(Address.Text) Then
            sendMessageErrorResponse = "Address field cannot be blank."
            Return False
        End If

        Dim addresses As String() = Address.Text.Trim().Split(","c)

        If addresses.Length > Me.maxAddresses Then
            sendMessageErrorResponse = "Message cannot be delivered to more than 10 receipients."
            Return False
        End If

        If groupCheckBox.Checked AndAlso addresses.Length < 1 Then
            sendMessageErrorResponse = "Specify more than one address for Group message."
            Return False
        End If

        For Each addressraw As String In addresses
            Dim address__1 As String = addressraw.Trim()
            If String.IsNullOrEmpty(address__1) Then
                Exit For
            End If

            If address__1.Length < 3 Then
                sendMessageErrorResponse = "Invalid address specified."
                Return False
            End If

            ' Verify if short codes are present in address
            If Not address__1.StartsWith("short") AndAlso (address__1.Length > 2 AndAlso address__1.Length < 9) Then
                If groupCheckBox.Checked Then
                    sendMessageErrorResponse = "Group Message with short codes is not allowed."
                    Return False
                End If

                Me.addressList.Add(address__1)
                Me.phoneNumbersParameter = (Me.phoneNumbersParameter & Convert.ToString("addresses=short:")) + Server.UrlEncode(address__1.ToString()) + "&"
            End If

            If address__1.StartsWith("short") Then
                If groupCheckBox.Checked Then
                    sendMessageErrorResponse = "Group Message with short codes is not allowed."
                    Return False
                End If

                Dim regex As System.Text.RegularExpressions.Regex = New Regex("^[0-9]*$")
                If Not regex.IsMatch(address__1.Substring(6)) Then
                    sendMessageErrorResponse = "Invalid short code specified."
                    Return False
                End If

                Me.addressList.Add(address__1)
                Me.phoneNumbersParameter = (Me.phoneNumbersParameter & Convert.ToString("addresses=")) + Server.UrlEncode(address__1.ToString()) + "&"
            ElseIf address__1.Contains("@") Then
                isValid = Me.IsValidEmail(address__1)
                If isValid = False Then
                    sendMessageErrorResponse = "Specified Email Address is invalid."
                    Return False
                Else
                    Me.addressList.Add(address__1)
                    Me.phoneNumbersParameter = (Me.phoneNumbersParameter & Convert.ToString("addresses=")) + Server.UrlEncode(address__1.ToString()) + "&"
                End If
            Else
                If Me.IsValidMISDN(address__1) = True Then
                    If address__1.StartsWith("tel:") Then
                        phonenumbers = address__1.Replace("-", String.Empty)
                        Me.phoneNumbersParameter = (Me.phoneNumbersParameter & Convert.ToString("addresses=")) + Server.UrlEncode(phonenumbers.ToString()) + "&"
                    Else
                        phonenumbers = address__1.Replace("-", String.Empty)
                        Me.phoneNumbersParameter = (Me.phoneNumbersParameter & Convert.ToString("addresses=")) + Server.UrlEncode("tel:" + phonenumbers.ToString()) + "&"
                    End If

                    Me.addressList.Add(address__1)
                End If
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' Validate given string for MSISDN
    ''' </summary>
    ''' <param name="number">Phone number to be validated</param>
    ''' <returns>true/false; true - if valid MSISDN, else false</returns>
    Private Function IsValidMISDN(ByVal number As String) As Boolean
        Dim smsAddressInput As String = number
        Dim tryParseResult As Long = 0
        Dim smsAddressFormatted As String
        Dim phoneStringPattern As String = "^\d{3}-\d{3}-\d{4}$"
        If Regex.IsMatch(smsAddressInput, phoneStringPattern) Then
            smsAddressFormatted = smsAddressInput.Replace("-", String.Empty)
        Else
            smsAddressFormatted = smsAddressInput
        End If

        If smsAddressFormatted.Length = 16 AndAlso smsAddressFormatted.StartsWith("tel:+1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(6, 10)
        ElseIf smsAddressFormatted.Length = 15 AndAlso smsAddressFormatted.StartsWith("tel:1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(5, 10)
        ElseIf smsAddressFormatted.Length = 14 AndAlso smsAddressFormatted.StartsWith("tel:") Then
            smsAddressFormatted = smsAddressFormatted.Substring(4, 10)
        ElseIf smsAddressFormatted.Length = 12 AndAlso smsAddressFormatted.StartsWith("+1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(2, 10)
        ElseIf smsAddressFormatted.Length = 11 AndAlso smsAddressFormatted.StartsWith("1") Then
            smsAddressFormatted = smsAddressFormatted.Substring(1, 10)
        End If

        If (smsAddressFormatted.Length <> 10) OrElse (Not Long.TryParse(smsAddressFormatted, tryParseResult)) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Validates given mail ID for standard mail format
    ''' </summary>
    ''' <param name="emailID">Mail Id to be validated</param>
    ''' <returns> true/false; true - if valid email id, else false</returns>
    Private Function IsValidEmail(ByVal emailID As String) As Boolean
        Dim strRegex As String = "^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + "\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + ".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
        Dim re As New Regex(strRegex)
        If re.IsMatch(emailID) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Validates a given string for digits
    ''' </summary>
    ''' <param name="address">string to be validated</param>
    ''' <returns>true/false; true - if passed string has all digits, else false</returns>
    Private Function IsNumber(ByVal address As String) As Boolean
        Dim isValid As Boolean = False
        Dim regex As New Regex("^[0-9]*$")
        If regex.IsMatch(address) Then
            isValid = True
        End If

        Return isValid
    End Function
End Class

''' <summary>
''' Access Token Data Structure
''' </summary>
Public Class AccessTokenResponse
    ''' <summary>
    ''' Gets or sets Access Token ID
    ''' </summary>
    Public Property access_token() As String
        Get
            Return m_access_token
        End Get
        Set(ByVal value As String)
            m_access_token = Value
        End Set
    End Property
    Private m_access_token As String

    ''' <summary>
    ''' Gets or sets Refresh Token ID
    ''' </summary>
    Public Property refresh_token() As String
        Get
            Return m_refresh_token
        End Get
        Set(ByVal value As String)
            m_refresh_token = Value
        End Set
    End Property
    Private m_refresh_token As String

    ''' <summary>
    ''' Gets or sets Expires in milli seconds
    ''' </summary>
    Public Property expires_in() As String
        Get
            Return m_expires_in
        End Get
        Set(ByVal value As String)
            m_expires_in = Value
        End Set
    End Property
    Private m_expires_in As String
End Class

Public Class MessageObject
    Public Property message() As Message
        Get
            Return m_message
        End Get
        Set(ByVal value As Message)
            m_message = Value
        End Set
    End Property
    Private m_message As Message
End Class

''' <summary>
''' Update message list datastructure
''' </summary>
Public Class MessagesList
    Public Property messages() As List(Of Message)
        Get
            Return m_messages
        End Get
        Set(ByVal value As List(Of Message))
            m_messages = Value
        End Set
    End Property
    Private m_messages As List(Of Message)
End Class

Public Class MmsContent
    Public Property contentType() As String
        Get
            Return m_contentType
        End Get
        Set(ByVal value As String)
            m_contentType = Value
        End Set
    End Property
    Private m_contentType As String
    Public Property contentName() As String
        Get
            Return m_contentName
        End Get
        Set(ByVal value As String)
            m_contentName = Value
        End Set
    End Property
    Private m_contentName As String
    Public Property contentUrl() As String
        Get
            Return m_contentUrl
        End Get
        Set(ByVal value As String)
            m_contentUrl = Value
        End Set
    End Property
    Private m_contentUrl As String
    Public Property type() As String
        Get
            Return m_type
        End Get
        Set(ByVal value As String)
            m_type = Value
        End Set
    End Property
    Private m_type As String
End Class

''' <summary>
''' Response from IMMN api
''' </summary>
Public Class MsgResponseId
    ''' <summary>
    ''' Gets or sets Message ID
    ''' </summary>
    Public Property Id() As String
        Get
            Return m_Id
        End Get
        Set(ByVal value As String)
            m_Id = Value
        End Set
    End Property
    Private m_Id As String
End Class

''' <summary>
''' Access Token Types
''' </summary>
Public Enum AccessTokenType
    ''' <summary>
    ''' Access Token Type is based on Authorization Code
    ''' </summary>
    Authorization_Code

    ''' <summary>
    ''' Access Token Type is based on Refresh Token
    ''' </summary>
    Refresh_Token
End Enum

Public Class csGetNotificationConnectionDetails
    Public Property notificationConnectionDetails() As csNotificationConnectionDetails
        Get
            Return m_notificationConnectionDetails
        End Get
        Set(ByVal value As csNotificationConnectionDetails)
            m_notificationConnectionDetails = Value
        End Set
    End Property
    Private m_notificationConnectionDetails As csNotificationConnectionDetails
End Class

Public Class csNotificationConnectionDetails
    Public Property username() As String
        Get
            Return m_username
        End Get
        Set(ByVal value As String)
            m_username = Value
        End Set
    End Property
    Private m_username As String
    Public Property password() As String
        Get
            Return m_password
        End Get
        Set(ByVal value As String)
            m_password = Value
        End Set
    End Property
    Private m_password As String
    Public Property httpsUrl() As String
        Get
            Return m_httpsUrl
        End Get
        Set(ByVal value As String)
            m_httpsUrl = Value
        End Set
    End Property
    Private m_httpsUrl As String
    Public Property wssUrl() As String
        Get
            Return m_wssUrl
        End Get
        Set(ByVal value As String)
            m_wssUrl = Value
        End Set
    End Property
    Private m_wssUrl As String
    Public Property queues() As csqueues
        Get
            Return m_queues
        End Get
        Set(ByVal value As csqueues)
            m_queues = Value
        End Set
    End Property
    Private m_queues As csqueues
End Class

Public Class csqueues
    Public Property text() As String
        Get
            Return m_text
        End Get
        Set(ByVal value As String)
            m_text = Value
        End Set
    End Property
    Private m_text As String
    Public Property mms() As String
        Get
            Return m_mms
        End Get
        Set(ByVal value As String)
            m_mms = Value
        End Set
    End Property
    Private m_mms As String
End Class

Public Class csGetMessageContentDetails
    Public Property MessageContentDetails() As csMessageContentDetails
        Get
            Return m_MessageContentDetails
        End Get
        Set(ByVal value As csMessageContentDetails)
            m_MessageContentDetails = Value
        End Set
    End Property
    Private m_MessageContentDetails As csMessageContentDetails
End Class

Public Class csMessageContentDetails
    Public Property contenttype() As String
        Get
            Return m_contenttype
        End Get
        Set(ByVal value As String)
            m_contenttype = Value
        End Set
    End Property
    Private m_contenttype As String
    Public Property textplain() As String
        Get
            Return m_textplain
        End Get
        Set(ByVal value As String)
            m_textplain = Value
        End Set
    End Property
    Private m_textplain As String
    Public Property imgjpg() As String
        Get
            Return m_imgjpg
        End Get
        Set(ByVal value As String)
            m_imgjpg = Value
        End Set
    End Property
    Private m_imgjpg As String
End Class

Public Class csGetMessageDetails
    Public Property message() As Message
        Get
            Return m_message
        End Get
        Set(ByVal value As Message)
            m_message = Value
        End Set
    End Property
    Private m_message As Message
End Class

Public Class csGetMessageListDetails
    Public Property messageList() As MessageList
        Get
            Return m_messageList
        End Get
        Set(ByVal value As MessageList)
            m_messageList = Value
        End Set
    End Property
    Private m_messageList As MessageList
End Class

Public Class From
    Public Property value() As String
        Get
            Return m_value
        End Get
        Set(ByVal value As String)
            m_value = Value
        End Set
    End Property
    Private m_value As String
End Class

Public Class Recipient
    Public Property value() As String
        Get
            Return m_value
        End Get
        Set(ByVal value As String)
            m_value = Value
        End Set
    End Property
    Private m_value As String
End Class

Public Class SegmentationDetails

    Public Property segmentationMsgRefNumber() As Integer
        Get
            Return m_segmentationMsgRefNumber
        End Get
        Set(ByVal value As Integer)
            m_segmentationMsgRefNumber = Value
        End Set
    End Property
    Private m_segmentationMsgRefNumber As Integer

    Public Property totalNumberOfParts() As Integer
        Get
            Return m_totalNumberOfParts
        End Get
        Set(ByVal value As Integer)
            m_totalNumberOfParts = Value
        End Set
    End Property
    Private m_totalNumberOfParts As Integer

    Public Property thisPartNumber() As Integer
        Get
            Return m_thisPartNumber
        End Get
        Set(ByVal value As Integer)
            m_thisPartNumber = Value
        End Set
    End Property
    Private m_thisPartNumber As Integer

End Class

Public Class TypeMetaData
    Public Property isSegmented() As Boolean
        Get
            Return m_isSegmented
        End Get
        Set(ByVal value As Boolean)
            m_isSegmented = Value
        End Set
    End Property
    Private m_isSegmented As Boolean
    Public Property segmentationDetails() As SegmentationDetails
        Get
            Return m_segmentationDetails
        End Get
        Set(ByVal value As SegmentationDetails)
            m_segmentationDetails = Value
        End Set
    End Property
    Private m_segmentationDetails As SegmentationDetails
    Public Property subject() As String
        Get
            Return m_subject
        End Get
        Set(ByVal value As String)
            m_subject = Value
        End Set
    End Property
    Private m_subject As String
End Class

Public Class Message

    Public Property messageId() As String
        Get
            Return m_messageId
        End Get
        Set(ByVal value As String)
            m_messageId = Value
        End Set
    End Property
    Private m_messageId As String
    Public Property from() As From
        Get
            Return m_from
        End Get
        Set(ByVal value As From)
            m_from = Value
        End Set
    End Property
    Private m_from As From
    Public Property recipients() As List(Of Recipient)
        Get
            Return m_recipients
        End Get
        Set(ByVal value As List(Of Recipient))
            m_recipients = Value
        End Set
    End Property
    Private m_recipients As List(Of Recipient)
    Public Property timeStamp() As String
        Get
            Return m_timeStamp
        End Get
        Set(ByVal value As String)
            m_timeStamp = Value
        End Set
    End Property
    Private m_timeStamp As String
    Public Property isFavorite() As [Boolean]
        Get
            Return m_isFavorite
        End Get
        Set(ByVal value As [Boolean])
            m_isFavorite = Value
        End Set
    End Property
    Private m_isFavorite As [Boolean]
    Public Property isUnread() As [Boolean]
        Get
            Return m_isUnread
        End Get
        Set(ByVal value As [Boolean])
            m_isUnread = Value
        End Set
    End Property
    Private m_isUnread As [Boolean]
    Public Property type() As String
        Get
            Return m_type
        End Get
        Set(ByVal value As String)
            m_type = Value
        End Set
    End Property
    Private m_type As String
    Public Property typeMetaData() As TypeMetaData
        Get
            Return m_typeMetaData
        End Get
        Set(ByVal value As TypeMetaData)
            m_typeMetaData = Value
        End Set
    End Property
    Private m_typeMetaData As TypeMetaData
    Public Property isIncoming() As String
        Get
            Return m_isIncoming
        End Get
        Set(ByVal value As String)
            m_isIncoming = Value
        End Set
    End Property
    Private m_isIncoming As String
    Public Property mmsContent() As List(Of MmsContent)
        Get
            Return m_mmsContent
        End Get
        Set(ByVal value As List(Of MmsContent))
            m_mmsContent = Value
        End Set
    End Property
    Private m_mmsContent As List(Of MmsContent)
    Public Property text() As String
        Get
            Return m_text
        End Get
        Set(ByVal value As String)
            m_text = Value
        End Set
    End Property
    Private m_text As String
    Public Property subject() As String
        Get
            Return m_subject
        End Get
        Set(ByVal value As String)
            m_subject = Value
        End Set
    End Property
    Private m_subject As String
End Class

Public Class MessageList
    Public Property messages() As List(Of Message)
        Get
            Return m_messages
        End Get
        Set(ByVal value As List(Of Message))
            m_messages = Value
        End Set
    End Property
    Private m_messages As List(Of Message)
    Public Property offset() As Integer
        Get
            Return m_offset
        End Get
        Set(ByVal value As Integer)
            m_offset = Value
        End Set
    End Property
    Private m_offset As Integer
    Public Property limit() As Integer
        Get
            Return m_limit
        End Get
        Set(ByVal value As Integer)
            m_limit = Value
        End Set
    End Property
    Private m_limit As Integer
    Public Property total() As Integer
        Get
            Return m_total
        End Get
        Set(ByVal value As Integer)
            m_total = Value
        End Set
    End Property
    Private m_total As Integer
    Public Property state() As String
        Get
            Return m_state
        End Get
        Set(ByVal value As String)
            m_state = Value
        End Set
    End Property
    Private m_state As String
    Public Property cacheStatus() As String
        Get
            Return m_cacheStatus
        End Get
        Set(ByVal value As String)
            m_cacheStatus = Value
        End Set
    End Property
    Private m_cacheStatus As String
    Public Property failedMessages() As List(Of String)
        Get
            Return m_failedMessages
        End Get
        Set(ByVal value As List(Of String))
            m_failedMessages = Value
        End Set
    End Property
    Private m_failedMessages As List(Of String)
End Class

Public Class csGetDeltaDetails
    Public Property deltaResponse() As DeltaResponse
        Get
            Return m_deltaResponse
        End Get
        Set(ByVal value As DeltaResponse)
            m_deltaResponse = Value
        End Set
    End Property
    Private m_deltaResponse As DeltaResponse
End Class

Public Class Delta
    Public Property adds() As List(Of Message)
        Get
            Return m_adds
        End Get
        Set(ByVal value As List(Of Message))
            m_adds = Value
        End Set
    End Property
    Private m_adds As List(Of Message)
    Public Property deletes() As List(Of Message)
        Get
            Return m_deletes
        End Get
        Set(ByVal value As List(Of Message))
            m_deletes = Value
        End Set
    End Property
    Private m_deletes As List(Of Message)
    Public Property type() As String
        Get
            Return m_type
        End Get
        Set(ByVal value As String)
            m_type = Value
        End Set
    End Property
    Private m_type As String
    Public Property updates() As List(Of Message)
        Get
            Return m_updates
        End Get
        Set(ByVal value As List(Of Message))
            m_updates = Value
        End Set
    End Property
    Private m_updates As List(Of Message)
End Class

Public Class DeltaResponse
    Public Property state() As String
        Get
            Return m_state
        End Get
        Set(ByVal value As String)
            m_state = Value
        End Set
    End Property
    Private m_state As String
    Public Property delta() As List(Of Delta)
        Get
            Return m_delta
        End Get
        Set(ByVal value As List(Of Delta))
            m_delta = Value
        End Set
    End Property
    Private m_delta As List(Of Delta)
End Class

Public Class MessageIndexInfo
    Public Property status() As String
        Get
            Return m_status
        End Get
        Set(ByVal value As String)
            m_status = Value
        End Set
    End Property
    Private m_status As String
    Public Property state() As String
        Get
            Return m_state
        End Get
        Set(ByVal value As String)
            m_state = Value
        End Set
    End Property
    Private m_state As String
    Public Property messageCount() As Integer
        Get
            Return m_messageCount
        End Get
        Set(ByVal value As Integer)
            m_messageCount = Value
        End Set
    End Property
    Private m_messageCount As Integer
End Class

Public Class csMessageIndexInfo
    Public Property messageIndexInfo() As MessageIndexInfo
        Get
            Return m_messageIndexInfo
        End Get
        Set(ByVal value As MessageIndexInfo)
            m_messageIndexInfo = Value
        End Set
    End Property
    Private m_messageIndexInfo As MessageIndexInfo
End Class
