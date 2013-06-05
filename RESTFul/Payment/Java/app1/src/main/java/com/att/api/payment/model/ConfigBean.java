package com.att.api.payment.model;

import java.io.IOException;
import java.io.Serializable;

import com.att.api.config.AppConfig;

public class ConfigBean implements Serializable {
    private static final long serialVersionUID = 138893983L;

    private AppConfig getConfig() throws IOException {
        return AppConfig.getInstance();
    }

    public String getLinkSource() throws IOException {
        return getConfig().getProperty("linkSource");
    }

    public String getLinkDownload() throws IOException {
        return getConfig().getProperty("linkDownload");
    }

    public String getLinkHelp() throws IOException {
        return getConfig().getProperty("linkHelp");
    }

    public String getNotaryEndpoint() throws  IOException {
        return new StringBuilder(getConfig().getFQDN()).append(getConfig().getProperty("notaryEndpoint")).toString();
    }

    public String getMerchantTransIdStatusEndpoint(String merchantTransId) throws IOException {
        return new StringBuilder(getConfig().getFQDN()).append(getConfig().getProperty("transtatusEndpoint.merchantTransId")).append(merchantTransId).toString();
    }

    public String getAuthCodeStatusEndpoint(String authCode) throws IOException {
        return new StringBuilder(getConfig().getFQDN()).append(getConfig().getProperty("transtatusEndpoint.authCode")).append(authCode).toString();
    }

    public String getTransIdStatusEndpoint(String transId) throws IOException {
        return new StringBuilder(getConfig().getFQDN()).append(getConfig().getProperty("transtatusEndpoint.transId")).append(transId).toString();
    }

    public String getPaymentRedirectURL() throws IOException {
        return new StringBuilder(getConfig().getProperty("paymentRedirectURL")).toString();
    }

    public String getTransactionsEndpoint() throws IOException {
        return new StringBuilder(getConfig().getFQDN()).append(getConfig().getProperty("transactionsEndpoint")).toString();
    }

    public int getTransactionPersistCount() throws IOException {
        return Integer.parseInt(getConfig().getProperty("transPersistCount"));
    }

    public String getMinTransactionAmount() throws IOException {
        return getConfig().getProperty("minTransactionAmount");
    }

    public String getMaxTransactionAmount() throws IOException {
        return getConfig().getProperty("maxTransactionAmount");
    }
    
    public String getMinSubscriptionAmount() throws IOException {
        return getConfig().getProperty("minSubscriptionAmount");
    }
    
    public String getMaxSubscriptionAmount() throws IOException {
        return getConfig().getProperty("maxSubscriptionAmount");
    }

    public String getMaxNotifications() throws IOException {
        return getConfig().getProperty("maxNotifications");
    }
}
