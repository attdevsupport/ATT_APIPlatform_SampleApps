<%@ attribute name="value" required="true" type="com.att.api.aab.service.ContactResultSet" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
  <t:contacts value="${value.contacts}" />
  <t:quickContacts value="${value.quickContacts}" />
</c:if>
