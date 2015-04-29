/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
import com.att.api.speech.service.SpeechService;

public class SpeechToTextController extends APIController {

    private static final long serialVersionUID = 161244360288778547L;

    private String getPath() {
        return getServletContext().getRealPath("/");
    }

    public void doPost(HttpServletRequest request,
            HttpServletResponse response) throws ServletException, IOException {

        final JSONObject jresponse = new JSONObject();
        try {
        
            final OAuthToken token = this.getFileToken();
            final SpeechService speechSrvc 
                = new SpeechService(appConfig.getApiFQDN(), token);

            final String speechContext = request.getParameter("speechContext");
            final String fname = request.getParameter("speechFile");
            final String xArg = request.getParameter("x_arg");
            final String subContext = request.getParameter("x_subcontext");
            // TODO: finish chunked portion
            // final Boolean chunked = request.getParameter("chunked") != null;

            final Set<String> allowedFiles = new HashSet<String>(Arrays.asList(
                new String[] {
                    "boston_celtics.wav", "california.amr", "coffee.amr",
                    "doctors.wav", "nospeech.wav",
                    "samplerate_conflict_error.wav", "this_is_a_test.spx",
                    "too_many_channels_error.wav", "boston_celtics.wav"
                }
             ));

            if (!allowedFiles.contains(fname)) {
                throw new IllegalArgumentException("Invalid file name");
            }

            final String audioFolder = appConfig.getProperty("audioFolder");
            final File file = new File(getPath() + audioFolder + "/" + fname);
            final SpeechResponse speechResponse = speechSrvc.speechToText(
                file, xArg, speechContext, subContext);
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
