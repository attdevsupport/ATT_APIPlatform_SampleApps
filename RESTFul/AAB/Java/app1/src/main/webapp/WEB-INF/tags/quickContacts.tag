<%@ attribute name="value" required="true" type="com.att.api.aab.service.QuickContact[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
<c:forEach var="qcontact" items="${value}">
    <fieldset>
        <legend>Individual</legend>
        <t:quickContact value="${qcontact}" />
    </fieldset>
</c:forEach>
</c:if>
