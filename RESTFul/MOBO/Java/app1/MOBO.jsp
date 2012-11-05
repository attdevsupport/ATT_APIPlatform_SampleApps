
<%
	//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title>AT&T Sample Mobo Application 1 - Basic Mobo Service
	Application</title>
<meta content="text/html; charset=ISO-8859-1" http-equiv="Content-Type" />
<link rel="stylesheet" type="text/css" href="style/common.css" />
<style type="text/css">
.style1 {
	font-style: normal;
	font-variant: normal;
	font-weight: bold;
	font-size: 12px;
	line-height: normal;
	font-family: Arial, Sans-serif;
	width: 92px;
}
</style>
</head>
<body>
	<%@ page language="java" session="true"%>
	<%@ page contentType="text/html; charset=iso-8859-1" language="java"%>
	<%@ page import="org.apache.commons.fileupload.*"%>
	<%@ page import="java.util.*"%>
	<%@page import="java.util.regex.Matcher"%>
	<%@page import="java.util.regex.Pattern"%>
	<%@ page import="java.io.*"%>
	<%@ page import="org.json.*"%>
	<%@ page import="java.net.*"%>
	<%@page import="org.apache.http.HttpHost"%>
	<%@page import="org.apache.http.HttpResponse"%>
	<%@page import="org.apache.http.client.methods.HttpPost"%>
	<%@page import="org.apache.http.entity.mime.FormBodyPart"%>
	<%@page import="org.apache.http.entity.mime.HttpMultipartMode"%>
	<%@page import="org.apache.http.entity.mime.MultipartEntity"%>
	<%@page import="org.apache.http.entity.mime.content.FileBody"%>
	<%@page import="org.apache.http.entity.mime.content.StringBody"%>
	<%@page import="org.apache.http.impl.client.DefaultHttpClient"%>
	<%@ page import="java.nio.charset.Charset"%>

	<%@ include file="config.jsp"%>
	<%
		final String contentBodyFormat = "FORM-ENCODED"; 
			final String responseFormat = "json";
			final String requestFormat = "json";
			final String endpoint = FQDN + "/rest/1/MyMessages";
			final String priority = "HIGH";
			String accessToken =  "";
			int statusCode = 0;
			String responze = "";
			String ID = "";
			accessToken = (String) session.getAttribute("accessToken");

			List<String> addresses = new java.util.ArrayList<String>();
			List badAddresses = new java.util.ArrayList();
			List<String> files = new ArrayList<String>();
			
			int numShort = 0;

			String sendMessageButton = request.getParameter("sendMessageButton");        
			boolean groupBoxError = false;
			String phoneTextBox = "";
			String subjectTextBox = "";
			String messageTextBox = "";
			String groupCheckBox  = "";
			
			//If Send MMS button was clicked, do this to get some parameters from the form.
			if( sendMessageButton != null) 
			{ 
				try
				{
					        DiskFileUpload fu = new DiskFileUpload();
					        List fileItems = fu.parseRequest(request);
					        Iterator itr = fileItems.iterator();
					        session.setAttribute("files",files);
					        
					        while(itr.hasNext()) 
					        {
				       FileItem fi = (FileItem)itr.next();
				       if(!fi.isFormField()) 
				 	   {
							if (fi.getName() != "" )
							{
								File fNew= new File(application.getRealPath("/"), fi.getName());
							    files.add(fNew.getAbsolutePath());
							    //files.add(fNew.getName());
							    if(!(fi.getName().equalsIgnoreCase("")))
							    {
							    	fi.write(fNew);
							    }
							}
					   } else {
					       session.setAttribute(fi.getFieldName(),fi.getString().trim());
					   }
					}
			    }
			    catch(Exception e)
			    {
			    } 

		phoneTextBox = (String) session.getAttribute("phoneTextBox") == null ? "" : (String) session.getAttribute("phoneTextBox");
		subjectTextBox = (String) session.getAttribute("subjectTextBox") == null ? "" : (String) session.getAttribute("subjectTextBox");
		messageTextBox = (String) session.getAttribute("messageTextBox") == null ? "" : (String) session.getAttribute("messageTextBox");
		groupCheckBox  = (String) session.getAttribute("groupCheckBox") == null ? "" : (String) session.getAttribute("groupCheckBox");
		
		//if we get redirected input parameters could be null
		if (phoneTextBox != null)
		{
			phoneTextBox = phoneTextBox.trim();
			//Parse the phone addresses
			String [] address = phoneTextBox.split(",");
			if (address == null || address.length == 0) {
				address = new String[] { phoneTextBox };
			}
			for(String a : address) 
			{
				if(a.length() >= 10)
				{	
					 String expression = "[+]?[0-15]*[0-9]+$";  
					 CharSequence inputStr = a;  
					 Pattern pattern = Pattern.compile(expression);  
					 Matcher matcher = pattern.matcher(inputStr);  
					 if(matcher.matches()){  
						a = "tel:"+a;
						addresses.add(a);
					 }		  
				}
				else if((a.length()>2) && (a.length()<=8))
				{
					String expression = "[0-15]*[0-9]+$";  
					CharSequence inputStr = a;  
					Pattern pattern = Pattern.compile(expression);  
					Matcher matcher = pattern.matcher(inputStr);  
					if(matcher.matches()){  
						a = "short:"+a;
						addresses.add(a);
					 }
				}
				else if(a.contains("@"))
				{
					a =a;
					addresses.add(a);
				}
				else
				{
					badAddresses.add(a);
				}
			}		
		}
			String requestBody = "";
			if (accessToken == null)
			{
			        	getServletContext().getRequestDispatcher("/oauth.jsp").forward(request, response);
			}
			else
			{
				  JSONArray numbers = new JSONArray(addresses);
				  JSONObject requestObject = new JSONObject();
				  requestObject.put("Addresses", numbers);	//numbers
				  requestObject.put("Text", messageTextBox.trim());
				  requestObject.put("Subject", subjectTextBox.trim());
			
				  if (groupCheckBox != null && groupCheckBox.equals("on")) 
				  {
					   requestObject.put("Group", "true");
				  }
				  else
				  {
					   requestObject.put("Group", "false");
				  }	
				  requestBody += requestObject.toString();
				  
			  	//Check whether attachments are present
			  	//if present do multipart/related or else single body part
				files = (List<String>) session.getAttribute("files");
				DefaultHttpClient mclient = new DefaultHttpClient();

				HttpPost post = new HttpPost(endpoint);
				post.addHeader("Authorization", "BEARER "+ accessToken);
				post.addHeader("Content-Type", "multipart/form-data; type=\"application/x-www-form-urlencoded\"; start=\"<startpart>\"; boundary=\"foo\"");
				MultipartEntity entity = new MultipartEntity(HttpMultipartMode.STRICT, "foo", Charset.forName("UTF-8"));

				StringBuilder sbuilder = new StringBuilder();
				sbuilder.append("Addresses=");
				if (addresses.size() > 0) {
					sbuilder.append(URLEncoder.encode(addresses.get(0), "UTF-8"));
				}

				for (int i = 1; i < addresses.size(); ++i) {
					sbuilder.append(URLEncoder.encode("," + addresses.get(i), "UTF-8"));
				}	

				sbuilder.append("&Text=");
				sbuilder.append(URLEncoder.encode(messageTextBox.trim(), "UTF-8"));
				
				sbuilder.append("&Subject=");
				sbuilder.append(URLEncoder.encode(subjectTextBox.trim(), "UTF-8"));

				String groupValue = "false";
				if (groupCheckBox != null && groupCheckBox.equals("on")) {
		   					groupValue = "true";
				}
				sbuilder.append("&Group=");
				sbuilder.append(groupValue);
				
				String sMsg = sbuilder.toString(); 
				
				StringBody sbody = new StringBody(sMsg, "application/x-www-form-urlencoded",
	        			Charset.forName("UTF-8"));
				FormBodyPart stringBodyPart = new FormBodyPart("root-fields", sbody);
				stringBodyPart.addField("Content-ID", "<startpart>");
				entity.addPart(stringBodyPart);

				if (files != null) {
					for (int i = 0; i < files.size(); ++i) {
						final String fname = files.get(i);
						String type = URLConnection.guessContentTypeFromStream(new FileInputStream(fname));
						if (type == null){
							type = URLConnection.guessContentTypeFromName(fname);
						}
						if (type == null) type="application/octet-stream";
						FileBody fb = new FileBody(new File(fname), type, "UTF-8");
						FormBodyPart fileBodyPart = new FormBodyPart(fb.getFilename(), fb);
						fileBodyPart.addField("Content-ID", "<fileattachment" + i + ">");
						fileBodyPart.addField("Content-Location", fb.getFilename());
						entity.addPart(fileBodyPart);
					}
				}

				post.setEntity(entity);
				HttpResponse responseBody = mclient.execute(post);
				statusCode = responseBody.getStatusLine().getStatusCode();
				responze = org.apache.http.util.EntityUtils.toString(responseBody.getEntity());
				
				//remove the files
				if (files != null)
				{
					for (String file: files) {
							new File(file).delete();
					}
				}
				//clean up input parameters from the session
				session.removeAttribute("phoneTextBox");
				session.removeAttribute("messageTextBox");
				session.removeAttribute("subjectTextBox");
				session.removeAttribute("groupCheckBox");
			}
			}
	%>
	<div id="container">
		<!-- open HEADER -->
		<div id="header">
			<div>
				<div id="hcRight">
					<%=new java.util.Date()%>
				</div>
				<div id="hcLeft">Server Time:</div>
			</div>
			<div>
				<div id="hcRight">
					<script language="JavaScript" type="text/javascript">
						var myDate = new Date();
						document.write(myDate);
					</script>
				</div>
				<div id="hcLeft">Client Time:</div>
			</div>
			<div>
				<div id="hcRight">
					<script language="JavaScript" type="text/javascript">
						document.write("" + navigator.userAgent);
					</script>
				</div>
				<div id="hcLeft">User Agent:</div>
			</div>
			<br clear="all" />
		</div>
		<!-- close HEADER -->
		<div id="wrapper">
			<div id="content">
				<h1>AT&T Sample Mobo Application 1 Basic Mobo Service
					Application</h1>
				<h2>Feature 1: Send Message</h2>
			</div>
		</div>
		<br clear="all" />
		<form method="post" name="sendMessageButton"
			enctype="multipart/form-data"
			action="MOBO.jsp?sendMessageButton=true">
			<div id="navigation">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td width="20%" valign="top" class="label">Address:</td>
							<td class="cell"><input name="phoneTextBox" type="text"
								maxlength="60" value="<%=phoneTextBox%>" style="width: 291px;" />
							</td>
						</tr>
						<tr>
							<td valign="top" class="label">Message:</td>
							<td class="cell"><textarea name="messageTextBox" rows="2"
									cols="20" style="height: 99px; width: 291px;"><%=messageTextBox%></textarea>
							</td>
						</tr>
						<tr>
							<td valign="top" class="label">Subject:</td>
							<td class="cell"><textarea name="subjectTextBox" rows="2"
									cols="20" style="height: 99px; width: 291px;"><%=subjectTextBox%></textarea>
							</td>
						</tr>
						<tr>
							<td valign="top" class="label">Group:</td>
							<td class="cell"><input name="groupCheckBox" type="checkbox"
								id="phoneTextBox" /></td>
						</tr>
					</tbody>
				</table>
			</div>
			<div id="extra">
				<div class="warning">
					<strong>WARNING:</strong><br />total size of all attachments
					cannot exceed 600 KB.
				</div>
			</div>
			<div id="extra">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td valign="bottom" class="style1">Attachment 1:</td>
							<td class="cell"><input type="file" name="FileUpload1"
								id="FileUpload1" /></td>
						</tr>
						<tr>
							<td valign="bottom" class="style1">Attachment 2:</td>
							<td class="cell"><input type="file" name="FileUpload2"
								id="FileUpload2" /></td>
						</tr>
						<tr>
							<td valign="bottom" class="style1">Attachment 3:</td>
							<td class="cell"><input type="file" name="FileUpload3"
								id="FileUpload3" /></td>
						</tr>
						<tr>
							<td valign="bottom" class="style1">Attachment 4:</td>
							<td class="cell"><input type="file" name="FileUpload4"
								id="FileUpload4" /></td>
						</tr>
						<tr>
							<td valign="bottom" class="style1">Attachment 5:</td>
							<td class="cell"><input type="file" name="FileUpload5"
								id="FileUpload5" /></td>
						</tr>
					</tbody>
				</table>
				<table>
					<tbody>
						<tr>
							<td><input type="submit" name="sendMessageButton"
								value="Send Message" id="sendMessageButton" /></td>
						</tr>
					</tbody>
				</table>
			</div>
			<br clear="all" />
		</form>
		<%
			if (sendMessageButton != null) {
				if (statusCode == 200 || statusCode == 201) {
					JSONObject rpcObject = new JSONObject(responze);
					ID = rpcObject.getString("Id");
					groupCheckBox = null;
		%>
		<div class="successWide">
			<strong>SUCCESS</strong><br /> <strong>Message ID: <%=ID%></strong>
		</div>
		<br />
		<%
			} else if (groupBoxError) {
		%>
		<div class="errorWide">
			<strong>ERROR:</strong><br /> <strong>Cant select group and
				short</strong>
		</div>
		<br />
		<%
			} else {
		%>
		<div class="errorWide">
			<strong>ERROR:</strong><br /> <strong><%=responze%></strong>
		</div>
		<br />
		<%
			}
			}
		%>
		<div align="center">
			<div id="sendMessagePanel"
				style="font-family: Calibri; font-size: XX-Small;"></div>
		</div>
		<br clear="all" />
		<div id="footer">
			<div
				style="float: right; width: 20%; font-size: 9px; text-align: right">Powered
				by AT&amp;T Cloud Architecture</div>
			<p>
				© 2012 AT&amp;T Intellectual Property. All rights reserved. <a
					href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
				<br> The Application hosted on this site are working examples
				intended to be used for reference in creating products to consume
				AT&amp;T Services and not meant to be used as part of your product.
				The data in these pages is for test purposes only and intended only
				for use as a reference in how the services perform. <br> For
				download of tools and documentation, please go to <a
					href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
				<br> For more information contact <a
					href="mailto:developer.support@att.com">developer.support@att.com</a>
		</div>
</body>
</html>