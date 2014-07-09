package com.att.api.payment.controller;

import java.io.IOException;
import java.io.InputStream;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.att.api.controller.APIController;
import com.att.api.payment.model.Notification;
import com.att.api.payment.model.NotificationPool;
import com.att.api.rest.RESTException;

public class PaymentListener extends APIController {
    /**
     *
     */
    private static final long serialVersionUID = 2252096678488146193L;


    public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws ServletException, IOException {
        doPost(request, response);
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response) {
        try {
            Notification notification =
                Notification.fromXml(request.getInputStream());

            NotificationPool pool = NotificationPool.getInstance();

            int maxNotifications =
                Integer.valueOf(appConfig.getProperty("maxNotifications"));

            pool.setMaxNotifications(maxNotifications);
            pool.addNotification(notification);
        } catch (RESTException ex) {
            // Just print stack trace as we don't want to crash the app cause
            // of a bad notification request
            ex.printStackTrace();
        } catch (IOException e) {
            // Just print stack trace as we don't want to crash the app cause
            // of a bad notification request
            e.printStackTrace();
        }
    }
}
