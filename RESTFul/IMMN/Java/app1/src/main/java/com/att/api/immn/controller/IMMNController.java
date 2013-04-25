package com.att.api.immn.controller;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.immn.model.IMMNResponse;
import com.att.api.immn.service.IMMNService;
import com.att.api.oauth.controller.PostOAuthController;

public class IMMNController extends HttpServlet {
    public static final String NAME = "sendMessage";

    /**
     * 
     */
    private static final long serialVersionUID = 7625669556057988325L;

    /**
     * Forwards the specified speech response to the viewer page.
     * 
     * @param immnResponse
     *            immn response to forward.
     * @param request
     *            servlet request
     * @param response
     *            servlet response
     * @throws ServletException
     *             if the target resource throws this exception
     * @throws java.io.IOException
     *             if the target resource throws this exception
     */
    private void forwardResponse(IMMNResponse immnResponse,
            HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        if (immnResponse != null) {
            request.setAttribute(NAME, immnResponse);
        }
        RequestDispatcher dispatcher = request
            .getRequestDispatcher("/IMMN.jsp");
        dispatcher.forward(request, response);
    }


    /**
     * Parses the upload request.
     * 
     * @param formFieldValues
     *            form field values to populate
     * @param request
     *            servlet request
     * @throws Exception
     *             if target resource throws this exception
     */
    @SuppressWarnings("unchecked")
    private File parseRequest(Config cfg,
            HashMap<String, String> formFieldValues,
            HttpServletRequest request) throws Exception {
        final Enumeration<String> params = request.getParameterNames();

        while (params.hasMoreElements()) 
        {
            final String name = params.nextElement();
            final String value = request.getParameter(name);
            formFieldValues.put(name, value);
        }

        request.getSession().setAttribute("formFieldValues", formFieldValues);

        File uploadFile = null;
        String fname = request.getParameter("attachment");
        String directory = request.getSession().getServletContext()
            .getRealPath("/").concat(cfg.attachmentFolder);

        if (fname != null) {
            uploadFile = new File(directory, fname);
        }

        if (uploadFile == null || !uploadFile.isFile()) {
            uploadFile = new File(directory, "");
        }

        return uploadFile;
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

        String addr = (String) request.getAttribute("Address");

        if (addr != null)
            session.setAttribute("address", addr);

        HashMap<String, String> sessionFormFieldValues = (HashMap<String, String>) session.getAttribute("formFieldValues");
        File sessionUploadFile = (File) session.getAttribute("uploadFile");

        //TODO on success or error , remove this attribute from session from JSP view
        if (sessionFormFieldValues != null)
        {
            session.setAttribute("formFieldValues", null);
            session.setAttribute("uploadFile", null);
            processRequest(request, response, cfg, sessionFormFieldValues, sessionUploadFile);
            return;
        }
        forwardResponse(null, request, response);
    }

    /**
     * Handles a POST file upload request.
     * 
     * @see HttpServlet
     */
    @Override
    public void doPost(HttpServletRequest request, HttpServletResponse response) throws IOException, ServletException {
        Config cfg = (Config) request.getSession().getAttribute("cfg");
        HashMap<String, String> formFieldValues = new HashMap<String, String>();
        File uploadFile;

        HttpSession session = request.getSession();

        String addr = (String) request.getAttribute("Address");

        if (addr != null)
            session.setAttribute("address", addr);

        try {
            uploadFile = parseRequest(cfg,formFieldValues, request);
        } catch (Exception e1) {
            forwardResponse(IMMNResponse.createErrorResponse(e1.getMessage()), request,response);
            return;
        }

        request.getSession().setAttribute("formFields", formFieldValues);
        if (cfg == null) {
            forwardResponse(IMMNResponse.createErrorResponse("No configuration supplied"),
                    request, response);
            return;
        }
        if (cfg.getError() != null) {
            forwardResponse(IMMNResponse.createErrorResponse(cfg.getError()), request,response);
            return;
        }

        //For the first time, we don't have access token, so we redirect to authenticate client id
        if((session.getAttribute("accessToken") == null ))
        {
            //todo take endpoint value and put it in config
            session.setAttribute(PostOAuthController.RETURN_TO_POST_OAUTH, NAME);
            session.setAttribute("formFieldValues", formFieldValues);
            session.setAttribute("uploadFile", uploadFile);
            response.sendRedirect(cfg.FQDN + "/oauth/authorize?client_id=" + cfg.clientIdAuth + "&scope=" + cfg.scope + "&redirect_uri=" + cfg.redirectUri);
            return;
        }
        processRequest(request, response, cfg, formFieldValues, uploadFile);
    }

