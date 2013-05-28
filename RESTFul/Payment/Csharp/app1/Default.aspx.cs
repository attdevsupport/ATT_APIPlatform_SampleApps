// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
#endregion

/// <summary>
/// Default Class
/// </summary>
public partial class Payment_App1 : System.Web.UI.Page
{
    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string accessTokenFilePath, apiKey, secretKey, accessToken, endPoint,
        scope, expirySeconds, refreshToken, accessTokenExpiryTime, refreshTokenExpiryTime,
        amount, channel, description, merchantTransactionId, merchantProductId, merchantApplicationId,
        transactionTimeString, notaryURL, notificationDetailsFile;

    private string merchantSubscriptionIdList, subscriptionRecurringPeriod, subscriptionRecurringNumber, subscriptionRecurringPeriodAmount, isPurchaseOnNoActiveSubscription;


    public string TransactionIdFile = string.Empty;
    public string MerchantTransactionIdFile = string.Empty;
    public string TransactionAuthorizationCodeFile = string.Empty;

    public string SubTransactionIdFile = string.Empty;
    public string SubMerchantTransactionIdFile = string.Empty;
    public string SubTransactionAuthorizationCodeFile = string.Empty;
    public string SubMerchantSubscriptionIdFile = string.Empty;

    public string ConsumerId = string.Empty;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int category, noOfNotificationsToDisplay;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private Uri merchantRedirectURI;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private DateTime transactionTime;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int recordsToDisplay = 0;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private List<KeyValuePair<string, string>> refundList = new List<KeyValuePair<string, string>>();

    /// <summary>
    /// Gets or sets the value of refreshTokenExpiresIn
    /// </summary>
    private int refreshTokenExpiresIn;


    /// <summary>
    /// Gets or sets the value of transaction amount.
    /// </summary>
    public string MinTransactionAmount, MaxTransactionAmount;
    public string MinSubscriptionAmount, MaxSubscriptionAmount;

    public string last_transaction_id = string.Empty;
    public string last_subscription_id = string.Empty;

    public List<string> MerTransactionIds = new List<string>();
    public List<string> transactionAuthCodes = new List<string>();
    public List<string> transactionIds = new List<string>();

    public List<string> SubMerTransactionIds = new List<string>();
    public List<string> SubtransactionAuthCodes = new List<string>();
    public List<string> SubtransactionIds = new List<string>();
    public List<string> SubMerchantSubscriptionIds = new List<string>();

    public string new_transaction_error = string.Empty;
    public string new_transaction_success = string.Empty;
    public string transaction_status_error = string.Empty;
    public string transaction_status_success = string.Empty;
    public string refund_error = string.Empty;
    public string refund_success = string.Empty;

    public List<string> MerSubscriptionIds = new List<string>();
    public List<string> SubscriptionAuthCodes = new List<string>();
    public List<string> SubscriptionIds = new List<string>();

    public string new_subscription_error = string.Empty;
    public string new_subscription_success = string.Empty;
    public string subscription_status_error = string.Empty;
    public string subscription_status_success = string.Empty;
    public string subscription_refund_error = string.Empty;
    public string subscription_refund_success = string.Empty;
    public string subscription_cancel_error = string.Empty;
    public string subscription_cancel_success = string.Empty;
    public string subscription_details_error = string.Empty;
    public string subscription_details_success = string.Empty;

    public string signedPayload = string.Empty;
    public string notary_error = string.Empty;
    public string notary_success = string.Empty;
    public string signedSignature = string.Empty;

    public Dictionary<string, string> formattedResponse = new Dictionary<string, string>();

    public Dictionary<string, string> getTransactionStatusResponse = new Dictionary<string, string>();
    public Dictionary<string, string> refundResponse = new Dictionary<string, string>();

    public Dictionary<string, string> getSubscriptionStatusResponse = new Dictionary<string, string>();
    public Dictionary<string, string> subscriptionRefundResponse = new Dictionary<string, string>();
    public Dictionary<string, string> getSubscriptionDetailsResponse = new Dictionary<string, string>();

    public List<Dictionary<string, string>> notificationDetails = new List<Dictionary<string, string>>();
    
    public string showTransaction = string.Empty;
    public string showSubscription = string.Empty;
    public string showNotary = string.Empty;
    public string showNotification = string.Empty;

    /// <summary>
    /// This function is used to neglect the ssl handshake error with authentication server.
    /// </summary>
    public static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        bool ableToReadFromConfig = this.ReadConfigFile();

        if (ableToReadFromConfig == false)
        {
            return;
        }

        if ((Request["TransactionAuthCode"] != null) && (Session["merTranId"] != null) && (Session["tranType"] != null))
        {
            showTransaction = "true";
            this.ProcessCreateTransactionResponse();
            //return;
        }
        if ((Request["SubscriptionAuthCode"] != null) && (Session["sub_merTranId"] != null) && (Session["subType"] != null))
        {
            showSubscription = "true";
            this.ProcessCreateSubscriptionResponse();
            //return;
        }

        if (!IsPostBack)
        {
            updateListForTransactionIds();
            updateListsForAuthCode();
            updateListsForMerchantTransactionId();

            updateListForSubMerchantSubscriptionIds();
            updateListForSubTransactionIds();
            updateListsForSubAuthCode();
            updateListsForSubMerchantTransactionId();
        }

        this.GetNotificationsFromFile();

