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
    private string commonXArg;
    private string xArgTVContext;
    private string xArgSocialMediaContext;

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
            tvContextPanel.Visible = false;
            tvContextProgramPanel.Visible = false;
            tvContextShowtimePanel.Visible = false;

            this.Initialize();
            txtXArgs.Text = this.commonXArg.Replace(",", "," + Environment.NewLine);  
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

            XSpeechContext speechContext = XSpeechContext.Generic;
            string contentLanguage = string.Empty;
            this.xArgData = this.commonXArg;
            switch (ddlSpeechContext.SelectedValue)
            {
                case "Generic": speechContext = XSpeechContext.Generic; contentLanguage = ddlContentLang.SelectedValue; break;
                case "BusinessSearch": speechContext = XSpeechContext.BusinessSearch; break;
                case "TV": speechContext = XSpeechContext.TV; this.xArgData = this.xArgTVContext; break;
                case "Gaming": speechContext = XSpeechContext.Gaming; break;
                case "SocialMedia": speechContext = XSpeechContext.SocialMedia; this.xArgData = this.xArgSocialMediaContext; break;
                case "WebSearch": speechContext = XSpeechContext.WebSearch; break;
                case "SMS": speechContext = XSpeechContext.SMS; break;
                case "VoiceMail": speechContext = XSpeechContext.VoiceMail; break;
                case "QuestionAndAnswer": speechContext = XSpeechContext.QuestionAndAnswer; break;
            }

            string subContext = txtSubContext.Text;
            if (subContext.ToLower().Contains("example"))
            {
                subContext = string.Empty;
            }

           SpeechResponse response = this.requestFactory.SpeechToText(fileToConvert, speechContext, this.xArgData, contentLanguage, subContext, ddlAudioContentType.SelectedValue);

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

        if (ddlSpeechContext.SelectedValue != "TV")
        {
            tvContextPanel.Visible = false;
            tvContextProgramPanel.Visible = false;
            tvContextShowtimePanel.Visible = false;
        }

        if (ddlSpeechContext.SelectedValue == "TV")
        {
            tvContextPanel.Visible = true;
            if (null != speechResponse.Recognition.Info)
            {
                lblInfoActionType.Text = speechResponse.Recognition.Info.ActionType;
                this.lblRecognized.Text = speechResponse.Recognition.Info.Recognized;
            }

            if (null != speechResponse.Recognition.Info.Interpretation)
            {
                lblInterpretation_genre_id.Text = speechResponse.Recognition.Info.Interpretation.Genre_id;
                lblInterpretation_genre_words.Text = speechResponse.Recognition.Info.Interpretation.Genre_words;
            }

            if (null != speechResponse.Recognition.Info.Metrics)
            {
                lblMetrics_audioBytes.Text = speechResponse.Recognition.Info.Metrics.AudioBytes.ToString();
                this.lblMetrics_audioTime.Text = speechResponse.Recognition.Info.Metrics.AudioTime.ToString();
            }

            List<Program> programs = null;

            if (null != speechResponse.Recognition.Info.Search && null != speechResponse.Recognition.Info.Search.Meta)
            {
                this.lblDescription.Text = speechResponse.Recognition.Info.Search.Meta.Description;

                if (null != speechResponse.Recognition.Info.Search.Meta.GuideDateStart)
                    this.lblGuideDateStart.Text = speechResponse.Recognition.Info.Search.Meta.GuideDateStart.ToString();

                if (null != speechResponse.Recognition.Info.Search.Meta.GuideDateEnd)
                    this.lblGuideDateEnd.Text = speechResponse.Recognition.Info.Search.Meta.GuideDateEnd.ToString();

                this.lblLineup.Text = speechResponse.Recognition.Info.Search.Meta.Lineup;
                this.lblMarket.Text = speechResponse.Recognition.Info.Search.Meta.Market;
                this.lblResultCount.Text = speechResponse.Recognition.Info.Search.Meta.ResultCount.ToString();

                programs = speechResponse.Recognition.Info.Search.Programs;

                if (null != programs)
                {
                    this.DisplayProgramDetails(programs);
                }
            }

            List<Showtime> showtimes = null;

            if (null != speechResponse.Recognition.Info.Search)
            {
                showtimes = speechResponse.Recognition.Info.Search.Showtimes;

                if (null != showtimes)
                {
                    this.DisplayShowTimeDetails(showtimes);
                }
            }
        }
    }

    private void DisplayProgramDetails(List<Program> programs)
    {
        DataTable programDetailsTable = this.GetProgramDetailsTable();

        DataRow row;
        foreach (Program program in programs)
        {
            row = programDetailsTable.NewRow();

            row["cast"] = program.Cast;
            row["category"] = program.Category;
            row["description"] = program.Description;
            row["director"] = program.Director;
            row["language"] = program.Language;
            row["mpaaRating"] = program.MpaaRating;
            row["originalAirDate"] = program.OriginalAirDate;
            row["pid"] = program.Pid;
            row["runTime"] = program.RunTime;
            row["showType"] = program.ShowType;
            row["starRating"] = program.StarRating;
            row["title"] = program.Title;
            row["subtitle"] = program.Subtitle;
            row["year"] = program.Year;

            programDetailsTable.Rows.Add(row);
        }

        tvContextProgramPanel.Visible = true;
        gvPrograms.DataSource = programDetailsTable;
        gvPrograms.DataBind();
    }

    private void DisplayShowTimeDetails(List<Showtime> showtimes)
    {
        DataTable showDetailsTable = this.GetShowTimeDetailsTable();

        DataRow row;
        foreach (Showtime showtime in showtimes)
        {
            row = showDetailsTable.NewRow();

            row["affiliate"] = showtime.Affiliate;
            row["callSign"] = showtime.CallSign;
            row["channel"] = showtime.Channel;
            row["closeCaptioned"] = showtime.CloseCaptioned;
            row["dolby"] = showtime.Dolby;
            row["duration"] = showtime.Duration;
            row["endTime"] = showtime.EndTime;
            row["finale"] = showtime.Finale;
            row["hdtv"] = showtime.Hdtv;
            row["newShow"] = showtime.NewShow;
            row["pid"] = showtime.Pid.ToString();
            row["premier"] = showtime.Premier;
            row["repeat"] = showtime.Repeat;
            row["showTime"] = showtime.ShowTime;
            row["station"] = showtime.Station;
            row["stereo"] = showtime.Stereo;
            row["subtitled"] = showtime.Subtitled;
            row["weekday"] = showtime.Weekday;

            showDetailsTable.Rows.Add(row);
        }

        tvContextShowtimePanel.Visible = true;
        gvShowTimes.DataSource = showDetailsTable;
        gvShowTimes.DataBind();
    }

    private DataTable GetShowTimeDetailsTable()
    {
        DataTable showtimeDetailsTable = new DataTable();
        DataColumn column = new DataColumn("affiliate");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("callsign");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("channel");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("closecaptioned");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("dolby");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("showtime");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("endtime");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("duration");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("finale");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("hdtv");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("newshow");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("pid");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("premier");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("repeat");
        showtimeDetailsTable.Columns.Add(column);

        column = new DataColumn("station");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("stereo");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("subtitled");
        showtimeDetailsTable.Columns.Add(column);
        column = new DataColumn("weekday");
        showtimeDetailsTable.Columns.Add(column);
        return showtimeDetailsTable;
    }

   private DataTable GetProgramDetailsTable()
   {
       DataTable programDetailsTable = new DataTable();
       DataColumn column = new DataColumn("cast");
       programDetailsTable.Columns.Add(column);

       column = new DataColumn("category");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("description");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("director");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("language");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("mpaaRating");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("originalAirDate");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("pid");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("runTime");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("showType");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("starRating");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("title");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("subtitle");
       programDetailsTable.Columns.Add(column);
       column = new DataColumn("year");
       programDetailsTable.Columns.Add(column);
       return programDetailsTable;
   }

    private void addCelltoTable(Table table, string cellOneEntry, string cellTwoEntry)
    {        
        TableRow row = new TableRow();
        TableCell rowCellOne = new TableCell();        
        rowCellOne.Text = cellOneEntry;
        TableCell rowCellTwo = new TableCell();
        rowCellTwo.Text = cellTwoEntry;
        row.Controls.Add(rowCellOne);
        row.Controls.Add(rowCellTwo);
        table.Controls.Add(row);
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

        this.commonXArg = ConfigurationManager.AppSettings["X-Arg"];
        this.xArgTVContext = ConfigurationManager.AppSettings["X-ArgTVContext"];
        this.xArgSocialMediaContext = ConfigurationManager.AppSettings["X-ArgSocialMedia"];

        return true;
    }

    #endregion

    protected void ddlSpeechContext_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Initialize();
        ddlContentLang.Enabled = false;
        switch (ddlSpeechContext.Text)
        {
            case "TV":
                txtXArgs.Text = this.xArgTVContext;
                break;
            case "SocialMedia":
                txtXArgs.Text = this.xArgSocialMediaContext;
                break;
            case "Generic":
                ddlContentLang.Enabled = true;
                txtXArgs.Text = this.commonXArg;
                break;
            default:
                txtXArgs.Text = this.commonXArg;
                break;
        }

        this.xArgData = txtXArgs.Text;
        txtXArgs.Text = txtXArgs.Text.Replace(",", "," + Environment.NewLine);

        resultsPanel.Visible = false;
        tvContextPanel.Visible = false;
        tvContextProgramPanel.Visible = false;
        tvContextShowtimePanel.Visible = false;
        statusPanel.Visible = false;
    }    
}