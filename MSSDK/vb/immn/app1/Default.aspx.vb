' <copyright file="Default.aspx.vb" company="AT&amp;T Intellectual Property">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Drawing
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.IMMNv1
#End Region

''' <summary>
''' This application allows the user to send SMS and MMS on behalf of subscriber, 
''' with subscriber’s consent, using the IMMN API.
''' </summary>
Partial Public Class MOBO_App1
    Inherits System.Web.UI.Page

    '* \addtogroup IMMN_App1
    '* Description of the application can be referred at \ref IMMN_app1 example
    '* @{
    '* 
    '*  \example IMMN_app1 immn\app1\Default.aspx.vb
    '* \n \n This application allows the user to send SMS and MMS on behalf of subscriber, with subscriber's consent, using the IMMN API.
    '*  
    '* <b>Send Message:</b>
    '* \li Import \c ATT_MSSDK and \c ATT_MSSDK.IMMNv1 NameSpace.
    '* \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
    '* Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
    '* \li Invoke \c SendIMMN() exposed in the \c RequestFactory class of MS SDK library.
    '* 
    '* <b>Sample code:</b>
    '* <pre>
    '*    Dim scopes As New List(Of RequestFactory.ScopeTypes)()
    '*    scopes.Add(requestFactory.ScopeTypes.IMMN)   
    '*    Dim requestFactory As New RequestFactory(endPoint, apiKey, secretKey, scopes, redirectUrl, Nothing)
    '*    Dim response As SendMessageResponse = requestFactory.SendIMMN(msgToSend.Addresses, msgToSend.Attachments, msgToSend.Message, msgToSend.Subject, msgToSend.Group)
    '* </pre>
    '*
    '* Installing and running the application, refer \ref Application 
    '* \n \n <b>Parameters in web.config</b> refer \ref parameters_sec section
    '* 
    '* \n Documentation can be referred at \ref IMMN_App1 section
    '* @{

