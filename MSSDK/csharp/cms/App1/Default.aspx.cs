// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.CallControlv1;

#endregion

/// <summary>
/// CallControl App1 class
/// </summary>
public partial class CallControl_App1 : System.Web.UI.Page
{
    #region Local variables

    /// <summary>
    /// RequestFactory instance
    /// </summary>
    private RequestFactory requestFactory;

    /// <summary>
    /// Access Token Variables
    /// </summary>
    private string endPoint, apiKey, secretKey;

    /// <summary>
    /// Phone numbers registered for Call Control Service.
    /// </summary>
    private string phoneNumbers;

    /// <summary>
    /// Script for Call Control Service.
    /// </summary>
    private string scriptName;

#endregion

    #region SSL Handshake Error
    
    /// <summary>
    /// Neglect the ssl handshake error with authentication server
    /// </summary>
    public static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    #endregion

    #region Application Events

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

            DateTime currentServerTime = DateTime.UtcNow;
            serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
            WriteNote(); 
            bool ableToRead = this.ReadConfigFile();
            if (!ableToRead)
            {
                return;
            }

            this.InitializeRequestFactory();
            lblPhoneNumbers.Text = this.phoneNumbers;            
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.ToString());
        }
    }

    /// <summary>
    /// Event that will be triggered when the user clicks on Send Signal button.
    /// This method will invoke SendSignal API.
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void btnSendSignal_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(lblSessionId.Text))
        {
            this.DrawPanelForFailure(pnlSendSignal, "Please create a session and then send signal");
            return;
        }

        this.SendSignal();
    }

    /// <summary>
    /// Event that will be triggered when the user clicks on Create Session button.
    /// This method will invoke CreateSession API.
    /// </summary>
    /// <param name="sender">object that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void btnCreateSession_Click(object sender, EventArgs e)
    {
        this.CreateSession();
    }

    #endregion

    #region API Invokation Methods

    /// <summary>
    /// This method creates a Session for an outgoing call or message.
    /// </summary>
    private void CreateSession()
    {
        try
        {
            NameValueCollection parameters = new NameValueCollection();
            if (!string.IsNullOrEmpty(txtNumberToDial.Text))
                parameters.Add("numberToDial", txtNumberToDial.Text);
            if (!string.IsNullOrEmpty(txtNumberForFeature.Text))
                parameters.Add("featurenumber", txtNumberForFeature.Text);
            if (!string.IsNullOrEmpty(txtMessageToPlay.Text))
                parameters.Add("messageToPlay", txtMessageToPlay.Text);
            if (lstTemplate.SelectedValue != "")
                parameters.Add("feature", lstTemplate.SelectedValue.ToString());

            CreateSessionResponse responseObject = this.requestFactory.CreateSession(parameters);
            if (null != responseObject)
            {
                lblSessionId.Text = responseObject.Id;
                NameValueCollection displayParam = new NameValueCollection();
                displayParam.Add("id", responseObject.Id);
                displayParam.Add("success", responseObject.Success.ToString());
                this.DrawPanelForSuccess(pnlCreateSession, displayParam, string.Empty);
            }
            else
            {
                this.DrawPanelForFailure(pnlCreateSession, "Unable to create session.");
            }
        }
        catch (InvalidScopeException ise)
        {
            this.DrawPanelForFailure(pnlCreateSession, ise.Message);
        }
        catch (InvalidResponseException ire)
        {
            this.DrawPanelForFailure(pnlCreateSession, ire.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.Message);
        }
    }

    /// <summary>
    /// This method sends a Signal to an active Session.
    /// </summary>
    private void SendSignal()
    {
        try
        {
            SendSignalResponse signalResponse = this.requestFactory.SendSignal(lblSessionId.Text, ddlSignal.SelectedValue);
            if (null != signalResponse)
            {
                NameValueCollection displayParam = new NameValueCollection();
                displayParam.Add("status", signalResponse.Status);
                this.DrawPanelForSuccess(pnlSendSignal, displayParam, string.Empty);
            }
            else
            {
                this.DrawPanelForFailure(pnlSendSignal, "Unable to send signal.");
            }
        }
        catch (ArgumentNullException ane)
        {
            this.DrawPanelForFailure(pnlSendSignal, ane.Message);
        }
        catch (InvalidScopeException ise)
        {
            this.DrawPanelForFailure(pnlSendSignal, ise.Message);
        }
        catch (InvalidResponseException ire)
        {
            this.DrawPanelForFailure(pnlSendSignal, ire.Body);
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlSendSignal, ex.Message);
        }
    }

    /// <summary>
    /// This method displays the contents of the note area.
    /// </summary>
    private void WriteNote()
    {
        string description = "<strong>Note:</strong> <br/>";
        Label1.Text = "Create Session will trigger an outbound call from application " + " to <strong>\"Make call to\"</strong> number.";
        switch (lstTemplate.SelectedValue)
        {
            case "ask":
                description +=
                    "For <strong>ask()</strong> script function, user is prompted to enter few digits and entered digits are played back. <br/>" +
                    "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "conference":
                description +=
                "For <strong>conference()</strong> script function, user is prompted to join the conference.<br/>" +
                "After quitting the conference, user is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "message":
                description +=
                    "For <strong>message()</strong> script function, user is played back <strong>\"Number parameter for Script Function\"</strong> number and an SMS Message is sent to that number.<br/>" +
                    "User is asked to press digit to activate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "reject":
                description +=
                "For <strong>reject()</strong> script function, if <strong>\"Number parameter for Script Function\"</strong> matches with calling id, call will be dropped.<br/>" +
                "If calling id doesnt match, calling id and <strong>\"Number parameter for Script Function\"</strong> number are played to User.<br/>" +
                "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "transfer":
                description +=
                "For <strong>transfer()</strong> script function, user is played back with <strong>\"Number parameter for Script Function\"</strong> and call be transferred to that number.<br/>" +
                "While doing transfer music on hold <strong>\"Message to Play\"</strong> is played. Once <strong>\"Number parameter for Script Function\"</strong> number disconnects the call, " +
                "user is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "wait":
                description +=
                "For <strong>wait()</strong> script function, if <strong>\"Number parameter for Script Function\"</strong> matches with calling id, call will be kept on hold for 3 seconds.<br/>" +
                "If calling id doesnt match, calling id and <strong>\"Number parameter for Script Function\"</strong> number are played to User.<br/>" +
                "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            case "":
                description +=
                "User is asked to press digit to activiate music on hold <strong>\"Message to Play\"</strong> to handle the signal (feature 2)";
                notesLiteral.Text = description;
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// This method displays the contents of call.js file onto create session textarea.
    /// </summary>
    private void GetOutboundScriptContent()
    {
        StreamReader streamReader = null;
        try
        {
            string scrFile = Request.MapPath(scriptName);
            streamReader = new StreamReader(scrFile);
            string javaScript = streamReader.ReadToEnd();
            txtCreateSession.Text = "Following is the Java Script Code: " + System.Environment.NewLine + javaScript;
        }
        catch (Exception ex)
        {
            this.DrawPanelForFailure(pnlCreateSession, ex.Message);
        }
        finally
        {
            if (null != streamReader)
            {
                streamReader.Close();
            }
        }
    }

    /// <summary>
    /// Read parameters from configuraton file and assigns to local variables.
    /// </summary>
    /// <returns>true/false; true if all required parameters are specified, else false</returns>
    private bool ReadConfigFile()
    {
        this.endPoint = ConfigurationManager.AppSettings["endPoint"];
        if (string.IsNullOrEmpty(this.endPoint))
        {
            this.DrawPanelForFailure(pnlCreateSession, "endPoint is not defined in configuration file");
            return false;
        }

        this.apiKey = ConfigurationManager.AppSettings["apiKey"];
        if (string.IsNullOrEmpty(this.apiKey))
        {
            this.DrawPanelForFailure(pnlCreateSession, "apiKey is not defined in configuration file");
            return false; 
        }

        this.secretKey = ConfigurationManager.AppSettings["secretKey"];
        if (string.IsNullOrEmpty(this.secretKey))
        {
            this.DrawPanelForFailure(pnlCreateSession, "secretKey is not defined in configuration file");
            return false;
        }

        this.phoneNumbers = ConfigurationManager.AppSettings["phoneNumbers"];
        if (string.IsNullOrEmpty(this.phoneNumbers))
        {
            this.DrawPanelForFailure(pnlCreateSession, "phoneNumbers parameter is not defined in configuration file");
            return false;
        }

        this.scriptName = ConfigurationManager.AppSettings["scriptName"];
        if (string.IsNullOrEmpty(this.scriptName))
        {
            this.DrawPanelForFailure(pnlCreateSession, "scriptName parameter is not defined in configuration file");
            return false;
        }
        this.GetOutboundScriptContent();
        return true;
    }

    /// <summary>
    /// Initialized RequestFactory with instance variable values.
    /// </summary>
    private void InitializeRequestFactory()
    {
        List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
        scopes.Add(RequestFactory.ScopeTypes.CallControl);
        this.requestFactory = new RequestFactory(this.endPoint, this.apiKey, this.secretKey, scopes, null, null);
    }

    #endregion

    #region Display Methods

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
    /// This function is called to draw the table in the panelParam panel for success response.
    /// </summary>
    /// <param name="panelParam">Panel Details</param>
    /// <param name="displayParams">Collection of message parameters to display.</param>
    /// <param name="message">Message as string</param>
    private void DrawPanelForSuccess(Panel panelParam, NameValueCollection displayParams, string message)
    {
        Table table = new Table();
        table.Font.Name = "Sans-serif";
        table.Font.Size = 9;
        table.BorderStyle = BorderStyle.Outset;
        table.CssClass = "successWide";
        table.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "SUCCESS:";
        rowOne.Controls.Add(rowOneCellOne);
        table.Controls.Add(rowOne);

        if (null != displayParams)
        {
            TableCell rowNextCellOne = null;
            TableCell rowNextCellTwo = null;
            foreach (string key in displayParams.Keys)
            {
                TableRow rowNext = new TableRow();
                rowNextCellOne = new TableCell();
                rowNextCellOne.Text = key;
                rowNextCellOne.Font.Bold = true;
                rowNextCellOne.Width = Unit.Pixel(70);
                rowNext.Controls.Add(rowNextCellOne);

                rowNextCellTwo = new TableCell();
                rowNextCellTwo.Text = displayParams[key];
                rowNext.Controls.Add(rowNextCellTwo);
                table.Controls.Add(rowNext);
            }
        }
        else
        {
            TableRow rowTwo = new TableRow();
            TableCell rowTwoCellOne = new TableCell();
            rowTwoCellOne.Text = message;
            rowTwo.Controls.Add(rowTwoCellOne);
            table.Controls.Add(rowTwo);
        }

        panelParam.Controls.Add(table);
    }

    #endregion
}