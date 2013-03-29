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

import java.io.*;
import java.util.Arrays;
import java.util.Hashtable;

public class SpeechController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig cfg;
    private Hashtable<String,String> templateContent;
    private String [] audioFiles;

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

    private String[] getAudioFileNames() {
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
        String speechContext = request.getParameter("SpeechContext");
        request.setAttribute("mimeData", getMimeData());
        request.getSession().setAttribute("sessionmimeData", getMimeData());

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
            HttpSession session = request.getSession();
            String xarg = cfg.getProperty("xArg");

            session.setAttribute("sessionContextName", speechContext);

            String endpoint = cfg.getFQDN() + cfg.getProperty("contextPath");
            SpeechService srvc = new SpeechService(getRESTConfig(endpoint));

            String fname = request.getParameter("audio_file");
	        session.setAttribute("sessionFileName", fname);

            String audioFolder = cfg.getProperty("audioFolder");
            File file = new File(getPath() + audioFolder + "/" + fname);

            String [] attachments = new String[3];
            attachments[0] = new File(getPath()+ "/" + cfg.getProperty("GenericHints.template")).getAbsolutePath();
            attachments[1] = new File(getPath()+ "/" + cfg.getProperty("GrammarList.template")).getAbsolutePath();
            attachments[2] = file.getAbsolutePath();

            String mimeData = this.templateContent.get(speechContext);
            session.setAttribute("sessionmimeData", mimeData);

            OAuthToken token = getToken();
            SpeechResponse response = srvc.sendRequest(attachments,
                    token.getAccessToken(), speechContext, xarg);

            request.setAttribute("resultSpeech", response.getResult());
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("errorSpeech", e.getMessage());
        }
    }

    public void init() {
        try {
            this.cfg = AppConfig.getInstance();
            loadTemplateContent();
            this.audioFiles = this.getAudioFileNames();
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    private String getMimeData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.append("x-dictionary:");
        stringBuilder.append(System.getProperty("line.separator"));
        stringBuilder.append(this.templateContent.get("GenericHints"));
        stringBuilder.append(System.getProperty("line.separator"));
        stringBuilder.append("x-grammar:");
        stringBuilder.append(System.getProperty("line.separator"));
        stringBuilder.append(this.templateContent.get("GrammarList"));
        return stringBuilder.toString();
    }

    private void loadTemplateContent()
    {
        String GrammerList = this.cfg.getProperty("GrammarList.template");
        String GenericHints = this.cfg.getProperty("GenericHints.template");
        this.templateContent = new Hashtable<String, String>();
        this.templateContent.put("GrammarList",readContent(GrammerList));
        this.templateContent.put("GenericHints",readContent(GenericHints));
    }

    private String readContent(String contentFile){

        StringBuilder content = new StringBuilder("");
        String path = getPath()+ "/" + contentFile;
        BufferedReader brdr;
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
        }
        return content.toString();
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleSpeechToText(request);   
    
        String speechContexts = cfg.getProperty("speechContexts");
        request.setAttribute("xArg", cfg.getProperty("xArg", ""));
        request.setAttribute("speechContexts", speechContexts.split(","));
        request.setAttribute("fnames", this.audioFiles);
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
