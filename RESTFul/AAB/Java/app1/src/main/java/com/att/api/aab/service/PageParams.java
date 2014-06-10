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

public final class PageParams {
    private final String order;
    private final String orderBy;
    private final String limit;
    private final String offset;

    public PageParams(String order, String orderBy, String limit, 
            String offset) {
        this.order = order;
        this.orderBy = orderBy;
        this.limit = limit;
        this.offset = offset;
    }

    public String getOrder() {
        return order;
    }

    public String getOrderBy() {
        return orderBy;
    }

    public String getLimit() {
        return limit;
    }

    public String getOffset() {
        return offset;
    }

}
