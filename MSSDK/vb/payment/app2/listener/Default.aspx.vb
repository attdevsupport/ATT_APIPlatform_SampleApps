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
Imports ATT_MSSDK
Imports ATT_MSSDK.Paymentv3

#End Region

''' <summary>
'''  PaymentApp1_Listener class to process request and response of notification objects.
''' </summary>
Partial Public Class PaymentApp2_Listener
    Inherits System.Web.UI.Page
#Region "Instance Variables"

    ''' <summary>
    ''' Instance variable to hold the location of notification details file.
    ''' </summary>
    Private notificationFilePath As String

    ''' <summary>
    ''' Local variables for processing of request stream
    ''' </summary>
    Private apiKey As String, endPoint As String, secretKey As String

    ''' <summary>
    ''' RequestFactory Instance to call payment notification methods.
    ''' </summary>
    Private requestFactory As RequestFactory

#End Region

#Region "Application Event"

    ''' <summary>
    ''' The Page_Load event is triggered when a page loads, and ASP.NET will automatically call the subroutine Page_Load, and execute the code inside it.
    ''' </summary>
    ''' <param name="sender">an object that raised the event</param>
    ''' <param name="e">Type EventArgs</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim ableToRead As Boolean = Me.ReadConfigFile()

            If Not ableToRead Then
                Return
            End If

            ' Instantiate RequestFactory Instance.
            Dim scopes As New List(Of RequestFactory.ScopeTypes)()
            scopes.Add(requestFactory.ScopeTypes.Payment)
            Me.requestFactory = New RequestFactory(Me.endPoint, Me.apiKey, Me.secretKey, scopes, Nothing, Nothing)

            Dim notificationIds As List(Of NotificationId) = Me.requestFactory.GetNotificationIds(Request.InputStream)

            Me.SaveNotificationDetails(notificationIds)
        Catch ie As InvalidResponseException
            Me.LogError(ie.Body)
        Catch ex As Exception
            Me.LogError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Listener specific functions"

    ''' <summary>
    ''' Reads from config file and instantiates local variables
    ''' </summary>
    ''' <returns>true/false; true if able to read from config else false</returns>
    Private Function ReadConfigFile() As Boolean
        Me.apiKey = ConfigurationManager.AppSettings("api_key").ToString()
        If String.IsNullOrEmpty(Me.apiKey) Then
            Me.LogError("api_key not defined")
            Return False
        End If

        Me.endPoint = ConfigurationManager.AppSettings("endPoint").ToString()
        If String.IsNullOrEmpty(Me.endPoint) Then
            Me.LogError("endPoint not defined")
            Return False
        End If

        Me.secretKey = ConfigurationManager.AppSettings("secret_key").ToString()
        If String.IsNullOrEmpty(Me.secretKey) Then
            Me.LogError("secret_key not defined")
            Return False
        End If

        Me.notificationFilePath = ConfigurationManager.AppSettings("notificationFilePath")
        If String.IsNullOrEmpty(Me.notificationFilePath) Then
            Me.notificationFilePath = "~\notificationDetailsFile.txt"
        End If

        Return True
    End Function

    ''' <summary>
    ''' Logs error message onto file
    ''' </summary>
    ''' <param name="text">Text to be logged</param>
    Private Sub LogError(text As String)
        File.AppendAllText(Request.MapPath("errorInNotification.txt"), Environment.NewLine & DateTime.Now.ToString() & ": " & text)
    End Sub

    ''' <summary>
    ''' Saves notification details to file for each notification Id.
    ''' </summary>
    ''' <param name="listOfNotificationIds">List of NotificationId objects</param>
    Private Sub SaveNotificationDetails(listOfNotificationIds As List(Of NotificationId))
        Dim notificationIds As New List(Of String)()
        Dim notificationObject As NotificationObject
        Dim notificationType__1 As String = String.Empty
        Dim originalTransactionId As String = String.Empty
        Dim notificationId As String = String.Empty

        For Each notificationIdObject As NotificationId In listOfNotificationIds
            notificationId = notificationIdObject.Id
            notificationObject = Me.requestFactory.GetNotification(notificationId)
            Try
                Dim response As AcknowledgeNotificationResponse = Me.requestFactory.AcknowledgeNotifications(notificationId)
            Catch ie As InvalidResponseException
                Me.LogError(ie.Body)
            Catch ex As Exception
                Me.LogError(ex.Message)
            End Try

            notificationType__1 = notificationObject.NotificationObjectType.ToString()

            If notificationType__1.Equals(NotificationType.CancelSubscription.ToString()) Then
                Dim cancelSubscriptionNotificationObject As CancelSubscriptionNotificationObject = DirectCast(notificationObject, CancelSubscriptionNotificationObject)
                originalTransactionId = cancelSubscriptionNotificationObject.OriginalTransactionId
            End If

            If notificationType__1.Equals(NotificationType.FreePeriodConversion.ToString()) Then
                Dim freePeriodConversionNotificationObject As FreePeriodConversionNotificationObject = DirectCast(notificationObject, FreePeriodConversionNotificationObject)
                originalTransactionId = freePeriodConversionNotificationObject.OriginalTransactionId
            End If

            If notificationType__1.Equals(NotificationType.SubscriptionRecurrence.ToString()) Then
                Dim subscriptionRecurrenceNotificationObject As SubscriptionRecurrenceNotificationObject = DirectCast(notificationObject, SubscriptionRecurrenceNotificationObject)
                originalTransactionId = subscriptionRecurrenceNotificationObject.OriginalTransactionId
            End If

            If notificationType__1.Equals(NotificationType.StopSubscription.ToString()) Then
                Dim stopSubscriptionNotificationObject As StopSubscriptionNotificationObject = DirectCast(notificationObject, StopSubscriptionNotificationObject)
                originalTransactionId = stopSubscriptionNotificationObject.OriginalTransactionId
            End If

            If notificationType__1.Equals(NotificationType.SuccesfulRefund.ToString()) Then
                Dim successfulRefundObject As SuccessfulRefundNotificationObject = DirectCast(notificationObject, SuccessfulRefundNotificationObject)
                originalTransactionId = successfulRefundObject.OriginalTransactionId
            End If

            If notificationType__1.Equals(NotificationType.RestoreSubscription.ToString()) Then
                Dim restoreSubscriptionNotificationObject As RestoreSubscriptionNotificationObject = DirectCast(notificationObject, RestoreSubscriptionNotificationObject)
                originalTransactionId = String.Empty
            End If

            Dim detailsToSave As String = String.Format("{0}:{1}:{2}$", notificationId, notificationType__1, originalTransactionId)
            Me.SaveDetailsToFie(detailsToSave)
        Next
    End Sub

    ''' <summary>
    ''' Logs error message onto file
    ''' </summary>
    ''' <param name="details">Details to be logged</param>
    Private Sub SaveDetailsToFie(details As String)
        File.AppendAllText(Request.MapPath(Me.notificationFilePath), details)
    End Sub

#End Region
End Class