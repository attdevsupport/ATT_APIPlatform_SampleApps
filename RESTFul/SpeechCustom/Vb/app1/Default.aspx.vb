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
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Web.UI.HtmlControls

#End Region

''' <summary>
''' Speech application
''' </summary>
Partial Public Class Speech_App1
    Inherits System.Web.UI.Page
#Region "Class variables and Data structures"
    ''' <summary>
    ''' Temporary variables for processing
    ''' </summary>
    Private fqdn As String, accessTokenFilePath As String

    ''' <summary>
    ''' Temporary variables for processing
    ''' </summary>
    Private apiKey As String, secretKey As String, accessToken As String, scope As String, refreshToken As String, refreshTokenExpiryTime As String, _
     accessTokenExpiryTime As String

    ''' <summary>
    ''' variable for having the posted file.
    ''' </summary>
    Private SpeechFilesDir As String

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    Private xgrammer As String = String.Empty
    Private xdictionary As String = String.Empty
    Private xgrammerContent As String = String.Empty
    Private xdictionaryContent As String = String.Empty


    Public speechErrorMessage As String = String.Empty
    Public speechSuccessMessage As String = String.Empty
    Public speechResponseData As SpeechResponse = Nothing

    ''' <summary>
    ''' Access Token Types
    ''' </summary>
    Public Enum AccessType
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

#Region "Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This function reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        Me.ReadConfigFile()
        Me.SetContent()
    End Sub

    ''' <summary>
    ''' Method that calls SpeechToText api when user clicked on submit button
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">eventargs of the button</param>
    Protected Sub BtnSubmit_Click(sender As Object, e As EventArgs)
        Try
            Dim IsValid As Boolean = True

            IsValid = Me.ReadAndGetAccessToken(speechErrorMessage)
            If IsValid = False Then
                speechErrorMessage = "Unable to get access token"
                Return
            End If
            Dim speechFile = Me.SpeechFilesDir & audio_file.SelectedValue.ToString()
            Me.ConvertToSpeech(Me.fqdn & "/speech/v3/speechToTextCustom", Me.accessToken, SpeechContext.SelectedValue.ToString(), x_arg.Text, speechFile)
        Catch ex As Exception
            speechErrorMessage = ex.Message
            Return
        End Try
    End Sub


    Private Sub SetContent()
        Dim streamReader As New StreamReader(Me.xdictionary)
        xdictionaryContent = streamReader.ReadToEnd()
        mimeData.Text = "x-dictionary:" & Environment.NewLine & xdictionaryContent
        Dim streamReader1 As New StreamReader(Me.xgrammer)
        xgrammerContent = streamReader1.ReadToEnd()
        mimeData.Text = mimeData.Text + Environment.NewLine & "x-grammar:" & Environment.NewLine & xgrammerContent
        streamReader.Close()
        streamReader1.Close()
    End Sub

#End Region

#Region "Access Token Related Functions"

    ''' <summary>
    ''' Read parameters from configuraton file
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\SpeechApp1AccessToken.txt"
        End If

        Me.fqdn = ConfigurationManager.AppSettings("FQDN")
        If String.IsNullOrEmpty(Me.fqdn) Then
            speechErrorMessage = "FQDN is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            speechErrorMessage = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            speechErrorMessage = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "SPEECH"
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

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SpeechFilesDir")) Then
            Me.SpeechFilesDir = Request.MapPath(ConfigurationManager.AppSettings("SpeechFilesDir"))
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("xgrammer")) Then
            Me.xgrammer = Request.MapPath(ConfigurationManager.AppSettings("xgrammer"))
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("xdictionary")) Then
            Me.xdictionary = Request.MapPath(ConfigurationManager.AppSettings("xdictionary"))
        End If

        If Not IsPostBack Then
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SpeechContext")) Then
                Dim speechContexts As String() = ConfigurationManager.AppSettings("SpeechContext").ToString().Split(";"c)
                For Each speechContext__1 As String In speechContexts
                    SpeechContext.Items.Add(speechContext__1)
                Next
                If speechContexts.Length > 0 Then
                    SpeechContext.Items(0).Selected = True
                End If
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("NameParameters")) Then
                Dim nameParameters As String() = ConfigurationManager.AppSettings("NameParameters").ToString().Split(";"c)
                For Each nameParameter As String In nameParameters
                    nameParam.Items.Add(nameParameter)
                Next
                If nameParameters.Length > 0 Then
                    nameParam.Items(0).Selected = True
                End If
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("X-ArgGeneric")) Then
                x_arg.Text = ConfigurationManager.AppSettings("X-ArgGeneric")
            End If
            If Not String.IsNullOrEmpty(SpeechFilesDir) Then
                Dim filePaths As String() = Directory.GetFiles(Me.SpeechFilesDir)
                For Each filePath As String In filePaths
                    audio_file.Items.Add(Path.GetFileName(filePath))
                Next
                If filePaths.Length > 0 Then
                    audio_file.Items(0).Selected = True
                End If
            End If
        End If

        Return True
    End Function

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
    ''' This function validates the expiry of the access token and refresh token.
    ''' function compares the current time with the refresh token taken time, if current time is greater then returns INVALID_REFRESH_TOKEN
    ''' function compares the difference of last access token taken time and the current time with the expiry seconds, if its more, returns INVALID_ACCESS_TOKEN    
    ''' otherwise returns VALID_ACCESS_TOKEN
    ''' </summary>
    ''' <returns>string, which specifies the token validity</returns>
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

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.fqdn & "/oauth/token")
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

                Dim accessTokenRequest As WebRequest = System.Net.HttpWebRequest.Create(String.Empty & Me.fqdn & "/oauth/token")
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

#End Region

#Region "Speech Service Functions"

    ''' <summary>
    ''' Content type based on the file extension.
    ''' </summary>
    ''' <param name="extension">file extension</param>
    ''' <returns>the Content type mapped to the extension"/> summed memory stream</returns>
    Private Function MapContentTypeFromExtension(extension As String) As String
        Dim extensionToContentTypeMapping As New Dictionary(Of String, String)() From { _
         {".amr", "audio/amr"}, _
         {".wav", "audio/wav"}, _
         {".awb", "audio/amr-wb"}, _
         {".spx", "audio/x-speex"} _
        }
        If extensionToContentTypeMapping.ContainsKey(extension) Then
            Return extensionToContentTypeMapping(extension)
        Else
            Throw New ArgumentException("invalid attachment extension")
        End If
    End Function

    ''' <summary>
    ''' This function invokes api SpeechToText to convert the given wav amr file and displays the result.
    ''' </summary>
    Private Sub ConvertToSpeech(parEndPoint As String, parAccessToken As String, parXspeechContext As String, parXArgs As String, parSpeechFilePath As String)
        Dim postStream As Stream = Nothing
        Dim audioFileStream As FileStream = Nothing
        audioFileStream = New FileStream(parSpeechFilePath, FileMode.Open, FileAccess.Read)
        Dim reader As New BinaryReader(audioFileStream)
        Try

            Dim binaryData As Byte() = reader.ReadBytes(CInt(audioFileStream.Length))
            If binaryData IsNot Nothing Then
                Dim boundary As String = "----------------------------" & DateTime.Now.Ticks.ToString("x")
                Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & parEndPoint), HttpWebRequest)
                httpRequest.Headers.Add("Authorization", "Bearer " & parAccessToken)
                httpRequest.Headers.Add("X-SpeechContext", parXspeechContext)
                httpRequest.Headers.Add("Content-Language", "en-us")
                httpRequest.ContentType = "multipart/x-srgs-audio; " & "boundary=" & boundary

                If Not String.IsNullOrEmpty(parXArgs) Then
                    httpRequest.Headers.Add("X-Arg", parXArgs)
                End If
                Dim filenameArgument As String = "filename"
                If Not String.IsNullOrEmpty(SpeechContext.SelectedValue) Then
                    If String.Compare("GenericHints", SpeechContext.SelectedValue) = 0 Then
                        filenameArgument = nameParam.SelectedValue.ToString()
                    End If
                End If

                Dim contentType As String = Me.MapContentTypeFromExtension(Path.GetExtension(parSpeechFilePath))

                Dim data As String = String.Empty


                data += "--" & boundary & vbCr & vbLf & "Content-Disposition: form-data; name=""x-dictionary""; " & filenameArgument & "=""speech_alpha.pls""" & vbCr & vbLf & "Content-Type: application/pls+xml" & vbCr & vbLf

                data += vbCr & vbLf & xdictionaryContent & vbCr & vbLf & vbCr & vbLf & vbCr & vbLf

                data += "--" & boundary & vbCr & vbLf & "Content-Disposition: form-data; name=""x-grammar"""

                'data += "filename=\"prefix.srgs\" ";

                data += vbCr & vbLf & "Content-Type: application/srgs+xml " & vbCr & vbLf & vbCr & vbLf & xgrammerContent & vbCr & vbLf & vbCr & vbLf & vbCr & vbLf & "--" & boundary & vbCr & vbLf

                data += ("Content-Disposition: form-data; name=""x-voice""; " & filenameArgument & "=""") + audio_file.SelectedValue & """"
                data += vbCr & vbLf & "Content-Type: " & contentType & vbCr & vbLf & vbCr & vbLf
                Dim encoding As New UTF8Encoding()
                Dim firstPart As Byte() = encoding.GetBytes(data)
                Dim newSize As Integer = firstPart.Length + binaryData.Length

                Dim memoryStream = New MemoryStream(New Byte(newSize - 1) {}, 0, newSize, True, True)
                memoryStream.Write(firstPart, 0, firstPart.Length)
                memoryStream.Write(binaryData, 0, binaryData.Length)

                Dim postBytes As Byte() = memoryStream.GetBuffer()

                Dim byteLastBoundary As Byte() = encoding.GetBytes(vbCr & vbLf & vbCr & vbLf & "--" & boundary & "--")
                Dim totalSize As Integer = postBytes.Length + byteLastBoundary.Length

                Dim totalMS = New MemoryStream(New Byte(totalSize - 1) {}, 0, totalSize, True, True)
                totalMS.Write(postBytes, 0, postBytes.Length)
                totalMS.Write(byteLastBoundary, 0, byteLastBoundary.Length)

                Dim finalpostBytes As Byte() = totalMS.GetBuffer()

                httpRequest.ContentLength = totalMS.Length
                'httpRequest.ContentType = contentType;
                httpRequest.Accept = "application/json"
                httpRequest.Method = "POST"
                httpRequest.KeepAlive = True
                postStream = httpRequest.GetRequestStream()
                postStream.Write(finalpostBytes, 0, finalpostBytes.Length)
                postStream.Close()

                Dim speechResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(speechResponse.GetResponseStream())
                    Dim speechRequestResponse As String = streamReader.ReadToEnd()
                    If Not String.IsNullOrEmpty(speechRequestResponse) Then
                        Dim deserializeJsonObject As New JavaScriptSerializer()
                        Dim deserializedJsonObj As SpeechResponse = DirectCast(deserializeJsonObject.Deserialize(speechRequestResponse, GetType(SpeechResponse)), SpeechResponse)
                        If deserializedJsonObj IsNot Nothing Then
                            speechResponseData = New SpeechResponse()
                            speechResponseData = deserializedJsonObj
                            'speechErrorMessage = speechRequestResponse;
                            speechSuccessMessage = "true"
                        Else
                            speechErrorMessage = "Empty speech to text response"
                        End If
                    Else
                        speechErrorMessage = "Empty speech to text response"
                    End If

                    streamReader.Close()
                End Using
            Else
                speechErrorMessage = "Empty speech to text response"
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

            speechErrorMessage = errorResponse
        Catch ex As Exception
            speechErrorMessage = ex.ToString()
        Finally
            reader.Close()
            audioFileStream.Close()
            If postStream IsNot Nothing Then
                postStream.Close()
            End If
        End Try
    End Sub

#End Region
End Class

#Region "Access Token and Speech Response Data Structures"

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
        Set(value As String)
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
        Set(value As String)
            m_expires_in = Value
        End Set
    End Property
    Private m_expires_in As String
End Class

''' <summary>
''' Speech Response to an audio file
''' </summary>
Public Class SpeechResponse
    ''' <summary>
    ''' Gets or sets the Recognition value returned by api
    ''' </summary>
    Public Property Recognition() As Recognition
        Get
            Return m_Recognition
        End Get
        Set(value As Recognition)
            m_Recognition = Value
        End Set
    End Property
    Private m_Recognition As Recognition
End Class

''' <summary>
''' Recognition returned by the server for Speech to text request.
''' </summary>
Public Class Recognition
    ''' <summary>
    ''' Gets or sets a unique string that identifies this particular transaction.
    ''' </summary>
    Public Property ResponseId() As String
        Get
            Return m_ResponseId
        End Get
        Set(value As String)
            m_ResponseId = Value
        End Set
    End Property
    Private m_ResponseId As String

    ''' <summary>
    ''' Gets or sets NBest Complex structure that holds the results of the transcription. Supports multiple transcriptions.
    ''' </summary>
    Public Property NBest() As List(Of NBest)
        Get
            Return m_NBest
        End Get
        Set(value As List(Of NBest))
            m_NBest = Value
        End Set
    End Property
    Private m_NBest As List(Of NBest)

    ''' <summary>
    ''' Gets or sets the Status of the transcription.
    ''' </summary>
    Public Property Status() As String
        Get
            Return m_Status
        End Get
        Set(value As String)
            m_Status = Value
        End Set
    End Property
    Private m_Status As String

End Class
Public Class nluHypothesis
    Public Property OutComposite() As outComposite()
        Get
            Return m_OutComposite
        End Get
        Set(value As outComposite())
            m_OutComposite = Value
        End Set
    End Property
    Private m_OutComposite As outComposite()
End Class
Public Class outComposite
    Public Property Grammar() As String
        Get
            Return m_Grammar
        End Get
        Set(value As String)
            m_Grammar = value
        End Set
    End Property
    Private m_Grammar As String
    Public Property Out() As String
        Get
            Return m_Out
        End Get
        Set(value As String)
            m_Out = Value
        End Set
    End Property
    Private m_Out As String
End Class

''' <summary>
''' Complex structure that holds the results of the transcription. Supports multiple transcriptions.
''' </summary>
Public Class NBest
    ''' <summary>
    ''' Gets or sets the transcription of the audio. 
    ''' </summary>
    Public Property Hypothesis() As String
        Get
            Return m_Hypothesis
        End Get
        Set(value As String)
            m_Hypothesis = Value
        End Set
    End Property
    Private m_Hypothesis As String

    ''' <summary>
    ''' Gets or sets the language used to decode the Hypothesis. 
    ''' Represented using the two-letter ISO 639 language code, hyphen, two-letter ISO 3166 country code in lower case, e.g. “en-us”.
    ''' </summary>
    Public Property LanguageId() As String
        Get
            Return m_LanguageId
        End Get
        Set(value As String)
            m_LanguageId = Value
        End Set
    End Property
    Private m_LanguageId As String

    ''' <summary>
    ''' Gets or sets the confidence value of the Hypothesis, a value between 0.0 and 1.0 inclusive.
    ''' </summary>
    Public Property Confidence() As Double
        Get
            Return m_Confidence
        End Get
        Set(value As Double)
            m_Confidence = Value
        End Set
    End Property
    Private m_Confidence As Double

    ''' <summary>
    ''' Gets or sets a machine-readable string indicating an assessment of utterance/result quality and the recommended treatment of the Hypothesis. 
    ''' The assessment reflects a confidence region based on prior experience with similar results. 
    ''' accept - the hypothesis value has acceptable confidence
    ''' confirm - the hypothesis should be independently confirmed due to lower confidence
    ''' reject - the hypothesis should be rejected due to low confidence
    ''' </summary>
    Public Property Grade() As String
        Get
            Return m_Grade
        End Get
        Set(value As String)
            m_Grade = Value
        End Set
    End Property
    Private m_Grade As String

    ''' <summary>
    ''' Gets or sets a text string prepared according to the output domain of the application package. 
    ''' The string will generally be a formatted version of the hypothesis, but the words may have been altered through 
    ''' insertions/deletions/substitutions to make the result more readable or usable for the client.  
    ''' </summary>
    Public Property ResultText() As String
        Get
            Return m_ResultText
        End Get
        Set(value As String)
            m_ResultText = Value
        End Set
    End Property
    Private m_ResultText As String

    ''' <summary>
    ''' Gets or sets the words of the Hypothesis split into separate strings.  
    ''' May omit some of the words of the Hypothesis string, and can be empty.  Never contains words not in hypothesis string.  
    ''' </summary>
    Public Property Words() As List(Of String)
        Get
            Return m_Words
        End Get
        Set(value As List(Of String))
            m_Words = Value
        End Set
    End Property
    Private m_Words As List(Of String)

    ''' <summary>
    ''' Gets or sets the confidence scores for each of the strings in the words array.  Each value ranges from 0.0 to 1.0 inclusive.
    ''' </summary>
    Public Property WordScores() As List(Of Double)
        Get
            Return m_WordScores
        End Get
        Set(value As List(Of Double))
            m_WordScores = Value
        End Set
    End Property
    Private m_WordScores As List(Of Double)

    Public Property NluHypothesis() As nluHypothesis
        Get
            Return m_NluHypothesis
        End Get
        Set(value As nluHypothesis)
            m_NluHypothesis = Value
        End Set
    End Property
    Private m_NluHypothesis As nluHypothesis
End Class
#End Region