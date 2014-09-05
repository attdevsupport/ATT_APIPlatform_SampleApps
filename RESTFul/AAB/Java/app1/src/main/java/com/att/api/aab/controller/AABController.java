package com.att.api.aab.controller;

import java.io.IOException;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.aab.model.ConfigBean;
import com.att.api.aab.service.AABService;
import com.att.api.aab.service.Address;
import com.att.api.aab.service.Contact;
import com.att.api.aab.service.ContactResultSet;
import com.att.api.aab.service.Email;
import com.att.api.aab.service.Gender;
import com.att.api.aab.service.Group;
import com.att.api.aab.service.GroupResultSet;
import com.att.api.aab.service.Im;
import com.att.api.aab.service.PageParams;
import com.att.api.aab.service.Phone;
import com.att.api.aab.service.WebURL;
import com.att.api.controller.APIController;
import com.att.api.oauth.OAuthToken;

public class AABController extends APIController {

    /**
     *
     */
    private static final long serialVersionUID = 1L;

    private String[] params = { "contactId",  "firstName", "lastName",
        "middleName", "prefix", "suffix", "nickname", "organization",
        "jobTitle", "anniversary", "gender", "spouse", "children", "hobby",
        "assistant", "attachPhoto", "phonePref", "phone[][number]",
        "phone[][type]", "imPref", "im[][uri]", "im[][type]", "addressPref",
        "address[][pobox]", "address[][addressLine1]",
        "address[][addressLine2]", "address[][city]", "address[][state]",
        "address[][zip]", "address[][country]", "address[][type]", "emailPref",
        "email[][email_address]", "email[][type]", "weburlPref",
        "weburl[][url]", "weburl[][type]", "searchVal", "groupType",
        "groupName", "groupId", "getGroupName", "order", "deleteContact",
        "updateContact", "createContact", "GetContacts", "getMyInfo",
        "updateMyInfo", "createGrp", "getGroups", "updateGrp", "deleteGrp",
        "groupIdContactsBtn", "groupIdContactsAddBtn", "groupIdContactsRemBtn", 
        "contactIdGroupsBtn",
    };


