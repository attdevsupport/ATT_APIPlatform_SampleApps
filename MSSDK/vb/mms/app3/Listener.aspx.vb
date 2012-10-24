' <copyright file="Listener.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2012
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2012 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"
Imports System.Collections.Generic
Imports System.IO
Imports ATT_MSSDK
Imports ATT_MSSDK.MMSv2
Imports System.Configuration
#End Region

' 
' * This is listener which runs at the MOListener URL configured at AT&T Developer Platform website
' * while registerying the application for MMS scope
' * AT&T platform invokes the MOListener URL whenever AT&T platform receives a MMS for the shortcode configured
' * for the application.
' * 
' * Pre-requisite:
' * -------------
' * The developer has to register his application in AT&T Developer Platform website, for the scope 
' * mms scope. AT&T Developer Platform website provides a shortcode for the application.
' * 
' * Steps to be followed by the application to invoke MMS APIs exposed by MS SDK wrapper library:
' * --------------------------------------------------------------------------------------------
' * 1. Import ATT_MSSDK and ATT_MSSDK.MMSv2 NameSpace.
' * 2. Create an instance of RequestFactory class provided in MS SDK library. The RequestFactory manages 
' * the connections and calls to the AT&T API Platform.Pass clientId, ClientSecret and scope as arguments
' * while creating RequestFactory instance.
' *
' * Note: Scopes that are not configured for your application will not work.
' * For example, your application may be configured in the AT&T API Platform to support the Payment and SMS scopes.
' * The RequestFactory may specify any combination of Payment or SMS.  You may specify other scopes, but they will not work.
' * 
' * 3.Listener application invokes GetMms() API exposed in the RequestFactory class of MS SDK library, by passing 
' * Request.InputStream and directory path where listner has to store the mms attachments.
' * GetMms() returns instance of inboundMmsMessage which contains  details of MMS attachments received.
' * 
' 
' * Sample code for sending mms:
' * ----------------------------
' 
' string directoryPath = "D:\\Webs\\wincod\\2_1Bin\\UAT\\csharp-mssdk\\mms\\app3\\MoImages\\";
' List<RequestFactory.ScopeTypes> scopes = new List<RequestFactory.ScopeTypes>();
' scopes.Add(RequestFactory.ScopeTypes.MMS);
' RequestFactory target = new RequestFactory(endPoint, apiKey, secretKey, scopes, null, null);
' System.IO.Stream inputStream = Request.InputStream;
' InboundMmsMessage inboundMmsMessage = requestFactory.GetMms(inputStream, directoryPath);
' 
'


''' <summary>
''' MMSApp3_Listener class
''' </summary>
Public Partial Class MMSApp3_Listener
	Inherits System.Web.UI.Page
	''' <summary>
	''' Event, that triggers when the applicaiton page is loaded into the browser
	''' Listens to server and stores the mms messages in server
	''' </summary>
	''' <param name="sender">Button that caused this event</param>
	''' <param name="e">Event that invoked this function</param>
	Protected Sub Page_Load(sender As Object, e As EventArgs)
		Dim directoryPath As String = Request.MapPath(ConfigurationManager.AppSettings("ImageDirectory"))
		Try
			Dim inputStream As System.IO.Stream = Request.InputStream

			' Invoke GetMms() to parse the inputstream and get the inboundMmsMessage object
			Dim inboundMmsMessage As InboundMmsMessage = RequestFactory.GetMms(inputStream, directoryPath)

			If inboundMmsMessage IsNot Nothing Then
				' Interate through InboundMmsMessage and store attachments according to application logic 
				Dim index As Integer = 0

				While index < inboundMmsMessage.InboundMmsAttachmentDetailsList.Count
					Dim receivedTime As String = inboundMmsMessage.ReceivedTimeDate.ToString("HH-MM-SS")
					Dim receivedDate As String = inboundMmsMessage.ReceivedTimeDate.ToString("MM-dd-yyyy")
					'string srcFilePath = Request.MapPath("@" + Convert.ToString(inboundMmsMessage.InboundMmsAttachmentDetailsList[index].MmsImagePath));
					'string moveTo = directoryPath + "From_" + inboundMmsMessage.SenderAddress.Replace("+","") + "_At_" + receivedTime + "_UTC_On_" + receivedDate + (new Random()).Next();
					'System.IO.File.Move(srcFilePath, moveTo);
					index += 1
				End While
			End If

				'File.AppendAllText(Request.MapPath(directoryPath) + "Error.txt", DateTime.Now.ToString() + ": " + ae.Message + Environment.NewLine);
		Catch ae As ArgumentException
				'File.AppendAllText(Request.MapPath(directoryPath) + "Error.txt", DateTime.Now.ToString() + ": " + ex.ToString() + Environment.NewLine);
		Catch ex As Exception
		End Try
	End Sub
End Class
