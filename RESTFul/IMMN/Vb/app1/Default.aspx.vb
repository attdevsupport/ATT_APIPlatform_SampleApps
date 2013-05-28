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
Partial Public Class Mobo_App1
    Inherits System.Web.UI.Page
#Region "Instance variables"

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

#End Region
    Public attachments As List(Of String) = Nothing
    Public sendMessageSuccessResponse As String = String.Empty
    Public sendMessageErrorResponse As String = String.Empty
    Public getHeadersErrorResponse As String = String.Empty
    Public getHeadersSuccessResponse As String = String.Empty
    Public getMessageSuccessResponse As String = String.Empty
    Public getMessageErrorResponse As String = String.Empty
    Public content_result As String = String.Empty
    Public receivedBytes As Byte() = Nothing
    Public getContentResponseObject As WebResponse = Nothing
    Public imageData As String() = Nothing
    Public messageList As MessageHeaderList

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)

        Me.ReadConfigFile()

        If (Session("vb_rest_appState") Is "GetToken") AndAlso (Request("Code") IsNot Nothing) Then
            Me.authCode = Request("code").ToString()
            If Me.GetAccessToken(AccessTokenType.Authorization_Code) = True Then
                RestoreRequestSessionVariables()
                ResetRequestSessionVariables()
                If String.Compare(Session("vb_rest_ServiceRequest").ToString(), "sendmessasge") = 0 Then
                    Me.SendMessageRequest()
                ElseIf String.Compare(Session("vb_rest_ServiceRequest").ToString(), "getmessageheader") = 0 Then
                    Me.GetMsgHeaders()
                ElseIf String.Compare(Session("vb_rest_ServiceRequest").ToString(), "getmessagecontent") = 0 Then
                    Me.GetMessageContentByIDnPartNumber()

                End If
            Else
                sendMessageErrorResponse = "Failed to get Access token"
                Me.ResetTokenSessionVariables()
                Me.ResetTokenVariables()
                Return
            End If
        End If

    End Sub

