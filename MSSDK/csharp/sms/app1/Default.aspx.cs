// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.SMSv3;
using System.IO;
using System.Text.RegularExpressions;
#endregion

/* This Application demonstrates usage of  AT&T MS SDK wrapper library for sending SMS,
 * getting delivery status and receiving sms.
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke SMS APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.SMSv3 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 * 
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the sms related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For sms services MS SDK library provides APIs SendSms(),GetSmsDeliveryResponse() and ReceiveSms()
 * These methods return response objects SmsResponse, SmsDeliveryResponse and InboundSmsMessageList.
 * 
 * Sample code for sending sms:
 * ----------------------------
 * List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 * scopes.Add(RequestFactory.ScopeTypes.SMS);
 * RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 * SmsResponse response = requestFactory.SendSms(PhoneNumber, Message);
 *  
 * Sample code for getting SMS delivery status:
 * --------------------------------------------
 * SmsDeliveryResponse resp = requestFactory.GetSmsDeliveryResponse(SmsId);
 * 
 * Sample code for receiving sms:
 * ------------------------------
 * InboundSmsMessageList message = requestFactory.ReceiveSms(shortCode);
 * */

/// <summary>
/// Default Class
/// </summary>
public partial class SMS_App1 : System.Web.UI.Page
{
    /** \addtogroup SMS_App1
    * Description of the application can be referred at \ref SMS_app1 example
    * @{
    */

    /** \example SMS_app1 sms\app1\Default.aspx.cs
     * \n \n This application allows a user to send an SMS message, check the delivery status of that SMS message, and check for received SMS messages.
     * <ul>
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Send Message:</b>
     * \n This method sends an SMS message to one Mobile Network device.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.SMSv3 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke \c SendSms() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
     * \n Sample code:
     * <pre>
     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
     *    scopes.Add(RequestFactory.ScopeTypes.MMS);
     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
     *    SmsResponse response = requestFactory.SendSms(PhoneNumber, Message);</pre>
     * <b>Get SMS Delivery Response</b>
     * \n This method gets the status of a previous SMS delivery request that was successfully accepted 
     * by the AT&T Network for delivery to the destination mobile device.
     * <pre>
     *    SmsDeliveryResponse resp = requestFactory.GetSmsDeliveryResponse(SmsId);</pre>
     * <b>Receive SMS</b>
     * \n This method retrieves all SMS messages received on its short code resource via polling mechanism. 
     * <pre>
     *    InboundSmsMessageList message = requestFactory.ReceiveSms(shortCode);</pre>
     * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * \n <li>Additional configuration to be done:</li>
     * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
     * <ul><li> short_code - This is mandatory key and is the value of the registration ID of App</li></ul>
     * 
     * \n <li>Documentation can be referred at \ref MMS_App1 section</li></ul>
     * @{
     */

    #region Variable Declaration

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string apiKey, secretKey, endPoint, shortCode;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string[] shortCodes;

    #endregion

    #region SMS Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// This fucntion reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();
            if (this.InitializeValues() == false)
            {
                return;
            }

            DateTime currentServerTime = DateTime.UtcNow;
            serverTimeLabel.Text = string.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

            this.shortCodes = this.shortCode.Split(';');
            this.shortCode = this.shortCodes[0];
            Table table = new Table();
            table.Font.Size = 8;
            foreach (string srtCode in this.shortCodes)
            {
                Button button = new Button();
                button.Click += new EventHandler(this.GetMessagesButton_Click);
                button.Text = "Get Messages for " + srtCode;
                TableRow rowOne = new TableRow();
                TableCell rowOneCellOne = new TableCell();
                rowOne.Controls.Add(rowOneCellOne);
                rowOneCellOne.Controls.Add(button);
                table.Controls.Add(rowOne);
            }

            receiveMessagePanel.Controls.Add(table);           
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendSMSPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This function is called with user clicks on send SMS
    /// This validates the access token and then calls sendSMS method to invoke send SMS API.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void BtnSendSMS_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtmsisdn.Text))
            {
                this.DrawPanelForFailure(sendSMSPanel, "Specify phone number");
                return;
            }

            if (string.IsNullOrEmpty(txtmsg.Text))
            {
                this.DrawPanelForFailure(sendSMSPanel, "Specify message to send");
                return;
            }

