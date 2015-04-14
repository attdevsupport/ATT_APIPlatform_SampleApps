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
require_once __DIR__ . '/../lib/IMMN/IMMNService.php';

use Att\Api\IMMN\IMMNService;

$arr = null;
try {
    envinit();
    $immnSrvc = new IMMNService(getFqdn(), getSessionToken());

    $msgState = $_POST['msgState'];
    $deltaResponse = $immnSrvc->getMessagesDelta($msgState);
    $tables = array();
    foreach($deltaResponse->getDeltas() as $delta) {
        $values = array();
        foreach ($delta->getAdds() as $add) {
            $values[] = array(
                'Add', $add->getMessageId(), $add->isFavorite(),
                $add->isUnread(),
            );
        }
        foreach ($delta->getDeletes() as $delete) {
            $values[] = array(
                'Delete', $delete->getMessageId(), $delete->isFavorite(),
                $delete->isUnread(),
            );
        }
        foreach ($delta->getUpdates() as $update) {
            $values[] = array(
                'Update', $update->getMessageId(), $update->isFavorite(),
                $update->isUnread(),
            );
        }
        $tables[] = array(
            'caption' => 'Delta Type: ' . $delta->getDeltaType(),
            'headers' => array(
                'Delta Operation', 'MessageId', 'Favorite', 'Unread',
            ),
            'values' => $values,
        );
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
