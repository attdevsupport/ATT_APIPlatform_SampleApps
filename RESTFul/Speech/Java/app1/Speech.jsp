<!DOCTYPE html>
<%@ page import="com.att.api.util.DateUtil"%>
<%@ page import="com.att.api.speech.model.SpeechResponse"%>
<%@ page import="com.att.api.speech.handler.Config"%>
<%@ page import="java.util.HashMap" %>
<%@ include file="getToken.jsp"%>
<%
	String cfgError = cfg.getError();
if (cfgError != null) {
	request.setAttribute("response", new SpeechResponse(cfgError));
}
%>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 
September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: 
http://developer.att.com/sdk_agreement/

Copyright 2011 AT&T Intellectual Property. All rights reserved. 
http://developer.att.com

For more information contact developer.support@att.com
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
<title>AT&amp;T Sample Speech Application - Speech to Text
	(Generic)</title>
<meta content="text/html; charset=UTF-8" http-equiv="Content-Type">
<meta id="viewport" name="viewport"
	content="width=device-width,minimum-scale=1,maximum-scale=1">
<meta http-equiv="content-type" content="text/html; charset=UTF-8">
<link rel="stylesheet" type="text/css" href="style/common.css">
</head>
<body>
	<div id="pageContainer" class="pageContainer">
		<div id="header">
			<div class="logo" id="top"></div>
			<div id="menuButton" class="hide">
				<a id="jump" href="#nav">Main Navigation</a>
			</div>
			<ul class="links" id="nav">
				<li><a href="#" target="_blank">Full Page<img
						src="images/max.png"></img></a> <span class="divider"> |&nbsp;</span>
				</li>
				<li><a href="<%=cfg.linkSource%>" target="_blank">Source<img
						src="images/source.png" /></a> <span class="divider"> |&nbsp;</span></li>
				<li><a href="<%=cfg.linkDownload%>" target="_blank">Download<img
						src="images/download.png"></a> <span class="divider">
						|&nbsp;</span></li>
				<li><a href="<%=cfg.linkHelp%>" target="_blank">Help</a></li>
				<li id="back"><a href="#top">Back to top</a></li>
			</ul>
			<!-- end of links -->
		</div>
		<!-- end of header -->
		<div class="content">
			<div class="contentHeading">
				<h1>AT&amp;T Sample Application - Speech to Text</h1>
				<div id="introtext">
					<div>
						<b>Server Time:</b>
						<%=DateUtil.getServerTime()%></div>
					<div>
						<b>Client Time&nbsp;:</b>
						<script>
							document.write("" + new Date());
						</script>
					</div>
					<div>
						<b>User Agent&nbsp;:</b>
						<script>
							document.write("" + navigator.userAgent);
						</script>
					</div>
				</div>
				<!-- end of introtext -->
			</div>
			<%
				HashMap<String,String> fieldValues = (HashMap<String,String>) session.getAttribute("formFields");
				String selectedContext = "";
				String sendChunked = "";
				String xArg = cfg.getXArgHTTPValue();
				String selectedFileName = "";
				if (fieldValues != null) {
					if (fieldValues.get("chkChunked") != null) sendChunked = " checked";
					if (fieldValues.get("x-arg") != null) xArg = fieldValues.get("x-arg");
					if (fieldValues.get("filename") != null) selectedFileName = fieldValues.get("filename");
				}
			%>
			<!-- end of contentHeading -->
			<div class="formBox" id="formBox">
				<div id="formContainer" class="formContainer">
					<form name="SpeechToText" action="upload" method="post">
						<div id="formData">
							<h3>Speech Context:</h3>
							<select name="context">
								<%
									if (cfg.speechContexts != null) {
										for (final String sContext : cfg.speechContexts) {
											if (fieldValues != null && fieldValues.get("context").equals(sContext)) {
												selectedContext = " selected";
											}
											else
											{
												selectedContext = "";
											}
								%>
										<option value="<%=sContext%>" <%=selectedContext%>><%=sContext%></option>
								<%
										}
									}
								%>
							</select>
							<h3>Audio File:</h3>
							<select name="filename">
								<%
									String selectedAtt = "";
									String directory = request.getSession().getServletContext()
											.getRealPath("/") + cfg.audioFolder;
									File[] list = new File(directory).listFiles();
									for (File f : list) {
										if (f.getName().equals(selectedFileName)) {
											selectedAtt = " selected";
										}
										else
										{
											selectedAtt = "";
										}
								%>
									<option value="<%=f.getName()%>" <%=selectedAtt%>><%=f.getName()%></option>
								<% 		
									}
								%>
							</select>
							<div id="chunked">
							<br />
							<b>Send Chunked:</b>
							<input name="chkChunked" value="Send Chunked" type="checkbox" <%=sendChunked %>>
							</div>
							<h3>X-Arg:</h3>
							<textarea id="x_arg" name="x-arg" readonly="readonly" rows="4"><%=xArg%></textarea>
							<br>
							<button type="submit" name="SpeechToText">Submit</button>
						</div>
					</form>
				</div>
				<%
					String error = (String) session.getAttribute("errorResponse");
					SpeechResponse speechResponse = null;
					if (error == null) {
						speechResponse = (SpeechResponse) request
								.getAttribute("response");
						if (speechResponse != null && speechResponse.hasError()) {
							error = speechResponse.getError();
						}
					}
					if (error != null) {
				%>
				<div class="errorWide">
					<strong>ERROR:</strong><br />
					<%=error%>
				</div>
				<%
					} else if (speechResponse != null) {
				%>
				<div class="successWide">
					<strong>SUCCESS:</strong> <br />Response parameters listed below.
				</div>
				<table class="kvp">
					<thead>
						<tr>
							<th class="label">Parameter</th>
							<th class="label">Value</th>
						</tr>
					</thead>
					<tbody>
						<%
							for (final String[] kvp : speechResponse.getResult()) {
									final String key = kvp[0];
									final String value = kvp[1];
						%>
						<tr>
							<td class="cell" align="center"><em><%=key%></em></td>
							<td class="cell" align="center"><em><%=value%></em></td>
						</tr>
						<%
							}
						%>
					</tbody>
				</table>
				<%
					}
				%>
			</div>
		</div>
		<!-- end of content -->
		<div id="footer">
			<div id="ft">
				<div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
				<p>
					The Application hosted on this site are working examples intended
					to be used for reference in creating products to consume AT&amp;T
					Services and not meant to be used as part of your product. The data
					in these pages is for test purposes only and intended only for use
					as a reference in how the services perform. <br> <br> For
					download of tools and documentation, please go to <a
						href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
					<br> For more information contact <a
						href="mailto:developer.support@att.com">developer.support@att.com</a>
					<br> <br> © 2012 AT&amp;T Intellectual Property. All
					rights reserved. <a href="http://developer.att.com/"
						target="_blank">http://developer.att.com</a>
				</p>
			</div>
			<!-- end of ft -->
		</div>
		<!-- end of footer -->
	</div>
	<!-- end of page_container -->


</body>
</html>
