package com.att.api.payment.service;

public class Subscription extends Transaction {
    private String idList;

    public Subscription() {
        //mod by 10,000,000,000 so we are under the 12 character limit and guarantee uniqueness
        this.idList = "S" + (System.currentTimeMillis() % 1000000000);
    }

    public String getMerchantSubscriptionIdList() {
        return this.idList;
    }

    public boolean isPurchaseOnNoActiveSubscription() {
        return false;
    }

    public int getSubscriptionRecurrences() {
        return 99999;
    }

    public String getSubscriptionPeriod() {
        return "MONTHLY";
    }

    public int getSubscriptionPeriodAmount() {
        return 1;
    }
}
