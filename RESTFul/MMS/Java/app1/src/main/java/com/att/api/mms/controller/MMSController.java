package com.att.api.mms.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;

import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;
import com.att.api.mms.file.ImageEntry;
import com.att.api.mms.file.ImageFileHandler;
import com.att.api.mms.model.ConfigBean;
import com.att.api.mms.model.MMSStatus;
import com.att.api.mms.service.MMSFileUtil;
import com.att.api.mms.service.MMSService;

import java.io.File;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

public class MMSController extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private volatile AppConfig cfg;

    private String getPath() {
        return getServletContext().getRealPath("/") + "MMSImages";
    }

    private RESTConfig getRESTConfig(final String endpoint) {
        final String proxyHost = cfg.getProxyHost(); 
        final int proxyPort = cfg.getProxyPort(); 
        final boolean trustAllCerts = cfg.getTrustAllCerts();

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
                        getRESTConfig(endpoint), clientId, clientSecret);

                token = service.getToken(cfg.getProperty("scope"));
                token.saveToken(tokenFile);
            }
            return token;
        } catch (IOException ioe) {
            throw new RESTException(ioe);
        }
    }

    private String[] getFileNames() {
        String attachmentsDir = cfg.getProperty("attachmentsDir");

        String dir = getServletContext().getRealPath("/")  + attachmentsDir;
        
        File[] files = new File(dir).listFiles();
        List<String> fnames = new LinkedList<String>();
        fnames.add(""); // no attachment
        for (int i = 0; i < files.length; ++i) {
            if (!files[i].isDirectory()) {
                fnames.add(files[i].getName());
            }
        }

        String[] toReturn = new String[fnames.size()];
        fnames.toArray(toReturn);
        return toReturn;
    }

    private void handleSendMMS(HttpServletRequest request) {
        if (request.getParameter("address") == null) {
            return;
        }

        try {
            String addr = request.getParameter("address");
            request.getSession().setAttribute("addr", addr);
            String subject = request.getParameter("subject");
            String attachment = request.getParameter("attachment");
            String dir = cfg.getProperty("attachmentsDir");
            boolean notify = request.getParameter("chkGetOnlineStatus") != null;
            String[] fnames;
            if (attachment.equals("")) {
                fnames = new String[] {};
            } else  {
                fnames = new String[] { 
                    getServletContext().getRealPath("/") + dir + "/" + attachment 
                };
            }

            OAuthToken token = this.getToken();
            String endpoint = cfg.getFQDN() + "/mms/v3/messaging/outbox";
            RESTConfig restCfg = this.getRESTConfig(endpoint);
            MMSService srvc = new MMSService(restCfg, token);
            JSONObject response = srvc.sendMMS(addr, fnames, subject, 
                    null, notify); /* priority = null... for now */
            JSONObject outboundMR 
                = response.getJSONObject("outboundMessageResponse");
            String id = outboundMR.getString("messageId");

            request.setAttribute("resultSendId", id);

            if (!notify) {
                request.getSession().setAttribute("resultId", id);
            }

            if (outboundMR.has("resourceReference")) {
                String rURL = outboundMR.getJSONObject("resourceReference")
                    .getString("resourceURL");
                request.setAttribute("resultResourceUrl", rURL);
            }
            request.setAttribute("resultSendMMS", true);
        } catch (RESTException e) {
            request.setAttribute("errorSendMMS", e.getErrorMessage());
        }
    }

    private void handleGetStatus(HttpServletRequest request) {
        if (request.getParameter("mmsId") == null) {
            return;
        }

        try {
            OAuthToken token = this.getToken();
            String mmsId = request.getParameter("mmsId");
            String endpoint = cfg.getFQDN() 
                + "/mms/v3/messaging/outbox/" + mmsId;

            MMSService srvc = new MMSService(getRESTConfig(endpoint), token);
            JSONObject jstatus = srvc.getMMSStatus();
            JSONObject infoList = jstatus.getJSONObject("DeliveryInfoList");
            String resultStatus = infoList.getJSONArray("DeliveryInfo")
                .getJSONObject(0).getString("DeliveryStatus");
            String resourceURL = infoList.getString("ResourceUrl");
            request.setAttribute("resultStatus", resultStatus);
            request.setAttribute("resultResourceUrl", resourceURL);
        } catch (Exception e) {
            request.setAttribute("errorGetStatus", e.getMessage());
        }
    }

    public void init() {
        try {
            this.cfg = AppConfig.getInstance();
        } catch (IOException e) {
            // print stack trace instead of handling
            e.printStackTrace();
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        this.handleSendMMS(request);
        this.handleGetStatus(request);

        final MMSStatus[] statuses = MMSFileUtil.getStatuses();
        request.setAttribute("resultStatuses", statuses);

        List<ImageEntry> entries = new ImageFileHandler(
                this.getPath() + "/mmslistener.db").getImageEntrys();

        request.setAttribute("imgCount", entries.size());
        request.setAttribute("images", entries);
        request.setAttribute("cfg", new ConfigBean());
        request.setAttribute("fnames", getFileNames());

        final String forward = "WEB-INF/MMS.jsp";
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}
