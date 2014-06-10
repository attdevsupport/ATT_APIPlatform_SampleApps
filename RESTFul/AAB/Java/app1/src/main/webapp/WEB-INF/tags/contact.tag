<%@ attribute name="value" required="true" type="com.att.api.aab.service.Contact" %>

<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>

<c:if test="${not empty value}">
<fieldset>
  <legend>Information</legend>
  <table class="contacttable">
    <thead>
      <tr>
        <th>ContactId</th>
        <th>CreationDate</th>
        <th>ModificationDate</th>
        <th>FormattedName</th>
        <th>FirstName</th>
        <th>MiddleName</th>
        <th>LastName</th>
        <th>Prefix</th>
        <th>Suffix</th>
        <th>Nickname</th>
        <th>Organization</th>
        <th>JobTitle</th>
        <th>Anniversary</th>
        <th>Gender</th>
        <th>Spouse</th>
        <th>Children</th>
        <th>Hobby</th>
        <th>Assistant</th>
        <th>Photo</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td data-value="ContactId"><c:out value="${value.contactId}" default="-" /></td>
        <td data-value="CreationDate"><c:out value="${value.creationDate}" default="-" /></td>
        <td data-value="ModificationDate"><c:out value="${value.modificationDate}" default="-" /></td>
        <td data-value="FormattedName"><c:out value="${value.formattedName}" default="-" /></td>
        <td data-value="FirstName"><c:out value="${value.firstName}" default="-" /></td>
        <td data-value="MiddleName"><c:out value="${value.middleName}" default="-" /></td>
        <td data-value="LastName"><c:out value="${value.lastName}" default="-" /></td>
        <td data-value="Prefix"><c:out value="${value.prefix}" default="-" /></td>
        <td data-value="Suffix"><c:out value="${value.suffix}" default="-" /></td>
        <td data-value="Nickname"><c:out value="${value.nickname}" default="-" /></td>
        <td data-value="Organization"><c:out value="${value.organization}" default="-" /></td>
        <td data-value="JobTitle"><c:out value="${value.jobTitle}" default="-" /></td>
        <td data-value="Anniversary"><c:out value="${value.anniversary}" default="-" /></td>
        <td data-value="Gender"><c:out value="${value.gender}" default="-" /></td>
        <td data-value="Spouse"><c:out value="${value.spouse}" default="-" /></td>
        <td data-value="Children"><c:out value="${value.children}" default="-" /></td>
        <td data-value="Hobby"><c:out value="${value.hobby}" default="-" /></td>
        <td data-value="Assistant"><c:out value="${value.assistant}" default="-" /></td>
        <td data-value="Photo"><c:out value="${value.photo}" default="-" /></td>
      </tr>
    </tbody>
  </table>
</fieldset>

<c:if test="${not empty value.phones}">
<fieldset>
  <legend>Phones</legend>
  <t:phones value="${value.phones}" />
</fieldset>
</c:if>

<c:if test="${not empty value.emails}">
<fieldset>
  <legend>Emails</legend>
  <t:emails value="${value.emails}" />
</fieldset>
</c:if>

<c:if test="${not empty value.ims}">
<fieldset>
  <legend>IMs</legend>
  <t:ims value="${value.ims}" />
</fieldset>
</c:if>

<c:if test="${not empty value.weburls}">
<fieldset>
  <legend>WebUrls</legend>
  <t:weburls value="${value.weburls}" />
</fieldset>
</c:if>

<c:if test="${not empty value.addresses}">
<fieldset>
  <legend>Addresses</legend>
  <t:addresses value="${value.addresses}" />
</fieldset>
</c:if>

</c:if>
