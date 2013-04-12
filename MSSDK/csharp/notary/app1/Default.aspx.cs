// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.Notaryv1;

#endregion

/// <summary>
/// Notary App1 class
/// This application demonstartes the usage of Notary API given by MS SDK library.
/// This application takes payload as argument and returns signed document and signature
/// </summary>
public partial class Notary_App1 : System.Web.UI.Page
{
    #region Instance Variables

    /// <summary>
    /// Gets or sets the value of amount
    /// </summary>
    private double amount;

    /// <summary>
    /// Gets or sets the value of category
    /// </summary>
    private int category;

    /// <summary>
    /// Table to draw messages in the page
    /// </summary>
    private Table failureTable;

    /// <summary>
    /// Transaction parameters
    /// </summary>
    private string channel, description, merchantTransactionId, merchantProductId, merchantApplicationId;

    /// <summary>
    /// Gets or sets the value of merchant Redirect Uri
    /// </summary>
    private Uri merchantRedirectURI;

    /// <summary>
    /// Instance variables for payment parameters
    /// </summary>
    private string paymentType, goBackURL, merchantSubscriptionIdList, subscriptionRecurringPeriod, isPurchaseOnNoActiveSubscription,
        transactionTimeString, payLoadStringFromRequest;

    /// <summary>
    /// Payment parameters
    /// </summary>
    private int subscriptionRecurringNumber, subscriptionRecurringPeriodAmount;

    /// <summary>
    /// Transaction Date Time
    /// </summary>
    private DateTime transactionTime;

    /// <summary>
    /// RequestFactory Instance
    /// </summary>
    private RequestFactory requestFactory;

    /// <summary>
    /// Application parameters
    /// </summary>
    private string apiKey, secretKey, endPoint;

    #endregion

    #region Notary Application Events

