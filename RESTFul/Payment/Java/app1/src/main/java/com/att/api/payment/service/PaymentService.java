package com.att.api.payment.service;

import java.text.ParseException;

import com.att.api.oauth.OAuthToken;
import com.att.api.payment.model.Notary;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.json.JSONObject;

public class PaymentService extends APIService {
    private static final String TRANS_SUBURL = 
        "/rest/3/Commerce/Payment/Transactions";
    private static final String SUBSCRIPTION_SUBURL = 
        "/rest/3/Commerce/Payment/Subscriptions";
    public static final String NOTIFICATION_SUBURL = 
        "/rest/3/Commerce/Payment/Notifications"; 
    public static final String SIG_SUBURL = 
        "/Security/Notary/Rest/1/SignedPayload";

    private static String generateUrl(String FQDN, String cid, Notary notary, 
            String type) {
        String URL = FQDN + "/rest/3/Commerce/Payment/" + type;
        String signedDoc = notary.getSignedDocument();
        String signature = notary.getSignature();
        return URL + "?clientid=" + cid + "&SignedPaymentDetail=" + 
            signedDoc + "&Signature=" + signature;
    }

    private JSONObject getPaymentInfo(final String suburl) throws RESTException {
        final String url = getFQDN() + suburl;
        APIResponse response = 
            new RESTClient(url)
            .setHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken())
            .httpGet();

        try {
            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }

    private JSONObject sendTransOptStatus(String rReasonTxt, int rReasonCode, 
            String transOptStatus, String suburl) throws RESTException {
        JSONObject req = new JSONObject();
        req.put("TransactionOperationStatus", transOptStatus);
        req.put("RefundReasonCode", rReasonCode);
        req.put("RefundReasonText", rReasonTxt);

        String url = getFQDN() + suburl;
        APIResponse response = 
            new RESTClient(url)
            .setHeader("Accept", "application/json")
            .setHeader("Content-Type", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPut(req.toString());

        try {
            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    } 
        
    public PaymentService(String fqdn, OAuthToken token) {
        super(fqdn, token);
    }

    public JSONObject getTransactionStatus(String type, String id) throws RESTException {
        String surl = TRANS_SUBURL + "/" + type + "/" + id;
        return this.getPaymentInfo(surl);
    }

    public JSONObject getSubscriptionStatus(String type, String id) throws RESTException {
        String surl = SUBSCRIPTION_SUBURL + "/" + type + "/" + id;
        return this.getPaymentInfo(surl);
    }

    public JSONObject getSubscriptionDetails(String merchId, 
            String consumerId) throws RESTException {
        String surl = SUBSCRIPTION_SUBURL + "/" + merchId + "/Detail/" + consumerId;
        return this.getPaymentInfo(surl);
    }

    public JSONObject getNotification(String id) throws RESTException {
        String surl = NOTIFICATION_SUBURL + "/" + id;
        return this.getPaymentInfo(surl);
    }

    public JSONObject deleteNotification(String id) throws RESTException {
        final String url = getFQDN() + PaymentService.NOTIFICATION_SUBURL + 
            "/" + id;
        APIResponse response = 
            new RESTClient(url)
            .setHeader("Accept", "application/json")
            .addAuthorizationHeader(getToken())
            .httpPut("");

        try {
            return new JSONObject(response.getResponseBody());
        } catch (ParseException e) {
            throw new RESTException(e);
        }
    }

    public JSONObject cancelSubscription(String subId, String reasonTxt, 
            int reasonCode) throws RESTException {
        String surl = TRANS_SUBURL + "/" + subId;
        return this.sendTransOptStatus(reasonTxt, reasonCode, 
                "SubscriptionCancelled", surl);
    }

    public JSONObject refundSubscription(String subId, String reasonTxt, 
            int reasonCode) throws RESTException {
        String surl = TRANS_SUBURL + "/" + subId;
        return this.sendTransOptStatus(reasonTxt, reasonCode, "Refunded", surl);
    }

    public JSONObject refundTransaction(String transId, String reasonTxt, 
            int reasonCode) throws RESTException {
        String surl = TRANS_SUBURL + "/" + transId;

        return this.sendTransOptStatus(reasonTxt, reasonCode, "Refunded", surl);
    }


    public static String getNewTransactionURL(String FQDN, String clientId,
            Notary notary) {
        return PaymentService.generateUrl(FQDN, clientId, notary, 
                "Transactions");
    }
        
    public static String getNewSubscriptionURL(String FQDN, String clientId,
            Notary notary) {
        return PaymentService.generateUrl(FQDN, clientId, notary, 
                "Subscriptions");
    }

}
