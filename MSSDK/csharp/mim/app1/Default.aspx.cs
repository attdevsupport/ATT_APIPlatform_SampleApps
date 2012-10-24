// <copyright file="Default.aspx.cs" company="AT&amp;T Intellectual Property">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.MIMv1;
#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for Message Inbox Management Api.
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke MIM APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.MIMv1 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the MIM related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For MIM services MS SDK library provides APIs GetMessageHeaders() and GetMessage()
 * These methods return response objects GetMessageHeadersResponse, GetMessageContentResponse.
 
 * Sample code for getting message headers:
 * ----------------------------
 List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 scopes.Add(RequestFactory.ScopeTypes.MIM);
 RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 GetMessageHeadersResponse response = requestFactory.GetMessageHeaders(headerCount);
 
 * Sample code for getting message content:
 * ----------------------------------------
 * GetMessageContentResponse response = requestFactory.GetMessage(messageId, partNumber);
 * 
 */

/// <summary>
/// This application allows the AT&T subscriber access to message related data 
/// stored in the AT&amp;T Messages environment.
/// </summary>
public partial class MIM_App1 : System.Web.UI.Page
{
    /** \addtogroup MIM_App1
     * Description of the application can be referred at \ref MIM_app1 example
     * @{
     */

    /** \example MIM_app1 mim\app1\Default.aspx.cs
     * \n \n This application allows the AT&T subscriber access to message related data stored in the AT&T Messages environment. 
     * <ul> 
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>GetMessageHeaders:</b>
     * \n This method is used to request meta-data for one or more Subscriber Messages from the AT&T Messages environment.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.MIMv1 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform). 
     * Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization.
     * \n AT&T displays the login and consent page. After getting the consent from the user, AT&T platform returns 'code' as a query parameter.</li>
     * <li>Retrive the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory by passing 'code' as the argument.</li>
     * <li>Invoke \c GetMessageHeaders() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
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
     * <li>Get Message Headers:</li>
     * <pre>
     *    GetMessageHeadersResponse response = requestFactory.GetMessageHeaders(headerCount);</pre></ul>
     * <b>GetMessageContent:</b>
     * \n This method uses information returned by the Get Message Headers operation to enable applications to fetch one or more subscriber messages from the AT&T Messages environment.
     * <pre>
     *          GetMessageContentResponse response = requestFactory.GetMessage(messageId, partNumber); </pre>
     * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * \n <li>Additional configuration to be done:</li> \n
     * \n Apart from parameters specified in \ref parameters_sec section, the following parameters need to be specified for this application
     * \li authorize_redirect_uri - This is mandatory key and value should be equal to  MIM Service registered applicaiton 'OAuth Redirect URL'
     * 
     * \n <li>Documentation can be referred at \ref MIM_App1 section</li></ul>
     * @{
     */

    #region Instance variables
    /// <summary>
    /// Request Factory object for calling api functions
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Application Service specific variables
    /// </summary>
    private string authCode, apiKey, secretKey, endPoint, redirectUrl;

    #endregion

    #region Application events
    
    /// <summary>
    /// This menthod is called when the page is loading
    /// </summary>
    /// <param name="sender">object pointing to the caller</param>
    /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            pnlHeader.Visible = false;
            imagePanel.Visible = false;
            smilpanel.Visible = false;
            DateTime currentServerTime = DateTime.UtcNow;
            lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
            if (this.requestFactory == null)
            {
                bool ableToReadConfigFile = this.ReadConfigFile();
                if (ableToReadConfigFile == false)
                {
                    return;
                }

                this.InitializeRequestFactory();
            }

            // Get Access Token
            if (!Page.IsPostBack)
            {
                if (Session["CSMIM_ACCESS_TOKEN"] == null)
                {
                    if (!string.IsNullOrEmpty(Request["code"]))
                    {
                        this.authCode = Request["code"];

                        if (null == this.requestFactory.AuthorizeCredential)
                        {
                            this.requestFactory.GetAuthorizeCredentials(this.authCode);
                        }

                        Session["CSMIM_ACCESS_TOKEN"] = this.requestFactory.AuthorizeCredential;
                    }
                }

                // Process the requests stored in the session
                bool userEnteredAnyValues = this.GetSessionValues();
                if (userEnteredAnyValues == true)
                {
                    if (Session["CSSDKRequest"] != null && Session["CSSDKRequest"].ToString().Equals("GetMessageHeaders"))
                    {
                        this.GetMessageHeaders();
                    }
                    else if (Session["CSSDKRequest"] != null && Session["CSSDKRequest"].ToString().Equals("GetMessageContent"))
                    {
                        this.GetMessageContentByMessage();
                    }
                }
            }
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(statusPanel, ie.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
    }

