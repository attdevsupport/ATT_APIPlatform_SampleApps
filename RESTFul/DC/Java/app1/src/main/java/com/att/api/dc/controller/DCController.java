package com.att.api.dc.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;
import com.att.api.dc.model.ConfigBean;
import com.att.api.dc.model.DCResponse;
import com.att.api.dc.service.DCService;

import java.io.IOException;

public class DCController extends APIController {
    private static final long serialVersionUID = 1L;

    private void handleGetDeviceInfo(HttpServletRequest request,
            HttpServletResponse response, OAuthToken token) {
            
        try {
            final String error = (String) request.getParameter("error");
            if (error != null) {
                String errorDesc = request.getParameter("error_description");
                throw new RESTException("error=" + error 
                        + "&error_description=" + errorDesc);
            }

            DCService service = new DCService(appConfig.getFQDN(), token);
            DCResponse dcResponse = service.getDeviceCapabilities();

            request.setAttribute("dcResponse", dcResponse);
        } catch (RESTException e) {
            request.setAttribute("error", e.getMessage());
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {

            OAuthToken token = null;
            try {
                token = this.getSessionToken(request, response);
                if (token == null) {
                    return;
                }
            } catch (RESTException e) {
                request.setAttribute("error", e.getMessage());
            }

            handleGetDeviceInfo(request, response, token);

            final String forward = "WEB-INF/DC.jsp";
            request.setAttribute("cfg", new ConfigBean());
            RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
            dispatcher.forward(request, response);
        }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            doPost(request, response);
        }
}
