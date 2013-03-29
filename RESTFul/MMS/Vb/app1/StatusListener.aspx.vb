' <copyright file="Listener.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
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
Partial Public Class StatusListener
    Inherits System.Web.UI.Page
#Region "variables"
    Public receivedDeliveryStatusFilePath As String = String.Empty
    Public numberOfDeliveryStatusToStore As Integer = 0
#End Region
#Region "Events"
    ''' <summary>
    ''' This method called when the page is loaded into the browser. This method requests input stream and parses it to get message counts.
    ''' </summary>
    ''' <param name="sender">object, which invoked this method</param>
    ''' <param name="e">EventArgs, which specifies arguments specific to this method</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)

        Dim stream As System.IO.Stream = Request.InputStream

        Me.receivedDeliveryStatusFilePath = ConfigurationManager.AppSettings("ReceivedDeliveryStatusFilePath")
        If String.IsNullOrEmpty(Me.receivedDeliveryStatusFilePath) Then
            Me.receivedDeliveryStatusFilePath = "~\DeliveryStatus.txt"
        End If

        Dim count As String = ConfigurationManager.AppSettings("NumberOfDeliveryStatusToStore")
        If Not String.IsNullOrEmpty(count) Then
            Me.numberOfDeliveryStatusToStore = Convert.ToInt32(count)
        Else
            Me.numberOfDeliveryStatusToStore = 5
        End If

        If stream IsNot Nothing Then
            Dim bytes As Byte() = New Byte(stream.Length - 1) {}
            stream.Position = 0
            stream.Read(bytes, 0, CInt(stream.Length))
            Dim responseData As String = Encoding.ASCII.GetString(bytes)
            Dim serializeObject As New JavaScriptSerializer()
            Dim message As DeliveryStatusNotification = DirectCast(serializeObject.Deserialize(responseData, GetType(DeliveryStatusNotification)), DeliveryStatusNotification)

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
    Private Sub SaveMessage(message As DeliveryStatusNotification)
        Try
            Dim list As New List(Of String)()
            If File.Exists(Request.MapPath(Me.receivedDeliveryStatusFilePath)) Then
                Dim file__1 As New FileStream(Request.MapPath(Me.receivedDeliveryStatusFilePath), FileMode.Open, FileAccess.Read)
                Dim sr As New StreamReader(file__1)
                Dim line As String

                While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                    list.Add(line)
                End While

                sr.Close()
                file__1.Close()
            End If

            If list.Count > Me.numberOfDeliveryStatusToStore Then
                Dim diff As Integer = list.Count - Me.numberOfDeliveryStatusToStore
                list.RemoveRange(0, diff)
            End If

            If list.Count = Me.numberOfDeliveryStatusToStore Then
                list.RemoveAt(0)
            End If

            Dim messageLineToStore As String = message.deliveryInfoNotification.messageId.ToString() & "_-_-" & message.deliveryInfoNotification.deliveryInfo.address.ToString() & "_-_-" & message.deliveryInfoNotification.deliveryInfo.deliveryStatus.ToString()
            list.Add(messageLineToStore)
            Using sw As StreamWriter = File.CreateText(Request.MapPath(Me.receivedDeliveryStatusFilePath))
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
Public Class DeliveryInfoNotification
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
Public Class DeliveryStatusNotification
    ''' <summary>
    ''' Gets or sets the list of messageId.
    ''' </summary>
    Public Property deliveryInfoNotification() As DeliveryInfoNotification
        Get
            Return m_deliveryInfoNotification
        End Get
        Set(value As DeliveryInfoNotification)
            m_deliveryInfoNotification = Value
        End Set
    End Property
    Private m_deliveryInfoNotification As DeliveryInfoNotification
End Class


#End Region