package com.att.api.payment.model;

public enum AppCategory {
    INAPP_GAMES(1),
        INAPP_OTHER(3),
        APP_GAME(4),
        APP_OTHER(5);

    private int value;

    private AppCategory(int value){
        this.value = value;
    }

    public int getValue(){
        return this.value;
    }
}

