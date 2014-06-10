<%@ attribute name="value" required="true" type="com.att.api.aab.service.Phone" %>

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
    <tr>
      <td data-value="type">
        <c:out value="${value.type}" default='-' />
      </td>
      <td data-value="number">
        <c:out value="${value.number}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${value.preferred}" default='-' />
      </td>
    </tr>
  </tbody>
</table>
</c:if>
