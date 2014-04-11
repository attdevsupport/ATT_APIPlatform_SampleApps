package com.att.api.payment.model;

public class Transaction {
    private double amount;
    private AppCategory category;
    private String channel;
    private String description;
    private String transactionId;
    private String productId;
    private String applicationId;
    private String redirectUrl;

    public enum Type {
        ID("TransactionId"),
        AUTHCODE("TransactionAuthCode"),
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

    public static class Builder {
        private double amount;
        private AppCategory category;
        private String description;
        private String transactionId;
        private String productId;
        private String redirectUrl;
        private String channel; 
        private String applicationId;

        public Builder(double amount, AppCategory category, String desc,
                String transactionId, String productId, String redirectUrl) {
            this.amount = amount;
            this.category = category;
            this.description = desc;
            this.transactionId = transactionId;
            this.productId = productId;
            this.redirectUrl = redirectUrl;
            this.channel = "MOBILE_WEB";
            this.applicationId = "";
        }

        public Builder setChannel(String channel) { 
            this.channel = channel;
            return this;
        }

        public Builder setAppId(String applicationId){
            this.applicationId = applicationId;
            return this;
        }

        public Transaction build(){
            return new Transaction(this);
        }
    }

    protected Transaction(Transaction.Builder builder) {
        this.amount = builder.amount;
        this.category = builder.category;
        this.description = builder.description;
        this.transactionId = builder.transactionId;
        this.productId = builder.productId;
        this.redirectUrl = builder.redirectUrl;
        this.channel = builder.channel;
        this.applicationId = builder.applicationId;
    }


    public AppCategory getAppCategory() {
        return category;
    }

    public String getChannel() {
        return channel;
    }

    public double getAmount() {
        return amount;
    }

    public String getDescription() {
        return description;
    }

    public String getTransactionId() {
        return transactionId;
    }

    public String getProductId() {
        return productId;
    }

    public String getPaymentRedirectUrl() {
        return redirectUrl;
    }

    public String getApplicationId() {
        return this.applicationId;
    }

}