    public boolean handleCreateContact(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("createContact") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            Contact contact = buildContact(request); 
            String r = srvc.createContact(contact);

            request.setAttribute("createContact", r);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            e.printStackTrace();
            request.setAttribute("contactError", e.getMessage());
        }
        return false;
    }

    public boolean handleUpdateContact(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("updateContact") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            Contact contact = buildContact(request);
            String contactId = (String) session.getAttribute("contactId");

            srvc.updateContact(contact, contactId);

            request.setAttribute("successContact", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("contactError", e.getMessage());
        }
        return false;
    }

    public boolean handleDeleteContact(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("deleteContact") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String contactId = (String) session.getAttribute("contactId");

            srvc.deleteContact(contactId);

            request.setAttribute("successContact", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("contactError", e.getMessage());
        }
        return false;
    }

    public boolean handleGetContacts(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("GetContacts") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String search = (String) session.getAttribute("searchVal");

            ContactResultSet contacts = srvc.getContacts(search);

            request.setAttribute("contactResultSet", contacts);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("contactError", e.getMessage());
        }
        return false;
    }

    public boolean handleGetMyInfo(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("getMyInfo") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            Contact myinfo = srvc.getMyInfo();

            request.setAttribute("myInfo", myinfo);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("myInfoError", e.getMessage());
        }
        return false;
    }

    public boolean handleUpdateMyInfo(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("updateMyInfo") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            Contact myinfo = buildContact(request);

            srvc.updateMyInfo(myinfo);

            request.setAttribute("updateMyInfo", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("myInfoError", e.getMessage());
        }
        return false;
    }

    public boolean handleCreateGroup(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("createGrp") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String name = (String) session.getAttribute("groupName");
            String type = (String) session.getAttribute("groupType");
            Group group = new Group(name, type);

            String location = srvc.createGroup(group);

            request.setAttribute("createGroup", location);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    public boolean handleUpdateGroup(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("updateGrp") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("groupId");
            String name = (String) session.getAttribute("groupName");
            String type = (String) session.getAttribute("groupType");
            Group group = new Group(gid, name, type);

            srvc.createGroup(group);

            request.setAttribute("successGroup", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    public boolean handleDeleteGroup(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("deleteGrp") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("groupId");

            srvc.deleteGroup(gid);

            request.setAttribute("successGroup", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    public boolean handleGetGroups(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("getGroups") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("getGroupName");
            String order = (String) session.getAttribute("order");

            PageParams p = new PageParams(order, null, null, null);

            GroupResultSet grs = srvc.getGroups(p, gid);

            request.setAttribute("groups", grs);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }
    public boolean handleGetGroupContacts(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("groupIdContactsBtn") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("groupId");

            String[] ids = srvc.getGroupContacts(gid);

            request.setAttribute("groupContactIds", ids);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    private boolean handleAddContactsToGroup(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("groupIdContactsAddBtn") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("groupId");
            String cid = (String) session.getAttribute("contactId");

            srvc.addContactsToGroup(gid, cid);

            request.setAttribute("manageGroups", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    private boolean handleRemoveContactsFromGroupForm(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("groupIdContactsRemBtn") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String gid = (String) session.getAttribute("groupId");
            String cid = (String) session.getAttribute("contactId");

            srvc.removeContactsFromGroup(gid, cid);

            request.setAttribute("manageGroups", true);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    private boolean handleGetContactGroups(HttpServletRequest request,
            HttpServletResponse response) {
        final HttpSession session = request.getSession();

        if (session.getAttribute("contactIdGroupsBtn") == null) {
            return false;
        }
        try{
            OAuthToken token = getSessionToken(request, response);
            if (token == null)
                return true;
            AABService srvc = new AABService(appConfig.getApiFQDN(), token);

            String cid = (String) session.getAttribute("contactId");

            GroupResultSet groups = srvc.getContactGroups(cid, null);

            request.setAttribute("contactGroups", groups);
            clearSession(request, params);
        } catch (Exception e) {
            clearSession(request, params);
            request.setAttribute("manageGroupsError", e.getMessage());
        }
        return false;
    }

    public void doPost(HttpServletRequest request, 
            HttpServletResponse response) throws ServletException, IOException {

        copyToSession(request, params);

        // Check our requests to see what function to perform, returns true if
        // we need to authenticate
        if ( handleCreateContact(request,response)
                || handleUpdateContact(request,response)
                || handleDeleteContact(request,response)
                || handleGetContacts(request,response)
                || handleGetMyInfo(request, response)
                || handleUpdateMyInfo(request, response)
                || handleCreateGroup(request, response)
                || handleDeleteGroup(request, response)
                || handleUpdateGroup(request, response)
                || handleGetGroups(request, response)
                || handleGetGroupContacts(request, response)
                || handleAddContactsToGroup(request, response)
                || handleRemoveContactsFromGroupForm(request, response)
                || handleGetContactGroups(request, response)
           ) return;

        final String forward = "WEB-INF/AAB.jsp";
        request.setAttribute("cfg", new ConfigBean());
        RequestDispatcher dispatcher = request.getRequestDispatcher(forward);
        dispatcher.forward(request, response);
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        doPost(request, response);
    }

    private Phone[] buildPhones(final HttpSession session) {
        String[] phoneTypes = (String[]) session.getAttribute("phone[][type]");
        String[] phoneNumber = (String[]) session.getAttribute("phone[][number]");

        if (phoneTypes == null || phoneNumber == null) 
            return null;

        String p = (String) session.getAttribute("phonePref");
        int phonePref = -1;
        if (p != null)
            phonePref = Integer.valueOf(p);

        int size = phoneTypes.length;

        Phone[] phones = new Phone[size];
        for (int i = 0; i < size; ++i) {
            boolean pref = (phonePref == i);
            phones[i] = new Phone(phoneTypes[i], phoneNumber[i], pref);
        }

        return phones;
    }

    private Im[] buildIms(final HttpSession session) {
        String[] imTypes = (String[]) session.getAttribute("im[][type]");
        String[] imUri = (String[]) session.getAttribute("im[][uri]");

        if (imTypes == null || imUri == null) 
            return null;

        String p = (String) session.getAttribute("imPref");
        int imPref = -1;
        if (p != null) 
            imPref = Integer.valueOf(p);

        int size = imTypes.length;

        Im[] ims = new Im[size];
        for (int i = 0; i < size; ++i) {
            boolean pref = (imPref == i);
            ims[i] = new Im(imTypes[i], imUri[i], pref);
        }

        return ims;
    }

    private Email[] buildEmails(final HttpSession session) {
        String[] emailTypes = (String[]) session.getAttribute("email[][type]");
        String[] emailAddress = (String[]) session.getAttribute("im[][email_address]");

        if (emailTypes == null || emailAddress == null)
            return null;

        String p = (String) session.getAttribute("emailPref");
        int emailPref = -1;
        if (p != null)
            emailPref = Integer.valueOf(p);

        int size = emailTypes.length;

        Email[] emails = new Email[size];
        for (int i = 0; i < size; ++i) {
            boolean pref = (emailPref == i);
            emails[i] = new Email(emailTypes[i], emailAddress[i], pref);
        }

        return emails;
    }

    private WebURL[] buildWebUrls(final HttpSession session) {
        String[] weburlType = (String[]) session.getAttribute("weburl[][type]");
        String[] weburl = (String[]) session.getAttribute("weburl[][url]");

        if (weburlType == null || weburl == null)
            return null;

        String p = (String) session.getAttribute("weburlPref");
        int weburlPref = -1;
        if (p != null)
            weburlPref = Integer.valueOf(p);

        int size = weburlType.length;

        WebURL[] weburls = new WebURL[size];
        for (int i = 0; i < size; ++i) {
            boolean pref = (weburlPref == i);
            weburls[i] = new WebURL(weburlType[i], weburl[i], pref);
        }

        return weburls;
    }

    private Address[] buildAddresses(final HttpSession session) {
        String[] pobox = (String[]) session.getAttribute("address[][pobox]");
        String[] addressLine1 = (String[]) session.getAttribute("address[][addressLine1]");
        String[] addressLine2 = (String[]) session.getAttribute("address[][addressLine2]");
        String[] city = (String[]) session.getAttribute("address[][city]");
        String[] state = (String[]) session.getAttribute("address[][state]");
        String[] zip = (String[]) session.getAttribute("address[][zip]");
        String[] country = (String[]) session.getAttribute("address[][country]");
        String[] type = (String[]) session.getAttribute("address[][type]");

        if (addressLine1 == null)
            return null;

        String p = (String) session.getAttribute("addressPref");
        int addressPref = -1;
        if (p != null)
            addressPref = Integer.valueOf(p);

        int size = pobox.length;

        Address[] addresses = new Address[size];
        for (int i = 0; i < size; ++i) {
            boolean pref = (addressPref == i);
            addresses[i] = new Address.Builder()
                .setPoBox(pobox[i])
                .setAddrLineOne(addressLine1[i])
                .setAddrLineTwo(addressLine2[i])
                .setCity(city[i])
                .setState(state[i])
                .setZipcode(zip[i])
                .setCountry(country[i])
                .setType(type[i])
                .setPreferred(pref)
                .build();
        }

        return addresses;
    }

    private Contact buildContact(HttpServletRequest request) {
        final HttpSession session = request.getSession();
        
        Contact.Builder cbuilder = new Contact.Builder();

        String attr = (String) session.getAttribute("firstName");
        if (attr != null && !attr.isEmpty()) cbuilder.setFirstName(attr);
        attr = (String) session.getAttribute("middleName");
        if (attr != null && !attr.isEmpty()) cbuilder.setMiddleName(attr);
        attr = (String) session.getAttribute("lastName");
        if (attr != null && !attr.isEmpty()) cbuilder.setLastName(attr);
        attr = (String) session.getAttribute("prefix");
        if (attr != null && !attr.isEmpty()) cbuilder.setPrefix(attr);
        attr = (String) session.getAttribute("suffix");
        if (attr != null && !attr.isEmpty()) cbuilder.setSuffix(attr);
        attr = (String) session.getAttribute("nickname");
        if (attr != null && !attr.isEmpty()) cbuilder.setNickname(attr);
        attr = (String) session.getAttribute("organization");
        if (attr != null && !attr.isEmpty()) cbuilder.setOrganization(attr);
        attr = (String) session.getAttribute("jobTitle");
        if (attr != null && !attr.isEmpty()) cbuilder.setJobTitle(attr);
        attr = (String) session.getAttribute("anniversary");
        if (attr != null && !attr.isEmpty()) cbuilder.setAnniversary(attr);
        attr = (String) session.getAttribute("gender");
        if (attr != null && !attr.isEmpty()) cbuilder.setGender(Gender.fromString(attr));
        attr = (String) session.getAttribute("spouse");
        if (attr != null && !attr.isEmpty()) cbuilder.setSpouse(attr);
        attr = (String) session.getAttribute("children");
        if (attr != null && !attr.isEmpty()) cbuilder.setChildren(attr);
        attr = (String) session.getAttribute("hobby");
        if (attr != null && !attr.isEmpty()) cbuilder.setHobby(attr);
        attr = (String) session.getAttribute("assistant");
        if (attr != null && !attr.isEmpty()) cbuilder.setAssistant(attr);

        Phone[] phones = buildPhones(session);
        if (phones != null) cbuilder.setPhones(phones);

        Im[] ims = buildIms(session);
        if (ims != null) cbuilder.setIms(ims);

        Address[] addresses = buildAddresses(session);
        if (addresses != null) cbuilder.setAddresses(addresses);

        Email[] emails = buildEmails(session);
        if (emails != null) cbuilder.setEmails(emails);

        WebURL[] weburls = buildWebUrls(session);
        if (weburls != null) cbuilder.setWeburls(weburls);

        return cbuilder.build();
    }
}
