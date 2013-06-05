package com.att.api.sms.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.config.AppConfig;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;
import com.att.api.sms.model.ConfigBean;
import com.att.api.sms.model.SMSMessage;
import com.att.api.sms.model.SMSReceiveMsg;
import com.att.api.sms.model.SMSStatus;
import com.att.api.sms.service.SMSFileUtil;
import com.att.api.sms.service.SMSService;

import java.io.IOException;

public class SMSController extends HttpServlet {
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

    private void handleSendSMS(final HttpServletRequest request) {
        if (request.getParameter("sendSMS") == null) {
            return;
        }

        try {
            final AppConfig cfg = AppConfig.getInstance();
            final OAuthToken token = this.getToken();
            final String endpoint = cfg.getFQDN()
                + "/sms/v3/messaging/outbox";

            SMSService service 
                = new SMSService(getRESTConfig(endpoint, cfg), token);

            final String addr = request.getParameter("address");
            request.getSession().setAttribute("addr", addr);
            final String msg = request.getParameter("message");
            final boolean getNotification 
                = request.getParameter("chkGetOnlineStatus") != null;
            
            JSONObject jresponse = service.sendSMS(addr, msg, getNotification);
            JSONObject smsResponse = jresponse
                .getJSONObject("outboundSMSResponse");
            String sendId = smsResponse.getString("messageId"); 
            request.setAttribute("sendId", sendId);

            if (!getNotification) {
                request.getSession().setAttribute("statusId", sendId);
            }
		
            request.setAttribute("statusId", sendId);
            if (smsResponse.has("resourceReference")) {
                String resourceURL = smsResponse.getJSONObject
                    ("resourceReference").getString("resourceURL");
                request.setAttribute("resourceURL", resourceURL);
            }
        } catch (Exception e) {
            request.setAttribute("sendError", e.getMessage());
        }
    }

    private void handleGetStatus(final HttpServletRequest request) {
        if (request.getParameter("getStatus") == null) {
            return;
        }

        try {
            final AppConfig cfg = AppConfig.getInstance();
            final OAuthToken token = this.getToken();
            final String msgid = request.getParameter("messageId");
            request.getSession().setAttribute("statusId", msgid);

            final String endPoint = 
                cfg.getFQDN() + "/sms/v3/messaging/outbox/" + msgid;


            SMSService service = 
                new SMSService(getRESTConfig(endPoint, cfg), token);
            JSONObject jobj = service.getSMSDeliveryStatus();
            JSONObject jInfoList = jobj.getJSONObject("DeliveryInfoList");
            JSONArray jstatuses =  jInfoList.getJSONArray("DeliveryInfo");
            SMSStatus[] statuses = new SMSStatus[jstatuses.length()];
            
            for (int i = 0; i < jstatuses.length(); ++i) {
                JSONObject jstatus = jstatuses.getJSONObject(i);
                SMSStatus status = new SMSStatus
                    (
                     jstatus.getString("Id"),
                     jstatus.getString("Address"),
                     jstatus.getString("DeliveryStatus")
                    );
                statuses[i] = status;
            }

            final String resourceURL = jInfoList.getString("ResourceUrl");

            request.setAttribute("resultGetStatuses", statuses);
            request.setAttribute("resourceURL", resourceURL);
        } catch (Exception e) {
            request.setAttribute("statusError", e.getMessage());
        }
    }

    private void handleGetMsgs(final HttpServletRequest request) {
        if (request.getParameter("getMessages") == null) {
            return;
        }

        try {
            final AppConfig cfg = AppConfig.getInstance();
            String shortCode = cfg.getProperty("getMsgsShortCode");
            if (shortCode == null) return;

            final OAuthToken token = this.getToken();

            final String endPoint = 
                cfg.getFQDN() + "/rest/sms/2/messaging/inbox";

            SMSService service = 
                new SMSService(getRESTConfig(endPoint, cfg), token);
            JSONObject jobj = service.getSMSReceive(shortCode);
            JSONObject msgList = jobj.getJSONObject("InboundSmsMessageList");

            final String numbMessages = 
                msgList.getString("NumberOfMessagesInThisBatch");

            final String numbPending = 
                msgList.getString("TotalNumberOfPendingMessages");

            final JSONArray msgs =
                msgList.getJSONArray("InboundSmsMessage"); 

            SMSMessage[] smsMsgs = new SMSMessage[msgs.length()];
            for (int i = 0; i < msgs.length(); ++i) {
                final JSONObject jmsg = msgs.getJSONObject(i);
                final String msgId = jmsg.getString("MessageId");
                final String msgIndex = jmsg.getString("Message");
                final String senderAddr = jmsg.getString("SenderAddress");
                SMSMessage smsMsg = new SMSMessage(msgId, msgIndex, senderAddr);
                smsMsgs[i] = smsMsg;
            }

            request.setAttribute("SMSMessages", smsMsgs);
            request.setAttribute("numbMessages", numbMessages);
            request.setAttribute("numbPending", numbPending);
        } catch (Exception e) {
            request.setAttribute("receiveError", e.getMessage());
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            handleSendSMS(request);

            handleGetStatus(request);

            handleGetMsgs(request);

            final SMSStatus[] statuses = SMSFileUtil.getStatuses();
            request.setAttribute("resultStatuses", statuses);

            final SMSReceiveMsg[] receiveMsgs = SMSFileUtil.getReceiveMsgs();
            request.setAttribute("resultReceiveMsgs", receiveMsgs);

            request.setAttribute("cfg", new ConfigBean());

            final String forward = "/WEB-INF/SMS.jsp";
            RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
            dispatcher.forward(request, response);
        }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
            doPost(request, response);
        }
}
