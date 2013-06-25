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


/* TODO: update this into where it's called
        this.amount = "0.00";
        this.category = "1";
        this.description = "DESC";
        this.transactionId = "T" + System.currentTimeMillis();
        this.productId = "0";
        this.paymentRedirectUrl = "localhost";
*/

    public Transaction(Transaction.Builder builder) {
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

    public void setAppCategory(AppCategory category) {
        this.category = category;
    }

    public String getChannel() {
        return channel;
    }

    public void setChannel(String channel) {
        this.channel = channel;
    }

    public double getAmount() {
        return amount;
    }

    public void setAmount(double amount) {
        this.amount = amount;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getTransactionId() {
        return transactionId;
    }

    public void setTransactionId(String transactionId) {
        this.transactionId = transactionId;
    }

    public String getProductId() {
        return productId;
    }

    public void setProductId(String productId) {
        this.productId = productId;
    }

    public String getPaymentRedirectUrl() {
        return redirectUrl;
    }

    public void setPaymentRedirectUrl(String paymentRedirectUrl) {
        this.redirectUrl = paymentRedirectUrl;
    }

    public void setApplicationId(String applicationId) {
        this.applicationId = applicationId;
    }

    public String getApplicationId() {
        return this.applicationId;
    }

}