#Region "Instance Variables"
    ''' <summary>
    ''' Request Factory object for calling api functions
    ''' </summary>
    Private requestFactory As RequestFactory = Nothing

    ''' <summary>
    ''' Application Service specific variables
    ''' </summary>
    Private authCode As String, apiKey As String, secretKey As String, endPoint As String, redirectUrl As String

    ''' <summary>
    ''' Maximum number of addresses user can specify
    ''' </summary>
    Private maxAddresses As Integer
    
#End Region

#Region "Application Events"

    ''' <summary>
    ''' This menthod is called when the page is loading
    ''' </summary>
    ''' <param name="sender">object pointing to the caller</param>
    ''' <param name="e">Event arguments</param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim currentServerTime As DateTime = DateTime.UtcNow
        lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC"

        If Me.requestFactory Is Nothing Then
            Dim ableToReadConfigFile = Me.ReadConfigFile()
            If ableToReadConfigFile = False Then
                Return
            End If

            Me.InitializeRequestFactory()
        End If

        If Not Page.IsPostBack Then
            If Session("VBMOBO_ACCESS_TOKEN") Is Nothing Then
                If Not String.IsNullOrEmpty(Request("code")) Then
                    Me.authCode = Request("code")
                    Me.requestFactory.GetAuthorizeCredentials(Me.authCode)
                    Session("VBMOBO_ACCESS_TOKEN") = Me.requestFactory.AuthorizeCredential
                End If
            End If

            If Session("VB_MessageToSend") IsNot Nothing Then
                Dim msg As MessageStructure = DirectCast(Session("VB_MessageToSend"), MessageStructure)
                txtMessage.Text = msg.Message
                Dim temp As String = String.Empty

                For Each addr As String In msg.Addresses
                    temp += addr & ","
                Next

                If Not String.IsNullOrEmpty(temp) Then
                    temp = temp.Substring(0, temp.LastIndexOf(","))
                End If

                txtPhone.Text = temp
                txtSubject.Text = msg.Subject
                chkGroup.Checked = msg.Group
                Me.SendMessage(msg)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Method will be called when the user clicks on send message button. This method calls SendIMMN function of Request Factory
    ''' </summary>
    ''' <param name="sender">object pointing to the caller</param>
    ''' <param name="e">Event arguments</param>
    Protected Sub BtnSendMessage_Click(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrEmpty(txtPhone.Text) Then
            Me.DrawPanelForFailure(statusPanel, "Address field cannot be blank")
            Return
        End If

        Dim msg As MessageStructure = Me.GetUserInput()
        If msg IsNot Nothing Then
            If msg.Addresses IsNot Nothing Then
                If (msg.Addresses.Count > Me.maxAddresses) Then
                    Me.DrawPanelForFailure(statusPanel, "Maximum addresses can not exceed " + Me.maxAddresses.ToString())
                    Return
                End If

                If (msg.Attachments Is Nothing Or msg.Attachments.Count = 0) Then
                    ' Message is mandatory, if no attachments
                    If (String.IsNullOrEmpty(txtMessage.Text)) Then
                        Me.DrawPanelForFailure(statusPanel, "Specify message/attachment to be sent")
                        Return
                    End If
                End If
                Me.SendMessage(msg)
            End If
        End If
    End Sub
#End Region

#Region "Send Message Functions"

    '''<summary>
    ''' This method reads from Config file and assigns to local variables.
    ''' </summary>
    ''' <returns>true/false; true if able to read from config; else false.</returns>
    Private Function ReadConfigFile() As Boolean

        Me.apiKey = ConfigurationManager.AppSettings("api_key")
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.DrawPanelForFailure(statusPanel, "api_key is not specified in Config file")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key")
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.DrawPanelForFailure(statusPanel, "secret_key is not specified in Config file")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endpoint")
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.DrawPanelForFailure(statusPanel, "endpoint is not specified in Config file")
            Return False
        End If

        Me.redirectUrl = ConfigurationManager.AppSettings("authorize_redirect_uri")
        If String.IsNullOrEmpty(Me.redirectUrl) Then
            Me.DrawPanelForFailure(statusPanel, "authorize_redirect_uri is not specified in Config file")
            Return False
        End If

        Me.maxAddresses = 10
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("max_addresses")) Then
            Me.maxAddresses = Convert.ToInt32(ConfigurationManager.AppSettings("max_addresses"))
        End If

        Return True
    End Function

    ''' <summary>
    ''' Initialize a new instance of RequestFactory.
    ''' </summary>
    Private Sub InitializeRequestFactory()
        Dim scopes As New List(Of RequestFactory.ScopeTypes)()
        scopes.Add(requestFactory.ScopeTypes.IMMN)

        Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Me.redirectUrl, Nothing)

        If Session("VBMOBO_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBMOBO_ACCESS_TOKEN"), OAuthToken)
        End If
    End Sub

    ''' <summary>
    ''' Prepares a list based on a string seperated by comma(,)
    ''' </summary>
    ''' <param name="strToList">String value to convert to list</param>
    ''' <returns>List of strings</returns>
    Private Function GetList(ByVal strToList As String) As List(Of String)
        Dim list As New List(Of String)()
        Dim ls As String() = strToList.Split(","c)
        For Each value As String In ls
            list.Add(value)
        Next

        Return list
    End Function

    ''' <summary>
    ''' Gets Message object based on user input
    ''' </summary>
    ''' <returns>Message object</returns>
    Private Function GetUserInput() As MessageStructure

        Dim msg As MessageStructure = New MessageStructure
        msg.Attachments = New List(Of String)

        If Not String.IsNullOrEmpty(fileUpload1.FileName) Then
            fileUpload1.SaveAs(Request.MapPath(fileUpload1.FileName))
            msg.Attachments.Add(Request.MapPath(fileUpload1.FileName))
        End If

        If Not String.IsNullOrEmpty(fileUpload2.FileName) Then
            fileUpload2.SaveAs(Request.MapPath(fileUpload2.FileName))
            msg.Attachments.Add(Request.MapPath(fileUpload2.FileName))
        End If

        If Not String.IsNullOrEmpty(fileUpload3.FileName) Then
            fileUpload3.SaveAs(Request.MapPath(fileUpload3.FileName))
            msg.Attachments.Add(Request.MapPath(fileUpload3.FileName))
        End If

        If Not String.IsNullOrEmpty(fileUpload4.FileName) Then
            fileUpload4.SaveAs(Request.MapPath(fileUpload4.FileName))
            msg.Attachments.Add(Request.MapPath(fileUpload4.FileName))
        End If

        If Not String.IsNullOrEmpty(fileUpload5.FileName) Then
            fileUpload5.SaveAs(Request.MapPath(fileUpload5.FileName))
            msg.Attachments.Add(Request.MapPath(fileUpload5.FileName))
        End If

        msg.Message = txtMessage.Text
        msg.Subject = txtSubject.Text
        msg.Group = chkGroup.Checked
        msg.Addresses = Me.GetList(txtPhone.Text)
        Session("VB_MessageToSend") = msg
        Return msg
    End Function

    ''' <summary>
    ''' Initiates SendMessage call to Request Factory
    ''' </summary>
    ''' <param name="msgToSend">Message object</param>
    Private Sub SendMessage(ByVal msgToSend As MessageStructure)
        If Session("VBMOBO_ACCESS_TOKEN") IsNot Nothing Then
            Me.requestFactory.AuthorizeCredential = DirectCast(Session("VBMOBO_ACCESS_TOKEN"), OAuthToken)
        End If

        If Me.requestFactory.AuthorizeCredential Is Nothing Then
            Response.Redirect(Me.requestFactory.GetOAuthRedirect().ToString())
        End If

        Try
            Dim messageResponse As SendMessageResponse = Me.requestFactory.SendIMMN(msgToSend.Addresses, msgToSend.Attachments, msgToSend.Message, msgToSend.Subject, msgToSend.Group)
            If messageResponse IsNot Nothing Then
                Me.DrawPanelForSuccess(statusPanel, messageResponse.Id)
            End If

            For Each file As String In msgToSend.Attachments
                If (System.IO.File.Exists(file)) Then
                    System.IO.File.Delete(file)
                End If
            Next
            Session("VB_MessageToSend") = Nothing

        Catch te As TokenExpiredException
            Me.DrawPanelForFailure(statusPanel, te.Message)
        Catch ae As ArgumentException
            Me.DrawPanelForFailure(statusPanel, ae.Message)
        Catch ire As InvalidResponseException
            Me.DrawPanelForFailure(statusPanel, ire.Body)
        Catch ur As UnauthorizedRequest
            Me.DrawPanelForFailure(statusPanel, ur.Message)
        Catch ex As Exception
            Me.DrawPanelForFailure(statusPanel, ex.ToString())
        End Try
    End Sub
#End Region

#Region "Display Status Functions"

    ''' <summary>
    ''' Display success message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForSuccess(ByVal panelParam As Panel, ByVal message As String)
        If panelParam.HasControls() Then
            panelParam.Controls.Clear()
        End If

        Dim table As New Table()
        table.CssClass = "success"
        table.Font.Name = "Sans-serif"
        table.Font.Size = 9
        Dim rowOne As New TableRow()
        Dim rowOneCellOne As New TableCell()
        rowOneCellOne.Font.Bold = True
        rowOneCellOne.Text = "SUCCESS:"
        rowOne.Controls.Add(rowOneCellOne)
        table.Controls.Add(rowOne)
        Dim rowTwo As New TableRow()
        Dim rowTwoCellOne As New TableCell()
        rowTwoCellOne.Font.Bold = True
        rowTwoCellOne.Text = "Message ID:"
        rowTwoCellOne.Width = Unit.Pixel(70)
        rowTwo.Controls.Add(rowTwoCellOne)
        Dim rowTwoCellTwo As New TableCell()
        rowTwoCellTwo.Text = message
        rowTwo.Controls.Add(rowTwoCellTwo)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub

    ''' <summary>
    ''' Displays error message
    ''' </summary>
    ''' <param name="panelParam">Panel to draw success message</param>
    ''' <param name="message">Message to display</param>
    Private Sub DrawPanelForFailure(ByVal panelParam As Panel, ByVal message As String)
        If panelParam.HasControls() Then
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
        rowTwoCellOne.Text = message
        rowTwo.Controls.Add(rowTwoCellOne)
        table.Controls.Add(rowTwo)
        panelParam.Controls.Add(table)
    End Sub
