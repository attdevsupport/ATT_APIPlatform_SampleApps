// 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
//

package com.att.api.cc.model;

import javax.servlet.http.HttpServletRequest;

public class CallControlResponse {

	public static final String DEFAULT_DISPLAY_SCRIPT = "answer.js";
	public static final String OUTBOUND_SCRIPT = "call.js";
	
	//action flags to display success/failure messages
	private boolean updateScript;
	private boolean createSession;
	private boolean sendSignal;
	
	private String selectedScriptName;
	private String selectedScriptText;
	private String outboundScriptText;
	private String sessionId;
	private String signalType;
	
	//API Status flags
	private int statusCode;
	private String errorResponse;
	private boolean resultStatus;
	
	//action status
	private String sessionStatus;
	private String signalStatus;
	
	private String phoneNumber;

	private String numberToDial;
	private String messageToDisplay;

	public boolean isUpdateScript() {
		return updateScript;
	}
	public void setUpdateScript(boolean updateScript) {
		this.updateScript = updateScript;
	}
	public boolean isCreateSession() {
		return createSession;
	}
	public void setCreateSession(boolean createSession) {
		this.createSession = createSession;
	}
	public boolean isSendSignal() {
		return sendSignal;
	}
	public void setSendSignal(boolean sendSignal) {
		this.sendSignal = sendSignal;
	}
	public String getSelectedScriptName() {
		return selectedScriptName;
	}
	public void setSelectedScriptName(String scriptName) {
		this.selectedScriptName = scriptName;
	}
	public String getScriptText() {
		return selectedScriptText;
	}
	public void setScriptText(String scriptText) {
		this.selectedScriptText = scriptText;
	}
	public String getSessionId() {
		return sessionId;
	}
	public void setSessionId(String sessionId) {
		this.sessionId = sessionId;
	}
	public String getSignalType() {
		return signalType;
	}
	public void setSignalType(String signalType) {
		this.signalType = signalType;
	}
	public int getStatusCode() {
		return statusCode;
	}
	public void setStatusCode(int statusCode) {
		this.statusCode = statusCode;
	}
	public String getErrorResponse() {
		return errorResponse;
	}
	public void setErrorResponse(String errorResponse) {
		this.errorResponse = errorResponse;
	}
	public void setResultStatus(boolean b) {
		this.resultStatus = b;
	}
	public boolean getResultStatus() {
		return this.resultStatus;
	}
	public void setSignalStatus(String status) {
		this.signalStatus = status;
	}
	public String getSessionStatus() {
		return sessionStatus;
	}
	public void setSessionStatus(String sessionStatus) {
		this.sessionStatus = sessionStatus;
	}
	public String getSignalStatus() {
		return signalStatus;
	}
	
	public String getSelectedScriptText() {
		return selectedScriptText;
	}
	public void setSelectedScriptText(String selectedScriptText) {
		this.selectedScriptText = selectedScriptText;
	}
	public String getPhoneNumber() {
		return phoneNumber;
	}
	public void setPhoneNumber(String phoneNumber) {
		this.phoneNumber = phoneNumber;
	}
	public String isSelected(HttpServletRequest request, String param, String value)
	{
		String pvalue =  (String) request.getParameter(param);
		if (pvalue ==  null) return "";
		if (pvalue.equals(value)) return " selected=\"selected\" ";
		return "";
	}
	public void setNumberToDial(String numberToDial) {
		this.numberToDial = numberToDial;
	}
	public String getNumberToDial()
	{
		return this.numberToDial;
	}
	public String getOutboundScriptText() {
		return outboundScriptText;
	}
	public void setOutboundScriptText(String outboundScriptText) {
		this.outboundScriptText = outboundScriptText;
	}
	public void setMessageToDisplay(String messageToDisplay) {
		this.messageToDisplay = messageToDisplay;
	}
	
	public String getMessageToDisplay(){
		return this.messageToDisplay;
	}
}
