// <copyright file="Default.aspx.cs" company="AT&amp;T Intellectual Property">
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.IMMNv1;
#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for utilizing the usage of 
 * In App Messaging from Mobile Number  Api send message.
 * 
 * Pre-requisite:
 * --------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke IMMN APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.IMMNv1 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
  * 3. Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform)
 *    Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization.
 *    
 * 4. Retrive the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory 
 *    by passing 'code' as the argument.
 *     
 * 5. Invoke the IMMN related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For IMMN services MS SDK library provides APIs SendIMMN().
 * These methods returns SendMessageResponse object.
 
 * Sample code for sending message:
 * --------------------------------
    SendMessageResponse resp = requestFactory.SendIMMN(addresses, attachments, text, subject, groupMessage, maxAddressCount);
 *
*/

/// <summary>
/// This application allows the user to send SMS and MMS on behalf of subscriber, 
/// with subscriber’s consent, using the IMMN API.
/// </summary>
public partial class MOBO_app1 : System.Web.UI.Page
{
    /** \addtogroup IMMN_App1
     * Description of the application can be referred at \ref IMMN_app1 example
     * @{
     */

    /** \example IMMN_app1 immn\app1\Default.aspx.cs
     * \n \n This application allows the user to send SMS and MMS on behalf of subscriber, with subscriber's consent, using the IMMN API 
     * <ul> 
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Send Messages:</b>
     * \n  This method is used to send SMS or MMS to maximum of ten recipients or configured number of recipients. The recipients can be email address, short code or mobile numbers or combination of email address, short code or/and mobile numbers. 
     * These Messages are processed synchronously by the AT&T system and are sent asynchronously to the destination.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.IMMNv1 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform). 
     * Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization.
     * \n AT&T displays the login and consent page. After getting the consent from the user, AT&T platform returns 'code' as a query parameter.</li>
     * <li>Retrive the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory by passing 'code' as the argument.</li>
     * <li>Invoke \c SendIMMN() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
     * \n Sample code:
     * <ul>
     * <li>Initializing \c RequestFactory instance:</li>
     * <pre>
     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
     *    scopes.Add(RequestFactory.ScopeTypes.MIM);
     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null); </pre>
     * <li>Getting user consent:</li>
     * <pre>
     *    Response.Redirect(requestFactory.GetOAuthRedirect());  </pre>
     * <li>Getting Access Token from AuthCode:</li>
     * <pre>
     *    In Page Load:
     *      if (!string.IsNullOrEmpty(Request["code"]))
     *       {
     *            authCode = Request["code"].ToString();
     *            this.requestFactory.GetAuthorizeCredentials(authCode);
     *       }</pre>
     * <li>Sending Message:</li>
     * <pre>
     *     SendMessageResponse response = requestFactory.SendIMMN(addresses, attachments, text, subject, groupMessage, maxAddressCount);</pre></ul>
     * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * \n <li>Additional configuration to be done:</li> \n
     * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
     * \li authorize_redirect_uri - This is mandatory key and value should be equal to  MIM Service registered applicaiton 'OAuth Redirect URL'
     * 
     * \n <li>Documentation can be referred at \ref MIM_App1 section</li></ul>
     * @{
     */

    #region Instance Variables
    /// <summary>
    /// Request Factory object for calling api functions
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Application Service specific variables
    /// </summary>
    private string authCode, apiKey, secretKey, endPoint, redirectUrl;

    /// <summary>
    /// Maximum number of addresses to send
    /// </summary>
    private int maxAddresses;
        
    #endregion

    #region Application Events
    /// <summary>
   /// This menthod is called when the page is loading
   /// </summary>
   /// <param name="sender">object pointing to the caller</param>
   /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            DateTime currentServerTime = DateTime.UtcNow;
            lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
            
            if (this.requestFactory == null)
            {
                bool ableToReadFromConfig = this.ReadConfigFile();
                if (ableToReadFromConfig == false)
                {
                    return;
                }

                this.InitializeRequestFactory();
            }

