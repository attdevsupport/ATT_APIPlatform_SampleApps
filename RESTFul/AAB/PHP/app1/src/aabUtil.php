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

require_once __DIR__ . '/../lib/Util/Util.php';

use Att\Api\Util\Util;

function generatePhonesTable($contactId, $phones) {
    if ($phones === null || count($phones) == 0)
        return null;

    $table = array();
    $table['caption'] = "Contact ($contactId) Phones:";
    $table['headers'] = array('type', 'number', 'preferred');
    $values = array();
    foreach ($phones as $phone) {
        $values[] = Util::convertNulls(array(
            $phone->getPhoneType(),
            $phone->getNumber(),
            $phone->isPreferred(),
        ));
    }
    $table['values'] = $values;
    return $table;
}

function generateEmailsTable($contactId, $emails) {
    if ($emails === null || count($emails) == 0)
        return null;

    $table = array();
    $table['caption'] = "Contact ($contactId) Emails:";
    $table['headers'] = array('type', 'address', 'preferred');
    $values = array();
    foreach ($emails as $email) {
        $values[] = Util::convertNulls(array(
            $email->getEmailType(),
            $email->getEmailAddress(),
            $email->isPreferred(),
        ));
    }
    $table['values'] = $values;
    return $table;
}

function generateImsTable($contactId, $ims) {
    if ($ims === null || count($ims) == 0)
        return null;

    $table = array();
    $table['caption'] = "Contact ($contactId) Ims:";
    $table['headers'] = array('type', 'uri', 'preferred');
    $values = array();
    foreach ($ims as $im) {
        $values[] = Util::convertNulls(array(
            $im->getImType(),
            $im->getImUri(),
            $im->isPreferred(),
        ));
    }
    $table['values'] = $values;
    return $table;
}

function generateAddressesTable($contactId, $addresses) {
    if ($addresses === null || count($addresses) == 0)
        return null;

    $table = array();
    $table['caption'] = "Contact ($contactId) Addresses:";
    $table['headers'] = array(
        'type', 'preferred', 'poBox', 'addressLine1', 'addressLine2',
        'city', 'state', 'zipcode', 'country'
    );
    $values = array();
    foreach ($addresses as $address) {
        $values[] = Util::convertNulls(array(
            $address->getAddressType(),
            $address->isPreferred(),
            $address->getPoBox(),
            $address->getAddressLineOne(),
            $address->getAddressLineTwo(),
            $address->getCity(),
            $address->getState(),
            $address->getZipCode(),
            $address->getCountry(),
        ));
    }
    $table['values'] = $values;
    return $table;
}

function generateWeburlsTable($contactId, $weburls) {
    if ($weburls === null || count($weburls) == 0)
        return null;

    $table = array();
    $table['caption'] = "Contact ($contactId) Weburls:";
    $table['headers'] = array('type', 'url', 'preferred');
    $values = array();
    foreach ($weburls as $weburl) {
        $values[] = Util::convertNulls(array(
            $weburl->getWebUrlType(),
            $weburl->getUrl(),
            $weburl->isPreferred(),
        ));
    }
    $table['values'] = $values;
    return $table;
}

function generateQuickContactTable($qcontact) {
    $table = array();
    $table['caption'] = 'Quick Contact:';
    $table['headers'] = array(
        'contactId', 'formattedName', 'firstName', 'middleName', 'lastName',
        'prefix', 'suffix', 'nickName', 'organization',
    );
    $values = array();
    $values[] = array(
        $qcontact->getContactId(), $qcontact->getFormattedName(),
        $qcontact->getFirstName(), $qcontact->getMiddleName(),
        $qcontact->getLastName(), $qcontact->getPrefix(),
        $qcontact->getSuffix(), $qcontact->getNickname(),
        $qcontact->getOrganization(),
    );
    $table['values'] = $values;
    return $table;
}

function generateContactTable($contact) {
    $table = array();
    $table['caption'] = 'Contact:';
    $table['headers'] = array(
        'contactId', 'creationDate', 'modificationDate', 'formattedName',
        'firstName', 'lastName', 'prefix', 'suffix', 'nickName',
        'organization', 'jobTitle', 'anniversary', 'gender', 'spouse',
        'hobby', 'assistant',
    );
    $values = array();
    $values[] = array(
        $contact->getContactId(), $contact->getCreationDate(),
        $contact->getModificationDate(), $contact->getFormattedName(),
        $contact->getFirstName(), $contact->getLastName(),
        $contact->getPrefix(), $contact->getSuffix(),
        $contact->getNickname(), $contact->getOrganization(),
        $contact->getJobTitle(), $contact->getAnniversary(),
        $contact->getGender(), $contact->getSpouse(), $contact->getHobby(),
        $contact->getAssistant(),
    );
    $table['values'] = $values;
    return $table;
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
