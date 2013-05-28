package com.att.api.tl.controller;

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
import com.att.api.tl.model.ConfigBean;
import com.att.api.tl.model.TLResponse;
import com.att.api.tl.service.TLService;

import java.io.IOException;

public class TLController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig appConfig;

    private void copyToSession(final HttpServletRequest request,
            final String[] names) {

        final HttpSession session = request.getSession();
        for (final String name : names) {
            final String value = (String) request.getParameter(name);
            if (value != null) {
                session.setAttribute(name, value);
            }
        }
    }
    
    private void clearSession(final HttpServletRequest request, 
            final String[] names) {

        final HttpSession session = request.getSession();
        for (final String name : names) {
            session.removeAttribute(name);
        }
    }

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

    private boolean handleGetLocation(final HttpServletRequest request,
            final HttpServletResponse response) {

        final String[] names = {
            "getLocation", "acceptableAccuracy", "requestedAccuracy", "tolerance"};

        copyToSession(request, names);
        HttpSession session = request.getSession();
        /* default values */
        if (session.getAttribute(names[1]) == null) {
            session.setAttribute(names[1], 10000);
        }
        if (session.getAttribute(names[2]) == null) {
            session.setAttribute(names[2], 1000);
        }
        if (session.getAttribute(names[3]) == null) {
            session.setAttribute(names[3], "LowDelay");
        }

        if (session.getAttribute(names[0]) == null) {
            return false;
        }

        try {
            final OAuthToken token = this.getToken(request, response);
            if (token == null) {
                return true;
            }

            String endpoint = appConfig.getFQDN() + "/2/devices/location";
            String acceptableAccuracy = (String) session.getAttribute(names[1]);
            String requestedAccuracy = (String) session.getAttribute(names[2]);
            String tolerance = (String) session.getAttribute(names[3]);


            TLService service = new TLService(getRESTConfig(endpoint), token);
            TLResponse tlResponse = service
                .getLocation(requestedAccuracy, acceptableAccuracy, tolerance);

            request.setAttribute("tlResponse", tlResponse);
            clearSession(request, new String[] { "getLocation" });
            return false;
        } catch (RESTException e) {
            clearSession(request, new String[] { "getLocation" });
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

            final boolean redirect = handleGetLocation(request, response);
            if (redirect) {
                return;
            }

            final String forward = "WEB-INF/TL.jsp";
            request.setAttribute("cfg", new ConfigBean());
            RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
            dispatcher.forward(request, response);
        }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            doPost(request, response);
        }
}
