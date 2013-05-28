' <copyright file="Listener.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
#End Region

''' <summary>
''' Listener class for saving message counts.
''' </summary>
Partial Public Class Listener
    Inherits System.Web.UI.Page
#Region "variables"
    Public receivedMessagesFilePath As String = String.Empty
    Public numberOfMessagesToStore As Integer = 0
#End Region
#Region "Events"
    ''' <summary>
    ''' This method called when the page is loaded into the browser. This method requests input stream and parses it to get message counts.
    ''' </summary>
    ''' <param name="sender">object, which invoked this method</param>
    ''' <param name="e">EventArgs, which specifies arguments specific to this method</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)

        Dim stream As System.IO.Stream = Request.InputStream

        Me.receivedMessagesFilePath = ConfigurationManager.AppSettings("ReceivedMessagesFilePath")
        If String.IsNullOrEmpty(Me.receivedMessagesFilePath) Then
            Me.receivedMessagesFilePath = "~\Messages.txt"
        End If

        Dim count As String = ConfigurationManager.AppSettings("NumberOfMessagesToStore")
        If Not String.IsNullOrEmpty(count) Then
            Me.numberOfMessagesToStore = Convert.ToInt32(count)
        End If

        If stream IsNot Nothing Then
            Dim bytes As Byte() = New Byte(stream.Length - 1) {}
            stream.Position = 0
            stream.Read(bytes, 0, CInt(stream.Length))
            Dim responseData As String = Encoding.ASCII.GetString(bytes)

            Dim serializeObject As New JavaScriptSerializer()
            Dim message As InboundSMSMessage = DirectCast(serializeObject.Deserialize(responseData, GetType(InboundSMSMessage)), InboundSMSMessage)

            If message IsNot Nothing Then
                Me.SaveMessage(message)
            End If
        End If
    End Sub
#End Region

#Region "Method to store the received message to file"
    ''' <summary>
    ''' This method reads the incoming message and stores the received message details.
    ''' </summary>
    ''' <param name="message">InboundSMSMessage, message received from Request</param>
    Private Sub SaveMessage(message As InboundSMSMessage)
        Try
            Dim list As New List(Of String)()
            Dim file__1 As New FileStream(Request.MapPath(Me.receivedMessagesFilePath), FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file__1)
            Dim line As String

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                list.Add(line)
            End While

            sr.Close()
            file__1.Close()

            If list.Count > Me.numberOfMessagesToStore Then
                Dim diff As Integer = list.Count - Me.numberOfMessagesToStore
                list.RemoveRange(0, diff)
            End If

            If list.Count = Me.numberOfMessagesToStore Then
                list.RemoveAt(0)
            End If

            Dim messageLineToStore As String = message.DateTime.ToString() & "_-_-" & message.MessageId.ToString() & "_-_-" & message.Message.ToString() & "_-_-" & message.SenderAddress.ToString() & "_-_-" & message.DestinationAddress.ToString()
            list.Add(messageLineToStore)
            Using sw As StreamWriter = File.CreateText(Request.MapPath(Me.receivedMessagesFilePath))
                Dim tempCount As Integer = 0
                While tempCount < list.Count
                    Dim lineToWrite As String = list(tempCount)
                    sw.WriteLine(lineToWrite)
                    tempCount += 1
                End While
                sw.Close()
            End Using
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
#End Region

End Class

#Region "Message Structure"
''' <summary>
''' Message structure received
''' </summary>
Public Class InboundSMSMessage
    ''' <summary>
    ''' Gets or sets the value of DateTime
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
    ''' Gets or sets the value of MessageId
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
    ''' Gets or sets the value of Message
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
    ''' Gets or sets the value of SenderAddress
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

    ''' <summary>
    ''' Gets or sets the value of DestinationAddress
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
End Class
#End Region