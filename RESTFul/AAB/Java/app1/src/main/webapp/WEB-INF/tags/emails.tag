<%@ attribute name="value" required="true" type="com.att.api.aab.service.Email[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<table>
  <thead>
    <th>type</th>
    <th>address</th>
    <th>preferred</th>
  </thead>
  <tbody>
    <c:forEach var="email" items="${value}">
    <tr>
      <td data-value="type">
        <c:out value="${email.type}" default='-' />
      </td>
      <td data-value="address">
        <c:out value="${email.emailAddress}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${email.preferred}" default='-' />
      </td>
    </tr>
    </c:forEach>
  </tbody>
</table>
