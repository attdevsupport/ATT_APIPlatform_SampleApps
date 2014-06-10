<%@ attribute name="value" required="true" type="com.att.api.aab.service.Group[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<c:if test="${not empty value}">
<table>
  <thead>
    <th>groupId</th>
    <th>groupName</th>
    <th>groupType</th>
  </thead>
  <tbody>
    <c:forEach var="group" items="${value}">
    <tr>
      <td data-value="groupId">
        <c:out value="${group.groupId}" default="-" />
      </td>
      <td data-value="groupName">
        <c:out value="${group.groupName}" default="-" />
      </td>
      <td data-value="groupType">
        <c:out value="${group.groupType}" default="-" />
      </td>
    </tr>
    </c:forEach>
  </tbody>
</table>
</c:if>
