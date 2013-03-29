package com.att.api.cms.model;

import java.io.IOException;
import java.io.Serializable;

import com.att.api.config.AppConfig;

public class ConfigBean implements Serializable {
    private static final long serialVersionUID = 138893983L;
    private String scriptContent;

    public ConfigBean() {
        this.scriptContent = null;
    }

    public void setScriptContent(final String content) {
        this.scriptContent = content;
    }

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

    public String getNumber() throws IOException {
        return getConfig().getProperty("number");
    }

    public String[] getScriptFunctions() throws IOException {
        final String scriptFuncs = getConfig().getProperty("scriptFunctions");
        return scriptFuncs.split(",");
    }

    public String getScriptContent() {
        return this.scriptContent;
    }
}
