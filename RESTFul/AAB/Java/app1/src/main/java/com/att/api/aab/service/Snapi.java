/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/*
 * ====================================================================
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools
 * Agreement.' 2014.
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * ====================================================================
 */

package com.att.api.aab.service;

public final class Snapi {
    private final String type;
    private final String uri;
    private final Boolean preferred;

    public Snapi(String type, String uri, Boolean preferred) {
        this.type = type;
        this.uri = uri;
        this.preferred = preferred;
    }

    public Boolean isPreferred() {
        return preferred;
    }

    public String getType() {
        return type;
    }

    public String getUri() {
        return uri;
    }

}