            SmsResponse resp = this.requestFactory.SendSms(txtmsisdn.Text.Trim(), txtmsg.Text.Trim(), chkReceiveNotification.Checked);

            if (!chkReceiveNotification.Checked)
            {
                txtSmsId.Text = resp.MessageId;
            }
            this.DrawPanelForSuccess(sendSMSPanel, resp.MessageId);
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(sendSMSPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(sendSMSPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendSMSPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This method is called when user clicks on get delivery status button
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    protected void GetDeliveryStatusButton_Click(object sender, EventArgs e)
    {
        try
        {
            Session["smsId"] = txtSmsId.Text.Trim();
            SmsDeliveryResponse resp = this.requestFactory.GetSmsDeliveryResponse(txtSmsId.Text);
            this.DrawGetStatusSuccess(resp.DeliveryInfo[0].DeliveryStatus, resp.ResourceURL);
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(getStatusPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            //this.DrawPanelForFailure(getStatusPanel, ex.Body + Environment.NewLine + ex.ToString());
            this.DrawPanelForFailure(getStatusPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(getStatusPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This method retrives received sms for a given short code.
    /// </summary>
    /// <param name="sender">Sender Information</param>
    /// <param name="e">List of Arguments</param>
    private void GetMessagesButton_Click(object sender, EventArgs e)
    {
        try
        {
            Button button = sender as Button;
            string buttonCaption = button.Text.ToString();
            this.shortCode = buttonCaption.Replace("Get Messages for ", string.Empty);
            this.RecieveSms();
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(getMessagePanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(getMessagePanel, ex.ToString());
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        this.DisplayDeliveryNotificationStatus();
    }

    #endregion

    #region SMS Application related functions

    /// <summary>
    /// Neglect the ssl handshake error with authentication server
    /// </summary>
    private static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    /// <summary>
    /// Initializes instance members of the <see cref="SMS_App1"/> class.
    /// </summary>
    /// <returns>true/false; true if able to read from config file and assigns to instance variables; else false</returns>
    private bool InitializeValues()
    {
        if (this.requestFactory == null)
        {

            this.endPoint = ConfigurationManager.AppSettings["endPoint"];
            if (string.IsNullOrEmpty(this.endPoint))
            {
                this.DrawPanelForFailure(sendSMSPanel, "endPoint is not defined in configuration file");
                return false;
            }

            this.shortCode = ConfigurationManager.AppSettings["short_code"];
            if (string.IsNullOrEmpty(this.shortCode))
            {
                this.DrawPanelForFailure(sendSMSPanel, "short_code is not defined in configuration file");
                return false;
            }

            this.apiKey = ConfigurationManager.AppSettings["api_key"];
            if (string.IsNullOrEmpty(this.apiKey))
            {
                this.DrawPanelForFailure(sendSMSPanel, "api_key is not defined in configuration file");
                return false;
            }

            this.secretKey = ConfigurationManager.AppSettings["secret_key"];
            if (string.IsNullOrEmpty(this.secretKey))
            {
                this.DrawPanelForFailure(sendSMSPanel, "secret_key is not defined in configuration file");
                return false;
            }

            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.SMS);
            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        }

        return true;
    }

    /// <summary>
    /// This function calls receive sms api to fetch the sms's
    /// </summary>
    private void RecieveSms()
    {
        InboundSmsMessageList message = this.requestFactory.ReceiveSms(this.shortCode);

        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.Width = Unit.Pixel(650);
        TableRow tableRow = new TableRow();
        TableCell tableCell = new TableCell();
        tableCell.Width = Unit.Pixel(110);
        tableCell.Text = "SUCCESS:";
        tableCell.Font.Bold = true;
        tableRow.Cells.Add(tableCell);
        table.Rows.Add(tableRow);
        tableRow = new TableRow();
        tableCell = new TableCell();
        tableCell.Width = Unit.Pixel(150);
        tableCell.Text = "Messages in this batch:";
        tableCell.Font.Bold = true;
        tableRow.Cells.Add(tableCell);
        tableCell = new TableCell();
        tableCell.HorizontalAlign = HorizontalAlign.Left;
        tableCell.Text = message.NumberOfMessagesInThisBatch.ToString();
        tableRow.Cells.Add(tableCell);
        table.Rows.Add(tableRow);
        tableRow = new TableRow();
        tableCell = new TableCell();
        tableCell.Width = Unit.Pixel(110);
        tableCell.Text = "Messages pending:";
        tableCell.Font.Bold = true;
        tableRow.Cells.Add(tableCell);
        tableCell = new TableCell();
        tableCell.Text = message.TotalNumberOfPendingMessages.ToString();
        tableRow.Cells.Add(tableCell);
        table.Rows.Add(tableRow);
        tableRow = new TableRow();
        table.Rows.Add(tableRow);
        tableRow = new TableRow();
        table.Rows.Add(tableRow);
        Table secondTable = new Table();
        if (message.NumberOfMessagesInThisBatch > 0)
        {
            tableRow = new TableRow();
            secondTable.Font.Name = "Sans-serif";
            secondTable.Font.Size = 9;
            tableCell = new TableCell();
            tableCell.Width = Unit.Pixel(100);
            tableCell.Text = "Message Index";
            tableCell.HorizontalAlign = HorizontalAlign.Center;
            tableCell.Font.Bold = true;
            tableRow.Cells.Add(tableCell);
            tableCell = new TableCell();
            tableCell.Font.Bold = true;
            tableCell.Width = Unit.Pixel(350);
            tableCell.Wrap = true;
            tableCell.Text = "Message Text";
            tableCell.HorizontalAlign = HorizontalAlign.Center;
            tableRow.Cells.Add(tableCell);
            tableCell = new TableCell();
            tableCell.Text = "Sender Address";
            tableCell.HorizontalAlign = HorizontalAlign.Center;
            tableCell.Font.Bold = true;
            tableCell.Width = Unit.Pixel(175);
            tableRow.Cells.Add(tableCell);
            secondTable.Rows.Add(tableRow);

            foreach (InboundSmsMessage prime in message.InboundSmsMessage)
            {
                tableRow = new TableRow();
                TableCell tableCellmessageId = new TableCell();
                tableCellmessageId.Width = Unit.Pixel(75);
                tableCellmessageId.Text = prime.MessageId.ToString();
                tableCellmessageId.HorizontalAlign = HorizontalAlign.Center;
                TableCell tableCellmessage = new TableCell();
                tableCellmessage.Width = Unit.Pixel(350);
                tableCellmessage.Wrap = true;
                tableCellmessage.Text = prime.Message.ToString();
                tableCellmessage.HorizontalAlign = HorizontalAlign.Center;
                TableCell tableCellsenderAddress = new TableCell();
                tableCellsenderAddress.Width = Unit.Pixel(175);
                tableCellsenderAddress.Text = prime.SenderAddress.ToString();
                tableCellsenderAddress.HorizontalAlign = HorizontalAlign.Center;
                tableRow.Cells.Add(tableCellmessageId);
                tableRow.Cells.Add(tableCellmessage);
                tableRow.Cells.Add(tableCellsenderAddress);
                secondTable.Rows.Add(tableRow);
            }
        }

        table.BorderColor = Color.DarkGreen;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc");
        table.BorderWidth = 2;

        getMessagePanel.Controls.Add(table);
        getMessagePanel.Controls.Add(secondTable);
    }

    /// <summary>
    /// This function calls receive sms api to fetch the sms's
    /// </summary>
    private void DisplayDeliveryNotificationStatus()
    {
        string receivedMessagesFile = ConfigurationManager.AppSettings["deliveryStatusFilePath"];
        if (!string.IsNullOrEmpty(receivedMessagesFile))
            receivedMessagesFile = Server.MapPath(receivedMessagesFile);
        else
            receivedMessagesFile = Server.MapPath("DeliveryStatus.txt");
        string messagesLine = String.Empty;

        List<DeliveryInfoNotification> receiveSMSDeliveryStatusResponseData = new List<DeliveryInfoNotification>();

        if (File.Exists(receivedMessagesFile))
        {
            using (StreamReader sr = new StreamReader(receivedMessagesFile))
            {
                while (sr.Peek() >= 0)
                {
                    DeliveryInfoNotification dNot = new DeliveryInfoNotification();
                    dNot.DeliveryInfo = new DeliveryInfo();
                    messagesLine = sr.ReadLine();
                    string[] messageValues = Regex.Split(messagesLine, "_-_-");
                    dNot.DeliveryInfo.Id = messageValues[0];
                    dNot.DeliveryInfo.Address= messageValues[1];
                    dNot.DeliveryInfo.DeliveryStatus = messageValues[2];
                    receiveSMSDeliveryStatusResponseData.Add(dNot);
                }
                sr.Close();
                receiveSMSDeliveryStatusResponseData.Reverse();
            }
        }

        Table notificationTable = new Table();
        notificationTable.Font.Name = "Sans-serif";
        notificationTable.Font.Size = 9;
        notificationTable.BorderStyle = BorderStyle.Outset;
        notificationTable.Width = Unit.Pixel(650);

        TableRow tableRow = new TableRow();

        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "Message ID";
        tableRow.Controls.Add(rowOneCellOne);

        rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "Address";
        tableRow.Controls.Add(rowOneCellOne);

        rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "Delivery Status";
        tableRow.Controls.Add(rowOneCellOne);

        notificationTable.Controls.Add(tableRow);

        foreach(DeliveryInfoNotification dNot in receiveSMSDeliveryStatusResponseData)
        {
            if (null != dNot.DeliveryInfo)
            {
                tableRow = new TableRow();

                rowOneCellOne = new TableCell();
                rowOneCellOne.Font.Bold = true;
                rowOneCellOne.Text = dNot.DeliveryInfo.Id;
                tableRow.Controls.Add(rowOneCellOne);

                rowOneCellOne = new TableCell();
                rowOneCellOne.Font.Bold = true;
                rowOneCellOne.Text = dNot.DeliveryInfo.Address;
                tableRow.Controls.Add(rowOneCellOne);

                rowOneCellOne = new TableCell();
                rowOneCellOne.Font.Bold = true;
                rowOneCellOne.Text = dNot.DeliveryInfo.DeliveryStatus;
                tableRow.Controls.Add(rowOneCellOne);
                notificationTable.Controls.Add(tableRow);
            }
        }

        notificationTable.BorderWidth = 1;
        notificationsPanel.Controls.Add(notificationTable);
        
    }

    /// <summary>
    /// This function is used to draw the table for get status success response
    /// </summary>
    /// <param name="status">Status as string</param>
    /// <param name="url">url as string</param>
    private void DrawGetStatusSuccess(string status, string url)
    {
        Table table = new Table();
        TableRow rowOne = new TableRow();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.Width = Unit.Pixel(650);
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellOne.Text = "Status: ";
        rowTwoCellOne.Font.Bold = true;
        rowTwo.Controls.Add(rowTwoCellOne);
        rowTwoCellTwo.Text = status.ToString();
        rowTwo.Controls.Add(rowTwoCellTwo);
        table.Controls.Add(rowTwo);
        TableRow rowThree = new TableRow();
        TableCell rowThreeCellOne = new TableCell();
        TableCell rowThreeCellTwo = new TableCell();
        rowThreeCellOne.Text = "ResourceURL: ";
        rowThreeCellOne.Font.Bold = true;
        rowThree.Controls.Add(rowThreeCellOne);
        rowThreeCellTwo.Text = url.ToString();
        rowThree.Controls.Add(rowThreeCellTwo);
        table.Controls.Add(rowThree);
        table.BorderWidth = 2;
        table.BorderColor = Color.DarkGreen;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc");
        getStatusPanel.Controls.Add(table);
    }

    /// <summary>
    /// This function is called to draw the table in the panelParam panel for success response
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="message">Message as string</param>
    private void DrawPanelForSuccess(Panel panelParam, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Font.Bold = true;
        rowTwoCellOne.Text = "Message ID:";
        rowTwoCellOne.Width = Unit.Pixel(70);
        rowTwo.Controls.Add(rowTwoCellOne);
        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellTwo.Text = message.ToString();
        rowTwo.Controls.Add(rowTwoCellTwo);
        table.Controls.Add(rowTwo);
        table.BorderWidth = 2;
        table.BorderColor = Color.DarkGreen;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#cfc");
        panelParam.Controls.Add(table);
    }

    /// <summary>
    /// This function draws table for failed response in the panalParam panel
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="message">Message as string</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "ERROR:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Text = message.ToString();
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        table.BorderWidth = 2;
        table.BorderColor = Color.Red;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc");
        panelParam.Controls.Add(table);
    }

    #endregion

    /** }@ */
    /** }@ */
}