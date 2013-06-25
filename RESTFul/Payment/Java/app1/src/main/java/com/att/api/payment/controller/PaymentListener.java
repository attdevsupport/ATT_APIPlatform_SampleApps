package com.att.api.payment.controller;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.json.JSONObject;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import com.att.api.config.AppConfig;
import com.att.api.controller.APIController;
import com.att.api.exception.ServiceException;
import com.att.api.payment.model.Notification;
import com.att.api.payment.model.NotificationPool;
import com.att.api.payment.service.PaymentService;
import com.att.api.rest.RESTException;

public class PaymentListener extends APIController {
    /**
     *
     */
    private static final long serialVersionUID = 2252096678488146193L;
    private AppConfig cfg;

    private Document buildDocument(HttpServletRequest request)
        throws ParserConfigurationException, SAXException, IOException {

        DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance()
            .newDocumentBuilder();

        return docBuilder.parse(request.getInputStream());
    }

    private Notification getNotification(String id) 
        throws RESTException {
        PaymentService srvc = new PaymentService(appConfig.getFQDN(),
                getFileToken());

        Notification notification = new Notification(srvc.getNotification(id), id);

        return notification;
    }

    private JSONObject deleteNotification(String id) 
        throws RESTException {

        PaymentService srvc = new PaymentService(appConfig.getFQDN(),getFileToken());

        return srvc.deleteNotification(id);
    }

    private List<Notification> parseNotifications(Document xml)
            throws RESTException, ServiceException {
        NodeList elements = xml.getElementsByTagName("hub:notificationId");
        List<Notification> notificationList = new LinkedList<Notification>();

        for (int i = 0; i < elements.getLength(); ++i) {
            Node node = elements.item(i);
            String notificationId = node.getTextContent();
            Notification notification = this.getNotification(notificationId);
            notificationList.add(notification);
            this.deleteNotification(notificationId);
        }
        return notificationList;
    }

    private void updateNotifications(List<Notification> notifications){
        NotificationPool pool = NotificationPool.getInstance();
        int maxNotifications = Integer.valueOf(appConfig.getProperty("maxNotifications"));
        pool.setMaxNotifications(maxNotifications);
        pool.updateNotifications(notifications);
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        doGet(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        try {

            Document xmlDoc = this.buildDocument(request);
            List<Notification> notifications = this.parseNotifications(xmlDoc);
            this.updateNotifications(notifications);
            
        } catch (Exception e) {
            //print notification errors
            e.printStackTrace();
        } 
    }
}
