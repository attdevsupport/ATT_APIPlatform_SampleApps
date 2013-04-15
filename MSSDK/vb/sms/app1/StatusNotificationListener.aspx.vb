' <copyright file="StatusNotificationListener.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports ATT_MSSDK
Imports ATT_MSSDK.SMSv3
#End Region

Partial Public Class StatusNotificationListener
    Inherits System.Web.UI.Page
#Region "Variable Declaration"

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private deiveryStatusFilePath As String = String.Empty

    ''' <summary>
    ''' Global Variable Declaration
    ''' </summary>
    Private numOfDeiveryStatus As Integer = 0

#End Region
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Me.deiveryStatusFilePath = ConfigurationManager.AppSettings("deiveryStatusFilePath")
        If String.IsNullOrEmpty(Me.deiveryStatusFilePath) Then
            Me.deiveryStatusFilePath = "DeliveryStatus.txt"
        End If

        Dim numOfDeiveryStatusToStore As String = ConfigurationManager.AppSettings("numberOfDeliveryStatusToStore")
        If String.IsNullOrEmpty(numOfDeiveryStatusToStore) Then
            Me.numOfDeiveryStatus = Convert.ToInt32(numOfDeiveryStatusToStore)
        End If

        Try
            Dim inputStream As System.IO.Stream = Request.InputStream

            Dim deliveryStatus As SmsDeliveryStatus = RequestFactory.GetSMSDeliveryStatus(inputStream)

            If deliveryStatus IsNot Nothing Then
                Me.SaveMessage(deliveryStatus)
            End If
        Catch ae As ArgumentException

            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ae.Message & Environment.NewLine)
        Catch ex As Exception
            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ex.ToString() & Environment.NewLine)
        End Try

    End Sub

    Private Sub SaveMessage(status As SmsDeliveryStatus)
        Try
            Dim list As New List(Of String)()
            Dim fileStr As New FileStream(Request.MapPath(Me.deiveryStatusFilePath), FileMode.OpenOrCreate, FileAccess.Read)
            Dim sr As New StreamReader(fileStr)

            Dim line As String
            While sr.Peek() >= 0
                line = sr.ReadLine()
                list.Add(line)
            End While
            sr.Close()
            fileStr.Close()

            Dim statusInfoToStore As String = Convert.ToString(status.DeliveryInfoNotification.MessageId) & "_-_-" & Convert.ToString(status.DeliveryInfoNotification.DeliveryInfo.Address) & "_-_-" & Convert.ToString(status.DeliveryInfoNotification.DeliveryInfo.DeliveryStatus)
            list.Add(statusInfoToStore)

            Using sw As StreamWriter = System.IO.File.CreateText(Request.MapPath(Me.deiveryStatusFilePath))
                Dim tempCount As Integer = 0
                While tempCount < list.Count
                    Dim lineToWrite As String = list(tempCount)
                    sw.WriteLine(lineToWrite)
                    tempCount += 1
                End While
                sw.Close()
            End Using
        Catch ex As Exception
            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ex.ToString() & Environment.NewLine)
        End Try
    End Sub
End Class