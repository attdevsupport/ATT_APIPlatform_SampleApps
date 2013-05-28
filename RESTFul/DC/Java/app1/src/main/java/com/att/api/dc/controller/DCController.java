package com.att.api.dc.controller;

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
import com.att.api.dc.model.ConfigBean;
import com.att.api.dc.model.DCResponse;
import com.att.api.dc.service.DCService;

import java.io.IOException;

public class DCController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig appConfig;
    
    private RESTConfig getRESTConfig(final String endpoint) {
        final String proxyHost = appConfig.getProxyHost(); 
        final int proxyPort = appConfig.getProxyPort(); 
        final boolean trustAllCerts = appConfig.getTrustAllCerts();

        return new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts);
    }

    private OAuthToken getToken(HttpServletRequest request, 
            HttpServletResponse response) throws RESTException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token"); 
        if (token != null && !token.isAccessTokenExpired()) {
            return token;
        }

        final String FQDN = appConfig.getFQDN();
        final String clientId = appConfig.getClientId();
        final String clientSecret = appConfig.getClientSecret(); 

        final String code = (String) request.getParameter("code");
        if (code != null) {
            final String endpoint = FQDN + OAuthService.API_URL; 
            final OAuthService service = new OAuthService(
                    getRESTConfig(endpoint), clientId, clientSecret);
            token = service.getTokenUsingCode(code);
            session.setAttribute("token", token);
            return token;
        }

        final String scope = appConfig.getProperty("scope");
        final String redirectUri = appConfig.getProperty("redirectUri");
        final String redirect = FQDN + "/oauth/authorize?client_id=" + 
            clientId + "&scope=" + scope + "&redirect_uri=" + redirectUri; 

        try {
            response.sendRedirect(redirect);
        } catch (IOException e) {
            throw new RESTException(e);
        }
        return null; // indicate redirection is needed 
    }

    private boolean handleGetDeviceInfo(final HttpServletRequest request,
            final HttpServletResponse response) {
            
        try {
            final String error = (String) request.getParameter("error");
            if (error != null) {
                String errorDesc = request.getParameter("error_description");
                throw new RESTException("error=" + error 
                        + "&error_description=" + errorDesc);
            }

            final OAuthToken token = this.getToken(request, response);
            if (token == null) {
                return true;
            }

            String endpoint = appConfig.getFQDN() + "/rest/2/Devices/Info";

            DCService service = new DCService(getRESTConfig(endpoint), token);
            DCResponse dcResponse = service.getDeviceCapabilities();

            request.setAttribute("dcResponse", dcResponse);
            return false;
        } catch (RESTException e) {
            request.setAttribute("error", e.getMessage());
            return false;
        }
    }

    public void init() {
        try {
            this.appConfig = AppConfig.getInstance();
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {

            final boolean redirect = handleGetDeviceInfo(request, response);
            if (redirect) {
                return;
            }

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
