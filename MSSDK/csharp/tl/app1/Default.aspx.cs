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
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.TLv1;

#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for getting  device location 
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke TL APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.TLv1 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.Also pass redirectUri(Uri ponting to the application).
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3. Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform)
 *    Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization.
 * 4. Retrive the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory 
 *    by passing 'code' as the argument.
 *    
 * 5. Invoke GetTerminalLocation() on requestFactory to get the device location by passing the device Id
 *    
 * For device location service MS SDK library provides API GetTerminalLocation()
 * This methods returns response objects DeviceLocation.
 
 * Sample code for getting device location:
 * ----------------------------------------
 DeviceLocation deviceLocationRequest = requestFactory.GetTerminalLocation(RequestedAccuracy,TerminalLocationTolerance,AcceptableAccuracy);
*/

/// <summary>
/// TL_App1 MSSDK Sample App for terminal Location
/// </summary>
public partial class TL_App1 : System.Web.UI.Page
{
    /** \addtogroup TL_App1
     * Description of the application can be referred at \ref TL_app1 example
     * @{
     */

    /** \example TL_app1 tl\app1\Default.aspx.cs
     * \n \n This application allows the AT&T subscriber access to message related data stored in the AT&T Messages environment. 
     * <ul> 
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>GetTerminalLocation:</b>
     * \n This method is used to to query  the location of an AT&amp;T MSISDN for latitude, longitude and accuracy coordinates.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.TLv1 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke GetOAuthRedirect() on requestFactory to get redirectUrl(Url for authorization endpoint of AT&T platform). 
     * Redirect the user to authorization endpoint of AT&T platform for getting user authentication and authorization. 
     * \n AT&T displays the login and consent page. After getting the consent from the user, AT&T platform returns 'code' as a query parameter.</li>
     * <li>Retrieve the 'code' query parameter from AT&T platform response and  Invoke GetAuthorizeCredentials() on requestFactory by passing 'code' as the argument.</li>
     * <li>Invoke \c GetTeminalLocation() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
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
     * <li>Get Device Location:</li>      
     * <pre>
     *      DeviceLocation deviceLocationRequest = requestFactory.GetTerminalLocation(requestedAccuracy, tolerance, acceptableAccuracy);</pre></ul>
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
    /// Gets or sets instance of requestFactory
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Local variables for storing app configuration details
    /// </summary>
    private string apiKey, secretKey, endPoint;
    
    /// <summary>
    /// OAuth redirect Url
    /// </summary>
    private string authorizeRedirectUri;

    /// <summary>
    /// Local variable to store request start time
    /// </summary>
    private DateTime startTime;

    /// <summary>
    /// Gets or sets the Status Table
    /// </summary>
    private Table getStatusTable;

    #endregion

    #region TL Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// This function reads the web.config and gets the values of the attributes
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BypassCertificateError();
            map_canvas.Visible = false;

            DateTime currentServerTime = DateTime.UtcNow;
            serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

            bool ableToRead = this.ReadConfigFile();
            if (!ableToRead)
            {
                return;
            }
            
            string authCode;

            if (null != Session["tl_session_acceptableAccuracy"])
            {
                Radio_AcceptedAccuracy.SelectedIndex = Convert.ToInt32(Session["tl_session_acceptableAccuracy"].ToString());
                Radio_RequestedAccuracy.SelectedIndex = Convert.ToInt32(Session["tl_session_requestedAccuracy"].ToString());
                Radio_DelayTolerance.SelectedIndex = Convert.ToInt32(Session["tl_session_tolerance"].ToString());
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Request["code"])))
                {
                    authCode = Request["code"];
                    this.requestFactory.GetAuthorizeCredentials(authCode);
                    Session["CSTL_ACCESS_TOKEN"] = this.requestFactory.AuthorizeCredential;

                    if (Convert.ToString(Session["buttonStatus"]) == "true")
                    {
                        this.GetDeviceLocation();
                        Session["buttonStatus"] = string.Empty;
                    }
                }
            }            
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.Body );
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
    }

    /// <summary>
    /// Event that will be triggered when the user clicks on GetPhoneLocation button
    /// This method calls GetDeviceLocation Api
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void GetDeviceLocation_Click(object sender, EventArgs e)
    {
        try
        {
            Session["buttonStatus"] = "true";
            this.startTime = DateTime.Now;
            Session["tl_session_acceptableAccuracy"] = Radio_AcceptedAccuracy.SelectedIndex;
            Session["tl_session_requestedAccuracy"] = Radio_RequestedAccuracy.SelectedIndex;
            Session["tl_session_tolerance"] = Radio_DelayTolerance.SelectedIndex;

            if (Session["CSTL_ACCESS_TOKEN"] == null)
            {
                string redirectUrl = this.requestFactory.GetOAuthRedirect().ToString();
                Response.Redirect(redirectUrl);
            }
            else
            {
                this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSTL_ACCESS_TOKEN"];
                this.GetDeviceLocation();
                Session["buttonStatus"] = string.Empty;
            }
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.Body );
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
    }

#endregion

    #region TL Application specific functions

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
    /// Reads from config file and assigns to local variables
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(tlPanel, "endPoint is not defined in configuration file");
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["api_key"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(tlPanel, "api_key is not defined in configuration file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secret_key"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(tlPanel, "secret_key is not defined in configuration file");
            return false;
        }

        this.authorizeRedirectUri = ConfigurationManager.AppSettings["authorize_redirect_uri"];
        if (string.IsNullOrEmpty(this.authorizeRedirectUri))
        {
            this.DrawPanelForFailure(tlPanel, "authorize_redirect_uri is not defined in configuration file");
            return false;
        }

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.TerminalLocation);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, this.authorizeRedirectUri, null);
        if (null != Session["CSTL_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSTL_ACCESS_TOKEN"];
        }

        return true;
    }

    /// <summary>
    /// Invokes GetTerminal Location method of SDK and displays the result
    /// </summary>
    private void GetDeviceLocation()
    {
        try
        {
            int[] definedReqAccuracy = new int[3] { 100, 1000, 10000 };
            int requestedAccuracy, acceptableAccuracy;
            
            acceptableAccuracy = definedReqAccuracy[Radio_AcceptedAccuracy.SelectedIndex];
            requestedAccuracy = definedReqAccuracy[Radio_RequestedAccuracy.SelectedIndex];
            
            TerminalLocationTolerance tolerance = TerminalLocationTolerance.DelayTolerant;
            switch (Radio_DelayTolerance.SelectedIndex)
            {
                case 0: 
                    tolerance = TerminalLocationTolerance.NoDelay;
                    break;
                case 1:
                    tolerance = TerminalLocationTolerance.LowDelay;
                    break;
                default:
                    tolerance = TerminalLocationTolerance.DelayTolerant;
                    break;
            }

            DeviceLocation deviceLocationRequest = this.requestFactory.GetTerminalLocation(requestedAccuracy, tolerance, acceptableAccuracy);

            DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - this.startTime;

            this.DrawPanelForGetLocationResult(string.Empty, string.Empty, true);
            this.DrawPanelForGetLocationResult("Accuracy:", deviceLocationRequest.Accuracy.ToString(), false);
            this.DrawPanelForGetLocationResult("Latitude:", deviceLocationRequest.Latitude.ToString(), false);
            this.DrawPanelForGetLocationResult("Longitude:", deviceLocationRequest.Longitude.ToString(), false);
            this.DrawPanelForGetLocationResult("TimeStamp:", deviceLocationRequest.TimeStamp.ToString(), false);
            this.DrawPanelForGetLocationResult("Response Time:", timeSpan.Seconds.ToString() + "seconds", false);

            MapTerminalLocation.Visible = true;
            map_canvas.Visible = true;
            StringBuilder googleString = new StringBuilder();
            googleString.Append("http://maps.google.com/?q=" + deviceLocationRequest.Latitude.ToString() + "+" + deviceLocationRequest.Longitude.ToString() + "&output=embed");
            MapTerminalLocation.Attributes["src"] = googleString.ToString();
        }
        catch (ArgumentException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
        catch (InvalidResponseException ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.Body );
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(tlPanel, ex.ToString());
        }
    }

    /// <summary>
    /// Displays error message
    /// </summary>
    /// <param name="panelParam">Panel to draw error message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForFailure(Panel panelParam, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.CssClass = "errorWide";
        table.Width = Unit.Pixel(650);
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
        table.BorderWidth = 2;
        table.BorderColor = Color.Red;
        table.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc");
        panelParam.Controls.Add(table);
    }

    /// <summary>
    /// This method is used to draw table for successful response of get device locations
    /// </summary>
    /// <param name="attribute">string, attribute to be displayed</param>
    /// <param name="value">string, value to be displayed</param>
    /// <param name="headerFlag">boolean, flag indicating to draw header panel</param>
    private void DrawPanelForGetLocationResult(string attribute, string value, bool headerFlag)
    {
        if (headerFlag == true)
        {
            this.getStatusTable = new Table();
            this.getStatusTable.CssClass = "successWide";
            TableRow rowOne = new TableRow();
            TableCell rowOneCellOne = new TableCell();
            rowOneCellOne.Font.Bold = true;
            rowOneCellOne.Text = "SUCCESS:";
            rowOne.Controls.Add(rowOneCellOne);
            this.getStatusTable.Controls.Add(rowOne);
            tlPanel.Controls.Add(this.getStatusTable);
        }
        else
        {
            TableRow row = new TableRow();
            TableCell cell1 = new TableCell();
            TableCell cell2 = new TableCell();
            cell1.Text = attribute.ToString();
            cell1.Font.Bold = true;
            cell1.Width = Unit.Pixel(100);
            row.Controls.Add(cell1);
            cell2.Text = value.ToString();
            row.Controls.Add(cell2);
            this.getStatusTable.Controls.Add(row);
        }
    }
    /** }@ */
    /** }@ */
    #endregion
}