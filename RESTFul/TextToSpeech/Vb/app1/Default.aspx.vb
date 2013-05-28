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
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI.HtmlControls


#End Region

''' <summary>
''' Speech application
''' </summary>
Partial Public Class TTS_App1
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
    Private TTSPlainText As String = String.Empty
    Private TTSSSML As String = String.Empty

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    Private xArgsData As String = String.Empty

    Public TTSErrorMessage As String = String.Empty
    Public TTSSuccessMessage As String = String.Empty
    Public receivedBytes As Byte() = Nothing

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

    Private Sub SetContent()
        Dim streamReaderPlain As New StreamReader(Me.TTSPlainText)
        plaintext.Text = streamReaderPlain.ReadToEnd()
        streamReaderPlain.Close()
        Dim streamReaderSSML As New StreamReader(Me.TTSSSML)
        ssml.Text = streamReaderSSML.ReadToEnd()
        streamReaderSSML.Close()
    End Sub

    ''' <summary>
    ''' Method that calls SpeechToText api when user clicked on submit button
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">eventargs of the button</param>
    Protected Sub BtnSubmit_Click(sender As Object, e As EventArgs)
        Try
            Dim IsValid As Boolean = True

            IsValid = Me.ReadAndGetAccessToken(TTSErrorMessage)
            If IsValid = False Then
                TTSErrorMessage = "Unable to get access token"
                Return
            End If
            Dim content As String = String.Empty
            If String.Compare(ContentType.SelectedValue, "text/plain") = 0 Then
                content = plaintext.Text
            Else
                content = ssml.Text
            End If

            '
            '            this.ConvertToSpeech(this.fqdn + "/rest/2/SpeechToText",
            '                this.accessToken, SpeechContext.SelectedValue.ToString(), x_arg.Text, speechFile, chunkValue);
            '             

            Me.TextToSpeech(Me.fqdn, "/speech/v3/textToSpeech", Me.accessToken, x_arg.Text, ContentType.SelectedValue, content)
        Catch ex As Exception
            TTSErrorMessage = ex.Message
            Return
        End Try
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
            TTSErrorMessage = "FQDN is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            TTSErrorMessage = "api_key is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            TTSErrorMessage = "secret_key is not defined in configuration file"
            Return False
        End If

        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "TTS"
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

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("TTSPlainText")) Then
            Me.TTSPlainText = Request.MapPath(ConfigurationManager.AppSettings("TTSPlainText"))
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("TTSSSML")) Then
            Me.TTSSSML = Request.MapPath(ConfigurationManager.AppSettings("TTSSSML"))
        End If

        If Not IsPostBack Then
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("X-Arg")) Then
                xArgsData = HttpUtility.UrlEncode(ConfigurationManager.AppSettings("X-Arg"))
                x_arg.Text = ConfigurationManager.AppSettings("X-Arg")
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
    ''' This function invokes api SpeechToText to convert the given wav amr file and displays the result.
    ''' </summary>
    Private Sub TextToSpeech(parEndPoint As String, parURI As String, parAccessToken As String, parXarg As String, parContentType As String, parContent As String)
        Try

            Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create(String.Empty & parEndPoint & parURI), HttpWebRequest)
            httpRequest.Headers.Add("Authorization", "Bearer " & parAccessToken)
            httpRequest.Headers.Add("X-SpeechContext", parXarg)
            httpRequest.ContentLength = parContent.Length
            httpRequest.ContentType = parContentType
            httpRequest.Accept = "audio/x-wav"
            httpRequest.Method = "POST"
            httpRequest.KeepAlive = True

            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(parContent)
            httpRequest.ContentLength = postBytes.Length

            Using writeStream As Stream = httpRequest.GetRequestStream()
                writeStream.Write(postBytes, 0, postBytes.Length)
                writeStream.Close()
            End Using
            Dim speechResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
            Dim offset As Integer = 0
            Dim remaining As Integer = Convert.ToInt32(speechResponse.ContentLength)
            Using stream = speechResponse.GetResponseStream()
                receivedBytes = New Byte(speechResponse.ContentLength - 1) {}
                While remaining > 0
                    Dim read As Integer = stream.Read(receivedBytes, offset, remaining)
                    If read <= 0 Then
                        TTSErrorMessage = [String].Format("End of stream reached with {0} bytes left to read", remaining)
                        Return
                    End If

                    remaining -= read
                    offset += read
                End While
                audioPlay.Attributes.Add("src", "data:audio/wav;base64," & Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None))
                TTSSuccessMessage = "Success"
            End Using
        Catch we As WebException
            Dim errorResponse As String = String.Empty
            Try
                Using sr2 As New StreamReader(we.Response.GetResponseStream())
                    errorResponse = sr2.ReadToEnd()
                    sr2.Close()
                End Using
                TTSErrorMessage = errorResponse
            Catch
                errorResponse = "Unable to get response"
                TTSErrorMessage = errorResponse
            End Try
        Catch ex As Exception
            TTSErrorMessage = ex.Message
            Return
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
#End Region