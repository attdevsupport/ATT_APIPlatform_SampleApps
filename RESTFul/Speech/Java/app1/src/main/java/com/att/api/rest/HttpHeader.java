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
 * An immutable class used to wrap an http header.
 *
 * @version 2.2
 * @since 2.2
 */
public class HttpHeader {
    private final String name;
    private final String value;

    /**
     * Create an http header using the specified name and value
     *
     * @param name name of http header
     * @param value value of http header
     */
    public HttpHeader(final String name, final String value) {
        if (name == null) {
            throw new IllegalArgumentException("Name may not be null.");
        }

        this.name = name;
        this.value = value;
    }

    /**
     * Gets the header name.
     * 
     * @return header name
     */
    public String getName() {
        return this.name;
    }

    /**
     * Gets the header value.
     * 
     * @return header value 
     */
    public String getValue() {
        return this.value;
    }
}
