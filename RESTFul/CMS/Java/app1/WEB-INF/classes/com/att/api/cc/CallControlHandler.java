// 
//Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
//TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
//Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
//For more information contact developer.support@att.com
//

package com.att.api.cc;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.text.ParseException;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.HttpException;
import org.apache.commons.httpclient.methods.PostMethod;
import org.apache.commons.httpclient.methods.StringRequestEntity;
import org.apache.commons.lang3.StringUtils;
import org.json.JSONObject;

import com.att.api.cc.model.CallControlResponse;

/**
 * Call Control Script Handler
 * 
 * @author Sendhil Chokkalingam
 * 
 */
public class CallControlHandler {

	private HttpServletRequest request;
	private ServletContext application;

	private CallControlResponse model;

	private String sessionEndPoint;
	private String scriptPath;
	private String signalEndPoint;

	/**
	 * Initializes the handler with end point,script path information Note:
	 * Signal end point will be dynamically constructed by replacing session id
	 * 
	 * @param request
	 * @param context
	 * @param sessionEndPoint
	 * @param signalEndPoint
	 * @param scriptPath
	 * @param phoneNumber
	 *            phone number
	 */
	public CallControlHandler(HttpServletRequest request,
			ServletContext context, String sessionEndPoint,
			String signalEndPoint, String scriptPath, String phoneNumber) {
		this.request = request;
		this.application = context;
		this.sessionEndPoint = sessionEndPoint;
		this.signalEndPoint = signalEndPoint;
		this.scriptPath = scriptPath;
		this.model = new CallControlResponse();
		this.model.setPhoneNumber(phoneNumber);
	}

	/**
	 * 
	 * @return
	 * @throws HttpException
	 * @throws IOException
	 * @throws ParseException
	 */
	public CallControlResponse processRequest() throws HttpException,
			IOException, ParseException {
		parseInputParams(request);
		String accessToken = (String) request.getSession().getAttribute(
				"accessToken");
		if (accessToken == null || accessToken.length() == 0) {
			model.setResultStatus(false);
			model.setStatusCode((Integer) request.getSession().getAttribute(
					"statusCode"));
			model.setErrorResponse((String) request.getSession().getAttribute(
					"errorResponse"));
			model.setOutboundScriptText(readScriptContent(CallControlResponse.SCRIPT));
			return model;
		}
		if (model.isCreateSession()) {
			createSession(accessToken);
		} else if (model.isSendSignal()) {
			sendSignal(model.getSignalType(), accessToken);
		} else {
			model.setResultStatus(true);
		}
		model.setOutboundScriptText(readScriptContent(CallControlResponse.SCRIPT));
		return model;
	}

	/**
	 * Parse the input params
	 * 
	 * @param request
	 */
	private void parseInputParams(HttpServletRequest request) {
		model.setCreateSession(StringUtils.isNotEmpty((String) request
				.getParameter("btnCreateSession")));
		model.setSendSignal(StringUtils.isNotEmpty((String) request
				.getParameter("btnSendSignal")));
		model.setSelectedScriptName((String) request
				.getParameter("lstTemplate"));
		model.setSignalType((String) request.getParameter("ddlSignal"));
		model.setNumberToDial((String) request.getParameter("txtNumberToDial"));
		model.setMessageToPlay((String) request
				.getParameter("txtMessageToPlay"));
		model.setFeaturedNumber((String) request
				.getParameter("txtNumberForFeature"));
	}

	/**
	 * Read script content from file system to populate the form
	 * 
	 * @param resource
	 * @return
	 * @throws IOException
	 */
	private String readScriptContent(String resource) throws IOException {
		StringBuilder sbuilder = new StringBuilder();
		String resourcePath = scriptPath + resource;
		BufferedReader bis = new BufferedReader(new InputStreamReader(
				new FileInputStream(application.getRealPath(resourcePath))));
		String line = null;
		while ((line = bis.readLine()) != null) {
			sbuilder.append(line);
		}
		bis.close();
		return sbuilder.toString();
	}

