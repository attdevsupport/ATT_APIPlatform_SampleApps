package com.att.api.immn.controller;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.model.FileHandler;
import com.att.api.immn.model.IAMFileHandler;
import com.att.api.util.DateUtil;

public class GetNotificationsController extends APIController {
    private static final long serialVersionUID = 1L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws IOException {
        HttpSession session = request.getSession();
        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();

        long tnow = DateUtil.xtimestamp();
        Long subExpiry = (Long) session.getAttribute("subscriptionExpiry");

        if (subExpiry != null && tnow >= subExpiry) {
            session.removeAttribute("subscriptionId");
        }

        String userSubId = (String) session.getAttribute("subscriptionId"); 
        if (userSubId == null) {
            JSONObject jresponse = new JSONObject();
            jresponse.put("stopPolling", true);
            writer.print(jresponse);
        } else {
            JSONArray jresponse = new JSONArray();
            final FileHandler fhandler = IAMFileHandler.getInstance();
            List<JSONObject> notifications = fhandler.loadObjs();

            for (JSONObject note : notifications) {
                JSONObject notification = note.getJSONObject("messageNotifications");
                JSONArray subscriptions = notification.getJSONArray("subscriptionNotifications");
                for (int k = 0; k < subscriptions.length(); ++k) {
                    JSONObject jobj = subscriptions.getJSONObject(k);
                    String subId = jobj.getString("subscriptionId");
                    if (subId.equals(userSubId)) {
                        String callbackData = jobj.getString("callbackData");
                        JSONArray events =  jobj.getJSONArray("notificationEvents");
                        for (int i = 0; i < events.length(); ++i) {
                            JSONObject event = events.getJSONObject(i);
                            jresponse.put(new JSONArray().put(userSubId)
                                    .put(callbackData)
                                    .put(event.get("messageId"))
                                    .put(event.get("conversationThreadId"))
                                    .put(event.get("eventType"))
                                    .put(event.get("text"))
                                    .put(event.get("event"))
                                    .put(event.get("isTextTruncated"))
                                    .put(event.get("isFavorite"))
                                    .put(event.get("isUnread")));
                        }
                    }
                }
            }
            writer.print(jresponse);
        }
        writer.flush();
    }
}
