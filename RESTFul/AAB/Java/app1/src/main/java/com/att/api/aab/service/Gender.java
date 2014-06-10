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

public enum Gender {
    MALE("Male"), FEMALE("Female");

    private final String val;

    private Gender(String val) {
        this.val = val;
    }

    public static Gender fromString(String val) {
        if (val.equalsIgnoreCase("male"))
            return Gender.MALE;
        else if (val.equalsIgnoreCase("female"))
            return Gender.FEMALE;
        return null;
    }

    public String getString() {
        return this.val;
    }
}
