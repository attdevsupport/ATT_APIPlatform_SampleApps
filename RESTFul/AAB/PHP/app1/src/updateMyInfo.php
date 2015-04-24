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
        $_POST['myInfoFirstName'], $_POST['myInfoMiddleName'],
        $_POST['myInfoLastName'], $_POST['myInfoPrefix'],
        $_POST['myInfoSuffix'], $_POST['myInfoNickname'],
        $_POST['myInfoOrganization'], $_POST['myInfoJobTitle'],
        $_POST['myInfoAnniversary'], $_POST['myInfoGender'],
        $_POST['myInfoSpouse'], $_POST['myInfoChildren'],
        $_POST['myInfoHobby'], $_POST['myInfoAssistant'], 
    );
    $contactArr = array();
    for ($i = 0; $i < count($keys); ++$i) {
        $key = $keys[$i];
        $value = $values[$i];
        $contactArr[$key] = $value === '' ? null : $value;
    }

    $phonesArr = array();
    $phoneCount = intval($_POST['myInfoPhoneIndex']);
    for ($i = 0; $i < $phoneCount; ++$i) {
        $number = $_POST['myInfoPhoneNumber' . $i];
        $pref = $_POST['myInfoPhonePref' . $i] === 'True' ? true : false;
        $type = $_POST['myInfoPhoneType' . $i];
        $phonesArr[] = array(
            'number' => $number,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['phones'] = array('phone' => $phonesArr);

    $imArr = array();
    $imCount = intval($_POST['myInfoIMIndex']);
    for ($i = 0; $i < $imCount; ++$i) {
        $uri = $_POST['myInfoIMUri' . $i];
        $pref = $_POST['myInfoIMPref' . $i] === 'True' ? true : false;
        $type = $_POST['myInfoIMType' . $i];
        $imArr[] = array(
            'imUri' => $uri,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['ims'] = array('im' => $imArr);

    $addressesArr = array();
    $addressCount = intval($_POST['myInfoAddressIndex']);
    for ($i = 0; $i < $addressCount; ++$i) {
        $pref = $_POST['myInfoAddressPref' . $i] === 'True' ? true : false;
        $type = $_POST['myInfoAddressType' . $i];
        $poBox = $_POST['myInfoAddressPoBox' . $i];
        $addrLine1 = $_POST['myInfoAddressLineOne' . $i];
        $addrLine2 = $_POST['myInfoAddressLineTwo' . $i];
        $city = $_POST['myInfoAddressCity' . $i];
        $state = $_POST['myInfoAddressState' . $i];
        $zipcode = $_POST['myInfoAddressZip' . $i];
        $country = $_POST['myInfoAddressCountry' . $i];

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
    $emailCount = intval($_POST['myInfoEmailIndex']);
    for ($i = 0; $i < $emailCount; ++$i) {
        $emailAddr = $_POST['myInfoEmailAddress' . $i];
        $pref = $_POST['myInfoEmailPref' . $i] === 'True' ? true : false;
        $type = $_POST['myInfoEmailType' . $i];
        $emailsArr[] = array(
            'emailAddress' => $emailAddr,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['emails'] = array('email' => $emailsArr);

    $weburlsArr = array();
    $weburlCount = intval($_POST['myInfoWeburlIndex']);
    for ($i = 0; $i < $weburlCount; ++$i) {
        $url = $_POST['myInfoWeburl' . $i];
        $pref = $_POST['myInfoWeburlPref' . $i] === 'True' ? true : false;
        $type = $_POST['myInfoWeburlType' . $i];
        $weburlsArr[] = array(
            'url' => $url,
            'preferred' => $pref,
            'type' => $type,
        );
    }
    $contactArr['weburls'] = array('webUrl' => $weburlsArr);

    $contactCommon = ContactCommon::fromArray($contactArr);
    $location = $aabService->updateMyInfo($contactCommon);

    $arr = array(
        'success' => true,
        'text' => 'Successfully updated MyInfo'
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
