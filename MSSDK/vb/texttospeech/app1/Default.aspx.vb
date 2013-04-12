
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.TextToSpeechv1
Imports System.Configuration
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.IO
Imports System.Media

''' <summary>
''' TextToSpeech_App1 Class
''' </summary>
Partial Public Class TextToSpeech_App1
    Inherits System.Web.UI.Page
    '* \addtogroup TextToSpeech_App1
    '   * Description of the application can be referred at \ref TextToSpeech_App1 example
    '   * @{
    '   


    '* \example TextToSpeech_App1 texttospeech\app1\Default.aspx.cs
    '     * \n \n This application allows a user to convert text to speech.
    '     * <ul>
    '     * <li>SDK methods showcased by the application:</li>
    '     * \n \n <b>Text To Speech:</b>
    '     * \n This method converts text to synthesized voice and returns the result in standard binary audio formats.
    '     * \n \n Steps followed in the app to invoke the method:
    '     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.TextToSpeechv1 NameSpace.</li>
    '     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
    '     * <li>Invoke \c TextToSpeech() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
    '     * \n Sample code:
    '     * <pre>
    '     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
    '     *    scopes.Add(RequestFactory.ScopeTypes.TTS);
    '     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
    '     *    TextToSpeechResponse response = requestFactory.TextToSpeech(textToConvert, contentType, contentLanguage, "audio/wav", xArgs);</pre>
    '     * 
    '     * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
    '     * 
    '     * \n <li>Documentation can be referred at \ref TextToSpeech_App1 section</li></ul>
    '     * @{
    '     

#Region "Class Variables and Constructor"
    ''' <summary>
    ''' Request Factory object for calling api functions
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Application Service specific variables
    ''' </summary>
    Private apiKey As String, secretKey As String, endPoint As String

    ''' <summary>
    ''' Flag for deletion of the temporary file
    ''' </summary>
    Private deleteFile As Boolean

    ''' <summary>
    ''' X-Arg parameter
    ''' A meta parameter to define multiple parameters within a single HTTP header.
    ''' </summary>
    Private xArgData As String

#End Region

#Region "Application Events"

    ''' <summary>
    ''' This function is called when the applicaiton page is loaded into the browser.
    ''' </summary>
    ''' <param name="sender">Button that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        BypassCertificateError()
        If Not Page.IsPostBack Then
            resultsPanel.Visible = False
            Me.Initialize()
            Dim contentLangs As String = ConfigurationManager.AppSettings("ContentLanguage")
            If Not String.IsNullOrEmpty(contentLangs) Then
                Dim speechContexts As String() = contentLangs.Split(";"c)
                For Each speechContext As String In speechContexts
                    ddlContentLanguage.Items.Add(speechContext)
                Next
            Else
                ddlContentLanguage.Items.Add("en-US")
            End If

            ddlContentLanguage.Items(0).Selected = True
            Dim contentTypes As String = ConfigurationManager.AppSettings("ContentType")
            If Not String.IsNullOrEmpty(contentTypes) Then
                Dim contentType As String() = contentTypes.Split(";"c)
                For Each type As String In contentType
                    ddlContentType.Items.Add(type)
                Next
            Else
                ddlContentType.Items.Add("text/plain")
            End If

            ddlContentType.Items(0).Selected = True
            txtXArgs.Text = Me.xArgData.Replace(",", "," & Environment.NewLine)
        End If

        Dim currentServerTime As DateTime = DateTime.UtcNow
        lblServerTime.Text = [String].Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) & " UTC"
    End Sub

    ''' <summary>
    ''' Method that calls SpeechToText method of RequestFactory when user clicked on submit button
    ''' </summary>
    ''' <param name="sender">sender that invoked this event</param>
    ''' <param name="e">eventargs of the button</param>
    Protected Sub SpeechToTextButton_Click(sender As Object, e As EventArgs)
        Try
            resultsPanel.Visible = False
            Me.Initialize()

            Dim response As TextToSpeechResponse = Me.requestFactory.TextToSpeech(txtTextToConvert.Text, ddlContentType.Text, ddlContentLanguage.Text, "audio/wav", xArgData)
            If response IsNot Nothing Then
                resultsPanel.Visible = True
                Me.DisplayResult(response)

            End If
        Catch invalidscope As InvalidScopeException
            Me.DrawPanelForFailure(statusPanel, invalidscope.Message)
        Catch argex As ArgumentException
            Me.DrawPanelForFailure(statusPanel, argex.Message)
        Catch ie As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ie.Body)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.Message)
        End Try
    End Sub

#End Region

#Region "TextToSpeech service functions"

    ''' <summary>
    ''' Initializes a new instance of the Speech_app1 class. This constructor reads from Config file and initializes Request Factory object
    ''' </summary>
    Private Sub Initialize()

        If Me.requestFactory Is Nothing Then
            Me.ReadConfigAndInitialize()
        End If
    End Sub

    ''' <summary>
    ''' Display success message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForSuccess(panelParam As Panel, message As String)
        If panelParam IsNot Nothing AndAlso panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "successWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellTwo.Text = message.ToString()
        rowTwo.Controls.Add(rowTwoCellTwo)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(panelParam As Panel, message As String)
        If panelParam IsNot Nothing AndAlso panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "errorWide"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "ERROR:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Text = message.ToString()
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Displays the result onto the page
    ''' </summary>
    ''' <param name="speechResponse">SpeechResponse received from api</param>
    Private Sub DisplayResult(speechResponse As TextToSpeechResponse)
        If speechResponse IsNot Nothing Then
            lblContentLength.Text = speechResponse.ContentLength.ToString()
            lblContentType.Text = speechResponse.ContentType
            If speechResponse.SpeechContent IsNot Nothing Then
                Try
                    audioPlay.Attributes.Add("src", "data:audio/wav;base64," + Convert.ToBase64String(speechResponse.SpeechContent, Base64FormattingOptions.None))
                Catch ex As Exception
                    Me.DrawPanelForFailure(resultsPanel, ex.Message)
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' Neglect the ssl handshake error with authentication server 
    ''' </summary>
    Private Shared Sub BypassCertificateError()
        ServicePointManager.ServerCertificateValidationCallback = Function(sender1 As [Object], certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
    End Sub

    ''' <summary>
    ''' Read from config and initialize RequestFactory object
    ''' </summary>
    ''' <returns>true/false; true - if able to read from config file; else false</returns>
    Private Function ReadConfigAndInitialize() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("apiKey")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(statusPanel, "apiKey is not defined in the config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secretKey")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(statusPanel, "secretKey is not defined in the config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(statusPanel, "endpoint is not defined in the config file")
            Return False
        End If

        Me.xArgData = ConfigurationManager.AppSettings("X-Arg")

        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(RequestFactory.ScopeTypes.TTS)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)
        Return True
    End Function

#End Region

    '* }@ 

    '* }@ 

End Class