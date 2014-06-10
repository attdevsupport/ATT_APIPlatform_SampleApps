<%@ attribute name="value" required="true" type="com.att.api.aab.service.Email" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<table>
  <thead>
    <th>type</th>
    <th>address</th>
    <th>preferred</th>
  </thead>
  <tbody>
    <tr>
      <td data-value="type">
        <c:out value="${value.type}" default='-' />
      </td>
      <td data-value="address">
        <c:out value="${value.emailAddress}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${value.preferred}" default='-' />
      </td>
    </tr>
  </tbody>
</table>
