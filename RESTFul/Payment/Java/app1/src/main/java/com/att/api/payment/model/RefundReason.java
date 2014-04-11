package com.att.api.payment.model;

public enum RefundReason {
    CP_NONE(1),
        CP_LOSS_OF_ELIGIBILITY(2),
        CP_PRODUCT_TERMINATION(3),
        CP_ABUSE_OF_PRIVILEGES(4),
        CP_CONVERSION_TO_NEW_PRODUCT(5),
        CP_NONRENEWABLE_PRODUCT(6),
        CP_DUPLICATE_SUBSCRIPTION(7),
        CP_OTHER(8),
        SUBSCRIBER_nONE(9),
        SUBSCRIBER_DID_NOT_USE(10),
        SUBSCRIBER_TOO_EXPENSIVE(11),
        SUBSCRIBER_DID_NOT_LIKE(12),
        SUBSCRIBER_REPLACED_BY_SAME_COMPANY(13),
        SUBSCRIBER_REPLACED_BY_DIFFERENT_COMPANY(14),
        SUBSCRIBER_DUPLICATE_SUBSCRIPTION(15),
        SUBSCRIBER_OTHER(16);

    private int value;

    private RefundReason(int value){
        this.value = value;
    }

    public int getValue(){
        return this.value;
    }
}

