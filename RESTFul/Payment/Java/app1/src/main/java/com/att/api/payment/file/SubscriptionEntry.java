package com.att.api.payment.file;

import org.json.JSONObject;

public class SubscriptionEntry extends TransactionEntry {

    public SubscriptionEntry(JSONObject json) {
        super(json);
    }

    public String getId() {
        return super.json.getString("SubscriptionId");
    }

    public String getConsumerId(){
        return super.json.getString("ConsumerId");
    }

    public String getMerchantSubId(){
        return super.json.getString("MerchantSubscriptionId");
    }
}
