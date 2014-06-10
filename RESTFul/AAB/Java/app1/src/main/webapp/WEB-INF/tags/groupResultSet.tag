<%@ attribute name="value" required="true" type="com.att.api.aab.service.GroupResultSet" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
    <t:groups value="${value.groups}" />
</c:if>
