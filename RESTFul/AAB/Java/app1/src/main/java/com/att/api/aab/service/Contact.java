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

import org.json.JSONArray;
import org.json.JSONObject;

public final class Contact {

    public static class Builder {
        private String contactId;
        private String creationDate;
        private String modificationDate;
        private String formattedName;
        private String firstName;
        private String middleName;
        private String lastName;
        private String prefix;
        private String suffix;
        private String nickname;
        private String organization;
        private String jobTitle;
        private String anniversary;
        private Gender gender;
        private String spouse;
        private String children;
        private String hobby;
        private String assistant;
        private Phone[] phones;
        private Address[] addresses;
        private Email[] emails;
        private Im[] ims;
        private WebURL[] weburls;
        private Photo photo;

        public Builder() {
            this.contactId = null;
            this.creationDate = null;
            this.modificationDate = null;
            this.formattedName = null;
            this.firstName = null;
            this.middleName = null;
            this.lastName = null;
            this.prefix = null;
            this.suffix = null;
            this.nickname = null;
            this.organization = null;
            this.jobTitle = null;
            this.anniversary = null;
            this.gender = null;
            this.spouse = null;
            this.children = null;
            this.hobby = null;
            this.assistant = null;
            this.phones = null;
            this.addresses = null;
            this.emails = null;
            this.ims = null;
            this.weburls = null;
            this.photo = null;
        }

        public Builder setContactId(String contactId) {
            this.contactId = contactId; return this;
        }

        public Builder setCreationDate(String creationDate) {
            this.creationDate = creationDate; return this;
        }

        public Builder setModificationDate(String modificationDate) {
            this.modificationDate = modificationDate; return this;
        }

        public Builder setFormattedName(String formattedName) {
            this.formattedName = formattedName; return this;
        }

        public Builder setFirstName(String firstName) {
            this.firstName = firstName; return this;
        }

        public Builder setMiddleName(String middleName) {
            this.middleName = middleName; return this;
        }

        public Builder setLastName(String lastName) {
            this.lastName = lastName; return this;
        }

        public Builder setPrefix(String prefix) {
            this.prefix = prefix; return this;
        }

        public Builder setSuffix(String suffix) {
            this.suffix = suffix; return this;
        }

        public Builder setNickname(String nickname) {
            this.nickname = nickname; return this;
        }

        public Builder setOrganization(String organization) {
            this.organization = organization; return this;
        }

        public Builder setJobTitle(String jobTitle) {
            this.jobTitle = jobTitle; return this;
        }

        public Builder setAnniversary(String anniversary) {
            this.anniversary = anniversary; return this;
        }

        public Builder setGender(Gender gender) {
            this.gender = gender; return this;
        }

        public Builder setSpouse(String spouse) {
            this.spouse = spouse; return this;
        }

        public Builder setChildren(String children) {
            this.children = children; return this;
        }

        public Builder setHobby(String hobby) {
            this.hobby = hobby; return this;
        }

        public Builder setAssistant(String assistant) {
            this.assistant = assistant; return this;
        }

        public Builder setPhones(Phone[] phones) {
            // TODO: create copy to maintain immutability
            this.phones = phones; return this;
        }

        public Builder setAddresses(Address[] addresses) {
            // TODO: create copy to maintain immutability
            this.addresses = addresses; return this;
        }

        public Builder setEmails(Email[] emails) {
            // TODO: create copy to maintain immutability
            this.emails = emails; return this;
        }

        public Builder setIms(Im[] ims) {
            // TODO: create copy to maintain immutability
            this.ims = ims; return this;
        }

        public Builder setWeburls(WebURL[] weburls) {
            // TODO: create copy to maintain immutability
            this.weburls = weburls; return this;
        }

        public Builder setPhoto(Photo photo) {
            this.photo = photo; return this;
        }

        public Contact build() {
            return new Contact(this);
        }
    }

    private final String contactId;
    private final String creationDate;
    private final String modificationDate;
    private final String formattedName;
    private final String firstName;
    private final String middleName;
    private final String lastName;
    private final String prefix;
    private final String suffix;
    private final String nickname;
    private final String organization;
    private final String jobTitle;
    private final String anniversary;
    private final Gender gender;
    private final String spouse;
    private final String children;
    private final String hobby;
    private final String assistant;
    private final Phone[] phones;
    private final Address[] addresses;
    private final Email[] emails;
    private final Im[] ims;
    private final WebURL[] weburls;
    private final Photo photo;

