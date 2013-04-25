package com.att.api.speech.controller;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.FileUtils;

import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;
import com.att.api.speech.model.ConfigBean;
import com.att.api.speech.service.SpeechService;

public class SpeechController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig cfg;
    private static String PLAIN_TEXT, SSML;

    private RESTConfig getRESTConfig(final String endpoint) {
        final String proxyHost = cfg.getProxyHost();
        final int proxyPort = cfg.getProxyPort();
        final boolean trustAllCerts = cfg.getTrustAllCerts();

        return new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts);
    }

    private OAuthToken getToken() throws RESTException {
        try {
            final AppConfig config = AppConfig.getInstance();
            final String path = "WEB-INF/token.properties";
            final String tokenFile = getServletContext().getRealPath(path);

            OAuthToken token = OAuthToken.loadToken(tokenFile);
            if (token == null || token.isAccessTokenExpired()) {
                final String endpoint = config.getFQDN() + OAuthService.API_URL;
                final String clientId = config.getClientId();
                final String clientSecret = config.getClientSecret();
                final OAuthService service = new OAuthService(
                        getRESTConfig(endpoint), clientId, clientSecret);

                token = service.getToken(config.getProperty("scope"));
                token.saveToken(tokenFile);
            }
            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    private void handleTextToSpeech(HttpServletRequest request) {
        if (request.getParameter("TextToSpeech") == null) {
            return;
        }

        try {
            HttpSession session = request.getSession();
            String xarg = cfg.getProperty("xArg");

            String contentType = request.getParameter("contentType");
            session.setAttribute("sessionContentType", contentType);

            String speechText = null;

            if (contentType.equalsIgnoreCase("text/plain")) {
                speechText = PLAIN_TEXT; 
            } else{
                speechText = SSML;
            }

            session.setAttribute("speechText", speechText);

            String endpoint = cfg.getFQDN() + "/speech/v3/textToSpeech";
            SpeechService srvc = new SpeechService(getRESTConfig(endpoint));

            OAuthToken token = getToken();
            byte[] wavBytes = srvc.sendRequest(token.getAccessToken(),
                    contentType, speechText, xarg);

            request.setAttribute("encodedWavBytes",
                    new String(Base64.encodeBase64(wavBytes)));
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }

    private String readFile(String path) {
        InputStream is = SpeechController.class.getClassLoader().getResourceAsStream(path);

        byte[] buf = new byte[1024];
        String str = "";
        int count = 0;

        try {
            while (-1 != (count = is.read(buf)))
                str += new String(buf,0, count);
        } catch (IOException e) {
            return e.getMessage();
        }

        return str;
    }

    @Override
    public void init() {
        try {
            this.cfg = AppConfig.getInstance();
            PLAIN_TEXT = readFile(cfg.getProperty("plainTextFile"));
            SSML = readFile(cfg.getProperty("ssmlFile"));
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    @Override
    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleTextToSpeech(request);   

        String contentTypes = cfg.getProperty("contentTypes");
        request.setAttribute("xArg", cfg.getProperty("xArg", ""));
        request.setAttribute("contentTypes", contentTypes.split(","));
        request.setAttribute("textContent", PLAIN_TEXT);
        request.setAttribute("ssmlContent", SSML);
        request.setAttribute("cfg", new ConfigBean());

        final String forward = "WEB-INF/Speech.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    @Override
    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
