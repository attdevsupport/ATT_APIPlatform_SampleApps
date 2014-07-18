// <copyright file="Default.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

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
public partial class PaymentApp1_Listener : System.Web.UI.Page
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
            this.notificationDetailsFile = @"~\\notificationDetailsFile.txt";
        }
        Stream inputstream = Request.InputStream;
        int streamLength = Convert.ToInt32(inputstream.Length);
        byte[] stringArray = new byte[streamLength];
        inputstream.Read(stringArray, 0, streamLength);

        //string xmlString = System.Text.Encoding.UTF8.GetString(stringArray);
        string xmlString ="<?xml version=\"1.0\" encoding=\"UTF-8\"?><ownershipEvent type=\"grant\" timestamp=\"2014-07-17T23:01:30+00:00\"><networkOperatorId>cingularmi</networkOperatorId><ownerIdentifier>T_NWS_PNW_1351286778000105651482</ownerIdentifier><purchaseDate>2014-07-17T23:01:19+00:00</purchaseDate><productIdentifier>Onetime_Cat3</productIdentifier><purchaseActivityIdentifier>SYusvaaFefcszswDAqAzY2LEVSMZ698NXjI5</purchaseActivityIdentifier><instanceIdentifier>dcce5466-2548-4fb7-b907-f16ab49ffb81-CSHARPUAT</instanceIdentifier><minIdentifier>2067472099</minIdentifier><sequenceNumber>6798</sequenceNumber><reasonCode>0</reasonCode><reasonMessage>Processed Normally</reasonMessage><vendorPurchaseIdentifier>CThuJul172014230053</vendorPurchaseIdentifier></ownershipEvent>";
        ownershipEvent notificationObj;
        if (!String.IsNullOrEmpty(xmlString))
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ownershipEvent));
            TextReader textReader = new StringReader(xmlString);

            notificationObj = (ownershipEvent)deserializer.Deserialize(textReader);
            textReader.Close();

            string notificationDetails = string.Empty;
            foreach (var prop in notificationObj.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(notificationObj, null));
                notificationDetails += prop.Name + "%" + prop.GetValue(notificationObj, null) + "$";
            }
            this.WriteRecord(notificationDetails);
        }
    }

    /// <summary>
    /// Logs error message onto file
    /// </summary>
    /// <param name="text">Text to be logged</param>
    private void WriteRecord(string text)
    {
        File.AppendAllText(Request.MapPath(this.notificationDetailsFile), text + Environment.NewLine);
    }

    public class ownershipEvent
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlAttribute]
        public string timestamp { get; set; }

        public string Effective
        { get; set; }

        public string networkOperatorId
        { get; set; }

        public string ownerIdentifier
        { get; set; }

        public string purchaseDate
        { get; set; }

        public string productIdentifier
        { get; set; }

        public string purchaseActivityIdentifier
        { get; set; }

        public string instanceIdentifier
        { get; set; }

        public string minIdentifier
        { get; set; }

        public string oldMinIdentifier
        { get; set; }

        public string sequenceNumber
        { get; set; }

        public string reasonCode
        { get; set; }

        public string reasonMessage
        { get; set; }

        public string vendorPurchaseIdentifier
        { get; set; }

    }
}
