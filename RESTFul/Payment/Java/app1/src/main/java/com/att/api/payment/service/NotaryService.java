package com.att.api.payment.service;

import org.json.JSONObject;

import com.att.api.exception.ServiceException;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTConfig;

public class NotaryService {
    private String clientId;
    private String clientSecret;
    private RESTConfig cfg;

    private JSONObject buildJSON(Transaction args) {
        final String[] varNames = {
            "Amount", "Category", "Channel", 
            "Description", "MerchantTransactionId",
            "MerchantProductId", "MerchantPaymentRedirectUrl"
        };

        final String[] varValues = {
            args.getAmount(), args.getCategory(), args.getChannel(),
            args.getDescription(), args.getTransactionId(), 
            args.getProductId(), args.getPaymentRedirectUrl()
        };

        JSONObject jrequest = new JSONObject();
        for (int i = 0; i < varNames.length; ++i) {
            jrequest.put(varNames[i], varValues[i]);
        }
        return jrequest;
    }

    private void setHeaders(RESTClient client) {
        client
            .setHeader("Content-Type", "application/json")
            .setHeader("Accept", "application/json")
            .setHeader("Client_id", this.clientId)
            .setHeader("Client_secret", this.clientSecret);
    }

    public NotaryService(RESTConfig cfg, String clientId, String clientSecret) {

        this.cfg = cfg;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    public Notary getNotary(String rawStr) throws ServiceException {
        try {
            RESTClient client = new RESTClient(this.cfg);
            this.setHeaders(client);
            APIResponse response = client.httpPost(rawStr);

            JSONObject jresponse = new JSONObject(response.getResponseBody());
            String signedDoc = jresponse.getString("SignedDocument");
            String signature = jresponse.getString("Signature");

            return new Notary(rawStr,signedDoc, signature);
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    }

    public Notary getTransactionNotary(Transaction args) 
            throws ServiceException {

        try {
            JSONObject jrequest = this.buildJSON(args);
            return this.getNotary(jrequest.toString());
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    }

    public Notary getSubscriptionNotary(Subscription args)
            throws ServiceException {
        try {
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
        } catch (Exception e) {
            throw new ServiceException(e);
        }
    }
}
