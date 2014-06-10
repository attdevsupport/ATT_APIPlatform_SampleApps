<%@ attribute name="value" required="true" type="com.att.api.aab.service.Address" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<table>
  <thead>
    <th>type</th>
    <th>preferred</th>
    <th>po box</th>
    <th>address line 1</th>
    <th>address line 2</th>
    <th>city</th>
    <th>state</th>
    <th>zipcode</th>
    <th>country</th>
  </thead>
  <tbody>
    <tr>
      <td data-value="type">
        <c:out value="${value.type}" default="-" />
      </td>
      <td data-value="preferred">
        <c:out value="${value.preferred}" default="-" />
      </td>
      <td data-value="po box">
        <c:out value="${value.poBox}" default="-" />
      </td>
      <td data-value="address line 1">
        <c:out value="${value.addressLineOne}" default="-" />
      </td>
      <td data-value="address line 2">
        <c:out value="${value.addressLineTwo}" default="-" />
      </td>
      <td data-value="city">
        <c:out value="${value.city}" default="-" />
      </td>
      <td data-value="state">
        <c:out value="${value.state}" default="-" />
      </td>
      <td data-value="zipcode">
        <c:out value="${value.zipcode}" default="-" />
      </td>
      <td data-value="country">
        <c:out value="${value.country}" default="-" />
      </td>
    </tr>
  </tbody>
</table>
