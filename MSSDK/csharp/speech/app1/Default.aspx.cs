// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region Application References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.Speechv2;

#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for converting speech to text
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke Speech APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.Speechv1 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the speech related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For speech services MS SDK library provides APIs SpeechToText()
 * This methods returns SpeechResponse object.
 
 * Sample code for converting text to speech:
 * ----------------------------
 List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 scopes.Add(RequestFactory.ScopeTypes.Speech);
 RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 SpeechResponse resp = requestFactory.SpeechToText(filePath, XSpeechContext.Generic, xArgNameValueCollection);
 */

/// <summary>
/// Speech_App1 class
/// </summary>
public partial class Speech_App1 : System.Web.UI.Page
{
    #region Class Variables and Constructor
    /// <summary>
    /// Request Factory object for calling api functions
    /// </summary>
    private RequestFactory requestFactory = null;

    /// <summary>
    /// Application Service specific variables
    /// </summary>
    private string apiKey, secretKey, endPoint;

    /// <summary>
    /// variable for having the posted file.
    /// </summary>
    private string fileToConvert;

    /// <summary>
    /// Flag for deletion of the temporary file
    /// </summary>
    private bool deleteFile;

    /// <summary>
    /// X-Arg parameter
    /// A meta parameter to define multiple parameters within a single HTTP header.
    /// </summary>
    private string xArgData;

    #endregion

    #region Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        if (!Page.IsPostBack)
        {
            resultsPanel.Visible = false;
            this.Initialize();
            string speechContxt = ConfigurationManager.AppSettings["SpeechContext"];
            if (!string.IsNullOrEmpty(speechContxt))
            {
                string[] speechContexts = speechContxt.Split(';');
                foreach (string speechContext in speechContexts)
                {
                    ddlSpeechContext.Items.Add(speechContext);
                }

                ddlSpeechContext.Items[0].Selected = true;
            }

            this.xArgData = ConfigurationManager.AppSettings["X-Arg"];
            txtXArgs.Text = this.xArgData.Replace(",", "," + Environment.NewLine);
        }

        DateTime currentServerTime = DateTime.UtcNow;
        lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
    }

    /// <summary>
    /// Method that calls SpeechToText method of RequestFactory when user clicked on submit button
    /// </summary>
    /// <param name="sender">sender that invoked this event</param>
    /// <param name="e">eventargs of the button</param>
    protected void SpeechToTextButton_Click(object sender, EventArgs e)
    {
        try
        {
            resultsPanel.Visible = false;
            this.Initialize();
            if (string.IsNullOrEmpty(fileUpload1.FileName))
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultFile"]))
                {
                    this.fileToConvert = Request.MapPath(ConfigurationManager.AppSettings["DefaultFile"]);
                }
                else
                {
                    this.DrawPanelForFailure(statusPanel, "No file selected, and default file is not defined in web.config");
                    return;
                }
            }
            else
            {
                string fileName = fileUpload1.FileName;
                if (fileName.CompareTo("default.wav") == 0)
                {
                    fileName = "1" + fileUpload1.FileName;
                }
                fileUpload1.PostedFile.SaveAs(Request.MapPath("") + "/" + fileName);
                this.fileToConvert = Request.MapPath("").ToString() + "/" + fileName;
                this.deleteFile = true;
            }

            SpeechResponse response = this.requestFactory.SpeechToText(this.fileToConvert, ddlSpeechContext.SelectedValue, this.xArgData);
            if (null != response)
            {
                this.DrawPanelForSuccess(statusPanel, "Response Parameters listed below");
                resultsPanel.Visible = true;
                this.DisplayResult(response);
            }

        }
        catch (InvalidScopeException invalidscope)
        {
            this.DrawPanelForFailure(statusPanel, invalidscope.Message);
        }
        catch (ArgumentException argex)
        {
            this.DrawPanelForFailure(statusPanel, argex.Message);
        }
        catch (InvalidResponseException ie)
        {
            this.DrawPanelForFailure(statusPanel, ie.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(statusPanel, ex.Message);
        }
        finally
        {
            if ((this.deleteFile == true) && (File.Exists(this.fileToConvert)))
            {
                File.Delete(this.fileToConvert);
                this.deleteFile = false;
            }
        }
    }

    #endregion

    #region SpeechToText service functions

    /// <summary>
    /// Initializes a new instance of the Speech_app1 class. This constructor reads from Config file and initializes Request Factory object
    /// </summary>
    private void Initialize()
    {

        if (this.requestFactory == null)
        {
            this.ReadConfigAndInitialize();
        }
    }

    /// <summary>
    /// Display success message
    /// </summary>
    /// <param name="panelParam">Panel to draw success message</param>
    /// <param name="message">Message to display</param>
    private void DrawPanelForSuccess(Panel panelParam, string message)
    {
        if (null != panelParam && panelParam.HasControls())
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
        TableCell rowTwoCellTwo = new TableCell();
        rowTwoCellTwo.Text = message.ToString();
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
        if (null != panelParam && panelParam.HasControls())
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
    /// Displays the result onto the page
    /// </summary>
    /// <param name="speechResponse">SpeechResponse received from api</param>
    private void DisplayResult(SpeechResponse speechResponse)
    {
        lblResponseId.Text = speechResponse.Recognition.ResponseId;
        foreach (NBest nbest in speechResponse.Recognition.NBest)
        {
            lblHypothesis.Text = nbest.Hypothesis;
            lblLanguageId.Text = nbest.LanguageId;
            lblResultText.Text = nbest.ResultText;
            lblGrade.Text = nbest.Grade;
            lblConfidence.Text = nbest.Confidence.ToString();
            string words = "[ ";
            if (null != nbest.Words)
            {
                foreach (string word in nbest.Words)
                {
                    words += "\"" + word + "\", ";
                }
                words = words.Substring(0, words.LastIndexOf(","));
                words = words + " ]";
            }

            lblWords.Text = nbest.Words != null ? words : string.Empty;

            if (null != nbest.WordScores)
            {
                lblWordScores.Text = "[ " + string.Join(", ", nbest.WordScores.ToArray()) + " ]";
            }
        }
    }

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
    /// Read from config and initialize RequestFactory object
    /// </summary>
    /// <returns>true/false; true - if able to read from config file; else false</returns>
    private bool ReadConfigAndInitialize()
    {
        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(statusPanel, "apiKey is not defined in the config file");
            return false;
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(statusPanel, "secretKey is not defined in the config file");
            return false;
        }

        this.endPoint = ConfigurationManager.AppSettings["endpoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(statusPanel, "endpoint is not defined in the config file");
            return false;
        }

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.Speech);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        return true;
    }

    #endregion
}