package com.att.api.immn.controller;

import java.io.File;
import java.io.IOException;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.controller.APIController;
import com.att.api.immn.model.ConfigBean;
import com.att.api.immn.service.IMMNBody;
import com.att.api.immn.service.IMMNHeader;
import com.att.api.immn.service.IMMNResponse;
import com.att.api.immn.service.IMMNService;
import com.att.api.oauth.OAuthToken;

public class IMMNController extends APIController {
    private static final long serialVersionUID = 7625669556057988325L;

    private final String[] sendingParams = {"Address", "groupCheckBox",
        "message", "subject", "attachment"}; 

    private final String[] msgBodyParams = {"MessageId", "PartNumber"}; 

    private final String[] msgHeaderParams = {"headerCountTextBox",
        "indexCursorTextBox"};

    private boolean handleSendMessage(HttpServletRequest request,
            HttpServletResponse response) {

        copyToSession(request, this.sendingParams);
        final HttpSession session = request.getSession();

        String addr = (String) session.getAttribute("Address");
         if (addr == null ) {
            return false;
        }

        String[] addrs = addr.split(",");
        String msg = (String) session.getAttribute("message");
        String subject = (String) session.getAttribute("subject");

        String fname = (String) session.getAttribute("file");
        File[] f = null;
        if (fname != null && !fname.equalsIgnoreCase("none"))
           f  = new File[]{ new File("attachments/" + fname) };

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }
            IMMNService srvc = new IMMNService(appConfig.getFQDN(), token);
            String id = srvc.sendMessage(addrs, msg, subject, f);
            request.setAttribute("immnResponse", id);
            clearSession(request, this.sendingParams);
        } catch (Exception e) {
            request.setAttribute("immnError", e.getMessage());
            clearSession(request, this.sendingParams);
        } 

        return false;
    }

    public boolean handleGetMessageBody(HttpServletRequest request,
            HttpServletResponse response) {

        copyToSession(request, this.msgBodyParams);
        HttpSession session = request.getSession();

        String msgId = (String) session.getAttribute("MessageId");
        if (msgId == null)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String partNumb = (String) session.getAttribute("PartNumber");

            IMMNService srvc = new IMMNService(appConfig.getFQDN(), token);
            IMMNBody body = srvc.getMessageBody(msgId, partNumb);
            request.setAttribute("mimContent", body);
            clearSession(request, this.msgBodyParams);
        } catch (Exception e) {
            request.setAttribute("mimError", e.getMessage());
            clearSession(request, this.msgBodyParams);
        } 

        return false;
    }

    public boolean handleGetMessageHeaders(HttpServletRequest request,
            HttpServletResponse response) {

        copyToSession(request, this.msgHeaderParams);
        HttpSession session = request.getSession();

        String headerCount = (String)
            session.getAttribute("headerCountTextBox");
         if (headerCount == null)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            int headerC = Integer.parseInt(headerCount);
            String indexC = (String) session.getAttribute("indexCursorTextBox");

            IMMNService srvc = new IMMNService(appConfig.getFQDN(), token);
            IMMNResponse immnResponse = srvc.getMessageHeaders(headerC, indexC);
            IMMNHeader[] immnHeaders = immnResponse.getHeaders();
            if (!immnHeaders[0].getFrom().equals("+14258028620"))
                throw new Exception(immnHeaders[0].getFrom());
            request.setAttribute("mimResponse", immnResponse);
            request.setAttribute("mimHeaders", immnHeaders);
            clearSession(request, this.msgHeaderParams);
        } catch (Exception e) {
            request.setAttribute("mimError", e.getMessage());
            clearSession(request, this.msgHeaderParams);
        } 

        return false;
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

            if ( handleSendMessage(request, response) 
                    || handleGetMessageBody(request, response) 
                    || handleGetMessageHeaders(request, response) ) {
                return;
            }

            request.setAttribute("cfg", new ConfigBean());

            final String forward = "WEB-INF/immn.jsp";
            RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
            dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        doPost(request, response);
    }
}
