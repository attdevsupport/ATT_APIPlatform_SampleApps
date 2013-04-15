#Region "References"
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ATT_MSSDK
Imports ATT_MSSDK.MMSv3
#End Region

Public Partial Class mms_app1_StatusNotificationListener
	Inherits System.Web.UI.Page
	#Region "Variable Declaration"

	''' <summary>
	''' Global Variable Declaration
	''' </summary>
	Private deiveryStatusFilePath As String = String.Empty

	''' <summary>
	''' Global Variable Declaration
	''' </summary>
	Private numOfDeiveryStatusToStore As Integer = 0

	#End Region
	Protected Sub Page_Load(sender As Object, e As EventArgs)
		Me.deiveryStatusFilePath = ConfigurationManager.AppSettings("deiveryStatusFilePath")
		If String.IsNullOrEmpty(Me.deiveryStatusFilePath) Then
			Me.deiveryStatusFilePath = "DeliveryStatus.txt"
		End If

		Dim numOfDeiveryStatusToStore As String = ConfigurationManager.AppSettings("numberOfDeliveryStatusToStore")
		If Not String.IsNullOrEmpty(numOfDeiveryStatusToStore) Then
			Me.numOfDeiveryStatusToStore = Convert.ToInt32(numOfDeiveryStatusToStore)
		Else
			Me.numOfDeiveryStatusToStore = 5
		End If


		Try
			Dim inputStream As System.IO.Stream = Request.InputStream

			Dim deliveryStatus As MmsDeliveryStatus = RequestFactory.GetMMSDeliveryStatus(inputStream)

			If deliveryStatus IsNot Nothing Then
				Me.SaveMessage(deliveryStatus)
			End If
		Catch ae As ArgumentException

			File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ae.Message & Environment.NewLine)
		Catch ex As Exception
			File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ex.ToString() & Environment.NewLine)
		End Try

	End Sub

	Private Sub SaveMessage(status As MmsDeliveryStatus)
		Try
			Dim list As New List(Of String)()
			Dim file__1 As New FileStream(Request.MapPath(Me.deiveryStatusFilePath), FileMode.OpenOrCreate, FileAccess.Read)
			Dim sr As New StreamReader(file__1)

			Dim line As String
			While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing

				list.Add(line)
			End While
			sr.Close()
			file__1.Close()

			If list.Count > Me.numOfDeiveryStatusToStore Then
				Dim diff As Integer = list.Count - Me.numOfDeiveryStatusToStore
				list.RemoveRange(0, diff)
			End If

			If list.Count = Me.numOfDeiveryStatusToStore Then
				If list.Count > 1 Then
					list.RemoveAt(0)
				End If
			End If

			Dim statusInfoToStore As String = Convert.ToString(status.deliveryInfoNotification.messageId) & "_-_-" & Convert.ToString(status.deliveryInfoNotification.deliveryInfo.address) & "_-_-" & Convert.ToString(status.deliveryInfoNotification.deliveryInfo.deliveryStatus)
			list.Add(statusInfoToStore)

			Using sw As StreamWriter = File.CreateText(Request.MapPath(Me.deiveryStatusFilePath))
				Dim tempCount As Integer = 0
				While tempCount < list.Count
					Dim lineToWrite As String = list(tempCount)
					sw.WriteLine(lineToWrite)
					tempCount += 1
				End While
				sw.Close()
			End Using
		Catch ex As Exception
			File.AppendAllText(Request.MapPath("Error.txt"), DateTime.Now.ToString() & ": " & ex.ToString() & Environment.NewLine)
		End Try
	End Sub
	Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
		target = value
		Return value
	End Function
End Class