#Region "Access Token functions"

    ''' <summary>
    ''' This function resets access token related session variable to null 
    ''' </summary>
    Private Sub ResetTokenSessionVariables()
        Session("vb_rest_AccessToken") = Nothing
        Session("vb_rest_AccessTokenExpirtyTime") = Nothing
        Session("vb_rest_RefreshToken") = Nothing
        Session("vb_rest_RefreshTokenExpiryTime") = Nothing
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
            Response.Redirect(String.Empty & Me.endPoint & "/oauth/authorize?scope=" & Me.scope & "&client_id=" & Me.apiKey & "&redirect_url=" & Me.authorizeRedirectUri)
        Catch ex As Exception
            If Session("vb_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("vb_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                sendMessageErrorResponse = ex.Message
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
        If Session("vb_rest_AccessToken") IsNot Nothing Then
            Me.accessToken = Session("vb_rest_AccessToken").ToString()
        Else
            Me.accessToken = Nothing
        End If

        If Session("vb_rest_AccessTokenExpirtyTime") IsNot Nothing Then
            Me.accessTokenExpiryTime = Session("vb_rest_AccessTokenExpirtyTime").ToString()
        Else
            Me.accessTokenExpiryTime = Nothing
        End If

        If Session("vb_rest_RefreshToken") IsNot Nothing Then
            Me.refreshToken = Session("vb_rest_RefreshToken").ToString()
        Else
            Me.refreshToken = Nothing
        End If

        If Session("vb_rest_RefreshTokenExpiryTime") IsNot Nothing Then
            Me.refreshTokenExpiryTime = Session("vb_rest_RefreshTokenExpiryTime").ToString()
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
        If Session("vb_rest_AccessToken") Is Nothing Then
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
    Private Function GetAccessToken(type As AccessTokenType) As Boolean
        Dim postStream As Stream = Nothing
        Try
            Dim currentServerTime As DateTime = DateTime.UtcNow.ToLocalTime()
            Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.endPoint & "/oauth/token")
            accessTokenRequest.Method = "POST"
            Dim oauthParameters As String = String.Empty

            If type = AccessTokenType.Authorization_Code Then
                oauthParameters = "client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&code=" & Me.authCode & "&grant_type=authorization_code&scope=" & Me.scope
            Else
                oauthParameters = "grant_type=refresh_token&client_id=" & Me.apiKey & "&client_secret=" & Me.secretKey & "&refresh_token=" & Me.refreshToken
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

                    Session("vb_rest_AccessTokenExpirtyTime") = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in))

                    If deserializedJsonObj.expires_in.Equals("0") Then
                        Dim defaultAccessTokenExpiresIn As Integer = 100
                        ' In Years
                        Session("vb_rest_AccessTokenExpirtyTime") = currentServerTime.AddYears(defaultAccessTokenExpiresIn)
                    End If

                    Me.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() & " " & refreshExpiry.ToLongTimeString()

                    Session("vb_rest_AccessToken") = Me.accessToken

                    Me.accessTokenExpiryTime = Session("vb_rest_AccessTokenExpirtyTime").ToString()
                    Session("vb_rest_RefreshToken") = Me.refreshToken
                    Session("vb_rest_RefreshTokenExpiryTime") = Me.refreshTokenExpiryTime.ToString()
                    Session("vb_rest_appState") = "TokenReceived"
                    accessTokenResponseStream.Close()
                    Return True
                Else
                    Dim errorMessage As String = "Auth server returned null access token"
                    If Session("vb_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("vb_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                        sendMessageErrorResponse = errorMessage
                    Else
                        getMessageErrorResponse = errorMessage
                    End If
                    Return False
                End If
            End Using
        Catch ex As Exception
            Dim errorMessage As String = ex.Message
            If Session("vb_rest_ServiceRequest") IsNot Nothing AndAlso (String.Compare(Session("vb_rest_ServiceRequest").ToString(), "sendmessasge") = 0) Then
                sendMessageErrorResponse = errorMessage
            Else
                getMessageErrorResponse = errorMessage
            End If
        Finally
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try

        Return False
    End Function

#End Region
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
    Private Function GetContentTypeFromExtension(extension As String) As String
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
    Private Sub SendMessageRequest(accToken As String, edPoint As String, subject As String, message As String, groupflag As String, attachments As ArrayList)
        Dim postStream As Stream = Nothing
        Try
            Dim boundaryToSend As String = "----------------------------" & DateTime.Now.Ticks.ToString("x")

            Dim msgRequestObject As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & edPoint & "/rest/1/MyMessages"), HttpWebRequest)
            msgRequestObject.Headers.Add("Authorization", "Bearer " & accToken)
            msgRequestObject.Method = "POST"
            Dim contentType As String = "multipart/form-data; type=""application/x-www-form-urlencoded""; start=""<startpart>""; boundary=""" & boundaryToSend & """" & vbCr & vbLf
            msgRequestObject.ContentType = contentType
            Dim mmsParameters As String = Me.phoneNumbersParameter & "Subject=" & Server.UrlEncode(subject) & "&Text=" & Server.UrlEncode(message) & "&Group=" & groupflag

            Dim dataToSend As String = String.Empty
            dataToSend += "--" & boundaryToSend & vbCr & vbLf
            dataToSend += "Content-Type: application/x-www-form-urlencoded; charset=UTF-8" & vbCr & vbLf & "Content-Transfer-Encoding: 8bit" & vbCr & vbLf & "Content-Disposition: form-data; name=""root-fields""" & vbCr & vbLf & "Content-ID: <startpart>" & vbCr & vbLf & vbCr & vbLf & mmsParameters & vbCr & vbLf

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
                    dataToSend += "--" & boundaryToSend & "--" & vbCr & vbLf
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
                        dataToSend += vbCr & vbLf & "--" & boundaryToSend & vbCr & vbLf
                    Else
                        dataToSend = vbCr & vbLf & "--" & boundaryToSend & vbCr & vbLf
                    End If

                    dataToSend += "Content-Disposition: form-data; name=""file" & count & """; filename=""" & mmsFileName & """" & vbCr & vbLf
                    dataToSend += "Content-Type:" & attachmentContentType & vbCr & vbLf
                    dataToSend += "Content-ID:<" & mmsFileName & ">" & vbCr & vbLf
                    dataToSend += "Content-Transfer-Encoding:binary" & vbCr & vbLf & vbCr & vbLf
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

                Dim byteLastBoundary As Byte() = encoding.GetBytes(vbCr & vbLf & "--" & boundaryToSend & "--" & vbCr & vbLf)
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
    Private Function JoinTwoByteArrays(firstByteArray As Byte(), secondByteArray As Byte()) As MemoryStream
        Dim newSize As Integer = firstByteArray.Length + secondByteArray.Length
        Dim totalMemoryStream = New MemoryStream(New Byte(newSize - 1) {}, 0, newSize, True, True)
        totalMemoryStream.Write(firstByteArray, 0, firstByteArray.Length)
        totalMemoryStream.Write(secondByteArray, 0, secondByteArray.Length)
        Return totalMemoryStream
    End Function

    Protected Sub getMessageHeaders_Click(sender As Object, e As EventArgs)
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("vb_rest_ServiceRequest") = "getmessageheader"
            Session("vb_rest_appState") = "GetToken"
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
        GetMsgHeaders()
    End Sub

    Protected Sub GetMsgHeaders()
        Me.GetMessageHeads(Me.accessToken, Me.endPoint, headerCountTextBox.Value, indexCursorTextBox.Value)
    End Sub
    Private Sub GetMessageHeads(acctoken As String, epoint As String, hCount As String, iCursor As String)
        Try
            Dim mimRequestObject1 As HttpWebRequest

            Dim getHeadersURL As String = String.Empty & endPoint & "/rest/1/MyMessages?HeaderCount=" & hCount
            If Not String.IsNullOrEmpty(iCursor) Then
                getHeadersURL += "&IndexCursor=" & iCursor
            End If
            mimRequestObject1 = DirectCast(WebRequest.Create(getHeadersURL), HttpWebRequest)
            mimRequestObject1.Headers.Add("Authorization", "Bearer " & accessToken)
            mimRequestObject1.Method = "GET"
            mimRequestObject1.KeepAlive = True

            Dim mimResponseObject1 As WebResponse = mimRequestObject1.GetResponse()
            Using sr As New StreamReader(mimResponseObject1.GetResponseStream())
                Dim mimResponseData As String = sr.ReadToEnd()

                Dim deserializeJsonObject As New JavaScriptSerializer()
                Dim deserializedJsonObj As MIMResponse = DirectCast(deserializeJsonObject.Deserialize(mimResponseData, GetType(MIMResponse)), MIMResponse)

                If deserializedJsonObj IsNot Nothing Then
                    getHeadersSuccessResponse = "Success"
                    messageList = deserializedJsonObj.MessageHeadersList
                Else
                    getHeadersErrorResponse = "No response from server"
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
                getHeadersErrorResponse = errorResponse
            Catch
                errorResponse = "Unable to get response"
                getHeadersErrorResponse = errorResponse
            End Try
        Catch ex As Exception
            getHeadersErrorResponse = ex.Message
            Return
        End Try
    End Sub


    ''' <summary>
    ''' Gets the message content for MMS messages based on Message ID and Part Number
    ''' </summary>
    Private Sub GetMessageContentByIDnPartNumber(accTok As String, endP As String, messId As String, partNum As String)
        Try
            Dim mimRequestObject1 As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & endP & "/rest/1/MyMessages/" & messId & "/" & partNum), HttpWebRequest)
            mimRequestObject1.Headers.Add("Authorization", "Bearer " & accTok)
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
                        getMessageErrorResponse = [String].Format("End of stream reached with {0} bytes left to read", remaining)
                        Return
                    End If

                    remaining -= read
                    offset += read
                End While

                imageData = Regex.Split(getContentResponseObject.ContentType.ToLower(), ";")
                Dim ext As String() = Regex.Split(imageData(0), "/")
                fetchedImage.Src = "data:" & imageData(0) & ";base64," & Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None)
                getMessageSuccessResponse = "Success"
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
        Session("vb_rest_Address") = Address.Value
        Session("vb_rest_Message") = message.Value
        Session("vb_rest_Subject") = subject.Value
        Session("vb_rest_Group") = groupCheckBox.Checked.ToString()
        Session("vb_rest_Attachments") = attachment.Value
        Session("vb_rest_GetHeadercount") = headerCountTextBox.Value
        Session("vb_rest_GetHeaderIndex") = indexCursorTextBox.Value
        Session("vb_rest_GetMessageId") = MessageId.Value
        Session("vb_rest_GetMessagePart") = PartNumber.Value
    End Sub

    Protected Sub ResetRequestSessionVariables()
        Session("vb_rest_Address") = Nothing
        Session("vb_rest_Message") = Nothing
        Session("vb_rest_Subject") = Nothing
        Session("vb_rest_Group") = Nothing
        Session("vb_rest_Attachments") = Nothing
        Session("vb_rest_GetHeadercount") = Nothing
        Session("vb_rest_GetHeaderIndex") = Nothing
        Session("vb_rest_GetMessageId") = Nothing
        Session("vb_rest_GetMessagePart") = Nothing
    End Sub

    Protected Sub RestoreRequestSessionVariables()
        Address.Value = Session("vb_rest_Address").ToString()
        message.Value = Session("vb_rest_Message").ToString()


        subject.Value = Session("vb_rest_Subject").ToString()
        groupCheckBox.Checked = Convert.ToBoolean(Session("vb_rest_Group").ToString())
        attachment.Value = Session("vb_rest_Attachments").ToString()
        headerCountTextBox.Value = Session("vb_rest_GetHeadercount").ToString()
        indexCursorTextBox.Value = Session("vb_rest_GetHeaderIndex").ToString()
        MessageId.Value = Session("vb_rest_GetMessageId").ToString()
        PartNumber.Value = Session("vb_rest_GetMessagePart").ToString()
    End Sub


    Protected Sub Button1_Click(sender As Object, e As EventArgs)
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("vb_rest_ServiceRequest") = "sendmessasge"
            Session("vb_rest_appState") = "GetToken"
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
        Me.SendMessageRequest(accessToken, endpoint, subject.Value, message.Value, groupCheckBox.Checked.ToString().ToLower(), attachmentList)
    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs)
        Me.ReadTokenSessionVariables()

        Dim tokentResult As String = Me.IsTokenValid()

        If tokentResult.CompareTo("INVALID_ACCESS_TOKEN") = 0 Then
            SetRequestSessionVariables()
            Session("vb_rest_ServiceRequest") = "getmessagecontent"
            Session("vb_rest_appState") = "GetToken"
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

        GetMessageContentByIDnPartNumber()
    End Sub

    Protected Sub GetMessageContentByIDnPartNumber()
        Me.GetMessageContentByIDnPartNumber(Me.accessToken, Me.endPoint, MessageId.Value, PartNumber.Value)
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
        If String.IsNullOrEmpty(Address.Value) Then
            sendMessageErrorResponse = "Address field cannot be blank."
            Return False
        End If

        Dim addresses As String() = Address.Value.Trim().Split(","c)

        If addresses.Length > Me.maxAddresses Then
            sendMessageErrorResponse = "Message cannot be delivered to more than 10 receipients."
            Return False
        End If

        If groupCheckBox.Checked AndAlso addresses.Length < 2 Then
            sendMessageErrorResponse = "Specify more than one address for Group message."
            Return False
        End If

        For Each address__1 As String In addresses
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
                Me.phoneNumbersParameter = Me.phoneNumbersParameter & "Addresses=short:" & Server.UrlEncode(address__1.ToString()) & "&"
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
                Me.phoneNumbersParameter = Me.phoneNumbersParameter & "Addresses=" & Server.UrlEncode(address__1.ToString()) & "&"
            ElseIf address__1.Contains("@") Then
                isValid = Me.IsValidEmail(address__1)
                If isValid = False Then
                    sendMessageErrorResponse = "Specified Email Address is invalid."
                    Return False
                Else
                    Me.addressList.Add(address__1)
                    Me.phoneNumbersParameter = Me.phoneNumbersParameter & "Addresses=" & Server.UrlEncode(address__1.ToString()) & "&"
                End If
            Else
                If Me.IsValidMISDN(address__1) = True Then
                    If address__1.StartsWith("tel:") Then
                        phonenumbers = address__1.Replace("-", String.Empty)
                        Me.phoneNumbersParameter = Me.phoneNumbersParameter & "Addresses=" & Server.UrlEncode(phonenumbers.ToString()) & "&"
                    Else
                        phonenumbers = address__1.Replace("-", String.Empty)
                        Me.phoneNumbersParameter = Me.phoneNumbersParameter & "Addresses=" & Server.UrlEncode("tel:" & phonenumbers.ToString()) & "&"
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
    Private Function IsValidMISDN(number As String) As Boolean
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
    Private Function IsValidEmail(emailID As String) As Boolean
        Dim strRegex As String = "^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" & "\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" & ".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
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
    Private Function IsNumber(address As String) As Boolean
        Dim isValid As Boolean = False
        Dim regex As New Regex("^[0-9]*$")
        If regex.IsMatch(address) Then
            isValid = True
        End If

        Return isValid
    End Function
