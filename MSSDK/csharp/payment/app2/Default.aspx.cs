// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.Paymentv3;

#endregion

/* 
' * This Application demonstrates usage of  payment related methods exposed by AT&T MS SDK wrapper library
' * for creating a new subscription, getting the subscription status, refunding the subscription and 
' * viewing the notifications received from the platform.
' *  
' * Application provides option for creating a new subscription, viewing the status of subscription, refunding
' * 5 latest subscription and viewing latest 5 notifications from the platform.
' *   
' * Pre-requisite:
' * -------------
' * The developer has to register his application in AT&T Developer Platform website, for the Payment scope 
' * of AT&T service. AT&T Developer Platform website provides a ClientId and client secret on registering 
' * the application.
' * Developper has configured the merchantRedirectURL to point to his application in AT&T Developer Platform 
' * website.
' * 
' * Steps to be followed by the application to invoke Payment APIs exposed by MS SDK wrapper library:
' * -------------------------------------------------------------------------------------------------
' * 1. Import ATT_MSSDK and ATT_MSSDK.Paymentv3 NameSpace.
' * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' *    the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' *    while creating RequestFactory instance.
' *
' *  Note: Scopes that are not configured for your application will not work.
' *  For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' *  The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' * 
' * 3.Invoke Payment related APIs exposed in the RequestFactory class of MS SDK library.
' *    
' *   i. - Invoke GetNewSubscriptionRedirect()on RequestFactory by passing subscription related parameters like amount, 
' *        product description, to get subscriptionRedirect url. 
' *
' *       -  Redirect the user to AT&T platform subscription endpoint.
' *
' *        AT&T platform thows a login page and authenticates the user credentials and requests the user 
' *        for authorizing the subscription.
' *        Once user authorizes the payment subcription, AT&T platform performs the payment subscription and
' *        returns the control back to application passing 'SubscriptionAuthCode' as a query string.
' *
' *    ii. Application can use 'SubscriptionAuthCode' to invoke GetSubscriptionStatus() on RequestFactory
' *        to get the status of subscription.
'       
' *    iii. Application can invoke Refund() on RequestFactory by passing the subscription Id and refund reason.
'
' *    iv.  Application can invoke CancelSubscription() on RequestFactory by passing the subscriptionId and refundReason to cancel a subscription.
' *         
 */

/// <summary>
/// Payment App2 class.
/// This application provides option for creating a new subscription, viewing the status of subscription, refunding
/// 5 latest subscription and viewing latest 5 notifications from the platform.
/// </summary>
public partial class Payment_App2 : System.Web.UI.Page
{
    /** \addtogroup Payment_App2
     * Description of the application can be referred at \ref Payment_app2 example
     * @{
     */

    /** \example Payment_app2 payment\app2\Default.aspx.cs
     * \n \n This application allows the user to 
     * \li Make a new subscription to buy product 1 or product 2
     * \li Get the subscription status
     * \li Refund any of the latest five subscriptions
     * \li View the latest five notifications
     * 
     * <b>Using Payment Methods:</b>
     * \li Import \c ATT_MSSDK and \c ATT_MSSDK.Paymentv3 NameSpace.
     * \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
     * \li Invoke GetNewSubscriptionRedirect()on RequestFactory by passing subscription related parameters like amount, product description, to get subscriptionRedirect url. 
     * \li Redirect the user to AT&T platform subscription endpoint.
     * \li AT&T platform thows a login page and authenticates the user credentials and requests the user for authorizing the payment subscription.
     * \li Once user authorizes the payment subscription, AT&T platform performs the payment subscription and returns the control back to application passing 'SubscriptionAuthCode' as a query string.
     * \li Application can use 'SubscriptionAuthCode' to invoke \c GetSubscriptionStatus() on \c RequestFactory to get the status of subscription.
     * \li Application can invoke \c Refund() on \c RequestFactory by passing the subscription Id and refund reason.
     * 
     * \n For Registration, Installation, Configuration and Execution, refer \ref Application
    * \n \n <b>Additional configuration to be done:</b>
    * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
    * \li MerchantPaymentRedirectUrl - Set to the URL pointing to the application. ATT platform uses this URL to return the control back to the application after subscription processing is completed.
     * \li subscriptionDetailsFilePath - This is optional parameter, which points to the file path, where subscription details will be stored by the application.
     * \li subscriptionRefundFilePath - This is optional parameter, which points to the file path, where latest subscription IDs will be stored.
     * \li notificationFilePath - This is optional parameter, which points to the file path, where latest notification details will be stored.
     * \li notificationCountToDisplay - This is optional key, which will allow to display the defined number of notifications.
     * 
     * \n Documentation can be referred at \ref Payment_App2 section
     * @{
    */

    #region Instance Variables

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string apiKey, secretKey, endPoint;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string description, merchantTransactionId, merchantProductId, merchantApplicationId, merchantRedirectURI,
        transactionTimeString, notificationDetailsFile, refundFilePath;

    /// <summary>
    /// Local Variables
    /// </summary>
    private Table successTable, failureTable, successTableGetTransaction, successTableGetSubscriptionDetails, successTableSubscriptionRefund;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private double amount;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int noOfNotificationsToDisplay;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private DateTime transactionTime;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string subscriptionDetailsFilePath;

    /// <summary>
    /// Local Variables
    /// </summary>
    private bool latestFive = true;

    /// <summary>
    /// Local Variables
    /// </summary>
    private int subsDetailsCountToDisplay = 0;

    /// <summary>
    /// Local Variables
    /// </summary>
    private Table notificationDetailsTable;

    /// <summary>
    /// Local Variables
    /// </summary>
    private List<KeyValuePair<string, string>> subsDetailsList = new List<KeyValuePair<string, string>>();

    /// <summary>
    /// Local Variables
    /// </summary>
    private List<KeyValuePair<string, string>> subsRefundList = new List<KeyValuePair<string, string>>();

    #endregion

    #region Payment App1 events

    /// <summary>
    /// Event that gets triggered when the page is loaded initially into the browser.
    /// This method will read all config parameters and initializes RequestFactory instance, 
    /// Creates refund radio buttons and processes subscription response.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();

            subsRefundSuccessTable.Visible = false;
            subsDetailsSuccessTable.Visible = false;
            subscriptionSuccessTable.Visible = false;
            subsGetStatusTable.Visible = false;

            DateTime currentServerTime = DateTime.UtcNow;
            lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

