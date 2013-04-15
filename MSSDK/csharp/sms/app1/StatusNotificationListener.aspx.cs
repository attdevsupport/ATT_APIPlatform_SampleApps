// <copyright file="StatusNotificationListener.aspx.cs" company="AT&amp;T">
// Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
// TERMS AND CONITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
// Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
// For more information contact developer.support@att.com
// </copyright>

#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ATT_MSSDK;
using ATT_MSSDK.SMSv3;
#endregion 

public partial class StatusNotificationListener : System.Web.UI.Page
{
    #region Variable Declaration
    
    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string deiveryStatusFilePath = string.Empty;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int numberOfDeliveryStatusToStore = 0;

    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        this.deiveryStatusFilePath = ConfigurationManager.AppSettings["deiveryStatusFilePath"];
        if (string.IsNullOrEmpty(this.deiveryStatusFilePath))
        {
            this.deiveryStatusFilePath = "DeliveryStatus.txt";
        }

        string numOfDeiveryStatusToStore = ConfigurationManager.AppSettings["numberOfDeliveryStatusToStore"];
        if (!string.IsNullOrEmpty(numOfDeiveryStatusToStore))
        {
            this.numberOfDeliveryStatusToStore = Convert.ToInt32(numOfDeiveryStatusToStore);
        }
        else
            this.numberOfDeliveryStatusToStore = 5;

        try
        {
            System.IO.Stream inputStream = Request.InputStream;

            SmsDeliveryStatus deliveryStatus = RequestFactory.GetSMSDeliveryStatus(inputStream);

            if (null != deliveryStatus)
            {
                this.SaveMessage(deliveryStatus);
            }
        }
        catch (ArgumentException ae)
        {
            
            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() + ": " + ae.Message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() + ": " + ex.ToString() + Environment.NewLine);
        }

    }

    private void SaveMessage(SmsDeliveryStatus status)
    {
        try
        {
            List<string> list = new List<string>();
            FileStream file = new FileStream(Request.MapPath(this.deiveryStatusFilePath), FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);

            }
            sr.Close();
            file.Close();

            if (list.Count > this.numberOfDeliveryStatusToStore)
            {
                int diff = list.Count - this.numberOfDeliveryStatusToStore;
                list.RemoveRange(0, diff);
            }

            if (list.Count == this.numberOfDeliveryStatusToStore)
            {
                if (list.Count > 1)
                list.RemoveAt(0);
            }

            string statusInfoToStore = status.DeliveryInfoNotification.MessageId + "_-_-" + status.DeliveryInfoNotification.DeliveryInfo.Address + "_-_-" + status.DeliveryInfoNotification.DeliveryInfo.DeliveryStatus;
            list.Add(statusInfoToStore);

            using (StreamWriter sw = File.CreateText(Request.MapPath(this.deiveryStatusFilePath)))
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
            File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() + ": " + ex.ToString() + Environment.NewLine);
        }
    }
}