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
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
#endregion

/// <summary>
/// MMS_App3 class
/// </summary>
public partial class MMS_App3 : System.Web.UI.Page
{  
    /** \addtogroup MMS_App3
     * Description of the application can be referred at \ref MMS_app3 example
     * @{
     */

    /** \example MMS_app3 mms\app3\Default.aspx.cs
     * \n \n This application allows a user to send an MMS message from his/her mobile device to a short code displayed in the application. 
     * This delivers the message to the application instantly, which creates a photo gallery. 
     * The interface allows the user to view the photo gallery of all MMS images and associated text sent to the application.
     * \n \n Installing and running the application, refer \ref Application 
     * \n \n <b>Parameters in web.config</b>
     * \n \n Specify
     * \li short_code - short code for the application
     * 
     * \n Documentation can be referred at \ref MMS_App3 section
     * @{
     */

    #region Instance Variables

    /// <summary>
    /// Instance Variables for local processing
    /// </summary>
    private string shortCode, directoryPath;

    /// <summary>
    /// Instance Variables for local processing
    /// </summary>
    private int numOfFilesToDisplay;

    #endregion

    #region MMS Application Events

    /// <summary>
    /// This function is called when the applicaiton page is loaded into the browser.
    /// </summary>
    /// <param name="sender">Button that caused this event</param>
    /// <param name="e">Event that invoked this function</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime currentServerTime = DateTime.UtcNow;
        lblServerTime.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";

        bool ableToRead = this.ReadConfigFile();

        if (ableToRead == true)
        {
            this.GetMmsFiles();
        }
    }

    #endregion

    #region MMS Application specific functions

    /// <summary>
    /// Gets the list of files from directory and displays them in the page
    /// </summary>
    private void GetMmsFiles()
    {
        int columnCount = 0;

        TableRow tableRow = null;
        TableRow secondRow = null;

        int totalFiles = 0;
        Table pictureTable = new Table();
        Table tableControl = new Table();

        DirectoryInfo directory = new DirectoryInfo(Request.MapPath(this.directoryPath));
        List<FileInfo> imageList = null;
        try
        {
            imageList = directory.GetFiles().OrderBy(f => f.CreationTime).ToList();
        }
        catch { }

        if (imageList == null)
        {
            lbl_TotalCount.Text = "0";
            return;
        }

        totalFiles = imageList.Count;

        string fileShownMessage = imageList.Count.ToString();
        lbl_TotalCount.Text = fileShownMessage;
        int fileCountIndex = 0;
        foreach (FileInfo file in imageList)
        {
            if (fileCountIndex == this.numOfFilesToDisplay)
            {
                break;
            }

            if (columnCount == 0)
            {
                tableRow = new TableRow();
                secondRow = new TableRow();
                TableCell tableCellImage = new TableCell();
                System.Web.UI.WebControls.Image image1 = new System.Web.UI.WebControls.Image();
                image1.ImageUrl = string.Format("{0}{1}", this.directoryPath, file.Name);
                image1.Width = 150;
                image1.Height = 150;
                tableCellImage.Controls.Add(image1);
                tableRow.Controls.Add(tableCellImage);

                TableCell tableCellSubject = new TableCell();
                tableCellSubject.Text = file.Name;
                tableCellSubject.Width = 150;
                secondRow.Controls.Add(tableCellSubject);
                columnCount += 1;
            }
            else
            {
                TableCell tableCellImage = new TableCell();
                System.Web.UI.WebControls.Image image1 = new System.Web.UI.WebControls.Image();
                image1.ImageUrl = string.Format("{0}{1}", this.directoryPath, file.Name);
                image1.Width = 150;
                image1.Height = 150;
                tableCellImage.Controls.Add(image1);
                tableRow.Controls.Add(tableCellImage);
                TableCell tableCellSubject = new TableCell();
                tableCellSubject.Text = file.Name;
                tableCellSubject.Width = 150;
                secondRow.Controls.Add(tableCellSubject);
                columnCount += 1;
                if (columnCount == 5)
                {
                    columnCount = 0;
                }

                fileCountIndex++;
            }

            pictureTable.Controls.Add(tableRow);
            pictureTable.Controls.Add(secondRow);
        }

        messagePanel.Controls.Add(pictureTable);
    }

    /// <summary>
    /// This method reads config file and assigns values to local variables
    /// </summary>
    /// <returns>true/false, true- if able to read from config file</returns>
    private bool ReadConfigFile()
    {
        this.shortCode = ConfigurationManager.AppSettings["short_code"];
        if (string.IsNullOrEmpty(this.shortCode))
        {
            this.DrawPanelForFailure("short_code is not defined in configuration file");
            return false;
        }

        shortCodeLabel.Text = this.shortCode;

        this.directoryPath = ConfigurationManager.AppSettings["ImageDirectory"];
        if (string.IsNullOrEmpty(this.directoryPath))
        {
            this.DrawPanelForFailure("ImageDirectory is not defined in configuration file");
            return false;
        }

        if (ConfigurationManager.AppSettings["NumOfFilesToDisplay"] == null)
        {
            this.numOfFilesToDisplay = 5;
        }
        else
        {
            this.numOfFilesToDisplay = Convert.ToInt32(ConfigurationManager.AppSettings["NumOfFilesToDisplay"]);
        }

        return true;
    }

    /// <summary>
    /// Displays error message
    /// </summary>
    /// <param name="message">string, message to be displayed</param>
    private void DrawPanelForFailure(string message)
    {
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

        messagePanel.Controls.Add(table);
    }

    #endregion

    /** }@ */
    /** }@ */
}