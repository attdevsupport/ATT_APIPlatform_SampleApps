<%@ attribute name="value" required="true" type="com.att.api.aab.service.Contact[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
  <c:forEach var="contact" items="${value}">
      <fieldset>
          <legend>Individual</legend>
          <t:contact value="${contact}" />
      </fieldset>
  </c:forEach>
</c:if>
