<%@ attribute name="value" required="true" type="com.att.api.aab.service.WebURL[]" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<c:if test="${not empty value}">
<table>
  <thead>
    <th>type</th>
    <th>url</th>
    <th>preferred</th>
  </thead>
  <tbody>
    <c:forEach var="web" items="${value}">
    <tr>
      <td data-value="type">
        <c:out value="${web.type}" default='-' />
      </td>
      <td data-value="url">
        <c:out value="${web.url}" default='-' />
      </td>
      <td data-value="preferred">
        <c:out value="${web.preferred}" default='-' />
      </td>
    </tr>
    </c:forEach>
  </tbody>
</table>
</c:if>
