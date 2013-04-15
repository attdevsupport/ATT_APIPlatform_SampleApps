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
using System.IO;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.MMSv3;
#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Coupons to the handsets and
 * getting delivery status of mms.
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke MMS APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.MMSv3 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and MMS scopes.
 * The RequestFactory may specify any combination of Payment or MMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the mms related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For mms services MS SDK library provides APIs SendMms() and GetMmsDeliveryResponse()
 * These methods return response objects MmsResponse, MmsDeliveryResponse.
 
 * Sample code for sending mms:
 * ----------------------------
 List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 scopes.Add(RequestFactory.ScopeTypes.MMS);
 RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 MmsResponse resp = requestFactory.SendMms(PhoneNumber, Subject, mmsAttachments);
 
 * Sample code for getting MMS delivery status:
 * --------------------------------------------
  MmsDeliveryResponse resp = requestFactory.GetMmsDeliveryResponse(MmsId);

*/

/// <summary>
/// MMS_App2 class
/// </summary>
/// <remarks>
/// This is a server side application which also has a web interface. 
/// The application looks for a file called numbers.txt containing MSISDNs of desired recipients, and an image called coupon.jpg, 
/// and message text from a file called subject.txt, and then sends an MMS message with the attachment to every recipient in the list. 
/// This can be triggered via a command line on the server, or through the web interface, which then displays all the returned mmsIds or respective errors
/// </remarks>
public partial class MMS_App2 : System.Web.UI.Page
{
    /** \addtogroup MMS_App2
      * Description of the application can be referred at \ref MMS_app2 example
      * @{
      */

    /** \example MMS_app2 mms\app2\Default.aspx.cs
     * \n \n This application allows an end user to send an MMS message with up to three attachments of any common format, and check the delivery status of that MMS message.
     * <ul> 
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Send MMS:</b>
     * \n This method sends MMS to one or more mobile network devices.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.MMSv3 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke \c SendMms() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
     * Sample code:
     * <pre>
     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
     *    scopes.Add(RequestFactory.ScopeTypes.MMS);
     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
     *    MmsResponse resp = requestFactory.SendMms(PhoneNumber, Subject, mmsAttachments);</pre>
     * <b>Get MMS Delivery status:</b>
     * <pre>
     *    MmsDeliveryResponse resp = requestFactory.GetMmsDeliveryResponse(MmsId);</pre>
     * 
     <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * \n <li>Additional configuration to be done:</li> \n
     * refer \ref parameters_sec section
     * 
     * \n <li>Documentation can be referred at \ref MMS_App2 section</li></ul>
     * @{
     */

    #region Instance Variables

    /// <summary>
    /// Gets or sets the value of requestFactory object
    /// </summary>
    private RequestFactory requestFactory;
    
    /// <summary>
    /// Gets or sets the value of apiKey
    /// </summary>
    private string apiKey;
    
    /// <summary>
    /// Gets or sets the value of secretKey
    /// </summary>
    private string secretKey;

    /// <summary>
    /// Gets or sets the value of endPoint
    /// </summary>
    private string endPoint;

    /// <summary>
    /// Gets or sets the values of filepath variables
    /// </summary>
    private string messageFilePath, phoneListFilePath, couponFilePath;

    /// <summary>
    /// List of addresses to be delivered
    /// </summary>
    private List<string> mmsAddressList;

    /// <summary>
    /// List of MMS attachments
    /// </summary>
    private List<string> mmsAttachments;

    /// <summary>
    /// Instance variables for get status table
    /// </summary>
    private Table getStatusTable, secondTable;

    #endregion
    
    #region MMS Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            DateTime currentServerTime = DateTime.UtcNow;
            serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

            if (this.Initialize() == false)
            {
                return;
            }

            this.mmsAttachments.Add(Request.MapPath(this.couponFilePath));
            Image1.ImageUrl = this.couponFilePath;
            
            using (StreamReader mmsSubjectReader = new StreamReader(Request.MapPath(this.messageFilePath)))
            {
                subjectLabel.Text = mmsSubjectReader.ReadToEnd();
                mmsSubjectReader.Close();
            }
            
