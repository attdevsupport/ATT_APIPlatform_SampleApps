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

public final class ContactWrapper {
    private final Contact contact;
    private final QuickContact quickContact;

    public ContactWrapper(Contact contact) {
        this.contact = contact;
        this.quickContact = null;
    }

    public ContactWrapper(QuickContact quickContact) {
        this.quickContact = quickContact;
        this.contact = null;
    }

    public Contact getContact() {
        return contact;
    }

    public QuickContact getQuickContact() {
        return quickContact;
    }

}
