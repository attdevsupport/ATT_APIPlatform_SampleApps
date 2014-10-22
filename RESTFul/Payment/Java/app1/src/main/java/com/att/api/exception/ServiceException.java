package com.att.api.exception;

public class ServiceException extends Exception {

    public ServiceException(Exception ex){
        super(ex);
    }
}
