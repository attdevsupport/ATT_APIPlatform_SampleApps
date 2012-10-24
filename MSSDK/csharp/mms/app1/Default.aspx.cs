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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.MMSv2;
#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for sending MMS and
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
 * 1. Import ATT_MSSDK and ATT_MSSDK.MMSv2 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
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
/// Mms_App1 class
/// </summary>
/// <remarks> This application allows an end user to send an MMS message with up to three attachments of any common format, 
/// and check the delivery status of that MMS message.
/// </remarks>
public partial class MMS_App1 : System.Web.UI.Page
{
    /** \addtogroup MMS_App1
     * Description of the application can be referred at \ref MMS_app1 example
     * @{
     */

    /** \example MMS_app1 mms\app1\Default.aspx.cs
     * \n \n This application allows an end user to send an MMS message with up to three attachments of any common format, and check the delivery status of that MMS message.
     * <ul> 
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Send MMS:</b>
     * \n This method sends MMS to one or more mobile network devices.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.MMSv2 NameSpace.</li>
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
     * \n <li>Documentation can be referred at \ref MMS_App1 section</li></ul>
     * @{
     */

    #region Instance Variables

    /// <summary>
    /// Gets or sets the value of requestFactory object
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Temporary variables for processing requests
    /// </summary>
    private string apiKey, secretKey, endPoint;

    /// <summary>
    /// List of attachments
    /// </summary>
    private List<string> mmsAttachments;

    #endregion

    #region MMS Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();

        DateTime currentServerTime = DateTime.UtcNow;
        serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
        if (this.requestFactory == null)
        {
            this.Initialize();
        }
    }

    /// <summary>
    /// This method will be called when user clicks on send mms button
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void SendMMSMessageButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(phoneTextBox.Text))
            {
                this.DrawPanelForFailure(sendMessagePanel, "Specify phone number");
                return;
            }

            long fileSize = 0;
            if (!string.IsNullOrEmpty(FileUpload1.FileName))
            {
               this.mmsAttachments.Add(Request.MapPath(FileUpload1.FileName));
                FileUpload1.PostedFile.SaveAs(Request.MapPath(FileUpload1.FileName));
                fileSize = fileSize + (FileUpload1.PostedFile.ContentLength / 1024);
            }

            if (!string.IsNullOrEmpty(FileUpload2.FileName))
            {
                this.mmsAttachments.Add(Request.MapPath(FileUpload2.FileName));
                FileUpload2.PostedFile.SaveAs(Request.MapPath(FileUpload2.FileName));
                fileSize = fileSize + (FileUpload2.PostedFile.ContentLength / 1024);
            }

            if (!string.IsNullOrEmpty(FileUpload3.FileName))
            {
                this.mmsAttachments.Add(Request.MapPath(FileUpload3.FileName));
                FileUpload3.PostedFile.SaveAs(Request.MapPath(FileUpload3.FileName));
                fileSize = fileSize + (FileUpload3.PostedFile.ContentLength / 1024);
            }

            if (fileSize <= 600)
            {
                this.SendMMS();

                if (File.Exists(Request.MapPath(FileUpload1.FileName)))
                {
                    File.Delete(Request.MapPath(FileUpload1.FileName));
                }

                if (File.Exists(Request.MapPath(FileUpload2.FileName)))
                {
                    File.Delete(Request.MapPath(FileUpload2.FileName));
                }

                if (File.Exists(Request.MapPath(FileUpload3.FileName)))
                {
                    File.Delete(Request.MapPath(FileUpload3.FileName));
                }
            }
            else
            {
                this.DrawPanelForFailure(sendMessagePanel, "Attachment file size exceeded 600kb");
                return;
            }
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.ToString());
        }
    }

    /// <summary>
    /// This method will be called when user click on get status button
    /// </summary>
    /// <param name="sender">object, that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void GetStatusButton_Click(object sender, EventArgs e)
    {
        try
        {
            string mmsId = messageIDTextBox.Text.Trim();
            if (mmsId == null || mmsId.Length <= 0)
            {
                this.DrawPanelForFailure(getStatusPanel, "Message Id is null or empty");
                return;
            }

            MmsDeliveryResponse mmsDeliveryResponseObj = this.requestFactory.GetMmsDeliveryResponse(mmsId);
            this.DrawGetStatusSuccess(mmsDeliveryResponseObj.DeliveryInfo[0].DeliveryStatus, mmsDeliveryResponseObj.ResourceURL);
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(getStatusPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(getStatusPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(getStatusPanel, ex.ToString());
        }
    }

    #endregion

    #region MMS Application specific functions

    /// <summary>
    /// This method neglects the ssl handshake error with authentication server
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
    /// Initializes local variables with values from config file and creates requestFactory object
    /// </summary>
    private void Initialize()
    {
        if (this.requestFactory == null)
        {
            this.apiKey = ConfigurationManager.AppSettings["api_key"];
            if (string.IsNullOrEmpty(this.apiKey))
            {
                this.DrawPanelForFailure(sendMessagePanel, "api_key is not defined in configuration file");
                return;
            }

            this.secretKey = ConfigurationManager.AppSettings["secret_key"];
            if (string.IsNullOrEmpty(this.secretKey))
            {
                this.DrawPanelForFailure(sendMessagePanel, "secret_key is not defined in configuration file");
                return;
            }

            this.endPoint = ConfigurationManager.AppSettings["endPoint"];
            if (string.IsNullOrEmpty(this.endPoint))
            {
                this.DrawPanelForFailure(sendMessagePanel, "endPoint is not defined in configuration file");
                return;
            }
           
            this.mmsAttachments = new List<string>();

            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.MMS);
            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        }
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
    /// Displays Resource url upon success of GetMmsDelivery
    /// </summary>
    /// <param name="status">string, status of the request</param>
    /// <param name="url">string, url of the resource</param>
    private void DrawGetStatusSuccess(string status, string url)
    {
        if (getStatusPanel.HasControls())
        {
            getStatusPanel.Controls.Clear();
        }

        Table table = new Table();
        table.CssClass = "successWide";
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;

        TableRow rowOne = new TableRow();
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

        getStatusPanel.Controls.Add(table);
    }

    /// <summary>
    /// Invoke SendMms method of requestFactory object
    /// </summary>
    private void SendMMS()
    {
        try
        {
            MmsResponse resp;
            if (null != this.mmsAttachments && this.mmsAttachments.Count == 0)
            {
                resp = this.requestFactory.SendMms(phoneTextBox.Text.Trim(), messageTextBox.Text.Trim());
            }
            else
            {
                resp = this.requestFactory.SendMms(phoneTextBox.Text.Trim(), messageTextBox.Text.Trim(), this.mmsAttachments);
            }

            messageIDTextBox.Text = resp.Id;
            this.DrawPanelForSuccess(sendMessagePanel, resp.Id);
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(sendMessagePanel, ex.ToString());
        }
    }

    #endregion

    /** }@ */
    /** }@ */
}