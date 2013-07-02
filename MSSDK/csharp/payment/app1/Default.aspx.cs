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
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.Notaryv1;
using ATT_MSSDK.Paymentv3;
#endregion

/* 
' * This Application demonstrates usage of  payment related methods exposed by AT&T MS SDK wrapper library
' * for creating a new payment transaction, getting the transaction status, refunding the transaction and 
' * viewing the notifications received from the platform.
' *  
' * Application ptrovides option for creating a new transaction, viewing the status of transaction, refunding
' * 5 latest transactions and viewing latest 5 notifications from the platform.
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
' *   i. - Invoke GetNotarizedForNewTransaction()on RequestFactory by passing transaction related parameters like ammount, 
' *        product description, to get NotaryResponse object. NotaryResponse contains transaction payload that has been 
' *        notarized/Encrypted by AT&T platform Notary service. 
' *
' *      - Get the Transaction redirect url pointing to AT&T platform transaction endpoint by invoking
' *        GetNewTransactionRedirect(NotaryResponse notaryResponse).
' *
' *       -  Redirect the user to AT&T platform transaction endpoint.
' *
' *        AT&T platform thows a login page and authenticates the user credentials and requests the user 
' *        for authorizing the payment transaction.
' *        Once user authorizes the payment transaction, AT&T platform performs the payment transaction and
' *        returns the control back to application passing 'TransactionAuthCode' as a query string.
' *
' *    ii. Application can use 'TransactionAuthCode' to invoke GetTransactionStatus() on RequestFactory
' *        to get the status of transaction.
'         
' *    iii. Application can invoke Refund() on RequestFactory by passing the transaction Id and refund reason
' *        
 */

/// <summary>
/// Payment class
/// </summary>
/// <remarks>
/// This application allows the user to 
/// Make a new transaction to buy product 1 or product 2, 
/// View the details of the notary signPayload request made in the background, 
/// Get the transaction status of that transaction, 
/// Refund any of the latest five transactions and
/// View the latest five notifications. 
/// </remarks>
public partial class Payment_App1 : System.Web.UI.Page
{
    /** \addtogroup Payment_App1
     * Description of the application can be referred at \ref Payment_app1 example
     * @{
     */

    /** \example Payment_app1 payment\app1\Default.aspx.cs
     * \n \n This application allows the user to 
     * \li Make a new transaction to buy product 1 or product 2
     * \li View the details of the notary signPayload request made in the background
     * \li Get the transaction status of that transaction
     * \li Refund any of the latest five transactions
     * \li View the latest five notifications. 
     * 
     * <b>Using Payment Methods:</b>
     * \li Import \c ATT_MSSDK and \c ATT_MSSDK.Paymentv3 NameSpace.
     * \li Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.
     * \li Invoke \c GetNotarizedForNewTransaction()on RequestFactory by passing transaction related parameters like ammount, product description, to get NotaryResponse object. 
     * \li \c NotaryResponse contains transaction payload that has been notarized/Encrypted by AT&T platform Notary service. 
     * \li Get the Transaction redirect url pointing to AT&T platform transaction endpoint by invoking \c GetNewTransactionRedirect(\c NotaryResponse notaryResponse).
     * \li Redirect the user to AT&T platform transaction endpoint.
     * \li AT&T platform thows a login page and authenticates the user credentials and requests the user for authorizing the payment transaction.
     * \li Once user authorizes the payment transaction, AT&T platform performs the payment transaction and returns the control back to application passing 'TransactionAuthCode' as a query string.
     * \li Application can use 'TransactionAuthCode' to invoke \c GetTransactionStatus() on \c RequestFactory to get the status of transaction.
     * \li Application can invoke \c Refund() on \c RequestFactory by passing the transaction Id and refund reason.
     * 
     * \n For Registration, Installation, Configuration and Execution, refer \ref Application
    * \n \n <b>Additional configuration to be done:</b>
    * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
    * \li MerchantPaymentRedirectUrl - Set to the URL pointing to the application. AT&T platform uses this URL to return the control back to the application after transaction is completed.
    * \li notificationFilePath - This is optional parameter, which points to the file path, where the notification details will be saved by listener.
     * \li refundFilePath - This is optional parameter, which points to the file path, where latest transaction IDs will be stored.
     * \li noOfNotificationsToDisplay - This is optional key, which will allow to display the defined number of notifications.
     * 
     * \n Documentation can be referred at \ref Payment_App1 section
     * @{
    */

