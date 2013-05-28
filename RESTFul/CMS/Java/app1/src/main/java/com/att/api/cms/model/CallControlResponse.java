package com.att.api.cms.model;

public class CallControlResponse {
    private final String sessionId;
    private final String signalStatus;

    private final String sessionError;
    private final String signalError;

    private final boolean success;

    private CallControlResponse(final String sessionId, final String sigStatus, 
            final String sessionError, final String signalError) {

        this(sessionId, false, sigStatus, sessionError, signalError);
    } 

    private CallControlResponse(String sessionId, boolean success, 
            String sigStatus, String sessionError, String signalError) {
        this.sessionId = sessionId;
        this.signalStatus = sigStatus;
        this.sessionError = sessionError;
        this.signalError = signalError;
        this.success = success;
    } 

    public String getSignalStatus() {
        return this.signalStatus;
    }

    public String getSessionId() {
        return this.sessionId;
    }

    public boolean getSuccess() {
        return this.success;
    }

    public String getSessionError() {
        return this.sessionError;
    }

    public String getSignalError() {
        return this.signalError;
    }

    public static CallControlResponse sessionErrorResponse(final String err) {
        return new CallControlResponse(null, null, err, null);
    }

    public static CallControlResponse signalErrorResponse(final String err) {
        return new CallControlResponse(null, null, null, err);
    }

    public static CallControlResponse sessionIdResponse(final String sid, 
            boolean success) {
        return new CallControlResponse(sid, success, null, null, null);
    }

    public static CallControlResponse signalStatusResponse(final String status) {
        return new CallControlResponse(null, status, null, null);
    } 
    
    public static CallControlResponse noResponse() {
        return new CallControlResponse(null, null, null, null);
    }
}
