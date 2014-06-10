<%@ attribute name="value" required="true" type="com.att.api.aab.service.Phone[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<c:if test="${not empty value}">
<table>
  <thead>
    <tr>
      <th>type</th>
      <th>number</th>
      <th>preferred</th>
    </tr>
  </thead>
  <tbody>
    <c:forEach var="phone" items="${value}">
    <tr>
      <td data-value="type">
        <c:out value="${phone.type}" default='-' />
      </td>
      <td data-value="number">
        <c:out value="${phone.number}" default='-' />
      </td>
      <c:if test="${not empty phone.preferred}">
      <td data-value="preferred">
        <c:out value="${phone.preferred}" default='-' />
      </td>
      </c:if>
    </tr>
    </c:forEach>
  </tbody>
</table>
</c:if>
