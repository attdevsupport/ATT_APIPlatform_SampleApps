package com.att.api.ads.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONObject;

import com.att.api.ads.service.ADSService;
import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import java.io.IOException;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

public class ADSController extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private static final String[] categories = { "auto", "business", "chat",
        "communication", "community", "entertainment", "finance", "games",
        "health", "local", "maps", "medical", "movies", "music", "news",
        "other", "personals", "photos", "social", "sports", "technology",
        "tools", "travel", "tv", "video", "weather" };

    private static final String[] MMASizes = { "", "120 x 20", "168 x 28", 
        "216 x 36", "300 x 50", "300 x 250", "320 x 50" };

    private static final String[] ageGroups = { "", "1-13", "14-25", "26-35",
        "36-55", "55-100" };

    private static final String[] premKeys = { "", "0", "1", "2" };
    private static final String[] premVals = { "", "NonPremium", "PremiumOnly",
        "Both" };

    private static final String[] genderKeys = { "", "M", "F" };
    private static final String[] genderVals = { "", "Male", "Female" };

    private static final String[] over18Keys = { "", "0", "2", "3" };
    private static final String[] over18Vals = { "", "Deny Over 18", 
        "Only Over 18", "Allow All Ads" };


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

            String[] inputs = { "MMA", "ageGroup", "Premium", "gender",
                    "over18", "zipCode", "city", "areaCode", "country", 
                    "latitude", "longitude", "keywords" };

            String[] keys = { "MMASize", "AgeGroup", "Premium", "Gender", "Over18",
                    "ZipCode", "City", "AreaCode", "Country", "Latitude", 
                    "Longitude", "Keywords" };

            for (int i = 0; i < inputs.length; ++i) {


            }
            
            // optional values
            Map<String, String> optVals = new HashMap<String, String>();


            JSONObject result = srvc.getAdvertisement(category, uagent, optVals);
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

            request.setAttribute("resultAds", true);
        } catch (RESTException re) {
            // check for no ads
            if (re.getStatusCode() == 204) {
                request.setAttribute("noAds", "No Ads were returned");
                request.setAttribute("resultAds", true);
            } else {
                request.setAttribute("error", re.getMessage());
            }
        } catch (Exception e) {
                request.setAttribute("error", e.getMessage());
        }
    }

    private void setInputs(HttpServletRequest request) {
        request.setAttribute("categories", categories);
        request.setAttribute("mmaSizes", MMASizes);
        request.setAttribute("ageGroups", ageGroups);

        LinkedHashMap<String, String> prems 
            = new LinkedHashMap<String, String>();
        for (int i = 0; i < premKeys.length; ++i) {
            prems.put(premKeys[i], premVals[i]);
        }
        request.setAttribute("premiums", prems);

        LinkedHashMap<String, String> genders
            = new LinkedHashMap<String, String>();
        for (int i = 0; i < genderKeys.length; ++i) {
            genders.put(genderKeys[i], genderVals[i]);
        }
        request.setAttribute("genders", genders);

        LinkedHashMap<String, String> over18 
            = new LinkedHashMap<String, String>();
        for (int i = 0; i < over18Keys.length; ++i) {
            over18.put(over18Keys[i], over18Vals[i]);
        }
        request.setAttribute("over18s", over18);
    }

    private void saveSession(HttpServletRequest request) {
        HttpSession session = request.getSession();
        String[] inputs = { "category", "MMA", "ageGroup", "Premium", "gender",
            "over18", "zipCode", "city", "areaCode", "country", "latitude",
            "longitude", "keywords" };
        for (String s : inputs) {
            if (request.getParameter(s) != null) {
                session.setAttribute(s, request.getParameter(s));
            }
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleGetAdvertisement(request);
        this.setInputs(request);
        this.saveSession(request);

        final String forward = "WEB-INF/ADS.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);

    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            doPost(request, response);
        }
}
