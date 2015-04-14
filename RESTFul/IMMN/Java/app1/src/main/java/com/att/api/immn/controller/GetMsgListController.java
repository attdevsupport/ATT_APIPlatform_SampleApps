/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.att.api.immn.controller;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.immn.service.IMMNService;
import com.att.api.immn.service.Message;
import com.att.api.immn.service.MessageList;
import com.att.api.immn.service.MessageListArgs;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.RESTException;

public class GetMsgListController extends APIController {

    private static final long serialVersionUID = -8656138832360158282L;

    public void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        final HttpSession session = request.getSession();
        OAuthToken token = (OAuthToken) session.getAttribute("token");
        IMMNService srvc = new IMMNService(appConfig.getApiFQDN(), token);
        int limit = Integer.valueOf(appConfig.getProperty("listLimit", "5"));
        final boolean favorite = request.getParameter("favorite") != null;
        final boolean unread = request.getParameter("unread") != null;
        final boolean incoming = request.getParameter("incoming") != null;
        final String keyword = request.getParameter("keyword");

        JSONObject jresponse = new JSONObject();
        try {
            /* TODO: convert null values to '-' characters */
            final MessageListArgs args = new MessageListArgs.Builder(limit, 0)
                .setIsFavorite(favorite)
                .setIsUnread(unread)
                .setIsIncoming(incoming)
                .setKeyword(keyword)
                .build();
            final MessageList msgList = srvc.getMessageList(args);
            final JSONArray tables = new JSONArray();

            final JSONArray detailValues = new JSONArray()
                .put(msgList.getLimit())
                .put(msgList.getOffset())
                .put(msgList.getTotal())
                .put(msgList.getCacheStatus())
                .put(msgList.getFailedMessages())
                .put(msgList.getState());

            final JSONArray msgValues = new JSONArray();
            for (final Message msg : msgList.getMessages()) {
                msgValues.put(new JSONArray()
                    .put(msg.getMessageId())
                    .put(msg.getFrom())
                    .put(msg.getRecipients())
                    .put(msg.getText())
                    .put(msg.getTimeStamp())
                    .put(msg.isFavorite())
                    .put(msg.isUnread())
                    .put(msg.isIncoming())
                    .put(msg.getType())
                );
            }
            tables.put(
                new JSONObject().put(
                    "caption", "Details:"
                ).put(
                    "headers", new JSONArray().put("Limit")
                               .put("Offset").put("Total")
                               .put("Cache Status").put("Failed Messages")
                               .put("State")
                ).put("values", new JSONArray().put(detailValues))
            ).put(
                new JSONObject().put(
                    "caption", "Messages:"
                ).put(
                    "headers", new JSONArray().put("Message ID")
                               .put("From").put("Recipients")
                               .put("Text").put("Timestamp")
                               .put("Favorite").put("Unread")
                               .put("Incoming").put("Type")
                ).put("values", msgValues)
            );
            jresponse.put("success", true).put("tables", tables);
        } catch (RESTException re) {
            jresponse.put("success", false).put("text", re.getMessage());
        }

        response.setContentType("text/html");
        PrintWriter writer = response.getWriter();
        writer.print(jresponse);
        writer.flush();
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
