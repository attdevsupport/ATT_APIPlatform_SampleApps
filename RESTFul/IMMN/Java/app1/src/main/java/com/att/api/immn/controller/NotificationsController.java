package com.att.api.immn.controller;

import java.io.BufferedReader;
import java.io.IOException;
import java.text.ParseException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONException;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.model.FileHandler;
import com.att.api.immn.model.IAMFileHandler;

public class NotificationsController extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws IOException {
        final FileHandler fhandler = IAMFileHandler.getInstance();
        final String content_type = request.getContentType();
        StringBuilder sb = new StringBuilder();
        BufferedReader reader = null;
        try {
            reader = request.getReader();
            String line;
            while ((line = reader.readLine()) != null) {
                sb.append(line).append('\n');
            }
        } finally {
            if (reader != null)
                reader.close();
        }
        if (content_type.equals("application/json")) {
            try {
                fhandler.addObj(new JSONObject(sb.toString()));
            } catch (JSONException e) {
                throw(new IOException(e));
            } catch (ParseException e) {
                throw(new IOException(e));
            }
        }
    }
}
