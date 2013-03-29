package com.att.api.mim.controller;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.mim.model.MIMResponse;
import com.att.api.mim.service.MIMService;
import com.att.api.immn.controller.Config;
import com.att.api.oauth.controller.PostOAuthController;

public class MIMHeaderController extends HttpServlet {
    public final static String NAME = "submitGetHeaders";

    /**
     * Forwards the specified speech response to the viewer page.
     *
     * @param attrName
     *            The name to set the attribute as to access inside jsp
     * @param MIMResponse
     *            mim response to forward.
     * @param request
     *            servlet request
     * @param response
     *            servlet response
     * @throws ServletException
     *             if the target resource throws this exception
     * @throws java.io.IOException
     *             if the target resource throws this exception
     */
    private void forwardResponse(MIMResponse mimResponse,
            HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        if (mimResponse != null) {
            request.setAttribute(NAME, mimResponse);
        }
        RequestDispatcher dispatcher = request.getRequestDispatcher("/IMMN.jsp");
        dispatcher.forward(request, response);
    }

    /**
     * Handles a GET request.
     * 
     * @throws java.io.IOException
     * @throws ServletException
     * @see HttpServlet
     */
    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        // Simply redirect to main page
        Config cfg = (Config) request.getSession().getAttribute("cfg");

        HttpSession session = request.getSession();

        String sessionHeaderCount = (String) session.getAttribute("HeaderCount");
        String sessionIndexCursor = (String) session.getAttribute("IndexCursor");
            
        if (sessionHeaderCount != null && sessionIndexCursor != null){
            session.setAttribute("HeaderCount", null);
            session.setAttribute("IndexCursor", null);
            processRequest(request, response, cfg, sessionHeaderCount, sessionIndexCursor);
            return;
        }

        //not setting any attributes so first two params are null
        forwardResponse(null, request, response);
    }

    /**
     * @see HttpServlet
     */
    @Override
    public void doPost(HttpServletRequest request, HttpServletResponse response) throws IOException, ServletException {
        Config cfg = (Config) request.getSession().getAttribute("cfg");

        if (cfg == null) {
            forwardResponse(MIMResponse.createErrorResponse("No configuration supplied"),
                    request, response);
            return;
        }
        if (cfg.getError() != null) {
            forwardResponse(MIMResponse.createErrorResponse(cfg.getError()), request,response);
            return;
        }

        String headerCount = (String) request.getParameter("headerCountTextBox");
        String indexCursor = (String) request.getParameter("indexCursorTextBox");

        HttpSession session = request.getSession();

        //For the first time, we don't have access token, so we redirect to authenticate client id
        if(session.getAttribute("accessToken") == null )
        {
            session.setAttribute(PostOAuthController.RETURN_TO_POST_OAUTH, NAME);
            session.setAttribute("HeaderCount", headerCount);
            session.setAttribute("IndexCursor", indexCursor);
            response.sendRedirect(cfg.FQDN + "/oauth/authorize?client_id=" + cfg.clientIdAuth + "&scope=" + cfg.scope + "&redirect_uri=" + cfg.redirectUri);
            return;
        }
        processRequest(request, response, cfg, headerCount, indexCursor);
    }


    private void processRequest(HttpServletRequest request, HttpServletResponse response, 
            Config cfg, String headerCount, String indexCursor) throws ServletException, IOException {
        try {
            String accessToken = (String) request.getSession().getAttribute("accessToken");
            if (accessToken == null || accessToken.equals("")) {
                forwardResponse(MIMResponse.createErrorResponse("Access token has not been set."), request, response);
                return;
            }

            MIMService mimService = new MIMService(cfg);
            MIMResponse mimResponse = mimService.getMessageHeader(headerCount, indexCursor, accessToken);

            forwardResponse(mimResponse, request, response);

        } catch (Exception e) {
            forwardResponse(MIMResponse.createErrorResponse(e.getMessage()), request, response);
        }
    }
}