    private Contact(Contact.Builder builder) {
        this.contactId = builder.contactId;
        this.creationDate = builder.creationDate;
        this.modificationDate = builder.modificationDate;
        this.formattedName = builder.formattedName;
        this.firstName = builder.firstName;
        this.middleName = builder.middleName;
        this.lastName = builder.lastName;
        this.prefix = builder.prefix;
        this.suffix = builder.suffix;
        this.nickname = builder.nickname;
        this.organization = builder.organization;
        this.jobTitle = builder.jobTitle;
        this.anniversary = builder.anniversary;
        this.gender = builder.gender;
        this.spouse = builder.spouse;
        this.children = builder.children;
        this.hobby = builder.hobby;
        this.assistant = builder.assistant;
        this.phones = builder.phones;
        this.addresses = builder.addresses;
        this.emails = builder.emails;
        this.ims = builder.ims;
        this.weburls = builder.weburls;
        this.photo = builder.photo;
    }

    public String getContactId() {
        return contactId;
    }

    public String getCreationDate() {
        return creationDate;
    }

    public String getModificationDate() {
        return modificationDate;
    }

    public String getFormattedName() {
        return formattedName;
    }

    public String getFirstName() {
        return firstName;
    }

    public String getMiddleName() {
        return middleName;
    }

    public String getLastName() {
        return lastName;
    }

    public String getPrefix() {
        return prefix;
    }

    public String getSuffix() {
        return suffix;
    }

    public String getNickname() {
        return nickname;
    }

    public String getOrganization() {
        return organization;
    }

    public String getJobTitle() {
        return jobTitle;
    }

    public String getAnniversary() {
        return anniversary;
    }

    public Gender getGender() {
        return gender;
    }

    public String getSpouse() {
        return spouse;
    }

    public String getChildren() {
        return children;
    }

    public String getHobby() {
        return hobby;
    }

    public String getAssistant() {
        return assistant;
    }
    
    public Photo getPhoto() {
        return photo;
    }

    public Phone[] getPhones() {
        if (phones == null) return null;

        // return copy to maintain immutability
        final Phone[] phonesCopy = new Phone[phones.length];
        for (int i = 0; i < phones.length; ++i) {
            phonesCopy[i] = phones[i];
        }
        return phonesCopy;
    }

    public Address[] getAddresses() {
        if (addresses == null) return null;

        // return copy to maintain immutability
        final Address[] addressesCopy = new Address[addresses.length];
        for (int i = 0; i < addresses.length; ++i) {
            addressesCopy[i] = addresses[i];
        }
        return addressesCopy;
    }

    public Email[] getEmails() {
        if (emails == null) return null;

        // return copy to maintain immutability
        final Email[] emailsCopy = new Email[emails.length];
        for (int i = 0; i < emails.length; ++i) {
            emailsCopy[i] = emails[i];
        }
        return emailsCopy;
    }

    public Im[] getIms() {
        if (ims == null) return null;

        // return copy to maintain immutability
        final Im[] imsCopy = new Im[ims.length];
        for (int i = 0; i < ims.length; ++i) {
            imsCopy[i] = ims[i];
        }
        return imsCopy;
    }

    public WebURL[] getWeburls() {
        if (weburls == null) return null;

        // return copy to maintain immutability
        final WebURL[] weburlsCopy = new WebURL[weburls.length];
        for (int i = 0; i < weburls.length; ++i) {
            weburlsCopy[i] = weburls[i];
        }
        return weburlsCopy;
    }

    public JSONObject toJson() {
        JSONObject jobj = new JSONObject();

        final String[] keys = { 
            "creationDate", "modificationDate", "formattedName", "firstName",
            "lastName", "prefix", "suffix", "nickName", "organization",
            "jobTitle", "anniversary", "gender", "spouse", "hobby",
            "assistant"
        };
        
        String genderStr = getGender() == null ? null : getGender().toString();
        final String[] values = {
            getCreationDate(), getModificationDate(), getFormattedName(),
            getFirstName(), getLastName(), getPrefix(), getSuffix(),
            getNickname(), getOrganization(), getJobTitle(), getAnniversary(),
            genderStr, getSpouse(), getHobby(), getAssistant()
        };

        for (int i = 0; i < keys.length; ++i) {
            // do not add any null values to the json object
            if (values[i] == null) continue;
            jobj.put(keys[i], values[i]);
        }

        if (getPhones() != null) {
            JSONObject jPhones = new JSONObject();
            JSONArray jPhonesArr = new JSONArray();
            for (final Phone phone : getPhones()) {
                jPhonesArr.put(phone.toJson());
            }
            jPhones.put("phone", jPhonesArr);
            jobj.put("phones", jPhones);
        }

        if (getAddresses() != null) {
            JSONObject jAddrs = new JSONObject();
            JSONArray jAddrsArr = new JSONArray();
            for (final Address addr : getAddresses()) {
                jAddrsArr.put(addr.toJson());
            }
            jAddrs.put("address", jAddrsArr);
            jobj.put("addresses", jAddrs);
        }

        if (getEmails() != null) {
            JSONObject jEmails = new JSONObject();
            JSONArray jEmailsArr = new JSONArray();
            for (final Email email : getEmails()) {
                jEmailsArr.put(email.toJson());
            }
            jEmails.put("email", jEmailsArr);
            jobj.put("emails", jEmails);
        }

        if (getIms() != null) {
            JSONObject jIms = new JSONObject();
            JSONArray jImsArr = new JSONArray();
            for (final Im im : getIms()) {
                jImsArr.put(im.toJson());
            }
            jIms.put("im", jImsArr);
            jobj.put("ims", jIms);
        }

        if (getWeburls() != null) {
            JSONObject jWeburls = new JSONObject();
            JSONArray jWeburlsArr = new JSONArray();
            for (final WebURL weburl: getWeburls()) {
                jWeburlsArr.put(weburl.toJson());
            }
            jWeburls.put("webUrl", jWeburlsArr);
            jobj.put("weburls", jWeburls);
        }
        
        if (getPhoto() != null) jobj.put("photo", getPhoto().toJson());

        return jobj;
    }

