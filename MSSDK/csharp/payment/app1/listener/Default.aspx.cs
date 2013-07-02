// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'AT&T SDK Tools Agreement' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using ATT_MSSDK;
using ATT_MSSDK.Paymentv3;

#endregion

/// <summary>
///  PaymentApp1_Listener class to process request and response of notification objects.
/// </summary>
public partial class PaymentApp1_Listener : System.Web.UI.Page
{
    #region Instance Variables

    /// <summary>
    /// Instance variable to hold the location of notification details file.
    /// </summary>
    private string notificationFilePath;

    /// <summary>
    /// Local variables for processing of request stream
    /// </summary>
    private string apiKey, endPoint, secretKey;

    /// <summary>
    /// RequestFactory Instance to call payment notification methods.
    /// </summary>
    private RequestFactory requestFactory;

    #endregion

    #region Application Event

    /// <summary>
    /// The Page_Load event is triggered when a page loads, and ASP.NET will automatically call the subroutine Page_Load, and execute the code inside it.
    /// </summary>
    /// <param name="sender">an object that raised the event</param>
    /// <param name="e">Type EventArgs</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            bool ableToRead = this.ReadConfigFile();

            if (!ableToRead)
            {
                return;
            }

            // Instantiate RequestFactory Instance.
            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.Payment);
            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);

            List<NotificationId> notificationIds = this.requestFactory.GetNotificationIds(Request.InputStream);

            this.SaveNotificationDetails(notificationIds);
        }
        catch (InvalidResponseException ie)
        {
            this.LogError(ie.Body);
        }
        catch (Exception ex)
        {
            this.LogError(ex.Message);
        }
    }

    #endregion

    #region Listener specific functions

    /// <summary>
    /// Reads from config file and instantiates local variables
    /// </summary>
    /// <returns>true/false; true if able to read from config else false</returns>
    private bool ReadConfigFile()
    {
        this.apiKey = ConfigurationManager.AppSettings["api_key"].ToString();
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.LogError("api_key not defined");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"].ToString();
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.LogError("endPoint not defined");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"].ToString();
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.LogError("secret_key not defined");
            return false;
        }

        this.notificationFilePath = ConfigurationManager.AppSettings["notificationFilePath"];
        if (string.IsNullOrEmpty(this.notificationFilePath))
        {
            this.notificationFilePath = "~\\notificationDetailsFile.txt";
        }

        return true;
    }

    /// <summary>
    /// Logs error message onto file
    /// </summary>
    /// <param name="text">Text to be logged</param>
    private void LogError(string text)
    {
        File.AppendAllText(Request.MapPath("errorInNotification.txt"), Environment.NewLine + DateTime.Now.ToString() + ": " + text);
    }

    /// <summary>
    /// Saves notification details to file for each notification Id.
    /// </summary>
    /// <param name="listOfNotificationIds">List of NotificationId objects</param>
    private void SaveNotificationDetails(List<NotificationId> listOfNotificationIds)
    {
        List<string> notificationIds = new List<string>();
        NotificationObject notificationObject;
        string notificationType = string.Empty;
        string originalTransactionId = string.Empty;
        string notificationId = string.Empty;

        foreach (NotificationId notificationIdObject in listOfNotificationIds)
        {
            notificationId = notificationIdObject.Id;
            notificationObject = this.requestFactory.GetNotification(notificationId);
            try
            {
                AcknowledgeNotificationResponse response = this.requestFactory.AcknowledgeNotifications(notificationId);
            }
            catch (InvalidResponseException ie)
            {
                this.LogError(ie.Body);
            }
            catch (Exception ex)
            {
                this.LogError(ex.Message);
            }

            notificationType = notificationObject.NotificationObjectType.ToString();

            if (notificationType.Equals(NotificationType.CancelSubscription.ToString()))
            {
                CancelSubscriptionNotificationObject cancelSubscriptionNotificationObject = (CancelSubscriptionNotificationObject)notificationObject;
                originalTransactionId = cancelSubscriptionNotificationObject.OriginalTransactionId;
            }

            if (notificationType.Equals(NotificationType.FreePeriodConversion.ToString()))
            {
                FreePeriodConversionNotificationObject freePeriodConversionNotificationObject = (FreePeriodConversionNotificationObject)notificationObject;
                originalTransactionId = freePeriodConversionNotificationObject.OriginalTransactionId;
            }

            if (notificationType.Equals(NotificationType.SubscriptionRecurrence.ToString()))
            {
                SubscriptionRecurrenceNotificationObject subscriptionRecurrenceNotificationObject = (SubscriptionRecurrenceNotificationObject)notificationObject;
                originalTransactionId = subscriptionRecurrenceNotificationObject.OriginalTransactionId;
            }

            if (notificationType.Equals(NotificationType.StopSubscription.ToString()))
            {
                StopSubscriptionNotificationObject stopSubscriptionNotificationObject = (StopSubscriptionNotificationObject)notificationObject;
                originalTransactionId = stopSubscriptionNotificationObject.OriginalTransactionId;
            }

            if (notificationType.Equals(NotificationType.SuccesfulRefund.ToString()))
            {
                SuccessfulRefundNotificationObject successfulRefundObject = (SuccessfulRefundNotificationObject)notificationObject;
                originalTransactionId = successfulRefundObject.OriginalTransactionId;
            }

            if (notificationType.Equals(NotificationType.RestoreSubscription.ToString()))
            {
                RestoreSubscriptionNotificationObject restoreSubscriptionNotificationObject = (RestoreSubscriptionNotificationObject)notificationObject;
                originalTransactionId = string.Empty;
            }

            string detailsToSave = string.Format("{0}:{1}:{2}$", notificationId, notificationType, originalTransactionId);
            this.SaveDetailsToFie(detailsToSave);
        }
    }

    /// <summary>
    /// Logs error message onto file
    /// </summary>
    /// <param name="details">Details to be logged</param>
    private void SaveDetailsToFie(string details)
    {
        File.AppendAllText(Request.MapPath(this.notificationFilePath), details);
    }

    #endregion
}