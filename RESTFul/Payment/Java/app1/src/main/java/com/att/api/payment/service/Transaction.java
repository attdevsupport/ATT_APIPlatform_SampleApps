package com.att.api.payment.service;

public class Transaction {
    private String amount;
    private String category;
    private String channel;
    private String description;
    private String transactionId;
    private String productId;
    private String applicationId;
    private String paymentRedirectUrl;

    public Transaction() {
        this.amount = "0.00";
        this.category = "1";
        this.channel = "MOBILE_WEB";
        this.description = "DESC";
        this.transactionId = "T" + System.currentTimeMillis();
        this.productId = "0";
        this.paymentRedirectUrl = "localhost";
    }

    public String getCategory() {
        return category;
    }

    public void setCategory(String category) {
        this.category = category;
    }

    public String getChannel() {
        return channel;
    }

    public void setChannel(String channel) {
        this.channel = channel;
    }

    public String getAmount() {
        return amount;
    }

    public void setAmount(String amount) {
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
        return paymentRedirectUrl;
    }

    public void setPaymentRedirectUrl(String paymentRedirectUrl) {
        this.paymentRedirectUrl = paymentRedirectUrl;
    }

    public void setApplicationId(String applicationId) {
        this.applicationId = applicationId;
    }

    public String getApplicationId() {
        return this.applicationId;
    }

}
