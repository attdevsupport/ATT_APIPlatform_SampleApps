package com.att.api.immn.controller;

import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.commons.codec.binary.Base64;

import com.att.api.controller.APIController;
import com.att.api.immn.model.ConfigBean;
import com.att.api.immn.service.DeltaChange;
import com.att.api.immn.service.DeltaResponse;
import com.att.api.immn.service.IMMNService;
import com.att.api.immn.service.Message;
import com.att.api.immn.service.MessageContent;
import com.att.api.immn.service.MessageIndexInfo;
import com.att.api.immn.service.MessageList;
import com.att.api.immn.service.MessageListArgs;
import com.att.api.immn.service.NotificationConnectionDetails;
import com.att.api.immn.service.SendResponse;
import com.att.api.oauth.OAuthToken;

public class IMMNController extends APIController {
    private static final long serialVersionUID = 7625669556057988325L;

    private final String[] sendingParams = {"address", "groupCheckBox",
        "message", "subject", "attachment"};

    private final String[] createMsgIndexParams = {"createMessageIndex"};

    private final String[] msgIndexInfoParams = {"getMessageIndexInfo"};

    private final String[] msgListParams = {"favorite", "incoming", "unread",
        "keyword", "getMessageList"};

    private final String[] getMsgParams = {"messageId", "getMessage"};

    private final String[] notifyDetailsParams = {"getNotifyDetails", "queues"};

    private final String[] getMsgContentParams = {"messageId", "partNumber", 
        "getMessageContent"};

    private final String[] getDeltaParams = { "getDelta", "state" };

    private final String[] updateMsgsParams = { "updateMessage", "readflag", 
        "messageId" };

    private final String[] deleteMsgsParams = { "deleteMessage", "messageId" };

    private boolean handleSendMessage(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.sendingParams);
        final HttpSession session = request.getSession();

        String addr = (String) session.getAttribute("address");
        if (addr == null ) {
            return false;
        }

        String[] addrs = addr.split(",");
        for (int i = 0; i < addrs.length; ++i)
            addrs[i] = addrs[i].trim();

        String msg = (String) session.getAttribute("message");
        String subject = (String) session.getAttribute("subject");
        boolean group = session.getAttribute("group") != null;

        String[] f = null;
        String fname = (String) session.getAttribute("file");
        if (fname != null && !fname.equalsIgnoreCase("none"))
            f  = new String[]{ "attachments/" + fname };

        try {
            OAuthToken token = getSessionToken(request, response);

            if (token == null) {
                return true;
            }

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
            SendResponse sendResponse = null;
            if (!subject.isEmpty() || group || f != null)
                sendResponse = srvc.sendMessage(addrs, msg, subject, group, f);
            else
                sendResponse = srvc.sendMessage(addrs, msg);

            request.setAttribute("immnResponse", sendResponse);
            clearSession(request, this.sendingParams);
        } catch (Exception e) {
            clearSession(request, this.sendingParams);
            request.setAttribute("immnError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "sendMsg");
        }

        return false;
    }

    public boolean handleCreateMessageIndex(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.createMsgIndexParams);
        HttpSession session = request.getSession();

        boolean createIndexButton = session.getAttribute("createMessageIndex") != null;

        if (!createIndexButton)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            srvc.createMessageIndex();

