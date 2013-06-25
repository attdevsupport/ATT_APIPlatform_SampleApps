package com.att.api.payment.service;

import java.text.ParseException;

import org.json.JSONObject;

import com.att.api.payment.model.Notary;
import com.att.api.payment.model.Subscription;
import com.att.api.payment.model.Transaction;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

public class NotaryService extends APIService {
    private String clientId;
    private String clientSecret;

    private JSONObject buildJSON(Transaction args) {
        final String[] varNames = { "Amount", "Category", "Channel",
                "Description", "MerchantTransactionId", "MerchantProductId",
                "MerchantPaymentRedirectUrl" };

        final String[] varValues = { 
            String.valueOf(args.getAmount()), 
            String.valueOf(args.getAppCategory().getValue()),
            args.getChannel(), args.getDescription(),
            args.getTransactionId(), args.getProductId(),
            args.getPaymentRedirectUrl() 
        };

        JSONObject jrequest = new JSONObject();
        for (int i = 0; i < varNames.length; ++i) {
            jrequest.put(varNames[i], varValues[i]);
        }
        return jrequest;
    }

    private void setHeaders(RESTClient client) {
        client.setHeader("Content-Type", "application/json")
                .setHeader("Accept", "application/json")
                .setHeader("Client_id", this.clientId)
                .setHeader("Client_secret", this.clientSecret);
    }

    public NotaryService(String fqdn, String clientId, 
            String clientSecret) {
        super(fqdn, null);
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    public Notary getNotary(String rawStr) throws RESTException {
        String url = getFQDN() + "/Security/Notary/Rest/1/SignedPayload";
        RESTClient client = new RESTClient(url);
        this.setHeaders(client);
        APIResponse response = client.httpPost(rawStr);

        try {
            JSONObject jresponse = new JSONObject(response.getResponseBody());
            String signedDoc = jresponse.getString("SignedDocument");
            String signature = jresponse.getString("Signature");

            return new Notary(rawStr, signedDoc, signature);
        } catch (ParseException e){
            throw new RESTException(e);
        }
    }

    public Notary getTransactionNotary(Transaction args) throws RESTException {
        JSONObject jrequest = this.buildJSON(args);
        return this.getNotary(jrequest.toString());
    }

    public Notary getSubscriptionNotary(Subscription args) 
            throws RESTException {
        JSONObject jrequest = this.buildJSON(args);
        jrequest.put("IsPurchaseOnNoActiveSubscription", 
                args.isPurchaseOnNoActiveSubscription());
        jrequest.put("SubscriptionRecurrences", 
                args.getSubscriptionRecurrences());
        jrequest.put("SubscriptionPeriod", 
                args.getSubscriptionPeriod());
        jrequest.put("SubscriptionPeriodAmount", 
                args.getSubscriptionPeriodAmount());
        jrequest.put("MerchantSubscriptionIdList",
                args.getMerchantSubscriptionIdList());

        return this.getNotary(jrequest.toString());
    }
}
