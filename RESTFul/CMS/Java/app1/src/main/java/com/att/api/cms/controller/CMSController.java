package com.att.api.cms.controller;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.HashMap;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.cms.model.CallControlResponse;
import com.att.api.cms.model.ConfigBean;
import com.att.api.cms.service.CMSService;
import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

public class CMSController extends HttpServlet {
    private static final long serialVersionUID = 6832764701357295887L;
    private static volatile String scriptContent;

    private CMSService cmsService;
    private AppConfig appConfig;

    private RESTConfig getRESTConfig(final String endpoint) {

        final String proxyHost = appConfig.getProxyHost();
        final int proxyPort = appConfig.getProxyPort();
        final boolean trustAllCerts = appConfig.getTrustAllCerts();
        return (new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts));
    }

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

    private OAuthToken getToken() throws RESTException {
        try {
            final String path = "WEB-INF/token.properties";
            final String tokenFile = getServletContext().getRealPath(path);

            OAuthToken token = OAuthToken.loadToken(tokenFile);
            if (token == null || token.isAccessTokenExpired()) {
                final String endpoint 
                    = appConfig.getFQDN() + OAuthService.API_URL;
                final String clientId = appConfig.getClientId();
                final String clientSecret = appConfig.getClientSecret();
                final OAuthService service = new OAuthService(getRESTConfig(
                            endpoint), clientId, clientSecret);

                token = service.getToken(appConfig.getProperty("scope"));
                token.saveToken(tokenFile);
            }
            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    @Override
    public void init() {
        this.cmsService = new CMSService();
        try {
            this.appConfig = AppConfig.getInstance();
        } catch (IOException ioe) {
            //don't handle, just print stacktrace
            ioe.printStackTrace();
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
                final String url = appConfig.getFQDN() + "/rest/1/Sessions";
                final RESTConfig cfg = getRESTConfig(url);
                final CallControlResponse ccresponse = cmsService.createSession
                    (cfg, buildScriptVariables(command, phoneNumber), getToken());

                final String sessionId = ccresponse.getSessionId();
                if (sessionId != null) {
                    request.getSession().setAttribute("sessionId", sessionId);
                }
                return ccresponse;
            } catch (Exception e) {
                final String errMsg = e.getMessage();
                return CallControlResponse.sessionErrorResponse(errMsg);
            }
        } else if (command.isSendSignal()) {
            try {
                final String sessionId = (String) session.getAttribute("sessionId");

                final String endpoint = "/rest/1/Sessions/" + sessionId
                    + "/Signals";

                final String url = appConfig.getFQDN() + endpoint;
                final RESTConfig cfg = getRESTConfig(url);
                return cmsService.sendSignal(cfg, sessionId,
                        command.getSignalType(), getToken());
            } catch (Exception e) {
                final String errMsg = e.getMessage();
                return CallControlResponse.sessionErrorResponse(errMsg);
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
