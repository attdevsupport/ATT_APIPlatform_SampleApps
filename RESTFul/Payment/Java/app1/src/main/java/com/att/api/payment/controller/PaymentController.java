package com.att.api.payment.controller;

import com.att.api.config.AppConfig;
import com.att.api.exception.ServiceException;
import com.att.api.oauth.OAuthService;
import com.att.api.oauth.OAuthToken;
import com.att.api.payment.file.PaymentFileHandler;
import com.att.api.payment.file.TransactionEntry;
import com.att.api.payment.service.Notary;
import com.att.api.payment.service.NotaryService;
import com.att.api.payment.service.PaymentService;
import com.att.api.payment.service.Transaction;
import com.att.api.payment.service.Subscription;
import com.att.api.rest.RESTConfig;
import com.att.api.rest.RESTException;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import com.att.api.payment.model.ConfigBean;
import com.att.api.payment.file.TransactionEntry;
import com.att.api.payment.file.SubscriptionEntry;

import java.io.IOException;
import java.text.ParseException;
import java.util.Iterator;
import java.util.Map;
import java.util.List;
import java.util.ArrayList;
import java.util.TreeMap;

public class PaymentController extends HttpServlet {

    private static final long serialVersionUID = 5677394140665947979L;
    private PaymentFileHandler transactionFile;
    private PaymentFileHandler subscriptionFile;
    private AppConfig cfg;

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
        transactionFile = new PaymentFileHandler("trans.db");
        subscriptionFile = new PaymentFileHandler("subs.db");
    }

    private RESTConfig getRESTConfig(String endpoint) {
        final String proxyHost = cfg.getProxyHost();
        final int proxyPort = cfg.getProxyPort();
        final boolean trustAllCerts = cfg.getTrustAllCerts();

        return new RESTConfig(endpoint, proxyHost, proxyPort, trustAllCerts);
    }

    private Notary getTransactionNotary(String price) throws ServiceException {
        Transaction trans = new Transaction();
        trans.setAmount(price);
        trans.setCategory("5"); // 5 = category application other 
        trans.setDescription("SAMPLE APP");
        trans.setTransactionId("mtid" + System.currentTimeMillis());
        trans.setProductId("mpid" + System.currentTimeMillis());
        trans.setPaymentRedirectUrl(cfg.getProperty("paymentRedirectURL"));

        String endpoint = cfg.getFQDN() 
            + "/Security/Notary/Rest/1/SignedPayload";
        RESTConfig nCfg = this.getRESTConfig(endpoint);
        NotaryService srvc = new NotaryService(nCfg, cfg.getClientId(), 
                cfg.getClientSecret());
        return srvc.getTransactionNotary(trans);
    }

    private Notary getSubscriptionNotary(String price) throws ServiceException {
        Subscription sub = new Subscription();
        sub.setAmount(price);
        sub.setCategory("5"); // 5 = category application other 
        sub.setDescription("SAMPLE APP");
        sub.setTransactionId("mtid" + System.currentTimeMillis());
        sub.setProductId("mpid" + System.currentTimeMillis());
        sub.setPaymentRedirectUrl(cfg.getProperty("paymentRedirectURL"));

        String endpoint = cfg.getFQDN() + "/Security/Notary/Rest/1/SignedPayload";
        RESTConfig nCfg = this.getRESTConfig(endpoint);
        NotaryService srvc = new NotaryService(nCfg, cfg.getClientId(), cfg.getClientSecret());
        return srvc.getSubscriptionNotary(sub);
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

    private JSONObject getTransactionInfo(String type, String value)
            throws RESTException, ServiceException {

        OAuthToken token = this.getToken();
        String subURL = "TransactionAuthCode";
        if (type.equals("2")) {
            subURL = "TransactionId";
        } else if (type.equals("3")) {
            subURL = "MerchantTransactionId";
        }

        String endpoint = cfg.getFQDN() + 
            "/rest/3/Commerce/Payment/Transactions/" + subURL + "/" + value; 
        PaymentService srvc = new PaymentService(this.getRESTConfig(endpoint),
                token);
        return srvc.getTransactionStatus();
    }

    private JSONObject getSubscriptionInfo(String type, String value)
        throws RESTException, ServiceException {

        OAuthToken token = this.getToken();
        String subURL = "SubscriptionAuthCode";
        if (type.equals("2")) {
            subURL = "SubscriptionId";
        } else if (type.equals("3")) {
            subURL = "MerchantTransactionId";
        }

        String endpoint = cfg.getFQDN() + 
            "/rest/3/Commerce/Payment/Subscriptions/" + subURL + "/" + value; 
        PaymentService srvc = new PaymentService(this.getRESTConfig(endpoint),
                token);
        return srvc.getTransactionStatus();
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
            request.setAttribute("resultTransInfo", this.JSONToMap(info));
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
            JSONObject info = this.getTransactionInfo("1", authCode);
            subscriptionFile.addTransactionEntry(info, authCode);
            request.setAttribute("showSub", true);
            request.setAttribute("resultSubInfo", this.JSONToMap(info));
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
                    JSONObject info = this.getTransactionInfo(types[i], value);
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
            String cid = request.getParameter("getSDetailsConsumerId");
            String mid = request.getParameter("getSDetailsMSID");
            List<SubscriptionEntry> subList = createSubscriptionEntries(subscriptionFile.getTransactionEntrys());

            String merch = "";
            String consumer = "";
            //TODO: set merchid and consumerid
            if (cid != null){
                consumer = cid;
                for (SubscriptionEntry sub : subList){
                    if (sub.getConsumerId().equals(cid))
                        merch = sub.getMerchantSubId();
                }
            }
            else if (mid != null){
                merch = mid;
                for (SubscriptionEntry sub : subList){
                    if (sub.getMerchantSubId().equals(mid))
                        consumer = sub.getConsumerId();
                }
            }
            else
                return;

            OAuthToken token = this.getToken();
            String endpoint = cfg.getFQDN() + "/rest/3/Commerce/Payment/Subscriptions/" + merch + "/Detail/" + consumer; 
            JSONObject info = new PaymentService(this.getRESTConfig(endpoint), token).getSubscriptionDetails();
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
            String cost = cfg.getProperty("minTransactionAmount");
            if (prod.equals("2")) {
                cost = cfg.getProperty("maxTransactionAmount");
            }

            Notary notary = this.getTransactionNotary(cost);
            request.getSession().setAttribute("notary", notary);

            String FQDN = cfg.getFQDN();
            String cid = cfg.getClientId();
            String URL = PaymentService.getNewTransactionURL(FQDN, cid, notary);
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
            String cost = cfg.getProperty("minSubscriptionAmount");
            if (prod.equals("2")) {
                cost = cfg.getProperty("maxSubscriptionAmount");
            }

            Notary notary = this.getSubscriptionNotary(cost);
            request.getSession().setAttribute("notary", notary);

            String FQDN = cfg.getFQDN();
            String cid = cfg.getClientId();
            String URL = PaymentService.getNewSubscriptionURL(FQDN, cid, notary);
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
            OAuthToken token = this.getToken();
            String endpoint = cfg.getFQDN() + 
                "/rest/3/Commerce/Payment/Transactions/" + tid; 
            PaymentService srvc 
                = new PaymentService(this.getRESTConfig(endpoint), token);
            JSONObject info = srvc.refundTransaction("Sample App Test", 1);
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
            OAuthToken token = this.getToken();
            String endpoint = cfg.getFQDN() + 
                "/rest/3/Commerce/Payment/Transactions/" + sid; 
            PaymentService srvc 
                = new PaymentService(this.getRESTConfig(endpoint), token);
            JSONObject info = srvc.refundSubscription("Sample App Test", 1);
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
            OAuthToken token = this.getToken();
            String endpoint = cfg.getFQDN() + 
                "/rest/3/Commerce/Payment/Transactions/" + sid; 
            PaymentService srvc 
                = new PaymentService(this.getRESTConfig(endpoint), token);
            JSONObject info = srvc.cancelSubscription("Sample App Test", 1);
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
            String endpoint = cfg.getFQDN() 
                + "/Security/Notary/Rest/1/SignedPayload";
            NotaryService srvc = new NotaryService(this.getRESTConfig(endpoint)
                    , cfg.getClientId(), cfg.getClientSecret());
            Notary notary = srvc.getNotary(payload);
            request.setAttribute("showNotary", true);
            request.setAttribute("resultNotaryDoc", notary.getSignedDocument());
            request.setAttribute("resultNotarySig", notary.getSignature());
        } catch (Exception e) {
            request.setAttribute("showNotary", true);
            request.setAttribute("errorNotary", e.getMessage());
        }
    }

    private void setFileResults(HttpServletRequest request) {
        this.createFileHandler();
        try {
            request.setAttribute("transactions", createTransactionEntries(transactionFile.getTransactionEntrys()));
            request.setAttribute("subscriptions", createSubscriptionEntries(subscriptionFile.getTransactionEntrys()));
        } catch (IOException e) {
            // don't handle, print stack trace
            e.printStackTrace();
        } catch (ParseException e){
            // don't handle, print stack trace
            e.printStackTrace();
        }
    }

    @Override
    public void init() {
        try {
            cfg = AppConfig.getInstance();
            this.createFileHandler();
        } catch (IOException ioe) {
            // don't handle, just print stack trace
            ioe.printStackTrace();
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse 
            response) throws ServletException, IOException {
        doGet(request,response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse 
            response) throws ServletException, IOException {

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

        request.setAttribute("cfg", new ConfigBean());

        // forward to view
        String forwardStr = "/WEB-INF/payment.jsp";
        request.getRequestDispatcher(forwardStr).forward(request, response);
    }
}
