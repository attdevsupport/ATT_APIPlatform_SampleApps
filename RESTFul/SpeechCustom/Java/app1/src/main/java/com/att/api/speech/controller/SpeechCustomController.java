package com.att.api.speech.controller;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.Arrays;
import java.util.Hashtable;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.speech.model.ConfigBean;
import com.att.api.speech.model.SpeechResponse;
import com.att.api.speech.service.SpeechCustomService;

public class SpeechCustomController extends APIController {
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
        HttpSession session = request.getSession();
        String speechContext = request.getParameter("SpeechContext");
        request.setAttribute("mimeData", getMimeData());
        session.setAttribute("sessionmimeData", getMimeData());

        if (request.getParameter("SpeechToText") == null) {
            if (speechContext != null)
            {
                request.getSession().setAttribute("sessionContextName", speechContext);
            }
            else {
                request.getSession().setAttribute("sessionContextName", "GenericHints");
            }
            return;
        }


        try {
            final OAuthToken token = getFileToken();
            String xarg = appConfig.getProperty("xArg");

            session.setAttribute("sessionContextName", speechContext);

            SpeechCustomService srvc = new SpeechCustomService(appConfig.getApiFQDN(), token);

            String fname = request.getParameter("audio_file");
            session.setAttribute("sessionFileName", fname);

            String audioFolder = appConfig.getProperty("audioFolder");
            File file = new File(getPath() + audioFolder + "/" + fname);

            String [] attachments = new String[3];
            attachments[0] = new File(getPath() + "/" + appConfig.getProperty("GenericHints.template")).getAbsolutePath();
            attachments[1] = new File(getPath()+ "/" + appConfig.getProperty("GrammarList.template")).getAbsolutePath();
            attachments[2] = file.getAbsolutePath();

            SpeechResponse response = srvc.sendRequest(attachments,
                     speechContext, xarg);

            request.setAttribute("resultSpeech", response.getResult());
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }


    private String getMimeData()
    {
        String GrammarList = this.appConfig.getProperty("GrammarList.template");
        String GenericHints = this.appConfig.getProperty("GenericHints.template");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.append("x-dictionary:\n");
        stringBuilder.append(readContent(GenericHints));
        stringBuilder.append("\nx-grammar:\n");
        stringBuilder.append(readContent(GrammarList));
        return stringBuilder.toString();
    }

    private String readContent(String contentFile){
        StringBuilder content = new StringBuilder("");
        String path = getPath()+ "/" + contentFile;
        BufferedReader brdr = null;
        try {
            brdr = new BufferedReader(new FileReader(path));
            String line = null;
            while ((line = brdr.readLine())!=null){
              content.append(line);
            }
        } catch (FileNotFoundException e) {
          content.append("Content not found");
        } catch (IOException e) {
            content.append("N/A");
        } finally {
            if (brdr != null)
                try {
                    brdr.close();
                } catch (IOException e) {
                    //ignore for now
                }
        }
        return content.toString();
    }

    public void doPost(HttpServletRequest request, 
            HttpServletResponse response) throws ServletException, IOException {

        this.handleSpeechToText(request);   
    
        String speechContexts = appConfig.getProperty("speechContexts");
        request.setAttribute("xArg", appConfig.getProperty("xArg", ""));
        request.setAttribute("speechContexts", speechContexts.split(","));
        request.setAttribute("fnames", this.getFileNames());
        request.setAttribute("cfg", new ConfigBean());

        final String forward = "WEB-INF/SpeechCustom.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
