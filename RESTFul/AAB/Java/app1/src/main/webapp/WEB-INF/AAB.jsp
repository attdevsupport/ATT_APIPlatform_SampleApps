<!DOCTYPE html>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="t" tagdir="/WEB-INF/tags" %>
<jsp:useBean id="dateutil" class="com.att.api.util.DateUtil" scope="request">
</jsp:useBean>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
Property. All rights reserved. http://developer.att.com For more information
contact developer.support@att.com
-->
<html lang="en">
  <head>
    <title>AT&amp;T Sample Application - Contact Services (Address Book)</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <link rel="stylesheet" type="text/css" href="style/common.css">
    <link rel="stylesheet" type="text/css" href="style/contacts.css">
    <script src="scripts/utils.js"></script>
    <script src="scripts/contacts.js"></script>
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-33466541-1']);
        _gaq.push(['_trackPageview']);

        (function () {
             var ga = document.createElement('script');
             ga.type = 'text/javascript';
             ga.async = true;
             ga.src = ('https:' == document.location.protocol ? 'https://ssl'
                                         : 'http://www')
                                         + '.google-analytics.com/ga.js';
             var s = document.getElementsByTagName('script')[0];
             s.parentNode.insertBefore(ga, s);
         })();
    </script>
  </head>
  <body>
    <div id="pageContainer">
      <div id="header">
        <div class="logo"></div>
        <div id="menuButton" class="hide">
          <a id="jump" href="#nav">Main Navigation</a>
        </div>
        <ul class="links" id="nav">
          <li>
          <a href="#" target="_blank">
            Full Page<img src="images/max.png" />
          </a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkSource}" target="_blank">
            Source<img src="images/opensource.png" />
          </a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkDownload}" target="_blank">
            Download<img src="images/download.png">
          </a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="${cfg.linkHelp}" target="_blank">
            Help
          </a>
          </li>
          <li id="back">
          <a href="#top">
            Back to top
          </a>
          </li>
        </ul> <!-- end of links -->
      </div><!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>
            AT&amp;T Sample Application - Contact Services (Address Book)
          </h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b>${dateutil.serverTime}</div> 
            <div>
              <b>Client Time:</b>
              <script> document.write("" + new Date()); </script>
            </div>
            <div>
              <b>User Agent:</b>
              <script> document.write("" + navigator.userAgent); </script>
            </div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- Start of Contacts -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">

            <c:if test="${not empty oauthError}">
              <div class="errorWide">
                <strong>ERROR:</strong>
                <c:out value="${oauthError}" />
              </div>
            </c:if>

            <a id="contactsToggle" href="javascript:toggle('contacts','contactsToggle', 'Contacts');"> Contacts </a>
            <div class="toggle" id="contacts">
              <p>
              <input name="pagetype" type="radio" value="1" onclick="showWindows(this);"/>Create Contact 
              <input name="pagetype" type="radio" value="2" onclick="showWindows(this);"/>Update/Delete Contact
              <input name="pagetype" type="radio" value="3" onclick="showWindows(this);"/>Get Contacts
              </p>
              <div id="createContact" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="createContactForm">
                    <table> 
                      <tr>
                        <td>firstName</td>
                        <td><input id="firstName" placeholder="firstName" name="firstName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>middleName</td>
                        <td><input id="middleName" placeholder="middleName" name="middleName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>lastName</td>
                        <td><input id="lastName" placeholder="lastName" name="lastName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>prefix</td>
                        <td><input id="prefix" placeholder="prefix" name="prefix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>suffix</td>
                        <td><input id="suffix" placeholder="suffix" name="suffix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>nickname</td>
                        <td><input id="nickname" placeholder="nickname" name="nickname" type="text" /></td>
                      </tr>
                      <tr>
                        <td>organization</td>
                        <td><input id="organization" placeholder="organization" name="organization" type="text" /></td>
                      </tr>
                      <tr>
                        <td>jobTitle</td>
                        <td><input id="jobTitle" placeholder="jobTitle" name="jobTitle" type="text" /></td>
                      </tr>
                      <tr>
                        <td>anniversary</td>
                        <td><input id="anniversary" placeholder="anniversary" name="anniversary" type="text" /></td>
                      </tr>
                      <tr>
                        <td>gender</td>
                        <td><input id="gender" placeholder="gender" name="gender" type="text" /></td>
                      </tr>
                      <tr>
                        <td>spouse</td>
                        <td><input id="spouse" placeholder="spouse" name="spouse" type="text" /></td>
                      </tr>
                      <tr>
                        <td>children</td>
                        <td><input id="children" placeholder="children" name="children" type="text" /></td>
                      </tr>
                      <tr>
                        <td>hobby</td>
                        <td><input id="hobby" placeholder="hobby" name="hobby" type="text" /></td>
                      </tr>
                      <tr>
                        <td>assistant</td>
                        <td><input id="assistant" placeholder="assistant" name="assistant" type="text" /></td>
                      </tr>
                    </table>
                    <fieldset>
                      <legend>
                        <button id="addPhone" type="button" 
                          onclick="addFields('phone', 'number', '8', '<c:out value="${cfg.phoneClass}" />', 'phone');">[+]
                        </button> Phones 
                      </legend>
                      <table id="phone">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addIm" type="button" 
                          onclick="addFields('im', 'uri', '4', '<c:out value="${cfg.imClass}" />', 'im');">[+]
                        </button> IM 
                      </legend>
                      <table id="im">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addAddress" type="button" 
                          onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<c:out value="${cfg.addressClass}" />', 'address');">[+]
                      </button> Addresses</legend>
                      <table id="address">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend><button id="addEmail" type="button" 
                          onclick="addFields('email', 'email_address', '4', '<c:out value="${cfg.emailClass}" />', 'email');">[+]
                        </button> Emails 
                      </legend>
                      <table id="email">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addWebURLS" type="button" 
                          onclick="addFields('weburl', 'url', '3', '<c:out value="${cfg.weburlClass}" />', 'weburl');">[+]
                        </button> WebURLS 
                      </legend>
                      <table id="weburl">
                      </table>
                    </fieldset>
                    <fieldset><legend>Photo</legend>
                      <table>
                        <tr>
                          <td>
                            <label> Upload Photo : </label> 
                          </td>
                          <td>
                            <select name="attachPhoto" id="attachPhoto">
                              <option value="ATTLogo.jpg">ATTLogo.jpg</option>
                              <option value="Coupon.jpeg">Coupon.jpeg</option>
                            </select>
                          </td>
                        </tr>
                      </table>
                    </fieldset>
                    <button id="createContact" name="createContact" type="submit" class="submit">Create Contact</button>
                    <button id="resetContact" type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
              <div id="updateContact" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="updateContactForm">
                    <table> 
                      <tr>
                        <td>contactId</td>
                        <td>
                          <input id="contactIdUpd" placeholder="contactId" name="contactId" type="text" />
                        </td>
                      </tr>
                      <tr>
                        <td>formattedName</td>
                        <td><input id="formattedNameUpd" placeholder="formattedName" name="formattedName" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td>firstName</td>
                        <td><input id="firstNameUpd" placeholder="firstName" name="firstName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>middleName</td>
                        <td><input id="middleNameUpd" placeholder="middleName" name="middleName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>lastName</td>
                        <td><input id="lastNameUpd" placeholder="lastName" name="lastName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>prefix</td>
                        <td><input id="prefixUpd" placeholder="prefix" name="prefix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>suffix</td>
                        <td><input id="suffixUpd" placeholder="suffix" name="suffix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>nickname</td>
                        <td><input id="nicknameUpd" placeholder="nickname" name="nickname" type="text" /></td>
                      </tr>
                      <tr>
                        <td>organization</td>
                        <td><input id="organizationUpd" placeholder="organization" name="organization" type="text" /></td>
                      </tr>
                      <tr>
                        <td>jobTitle</td>
                        <td><input id="jobTitleUpd" placeholder="jobTitle" name="jobTitle" type="text" /></td>
                      </tr>
                      <tr>
                        <td>anniversary</td>
                        <td><input id="anniversaryUpd" placeholder="anniversary" name="anniversary" type="text" /></td>
                      </tr>
                      <tr>
                        <td>gender</td>
                        <td><input id="genderUpd" placeholder="gender" name="gender" type="text" /></td>
                      </tr>
                      <tr>
                        <td>spouse</td>
                        <td><input id="spouseUpd" placeholder="spouse" name="spouse" type="text" /></td>
                      </tr>
                      <tr>
                        <td>children</td>
                        <td><input id="childrenUpd" placeholder="children" name="children" type="text" /></td>
                      </tr>
                      <tr>
                        <td>hobby</td>
                        <td><input id="hobbyUpd" placeholder="hobby" name="hobby" type="text" /></td>
                      </tr>
                      <tr>
                        <td>assistant</td>
                        <td><input id="assistantUpd" placeholder="assistant" name="assistant" type="text" /></td>
                      </tr>
                    </table>
                    <fieldset>
                      <legend>
                        <button id="addPhoneUpd" type="button" 
                          onclick="addFields('phone', 'number', '8', '<c:out value="${cfg.phoneClass}" />', 'phoneUpd');">[+]
                        </button> Phones 
                      </legend> 
                      <table id="phoneUpd">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addImUpd" type="button" 
                          onclick="addFields('im', 'uri', '4', '<c:out value="${cfg.imClass}" />', 'imUpd');">[+]
                        </button> IM 
                      </legend>
                      <table id="imUpd">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addAddressUpd" type="button" 
                          onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<c:out value="${cfg.addressClass}" />', 'addressUpd');">[+]
                        </button> Addresses
                      </legend>
                      <table id="addressUpd">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addEmailUpd" type="button" 
                          onclick="addFields('email', 'email_address', '4', '<c:out value="${cfg.emailClass}" />', 'emailUpd');">[+]
                        </button> Emails 
                      </legend>
                      <table id="emailUpd">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addWebURLSUpd" type="button" 
                          onclick="addFields('weburl', 'url', '3', '<c:out value="${cfg.weburlClass}" />', 'webUrlsUpd');">[+]
                        </button> WebURLS 
                      </legend>
                      <table id="webUrlsUpd">
                      </table>
                    </fieldset>
                    <fieldset><legend>Photo <i>(only for update operation)</i></legend>
                      <table>
                        <tr>
                          <td>
                            <label> Upload Photo : </label> 
                          </td>
                          <td>
                            <select id="attachPhotoUpd">
                              <option value="">ATTLogo.jpg</option>
                              <option value="">Coupon.jpeg</option>
                            </select>
                          </td>
                        </tr>
                      </table>
                    </fieldset>
                    <button name="updateContact" type="submit" class="submit">Update Contact</button>
                    <button name="deleteContact" type="submit" class="submit">Delete Contact</button>
                    <button type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
              <div id="getContacts" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="getContactsForm">
                    <table> 
                      <tr>
                        <td>Search:</td>
                        <td> <input id="searchVal" placeholder="Search Value" name="searchVal" type="text" /> </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button name="GetContacts" id="GetContacts" type="submit" class="submit">Get Contacts</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
            </div>
          </div>
          <c:if test="${not empty createContact}">
          <div class="successWide">
            <strong>SUCCESS:</strong>
            <c:out value="${createContact}" />
          </div>
          </c:if>
          <c:if test="${not empty successContact}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
          </c:if>

          <c:if test="${not empty contactResultSet}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <t:contactResultSet value="${contactResultSet}" />
          </c:if>

          <c:if test="${not empty contactError}">
            <div class="errorWide">
              <strong>ERROR:</strong>
              <c:out value="${contactError}" />
            </div>
          </c:if>
        </div>
        <!-- end of Contacts -->

        <!-- Start of My User Profile -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="userProfileToggle"
              href="javascript:toggle('userProfile','userProfileToggle', 'My User Profile');">My User Profile</a>
            <div class="toggle" id="userProfile">
              <form method="post" action="">
                <button id="getMyInfo" name="getMyInfo" type="submit">Get My Info</button>
              </form>
              <p>
              <input name="pagetype" type="radio" value="4"  onclick="showWindows(this);"/>Update MyInfo
              </p>
              <div id="updateMyinfo" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="updateMyinfoForm">
                    <table> 
                      <tr>
                        <td>formattedName</td>
                        <td><input id="formattedNameMyInf" placeholder="formattedName" name="formattedName" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td>firstName</td>
                        <td><input id="firstNameMyInf" placeholder="firstName" name="firstName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>middleName</td>
                        <td><input id="middleNameMyInf" placeholder="middleName" name="middleName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>lastName</td>
                        <td><input id="lastNameMyInf" placeholder="lastName" name="lastName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>prefix</td>
                        <td><input id="prefixMyInf" placeholder="prefix" name="prefix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>suffix</td>
                        <td><input id="suffixMyInf" placeholder="suffix" name="suffix" type="text" /></td>
                      </tr>
                      <tr>
                        <td>nickname</td>
                        <td><input id="nicknameMyInf" placeholder="nickname" name="nickname" type="text" /></td>
                      </tr>
                      <tr>
                        <td>organization</td>
                        <td><input id="organizationMyInf" placeholder="organization" name="organization" type="text" /></td>
                      </tr>
                      <tr>
                        <td>jobTitle</td>
                        <td><input id="jobTitleMyInf" placeholder="jobTitle" name="jobTitle" type="text" /></td>
                      </tr>
                      <tr>
                        <td>anniversary</td>
                        <td><input id="anniversaryMyInf" placeholder="anniversary" name="anniversary" type="text" /></td>
                      </tr>
                      <tr>
                        <td>gender</td>
                        <td><input id="genderMyInf" placeholder="gender" name="gender" type="text" /></td>
                      </tr>
                      <tr>
                        <td>spouse</td>
                        <td><input id="spouseMyInf" placeholder="spouse" name="spouse" type="text" /></td>
                      </tr>
                      <tr>
                        <td>children</td>
                        <td><input id="childrenMyInf" placeholder="children" name="children" type="text" /></td>
                      </tr>
                      <tr>
                        <td>hobby</td>
                        <td><input id="hobbyMyInf" placeholder="hobby" name="hobby" type="text" /></td>
                      </tr>
                      <tr>
                        <td>assistant</td>
                        <td><input id="assistantMyInf" placeholder="assistant" name="assistant" type="text" /></td>
                      </tr>
                    </table>
                    <fieldset>
                      <legend>
                        <button id="addPhoneMyInf" type="button" 
                          onclick="addFields('phone', 'number', '8', '<c:out value="${cfg.phoneClass}" />', 'phoneMyInf');">[+]
                        </button> Phones 
                      </legend>    
                      <table id="phoneMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addImMyInf" type="button" 
                          onclick="addFields('im', 'uri', '4', '<c:out value="${cfg.imClass}" />', 'imMyInf');">[+]
                      </button> IM </legend>
                      <table id="imMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addAddressMyInf" type="button" 
                          onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<c:out value="${cfg.addressClass}" />', 'addressMyInf');">[+]
                      </button> Addresses</legend>
                      <table id="addressMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addEmailMyInf" type="button" 
                          onclick="addFields('email', 'email_address', '4', '<c:out value="${cfg.emailClass}" />', 'emailMyInf');">[+]
                      </button> Emails </legend>
                      <table id="emailMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addWebURLSMyInf" type="button" 
                          onclick="addFields('weburl', 'url', '3', '<c:out value="${cfg.weburlClass}" />', 'webUrlsMyInf');">[+]
                      </button> WebURLS </legend>
                      <table id="webUrlsMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend>Photo <i>(only for update operation)</i></legend>
                      <table>
                        <tr>
                          <td>
                            <label> Upload Photo : </label> 
                          </td>
                          <td>
                            <select id="attachPhotoMyInf" name="photo_image">
                              <option value="">ATTLogo.jpg</option>
                              <option value="">Coupon.jpeg</option>
                            </select>
                          </td>
                        </tr>
                      </table>
                    </fieldset>
                    <button id="updateMyinfo" name="updateMyInfo" type="submit" class="submit">Update MyInfo</button>
                    <button id="resetContact" type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
            </div>
          </div>

          <c:if test="${not empty updateMyInfo}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
          </c:if>

          <c:if test="${not empty myInfo}">
          <fieldset>
            <legend>Individual</legend>
            <t:contact value="${myInfo}" />
          </fieldset>
          </c:if>
          <c:if test="${not empty myInfoError}">
          <div class="errorWide">
            <strong>ERROR:</strong>
            <c:out value="${myInfoError}" />
          </div>
          </c:if>
        </div> <!-- end of My User Profile -->

        <!-- Start of Groups -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="groupsToggle"
              href="javascript:toggle('groups','groupsToggle', 'Groups');">Groups</a>
            <div class="toggle" id="groups">
              <p>
              <input name="pagetype" type="radio" value="5"  onclick="showWindows(this);"/>Create Group
              <input name="pagetype" type="radio" value="6"  onclick="showWindows(this);"/>Update Group
              <input name="pagetype" type="radio" value="7"  onclick="showWindows(this);"/>Delete Group
              <input name="pagetype" type="radio" value="8"  onclick="showWindows(this);"/>Get Groups
              </p>
              <div id="createGroup" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="createGroupForm">
                    <table> 
                      <tr>
                        <td>groupName</td>
                        <td><input id="groupName" placeholder="groupName" name="groupName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>groupType</td>
                        <td><input id="groupType" value="USER" name="groupType" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button id="createGrp" name="createGrp" type="submit" class="submit">Create Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="updateGroup" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="updateGroupForm">
                    <table>
                      <tr>
                        <td>groupId</td>
                        <td><input id="groupIdUpd" placeholder="groupId" name="groupId" type="text" /></td>
                      </tr> 
                      <tr>
                        <td>groupName</td>
                        <td><input id="groupNameUpd" placeholder="groupName" name="groupName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>groupType</td>
                        <td><input id="groupTypeUpd" value="USER" name="groupType" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button id="updateGrpBtn" name="updateGrp" type="submit" class="submit">Update Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="deleteGroup" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="deleteGroupForm">
                    <table>
                      <tr>
                        <td>groupId</td>
                        <td><input id="groupIdDel" placeholder="groupId" name="groupId" type="text" /></td>
                      </tr> 
                      <tr>
                        <td></td>
                        <td></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button id="deleteGrp" name="deleteGrp" type="submit" class="submit">Delete Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="getGroups" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="getGroupForm">
                    <table> 
                      <tr>
                        <td>groupName</td>
                        <td><input id="getGroupName" placeholder="groupName" name="getGroupName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>order</td>
                        <td><select name="order">
                            <option value="ASC">Ascending</option>
                            <option value="DESC">Descending</option>
                          </select>
                        </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button id="searchGrp" name="getGroups" type="submit" class="submit">Search Group</button></td>
                      </tr>
                    </table>
                  </form>
                  <label><b>Search Results </b><i>(Displays Max 3 Search Results)</i></label>
                </div>
              </div>
            </div>
          </div>
          <c:if test="${not empty createGroup}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
              <c:out value="${createGroup}" />
            </div>
          </c:if>
          <c:if test="${not empty successGroup}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
          </c:if>
          <c:if test="${not empty groupError}">
            <div class="errorWide">
              <strong>ERROR:</strong>
              <c:out value="${groupError}" />
            </div>
          </c:if>

          <c:if test="${not empty groups}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <t:groupResultSet value="${groups}" />
          </c:if>
        </div><!-- end of Groups -->

        <!-- Start of Managing Groups/Contacts -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="grpContactsToggle"
              href="javascript:toggle('grpContacts','grpContactsToggle', 'Managing Groups/Contacts');">Managing Groups/Contacts</a>
            <div class="toggle" id="grpContacts">
              <p>
              <input name="pagetype" type="radio" value="9"  onclick="showWindows(this);"/>Contact and Group Operations
              </p>
              <div id="getGroupContacts" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="" name="getGroupContactsForm">
                    <fieldset><legend>Get Group Contacts</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdContacts" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button id="groupIdContactsBtn" name="groupIdContactsBtn" type="submit" class="submit">Get Group Contacts</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="" name="addContactsToGroupForm">
                    <fieldset><legend>Add Contacts to Group</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdAddDel" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr>
                        <tr>
                          <td>Contact Id's <i>(comma (,) delimited)</i></td>
                          <td><input id="addContactsGrp" placeholder="contactId(s)" name="contactId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button id="groupIdContactsAddBtn" name="groupIdContactsAddBtn" type="submit" class="submit">Add Contacts to Group</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="" name="rmContactsFromGroupForm">
                    <fieldset><legend>Remove Contacts from Group</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdRemDel" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr>
                        <tr>
                          <td>Contact Id's <i>(comma (,) delimited)</i></td>
                          <td><input id="remContactsGrp" placeholder="contactId(s)" name="contactId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button id="groupIdContactsRemBtn" name="groupIdContactsRemBtn" type="submit" class="submit">Remove Contacts from Group</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="" name="getContactGroupsForm">
                    <fieldset><legend>Get Contact Groups</legend>
                      <table>
                        <tr>
                          <td>contactId</td>
                          <td><input id="contactsIdGroups" placeholder="contactId" name="contactId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button id="contactsIdGroupsBtn" name="contactIdGroupsBtn" type="submit" class="submit">Get Contact Groups</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                </div>
              </div>
            </div>
          </div>

          <c:if test="${not empty groupContactIds}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <table>
              <thead>
                <th>Contact Id</th>
              </thead>
              <tbody>
                <c:forEach var="id" items="${groupContactIds}">
                  <tr>
                    <td data-value="Contact Id">
                      <c:out value="${id}" />
                    </td>
                  </tr>
                </c:forEach>
              </tbody>
            </table>
          </c:if>

          <c:if test="${not empty createGroup}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
              <c:out value="${createGroup}" />
            </div>
          </c:if>

          <c:if test="${not empty contactGroups}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <t:groupResultSet value="${contactGroups}" />
          </c:if>

          <c:if test="${not empty manageGroups}">
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
          </c:if>

          <c:if test="${not empty manageGroupsError}">
            <div class="errorWide">
              <strong>ERROR:</strong>
              <c:out value="${manageGroupsError}" />
            </div>
          </c:if>

        </div> <!-- end of Managing Groups/Contacts -->
        <div class="lightBorder">
        </div> <!-- End of Operations  -->
      </div>
      <div class="border"></div>
      <div id="footer">
        <div id="powered_by">Powered by AT&amp;T Cloud Architecture</div>
        <p>
        The Application hosted on this site are working examples intended to
        be used for reference in creating products to consume AT&amp;T
        Services and not meant to be used as part of your product. The data
        in these pages is for test purposes only and intended only for use
        as a reference in how the services perform. <br>
        <br> For download of tools and documentation, please go to <a
          href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
        <br> For more information contact <a
          href="mailto:developer.support@att.com">developer.support@att.com</a>
        <br>
        <br> &#169; 2014 AT&amp;T Intellectual Property. All rights
        reserved. <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