    /// <summary>
    /// This method will be called during loading the page
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        lblServerTime.Text = DateTime.UtcNow.ToString("ddd MMM dd yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) + " UTC";

        if ((Request["signed_payload"] != null) && (Request["signed_signature"] != null)
            && (Request["goBackURL"] != null) && (Request["signed_request"] != null))
        {
            signPayLoadButton.Text = "Back";
            requestText.Text = Request["signed_request"];
            SignedPayLoadTextBox.Text = Request["signed_payload"];
            SignatureTextBox.Text = Request["signed_signature"];
            this.goBackURL = Request["goBackURL"];
        }
        else if ((Request["request_to_sign"] != null) && (Request["goBackURL"] != null)
                  && (Request["api_key"] != null) && (Request["secret_key"] != null))
        {
            this.payLoadStringFromRequest = Request["request_to_sign"];
            this.goBackURL = Request["goBackURL"];
            SignedPayLoadTextBox.Text = this.payLoadStringFromRequest;
            this.apiKey = Request["api_key"];
            this.secretKey = Request["secret_key"];
            this.requestFactory = null;
            this.endPoint = ConfigurationManager.AppSettings["endpoint"];
            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.Payment);
            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
            this.ExecuteSignedPayloadFromRequest();
        }
        else
        {
            if (ConfigurationManager.AppSettings["paymentType"] == null)
            {
                this.DrawPanelForFailure(notaryPanel, "paymentType is not defined in configuration file");
                return;
            }

            this.paymentType = ConfigurationManager.AppSettings["paymentType"];
            if (this.paymentType.Equals("Transaction", StringComparison.OrdinalIgnoreCase))
            {
                this.ReadTransactionParametersFromConfigurationFile();
            }
            else if (this.paymentType.Equals("Subscription", StringComparison.OrdinalIgnoreCase))
            {
                if (!Page.IsPostBack)
                {
                    this.ReadTransactionParametersFromConfigurationFile();
                    this.ReadSubscriptionParametersFromConfigurationFile();
                }
            }
            else
            {
                this.DrawPanelForFailure(notaryPanel, "paymentType is  defined with invalid value in configuration file.  Valid values are Transaction or Subscription.");
                return;
            }

            if (!Page.IsPostBack)
            {
                string payLoadString = "{'Amount':'" + this.amount.ToString() +
                                        "','Category':'" + this.category.ToString() +
                                        "','Channel':'" + this.channel +
                                        "','Description':'" + this.description +
                                        "','MerchantTransactionId':'" + this.merchantTransactionId +
                                        "','MerchantProductId':'" + this.merchantProductId +
                                        "','MerchantApplicaitonId':'" + this.merchantApplicationId +
                                        "','MerchantPaymentRedirectUrl':'" + this.merchantRedirectURI.ToString() +
                                        "','MerchantSubscriptionIdList':'" + this.merchantSubscriptionIdList +
                                        "','IsPurchaseOnNoActiveSubscription':'" + this.isPurchaseOnNoActiveSubscription +
                                        "','SubscriptionRecurringNumber':'" + this.subscriptionRecurringNumber.ToString() +
                                        "','SubscriptionRecurringPeriod':'" + this.subscriptionRecurringPeriod +
                                        "','SubscriptionRecurringPeriodAmount':'" + this.subscriptionRecurringPeriodAmount.ToString();

                requestText.Text = payLoadString;
            }
        }
    }

    /// <summary>
    /// This method will be called when the user clicks on SignPayload button.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">Event Arguments</param>
    protected void SignPayLoadButton_Click(object sender, EventArgs e)
    {
        if (signPayLoadButton.Text.Equals("Back", StringComparison.CurrentCultureIgnoreCase))
        {
            try
            {
                Response.Redirect(this.goBackURL + "?shown_notary=true");
            }
            catch (Exception ex)
            {
                this.DrawPanelForFailure(notaryPanel, ex.Message);
            }
        }
        else
        {
            this.ExecuteSignedPayload();
        }
    }

    #endregion

    #region Notary Application specific methods

    /// <summary>
    /// Reads the values from config file and initializes the instance of RequestFactory
    /// </summary>
    /// <returns>true/false; true if able to initialize successfully, else false</returns>
    private bool Initialize()
    {
        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(notaryPanel, "api_key is not defined in the config file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(notaryPanel, "secret_key is not defined in the config file");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endpoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(notaryPanel, "endpoint is not defined in the config file");
            return false;
        }

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.Payment);
        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);

        return true;
    }

    /// <summary>
    /// Read Transaction Parameters From ConfigurationFile is used to read all transaction related values from web.config file
    /// </summary>
    private void ReadTransactionParametersFromConfigurationFile()
    {
        this.transactionTime = DateTime.UtcNow;
        this.transactionTimeString = String.Format("{0:ddd-MMM-dd-yyyy-HH-mm-ss}", this.transactionTime);
        if (ConfigurationManager.AppSettings["Amount"] == null)
        {
            this.DrawPanelForFailure(notaryPanel, "Amount is not defined in configuration file");
            return;
        }

        this.amount = Convert.ToDouble(ConfigurationManager.AppSettings["Amount"]);
        requestText.Text = "Amount: " + this.amount + "\r\n";
        if (ConfigurationManager.AppSettings["Category"] == null)
        {
            this.DrawPanelForFailure(notaryPanel, "Category is not defined in configuration file");
            return;
        }

        this.category = Convert.ToInt32(ConfigurationManager.AppSettings["Category"]);
        requestText.Text = requestText.Text + "Category: " + this.category + "\r\n";

        this.channel = ConfigurationManager.AppSettings["Channel"];
        if (string.IsNullOrEmpty(this.channel))
        {
            this.channel = "MOBILE_WEB";
        }

        this.description = "TrDesc" + this.transactionTimeString;
        requestText.Text = requestText.Text + "Description: " + this.description + "\r\n";
        this.merchantTransactionId = "TrId" + this.transactionTimeString;
        requestText.Text = requestText.Text + "MerchantTransactionId: " + this.merchantTransactionId + "\r\n";
        this.merchantProductId = "ProdId" + this.transactionTimeString;
        requestText.Text = requestText.Text + "MerchantProductId: " + this.merchantProductId + "\r\n";
        this.merchantApplicationId = "MerAppId" + this.transactionTimeString;
        requestText.Text = requestText.Text + "MerchantApplicationId: " + this.merchantApplicationId + "\r\n";
        if (ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"] == null)
        {
            this.DrawPanelForFailure(notaryPanel, "MerchantPaymentRedirectUrl is not defined in configuration file");
            return;
        }

        this.merchantRedirectURI = new Uri(ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"]);

        requestText.Text = requestText.Text + "MerchantPaymentRedirectUrl: " + this.merchantRedirectURI;
    }

    /// <summary>
    /// Read subscription Parameters From ConfigurationFile is used to read all subscription related values from web.config file
    /// </summary>
    private void ReadSubscriptionParametersFromConfigurationFile()
    {
        this.merchantSubscriptionIdList = ConfigurationManager.AppSettings["MerchantSubscriptionIdList"];
        if (string.IsNullOrEmpty(this.merchantSubscriptionIdList))
        {
            this.merchantSubscriptionIdList = "merSubIdList" + this.transactionTimeString;
        }

        requestText.Text = requestText.Text + "\r\n" + "MerchantSubscriptionIdList: " + this.merchantSubscriptionIdList + "\r\n";
        this.subscriptionRecurringPeriod = ConfigurationManager.AppSettings["SubscriptionRecurringPeriod"];
        if (string.IsNullOrEmpty(this.subscriptionRecurringPeriod))
        {
            this.subscriptionRecurringPeriod = "MONTHLY";
        }

        requestText.Text = requestText.Text + "SubscriptionRecurringPeriod: " + this.subscriptionRecurringPeriod + "\r\n";

        this.subscriptionRecurringNumber = 9999;
        if (ConfigurationManager.AppSettings["SubscriptionRecurringNumber"] != null)
        {
            this.subscriptionRecurringNumber = Convert.ToInt32(ConfigurationManager.AppSettings["SubscriptionRecurringNumber"]);
        }

        requestText.Text = requestText.Text + "SubscriptionRecurringNumber: " + this.subscriptionRecurringNumber + "\r\n";

        this.subscriptionRecurringPeriodAmount = 1;
        if (ConfigurationManager.AppSettings["SubscriptionRecurringPeriodAmount"] != null)
        {
            this.subscriptionRecurringPeriodAmount = Convert.ToInt32(ConfigurationManager.AppSettings["SubscriptionRecurringPeriodAmount"]);
        }

        requestText.Text = requestText.Text + "SubscriptionRecurringPeriodAmount: " + this.subscriptionRecurringPeriodAmount + "\r\n";

        this.isPurchaseOnNoActiveSubscription = ConfigurationManager.AppSettings["IsPurchaseOnNoActiveSubscription"];
        if (string.IsNullOrEmpty(this.isPurchaseOnNoActiveSubscription))
        {
            this.isPurchaseOnNoActiveSubscription = "false";
        }

        requestText.Text = requestText.Text + "IsPurchaseOnNoActiveSubscription: " + this.isPurchaseOnNoActiveSubscription;
    }

    /// <summary>
    /// This method invokes GetNotaryResponse() method of RequestFactory by passing payload string.
    /// and displays the notarized payload in the text box.
    /// </summary>
    private void ExecuteSignedPayload()
    {
        try
        {
            this.Initialize();
            string payLoadString = requestText.Text;
            NotaryResponse notaryResponse = this.requestFactory.GetNotaryResponse(payLoadString);
            if (null != notaryResponse)
            {
                SignedPayLoadTextBox.Text = notaryResponse.SignedDocument;
                SignatureTextBox.Text = notaryResponse.Signature;
            }
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(notaryPanel, ae.Message);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(notaryPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(notaryPanel, ex.Message);
        }
    }

    /// <summary>
    /// This method get called on new transaction or new subscription payload notarization.
    /// </summary>
    private void ExecuteSignedPayloadFromRequest()
    {
        try
        {
            string payLoadString = requestText.Text;
            NotaryResponse notaryResponse = this.requestFactory.GetNotaryResponse(payLoadString);
            if (null != notaryResponse)
            {
                Response.Redirect(this.goBackURL + "?ret_signed_payload=" + notaryResponse.SignedDocument + "&ret_signature=" + notaryResponse.Signature);
            }
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(notaryPanel, ae.Message);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(notaryPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(notaryPanel, ex.Message);
        }
    }

    /// <summary>
    /// This method displays error information in the panel.
    /// </summary>
    /// <param name="panelParam">Panel, where the message needs to be displayed</param>
    /// <param name="message">string, the message to be displayed</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        this.failureTable = new Table();
        this.failureTable.Font.Name = "Sans-serif";
        this.failureTable.Font.Size = 9;
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

    #endregion
}
