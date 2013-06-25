package com.att.api.sms.controller;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.sms.model.ConfigBean;
import com.att.api.sms.model.SMSMessage;
import com.att.api.sms.model.SMSReceiveMsg;
import com.att.api.sms.model.SMSStatus;
import com.att.api.sms.service.SMSFileUtil;
import com.att.api.sms.service.SMSService;

import java.io.IOException;

public class SMSController extends APIController {
    private static final long serialVersionUID = 1L;

    private void handleSendSMS(final HttpServletRequest request) {
        if (request.getParameter("sendSMS") == null) {
            return;
        }

        try {
            OAuthToken token = getFileToken();
            SMSService service = new SMSService(appConfig.getFQDN(), token);

            String addr = request.getParameter("address");
            request.getSession().setAttribute("addr", addr);
            String msg = request.getParameter("message");
            boolean getNotification 
                = request.getParameter("chkGetOnlineStatus") != null;
            
            JSONObject jresponse = service.sendSMS(addr, msg, getNotification);
            JSONObject smsResponse 
                = jresponse.getJSONObject("outboundSMSResponse");
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
            final OAuthToken token = getFileToken();
            final String msgid = request.getParameter("messageId");
            request.getSession().setAttribute("statusId", msgid);

            SMSService service = new SMSService(appConfig.getFQDN(), token);
            JSONObject jobj = service.getSMSDeliveryStatus(msgid);
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
            String shortCode = appConfig.getProperty("getMsgsShortCode");
            if (shortCode == null) return;

            OAuthToken token = getFileToken();

            SMSService service = new SMSService(appConfig.getFQDN(), token);
            JSONObject jobj = service.getSMSReceive(shortCode);
            JSONObject msgList = jobj.getJSONObject("InboundSmsMessageList");

            String numbMessages = 
                msgList.getString("NumberOfMessagesInThisBatch");

            String numbPending = 
                msgList.getString("TotalNumberOfPendingMessages");

            JSONArray msgs = msgList.getJSONArray("InboundSmsMessage"); 

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
