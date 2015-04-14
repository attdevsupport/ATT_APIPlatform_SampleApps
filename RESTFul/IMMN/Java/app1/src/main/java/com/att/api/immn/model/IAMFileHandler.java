package com.att.api.immn.model;

import java.io.IOException;

public class IAMFileHandler {
    private static FileHandler instance = null;

    public static FileHandler getInstance() throws IOException {
        if (instance == null)
            instance = new FileHandler("iamNotifications_", "db");
        return instance;
    }
}
