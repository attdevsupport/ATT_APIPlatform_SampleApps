using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.TextToSpeechv1;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Media;

/// <summary>
/// TextToSpeech_App1 Class
/// </summary>
public partial class TextToSpeech_App1 : System.Web.UI.Page
{
    /** \addtogroup TextToSpeech_App1
   * Description of the application can be referred at \ref TextToSpeech_App1 example
   * @{
   */

    /** \example TextToSpeech_App1 texttospeech\app1\Default.aspx.cs
     * \n \n This application allows a user to convert text to speech.
     * <ul>
     * <li>SDK methods showcased by the application:</li>
     * \n \n <b>Text To Speech:</b>
     * \n This method converts text to synthesized voice and returns the result in standard binary audio formats.
     * \n \n Steps followed in the app to invoke the method:
     * <ul><li>Import \c ATT_MSSDK and \c ATT_MSSDK.TextToSpeechv1 NameSpace.</li>
     * <li>Create an instance of \c RequestFactory class provided in MS SDK library. The \c RequestFactory manages the connections and calls to the AT&T API Platform.
     * Pass clientId, ClientSecret and scope as arguments while creating \c RequestFactory instance.</li>
     * <li>Invoke \c TextToSpeech() exposed in the \c RequestFactory class of MS SDK library.</li></ul>
     * \n Sample code:
     * <pre>
     *    List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
     *    scopes.Add(RequestFactory.ScopeTypes.TTS);
     *    RequestFactory requestFactory = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
     *    TextToSpeechResponse response = requestFactory.TextToSpeech(textToConvert, contentType, contentLanguage, "audio/wav", xArgs);</pre>
     * 
     * <li>For Registration, Installation, Configuration and Execution, refer \ref Application </li> \n
     * 
     * \n <li>Documentation can be referred at \ref TextToSpeech_App1 section</li></ul>
     * @{
     */
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
            string contentLangs = ConfigurationManager.AppSettings["ContentLanguage"];
            if (!string.IsNullOrEmpty(contentLangs))
            {
                string[] speechContexts = contentLangs.Split(';');
                foreach (string speechContext in speechContexts)
                {
                    ddlContentLanguage.Items.Add(speechContext);
                }
            }
            else
            {
                ddlContentLanguage.Items.Add("en-US");
            }

            ddlContentLanguage.Items[0].Selected = true;
            string contentTypes = ConfigurationManager.AppSettings["ContentType"];
            if (!string.IsNullOrEmpty(contentTypes))
            {
                string[] contentType = contentTypes.Split(';');
                foreach (string type in contentType)
                {
                    ddlContentType.Items.Add(type);
                }
            }
            else
            {
                ddlContentType.Items.Add("text/plain");
            }

            ddlContentType.Items[0].Selected = true;            
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

            TextToSpeechResponse response = this.requestFactory.TextToSpeech(txtTextToConvert.Text, ddlContentType.Text, ddlContentLanguage.Text, "audio/wav", xArgData);
            if (null != response)
            {
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
    }

    #endregion

    #region TextToSpeech service functions

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
    private void DisplayResult(TextToSpeechResponse speechResponse)
    {
        if (null != speechResponse)
        {
            lblContentLength.Text = speechResponse.ContentLength.ToString();
            lblContentType.Text = speechResponse.ContentType;
            if (null != speechResponse.SpeechContent)
            {
                try
                {
                    audioPlay.Attributes.Add("src", "data:audio/wav;base64," + Convert.ToBase64String(speechResponse.SpeechContent, Base64FormattingOptions.None));
                }
                catch (Exception ex)
                {
                    this.DrawPanelForFailure(resultsPanel, ex.Message);
                }
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

        this.xArgData = ConfigurationManager.AppSettings["X-Arg"];

        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.TTS);

        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
        return true;
    }

    #endregion    

    /** }@ */
    /** }@ */
}