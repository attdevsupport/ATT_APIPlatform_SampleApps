// <copyright file="Listener.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ATT_MSSDK;
using ATT_MSSDK.SMSv3;
#endregion

/// <summary>
/// Listener class for saving message counts.
/// </summary>
public partial class Listener : System.Web.UI.Page
{
    #region Listener Application Events

    /// <summary>
    /// This method called when the page is loaded into the browser. This method requests input stream and parses it to get message counts.
    /// </summary>
    /// <param name="sender">object, which invoked this method</param>
    /// <param name="e">EventArgs, which specifies arguments specific to this method</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        System.IO.Stream stream = Request.InputStream;

        if (null != stream)
        {
            ReceivedSMS message = RequestFactory.GetSMS(stream);
            if (null != message)
            {
                this.SaveMessageCount(message);
                this.SaveMessage(message);
            }
        }
    }

    #endregion

    #region Listner Application specific functions

    /// <summary>
    /// This method reads the incoming message and decides on to which message count needs to be updated.
    /// This method invokes another method to write the count to file
    /// </summary>
    /// <param name="message">ReceivedSMS, message received from Request</param>
    private void SaveMessageCount(ReceivedSMS message)
    {
        if (!string.IsNullOrEmpty(message.Message))
        {
            string messageText = message.Message.Trim().ToLower();

            string filePathConfigKey = string.Empty;
            switch (messageText)
            {
                case "basketball":
                    filePathConfigKey = "BasketBallFilePath";
                    break;
                case "football":
                    filePathConfigKey = "FootBallFilePath";
                    break;
                case "baseball":
                    filePathConfigKey = "BaseBallFilePath";
                    break;
            }

            if (!string.IsNullOrEmpty(filePathConfigKey))
            {
                this.WriteToFile(filePathConfigKey);
            }
        }
    }

    /// <summary>
    /// This method gets the file name, reads from the file, increments the count(if any) and writes back to the file.
    /// </summary>
    /// <param name="filePathConfigKey">string, parameter which specifies the config key to the file</param>
    private void WriteToFile(string filePathConfigKey)
    {
        string filePath = ConfigurationManager.AppSettings[filePathConfigKey];

        int count = 0;
        using (StreamReader streamReader = File.OpenText(Request.MapPath(filePath)))
        {
            count = Convert.ToInt32(streamReader.ReadToEnd());
            streamReader.Close();
        }

        count = count + 1;

        using (StreamWriter streamWriter = File.CreateText(Request.MapPath(filePath)))
        {
            streamWriter.Write(count);
            streamWriter.Close();
        }
    }

    /// <summary>
    /// This method reads the incoming message and stores the received message details.
    /// </summary>
    /// <param name="message">ReceivedSMS, message received from Request</param>
    private void SaveMessage(ReceivedSMS message)
    {
        string filePath = ConfigurationManager.AppSettings["MessagesFilePath"];

        string messageLineToStore = message.DateTime.ToString() + "_-_-" +
                                    message.MessageId.ToString() + "_-_-" +
                                    message.Message.ToString() + "_-_-" +
                                    message.SenderAddress.ToString() + "_-_-" +
                                    message.DestinationAddress.ToString();

        using (StreamWriter streamWriter = File.AppendText(Request.MapPath(filePath)))
        {
            streamWriter.WriteLine(messageLineToStore);
            streamWriter.Close();
        }
    }

    #endregion

}