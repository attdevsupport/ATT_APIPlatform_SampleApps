// <copyright file="Default.aspx.cs" company="AT&amp;T Intellectual Property">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region Application References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI;
using ATT_MSSDK;
using ATT_MSSDK.DeviceCapabilitiesv2;
using System.Web;
#endregion

/// <summary>
/// Device Capabilities sample application
/// </summary>
public partial class DC_App1 : System.Web.UI.Page
{
    #region Instance Variables
    /// <summary>
    /// Instance variables
    /// </summary>
    private string apiKey, secretKey, endPoint, redirectUrl;

    /// <summary>
    /// RequestFactory instance variable
    /// </summary>
    private RequestFactory requestFactory;

    #endregion

    #region Application Events

    /// <summary>
    /// This event gets triggered when the page is loaded into the client browser.
    /// This will perform the following activities:
    /// 1. Reads config file and assigns configuration values to instance variables.
    /// 2. Initializes an instance of RequestFactory class.
    /// 3. Checks for Auth code and exchanges the auth code for getting access token.
    /// 4. Call GetDeviceCapabilities().
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        DateTime currentServerTime = DateTime.UtcNow;
        lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
        bool ableToReadConfigFile = this.ReadConfigFile();
        if (!ableToReadConfigFile)
        {
            tbDeviceCapabError.Visible = true;
            return;
        }

        this.InitializeRequestFactory();

        try
        {
            if (!Page.IsPostBack)
            {
                if (Session["CSDC_ACCESS_TOKEN"] == null)
                {
                    if (!string.IsNullOrEmpty(Request["code"]))
                    {
                        this.requestFactory.GetAuthorizeCredentials(Request["code"]);
                        Session["CSDC_ACCESS_TOKEN"] = this.requestFactory.AuthorizeCredential;
                    }
                }
                if (Request.QueryString["error"] != null && Session["mssdk_cs_dc_state"] != null)
                {
                    Session["mssdk_cs_dc_state"] = null;
                    string errorString = Request.Url.Query.Remove(0, 1);
                    lblErrorMessage.Text = HttpUtility.UrlDecode(errorString);
                    tbDeviceCapabError.Visible = true;
                    return;
                }
            }

            this.GetDeviceCapabilities();
        }
        catch (InvalidResponseException ie)
        {
            lblErrorMessage.Text = ie.Body;
            tbDeviceCapabError.Visible = true;
        }
        catch (TokenExpiredException te)
        {
            lblErrorMessage.Text = te.Message;
            tbDeviceCapabError.Visible = true;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message;
            tbDeviceCapabError.Visible = true;
        }

    }

    #endregion

    #region Application Specific Methods

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
    /// This method reads the configuration parameters and assigns to instance variables.
    /// </summary>
    /// <returns>true/false; true if able to read config parameters; else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            lblErrorMessage.Text = "endPoint is not defined in configuration file";
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            lblErrorMessage.Text = "apiKey is not defined in configuration file";
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            lblErrorMessage.Text = "secretKey is not defined in configuration file";
            return false;
        }

        this.redirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        if (string.IsNullOrEmpty(this.redirectUrl))
        {
            lblErrorMessage.Text = "redirectUrl is not defined in configuration file";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Initialize a new instance of RequestFactory.
    /// </summary>
    private void InitializeRequestFactory()
    {
        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.DeviceCapability);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, this.redirectUrl, null);

        if (null != Session["CSDC_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSDC_ACCESS_TOKEN"];
        }
    }

    /// <summary>
    /// This method checks if the access token is already present, and calls GetDeviceCapabilities() of RequestFactory.
    /// Else, it redirects the user to get the OAuth consent.
    /// </summary>
    private void GetDeviceCapabilities()
    {
        if (null != Session["CSDC_ACCESS_TOKEN"])
        {
            this.requestFactory.AuthorizeCredential = (OAuthToken)Session["CSDC_ACCESS_TOKEN"];
        }

        if (this.requestFactory.AuthorizeCredential == null)
        {
            Session["mssdk_cs_dc_state"] = "FetchAuthCode";
            Response.Redirect(this.requestFactory.GetOAuthRedirect().ToString());
        }
        Session["mssdk_cs_dc_state"] = null;
        DeviceCapabilities deviceCapabilities = this.requestFactory.GetDeviceCapabilities();
        this.DisplayDeviceCapabilities(deviceCapabilities);
    }

    /// <summary>
    /// This method displays the Device Capabilities.
    /// </summary>
    /// <param name="deviceCapabilities">Device Capabilities</param>
    private void DisplayDeviceCapabilities(DeviceCapabilities deviceCapabilities)
    {
        if (null != deviceCapabilities)
        {
            lblTypeAllocationCode.Text = deviceCapabilities.deviceId.TypeAllocationCode;
            lblName.Text = deviceCapabilities.capabilities.Name;
            lblVendor.Text = deviceCapabilities.capabilities.Vendor;
            lblModel.Text = deviceCapabilities.capabilities.Model;
            lblFirmwareVersion.Text = deviceCapabilities.capabilities.FirmwareVersion;
            lblUAProf.Text = deviceCapabilities.capabilities.UaProf;
            lblMMSCapable.Text = deviceCapabilities.capabilities.MmsCapable;
            lblAssistedGps.Text = deviceCapabilities.capabilities.AssistedGps;
            lblLocationTechnology.Text = deviceCapabilities.capabilities.LocationTechnology;
            lblDeviceBrowser.Text = deviceCapabilities.capabilities.DeviceBrowser;
            lblWAPPush.Text = deviceCapabilities.capabilities.WapPushCapable;
            tb_dc_output.Visible = true;
            tbDeviceCapabilities.Visible = true;
        }
    }

    #endregion
}