    /// <summary>
    /// Event, that gets called when user clicks on Get message headers button, 
    /// performs validations and calls GetMessageHeaders() in RequestFactory instance.
    /// </summary>
    /// <param name="sender">object that initiated this method</param>
    /// <param name="e">Event Agruments</param>
    protected void GetHeaderButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtHeaderCount.Text.Trim()))
        {
            this.DrawPanelForFailure(statusPanel, "Specify number of messages to be retrieved");
            return;
        }

        Regex regex = new Regex(@"\d+");
        if (!regex.IsMatch(txtHeaderCount.Text.Trim()))
        {
            this.DrawPanelForFailure(statusPanel, "Specify valid header count");
            return;
        }

        txtHeaderCount.Text = txtHeaderCount.Text.Trim();
        Session["CSSDKHeaderCount"] = txtHeaderCount.Text;
        Session["CSSDKIndexCursor"] = txtIndexCursor.Text;

        int headerCount = Convert.ToInt32(txtHeaderCount.Text.Trim());
        if (headerCount < 1 || headerCount > 500)
        {
            this.DrawPanelForFailure(statusPanel, "Header Count must be a number between 1-500");
            return;
        }

        Session["CSSDKRequest"] = "GetMessageHeaders";

        this.GetMessageHeaders();
    }

    /// <summary>
    /// Event, that gets called when user clicks on Get message content button, performs validations and initiates api call to get message content
    /// </summary>
    /// <param name="sender">object that initiated this method</param>
    /// <param name="e">Event Agruments</param>    
    protected void GetMessageContent_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtMessageId.Text))
        {
            this.DrawPanelForFailure(ContentPanelStatus, "Specify Message ID");
            return;
        }

        if (string.IsNullOrEmpty(txtPartNumber.Text))
        {
            this.DrawPanelForFailure(ContentPanelStatus, "Specify Part Number of the message");
            return;
        }

        Session["CSSDKMessageID"] = txtMessageId.Text;
        Session["CSSDKPartNumber"] = txtPartNumber.Text;

        Session["CSSDKRequest"] = "GetMessageContent";

        this.GetMessageContentByMessage();
    }
