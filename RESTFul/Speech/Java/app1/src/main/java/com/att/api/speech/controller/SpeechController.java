package com.att.api.speech.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;
import com.att.api.speech.model.ConfigBean;
import com.att.api.speech.model.SpeechResponse;
import com.att.api.speech.service.SpeechService;

import java.io.File;
import java.io.IOException;
import java.util.Arrays;

public class SpeechController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig cfg;

    private String getPath() {
        return getServletContext().getRealPath("/");
    }

    private RESTConfig getRESTConfig(final String endpoint) {
        final String proxyHost = cfg.getProxyHost(); 
        final int proxyPort = cfg.getProxyPort(); 
        final boolean trustAllCerts = cfg.getTrustAllCerts();

        return new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts);
    }

    private OAuthToken getToken() throws RESTException {
        try {
            final AppConfig cfg = AppConfig.getInstance();
            final String path = "WEB-INF/token.properties";
            final String tokenFile = getServletContext().getRealPath(path);

            OAuthToken token = OAuthToken.loadToken(tokenFile);
            if (token == null || token.isAccessTokenExpired()) {
                final String endpoint = cfg.getFQDN() + OAuthService.API_URL;
                final String clientId = cfg.getClientId();
                final String clientSecret = cfg.getClientSecret();
                final OAuthService service = new OAuthService(
                        getRESTConfig(endpoint), clientId, clientSecret);

                token = service.getToken(cfg.getProperty("scope"));
                token.saveToken(tokenFile);
            }
            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    private String[] getFileNames() {
        String audioFolder = cfg.getProperty("audioFolder");

        String dir = getServletContext().getRealPath("/")  + audioFolder;
        
        File[] files = new File(dir).listFiles();
        String[] fnames = new String[files.length];
        for (int i = 0; i < files.length; ++i) {
            fnames[i] = files[i].getName();
        }

        Arrays.sort(fnames);
        return fnames;
    }

    private void handleSpeechToText(HttpServletRequest request) {
        if (request.getParameter("SpeechToText") == null) {
            return;
        }

        try {
            HttpSession session = request.getSession();
            String xarg = cfg.getProperty("xArg");

            String speechContext = request.getParameter("SpeechContext");
            session.setAttribute("sessionContextName", speechContext);

            String x_subContext = request.getParameter("x_subContext");
            session.setAttribute("sessionx_subContext", x_subContext);

            String endpoint = cfg.getFQDN() + "/speech/v3/speechToText";
            SpeechService srvc = new SpeechService(getRESTConfig(endpoint));

            srvc.setChunked(false);
            if (request.getParameter("chkChunked") != null) {
                session.setAttribute("sessionChunked", true); 
                srvc.setChunked(true);
            }


            String fname = request.getParameter("audio_file");
	        session.setAttribute("sessionFileName", fname);
            String audioFolder = cfg.getProperty("audioFolder");
            File file = new File(getPath() + audioFolder + "/" + fname);

            OAuthToken token = getToken();
            SpeechResponse response = srvc.sendRequest(file, 
                    token.getAccessToken(), speechContext, xarg,x_subContext);

            request.setAttribute("resultSpeech", response.getResult());
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }

    public void init() {
        try {
            this.cfg = AppConfig.getInstance();
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleSpeechToText(request);   
    
        String speechContexts = cfg.getProperty("speechContexts");
        request.setAttribute("xArg", cfg.getProperty("xArg", ""));
        request.setAttribute("speechContexts", speechContexts.split(","));
        request.setAttribute("x_subContext", cfg.getProperty("xArgSubContext"));
        request.setAttribute("fnames", this.getFileNames());
        request.setAttribute("cfg", new ConfigBean());

        final String forward = "WEB-INF/Speech.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
