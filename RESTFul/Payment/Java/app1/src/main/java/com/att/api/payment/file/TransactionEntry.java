package com.att.api.payment.file;

import org.json.JSONObject;

public class TransactionEntry {
    protected JSONObject json;

    public TransactionEntry(JSONObject json) {
        this.json = json;
    }

    public String getId() {
        return this.json.getString("TransactionId");
    }

    public String getMerchantId() {
        return this.json.getString("MerchantTransactionId");
    }

    public String getAuthCode() {
        return this.json.getString("authCode");
    }

    public JSONObject getJSON(){
        return this.json;
    }
}