    public static Contact valueOf(JSONObject jobj) {
        Builder builder = new Builder();
        if (jobj.has("contactId"))
            builder.setContactId(jobj.getString("contactId"));
        if (jobj.has("creationDate"))
            builder.setCreationDate(jobj.getString("creationDate"));
        if (jobj.has("formattedName"))
            builder.setFormattedName(jobj.getString("formattedName"));
        if (jobj.has("firstName"))
            builder.setFirstName(jobj.getString("firstName"));
        if (jobj.has("middleName"))
            builder.setMiddleName(jobj.getString("middleName"));
        if (jobj.has("lastName"))
            builder.setLastName(jobj.getString("lastName"));
        if (jobj.has("prefix"))
            builder.setPrefix(jobj.getString("prefix"));
        if (jobj.has("suffix"))
            builder.setSuffix(jobj.getString("suffix"));
        if (jobj.has("nickName"))
            builder.setNickname(jobj.getString("nickName"));
        if (jobj.has("jobTitle"))
            builder.setJobTitle(jobj.getString("jobTitle"));
        if (jobj.has("anniversary"))
            builder.setAnniversary(jobj.getString("anniversary"));
        if (jobj.has("gender")) {
            String genderStr = jobj.getString("gender");
            builder.setGender(genderStr.equals("MALE") ? Gender.MALE : Gender.FEMALE);
        }
        if (jobj.has("spouse"))
            builder.setSpouse(jobj.getString("spouse"));
        if (jobj.has("children"))
            builder.setChildren(jobj.getString("children"));
        if (jobj.has("hobby"))
            builder.setHobby(jobj.getString("hobby"));
        if (jobj.has("assitant"))
            builder.setAssistant(jobj.getString("assitant"));

        if (jobj.has("phones")) {
            JSONArray jarr = jobj.getJSONObject("phones").getJSONArray("phone");
            Phone[] phones = new Phone[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                phones[i] = Phone.valueOf(jarr.getJSONObject(i));
            }
            builder.setPhones(phones);
        }
        if (jobj.has("addresses")) {
            JSONArray jarr = jobj.getJSONObject("addresses").getJSONArray("address");
            Address[] addrs = new Address[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                addrs[i] = Address.valueOf(jarr.getJSONObject(i));
            }
            builder.setAddresses(addrs);
        }
        if (jobj.has("emails")) {
            JSONArray jarr = jobj.getJSONObject("emails").getJSONArray("email");
            Email[] emails = new Email[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                emails[i] = Email.valueOf(jarr.getJSONObject(i));
            }
            builder.setEmails(emails);
        }
        if (jobj.has("ims")) {
            JSONArray jarr = jobj.getJSONObject("ims").getJSONArray("im");
            Im[] ims = new Im[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                ims[i] = Im.valueOf(jarr.getJSONObject(i));
            }
            builder.setIms(ims);
        }
        if (jobj.has("weburls")) {
            JSONArray jarr = jobj.getJSONObject("weburls").getJSONArray("webUrl");
            WebURL[] weburls = new WebURL[jarr.length()];
            for (int i = 0; i < jarr.length(); ++i) {
                weburls[i] = WebURL.valueOf(jarr.getJSONObject(i));
            }
            builder.setWeburls(weburls);
        }

        if (jobj.has("photo")) {
            builder.setPhoto(Photo.valueOf(jobj.getJSONObject("photo")));
        }

        return builder.build();
    }

}
