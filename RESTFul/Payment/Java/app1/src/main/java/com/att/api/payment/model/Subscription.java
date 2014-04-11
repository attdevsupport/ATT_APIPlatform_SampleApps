package com.att.api.payment.model;

public class Subscription extends Transaction {
    private String merchSubIdList;
    private boolean iponas;
    private int recurrences;
    private String subPeriod;
    private int subPeriodAmount;

    public enum Type {
        ID("SubscriptionId"),
        AUTHCODE("SubscriptionAuthCode"),
        TRANSACTIONID("MerchantTransactionId");

        private String value;

        private Type(String value){
            this.value = value;
        }

        public String getValue(){
            return this.value;
        }

        public String toString(){
            return this.getValue();
        }
    }

    public static class Builder extends Transaction.Builder {
        private String merchSubIdList;
        private boolean iponas;
        private int recurrences;
        private String subPeriod;
        private int subPeriodAmount;

        public Builder(double amount, AppCategory category, String desc,
                String transactionId, String productId, String redirectUrl,
                String merchSubIdList) {
            super(amount, category, desc, transactionId, productId, redirectUrl);

            this.merchSubIdList = merchSubIdList;
            this.recurrences = 99999;
            this.iponas = false;
            this.subPeriod = "MONTHLY";
            this.subPeriodAmount = 1;
        }

        public Subscription build() {
            return new Subscription(this);
        }
    }

    public Subscription(Subscription.Builder builder) {
        super(builder);
        this.merchSubIdList = builder.merchSubIdList;
        this.iponas = builder.iponas;
        this.recurrences = builder.recurrences;
        this.subPeriod = builder.subPeriod;
        this.subPeriodAmount = builder.subPeriodAmount;
    }

    public String getMerchantSubscriptionIdList() {
        return this.merchSubIdList;
    }

    public boolean isPurchaseOnNoActiveSubscription() {
        return this.iponas;
    }

    public int getSubscriptionRecurrences() {
        return this.recurrences;
    }

    public String getSubscriptionPeriod() {
        return this.subPeriod;
    }

    public int getSubscriptionPeriodAmount() {
        return this.subPeriodAmount;
    }
}
