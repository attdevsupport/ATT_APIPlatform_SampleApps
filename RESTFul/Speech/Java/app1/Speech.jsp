<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																								
For more information contact developer.support@att.com
-->
<%@ page import="com.att.api.util.DateUtil"%>
<%@ page import="com.att.api.speech.model.SpeechResponse"%>
<%@ page import="com.att.api.speech.handler.Config"%>
<%@ include file="getToken.jsp"%>
<%
	String cfgError = cfg.getError();
if (cfgError != null) {
	request.setAttribute("response", new SpeechResponse(cfgError));
}
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title>AT&amp;T Sample Speech Application - Speech to Text
	(Generic) Application</title>
<meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
<link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>
	<div id="container">
		<!-- open HEADER -->
		<div id="header">
			<div>
				<div class="hcRight"><%=DateUtil.getServerTime()%></div>
				<div class="hcLeft">Server Time:</div>
			</div>
			<div>
				<div class="hcRight">
					<script language="JavaScript" type="text/javascript">
						var myDate = new Date();
						document.write(myDate);
					</script>
				</div>
				<div class="hcLeft">Client Time:</div>
			</div>
			<div>
				<div class="hcRight">
					<script language="JavaScript" type="text/javascript">
						document.write("" + navigator.userAgent);
					</script>
				</div>
				<div class="hcLeft">User Agent:</div>
			</div>
			<br clear="all" />
		</div>
		<!-- close HEADER -->
		<div>
			<div class="content">
				<h1>AT&amp;T Sample Speech Application - Speech to Text
					Application</h1>
				<h2>Feature 1: Speech to Text</h2>
			</div>
		</div>
		<br /> <br />
		<form name="SpeechToText" action="upload" method="post"
			enctype="multipart/form-data">
			<div class="navigation">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td valign="middle" class="label" align="right">Speech
								Context:</td>
							<td class="cell"><select name="context">
									<%
										if (cfg.speechContexts != null) {
											for (final String sContext : cfg.speechContexts) {
									%>
									<option value="<%=sContext%>"><%=sContext%></option>
									<%
										}
										}
									%>

							</select></td>
						</tr>
						<tr>
							<td width="25%" valign="top" class="label" align="right">
								Audio File:</td>
							<td class="cell"><input name="f1" type="file" /></td>
						</tr>
						<tr>
							<td></td>
							<td class="cell"><input type="checkbox" name="chkChunked"
								value="Send Chunked" />Send Chunked</td>
						</tr>
						<tr>
							<td valign="middle" class="label" align="right">X-Arg:</td>
							<td class="cell"><input type="text" name="x-arg"
								readonly="readonly" style="height: 120px; width: 234px"
								value="<%=cfg.getXArgHTTPValue()%>" /></td>
						</tr>
						<tr>
							<td></td>
							<td></td>
							<td>
								<button type="submit" name="SpeechToText">Submit</button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
			<div class="extra">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td />
							<td>
								<div id="extraleft">
									<div class="warning">
										<strong>Note:</strong><br /> If no file is chosen, a <a
											href="./<%=cfg.defaultFile%>">default</a> file will be loaded
										on submit.<br /> <strong>Speech file format
											constraints:</strong> <br /> 16 bit PCM WAV, single channel, 8 kHz
										sampling <br /> 16 bit PCM WAV, single channel, 16 kHz
										sampling <br /> AMR (narrowband), 12.2 kbit/s, 8 kHz sampling
										<br /> AMR-WB (wideband) is 12.65 kbit/s, 16khz sampling <br />
										OGG - speex encoding, 8kHz sampling <br /> OGG - speex
										encoding, 16kHz sampling
									</div>
								</div>
							</td>
							<td />
						</tr>
					</tbody>
				</table>
			</div>
		</form>
		<br clear="all" />
		<%
			String error = (String) session.getAttribute("errorResponse");
			SpeechResponse speechResponse = null;
			if (error == null) {
				speechResponse = (SpeechResponse) request.getAttribute("response");
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
		<br />
		<div align="center">
			<table width="500" cellpadding="1" cellspacing="1" border="0">
				<thead>
					<tr>
						<th width="50%" class="label">Parameter</th>
						<th width="50%" class="label">Value</th>
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
		</div>
		<%
			}
		%>
		<div id="footer">
			<div
				style="float: right; width: 20%; font-size: 9px; text-align: right">
				Powered by AT&amp;T Cloud Architecture</div>
			<p>
				&#169; 2012 AT&amp;T Intellectual Property. All rights reserved. <a
					href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
				<br /> The Application hosted on this site are working examples
				intended to be used for reference in creating products to consume
				AT&amp;T Services and not meant to be used as part of your product.
				The data in these pages is for test purposes only and intended only
				for use as a reference in how the services perform. <br /> For
				download of tools and documentation, please go to <a
					href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
				<br /> For more information contact <a
					href="mailto:developer.support@att.com">developer.support@att.com</a>
			</p>
		</div>
	</div>
	<p>&nbsp;</p>
</body>
</html>
