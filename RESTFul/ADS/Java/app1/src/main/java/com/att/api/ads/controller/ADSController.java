package com.att.api.ads.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;

import com.att.api.ads.service.ADSService;
import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import java.io.IOException;

public class ADSController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private RESTConfig getRESTConfig(final String endpoint, 
            final AppConfig acfg) {

        final String proxyHost = acfg.getProxyHost(); 
        final int proxyPort = acfg.getProxyPort(); 
        final boolean trustAllCerts = acfg.getTrustAllCerts();

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
                        getRESTConfig(endpoint, cfg), clientId, clientSecret);

                token = service.getToken(cfg.getProperty("scope"));
                token.saveToken(tokenFile);
            }

            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    private void handleGetAdvertisement(final HttpServletRequest request) {
        if (request.getParameter("btnGetAds") == null) {
            return;
        }

        try {
            final AppConfig cfg = AppConfig.getInstance();
            final OAuthToken token = this.getToken();
            final String endpoint = cfg.getFQDN() + "/rest/1/ads";

            ADSService srvc 
                = new ADSService(getRESTConfig(endpoint, cfg), token);
            String category = request.getParameter("category");
            String uagent = request.getHeader("User-Agent");
            JSONObject result = srvc.getAdvertisement(category, uagent);
            JSONObject response = result.getJSONObject("AdsResponse");
            JSONObject ads = response.getJSONObject("Ads");
            if (ads.has("Type")) {
                request.setAttribute("type", ads.getString("Type"));
            }
            if (ads.has("ClickUrl")) {
                request.setAttribute("clickUrl", ads.getString("ClickUrl"));
            }
            if (ads.has("Text")) {
                request.setAttribute("text", ads.getString("Text"));
            }
            if (ads.has("Content")) {
                request.setAttribute("content", ads.getString("Content"));
            }
            if (ads.has("ImageUrl")) {
                request.setAttribute("image", ads.getJSONObject("ImageUrl")
                        .getString("Image"));
            }
        } catch (Exception e) {
            // no response returned 
            if (e.getMessage().equals("HTTP entity may not be null")) {
                request.setAttribute("error", "No Ads were returned");
            } else {
                request.setAttribute("error", e.getMessage());
            }
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {

            this.handleGetAdvertisement(request);

            final String forward = "WEB-INF/ADS.jsp";
            RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
            dispatcher.forward(request, response);
        }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            doPost(request, response);
        }
}
