package com.att.api.mms.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONObject;

import com.att.api.config.AppConfig;
import com.att.api.controller.APIController;
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

public class MMSController extends APIController {
    private static final long serialVersionUID = 1L;

    private String getPath() {
        return getServletContext().getRealPath("/") + "MMSImages";
    }

    private String[] getFileNames() {
        String attachmentsDir = appConfig.getProperty("attachmentsDir");

        String dir = getServletContext().getRealPath("/") + attachmentsDir;

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
            String subject = request.getParameter("subject");
            String attachment = request.getParameter("attachment");
            String dir = appConfig.getProperty("attachmentsDir");
            boolean notify = request.getParameter("chkGetOnlineStatus") != null;
            String[] fnames;
            if (attachment.equals("")) {
                fnames = new String[] {};
            } else {
                fnames = new String[] { getServletContext().getRealPath("/")
                        + dir + "/" + attachment };
            }

            /* save input to session */
            HttpSession session = request.getSession();
            session.setAttribute("addr", addr);
            session.setAttribute("subject", subject);
            session.setAttribute("notify", notify);
            session.setAttribute("attachment", attachment);

            OAuthToken token = getFileToken();
            MMSService srvc = new MMSService(appConfig.getFQDN(), token);
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
            OAuthToken token = getFileToken();
            String mmsId = request.getParameter("mmsId");

            request.getSession().setAttribute("resultId", mmsId);

            MMSService srvc = new MMSService(appConfig.getFQDN(), token);
            JSONObject jstatus = srvc.getMMSStatus(mmsId);
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
