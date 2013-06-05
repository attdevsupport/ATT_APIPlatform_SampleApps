package com.att.api.payment.service;

import com.att.api.exception.ServiceException;
import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;
import org.json.JSONObject;


public class PaymentService {
    private RESTConfig cfg;
    private OAuthToken token;

    private static String getURL(String FQDN, String cid, Notary notary, 
            boolean isTrans) {
        String type = isTrans ? "Transactions" : "Subscriptions";

        String URL = FQDN + "/rest/3/Commerce/Payment/" + type;
        String signedDoc = notary.getSignedDocument();
        String signature = notary.getSignature();
        return URL + "?clientid=" + cid + "&SignedPaymentDetail=" + signedDoc + "&Signature=" + signature;
    }
    
    private JSONObject putURL(String body) throws ServiceException {
        try {
            APIResponse response = 
                new RESTClient(cfg)
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(this.token)
                .httpPut(body);

            return new JSONObject(response.getResponseBody());
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    }

    private JSONObject getInfo() throws ServiceException {
        try {
            APIResponse response = 
                new RESTClient(cfg)
                .setHeader("Accept", "application/json")
                .addAuthorizationHeader(this.token)
                .httpGet();

            return new JSONObject(response.getResponseBody());
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    }

    private JSONObject sendTransOptStatus(String rReasonTxt, int rReasonCode, 
            String transOptStatus) throws ServiceException {

        try {
            JSONObject req = new JSONObject();
            req.put("TransactionOperationStatus", transOptStatus);
            req.put("RefundReasonCode", rReasonCode);
            req.put("RefundReasonText", rReasonTxt);

            APIResponse response = 
                new RESTClient(this.cfg)
                .setHeader("Accept", "application/json")
                .setHeader("Content-Type", "application/json")
                .addAuthorizationHeader(this.token)
                .httpPut(req.toString());

            return new JSONObject(response.getResponseBody());
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    } 
        
    public PaymentService(RESTConfig cfg, OAuthToken token) {
        this.cfg = cfg;
        this.token = token;
    }

    public JSONObject getTransactionStatus() throws ServiceException {
        return this.getInfo();
    }

    public JSONObject getSubscriptionStatus() throws ServiceException {
        return this.getInfo();
    }

    public JSONObject getSubscriptionDetails() throws ServiceException {
        return this.getInfo();
    }

    public JSONObject getNotification() throws ServiceException {
        return this.getInfo();
    }

    public JSONObject deleteNotification() throws ServiceException {
        return this.putURL("");
    }

    public JSONObject cancelSubscription(String reasonTxt, int reasonCode)
            throws ServiceException {

        String type = "SubscriptionCancelled";
        return this.sendTransOptStatus(reasonTxt, reasonCode, type);
    }

    public JSONObject refundSubscription(String reasonTxt, int reasonCode)
            throws ServiceException {

        return this.sendTransOptStatus(reasonTxt, reasonCode, "Refunded");
    }

    public JSONObject refundTransaction(String reasonTxt, int reasonCode)
            throws ServiceException {

        return this.sendTransOptStatus(reasonTxt, reasonCode, "Refunded");
    }


    public static String getNewTransactionURL(String FQDN, String clientId,
            Notary notary) {
        return PaymentService.getURL(FQDN, clientId, notary, true);
    }
        
    public static String getNewSubscriptionURL(String FQDN, String clientId,
            Notary notary) {
        return PaymentService.getURL(FQDN, clientId, notary, false);
    }

}
