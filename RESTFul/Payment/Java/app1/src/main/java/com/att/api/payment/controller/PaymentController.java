package com.att.api.payment.controller;

import java.io.IOException;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;

import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;
import com.att.api.payment.file.PaymentFileHandler;
import com.att.api.payment.file.SubscriptionEntry;
import com.att.api.payment.file.TransactionEntry;
import com.att.api.payment.model.AppCategory;
import com.att.api.payment.model.ConfigBean;
import com.att.api.payment.model.Notary;
import com.att.api.payment.model.NotificationPool;
import com.att.api.payment.model.Subscription;
import com.att.api.payment.model.Transaction;
import com.att.api.payment.service.NotaryService;
import com.att.api.payment.service.PaymentService;
import com.att.api.rest.RESTException;

public class PaymentController extends APIController {

    private static final long serialVersionUID = 5677394140665947979L;
    private static PaymentFileHandler transactionFile = null;
    private static PaymentFileHandler subscriptionFile = null;

    private Map<String, String> JSONToMap(JSONObject obj) {
        Map<String, String> map = new TreeMap<String, String>();
        @SuppressWarnings("rawtypes")
        Iterator keys = obj.keys();
        while (keys.hasNext()) {
            String key = (String) keys.next();
            map.put(key, obj.getString(key));
        }
        return map;
    }

    private List<TransactionEntry> createTransactionEntries(List<JSONObject> list){
        List<TransactionEntry> entries = new ArrayList<TransactionEntry>();
        for(JSONObject json : list){
            entries.add(new TransactionEntry(json));
        }
        return entries;
    }

    private List<SubscriptionEntry> createSubscriptionEntries(List<JSONObject> list){
        List<SubscriptionEntry> entries = new ArrayList<SubscriptionEntry>();
        for(JSONObject json : list){
            entries.add(new SubscriptionEntry(json));
        }
        return entries;
    }