#Region "Data Structures"

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
            Set(value As String)
                m_access_token = value
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
            Set(value As String)
                m_refresh_token = value
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
            Set(value As String)
                m_expires_in = value
            End Set
        End Property
        Private m_expires_in As String
    End Class

    ''' <summary>
    ''' Response returned from MyMessages api
    ''' </summary>
    Public Class MIMResponse
        ''' <summary>
        ''' Gets or sets the value of message header list.
        ''' </summary>
        Public Property MessageHeadersList() As MessageHeaderList
            Get
                Return m_MessageHeadersList
            End Get
            Set(value As MessageHeaderList)
                m_MessageHeadersList = value
            End Set
        End Property
        Private m_MessageHeadersList As MessageHeaderList
    End Class

    ''' <summary>
    ''' Message Header List
    ''' </summary>
    Public Class MessageHeaderList
        ''' <summary>
        ''' Gets or sets the value of object containing a List of Messages Headers
        ''' </summary>
        Public Property Headers() As List(Of Header)
            Get
                Return m_Headers
            End Get
            Set(value As List(Of Header))
                m_Headers = value
            End Set
        End Property
        Private m_Headers As List(Of Header)

        ''' <summary>
        ''' Gets or sets the value of a number representing the number of headers returned for this request.
        ''' </summary>
        Public Property HeaderCount() As Integer
            Get
                Return m_HeaderCount
            End Get
            Set(value As Integer)
                m_HeaderCount = value
            End Set
        End Property
        Private m_HeaderCount As Integer

        ''' <summary>
        ''' Gets or sets the value of a string which defines the start of the next block of messages for the current request.
        ''' A value of zero (0) indicates the end of the block.
        ''' </summary>
        Public Property IndexCursor() As String
            Get
                Return m_IndexCursor
            End Get
            Set(value As String)
                m_IndexCursor = value
            End Set
        End Property
        Private m_IndexCursor As String
    End Class

    ''' <summary>
    ''' Object containing a List of Messages Headers
    ''' </summary>
    Public Class Header
        ''' <summary>
        ''' Gets or sets the value of Unique message identifier
        ''' </summary>
        Public Property MessageId() As String
            Get
                Return m_MessageId
            End Get
            Set(value As String)
                m_MessageId = value
            End Set
        End Property
        Private m_MessageId As String

        ''' <summary>
        ''' Gets or sets the value of message sender
        ''' </summary>
        Public Property From() As String
            Get
                Return m_From
            End Get
            Set(value As String)
                m_From = value
            End Set
        End Property
        Private m_From As String

        ''' <summary>
        ''' Gets or sets the value of the addresses, whom the message need to be delivered. 
        ''' If Group Message, this will contain multiple Addresses.
        ''' </summary>
        Public Property [To]() As List(Of String)
            Get
                Return m_To
            End Get
            Set(value As List(Of String))
                m_To = value
            End Set
        End Property
        Private m_To As List(Of String)

        ''' <summary>
        ''' Gets or sets a value of message text
        ''' </summary>
        Public Property Text() As String
            Get
                Return m_Text
            End Get
            Set(value As String)
                m_Text = value
            End Set
        End Property
        Private m_Text As String

        ''' <summary>
        ''' Gets or sets a value of message part descriptions
        ''' </summary>
        Public Property MmsContent() As List(Of MMSContent)
            Get
                Return m_MmsContent
            End Get
            Set(value As List(Of MMSContent))
                m_MmsContent = value
            End Set
        End Property
        Private m_MmsContent As List(Of MMSContent)

        ''' <summary>
        ''' Gets or sets the value of date/time message received
        ''' </summary>
        Public Property Received() As DateTime
            Get
                Return m_Received
            End Get
            Set(value As DateTime)
                m_Received = value
            End Set
        End Property
        Private m_Received As DateTime

        ''' <summary>
        ''' Gets or sets a value indicating whether its a favourite or not
        ''' </summary>
        Public Property Favorite() As Boolean
            Get
                Return m_Favorite
            End Get
            Set(value As Boolean)
                m_Favorite = value
            End Set
        End Property
        Private m_Favorite As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether message is read or not
        ''' </summary>
        Public Property Read() As Boolean
            Get
                Return m_Read
            End Get
            Set(value As Boolean)
                m_Read = value
            End Set
        End Property
        Private m_Read As Boolean

        ''' <summary>
        ''' Gets or sets the value of type of message, TEXT or MMS
        ''' </summary>
        Public Property Type() As String
            Get
                Return m_Type
            End Get
            Set(value As String)
                m_Type = value
            End Set
        End Property
        Private m_Type As String

        ''' <summary>
        ''' Gets or sets the value of indicator, which indicates if message is Incoming or Outgoing “IN” or “OUT”
        ''' </summary>
        Public Property Direction() As String
            Get
                Return m_Direction
            End Get
            Set(value As String)
                m_Direction = value
            End Set
        End Property
        Private m_Direction As String
    End Class

    ''' <summary>
    ''' Message part descriptions
    ''' </summary>
    Public Class MMSContent
        ''' <summary>
        ''' Gets or sets the value of content name
        ''' </summary>
        Public Property ContentName() As String
            Get
                Return m_ContentName
            End Get
            Set(value As String)
                m_ContentName = value
            End Set
        End Property
        Private m_ContentName As String

        ''' <summary>
        ''' Gets or sets the value of content type
        ''' </summary>
        Public Property ContentType() As String
            Get
                Return m_ContentType
            End Get
            Set(value As String)
                m_ContentType = value
            End Set
        End Property
        Private m_ContentType As String

        ''' <summary>
        ''' Gets or sets the value of part number
        ''' </summary>
        Public Property PartNumber() As String
            Get
                Return m_PartNumber
            End Get
            Set(value As String)
                m_PartNumber = value
            End Set
        End Property
        Private m_PartNumber As String
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
            Set(value As String)
                m_Id = value
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
#End Region
End Class