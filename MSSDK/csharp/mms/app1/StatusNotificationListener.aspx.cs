#region References
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATT_MSSDK;
using ATT_MSSDK.MMSv3;
#endregion

public partial class mms_app1_StatusNotificationListener : System.Web.UI.Page
{
    #region Variable Declaration

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private string deiveryStatusFilePath = string.Empty;

    /// <summary>
    /// Global Variable Declaration
    /// </summary>
    private int numOfDeiveryStatusToStore = 0;

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
            this.numOfDeiveryStatusToStore = Convert.ToInt32(numOfDeiveryStatusToStore);
        }
        else
        {
            this.numOfDeiveryStatusToStore = 5;
        }


        try
        {
            System.IO.Stream inputStream = Request.InputStream;

            MmsDeliveryStatus deliveryStatus = RequestFactory.GetMMSDeliveryStatus(inputStream);

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

    private void SaveMessage(MmsDeliveryStatus status)
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

            if (list.Count > this.numOfDeiveryStatusToStore)
            {
                int diff = list.Count - this.numOfDeiveryStatusToStore;
                list.RemoveRange(0, diff);
            }

            if (list.Count == this.numOfDeiveryStatusToStore)
            {
                if (list.Count > 1)
                list.RemoveAt(0);
            }

            string statusInfoToStore = status.deliveryInfoNotification.messageId + "_-_-" + status.deliveryInfoNotification.deliveryInfo.Address + "_-_-" + status.deliveryInfoNotification.deliveryInfo.DeliveryStatus;
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