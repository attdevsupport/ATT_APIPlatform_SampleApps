package com.att.api.cms.controller;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.HashMap;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.cms.model.CallControlResponse;
import com.att.api.cms.model.ConfigBean;
import com.att.api.cms.service.CMSService;
import com.att.api.cms.service.CMSSessionResponse;
import com.att.api.controller.APIController;

public class CMSController extends APIController {
    private static final long serialVersionUID = 6832764701357295887L;
    private static volatile String scriptContent;

    private String getScriptContent(final String localPath) throws IOException {
        if (scriptContent != null)
            return scriptContent;

        final String fullResourcePath = getServletContext().getRealPath(
                localPath);

        BufferedReader bis = null;
        try {
            StringBuilder sbuilder = new StringBuilder();
            bis = new BufferedReader(new InputStreamReader(new FileInputStream(
                    fullResourcePath)));
            String line = null;
            while ((line = bis.readLine()) != null) {
                sbuilder.append(line);
            }
            scriptContent = sbuilder.toString();
            return sbuilder.toString();
        } catch (IOException ioe) {
            throw ioe; // pass along exception, but close reader first
        } finally {
            if (bis != null)
                bis.close();
        }
    }

    private HashMap<String, String> buildScriptVariables(CMSCommand command,
            String phoneNumber) {

        final HashMap<String, String> scriptVars = new HashMap<String, String>();
        final String[] variables = { phoneNumber,
                command.getSelectedScriptName(), command.getNumberToDial(),
                command.getMessageToPlay(), command.getFeaturedNumber() };
        final String[] names = { "smsCallerID", "feature", "numberToDial",
                "messageToPlay", "featurenumber" };

        for (int i = 0; i < variables.length; ++i) {
            if (variables[i].length() > 0) {
                scriptVars.put(names[i], variables[i]);
            }
        }
        return scriptVars;
    }

    private CallControlResponse processRequest(HttpServletRequest request)
            throws IOException {

        final HttpSession session = request.getSession();
        final String phoneNumber = appConfig.getProperty("number");
        final CMSCommand command = new CMSCommand(request);

        final String[] vars = { command.getFeaturedNumber(),
            command.getMessageToPlay(), command.getNumberToDial(),
            command.getSelectedScriptName() };

        final String[] names = {"featuredNumb", "msgToPlay", "numbToDial",
            "scriptName"};
        for (int i = 0; i < vars.length; ++i) {
            if (vars[i] != null) {
                session.setAttribute(names[i], vars[i]);
            }
        }

        if (command.isCreateSession()) {
            try {
                CMSService cmsService = new CMSService(appConfig.getFQDN(), getFileToken());
                HashMap<String, String> svars = buildScriptVariables(command, phoneNumber);
                CMSSessionResponse csr = cmsService.createSession(svars);
                final String sessionId = csr.getId();
                if (sessionId != null) {
                    request.getSession().setAttribute("sessionId", sessionId);
                }
                return CallControlResponse
                    .sessionIdResponse(sessionId, csr.getSuccess());
            } catch (Exception e) {
                final String errMsg = e.getMessage();
                return CallControlResponse.sessionErrorResponse(errMsg);
            }
        } else if (command.isSendSignal()) {
            try {
                final String sessionId 
                    = (String) session.getAttribute("sessionId");

                CMSService cmsService = new CMSService(appConfig.getFQDN(), getFileToken());
                String status = cmsService.sendSignal(sessionId, command.getSignalType());
                return CallControlResponse.signalStatusResponse(status);
            } catch (Exception e) {
                final String errMsg = e.getMessage();
                return CallControlResponse.signalErrorResponse(errMsg);
            }
        }

        return CallControlResponse.noResponse();
    }

    private void forwardRequest(final HttpServletRequest request,
            final HttpServletResponse response, final CallControlResponse model)
            throws IOException, ServletException {

        request.setAttribute("model", model);
        ConfigBean cfgBean = new ConfigBean();
        final String resourcePath = appConfig.getProperty("resourcePath");
        cfgBean.setScriptContent(getScriptContent(resourcePath));
        request.setAttribute("cfg", cfgBean);

        // forward the request to CMS.jsp.
        String nextJSP = "/WEB-INF/CMS.jsp";
        RequestDispatcher dispatcher = getServletContext()
                .getRequestDispatcher(nextJSP);
        dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws IOException, ServletException {
        CallControlResponse model = processRequest(request);

        forwardRequest(request, response, model);
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws IOException, ServletException {

        doGet(request, response);
    }
}
