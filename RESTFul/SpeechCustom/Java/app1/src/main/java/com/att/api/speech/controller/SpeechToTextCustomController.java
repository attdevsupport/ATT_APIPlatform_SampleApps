package com.att.api.speech.controller;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.speech.model.NBest;
import com.att.api.speech.model.SpeechResponse;
import com.att.api.speech.service.SpeechCustomService;

public class SpeechToTextCustomController extends APIController {
    private static final long serialVersionUID = 1L;

    private String getPath() {
        return getServletContext().getRealPath("/");
    }

    public void doPost(HttpServletRequest request, 
            HttpServletResponse response) throws ServletException, IOException {
        final JSONObject jresponse = new JSONObject();
        try {
        
            final OAuthToken token = this.getFileToken();
            SpeechCustomService srvc 
                = new SpeechCustomService(appConfig.getApiFQDN(), token);

            final String fname = request.getParameter("audioFile");
            final Set<String> allowedFiles = new HashSet<String>(Arrays.asList(
                new String[] { "pizza-en-US.wav" }
            ));
            if (!allowedFiles.contains(fname)) {
                throw new IllegalArgumentException("Invalid file name");
            }
            final String speechContext = request.getParameter("speechContext");
            final String xarg = request.getParameter("x_arg");
            final String audioFolder = appConfig.getProperty("audioFolder");
            final File audioFile = new File(getPath() + audioFolder + "/" 
                    + fname);
            final File grammar = new File(getPath()+ "/"
                    + appConfig.getProperty("GrammarList.template"));
            final File dictionary = new File(getPath() + "/" 
                    + appConfig.getProperty("GenericHints.template"));

            final SpeechResponse speechResponse = srvc.speechToText(audioFile,
                grammar, dictionary, speechContext, xarg);

            final List<NBest> nbests = speechResponse.getNbests();
            final JSONArray jheaders = new JSONArray();
            final JSONArray jvalues = new JSONArray();
            if (nbests.size() > 0) {
                    jheaders.put("ResponseId").put("Status").put("Hypothesis")
                    .put("LanguageId").put("Confidence").put("Grade")
                    .put("ResultText").put("Words").put("WordScores");
                
                for (final NBest nb : nbests) {
                    final JSONArray jvalue = new JSONArray()
                    .put(speechResponse.getResponseId())
                    .put(speechResponse.getStatus())
                    .put(nb.getHypothesis())  
                    .put(nb.getLanguageId())
                    .put(nb.getConfidence())
                    .put(nb.getGrade())
                    .put(nb.getResultText())
                    .put(nb.getWords())
                    .put(nb.getWordScores());
                    jvalues.put(jvalue);
                }
            } else {
                jheaders.put("ResponseId").put("Status");
                final JSONArray jvalue = new JSONArray()
                .put(speechResponse.getResponseId())
                .put(speechResponse.getStatus());
                jvalues.put(jvalue);
            }
            final JSONObject jtable = new JSONObject()
                .put("caption", "Speech Response:")
                .put("headers", jheaders)
                .put("values", jvalues);
            final JSONArray jtables = new JSONArray().put(jtable);

            jresponse.put("success", true).put("tables", jtables);
        } catch (Exception e) {
            jresponse.put("success", false).put("text", e.getMessage());
        }

        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();
        writer.print(jresponse);
        writer.flush();
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
