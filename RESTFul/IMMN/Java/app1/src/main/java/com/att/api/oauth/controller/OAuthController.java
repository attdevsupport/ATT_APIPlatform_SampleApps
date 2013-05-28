package com.att.api.oauth.controller;

import com.att.api.immn.controller.Config;
import com.att.api.immn.model.IMMNResponse;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import org.apache.commons.collections.map.MultiValueMap;
import org.json.JSONObject;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import java.io.IOException;
import java.net.URLDecoder;
import java.text.ParseException;

/**
 * Created with IntelliJ IDEA.
 * User: sendhilc
 * Date: 1/22/13
 * Time: 7:39 PM
 * To change this template use File | Settings | File Templates.
 */
public class OAuthController extends HttpServlet {

    private void forwardResponse(IMMNResponse immnResponse,
            HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        if (immnResponse != null) {
            request.setAttribute("response", immnResponse);
        }
        //todo read this redirect uri from config so that we can use this controller for consent requests for other sample apps
        RequestDispatcher dispatcher = request
            .getRequestDispatcher("/IMMN.jsp");
        dispatcher.forward(request, response);
    }

    /**
     * Handles a GET request.
     *
     * @throws java.io.IOException
     * @throws javax.servlet.ServletException
     * @see HttpServlet
     */
    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        Config cfg = (Config) request.getSession().getAttribute("cfg");
        if (cfg == null) {
            forwardResponse(new IMMNResponse("No configuration supplied"),
                    request, response);
            return;
        }
        if (cfg.getError() != null) {
            forwardResponse(new IMMNResponse(cfg.getError()), request,
                    response);
            return;
        }

        HttpSession session = request.getSession();
        String accessTokenError = "";
        String code = request.getParameter("code");
        if(code==null) code="";

        String refreshToken = request.getParameter("refreshToken");
        if (refreshToken==null)
            refreshToken=(String) session.getAttribute("refreshToken");
        if (refreshToken==null)
            refreshToken="";
        String getRefreshToken = request.getParameter("getRefreshToken");
        if (getRefreshToken==null) getRefreshToken="";
        //if client id is valid we should now have the code parameter on the url. 
        //Use the code get the access token and set the token in session
        if(!code.equals(""))
        {
            APIResponse apiResponse = getToken(cfg, code,"authorization_code",null);
            System.out.println(apiResponse.getResponseBody());

            if(apiResponse.getStatusCode() ==200)
            {
                JSONObject rpcObject = null;
                try {
                    rpcObject = new JSONObject(apiResponse.getResponseBody());
                } catch (ParseException e) {
                    session.setAttribute("accessTokenError",e.getMessage());
                }
                String accessToken = rpcObject.getString("access_token");

                refreshToken = rpcObject.getString("refresh_token");
                session.setAttribute("refreshToken", refreshToken);
                session.setAttribute("accessToken", accessToken);
            }
            else
            {
                accessTokenError = apiResponse.getResponseBody();
                if (accessTokenError == null || accessTokenError.length() == 0) accessTokenError = "" + apiResponse.getStatusCode();
                session.setAttribute("accessTokenError",accessTokenError);
            }
        }
        //Refresh token scenario
        else if(!getRefreshToken.equals(""))
        {
            APIResponse apiResponse = getToken(cfg, code,"refresh_token",refreshToken);
            System.out.println(apiResponse.getResponseBody());
            if(apiResponse.getStatusCode() ==200)
            {
                String accessToken = apiResponse.getResponseBody().substring(18, 50);
                session.setAttribute("accessToken", accessToken);
            }
        }
        else if (request.getParameter("error") != null)
        {
            String error = request.getParameter("error");
            String errorResponse = "";
            String errorDescription = request.getParameter("error_description");
            if (error != null )
            {
                errorResponse = URLDecoder.decode(request.getQueryString(), "iso-8859-1");
            }
            else if (errorDescription != null && errorDescription.length() != 0)
            {
                errorResponse = errorDescription;
            }
            session.setAttribute("accessTokenError", errorResponse);
        }
        request.getRequestDispatcher(cfg.postOAuthUri).forward(request,response);
        //todo we should expose oauth response(success,error etc) in oauthresponse bean
    }

    private APIResponse getToken(Config cfg, String code, String grantType, String refreshToken) {
        //todo use the oauth service
        RESTClient restClient = new RESTClient(cfg.trustAllCerts,cfg.proxyHost,cfg.proxyPort);
        MultiValueMap headers = new MultiValueMap();
        headers.put("Accept", "application/json");
        headers.put("Content-Type", "application/x-www-form-urlencoded");

        MultiValueMap nameValuePairs = new MultiValueMap();
        nameValuePairs.put("client_id",cfg.clientIdAuth);
        nameValuePairs.put("client_secret",cfg.clientSecretAuth);
        nameValuePairs.put("grant_type",grantType);
        if (refreshToken != null) nameValuePairs.put("refresh_token",refreshToken);
        nameValuePairs.put("code",code);
        //todo read the uri from config
        return restClient.sendHttpPostRequest(nameValuePairs, cfg.FQDN + "/oauth/token", headers);
    }
}