    #region Instance Variables

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private RequestFactory requestFactory;
    
    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string apiKey, secretKey, endPoint;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private Table failureTable, successTableGetTransaction, notificationDetailsTable, successTableRefund;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int refundCountToDisplay;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private double amount;

    /// <summary>
    /// Glboal Variable Declaration
    /// </summary>
    private string description, merchantTransactionId, merchantProductId, merchantApplicationId, merchantRedirectURI,
        transactionTimeString, notificationDetailsFile;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private DateTime transactionTime;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private bool latestFive;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private List<KeyValuePair<string, string>> refundList;

    /// <summary>
    /// No of notifications to display
    /// </summary>
    private int noOfNotificationsToDisplay;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string refundFilePath;

    #endregion

    #region Payment Application events

    /// <summary>
    /// Event that gets triggered when the page is loaded initially into the browser.
    /// This method will read all config parameters and initializes RequestFactory instance, 
    /// Creates refund radio buttons and processes transaction response.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        transactionSuccessTable.Visible = false;
        tranGetStatusTable.Visible = false;
        refundSuccessTable.Visible = false;

        DateTime currentServerTime = DateTime.UtcNow;
        serverTimeLabel.Text = string.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC"; //// Convert.ToString(Session["merTranId"]);

        bool ableToInitialize = this.Initialize();
        if (ableToInitialize == false)
        {
            return;
        }

        if ((Request["TransactionAuthCode"] != null) && (Session["merTranId"] != null))
        {
            this.ProcessTransactionResponse();
        }
        else if ((Request["shown_notary"] != null) && (Session["processNotary"] != null))
        {
            Session["processNotary"] = null;
            GetTransactionMerchantTransID.Text = "Merchant Transaction ID: " + Session["tempMerTranId"].ToString();
            GetTransactionAuthCode.Text = "Auth Code: " + Session["TranAuthCode"].ToString();
        }

