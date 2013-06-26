package com.att.api.immn.model;

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
}
