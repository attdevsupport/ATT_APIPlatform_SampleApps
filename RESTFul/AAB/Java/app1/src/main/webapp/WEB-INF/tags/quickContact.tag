<%@ attribute name="value" required="true" type="com.att.api.aab.service.QuickContact" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
<fieldset>
  <legend>Information</legend>
  <table>
    <thead>
      <tr>
        <th>ContactId</th>
        <th>FormattedName</th>
        <th>FirstName</th>
        <th>MiddleName</th>
        <th>LastName</th>
        <th>Prefix</th>
        <th>Suffix</th>
        <th>Nickname</th>
        <th>Organization</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td data-value="ContactId"><c:out value="${value.contactId}" default="-" /></td>
        <td data-value="FormattedName"><c:out value="${value.formattedName}" default="-" /></td>
        <td data-value="FirstName"><c:out value="${value.firstName}" default="-" /></td>
        <td data-value="MiddleName"><c:out value="${value.middleName}" default="-" /></td>
        <td data-value="LastName"><c:out value="${value.lastName}" default="-" /></td>
        <td data-value="Prefix"><c:out value="${value.prefix}" default="-" /></td>
        <td data-value="Suffix"><c:out value="${value.suffix}" default="-" /></td>
        <td data-value="Nickname"><c:out value="${value.nickname}" default="-" /></td>
        <td data-value="Organization"><c:out value="${value.organization}" default="-" /></td>
      </tr>
    </tbody>
  </table>
</fieldset>

<c:if test="${not empty value.phone}">
<fieldset>
  <legend>Phone</legend>
  <t:phone value="${value.phone}" />
</fieldset>
</c:if>

<c:if test="${not empty value.email}">
<fieldset>
  <legend>Email</legend>
  <t:email value="${value.email}" />
</fieldset>
</c:if>

<c:if test="${not empty value.im}">
<fieldset>
  <legend>IM</legend>
  <t:im value="${value.im}" />
</fieldset>
</c:if>

<c:if test="${not empty value.address}">
<fieldset>
  <legend>Address</legend>
  <t:address value="${value.address}" />
</fieldset>
</c:if>

</c:if>
