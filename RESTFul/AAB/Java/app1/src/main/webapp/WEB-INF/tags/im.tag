<%@ attribute name="value" required="true" type="com.att.api.aab.service.Im" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<table>
  <thead>
    <th>type</th>
    <th>uri</th>
    <th>preferred</th>
  </thead>
  <tbody>
    <tr>
      <td data-value="type">
        <c:out value="${value.type}" default='-' />
      </td>
      <td data-value="uri">
        <c:out value="${value.imUri}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${value.preferred}" default='-' />
      </td>
    </tr>
  </tbody>
</table>
