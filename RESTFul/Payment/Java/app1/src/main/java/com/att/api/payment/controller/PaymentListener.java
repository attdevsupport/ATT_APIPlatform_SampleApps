package com.att.api.payment.controller;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
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
import com.att.api.exception.ServiceException;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.payment.model.Notification;
import com.att.api.payment.model.NotificationPool;
import com.att.api.payment.service.PaymentService;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

public class PaymentListener extends HttpServlet {
    private AppConfig cfg;

    private Document buildDocument(HttpServletRequest request)
        throws ParserConfigurationException, SAXException, IOException {

        DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance()
            .newDocumentBuilder();

        return docBuilder.parse(request.getInputStream());
    }

    @Override
    public void init() {
        try {
            cfg = AppConfig.getInstance();
        } catch (IOException ioe) {
            // don't handle, just print stack trace
            ioe.printStackTrace();
        }
    }

    private OAuthToken getToken() throws RESTException {
        try {
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

    private Notification getNotification(String id) 
        throws RESTException, ServiceException {
        OAuthToken token = this.getToken();

        String endpoint = cfg.getFQDN() + 
            "/rest/3/Commerce/Payment/Notifications/" + id;
        PaymentService srvc = new PaymentService(this.getRESTConfig(endpoint),
                token);

        Notification notification = new Notification(srvc.getNotification(), id);

        return notification;
    }

    private JSONObject deleteNotification(String id) 
        throws RESTException, ServiceException {
        OAuthToken token = this.getToken();

        String endpoint = cfg.getFQDN() + 
            "/rest/3/Commerce/Payment/Notifications/" + id;
        PaymentService srvc = new PaymentService(this.getRESTConfig(endpoint),
                token);

        return srvc.deleteNotification();
    }

    private RESTConfig getRESTConfig(String endpoint) {
        final String proxyHost = cfg.getProxyHost();
        final int proxyPort = cfg.getProxyPort();
        final boolean trustAllCerts = cfg.getTrustAllCerts();

        return new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts);
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
        pool.setMaxNotifications(cfg.getMaxNotifications());
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
            
        } catch (SAXException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (RESTException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (ServiceException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (ParserConfigurationException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } 
    }
}
