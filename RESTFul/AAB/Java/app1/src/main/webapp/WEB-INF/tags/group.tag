<%@ attribute name="value" required="true" type="com.att.api.aab.service.Group" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<c:if test="${not empty value}">
<table>
  <thead>
    <th>groupId</th>
    <th>groupName</th>
    <th>groupType</th>
  </thead>
  <tbody>
    <tr>
      <td data-value="groupId">
        <c:out value="${value.groupId}" default="-" />
      </td>
      <td data-value="groupName">
        <c:out value="${value.groupName}" default="-" />
      </td>
      <td data-value="groupType">
        <c:out value="${value.groupType}" default="-" />
      </td>
    </tr>
  </tbody>
</table>
</c:if>
