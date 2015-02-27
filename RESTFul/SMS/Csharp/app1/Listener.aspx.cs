/*
* Copyright 2014 AT&T
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
#endregion

/// <summary>
/// Listener class for saving message counts.
/// </summary>
public partial class Listener : System.Web.UI.Page
{
#region variables
    public string receivedMessagesFilePath = string.Empty;
    public int numberOfMessagesToStore = 0;
#endregion
    #region Events
    /// <summary>
    /// This method called when the page is loaded into the browser. This method requests input stream and parses it to get message counts.
    /// </summary>
    /// <param name="sender">object, which invoked this method</param>
    /// <param name="e">EventArgs, which specifies arguments specific to this method</param>
    protected void Page_Load(object sender, EventArgs e)
    {

        System.IO.Stream stream = Request.InputStream;

        this.receivedMessagesFilePath = ConfigurationManager.AppSettings["ReceivedMessagesFilePath"];
        if (string.IsNullOrEmpty(this.receivedMessagesFilePath))
            this.receivedMessagesFilePath = "~\\Messages.txt";

        string count = ConfigurationManager.AppSettings["NumberOfMessagesToStore"];
        if (!string.IsNullOrEmpty(count))
            this.numberOfMessagesToStore = Convert.ToInt32(count);

        if (null != stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            string responseData = Encoding.ASCII.GetString(bytes);

            JavaScriptSerializer serializeObject = new JavaScriptSerializer();
            InboundSMSMessage message = (InboundSMSMessage)serializeObject.Deserialize(responseData, typeof(InboundSMSMessage));

            if (null != message)
            {
                this.SaveMessage(message);
            }
        }
    }
    #endregion

    #region Method to store the received message to file
    /// <summary>
    /// This method reads the incoming message and stores the received message details.
    /// </summary>
    /// <param name="message">InboundSMSMessage, message received from Request</param>
    private void SaveMessage(InboundSMSMessage message)
    {
        try
        {
            List<string> list = new List<string>();
            FileStream file = new FileStream(Request.MapPath(this.receivedMessagesFilePath), FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }

            sr.Close();
            file.Close();

            if (list.Count > this.numberOfMessagesToStore)
            {
                int diff = list.Count - this.numberOfMessagesToStore;
                list.RemoveRange(0, diff);
            }

            if (list.Count == this.numberOfMessagesToStore)
            {
                list.RemoveAt(0);
            }

            string messageLineToStore = message.DateTime.ToString() + "_-_-" +
                            message.MessageId.ToString() + "_-_-" +
                            message.Message.ToString() + "_-_-" +
                            message.SenderAddress.ToString() + "_-_-" +
                            message.DestinationAddress.ToString();
            list.Add(messageLineToStore);
            using (StreamWriter sw = File.CreateText(Request.MapPath(this.receivedMessagesFilePath)))
            {
                int tempCount = 0;
                while (tempCount < list.Count)
                {
                    string lineToWrite = list[tempCount];
                    sw.WriteLine(lineToWrite);
                    tempCount++;
                }
                sw.Close();
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
#endregion

}

#region Message Structure
/// <summary>
/// Message structure received
/// </summary>
public class InboundSMSMessage
{
    /// <summary>
    /// Gets or sets the value of DateTime
    /// </summary>
    public string DateTime
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of MessageId
    /// </summary>
    public string MessageId
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of Message
    /// </summary>
    public string Message
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of SenderAddress
    /// </summary>
    public string SenderAddress
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of DestinationAddress
    /// </summary>
    public string DestinationAddress
    {
        get;
        set;
    }
}
#endregion