    private void createFileHandler() {
        try {
            if (transactionFile == null || subscriptionFile == null) {
                transactionFile = new PaymentFileHandler("trans", "db");
                subscriptionFile = new PaymentFileHandler("subs", "db");
            }
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
    }

    private Notary getTransactionNotary(double price) throws RESTException {
        String transId = "J" + System.currentTimeMillis();
        String prodId = appConfig.getProperty("merchantProductId");
        String redirectUrl = appConfig.getProperty("paymentRedirectURL");
        Transaction trans = new Transaction.Builder(price, AppCategory.APP_OTHER,
                "Sample App", transId, prodId, redirectUrl).build();

        NotaryService srvc = new NotaryService(appConfig.getFQDN(), appConfig.getClientId(), 
                appConfig.getClientSecret());
        return srvc.getTransactionNotary(trans);
    }

    private Notary getSubscriptionNotary(double price) throws RESTException {
        String transId = "J" + System.currentTimeMillis();
        String mpid = appConfig.getProperty("merchantProductId");
        String redirectUrl = appConfig.getProperty("paymentRedirectURL");
        String merchSubIdList = appConfig.getProperty("merchantSubscriptionId");

        Subscription sub = new Subscription.Builder(price, AppCategory.APP_OTHER,
                "Sample App", transId, mpid, redirectUrl, merchSubIdList).build();

        NotaryService srvc = new NotaryService(appConfig.getFQDN(), appConfig.getClientId(), 
                appConfig.getClientSecret());
        return srvc.getSubscriptionNotary(sub);
    }

    private JSONObject getTransactionInfo(String type, String value)
        throws RESTException {

        //TODO: update this to enums
        String transType = "TransactionAuthCode";
        if (type.equals("2")) {
            transType = "TransactionId";
        } else if (type.equals("3")) {
            transType = "MerchantTransactionId";
        }

        PaymentService srvc = new PaymentService(appConfig.getFQDN(),
                getFileToken());
        return srvc.getTransactionStatus(transType, value);
    }

    private JSONObject getSubscriptionInfo(String type, String value)
        throws RESTException {

        String subType = "SubscriptionAuthCode";
        if (type.equals("2")) {
            subType = "SubscriptionId";
        } else if (type.equals("3")) {
            subType = "MerchantTransactionId";
        }

        PaymentService srvc = new PaymentService(appConfig.getFQDN(),
                getFileToken());
        return srvc.getSubscriptionStatus(subType, value);
    }

    private void handleTransactionAuthCode(HttpServletRequest request) {
        try {
            if (request.getParameter("TransactionAuthCode") == null) {
                return;
            }

            String authCode = (String) request.getParameter("TransactionAuthCode");
            JSONObject info = this.getTransactionInfo("1", authCode);
            transactionFile.addTransactionEntry(info, authCode);
            request.setAttribute("showTrans", true);
            Map<String, String> m = this.JSONToMap(info);
            m.remove("authCode");
            request.setAttribute("resultTransInfo", m);
        } catch (Exception e) {
            request.setAttribute("showTrans", true);
            request.setAttribute("errorTransAuthCode", e.getMessage());
        }
    }

    private void handleSubscriptionAuthCode(HttpServletRequest request){
        try {
            if (request.getParameter("SubscriptionAuthCode") == null) {
                return;
            }
            String authCode = (String) request.getParameter("SubscriptionAuthCode");
            JSONObject info = this.getSubscriptionInfo("1", authCode);
            subscriptionFile.addTransactionEntry(info, authCode);
            request.setAttribute("showSub", true);
            Map<String, String> m = this.JSONToMap(info);
            m.remove("authCode");
            request.setAttribute("resultSubInfo", m);
        } catch (Exception e) {
            request.setAttribute("showSub", true);
            request.setAttribute("errorSubAuthCode", e.getMessage());
        }

    }

    private void handleGetTransactionStatus(HttpServletRequest request) {
        try {
            String[] names = { "getTransactionMTID", "getTransactionAuthCode",
                "getTransactionTID" };
            String[] types = { "3", "1", "2" };

            for (int i = 0; i < names.length; ++i) {
                String name = names[i];
                if (request.getParameter(name) != null) {
                    String value = request.getParameter(name);
                    JSONObject info = this.getTransactionInfo(types[i], value);
                    request.setAttribute("showTrans", true);
                    request.setAttribute("resultTransInfo", this.JSONToMap(info));
                    return;
                }
            }
        } catch (Exception e) {
            request.setAttribute("showTrans", true);
            request.setAttribute("errorTransInfo", e.getMessage());
        }
    }

    private void handleGetSubscriptionStatus(HttpServletRequest request) {
        try {
            String[] names = { "getSubscriptionMTID", "getSubscriptionAuthCode",
                "getSubscriptionTID" };
            String[] types = { "3", "1", "2" };

            for (int i = 0; i < names.length; ++i) {
                String name = names[i];
                if (request.getParameter(name) != null) {
                    String value = request.getParameter(name);
                    JSONObject info = this.getSubscriptionInfo(types[i], value);
                    request.setAttribute("showSub", true);
                    request.setAttribute("resultSubInfo", this.JSONToMap(info));
                    return;
                }
            }
        } catch (Exception e) {
            request.setAttribute("showSub", true);
            request.setAttribute("errorSubInfo", e.getMessage());
        }
    }

    private void handleGetSubscriptionDetail(HttpServletRequest request) {
        try {
            String mid = request.getParameter("getSDetailsMSID");

            if (mid == null)
                return;

            List<SubscriptionEntry> subList = createSubscriptionEntries(subscriptionFile.getTransactionEntrys());

            String merch = mid;
            String consumer = "";

            for (SubscriptionEntry sub : subList){
                if (sub.getMerchantSubId().equals(mid))
                    consumer = sub.getConsumerId();
            }

            OAuthToken token = this.getFileToken();

            JSONObject info = new PaymentService(appConfig.getFQDN(), 
                    token).getSubscriptionDetails(merch, consumer);

            request.setAttribute("showSub", true);
            request.setAttribute("resultSubDetail", this.JSONToMap(info));
        } catch (Exception e) {
            request.setAttribute("showSub", true);
            request.setAttribute("errorSubDetail", e.getMessage());
        }
    }

    private boolean handleNewTransaction(HttpServletRequest request,
            HttpServletResponse response) {

        try {
            if (request.getParameter("newTransaction") == null) {
                // no redirect
                return false;
            }

            String prod = (String) request.getParameter("product");
            String cost = appConfig.getProperty("minTransactionAmount");
            if (prod.equals("2")) {
                cost = appConfig.getProperty("maxTransactionAmount");
            }

            Notary notary = this.getTransactionNotary(Double.valueOf(cost));
            request.getSession().setAttribute("notary", notary);

            String URL = PaymentService.getNewTransactionURL(
                    appConfig.getFQDN(), appConfig.getClientId(), notary);
            response.sendRedirect(URL);
            // redirect required
            return true;
        } catch (Exception e) {
            request.setAttribute("errorNewTrans", e.getMessage());
            return false;
        }
    }

    private boolean handleNewSubscription(HttpServletRequest request,
            HttpServletResponse response) {

        try {
            if (request.getParameter("newSubscription") == null) {
                // no redirect
                return false;
            }

            String prod = (String) request.getParameter("product");
            String cost = appConfig.getProperty("minSubscriptionAmount");
            if (prod.equals("2")) {
                cost = appConfig.getProperty("maxSubscriptionAmount");
            }

            Notary notary = this.getSubscriptionNotary(Double.valueOf(cost));
            request.getSession().setAttribute("notary", notary);

            String URL = PaymentService.getNewSubscriptionURL(
                    appConfig.getFQDN(), appConfig.getClientId(), notary);
            response.sendRedirect(URL);
            // redirect required
            return true;
        } catch (Exception e) {
            request.setAttribute("errorNewSub", e.getMessage());
            return false;
        }
    }

    private void handleTransRefund(HttpServletRequest request) {
        try {
            if (request.getParameter("refundTransactionId") == null) {
                return;
            }
            String tid = (String) request.getParameter("refundTransactionId");

            PaymentService srvc = new PaymentService(appConfig.getFQDN(), 
                    this.getFileToken());

            JSONObject info = srvc.refundTransaction(tid, 
                    "Sample App Test", 1);

            request.setAttribute("showTrans", true);
            request.setAttribute("resultTRefund", this.JSONToMap(info));
        } catch (Exception e) {
            request.setAttribute("showTrans", true);
            request.setAttribute("errorTRefund", e.getMessage());
        }
    }

    private void handleSubscriptionRefund(HttpServletRequest request){
        try {
            if (request.getParameter("refundSubscriptionId") == null) {
                return;
            }
            String sid = (String) request.getParameter("refundSubscriptionId");

            PaymentService srvc = new PaymentService(appConfig.getFQDN(), 
                    getFileToken());

            JSONObject info = srvc.refundSubscription(sid, "Sample App Test", 1);

            request.setAttribute("showSub", true);
            request.setAttribute("resultSRefund", this.JSONToMap(info));
        } catch (Exception e) {
            request.setAttribute("showSub", true);
            request.setAttribute("errorSRefund", e.getMessage());
        }
    }

    private void handleSubscriptionCancel(HttpServletRequest request){
        try {
            if (request.getParameter("cancelSubscriptionId") == null) {
                return;
            }
            String sid = (String) request.getParameter("cancelSubscriptionId");

            PaymentService srvc = new PaymentService(appConfig.getFQDN(), 
                    getFileToken());

            JSONObject info = srvc.cancelSubscription(sid, "Sample App Test", 1);
            request.setAttribute("showSub", true);
            request.setAttribute("resultSCancel", this.JSONToMap(info));
        } catch (Exception e) {
            request.setAttribute("showSub", true);
            request.setAttribute("errorSCancel", e.getMessage());
        }
    }

    private void handleNotary(HttpServletRequest request) {
        try {
            if (request.getParameter("signPayload") == null) {
                Notary notary = (Notary) request.getSession().getAttribute("notary");
                if (notary != null){
                    request.setAttribute("showNotary", true);
                    request.setAttribute("notaryPayload", notary.getPayload().trim());
                    request.setAttribute("resultNotaryDoc", notary.getSignedDocument());
                    request.setAttribute("resultNotarySig", notary.getSignature());
                }
                return;
            }
            String payload = (String) request.getParameter("payload");

            NotaryService srvc = new NotaryService(appConfig.getFQDN(), 
                    appConfig.getClientId(), appConfig.getClientSecret());

            Notary notary = srvc.getNotary(payload);
            request.setAttribute("showNotary", true);
            request.setAttribute("notaryPayload", notary.getPayload().trim());
            request.setAttribute("resultNotaryDoc", notary.getSignedDocument());
            request.setAttribute("resultNotarySig", notary.getSignature());
        } catch (Exception e) {
            request.setAttribute("showNotary", true);
            request.setAttribute("errorNotary", e.getMessage());
        }
    }

    private void handleNotifications(HttpServletRequest request) {
        NotificationPool pool = NotificationPool.getInstance();
        request.setAttribute("notifications", pool.getNotifications());
        if (request.getParameter("refreshNotifications") != null)
            request.setAttribute("showNote", true);
    }

    private void setFileResults(HttpServletRequest request) {
        this.createFileHandler();
        try {
            request.setAttribute("transactions", createTransactionEntries(
                        transactionFile.getTransactionEntrys()));

            request.setAttribute("subscriptions", createSubscriptionEntries(
                        subscriptionFile.getTransactionEntrys()));
        } catch (IOException e) {
            // don't handle, print stack trace
            e.printStackTrace();
        } catch (ParseException e){
            // don't handle, print stack trace
            e.printStackTrace();
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse 
            response) throws ServletException, IOException {
        doGet(request,response);
            }

    public void doGet(HttpServletRequest request, HttpServletResponse 
            response) throws ServletException, IOException {

        request.setAttribute("cfg", new ConfigBean());

        this.handleNotifications(request);

        // handle transactions
        if (this.handleNewTransaction(request, response)) {
            return;
        }
        this.handleTransactionAuthCode(request); 
        this.handleGetTransactionStatus(request);
        this.handleTransRefund(request);

        // handle notary
        this.handleNotary(request);

        //handle subscriptions
        if (this.handleNewSubscription(request, response)) {
            return;
        }
        this.handleSubscriptionAuthCode(request);
        this.handleGetSubscriptionStatus(request);
        this.handleGetSubscriptionDetail(request);
        this.handleSubscriptionRefund(request);
        this.handleSubscriptionCancel(request);

        this.setFileResults(request);

        // forward to view
        String forwardStr = "/WEB-INF/payment.jsp";
        request.getRequestDispatcher(forwardStr).forward(request, response);
            }
}
