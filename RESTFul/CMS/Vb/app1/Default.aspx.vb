' <copyright file="Default.aspx.cs" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web.Script.Serialization


#End Region

''' <summary>
''' CallControl App1 class
''' </summary>
Partial Public Class CallControl_App1
    Inherits System.Web.UI.Page
#Region "Local variables"

    ''' <summary>
    ''' Access Token Variables
    ''' </summary>
    Private endPoint As String, accessTokenFilePath As String, apiKey As String, secretKey As String, accessToken As String, accessTokenExpiryTime As String, _
     scope As String, refreshToken As String, refreshTokenExpiryTime As String

    ''' <summary>
    ''' Phone numbers registered for Call Control Service.
    ''' </summary>
    Public phoneNumbers As String

    ''' <summary>
    ''' Script for Call Control Service.
    ''' </summary>
    Private scriptName As String

    ''' <summary>
    ''' Gets or sets the value of refreshTokenExpiresIn
    ''' </summary>
    Private refreshTokenExpiresIn As Integer

    Public createSessionSuccessResponse As String = String.Empty
    Public createSessionErrorResponse As String = String.Empty
    Public sendSignalSuccessResponse As String = String.Empty
    Public sendSignalErrorResponse As String = String.Empty
    Public sessionIdOfCreateSessionResponse As String = String.Empty
    Public successOfCreateSessionResponse As String = String.Empty
    Public statusOfSendSignalResponse As String = String.Empty


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

#Region "SSL Handshake Error"

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

#End Region

#Region "Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' This function reads the web.config and gets the values of the attributes
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Try
            !ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)

            Me.BypassCertificateError()
            Dim ableToRead As Boolean = Me.ReadConfigFile()

            If Not ableToRead Then
                Return
            End If
            If Session("VbRestfulsessionId") IsNot Nothing Then
                sessionIdOfCreateSessionResponse = Session("VbRestfulsessionId").ToString()
            End If
        Catch ex As Exception
            createSessionErrorResponse = ex.ToString()
        End Try
    End Sub

    Private Sub BypassCertificateError()
        Dim bypassSSL As String = ConfigurationManager.AppSettings("IgnoreSSL")
        If (Not String.IsNullOrEmpty(bypassSSL)) AndAlso (String.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)) Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateValidationCallBack)
        End If
    End Sub

    ''' <summary>
    ''' Event that will be triggered when the user clicks on Send Signal button.
    ''' This method will invoke SendSignal API.
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub btnSendSignal_Click(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrEmpty(sessionIdOfCreateSessionResponse) Then
                sendSignalErrorResponse = "Create a session and then send signal"
                Return
            End If

            Dim ableToGetAccessToken As Boolean = Me.ReadAndGetAccessToken(sendSignalErrorResponse)
            If ableToGetAccessToken Then
                Me.SendSignal()
            Else
                sendSignalErrorResponse = "Unable to get access token"
            End If
        Catch ex As Exception
            sendSignalErrorResponse = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' Event that will be triggered when the user clicks on Create Session button.
    ''' This method will invoke CreateSession API.
    ''' </summary>
    ''' <param name="sender">object that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub btnCreateSession_Click(sender As Object, e As EventArgs)
        Try
            Dim ableToGetAccessToken As Boolean = Me.ReadAndGetAccessToken(createSessionErrorResponse)
            If ableToGetAccessToken Then
                Me.CreateSession()
            Else
                createSessionErrorResponse = "Unable to get access token"
            End If
        Catch ex As Exception
            createSessionErrorResponse = ex.Message
        End Try
    End Sub
#End Region

#Region "API Invokation Methods"

    ''' <summary>
    ''' This method creates a Session for an outgoing call or message.
    ''' </summary>
    Private Sub CreateSession()
        Try
            Dim createSessionData As New CreateSessionClass()
            createSessionData.numberToDial = txtNumberToDial.Value
            If Not String.IsNullOrEmpty(scriptType.Value) Then
                createSessionData.feature = scriptType.Value.ToString()
            Else
                createSessionData.feature = String.Empty
            End If
            createSessionData.messageToPlay = txtMessageToPlay.Value.ToString()
            createSessionData.featurenumber = txtNumber.Value.ToString()
            Dim oSerializer As New System.Web.Script.Serialization.JavaScriptSerializer()

            Dim requestParams As String = oSerializer.Serialize(createSessionData)
            Dim createSessionResponse As String
            Dim createSessionRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/1/Sessions"), HttpWebRequest)
            createSessionRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)

            createSessionRequestObject.Method = "POST"
            createSessionRequestObject.ContentType = "application/json"
            createSessionRequestObject.Accept = "application/json"

            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(requestParams)
            createSessionRequestObject.ContentLength = postBytes.Length

            Dim postStream As Stream = createSessionRequestObject.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim createSessionResponseObject As HttpWebResponse = DirectCast(createSessionRequestObject.GetResponse(), HttpWebResponse)
            Using createSessionResponseStream As New StreamReader(createSessionResponseObject.GetResponseStream())
                createSessionResponse = createSessionResponseStream.ReadToEnd()
                If Not String.IsNullOrEmpty(createSessionResponse) Then
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim deserializedJsonObj As CreateSessionResponse = DirectCast(deserializeJsonObject.Deserialize(createSessionResponse, GetType(CreateSessionResponse)), CreateSessionResponse)
                    If deserializedJsonObj IsNot Nothing Then
                        sessionIdOfCreateSessionResponse = deserializedJsonObj.id.ToString()
                        Session("VbRestfulsessionId") = sessionIdOfCreateSessionResponse
                        successOfCreateSessionResponse = deserializedJsonObj.success.ToString()
                    Else
                        createSessionErrorResponse = "Got response but not able to deserialize json" & createSessionResponse
                    End If
                Else
                    createSessionErrorResponse = "Success response but with empty ad"
                End If

                createSessionResponseStream.Close()
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

            createSessionErrorResponse = errorResponse & Environment.NewLine & we.ToString()
        Catch ex As Exception
            createSessionErrorResponse = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' This method sends a Signal to an active Session.
    ''' </summary>
    Private Sub SendSignal()
        Try
            Dim sendSignalResponse As String
            Dim sendSignalRequestObject As HttpWebRequest = DirectCast(System.Net.WebRequest.Create(String.Empty & Me.endPoint & "/rest/1/Sessions/" & sessionIdOfCreateSessionResponse & "/Signals"), HttpWebRequest)
            Dim strReq As String = "{""signal"":""" & signal.Value.ToString() & """}"
            sendSignalRequestObject.Method = "POST"
            sendSignalRequestObject.Headers.Add("Authorization", "Bearer " & Me.accessToken)
            sendSignalRequestObject.ContentType = "application/json"
            sendSignalRequestObject.Accept = "application/json"

            Dim encoding As New UTF8Encoding()
            Dim postBytes As Byte() = encoding.GetBytes(strReq)
            sendSignalRequestObject.ContentLength = postBytes.Length

            Dim postStream As Stream = sendSignalRequestObject.GetRequestStream()
            postStream.Write(postBytes, 0, postBytes.Length)
            postStream.Close()

            Dim sendSignalResponseObject As HttpWebResponse = DirectCast(sendSignalRequestObject.GetResponse(), HttpWebResponse)
            Using sendSignalResponseStream As New StreamReader(sendSignalResponseObject.GetResponseStream())
                sendSignalResponse = sendSignalResponseStream.ReadToEnd()
                If Not String.IsNullOrEmpty(sendSignalResponse) Then
                    Dim deserializeJsonObject As New JavaScriptSerializer()
                    Dim deserializedJsonObj As SendSignalResponse = DirectCast(deserializeJsonObject.Deserialize(sendSignalResponse, GetType(SendSignalResponse)), SendSignalResponse)
                    If deserializedJsonObj IsNot Nothing Then
                        statusOfSendSignalResponse = deserializedJsonObj.status
                    Else
                        sendSignalErrorResponse = "Got response but not able to deserialize json " & sendSignalResponse
                    End If
                Else
                    sendSignalErrorResponse = "No response from the gateway."
                End If

                sendSignalResponseStream.Close()
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

            sendSignalErrorResponse = errorResponse & Environment.NewLine & we.ToString()
        Catch ex As Exception
            sendSignalErrorResponse = ex.ToString()
        End Try
    End Sub

    ''' <summary>
    ''' This method displays the contents of call.js file onto create session textarea.
    ''' </summary>
    Private Sub GetOutboundScriptContent()
        Dim streamReader As StreamReader = Nothing
        Try
            Dim scrFile As String = Request.MapPath(scriptName)
            streamReader = New StreamReader(scrFile)
            Dim javaScript As String = streamReader.ReadToEnd()
            txtCreateSession.Value = "Following is the Java Script Code: " & System.Environment.NewLine & javaScript
        Catch ex As Exception
            createSessionErrorResponse = ex.Message
        Finally
            If streamReader IsNot Nothing Then
                streamReader.Close()
            End If
        End Try
    End Sub

