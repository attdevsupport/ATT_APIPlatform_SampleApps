<?php
session_start();
require __DIR__ . '/config.php';
require_once __DIR__ . '/src/Controller/AABController.php';
require_once __DIR__ . '/lib/Util/Util.php';
use Att\Api\Util\Util;

$controller = new AABController();
$controller->handleRequest();
$errors = $controller->getErrors();
$results = $controller->getResults();
?>
<!DOCTYPE html>
<html lang="en">
  <head>
    <title>AT&amp;T Sample Application - Contact Services (Address Book)</title>
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1">
    <link rel="stylesheet" type="text/css" href="style/common.css">
    <link rel="stylesheet" type="text/css" href="style/contacts.css">
    <script src="scripts/utils.js"></script>
    <script src="scripts/contacts.js"></script>
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
          <a href="<?php echo $linkSource; ?>" target="_blank">Source<img src="images/opensource.png" /></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkDownload; ?>" target="_blank">Download<img src="images/download.png"></a>
          <span class="divider"> |&nbsp;</span>
          </li>
          <li>
          <a href="<?php echo $linkHelp; ?>" target="_blank">Help</a>
          </li>
          <li id="back"><a href="#top">Back to top</a>
          </li>
        </ul> <!-- end of links -->
      </div>
      <!-- end of header -->
      <div id="content">
        <div id="contentHeading">
          <h1>AT&amp;T Sample Application - Contact Services (Address Book)</h1>
          <div class="border"></div>
          <div id="introtext">
            <div><b>Server Time:&nbsp;</b><?php echo Util::getServerTime(); ?></div>
            <div><b>Client Time:&nbsp;</b><script>document.write("" + new Date());</script></div>
            <div><b>User Agent:&nbsp;</b><script>document.write("" + navigator.userAgent);</script></div>
          </div> <!-- end of introtext -->
        </div> <!-- end of contentHeading -->

        <!-- Start of Contacts -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">

            <?php if (isset($errors[C_OAUTH_ERROR])) { ?>
              <div class="errorWide">
                <strong>ERROR:</strong>
                <?php echo htmlspecialchars($errors[C_OAUTH_ERROR]); ?>
              </div>
            <?php } ?>

            <a id="contactsToggle"
              href="javascript:toggle('contacts','contactsToggle', 'Contacts');">Contacts</a>
            <div class="toggle" id="contacts">
              <p>
              <input name="pagetype" type="radio" value="1" onclick="showWindows(this);"/>Create Contact 
              <input name="pagetype" type="radio" value="2" onclick="showWindows(this);"/>Update/Delete Contact
              <input name="pagetype" type="radio" value="3" onclick="showWindows(this);"/>Get Contacts
              </p>
              <div id="createContact" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="createContactForm">
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
                        <button id="addPhone" type="button" onclick="addFields('phone', 'number', '8', '<?php echo $phoneClass; ?>', 'phone');">[+]
                        </button> Phones 
                      </legend>
                      <table id="phone">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addIm" type="button" onclick="addFields('im', 'uri', '4', '<?php echo $imClass; ?>', 'im');">[+]
                        </button> IM 
                      </legend>
                      <table id="im">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addAddress" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<?php echo $addressClass; ?>', 'address');">[+]</button> Addresses</legend>
                      <table id="address">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addEmail" type="button" onclick="addFields('email', 'email_address', '4', '<?php echo $emailClass; ?>', 'email');">[+]</button> Emails </legend>
                      <table id="email">
                      </table>
                    </fieldset>
                    <fieldset>
                      <legend>
                        <button id="addWebURLS" type="button" onclick="addFields('weburl', 'url', '3', '<?php echo $weburlClass; ?>', 'weburl');">[+]
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
                            <select id="attachPhoto">
                              <option value="">ATTLogo.jpg</option>
                              <option value="">Coupon.jpeg</option>
                            </select>
                          </td>
                        </tr>
                      </table>
                    </fieldset>
                    <button name="createContact" id="createContact" type="submit" class="submit">Create Contact</button>
                    <button id="resetContact" type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
              <div id="updateContact" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="updateContactForm">
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
                    <fieldset><legend><button id="addPhoneUpd" type="button" onclick="addFields('phone', 'number', '8', '<?php echo $phoneClass; ?>', 'phoneUpd');">[+]</button> Phones </legend> 
                      <table id="phoneUpd">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addImUpd" type="button" onclick="addFields('im', 'uri', '4', '<?php echo $imClass; ?>', 'imUpd');">[+]</button> IM </legend>
                      <table id="imUpd">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addAddressUpd" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<?php echo $addressClass; ?>', 'addressUpd');">[+]</button> Addresses</legend>
                      <table id="addressUpd">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addEmailUpd" type="button" onclick="addFields('email', 'email_address', '4', '<?php echo $emailClass; ?>', 'emailUpd');">[+]</button> Emails </legend>
                      <table id="emailUpd">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addWebURLSUpd" type="button" onclick="addFields('webUrl', 'url', '3', '<?php echo $weburlClass; ?>', 'webUrlsUpd');">[+]</button> WebURLS </legend>
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
                    <button name="updateContact" id="updateContact" type="submit" class="submit">Update Contact</button>
                    <button name="deleteContact" id="deleteContact" type="submit" class="submit">Delete Contact</button>
                    <button id="resetContact" type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
              <div id="getContacts" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="getContactsForm">
                    <table> 
                      <tr>
                        <td>Search:</td>
                        <td><input id="searchVal" placeholder="Search Value" name="searchVal" type="text" /></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button name="getContacts" id="GetContacts" type="submit" class="submit">Get Contacts</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
            </div>
          </div>
          <?php if (isset($results[C_CREATE_CONTACT])) { ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
                <?php echo htmlspecialchars($results[C_CREATE_CONTACT]); ?>
            </div>
          <?php } ?>
          <?php if (isset($results[C_CONTACT_SUCCESS])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>
          <?php 
          if (isset($results[C_GET_CONTACTS])) {
            $resultSet = $results[C_GET_CONTACTS];
            $contacts = $resultSet->getContacts();
          ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>

            <?php if ($contacts != null) ?>
              <?php foreach ($contacts as $contact) { ?>
                <fieldset>
                  <legend>Individual</legend>
                  <fieldset>
                    <legend>Information</legend>
                    <table>
                      <thead>
                        <?php 
                        $mappings = array(
                          'contactId' => $contact->getContactId(),
                          'creationDate' => $contact->getCreationDate(),
                          'modificationDate' => $contact->getModificationDate(),
                          'formattedName' => $contact->getFormattedName(),
                          'firstName' => $contact->getFirstName(),
                          'lastName' => $contact->getLastName(),
                          'prefix' => $contact->getPrefix(),
                          'suffix' => $contact->getSuffix(),
                          'nickName' => $contact->getNickname(),
                          'organization' => $contact->getOrganization(),
                          'jobTitle' => $contact->getJobTitle(),
                          'anniversary' => $contact->getAnniversary(),
                          'gender' => $contact->getGender(),
                          'spouse' => $contact->getSpouse(),
                          'hobby' => $contact->getHobby(),
                          'assistant' => $contact->getAssistant(),
                        );
                        ?>
                        <?php foreach ($mappings as $k => $v) { ?>
                         <?php if ($v != null) { ?> 
                            <th><?php echo htmlspecialchars($k); ?></th>
                          <?php } ?>
                        <?php } ?>
                      </thead>
                      <tbody>
                        <?php foreach ($mappings as $k => $v) { ?>
                         <?php if ($v != null) { ?> 
                            <td data-value="<?php echo $k; ?>">
                            <?php echo htmlspecialchars($v); ?>
                            </td>
                          <?php } ?>
                        <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php if (count($contact->getPhones()) > 0) { ?>
                    <fieldset>
                      <legend>Phones</legend>
                      <table>
                        <thead>
                          <th>type</th>
                          <th>number</th>
                          <th>preferred</th>
                        </thead>
                        <tbody>
                          <?php foreach ($contact->getPhones() as $phone) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $phone->getPhoneType() != null ? htmlspecialchars($phone->getPhoneType()) : '-'; ?>
                              </td>
                              <td data-value="number">
                                <?php echo $phone->getNumber() != null ? htmlspecialchars($phone->getNumber()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $phone->isPreferred() != null ? htmlspecialchars($phone->isPreferred()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getEmails()) > 0) { ?>
                    <fieldset>
                      <legend>Emails</legend>
                      <table>
                        <thead>
                          <th>type</th>
                          <th>address</th>
                          <th>preferred</th>
                        </thead>
                        <tbody>
                          <?php foreach ($contact->getEmails() as $email) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $email->getEmailType() != null ? htmlspecialchars($email->getEmailType()) : '-'; ?>
                              </td>
                              <td data-value="number">
                                <?php echo $email->getEmailAddress() != null ? htmlspecialchars($email->getEmailAddress()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $email->isPreferred() != null ? htmlspecialchars($email->isPreferred()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getIms()) > 0) { ?>
                  <fieldset>
                    <legend>IMs</legend>
                    <table>
                      <thead>
                        <th>type</th>
                        <th>uri</th>
                        <th>preferred</th>
                      </thead>
                      <tbody>
                          <?php foreach ($contact->getIms() as $im) { ?>
                          <tr>
                            <td data-value="type">
                                <?php echo $im->getImType() != null ? htmlspecialchars($im->getImType()) : '-'; ?>
                            </td>
                            <td data-value="uri">
                                <?php echo $im->getImUri() != null ? htmlspecialchars($im->getImUri()) : '-'; ?>
                            </td>
                            <td data-value="preferred">
                                <?php echo $im->isPreferred() != null ? htmlspecialchars($im->isPreferred()) : '-'; ?>
                            </td>
                          </tr>
                          <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getWeburls()) > 0) { ?>
                  <fieldset>
                    <legend>IMs</legend>
                    <table>
                      <thead>
                        <th>type</th>
                        <th>url</th>
                        <th>preferred</th>
                      </thead>
                      <tbody>
                        <?php foreach ($contact->getWeburls() as $weburl) { ?>
                          <tr>
                            <td data-value="type">
                                <?php echo $weburl->getWebUrlType() != null ? htmlspecialchars($weburl->getWebUrlType()) : '-'; ?>
                            </td>
                            <td data-value="uri">
                                <?php echo $weburl->getUrl() != null ? htmlspecialchars($weburl->getUrl()) : '-'; ?>
                            </td>
                            <td data-value="preferred">
                                <?php echo $weburl->isPreferred() != null ? htmlspecialchars($weburl->isPreferred()) : '-'; ?>
                            </td>
                          </tr>
                        <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getAddresses()) > 0) { ?>
                    <fieldset>
                      <legend>Addresses</legend>
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
                          <?php foreach ($contact->getAddresses() as $address) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $address->getAddressType() != null ? htmlspecialchars($address->getAddressType()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $address->isPreferred() != null ? htmlspecialchars($address->isPreferred()) : '-'; ?>
                              </td>
                              <td data-value="po box">
                                <?php echo $address->getPoBox() != null ? htmlspecialchars($address->getPoBox()) : '-'; ?>
                              </td>
                              <td data-value="address line 1">
                                <?php echo $address->getAddressLineOne() != null ? htmlspecialchars($address->getAddressLineOne()) : '-'; ?>
                              </td>
                              <td data-value="address line 2">
                                <?php echo $address->getAddressLineTwo() != null ? htmlspecialchars($address->getAddressLineTwo()) : '-'; ?>
                              </td>
                              <td data-value="city">
                                <?php echo $address->getCity() != null ? htmlspecialchars($address->getCity()) : '-'; ?>
                              </td>
                              <td data-value="state">
                                <?php echo $address->getState() != null ? htmlspecialchars($address->getState()) : '-'; ?>
                              </td>
                              <td data-value="zipcode">
                                <?php echo $address->getZipCode() != null ? htmlspecialchars($address->getZipCode()) : '-'; ?>
                              </td>
                              <td data-value="country">
                                <?php echo $address->getCountry() != null ? htmlspecialchars($address->getCountry()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                </fieldset>
            <?php } ?>
          <?php } ?>
          <?php if (isset($errors[C_CONTACT_ERROR])) { ?>
            <div class="errorWide">
              <strong>ERROR:</strong>
              <?php echo htmlspecialchars($errors[C_CONTACT_ERROR]); ?>
            </div>
          <?php } ?>
        </div>
        <!-- end of Contacts -->

        <!-- Start of My User Profile -->
        <div class="lightBorder"></div>
        <div class="formBox" id="formBox">
          <div id="formContainer" class="formContainer">
            <a id="userProfileToggle"
              href="javascript:toggle('userProfile','userProfileToggle', 'My User Profile');">My User Profile</a>
            <div class="toggle" id="userProfile">
              <form method="post" action="index.php">
                <button name="getMyInfo" id="getMyInfo" type="submit">Get My Info</button>
              </form>
              <p>
              <input name="pagetype" type="radio" value="4"  onclick="showWindows(this);"/>Update MyInfo
              </p>
              <div id="updateMyinfo" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="updateMyinfoForm">
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
                        <button id="addPhoneMyInf" type="button" onclick="addFields('phone', 'number', '8', '<?php echo $phoneClass; ?>', 'phoneMyInf');">[+]
                        </button> Phones 
                      </legend>    
                      <table id="phoneMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addImMyInf" type="button" onclick="addFields('im', 'uri', '4', '<?php echo $imClass; ?>', 'imMyInf');">[+]</button> IM </legend>
                      <table id="imMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addAddressMyInf" type="button" onclick="addFields('address', 'pobox addressLine1 addressLine2 city state zip country', '4', '<?php echo $addressClass; ?>', 'addressMyInf');">[+]</button> Addresses</legend>
                      <table id="addressMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addEmailMyInf" type="button" onclick="addFields('email', 'email_address', '4', '<?php echo $emailClass; ?>', 'emailMyInf');">[+]</button> Emails </legend>
                      <table id="emailMyInf">
                      </table>
                    </fieldset>
                    <fieldset><legend><button id="addWebURLSMyInf" type="button" onclick="addFields('weburl', 'url', '3', '<?php echo $weburlClass; ?>', 'webUrlsMyInf');">[+]</button> WebURLS </legend>
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
                    <button name="updateMyInfo" id="updateMyinfo" type="submit" class="submit">Update MyInfo</button>
                    <button id="resetContact" type="reset" class="submit">Reset Fields</button>
                  </form>
                </div>
              </div>
            </div>
          </div>
          <?php if (isset($results[C_UPDATE_MY_INFO])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>
          <?php 
          if (isset($results[C_MY_INFO])) { 
            $contact = $results[C_MY_INFO];
            $contacts = array($contact);
          ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>

            <?php if ($contacts != null) ?>
              <?php foreach ($contacts as $contact) { ?>
                <fieldset>
                  <legend>Individual</legend>
                  <fieldset>
                    <legend>Information</legend>
                    <table>
                      <thead>
                        <?php 
                        $mappings = array(
                          'contactId' => $contact->getContactId(),
                          'creationDate' => $contact->getCreationDate(),
                          'modificationDate' => $contact->getModificationDate(),
                          'formattedName' => $contact->getFormattedName(),
                          'firstName' => $contact->getFirstName(),
                          'lastName' => $contact->getLastName(),
                          'prefix' => $contact->getPrefix(),
                          'suffix' => $contact->getSuffix(),
                          'nickName' => $contact->getNickname(),
                          'organization' => $contact->getOrganization(),
                          'jobTitle' => $contact->getJobTitle(),
                          'anniversary' => $contact->getAnniversary(),
                          'gender' => $contact->getGender(),
                          'spouse' => $contact->getSpouse(),
                          'hobby' => $contact->getHobby(),
                          'assistant' => $contact->getAssistant(),
                        );
                        ?>
                        <?php foreach ($mappings as $k => $v) { ?>
                         <?php if ($v != null) { ?> 
                            <th><?php echo htmlspecialchars($k); ?></th>
                          <?php } ?>
                        <?php } ?>
                      </thead>
                      <tbody>
                        <?php foreach ($mappings as $k => $v) { ?>
                         <?php if ($v != null) { ?> 
                            <td data-value="<?php echo $k; ?>">
                            <?php echo htmlspecialchars($v); ?>
                            </td>
                          <?php } ?>
                        <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php if (count($contact->getPhones()) > 0) { ?>
                    <fieldset>
                      <legend>Phones</legend>
                      <table>
                        <thead>
                          <th>type</th>
                          <th>number</th>
                          <th>preferred</th>
                        </thead>
                        <tbody>
                          <?php foreach ($contact->getPhones() as $phone) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $phone->getPhoneType() != null ? htmlspecialchars($phone->getPhoneType()) : '-'; ?>
                              </td>
                              <td data-value="number">
                                <?php echo $phone->getNumber() != null ? htmlspecialchars($phone->getNumber()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $phone->isPreferred() != null ? htmlspecialchars($phone->isPreferred()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getEmails()) > 0) { ?>
                    <fieldset>
                      <legend>Emails</legend>
                      <table>
                        <thead>
                          <th>type</th>
                          <th>address</th>
                          <th>preferred</th>
                        </thead>
                        <tbody>
                          <?php foreach ($contact->getEmails() as $email) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $email->getEmailType() != null ? htmlspecialchars($email->getEmailType()) : '-'; ?>
                              </td>
                              <td data-value="number">
                                <?php echo $email->getEmailAddress() != null ? htmlspecialchars($email->getEmailAddress()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $email->isPreferred() != null ? htmlspecialchars($email->isPreferred()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getIms()) > 0) { ?>
                  <fieldset>
                    <legend>IMs</legend>
                    <table>
                      <thead>
                        <th>type</th>
                        <th>uri</th>
                        <th>preferred</th>
                      </thead>
                      <tbody>
                          <?php foreach ($contact->getIms() as $im) { ?>
                          <tr>
                            <td data-value="type">
                                <?php echo $im->getImType() != null ? htmlspecialchars($im->getImType()) : '-'; ?>
                            </td>
                            <td data-value="uri">
                                <?php echo $im->getImUri() != null ? htmlspecialchars($im->getImUri()) : '-'; ?>
                            </td>
                            <td data-value="preferred">
                                <?php echo $im->isPreferred() != null ? htmlspecialchars($im->isPreferred()) : '-'; ?>
                            </td>
                          </tr>
                          <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getWeburls()) > 0) { ?>
                  <fieldset>
                    <legend>IMs</legend>
                    <table>
                      <thead>
                        <th>type</th>
                        <th>url</th>
                        <th>preferred</th>
                      </thead>
                      <tbody>
                        <?php foreach ($contact->getWeburls() as $weburl) { ?>
                          <tr>
                            <td data-value="type">
                                <?php echo $weburl->getWebUrlType() != null ? htmlspecialchars($weburl->getWebUrlType()) : '-'; ?>
                            </td>
                            <td data-value="uri">
                                <?php echo $weburl->getUrl() != null ? htmlspecialchars($weburl->getUrl()) : '-'; ?>
                            </td>
                            <td data-value="preferred">
                                <?php echo $weburl->isPreferred() != null ? htmlspecialchars($weburl->isPreferred()) : '-'; ?>
                            </td>
                          </tr>
                        <?php } ?>
                      </tbody>
                    </table>
                  </fieldset>
                  <?php } ?>
                  <?php if (count($contact->getAddresses()) > 0) { ?>
                    <fieldset>
                      <legend>Addresses</legend>
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
                          <?php foreach ($contact->getAddresses() as $address) { ?>
                            <tr>
                              <td data-value="type">
                                <?php echo $address->getAddressType() != null ? htmlspecialchars($address->getAddressType()) : '-'; ?>
                              </td>
                              <td data-value="preferred">
                                <?php echo $address->isPreferred() != null ? htmlspecialchars($address->isPreferred()) : '-'; ?>
                              </td>
                              <td data-value="po box">
                                <?php echo $address->getPoBox() != null ? htmlspecialchars($address->getPoBox()) : '-'; ?>
                              </td>
                              <td data-value="address line 1">
                                <?php echo $address->getAddressLineOne() != null ? htmlspecialchars($address->getAddressLineOne()) : '-'; ?>
                              </td>
                              <td data-value="address line 2">
                                <?php echo $address->getAddressLineTwo() != null ? htmlspecialchars($address->getAddressLineTwo()) : '-'; ?>
                              </td>
                              <td data-value="city">
                                <?php echo $address->getCity() != null ? htmlspecialchars($address->getCity()) : '-'; ?>
                              </td>
                              <td data-value="state">
                                <?php echo $address->getState() != null ? htmlspecialchars($address->getState()) : '-'; ?>
                              </td>
                              <td data-value="zipcode">
                                <?php echo $address->getZipCode() != null ? htmlspecialchars($address->getZipCode()) : '-'; ?>
                              </td>
                              <td data-value="country">
                                <?php echo $address->getCountry() != null ? htmlspecialchars($address->getCountry()) : '-'; ?>
                              </td>
                            </tr>
                          <?php } ?>
                        </tbody>
                      </table>
                    </fieldset>
                  <?php } ?>
                </fieldset>
              <?php } ?>
            <?php } ?>
          <?php if (isset($errors[C_MY_INFO])) { ?>
            <div class="errorWide">
              <strong>ERROR:</strong>
              <?php echo htmlspecialchars($errors[C_MY_INFO]); ?>
            </div>
          <?php } ?>
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
                  <form method="post" action="index.php" name="createGroupForm">
                    <table> 
                      <tr>
                        <td>groupName</td>
                        <td><input id="groupName" placeholder="groupName" name="groupName" type="text" /></td>
                      </tr>
                      <tr>
                        <td>groupType</td>
                        <td><input id="groupType" placeholder="USER" name="groupType" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button name="createGroup" id="createGrp" type="submit" class="submit">Create Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="updateGroup" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="updateGroupForm">
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
                        <td><input id="groupTypeUpd" placeholder="USER" name="groupType" type="text" disabled/></td>
                      </tr>
                      <tr>
                        <td></td>
                        <td><button name="updateGroup" id="updateGrpBtn" type="submit" class="submit">Update Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="deleteGroup" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="deleteGroupForm">
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
                        <td><button name="deleteGroup" id="deleteGrp" type="submit" class="submit">Delete Group</button></td>
                      </tr>
                    </table>
                  </form>
                </div>
              </div>
              <div id="getGroups" style="display:none;" class="CF">
                <div id="fields" class="menu">
                  <form method="post" action="index.php" name="getGroupForm">
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
                        <td><button name="getGroups" id="searchGrp" type="submit" class="submit">Search Group</button></td>
                      </tr>
                    </table>
                  </form>
                  <label><b>Search Results </b><i>(Displays Max 3 Search Results)</i></label>
                </div>
              </div>
            </div>
          </div>
          <?php if (isset($results[C_CREATE_GROUP])) { ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
              <?php echo htmlspecialchars($results[C_CREATE_GROUP]); ?>
            </div>
          <?php } ?>
          <?php if (isset($results[C_UPDATE_GROUP])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>
          <?php if (isset($results[C_DELETE_GROUP])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>
          <?php if (isset($errors[C_GROUP_ERROR])) { ?>
            <div class="errorWide">
              <strong>ERROR:</strong>
              <?php echo htmlspecialchars($errors[C_GROUP_ERROR]); ?>
            </div>
          <?php } ?>

          <?php if (isset($results[C_GET_GROUPS])) { ?>
          <?php $groups = $results[C_GET_GROUPS]->getGroups(); ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <?php if (count($groups) > 0) { ?>
              <table>
                <thead>
                  <th>groupId</th>
                  <th>groupName</th>
                  <th>groupType</th>
                </thead>
                <tbody>
                  <?php foreach ($groups as $group) { ?>
                    <tr>
                      <td data-value="groupId">
                        <?php echo $group->getGroupId() != null ? htmlspecialchars($group->getGroupId()) : '-'; ?>
                      </td>
                      <td data-value="groupName">
                        <?php echo $group->getGroupName() != null ? htmlspecialchars($group->getGroupName()) : '-'; ?>
                      </td>
                    <td data-value="groupType">
                        <?php echo $group->getGroupType() != null ? htmlspecialchars($group->getGroupType()) : '-'; ?>
                    </td>
                  </tr>
                <?php } ?>
              </tbody>
            </table>
          <?php } ?>
        <?php } ?>
        </div>
        <!-- end of Groups -->

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
                  <form method="post" action="index.php" name="getGroupContactsForm">
                    <fieldset><legend>Get Group Contacts</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdContacts" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button name="getGroupContacts" id="groupIdContactsBtn" type="submit" class="submit">Get Group Contacts</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="index.php" name="addContactsToGroupForm">
                    <fieldset><legend>Add Contacts to Group</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdAddDel" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr>
                        <tr>
                          <td>Contact Id's <i>(comma (,) delimited)</i></td>
                          <td><input id="addContactsGrp" placeholder="contactId(s)" name="contactIds" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button name="addContactsToGroup" id="groupIdContactsAddBtn" type="submit" class="submit">Add Contacts to Group</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="index.php" name="rmContactsFromGroupForm">
                    <fieldset><legend>Remove Contacts from Group</legend>
                      <table>
                        <tr>
                          <td>groupId</td>
                          <td><input id="groupIdRemDel" placeholder="groupId" name="groupId" type="text" /></td>
                        </tr>
                        <tr>
                          <td>Contact Id's <i>(comma (,) delimited)</i></td>
                          <td><input id="remContactsGrp" placeholder="contactId(s)" name="contactIds" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button name="removeContactsFromGroup" id="groupIdContactsRemBtn" type="submit" class="submit">Remove Contacts from Group</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                  <form method="post" action="index.php" name="getContactGroupsForm">
                    <fieldset><legend>Get Contact Groups</legend>
                      <table>
                        <tr>
                          <td>contactId</td>
                          <td><input id="contactsIdGroups" placeholder="contactId" name="contactId" type="text" /></td>
                        </tr> 
                        <tr>
                          <td></td>
                          <td><button name="getContactGroups" id="contactsIdGroupsBtn" type="submit" class="submit">Get Contact Groups</button></td>
                        </tr>
                      </table>
                    </fieldset>
                  </form>
                </div>
              </div>
            </div>
          </div>

          <?php if (isset($results[C_GET_GROUP_CONTACTS])) { ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
              <table>
                <thead>
                  <th>Contact Id</th>
                </thead>
                <tbody>
                  <?php
                    $cids = $results[C_GET_GROUP_CONTACTS];
                    foreach($cids as $cid) { 
                  ?>
                    <tr>
                      <td data-value="Contact Id">
                        <?php echo $cid != null ? htmlspecialchars($cid) : '-'; ?>
                      </td>
                    </tr>
                  <?php } ?>
                </tbody>
              </table>
          <?php } ?>

          <?php if (isset($results[C_ADD_CONTACTS_TO_GROUP])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>
          <?php if (isset($results[C_REMOVE_CONTACTS_FROM_GROUP])) { ?>
            <div class="successWide">
              <strong>SUCCESS</strong>
            </div>
          <?php } ?>

          <?php if (isset($results[C_GET_CONTACT_GROUPS])) { ?>
          <?php $groups = $results[C_GET_CONTACT_GROUPS]->getGroups(); ?>
            <div class="successWide">
              <strong>SUCCESS:</strong>
            </div>
            <?php if (count($groups) > 0) { ?>
              <table>
                <thead>
                  <th>groupId</th>
                  <th>groupName</th>
                  <th>groupType</th>
                </thead>
                <tbody>
                  <?php foreach ($groups as $group) { ?>
                    <tr>
                      <td data-value="groupId">
                        <?php echo $group->getGroupId() != null ? htmlspecialchars($group->getGroupId()) : '-'; ?>
                      </td>
                      <td data-value="groupName">
                        <?php echo $group->getGroupName() != null ? htmlspecialchars($group->getGroupName()) : '-'; ?>
                      </td>
                    <td data-value="groupType">
                        <?php echo $group->getGroupType() != null ? htmlspecialchars($group->getGroupType()) : '-'; ?>
                    </td>
                  </tr>
                <?php } ?>
              </tbody>
            </table>
            <?php } ?>
          <?php } ?>
          <?php if (isset($errors[C_MANAGE_GROUPS_ERROR])) { ?>
            <div class="errorWide">
              <strong>ERROR:</strong>
              <?php echo htmlspecialchars($errors[C_MANAGE_GROUPS_ERROR]); ?>
            </div>
          <?php } ?>
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
        <br>
        To access your apps, please go to
        <a href="https://developer.att.com/developer/mvc/auth/login"
          target="_blank">https://developer.att.com/developer/mvc/auth/login</a>
        <br> For support refer to
        <a href="https://developer.att.com/support">https://developer.att.com/support</a>
        <br> &#169; 2014 AT&amp;T Intellectual Property. All rights
        reserved. <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
        </p>
      </div> <!-- end of footer -->
    </div> <!-- end of page_container -->
  </body>
</html>
