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
using ATT_MSSDK;
using ATT_MSSDK.WapPush;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web.UI.WebControls;
#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for sending Wap Push
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke Wap APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.WapPush NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the wap related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For wap services MS SDK library provides APIs SendWapPush().
 * This methods return response objects WapPushResponse.
 
 * Sample code for sending wap push message:
 * -----------------------------------------
 List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 scopes.Add(RequestFactory.ScopeTypes.WAPPush);
 RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 WapPushResponse wapResponse = requestFactory.SendWapPush(address, url, alertText);
*/

/// <summary>
/// WapPush_App1 application
/// </summary>
/// <remarks>
/// This application allows a user to send a WAP Push message to a mobile device, by entering the address, alert text, and URL to be sent.
/// This application uses Autonomous Client Credentials consumption model to send messages. The user enters the alert text and URL, 
/// but the application in the background must build the push.txt file to attach with the requested values.
/// </remarks>
public partial class WapPush_App1 : System.Web.UI.Page
{
    /** \addtogroup WapPush_App1
     * Description of the application can be referred at \ref WapPush_app1 example
     * @{
     */

    /** \example WapPush_app1 wap\app1\Default.aspx.cs
     * \n \n This application allows a user to send a WAP Push message to a mobile device, by entering the address, alert text, and URL to be sent.  
     * \n \n This application uses Autonomous Client Credentials consumption model to send messages. The user enters the alert text and URL, 
     * but the application in the background must build the push.txt file to attach with the requested values.
     * <ul>
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Send WapPush Message:</b>
     * \n This method sends WAP Push message to one mobile network device.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.WapPush NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke \c SendWapPush() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
     * \n Sample code:
     * <pre>
     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
     *    scopes.Add(RequestFactory.ScopeTypes.WapPush);
     *    RequestFactory target = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
     *    WapPushResponse wapResponse = requestFactory.SendWapPush(address, url, alertText);</pre>
    * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * \n <li>Additional configuration to be done:</li>
     * \n Refer \ref parameters_sec section
     * 
     * <li>Documentation can be referred at \ref WapPush_App1 section</li></ul>
     * @{
     */

    #region Instance Variables

    /// <summary>
    /// Gets or sets the value of requestFactory
    /// </summary>
    private RequestFactory requestFactory = null;

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

    #endregion

    #region WapPush Application Events

    /// <summary>
   /// This function is called when the applicaiton page is loaded into the browser.
   /// </summary>
   /// <param name="sender">Button that caused this event</param>
   /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime currentServerTime = DateTime.UtcNow;
        serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

        this.Initialize();
    }

    /// <summary>
    /// Event that invokes send wap API of MS SDK wrapper library.
    /// </summary>
    /// <param name="sender">sender that invoked this event</param>
    /// <param name="e">eventargs of the button</param>
    protected void SendWAPButton_Click(object sender, EventArgs e)
    {
        try
        {
            WapPushResponse wapResponse = this.requestFactory.SendWapPush(txtAddressWAPPush.Text.Trim().ToString(), txtUrl.Text, txtAlert.Text);
            this.DrawPanelForSuccess(wapPanel, wapResponse.Id.ToString());
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(wapPanel, ex.Message);
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(wapPanel, ex.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(wapPanel, ex.ToString());
        }
    }

    #endregion

    #region WapPush Application specific functions

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
    /// Initialized the application variables with values from config file and displays errors, if any.
    /// </summary>
    /// <returns>true/false; true if able to read and assign to local variables; else false</returns>
    private bool Initialize()
    {
        if (this.requestFactory == null)
        {
            this.apiKey = ConfigurationManager.AppSettings["api_key"];
            if (string.IsNullOrEmpty(this.apiKey))
            {
                this.DrawPanelForFailure(wapPanel, "api_key is not defined in configuration file");
                return false;
            }

            this.secretKey = ConfigurationManager.AppSettings["secret_key"];
            if (string.IsNullOrEmpty(this.secretKey))
            {
                this.DrawPanelForFailure(wapPanel, "secret_key is not defined in configuration file");
                return false;
            }

            this.endPoint = ConfigurationManager.AppSettings["endPoint"];
            if (string.IsNullOrEmpty(this.endPoint))
            {
                this.DrawPanelForFailure(wapPanel, "endPoint is not defined in configuration file");
                return false;
            }

            List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
            scopes.Add(RequestFactory.ScopeTypes.WAPPush);
            this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        }
        return true;
    }

    #endregion

    /** }@ */
    /** }@ */
}