package com.att.api.speech.controller;

import java.io.File;
import java.io.IOException;
import java.util.Arrays;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.speech.model.ConfigBean;
import com.att.api.speech.model.SpeechResponse;
import com.att.api.speech.service.SpeechService;

public class SpeechController extends APIController {
    private static final long serialVersionUID = 1L;

    private String getPath() {
        return getServletContext().getRealPath("/");
    }

    private String[] getFileNames() {
        String audioFolder = appConfig.getProperty("audioFolder");

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
            final OAuthToken token = getFileToken();
            HttpSession session = request.getSession();
            String xarg = appConfig.getProperty("xArg");

            String speechContext = request.getParameter("SpeechContext");
            session.setAttribute("sessionContextName", speechContext);

            String x_subContext = request.getParameter("x_subContext");
            session.setAttribute("sessionx_subContext", x_subContext);

            SpeechService srvc = new SpeechService(appConfig.getApiFQDN(), token);

            srvc.setChunked(false);
            if (request.getParameter("chkChunked") != null) {
                session.setAttribute("sessionChunked", true); 
                srvc.setChunked(true);
            }


            String fname = request.getParameter("audio_file");
	        session.setAttribute("sessionFileName", fname);
            String audioFolder = appConfig.getProperty("audioFolder");
            File file = new File(getPath() + audioFolder + "/" + fname);

            SpeechResponse response = srvc.speechToText(file, xarg, 
                    speechContext, x_subContext);

            request.setAttribute("resultSpeech", response);
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleSpeechToText(request);   
    
        String speechContexts = appConfig.getProperty("speechContexts");
        request.setAttribute("xArg", appConfig.getProperty("xArg", ""));
        request.setAttribute("speechContexts", speechContexts.split(","));
        request.setAttribute("x_subContext", appConfig.getProperty("xArgSubContext"));
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