#End Region

#Region "Message Data Structure"
    ''' <summary>
    ''' message container for sending sms/mms
    ''' </summary>
    <Serializable()> _
    Private Class MessageStructure
        ''' <summary>
        ''' Gets or sets List of addresses
        ''' </summary>
        Public Property Addresses() As List(Of String)
            Get
                Return m_Addresses
            End Get
            Set(ByVal value As List(Of String))
                m_Addresses = value
            End Set
        End Property
        Private m_Addresses As List(Of String)

        ''' <summary>
        ''' Gets or sets List of attachments
        ''' </summary>
        Public Property Attachments() As List(Of String)
            Get
                Return m_Attachments
            End Get
            Set(ByVal value As List(Of String))
                m_Attachments = value
            End Set
        End Property
        Private m_Attachments As List(Of String)

        ''' <summary>
        ''' Gets or sets Message to be sent
        ''' </summary>
        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal value As String)
                m_Message = value
            End Set
        End Property
        Private m_Message As String

        ''' <summary>
        ''' Gets or sets Subject of the message
        ''' </summary>
        Public Property Subject() As String
            Get
                Return m_Subject
            End Get
            Set(ByVal value As String)
                m_Subject = value
            End Set
        End Property
        Private m_Subject As String

        ''' <summary>
        ''' Gets or sets a value indicating whether message to be sent to group or individual
        ''' </summary>
        Public Property Group() As Boolean
            Get
                Return m_Group
            End Get
            Set(ByVal value As Boolean)
                m_Group = value
            End Set
        End Property
        Private m_Group As Boolean
    End Class
#End Region
    '* }@
    '* }@        
End Class