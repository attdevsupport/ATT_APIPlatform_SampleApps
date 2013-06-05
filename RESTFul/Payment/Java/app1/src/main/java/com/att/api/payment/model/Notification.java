package com.att.api.payment.model;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import org.json.JSONObject;

public class Notification {
    private JSONObject json;
    private String notificationId;

    public Notification(JSONObject json, String id) {
        this.json = json;
        this.notificationId = id;
    }

    public JSONObject getRawJson() {
        return this.json;
    }

    public String getNotificationId() {
        return this.notificationId;
    }

    public String getHtmlTable(){
        return this.generateHtmlTable();
    }

    public String generateHtmlTable() {
        StringBuilder table = new StringBuilder();
        StringBuilder tbody = new StringBuilder();
        StringBuilder thead = new StringBuilder();

        List<String> headers = new LinkedList<String>();

        Iterator<String> itr = this.json.keys();

        //start with creating the table body
        tbody.append("<tbody><tr>");
        headers.add("NotificationId");
        tbody.append("<td data-value=\"NotificationId\">")
            .append(this.notificationId)
            .append("</td>");
        while (itr.hasNext()) {
            String key = itr.next();
            if (key.equalsIgnoreCase("GetNotificationResponse")){
                Iterator<String> subitr = this.json.getJSONObject(key).keys();
                while (subitr.hasNext()){
                    String subkey = subitr.next();
                    String subval = this.json.getJSONObject(key)
                        .getString(subkey);

                    headers.add(subkey);
                    tbody.append("<td data-value=\"")
                        .append(subkey)
                        .append("\">");

                    if (subval.isEmpty())
                        tbody.append("-");
                    else
                        tbody.append(subval);

                    tbody.append("</td>");
                }
            }
            else{
                headers.add(key);
                String value = this.json.getString(key);
                tbody.append("<td data-value=\"")
                    .append(key)
                    .append("\">");
                if (value.isEmpty())
                    tbody.append("-");
                else
                    tbody.append(value);
                tbody.append("</td>");
            }
        }
        tbody.append("</tr></tbody>");

        //generate headers
        thead.append("<thead><tr>");
        for (String h : headers){
            thead.append("<th>").append(h).append("</th>");
        }
        thead.append("</tr></thead>");

        table.append("<table>")
            .append(thead)
            .append(tbody)
            .append("</table>");

        return table.toString();
    }

    public String getNotificationType() {
        JSONObject response = this.json
                .getJSONObject("GetNotificationResponse");
        String notificationType = response.getString("NotificationType");
        return notificationType;
    }

    public String getTransactionId() {
        JSONObject response = this.json
                .getJSONObject("GetNotificationResponse");
        String transactionId = response.getString("OriginalTransactionId");
        return transactionId;
    }
}
