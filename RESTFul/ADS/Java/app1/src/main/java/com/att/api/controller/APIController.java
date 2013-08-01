package com.att.api.controller;

import java.io.IOException;

import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

public abstract class APIController extends HttpServlet {
    private static final long serialVersionUID = -343932180266512049L;

    protected AppConfig appConfig;

    protected void copyToSession(final HttpServletRequest request,
            final String[] names) {

        final HttpSession session = request.getSession();
        for (final String name : names) {
            final String value = (String) request.getParameter(name);
            if (value != null) {
                session.setAttribute(name, value);
            }
        }
    }
    
    protected void clearSession(final HttpServletRequest request, 
            final String[] names) {

        final HttpSession session = request.getSession();
        for (final String name : names) {
            session.removeAttribute(name);
        }
    }

    protected OAuthToken getFileToken() throws RESTException {
        try {
            final AppConfig cfg = AppConfig.getInstance();
            final String path = "WEB-INF/token.properties";
            final String tokenFile = getServletContext().getRealPath(path);

            OAuthToken token = OAuthToken.loadToken(tokenFile);
            if (token == null || token.isAccessTokenExpired()) {
                final String clientId = cfg.getClientId();
                final String clientSecret = cfg.getClientSecret();
                final OAuthService service = new OAuthService(
                        appConfig.getOauthFQDN(), clientId, clientSecret);

                token = service.getToken(cfg.getProperty("scope"));
                token.saveToken(tokenFile);
            }

            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    protected OAuthToken getSessionToken(HttpServletRequest request, 
            HttpServletResponse response) throws RESTException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token"); 
        if (token != null && !token.isAccessTokenExpired()) {
            return token;
        }
        
        final String FQDN = appConfig.getOauthFQDN();
        final String clientId = appConfig.getClientId();
        final String clientSecret = appConfig.getClientSecret(); 

        final String code = (String) request.getParameter("code");
        if (code != null) {
            final OAuthService service = new OAuthService(
                    appConfig.getOauthFQDN(), clientId, clientSecret);
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

    @Override
    public void init() {
        try {
            this.appConfig = AppConfig.getInstance();
            boolean shouldTrust = appConfig.getTrustAllCerts();
            String proxyHost = appConfig.getProxyHost();
            int proxyPort = appConfig.getProxyPort();
            RESTConfig.setDefaultTrustAllCerts(shouldTrust);
            RESTConfig.setDefaultProxy(proxyHost, proxyPort);
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }
}
