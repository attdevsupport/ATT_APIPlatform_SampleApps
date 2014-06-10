<%@ attribute name="value" required="true" type="com.att.api.aab.service.Im[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<table>
  <thead>
    <th>type</th>
    <th>uri</th>
    <th>preferred</th>
  </thead>
  <tbody>
    <c:forEach var="im" items="${value}">
    <tr>
      <td data-value="type">
        <c:out value="${im.type}" default='-' />
      </td>
      <td data-value="uri">
        <c:out value="${im.imUri}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${im.preferred}" default='-' />
      </td>
    </tr>
    </c:forEach>
  </tbody>
</table>
