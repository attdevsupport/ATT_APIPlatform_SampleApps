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
require_once __DIR__ . '/aabUtil.php';
require_once __DIR__ . '/../lib/AAB/AABService.php';

use Att\Api\AAB\AABService;

$arr = null;
try {
    envinit();
    $aabService = new AABService(getFqdn(), getSessionToken());
    $searchVal = $_POST['contactsSearchValue'];

    $resultSet = $aabService->getContacts(null, null, $searchVal);
    $tables = array();

    $contacts = $resultSet->getContacts();
    $qcontacts = $resultSet->getQuickContacts();
    foreach ($contacts as $contact) {
        $tables[] = generateContactTable($contact);
        $contactId = $contact->getContactId();
        $table = generatePhonesTable($contactId, $contact->getPhones());
        if ($table !== null) { $tables[] = $table; }
        $table = generateEmailsTable($contactId, $contact->getEmails());
        if ($table !== null) { $tables[] = $table; }
        $table = generateImsTable($contactId, $contact->getIms());
        if ($table !== null) { $tables[] = $table; }
        $table = generateAddressesTable($contactId, $contact->getAddresses());
        if ($table !== null) { $tables[] = $table; }
        $table = generateWeburlsTable($contactId, $contact->getWeburls());
        if ($table !== null) { $tables[] = $table; }
    }

    foreach ($qcontacts as $qcontact) {
        $tables[] = generateQuickContactTable($qcontact);
        if ($qcontact->getPhone() !== null) {
            $tables[] = generatePhonesTable(
                $qcontact->getContactId(), array($qcontact->getPhone())
            );
        }
        if ($qcontact->getEmail() !== null) {
            $tables[] = generateEmailsTable(
                $qcontact->getContactId(), array($qcontact->getEmail())
            );
        }
        if ($qcontact->getIm() !== null) {
            $tables[] = generateImsTable(
                $qcontact->getContactId(), array($qcontact->getIm())
            );
        }
        if ($qcontact->getAddress() !== null) {
            $tables[] = generateAddressesTable(
                $qcontact->getContactId(), array($qcontact->getAddress())
            );
        }
    }

    $arr = array(
        'success' => true,
        'tables' => $tables,
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
