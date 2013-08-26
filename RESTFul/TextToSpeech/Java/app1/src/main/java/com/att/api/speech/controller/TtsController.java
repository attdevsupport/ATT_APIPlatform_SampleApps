package com.att.api.speech.controller;

import java.io.IOException;
import java.io.InputStream;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.commons.codec.binary.Base64;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.speech.model.ConfigBean;
import com.att.api.speech.service.TtsService;

public class TtsController extends APIController {
    private static final long serialVersionUID = 1L;
    private static String PLAIN_TEXT, SSML;

    private void handleTextToSpeech(HttpServletRequest request) {
        if (request.getParameter("TextToSpeech") == null) {
            return;
        }

        try {
            final HttpSession session = request.getSession();
            final OAuthToken token = getFileToken();
            String xarg = appConfig.getProperty("xArg");

            String contentType = request.getParameter("contentType");
            session.setAttribute("sessionContentType", contentType);

            String speechText = null;

            if (contentType.equalsIgnoreCase("text/plain")) {
                speechText = PLAIN_TEXT;
            } else {
                speechText = SSML;
            }

            session.setAttribute("speechText", speechText);

            TtsService srvc = new TtsService(appConfig.getApiFQDN(), token);

            byte[] wavBytes = srvc.sendRequest(contentType, speechText, xarg);

            request.setAttribute("encodedWavBytes",
                    new String(Base64.encodeBase64(wavBytes)));
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }

    private String readFile(String path) throws IOException {
        InputStream is = TtsController.class.getClassLoader().getResourceAsStream(path);

        byte[] buf = new byte[1024];
        String str = "";
        int count = 0;

        while (-1 != (count = is.read(buf)))
            str += new String(buf,0, count);

        return str;
    }

    @Override
    public void init() {
        super.init();
        try {
            PLAIN_TEXT = readFile(appConfig.getProperty("plainTextFile"));
            SSML = readFile(appConfig.getProperty("ssmlFile"));
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    @Override
    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleTextToSpeech(request);   

        String contentTypes = appConfig.getProperty("contentTypes");
        request.setAttribute("xArg", appConfig.getProperty("xArg", ""));
        request.setAttribute("contentTypes", contentTypes.split(","));
        request.setAttribute("textContent", PLAIN_TEXT);
        request.setAttribute("ssmlContent", SSML);
        request.setAttribute("cfg", new ConfigBean());

        final String forward = "WEB-INF/tts.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    @Override
    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
