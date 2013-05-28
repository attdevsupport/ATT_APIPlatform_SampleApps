package com.att.api.exception;

/**
 * Created with IntelliJ IDEA.
 * User: sendhilc
 * Date: 2/12/13
 * Time: 9:41 AM
 * To change this template use File | Settings | File Templates.
 */
public class ServiceException extends Exception {

    public ServiceException(Exception ex){
        super(ex);
    }
}
