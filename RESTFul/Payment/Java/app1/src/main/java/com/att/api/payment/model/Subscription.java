package com.att.api.payment.model;

public class Subscription extends Transaction {
    private String merchSubIdList;
    private boolean iponas;
    private int recurrences;
    private String subPeriod;
    private int subPeriodAmount;

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

        /**
         * @param merchSubIdList the merchSubIdList to set
         */
        public void setMerchSubIdList(String merchSubIdList) {
            this.merchSubIdList = merchSubIdList;
        }

        /**
         * @param iponas the iponas to set
         */
        public void setIponas(boolean iponas) {
            this.iponas = iponas;
        }

        /**
         * @param recurrences the recurrences to set
         */
        public void setRecurrences(int recurrences) {
            this.recurrences = recurrences;
        }

        /**
         * @param subPeriod the subPeriod to set
         */
        public void setSubPeriod(String subPeriod) {
            this.subPeriod = subPeriod;
        }

        /**
         * @param subPeriodAmount the subPeriodAmount to set
         */
        public void setSubPeriodAmount(int subPeriodAmount) {
            this.subPeriodAmount = subPeriodAmount;
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
