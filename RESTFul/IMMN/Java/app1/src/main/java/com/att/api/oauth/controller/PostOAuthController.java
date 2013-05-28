package com.att.api.oauth.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import java.io.IOException;
import javax.servlet.ServletException;

/**
 * Created with IntelliJ IDEA.
 * User: sendhilc
 * Date: 1/22/13
 * Time: 7:39 PM
 * To change this template use File | Settings | File Templates.
 */
public class PostOAuthController extends HttpServlet {
    public static final String RETURN_TO_POST_OAUTH = "return_to_post_oauth";

    /**
     * Handles a GET request.
     *
     * @throws java.io.IOException
     * @throws javax.servlet.ServletException
     * @see HttpServlet
     */
    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {

        HttpSession session = request.getSession();

        String redirect_to = (String) session.getAttribute(RETURN_TO_POST_OAUTH);

        request.getRequestDispatcher(redirect_to).forward(request,response);
    }
}
