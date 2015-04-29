package com.att.api.aab.controller;

import org.json.JSONArray;
import org.json.JSONObject;

import com.att.api.aab.service.Address;
import com.att.api.aab.service.Contact;
import com.att.api.aab.service.Email;
import com.att.api.aab.service.Im;
import com.att.api.aab.service.Phone;
import com.att.api.aab.service.QuickContact;
import com.att.api.aab.service.WebURL;

public final class AabUtil {
    private AabUtil() {
    }

    public static JSONObject generatePhonesTable(
        final String contactId, final Phone[] phones
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact (" + contactId + ") Phones:");
        jtable.put("headers", new JSONArray().put("type").put("number").put("preferred"));
        JSONArray jphones = new JSONArray();
        for (final Phone phone : phones) {
            final JSONArray jphone = new JSONArray();
            jphone
                .put(phone.getType())
                .put(phone.getNumber())
                .put(phone.isPreferred());
            jphones.put(jphone);
        }
        jtable.put("values", jphones);
        return jtable;
    }

    public static JSONObject generateEmailsTable(
        final String contactId, final Email[] emails
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact (" + contactId + ") Emails:");
        jtable.put("headers", new JSONArray().put("type").put("address").put("preferred"));
        JSONArray jemails = new JSONArray();
        for (final Email email : emails) {
            final JSONArray jemail = new JSONArray();
            jemail
                .put(email.getType())
                .put(email.getEmailAddress())
                .put(email.isPreferred());
            jemails.put(jemail);
        }
        jtable.put("values", jemails);
        return jtable;
    }

    public static JSONObject generateImsTable(
        final String contactId, final Im[] ims
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact (" + contactId + ") Ims:");
        jtable.put("headers", new JSONArray().put("type").put("uri").put("preferred"));
        JSONArray jims = new JSONArray();
        for (final Im im : ims) {
            final JSONArray jim = new JSONArray();
            jim
                .put(im.getType())
                .put(im.getImUri())
                .put(im.isPreferred());
            jims.put(jim);
        }
        jtable.put("values", jims);
        return jtable;
    }

    public static JSONObject generateAddressesTable(
        final String contactId, final Address[] addresses
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact (" + contactId + ") Addresses:");
        jtable.put("headers", 
            new JSONArray()
                .put("type")
                .put("preferred")
                .put("poBox")
                .put("addressLine1")
                .put("addressLine2")
                .put("city")
                .put("state")
                .put("zipcode")
                .put("country")
        );

        JSONArray jaddresses = new JSONArray();
        for (final Address address : addresses) {
            final JSONArray jaddress = new JSONArray();
            jaddress
                .put(address.getType())
                .put(address.isPreferred())
                .put(address.getPoBox())
                .put(address.getAddressLineOne())
                .put(address.getAddressLineTwo())
                .put(address.getCity())
                .put(address.getState())
                .put(address.getZipcode())
                .put(address.getCountry());

            jaddresses.put(jaddress);
        }
        jtable.put("values", jaddresses);
        return jtable;
    }

    public static JSONObject generateWeburlsTable(
        final String contactId, final WebURL[] weburls
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact (" + contactId + ") Weburls:");
        jtable.put("headers", new JSONArray().put("type").put("address").put("preferred"));
        JSONArray jweburls = new JSONArray();
        for (final WebURL weburl : weburls) {
            final JSONArray jweburl = new JSONArray();
            jweburl
                .put(weburl.getType())
                .put(weburl.getUrl())
                .put(weburl.isPreferred());
            jweburls.put(jweburl);
        }
        jtable.put("values", jweburls);
        return jtable;
    }

    public static JSONObject generateQuickContactsTable(
        final QuickContact qcontact
    ) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Quick Contact:");
        jtable.put("headers", 
            new JSONArray()
                .put("contactId")
                .put("formattedName")
                .put("firstName")
                .put("middleName")
                .put("lastName")
                .put("prefix")
                .put("suffix")
                .put("nickName")
                .put("organization")
        );
        JSONArray jValuesEntry = new JSONArray()
            .put(qcontact.getContactId())
            .put(qcontact.getFormattedName())
            .put(qcontact.getFirstName())
            .put(qcontact.getMiddleName())
            .put(qcontact.getLastName())
            .put(qcontact.getPrefix())
            .put(qcontact.getSuffix())
            .put(qcontact.getNickname())
            .put(qcontact.getOrganization());

        jtable.put("values", new JSONArray().put(jValuesEntry));
        return jtable;
    }

    public static JSONObject generateContactsTable(final Contact contact) {
        JSONObject jtable = new JSONObject();
        jtable.put("caption", "Contact:");
        jtable.put("headers", 
            new JSONArray()
                .put("contactId")
                .put("creationDate")
                .put("modificationDate")
                .put("formattedName")
                .put("firstName")
                .put("lastName")
                .put("prefix")
                .put("suffix")
                .put("nickName")
                .put("organization")
                .put("jobTitle")
                .put("anniversary")
                .put("gender")
                .put("spouse")
                .put("hobby")
                .put("gender")
        );
        JSONArray jValuesEntry = new JSONArray()
            .put(contact.getContactId())
            .put(contact.getCreationDate())
            .put(contact.getModificationDate())
            .put(contact.getFormattedName())
            .put(contact.getFirstName())
            .put(contact.getLastName())
            .put(contact.getPrefix())
            .put(contact.getSuffix())
            .put(contact.getNickname())
            .put(contact.getOrganization())
            .put(contact.getJobTitle())
            .put(contact.getAnniversary())
            .put(contact.getGender())
            .put(contact.getSpouse())
            .put(contact.getHobby())
            .put(contact.getGender() == null ? null : contact.getGender().getString());

        jtable.put("values", new JSONArray().put(jValuesEntry));
        return jtable;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
