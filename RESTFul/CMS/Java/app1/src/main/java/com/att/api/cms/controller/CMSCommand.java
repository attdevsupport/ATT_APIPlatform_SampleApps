package com.att.api.cms.controller;

import org.apache.commons.lang3.StringUtils;

import javax.servlet.http.HttpServletRequest;

public class CMSCommand {

    private HttpServletRequest request;
    private String featuredNumber;
    private String messageToPlay;
    private String numberToDial;
    private String signalType;
    private String selectedScriptName;
    private boolean createSession;
    private boolean sendSignal;

    public CMSCommand(HttpServletRequest request) {
        this.request = request;
        parseInputParams();
    }

    /**
     * Parse the input params
     */
    private void parseInputParams() {
        setCreateSession(getParam(request, "btnCreateSession") != null);
        setSendSignal(getParam(request, "btnSendSignal") != null);
        setSelectedScriptName(getParam(request, "scriptType"));
        setSignalType(getParam(request, "signal"));
        setNumberToDial(getParam(request, "txtNumberToDial"));
        setMessageToPlay(getParam(request, "txtMessageToPlay"));
        setFeaturedNumber(getParam(request, "txtNumber"));
    }

    /**
     * Get key specific request parameter.
     * @param request
     * 			HttpServletRequest.
     * @param key
     * 			request parameter name.
     * @return
     * 			request parameter value.
     */
    private String getParam(HttpServletRequest request,String key){
        return (String) request.getParameter(key);
    }


    public void setFeaturedNumber(String featuredNumber) {
        this.featuredNumber = featuredNumber;
    }

    public String getFeaturedNumber() {
        return featuredNumber;
    }

    public void setMessageToPlay(String messageToPlay) {
        this.messageToPlay = messageToPlay;
    }

    public String getMessageToPlay() {
        return messageToPlay;
    }

    public void setNumberToDial(String numberToDial) {
        this.numberToDial = numberToDial;
    }

    public String getNumberToDial() {
        return numberToDial;
    }

    public void setSignalType(String signalType) {
        this.signalType = signalType;
    }

    public String getSignalType() {
        return signalType;
    }

    public void setSelectedScriptName(String selectedScriptName) {
        this.selectedScriptName = selectedScriptName;
    }

    public String getSelectedScriptName() {
        return selectedScriptName;
    }

    public void setCreateSession(boolean createSession) {
        this.createSession = createSession;
    }

    public boolean isCreateSession() {
        return createSession;
    }

    public void setSendSignal(boolean sendSignal) {
        this.sendSignal = sendSignal;
    }

    public boolean isSendSignal() {
        return sendSignal;
    }
}