#endregion

    #region MIM Invocation methods

    /// <summary>
    /// Gets message headers based on header count and index cursor.
    /// </summary>
    private void GetMessageHeaders()
    {
        try
        {
            if (null != Session["CSMIM_ACCESS_TOKEN"])
            {
                this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSMIM_ACCESS_TOKEN"];
            }

            if (this.requestFactory.AuthorizeCredential == null)
            {
                Response.Redirect(this.requestFactory.GetOAuthRedirect().ToString());
            }

            GetMessageHeadersResponse headerResponse = this.requestFactory.GetMessageHeaders(Convert.ToInt32(txtHeaderCount.Text), txtIndexCursor.Text);
            if (null != headerResponse)
            {
                MessageHeadersListData listData = headerResponse.MessageHeadersList;
                if (null != listData)
                {
                    this.DisplayGrid(listData);
                }
            }
        }
        catch (TokenExpiredException te)
        {
            this.DrawPanelForFailure(ContentPanelStatus, te.Message);
        }
        catch (UnauthorizedRequest ur)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ur.Message);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ie.Body);
        }
        catch (ArgumentNullException are)
        {
            this.DrawPanelForFailure(ContentPanelStatus, are.Message);
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ae.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ex.Message);
        }
    }

    /// <summary>
    /// Displays the deserialized output to a grid.
    /// </summary>
    /// <param name="messageHeaders">Deserialized message header list</param>
    private void DisplayGrid(MessageHeadersListData messageHeaders)
    {
        try
        {
            DataTable headerTable = this.GetHeaderDataTable();

            if (null != messageHeaders && null != messageHeaders.Headers)
            {
                pnlHeader.Visible = true;
                lblHeaderCount.Text = messageHeaders.HeaderCount.ToString();
                lblIndexCursor.Text = messageHeaders.IndexCursor;

                DataRow row;
                foreach (HeadersData header in messageHeaders.Headers)
                {
                    row = headerTable.NewRow();

                    row["MessageId"] = header.MessageId;
                    row["From"] = header.From;
                    row["To"] = header.To != null ? string.Join(",", header.To.ToArray()) : string.Empty;
                    row["Received"] = header.Received;
                    row["Text"] = header.Text;
                    row["Favourite"] = header.Favorite;
                    row["Read"] = header.Read;
                    row["Direction"] = header.Direction;
                    row["Type"] = header.Type;
                    headerTable.Rows.Add(row);
                    if (null != header.Type && header.Type.ToLower() == "mms")
                    {
                        foreach (MMSContentData mmsCont in header.MmsContent)
                        {
                            DataRow mmsDetailsRow = headerTable.NewRow();
                            mmsDetailsRow["PartNumber"] = mmsCont.PartNumber;
                            mmsDetailsRow["ContentType"] = mmsCont.ContentType;
                            mmsDetailsRow["ContentName"] = mmsCont.ContentName;
                            headerTable.Rows.Add(mmsDetailsRow);
                        }
                    }
                }

                gvMessageHeaders.DataSource = headerTable;
                gvMessageHeaders.DataBind();
            }
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
    }

    /// <summary>
    /// Creates a datatable with message header columns
    /// </summary>
    /// <returns>data table with the structure of the grid</returns>
    private DataTable GetHeaderDataTable()
    {
        DataTable messageTable = new DataTable();
        DataColumn column = new DataColumn("MessageId");
        messageTable.Columns.Add(column);

        column = new DataColumn("PartNumber");
        messageTable.Columns.Add(column);

        column = new DataColumn("ContentType");
        messageTable.Columns.Add(column);

        column = new DataColumn("ContentName");
        messageTable.Columns.Add(column);

        column = new DataColumn("From");
        messageTable.Columns.Add(column);

        column = new DataColumn("To");
        messageTable.Columns.Add(column);

        column = new DataColumn("Received");
        messageTable.Columns.Add(column);

        column = new DataColumn("Text");
        messageTable.Columns.Add(column);

        column = new DataColumn("Favourite");
        messageTable.Columns.Add(column);

        column = new DataColumn("Read");
        messageTable.Columns.Add(column);

        column = new DataColumn("Type");
        messageTable.Columns.Add(column);

        column = new DataColumn("Direction");
        messageTable.Columns.Add(column);

        return messageTable;
    }

    /// <summary>
    /// Gets message content my message id and part number
    /// </summary>
    private void GetMessageContentByMessage()
    {
        if (null != Session["CSMIM_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSMIM_ACCESS_TOKEN"];
        }

        if (this.requestFactory.AuthorizeCredential == null)
        {
            Response.Redirect(this.requestFactory.GetOAuthRedirect().ToString());
        }

        try
        {
            GetMessageContentResponse contentResponse = this.requestFactory.GetMessage(txtMessageId.Text, txtPartNumber.Text);
            if (null != contentResponse)
            {
                string[] splitData = Regex.Split(contentResponse.MessageType.ToLower(), ";");
                if (contentResponse.MessageType.ToLower().Contains("application/smil"))
                {
                    smilpanel.Visible = true;
                    txtSmilContents.Text = System.Text.Encoding.Default.GetString(contentResponse.MessageContent);
                    this.DrawPanelForSuccess(ContentPanelStatus, string.Empty);
                }
                else if (contentResponse.MessageType.ToLower().Contains("text/plain"))
                {
                    this.DrawPanelForSuccess(ContentPanelStatus, System.Text.Encoding.Default.GetString(contentResponse.MessageContent));
                }
                else
                {
                    imagePanel.Visible = true;
                    this.DrawPanelForSuccess(ContentPanelStatus, string.Empty);
                    imagetoshow.Src = "data:" + splitData[0] + ";base64," + Convert.ToBase64String(contentResponse.MessageContent, Base64FormattingOptions.None);
                }
            }
        }
        catch (TokenExpiredException te)
        {
            this.DrawPanelForFailure(ContentPanelStatus, te.Message);
        }
        catch (UnauthorizedRequest ur)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ur.Message);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ie.Body);
        }
        catch (ArgumentNullException are)
        {
            this.DrawPanelForFailure(ContentPanelStatus, are.Message);
        }
        catch (ArgumentException ae)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ae.Message);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(ContentPanelStatus, ex.Message);
        }
    }

    /// <summary>
    /// Get session values, user supplied and assign to controls.
    /// </summary>
    /// <returns>true/false; true if values supplied, else false</returns>
    private bool GetSessionValues()
    {
        bool isValuesPresent = false;

        if (null != Session["CSSDKHeaderCount"])
        {
            txtHeaderCount.Text = Session["CSSDKHeaderCount"].ToString();
            isValuesPresent = true;
        }

        if (null != Session["CSSDKIndexCursor"])
        {
            txtIndexCursor.Text = Session["CSSDKIndexCursor"].ToString();
            isValuesPresent = true;
        }

        if (null != Session["CSSDKMessageID"])
        {
            txtMessageId.Text = Session["CSSDKMessageID"].ToString();
            isValuesPresent = true;
        }

        if (null != Session["CSSDKPartNumber"])
        {
            txtPartNumber.Text = Session["CSSDKPartNumber"].ToString();
            isValuesPresent = true;
        }

        return isValuesPresent;
    }

    /// <summary>
    /// Initializes Request Factory object
    /// </summary>
    /// <returns>true/false; true if able to read from config file; else false</returns>
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

        return true;
    }

    /// <summary>
    /// Initialize an instance of RequestFactory class.
    /// </summary>
    private void InitializeRequestFactory()
    {
        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.MIM);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, this.redirectUrl, null);

        if (null != Session["CSMIM_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSMIM_ACCESS_TOKEN"];
        }
    }

    /// <summary>
    /// Displays success message
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
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        rowTwoCellOne.Text = message;
        rowTwo.Controls.Add(rowTwoCellOne);
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

    /** }@ */
    /** }@ */
}