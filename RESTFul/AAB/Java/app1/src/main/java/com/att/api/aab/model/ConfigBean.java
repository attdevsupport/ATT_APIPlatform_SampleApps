package com.att.api.aab.model;

import java.io.IOException;
import java.io.Serializable;

import com.att.api.config.AppConfig;

public class ConfigBean implements Serializable {
    private static final long serialVersionUID = 138893983L;

    public AppConfig getConfig() throws IOException {
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

    public String getPhoneClass() throws IOException {
        return getConfig().getProperty("phoneClass");
    }
    public String getEmailClass() throws IOException {
        return getConfig().getProperty("emailClass");
    }
    public String getImClass() throws IOException {
        return getConfig().getProperty("imClass");
    }
    public String getAddressClass() throws IOException {
        return getConfig().getProperty("addressClass");
    }
    public String getWeburlClass() throws IOException {
        return getConfig().getProperty("weburlClass");
    }
}