	/**
	 * Create the Call Control session, upon success retrieve and set the
	 * session id in Session object
	 * 
	 * @param accessToken
	 * @throws HttpException
	 * @throws IOException
	 * @throws ParseException
	 */
	private void createSession(String accessToken) throws HttpException,
			IOException, ParseException {

		JSONObject scriptVars = new JSONObject();
		if (model.getPhoneNumber().length() >0)
			scriptVars.put("smsCallerID", model.getPhoneNumber());
		if (model.getSelectedScriptName().length() > 0)
			scriptVars.put("feature", model.getSelectedScriptName());
		if (model.getNumberToDial().length() > 0)
			scriptVars.put("numberToDial", model.getNumberToDial());
		if (model.getMessageToPlay().length() > 0)
			scriptVars.put("messageToPlay", model.getMessageToPlay());
		if (model.getFeaturedNumber().length() > 0)
			scriptVars.put("featurenumber", model.getFeaturedNumber());
		String variables = scriptVars.toString();
		PostMethod method = null;
		try {
			method = sendReceive(sessionEndPoint, accessToken, variables);
			int statusCode = method.getStatusCode();
			if (statusCode == 200) {
				model.setResultStatus(true);
				model.setStatusCode(200);
				model.setErrorResponse(null);
				JSONObject jsonResponse;
				jsonResponse = new JSONObject(method.getResponseBodyAsString());
				String success = jsonResponse.getString("success");
				if (success.equals("true")) {
					String id = jsonResponse.getString("id");
					model.setSessionId(id);
					request.getSession().setAttribute("sessionId", id);
				}
			} else {
				model.setResultStatus(false);
				String errorResponse = method.getResponseBodyAsString();
				if (errorResponse == null) {
					errorResponse = method.getStatusText();
				}
				model.setErrorResponse(errorResponse);
				model.setStatusCode(statusCode);
			}
		} finally {
			method.releaseConnection();
		}
	}

	/**
	 * Send Signal to active session
	 * 
	 * @param signal
	 * @throws HttpException
	 * @throws IOException
	 * @throws ParseException
	 */
	private void sendSignal(String signal, String accessToken)
			throws HttpException, IOException, ParseException {
		String sessionId = (String) request.getSession().getAttribute(
				"sessionId");
		if (sessionId == null) {
			model.setResultStatus(false);
			model.setErrorResponse("Please create a session and then send signal");
			model.setStatusCode(0);
			return;
		}

		model.setSessionId(sessionId);
		JSONObject msgBody = new JSONObject();
		msgBody.put("signal", signal);
		signalEndPoint = signalEndPoint.replace("{sessionid}", sessionId);
		PostMethod method = null;
		try {
			method = sendReceive(signalEndPoint, accessToken,
					msgBody.toString());
			int statusCode = method.getStatusCode();
			if (statusCode == 200) {
				model.setResultStatus(true);
				model.setStatusCode(200);
				model.setErrorResponse(null);
			} else {
				model.setResultStatus(false);
				String errorResponse = method.getResponseBodyAsString();
				if (errorResponse == null) {
					errorResponse = method.getStatusText();
				}
				model.setErrorResponse(errorResponse);
				model.setStatusCode(statusCode);
			}
			if (model.getResultStatus()) {
				JSONObject jsonResponse;
				jsonResponse = new JSONObject(method.getResponseBodyAsString());
				String status = jsonResponse.getString("status");
				model.setSignalStatus(status);
			}
		} finally {
			method.releaseConnection();
		}
	}

	/**
	 * 
	 * @param endpointURI
	 * @param accessToken
	 * @param body
	 * @return
	 * @throws HttpException
	 * @throws IOException
	 */
	private PostMethod sendReceive(String endpointURI, String accessToken,
			String body) throws HttpException, IOException {
		// Initialize the client
		HttpClient client = new HttpClient();
		PostMethod method = new PostMethod(endpointURI);
		// setup request parameters
		method.addRequestHeader("Accept", "application/json");
		method.addRequestHeader("Content-Type", "application/json");
		method.addRequestHeader("Authorization", "Bearer " + accessToken);
		method.addRequestHeader("User-Agent", request.getHeader("user-agent"));
		if (body != null && body.length() > 0)
			method.setRequestEntity(new StringRequestEntity(body));
		client.executeMethod(method);
		return method;
	}
}
