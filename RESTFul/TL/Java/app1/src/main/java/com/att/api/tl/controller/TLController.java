package com.att.api.tl.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;
import com.att.api.tl.model.ConfigBean;
import com.att.api.tl.model.TLResponse;
import com.att.api.tl.service.TLService;

import java.io.IOException;

public class TLController extends APIController {
    private static final long serialVersionUID = 1L;

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
            final OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String acceptableAccuracy = (String) session.getAttribute(names[1]);
            String requestedAccuracy = (String) session.getAttribute(names[2]);
            String tolerance = (String) session.getAttribute(names[3]);

            TLService service = new TLService(appConfig.getFQDN(), token);
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