        refundTable.Controls.Clear();
        this.DrawRefundSection(false);
        this.DrawNotificationTableHeaders();
        this.GetNotificationDetails();
    }

    /// <summary>
    /// Event to create a new payment transaction
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void NewTransactionButton_Click(object sender, EventArgs e)
    {
        try
        {
            this.transactionTime = DateTime.UtcNow;
            this.transactionTimeString = string.Format("{0:dddMMMddyyyyHHmmss}", this.transactionTime);
            if (Radio_TransactionProductType.SelectedIndex == 0)
            {
                this.amount = 0.99;
            }
            else if (Radio_TransactionProductType.SelectedIndex == 1)
            {
                this.amount = 2.99;
            }

            Session["tranType"] = Radio_TransactionProductType.SelectedIndex.ToString();
            this.description = "TrDesc" + this.transactionTimeString;
            this.merchantTransactionId = "TrId" + this.transactionTimeString;
            Session["merTranId"] = this.merchantTransactionId.ToString();
            this.merchantProductId = "ProdId" + this.transactionTimeString;
            this.merchantApplicationId = "MerAppId" + this.transactionTimeString;

            //// Get payment payload notarized.
            NotaryResponse notaryResponse = this.requestFactory.GetNotarizedForNewTransaction(this.amount, PaymentCategories.ApplicationGames, this.description, this.merchantTransactionId, this.merchantProductId, this.merchantRedirectURI);

            //// Get the TransactionRedirectUrl which points to AT&T platform transaction endpoint
            string newTransactionRedirectUrl = this.requestFactory.GetNewTransactionRedirect(notaryResponse);
            Response.Redirect(newTransactionRedirectUrl);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(newTransactionPanel, ex.Message);
        }
    }

    /// <summary>
    /// Event to get notification details
    /// </summary>
    /// <param name="sender">an object that raised the event</param>
    /// <param name="e">Type EventArgs</param>
    protected void BtnGetNotification_Click(object sender, EventArgs e)
    {
        if (null != this.notificationDetailsTable.Controls)
        {
            this.notificationDetailsTable.Controls.Clear();
        }

        this.DrawNotificationTableHeaders();
        this.GetNotificationDetails();
    }
    
    /// <summary>
    /// Event to Refund a transaction
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void BtnRefundTransaction_Click1(object sender, EventArgs e)
    {
        try
        {
            string transactionToRefund = string.Empty;
            bool recordFound = false;

            if (this.refundList.Count > 0)
            {
                foreach (Control refundTableRow in refundTable.Controls)
                {
                    if (refundTableRow is TableRow)
                    {
                        foreach (Control refundTableRowCell in refundTableRow.Controls)
                        {
                            if (refundTableRowCell is TableCell)
                            {
                                foreach (Control refundTableCellControl in refundTableRowCell.Controls)
                                {
                                    if (refundTableCellControl is RadioButton)
                                    {
                                        if (((RadioButton)refundTableCellControl).Checked)
                                        {
                                            transactionToRefund = ((RadioButton)refundTableCellControl).Text.ToString();
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
                    RefundResponseObject refundResponse = this.requestFactory.Refund(transactionToRefund, 1, "Customer was not happy");
                    refundSuccessTable.Visible = true;
                    this.DrawPanelForRefundSuccess(refundPanel);
                    this.AddRowToRefundSuccessPanel(refundPanel, "IsSuccess", refundResponse.IsSuccess.ToString());
                    this.AddRowToRefundSuccessPanel(refundPanel, "OriginalPurchaseAmount", refundResponse.OriginalPurchaseAmount.ToString());
                    this.AddRowToRefundSuccessPanel(refundPanel, "TransactionId", refundResponse.TransactionId);
                    this.AddRowToRefundSuccessPanel(refundPanel, "TransactionStatus", refundResponse.TransactionStatus);
                    this.AddRowToRefundSuccessPanel(refundPanel, "Version", refundResponse.Version);
                    if (this.latestFive == false)
                    {
                        this.refundList.RemoveAll(x => x.Key.Equals(transactionToRefund));
                        this.UpdateRefundListToFile();
                        this.ResetRefundList();
                        refundTable.Controls.Clear();
                        this.DrawRefundSection(false);
                        GetTransactionMerchantTransID.Text = "Merchant Transaction ID: ";
                        GetTransactionAuthCode.Text = "Auth Code: ";
                        GetTransactionTransID.Text = "Transaction ID: ";
                    }
                }
            }
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(refundPanel, ex.Message);
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(refundPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(refundPanel, ex.Message);
        }
    }

    /// <summary>
    /// Event to get transaction details
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void GetTransactionButton_Click(object sender, EventArgs e)
    {
        try
        {
            string keyValue = string.Empty;
            TransactionStatus transactionStatus = null;

            //// Read the TransactionIdTypes selected by user to get the transaction status
            if (Radio_TransactionStatus.SelectedIndex == 0)
            {
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Merchant Transaction ID: ", string.Empty);
                transactionStatus = this.requestFactory.GetTransactionStatus(TransactionIdTypes.MerchantTransactionId, keyValue);
            }

            if (Radio_TransactionStatus.SelectedIndex == 1)
            {
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Auth Code: ", string.Empty);
                transactionStatus = this.requestFactory.GetTransactionStatus(TransactionIdTypes.TransactionAuthCode, keyValue);
            }
            
            if (Radio_TransactionStatus.SelectedIndex == 2)
            {
                keyValue = Radio_TransactionStatus.SelectedItem.Value.ToString().Replace("Transaction ID: ", string.Empty);
                transactionStatus = this.requestFactory.GetTransactionStatus(TransactionIdTypes.TransactionId, keyValue);
            }

            //// Disaply transaction status
            if (transactionStatus != null)
            {
                GetTransactionTransID.Text = "Transaction ID: " + transactionStatus.Id;
                
                if (this.CheckItemInRefundFile(transactionStatus.Id, transactionStatus.MerchantTransactionId) == false)
                {
                    this.WriteRefundToFile(transactionStatus.Id, transactionStatus.MerchantTransactionId);
                }

                refundTable.Controls.Clear();
                this.DrawRefundSection(false);
                tranGetStatusTable.Visible = true;
                this.DrawPanelForGetTransactionSuccess(newTransactionStatusPanel);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Amount", transactionStatus.Amount.ToString());
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Channel", transactionStatus.Channel);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "ConsumerId", transactionStatus.ConsumerId);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "ContentCategory", transactionStatus.ContentCategory);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Currency", transactionStatus.Currency);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Description", transactionStatus.Description);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "IsSuccess", transactionStatus.IsSuccess.ToString());
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantApplicationId", transactionStatus.MerchantApplicationId);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantId", transactionStatus.MerchantId);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantProductId", transactionStatus.MerchantProductId);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "MerchantTransactionId", transactionStatus.MerchantTransactionId);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "OriginalTransactionId", string.Empty);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionId", transactionStatus.Id);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionStatus", transactionStatus.Status);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "TransactionType", transactionStatus.Type);
                this.AddRowToGetTransactionSuccessPanel(newTransactionStatusPanel, "Version", transactionStatus.Version);                
            }
            else
            {
                this.DrawPanelForFailure(newTransactionStatusPanel, "No response from server.");
            }
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(newTransactionStatusPanel, ex.Message);
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(newTransactionStatusPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(newTransactionStatusPanel, ex.Message);
        }
    }

    #endregion

    #region Payment Application specific functions

    /// <summary>
    /// Instantiate RequestFactory of ATT_MSSDK by passing endPoint, apiKey, secretKey, scopes
    /// scope should contain Payment as RequestFactory.ScopeTypes
    /// </summary>
    /// <returns>Returns Boolean</returns>
    private bool Initialize()
    {
        if (this.requestFactory == null)
        {
            this.apiKey = ConfigurationManager.AppSettings["api_key"];
            if (string.IsNullOrEmpty(this.apiKey))
            {
                this.DrawPanelForFailure(newTransactionPanel, "api_key is not defined in configuration file.");
                return false;
            }

            this.secretKey = ConfigurationManager.AppSettings["secret_key"];
            if (string.IsNullOrEmpty(this.secretKey))
            {
                this.DrawPanelForFailure(newTransactionPanel, "secret_key is not defined in configuration file.");
                return false;
            }

            this.endPoint = ConfigurationManager.AppSettings["endpoint"];
            if (string.IsNullOrEmpty(this.endPoint))
            {
                this.DrawPanelForFailure(newTransactionPanel, "endpoint is not defined in configuration file.");
                return false;
            }

            this.merchantRedirectURI = ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"];
            if (string.IsNullOrEmpty(this.merchantRedirectURI))
            {
                this.DrawPanelForFailure(newTransactionPanel, "MerchantPaymentRedirectUrl is not defined in configuration file");
                return false;
            }

            this.notificationDetailsFile = ConfigurationManager.AppSettings["notificationFilePath"];
            if (string.IsNullOrEmpty(this.notificationDetailsFile))
            {
                this.notificationDetailsFile = "Xmlnotification.txt";
            }

            this.refundFilePath = ConfigurationManager.AppSettings["refundFilePath"];
            if (string.IsNullOrEmpty(this.refundFilePath))
            {
                this.refundFilePath = "refund.txt";
            }

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["noOfNotificationsToDisplay"]))
            {
                this.noOfNotificationsToDisplay = 5;
            }
            else
            {
                this.noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["noOfNotificationsToDisplay"]);
            }

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["refundCountToDisplay"]))
            {
                this.refundCountToDisplay = 5;
            }
            else
            {
                this.refundCountToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["refundCountToDisplay"]);
            }

            this.refundList = new List<KeyValuePair<string, string>>();
            this.latestFive = true;

            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.Payment);

            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        }

        return true;
    }

    /// <summary>
    /// Method to update refund list to file.
    /// </summary>
    private void UpdateRefundListToFile()
    {
        if (this.refundList.Count != 0)
        {
            this.refundList.Reverse(0, this.refundList.Count);
        }

        using (StreamWriter sr = File.CreateText(Request.MapPath(this.refundFilePath)))
        {
            int tempCount = 0;
            while (tempCount < this.refundList.Count)
            {
                string lineToWrite = this.refundList[tempCount].Key + ";" + this.refundList[tempCount].Value;
                sr.WriteLine(lineToWrite);
                tempCount++;
            }

            sr.Close();
        }
    }

    /// <summary>
    /// Method to get refung list from file.
    /// </summary>
    private void GetRefundListFromFile()
    {
        if (File.Exists(Request.MapPath(this.refundFilePath)))
        {
            //// Read the refund file for the list of transactions and store locally
            FileStream fileStream = null;
            StreamReader streamReader = null;

            try
            {
                fileStream = new FileStream(Request.MapPath(this.refundFilePath), FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream);
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] refundKeys = Regex.Split(line, ";");
                    if (null != refundKeys && refundKeys.Length >= 2)
                    {
                        if (refundKeys[0] != null && refundKeys[1] != null)
                        {
                            this.refundList.Add(new KeyValuePair<string, string>(refundKeys[0], refundKeys[1]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.DrawPanelForFailure(newTransactionPanel, ex.Message);
            }
            finally
            {
                if (null != streamReader)
                {
                    streamReader.Close();
                }

                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }

            this.refundList.Reverse(0, this.refundList.Count);
        }
    }

    /// <summary>
    /// Method to reset refund list
    /// </summary>
    private void ResetRefundList()
    {
        this.refundList.RemoveRange(0, this.refundList.Count);
    }

    /// <summary>
    /// Method to write refund to file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    private void WriteRefundToFile(string transactionid, string merchantTransactionId)
    {
        //// Read the refund file for the list of transactions and store locally
        using (StreamWriter appendContent = File.AppendText(Request.MapPath(this.refundFilePath)))
        {
            string line = transactionid + ";" + merchantTransactionId;
            appendContent.WriteLine(line);
            appendContent.Flush();
            appendContent.Close();
        }
    }

    /// <summary>
    /// processTransactionResponse is used to display TransactionAuthCode and merchantTransactionId from query string.
    /// This method will get called on page load after authorization process.
    /// </summary>
    private void ProcessTransactionResponse()
    {
        lbltrancode.Text = Request["TransactionAuthCode"];
        Session["TransactionAuthCode"] = Request["TransactionAuthCode"];
        lbltranid.Text = Session["merTranId"].ToString();
        transactionSuccessTable.Visible = true;
        GetTransactionMerchantTransID.Text = "Merchant Transaction ID: " + Session["merTranId"].ToString();
        GetTransactionAuthCode.Text = "Auth Code: " + Session["TransactionAuthCode"].ToString();
        GetTransactionTransID.Text = "Transaction ID: ";
        Session["tempMerTranId"] = Session["merTranId"].ToString();
        Session["merTranId"] = null;
        Session["TranAuthCode"] = Request["TransactionAuthCode"];
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

            int count = 0;
            if (noOfNotifications > 0)
            {
                if (null != this.notificationDetailsTable && null != this.notificationDetailsTable.Controls)
                {
                    this.notificationDetailsTable.Controls.Clear();
                }

                this.DrawNotificationTableHeaders();
            }

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
    /// Method to check items in refund file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    /// <returns>Returns Boolean</returns>
    private bool CheckItemInRefundFile(string transactionid, string merchantTransactionId)
    {
        string line;
        string lineToFind = transactionid + ";" + merchantTransactionId;
        StreamReader fileStream = null;
        bool ableToFind = false;
        if (File.Exists(Request.MapPath(this.refundFilePath)))
        {  
            try
            {
                fileStream = new StreamReader(Request.MapPath(this.refundFilePath));

                while ((line = fileStream.ReadLine()) != null)
                {
                    if (line.CompareTo(lineToFind) == 0)
                    {
                        ableToFind = true;
                        break;
                    }
                }
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }

        return ableToFind;
    }

    /// <summary>
    /// Method is used to create table dynamically with failure message.
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
    /// Method is used to create table dynamically with GetTransaction details.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForGetTransactionSuccess(Panel panelParam)
    {
        this.successTableGetTransaction = new Table();
        this.successTableGetTransaction.Font.Name = "Sans-serif";
        this.successTableGetTransaction.Font.Size = 8;
        this.successTableGetTransaction.Width = Unit.Pixel(650);
        this.successTableGetTransaction.HorizontalAlign = HorizontalAlign.Center;
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
    /// Method to draw panel for refund success
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    private void DrawPanelForRefundSuccess(Panel panelParam)
    {
        this.successTableRefund = new Table();
        this.successTableRefund.Font.Name = "Sans-serif";
        this.successTableRefund.Font.Size = 8;
        this.successTableRefund.Width = Unit.Pixel(650);
        this.successTableRefund.HorizontalAlign = HorizontalAlign.Center;
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
        this.successTableRefund.Controls.Add(rowOne);
        panelParam.Controls.Add(this.successTableRefund);
    }

    /// <summary>
    /// Method to draw refund section
    /// </summary>
    /// <param name="onlyRow">Row details</param>
    private void DrawRefundSection(bool onlyRow)
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
                headingCellOne.Text = "Transaction ID";
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
                headingCellThree.Text = "Merchant Transaction ID";
                headingRow.Controls.Add(headingCellThree);
                TableCell headingCellFour = new TableCell();
                headingCellFour.CssClass = "warning";
                LiteralControl warningMessage = new LiteralControl("<b>WARNING:</b><br/>You must use Get Transaction Status to get the Transaction ID before you can refund it.");
                headingCellFour.Controls.Add(warningMessage);
                headingRow.Controls.Add(headingCellFour);
                refundTable.Controls.Add(headingRow);
            }

            this.ResetRefundList();
            this.GetRefundListFromFile();

            int tempCountToDisplay = 1;
            while ((tempCountToDisplay <= this.refundCountToDisplay) && (tempCountToDisplay <= this.refundList.Count) && (this.refundList.Count > 0))
            {
                this.AddRowToRefundSection(this.refundList[tempCountToDisplay - 1].Key, this.refundList[tempCountToDisplay - 1].Value);
                tempCountToDisplay++;
            }

            //// addButtonToRefundSection("Refund Transaction");
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(newTransactionPanel, ex.ToString());
        }
    }

    /// <summary>
    /// method to display notification response table with headers
    /// </summary>
    private void DrawNotificationTableHeaders()
    {
        this.notificationDetailsTable = new Table();
        this.notificationDetailsTable.Font.Name = "Sans-serif";
        this.notificationDetailsTable.Font.Size = 8;
        this.notificationDetailsTable.Width = Unit.Pixel(650);
        this.notificationDetailsTable.HorizontalAlign = HorizontalAlign.Center;
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
        this.notificationPanel.Controls.Add(this.notificationDetailsTable);
    }

    /// <summary>
    /// Method to add row to refund section.
    /// </summary>
    /// <param name="transaction">Transaction as String</param>
    /// <param name="merchant">Merchant as string</param>
    private void AddRowToRefundSection(string transaction, string merchant)
    {
        TableRow rowOne = new TableRow();
        TableCell cellOne = new TableCell();
        cellOne.HorizontalAlign = HorizontalAlign.Left;
        cellOne.CssClass = "cell";
        cellOne.Width = Unit.Pixel(150);
        RadioButton rbutton = new RadioButton();
        rbutton.Text = transaction.ToString();
        rbutton.GroupName = "RefundSection";
        rbutton.ID = transaction;
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
        cellThree.Text = merchant;
        rowOne.Controls.Add(cellThree);

        TableCell cellFour = new TableCell();
        cellFour.CssClass = "cell";
        rowOne.Controls.Add(cellFour);

        refundTable.Controls.Add(rowOne);
    }

    /// <summary>
    /// This function adds row to the refund success table.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">Attribute as string</param>
    /// <param name="value">Value as string</param>
    private void AddRowToRefundSuccessPanel(Panel panelParam, string attribute, string value)
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
        this.successTableRefund.Controls.Add(row);
    }

    /// <summary>
    /// Method is used to add row to the GetTransactionDetails table dynamically with GetTransaction details.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="attribute">Attributes as String</param>
    /// <param name="value">Value as String</param>
    private void AddRowToGetTransactionSuccessPanel(Panel panelParam, string attribute, string value)
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
        this.notificationPanel.Controls.Add(this.notificationDetailsTable);
    }

    #endregion

    /** }@ */
    /** }@ */
}