            request.setAttribute("indexResponse", "Success!");
            clearSession(request, this.createMsgIndexParams);
        } catch (Exception e) {
            e.printStackTrace();
            request.setAttribute("indexError", e.getMessage());
            clearSession(request, this.createMsgIndexParams);
        } finally {
            request.setAttribute("toggleDiv", "createMsg");
        }


        return false;
    }

    public boolean handleGetMessageList(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.msgListParams);
        HttpSession session = request.getSession();

        boolean getMsgList = session.getAttribute("getMessageList") != null;

        if (!getMsgList)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            int limit = Integer.valueOf(appConfig.getProperty("listLimit", "5"));

            boolean favorite = session.getAttribute("favorite") != null;
            boolean unread = session.getAttribute("unread") != null;
            boolean incoming = session.getAttribute("incoming") != null;

            String keyword = (String) session.getAttribute("keyword");

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            MessageListArgs.Builder builder = new MessageListArgs.Builder(limit, 0);

            if (favorite)
                builder.setIsFavorite(favorite);
            if (unread)
                builder.setIsUnread(unread);
            if (incoming)
                builder.setIsIncoming(incoming);
            if (keyword != null && !keyword.isEmpty())
                builder.setKeyword(keyword);

            MessageListArgs args = builder.build();

            MessageList msgList = srvc.getMessageList(args);

            request.setAttribute("msgList", msgList);
            clearSession(request, this.msgListParams);
        } catch (Exception e) {
            clearSession(request, this.msgListParams);
            request.setAttribute("msgListError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsg");
        }

        return false;
    }

    public boolean handleGetMessage(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.getMsgParams);
        HttpSession session = request.getSession();

        boolean getMsg = session.getAttribute("getMessage") != null;

        if (!getMsg)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String msgId = (String) session.getAttribute("messageId");

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            Message msg = srvc.getMessage(msgId);

            request.setAttribute("getMsg", msg); 
            clearSession(request, this.getMsgParams);
        } catch (Exception e) {
            clearSession(request, this.getMsgParams);
            request.setAttribute("getMsgError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsg");
        }

        return false;
    }

    public boolean handleUpdateMessages(HttpServletRequest request, 
            HttpServletResponse response) {

        copyToSession(request, this.updateMsgsParams);
        HttpSession session = request.getSession();

        boolean updateMsgs = session.getAttribute("updateMessage") != null;

        if (!updateMsgs)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String readflag = (String) session.getAttribute("readflag");
            String msgIdStr = (String) session.getAttribute("messageId");
            String[] msgIds = msgIdStr.split(",");
            Boolean isUnread = readflag.equals("unread") ? true : false;
            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
            if (msgIds.length > 1) {
                DeltaChange[] changes = new DeltaChange[msgIds.length];
                for (int i = 0; i < msgIds.length; ++i) {
                    DeltaChange dc = new DeltaChange(msgIds[i], null, isUnread);
                    changes[i] = dc;
                }
                srvc.updateMessages(changes);
            } else {
                srvc.updateMessage(msgIds[0], isUnread, null);
            }

            request.setAttribute("updateMsg", true); 
            clearSession(request, this.updateMsgsParams);
        } catch (Exception e) {
            clearSession(request, this.updateMsgsParams);
            request.setAttribute("updateMsgError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "updateMsg");
        }

        return false;
    }

    public boolean handleDeleteMessages(HttpServletRequest request, 
            HttpServletResponse response) {

        copyToSession(request, this.deleteMsgsParams);
        HttpSession session = request.getSession();

        boolean delMsgs = session.getAttribute("deleteMessage") != null;

        if (!delMsgs)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String msgIdStr = (String) session.getAttribute("messageId");
            String[] msgIds = msgIdStr.split(",");

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
            if (msgIds.length > 1) {
                srvc.deleteMessages(msgIds);
            } else {
                final String msgId = msgIds[0];
                srvc.deleteMessage(msgId);
            }

            request.setAttribute("deleteMsg", true); 
            clearSession(request, this.deleteMsgsParams);
        } catch (Exception e) {
            clearSession(request, this.deleteMsgsParams);

            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            e.printStackTrace(pw);
            String s = sw.toString(); // stack trace as a string

            request.setAttribute("deleteMsgError", s);
        } finally {
            request.setAttribute("toggleDiv", "delMsg");
        }

        return false;
    }

    public boolean handleGetDelta(HttpServletRequest request, 
            HttpServletResponse response) {

        copyToSession(request, this.getDeltaParams);
        HttpSession session = request.getSession();

        boolean getDelta = session.getAttribute("getDelta") != null;

        if (!getDelta)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String state = (String) session.getAttribute("state");
            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
            DeltaResponse deltaR = srvc.getDelta(state);
            request.setAttribute("deltas", deltaR); 
            clearSession(request, this.getDeltaParams);
        } catch (Exception e) {
            clearSession(request, this.getDeltaParams);
            request.setAttribute("getDeltaError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsg");
        }

        return false;
    }

    public boolean handleGetMsgContent(HttpServletRequest request,
            HttpServletResponse response) {

        copyToSession(request, this.getMsgContentParams);
        HttpSession session = request.getSession();

        boolean getMsg = session.getAttribute("getMessageContent") != null;

        if (!getMsg)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            String msgId = (String) session.getAttribute("messageId");
            String partNum = (String) session.getAttribute("partNumber");

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            MessageContent msg = srvc.getMessageContent(msgId, partNum);

            String content = "";
            String type = msg.getContentType().toLowerCase();
            if (type.contains("image")) {
                byte[] binary = msg.getContent().getBytes("ISO-8859-1");
                String base64 = new String(Base64.encodeBase64(binary));

                content = "<img src=\"data:" + msg.getContentType()
                    + ";base64," + base64 + "\"/>";
            } else if (type.contains("video")){
                byte[] binary = msg.getContent().getBytes("ISO-8859-1");
                String base64 = new String(Base64.encodeBase64(binary));

                content = "<video controls=\"controls\" autobuffer=\"autobuffer\" autoplay=\"autoplay\">";
                content += "<source src=\"data:" + msg.getContentType() 
                    + ";base64," + base64 + "\" >";
                content += "</video>";
            } else if(type.contains("audio")) {
                byte[] binary = msg.getContent().getBytes("ISO-8859-1");
                String base64 = new String(Base64.encodeBase64(binary));

                content = "<audio controls=\"controls\" autobuffer=\"autobuffer\" autoplay=\"autoplay\">";
                content += "<source src=\"data:" + msg.getContentType() 
                    + ";base64," + base64 + "\" >";
                content += "</audio>";
            } else if (type.contains("text")) {
                content = msg.getContent();
            }else {
                content = "Unknown content type!";
            }

            request.setAttribute("msgContent", content); 
            clearSession(request, this.getMsgContentParams);
        } catch (Exception e) {
            clearSession(request, this.getMsgContentParams);
            request.setAttribute("msgContentError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsg");
        }
        return false;
    }

    public boolean handleMessageIndexInfo(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.msgIndexInfoParams);
        HttpSession session = request.getSession();

        boolean indexInfo = session.getAttribute("getMessageIndexInfo") != null;

        if (!indexInfo)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            MessageIndexInfo index = srvc.getMessageIndexInfo();

            request.setAttribute("msgIndexInfo", index); 
            clearSession(request, this.msgIndexInfoParams);
        } catch (Exception e) {
            clearSession(request, this.msgIndexInfoParams);
            e.printStackTrace();
            request.setAttribute("msgIndexInfoError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsg");
        }

        return false;
    }

    public boolean handleNotificationConnectionDetails(HttpServletRequest request,
            HttpServletResponse response) {
        copyToSession(request, this.notifyDetailsParams);
        HttpSession session = request.getSession();

        boolean notify = session.getAttribute("getNotifyDetails") != null;

        if (!notify)
            return false;

        try {
            OAuthToken token = getSessionToken(request, response);
            if (token == null) {
                return true;
            }

            IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);

            String queue = (String) session.getAttribute("queues");

            NotificationConnectionDetails details = 
                srvc.getNotificationConnectionDetails(queue);

            request.setAttribute("notificationDetails", details); 
            clearSession(request, this.notifyDetailsParams);
        } catch (Exception e) {
            clearSession(request, this.notifyDetailsParams);
            e.printStackTrace();
            request.setAttribute("notificationDetailsError", e.getMessage());
        } finally {
            request.setAttribute("toggleDiv", "getMsgNot");
        }

        return false;
    }


    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {

        if ( handleSendMessage(request, response)
                || handleCreateMessageIndex(request, response)
                || handleGetMessage(request, response)
                || handleGetMsgContent(request, response)
                || handleMessageIndexInfo(request, response)
                || handleNotificationConnectionDetails(request, response)
                || handleGetMessageList(request, response)
                || handleGetDelta(request, response) 
                || handleUpdateMessages(request, response)
                || handleDeleteMessages(request, response)
           )
            return;

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
