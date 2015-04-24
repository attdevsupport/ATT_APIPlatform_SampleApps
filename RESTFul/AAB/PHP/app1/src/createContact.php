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
require_once __DIR__ . '/../lib/AAB/ContactCommon.php';

use Att\Api\AAB\AABService;
use Att\Api\AAB\ContactCommon;

$arr = null;
try {
    envinit();
    $aabService = new AABService(getFqdn(), getSessionToken());
    $keys = array(
        'firstName', 'middleName', 'lastName', 'prefix', 'suffix', 'nickname',
        'organization', 'jobTitle', 'anniversary', 'gender', 'spouse',
        'children', 'hobby', 'assistant'
    );
    $values = array(
        $_POST['createFirstName'], $_POST['createMiddleName'],
        $_POST['createLastName'], $_POST['createPrefix'],
        $_POST['createSuffix'], $_POST['createNickname'],
        $_POST['createOrganization'], $_POST['createJobTitle'],
        $_POST['createAnniversary'], $_POST['createGender'],
        $_POST['createSpouse'], $_POST['createChildren'],
        $_POST['createHobby'], $_POST['createAssistant'], 
    );
    $contactArr = array();
    for ($i = 0; $i < count($keys); ++$i) {
        $key = $keys[$i];
        $value = $values[$i];
        $contactArr[$key] = $value === '' ? null : $value;
    }

    $phonesArr = array();
    $phoneCount = intval($_POST['createPhoneIndex']);
    for ($i = 0; $i < $phoneCount; ++$i) {
        $number = $_POST['createPhoneNumber' . $i];
        $pref = $_POST['createPhonePref' . $i] === 'True' ? true : false;
        $type = $_POST['createPhoneType' . $i];
        $phonesArr[] = array(
            'number' => $number,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['phones'] = array('phone' => $phonesArr);

    $imArr = array();
    $imCount = intval($_POST['createIMIndex']);
    for ($i = 0; $i < $imCount; ++$i) {
        $uri = $_POST['createIMUri' . $i];
        $pref = $_POST['createIMPref' . $i] === 'True' ? true : false;
        $type = $_POST['createIMType' . $i];
        $imArr[] = array(
            'imUri' => $uri,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['ims'] = array('im' => $imArr);

    $addressesArr = array();
    $addressCount = intval($_POST['createAddressIndex']);
    for ($i = 0; $i < $addressCount; ++$i) {
        $pref = $_POST['createAddressPref' . $i] === 'True' ? true : false;
        $type = $_POST['createAddressType' . $i];
        $poBox = $_POST['createAddressPoBox' . $i];
        $addrLine1 = $_POST['createAddressLineOne' . $i];
        $addrLine2 = $_POST['createAddressLineTwo' . $i];
        $city = $_POST['createAddressCity' . $i];
        $state = $_POST['createAddressState' . $i];
        $zipcode = $_POST['createAddressZip' . $i];
        $country = $_POST['createAddressCountry' . $i];

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
    $emailCount = intval($_POST['createEmailIndex']);
    for ($i = 0; $i < $emailCount; ++$i) {
        $emailAddr = $_POST['createEmailAddress' . $i];
        $pref = $_POST['createEmailPref' . $i] === 'True' ? true : false;
        $type = $_POST['createEmailType' . $i];
        $emailsArr[] = array(
            'emailAddress' => $emailAddr,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['emails'] = array('email' => $emailsArr);

    $weburlsArr = array();
    $weburlCount = intval($_POST['createWeburlIndex']);
    for ($i = 0; $i < $weburlCount; ++$i) {
        $url = $_POST['createWeburl' . $i];
        $pref = $_POST['createWeburlPref' . $i] === 'True' ? true : false;
        $type = $_POST['createWeburlType' . $i];
        $weburlsArr[] = array(
            'url' => $url,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['weburls'] = array('webUrl' => $weburlsArr);

    $contactCommon = ContactCommon::fromArray($contactArr);
    $location = $aabService->createContact($contactCommon);

    $arr = array(
        'success' => true,
        'text' => $location
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