            if (!Page.IsPostBack)
            {
                if (Session["CSMOBO_ACCESS_TOKEN"] == null)
                {
                    if (!string.IsNullOrEmpty(Request["code"]))
                    {
                        this.authCode = Request["code"];
                        this.requestFactory.GetAuthorizeCredentials(this.authCode);
                        Session["CSMOBO_ACCESS_TOKEN"] = this.requestFactory.AuthorizeCredential;
                    }
                }

                if (null != Session["CS_MessageToSend"])
                {
                    MessageStructure msg = (MessageStructure)Session["CS_MessageToSend"];
                    txtMessage.Text = msg.Message;
                    string temp = string.Empty;

                    foreach (string addr in msg.Addresses)
                    {
                        temp += addr + ",";
                    }

                    if (!string.IsNullOrEmpty(temp))
                    {
                        temp = temp.Substring(0, temp.LastIndexOf(","));
                    }

                    txtPhone.Text = temp;
                    txtSubject.Text = msg.Subject;
                    chkGroup.Checked = msg.Group;
                    this.SendMessage(msg);
                }
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(statusPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
    }

    /// <summary>
    /// Method will be called when the user clicks on send message button. This method calls SendIMMN function of Request Factory
    /// </summary>
    /// <param name="sender">object pointing to the caller</param>
    /// <param name="e">Event arguments</param>
    protected void BtnSendMessage_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtPhone.Text))
        {
            this.DrawPanelForFailure(statusPanel, "Address field cannot be blank");
            return;
        }

        MessageStructure msg = this.GetUserInput();
        if (null != msg && null != msg.Addresses)
        {
            if (msg.Addresses.Count > this.maxAddresses)
            {
                this.DrawPanelForFailure(statusPanel, "Maximum addresses can not exceed " + this.maxAddresses.ToString());
                return;
            }

            if (null == msg.Attachments || 0 == msg.Attachments.Count)
            {
                // Message is mandatory, if no attachments
                if (string.IsNullOrEmpty(txtMessage.Text))
                {
                    this.DrawPanelForFailure(statusPanel, "Specify message/attachment to be sent");
                    return;
                }
            }

            this.SendMessage(msg);
        }
    }
    #endregion

    #region Send Message Functions

    /// <summary>
    /// This method reads from Config file and assigns to local variables.
    /// </summary>
    /// <returns>true/false; true if able to read from config; else false.</returns>
    private bool ReadConfigFile()
    {
        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(statusPanel, "api_key is not specified in Config file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(statusPanel, "secret_key is not specified in Config file");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endpoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(statusPanel, "endpoint is not specified in Config file");
            return false;
        }

        this.redirectUrl = ConfigurationManager.AppSettings["authorize_redirect_uri"];
        if (string.IsNullOrEmpty(this.redirectUrl))
        {
            this.DrawPanelForFailure(statusPanel, "authorize_redirect_uri is not specified in Config file");
            return false;
        }

        this.maxAddresses = 10;
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["max_addresses"]))
        {
            this.maxAddresses = Convert.ToInt32(ConfigurationManager.AppSettings["max_addresses"]);
        }

        return true;
    }

    /// <summary>
    /// Initialize a new instance of RequestFactory.
    /// </summary>
    private void InitializeRequestFactory()
    {
        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.IMMN);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, this.redirectUrl, null);

        if (null != Session["CSMOBO_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSMOBO_ACCESS_TOKEN"];
        }
    }

    /// <summary>
    /// Prepares a list based on a string seperated by comma(,)
    /// </summary>
    /// <param name="strToList">String value to convert to list</param>
    /// <returns>List of strings</returns>
    private List<string> GetList(string strToList)
    {
        List<string> list = new List<string>();
        string[] ls = strToList.Split(',');
        foreach (string value in ls)
        {
            list.Add(value);
        }

        return list;
    }  

    /// <summary>
    /// Gets Message object based on user input
    /// </summary>
    /// <returns>Message object</returns>
    private MessageStructure GetUserInput()
    {
        MessageStructure msg = new MessageStructure();
        msg.Attachments = new List<string>();

        if (!string.IsNullOrEmpty(fileUpload1.FileName))
        {
            fileUpload1.SaveAs(Request.MapPath(fileUpload1.FileName));
            msg.Attachments.Add(Request.MapPath(fileUpload1.FileName));
        }

        if (!string.IsNullOrEmpty(fileUpload2.FileName))
        {
            fileUpload2.SaveAs(Request.MapPath(fileUpload2.FileName));
            msg.Attachments.Add(Request.MapPath(fileUpload2.FileName));
        }

        if (!string.IsNullOrEmpty(fileUpload3.FileName))
        {
            fileUpload3.SaveAs(Request.MapPath(fileUpload3.FileName));
            msg.Attachments.Add(Request.MapPath(fileUpload3.FileName));
        }

        if (!string.IsNullOrEmpty(fileUpload4.FileName))
        {
            fileUpload4.SaveAs(Request.MapPath(fileUpload4.FileName));
            msg.Attachments.Add(Request.MapPath(fileUpload4.FileName));
        }

        if (!string.IsNullOrEmpty(fileUpload5.FileName))
        {
            fileUpload5.SaveAs(Request.MapPath(fileUpload5.FileName));
            msg.Attachments.Add(Request.MapPath(fileUpload5.FileName));
        }

        msg.Message = txtMessage.Text;
        msg.Subject = txtSubject.Text;
        msg.Group = chkGroup.Checked;
        msg.Addresses = this.GetList(txtPhone.Text);

        Session["CS_MessageToSend"] = msg;
        return msg;
    }

    /// <summary>
    /// Initiates SendIMMN call to Request Factory
    /// </summary>
    /// <param name="msgToSend">Message object</param>
    private void SendMessage(MessageStructure msgToSend)
    {
        if (null != Session["CSMOBO_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSMOBO_ACCESS_TOKEN"];
        }

        if (this.requestFactory.AuthorizeCredential == null)
        {
            Response.Redirect(this.requestFactory.GetOAuthRedirect().ToString());
        }

        try
        {
            SendMessageResponse response = this.requestFactory.SendIMMN(msgToSend.Addresses, msgToSend.Attachments, msgToSend.Message, msgToSend.Subject, msgToSend.Group);
            if (null != response)
            {
                this.DrawPanelForSuccess(statusPanel, response.Id);
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(statusPanel, ie.Body);
        }
        catch (TokenExpiredException te)
        {
            this.DrawPanelForFailure(statusPanel, te.Message);
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(statusPanel, ae.Message);
        }
        catch (UnauthorizedRequest ur)
        {
            this.DrawPanelForFailure(statusPanel, ur.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
        finally
        {
            foreach (string file in msgToSend.Attachments)
            {
                if (System.IO.File.Exists(file))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch { }
                }
            }

            Session["CS_MessageToSend"] = null;
        }
    }
    #endregion

    #region Display Status Functions
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
        table.CssClass = "success";
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
        rowTwoCellOne.Font.Bold = true;
        rowTwoCellOne.Text = "Message ID:";
        rowTwoCellOne.Width = Unit.Pixel(70);
        rowTwo.Controls.Add(rowTwoCellOne);
        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellTwo.Text = message;
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
        rowTwoCellOne.Text = message;
        rowTwo.Controls.Add(rowTwoCellOne);
        table.Controls.Add(rowTwo);
        panelParam.Controls.Add(table);
    }
    #endregion

    #region Message Data Structure
    /// <summary>
    /// message container for sending sms/mms
    /// </summary>
    [Serializable]
    private class MessageStructure
    {
        /// <summary>
        /// Gets or sets List of addresses
        /// </summary>
        public List<string> Addresses { get; set; }
        
        /// <summary>
        /// Gets or sets List of attachments
        /// </summary>
        public List<string> Attachments { get; set; }
        
        /// <summary>
        /// Gets or sets Message to be sent
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets Subject of the message
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether message to be sent to group or individual
        /// </summary>
        public bool Group { get; set; }
    }
    #endregion
    /** }@ */
    /** }@ */
}