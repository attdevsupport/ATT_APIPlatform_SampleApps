package com.att.api.payment.model;

import java.util.HashMap;
import org.json.JSONObject;
import java.util.Iterator;

public class TransactionStatus {

    private HashMap<String,String> parameters;
    private String errorResponse;
    private String authCode;
    private String transactionId;
    private String merchantTransactionId;
    private String consumerId;

    public TransactionStatus()
    {
        parameters = new HashMap<String, String>();
    }

    public TransactionStatus(String errorResponse)
    {
        parameters = new HashMap<String, String>();
        this.errorResponse = errorResponse;
    }

    public TransactionStatus(JSONObject json){
        parameters = convertJsonToMap(json);
    }

    public void addParameter(String name, String value)
    {
        parameters.put(name,value);
    }

    public String getParameterValue(String name){
        return parameters.get(name);
    }

    public HashMap<String,String> getParameters()
    {
        return parameters;
    }

    public String getErrorResponse() {
        return errorResponse;
    }

    /**
     * Payment transaction id
     * @return
     */
    public String getTransactionId()
    {
        if (this.transactionId != null) return this.transactionId;
        return getParameterValue("TransactionId");
    }

    public String getConsumerId()
    {
        if (this.consumerId !=null) return this.consumerId;
        return getParameterValue("ConsumerId");
    }

    /**
     * Merchant's transaction id
     * @return
     */
    public String getMerchantTransactionId()
    {
        if (this.merchantTransactionId !=null) return this.merchantTransactionId;
        return getParameterValue("MerchantTransactionId");
    }

    public void setAuthCode(String authCode) {
        this.authCode = authCode;
    }

    public String getAuthCode() {
        return authCode;
    }

    public void setTransactionId(String transactionId) {
        this.transactionId = transactionId;
    }

    public void setMerchantTransactionId(String merchantTransactionId) {
        this.merchantTransactionId = merchantTransactionId;
    }

    public void setConsumerId(String consumerId) {
        this.consumerId = consumerId;
    }

    /**
     * Converts a simple key-value pair json object to a hashmap
     * Will not take care of embedded json elements in value
     */
    private HashMap<String, String> convertJsonToMap(JSONObject json) {
        HashMap<String, String> map = new HashMap<String, String>();

        Iterator itr = json.keys();
        while (itr.hasNext()){
            String key = (String) itr.next();
            map.put(key, json.getString(key));
        }
        return map;
    }
}