            bool ableToInitialize = this.Initialize();
            if (ableToInitialize == false)
            {
                return;
            }

            if ((Request["SubscriptionAuthCode"] != null) && (Session["sub_merTranId"] != null))
            {
                this.ProcessCreateSubscriptionResponse();
            }
            else if ((Request["shown_notary"] != null) && (Session["sub_processNotary"] != null))
            {
                Session["sub_processNotary"] = null;
                GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: " + Session["sub_tempMerTranId"].ToString();
                GetSubscriptionAuthCode.Text = "Auth Code: " + Session["sub_TranAuthCode"].ToString();
            }

            subsDetailsTable.Controls.Clear();
            this.DrawSubsDetailsSection(false);
            subsRefundTable.Controls.Clear();
            this.DrawSubsRefundSection(false);
            this.DrawNotificationTableHeaders();
            this.GetNotificationDetails();
        }
        catch 
        { 
        }
    }

    /// <summary>
    /// This method gets subscription redirect Url and redirects the user for getting consent.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void NewSubscriptionButton_Click1(object sender, EventArgs e)
    {
        try
        {
            this.ReadTransactionParametersFromConfigurationFile();
            string subscriptionRedirect = this.requestFactory.GetNewSubscriptionRedirect(this.amount, PaymentCategories.ApplicationGames, this.description, this.merchantTransactionId, this.merchantProductId, this.merchantRedirectURI);
            Response.Redirect(subscriptionRedirect);
        }
        catch (InvalidResponseException ire)
        {
            this.DrawPanelForFailure(newSubscriptionPanel, ire.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(newSubscriptionPanel, ex.ToString());
        }
    }

    /// <summary>
    /// View notary button click event
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void ViewNotaryButton_Click(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Get subscription details for a selected subscription Id
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void GetSubscriptionButton_Click(object sender, EventArgs e)
    {
        try
        {
            SubscriptionStatus subscriptionStatus = null;
            string keyValue = string.Empty;
            if (Radio_SubscriptionStatus.SelectedIndex == 0)
            {
                keyValue = Radio_SubscriptionStatus.SelectedItem.Value.ToString().Replace("Merchant Transaction ID: ", string.Empty);
                subscriptionStatus = this.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.MerchantTransactionId, keyValue);
            }

            if (Radio_SubscriptionStatus.SelectedIndex == 1)
            {
                keyValue = Radio_SubscriptionStatus.SelectedItem.Value.ToString().Replace("Auth Code: ", string.Empty);
                subscriptionStatus = this.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.SubscriptionAuthCode, keyValue);
            }

            if (Radio_SubscriptionStatus.SelectedIndex == 2)
            {
                subscriptionStatus = this.requestFactory.GetSubscriptionStatus(SubscriptionIdTypes.SubscriptionId, Session["subscriptionId"].ToString());
            }

            Session["subscriptionId"] = subscriptionStatus.Id;
            GetSubscriptionID.Text = "Subscription ID: " + subscriptionStatus.Id;

            if (this.CheckItemInSubsDetailsFile(subscriptionStatus.MerchantSubscriptionId, subscriptionStatus.ConsumerId) == false)
            {
                this.WriteSubsDetailsToFile(subscriptionStatus.MerchantSubscriptionId, subscriptionStatus.ConsumerId);
            }

            if (this.CheckItemInSubsRefundFile(subscriptionStatus.Id, subscriptionStatus.MerchantSubscriptionId) == false)
            {
                this.WriteSubsRefundToFile(subscriptionStatus.Id, subscriptionStatus.MerchantSubscriptionId);
            }

            subsDetailsTable.Controls.Clear();
            this.DrawSubsDetailsSection(false);
            subsRefundTable.Controls.Clear();
            this.DrawSubsRefundSection(false);
            subsGetStatusTable.Visible = true;

            this.DrawPanelForGetSubscriptionSuccess(getSubscriptionStatusPanel);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Amount", subscriptionStatus.Amount.ToString());
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Channel", subscriptionStatus.Channel);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "ConsumerId", subscriptionStatus.ConsumerId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "ContentCategory", subscriptionStatus.ContentCategory);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Currency", subscriptionStatus.Currency);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Description", subscriptionStatus.Description);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "IsAutoCommitted", subscriptionStatus.IsAutoCommitted);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "IsSuccess", subscriptionStatus.IsSuccess.ToString());
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantApplicationId", subscriptionStatus.MerchantApplicationId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantId", subscriptionStatus.MerchantId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantProductId", subscriptionStatus.MerchantProductId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantSubscriptionId", subscriptionStatus.MerchantSubscriptionId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "MerchantTransactionId", subscriptionStatus.MerchantTransactionId);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "OriginalTransactionId", string.Empty);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionId", subscriptionStatus.Id);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionPeriod", subscriptionStatus.SubscriptionPeriod);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionPeriodAmount", subscriptionStatus.PeriodAmount.ToString());
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionRecurrences", subscriptionStatus.Recurrences);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionStatus", subscriptionStatus.Status);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "SubscriptionType", subscriptionStatus.Type);
            this.AddRowToGetSubscriptionSuccessPanel(getSubscriptionStatusPanel, "Version", subscriptionStatus.Version);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(getSubscriptionStatusPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(getSubscriptionStatusPanel, ex.ToString());
        }
    }

    /// <summary>
    /// Get subscription details button click event
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void BtnGetSubscriptionDetails_Click(object sender, EventArgs e)
    {
        string merSubsID = string.Empty;
        bool recordFound = false;
        try
        {
            if (this.subsDetailsList.Count > 0)
            {
                foreach (Control subDetailsTableRow in subsDetailsTable.Controls)
                {
                    if (subDetailsTableRow is TableRow)
                    {
                        foreach (Control subDetailsTableRowCell in subDetailsTableRow.Controls)
                        {
                            if (subDetailsTableRowCell is TableCell)
                            {
                                foreach (Control subDetailsTableCellControl in subDetailsTableRowCell.Controls)
                                {
                                    if (subDetailsTableCellControl is RadioButton)
                                    {
                                        if (((RadioButton)subDetailsTableCellControl).Checked)
                                        {
                                            merSubsID = ((RadioButton)subDetailsTableCellControl).Text.ToString();
                                            recordFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (recordFound == true)
                {
                    SubscriptionDetails subscriptionDetails = null;
                    string consID = this.GetValueOfKey(merSubsID);
                    if (string.IsNullOrEmpty(consID))
                    {
                        return;
                    }

                    subscriptionDetails = this.requestFactory.GetSubscriptionDetails(merSubsID, consID);

                    subsDetailsSuccessTable.Visible = true;

                    this.DrawPanelForGetSubscriptionDetailsSuccess(subsDetailsPanel);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CreationDate", subscriptionDetails.CreationDate);                    
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Currency", subscriptionDetails.Currency);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CurrentEndDate", subscriptionDetails.CurrentEndDate);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "CurrentStartDate", subscriptionDetails.CurrentStartDate);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "GrossAmount", subscriptionDetails.GrossAmount);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "IsActiveSubscription", subscriptionDetails.IsActiveSubscription);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "IsSuccess", subscriptionDetails.IsSuccess);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Recurrences", subscriptionDetails.Recurrences);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "RecurrencesLeft", subscriptionDetails.RecurrencesLeft);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Status", subscriptionDetails.Status);
                    this.AddRowToGetSubscriptionDetailsSuccessPanel(getSubscriptionStatusPanel, "Version", subscriptionDetails.Version);
                }
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(subsDetailsPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(subsDetailsPanel, ex.ToString());
        }
    }

    /// <summary>
    /// Refunds a subscription.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void BtnGetSubscriptionRefund_Click(object sender, EventArgs e)
    {
        string subsID = string.Empty;
        bool recordFound = false;

        try
        {
            if (this.subsRefundList.Count > 0)
            {
                foreach (Control subRefundTableRow in subsRefundTable.Controls)
                {
                    if (subRefundTableRow is TableRow)
                    {
                        foreach (Control subRefundTableRowCell in subRefundTableRow.Controls)
                        {
                            if (subRefundTableRowCell is TableCell)
                            {
                                foreach (Control subRefundTableCellControl in subRefundTableRowCell.Controls)
                                {
                                    if (subRefundTableCellControl is RadioButton)
                                    {
                                        if (((RadioButton)subRefundTableCellControl).Checked)
                                        {
                                            subsID = ((RadioButton)subRefundTableCellControl).Text.ToString();
                                            recordFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (recordFound == true)
                {   
                    if (btnGetSubscriptionRefund.Text.ToLower().StartsWith("cancel"))
                    {
                        CancelSubscriptionResponse cancelSubscriptionResponse = this.requestFactory.CancelSubscription(subsID, 1, "Customer was not happy");

                        subsRefundSuccessTable.Visible = true;
                        this.DrawPanelForSubscriptionRefundSuccess(subsRefundPanel);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "IsSuccess", cancelSubscriptionResponse.IsSuccess == null ? string.Empty : cancelSubscriptionResponse.IsSuccess.ToString());
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "OriginalPurchaseAmount", cancelSubscriptionResponse.OriginalPurchaseAmount == null ? string.Empty : cancelSubscriptionResponse.OriginalPurchaseAmount.ToString());
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionId", cancelSubscriptionResponse.TransactionId);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionStatus", cancelSubscriptionResponse.TransactionStatus);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "Version", cancelSubscriptionResponse.Version);
                    }
                    else
                    {
                        RefundResponseObject refundResponse = this.requestFactory.Refund(subsID, 1, "Customer was not happy");
                        subsRefundSuccessTable.Visible = true;

                        this.DrawPanelForSubscriptionRefundSuccess(subsRefundPanel);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "IsSuccess", refundResponse.IsSuccess == null ? string.Empty : refundResponse.IsSuccess.ToString());
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "OriginalPurchaseAmount", refundResponse.OriginalPurchaseAmount == null ? string.Empty : refundResponse.OriginalPurchaseAmount.ToString());
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionId", refundResponse.TransactionId);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionStatus", refundResponse.TransactionStatus);
                        this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "Version", refundResponse.Version);
                    }

                    if (this.latestFive == false)
                    {
                        this.subsRefundList.RemoveAll(x => x.Key.Equals(subsID));
                        this.UpdatesSubsRefundListToFile();
                        this.ResetSubsRefundList();
                        subsRefundTable.Controls.Clear();
                        this.DrawSubsRefundSection(false);
                        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: ";
                        GetSubscriptionAuthCode.Text = "Auth Code: ";
                        GetSubscriptionID.Text = "Subscription ID: ";
                    }
                }
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(subsRefundPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(subsRefundPanel, ex.ToString());
        }
    }

    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of arguments</param>
    protected void BtnCancelSubscription_Click(object sender, EventArgs e)
    {
        string subsID = string.Empty;
        bool recordFound = false;

        try
        {
            if (this.subsRefundList.Count > 0)
            {
                foreach (Control subRefundTableRow in subsRefundTable.Controls)
                {
                    if (subRefundTableRow is TableRow)
                    {
                        foreach (Control subRefundTableRowCell in subRefundTableRow.Controls)
                        {
                            if (subRefundTableRowCell is TableCell)
                            {
                                foreach (Control subRefundTableCellControl in subRefundTableRowCell.Controls)
                                {
                                    if (subRefundTableCellControl is RadioButton)
                                    {
                                        if (((RadioButton)subRefundTableCellControl).Checked)
                                        {
                                            subsID = ((RadioButton)subRefundTableCellControl).Text.ToString();
                                            recordFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (recordFound == true)
                {
                    CancelSubscriptionResponse cancelSubscriptionResponse = this.requestFactory.CancelSubscription(subsID, 1, "Customer was not happy");

                    subsRefundSuccessTable.Visible = true;
                    this.DrawPanelForSubscriptionRefundSuccess(subsRefundPanel);
                    this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "IsSuccess", cancelSubscriptionResponse.IsSuccess == null ? string.Empty : cancelSubscriptionResponse.IsSuccess.ToString());
                    this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "OriginalPurchaseAmount", cancelSubscriptionResponse.OriginalPurchaseAmount == null ? string.Empty : cancelSubscriptionResponse.OriginalPurchaseAmount.ToString());
                    this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionId", cancelSubscriptionResponse.TransactionId);
                    this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "TransactionStatus", cancelSubscriptionResponse.TransactionStatus);
                    this.AddRowToSubscriptionRefundSuccessPanel(subsRefundPanel, "Version", cancelSubscriptionResponse.Version);

                    if (this.latestFive == false)
                    {
                        this.subsRefundList.RemoveAll(x => x.Key.Equals(subsID));
                        this.UpdatesSubsRefundListToFile();
                        this.ResetSubsRefundList();
                        subsRefundTable.Controls.Clear();
                        this.DrawSubsRefundSection(false);
                        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: ";
                        GetSubscriptionAuthCode.Text = "Auth Code: ";
                        GetSubscriptionID.Text = "Subscription ID: ";
                    }
                }
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(subsRefundPanel, ie.Message.ToString());
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(subsRefundPanel, ex.Message.ToString());
        }
    }

    /// <summary>
    /// Refresh notification messages
    /// </summary>
    /// <param name="sender">Sender Details</param>
    /// <param name="e">List of Arguments</param>
    protected void BtnRefreshNotifications_Click(object sender, EventArgs e)
    {
        this.notificationDetailsTable.Controls.Clear();
        this.DrawNotificationTableHeaders();
        this.GetNotificationDetails();           
    }

    #endregion

    #region Payment App2 specific functions

    /// <summary>
    /// This function is used to neglect the ssl handshake error with authentication server.
    /// </summary>
    private static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(Object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    /// <summary>
    /// Method to read Transaction Parameters from Configuration file.
    /// </summary>
    private void ReadTransactionParametersFromConfigurationFile()
    {
        this.transactionTime = DateTime.UtcNow;
        this.transactionTimeString = String.Format("{0:dddMMMddyyyyHHmmss}", this.transactionTime);
        if (Radio_SubscriptionProductType.SelectedIndex == 0)
        {
            this.amount = 1.99;
        }
        else if (Radio_SubscriptionProductType.SelectedIndex == 1)
        {
            this.amount = 3.99;
        }

        this.description = "TrDesc" + this.transactionTimeString;
        this.merchantTransactionId = "TrId" + this.transactionTimeString;
        Session["sub_merTranId"] = this.merchantTransactionId;
        this.merchantProductId = "ProdId" + this.transactionTimeString;
        this.merchantApplicationId = "MerAppId" + this.transactionTimeString;
    }

    /// <summary>
    /// Method to get the value of key from the selected row in Refund Section
    /// </summary>
    /// <param name="key">Key Value to be found</param>
    /// <returns>Returns the value in String</returns>
    private string GetValueOfKeyFromRefund(string key)
    {
        int tempCount = 0;
        while (tempCount < this.subsRefundList.Count)
        {
            if (this.subsRefundList[tempCount].Key.CompareTo(key) == 0)
            {
                return this.subsRefundList[tempCount].Value;
            }

            tempCount++;
        }

        return string.Empty;
    }

    /// <summary>
    /// Method to get the value from Key value
    /// </summary>
    /// <param name="key">Key Value to be found</param>
    /// <returns>Returns the value in String</returns>
    private string GetValueOfKey(string key)
    {
        int tempCount = 0;
        while (tempCount < this.subsDetailsList.Count)
        {
            if (this.subsDetailsList[tempCount].Key.CompareTo(key) == 0)
            {
                return this.subsDetailsList[tempCount].Value;
            }

            tempCount++;
        }

        return string.Empty;
    }

    /// <summary>
    /// Method to add row to success table.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">Attribute as String</param>
    /// <param name="value">Value as String</param>
    private void AddRowToSubscriptionRefundSuccessPanel(Panel panelParam, string attribute, string value)
    {
        TableRow row = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Right;
        cellOne.Text = attribute;
        cellOne.Width = Unit.Pixel(300);
        row.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.Width = Unit.Pixel(50);
        row.Controls.Add(cellTwo);
        TableCell cellThree = new TableCell();
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Text = value;
        cellThree.Width = Unit.Pixel(300);
        row.Controls.Add(cellThree);
        this.successTableSubscriptionRefund.Controls.Add(row);
    }

    /// <summary>
    /// Method to draw panel for successful refund.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForSubscriptionRefundSuccess(Panel panelParam)
    {
        this.successTableSubscriptionRefund = new Table();
        this.successTableSubscriptionRefund.Font.Name = "Sans-serif";
        this.successTableSubscriptionRefund.Font.Size = 8;
        this.successTableSubscriptionRefund.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right;
        rowOneCellOne.Text = "Parameter";
        rowOneCellOne.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellOne);
        TableCell rowOneCellTwo = new TableCell();
        rowOneCellTwo.Width = Unit.Pixel(50);
        rowOne.Controls.Add(rowOneCellTwo);

        TableCell rowOneCellThree = new TableCell();
        rowOneCellThree.Font.Bold = true;
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellThree.Text = "Value";
        rowOneCellThree.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellThree);
        this.successTableSubscriptionRefund.Controls.Add(rowOne);
        panelParam.Controls.Add(this.successTableSubscriptionRefund);
    }

    /// <summary>
    /// Method to draw panel for successful transaction.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForGetSubscriptionDetailsSuccess(Panel panelParam)
    {
        this.successTableGetSubscriptionDetails = new Table();
        this.successTableGetSubscriptionDetails.Font.Name = "Sans-serif";
        this.successTableGetSubscriptionDetails.Font.Size = 8;
        this.successTableGetSubscriptionDetails.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right;
        rowOneCellOne.Text = "Parameter";
        rowOneCellOne.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellOne);
        TableCell rowOneCellTwo = new TableCell();
        rowOneCellTwo.Width = Unit.Pixel(50);
        rowOne.Controls.Add(rowOneCellTwo);

        TableCell rowOneCellThree = new TableCell();
        rowOneCellThree.Font.Bold = true;
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellThree.Text = "Value";
        rowOneCellThree.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellThree);
        this.successTableGetSubscriptionDetails.Controls.Add(rowOne);
        panelParam.Controls.Add(this.successTableGetSubscriptionDetails);
    }

    /// <summary>
    /// Method to add row to success table.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">Attribute as String</param>
    /// <param name="value">Value as String</param>
    private void AddRowToGetSubscriptionDetailsSuccessPanel(Panel panelParam, string attribute, string value)
    {
        TableRow row = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Right;
        cellOne.Text = attribute;
        cellOne.Width = Unit.Pixel(300);
        row.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.Width = Unit.Pixel(50);
        row.Controls.Add(cellTwo);
        TableCell cellThree = new TableCell();
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Text = value;
        cellThree.Width = Unit.Pixel(300);
        row.Controls.Add(cellThree);
        this.successTableGetSubscriptionDetails.Controls.Add(row);
    }

    /// <summary>
    /// Method to update Subscription Refund list to the file.
    /// </summary>
    private void UpdatesSubsRefundListToFile()
    {
        if (this.subsRefundList.Count != 0)
        {
            this.subsRefundList.Reverse(0, this.subsRefundList.Count);
        }

        using (StreamWriter sr = File.CreateText(Request.MapPath(this.refundFilePath)))
        {
            int tempCount = 0;
            while (tempCount < this.subsRefundList.Count)
            {
                string lineToWrite = this.subsRefundList[tempCount].Key + ":-:" + this.subsRefundList[tempCount].Value;
                sr.WriteLine(lineToWrite);
                tempCount++;
            }

            sr.Close();
        }
    }

    /// <summary>
    /// Method to update Subscription Details list to the file.
    /// </summary>
    private void UpdateSubsDetailsListToFile()
    {
        if (this.subsDetailsList.Count != 0)
        {
            this.subsDetailsList.Reverse(0, this.subsDetailsList.Count);
        }

        using (StreamWriter sr = File.CreateText(this.subscriptionDetailsFilePath))
        {
            int tempCount = 0;
            while (tempCount < this.subsDetailsList.Count)
            {
                string lineToWrite = this.subsDetailsList[tempCount].Key + ":-:" + this.subsDetailsList[tempCount].Value;
                sr.WriteLine(lineToWrite);
                tempCount++;
            }

            sr.Close();
        }
    }

    /// <summary>
    /// Method to check item in Subscription Refund file.
    /// </summary>
    /// <param name="transactionid">Transaction Id details</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id details</param>
    /// <returns>Returns True or False</returns>
    private bool CheckItemInSubsRefundFile(string transactionid, string merchantTransactionId)
    {
        string line;
        string lineToFind = transactionid + ":-:" + merchantTransactionId;
        if (File.Exists(Request.MapPath(this.refundFilePath)))
        {
            System.IO.StreamReader file = new System.IO.StreamReader(Request.MapPath(this.refundFilePath));
            while ((line = file.ReadLine()) != null)
            {
                if (line.CompareTo(lineToFind) == 0)
                {
                    file.Close();
                    return true;
                }
            }

            file.Close();
        }

        return false;
    }

    /// <summary>
    /// Method to check item in Subscription Details file.
    /// </summary>
    /// <param name="transactionid">Transaction Id details</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id details</param>
    /// <returns>Returns True or False</returns>
    private bool CheckItemInSubsDetailsFile(string transactionid, string merchantTransactionId)
    {
        string line;
        string lineToFind = transactionid + ":-:" + merchantTransactionId;
        if (File.Exists(this.subscriptionDetailsFilePath))
        {
            System.IO.StreamReader file = new System.IO.StreamReader(this.subscriptionDetailsFilePath);
            while ((line = file.ReadLine()) != null)
            {
                if (line.CompareTo(lineToFind) == 0)
                {
                    file.Close();
                    return true;
                }
            }

            file.Close();
        }

        return false;
    }

    /// <summary>
    /// Method to write Subscription Refund to file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    private void WriteSubsRefundToFile(string transactionid, string merchantTransactionId)
    {
        using (StreamWriter appendContent = File.AppendText(Request.MapPath(this.refundFilePath)))
        {
            string line = transactionid + ":-:" + merchantTransactionId;
            appendContent.WriteLine(line);
            appendContent.Flush();
            appendContent.Close();
        }
    }

    /// <summary>
    /// Method to write Subscription Details to file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    private void WriteSubsDetailsToFile(string transactionid, string merchantTransactionId)
    {
        using (StreamWriter appendContent = File.AppendText(this.subscriptionDetailsFilePath))
        {
            string line = transactionid + ":-:" + merchantTransactionId;
            appendContent.WriteLine(line);
            appendContent.Flush();
            appendContent.Close();
        }
    }

    /// <summary>
    /// Initializes instance variables from Config file
    /// </summary>
    /// <returns>true/false; true if able to read from config file and able to instantiate values; else false</returns>
    private bool Initialize()
    {
        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(newSubscriptionPanel, "api_key is not defined in config file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(newSubscriptionPanel, "secret_key is not defined in config file");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endpoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(newSubscriptionPanel, "endPoint is not defined in config file");
            return false;
        }

        this.merchantRedirectURI = ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"];
        if (string.IsNullOrEmpty(this.merchantRedirectURI))
        {
            this.DrawPanelForFailure(newSubscriptionPanel, "MerchantPaymentRedirectUrl is not defined in config file");
            return false;
        }

        this.refundFilePath = ConfigurationManager.AppSettings["subscriptionRefundFilePath"];
        if (string.IsNullOrEmpty(this.refundFilePath))
        {
            this.refundFilePath = "subscriptionRefund.txt";
        }

        this.subscriptionDetailsFilePath = ConfigurationManager.AppSettings["subscriptionDetailsFilePath"];
        if (string.IsNullOrEmpty(this.subscriptionDetailsFilePath))
        {
            this.subscriptionDetailsFilePath = Request.MapPath("subscriptionDetails.txt");
        }
        else
        {
            this.subscriptionDetailsFilePath = Request.MapPath(this.subscriptionDetailsFilePath);
        }

        this.notificationDetailsFile = ConfigurationManager.AppSettings["notificationFilePath"];
        if (string.IsNullOrEmpty(this.notificationDetailsFile))
        {
            this.notificationDetailsFile = "notificationDetails.txt";
        }

        this.noOfNotificationsToDisplay = 5;
        if (ConfigurationManager.AppSettings["notificationCountToDisplay"] != null)
        {
            this.noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["notificationCountToDisplay"]);
        }

        this.subsDetailsCountToDisplay = 5;
        if (ConfigurationManager.AppSettings["subsDetailsCountToDisplay"] != null)
        {
            this.subsDetailsCountToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["subsDetailsCountToDisplay"]);
        }

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.Payment);
        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);

        return true;
    }

    /// <summary>
    /// Method to check item in refund file
    /// </summary>
    /// <param name="subscriptionid">Subscription Id</param>
    /// <param name="merchantSubscriptionId">Merchant Subscription Id</param>
    /// <returns>Return boolean</returns>
    private bool CheckItemInRefundFile(string subscriptionid, string merchantSubscriptionId)
    {
        string line;
        string lineToFind = subscriptionid + ":-:" + this.merchantTransactionId;
        StreamReader fileStream = null;
        if (File.Exists(Request.MapPath(this.refundFilePath)))
        {
            try
            {
                fileStream = new System.IO.StreamReader(Request.MapPath(this.refundFilePath));
                while ((line = fileStream.ReadLine()) != null)
                {
                    if (line.Equals(lineToFind))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.DrawPanelForFailure(newSubscriptionPanel, ex.Message);
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Method to check item in Subscription file
    /// </summary>
    /// <param name="consumerId">Consumer Id</param>
    /// <param name="merchantSubscriptionId">Merchant Subscription If</param>
    /// <returns>Return Boolean</returns>
    private bool CheckItemInSubscriptionFile(string consumerId, string merchantSubscriptionId)
    {
        string line;
        string lineToFind = consumerId + ":-:" + merchantSubscriptionId;
        StreamReader fileStream = null;
        if (File.Exists(this.subscriptionDetailsFilePath))
        {
            try
            {
                fileStream = new StreamReader(this.subscriptionDetailsFilePath);
                while ((line = fileStream.ReadLine()) != null)
                {
                    if (line.Equals(lineToFind))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.DrawPanelForFailure(newSubscriptionPanel, ex.Message);
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Method display SubscriptionAuthCode and Subscription Id from query string.This method will get called
    /// on page load after authorization process.
    /// </summary>
    private void ProcessCreateSubscriptionResponse()
    {
        List<string> radioButtonValueList = new List<string>();
        lblsubscode.Text = Request["SubscriptionAuthCode"];
        lblsubsid.Text = Session["sub_merTranId"].ToString();
        subscriptionSuccessTable.Visible = true;
        GetSubscriptionMerchantSubsID.Text = "Merchant Transaction ID: " + Session["sub_merTranId"].ToString();
        GetSubscriptionAuthCode.Text = "Auth Code: " + Request["SubscriptionAuthCode"];
        GetSubscriptionID.Text = "Subscription ID: ";
        Session["sub_merTranId"] = null;
    }

    /// <summary>
    /// This medthod is used for Drawing Subscription Details Section
    /// </summary>
    /// <param name="onlyRow">Row Details</param>
    private void DrawSubsDetailsSection(bool onlyRow)
    {
        try
        {
            if (onlyRow == false)
            {
                TableRow headingRow = new TableRow();
                TableCell headingCellOne = new TableCell();
                headingCellOne.HorizontalAlign = HorizontalAlign.Left;
                headingCellOne.CssClass = "cell";
                headingCellOne.Width = Unit.Pixel(200);
                headingCellOne.Font.Bold = true;
                headingCellOne.Text = "Merchant Subscription ID";
                headingRow.Controls.Add(headingCellOne);
                TableCell headingCellTwo = new TableCell();
                headingCellTwo.CssClass = "cell";
                headingCellTwo.Width = Unit.Pixel(100);
                headingRow.Controls.Add(headingCellTwo);
                TableCell headingCellThree = new TableCell();
                headingCellThree.CssClass = "cell";
                headingCellThree.HorizontalAlign = HorizontalAlign.Left;
                headingCellThree.Width = Unit.Pixel(240);
                headingCellThree.Font.Bold = true;
                headingCellThree.Text = "Consumer ID";
                headingRow.Controls.Add(headingCellThree);
                TableCell headingCellFour = new TableCell();
                headingCellFour.CssClass = "warning";
                LiteralControl warningMessage = new LiteralControl("<b>WARNING:</b><br/>You must use Get Subscription Status before you can view details of it.");
                headingCellFour.Controls.Add(warningMessage);
                headingRow.Controls.Add(headingCellFour);
                subsDetailsTable.Controls.Add(headingRow);
            }

            this.ResetSubsDetailsList();
            this.GetSubsDetailsFromFile();

            int tempCountToDisplay = 1;
            while ((tempCountToDisplay <= this.subsDetailsCountToDisplay) && (tempCountToDisplay <= this.subsDetailsList.Count) && (this.subsDetailsList.Count > 0))
            {
                this.AddRowToSubsDetailsSection(this.subsDetailsList[tempCountToDisplay - 1].Key, this.subsDetailsList[tempCountToDisplay - 1].Value);
                tempCountToDisplay++;
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(subsDetailsPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This medthod is used for adding row in Subscription Details Section
    /// </summary>
    /// <param name="subscription">Subscription Details</param>
    /// <param name="merchantsubscription">Merchant Details</param>
    private void AddRowToSubsDetailsSection(string subscription, string merchantsubscription)
    {
        TableRow rowOne = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Left;
        cellOne.CssClass = "cell";
        cellOne.Width = Unit.Pixel(150);
        RadioButton rbutton = new RadioButton();
        rbutton.Text = subscription;
        rbutton.GroupName = "SubsDetailsSection";
        rbutton.ID = subscription + "ctl " + merchantsubscription;
        cellOne.Controls.Add(rbutton);
        rowOne.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.CssClass = "cell";
        cellTwo.Width = Unit.Pixel(100);
        rowOne.Controls.Add(cellTwo);
        TableCell cellThree = new TableCell();
        cellThree.CssClass = "cell";
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Width = Unit.Pixel(240);
        cellThree.Text = merchantsubscription;
        rowOne.Controls.Add(cellThree);
        TableCell cellFour = new TableCell();
        cellFour.CssClass = "cell";
        rowOne.Controls.Add(cellFour);

        subsDetailsTable.Controls.Add(rowOne);
    }

    /// <summary>
    /// Method to reset Subscription Details List
    /// </summary>
    private void ResetSubsDetailsList()
    {
        this.subsDetailsList.RemoveRange(0, this.subsDetailsList.Count);
    }

    /// <summary>
    /// Method to get Subscription Details from the file.
    /// </summary>
    private void GetSubsDetailsFromFile()
    {
        if (File.Exists(this.subscriptionDetailsFilePath))
        {
            FileStream file = new FileStream(this.subscriptionDetailsFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] subsDetailsKeys = Regex.Split(line, ":-:");
                if (subsDetailsKeys[0] != null && subsDetailsKeys[1] != null)
                {
                    this.subsDetailsList.Add(new KeyValuePair<string, string>(subsDetailsKeys[0], subsDetailsKeys[1]));
                }
            }

            sr.Close();
            file.Close();
            this.subsDetailsList.Reverse(0, this.subsDetailsList.Count);
        }
    }

    /// <summary>
    /// This medthod is used for drawing Subscription Refund Section
    /// </summary>
    /// <param name="onlyRow">Row Details</param>
    private void DrawSubsRefundSection(bool onlyRow)
    {
        try
        {
            if (onlyRow == false)
            {
                TableRow headingRow = new TableRow();
                TableCell headingCellOne = new TableCell();
                headingCellOne.HorizontalAlign = HorizontalAlign.Left;
                headingCellOne.CssClass = "cell";
                headingCellOne.Width = Unit.Pixel(200);
                headingCellOne.Font.Bold = true;
                headingCellOne.Text = "Subscription ID";
                headingRow.Controls.Add(headingCellOne);
                TableCell headingCellTwo = new TableCell();
                headingCellTwo.CssClass = "cell";
                headingCellTwo.Width = Unit.Pixel(100);
                headingRow.Controls.Add(headingCellTwo);
                TableCell headingCellThree = new TableCell();
                headingCellThree.CssClass = "cell";
                headingCellThree.HorizontalAlign = HorizontalAlign.Left;
                headingCellThree.Width = Unit.Pixel(240);
                headingCellThree.Font.Bold = true;
                headingCellThree.Text = "Merchant Subscription ID";
                headingRow.Controls.Add(headingCellThree);
                TableCell headingCellFour = new TableCell();
                headingCellFour.CssClass = "warning";
                LiteralControl warningMessage = new LiteralControl("<b>WARNING:</b><br/>You must use Get Subscription Status before you can refund or cancel it.");
                headingCellFour.Controls.Add(warningMessage);
                headingRow.Controls.Add(headingCellFour);
                subsRefundTable.Controls.Add(headingRow);
            }

            this.ResetSubsRefundList();
            this.GetSubsRefundFromFile();

            int tempCountToDisplay = 1;
            while ((tempCountToDisplay <= this.subsDetailsCountToDisplay) && (tempCountToDisplay <= this.subsRefundList.Count) && (this.subsRefundList.Count > 0))
            {
                this.AddRowToSubsRefundSection(this.subsRefundList[tempCountToDisplay - 1].Key, this.subsRefundList[tempCountToDisplay - 1].Value);
                tempCountToDisplay++;
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(subsRefundPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This medthod is used for adding row in Subscription Refund Section
    /// </summary>
    /// <param name="subscription">Subscription Details</param>
    /// <param name="merchantsubscription">Merchant Details</param>
    private void AddRowToSubsRefundSection(string subscription, string merchantsubscription)
    {
        TableRow rowOne = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Left;
        cellOne.CssClass = "cell";
        cellOne.Width = Unit.Pixel(150);
        RadioButton rbutton = new RadioButton();
        rbutton.Text = subscription;
        rbutton.GroupName = "SubsRefundSection";
        rbutton.ID = subscription;
        cellOne.Controls.Add(rbutton);
        rowOne.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.CssClass = "cell";
        cellTwo.Width = Unit.Pixel(100);
        rowOne.Controls.Add(cellTwo);
        TableCell cellThree = new TableCell();
        cellThree.CssClass = "cell";
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Width = Unit.Pixel(240);
        cellThree.Text = merchantsubscription;
        rowOne.Controls.Add(cellThree);
        TableCell cellFour = new TableCell();
        cellFour.CssClass = "cell";
        rowOne.Controls.Add(cellFour);

        subsRefundTable.Controls.Add(rowOne);
    }

    /// <summary>
    /// Method to get Subscription Refund from the file.
    /// </summary>
    private void GetSubsRefundFromFile()
    {
        if (File.Exists(Request.MapPath(this.refundFilePath)))
        {
            FileStream file = new FileStream(Request.MapPath(this.refundFilePath), FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] subsRefundKeys = Regex.Split(line, ":-:");
                if (subsRefundKeys.Length == 2)
                {
                    if (subsRefundKeys[0] != null && subsRefundKeys[1] != null)
                    {
                        this.subsRefundList.Add(new KeyValuePair<string, string>(subsRefundKeys[0], subsRefundKeys[1]));
                    }
                }
            }

            sr.Close();
            file.Close();
            if (this.subsRefundList.Count != 0)
            {
                this.subsRefundList.Reverse(0, this.subsRefundList.Count);
            }
        }
    }

    /// <summary>
    /// Method to reset Subscription Refund List
    /// </summary>
    private void ResetSubsRefundList()
    {
        this.subsRefundList.RemoveRange(0, this.subsRefundList.Count);
    }

    /// <summary>
    /// This function draws the success table
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForSuccess(Panel panelParam)
    {
        this.successTable = new Table();
        this.successTable.Font.Name = "Sans-serif";
        this.successTable.Font.Size = 8;
        this.successTable.BorderStyle = BorderStyle.Outset;
        this.successTable.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOne.Controls.Add(rowOneCellOne);
        this.successTable.Controls.Add(rowOne);
        this.successTable.BorderWidth = 2;
        this.successTable.BorderColor = Color.DarkGreen;
        this.successTable.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc");
        panelParam.Controls.Add(this.successTable);
    }

    /// <summary>
    /// This function adds row to the success table
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">List of attributes</param>
    /// <param name="value">Value as string</param>
    private void AddRowToSuccessPanel(Panel panelParam, string attribute, string value)
    {
        TableRow row = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.Text = attribute;
        cellOne.Font.Bold = true;
        row.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.Text = value;
        row.Controls.Add(cellTwo);
        this.successTable.Controls.Add(row);
    }
    
    /// <summary>
    /// This function draws error table
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="message">Message as String</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        this.failureTable = new Table();
        this.failureTable.Font.Name = "Sans-serif";
        this.failureTable.Font.Size = 8;
        this.failureTable.BorderStyle = BorderStyle.Outset;
        this.failureTable.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "ERROR:";
        rowOne.Controls.Add(rowOneCellOne);
        this.failureTable.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Text = message.ToString();
        rowTwo.Controls.Add(rowTwoCellOne);
        this.failureTable.Controls.Add(rowTwo);
        this.failureTable.BorderWidth = 2;
        this.failureTable.BorderColor = Color.Red;
        this.failureTable.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc");
        panelParam.Controls.Add(this.failureTable);
    }

    /// <summary>
    /// This function draws success table transaction status details
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForGetSubscriptionSuccess(Panel panelParam)
    {
        this.successTableGetTransaction = new Table();
        this.successTableGetTransaction.HorizontalAlign = HorizontalAlign.Center;
        this.successTableGetTransaction.Font.Name = "Sans-serif";
        this.successTableGetTransaction.Font.Size = 8;
        this.successTableGetTransaction.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Right;
        rowOneCellOne.Text = "Parameter";
        rowOne.Controls.Add(rowOneCellOne);
        TableCell rowOneCellTwo = new TableCell();
        rowOneCellTwo.Width = Unit.Pixel(50);
        rowOne.Controls.Add(rowOneCellTwo);

        TableCell rowOneCellThree = new TableCell();
        rowOneCellThree.Font.Bold = true;
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellThree.Text = "Value";
        rowOne.Controls.Add(rowOneCellThree);
        this.successTableGetTransaction.Controls.Add(rowOne);
        panelParam.Controls.Add(this.successTableGetTransaction);
    }

    /// <summary>
    /// This function adds row to the success table
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">List of attributes</param>
    /// <param name="value">Value as String</param>
    private void AddRowToGetSubscriptionSuccessPanel(Panel panelParam, string attribute, string value)
    {
        TableRow row = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Right;
        cellOne.Text = attribute;
        row.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.Width = Unit.Pixel(50);
        row.Controls.Add(cellTwo);
        TableCell cellThree = new TableCell();
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Text = value;
        row.Controls.Add(cellThree);
        this.successTableGetTransaction.Controls.Add(row);
    }

    /// <summary>
    /// Method to get notification details
    /// </summary>
    private void GetNotificationDetails()
    {
        StreamReader notificationDetailsStream = null;
        string notificationDetail = string.Empty;
        if (!File.Exists(Request.MapPath(this.notificationDetailsFile)))
        {
            return;
        }

        try
        {
            using (notificationDetailsStream = File.OpenText(Request.MapPath(this.notificationDetailsFile)))
            {
                notificationDetail = notificationDetailsStream.ReadToEnd();
                notificationDetailsStream.Close();
            }

            string[] notificationDetailArray = notificationDetail.Split('$');
            int noOfNotifications = 0;
            if (null != notificationDetailArray)
            {
                noOfNotifications = notificationDetailArray.Length - 1;
            }

            if (noOfNotifications > 0)
            {
                if (this.notificationDetailsTable != null && this.notificationDetailsTable.Controls != null)
                {
                    this.notificationDetailsTable.Controls.Clear();
                }

                this.DrawNotificationTableHeaders();
            }

            int count = 0;

            while (noOfNotifications >= 0)
            {
                string[] notificationDetails = notificationDetailArray[noOfNotifications].Split(':');
                if (count <= this.noOfNotificationsToDisplay)
                {
                    if (notificationDetails.Length == 3)
                    {
                        this.AddRowToNotificationTable(notificationDetails[0], notificationDetails[1], notificationDetails[2]);
                    }
                }
                else
                {
                    break;
                }

                count++;
                noOfNotifications--;
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(notificationPanel, ex.ToString());
        }
        finally
        {
            if (null != notificationDetailsStream)
            {
                notificationDetailsStream.Close();
            }
        }
    }

    /// <summary>
    /// Method to display notification response table with headers
    /// </summary>
    private void DrawNotificationTableHeaders()
    {
        this.notificationDetailsTable = new Table();
        this.notificationDetailsTable.Font.Name = "Sans-serif";
        this.notificationDetailsTable.Font.Size = 8;
        this.notificationDetailsTable.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellOne.Text = "Notification ID";
        rowOneCellOne.Width = Unit.Pixel(400);
        rowOne.Controls.Add(rowOneCellOne);
        TableCell rowOneCellTwo = new TableCell();
        rowOneCellTwo.Width = Unit.Pixel(50);
        rowOne.Controls.Add(rowOneCellTwo);

        TableCell rowOneCellThree = new TableCell();
        rowOneCellThree.Font.Bold = true;
        rowOneCellThree.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellThree.Text = "Notification Type";
        rowOneCellThree.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellThree);
        this.notificationDetailsTable.Controls.Add(rowOne);
        TableCell rowOneCellFour = new TableCell();
        rowOneCellFour.Width = Unit.Pixel(50);
        rowOne.Controls.Add(rowOneCellFour);

        TableCell rowOneCellFive = new TableCell();
        rowOneCellFive.Font.Bold = true;
        rowOneCellFive.HorizontalAlign = HorizontalAlign.Left;
        rowOneCellFive.Text = "Transaction ID";
        rowOneCellFive.Width = Unit.Pixel(300);
        rowOne.Controls.Add(rowOneCellFive);
        this.notificationDetailsTable.Controls.Add(rowOne);
        notificationPanel.Controls.Add(this.notificationDetailsTable);
    }

    /// <summary>
    /// Method to add rows to notification response table with notification details
    /// </summary>
    /// <param name="notificationId">Notification Id</param>
    /// <param name="notificationType">Notification Type</param>
    /// <param name="transactionId">Transaction Id</param>
    private void AddRowToNotificationTable(string notificationId, string notificationType, string transactionId)
    {
        TableRow row = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Left;
        cellOne.Text = notificationId;
        cellOne.Width = Unit.Pixel(400);
        row.Controls.Add(cellOne);
        TableCell cellTwo = new TableCell();
        cellTwo.Width = Unit.Pixel(50);
        row.Controls.Add(cellTwo);

        TableCell cellThree = new TableCell();
        cellThree.HorizontalAlign = HorizontalAlign.Left;
        cellThree.Text = notificationType;
        cellThree.Width = Unit.Pixel(300);
        row.Controls.Add(cellThree);
        TableCell cellFour = new TableCell();
        cellFour.Width = Unit.Pixel(50);
        row.Controls.Add(cellFour);

        TableCell cellFive = new TableCell();
        cellFive.HorizontalAlign = HorizontalAlign.Left;
        cellFive.Text = transactionId;
        cellFive.Width = Unit.Pixel(300);
        row.Controls.Add(cellFive);
        TableCell cellSix = new TableCell();
        cellSix.Width = Unit.Pixel(50);
        row.Controls.Add(cellSix);

        this.notificationDetailsTable.Controls.Add(row);
        notificationPanel.Controls.Add(this.notificationDetailsTable);
    }

    #endregion

    /** }@ */
    /** }@ */
}