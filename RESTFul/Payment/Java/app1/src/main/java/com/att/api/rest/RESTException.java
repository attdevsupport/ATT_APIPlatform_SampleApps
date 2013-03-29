package com.att.api.rest;

public class RESTException extends Exception {
    private static final long serialVersionUID = -6874042744590915838L;
    // http status code
    private final int statusCode;

    // error message
    private final String errorMessage;
    
    public RESTException(final String errorMessage) {
        this(-1, errorMessage);
    }

    public RESTException(final Throwable cause) {
        super(cause);
        this.statusCode = -1;
        this.errorMessage = cause.getMessage();
    }

    public RESTException(final int statusCode, final String errorMessage) {
        super(statusCode + ":" + errorMessage);
        this.statusCode = statusCode;
        this.errorMessage = errorMessage;
    }

    public int getStatusCode() {
        return this.statusCode;
    }

    public String getErrorMessage() {
        return this.errorMessage; 
    }
}