            if (!Page.IsPostBack)
            {
                using (StreamReader phoneNumberReader = new StreamReader(Request.MapPath(this.phoneListFilePath)))
                {
                    phoneListTextBox.Text = phoneNumberReader.ReadToEnd();
                    phoneNumberReader.Close();
                }
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendMMSPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This method will be called when user clicks on send mms button
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void SendButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(phoneListTextBox.Text))
            {
                this.DrawPanelForFailure(sendMMSPanel, "Specify phone number");
                return;
            }

            string[] phoneNumbers = phoneListTextBox.Text.Split(',');
            List<string> phoneNumbersList = new List<string>(phoneNumbers.Length);
            phoneNumbersList.AddRange(phoneNumbers);

            MmsResponse resp = this.requestFactory.SendMms(phoneNumbersList, subjectLabel.Text.Trim(), this.mmsAttachments);
            msgIdLabel.Text = resp.MessageId;
            this.DrawPanelForSuccess(sendMMSPanel, resp.MessageId);
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(sendMMSPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(sendMMSPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendMMSPanel, ex.ToString());
        }
    }

    /// <summary>
    /// This method will be called when user clicks on  get status button
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void StatusButton_Click(object sender, EventArgs e)
    {
        try
        {
            MmsDeliveryResponse mmsDeliveryResponseObj = this.requestFactory.GetMmsDeliveryResponse(msgIdLabel.Text.Trim());
            this.DrawPanelForGetStatusResult(null, null, null, true);

            foreach (DeliveryInfo deliveryInfo in mmsDeliveryResponseObj.DeliveryInfo)
            {
                this.DrawPanelForGetStatusResult(deliveryInfo.Id, deliveryInfo.Address, deliveryInfo.DeliveryStatus, false);
            }

            msgIdLabel.Text = string.Empty;
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.ToString());
        }
    }

    #endregion

    #region MMS Application specific functions

    /// <summary>
    /// Instantiate RequestFactory of ATT_MSSDK by passing endPoint, apiKey, secretKey, scopes
    /// </summary>
    /// <returns>true/false; true if able to read else false</returns>
    private bool Initialize()
    {
        if (this.requestFactory == null)
        {
            this.apiKey = ConfigurationManager.AppSettings["api_key"];
            if (string.IsNullOrEmpty(this.apiKey))
            {
                this.DrawPanelForFailure(sendMMSPanel, "api_key is not defined in configuration file");
                return false;
            }

            this.secretKey = ConfigurationManager.AppSettings["secret_key"];
            if (string.IsNullOrEmpty(this.secretKey))
            {
                this.DrawPanelForFailure(sendMMSPanel, "secret_key is not defined in configuration file");
                return false;
            }

            this.endPoint = ConfigurationManager.AppSettings["endPoint"];
            if (string.IsNullOrEmpty(this.endPoint))
            {
                this.DrawPanelForFailure(sendMMSPanel, "endPoint is not defined in configuration file");
                return false;
            }

            this.messageFilePath = ConfigurationManager.AppSettings["messageFilePath"];
            if (string.IsNullOrEmpty(this.messageFilePath))
            {
                this.DrawPanelForFailure(sendMMSPanel, "Message file path is missing in configuration file");
                return false;
            }

            this.phoneListFilePath = ConfigurationManager.AppSettings["phoneListFilePath"];
            if (string.IsNullOrEmpty(this.phoneListFilePath))
            {
                this.DrawPanelForFailure(sendMMSPanel, "Phone list file path is missing in configuration file");
                return false;
            }

            this.couponFilePath = ConfigurationManager.AppSettings["couponFilePath"];
            if (string.IsNullOrEmpty(this.couponFilePath))
            {
                this.DrawPanelForFailure(sendMMSPanel, "Coupon file name is missing in configuration file");
                return false;
            }

            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.MMS);

            this.mmsAddressList = new List<string>();
            this.mmsAttachments = new List<string>();

            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);            
        }

        return true;
    }

    /// <summary>
    /// Display success message
    /// </summary>
    /// <param name="panelParam">Panel to draw success message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForSuccess(Panel panelParam, string message)
    {
        if (panelParam.HasControls())
        {
            panelParam.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "successWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOneCellOne.Width = Unit.Pixel(75);
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);

        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Font.Bold = true;
        rowTwoCellOne.Text = "Message ID:";
        rowTwoCellOne.Width = Unit.Pixel(75);
        rowTwo.Controls.Add(rowTwoCellOne);

        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellTwo.Text = message;
        rowTwoCellTwo.HorizontalAlign = HorizontalAlign.Left;
        rowTwo.Controls.Add(rowTwoCellTwo);
        table.Controls.Add(rowTwo);

        panelParam.Controls.Add(table);
    }

    /// <summary>
    /// Displays error message
    /// </summary>
    /// <param name="panelParam">Panel to draw success message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        if (panelParam.HasControls())
        {
            panelParam.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "errorWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
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
        panelParam.Controls.Add(table);
    }

    /// <summary>
    /// This method draws table for get status response
    /// </summary>
    /// <param name="msgid">string, Message Id</param>
    /// <param name="phone">string, phone number</param>
    /// <param name="status">string, status</param>
    /// <param name="headerFlag">bool, headerFlag</param>
    private void DrawPanelForGetStatusResult(string msgid, string phone, string status, bool headerFlag)
    {
        if (headerFlag == true)
        {
            this.getStatusTable = new Table();
            this.getStatusTable.CssClass = "successWide";
            this.getStatusTable.Font.Name = "Sans-serif";
            this.getStatusTable.Font.Size = 9;

            TableRow rowOne = new TableRow();
            TableCell rowOneCellOne = new TableCell();
            rowOneCellOne.Width = Unit.Pixel(110);
            rowOneCellOne.Font.Bold = true;
            rowOneCellOne.Text = "SUCCESS:";
            rowOne.Controls.Add(rowOneCellOne);
            this.getStatusTable.Controls.Add(rowOne);
            TableRow rowTwo = new TableRow();
            TableCell rowTwoCellOne = new TableCell();
            rowTwoCellOne.Width = Unit.Pixel(250);
            rowTwoCellOne.Text = "Messages Delivered";

            rowTwo.Controls.Add(rowTwoCellOne);
            this.getStatusTable.Controls.Add(rowTwo);
            this.getStatusTable.Controls.Add(rowOne);
            this.getStatusTable.Controls.Add(rowTwo);
            statusPanel.Controls.Add(getStatusTable);

            this.secondTable = new Table();
            this.secondTable.Font.Name = "Sans-serif";
            this.secondTable.Font.Size = 9;
            this.secondTable.Width = Unit.Pixel(650);
            TableRow tableRow = new TableRow();
            TableCell tableCell = new TableCell();
            tableCell.Width = Unit.Pixel(300);
            tableCell.Text = "Recipient";
            tableCell.HorizontalAlign = HorizontalAlign.Center;
            tableCell.Font.Bold = true;
            tableRow.Cells.Add(tableCell);
            tableCell = new TableCell();
            tableCell.Font.Bold = true;
            tableCell.Width = Unit.Pixel(300);
            tableCell.Wrap = true;
            tableCell.Text = "Status";
            tableCell.HorizontalAlign = HorizontalAlign.Center;
            tableRow.Cells.Add(tableCell);
            this.secondTable.Rows.Add(tableRow);
            statusPanel.Controls.Add(secondTable);
        }
        else
        {
            TableRow row = new TableRow();
            TableCell cell1 = new TableCell();
            TableCell cell2 = new TableCell();
            cell1.Text = phone.ToString();
            cell1.Width = Unit.Pixel(300);
            cell1.HorizontalAlign = HorizontalAlign.Center;
            row.Controls.Add(cell1);
            cell2.Text = status.ToString();
            cell2.Width = Unit.Pixel(300);
            cell2.HorizontalAlign = HorizontalAlign.Center;
            row.Controls.Add(cell2);
            this.secondTable.Controls.Add(row);
            statusPanel.Controls.Add(secondTable);
        }
    }

    #endregion

    /** }@ */
    /** }@ */
}