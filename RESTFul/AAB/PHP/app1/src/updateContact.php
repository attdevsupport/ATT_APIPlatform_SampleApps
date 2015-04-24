<?php
/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
session_start();

require_once __DIR__ . '/common.php';
require_once __DIR__ . '/../lib/AAB/AABService.php';
require_once __DIR__ . '/../lib/AAB/Contact.php';

use Att\Api\AAB\AABService;
use Att\Api\AAB\Contact;

$arr = null;
try {
    envinit();
    $aabService = new AABService(getFqdn(), getSessionToken());
    $keys = array(
        'contactId', 'firstName', 'middleName', 'lastName', 'prefix', 'suffix',
        'nickname', 'organization', 'jobTitle', 'anniversary', 'gender',
        'spouse', 'children', 'hobby', 'assistant'
    );
    $values = array(
        $_POST['updateContactId'], $_POST['updateFirstName'],
        $_POST['updateMiddleName'], $_POST['updateLastName'],
        $_POST['updatePrefix'], $_POST['updateSuffix'],
        $_POST['updateNickname'], $_POST['updateOrganization'],
        $_POST['updateJobTitle'], $_POST['updateAnniversary'],
        $_POST['updateGender'], $_POST['updateSpouse'],
        $_POST['updateChildren'], $_POST['updateHobby'],
        $_POST['updateAssistant'], 
    );
    $contactArr = array();
    for ($i = 0; $i < count($keys); ++$i) {
        $key = $keys[$i];
        $value = $values[$i];
        $contactArr[$key] = $value === '' ? null : $value;
    }

    $phonesArr = array();
    $phoneCount = intval($_POST['updatePhoneIndex']);
    for ($i = 0; $i < $phoneCount; ++$i) {
        $number = $_POST['updatePhoneNumber' . $i];
        $pref = $_POST['updatePhonePref' . $i] === 'True' ? true : false;
        $type = $_POST['updatePhoneType' . $i];
        $phonesArr[] = array(
            'number' => $number,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['phones'] = array('phone' => $phonesArr);

    $imArr = array();
    $imCount = intval($_POST['updateIMIndex']);
    for ($i = 0; $i < $imCount; ++$i) {
        $uri = $_POST['updateIMUri' . $i];
        $pref = $_POST['updateIMPref' . $i] === 'True' ? true : false;
        $type = $_POST['updateIMType' . $i];
        $imArr[] = array(
            'imUri' => $uri,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['ims'] = array('im' => $imArr);

    $addressesArr = array();
    $addressCount = intval($_POST['updateAddressIndex']);
    for ($i = 0; $i < $addressCount; ++$i) {
        $pref = $_POST['updateAddressPref' . $i] === 'True' ? true : false;
        $type = $_POST['updateAddressType' . $i];
        $poBox = $_POST['updateAddressPoBox' . $i];
        $addrLine1 = $_POST['updateAddressLineOne' . $i];
        $addrLine2 = $_POST['updateAddressLineTwo' . $i];
        $city = $_POST['updateAddressCity' . $i];
        $state = $_POST['updateAddressState' . $i];
        $zipcode = $_POST['updateAddressZip' . $i];
        $country = $_POST['updateAddressCountry' . $i];

        $addressesArr[] = array(
            'preferred' => $pref,
            'type' => $type,
            'poBox' => $poBox,
            'addressLine1' => $addrLine1,
            'addressLine2' => $addrLine2,
            'city' => $city,
            'state' => $state,
            'zip' => $zipcode,
            'country' => $country,
        );
    }
    $contactArr['addresses'] = array('address' => $addressesArr);

    $emailsArr = array();
    $emailCount = intval($_POST['updateEmailIndex']);
    for ($i = 0; $i < $emailCount; ++$i) {
        $emailAddr = $_POST['updateEmailAddress' . $i];
        $pref = $_POST['updateEmailPref' . $i] === 'True' ? true : false;
        $type = $_POST['updateEmailType' . $i];
        $emailsArr[] = array(
            'emailAddress' => $emailAddr,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['emails'] = array('email' => $emailsArr);

    $weburlsArr = array();
    $weburlCount = intval($_POST['updateWeburlIndex']);
    for ($i = 0; $i < $weburlCount; ++$i) {
        $url = $_POST['updateWeburl' . $i];
        $pref = $_POST['updateWeburlPref' . $i] === 'True' ? true : false;
        $type = $_POST['updateWeburlType' . $i];
        $weburlsArr[] = array(
            'url' => $url,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['weburls'] = array('webUrl' => $weburlsArr);

    $contact = Contact::fromArray($contactArr);
    $aabService->updateContact($contact);

    $arr = array(
        'success' => true,
        'text' => 'Successfully updated contact.'
    );
} catch (Exception $e) {
    $arr = array(
        'success' => false,
        'text' => $e->getMessage()
    );
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