    private void processRequest(HttpServletRequest request, HttpServletResponse response, Config cfg, HashMap<String, String> formFieldValues, File uploadFile) throws ServletException, IOException {
        List<String> addresses = validatePhoneNumber(formFieldValues.get("Address"));
        request.getSession().setAttribute("address", formFieldValues.get("Address"));
        List<String> files = new ArrayList<String>();

        String subjectText = formFieldValues.get("subject") == null ? "" : formFieldValues.get("subject");
        String messageText = formFieldValues.get("message") == null ? "" : formFieldValues.get("message");

        try {
            String accessToken = (String) request.getSession().getAttribute(
                    "accessToken");
            if (accessToken == null || accessToken.equals("")) {
                forwardResponse(IMMNResponse.createErrorResponse("Access token has not been set."), request, response);
                return;
            }
            if(!formFieldValues.get("attachment").contains("None"))
                files.add(uploadFile.getAbsolutePath());

            String groupValue = "false";
            String groupCheckBox = formFieldValues.get("message") == null ? "" : formFieldValues.get("groupCheckBox");
            if (groupCheckBox != null && groupCheckBox.equals("on")) {
                groupValue = "true";
            }
            StringBuilder addressStr = new StringBuilder();
            Iterator<String> itr = addresses.iterator();
            if (itr.hasNext()){
                addressStr.append(itr.next());
                while(itr.hasNext()){
                    addressStr.append("&Addresses=").append(itr.next());
                }
            }
            else {
                throw new Exception("Please enter a valid phone address");
            }
            IMMNService immnService = new IMMNService(cfg);
            IMMNResponse immnResponse = immnService.sendMessages(accessToken, files, addressStr.toString(),messageText,subjectText,groupValue);

            forwardResponse(immnResponse, request, response);
        } catch (Exception e) {
            forwardResponse(IMMNResponse.createErrorResponse(e.getMessage()), request,
                    response);
        }
    }
    //todo this should move into utility class
    private List<String> validatePhoneNumber(String phoneNumber)
    {
        List<String> addresses = new ArrayList<String>();
        if (phoneNumber != null)
        {
            phoneNumber = phoneNumber.trim();
            //Parse the phone addresses
            String [] address = phoneNumber.split(",");
            if (address == null || address.length == 0) {
                address = new String[] { phoneNumber };
            }
            for(String a : address) 
            {
                if(a.contains("@"))
                {
                    addresses.add(a);
                }

                else if(a.length() >= 10)
                {	
                    String expression = "[+]?[0-15]*[0-9]+$";  
                    CharSequence inputStr = a;  
                    Pattern pattern = Pattern.compile(expression);  
                    Matcher matcher = pattern.matcher(inputStr);  
                    if(matcher.matches()){  
                        a = "tel:"+a;
                        addresses.add(a);
                    }		  
                }
                else if(a.length()>2 && a.length()<=8)
                {
                    String expression = "[0-15]*[0-9]+$";  
                    CharSequence inputStr = a;  
                    Pattern pattern = Pattern.compile(expression);  
                    Matcher matcher = pattern.matcher(inputStr);  
                    if(matcher.matches()){  
                        a = "short:"+a;
                        addresses.add(a);
                    }
                }
            }		
        }
        return addresses;
    }
}