        if (Session["tranType"] == null)
        {
            product_1.Checked = true;
        }
        if (Session["subType"] == null)
        {
            subproduct_1.Checked = true;
        }
    }

    private void DisplayDictionary(Dictionary<string, object> dict)
    {
        foreach (string strKey in dict.Keys)
        {
            //string strOutput = "".PadLeft(indentLevel * 8) + strKey + ":";

            object o = dict[strKey];
            if (o is Dictionary<string, object>)
            {
                DisplayDictionary((Dictionary<string, object>)o);
            }
            else if (o is ArrayList)
            {
                foreach (object oChild in ((ArrayList)o))
                {
                    if (oChild is string)
                    {
                        string strOutput = ((string)oChild);
                        //formattedResponse.Add(strOutput, "");
                    }
                    else if (oChild is Dictionary<string, object>)
                    {
                        DisplayDictionary((Dictionary<string, object>)oChild);
                    }
                }
            }
            else
            {
                formattedResponse.Add(strKey.ToString(), o.ToString());
            }
        }
    }

    /// <summary>
    /// Method to process create transaction response
    /// </summary>
    public void ProcessCreateTransactionResponse()
    {
        if (this.CheckItemInFile(Session["merTranId"].ToString(), this.MerchantTransactionIdFile) == false)
        {
            this.WriteRecordToFile(Session["merTranId"].ToString(), this.MerchantTransactionIdFile);
            updateListsForMerchantTransactionId();
        }
        if (this.CheckItemInFile(Request["TransactionAuthCode"].ToString(), this.TransactionAuthorizationCodeFile) == false)
        {
            this.WriteRecordToFile(Request["TransactionAuthCode"].ToString(), this.TransactionAuthorizationCodeFile);
            updateListsForAuthCode();
        }
        string sessionValue = Session["tranType"].ToString();
        if (!string.IsNullOrEmpty(sessionValue))
        {
            if (sessionValue.CompareTo("product_1") == 0)
                product_1.Checked = true;
            else 
                product_2.Checked = true;
        }
        //Session["tranType"] = null;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/MerchantTransactionId/" + Session["merTranId"].ToString();
        this.getTransactionStatus(resourcePathString, ref new_transaction_error, ref transaction_status_success);
        payload.Value = Session["signedData"].ToString();
        signedPayload = Session["signedPayload"].ToString();
        signedSignature = Session["signedSignature"].ToString();
        Session["signedData"] = null;
        Session["signedPayload"] = null;
        Session["signedSignature"] = null;
        Session["merTranId"] = null;
        return;
    }

    /// <summary>
    /// Method to process create transaction response
    /// </summary>
    public void ProcessCreateSubscriptionResponse()
    {
        if (this.CheckItemInFile(Session["sub_merTranId"].ToString(), this.SubMerchantTransactionIdFile) == false)
        {
            this.WriteRecordToFile(Session["sub_merTranId"].ToString(), this.SubMerchantTransactionIdFile);
            updateListsForSubMerchantTransactionId();
        }
        if (this.CheckItemInFile(Request["SubscriptionAuthCode"].ToString(), this.SubTransactionAuthorizationCodeFile) == false)
        {
            this.WriteRecordToFile(Request["SubscriptionAuthCode"].ToString(), this.SubTransactionAuthorizationCodeFile);
            updateListsForSubAuthCode();
        }
        string sessionValue = Session["subType"].ToString();
        if (!string.IsNullOrEmpty(sessionValue))
        {
            if (sessionValue.CompareTo("subproduct_1") == 0)
                subproduct_1.Checked = true;
            else
                subproduct_2.Checked = true;
        }
        //Session["subType"] = null;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Subscriptions/MerchantTransactionId/" + Session["sub_merTranId"].ToString();
        this.getSubscriptionStatus(resourcePathString, ref new_subscription_error, ref subscription_status_success);
        payload.Value = Session["signedData"].ToString();
        signedPayload = Session["signedPayload"].ToString();
        signedSignature = Session["signedSignature"].ToString();
        Session["signedData"] = null;
        Session["signedPayload"] = null;
        Session["signedSignature"] = null;
        Session["sub_merTranId"] = null;
        return;
    }

    /// <summary>
    /// Get Subscription button click event
    /// </summary>
    /// <param name="sender">Sender Details</param>
    /// <param name="e">List of Arguments</param>
    protected void getSubscriptionStatus(string resourcePathString, ref string error, ref string success)
    {
        try
        {
            if (this.ReadAndGetAccessToken(ref error) == true)
            {
                if (this.accessToken == null || this.accessToken.Length <= 0)
                {
                    return;
                }

                HttpWebRequest objRequest = (HttpWebRequest)System.Net.WebRequest.Create(resourcePathString);
                objRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                objRequest.Method = "GET";

                HttpWebResponse getTransactionStatusResponseObject = (HttpWebResponse)objRequest.GetResponse();

                using (StreamReader getTransactionStatusResponseStream = new StreamReader(getTransactionStatusResponseObject.GetResponseStream()))
                {
                    string getTransactionStatusResponseData = getTransactionStatusResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(getTransactionStatusResponseData);
                    DisplayDictionary(dict);
                    getSubscriptionStatusResponse = formattedResponse;
                    success = "true";
                    Session["ConsumerId"] = getSubscriptionStatusResponse["ConsumerId"];
                    if (this.CheckItemInFile(getSubscriptionStatusResponse["SubscriptionId"], this.SubTransactionIdFile) == false)
                    {
                        this.WriteRecordToFile(getSubscriptionStatusResponse["SubscriptionId"], this.SubTransactionIdFile);
                        updateListForSubTransactionIds();
                    }
                    if (this.CheckItemInFile(getSubscriptionStatusResponse["MerchantSubscriptionId"], this.SubMerchantSubscriptionIdFile) == false)
                    {
                        this.WriteRecordToFile(getSubscriptionStatusResponse["MerchantSubscriptionId"], this.SubMerchantSubscriptionIdFile);
                        updateListForSubMerchantSubscriptionIds();
                    }
                    getTransactionStatusResponseStream.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    error = reader.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            error = ex.ToString();
        }
    }

    protected void GetSubscriptionDetails_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(getSDetailsMSID.SelectedValue.ToString(), "Select") == 0)
            return;
        string merSubsID = getSDetailsMSID.SelectedValue.ToString();
        string consID = string.Empty;
        if (Session["ConsumerId"] != null)
        {
            consID = Session["ConsumerId"].ToString();
        }
        else
        {
            consID = this.ConsumerId;
        }
        try
        {
            if (this.ReadAndGetAccessToken(ref subscription_details_error) == true)
            {
                if (this.accessToken == null || this.accessToken.Length <= 0)
                {
                    return;
                }

                WebRequest objRequest = (WebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Subscriptions/" + merSubsID + "/Detail/" + consID);

                objRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                objRequest.Method = "GET";
                objRequest.ContentType = "application/json";

                WebResponse subsDetailsResponeObject = (WebResponse)objRequest.GetResponse();

                using (StreamReader subsDetailsResponseStream = new StreamReader(subsDetailsResponeObject.GetResponseStream()))
                {
                    string subsDetailsResponseData = subsDetailsResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(subsDetailsResponseData);
                    DisplayDictionary(dict);
                    getSubscriptionDetailsResponse = formattedResponse;
                    subscription_details_success = "true";
                    subsDetailsResponseStream.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    subscription_details_error = reader.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            subscription_details_error =  ex.ToString();
        }
    }

    
    protected void CancelSubscription_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(cancelSubscriptionId.SelectedValue.ToString(), "Select") == 0)
            return;
        string subscriptionToCancel = cancelSubscriptionId.SelectedValue.ToString();
        string strReq = "{\"TransactionOperationStatus\":\"SubscriptionCancelled\",\"RefundReasonCode\":1,\"RefundReasonText\":\"Customer was not happy\"}";
        RefundSubscriptionOperation(strReq, subscriptionToCancel, ref subscription_cancel_success, ref subscription_cancel_error);
    }
    
    protected void RefundSubscription_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(refundSubscriptionId.SelectedValue.ToString(), "Select") == 0)
            return;
        string subscriptionToRefund = refundSubscriptionId.SelectedValue.ToString();
        string strReq = "{\"TransactionOperationStatus\":\"Refunded\",\"RefundReasonCode\":1,\"RefundReasonText\":\"Customer was not happy\"}";
        RefundSubscriptionOperation(strReq, subscriptionToRefund, ref subscription_refund_success, ref subscription_refund_error);
    }

    protected void RefundSubscriptionOperation (string strRequest, string subid, ref string success, ref string error)
    {
        string dataLength = string.Empty;
        try
        {
            if (this.ReadAndGetAccessToken(ref subscription_refund_error) == true)
            {
                if (this.accessToken == null || this.accessToken.Length <= 0)
                {
                    return;
                }
                WebRequest objRequest = (WebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/" + subid.ToString());
                objRequest.Method = "PUT";
                objRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                objRequest.ContentType = "application/json";
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(strRequest);
                objRequest.ContentLength = postBytes.Length;
                Stream postStream = objRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                dataLength = postBytes.Length.ToString();
                postStream.Close();
                WebResponse refundTransactionResponeObject = (WebResponse)objRequest.GetResponse();
                using (StreamReader refundResponseStream = new StreamReader(refundTransactionResponeObject.GetResponseStream()))
                {
                    string refundTransactionResponseData = refundResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(refundTransactionResponseData);
                    DisplayDictionary(dict);
                    subscriptionRefundResponse = formattedResponse;
                    success = "true";
                    refundResponseStream.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    error = new StreamReader(stream).ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            error = ex.ToString();
        }
    }


    /// <summary>
    /// Event to get transaction.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void GetSubscriptionStatusForAuthCode_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(getSubscriptionAuthCode.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Subscriptions/TransactionAuthCode/" + getSubscriptionAuthCode.SelectedValue.ToString();
        this.getSubscriptionStatus(resourcePathString, ref subscription_status_error, ref subscription_status_success);
    }
    protected void GetSubscriptionStatusForMerTranId_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(getSubscriptionMTID.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Subscriptions/MerchantTransactionId/" + getSubscriptionMTID.SelectedValue.ToString();
        this.getSubscriptionStatus(resourcePathString, ref subscription_status_error, ref subscription_status_success);
    }
    protected void GetSubscriptionStatusForTransactionId_Click(object sender, EventArgs e)
    {
        showSubscription = "true";
        if (string.Compare(getSubscriptionTID.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Subscriptions/SubscriptionId/" + getSubscriptionTID.SelectedValue.ToString();
        this.getSubscriptionStatus(resourcePathString, ref subscription_status_error, ref subscription_status_success);
    }
    /// <summary>
    /// Event to get transaction.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void GetTransactionStatusForAuthCode_Click(object sender, EventArgs e)
    {
        showTransaction = "true";
        if (string.Compare(getTransactionAuthCode.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/TransactionAuthCode/" + getTransactionAuthCode.SelectedValue.ToString();
        this.getTransactionStatus(resourcePathString, ref transaction_status_error, ref transaction_status_success);
    }
    protected void GetTransactionStatusForMerTranId_Click(object sender, EventArgs e)
    {
        showTransaction = "true";
        if (string.Compare(getTransactionMTID.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/MerchantTransactionId/" + getTransactionMTID.SelectedValue.ToString();
        this.getTransactionStatus(resourcePathString, ref transaction_status_error, ref transaction_status_success);
    }
    protected void GetTransactionStatusForTransactionId_Click(object sender, EventArgs e)
    {
        showTransaction = "true";
        if (string.Compare(getTransactionTID.SelectedValue.ToString(), "Select") == 0)
            return;
        string resourcePathString = string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/TransactionId/" + getTransactionTID.SelectedValue.ToString();
        this.getTransactionStatus(resourcePathString, ref transaction_status_error, ref transaction_status_success);
    }
   
    protected void getTransactionStatus (string resourcePathString, ref string error, ref string success)
    {
        try
        {
            if (this.ReadAndGetAccessToken( ref error) == true)
            {
                if (this.accessToken == null || this.accessToken.Length <= 0)
                {
                    return;
                }

                HttpWebRequest objRequest = (HttpWebRequest)System.Net.WebRequest.Create(resourcePathString);
                objRequest.Method = "GET";
                objRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                HttpWebResponse getTransactionStatusResponseObject = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader getTransactionStatusResponseStream = new StreamReader(getTransactionStatusResponseObject.GetResponseStream()))
                {
                    string getTransactionStatusResponseData = getTransactionStatusResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(getTransactionStatusResponseData);
                    DisplayDictionary(dict);
                    getTransactionStatusResponse = formattedResponse;
                    success = "true";
                    if (this.CheckItemInFile(getTransactionStatusResponse["TransactionId"], this.TransactionIdFile) == false)
                    {
                        this.WriteRecordToFile(getTransactionStatusResponse["TransactionId"], this.TransactionIdFile);
                        updateListForTransactionIds();
                    }
                    getTransactionStatusResponseStream.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    error = new StreamReader(stream).ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            error =  ex.ToString();
        }
    }

    protected void ShowNotifications_Click(object sender, EventArgs e)
    {
        showNotification = "true";
        this.GetNotificationsFromFile();        
    }

    private void GetNotificationsFromFile()
    {
        if (null != notificationDetails)
        {
            notificationDetails.Clear();
        }

        List<string> notifications = new List<string>();
        this.GetListFromFile(this.notificationDetailsFile, ref notifications);

        Dictionary<string, string> notificationPair = null;
        int count = 1;
        foreach (string notification in notifications)
        {
            if (count > this.recordsToDisplay) break;

            string[] kvps = notification.Split('$');
            notificationPair = new Dictionary<string, string>();
            foreach (string kvp in kvps)
            {
                string[] values = kvp.Split('%');
                if (null != values)
                {
                    if (values.Length > 1)
                    {
                        notificationPair.Add(values[0], values[1]);
                    }
                }
            }

            notificationDetails.Add(notificationPair);
            count++;
        }
    }

    /// <summary>
    /// Reads from config file
    /// </summary>
    /// <returns>true/false; true if able to read else false</returns>
    private bool ReadConfigFile()
    {

        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MinTransactionAmount"]))
        {
            this.MinTransactionAmount = "0.00";
        }
        else
        {
            this.MinTransactionAmount = ConfigurationManager.AppSettings["MinTransactionAmount"];
        }


        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MaxTransactionAmount"]))
        {
            this.MaxTransactionAmount = "2.99";
        }
        else
        {
            this.MaxTransactionAmount = ConfigurationManager.AppSettings["MaxTransactionAmount"];
        }

        this.MinSubscriptionAmount = ConfigurationManager.AppSettings["MinSubscriptionAmount"];
        if (string.IsNullOrEmpty(this.MinSubscriptionAmount))
        {
            this.MinSubscriptionAmount = "0.00";
        }

        this.MaxSubscriptionAmount = ConfigurationManager.AppSettings["MaxSubscriptionAmount"];
        if (string.IsNullOrEmpty(this.MaxSubscriptionAmount))
        {
            this.MaxSubscriptionAmount = "3.99";
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            new_transaction_error = "api_key is not defined in configuration file";
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
             new_transaction_error = "endPoint is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
             new_transaction_error =  "secret_key is not defined in configuration file";
            return false;
        }

        this.accessTokenFilePath = ConfigurationManager.AppSettings["AccessTokenFilePath"];
        if (string.IsNullOrEmpty(this.accessTokenFilePath))
        {
            this.accessTokenFilePath = "~\\PayApp1AccessToken.txt";
        }

        this.TransactionIdFile = ConfigurationManager.AppSettings["TransactionIdFile"];
        if (string.IsNullOrEmpty(this.TransactionIdFile))
        {
            this.TransactionIdFile = "~\\TransactionIdFile.txt";
        }

        this.MerchantTransactionIdFile = ConfigurationManager.AppSettings["MerchantTransactionIdFile"];
        if (string.IsNullOrEmpty(this.MerchantTransactionIdFile))
        {
            this.MerchantTransactionIdFile = "~\\MerchantTransactionIdFile.txt";
        }

        this.TransactionAuthorizationCodeFile = ConfigurationManager.AppSettings["TransactionAuthorizationCodeFile"];
        if (string.IsNullOrEmpty(this.TransactionAuthorizationCodeFile))
        {
            this.TransactionAuthorizationCodeFile = "~\\TransactionAuthorizationCodeFile.txt";
        }

        this.SubTransactionIdFile = ConfigurationManager.AppSettings["SubTransactionIdFile"];
        if (string.IsNullOrEmpty(this.SubTransactionIdFile))
        {
            this.SubTransactionIdFile = "~\\SubTransactionIdFile.txt";
        }

        this.SubMerchantTransactionIdFile = ConfigurationManager.AppSettings["SubMerchantTransactionIdFile"];
        if (string.IsNullOrEmpty(this.SubMerchantTransactionIdFile))
        {
            this.SubMerchantTransactionIdFile = "~\\SubMerchantTransactionIdFile.txt";
        }

        this.SubTransactionAuthorizationCodeFile = ConfigurationManager.AppSettings["SubTransactionAuthorizationCodeFile"];
        if (string.IsNullOrEmpty(this.SubTransactionAuthorizationCodeFile))
        {
            this.SubTransactionAuthorizationCodeFile = "~\\SubTransactionAuthorizationCodeFile.txt";
        }

        this.SubMerchantSubscriptionIdFile = ConfigurationManager.AppSettings["SubMerchantSubscriptionIdFile"];
        if (string.IsNullOrEmpty(this.SubMerchantSubscriptionIdFile))
        {
            this.SubMerchantSubscriptionIdFile = "~\\SubMerchantSubscriptionIdFile.txt";
        }

        this.ConsumerId = ConfigurationManager.AppSettings["ConsumerId"];

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["recordsToDisplay"]))
        {
            this.recordsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["recordsToDisplay"]);
        }
        else
        {
            this.recordsToDisplay = 5;
        }

        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["noOfNotificationsToDisplay"]))
        {
            this.noOfNotificationsToDisplay = 5;
        }
        else
        {
            noOfNotificationsToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["noOfNotificationsToDisplay"]);
        }

        this.notificationDetailsFile = ConfigurationManager.AppSettings["notificationDetailsFile"];
        if (string.IsNullOrEmpty(this.notificationDetailsFile))
        {
            this.notificationDetailsFile = "notificationDetailsFile.txt";
        }

        this.scope = ConfigurationManager.AppSettings["scope"];
        if (string.IsNullOrEmpty(this.scope))
        {
            this.scope = "PAYMENT";
        }

        if (ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"] == null)
        {
             new_transaction_error =  "MerchantPaymentRedirectUrl is not defined in configuration file";
            return false; ;
        }

        this.merchantRedirectURI = new Uri(ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"]);

        string refreshTokenExpires = ConfigurationManager.AppSettings["refreshTokenExpiresIn"];
        if (!string.IsNullOrEmpty(refreshTokenExpires))
        {
            this.refreshTokenExpiresIn = Convert.ToInt32(refreshTokenExpires);
        }
        else
        {
            this.refreshTokenExpiresIn = 24;
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceLink"]))
        {
            SourceLink.HRef = ConfigurationManager.AppSettings["SourceLink"];
        }
        else
        {
            SourceLink.HRef = "#"; // Default value
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DownloadLink"]))
        {
            DownloadLink.HRef = ConfigurationManager.AppSettings["DownloadLink"];
        }
        else
        {
            DownloadLink.HRef = "#"; // Default value
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["HelpLink"]))
        {
            HelpLink.HRef = ConfigurationManager.AppSettings["HelpLink"];
        }
        else
        {
            HelpLink.HRef = "#"; // Default value
        }

        return true;
    }

    protected void createNewTransaction(object sender, EventArgs e)
    {
        this.ReadTransactionParametersFromConfigurationFile();
        string payLoadString = "{\"Amount\":" + this.amount.ToString() + ",\"Category\":" + this.category.ToString() + ",\"Channel\":\"" +
                        this.channel.ToString() + "\",\"Description\":\"" + this.description.ToString() + "\",\"MerchantTransactionId\":\""
                        + this.merchantTransactionId.ToString() + "\",\"MerchantProductId\":\"" + this.merchantProductId.ToString()
                        + "\",\"MerchantPaymentRedirectUrl\":\"" + this.merchantRedirectURI.ToString() + "\"}";
        SubmitToNotary(payLoadString, ref new_transaction_error, ref new_transaction_success);
        Session["signedData"] = payLoadString;
        Session["signedPayload"] = signedPayload;
        Session["signedSignature"] = signedSignature;
        Response.Redirect(this.endPoint + "/rest/3/Commerce/Payment/Transactions?clientid=" + this.apiKey.ToString() + "&SignedPaymentDetail=" + this.signedPayload.ToString() + "&Signature=" + this.signedSignature.ToString());
    }

    protected void createNewSubscription(object sender, EventArgs e)
    {
        this.ReadSubscriptionParametersFromConfigurationFile();
        string payLoadString = "{\"Amount\":" + this.amount + ",\"Category\":" + this.category + ",\"Channel\":\"" +
                        this.channel + "\",\"Description\":\"" + this.description + "\",\"MerchantTransactionId\":\""
                        + this.merchantTransactionId + "\",\"MerchantProductId\":\"" + this.merchantProductId
                        + "\",\"MerchantPaymentRedirectUrl\":\"" + this.merchantRedirectURI + "\",\"MerchantSubscriptionIdList\":\""
                        + this.merchantSubscriptionIdList + "\",\"IsPurchaseOnNoActiveSubscription\":\""
                        + this.isPurchaseOnNoActiveSubscription + "\",\"SubscriptionRecurrences\":" + this.subscriptionRecurringNumber
                        + ",\"SubscriptionPeriod\":\"" + this.subscriptionRecurringPeriod
                        + "\",\"SubscriptionPeriodAmount\":" + this.subscriptionRecurringPeriodAmount +
                        "}";
        SubmitToNotary(payLoadString, ref new_subscription_error, ref new_subscription_success);
        Session["signedData"] = payLoadString;
        Session["signedPayload"] = signedPayload;
        Session["signedSignature"] = signedSignature;
        Response.Redirect(this.endPoint + "/rest/3/Commerce/Payment/Subscriptions?clientid=" + this.apiKey.ToString() + "&SignedPaymentDetail=" + this.signedPayload.ToString() + "&Signature=" + this.signedSignature.ToString());
    }

    protected void Notary_Click(object sender, EventArgs e)
    {
        showNotary = "true";
        SubmitToNotary(payload.Value, ref notary_error, ref notary_success);
    }

    public void SubmitToNotary(string sendingData, ref string error, ref string success)
    {
        try
        {
            String newTransactionResponseData;
            string notaryAddress;
            notaryAddress = "" + this.endPoint + "/Security/Notary/Rest/1/SignedPayload";
            WebRequest newTransactionRequestObject = (WebRequest)System.Net.WebRequest.Create(notaryAddress);
            newTransactionRequestObject.Headers.Add("client_id", this.apiKey.ToString());
            newTransactionRequestObject.Headers.Add("client_secret", this.secretKey.ToString());
            newTransactionRequestObject.Method = "POST";
            newTransactionRequestObject.ContentType = "application/json";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(sendingData);
            newTransactionRequestObject.ContentLength = postBytes.Length;

            Stream postStream = newTransactionRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            WebResponse newTransactionResponseObject = (HttpWebResponse)newTransactionRequestObject.GetResponse();
            using (StreamReader newTransactionResponseStream = new StreamReader(newTransactionResponseObject.GetResponseStream()))
            {
                newTransactionResponseData = newTransactionResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                NotaryResponse deserializedJsonObj = (NotaryResponse)deserializeJsonObject.Deserialize(newTransactionResponseData, typeof(NotaryResponse));
                signedPayload = deserializedJsonObj.SignedDocument;
                signedSignature = deserializedJsonObj.Signature;
                success = "Success";
                payload.Value = sendingData;
                newTransactionResponseStream.Close();
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
    }
    /// <summary>
    /// Method to read transaction parameters from configuration file.
    /// </summary>
    private void ReadTransactionParametersFromConfigurationFile()
    {
        this.transactionTime = DateTime.UtcNow;
        this.transactionTimeString = string.Format("{0:dddMMMddyyyyHHmmss}", this.transactionTime);
        if (ConfigurationManager.AppSettings["Category"] == null)
        {
            new_transaction_error =  "Category is not defined in configuration file";
            return;
        }

        this.category = Convert.ToInt32(ConfigurationManager.AppSettings["Category"]);
        if (ConfigurationManager.AppSettings["Channel"] == null)
        {
            this.channel = "MOBILE_WEB";
        }
        else
        {
            this.channel = ConfigurationManager.AppSettings["Channel"];
        }

        this.description = "TrDesc" + this.transactionTimeString;
        this.merchantTransactionId = "C" + this.transactionTimeString;
        Session["merTranId"] = this.merchantTransactionId.ToString();
        this.merchantProductId = "ProdId" + this.transactionTimeString;
        this.merchantApplicationId = "MerAppId" + this.transactionTimeString;
        if (product_1.Checked)
        {
            this.amount = this.MinTransactionAmount;
            Session["tranType"] = "product_1";
        }
        else if (product_2.Checked)
        {
            Session["tranType"] = "product_2";
            this.amount = this.MaxTransactionAmount;
        }
    }

    private void ReadSubscriptionParametersFromConfigurationFile()
    {
        this.transactionTime = DateTime.UtcNow;
        this.transactionTimeString = String.Format("{0:dddMMMddyyyyHHmmss}", this.transactionTime);

        if (ConfigurationManager.AppSettings["Category"] == null)
        {
            new_subscription_error = "Category is not defined in configuration file";
            return;
        }

        this.category = Convert.ToInt32(ConfigurationManager.AppSettings["Category"]);
        this.channel = ConfigurationManager.AppSettings["Channel"];
        if (string.IsNullOrEmpty(this.channel))
        {
            this.channel = "MOBILE_WEB";
        }

        this.description = "TrDesc" + this.transactionTimeString;
        this.merchantTransactionId = "C" + this.transactionTimeString;
        Session["sub_merTranId"] = this.merchantTransactionId;
        this.merchantProductId = "ProdId" + this.transactionTimeString;
        this.merchantApplicationId = "MerAppId" + this.transactionTimeString;
        this.merchantSubscriptionIdList = "CML" + new Random().Next();
        Session["MerchantSubscriptionIdList"] = this.merchantSubscriptionIdList;

        this.isPurchaseOnNoActiveSubscription = ConfigurationManager.AppSettings["IsPurchaseOnNoActiveSubscription"];

        if (string.IsNullOrEmpty(this.isPurchaseOnNoActiveSubscription))
        {
            this.isPurchaseOnNoActiveSubscription = "false";
        }

        this.subscriptionRecurringNumber = ConfigurationManager.AppSettings["SubscriptionRecurringNumber"];
        if (string.IsNullOrEmpty(this.subscriptionRecurringNumber))
        {
            this.subscriptionRecurringNumber = "99999";
        }

        this.subscriptionRecurringPeriod = ConfigurationManager.AppSettings["SubscriptionRecurringPeriod"];
        if (string.IsNullOrEmpty(this.subscriptionRecurringPeriod))
        {
            this.subscriptionRecurringPeriod = "MONTHLY";
        }

        this.subscriptionRecurringPeriodAmount = ConfigurationManager.AppSettings["SubscriptionRecurringPeriodAmount"];
        if (string.IsNullOrEmpty(this.subscriptionRecurringPeriodAmount))
        {
            this.subscriptionRecurringPeriodAmount = "1";
        }

        if (subproduct_1.Checked)
        {
            this.amount = this.MinSubscriptionAmount;
            Session["subType"] = "subproduct_1";
        }
        else if (subproduct_2.Checked)
        {
            Session["subType"] = "subproduct_2";
            this.amount = this.MaxSubscriptionAmount;
        }
    }

    /// <summary>
    /// This function reads the Access Token File and stores the values of access token, expiry seconds
    /// refresh token, last access token time and refresh token expiry time
    /// This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns boolean</returns>    
    private bool ReadAccessTokenFile(ref string message)
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        try
        {
            fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Read);
            streamReader = new StreamReader(fileStream);
            this.accessToken = streamReader.ReadLine();
            this.accessTokenExpiryTime = streamReader.ReadLine();
            this.refreshToken = streamReader.ReadLine();
            this.refreshTokenExpiryTime = streamReader.ReadLine();
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
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

        if ((this.accessToken == null) || (this.accessTokenExpiryTime == null) || (this.refreshToken == null) || (this.refreshTokenExpiryTime == null))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// This function validates the expiry of the access token and refresh token,
    /// function compares the current time with the refresh token taken time, if current time is greater then 
    /// returns INVALID_REFRESH_TOKEN
    /// function compares the difference of last access token taken time and the current time with the expiry seconds, if its more,
    /// funciton returns INVALID_ACCESS_TOKEN
    /// otherwise returns VALID_ACCESS_TOKEN
    /// </summary>
    /// <returns>Return String</returns>
    private string IsTokenValid()
    {
        try
        {
            DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();
            if (currentServerTime >= DateTime.Parse(this.accessTokenExpiryTime))
            {
                if (currentServerTime >= DateTime.Parse(this.refreshTokenExpiryTime))
                {
                    return "INVALID_ACCESS_TOKEN";
                }
                else
                {
                    return "REFRESH_TOKEN";
                }
            }
            else
            {
                return "VALID_ACCESS_TOKEN";
            }
        }
        catch
        {
            return "INVALID_ACCESS_TOKEN";
        }
    }

    /// <summary>
    /// This function is used to read access token file and validate the access token
    /// this function returns true if access token is valid, or else false is returned
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <returns>Returns Boolean</returns>
    private bool ReadAndGetAccessToken(ref string responseString)
    {
        bool result = true;
        if (this.ReadAccessTokenFile(ref responseString) == false)
        {
            result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
        }
        else
        {
            string tokenValidity = this.IsTokenValid();
            if (tokenValidity == "REFRESH_TOKEN")
            {
                result = this.GetAccessToken(AccessType.RefreshToken, ref responseString);
            }
            else if (string.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") == 0)
            {
                result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
            }
        }

        if (this.accessToken == null || this.accessToken.Length <= 0)
        {
            return false;
        }
        else
        {
            return result;
        }
    }

    /// <summary>
    /// Method to read the entries from file and update list.
    /// </summary>
    public void GetListFromFile(string filename, ref List<string> list)
    {
        try
        {

            FileStream file = new FileStream(Request.MapPath(filename), FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }

            sr.Close();
            file.Close();
            list.Reverse(0, list.Count);
        }
        catch (Exception ex)
        {
            return;
        }
    }

    /// <summary>
    /// Method to update refund list to file.
    /// </summary>
    public void UpdateListToFile(string filename, ref List<string> list)
    {
        try 
        {
            if (list.Count != 0)
            {
                list.Reverse(0, list.Count);
            }

            using (StreamWriter sr = File.CreateText(Request.MapPath(filename)))
            {
                int tempCount = 0;
                while (tempCount < list.Count)
                {
                    string lineToWrite = list[tempCount];
                    sr.WriteLine(lineToWrite);
                    tempCount++;
                }
                sr.Close();
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }

    /// <summary>
    /// Event for refund transaction
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void RefundTransaction_Click(object sender, EventArgs e)
    {
        showTransaction = "true";
        if (string.Compare(refundTransactionId.SelectedValue.ToString(), "Select") == 0)
            return;
        string transactionToRefund = refundTransactionId.SelectedValue.ToString();
        string strReq = "{\"TransactionOperationStatus\":\"Refunded\",\"RefundReasonCode\":1,\"RefundReasonText\":\"Customer was not happy\"}";
        // string strReq = "{\"RefundReasonCode\":1,\"RefundReasonText\":\"Customer was not happy\"}";
        string dataLength = string.Empty;
        try
        {
            if (this.ReadAndGetAccessToken(ref refund_error) == true)
            {
                if (this.accessToken == null || this.accessToken.Length <= 0)
                {
                    return;
                }
                WebRequest objRequest = (WebRequest)System.Net.WebRequest.Create(string.Empty + this.endPoint + "/rest/3/Commerce/Payment/Transactions/" + transactionToRefund.ToString());
                objRequest.Method = "PUT";
                objRequest.Headers.Add("Authorization", "Bearer " + this.accessToken);
                objRequest.ContentType = "application/json";
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(strReq);
                objRequest.ContentLength = postBytes.Length;
                Stream postStream = objRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                dataLength = postBytes.Length.ToString();
                postStream.Close();
                WebResponse refundTransactionResponeObject = (WebResponse)objRequest.GetResponse();
                using (StreamReader refundResponseStream = new StreamReader(refundTransactionResponeObject.GetResponseStream()))
                {
                    string refundTransactionResponseData = refundResponseStream.ReadToEnd();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                    Dictionary<string, object> dict = deserializeJsonObject.Deserialize<Dictionary<string, object>>(refundTransactionResponseData);
                    DisplayDictionary(dict);
                    refundResponse = formattedResponse;
                    refund_success = "true";
                    refundResponseStream.Close();
                }
            }
        }
        catch (WebException we)
        {
            if (null != we.Response)
            {
                using (Stream stream = we.Response.GetResponseStream())
                {
                    refund_error = new StreamReader(stream).ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            //// + strReq + transactionToRefund.ToString() + dataLength
            refund_error = ex.ToString() ;
        }
    }

    public void updateListForTransactionIds()
    {
        getTransactionTID.Items.Clear();
        refundTransactionId.Items.Clear();
        ResetList(ref transactionIds);
        GetListFromFile(TransactionIdFile, ref transactionIds);
        getTransactionTID.Items.Add("Select");
        refundTransactionId.Items.Add("Select");
        foreach (var id in transactionIds)
        {
            refundTransactionId.Items.Add(id);
            getTransactionTID.Items.Add(id);
        }
    }

    public void updateListsForAuthCode()
    {
        getTransactionAuthCode.Items.Clear();
        ResetList(ref transactionAuthCodes);
        GetListFromFile(TransactionAuthorizationCodeFile, ref transactionAuthCodes);
        getTransactionAuthCode.Items.Add("Select");
        foreach (var id in transactionAuthCodes)
            getTransactionAuthCode.Items.Add(id);
    }

    public void updateListsForMerchantTransactionId()
    {
        getTransactionMTID.Items.Clear();
        ResetList(ref MerTransactionIds);
        GetListFromFile(MerchantTransactionIdFile, ref MerTransactionIds);
        getTransactionMTID.Items.Add("Select");
        foreach (var id in MerTransactionIds)
            getTransactionMTID.Items.Add(id);
    }

    public void updateListForSubMerchantSubscriptionIds()
    {
        getSDetailsMSID.Items.Clear();
        ResetList(ref SubMerchantSubscriptionIds);
        GetListFromFile(SubMerchantSubscriptionIdFile, ref SubMerchantSubscriptionIds);
        getSDetailsMSID.Items.Add("Select");
        foreach (var id in SubMerchantSubscriptionIds)
        {
            getSDetailsMSID.Items.Add(id);
        }
    }

    public void updateListForSubTransactionIds()
    {
        refundSubscriptionId.Items.Clear();
        cancelSubscriptionId.Items.Clear();
        getSubscriptionTID.Items.Clear();
        ResetList(ref SubtransactionIds);
        GetListFromFile(SubTransactionIdFile, ref SubtransactionIds);
        getSubscriptionTID.Items.Add("Select");
        refundSubscriptionId.Items.Add("Select");
        cancelSubscriptionId.Items.Add("Select");
        foreach (var id in SubtransactionIds)
        {
            refundSubscriptionId.Items.Add(id);
            getSubscriptionTID.Items.Add(id);
            cancelSubscriptionId.Items.Add(id);
        }


    }

    public void updateListsForSubAuthCode()
    {
        getSubscriptionAuthCode.Items.Clear();
        ResetList(ref SubtransactionAuthCodes);
        GetListFromFile(SubTransactionAuthorizationCodeFile, ref SubtransactionAuthCodes);
        getSubscriptionAuthCode.Items.Add("Select");
        foreach (var id in SubtransactionAuthCodes)
            getSubscriptionAuthCode.Items.Add(id);
    }

    public void updateListsForSubMerchantTransactionId()
    {
        getSubscriptionMTID.Items.Clear();
        ResetList(ref SubMerTransactionIds);
        GetListFromFile(SubMerchantTransactionIdFile, ref SubMerTransactionIds);
        getSubscriptionMTID.Items.Add("Select");
        foreach (var id in SubMerTransactionIds)
            getSubscriptionMTID.Items.Add(id);
    }

    /// <summary>
    /// Method to reset refund list
    /// </summary>
    public void ResetList(ref List<string> list)
    {
        if (list.Count > 0)
            list.RemoveRange(0, list.Count);
    }

    /// <summary>
    /// Method to check item in file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    /// <returns>Return Boolean</returns>
    public bool CheckItemInFile(string valueToSearch, string filename)
    {
        try
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(Request.MapPath(filename));
            while ((line = file.ReadLine()) != null)
            {
                if (line.CompareTo(valueToSearch) == 0)
                {
                    file.Close();
                    return true;
                }
            }
            file.Close();
            return false;
        }
        catch (Exception ex)
        {
            return true;
        }
    }

    /// <summary>
    /// Method to update file.
    /// </summary>
    /// <param name="transactionid">Transaction Id</param>
    /// <param name="merchantTransactionId">Merchant Transaction Id</param>
    public void WriteRecordToFile(string value, string filename)
    {
        try
        {

            List<string> list = new List<string>();
            FileStream file = new FileStream(Request.MapPath(filename), FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }

            sr.Close();
            file.Close();

            if (list.Count > this.recordsToDisplay)
            {
                int diff = list.Count - this.recordsToDisplay;
                list.RemoveRange(0, diff);
            }

            if (list.Count == this.recordsToDisplay)
            {
                list.RemoveAt(0);
            }
            list.Add(value);
            using (StreamWriter sw = File.CreateText(Request.MapPath(filename)))
            {
                int tempCount = 0;
                while (tempCount < list.Count)
                {
                    string lineToWrite = list[tempCount];
                    sw.WriteLine(lineToWrite);
                    tempCount++;
                }
                sw.Close();
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }

    /// <summary>
    /// This function get the access token based on the type parameter type values.
    /// If type value is 1, access token is fetch for client credential flow
    /// If type value is 2, access token is fetch for client credential flow based on the exisiting refresh token
    /// </summary>
    /// <param name="type">Type as integer</param>
    /// <param name="panelParam">Panel details</param>
    /// <returns>Return boolean</returns>
    private bool GetAccessToken(AccessType type, ref string message)
    {
        FileStream fileStream = null;
        Stream postStream = null;
        StreamWriter streamWriter = null;

        // This is client credential flow
        if (type == AccessType.ClientCredential)
        {
            try
            {
                DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
                accessTokenRequest.Method = "POST";
                string oauthParameters = string.Empty;
                if (type == AccessType.ClientCredential)
                {
                    oauthParameters = "client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&grant_type=client_credentials&scope=" + this.scope;
                }
                else
                {
                    oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
                }

                accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(oauthParameters);
                accessTokenRequest.ContentLength = postBytes.Length;

                postStream = accessTokenRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);

                WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
                using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
                {
                    string jsonAccessToken = accessTokenResponseStream.ReadToEnd().ToString();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                    AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(jsonAccessToken, typeof(AccessTokenResponse));

                    this.accessToken = deserializedJsonObj.access_token;
                    this.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString();
                    this.refreshToken = deserializedJsonObj.refresh_token;

                    DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                    if (deserializedJsonObj.expires_in.Equals("0"))
                    {
                        int defaultAccessTokenExpiresIn = 100; // In Yearsint yearsToAdd = 100;
                        this.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongDateString() + " " + currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongTimeString();
                    }

                    this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();

                    fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.WriteLine(this.accessToken);
                    streamWriter.WriteLine(this.accessTokenExpiryTime);
                    streamWriter.WriteLine(this.refreshToken);
                    streamWriter.WriteLine(this.refreshTokenExpiryTime);

                    // Close and clean up the StreamReader
                    accessTokenResponseStream.Close();
                    return true;
                }
            }
            catch (WebException we)
            {
                string errorResponse = string.Empty;

                try
                {
                    using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                    {
                        errorResponse = sr2.ReadToEnd();
                        sr2.Close();
                    }
                }
                catch
                {
                    errorResponse = "Unable to get response";
                }

                message = errorResponse + Environment.NewLine + we.ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            finally
            {
                if (null != postStream)
                {
                    postStream.Close();
                }

                if (null != streamWriter)
                {
                    streamWriter.Close();
                }

                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }
        else if (type == AccessType.RefreshToken)
        {
            try
            {
                DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

                WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.endPoint + "/oauth/token");
                accessTokenRequest.Method = "POST";

                string oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
                accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(oauthParameters);
                accessTokenRequest.ContentLength = postBytes.Length;

                postStream = accessTokenRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);

                WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
                using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
                {
                    string accessTokenJSon = accessTokenResponseStream.ReadToEnd().ToString();
                    JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                    AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(accessTokenJSon, typeof(AccessTokenResponse));
                    this.accessToken = deserializedJsonObj.access_token.ToString();
                    DateTime accessTokenExpiryTime = currentServerTime.AddMilliseconds(Convert.ToDouble(deserializedJsonObj.expires_in.ToString()));
                    this.refreshToken = deserializedJsonObj.refresh_token.ToString();

                    fileStream = new FileStream(Request.MapPath(this.accessTokenFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.WriteLine(this.accessToken);
                    streamWriter.WriteLine(this.accessTokenExpiryTime);
                    streamWriter.WriteLine(this.refreshToken);

                    // Refresh token valids for 24 hours
                    DateTime refreshExpiry = currentServerTime.AddHours(24);
                    this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();
                    streamWriter.WriteLine(refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString());

                    accessTokenResponseStream.Close();
                    return true;
                }
            }
            catch (WebException we)
            {
                string errorResponse = string.Empty;

                try
                {
                    using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                    {
                        errorResponse = sr2.ReadToEnd();
                        sr2.Close();
                    }
                }
                catch
                {
                    errorResponse = "Unable to get response";
                }

                message = errorResponse + Environment.NewLine + we.ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            finally
            {
                if (null != postStream)
                {
                    postStream.Close();
                }

                if (null != streamWriter)
                {
                    streamWriter.Close();
                }

                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }

        return false;
    }

    public class NotaryResponse
    {
        public string SignedDocument
        {
            get;
            set;
        }
        public string Signature
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Access Token Types
    /// </summary>
    private enum AccessType
    {
        /// <summary>
        /// Access Token Type is based on Client Credential Mode
        /// </summary>
        ClientCredential,

        /// <summary>
        /// Access Token Type is based on Refresh Token
        /// </summary>
        RefreshToken
    }

    /// <summary>
    /// Class to hold access token response
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Gets or sets access token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Gets or sets refresh token
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// Gets or sets expires in
        /// </summary>
        public string expires_in { get; set; }
    }
}