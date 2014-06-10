package com.att.api.aab.controller;

import java.io.IOException;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.att.api.aab.model.ConfigBean;
import com.att.api.aab.service.AABService;
import com.att.api.aab.service.Contact;
import com.att.api.aab.service.ContactResultSet;
import com.att.api.aab.service.Gender;
import com.att.api.aab.service.Group;
import com.att.api.aab.service.GroupResultSet;
import com.att.api.aab.service.PageParams;
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

        //TODO: update to handle photos
        //attr = (String) session.getAttribute("attachPhoto");
        //if (attr != null) cbuilder.setPhoto(attr);

        return cbuilder.build();
    }

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
}