#End Region

#Region "Access Token Methods"

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
            If File.Exists(Request.MapPath(Me.accessTokenFilePath)) Then
                fileStream = New FileStream(Request.MapPath(Me.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read)
                streamReader = New StreamReader(fileStream)
                Me.accessToken = streamReader.ReadLine()
                Me.accessTokenExpiryTime = streamReader.ReadLine()
                Me.refreshToken = streamReader.ReadLine()
                Me.refreshTokenExpiryTime = streamReader.ReadLine()
            End If
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
    ''' Reads from session variables and gets access token.
    ''' </summary>
    ''' <param name="panelParam">Panel Details</param>
    ''' <returns>true/false; true on successfully getting access token, else false</returns>
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
    ''' Get access token based on the type parameter type values.
    ''' </summary>
    ''' <param name="type">If type value is Authorization_code, access token is fetch for authorization code flow
    ''' If type value is Refresh_Token, access token is fetch for authorization code floww based on the exisiting refresh token</param>
    ''' <param name="panelParam">Panel to display status message</param>
    ''' <returns>true/false; true if success, else false</returns>  
    Private Function GetAccessToken(type As AccessType, ByRef message As String) As Boolean
        Dim fileStream As FileStream = Nothing
        Dim postStream As Stream = Nothing
        Dim streamWriter As StreamWriter = Nothing

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

        Return False
    End Function

    ''' <summary>
    ''' Read parameters from configuraton file and assigns to local variables.
    ''' </summary>
    ''' <returns>true/false; true if all required parameters are specified, else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.endPoint = ConfigurationManager.AppSettings("endPoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            createSessionErrorResponse = "endPoint is not defined in configuration file"
            Return False
        End If

        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            createSessionErrorResponse = "apiKey is not defined in configuration file"
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            createSessionErrorResponse = "secretKey is not defined in configuration file"
            Return False
        End If

        Me.phoneNumbers = ConfigurationManager.AppSettings("phoneNumbers")
        If String.IsNullOrEmpty(Me.phoneNumbers) Then
            createSessionErrorResponse = "phoneNumbers parameter is not defined in configuration file"
            Return False
        End If

        Me.scriptName = ConfigurationManager.AppSettings("scriptName")
        If String.IsNullOrEmpty(Me.scriptName) Then
            createSessionErrorResponse = "scriptName parameter is not defined in configuration file"
            Return False
        End If
        GetOutboundScriptContent()
        Me.scope = ConfigurationManager.AppSettings("scope")
        If String.IsNullOrEmpty(Me.scope) Then
            Me.scope = "CCS"
        End If

        Me.accessTokenFilePath = ConfigurationManager.AppSettings("AccessTokenFilePath")
        If String.IsNullOrEmpty(Me.accessTokenFilePath) Then
            Me.accessTokenFilePath = "~\CCSApp1AccessToken.txt"
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


        Return True
    End Function

#End Region

End Class

#Region "Data Structures"

''' <summary>
''' Access Token Data Structure
''' </summary>
Public Class CreateSessionClass
    ''' <summary>
    ''' Gets or sets numberToDial
    ''' </summary>
    Public Property numberToDial() As String
        Get
            Return m_numberToDial
        End Get
        Set(value As String)
            m_numberToDial = Value
        End Set
    End Property
    Private m_numberToDial As String

    ''' <summary>
    ''' Gets or sets messageToPlay
    ''' </summary>
    Public Property messageToPlay() As String
        Get
            Return m_messageToPlay
        End Get
        Set(value As String)
            m_messageToPlay = Value
        End Set
    End Property
    Private m_messageToPlay As String

    ''' <summary>
    ''' Gets or sets featureNumber
    ''' </summary>
    Public Property featurenumber() As String
        Get
            Return m_featurenumber
        End Get
        Set(value As String)
            m_featurenumber = Value
        End Set
    End Property
    Private m_featurenumber As String

    ''' <summary>
    ''' Gets or sets feature
    ''' </summary>
    Public Property feature() As String
        Get
            Return m_feature
        End Get
        Set(value As String)
            m_feature = Value
        End Set
    End Property
    Private m_feature As String
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
''' Create Session Response object
''' </summary>
Public Class CreateSessionResponse
    ''' <summary>
    ''' Gets or sets the value of id.
    ''' The SessionID for the newly created session.
    ''' </summary>
    Public Property id() As String
        Get
            Return m_id
        End Get
        Set(value As String)
            m_id = Value
        End Set
    End Property
    Private m_id As String

    ''' <summary>
    ''' Gets or sets a value indicating whether or not the token launch was successful.
    ''' </summary>
    Public Property success() As Boolean
        Get
            Return m_success
        End Get
        Set(value As Boolean)
            m_success = Value
        End Set
    End Property
    Private m_success As Boolean
End Class

''' <summary>
''' Send Signal Response object
''' </summary>
Public Class SendSignalResponse
    ''' <summary>
    ''' Gets or sets the value of status.
    ''' Returns the status of the request. Possible values are:
    ''' QUEUED - The event delivered successfully to the event queue of the target.
    ''' NOTFOUND - The target session could not be found.
    ''' FAILED - Some other failure occurred.
    ''' </summary>
    Public Property status() As String
        Get
            Return m_status
        End Get
        Set(value As String)
            m_status = Value
        End Set
    End Property
    Private m_status As String
End Class

#End Region