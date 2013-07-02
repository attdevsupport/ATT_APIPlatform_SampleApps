// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
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
using ATT_MSSDK.Speechv3;
using System.Data;

#endregion

/* 
 * This Application demonstrates usage of  AT&T MS SDK wrapper library for converting speech to text
 * by passing inline grammar and inline hints as additional data set along with the audio.
 * 
 * Pre-requisite:
 * -------------
 * The developer has to register his application in AT&T Developer Platform website, for the scope 
 * of AT&T services to be used by application. AT&T Developer Platform website provides a ClientId
 * and client secret on registering the application.
 * 
 * Steps to be followed by the application to invoke Speech APIs exposed by MS SDK wrapper library:
 * --------------------------------------------------------------------------------------------
 * 1. Import ATT_MSSDK and ATT_MSSDK.Speechv3 NameSpace.
 * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
 * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
 * while creating RequestFactory instance.
 *
 * Note: Scopes that are not configured for your application will not work.
 * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
 * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
 * 
 * 3.Invoke the custom speech related APIs exposed in the RequestFactory class of MS SDK library.
 * 
 * For speech services MS SDK library provides APIs SpeechToText()
 * This methods returns SpeechResponse object.
 
 * Sample code for converting text to speech:
 * ----------------------------
 List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
 scopes.Add(RequestFactory.ScopeTypes.STTC);
 RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
 SpeechResponse resp = requestFactory.SpeechToText(fileToConvert,dictionaryFile,grammarFile, speechContext, this.xArgData);
 */

/// <summary>
/// SpeechCustom_App1 class
/// </summary>
public partial class SpeechCustom_App1 : System.Web.UI.Page
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
    /// X-Arg parameter
    /// A meta parameter to define multiple parameters within a single HTTP header.
    /// </summary>
    private string xArgData;

    /// <summary>
    /// variable for having the posted file.
    /// </summary>
    private string SpeechFilesDir;

    /// <summary>
    ///  Variable for path of the file containing inline hints 
    /// </summary>
    private string dictionaryFilePath;

    /// <summary>
    ///  Variable for path of the file containing inline grammar 
    /// </summary>
    private string grammarFilePath;

    #endregion

    #region SpeechToTextCustom Application Events

    /// <summary>
    /// This function is called when the application page is loaded into the browser.
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        resultsPanel.Visible = false;
        this.Initialize();
        this.SetContent();
    }

    /// <summary>
    /// Method that calls SpeechToText Custom method of RequestFactory when user clicked on submit button
    /// </summary>
    /// <param name="sender">sender that invoked this event</param>
    /// <param name="e">event args of the button</param>
    protected void SpeechToTextButton_Click(object sender, EventArgs e)
    {
        try
        {
            string fileToConvert = this.SpeechFilesDir + "/" + ddlAudioFile.SelectedValue;

            XSpeechCustomContext speechContext = XSpeechCustomContext.GenericHints;
            string contentLanguage = string.Empty;
            
            switch (ddlSpeechContext.SelectedValue)
            {
                case "GenericHints": 
                    speechContext = XSpeechCustomContext.GenericHints;
                    break;

                case "GrammarList": 
                    speechContext = XSpeechCustomContext.GrammarList;
                    break;
            }

           string dictionaryFile = Request.MapPath(this.dictionaryFilePath);
           string grammarFile = Request.MapPath(this.grammarFilePath);

           SpeechResponse response = this.requestFactory.SpeechToTextCustom(fileToConvert, dictionaryFile, grammarFile, speechContext);

            if (null != response)
            {
                resultsPanel.Visible = true;
                this.DrawPanelForSuccess(statusPanel, "Response Parameters listed below");                
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
    }

    #endregion

    #region SpeechToTextCustom service functions

    /// <summary>
    /// Initializes a new instance of the Speech_app1 class. This constructor reads from Config file and initializes Request Factory object
    /// </summary>
    private void Initialize()
    {
        DateTime currentServerTime = DateTime.UtcNow;
        lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
        this.ReadConfigAndInitialize();
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
        statusPanel.Visible = true;
        lblStatus.Text = speechResponse.Recognition.Status;
        lblResponseId.Text = speechResponse.Recognition.Responseid;
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

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SpeechFilesDir"]))
        {
            this.SpeechFilesDir = Request.MapPath(ConfigurationManager.AppSettings["SpeechFilesDir"]);
        }

        this.xArgData = ConfigurationManager.AppSettings["X-Arg"];
        this.grammarFilePath = ConfigurationManager.AppSettings["X-Grammar"];
        this.dictionaryFilePath = ConfigurationManager.AppSettings["X-Dictionary"];
        txtXArgs.Text = this.xArgData.Replace(",", "," + Environment.NewLine);

        if (!Page.IsPostBack)
        {
            string speechContxt = ConfigurationManager.AppSettings["SpeechContext"];
            if (!string.IsNullOrEmpty(speechContxt))
            {
                string[] speechContexts = speechContxt.Split(';');
                foreach (string speechContext in speechContexts)
                {
                    ddlSpeechContext.Items.Add(speechContext);
                }

                if (ddlSpeechContext.Items.Count > 0)
                {
                    ddlSpeechContext.Items[0].Selected = true;
                }
            }

            if (!string.IsNullOrEmpty(SpeechFilesDir))
            {
                string[] filePaths = Directory.GetFiles(this.SpeechFilesDir);
                foreach (string filePath in filePaths)
                {
                    ddlAudioFile.Items.Add(Path.GetFileName(filePath));
                }

                if (filePaths.Length > 0)
                {
                    ddlAudioFile.Items[0].Selected = true;
                }
            }
        }

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.STTC);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);

        return true;
    }

    /// <summary>
    /// Populate the controls on the page.
    /// </summary>
    private void SetContent()
    {
        string xdictionaryContent = string.Empty;
        string xgrammerContent = string.Empty;
        string dictionaryFile = Request.MapPath(this.dictionaryFilePath);
        string grammarFile = Request.MapPath(this.grammarFilePath);

        if (!string.IsNullOrEmpty(dictionaryFilePath) && File.Exists(dictionaryFile))
        {
            StreamReader streamReader = new StreamReader(dictionaryFile);
            xdictionaryContent = streamReader.ReadToEnd();
            streamReader.Close();
            txtMimeData.Text = "x-dictionary:" + Environment.NewLine + Environment.NewLine + xdictionaryContent + Environment.NewLine;
        }

        if (!string.IsNullOrEmpty(grammarFilePath) && File.Exists(grammarFile))
        {
            StreamReader streamReader = new StreamReader(grammarFile);
            xgrammerContent = streamReader.ReadToEnd();
            streamReader.Close();
            txtMimeData.Text += Environment.NewLine + "x-grammar:" + Environment.NewLine + Environment.NewLine + xgrammerContent;
        }
    }

    #endregion
}