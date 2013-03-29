/*                                                                             
 * ==================================================================== 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools          
 * Agreement.' 2013.                                                            
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:               
 * http://developer.att.com/sdk_agreement/                                      
 *                                                                              
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.              
 * For more information contact developer.support@att.com                       
 * ==================================================================== 
 */  

package com.att.api.rest;

/**
 * A custom exception class. 
 *
 */
public class RESTException extends Exception {
    private static final long serialVersionUID = -6874042744590915838L;
    // http status code
    private final int statusCode;

    // error message
    private final String errorMessage;
    
    /**
     * {@inheritDoc}
     * @see Exception#RESTException(String)
     */
    public RESTException(final String errorMessage) {
        this(-1, errorMessage);
    }

    /**
     * {@inheritDoc}
     * @see Exception#RESTException(Throwable)
     */
    public RESTException(final Throwable cause) {
        super(cause);
        this.statusCode = -1;
        this.errorMessage = cause.getMessage();
    }

    /**
     * Creates a RESTException with the specified status code and error 
     * message.
     *
     * @param statusCode http status code
     * @param errorMessage http error message
     */
    public RESTException(final int statusCode, final String errorMessage) {
        super(statusCode + ":" + errorMessage);
        this.statusCode = statusCode;
        this.errorMessage = errorMessage;
    }

    /**
     * Gets the status code or -1 if none has been set.
     *
     * @return status code
     */
    public int getStatusCode() {
        return this.statusCode;
    }

    /**
     * Gets the error message.
     *
     * @return error message
     */
    public String getErrorMessage() {
        return this.errorMessage; 
    }
}
