/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

/**
 * Provides classes for sending RESTFul requests and handling responses.
 *
 * <p>
 * This class follows the dependency inversion principle by applying a varation
 * of the adapter pattern. That is, this class is essentially a wrapper with a
 * simplified interface to a full http client.
 * </p>
 *
 * @author pk9069
 * @since 1.0
 * @see com.att.api.rest.RESTClient
 */

package com.att.api.rest;
