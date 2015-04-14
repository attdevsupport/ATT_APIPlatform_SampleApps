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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

#endregion

/// <summary>
/// Payment App2 Listener class
/// </summary>
public partial class WebhookApp1_Listener : System.Web.UI.Page
{
    private string notificationDetailsFile;
    /// <summary>
    /// Default method, that gets called upon loading the page.
    /// </summary>
    /// <param name="sender">object that invoked this method</param>
    /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {

        this.notificationDetailsFile = ConfigurationManager.AppSettings["notificationDetailsFile"];
        if (string.IsNullOrEmpty(this.notificationDetailsFile))
        {
            this.notificationDetailsFile = @"~\\APIPlatform\\UAT\\Csharp-RESTful\\immn\\app1\\notificationDetailsFile.txt";
        }
        Stream inputstream = Request.InputStream;
        int streamLength = Convert.ToInt32(inputstream.Length);
        byte[] stringArray = new byte[streamLength];
        inputstream.Read(stringArray, 0, streamLength);
        string jsonString = System.Text.Encoding.UTF8.GetString(stringArray);
        this.WriteRecord(jsonString);
        /*notificationMsg notificationObj;
        
        if (!String.IsNullOrEmpty(jsonString))
        {
            JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
            notificationObj = (notificationMsg)deserializeJsonObject.Deserialize(jsonString, typeof(notificationMsg));
            string notificationDetails = string.Empty;
            foreach (var prop in notificationObj.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(notificationObj, null));
                notificationDetails += prop.Name + "%" + prop.GetValue(notificationObj, null) + "$";
            }
           
        }*/

    }

    /// <summary>
    /// Logs error message onto file
    /// </summary>
    /// <param name="text">Text to be logged</param>
    private void WriteRecord(string text)
    {
        File.AppendAllText(Request.MapPath(this.notificationDetailsFile), text + Environment.NewLine);
    }


}
