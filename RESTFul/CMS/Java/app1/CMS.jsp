<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<%@ page import="com.att.api.cc.CallControlHandler"%>
<%@ page import="com.att.api.cc.model.CallControlResponse"%>
<%@ page import="com.att.api.util.DateUtil"%>
<%@ include file="getToken.jsp"%>

<%!
public String escape(String str) {
	return org.apache.commons.lang3.StringEscapeUtils.escapeHtml4(str);
}
%>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title>AT&amp;T Sample Application for Call Management</title>
<meta content="text/html; charset=ISO-8859-1" http-equiv="Content-Type" />
<link rel="stylesheet" type="text/css" href="style/common.css" />
<style type="text/css">
.style4 {
	font-style: normal;
	font-variant: normal;
	font-weight: bold;
	font-size: 12px;
	line-height: normal;
	font-family: Arial, Sans-serif;
	width: 35%;
}

.style6 {
	font-style: normal;
	font-variant: normal;
	font-weight: bold;
	font-size: 12px;
	line-height: normal;
	font-family: Arial, Sans-serif;
	width: 25%;
}
</style>
</head>
<body>

	<%
		CallControlHandler handler = new CallControlHandler(request,
				application, sessionEndPoint, signalEndPoint, scriptPath,
				phoneNumber);
		CallControlResponse model = handler.processRequest();
	%>

	<form method="post" action="" id="form1" name="callcontrol">
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
					<h1>AT&amp;T Sample Application for Call Management</h1>
					<h2>
						Feature 1: Outbound Session from <span id="lblPhoneNumbers"><%=model.getPhoneNumber()%></span>
					</h2>
				</div>
			</div>
			<div class="navigation">
				<br />
				<table>
					<tbody>
						<tr>
							<td class="style4">Make call to:</td>
							<td class="cell" style="width: 60%"><input
								name="txtNumberToDial" type="text" id="txtNumberToDial"
								title="telephone number or sip address"
								value="<%=escape(model.getNumberToDial())%>" /></td>
						</tr>
						<tr>
							<td class="style4">Script Function:</td>
							<td class="cell" style="width: 20%"><input type="hidden"
								name="selectedScript" value=""></input> <select
								name="lstTemplate"
								onchange="javascript:document.forms['callcontrol'].selectedScript.value=this.options[this.selectedIndex].value; document.forms['callcontrol'].submit();"
								id="lstTemplate">
									<option selected="selected" value=""></option>
									<option <%=model.isSelected(request, "lstTemplate", "ask")%>
										value="ask">ask</option>
									<option
										<%=model.isSelected(request, "lstTemplate", "conference")%>
										value="conference">conference</option>
									<option
										<%=model.isSelected(request, "lstTemplate", "message")%>
										value="message">message</option>
									<option <%=model.isSelected(request, "lstTemplate", "reject")%>
										value="reject">reject</option>
									<option
										<%=model.isSelected(request, "lstTemplate", "transfer")%>
										value="transfer">transfer</option>
									<option <%=model.isSelected(request, "lstTemplate", "wait")%>
										value="wait">wait</option>
							</select></td>
						</tr>
						<tr>
							<td class="style4">Number parameter for Script Function:</td>
							<td class="cell" style="width: 60%"><input
								name="txtNumberForFeature" type="text"
								value="<%=escape(model.getFeaturedNumber())%>" id="txtNumberForFeature"
								title="If message or transfer or wait or reject is selected as script function, enter number for transfer-to or message-to or wait-from or reject-from" />
							</td>
						</tr>
						<tr>
							<td class="style4">Message To Play:</td>
							<td class="cell" style="width: 60%"><input
								name="txtMessageToPlay" type="text"
								value="<%=escape(model.getMessageToPlay())%>" id="txtMessageToPlay"
								title="enter long message or mp3 audio url, this is used as music on hold for transfer and signals" />
							</td>
						</tr>
						<tr>
							<td class="style4">Script Source Code:</td>
							<td class="cell" style="width: 60%"><textarea
									name="txtCreateSession" rows="2" cols="20"
									id="txtCreateSession" disabled="disabled"
									class="aspNetDisabled"
									title="Create Session will trigger an outbound call from application to &lt;Make call to> number."
									style="height: 141px; width: 400px;"><%=escape(model.getOutboundScriptText())%></textarea></td>
						</tr>
						<tr>
							<td class="style4"></td>
							<td align="center"><input type="submit"
								name="btnCreateSession" value="Create Session"
								id="btnCreateSession" /> <span id="Label1" class="warning"
								style="display: inline-block; width: 200px;">Create
									Session will trigger an outbound call from application to <strong>"Make
										call to"</strong> number.
							</span></td>
						</tr>
					</tbody>
				</table>
			</div>
			<div class="extra" align="left">
				<div class="warning">
					<div id="notesPanel">
						<strong>Note:</strong> <br />
						<%
							String selectedScript = model.getSelectedScriptName();
							if (selectedScript.equals("ask")) {
						%>
						Create Session will trigger an outbound call from application to <strong>Make
							call to</strong> number. <br /> For ask script function, user is prompted
						to enter few digits and entered digits are played back. <br />
						User is asked to press digit to activiate music on hold <strong>Message
							to Play</strong> to handle the signal (feature 2);
						<%
							} else if (selectedScript.equals("conference")) {
						%>
						description Create Session will trigger an outbound call from
						application to <strong>Make call to</strong> number. <br /> For
						conference script function, user is prompted to join the
						conference.<br /> After quitting the conference, user is asked to
						press digit to activiate music on hold <strong>Message to
							Play</strong> to handle the signal (feature 2);
						<%
							} else if (selectedScript.equals("message")) {
						%>
						For
					        <strong>message()</strong> script function, user is played
						back <strong>"Number parameter for Script Function"</strong>
						number and an SMS Message is sent to that number.<br/>
					        User is asked to press digit to activate music on hold
					        <strong>Message to Play</strong> to handle the signal (feature 2)
						<%
							} else if (selectedScript.equals("reject")) {
						%>
						description Create Session will trigger an outbound call from
						application to <strong>Make call to</strong> number. <br /> For
						reject script function, if <strong>Number parameter for
							Script Function</strong> matches with calling id, call will be dropped.<br />
						If calling id doesnt match, calling id and <strong>Number
							parameter for Script Function</strong> number are played to User.<br />
						User is asked to press digit to activiate music on hold <strong>Message
							to Play</strong> to handle the signal (feature 2);
						<%
							} else if (selectedScript.equals("transfer")) {
						%>
						description Create Session will trigger an outbound call from
						application to <strong>Make call to</strong> number. <br /> For
						transfer script function, user is played back with <strong>Number
							parameter for Script Function</strong> and call be transferred to that
						number.<br /> While doing transfer music on hold <strong>Message
							to Play</strong> is played. Once <strong>Number parameter for
							Script Function</strong> number disconnects the call, user is asked to
						press digit to activiate music on hold <strong>Message to
							Play</strong> to handle the signal (feature 2);
						<%
							} else if (selectedScript.equals("wait")) {
						%>
						description Create Session will trigger an outbound call from
						application to <strong>Make call to</strong> number. <br /> For
						wait script function, if <strong>Number parameter for
							Script Function</strong> matches with calling id, call will be kept on
						hold for 3 seconds.<br /> If calling id doesnt match, calling id
						and <strong>Number parameter for Script Function</strong> number
						are played to User.<br /> User is asked to press digit to
						activiate music on hold <strong>Message to Play</strong> to handle
						the signal (feature 2);
						<%
							} else {
						%>
						User is asked to press digit to activiate music on hold <strong>"Message
							to Play"</strong> to handle the signal (feature 2)
						<%
							}
						%>
					</div>
				</div>
			</div>
			<br style="clear: both;" />
			<div>
				<div id="pnlCreateSession" style="font-family: Calibri;">
					<%
						if (model.isCreateSession() && model.getResultStatus()) {
					%>
					<table class="successWide"
						style="border-style: Outset; font-family: Sans-serif; font-size: 9pt; width: 650px;">
						<tbody>
							<tr>
								<td style="font-weight: bold;">SUCCESS:</td>
							</tr>
							<tr>
								<td class="label" style="width: 10%">id</td>
								<td class="cell"><%=model.getSessionId()%></td>
							</tr>
							<tr>
								<td class="label" style="width: 10%">success</td>
								<td class="cell">true</td>
							</tr>
						</tbody>
					</table>
					<%
						} else if (model.isCreateSession() && !model.getResultStatus()) {
					%>
					<table class="errorWide"
						style="border-style: Outset; font-family: Sans-serif; font-size: 9pt; width: 650px;">
						<tbody>
							<tr>
								<td style="font-weight: bold;">ERROR:</td>
							</tr>
							<tr>
								<td>Status:<%=model.getStatusCode()%>
								</td>
							</tr>
							<tr>
								<td><%=model.getErrorResponse()%></td>
							</tr>
						</tbody>
					</table>
					<%
						}
					%>
					<br clear="all" />
				</div>
			</div>
			<br style="clear: both;" />
			<div>
				<div class="content">
					<h2>&nbsp;</h2>
					<h2>Feature 2: Send Signal to Session</h2>
					<p>&nbsp;</p>
				</div>
			</div>
			<div class="navigation">
				<table style="width: 100%">
					<tbody>
						<%
							String sessionId = model.getSessionId();
							if (sessionId == null) sessionId = "";
						%>
						<tr>
							<td class="style6"><span id="lblSession" class="label">Session
									ID: </span></td>
							<td><span id="lblSessionId"><%=sessionId%></span></td>
						</tr>
						<tr>
							<td class="style6"><span id="lblSignal" class="label">Signal
									to Send: </span></td>
							<td><select name="ddlSignal" id="ddlSignal">
									<option <%=model.isSelected(request, "ddlSignal", "exit")%>
										value="exit">exit</option>
									<option <%=model.isSelected(request, "ddlSignal", "stopHold")%>
										value="stopHold">stopHold</option>
									<option <%=model.isSelected(request, "ddlSignal", "dequeue")%>
										value="dequeue">dequeue</option>
							</select></td>
						</tr>
						<tr>
							<td class="style6"></td>
							<td align="center"><input type="submit" name="btnSendSignal"
								value="Send Signal" id="btnSendSignal" /></td>
						</tr>
					</tbody>
				</table>
			</div>
			<div class="extra"></div>
			<br style="clear: both;" />
			<div>
				<div id="pnlSendSignal" style="font-family: Calibri;">
					<%
						if (model.isSendSignal() && model.getResultStatus()) {
					%>
					<table class="successWide"
						style="border-style: Outset; font-family: Sans-serif; font-size: 9pt; width: 650px;">
						<tbody>
							<tr>
								<td style="font-weight: bold;">SUCCESS:</td>
							</tr>
							<tr>
								<td class="label" style="width: 10%">Status</td>
								<td class="cell"><%=model.getSignalStatus()%></td>
							</tr>
						</tbody>
					</table>
					<%
						} else if (model.isSendSignal() && !model.getResultStatus()) {
					%>

					<table class="errorWide"
						style="border-style: Outset; font-family: Sans-serif; font-size: 9pt; width: 650px;">
						<tbody>
							<tr>
								<td style="font-weight: bold;">ERROR:</td>
							</tr>
							<tr>
								<td>Status:<%=model.getStatusCode()%>
								</td>
							</tr>
							<tr>
								<td><%=model.getErrorResponse()%></td>
							</tr>
						</tbody>
					</table>
					<%
						}
					%>
				</div>
			</div>
			<br style="clear: both;" />
			<div id="footer">
				<div
					style="float: right; width: 20%; font-size: 9px; text-align: right">
					Powered by AT&amp;T Cloud Architecture</div>
				<p>
					© 2012 AT&amp;T Intellectual Property. All rights reserved. <a
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
	</form>
</body>
</html>
