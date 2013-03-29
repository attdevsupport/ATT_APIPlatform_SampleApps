' <copyright file="Default.aspx.vb" company="AT&amp;T">
' Licensed by AT&amp;T under 'Software Development Kit Tools Agreement.' 2013
' TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
' Copyright 2013 AT&amp;T Intellectual Property. All rights reserved. http://developer.att.com
' For more information contact developer.support@att.com
' </copyright>

#Region "References"

Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions



#End Region

''' <summary>
''' MMSApp3_Listener class
''' </summary>
Partial Public Class MMSApp3_Listener
    Inherits System.Web.UI.Page
    ''' <summary>
    ''' Event, that triggers when the applicaiton page is loaded into the browser
    ''' Listens to server and stores the mms messages in server
    ''' </summary>
    ''' <param name="sender">object, that caused this event</param>
    ''' <param name="e">Event that invoked this function</param>
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Dim fileStream As FileStream = Nothing
        Try
            Dim random As New Random()
            Dim currentServerTime As DateTime = DateTime.UtcNow

            Dim receivedTime As String = currentServerTime.ToString("HH-MM-SS")
            Dim receivedDate As String = currentServerTime.ToString("MM-dd-yyyy")

            Dim inputStreamContents As String
            Dim stringLength As Integer
            Dim strRead As Integer

            Dim stream As Stream = Request.InputStream
            stringLength = Convert.ToInt32(stream.Length)

            Dim stringArray As Byte() = New Byte(stringLength - 1) {}
            strRead = stream.Read(stringArray, 0, stringLength)
            inputStreamContents = System.Text.Encoding.UTF8.GetString(stringArray)

            Dim splitData As String() = Regex.Split(inputStreamContents, "</SenderAddress>")
            Dim data As String = splitData(0).ToString()
            Dim senderAddress As String = inputStreamContents.Substring(data.IndexOf("tel:") + 4, data.Length - (data.IndexOf("tel:") + 4))
            Dim parts As String() = Regex.Split(inputStreamContents, "--Nokia-mm-messageHandler-BoUnDaRy")
            Dim lowerParts As String() = Regex.Split(parts(2), "BASE64")
            Dim imageType As String() = Regex.Split(lowerParts(0), "image/")
            Dim indexOfSemicolon As Integer = imageType(1).IndexOf(";")
            Dim type As String = imageType(1).Substring(0, indexOfSemicolon)
            Dim encoder As UTF8Encoding = New System.Text.UTF8Encoding()
            Dim utf8Decode As Decoder = encoder.GetDecoder()

            Dim todecode_byte As Byte() = Convert.FromBase64String(lowerParts(1))

            If Not Directory.Exists(Request.MapPath(ConfigurationManager.AppSettings("ImageDirectory"))) Then
                Directory.CreateDirectory(Request.MapPath(ConfigurationManager.AppSettings("ImageDirectory")))
            End If
            Dim detailsToStore As String = "tel:" & senderAddress & ":-:" & receivedDate & " At " & receivedTime & "UTC"
            Dim fileNameToSave As String = "From_" & senderAddress.Replace("+", "") & "_At_" & receivedTime & "_UTC_On_" & receivedDate & random.[Next]()
            detailsToStore = detailsToStore & ":-:" & fileNameToSave & "." & type & ":-:" & "Test Subject"
            fileStream = New FileStream(Request.MapPath(ConfigurationManager.AppSettings("ImageDirectory")) & fileNameToSave & "." & type, FileMode.CreateNew, FileAccess.Write)
            fileStream.Write(todecode_byte, 0, todecode_byte.Length)
            WriteRecordToFile(detailsToStore)
        Catch
        Finally
            If fileStream IsNot Nothing Then
                fileStream.Close()
            End If
        End Try

    End Sub

    ''' <summary>
    ''' Method to update file.
    ''' </summary>
    ''' <param name="transactionid">Transaction Id</param>
    ''' <param name="merchantTransactionId">Merchant Transaction Id</param>
    Public Sub WriteRecordToFile(value As String)
        Try
            Dim imageDetails As String = Request.MapPath(ConfigurationManager.AppSettings("ImageDirectory")) & "imageDetails.txt"
            Dim NumOfFilesToStoreAndDisplay As Integer = 5
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("NumOfFilesToStoreAndDisplay")) Then
                NumOfFilesToStoreAndDisplay = Convert.ToInt32(ConfigurationManager.AppSettings("NumOfFilesToStoreAndDisplay").ToString())
            End If
            Dim list As New List(Of String)()
            Dim file__1 As New FileStream(imageDetails, FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(file__1)
            Dim line As String

            While (InlineAssignHelper(line, sr.ReadLine())) IsNot Nothing
                list.Add(line)
            End While

            sr.Close()
            file__1.Close()

            If list.Count > NumOfFilesToStoreAndDisplay Then
                Dim diff As Integer = list.Count - NumOfFilesToStoreAndDisplay
                list.RemoveRange(0, diff)
            End If

            If list.Count = NumOfFilesToStoreAndDisplay Then
                'delete file too.
                list.RemoveAt(0)
            End If
            list.Add(value)
            Using sw As StreamWriter = File.CreateText(imageDetails)
                Dim tempCount As Integer = 0
                While tempCount < list.Count
                    Dim lineToWrite As String = list(tempCount)
                    sw.WriteLine(lineToWrite)
                    tempCount += 1
                End While
                sw.Close()
            End Using
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class